# Camp placement test build

October 6, 2026. This is **CampPlacement-Test-v3**, an experimental NPC placement build. It is not the finished paid-upgrade mod.

## What this build tests

Three native Tower smith actor copies are spawned through an added Blueprint function on Camp's existing BP_LobbyChest. The hook runs before the original begin-play graph, preserving that graph. It obtains the live chest position and uses authored offsets `(350, -220, 0)`, `(350, 0, 0)`, `(350, 220, 0)` in Unreal units, with yaw 180. These offsets have not been checked for Camp walkability. Engine collision handling is AdjustIfPossibleButAlwaysSpawn (enum 2), which can still spawn inside a collision if adjustment fails; it is not navigation projection.

Only authority runs the spawn function. Each cloned class is checked with GetAllActorsOfClass before spawning, so later chest begin-play events skip an existing actor. These checks are per class, not a persistent spawn registry. SetReplicates(false) is applied before FinishSpawningActor: this preview is intended for single-player/host visibility, not clients. The native MerchantActor interactable component is read after finishing the spawn and DisableInteraction is called. If the component is absent, the actor is destroyed. Native construction/begin-play still runs; real-game testing must establish that native lifecycle behavior does not undo the disabled state.

The three actors retain their original Tower definitions and transaction flags. Their separate owned root screens/content retain the native item decision flow and new presentation/selected-item readers, but no upgrade interaction is enabled. This graph contains no wallet deduction, inventory mutation, merchant transaction execution or save call. Currency/item persistence and multiplayer shared gold are not implemented.

If no BP_LobbyChest instance begins play, this hook will not spawn NPCs. Where Camp instantiates its chest and whether a claimed chest appears early enough remain runtime checks. Do not infer Camp placement success from the serializer tests.

## Install and test

1. Close the game. Remove the previous Rebalance HuntsEconomy-Test-v2 PAK from the mods folder; this build includes its changes. Avoid another mod overriding BP_LobbyChest while testing.
2. Put MinecraftDungeonsRebalance-CampPlacement-Test-v3.pak in `C:\XboxGames\Minecraft Dungeons\Content\Dungeons\Content\Paks\~mods` (or your installation's equivalent Paks/~mods directory). No new loader or collector is required.
3. Start a single-player session and enter Camp. Look near the Camp reward chest for the three Tower smith NPCs. Check their number, appearance and position; they should have no usable upgrade interaction.
4. Leave Camp and return; check for duplicates. Confirm Camp chests still award 100 emeralds. Check normal Camp loading and movement, then existing Hunt progression if desired.
5. Report whether all three appeared and whether positions/animations/interactions behaved as described. If Camp fails to load, remove this PAK and return to v2. No save editing is part of the test.

No Powersmith/Unique/gilding purchase should be attempted in this preview. Paid-service tests (balance delta, insufficient funds, chosen outcome, item preservation, save/reload and multiplayer authority) remain pending implementation.

## Verification and provenance

Nine real-source integration tests passed. All ten package pairs reopened with exact parsed equality; source hashes were unchanged. Eight negative checks reject lost native local construction, non-host branch execution, replicated actors, altered collision mode, missing interaction disabling, missing hook, invalid branch destinations and regression to the 50-emerald reward. Earlier presentation/selection/screen guards remain included.

The merged PAK has 37 entries: 17 unchanged v2 files plus 20 placement files. Exactly the BP_LobbyChest pair replaces v2 entries; its reward remains 100. PAK integrity and unpacked file set/bytes match exactly. Private inputs/raw cooked assets are not committed. Hash-only reports are in research/camp-placement-stage-v3.json and research/camp-placement-build-v3.json.

Build reproduction: run CampSmithStager with `--placement-preview <private-PatchSources-root> <fresh-stage>`, then scripts/build_camp_preview.py with `--baseline <verified-v2-build> --smiths <fresh-stage> --output <fresh-build> --packager <pinned-u4pak.py>`. The script requires the accepted v2 report and exact input hashes, permits only the two chest overlaps, and excludes unrelated files.

Local compilation used the pinned .NET 8 Roslyn route; its known Newtonsoft net6/net8 reference warning remains. Windows CI performs normal project compilation and permitted-fixture checks; it does not receive private retail sources or launch Minecraft Dungeons.

## Remaining work

The next execution dependency is a supported native custom-price/payment and normal-inventory persistence path. Captured GetPrice is not a Blueprint override event; returned MerchantPricing values are not a demonstrated transaction setter. Wallet Deduct returns void and does not establish rollback or atomic mutation. MerchantBase refers to a MerchantPricingComponent whose implementation is not supplied. Do not label an uncharged Tower transaction a paid Camp upgrade. Use current evidence/public native-compatible implementation research; no further declaration collection is requested. A confirmed NPC placement test will settle the Camp lifecycle/geometry questions separately.

Agreed prices remain Common-to-Rare 750 emeralds, selected Unique 2500 emeralds, gild 150 gold and gild reroll 250 gold. Powersmith pricing/rules are unresolved. Shared gold, completion rewards and higher native Ancient selection probability remain unfinished.

PAK SHA-256: `6f13bc4f04f2f5d4a4782803158e41943c29d647b98968cae2066a9495fd409a`; bytes: 6075200.
