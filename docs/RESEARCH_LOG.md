# Research log

## 2026-10-05 — Project bootstrap

Created the standalone repository **Stoffe101/Minecraft-Dungeons-Rebalance**.

### User goals captured

The project should be separate from Minecraft-Dungeons-QoL but may reuse relevant code/tooling from it.

Agreed direction:

- reuse Tower Powersmith, Uniquesmith and Gildsmith as physical Camp NPCs
- paid upgrades rather than free/save-editor changes
- Common -> Rare: 750 emeralds
- Rare -> Unique: 2,500 emeralds
- Add Gilded: 150 gold
- Gilded reroll/improvement: 250 gold
- increase Ancient Hunt gold substantially
- more Gold Rooms/chests where feasible
- more mobs
- more Ancient mobs/encounters
- richer emerald drops/containers
- improve co-op rewards, especially gold sharing

### Vanilla economy research

Minecraft Wiki research confirmed:

- Ancient Hunt completion reward is 30 gold.
- Normal Gold Chest is 4-6 gold.
- Rare Gold Chest is 8-10 gold.
- Version 1.17.0.0 increased these from lower historical values.
- Loot Urns drop 15-30 emeralds.
- Camp Emerald Chest contains 50 emeralds.

Initial target:

- typical full Hunt ~150-220 gold
- lucky/exploration Hunt ~250-300 gold
- emerald economy ~2x effective natural income

### Tower research

Confirmed Tower merchant variants:

- Powersmith
- Uniquesmith
- Gildsmith

Confirmed:

- Uniquesmith level ID: Artisan
- Gildsmith level ID: Gilder
- Gildsmith applies random gilded enchantment + random tier
- Uniquesmith has historically also normalized item power to strongest gear

Decision: locate/reuse native item mutation paths before inventing replacements.

### Better Ancient Hunt research

Nexus page inspected for Better Ancient Hunt v1.0 by Onetoeisenough.

Documented mod behavior:

- longer side paths
- harder mobs/minibosses
- more mobs
- second Ancient wave with two Ancients
- raid captains after gold/Ancient battles

Permission page explicitly allows modification and asset reuse.

Decision:

- use it as a legitimate reuse/reference source
- credit author
- inspect exact cooked changes before importing
- apply our own stability limits rather than blindly multiplying spawns

### Repository documentation created

- README
- docs index
- project charter
- current state
- decisions
- balance targets
- research findings
- architecture
- multiplayer plan
- reuse/credits
- roadmap
- test plan
- sources

### Next work

1. Import/audit reusable Minecraft-Dungeons-QoL tooling.
2. Obtain and diff Better Ancient Hunt v1.0.
3. Locate Tower merchant assets and native mutation functions.
4. Build a read-only/diagnostic proof before destructive item mutations.


## 2026-10-05 — Work Mode asset and tooling pass

Recovered agreement, cloned both repositories, inspected Store catalog plus prior inventory upload, verified Nexus permissions/file metadata and public archive-key provenance. Implemented standalone evidence tooling derived from QoL. Locally compiled via Roslyn against .NET 8 and pinned libraries. Five integration tests passed after correcting the test library location to the full pinned dependency directory; PowerShell scripts parsed. Attempting ordinary `dotnet build` in this container failed before MSBuild due to a host process-information error, so it is not claimed as a local MSBuild pass. Windows CI added for that build route. No game execution, new reward values or NPC transactions tested. Next input: targeted collector output and Better Ancient Hunt PAK.


## 2026-10-05 — actual gameplay patch pass

Received both requested uploads. Initial extraction normalized Windows ZIP directory separators; the extractor mistakenly treated a trailing backslash directory as a file, then was corrected and cleanly re-extracted. No uploaded archive modified. Collection: 72 targets complete, zero errors, 97 source hashes verified. Unpacked and semantically compared Better Ancient Hunt 1.0 (13 JSON entries).

Implemented CookedEconomyPatcher for the three verified reward components; direct Roslyn compile passed, all packages re-opened with full semantic comparison and original imports/schema/scripts preserved. Implemented a string-aware comment/trailing-comma parser and whitelist Hunt adaptation onto retail originals. Validation encountered a globally resolved vanilla wave group absent from local declarations; it now permits only groups already referenced by native waves plus declared adapted groups. Eight tests passed across all eleven real levels plus negative mutated identities/routes/gates/references and excessive wave counts. PAK builder produced 17 entries; u4pak integrity passed and extracted files exactly matched staging. At build time Windows CI was queued, not reported as green. Subsequent inspection of run 37364457448 found workflow failure with the sole collector job cancelled, zero steps and no assigned runner; no compilation/test step ran. Latest source CI is unverified. No retail game run, co-op grant or paid smith transaction has been tested.

## 2026-10-05 — guaranteed Ancient continuation / test v2

User requested guaranteed random Ancient spawning regardless of sacrifice count. Investigated collected encounter/biome definitions and the actual upstream hypermission configuration; traced the missed catalog-confirmed hypermission root plus offering/chance/door package leads. No proven authoritative spawn-roll field is in the current input set. Prepared 15 new allowlisted targets and a dedicated portable collection entry point; no overlap with the initial collection. Web searches did not establish a usable implementation/contract. No old SDK code or signatures adopted.

Implemented the separately verified Camp reward scalar 50 -> 100 in CookedEconomyPatcher. Direct Roslyn compilation passed with the existing .NET 6 dependency/.NET 8 reference-assumption warning; SDK/MSBuild still not certified locally. Three real-package economy tests passed, including four-package re-read, wrong Camp defaults/invalid ranges rejected before output and existing-output preservation. Rebuilt test v2: 19 PAK entries, integrity passed, all extracted bytes matched staging. Ancient guarantee, paid Camp smiths/shared gold remain unimplemented; retail execution not available. Supplemental metadata is the next concrete input; later native reflection may still be necessary.

Supplemental manifest validated by compiled collector; exact target paths checked against all 134,799 catalog entries and initial allowlist disjointness confirmed. Modified PowerShell scripts parsed successfully. All eight real-input Hunt tests re-ran successfully after the goal/config updates. No supplemental game metadata has been received or fabricated.

## 2026-10-05 — supplemental evidence / upgrade choice foundation

Received Ancient collection: 15 successful targets, zero errors, all 29 hashes verified. Inspected native hypermission root (`hypermission-mobs`, not upstream `mobs`), offering/request widgets, MissionChancesUtil call and door presentation. No authoritative count/selection writer is exposed in these inputs. Recorded native classes directly from actual /Script/Dungeons Class imports. No assets re-requested and no speculative guarantee patch produced.

User added explicit Unique variant picker requirements. Implemented reference choice/pricing/confirmation model, default-disabled native capabilities and selected-result receipt validation. Seventeen synthetic-adapter tests pass; no in-game UI/mutation claim. Studied official UE4SS reflection/key/game-thread docs; wrote source-only read-only native class inventory. Two Lua 5.4 API-shim tests pass; exact Store loader compatibility and actual wrapper/runtime behavior untested. No loader or hook installed. Public searches supplied no reliable selected-result native ABI; unlicensed/empty old SDK mirrors were not adopted. Previous workspace tooling had expired; no .NET rebuild is claimed this pass. Gameplay v2 remains unchanged.

Next: native runtime contract discovery and selected-type support, native family/presentation provider, UMG choice panel, persistent Camp transaction integration; authoritative guaranteed encounter contract. Document/update every pass.

## 2026-10-06 — exhaustive supplied native call indexing

Added reproducible native call indexing of 39 supplied package reports: 247 call sites, with receiver/result/argument expressions, import candidates and metadata hashes. Five tests pass for nested receiver context, argument-call receiver isolation, ambiguous imports, unresolved serializer markers and rejected broken evidence. Expanded the read-only probe from 23 to 32 evidenced classes; both Lua shim tests pass. Twenty-two call sites retain unresolved pointer/name markers; no signatures or native semantics were inferred from them.

Identified actual OpenTowerMerchant, QueryProblemStatus, CreateMissionRequest and BreakItemId caller expressions. None establishes a safe native wallet writer, selected-Unique transaction or authoritative encounter reservation. Re-read official release dumper documentation: header generation is available, but memory layout is not certified; force-loading should remain off. Development-only jmap was excluded from the proposed release diagnostics. No third-party source copied. Added NATIVE_RUNTIME.md with the concrete native integration dependency and implementation order. This workspace has no running game/executable. v2 artifact hash rechecked unchanged; no rebuilt/new gameplay PAK or retail tests claimed. Next dependency: current live reflection and game testing, not another copy of the same cooked assets.

Windows run 37381147650 compiled the collector and validated both target manifests, then failed because CI searched the downloaded fixture ZIP for a loose .uasset while it contains a .pak. Economy build was skipped. Corrected fixture unpack via the pinned u4pak -C option and exact-one-asset checks, switched test repacking to sys.executable for Windows portability, and pinned ZstdSharp.Port 0.8.4 to match the inspector's CUE4Parse dependency instead of the conflicting transitive 0.8.1. Policy CI run 37381147686 succeeded. Windows fixes require a new CI result before claiming integration success.

Follow-up Windows run 37381427905 succeeded at commit 1c8828775b521ca58592de31d2ce95da8c74623d: collector build, both manifests, five permitted-fixture integration tests, PowerShell parsing, economy patcher build and source syntax. Policy run 37381427936 succeeded (17 synthetic upgrade policy cases and five evidence-index cases). Local source-only Lua shim tests: two passed. Restored the pinned Mod Kit tooling, verified the permitted fixture download hash and fixture unpack locally, and rechecked the existing v2 PAK integrity with u4pak: All ok. Its 19 unpacked file hashes still match the build report. Eight real-input Hunt checks also passed. No .NET build was run locally and no new gameplay PAK/retail execution is claimed. Full integration remains dependent on current native reflection and a running game.

## 2026-10-06 — Ancient chance scope revision

User withdrew guaranteed Ancient spawning in favor of a higher chance of one or more. Updated active goals, config, roadmap and acceptance guidance; retained the guarantee research as explicitly historical. Re-inspected received root JSON: 29 hyperdungeons, rune eligibility requirements and no verified chance/count scalar. Higher selection probability remains unimplemented. Extra encounter waves do not imply a higher spawn chance, and no arbitrary rune rewrite/UI label change was shipped. Config JSON validation and diff checks pass. Also corrected stale Unique-choice wording in BALANCE_TARGETS to the already-agreed explicit choice at 2,500 emeralds. No gameplay code or PAK bytes changed; the delivered v2 remains the test build. Next: authoritative native probability/count control and measured matched-offering comparison, alongside paid Camp service integration.

## 2026-10-06 — direct Drive audit and diagnostic withdrawal

User supplied the copied installation on Drive, explicitly reporting no v2 retail test yet. Enumerated 51 descendant folders/215 files with no errors/truncated folder pages; listed all 47 game archives. Read installation manifests: Microsoft.Lovika 1.17.0.0 x64, native executable absent. Eight small archives materialized and CUE4Parse-mounted into a 622-path catalog. Six targeted reads succeeded (four packages, two JSON), zero export errors; ten source hashes verified. Four gold-chest source files exactly match earlier evidence. AH DoorLarge/Inner expose components/defaults but zero Blueprint functions, no selection writer. See DRIVE_GAME_RESEARCH.md and sanitized archive hashes.

Restored .NET 8 runtime and checksum-verified pinned inspector dependencies. Initial tar ownership preservation failed under the workspace filesystem; re-extracted without changing owners. Collector output-directory guard correctly rejected an unsafe research layout; moved only local copied archives into a game-shaped tree outside the output. Missing inspector dependencies were restored from the pinned ZIP. u4pak cannot read these original version-8 footers; existing CUE4Parse mounting succeeds without source rewriting. Initial catalog probes intentionally report missing targeted assets outside those chunks; these are not complete collections. Final six-target extraction passes. Main archive fetch failed with explicit 256 MiB connector cap; three medium file references could not materialize via signed URL (403), so none counted as inspected contents. No browser fallback, protected file access or Drive write occurred.

Fetched latest QoL docs and inspected the existing related reflection ZIP, rather than asking for another capture. It contains only an incomplete report and 180-byte startup log, no headers/object dump. Corrected Rebalance's previously suggested UE4SS route based on the related user startup crash: disabled the source-only probe by default and withdrew install/re-enable instructions. Three Lua shim tests pass; the disabled Lua flag is explicitly not a fix for earlier native loader hooks. No UE4SS installation/retail runtime occurred here. Native upgrades/chance/shared-gold remain unfinished; v2 unchanged. Next: supported direct archive transfer and native integration research using a compatible developer diagnostic route, not repeated UE4SS installation on the user's game.

## 2026-10-06 — First retail reward confirmation

The user reports that Camp chests now give 100 emeralds instead of 50. Recorded the Camp chest reward as an in-game pass in CURRENT_STATE.md, BUILD_AND_TEST.md, BALANCE_TARGETS.md and TEST_PLAN.md. This is sufficient evidence for that reward change in the tested setup. No screenshot, repeated test, test count, multiplayer role or broader compatibility was assumed. Other rewards, Hunt behavior, overall income, paid smith services and shared gold remain unverified or unfinished as previously documented. Prioritize gold chest ranges, extra Ancient waves/exit progression and urn wallet deltas next. Documentation only: no tests rerun, no PAK rebuilt, v2 artifact unchanged.

## Additional user results — October 6, 2026

Emerald urn: wallet 241 -> 266, observed +25 emeralds. Gold chests: user requests marking the feature complete; recorded as gameplay acceptance passed by user confirmation, with exact per-chest measurements unspecified. These observations do not establish aggregate income, all urn subclasses, Prospector behavior or multiplayer distribution. Camp NPCs and paid upgrade mechanics, including the Unique variant picker, are the user’s highest priority.

## Camp smith asset implementation — October 6, 2026

Implemented CampSmithStager: six native actor/widget clones in an isolated Rebalance namespace; original sources and Tower flags preserved; all outputs re-opened and semantically verified. Four private-source integration checks passed. Native Powersmith uses TowerBlacksmith enum and equipped-gear view; Artisan/Gilder use owning-player inventory selection. Upgrade buttons bind TransactionClassPrio to native transaction classes, not a demonstrated price/selected-result API. Tower NPC objectgroup contains complete floor tiles rather than portable props. See CAMP_SMITH_IMPLEMENTATION.md and research/camp-smith-stage.json. No live Camp placement, payment, persistent mutation or Unique picker was enabled; latest gameplay v2 unchanged. Local SDK/MSBuild process-information failures were bypassed with SDK Roslyn compilation; Windows CI now builds the normal project. Next: compatible native transaction route, paid selected-result mutation and native presentation binding, then Camp spawn/dispatch and real saved-item tests.

## Camp smith CI validation — October 6, 2026

At implementation commit e5ed4a8ee150f9ce3cfed500d34b5bd8130182d9, Windows workflow 37401665954 passed normal project compilation of CampSmithStager, the existing collector/fixture checks, economy compilation and Python syntax. Policy workflow 37401665905 passed. Four Camp smith integration tests were run locally against the private originals; CI does not have those retail inputs. These results validate tooling and asset staging, not gameplay or paid native transactions.


## Read-only native declaration work — October 6, 2026

Implemented a separate bounded external query/read collector and capture runner to investigate the paid Camp upgrade blocker without the withdrawn loader. Eighteen local synthetic/bootstrap checks pass. The dedicated Windows workflow adds normal compilation, a self-process WinAPI read, PowerShell parsing, incomplete-report and existing-output checks. No Dungeons capture or native gameplay test has occurred; declaration success cannot certify payment or item mutation. Latest gameplay v2 is unchanged. See [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md) for sources, limitations, exact tests and next work. The remaining immediate dependency is a new capture from a running Windows Dungeons process, followed by native transaction implementation and real balance/item/reload tests.


## First native reader Windows run — October 6

At source commit 4c90eb3929e288bd33908701fe053df6429b2184, run 37467815494 compiled with zero warnings/errors and passed all 19 checks, including the own-process query/read marker. No-game reporting and existing-output preservation behaved as expected. The job then failed because the intentional negative-test exit code remained in PowerShell LASTEXITCODE and the Actions epilogue treated it as a failure. Reset LASTEXITCODE only after asserting the expected failure and output conditions; apply the same fix to the packaged-runner negative test. Bundle preparation did not run in this first attempt. Local Linux incomplete-report and output-preservation checks also passed. This is a CI exit-handling fix, not a retail compatibility result.


## Native collector Windows validation — October 6, 2026

At source commit 2630d8732cbcc25828b18e87e7c16a5946525032, Native declaration reader run 37468028504 passed: normal Windows .NET build (zero warnings/errors), all 19 synthetic/self-process checks, PowerShell parsing, no-game incomplete-report behavior, existing-report preservation, compiled ZIP preparation and execution of the unpacked capture runner from paths containing spaces. The runner's failure ZIP contained NativeContracts.json, REPORT.json, Capture.log and SelfTest.log. Upgrade policy run 37468028478 and existing evidence tooling run 37468028467 also passed. None has a Dungeons runtime.

The downloadable compiled research bundle is Minecraft-Dungeons-Rebalance-NativeCollector.zip, 109,031 bytes, SHA-256 f6e3803ee5e9f8564274dbc7bb18b1b790ad795fed6681b2577f1a3fdc11e642. DLL SHA-256 bc5ab21c4b76a580e0a0b50ccc809beeca970c3edf6c6171a36b02c8ad14b74e. Downloaded Actions wrapper digest matched GitHub's advertised checksum; the nested bundle includes the runner, configuration and compiled tools, with no game-owned assets/PAK. The capture bundle is research tooling, not a new mod release. Dungeons compatibility and native transaction semantics remain unverified.

Next required input: one new declaration capture with Dungeons running at Camp, returned even if incomplete. Instructions are in tools/NativeContractReader/README.md. Do not repeat the existing asset collection or re-enable the withdrawn UE4SS loader. Paid Camp services and Unique choice remain unfinished; delivered gameplay v2 and its hash are unchanged.


## First retail native capture and discovery correction — October 6

Received native-contracts-20261006-131039.zip. All 19 original self-tests passed. The game query/read handle and PE/data reads succeeded: 6,345,728 bytes, nine reads. Collection stopped at “Too many name-table candidates” before validating any name table; zero classes/control contracts were collected and no game functions/writes occurred. This is a collector discovery-filter failure, not evidence of denied process access. Sanitized result: research/native-capture-20261006.json.

Corrected discovery to use cached, sorted VirtualQueryEx region queries and filter committed readable non-executable data before the existing 200,000 candidate bound. Complete candidate headers must fit their region. Retained byte/read/time bounds; added a 4,096-query bound and 1,000,000 raw-entry workspace bound. No new process rights or protection fallback. Reports include counts and at most eight distinct rejected object-layout reasons, without process addresses or game values. Twenty-one local checks now pass, including >210,000 numeric-noise candidates, memory protection states and boundary rejection; Windows adds its own-process read/query check. Native upgrade work remains blocked on an actual declaration capture. A new corrected capture is needed; no repeat cooked asset collection is requested.

The first local filter run rejected an incorrectly modeled synthetic free region that did not cover the requested high address. Corrected that fixture range; all 21 checks then passed. The containment check correctly remained in the production filter.


## Corrected collector Windows result — October 6

At d55fe531e12a84a39d87407d4942b89656ba17a5, Windows native-reader workflow 37469426624 passed normal compilation, all 22 checks (including own-process VirtualQueryEx/ReadProcessMemory), PowerShell parsing, report preservation and execution/archive checks for the unpacked bundle. This validates the filter implementation; the corrected collector has not yet run on Dungeons.

NativeCollector-v2.zip: 113,119 bytes; SHA-256 8068ef6a006f39f0dd389bcc531eda4e3a86bce43fce988588e0dc3019c592f0. DLL SHA-256 16b03d99f63ddc0cbe22f15a9c88faad7cbdd4e40cde8e7d7efd497c3d80d36c. Verified the downloaded Actions wrapper against its advertised digest before extracting the new bundle. No gameplay PAK or paid upgrade is enabled. Next: run the corrected collector from its separate extracted folder with Dungeons at Camp and inspect the new report.

Also reviewed newer same-owner QoL main 3914fec (implementation 44ee500): project-authored LegacyNativeEvidence collector with alternate child/target/name layouts and seven profile/salvage controls. No merchant/currency transaction bridge is implemented there. It remains a useful comparison for subsequent layout failures; no code was copied in this filter correction and its retail result was not assumed.


## Second retail capture and source-grounded layout correction — October 6

Received native-contracts-20261006-132200.zip. v2 filtering succeeded: 211,959 raw candidates reduced to 153,213 mapped-data candidates; 7,571,532 bytes, 153,233 reads, 1,054 region queries. No name table validated and no class declarations were collected. Sanitized report: research/native-capture-20261006-v2.json.

Read the related QoL native-favorites-20261006-132111-71d697.zip privately: revision legacy-names-256-v2 progressed past name validation but reported zero object-array shape candidates. It contains no accepted declarations. Reviewed latest same-owner QoL e535cab78a11999329ff3ae5d5c5608dd7986f07, including reserved-object capacity correction, as a reuse reference. Reviewed Epic-authored UE4.22.3 NameTypes.h and UObjectArray.h at 99a530d4ccbe6bea1e8f49df20acfeb294006962 through the GitHub connector. The engine name-array maximum/chunk size implies 256 pointer slots. Object preallocation rounds its reservation to an extra chunk; 2 Mi elements can reserve 33 chunks (2,162,688 slots), which the original max-capacity guard rejected. Reserved capacity is independent of live-object work.

Implemented inline and pointer-backed name tables, 128/256 capacities, reserved name chunks, bounded ANSI/UTF-16 text, and independently verified child/target layouts. Native function header selection remains controlled by four getters, and all six profile contracts must match. Object capacity now permits up to 4 Mi reserved slots/64 chunks while actual traversal remains bounded by the unchanged 1M live-object limit. The old 500K call ceiling was insufficient for two header candidates plus that live walk; now 2M calls, with the original 128 MiB/60-second and 4,096-query limits retained. No process rights, game calls, writes or protection fallback added.

Forty-nine local synthetic checks pass, including 24 inline/pointer/name/child/target combinations, distinct valid table ambiguity, 256-capacity tables with 33 reserved object chunks, and inconsistent-capacity rejection. The first compile caught a local variable/constructor-parameter name collision; renamed the parameter before testing. These fixtures validate declaration consistency and rejection behavior, not retail upgrade ABI or payment semantics. Windows validation and a new compiled capture bundle follow. Native Camp upgrade integration remains unfinished; v2 gameplay PAK unchanged.


## Native collector v3 Windows validation — October 6

At 6ab37c672d1cde522ce1997fcbddaa0e5aab10b0, native-reader Windows run 37471659764 passed normal compilation (zero warnings/errors), all 50 checks, PowerShell parsing, fresh/incomplete/report-preservation behavior and execution of the unpacked ZIP runner. Upgrade policy run 37471659806 and existing tooling run 37471659602 also passed. Retail v3 declaration discovery remains untested.

NativeCollector-v3.zip: 118,244 bytes; SHA-256 9b21fc20fd416a76939662a9da381b99be6afba9f589be549b5b0b76fe3fb252. DLL SHA-256 a78ea33df90e54cc41a6daa935a0d39bb2b951312d7c5fb5a6fb4b7a476e6783. The downloaded Actions wrapper matched its advertised digest before extraction. Compiled bundle saved for private testing. Next: capture once with this version at Camp, then use accepted declarations (or precise failure stage) to continue native transaction work. No native paid upgrade or new gameplay PAK is claimed.


## Successful retail declarations and native presentation — October 6

Received native-contracts-20261006-133711.zip. Completed=true, all six controls passed, all 18 allowlisted classes present, no runner issues. Accepted layout: name capacity 256, characters +12, children +0x48, property target +0x70, function header +0x98, parameter-count delta 4. Read 51,841,974 bytes in 1,292,194 calls and 1,045 region queries. Capture invoked no gameplay function and wrote no game memory. All 50 Windows self-tests passed. Sanitized summary: research/native-capture-20261006-v3.json; selected declarations and input SHA-256: research/native-upgrade-contracts-v1.json. Historical failed-capture sections above describe earlier attempts. A repeat of the same v3 capture is not needed.

Native ItemFunctionLibrary provides SerializableItemId-based name, description and Texture2D icon calls. MerchantTransactionBase exposes OnTransactionDecisionMade(InventoryItemData), CanExecute() and TryExecute(). WalletComponent.Balance returns int32; Deduct has no return value. GildItem, UniqueCollectItem and UpgradeTowerItem declare no direct functions and inherit InventoryItemSlotTransactionBase, which this capture did not collect. The selected-result declaration does not establish native outcome generation, transaction lifetime, payment override, mutation or persistent Camp behavior. InventoryItem.TryUpgradeItem must not be assumed to implement the agreed rarity/power policy from its name.

CampSmithStager now embeds the selected captured contract manifest and adds RebalanceUniqueName, RebalanceUniqueDescription and RebalanceUniqueIcon to the cloned Uniquesmith content widget. Each accepts the native item ID and calls the exact captured static native owner. Added function fields, ownership/function maps, native property archetypes and preload dependencies are validated before writing and after reopening. Original graphs and six source package pairs are retained; the three functions do not execute paid upgrades. Full choice list, native family enumeration, confirmation, Camp spawning/dispatch, persistent payment and shared gold remain unfinished.

Five private-source integration tests pass, including seven deliberate graph/signature/preload rejection checks. All six packages write/reopen with exact expected parsed equality; exactly three functions are added. Initial compilation caught an ArrayDim enum assignment; first round-trip caught nonserialized ElementSize/Next fields. Corrected both and reran successfully. SDK Roslyn compiled locally; one existing Newtonsoft net6/net8 reference-unification warning remains in this direct-compiler path. Report: research/camp-smith-presentation-stage.json. No Unreal/gameplay execution occurred.

Reviewed related QoL native-favorites-20261006-133827-9942ba.zip: incomplete at the 2M call budget, empty declarations. It supplies no missing superclass/struct contracts. Reused the same-owner QoL graph/property/preload construction approach at the user's explicit request; no third-party unlicensed source or game-owned assets published.

Next work: obtain the missing InventoryItemSlotTransactionBase/MerchantSubobjectBase/MerchantDef and InventoryItemData/SerializableItemId/MerchantDisplayPrice/EnchantmentData declarations, including nested array/enum types; trace native outcome enumeration and chosen-result handling; wire the presentation functions into an owned picker and Camp screen dispatch; establish persistent item/currency transaction behavior before gameplay acceptance. No new collector or NPC PAK is delivered in this increment.

Existing upgrade policy tests (17) and native call indexing tests (5) pass after this increment. Windows normal compilation subsequently passed at implementation 5f5b0a3 (workflow 37474277976, zero warnings/errors); no retail presentation/upgrade test is claimed.


## Native presentation Windows validation — October 6

Implementation 5f5b0a33b9109e7f5ba0c495cd5296ced58c6818 passed evidence tooling workflow 37474277976, including normal Windows CampSmithStager compilation with its embedded captured manifest, the existing collector fixtures, economy compilation and source syntax checks. Upgrade policy workflow 37474278027 and native reader workflow 37474278115 also passed. Private game assets are not available to CI; the five Camp asset tests and seven negative graph checks were run locally against the supplied originals. These results do not certify in-game names/icons, choice UI, payment or persistent upgrades.


## Native selected-item readers and targeted dependencies — October 6

Privately read related QoL native-favorites-20261006-134806-1f6922.zip: reader legacy-batched-slots-v5 completed, no issues, 11 declarations, 1,667,667 reads/49,019,979 bytes/3,345 ms. Native InventoryItemData fields include ItemId, ItemPower, Enchantments (EnchantmentData array), ArmorProperties (ArmorPropertyData array), Rarity, upgrade/gift/modified flags, modification count and Netherite fields. SerializableItemId has a SerializedId FName; this is not evidence of a permanent physical item identifier. Inventory slots contain native InventoryItemSlot objects. Selected sanitized records: research/qol-item-record-declarations.json. The capture lacks field sizes/offsets, enum identities and merchant transaction parent classes. Reviewed QoL main 24d3d39c1c58e1f28ee5b806a48c1b291e3c18a6, including bounded nested array declaration decoding.

Added two native read functions to each of the three isolated smith content widgets: RebalanceSelectedSlotItem and RebalanceSelectedItemData. They read the accepted InventoryItemSlot.Item and InventoryItem.Item fields through exact declaring-owner UProperty imports. The second reads the full native 120-byte item record, preserving unknown fields rather than reconstructing gear from a display name. Native callers/transaction lifecycle and persistence remain unimplemented. No mutation, currency change or save call added.

Six private asset integration tests pass, including the prior seven presentation rejection checks and four new read-graph rejection checks (missing construction flag, invalid skip offset, wrong result pointer, wrong native field). All six package pairs write/reopen exactly; six selection readers and three presentation functions are added, original functions and inputs retained. The normal Windows build follows publication. Existing 17 upgrade-model and five call-index tests also pass. No game execution is available in this workspace.

Expanded NativeContractReader's bounded target set by 11 exact dependencies: three parent classes (InventoryItemSlotTransactionBase, MerchantSubobjectBase, MerchantDef) and eight structs (InventoryItemData, SerializableItemId, MerchantDisplayPrice, EnchantmentData, ArmorPropertyData, ProblemStatus, InventoryItemMetaData, TowerFloorItemUpgrades). ScriptStruct identity is reported explicitly, and array elements decode recursively with a 16-node bound. Native array-inner owning-object equality is not assumed; that relation was not established by accepted retail evidence. New missing-dependency/coverage fields are separate from control-validation success. No process rights or byte/call/time/query bounds changed. Fifty-four local collector checks pass; Windows adds its own-process check. New cases cover target-kind separation, inherited merchant class names, nested native structs, null/non-property/cyclic array targets.

Dependency capture v4 is needed because the successful original capture deliberately omitted these classes/structs. Do not rerun v3 or repeat the cooked asset collection. Next: inspect these added declarations, implement native outcome enumeration/selected-result and verified paid transaction lifecycle, then connect the picker and Camp dispatch. A new gameplay PAK is not ready; economy/Hunts v2 remains the latest playable build.


## Enum dependencies in collector v4

Read Epic-authored UE4.22.3 Class.h, UnrealType.h and EnumProperty.h at folgerwang/UnrealEngine 99a530d4ccbe6bea1e8f49df20acfeb294006962 through GitHub. UEnumProperty stores its underlying numeric UProperty followed by the UEnum pointer; UEnum stores FString CppType followed by the Names array of FName/int64 pairs. Using the previously accepted UField/base-property layouts, v4 now records enum target identities/underlying numeric types and referenced Dungeons enum symbol/value pairs. Values are retained directly; indices are not treated as values. FName number is retained separately. Names arrays are bounded (512 live entries, 4096 capacity), symbols must be distinct, and the header is checked again after reading. No engine source is copied or bundled. This extended layout remains a source-grounded candidate until the new retail capture validates its metadata.

Fifty-seven local collector checks pass, adding sparse-value preservation, duplicate-symbol and oversized-array rejection. Windows adds its own-process test (expected 58). The full native record/enum dependency capture is intended to avoid another follow-up merely to obtain rarity/currency enum identities. Existing original controls and process rights/budgets are unchanged.

Primary source links: https://github.com/folgerwang/UnrealEngine/blob/99a530d4ccbe6bea1e8f49df20acfeb294006962/Engine/Source/Runtime/CoreUObject/Public/UObject/EnumProperty.h and https://github.com/folgerwang/UnrealEngine/blob/99a530d4ccbe6bea1e8f49df20acfeb294006962/Engine/Source/Runtime/CoreUObject/Public/UObject/Class.h .


## Dependency collector v4 Windows and bundle validation

Source ebecf590f3fd055db8697e4dc7444441d7e16c9a: native reader workflow 37476889616 passed normal Windows compilation (zero warnings/errors), all 58 synthetic/self-process checks, PowerShell parse/incomplete/report preservation and unpacked runner/archive checks. Upgrade policy workflow 37476889740 and evidence tooling workflow 37476889618 passed. Prior selected-item implementation ff74774352f72e5c8ed1e6fea8521962565b8ab1 also passed all three workflows (37476198325, 37476198286, 37476198223), including normal CampSmithStager compilation. Six private asset integration tests and their eleven deliberate graph-rejection cases passed locally; CI lacks retail input assets.

Downloaded artifact 11419715783 and matched its outer SHA-256 6d3df97af71a5b8464bd3887cd737dafdd9e3f7b04f79924f501c13434e7dc05 before extraction. NativeCollector-v4.zip is 124,625 bytes, nine entries, SHA-256 696d52d3eeff3ad42bff8a3a984a2fc2ed48204051c16a2069f20b5e226a5a4d; DLL SHA-256 7c3b00513caec5c8c49f341be5bc57f03eb15775812674e6865404448c77155a. Both ZIP integrity checks passed, and bundle README includes the new dependency and enum scope. Compiled research bundle made available for private testing; no game assets or loader included. This is not a gameplay PAK.

Required next input: one v4 dependency capture with Dungeons at Camp, using the same PowerShell command from a fresh extracted v4 folder. The original successful v3 capture is retained/accepted. Newly targeted inherited transaction APIs, full record field dimensions and enum identities/values are the remaining declaration input; native payment/mutation/save behavior then still requires implementation and gameplay acceptance. No paid upgrade or shared-gold completion is claimed.

## October 6 — successful v4 retail contracts and automatic dependency closure

Input: native-contracts-20261006-142012.zip, SHA-256 23f8c58f7c46b7df653281ee9942fb445fe40de71fee3a4a17ba965fffba8d99. Completed=true, controls pass, 29/29 requested types and 16 enums; missing lists empty. Read 52,016,905 bytes in 1,352,914 calls, 1,046 region queries. Accepted function header 0x98, NumParms delta 4, children 0x48, property target 0x70, name characters 12, capacity 256. No gameplay function or game-memory write occurred. Public review: research/native-capture-20261006-v4.json; selected declarations: research/native-upgrade-contracts-v1.json. Private source capture is not committed.

Findings and decisions:

- InventoryItemData is a native 120-byte parameter record. Reflected fields leave bytes 64–95 unaccounted for; SerializableItemId.SerializedId is at offset 12 in a 20-byte record. Do not construct item/currency IDs or replace records by assigning guessed bytes. Preserve native state through verified native operations. A struct read graph does not certify preservation during mutation/save.
- Rarity Common/Rare/Unique values are 0/1/2. EnchantmentCategory is sparse (Aoe=4, Armor=8, Permanent=64), so enum indices cannot substitute for values. Native Netherite source is 3. This establishes declarations, not a chosen gilding effect or persistence.
- InventoryItemSlotTransactionBase inherits MerchantSlotTransactionBase, absent from the earlier fixed request. MerchantDisplayPrice.Pricing targets MerchantPricing (8 bytes); MerchantSubobjectBase.mMerchant targets MerchantBase; WalletComponent currency slots contain ItemSlot objects. These are unresolved declarations, not missing existing assets.
- TowerMerchantUtil.OpenTowerMerchant takes ETowerUIMerchantType, whose values are TowerComplete=0 and TowerFloorComplete=1. It is not the actor merchant enum and cannot be treated as a direct smith opener.
- WalletComponent.Deduct returns void; ClientAdd is a client RPC. No atomic payment, rollback or authoritative party-wide gold contract follows from these signatures. MerchantTransactionBase.GetPrice is a native callable, not an observed Blueprint override event; a custom price label would not establish actual charging.

Implementation: update embedded selected native contracts from this capture; gate selected-record asset generation on exact record fields, array elements and enum values. Four corruption checks reject an ItemId name offset of zero, wrong enchantment array element, enum index replacing sparse value, and truncated gilded enchantment state. Existing native read/presentation graphs remain unchanged; no fabricated records or gameplay mutation are enabled.

Collector improvement: index at most 4,096 native declaration identities during the existing object-table scan; emit only the explicit roots plus recursively referenced native structs and superclass declarations (128 maximum). Nine roots come from retail package imports: MerchantActor, MerchantBase, MerchantBaseWidget, MerchantCurrencyComponent, MerchantDefComponent, SelectInventorySlotItem, SelectMerchantSlot, UpgraderItemSlot, ItemSlot. Object instance values and unrelated declaration bodies are not exported. External engine references remain qualified without exporting their bodies. Missing roots have a separate report field; DependencyClosureComplete requires original roots, selected dependencies and merchant roots. Native function bodies and map inner types remain undecoded.

Validation: direct Roslyn collector compilation and 62 synthetic checks pass. Five new checks cover recursive parent/nested pricing records, unrelated type exclusion, inheritance cycles, wrong parent/struct kinds and declaration bound. Camp stager compilation and six private real-source integration tests pass, including 15 graph/contract rejection checks; originals preserved and six package pairs reopened. The local direct Roslyn Camp build retains its known Newtonsoft net6/net8 reference-unification warning. No retail upgrade test has occurred.

Next: validate the compiled closure collector on Windows, resolve merchant factory/outcome selection and native pricing behavior, then integrate paid Camp services and an owned Unique choice list. Continue toward a PAK after those paths are wired; the current usable PAK remains v2. Do not ask for another copy of the successful v4 capture or unchanged game assets.

## Transitive collector Windows validation and v5 bundle

At implementation commit de006824189a335033958f084ac1d98737ece46f, native reader workflow 37481190026 passed (job 112329420111): normal Windows build with zero warnings/errors, 63 checks including self-process reads, PowerShell parsing, no-game incomplete reporting, existing-report preservation, bundle preparation and packaged runner execution. Evidence workflow 37481190032 also passed (job 112329420366), including normal CampSmithStager build with zero warnings/errors. Policy workflow 37481190004 passed. These are tooling/source results; retail merchant transactions are not tested.

Artifact 11421416403 outer digest d79a12cc53980ce0170cf88e4adba8061bf9be29d9aa67d71dc8ebc51942c611 matched before extraction. NativeCollector-v5.zip: 127,660 bytes, nine entries, SHA-256 4e76ac087cff0ce2fee3175e64bc8ee5a93425aa9f2c21eea1be3127236a6ef5; DLL SHA-256 8bab5ecd7b6f1df493fdccce2badf786920d78e9ef8b8cb4c9817d6c1b92fb02. Both ZIP integrity checks pass. Bundle made available for the newly exposed merchant dependency capture; no game assets, loader or upgrades included.

Required next input: run the capture command from a fresh extracted v5 folder with Dungeons at Camp and return its output ZIP. The accepted v4 result is retained; this targets merchant/selection roots and their transitive native parent/struct dependencies. This is the concrete declaration blocker before native paid service integration. No new gameplay PAK or full-project completion is claimed.

## October 6 — v5 accepted; actor/screen/content integration

Accepted native-contracts-20261006-145714.zip: controls and dependency closure pass; all requested roots/dependencies present, 53 types and 26 enums. Read 52,772,581 bytes / 1,379,749 calls / 1,051 queries. No native gameplay calls, game writes or upgrade semantics verified. Public review: research/native-capture-20261006-v5.json. The earlier request for a v5 capture is fulfilled. **No further collector requested.**

Research resolves actual paths:

- MerchantActor.mMerchantWidgetClass is a 40-byte native SoftClassProperty. Override it in each isolated actor CDO to its owned Camp root screen.
- MerchantBaseWidget.GetSoftContentWidget is a BlueprintNativeEvent. The existing UMG_Merchant override can return the corresponding cloned smith content as a soft class, avoiding the shared BPL_Merchants dispatch.
- MerchantBaseWidget.OnDecisionToBeMade receives an array of native InventoryItemData. The original UMG_Merchant event routes this to the existing Content_Season1 UMG_MerchantItemDecision.SetItemsToDecide. Its InstDecisionContent function creates the widget with GetOwningPlayer and binds OnDecisionMade to the merchant root's inherited native handler. This is an existing item-choice path, not a need to fabricate an SDK or reconstruct item records.
- SelectInventorySlot exposes SelectInventorySlot, GetSelectableInventorySlots and GetInventorySlot; MerchantBaseWidget exposes GetSelectionByClass and GetTransactionByClass. MerchantSelectionBase exposes confirm/cancel state and actions. These declarations establish available APIs, not Tower-versus-normal-inventory behavior.
- MerchantPricing has Price (int32 offset 0) and RebateApplied (float offset 4). It is returned inside a price value, not an observed mutable transaction pricing field. MerchantTransactionBase.GetPrice/TryExecute are native callables; GetPrice is not a Blueprint override event. No supported custom pricing setter or persistent normal-inventory upgrade contract was identified in this capture. MerchantBase refers to a MerchantPricingComponent whose body was not part of this capture; absence of a setter here is not proof that all possible native extension paths are impossible.

Implementation: MerchantScreens.cs clones UMG_Merchant separately for each smith, relocates only exact self identities (avoids renaming foreign UMG_MerchantItemDecision imports), replaces only GetSoftContentWidget with a typed soft-class return, binds each actor to its own root, and validates the native event/array/screen ABI from the v5 manifest. All remaining root function bytecode is checked byte-for-byte before and after serialization; the stock choice-widget reference is required. Nine private package pairs now connect actor -> root -> content, retaining the native item-choice/input/owning-player graphs. Camp world placement, paid execution, repeatability and retail behavior are still not implemented/verified. Existing native one-transaction flags remain true.

Tests: seven real-source integration tests pass, including nine-pair semantic round-trip, original hashes, three correct content routes and three new rejection checks (wrong content route, erased decision graph, mismatched actor screen). Existing 15 graph/record rejection checks continue to pass. First direct graph JSON serialization failed because UAssetAPI FName converters need asset context; replaced it with actual serialized bytecode comparison. First actor write then caught the absent SoftObjectProperty type name; registered it before serialization. Final tests and a retained private stage pass. Direct Roslyn compilation retains the known Newtonsoft net6/net8 reference-unification warning; Windows CI is checked separately. Report: research/camp-merchant-wired-stage.json.

Decision: stop iterative declaration collection. The capture alone cannot provide native C++ behavior or a working runtime extension. A complete paid Camp-upgrade PAK needs a verified custom pricing/execution path and normal-inventory/save behavior; none is established for the current Store setup. Do not silently ship free Tower services, guessed mutations or another collector. The old loader remains withdrawn. This pass implements concrete screen integration without claiming runtime upgrade completion.

Next work: seek an existing exposed customization path or a validated native mod runtime/toolchain that can implement the agreed prices and normal-inventory preservation; then wire execution and Camp placement and run real item/balance/reload/party tests. If neither route is available, report this limitation rather than continuing capture versions. HuntsEconomy-Test-v2 remains the usable gameplay PAK.

## Native screen integration CI verification

Implementation commit 0285e2b9c8199ad4bab0fc96bd74c86c4a7e98c3 is published to main. Evidence workflow 37485292520, native reader workflow 37485292521 and policy workflow 37485292512 all succeeded. Windows evidence builds CampSmithStager normally; private retail source integration tests are the seven locally passing tests, not part of public CI. No new collector or playable PAK is delivered. The uploaded v5 capture is accepted and no further command/capture request is pending.

Additional public searches for existing Camp Tower-merchant mods and MerchantPricingComponent source did not produce an inspectable primary implementation in the returned results. Results included unrelated projects and general game pages; this search does not prove absence of an existing mod or pricing API. No unverified source was reused or recommended.

## October 6 — Camp placement preview implementation and packaged build

Implemented SpawnGraph/CampPlacement using the permitted same-owner QoL graph construction pattern. Inspected retail native call sites and pinned UE4.22.3 GameplayStatics/Actor/EngineTypes declarations before generating calls. Rejected embedding an entire Tower floor or guessing Lovika prefab block encoding. Chose the already supplied Camp lobby-chest begin-play hook, authority guard, per-class dedupe and authored nearby offsets. Spawn runs before the preserved original chest graph to avoid using an anchor removed by that graph. Native interaction disabling occurs after FinishSpawningActor; missing component destroys the preview actor. Replication is disabled before finishing; clients are out of preview scope.

Initial local compilation failed because EX_VectorConst/EX_RotationConst store Value structs, rather than XYZ fields; fixed using inspected UAssetAPI FVector/FRotator constructors. Initial merge review found the Camp chest pair also carries the accepted v2 100-emerald change. Preserved that scalar explicitly, validated original 50 before changes, and added a regression rejection check. Corrected the proposed merge count from 39 to 37 because those two entries overlap. Collision handling verified as enum2 AdjustIfPossibleButAlwaysSpawn, not guaranteed collision-free placement. Hook order changed before final build; final tests/stage/PAK use the corrected graph.

Final results: nine real-source integration tests pass, including eight placement rejection checks; ten package pairs reopen with exact parsed equality and unchanged original hashes. Final 37-entry PAK integrity-tests and unpack-compares exact paths/bytes. Existing baseline 17 entries remain identical; the two replaced chest files retain the 100 reward. Public hash-only reports and build/install instructions saved. No game runtime is available here: NPC visibility, native initialization, geometry, duplicate behavior and interaction state remain retail-unverified. No new collector request, native charge, inventory mutation, explicit Unique picker, shared gold or gameplay completion is claimed.

Next: placement retail acceptance; supported native payment/persistent selected-result transaction integration and repeatability. Native pricing absence in captured declarations is not proof of impossibility, and no unsupported setter/struct mutation has been invented.

## Camp placement preview Windows validation — October 6, 2026

At implementation commit 7972a9127c4ebaf1c90ad51452618afe9b4c9fc4, evidence run 37521291248 passed normal Windows CampSmithStager compilation, existing permitted-fixture integration checks and Python syntax validation of the preview builder/tests. Native declaration reader run 37521290998 and upgrade policy run 37521290989 also passed. Local private-source checks passed nine tests, including eight placement corruption rejections; an additional packager check preserved an existing output directory. Windows CI does not have private retail assets or a Dungeons runtime. The final 37-entry PAK hash is 6f13bc4f04f2f5d4a4782803158e41943c29d647b98968cae2066a9495fd409a (6,075,200 bytes). Test artifact is saved and source/documentation published; retail NPC placement remains pending.

## October 6 — Reuse Camp purchase transactions for smith costs

User proposed combining existing Tower upgrade behavior with Camp merchants' purchase/payment behavior and fixed prices selected by upgrade rarity. This is the preferred integration candidate: reuse native affordability, payment and transaction completion while retaining native smith result selection/mutation. It is not yet a demonstrated interchangeable component.

Rechecked supplied UMG_MerchantBlacksmithContent metadata: TransactionClassPrio selects native UpgradeInsertItem and UpgradeCollectItem. BPL_Price exposes GetRebateText; it is presentation, not a demonstrated charging hook. The captured MerchantTransactionBase shared base has CanExecute, TryExecute, GetPrice and OnTransactionDecisionMade; MerchantBaseWidget exposes transaction resolution and native execution/decision events. Thus Camp and Tower already share transaction infrastructure, supporting the reuse proposal. A Camp Blacksmith transaction also has its own insert/collect service semantics; swapping that entire transaction does not establish an instant Tower upgrade.

Outstanding connection: configure/replace the price provider used by the actual upgrade transaction and its currency/eligibility rules, with payment tied to successful persistent mutation. GetPrice is not a captured Blueprint override event; MerchantPricing is a returned value, not a verified mutable transaction property. MerchantPricingComponent remains an implementation lead, not an identified setter. No new capture requested and no runtime service enabled by this research.

Price policy remains Common-to-Rare 750 emeralds; Rare-to-selected-Unique 2500 emeralds; gild 150 gold; reroll 250 gold. Select price from the requested operation plus selected item eligibility; gold operations are not determined by rarity alone. Powersmith price/rules remain undecided. Next: trace Camp price-provider/transaction initialization using already available evidence and any inspectable native-compatible implementation, then validate affordability, exact single charge, cancellation/failure and save/reload with native smith mutations. No new tests were needed for this research-only pass; existing v3 artifact is unchanged.

## October 6 — Requested combined placement/interaction/paid-upgrade test

User asked to enable the purchase-reuse approach now and test placement, interaction and upgrades together. Reopened the successful v5 raw declaration ZIP locally and inspected the supplied Camp Blacksmith bindings/asset inventory. GetPrice has native flags0x54020401, HasPrice0x54020401; neither has FUNC_BlueprintEvent. TryUpgradeItem has no input parameters, so it cannot directly receive our requested price/rarity/outcome. GildItem takes EnchantmentData and returns bool; that signature does not establish random roll construction or persistence. Wallet Deduct takes currency ID and amount but returns void; ClientAdd is a client RPC and is not a demonstrated rollback API. CanExecute/TryExecute share the native transaction framework but do not expose a price setter in captured declarations.

MerchantBase.mMerchantPricingComponent is referenced, but MerchantPricingComponent's declaration/body was not captured. The previously fetched MCD-PE source-path inventory lists MerchantPricingComponent.h and ProgressPowerRarityPricing.h, marked in_binary=false; this is a naming lead only, not executable code or a pricing contract. No game binary/protection fallback was used. The supplied asset inventory identifies UMG price/bind/button assets; a UMG price counter or rebate-text function does not set the native amount charged. Public searches for exact classes and existing Camp Tower smith implementations produced no usable primary implementation (unrelated/general/community-feature-request results excluded); this does not prove impossibility.

Result: no supported paid native integration can currently be produced from these inputs. No combined upgrade test PAK generated, and v3 remains a non-interactive placement test. Do not activate native Tower buttons and report them as the requested paid service. A test of native free behavior would be a different feature, with normal-inventory persistence still unverified. User's requested combined test remains blocked on an actual price-provider initialization/setter/override route and verified persistent selected-result mutation. No further collector requested, no payment/inventory/save calls executed, no new gameplay success claimed. New hash-only feasibility report records the exact method flags and missing pricing body. Research-only pass: no implementation change requiring new tests; prior nine asset tests/37-entry PAK verification remain valid for unchanged v3.

## October 6 — v3 crash evidence, loading-contract correction and pricing source audit

Accepted user's v3 crash report; marked v3 withdrawn and v2 the reward-accepted baseline. Read supplied XML and minidump directly from scratch; no Library reread. Standard-library bounded parser now saves a sanitized hash/exception/RVA summary without module paths. XML: Engine4.22.3 and elapsed36s. Dump: 0xc0000005 write0x28; RVA0x11d838c. Objdump of captured instruction bytes loads last childarray entry(count7) and writes Next at0x28; matched pinned UE4.22.3 UStruct::Serialize. Missing heap coverage/symbols prevents exact loaded-object identification. This matches the modified seven-child Camp chest class but does not prove asset identity or exclude another failure.

Found and corrected independent code defect in generated functions: cleared native Function archetype and missing SerializationBeforeCreateDependencies on native class/archetype. Restored original TemplateIndex and prerequisites in SpawnGraph, UniquePresentation, SelectedItemReaders; validates imported native Function/Default__Function plus owner creation before every generated function. Initial private inspector compile had a formatting error; corrected before using results. New negative checks include the actual crashed v3 package. Eleven real-source integration tests pass, ten package pairs round-trip exactly with unchanged originals. The corrected37-entry v4 candidate integrity-tests/unpack-compares; no retail fix/placement/payment success is claimed. Prices/interactions unchanged and disabled. No further collector requested.

Pricing research found zH4x-SDK/zMCDungeons-SDK pinned ab9d8f0ab04b215577dd2eb067e65015b5a70521. Read Dungeons_classes.h and Dungeons_parameters.h. It identifies MerchantPricingComponent and ProgressPowerRarityPricing but exposes no fields/functions for either. Its GetPrice/Wallet parameter structs are empty, in direct conflict with v5's known28-byte price result and24-byte Deduct input. This SDK is useful only as a naming lead, not a valid current ABI or a verified empty pricing implementation. No SDK code/assets copied, no licensed engine/game binary obtained or protection route used. No supported custom pricing setter/provider initialization route was found. Price enablement remains unresolved and the project remains incomplete.

Next: confirm candidate loading correction in game; investigate a supported native pricing extension or an authoritative custom payment gateway coupled to the native selected-result transaction. Such a gateway needs verified affordability interception, exactly-one charge after successful mutation, cancellation/failure behavior, valid currency identity/owner wallet access and persistent inventory semantics. Existing read-only signatures do not establish those behaviors. Do not silently expose free Tower buttons as completion of the paid design.
