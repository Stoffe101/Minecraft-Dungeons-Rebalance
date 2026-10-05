# Test plan

## Test philosophy

Economy/item mutation mods can appear correct while silently corrupting state or duplicating currency.

Every destructive or economy-changing feature needs:

- precondition test
- success test
- failure test
- save/reload test
- co-op test where relevant

## Baseline regression

Before feature testing:

- launch game normally
- load hero
- enter Camp
- open inventory
- start ordinary mission
- return to Camp
- start Ancient Hunt
- finish/exit Hunt
- close/relaunch game

No feature should break unrelated vanilla flows.

---

## Camp smith placement

For each Tower smith:

- NPC appears in intended Camp location.
- No overlap with vanilla NPCs/props.
- No blocking of player navigation.
- Correct idle animation.
- Correct interaction prompt.
- Correct audio where available.
- Interaction works after mission return.
- Interaction works after full game restart.
- Multiple players do not create duplicate NPC instances.

## Common -> Rare

- Common supported gear can be upgraded.
- Correct 750 emerald price is shown.
- Insufficient funds blocks action.
- Failed/stale item blocks action without spending.
- Result remains same base gear family.
- Enchantments/state are preserved according to design.
- Favorite/QoL metadata is not assumed to exist.
- Save/reload preserves result.
- Equipped and storage edge cases tested.

## Rare -> Unique

- Rare supported gear can be converted.
- Correct 2,500 emerald price.
- Insufficient funds blocks safely.
- Native Unique variant behavior documented.
- Multi-variant families tested.
- Unique property/name/appearance valid.
- No duplicate/lost item.
- Power behavior documented.
- Save/reload valid.

## Gildsmith

- Eligible item can be gilded for 150 gold.
- Gilded enchantment is valid for item category.
- Gilded tier is valid.
- Insufficient gold blocks safely.
- Failed action does not spend.
- Existing enchantments remain valid.
- Enchantment points do not become negative.
- Save/reload preserves gilded metadata.

## Gilded reroll/improvement

- Only valid gilded items are accepted.
- 250 gold price.
- Result has exactly one valid intended built-in gilded enchant.
- No duplicate hidden gild data.
- Failure does not spend.
- Repeated use remains stable.

## Powersmith

Final cases depend on native behavior research.

At minimum:

- power increases legally
- cap rules obeyed
- price displayed
- no downgrade
- no invalid over-cap item
- enchantments preserved
- save/reload valid

---

## Ancient Hunts+

### Generation

Across at least 20 generated Hunts:

- all three sub-missions generate.
- portals/doors remain reachable.
- Gold Rooms reachable.
- Ancient rooms reachable.
- no softlocked doors.
- no missing completion exit.
- extra waves complete correctly.

### Density / stability

Stress cases:

- multiple minibosses in one encounter
- extra Ancient wave
- raid captains
- four-player co-op
- high particle/effect builds
- repeated Hunts without restart

Record:

- crash
- severe hitch
- AI stall
- mission objective stall
- enemy stuck outside arena
- excessive encounter duration

### Gold measurement

For at least 10 representative full runs record:

- completion gold
- Gold Room count
- normal Gold Chest count
- Rare Gold Chest count
- mob gold
- total gold
- run duration

Pass target:

- typical runs broadly near 150-220
- lucky/exploration runs can reach ~250-300
- unlucky runs are not routinely near vanilla ~50-ish experience

---

## Emerald Economy+

For at least 10 ordinary missions:

- emeralds from urns
- emeralds from mobs
- emeralds from other containers
- total emeralds
- duration

Repeat comparable tests with Prospector.

Pass target:

- effective normal income approximately 2x baseline
- Prospector still gives a meaningful advantage
- no absurd screen-filling pickup spam
- no wallet desync

---

## Multiplayer

Test at minimum:

### Two modded players

- modded host + modded client
- extra Hunt rooms/mobs visible to both
- chest state synchronized
- Ancient wave synchronized
- mission completion synchronized
- currency totals correct

### Host/client mismatch research

- modded host + unmodded client
- unmodded host + modded client

Do not publish these as supported configurations until proven.

### Shared gold

- host collects
- client collects
- simultaneous collection attempts
- dead player present
- joining late
- disconnect/reconnect

No player should receive unintended duplicate gold.

---

## Compatibility with Minecraft-Dungeons-QoL

With both mods installed:

- inventory opens
- favorites/selection UI still renders
- multi-salvage still works
- smith item selection works
- smith mutations do not confuse physical-item identity
- no duplicate patched function collisions
- Ancient Hunts unaffected by inventory UI patches

Neither mod should require the other.
