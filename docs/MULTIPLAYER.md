# Multiplayer goals

Multiplayer behavior is a first-class requirement, not a post-release extra.

## Desired player experience

If a party runs a Rebalanced Ancient Hunt:

- all players see the same additional encounters
- all players see stable mission progression
- richer chests behave consistently
- gold rewards do not encourage teammates to race each other
- no client receives duplicate rewards
- no client is silently denied mission completion rewards

## Gold-sharing goal

Preferred behavior:

> A gold reward earned by the party should benefit each connected hero appropriately.

The exact implementation depends on the game's native network model.

Possible implementations, in order of preference:

1. Reuse a native shared-currency award path.
2. Make gold pickups use the same party-sharing behavior as a known shared reward.
3. Host detects a valid gold award and explicitly grants equivalent reward to other heroes.
4. Require all clients to install the mod if safe host-only behavior cannot be established.

Do not assume host-only support until tested.

## Host authority

Ancient Hunt generation changes should ideally be host authoritative.

Questions:

- Does the host alone determine hunt-room generation?
- Are extra spawned mobs/chests replicated automatically?
- Do modified reward values come from host authority or local data?
- What happens when host and client have different .pak sets?

## Exploit risks

Explicitly test:

- one pickup awarding twice
- one reward per client plus another party grant
- reconnecting during a Hunt
- client joining after generation
- host migration/disconnect
- same gold chest opened nearly simultaneously
- mission completion triggered while a client is loading
- spectator/dead player reward state

## Compatibility targets

Test matrix should eventually include:

1. Solo.
2. Host + one modded client.
3. Host + multiple modded clients.
4. Modded host + unmodded client, if the game permits connection.
5. Unmodded host + modded client, for failure/compatibility characterization.

The supported release configuration should be the simplest one proven reliable. Do not claim host-only compatibility based on theory.
