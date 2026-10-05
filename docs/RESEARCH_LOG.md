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

Added reproducible native call indexing of 39 supplied package reports: 247 call sites, with receiver/result/argument expressions, import candidates and metadata hashes. Four tests pass for nested receiver context, ambiguous imports, unresolved serializer markers and rejected broken evidence. Expanded the read-only probe from 23 to 32 evidenced classes; both Lua shim tests pass. Twenty-two call sites retain unresolved pointer/name markers; no signatures or native semantics were inferred from them.

Identified actual OpenTowerMerchant, QueryProblemStatus, CreateMissionRequest and BreakItemId caller expressions. None establishes a safe native wallet writer, selected-Unique transaction or authoritative encounter reservation. Re-read official release dumper documentation: header generation is available, but memory layout is not certified; force-loading should remain off. Development-only jmap was excluded from the proposed release diagnostics. No third-party source copied. Added NATIVE_RUNTIME.md with the concrete native integration dependency and implementation order. This workspace has no running game/executable. v2 artifact hash rechecked unchanged; no rebuilt/new gameplay PAK or retail tests claimed. Next dependency: current live reflection and game testing, not another copy of the same cooked assets.
