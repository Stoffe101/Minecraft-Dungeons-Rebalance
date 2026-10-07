"""Combine accepted v2 rewards/Hunt waves with explicit Camp test modes."""
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
    parser.add_argument("--upgrade-test", action="store_true", help="Explicitly permit the guarded native upgrade prototype")
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
    upgrade_test = smith_report['status'] == 'camp_native_upgrade_test'
    ui_preview = smith_report['status'] == 'camp_interaction_preview_only'
    interactive = ui_preview or upgrade_test
    if args.upgrade_test != upgrade_test:
        raise ValueError('Upgrade prototype requires matching explicit --upgrade-test')
    if (smith_report['status'] not in ['camp_placement_preview_only', 'camp_interaction_preview_only', 'camp_native_upgrade_test'] or len(smith_report['packages']) != 10
            or smith_report['interactionsDisabledBySpawn'] == interactive or smith_report['paidTransactionsImplemented'] != upgrade_test
            or smith_report['gameplayVerified'] or not smith_report.get('functionCreationPreloadsValidated')):
        raise ValueError('Expected an unverified, contract-validated Camp test stage')
    if ui_preview and (not smith_report.get('nativeUpgradeActionsBlockedByBindings')
                       or sum(p.get('disabledActionBindingCount', 0) for p in smith_report['packages']) != 11):
        raise ValueError('Read-only UI requires all eleven transaction bindings blocked')
    if upgrade_test and (not smith_report.get('affordabilityConnectedToActions') or smith_report.get('completePaidDesignImplemented')
                         or sum(p.get('controlledUpgradeButtonCount', 0) for p in smith_report['packages']) != 8):
        raise ValueError('Upgrade prototype requires eight controlled button templates and payment bindings')
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
    if not chest['hostOnly'] or chest['replicated'] or chest['interactionsDisabled'] == interactive:
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
    build_name = 'CampUpgrade-Test-v6' if upgrade_test else 'CampUI-LoadFix-Test-v6' if ui_preview else 'CampCentralNames-Test-v6'
    pak = output / ('MinecraftDungeonsRebalance-' + build_name + '.pak')
    subprocess.run([sys.executable, str(packager), 'pack', str(pak), 'Dungeons', '-p'], cwd=stage, check=True)
    subprocess.run([sys.executable, str(packager), 'test', str(pak)], check=True)
    unpack = output / 'verified-unpack'
    subprocess.run([sys.executable, str(packager), 'unpack', '-C', str(unpack), str(pak)], check=True)
    actual = {p.relative_to(unpack).as_posix(): p.read_bytes() for p in unpack.rglob('*') if p.is_file()}
    expected = {n: p.read_bytes() for n, p in files.items()}
    if actual != expected:
        raise ValueError('PAK did not preserve the exact file set and bytes')
    report = dict(build=build_name, gameplayVerified=False, completeDesignImplemented=False,
                  priorV4NpcLoadingUserConfirmed=True, functionCreationPreloadsValidated=True, npcInteractionsEnabled=interactive,
                  nativeUpgradeActionsBlockedByBindings=ui_preview, upgradeActionsEnabled=upgrade_test,
                  giftWrapperPlacementImplemented=True, namesImplemented=True, nativeTestAffordabilityImplemented=True,
                  testUpgradeAmount=1, testGildAmount=1, affordabilityConnectedToActions=upgrade_test, npcReplicated=False, paidTransactionsImplemented=upgrade_test, nativeInventoryPersistenceVerified=False,
                  pakSha256=digest(pak), pakBytes=pak.stat().st_size, baselinePakSha256=BASELINE_HASH,
                  entries=[dict(path=n, bytes=len(b), sha256=hashlib.sha256(b).hexdigest()) for n, b in sorted(expected.items())],
                  replacedBaselineEntries=sorted(replaced), retainedBaselineEntries=17,
                  placementStageReportSha256=digest(smiths / 'CAMP_SMITH_STAGE_REPORT.json'),
                  features=baseline_report['features'] + ['experimental Gift Wrapper-relative smith placement and TextRender names',
                                                         'guarded native upgrade transactions; temporary 1 emerald / 1 gold pricing' if upgrade_test else 'read-only native merchant UI; stock upgrade actions blocked' if ui_preview else 'NPC interactions disabled'],
                  excluded=['production price policy and verified inventory/save persistence', 'custom Unique picker', 'client NPC replication',
                            'shared gold', 'higher Ancient encounter selection chance', 'completion gold', 'global mob income rebalance'])
    (output / 'BUILD_REPORT.json').write_text(json.dumps(report, indent=2) + '\n')
    print('Built, integrity-tested and unpack-compared all 37 PAK entries:', pak)


if __name__ == '__main__':
    main()
