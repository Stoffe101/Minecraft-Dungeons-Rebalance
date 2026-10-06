# Native integration gate

## What this pass established

`scripts/index_native_calls.py` indexes all 39 received cooked-package metadata reports: 247 observed native calls. The generated [table](research/native-calls.md) and [JSON](research/native-calls.json) retain import candidates, enclosing export/statement locations, receiver expressions, serialized arguments, return-property references and source metadata SHA-256 values. It rejects collector errors, broken reference chains and conflicting duplicate packages. It retains ambiguous function-name imports rather than choosing a class by guesswork.

Twenty-two calls contain serializer pointer/name error markers. Their JSON entries set `hasUnresolvedMetadata=true`. None of the entries certifies a native signature. Argument counts include serialized caller expressions, potentially including out parameters.

## Concrete call paths

| Integration | Actual evidence | Still required |
| --- | --- | --- |
| Tower merchant UI | BP_PlayerController calls TowerMerchantUtil.OpenTowerMerchant with a controller expression and byte value 0 or 1 | Native enum meaning, Camp support, actor association and persistent inventory semantics |
| Transaction feedback | Mission offering problem widget calls MerchantTransactionBase.QueryProblemStatus with a local hasProblem expression | Parameter direction; atomic payment and item mutation implementation; selected Unique result support |
| Mission request | UMG_TransactionAncientMobChances.RefreshAncientMobs calls MissionRequestUtil.CreateMissionRequest with player, byte 0, MissionSelection, integer 0 and offerings expressions | Current native types/flags and authoritative launch/generation handoff; this caller is a preview refresh |
| Probability UI | MissionChancesUtil.GetMissionProbabilities consumes MissionState | Actual authoritative encounter reservation/selection, not display probabilities |
| Currency display | BPL_Currency uses ItemFunctionLibrary.BreakItemId | Wallet writer and server authority; this is display data, not a payment API |
| Pickup presentation | Gold storable graphs include WalkPickupComponent.ResetPickup | Award owner, persistence and duplicate protection for host/client gold grants |

Do not use the byte/integer literals above to infer undocumented enum values or costs. Neither CreateMissionRequest nor OpenTowerMerchant is invoked by the Rebalance runtime source.

## Runtime evidence needed

The source-only 32-class UE4SS probe is **withdrawn and disabled by default** after the related QoL startup crash. The supplied reflection ZIP has no generated headers or completed capture. Three local Lua shim checks pass, including default disable; none proves game compatibility or fixes native loader startup. No loader DLL or installer is provided. Do not install/re-enable UE4SS for this probe on the user's game.

Current native contracts remain a dependency. The Drive copy gives direct asset access, but has no game executable, game-specific native DLL, symbols or runtime reflection output. Eight archived source mounts and six targeted reads succeeded; the two additional Ancient door Blueprints contain no functions. See [DRIVE_GAME_RESEARCH.md](DRIVE_GAME_RESEARCH.md) for exact inspected scope and transfer limits.

A future diagnostic/bridge must first establish compatibility in a developer environment. Imported names, generated offsets from another build and synthetic models cannot certify the user's native ABI or transaction semantics. The prior F8/Ctrl+H installation/test recommendation is withdrawn. Do not repeat loader installation or guess engine/signature settings.

Official references inspected: [dumpers](https://docs.ue4ss.com/feature-overview/dumpers.html), [UE4SS](https://docs.ue4ss.com/). The release documentation warns that generated memory layout is not accurate, that force-loading can crash the game, and that game-specific compatibility work may be necessary. Development-only jmap features are not assumed available in a release.

## Implementation after reflection

1. Establish native item family/presentation and selected-result conversion contracts, with persistent inventory ownership and preservation of item state.
2. Implement a native authority adapter with payment and mutation committed atomically, or a demonstrated recoverable transaction. Validate failure, duplicate confirmation and reconnect cases before exposing paid Camp services.
3. Build the native UMG names/icons/details picker and bind its explicit selection to that adapter; place the three native NPCs in Camp once their services work there.
4. Identify and tune authoritative native encounter probability/count to increase the chance of one or more Ancients. The user withdrew the guaranteed minimum on October 6. Compare matched offerings across runs and verify multiplayer agreement; retain gold rooms.
5. Establish native gold award authority and idempotency; implement party distribution and completion reward through it.
6. Measure income, Prospector, salvage, loot variant coverage and Gold Room generation in retail; tune against the agreed targets and complete the full test matrix.

The current v2 PAK remains a partial Hunts/Economy test build. None of the outstanding services is marked implemented by this research pass.


## Read-only native declaration work — October 6, 2026

Implemented a separate bounded external query/read collector and capture runner to investigate the paid Camp upgrade blocker without the withdrawn loader. Eighteen local synthetic/bootstrap checks pass. The dedicated Windows workflow adds normal compilation, a self-process WinAPI read, PowerShell parsing, incomplete-report and existing-output checks. No Dungeons capture or native gameplay test has occurred; declaration success cannot certify payment or item mutation. Latest gameplay v2 is unchanged. See [NATIVE_DECLARATION_RESEARCH.md](NATIVE_DECLARATION_RESEARCH.md) for sources, limitations, exact tests and next work. The remaining immediate dependency is a new capture from a running Windows Dungeons process, followed by native transaction implementation and real balance/item/reload tests.


## First retail native capture and discovery correction — October 6

Received native-contracts-20261006-131039.zip. All 19 original self-tests passed. The game query/read handle and PE/data reads succeeded: 6,345,728 bytes, nine reads. Collection stopped at “Too many name-table candidates” before validating any name table; zero classes/control contracts were collected and no game functions/writes occurred. This is a collector discovery-filter failure, not evidence of denied process access. Sanitized result: research/native-capture-20261006.json.

Corrected discovery to use cached, sorted VirtualQueryEx region queries and filter committed readable non-executable data before the existing 200,000 candidate bound. Complete candidate headers must fit their region. Retained byte/read/time bounds; added a 4,096-query bound and 1,000,000 raw-entry workspace bound. No new process rights or protection fallback. Reports include counts and at most eight distinct rejected object-layout reasons, without process addresses or game values. Twenty-one local checks now pass, including >210,000 numeric-noise candidates, memory protection states and boundary rejection; Windows adds its own-process read/query check. Native upgrade work remains blocked on an actual declaration capture. A new corrected capture is needed; no repeat cooked asset collection is requested.
