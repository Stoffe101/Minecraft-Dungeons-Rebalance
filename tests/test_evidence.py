"""Integration checks using a permitted external UE4.22 fixture; never retail game inputs."""
import argparse, hashlib, json, shutil, subprocess, tempfile, unittest
from pathlib import Path

parser = argparse.ArgumentParser()
parser.add_argument('--dotnet', required=True)
parser.add_argument('--exporter', required=True)
parser.add_argument('--libraries', required=True)
parser.add_argument('--fixture', required=True)
parser.add_argument('--packager', required=True)
args, remaining = parser.parse_known_args()
for name in ('dotnet','exporter','libraries','fixture','packager'):
    setattr(args, name, str(Path(getattr(args,name)).resolve()))

class EvidenceTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.package = 'Dungeons/Content/Mods/Fixture/BP_Fixture.uasset'
        self.data = 'Dungeons/Content/data/lovika/levels/fixture.json'
        self.targets = self.root/'targets.json'
        self.write_targets([self.package], [self.data])

    def tearDown(self): self.temp.cleanup()

    def write_targets(self, packages, data):
        self.targets.write_text(json.dumps(dict(schemaVersion=1,packages=packages,dataFiles=data)))

    def run_tool(self, *parameters):
        return subprocess.run([args.dotnet,args.exporter,*map(str,parameters)],capture_output=True,text=True)

    def make_pak(self, include_data=True):
        stage=self.root/'stage'
        asset=stage/self.package
        asset.parent.mkdir(parents=True)
        shutil.copy(args.fixture,asset)
        companion=Path(args.fixture).with_suffix('.uexp')
        if companion.exists(): shutil.copy(companion,asset.with_suffix('.uexp'))
        if include_data:
            data=stage/self.data
            data.parent.mkdir(parents=True)
            data.write_text('{"fixture": true, "count": 17}')
        paks=self.root/'game/Dungeons/Content/Paks'
        paks.mkdir(parents=True)
        subprocess.run(['python3',args.packager,'pack',str(paks/'fixture.pak'),'Dungeons','-p'],cwd=stage,check=True,capture_output=True)
        return stage,paks

    def test_manifest_rejects_traversal_and_ambiguity(self):
        for bad in ['../fixture.uasset','Dungeons/Content/../fixture.uasset','Dungeons/Content//fixture.uasset','Dungeons/Content/fixture.exe','Dungeons/Content/C:/fixture.uasset','Dungeons/Content\\fixture.uasset']:
            with self.subTest(path=bad):
                self.write_targets([bad],[self.data])
                self.assertNotEqual(self.run_tool('--validate-targets',self.targets).returncode,0)
        self.write_targets([self.package,self.package.lower()],[self.data])
        self.assertNotEqual(self.run_tool('--validate-targets',self.targets).returncode,0)

    def test_valid_manifest(self):
        self.assertEqual(self.run_tool('--validate-targets',self.targets).returncode,0)

    def test_collection_preserves_exact_inputs_and_defaults(self):
        stage,paks=self.make_pak()
        before=hashlib.sha256((paks/'fixture.pak').read_bytes()).digest()
        out=self.root/'output'
        result=self.run_tool('--paks',paks,'0'*64,out,args.libraries,self.targets)
        self.assertEqual(result.returncode,0,result.stdout+result.stderr)
        report=json.loads((out/'EXPORT_REPORT.json').read_text())
        self.assertEqual(len(report['completed']),2)
        self.assertEqual(report['errors'],[])
        for entry in report['sourceFiles']:
            self.assertEqual(hashlib.sha256((out/entry['path']).read_bytes()).hexdigest(),entry['sha256'])
            self.assertEqual((out/entry['path']).read_bytes(),(stage/entry['path'].removeprefix('PatchSources/')).read_bytes())
        metadata=json.loads(next((out/'Metadata').glob('*.json')).read_text())
        self.assertGreater(metadata['propertyCount'],0)
        self.assertTrue(any('data' in x for x in metadata['exports']))
        self.assertEqual(before,hashlib.sha256((paks/'fixture.pak').read_bytes()).digest())
        rerun=self.run_tool('--paks',paks,'0'*64,out,args.libraries,self.targets)
        self.assertNotEqual(rerun.returncode,0)
        self.assertEqual(before,hashlib.sha256((paks/'fixture.pak').read_bytes()).digest())

    def test_missing_target_returns_failure_and_retains_partial_evidence(self):
        _,paks=self.make_pak(False)
        out=self.root/'output'
        result=self.run_tool('--paks',paks,'0'*64,out,args.libraries,self.targets)
        self.assertNotEqual(result.returncode,0)
        report=json.loads((out/'EXPORT_REPORT.json').read_text())
        self.assertEqual(report['completed'],[self.package])
        self.assertTrue(any(self.data in e for e in report['errors']))
        self.assertTrue((out/'PatchSources'/self.package).exists())

    def test_game_directory_output_is_rejected(self):
        _,paks=self.make_pak()
        result=self.run_tool('--paks',paks,'0'*64,paks.parent/'evidence',args.libraries,self.targets)
        self.assertNotEqual(result.returncode,0)
        self.assertFalse((paks.parent/'evidence').exists())

unittest.main(argv=['test_evidence.py',*remaining])
