"""Build the implemented Hunt/economy test subset and verify every packaged byte."""
import argparse, hashlib, json, shutil, subprocess, sys
from pathlib import Path
from build_hunts import build

p=argparse.ArgumentParser()
for name in ['sources','upstream','balance','output','dotnet','patcher','packager']:
    p.add_argument('--'+name,type=Path,required=True)
a=p.parse_args()
for key in vars(a):setattr(a,key,getattr(a,key).resolve())
if a.output.exists():raise ValueError('Output exists; use a fresh build directory')
a.output.mkdir(parents=True)
cfg=json.loads(a.balance.read_text())
subprocess.run([str(a.dotnet),str(a.patcher),str(a.sources),str(a.output/'economy'),str(a.balance)],check=True)
build(a.sources,a.upstream,a.output/'hunts',cfg)
stage=a.output/'stage';stage.mkdir()
for feature in ['economy','hunts']:
    shutil.copytree(a.output/feature/'Dungeons',stage/'Dungeons',dirs_exist_ok=True)
files=sorted(x for x in stage.rglob('*') if x.is_file())
if len(files)!=19:raise ValueError('Unexpected package contents count')
pak=a.output/'MinecraftDungeonsRebalance-HuntsEconomy-Test-v2.pak'
subprocess.run([sys.executable,str(a.packager),'pack',str(pak),'Dungeons','-p'],cwd=stage,check=True)
subprocess.run([sys.executable,str(a.packager),'test',str(pak)],check=True)
unpack=a.output/'verified-unpack'
subprocess.run([sys.executable,str(a.packager),'unpack','-C',str(unpack),str(pak)],check=True)
expected={str(x.relative_to(stage)).replace('\\','/'):x.read_bytes() for x in files}
actual={str(x.relative_to(unpack)).replace('\\','/'):x.read_bytes() for x in unpack.rglob('*') if x.is_file()}
if expected!=actual:raise ValueError('PAK did not preserve exact file set and bytes')
report=dict(build='HuntsEconomy-Test-v2',gameplayVerified=False,completeDesignImplemented=False,
            balanceSha256=hashlib.sha256(a.balance.read_bytes()).hexdigest(),
            pakSha256=hashlib.sha256(pak.read_bytes()).hexdigest(),
            entries=[dict(path=n,sha256=hashlib.sha256(data).hexdigest(),bytes=len(data)) for n,data in expected.items()],
            features=['normal gold chests 10-15','rare gold chests 20-30','base Loot Urn emerald drops 6-14','Camp emerald chest reward 100','adapted bounded Hunt mobs/sidepaths/arenas','26 Ancient encounter extra waves'],
            excluded=['increased Ancient encounter selection chance','Camp smith NPC placement/paid transactions','Hunt completion gold change','global mob currency bundle multipliers','party-wide gold grants'])
(a.output/'BUILD_REPORT.json').write_text(json.dumps(report,indent=2)+'\n')
print('Built, integrity-tested and unpack-compared all 19 PAK entries:',pak)
