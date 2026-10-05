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

The existing source-only UE4SS probe now inventories 32 classes, adding nine classes identified by this call index. It still performs bounded metadata reads only. No loader DLL, native call hook, NPC spawn, item setter or wallet mutation is deployed. Its two Lua API-shim tests validate source behavior, not Minecraft Dungeons compatibility.

Current game reflection is the next dependency. The supplied PAK assets cannot contain the native /Script/Dungeons implementations. This Linux workspace has no running game, game executable or current native reflection output. Further offline model tests cannot establish those contracts or fulfill gameplay acceptance.

For a developer environment where UE4SS compatibility has already been established, enable the NativeContracts source mod using that loader's normal mod mechanism, enter Camp and press F8. Retain the `[RebalanceContracts]` log entries. Obtain the loader's C++ headers as well: the official documentation lists Ctrl+H for its header generator. Keep force-loading disabled. Record the precise game build, loader release/commit, whether the game reaches Camp, and which classes are missing. Repeat while the Tower merchant UI and Ancient Hunt offerings UI are loaded if relevant classes are absent. Do not treat header offsets as trustworthy merely because the generator emits them.

UE4SS compatibility with this Store build remains unverified. This document does not direct installation of an untested loader into the user's game, and no automatic loader installer is provided.

Official references inspected: [dumpers](https://docs.ue4ss.com/feature-overview/dumpers.html), [UE4SS](https://docs.ue4ss.com/). The release documentation warns that generated memory layout is not accurate, that force-loading can crash the game, and that game-specific compatibility work may be necessary. Development-only jmap features are not assumed available in a release.

## Implementation after reflection

1. Establish native item family/presentation and selected-result conversion contracts, with persistent inventory ownership and preservation of item state.
2. Implement a native authority adapter with payment and mutation committed atomically, or a demonstrated recoverable transaction. Validate failure, duplicate confirmation and reconnect cases before exposing paid Camp services.
3. Build the native UMG names/icons/details picker and bind its explicit selection to that adapter; place the three native NPCs in Camp once their services work there.
4. Reserve one reachable native Ancient encounter in authoritative generation; test no-match and one-item offerings across seeds and multiplayer.
5. Establish native gold award authority and idempotency; implement party distribution and completion reward through it.
6. Measure income, Prospector, salvage, loot variant coverage and Gold Room generation in retail; tune against the agreed targets and complete the full test matrix.

The current v2 PAK remains a partial Hunts/Economy test build. None of the outstanding services is marked implemented by this research pass.
