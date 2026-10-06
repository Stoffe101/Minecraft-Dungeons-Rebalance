# Balance targets

These numbers are **first-pass tuning targets**, not final promises. Retail playtesting should decide the final values.

## Design philosophy

The mod should make rewards feel generous enough that players engage with merchants and upgrades instead of hoarding forever, while still preserving reasons to run missions.

The target is not "everything costs nothing." It is "playing the game funds the game's systems at a satisfying rate."

---

## Gold

### Vanilla reference points

Current documented vanilla values:

| Source | Vanilla |
| --- | ---: |
| Ancient Hunt completion | 30 gold |
| Normal Gold Chest | 4-6 gold |
| Rare Gold Chest | 8-10 gold |

Gold also drops from mobs during Ancient Hunts.

### Rebalance target

A complete Ancient Hunt should broadly land around:

| Run quality | Target |
| --- | ---: |
| Short / unlucky | ~100-150 gold |
| Typical full run | ~150-220 gold |
| Lucky / exploration-heavy | ~250-300 gold |

This should remain variable. We do not want a fixed payout per run.

### First-pass tuning

| Source | First-pass target |
| --- | ---: |
| Ancient Hunt completion | ~50 gold |
| Normal Gold Chest | ~10-15 gold |
| Rare Gold Chest | ~20-30 gold |
| Gold Rooms | More opportunities than vanilla |
| Mob gold | More frequent and/or larger bundles |

The final average must be measured from real runs rather than inferred only from individual reward values.

---

## Emeralds

### Vanilla reference points

Current documented values include:

- Loot Urn: 15-30 emeralds.
- Camp Emerald Chest: 50 emeralds.

### Rebalance target

Aim for approximately **2x effective natural emerald income** across ordinary play.

"2x effective" does not mean every emerald-related number must literally be doubled.

### First-pass tuning

| Source | Vanilla | First-pass target |
| --- | ---: | ---: |
| Loot Urn | 15-30 | ~30-60 |
| Camp Emerald Chest | 50 | ~100 |
| Mob bundles | vanilla | richer bundles |
| Mob drop frequency | vanilla | modest increase only if needed |
| Salvage | vanilla | keep vanilla initially |

### Prospector protection

Prospector should remain worthwhile.

Therefore:

1. Increase bundle size before heavily increasing base proc frequency.
2. Measure Prospector vs non-Prospector income separately.
3. Avoid making emerald drops so common that an emerald build has no identity.

---

## Camp smith pricing

### Uniquesmith / rarity services

| Service | Initial price |
| --- | ---: |
| Common -> Rare | 750 emeralds |
| Rare -> Unique | 2,500 emeralds |

Important research question: the Tower Uniquesmith natively converts gear to a Unique variant. Common -> Rare may require a separate mutation path.

If a base item has multiple Unique variants, the user must choose the exact outcome from native names/icons/details before confirmation. The agreed Rare -> Unique price remains 2,500 emeralds; no extra selection surcharge is agreed.

### Gildsmith

| Service | Initial price |
| --- | ---: |
| Add Gilded | 150 gold |
| Reroll / improve Gilded enchantment | 250 gold |

The native Tower Gildsmith applies a random gilded enchantment and random tier.

We should first preserve that behavior rather than inventing a deterministic best-in-slot machine.

### Powersmith

Price is **TBD**.

Research first:

- exact native power-upgrade behavior
- power cap rules
- current item power scaling
- whether repeated use should increase cost
- interaction with Blacksmith upgrades

---

## Economy relationship

The economy should roughly support this rhythm:

1. Run ordinary missions -> gain enough emeralds to gamble, shop and occasionally upgrade rarity.
2. Run Ancient Hunts -> earn enough gold that gilding/rerolling is attainable.
3. Spend currency at Camp -> create a reason to keep playing.
4. Continue chasing drops because smiths improve progression but do not replace all loot hunting.

## Metrics to collect during testing

For at least 10 runs per test configuration:

- gold per Hunt
- number of Gold Rooms
- normal/rare gold chest count
- gold from mobs
- run duration
- Ancients encountered
- deaths / difficulty perception
- emeralds per normal mission
- emeralds with and without Prospector
- smith uses affordable per hour of normal play


## Measured-input correction and implemented first pass

Actual base Loot Urn package amount range is **3–7**, not the earlier community estimate of 15–30. First-pass patch doubles its serialized range to **6–14**. Native field/bundle semantics and child override coverage need retail verification before expressing that as exact final emeralds. Normal/rare chest defaults directly confirm 4–6 and 8–10 and are patched to 10–15 and 20–30. Completion stays unchanged in this first test subset. The 150–220/250–300 Hunt totals, approximately 2x overall emerald income and +50–75% Gold Room opportunities remain unmeasured design targets.

## Current v2 bindings

Camp emerald chest native EmeraldsReward=50 is now patched to the agreed 100; availability remains native. On October 6, the user confirmed the in-game reward is 100 emeralds instead of 50. The October 6 goal is a higher chance of one or more Ancient encounters, with no guaranteed minimum; its probability/count control is not implemented in v2. Full gold/run and effective emerald-income goals remain unmeasured.
