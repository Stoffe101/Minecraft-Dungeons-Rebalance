"""Exercise actual reward packages, including fail-before-write on changed Camp defaults."""
import argparse,json,shutil,subprocess,tempfile,unittest
from pathlib import Path
p=argparse.ArgumentParser()
for name in ['sources','balance','dotnet','patcher']:p.add_argument('--'+name,type=Path,required=True)
a,tail=p.parse_known_args()
paths=['Dungeons/Content/Content_DLC4/Decor/Prefab/Functional/GoldChests/BP_GoldChest_Small.uasset','Dungeons/Content/Content_DLC4/Decor/Prefab/Functional/GoldChests/BP_GoldChest_Rare.uasset','Dungeons/Content/Decor/Prefabs/_Urns/LootUrnsBlueprints/BP_LootUrnBase.uasset','Dungeons/Content/Decor/Prefabs/RewardChest/BP_LobbyChest.uasset']
class EconomyTests(unittest.TestCase):
    def setUp(self):
        self.tmp=tempfile.TemporaryDirectory();self.addCleanup(self.tmp.cleanup);self.root=Path(self.tmp.name)
    def run_patch(self,sources,out,balance=None):
        return subprocess.run([str(a.dotnet.resolve()),str(a.patcher.resolve()),str(sources),str(out),str(balance or a.balance.resolve())],capture_output=True,text=True)
    def test_real_packages_and_changed_camp_default_rejected_before_write(self):
        out=self.root/'patched';r=self.run_patch(a.sources.resolve(),out);self.assertEqual(r.returncode,0,r.stdout+r.stderr)
        records=json.loads((out/'ECONOMY_PATCH_REPORT.json').read_text());self.assertEqual(len(records),4);self.assertTrue(all(x['roundTrip'] for x in records));self.assertEqual(records[-1]['after'],100)
        source=self.root/'mixed'
        for rel in paths:
            for suffix in ['.uasset','.uexp']:
                path=Path(rel).with_suffix(suffix);src=(out if rel==paths[-1] else a.sources)/path
                dest=source/path;dest.parent.mkdir(parents=True,exist_ok=True);shutil.copy2(src,dest)
        badout=self.root/'should-not-exist';r=self.run_patch(source,badout)
        self.assertNotEqual(r.returncode,0);self.assertIn('Unexpected/invalid Camp emerald reward',r.stderr);self.assertFalse(badout.exists())
    def test_invalid_camp_reward_rejected_before_write(self):
        cfg=json.loads(a.balance.read_text());cfg['emeralds']['campChest']=25;balance=self.root/'balance.json';balance.write_text(json.dumps(cfg));out=self.root/'should-not-exist'
        r=self.run_patch(a.sources.resolve(),out,balance);self.assertNotEqual(r.returncode,0);self.assertIn('Unexpected/invalid Camp emerald reward',r.stderr);self.assertFalse(out.exists())
    def test_existing_output_refused(self):
        out=self.root/'existing';out.mkdir();marker=out/'marker';marker.write_text('preserve');r=self.run_patch(a.sources.resolve(),out)
        self.assertNotEqual(r.returncode,0);self.assertEqual(marker.read_text(),'preserve')
unittest.main(argv=['test_economy.py',*tail])
