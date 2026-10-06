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

    def test_six_isolated_packages_and_unchanged_originals(self):
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
            self.assertEqual(len(report['packages']), 6)
            self.assertEqual(len(list(output.rglob('*.uasset'))), 6)
            self.assertEqual(len(list(output.rglob('*.uexp'))), 6)
            definitions = set()
            for package in report['packages']:
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

    def test_missing_sources_create_no_output(self):
        with tempfile.TemporaryDirectory() as folder:
            source, output = Path(folder) / 'empty', Path(folder) / 'stage'
            source.mkdir()
            self.assertNotEqual(self.run_tool(source, output).returncode, 0)
            self.assertFalse(output.exists())

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
