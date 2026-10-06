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
