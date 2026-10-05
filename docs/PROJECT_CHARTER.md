# Project charter

## Working name

**Minecraft Dungeons Rebalance**

## Problem

Minecraft Dungeons contains progression systems that are fun in concept but can feel too stingy in practice:

- Ancient Hunt gold income is low relative to useful gold sinks.
- Emerald costs from merchants, gambling and upgrading can outpace comfortable natural income.
- The Tower contains excellent gear-upgrade NPCs that are not normally permanent Camp services.
- Co-op currency behavior should reward the party rather than encourage players to race for valuable pickups.

## Product direction

Create a standalone vanilla-plus gameplay rebalance mod.

The player should still earn currency by playing missions, exploring, opening chests and fighting mobs. The mod should make those activities more rewarding and give the extra currency useful sinks.

The intended feeling is **richer Minecraft Dungeons**, not a trainer or save editor.

## Pillar 1 — Camp smiths

Reuse the Tower's native:

- Powersmith
- Uniquesmith
- Gildsmith

Place them permanently in Camp and adapt their existing interactions to persistent player inventory and normal currencies.

Initial services:

- Common -> Rare
- Rare -> Unique
- Power upgrade
- Add Gilded
- Reroll / improve Gilded enchantment

## Pillar 2 — Ancient Hunts+

Ancient Hunts should become the primary repeatable gold activity.

Goals:

- substantially more gold
- more Gold Room / gold chest opportunities
- more enemies
- more Ancient encounters
- harder fights
- bounded spawn counts for stability
- co-op safe behavior

## Pillar 3 — Emerald economy+

Normal missions should yield enough emeralds to engage comfortably with merchants and upgrade systems.

Prefer:

- physical emerald drops
- richer loot urns and containers
- rewarding exploration
- visible loot feedback

Avoid:

- arbitrary wallet injection as the main mechanic
- free/infinite upgrades
- making Prospector irrelevant

## Pillar 4 — Better co-op economy

Where technically possible, valuable currency rewards should benefit the whole party.

Gold sharing is a first-class research goal.

## Separation from Minecraft-Dungeons-QoL

This repository is intentionally independent from Minecraft-Dungeons-QoL.

Code and tooling may be reused, but the final .pak(s) must not require the QoL mod unless a future explicit shared-core design is adopted.

## Non-goals

- Save-editor menus.
- Infinite currency buttons.
- Free item generation.
- Replacing Minecraft-Dungeons-QoL.
- Rewriting every progression system.
- Copying third-party assets/code without permission.
