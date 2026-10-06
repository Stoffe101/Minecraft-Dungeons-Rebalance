"""Integration checks against private original assets, never retail gameplay tests."""
import argparse
import hashlib
import json
from pathlib import Path
import subprocess
import tempfile
import unittest


class CampSmithTests(unittest.TestCase):
    def run_tool(self, source, output):
        return subprocess.run([ARGS.dotnet, ARGS.stager, str(source), str(output)],
                              capture_output=True, text=True, timeout=120)

    def test_nine_connected_packages_and_unchanged_originals(self):
        with tempfile.TemporaryDirectory() as folder:
            output = Path(folder) / 'stage'
            result = self.run_tool(ARGS.source, output)
            self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
            report = json.loads((output / 'CAMP_SMITH_STAGE_REPORT.json').read_text())
            self.assertFalse(report['deployable'])
            self.assertFalse(report['campPlacementImplemented'])
            self.assertFalse(report['paidTransactionsImplemented'])
            self.assertFalse(report['uniquePickerImplemented'])
            self.assertTrue(report['nativeTowerFlagsPreserved'])
            self.assertTrue(report['uniquePresentationBindingsImplemented'])
            self.assertTrue(report['selectedItemReadBindingsImplemented'])
            self.assertTrue(report['actorScreenBindingsImplemented'])
            self.assertTrue(report['campContentDispatchImplemented'])
            self.assertTrue(report['nativeDecisionFlowRetained'])
            self.assertEqual(len(report['packages']), 9)
            self.assertEqual(len(list(output.rglob('*.uasset'))), 9)
            self.assertEqual(len(list(output.rglob('*.uexp'))), 9)
            screens = [p for p in report['packages'] if p.get('nativeDecisionGraphsPreserved')]
            self.assertEqual(len(screens), 3)
            self.assertEqual({p['contentDispatch'].split('.')[-1] for p in screens},
                             {'UMG_RebalanceCampUniquesmithContent_C', 'UMG_RebalanceCampPowersmithContent_C', 'UMG_RebalanceCampGildsmithContent_C'})
            definitions = set()
            added_functions = 0
            selection_functions = 0
            for package in report['packages']:
                added_functions += package['addedPresentationFunctions']
                selection_functions += package['addedSelectionReadFunctions']
                self.assertEqual(package['functionCount'], package['originalFunctionCount'] + package['addedPresentationFunctions'] + package['addedSelectionReadFunctions'])
                self.assertTrue(package['semanticRoundTrip'])
                self.assertGreater(package['relocatedNames'], 0)
                self.assertIn('/Mods/MinecraftDungeonsRebalance/Camp/', package['clonePackage'])
                source = Path(ARGS.source) / package['sourcePackage']
                for filename, expected in package['originalHashes'].items():
                    self.assertEqual(hashlib.sha256((source.parent / filename).read_bytes()).hexdigest(), expected)
                clone = output / package['clonePackage']
                for filename, expected in package['outputHashes'].items():
                    self.assertEqual(hashlib.sha256((clone.parent / filename).read_bytes()).hexdigest(), expected)
                if package['nativeDefinition']:
                    definitions.add(package['nativeDefinition'])
            self.assertEqual(definitions, {'TowerArtisanMerchantDef', 'TowerBlacksmithMerchantDef', 'TowerGilderMerchantDef'})
            self.assertEqual(added_functions, 3)
            self.assertEqual(selection_functions, 6)

    def test_presentation_rejects_invalid_native_graphs(self):
        source = Path(ARGS.source) / 'Dungeons/Content/Content_Season1/UI/Merchant/UMG_TowerMerchantArtisanContent.uasset'
        result = subprocess.run([ARGS.dotnet, ARGS.stager, '--self-test-presentation', str(source)],
                                capture_output=True, text=True, timeout=120)
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
        self.assertIn('Seven presentation graph rejection checks passed', result.stdout)

    def test_selected_item_readers_reject_invalid_native_graphs(self):
        source = Path(ARGS.source) / 'Dungeons/Content/Content_Season1/UI/Merchant/UMG_TowerMerchantArtisanContent.uasset'
        result = subprocess.run([ARGS.dotnet, ARGS.stager, '--self-test-selection', str(source)],
                                capture_output=True, text=True, timeout=120)
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
        self.assertIn('Four selected-item graph rejection checks passed', result.stdout)
        self.assertIn('Four native record contract rejection checks passed', result.stdout)

    def test_host_only_noninteractive_placement_preview(self):
        with tempfile.TemporaryDirectory() as folder:
            output = Path(folder) / 'preview'
            result = subprocess.run([ARGS.dotnet, ARGS.stager, '--placement-preview', str(ARGS.source), str(output)],
                                    capture_output=True, text=True, timeout=120)
            self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
            report = json.loads((output / 'CAMP_SMITH_STAGE_REPORT.json').read_text())
            self.assertTrue(report['deployable'])
            self.assertTrue(report['npcPreviewOnly'])
            self.assertTrue(report['interactionsDisabledBySpawn'])
            self.assertFalse(report['paidTransactionsImplemented'])
            self.assertFalse(report['gameplayVerified'])
            self.assertEqual(len(report['packages']), 10)
            self.assertEqual(len(list(output.rglob('*.uasset'))), 10)
            chest = next(p for p in report['packages'] if p.get('placementHook'))
            self.assertTrue(chest['hostOnly'])
            self.assertFalse(chest['replicated'])
            for package in report['packages']:
                self.assertTrue(package['semanticRoundTrip'])
                for key, root, path_key in [('originalHashes', Path(ARGS.source), 'sourcePackage'),
                                           ('outputHashes', output, 'clonePackage')]:
                    for name, expected in package[key].items():
                        self.assertEqual(hashlib.sha256((root / package[path_key]).with_name(name).read_bytes()).hexdigest(), expected)

    def test_placement_rejects_unsafe_guards(self):
        result = subprocess.run([ARGS.dotnet, ARGS.stager, '--self-test-placement', str(ARGS.source)],
                                capture_output=True, text=True, timeout=120)
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
        self.assertIn('Eight Camp placement rejection checks passed', result.stdout)

    def test_missing_sources_create_no_output(self):
        with tempfile.TemporaryDirectory() as folder:
            source, output = Path(folder) / 'empty', Path(folder) / 'stage'
            source.mkdir()
            self.assertNotEqual(self.run_tool(source, output).returncode, 0)
            self.assertFalse(output.exists())

    def test_screen_dispatch_and_decision_graph_guards(self):
        result = subprocess.run([ARGS.dotnet, ARGS.stager, '--self-test-screens', str(ARGS.source)],
                                capture_output=True, text=True, timeout=120)
        self.assertEqual(result.returncode, 0, result.stdout + result.stderr)
        self.assertIn('Three merchant screen rejection checks passed', result.stdout)

    def test_existing_output_is_preserved(self):
        with tempfile.TemporaryDirectory() as folder:
            marker = Path(folder) / 'keep.txt'
            marker.write_text('existing output')
            self.assertNotEqual(self.run_tool(ARGS.source, folder).returncode, 0)
            self.assertEqual(marker.read_text(), 'existing output')
            self.assertEqual(list(Path(folder).iterdir()), [marker])

    def test_nested_output_is_rejected_before_writing(self):
        with tempfile.TemporaryDirectory() as folder:
            output = Path(folder) / 'nested'
            self.assertNotEqual(self.run_tool(folder, output).returncode, 0)
            self.assertFalse(output.exists())


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--dotnet', required=True)
    parser.add_argument('--stager', required=True)
    parser.add_argument('--source', required=True)
    ARGS, extra = parser.parse_known_args()
    unittest.main(argv=[__file__, *extra])
