# Collect the missing Rebalance inputs

The previous inventory collector already supplied the inventory evidence. This collection targets different assets: smiths, currencies, containers and Camp/Hunt generation. It does not request another copy of that same inventory export.

## Easy route

1. Extract `Minecraft-Dungeons-Rebalance-Collector.zip` to a normal folder outside the game.
2. Double-click `Collect-RebalanceEvidence.cmd`.
3. Send the newly created `.research/rebalance-evidence-*.zip` back to the chat.
4. Also supply the author's original Better Ancient Hunt PAK from https://www.nexusmods.com/minecraftdungeons/mods/173?tab=files . It is not bundled with this collector.

The portable bundle has a compiled inspector. If .NET 8 is unavailable, it downloads a checksum-verified local runtime rather than installing anything system-wide. The source checkout builds the inspector with an installed or checksum-pinned local .NET 8 SDK instead.

The known Store path is auto-detected on mounted drives. For a different installation:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Collect-RebalanceEvidence.ps1 -PaksPath "C:\XboxGames\Minecraft Dungeons\Content\Dungeons\Content\Paks"
```

The default public Dungeons 1 archive key is the key already verified by this user's October 4 catalog collection. `-AesKey` can override it. Its source and prior validation are documented in QoL's GAME_EVIDENCE and in SOURCES here. This does not change game access permissions.

## Output and errors

Output contains `REPORT.json`, exporter logs, a catalog, a collection manifest with per-file SHA-256 values, tagged defaults/imports/Kismet metadata, and allowlisted originals under `PatchSources`. No saves, executables, textures, or unlisted raw packages are collected. Installed `~mods` directories are not recursively mounted; mods placed directly in Paks may still overlay originals and must be identified during review.

A missing asset or parser error produces a nonzero exit status and retained partial evidence. Send the ZIP even if issues are reported. Existing output directories are refused. The source and output archives are not published in git.

## Limits

Local integration tests exercised an unencrypted UE4.22 third-party mod fixture and synthetic JSON, not this retail game's encrypted archives or your Windows installation. Windows CI tests the source build and native-library initialization when it runs. Neither test certifies smith gameplay, multiplayer sharing or balance.

## Supplemental Ancient spawn evidence

The initial 72 targets and upstream mod were received successfully. For the new guarantee request, use **Collect-AncientSpawnEvidence.cmd** from the Ancient collector bundle. It collects 14 offering/chance/door/merchant packages plus the separate netherhypermission-hyper.json configuration: 15 catalog-confirmed new targets, zero overlap. It produces rebalance-ancient-evidence-*.zip. Do not resend the initial upload or mod. See ANCIENT_GUARANTEE.md for why these are needed. Source checkout can use -TargetSet AncientSpawn; Baseline stays the default.
