# Current state

Last updated: 2026-10-05

## Status

**Research/design bootstrap. No gameplay build yet.**

The repository and canonical docs are now initialized.

## Confirmed external research

### Ancient Hunt economy

Current community documentation records:

- Ancient Hunt completion: **30 gold**
- normal Gold Chest: **4-6 gold**
- Rare Gold Chest: **8-10 gold**
- gold is obtained from chests and mobs in Ancient Hunts
- hunt doors can lead to Gold Rooms containing piglin chests

Minecraft Dungeons version 1.17.0.0 previously increased these values from even lower numbers.

### Tower smiths

The Tower has three merchant floor variants:

- Powersmith — upgrades item power
- Uniquesmith — turns selected gear into a Unique variant
- Gildsmith — gilds selected gear

Known level IDs documented by the community include:

- Uniquesmith: Artisan
- Gildsmith: Gilder

### Better Ancient Hunt

Better Ancient Hunt by Onetoeisenough is a usable reference/reuse source.

The author documents these changes:

- longer side paths
- harder mobs, including minibosses
- more mobs
- a second Ancient wave containing two Ancients
- raid captains after gold and Ancient battles

The Nexus page explicitly allows:

- modification and release of improvements/bug fixes
- asset reuse without permission
- redistribution with creator credit where required by the upload rule

This project will still credit the author even where the permission wording does not require credit.

## Agreed first-pass balance direction

### Gold

Target a complete Ancient Hunt around:

- **typical:** 150-220 gold
- **lucky/exploration-heavy:** 250-300 gold

Initial values to test:

- completion: ~50 gold
- normal gold chest: ~10-15 gold
- rare gold chest: ~20-30 gold
- more Gold Room opportunities
- more gold-bearing enemies
- more Ancient encounters

### Emeralds

Target roughly **2x effective natural income**.

Initial direction:

- loot urn: 15-30 -> ~30-60
- Camp emerald chest: 50 -> ~100 where applicable
- richer mob emerald bundles
- modest frequency increase only if needed
- keep Prospector meaningful
- leave salvage rewards vanilla initially

### Smith prices

- Common -> Rare: 750 emeralds
- Rare -> Unique: 2,500 emeralds
- Add Gilded: 150 gold
- Gilded reroll/improvement: 250 gold
- Powersmith: TBD after native implementation research

## Known reusable work

Minecraft-Dungeons-QoL already contains useful infrastructure for:

- cooked package patching
- bytecode/graph construction
- inventory item resolution
- UI composition
- validation and structural tests

Reuse should be deliberate and documented.

## Major unknowns

- Exact cooked assets/functions used by all three Tower smiths.
- Whether Tower mutations operate unchanged on persistent Camp inventory.
- Best native/safe path for Common -> Rare.
- Exact Gold Room generation controls.
- Exact gold/emerald drop actor and reward functions.
- Host/client replication behavior for extra drops.
- Whether host-only installation can benefit unmodded clients.
- Safe upper bound for extra minibosses/Ancients.
- Best Camp placement without collisions with existing content.
