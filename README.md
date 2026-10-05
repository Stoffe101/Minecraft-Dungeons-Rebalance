# Minecraft Dungeons Rebalance

A standalone Minecraft Dungeons gameplay rebalance mod focused on making Ancient Hunts, currency progression, and gear upgrading more rewarding while keeping the game recognizably vanilla.

> **Status:** research/design bootstrap. No gameplay build has been produced yet.

## Core goals

- Bring the Tower's **Powersmith**, **Uniquesmith**, and **Gildsmith** into Camp as permanent upgrade NPCs.
- Adapt those native smith interactions to the player's persistent gear and normal currencies.
- Make **Ancient Hunts** substantially more rewarding:
  - more gold
  - more Gold Room/chest opportunities
  - more mobs
  - more Ancient encounters
  - higher challenge without unstable spawn spam
- Increase normal **emerald income** mainly through richer physical drops and containers.
- Investigate **party-wide gold sharing** so co-op players benefit together.
- Remain completely installable independently from Minecraft-Dungeons-QoL.
- Reuse proven infrastructure from Minecraft-Dungeons-QoL where useful.
- Reuse third-party mod code/assets only where permissions allow, with explicit credit and documentation.

## Initial economy targets

### Camp smiths

| Service | First-pass price |
| --- | ---: |
| Common -> Rare | 750 emeralds |
| Rare -> Unique | 2,500 emeralds |
| Add Gilded | 150 gold |
| Reroll / improve Gilded enchantment | 250 gold |
| Power upgrade | TBD after native Powersmith research |

### Ancient Hunts

Initial target after a complete run:

- **Typical:** ~150-220 gold
- **Lucky / exploration-heavy:** ~250-300 gold

First-pass values to investigate:

- completion: ~50 gold
- normal gold chest: ~10-15 gold
- rare gold chest: ~20-30 gold
- more Gold Room opportunities
- more gold-bearing enemies
- more Ancient encounters with safe spawn caps

### Emeralds

Target roughly **2x effective normal emerald income**, with emphasis on visible world drops rather than wallet injection.

Current first-pass ideas:

- loot urns: 15-30 -> ~30-60
- camp emerald chest: 50 -> ~100 where applicable
- richer mob emerald bundles
- modest drop-frequency increase if needed
- preserve the usefulness of Prospector
- leave salvage rewards vanilla initially

## Documentation

The canonical project memory lives in [docs/](docs/README.md).

Important documents:

- [PROJECT_CHARTER.md](docs/PROJECT_CHARTER.md)
- [CURRENT_STATE.md](docs/CURRENT_STATE.md)
- [DECISIONS.md](docs/DECISIONS.md)
- [BALANCE_TARGETS.md](docs/BALANCE_TARGETS.md)
- [RESEARCH_FINDINGS.md](docs/RESEARCH_FINDINGS.md)
- [ARCHITECTURE.md](docs/ARCHITECTURE.md)
- [MULTIPLAYER.md](docs/MULTIPLAYER.md)
- [REUSE_AND_CREDITS.md](docs/REUSE_AND_CREDITS.md)
- [ROADMAP.md](docs/ROADMAP.md)
- [TEST_PLAN.md](docs/TEST_PLAN.md)
- [RESEARCH_LOG.md](docs/RESEARCH_LOG.md)
- [SOURCES.md](docs/SOURCES.md)

## Relationship to Minecraft-Dungeons-QoL

This repository is deliberately separate from **Minecraft-Dungeons-QoL**.

The QoL project remains focused on inventory quality-of-life such as favorites, locking and multi-salvage. This project may reuse its cooked-package tooling, graph helpers, inventory-item resolution, UI construction patterns and validation infrastructure, but the two mods should remain independently installable.

## Licensing

No project-wide license has been selected yet.

Third-party reuse must be documented in docs/REUSE_AND_CREDITS.md before distribution.
