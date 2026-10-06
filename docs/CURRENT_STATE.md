# Current state

Updated 2026-10-06 after building CampPlacement-Test-v3.

**A new experimental Camp NPC placement PAK is available for game testing. It contains three native smith actor copies, owned merchant root/content screens, and a host-only spawn hook in Camp's existing lobby-chest Blueprint. Interactions and actor replication are deliberately disabled in this preview. Ten cooked package pairs reopen with exact parsed equality; nine real-source integration tests pass, including eight placement rejection checks. The 37-entry PAK passes integrity and exact unpack-byte verification. The accepted 100-emerald Camp chest reward is preserved. Paid/repeatable/persistent upgrades, explicit Unique picker and shared gold remain unfinished. No further collector is requested. See CAMP_PLACEMENT_TEST.md.**

The successful v5 capture remains accepted: 53 types and 26 enums, complete requested dependency closure. Capture/tooling success does not establish native paid transactions or save behavior.

**First Hunts/Economy test PAK built and structurally verified. Full design is unfinished. The user confirmed the Camp chest reward is 100 emeralds instead of 50; gold chests are accepted by user confirmation, and one urn payout of +25 emeralds was observed. Hunt progression and broader reward coverage remain unverified.**

## Input and research results

All 72 targets collected without errors. All 97 raw package/companion/JSON hashes verified against the collection manifest. Input lists 47 top-level game archives and no apparent directly installed mod archive. Better Ancient Hunt 1.0 contains 13 JSON files, no cooked Blueprints or source scripts.

Gold chest native defaults confirm ranges 4–6 and 8–10. Loot Urn base defaults specify 3–7 (not the earlier assumed 15–30); native unit/bundle semantics and subclass overrides still need testing. Smith actors point to native `/Script/Dungeons` merchant definition classes. Their widgets select native GildItem, UniqueCollectItem and UpgradeTowerItem transactions. Those implementations are not editable Blueprint graphs supplied by these packages.

## Implemented build: HuntsEconomy-Test-v2

- Normal/rare chest component ranges patched to 10–15 / 20–30.
- Base Loot Urn component amount range doubled to 6–14.
- Camp emerald chest native `EmeraldsReward` increased from 50 to 100.
- Three economy integration tests pass, including rejecting modified Camp defaults/invalid price ranges before output.
- Eleven original Hunt levels adapted with permitted mod enemy groups, arena waves and side paths.
- Twenty-six Ancient encounter definitions contain first wave of one, extra wave of two, and one raid captain.
- Finite generation-request bounds and exact native route/trigger/gate/reward preservation.
- Standalone 19-entry PAK produced, integrity checked and unpack-compared byte-for-byte.
- Four cooked package outputs re-opened; all imports, property schema and Blueprint scripts match their originals.
- Eight Hunt tests pass, including all eleven real levels and negative route/gate/reference/identity checks.

Windows CI run 37381427905 passed at source commit 1c8828775b521ca58592de31d2ce95da8c74623d: collector compilation, both manifest validations, five permitted-fixture integration tests, PowerShell parsing, economy patcher compilation and Python syntax checks. Policy run 37381427936 also passed. These are tooling/source checks, not retail gameplay tests. Earlier cancelled/fixture-setup failures are retained in RESEARCH_LOG.md.

See BUILD_AND_TEST.md for installation, actual scope and runtime checklist.

## Full design status

| Feature | State |
| --- | --- |
| Three Tower NPCs and paid smith services in Camp | Ten package pairs in placement preview; host-only non-interactive NPC spawning implemented; paid services/picker/persistence and retail placement unverified |
| Chest gold ranges | Implemented; user marked gold chests complete on October 6 (individual amounts not supplied) |
| Completion 50 gold | Unfinished; reward writer not identified in supplied Blueprint graphs |
| More enemies / Ancient waves / longer side paths | Implemented adaptation, retail untested |
| Gold Room opportunities +50–75% | Still a tuning goal; explicit native room weighting unresolved |
| Base urn drop amount doubling | Implemented; observed +25 emeralds (241 -> 266), baseline/effective multiplier/variant coverage unmeasured |
| Camp emerald chest | Implemented 50 -> 100; user confirmed 100 emeralds in game on October 6 |
| Global mob emerald/gold income | Unfinished |
| Higher chance of one or more Ancient encounters | New goal replaces guarantee; native probability/count control unresolved, v2 unchanged |
| Party-wide gold awards | Unfinished; native pickup/store authority contract unresolved |
| Typical 150–220 / lucky 250–300 gold | Target only; not measured |

## Next work

Prioritize Camp smith implementation: native selected-result transactions, currency payment, persistent inventory and presentation bindings. Remaining Hunt retail checks can proceed separately. Determine safe charge-before-mutation/commit-on-success semantics, persistent item preservation and repeatability before enabling Camp smiths. Avoid placing free Tower merchants into Camp and calling that a completed paid upgrade system. Source package collection is complete for the initial targets; do not ask for the same upload again.

## Latest continuation

Supplemental Ancient collection received: 15/15 targets, zero errors, 29/29 source hashes verified. The offering/chance/door inspection resolves to native probability/request paths and presentation, not an exposed guaranteed encounter writer. Guaranteed Ancient selection remains unimplemented. No repeat collection is needed.

Started upgrade choice/confirmation source foundation in src/rebalance/upgrades.py, including selected Unique outcomes with name/icon/description/effect/stat row data, agreed prices and item/ownership/catalog/balance/receipt guards. Seventeen synthetic-adapter policy tests pass. **No native adapter or in-game picker is implemented**; this Python module is not loaded by Minecraft Dungeons. Two developer-only Lua reflection API-shim tests pass; UE4SS compatibility remains unverified and no runtime loader is enabled. See UPGRADES.md.

Next: current native selected-variant/transaction contracts, native presentation/family resolution, native UMG picker and persistent Camp integration; native authoritative Hunt generation; shared-gold authority. Latest game test remains v2.

## Native call indexing continuation

Indexed all 39 supplied cooked-package metadata reports, recording 247 native call sites with exact serialized argument/receiver expressions and source hashes. Twenty-two sites contain unresolved serializer pointer/name markers and are flagged. Five evidence-indexing tests pass; two updated Lua shim tests pass for the expanded 32-class probe. No game integration was enabled and no new PAK build is claimed. Existing v2 SHA-256 remains 349181e2efaa8dfa6861133dab7286b03920dd469196de8b65cd5189f7c77b00.

The hard dependency is current live native reflection and access to the running game for acceptance testing. There is no game executable/runtime in this workspace. See [NATIVE_RUNTIME.md](NATIVE_RUNTIME.md) for actual call paths, required contracts, runtime diagnostic scope and implementation sequence. Repeating the asset collection or adding more synthetic upgrade tests will not resolve that dependency.

## October 6 scope revision

The user withdrew the guaranteed Ancient minimum and requested a higher chance of one or more encounters instead. Configuration and active roadmap now reflect that goal, with no fixed multiplier chosen and encounterChanceImplemented=false. Existing Ancient extra waves remain implemented, but native encounter-selection probability remains unchanged in delivered v2. Rune eligibility and UI chance presentation are not treated as actual spawn probability controls. See ANCIENT_GUARANTEE.md for the superseding goal and retained historical research.

## Drive source and runtime diagnostic correction

At the time of this Drive audit, the user had supplied the installation copy and had not yet tested v2. The subsequent Camp chest confirmation is recorded below. Traversed all 51 descendant folders/215 descendant files; read both manifests (1.17.0.0 x64), mounted eight archives/622 paths, and read six targeted assets with zero export errors. All ten preserved hashes match, including four gold-chest files identical to the earlier collection. Two additional Ancient doors have zero Blueprint functions and no chance control. Main archive download is limited by the connector; complete archive contents have not been audited.

Inspected the related QoL reflection ZIP: capture incomplete, no headers/object dump, only a three-line startup log. That project's user-reported UE4SS startup crash supersedes the earlier suggested diagnostic route. Rebalance's source-only probe is now disabled by default and the recommendation withdrawn; three Lua shim checks pass. This cannot repair a native loader crash. No new gameplay PAK/features or retail results are claimed. See DRIVE_GAME_RESEARCH.md.

## Retail observation — October 6, 2026

The user confirmed that Camp chests now give 100 emeralds instead of 50. This passes the Camp chest reward check for the tested setup. No screenshot or repeat is needed to accept that observation. Test count, multiplayer role and broader compatibility were not reported. This does not validate urn rewards, gold chest ranges, Hunt generation/progression, income totals, smith upgrades or shared gold. The v2 artifact is unchanged. Next retail checks: normal/rare gold chest payouts, extra Ancient waves and exit progression, then urn wallet deltas.

## Retail observations and priority — October 6, 2026

The user reports an emerald urn wallet increase from 241 to 266 (+25 emeralds) and asks to mark gold chests complete. Gold chest gameplay acceptance is recorded as passed by user confirmation; no individual normal/rare payout values or multiplayer role were supplied. The urn result is one observed payout, not evidence of a 2x global multiplier or every subclass. Camp NPCs and paid upgrades are now the highest implementation priority, including explicit Unique outcome selection with native names/icons/details.

## Camp smith implementation — October 6, 2026

CampSmithStager creates and reopens three isolated native actor copies and three content widget copies without overriding Tower assets. Four real-source integration checks passed. Exact actor definitions, native merchant enum values, owning-player selection graphs and TransactionClassPrio button bindings were traced. Native price/mutation/selected-result implementations remain unresolved, so no Camp spawner, charge, item upgrade or Unique picker is enabled. See CAMP_SMITH_IMPLEMENTATION.md for code, findings, validation and next work; v2 is unchanged.

## Camp smith CI validation — October 6, 2026

At implementation commit e5ed4a8ee150f9ce3cfed500d34b5bd8130182d9, Windows workflow 37401665954 passed normal project compilation of CampSmithStager, the existing collector/fixture checks, economy compilation and Python syntax. Policy workflow 37401665905 passed. Four Camp smith integration tests were run locally against the private originals; CI does not have those retail inputs. These results validate tooling and asset staging, not gameplay or paid native transactions.


## Read-only native declaration work — October 6, 2026

Implemented a separate bounded external query/read collector and capture runner to investigate the paid Camp upgrade blocker without the withdrawn loader. Eighteen local synthetic/bootstrap checks pass. The dedicated Windows workflow adds normal compilation, a self-process WinAPI read, PowerShell parsing, incomplete-report and existing-output checks. No Dungeons capture or native gameplay test has occurred; declaration success cannot certify payment or item mutation. Latest gameplay v2 is unchanged. See [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md) for sources, limitations, exact tests and next work. The remaining immediate dependency is a new capture from a running Windows Dungeons process, followed by native transaction implementation and real balance/item/reload tests.


## Native collector Windows validation — October 6, 2026

At source commit 2630d8732cbcc25828b18e87e7c16a5946525032, Native declaration reader run 37468028504 passed: normal Windows .NET build (zero warnings/errors), all 19 synthetic/self-process checks, PowerShell parsing, no-game incomplete-report behavior, existing-report preservation, compiled ZIP preparation and execution of the unpacked capture runner from paths containing spaces. The runner's failure ZIP contained NativeContracts.json, REPORT.json, Capture.log and SelfTest.log. Upgrade policy run 37468028478 and existing evidence tooling run 37468028467 also passed. None has a Dungeons runtime.

The downloadable compiled research bundle is Minecraft-Dungeons-Rebalance-NativeCollector.zip, 109,031 bytes, SHA-256 f6e3803ee5e9f8564274dbc7bb18b1b790ad795fed6681b2577f1a3fdc11e642. DLL SHA-256 bc5ab21c4b76a580e0a0b50ccc809beeca970c3edf6c6171a36b02c8ad14b74e. Downloaded Actions wrapper digest matched GitHub's advertised checksum; the nested bundle includes the runner, configuration and compiled tools, with no game-owned assets/PAK. The capture bundle is research tooling, not a new mod release. Dungeons compatibility and native transaction semantics remain unverified.

Next required input: one new declaration capture with Dungeons running at Camp, returned even if incomplete. Instructions are in tools/NativeContractReader/README.md. Do not repeat the existing asset collection or re-enable the withdrawn UE4SS loader. Paid Camp services and Unique choice remain unfinished; delivered gameplay v2 and its hash are unchanged.


## First retail native capture and discovery correction — October 6

Received native-contracts-20261006-131039.zip. All 19 original self-tests passed. The game query/read handle and PE/data reads succeeded: 6,345,728 bytes, nine reads. Collection stopped at “Too many name-table candidates” before validating any name table; zero classes/control contracts were collected and no game functions/writes occurred. This is a collector discovery-filter failure, not evidence of denied process access. Sanitized result: research/native-capture-20261006.json.

Corrected discovery to use cached, sorted VirtualQueryEx region queries and filter committed readable non-executable data before the existing 200,000 candidate bound. Complete candidate headers must fit their region. Retained byte/read/time bounds; added a 4,096-query bound and 1,000,000 raw-entry workspace bound. No new process rights or protection fallback. Reports include counts and at most eight distinct rejected object-layout reasons, without process addresses or game values. Twenty-one local checks now pass, including >210,000 numeric-noise candidates, memory protection states and boundary rejection; Windows adds its own-process read/query check. Native upgrade work remains blocked on an actual declaration capture. A new corrected capture is needed; no repeat cooked asset collection is requested.


## Corrected collector Windows result — October 6

At d55fe531e12a84a39d87407d4942b89656ba17a5, Windows native-reader workflow 37469426624 passed normal compilation, all 22 checks (including own-process VirtualQueryEx/ReadProcessMemory), PowerShell parsing, report preservation and execution/archive checks for the unpacked bundle. This validates the filter implementation; the corrected collector has not yet run on Dungeons.

NativeCollector-v2.zip: 113,119 bytes; SHA-256 8068ef6a006f39f0dd389bcc531eda4e3a86bce43fce988588e0dc3019c592f0. DLL SHA-256 16b03d99f63ddc0cbe22f15a9c88faad7cbdd4e40cde8e7d7efd497c3d80d36c. Verified the downloaded Actions wrapper against its advertised digest before extracting the new bundle. No gameplay PAK or paid upgrade is enabled. Next: run the corrected collector from its separate extracted folder with Dungeons at Camp and inspect the new report.


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


## Selected-item native integration — October 6

Added native physical-slot/item and full-record read functions to all three cloned smith screens, validated against captured InventoryItemSlot.Item and InventoryItem.Item declarations. Six private asset tests pass (11 deliberate graph rejection cases); no currency/item/save mutation is enabled. The newer QoL capture succeeded and confirms item-record field names/nested array types, but lacks transaction parent classes and field sizes/offsets.

Prepared collector v4 with 11 additional exact parent/struct targets and bounded nested array declarations; 54 local checks pass. Full paid Camp services still require that added capture and native behavior validation. No repeat v3 or asset collection is required. Detailed findings, source pins and next work: [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md). Gameplay PAK remains v2.


Collector v4 additionally requests referenced Dungeons enum symbols/values and underlying numeric types from source-grounded UE4.22 metadata layouts. Local collector checks now total 57; Windows/retail validation follows. No rarity/currency numeric values are guessed or enabled in gameplay.


## Dependency collector v4 Windows and bundle validation

Source ebecf590f3fd055db8697e4dc7444441d7e16c9a: native reader workflow 37476889616 passed normal Windows compilation (zero warnings/errors), all 58 synthetic/self-process checks, PowerShell parse/incomplete/report preservation and unpacked runner/archive checks. Upgrade policy workflow 37476889740 and evidence tooling workflow 37476889618 passed. Prior selected-item implementation ff74774352f72e5c8ed1e6fea8521962565b8ab1 also passed all three workflows (37476198325, 37476198286, 37476198223), including normal CampSmithStager compilation. Six private asset integration tests and their eleven deliberate graph-rejection cases passed locally; CI lacks retail input assets.

Downloaded artifact 11419715783 and matched its outer SHA-256 6d3df97af71a5b8464bd3887cd737dafdd9e3f7b04f79924f501c13434e7dc05 before extraction. NativeCollector-v4.zip is 124,625 bytes, nine entries, SHA-256 696d52d3eeff3ad42bff8a3a984a2fc2ed48204051c16a2069f20b5e226a5a4d; DLL SHA-256 7c3b00513caec5c8c49f341be5bc57f03eb15775812674e6865404448c77155a. Both ZIP integrity checks passed, and bundle README includes the new dependency and enum scope. Compiled research bundle made available for private testing; no game assets or loader included. This is not a gameplay PAK.

Required next input: one v4 dependency capture with Dungeons at Camp, using the same PowerShell command from a fresh extracted v4 folder. The original successful v3 capture is retained/accepted. Newly targeted inherited transaction APIs, full record field dimensions and enum identities/values are the remaining declaration input; native payment/mutation/save behavior then still requires implementation and gameplay acceptance. No paid upgrade or shared-gold completion is claimed.

## Successful v4 capture and transitive merchant research

Accepted native-contracts-20261006-142012.zip (SHA-256 23f8c58f7c46b7df653281ee9942fb445fe40de71fee3a4a17ba965fffba8d99): controls pass, all 29 requested types and 16 enums present, no gameplay calls or game writes. This supersedes the pending v4 input statements above; those remain historical. Reviewed full item fields, wallet calls, merchant parents and Tower UI enums. Native selected-record helpers now verify the real SerializableItemId.SerializedId offset (12), both array element types, native item flags/gilded record fields and actual sparse enum values. Four additional corruption checks pass. The selected embedded contract manifest now derives from this accepted capture.

Newly exposed dependencies are MerchantSlotTransactionBase, MerchantPricing, MerchantBase and ItemSlot. Collector source now follows native parents and nested structs automatically (128 declarations maximum), with nine additional roots taken from observed merchant package imports. It excludes unrelated declarations, external engine declaration bodies and object-instance values; the original global read/time limits remain. Five new closure tests pass; total local collector checks: 62. A new Windows bundle will be released only after CI verification.

Next: resolve native merchant factory/selection/pricing APIs using the bounded closure, then implement native outcome enumeration and paid execution, wire the picker and Camp dispatch, and test balances, item preservation, repeat upgrades and reloads in retail. Declaration collection alone cannot establish native behavior. No new gameplay PAK or smith completion is claimed.

## Transitive collector Windows validation and v5 bundle

At implementation commit de006824189a335033958f084ac1d98737ece46f, native reader workflow 37481190026 passed (job 112329420111): normal Windows build with zero warnings/errors, 63 checks including self-process reads, PowerShell parsing, no-game incomplete reporting, existing-report preservation, bundle preparation and packaged runner execution. Evidence workflow 37481190032 also passed (job 112329420366), including normal CampSmithStager build with zero warnings/errors. Policy workflow 37481190004 passed. These are tooling/source results; retail merchant transactions are not tested.

Artifact 11421416403 outer digest d79a12cc53980ce0170cf88e4adba8061bf9be29d9aa67d71dc8ebc51942c611 matched before extraction. NativeCollector-v5.zip: 127,660 bytes, nine entries, SHA-256 4e76ac087cff0ce2fee3175e64bc8ee5a93425aa9f2c21eea1be3127236a6ef5; DLL SHA-256 8bab5ecd7b6f1df493fdccce2badf786920d78e9ef8b8cb4c9817d6c1b92fb02. Both ZIP integrity checks pass. Bundle made available for the newly exposed merchant dependency capture; no game assets, loader or upgrades included.

Required next input: run the capture command from a fresh extracted v5 folder with Dungeons at Camp and return its output ZIP. The accepted v4 result is retained; this targets merchant/selection roots and their transitive native parent/struct dependencies. This is the concrete declaration blocker before native paid service integration. No new gameplay PAK or full-project completion is claimed.

## V5 fulfilled; collector loop stopped

Successful v5 capture and concrete actor/root/content wiring supersede earlier pending capture requests. No additional collector run requested. The native choice list event/selection return path is identified and retained. Pricing replacement and normal-inventory mutation/save behavior remain the blocking implementation work; declaration success does not make those executable. See CAMP_SMITH_IMPLEMENTATION.md for code, seven integration tests, limitations and next work.

## Native screen integration CI verification

Implementation commit 0285e2b9c8199ad4bab0fc96bd74c86c4a7e98c3 is published to main. Evidence workflow 37485292520, native reader workflow 37485292521 and policy workflow 37485292512 all succeeded. Windows evidence builds CampSmithStager normally; private retail source integration tests are the seven locally passing tests, not part of public CI. No new collector or playable PAK is delivered. The uploaded v5 capture is accepted and no further command/capture request is pending.

Additional public searches for existing Camp Tower-merchant mods and MerchantPricingComponent source did not produce an inspectable primary implementation in the returned results. Results included unrelated projects and general game pages; this search does not prove absence of an existing mod or pricing API. No unverified source was reused or recommended.
