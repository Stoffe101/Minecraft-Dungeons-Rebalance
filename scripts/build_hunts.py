"""Adapt permitted Better Ancient Hunt inputs onto this user's verified retail levels."""
import argparse, copy, hashlib, json
from pathlib import Path
from lovika_json import load

LEVELS = ['ancientdungeons', 'hm_basaltdeltas', 'hm_creeperwoods', 'hm_crimsonforest', 'hm_deserttemple', 'hm_netherfortress', 'hm_netherwastes', 'hm_soulsandvalley', 'hm_spidercave', 'hm_warpedforest', 'hm_woodlandmansion']

def adapt(original, upstream, caps):
    if original['id'] != upstream['id']:
        raise ValueError('Level identity mismatch')
    result = copy.deepcopy(original)
    # These are actual keys observed in both installed levels and the permitted mod.
    result['mob-groups'] = copy.deepcopy(upstream['mob-groups'])
    incoming_ids = {g['id'] for g in result['mob-groups']}
    result['mob-groups'].extend(copy.deepcopy(g) for g in original['mob-groups'] if g['id'] not in incoming_ids)
    for group in result['mob-groups']:
        for mob in group['types']:
            for key in ['min', 'max']:
                if key in mob: mob[key] = min(mob[key], caps['maxMobTypeCount'])
            if 'min' in mob and 'max' in mob and mob['min'] > mob['max']:
                raise ValueError('Inverted mob count')
    if 'default-mobs' in upstream:
        result['default-mobs'] = copy.deepcopy(upstream['default-mobs'])
        if 'density' in result['default-mobs']:
            result['default-mobs']['density'] = min(result['default-mobs']['density'], caps['maxDensity'])
    native_challenges = {c['id']: c for c in result['challenges']}
    if set(native_challenges) != {c['id'] for c in upstream['challenges']}:
        raise ValueError('Challenge identity mismatch')
    for incoming in upstream['challenges']:
        native = native_challenges[incoming['id']]
        if not isinstance(incoming.get('arena'), dict): continue
        if not isinstance(native.get('arena'), dict): raise ValueError('Arena kind changed')
        arena, incoming_arena = native['arena'], incoming['arena']
        if 'waves' in incoming_arena:
            waves = copy.deepcopy(incoming_arena['waves'])
            if len(waves) > caps['maxArenaWaves']: raise ValueError('Too many arena waves')
            for wave in waves:
                wave[0] = min(wave[0], caps['maxArenaWaveCount'])
                if wave[0] <= 0: raise ValueError('Non-positive wave count')
            arena['waves'] = waves
        for key in ['interval', 'rest-interval']:
            if key in incoming_arena: arena[key] = incoming_arena[key]
        # Preserve installed triggers/gates/rewards, not the mod's unverified arena reward arrays.
    native_dungeons = {d['id']:d for d in result['dungeons']}
    if set(native_dungeons) != {d['id'] for d in upstream['dungeons']}:
        raise ValueError('Dungeon identity mismatch')
    for dungeon in upstream['dungeons']:
        native = native_dungeons[dungeon['id']]
        if len(native['stretches']) != len(dungeon['stretches']): raise ValueError('Stretch count changed')
        for target, incoming in zip(native['stretches'], dungeon['stretches']):
            if 'mobs' in incoming:
                target['mobs'] = copy.deepcopy(incoming['mobs'])
                if 'density' in target['mobs']: target['mobs']['density'] = min(target['mobs']['density'], caps['maxDensity'])
            if 'side-paths' in incoming:
                target['side-paths'] = copy.deepcopy(incoming['side-paths'])
                side = target['side-paths']
                side['probability'] = min(1.0,side['probability'])
                for option in [side.get('default',{}), *side.get('variants',[])]:
                    if 'max-length' in option:
                        option['max-length'] = [min(x,caps['maxSidePathLength']) for x in option['max-length']]
                # Main-route length/doors/tiles/objective flow stay from the installed game.
    validate(result, original, caps)
    return result

def validate(result, original, caps):
    for key in ['id','objectives','object-groups','props','tiles','tile-groups','prop-groups','resource-packs']:
        if result.get(key) != original.get(key): raise ValueError('Protected native field changed: '+key)
    native_challenges = {c['id']: c for c in original['challenges']}
    for c in result['challenges']:
        before = native_challenges[c['id']]
        for key in set(c) | set(before):
            if key != 'arena' and c.get(key) != before.get(key): raise ValueError('Native challenge field changed: '+key)
        for key in set(c.get('arena') or {}) | set(before.get('arena') or {}):
            if key not in ['waves','interval','rest-interval'] and (c.get('arena') or {}).get(key) != (before.get('arena') or {}).get(key): raise ValueError('Native arena field changed: '+key)
    groups = {g['id'] for g in result['mob-groups']}
    # Installed challenges also use globally resolved groups not declared in this level.
    groups.update(w[1] for c in original['challenges'] for w in (c.get('arena') or {}).get('waves', []))
    for challenge in result['challenges']:
        for wave in (challenge.get('arena') or {}).get('waves',[]):
            if wave[1] not in groups: raise ValueError('Unknown wave mob group: '+wave[1])
    for dungeon, before in zip(result['dungeons'], original['dungeons']):
        if dungeon['id'] != before['id']: raise ValueError('Dungeon order changed')
        for stretch,old in zip(dungeon['stretches'],before['stretches']):
            for key in set(old)|set(stretch):
                if key not in ['side-paths','mobs'] and stretch.get(key)!=old.get(key):raise ValueError('Native route changed: '+key)
    if result['id']=='ancientdungeons':
        count=0
        for c in result['challenges']:
            if c['id'].startswith('goldarena'):continue
            waves=c['arena']['waves']
            if len(waves)!=3 or [w[0] for w in waves]!=[1,2,1] or waves[-1][1]!='raid':
                raise ValueError('Unexpected Ancient wave layout')
            count+=1
        if count!=26:raise ValueError('Ancient coverage mismatch')

def build(sources, upstream, output, config):
    if output.exists():raise ValueError('Output already exists')
    prepared=[]
    for name in LEVELS:
        rel=Path('Dungeons/Content/data/lovika/levels')/(name+'.json')
        src=sources/rel;mod=upstream/rel
        original,incoming=load(src),load(mod)
        result=adapt(original,incoming,config['ancients'])
        prepared.append((rel,src,mod,result))
    output.mkdir(parents=True)
    records=[]
    for rel,src,mod,result in prepared:
        dest=output/rel;dest.parent.mkdir(parents=True,exist_ok=True)
        dest.write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
        if load(dest)!=result:raise ValueError('JSON re-read mismatch')
        records.append(dict(path=rel.as_posix(),retailSha256=hashlib.sha256(src.read_bytes()).hexdigest(),upstreamSha256=hashlib.sha256(mod.read_bytes()).hexdigest(),outputSha256=hashlib.sha256(dest.read_bytes()).hexdigest()))
    (output/'HUNT_PATCH_REPORT.json').write_text(json.dumps(dict(author='Onetoeisenough',source='Better Ancient Hunt 1.0',caps=config['ancients'],files=records),indent=2)+'\n')
    print('Adapted and validated 11 installed Hunt levels; 26 Ancient encounters have bounded extra waves.')

if __name__=='__main__':
    p=argparse.ArgumentParser();p.add_argument('--sources',type=Path,required=True);p.add_argument('--upstream',type=Path,required=True);p.add_argument('--output',type=Path,required=True);p.add_argument('--balance',type=Path,required=True);a=p.parse_args()
    build(a.sources,a.upstream,a.output,json.loads(a.balance.read_text()))
