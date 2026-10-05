# Architecture

This is the intended architecture before implementation research is complete.

## Principle

Keep **Minecraft Dungeons Rebalance** independent from **Minecraft-Dungeons-QoL**, while sharing proven source/tooling patterns where appropriate.

## Proposed modules

### 1. Tooling / build layer

Responsibilities:

- cooked package reading/writing
- Blueprint bytecode/graph patching
- package validation
- asset preservation checks
- reproducible .pak generation

Primary reuse candidate: Minecraft-Dungeons-QoL tooling.

Do not create a runtime dependency merely to share build code.

### 2. Camp Smiths

Responsibilities:

- place Powersmith, Uniquesmith and Gildsmith in Camp
- preserve native visuals/animations/interactions
- bind interactions to persistent hero inventory
- validate and spend emerald/gold costs
- execute native item mutations
- refresh inventory/UI safely
- prevent invalid/equipped/stale-item corruption

Subfeatures:

- Powersmith
- Common -> Rare service
- Rare -> Unique service
- Gild service
- optional Gilded reroll/improvement service

### 3. Ancient Hunts+

Responsibilities:

- tune hunt generation
- increase Gold Room opportunities
- tune normal/rare gold chest rewards
- increase mob density within safe bounds
- add/reuse extra Ancient waves
- optionally add raid captains
- preserve mission completion and co-op progression

Better Ancient Hunt is a reference/reuse input for this module.

### 4. Emerald Economy+

Responsibilities:

- tune Loot Urn rewards
- tune Camp Emerald Chest
- tune mob bundle size
- modestly tune drop frequency if needed
- preserve Prospector value

### 5. Party Rewards

Responsibilities:

- investigate shared gold
- prevent per-client duplication
- ensure host/client reward consistency
- make extra spawned rewards visible and collectible consistently

This module may end up being a small patch rather than a separate runtime system.

## Data-driven configuration

Where practical, put balance values in one documented source definition rather than scattering constants through bytecode patches.

Desired configuration model:

- Ancient Hunt completion reward
- normal gold chest min/max
- rare gold chest min/max
- Gold Room weighting
- additional Ancient wave counts
- mob-density caps
- Loot Urn min/max
- Camp Emerald Chest reward
- smith prices

Even if end users cannot edit these at runtime, centralized source constants make tuning safer.

## Item safety rules

Camp smiths must:

1. resolve the current physical item at interaction time;
2. reject missing/stale items;
3. reject unsupported item categories;
4. validate currency before mutation;
5. spend currency only if mutation can proceed;
6. prefer native mutation functions;
7. verify the resulting item;
8. fail closed on ambiguity.

No "spend first, hope mutation worked" behavior.

## Packaging

Initial preference:

- one independent Rebalance .pak for normal users
- debug/research builds may be split internally by feature

Possible debug packages:

- CampSmithsResearch
- AncientHuntResearch
- EconomyResearch
- MultiplayerRewardResearch

This makes retail testing safer before combining features.

## Documentation integration

Every implementation pass updates:

- CURRENT_STATE
- RESEARCH_FINDINGS
- RESEARCH_LOG
- TEST_PLAN results
- DECISIONS when architecture changes
- REUSE_AND_CREDITS when copied/reused material changes


## Implemented tooling as of 2026-10-05

`tools/RebalanceEvidence` reads targeted UE4.22 package metadata/defaults and exact raw companions plus Lovika JSON from game archives. `scripts/Collect-RebalanceEvidence.ps1` validates the installation/output, checksum-pins dependencies and produces a private evidence ZIP. `config/evidence-targets.json` contains catalog-verified exact paths; `config/balance.json` contains design values only. The gameplay architecture described above remains proposed, with native item/reward/replication bindings pending actual source collection.


## Implemented gameplay tooling

`CookedEconomyPatcher` changes six verified integer fields in three cooked reward components, validates native before-values/categories, writes/re-opens output and requires exact semantic equality to expected mutations. `build_hunts.py` adapts whitelisted upstream JSON fields onto installed originals with validation and bounds. `build_test_pak.py` composes a fresh standalone stage, packages seventeen known entries, integrity-tests the archive and unpack-compares every byte. Reports remain outside the runtime PAK. Native smith/party-reward architecture remains unresolved and is excluded from this build.
