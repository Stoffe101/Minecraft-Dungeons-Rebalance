# Camp placement test build

**Current (October 7): v5 crashed with `Unsupported UBoolProperty ReturnValue size 0` and is withdrawn. v6 fixes that serialized Boolean and implements interactable native smith buttons, guarded native upgrade transactions and temporary 1 emerald / 1 gold fees. It retains Gift Wrapper-relative placement, names and the accepted v2 reward/Hunt subset. Fifteen private integration tests and all 37 PAK entries pass structural checks. Gameplay, native Camp eligibility and save/reload are unverified; full prices, the custom Unique picker and shared gold remain unfinished. v4 NPC loading/spawning was user-confirmed. See [CAMP_UPGRADE_TEST_V6.md](CAMP_UPGRADE_TEST_V6.md) for implementation, tests, limitations and next work.**

For v5, remove v4/v3 from the existing ~mods directory and install only MinecraftDungeonsRebalance-CampUI-Test-v5.pak. Enter single-player Camp with Gift Wrapper unlocked. Check positions, names, dialogue opening and item selection, then cancel/reopen/reload. The native upgrade action should be unavailable; this build does not charge 1 emerald or 1 gold. No command/collector is needed to test the PAK.

## Historical v3/v4 placement and crash work

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

## v3 crash and v4 correction

The user reported v3 crashing and supplied CrashContext(7).runtime-xml, CrashReportClient(7).ini and UE4Minidump(7).dmp. Sanitized summary: research/camp-v3-crash-summary.json. CrashReportClient INI was not used to infer cause; it is reporting configuration. Raw attachments/module paths/registers are not committed.

The exception is 0xc0000005, a write to 0x28 at Dungeons RVA0x11d838c after 36 seconds. The dump contains instruction bytes for loading the last entry of a seven-element child array then writing its Next pointer. The null register and offsets match UE4.22.3 UStruct::Serialize's child-field chain construction. The modified BP_LobbyChest class has seven children (original six plus new function), consistent with this failure. No symbols or referenced heap capture identify the exact loaded object, so this is a strong loading-path inference rather than a symbolized asset diagnosis.

Independent source/package review found generated functions had TemplateIndex zero and omitted native Function class/archetype prerequisites from SerializationBeforeCreateDependencies. Supplied original Function exports retain Default__Function and those prerequisites. v4 retains that original archetype and explicitly schedules native class/archetype serialization before new function creation, along with owner creation. This applies to the spawn function and nine presentation/selection wrapper functions. No operation or price was changed.

FunctionLoadContract validates qualified native Function/Default__Function imports and three required creation prerequisites. The actual retained crashed v3 chest package is now rejected; four corruption checks remove archetype, class prerequisite, archetype prerequisite or owner prerequisite. Eleven private-source tests passed, all ten package pairs reopened exactly and original hashes stayed unchanged. Rebuilt all37 entries and compared exact unpack paths/bytes. A correct parsed round trip alone did not catch the original loader contract omission; new checks target that gap. These checks cannot certify the absence of additional runtime failures.

To test v4: remove v3, place v4 in the same ~mods folder, enter Camp in single-player, and first check whether loading succeeds. Then check the three non-interactive NPCs/duplicates and the unchanged 100-emerald reward. If loading still fails, remove v4 and use v2. Upgrade/payment testing remains unavailable.

v4 PAK SHA-256: `1fc846ee8c0633cd10322a14ac4dedf23f67971ddfc2147a60cd6a1edbd3b527`; bytes: 6075360. Runtime crash correction verified: false.


## October 7: confirmed v4 NPC load, central placement, names and UI preview

The user confirms all three v4 smiths are visible in Camp. The screenshots show overlapping chairs below the bridge beside Ancient Hunt, and request the clearing beside the Gift Wrapper instead. This accepts v4 NPC loading/spawning only; it does not accept interaction, prices, item mutation, saves or multiplayer. The current test-price request is **1 emerald per upgrade and 1 gold for gilding**. Production prices remain separate.

Implemented Gift Wrapper-relative spawning using the exact collected native class path `/Game/Decor/Prefabs/Merchants/BP_LobbyVillager_GiftWrapper`. The prototype offsets are right 900 and forward -650/0/+650 in that actor's basis; these are authored test positions, not measured coordinates from the screenshots. All three seats have 650-unit spacing rather than 220. If no valid Gift Wrapper is present, no smiths spawn; there is no fallback to the old bridge position. Reload Camp after swapping builds. Host-only, nonreplicated behavior and per-class duplicate checks remain.

Added independent engine TextRenderActor labels: Uniquesmith, Powersmith and Gildsmith. Labels sit 70 units above each smith's actor bounds and face the available player camera when created, with 40-unit text size. This uses reflected `TextRenderActor.TextRender`, not its non-UFUNCTION C++ getter. Names are separate world text, not a complete native balloon/TTS implementation; camera orientation is sampled once. Font visibility, exact location, overlap and navigation need retail confirmation.

Added `--interaction-preview`: native interaction is allowed to open the owned merchant screen. In the three owned content copies, emptied and disabled **all 11 TransactionClassPrio-bearing widget/template configurations**:2 Unique,7 Power (including collect/bullet/slot views),2 Gild. Original Tower assets are untouched. No action in the owned graph calls TryExecute, Deduct, ClientAdd, GildItem or TryUpgradeItem. The intent is a read-only dialogue/selection test; native runtime behavior is not certified by serialization. If an upgrade action unexpectedly becomes enabled, do not treat that as the paid feature passing.

Added `RebalanceCanAffordTestUpgrade` to each owned root screen. Its native path validates the supplied player, requires authority, obtains WalletComponent using Actor.GetComponentByClass, validates the wallet, and checks captured WalletComponent.Balance against 1. It accepts the actual native SerializableItemId from its future caller, avoiding a guessed currency ID or player wallet field. Invalid inputs return false. This is a native preflight foundation **not wired to buttons, not a price override, and not a charge**. Emerald/Gold selection, an exclusive action gateway, chosen-Unique confirmation, success-only charging and normal-inventory persistence remain unfinished. Captured GetPrice is not a Blueprint event; post-success Deduct alone cannot enforce affordability before mutation. No further collector is requested.

Final local validation: 13 private-source integration tests pass; all 10 package pairs reopen with exact parsed equality and unchanged source hashes. Placement negative cases now 10, affordability 4, read-only bindings 2; prior presentation/record/selection/screen/loading checks remain. The actual crashed v3 still fails the loading-contract regression check. Compiler is the pinned direct Roslyn route with the known Newtonsoft net6/net8 warning. A first UI test found Powersmith has 7 transaction configurations rather than 2; corrected the source-specific count and reran all checks. These tests do not launch Dungeons.

Built `MinecraftDungeonsRebalance-CampUI-Test-v5.pak`:37 entries, 6133071 bytes, SHA-256 `eb7aa6324eb4bf9f47942234ab036e8ed28449a47a1d8a98c11a7137be9c080c`. Integrity test and exact path/byte unpack comparison pass. All17 non-chest v2 entries stay byte-identical; the replacement chest keeps100 emeralds. v5 is untested in-game. Reports: research/camp-central-ui-stage-v5.json and research/camp-central-ui-build-v5.json. Windows CI follows the source commit.

Next work: connect a single owned action button to the preflight and native transaction decision/success path; recheck funds at confirmation; charge once after verified success; establish persistent Camp inventory behavior and reloading. Do not re-enable the original free Tower action as a substitute. Shared gold, full economy/Hunt design and custom Unique picker are still incomplete.
