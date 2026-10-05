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
