# Current state

Updated 2026-10-06 after read-only native declaration tooling; full Camp upgrade integration remains unfinished.

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
| Three Tower NPCs and paid smith services in Camp | Six isolated native actor/widget packages staged and validated; Camp placement/payment/picker unfinished |
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
