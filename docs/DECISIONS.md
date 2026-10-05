# Decisions

## D-001 — Standalone mod

Minecraft Dungeons Rebalance is separate from Minecraft-Dungeons-QoL.

Reason: economy/progression changes are a different product surface and players may want either mod independently.

## D-002 — Reuse QoL infrastructure, not runtime dependency

Relevant tooling may be reused from Minecraft-Dungeons-QoL:

- package patching
- cooked graph helpers
- inventory resolution
- UI helpers
- structural validation patterns

The final Rebalance mod should not require the QoL .pak.

## D-003 — Use native Tower smith behavior first

Prefer adapting the existing Tower Powersmith, Uniquesmith and Gildsmith rather than recreating item transformations from scratch.

Reason:

- preserves native item metadata
- reduces corruption risk
- preserves vanilla presentation and animation
- makes the Camp smiths feel official

## D-004 — Place Tower smith NPCs in Camp

The preferred UX is the actual Tower NPCs physically placed in Camp, not a generic debug/menu overlay.

Placement must avoid collisions with existing Camp NPCs, DLC/seasonal objects and common player paths.

## D-005 — Physical loot first

Economy changes should primarily increase world rewards rather than directly editing wallet totals.

Reason: opening a chest and seeing more treasure is better gameplay feedback than an invisible multiplier.

## D-006 — Ancient Hunts are the primary gold activity

Initial target:

- average complete run ~150-220 gold
- lucky/exploration-heavy run ~250-300 gold

Run-to-run variance should remain.

## D-007 — Emerald economy target is about 2x effective income

Do not blindly double every positive emerald transaction.

Prefer richer containers and bundles, then tune mob frequency as needed.

## D-008 — Preserve Prospector value

Do not raise base mob emerald chance so far that Prospector becomes pointless.

## D-009 — Better Ancient Hunt may be reused

The current Nexus permissions explicitly allow modification and asset reuse.

We will:

- inspect and reuse only relevant pieces
- document exactly what was reused or changed
- credit Onetoeisenough
- add stability caps rather than blindly increasing spawn density

## D-010 — Co-op gold sharing is a project goal

Where technically feasible, gold should benefit the party rather than only the collector.

Implementation must be tested for duplication exploits and host/client desync.


## D-011 — Exact retail evidence before native transactions

Use catalog-verified targets and preserve raw originals/defaults rather than inventing mutation signatures, spawn fields or shared-gold RPCs. Build the collector to unblock implementation. Collection errors retain raw originals and partial metadata; they do not authorize a gameplay release. Accepted design remains unchanged.

## D-012 — Prefer observed Lovika inputs for layout research

Camp/Hunt generation and Tower NPC definitions have actual JSON catalog paths. Inspect those alongside actor/widgets before choosing a Blueprint-loader spawning scheme or replacing a Camp map.


## D-013 — Deliver proven Hunt/economy subset for retail testing

Produce a clearly named test PAK from verified reward fields and adapted Lovika JSON while marking all native smith/shared-gold work unfinished. Do not silently treat native Tower's free transaction classes as priced Camp services. Do not infer network sharing from replicated actors or native presentation graphs.

## D-014 — Bounds and native-flow preservation

Bound adapted density at 1.75, per-type counts at 4, arena requests at 24 per wave/10 waves, side-path probability at 1 and maximum length at 4. These are experimental request limits, not a global active-mob cap. Preserve current native objectives/main-route geometry/triggers/gates/rewards. Keep the permitted extra two-Ancient wave and later raid captain; preserve upstream native arena timing controls pending runtime observation.

## D-015 — Guarantee a random Ancient independently of offerings

Accept the user request for at least one random reachable Ancient per successfully started Hunt, including a single-item/no-enchantment-point/no-matching-runes case. Do not equate rune eligibility, a chance-label change or additional arena waves with guaranteed generation. Do not remove gold rooms to achieve it. Implement at the authoritative generation/selection step after observing its contract; until then mark unimplemented.

## D-016 — Camp reward scalar

The actual BP_LobbyChest CDO contains EmeraldsReward=50 and derives from native LobbyChest. Set this single native field to 100 without altering availability or other chest graphs. Validate the vanilla value, bound configuration, serialize/re-open and reject mixed already-patched inputs before writing. Retail wallet measurement is still required.

## D-017 — Player-selected Unique outcome

For a native item family with multiple Unique variants, show localized names, native icons, descriptions/innate effects and native stat preview where available. Require explicit selection and confirmation of the exact result and price. Never fall back to random conversion or infer families from cooked filename suffixes. One-result families still show the outcome for confirmation.

## D-018 — Model before unverified native mutation

Implement choice/pricing/state/receipt guards as a tested reference model, clearly excluded from game PAKs until a native bridge exists. Default native capabilities false. Run on serialized native authority; actual atomic payment/state preservation and reconnect receipts require native validation. Prepare a source-only bounded reflection inventory using documented APIs, without enabling a new loader dependency or guessed native transaction.

## D-019 — Higher Ancient chance replaces guaranteed minimum

On October 6 the user withdrew D-015's guaranteed random Ancient requirement. Aim for a higher chance of one or more Ancient encounters; unlucky zero-Ancient Hunts are permitted. Keep native offering admission, gold rooms and the existing extra Ancient waves. No multiplier/percentage is agreed yet. The probability increase remains unimplemented until an authoritative control is verified and actual encounters are measured. This supersedes D-015's guarantee acceptance criteria.
