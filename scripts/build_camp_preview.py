"""Combine verified v2 files with the non-interactive Camp placement stage."""
import argparse
import hashlib
import json
from pathlib import Path
import shutil
import subprocess
import sys

BASELINE_HASH = '349181e2efaa8dfa6861133dab7286b03920dd469196de8b65cd5189f7c77b00'
CHEST = 'Dungeons/Content/Decor/Prefabs/RewardChest/BP_LobbyChest'
OWNED = 'Dungeons/Content/Mods/MinecraftDungeonsRebalance/Camp/'


def digest(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def main():
    parser = argparse.ArgumentParser()
    for name in ['baseline', 'smiths', 'output', 'packager']:
        parser.add_argument('--' + name, type=Path, required=True)
    args = parser.parse_args()
    baseline, smiths, output, packager = [getattr(args, n).resolve() for n in ['baseline', 'smiths', 'output', 'packager']]
    if output.exists():
        raise ValueError('Output exists; choose a fresh build directory')
    for source in [baseline, smiths]:
        if output.is_relative_to(source) or source.is_relative_to(output):
            raise ValueError('Source/output trees must be separate')
    baseline_report = json.loads((baseline / 'BUILD_REPORT.json').read_text())
    if baseline_report['pakSha256'] != BASELINE_HASH or len(baseline_report['entries']) != 19:
        raise ValueError('Expected the accepted v2 baseline')
    entries = {e['path']: e for e in baseline_report['entries']}
    baseline_stage = baseline / 'stage'
    files = {p.relative_to(baseline_stage).as_posix(): p for p in baseline_stage.rglob('*') if p.is_file()}
    if files.keys() != entries.keys() or any(digest(p) != entries[n]['sha256'] for n, p in files.items()):
        raise ValueError('Baseline content differs from verified report')
    smith_report = json.loads((smiths / 'CAMP_SMITH_STAGE_REPORT.json').read_text())
    if (smith_report['status'] != 'camp_placement_preview_only' or len(smith_report['packages']) != 10
            or not smith_report['interactionsDisabledBySpawn'] or smith_report['paidTransactionsImplemented']
            or smith_report['gameplayVerified']):
        raise ValueError('Expected an unverified, non-interactive placement stage')
    smith_files = {}
    for package in smith_report['packages']:
        if not package['semanticRoundTrip']:
            raise ValueError('Unverified cooked package')
        parent = Path(package['clonePackage']).parent
        for name, expected in package['outputHashes'].items():
            relative = (parent / name).as_posix()
            if not (relative.startswith(OWNED) or relative in [CHEST + '.uasset', CHEST + '.uexp']):
                raise ValueError('Unexpected package override')
            path = smiths / relative
            if digest(path) != expected or relative in smith_files:
                raise ValueError('Changed or duplicate placement package')
            smith_files[relative] = path
    actual_smith_files = {p.relative_to(smiths).as_posix() for p in smiths.rglob('Dungeons/**/*') if p.is_file()}
    if actual_smith_files != smith_files.keys() or len(smith_files) != 20:
        raise ValueError('Unexpected placement stage file set')
    replaced = set(files) & set(smith_files)
    if replaced != {CHEST + '.uasset', CHEST + '.uexp'}:
        raise ValueError('Only the Camp chest pair may replace v2 files')
    chest = next(p for p in smith_report['packages'] if p.get('placementHook'))
    if not chest['hostOnly'] or chest['replicated'] or not chest['interactionsDisabled']:
        raise ValueError('Placement preview guards changed')
    files.update(smith_files)
    if len(files) != 37:
        raise ValueError('Expected exactly 37 merged entries')
    output.mkdir(parents=True)
    stage = output / 'stage'
    for relative, path in sorted(files.items()):
        target = stage / relative
        target.parent.mkdir(parents=True, exist_ok=True)
        shutil.copyfile(path, target)
    pak = output / 'MinecraftDungeonsRebalance-CampPlacement-Test-v3.pak'
    subprocess.run([sys.executable, str(packager), 'pack', str(pak), 'Dungeons', '-p'], cwd=stage, check=True)
    subprocess.run([sys.executable, str(packager), 'test', str(pak)], check=True)
    unpack = output / 'verified-unpack'
    subprocess.run([sys.executable, str(packager), 'unpack', '-C', str(unpack), str(pak)], check=True)
    actual = {p.relative_to(unpack).as_posix(): p.read_bytes() for p in unpack.rglob('*') if p.is_file()}
    expected = {n: p.read_bytes() for n, p in files.items()}
    if actual != expected:
        raise ValueError('PAK did not preserve the exact file set and bytes')
    report = dict(build='CampPlacement-Test-v3', gameplayVerified=False, completeDesignImplemented=False,
                  npcInteractionsEnabled=False, npcReplicated=False, paidTransactionsImplemented=False,
                  pakSha256=digest(pak), pakBytes=pak.stat().st_size, baselinePakSha256=BASELINE_HASH,
                  entries=[dict(path=n, bytes=len(b), sha256=hashlib.sha256(b).hexdigest()) for n, b in sorted(expected.items())],
                  replacedBaselineEntries=sorted(replaced), retainedBaselineEntries=17,
                  placementStageReportSha256=digest(smiths / 'CAMP_SMITH_STAGE_REPORT.json'),
                  features=baseline_report['features'] + ['experimental host-only Camp smith placement; interactions disabled'],
                  excluded=['paid/repeatable/persistent smith upgrades', 'custom Unique picker', 'client NPC replication',
                            'shared gold', 'higher Ancient encounter selection chance', 'completion gold', 'global mob income rebalance'])
    (output / 'BUILD_REPORT.json').write_text(json.dumps(report, indent=2) + '\n')
    print('Built, integrity-tested and unpack-compared all 37 PAK entries:', pak)


if __name__ == '__main__':
    main()
