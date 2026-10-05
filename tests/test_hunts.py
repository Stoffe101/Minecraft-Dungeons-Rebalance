"""Validate real private level inputs and mutation rejection without shipping game fixtures."""
import argparse, copy, json, sys, unittest
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/'scripts'))
from build_hunts import LEVELS, adapt, validate
from lovika_json import load
p=argparse.ArgumentParser();p.add_argument('--sources',type=Path,required=True);p.add_argument('--upstream',type=Path,required=True);p.add_argument('--balance',type=Path,required=True);args,tail=p.parse_known_args()
cfg=json.loads(args.balance.read_text())
class HuntsTests(unittest.TestCase):
    def setUp(self):
        self.rel=Path('Dungeons/Content/data/lovika/levels/ancientdungeons.json')
        self.native=load(args.sources/self.rel);self.upstream=load(args.upstream/self.rel)
    def test_all_real_levels_preserve_native_routes_and_bounded_adaptation(self):
        for name in LEVELS:
            with self.subTest(level=name):
                rel=Path('Dungeons/Content/data/lovika/levels')/(name+'.json')
                native,upstream=load(args.sources/rel),load(args.upstream/rel)
                before=copy.deepcopy(native);before_upstream=copy.deepcopy(upstream)
                result=adapt(native,upstream,cfg['ancients'])
                self.assertEqual(native,before);self.assertEqual(upstream,before_upstream)
                def bounds(v):
                    if isinstance(v,list):
                        for x in v:bounds(x)
                    if isinstance(v,dict):
                        if 'density' in v:self.assertLessEqual(v['density'],cfg['ancients']['maxDensity'])
                        if 'side-paths' in v:
                            side=v['side-paths'];self.assertLessEqual(side['probability'],1)
                            for choice in [side.get('default',{}),*side.get('variants',[])]:
                                for x in choice.get('max-length',[]):self.assertLessEqual(x,cfg['ancients']['maxSidePathLength'])
                        for x in v.values():bounds(x)
                bounds(result)
    def test_no_unverified_array_rewards_adopted(self):
        result=adapt(self.native,self.upstream,cfg['ancients'])
        for native,updated in zip(self.native['challenges'],result['challenges']):
            self.assertEqual((native.get('arena') or {}).get('reward'),(updated.get('arena') or {}).get('reward'))
    def test_changed_level_identity_fails(self):
        self.upstream['id']='wrong'
        with self.assertRaises(ValueError):adapt(self.native,self.upstream,cfg['ancients'])
    def test_unknown_added_wave_group_fails(self):
        self.upstream['challenges'][0]['arena']['waves'][0][1]='not-a-native-or-mod-group'
        with self.assertRaises(ValueError):adapt(self.native,self.upstream,cfg['ancients'])
    def test_excess_wave_count_fails(self):
        self.upstream['challenges'][0]['arena']['waves']*=20
        with self.assertRaises(ValueError):adapt(self.native,self.upstream,cfg['ancients'])
    def test_mutated_gate_or_native_route_is_detected(self):
        result=adapt(self.native,self.upstream,cfg['ancients'])
        result['dungeons'][0]['stretches'][0]['tiles']=['wrong-tile']
        with self.assertRaises(ValueError):validate(result,self.native,cfg['ancients'])
    def test_mutated_native_arena_gate_is_detected(self):
        result=adapt(self.native,self.upstream,cfg['ancients'])
        result['challenges'][0]['arena']['gate']=None
        with self.assertRaises(ValueError):validate(result,self.native,cfg['ancients'])
    def test_comment_marker_inside_string_is_preserved(self):
        import tempfile
        with tempfile.TemporaryDirectory() as d:
            p=Path(d)/'fixture.json';p.write_text('//comment\n{"url":"https://example.test", "base64":"AA//BB", "list":[1,2,],}')
            self.assertEqual(load(p),{'url':'https://example.test','base64':'AA//BB','list':[1,2]})
unittest.main(argv=['test_hunts.py',*tail])
