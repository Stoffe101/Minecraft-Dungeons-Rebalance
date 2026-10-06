# Private Camp smith asset stage

Build with .NET 8, then run:

```text
dotnet build tools/CampSmithStager -c Release
dotnet tools/CampSmithStager/bin/Release/net8.0/CampSmithStager.dll <private-PatchSources-root> <fresh-private-output-root>
```

The tool creates separate Rebalance copies of the three native Tower NPC actors and their three native content widgets. It relocates self and cross-package names into `/Game/Mods/MinecraftDungeonsRebalance/Camp`, preserves native merchant definitions, visuals, property values and graph behavior, writes and reopens every package, checks exact parsed semantic equality and verifies original source hashes.

All original inputs are checked before writing; failure removes the newly created stage. Output cannot overlap the input tree or overwrite an existing directory. Raw source/generated game assets must remain private and outside git.

**This is an asset foundation, not a playable Camp smith build.** The original one-transaction flags remain true. Native merchant types still dispatch to native Tower behavior. There is no Camp spawn binding, paid persistent transaction, custom Unique selector or adapter to the Python policy model. The report explicitly marks those features false. The gameplay PAK builder does not include these clones. Do not package them as a completed upgrade feature.

Integration tests require the six original private packages and their companions:

```text
python tests/test_camp_smiths.py --dotnet <dotnet> --stager <CampSmithStager.dll> --source <private-PatchSources-root> -v
```
