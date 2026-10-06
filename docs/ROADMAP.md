# Roadmap

This roadmap is ordered to reduce risk. Do not jump directly to a combined production .pak before the native systems are understood.

## Phase 0 — Bootstrap

- [x] Create standalone repository.
- [x] Document project goals.
- [x] Document first-pass balance targets.
- [x] Confirm Better Ancient Hunt reuse permissions.
- [x] Record Tower smith research.
- [x] Define documentation rule.
- [ ] Decide project-wide license before public binary/source release.

## Phase 1 — Import proven tooling

- [ ] Audit Minecraft-Dungeons-QoL tooling for reusable components.
- [ ] Copy/extract only relevant package/graph/test infrastructure.
- [ ] Record exact reused files in REUSE_AND_CREDITS.
- [ ] Build a no-op/read-only diagnostic package.
- [ ] Verify Microsoft Store / Launcher installation path workflow.
- [ ] Verify original assets remain structurally intact after no-op patching.

## Phase 2 — Tower smith research

### Asset discovery

- [ ] Locate Powersmith actor/class.
- [ ] Locate Uniquesmith / Artisan assets.
- [ ] Locate Gildsmith / Gilder assets.
- [ ] Locate merchant-floor setup assets.
- [ ] Identify interaction widgets and dependencies.
- [ ] Identify Tower-only state/context assumptions.

### Mutation contracts

- [ ] Identify native Unique conversion function/path.
- [ ] Confirm how multiple Unique variants are selected.
- [ ] Identify native Gild mutation path.
- [ ] Confirm gilded enchant/tier generation.
- [ ] Identify native Powersmith upgrade path.
- [ ] Determine whether Uniquesmith also normalizes power in current retail.
- [ ] Find safe Common -> Rare mutation path.
- [ ] Identify save/refresh notifications after mutation.

## Phase 3 — Camp NPC proof of concept

- [ ] Spawn one Tower smith safely in Camp.
- [ ] Verify animations/audio.
- [ ] Verify interaction prompt.
- [ ] Verify no collision with existing Camp state.
- [ ] Verify restart/reload behavior.
- [ ] Repeat for all three smiths.
- [ ] Select final Camp placement.

## Phase 4 — Paid smith services

- [ ] Read player emerald balance natively.
- [ ] Read player gold balance natively.
- [ ] Validate price before mutation.
- [ ] Common -> Rare at 750 emeralds.
- [ ] Rare -> Unique at 2,500 emeralds.
- [ ] Add Gilded at 150 gold.
- [ ] Gilded reroll/improvement at 250 gold.
- [ ] Define Powersmith price model.
- [ ] Ensure failed mutations never consume currency.
- [ ] Preserve item enchantments/state where native behavior should.
- [ ] Add clear price/result UI.

## Phase 5 — Better Ancient Hunt analysis

- [ ] Obtain Better Ancient Hunt v1.0 file.
- [ ] Hash/archive research input.
- [ ] Diff modified assets against vanilla.
- [ ] Map longer-path changes.
- [ ] Map mob-density changes.
- [ ] Map second Ancient wave.
- [ ] Map raid captain additions.
- [ ] Identify any direct gold changes.
- [ ] Decide exact pieces to reuse vs reimplement.

## Phase 6 — Ancient Hunts+

- [ ] Increase completion reward toward 50.
- [ ] Increase normal Gold Chest to ~10-15.
- [ ] Increase Rare Gold Chest to ~20-30.
- [ ] Increase Gold Room opportunities safely.
- [ ] Add more mobs with caps.
- [ ] Add more Ancient encounters.
- [ ] Add/reuse extra Ancient wave.
- [ ] Evaluate raid captains.
- [ ] Stress-test generated Hunts for crashes/softlocks.
- [ ] Measure real gold/run across at least 10 runs.

## Phase 7 — Emerald Economy+

- [ ] Locate Loot Urn reward definition.
- [ ] Tune 15-30 -> ~30-60.
- [ ] Locate Camp Emerald Chest reward.
- [ ] Tune 50 -> ~100 where appropriate.
- [ ] Locate mob emerald bundle values.
- [ ] Increase bundle size.
- [ ] Measure whether drop-frequency change is needed.
- [ ] Compare Prospector vs non-Prospector.
- [ ] Keep salvage vanilla for first pass.

## Phase 8 — Multiplayer rewards

- [ ] Characterize vanilla emerald sharing.
- [ ] Characterize vanilla gold pickup ownership.
- [ ] Test host authority for Hunt generation.
- [ ] Test extra chests/mobs with clients.
- [ ] Prototype party gold sharing.
- [ ] Prevent duplicate awards.
- [ ] Test modded/unmodded host-client combinations.
- [ ] Document supported multiplayer configuration.

## Phase 9 — Combined retail build

- [ ] Combine validated Camp Smiths + Hunts + Emerald changes.
- [ ] Full regression test.
- [ ] Test at common UI scales/resolutions.
- [ ] Test solo and co-op.
- [ ] Test save/reload.
- [ ] Test multiple consecutive Hunts.
- [ ] Verify no conflict with Minecraft-Dungeons-QoL.
- [ ] Produce install documentation.
- [ ] Produce release notes and credits.

## Future ideas

Only after the core rebalance is stable:

- configurable balance presets
- optional harsher Ancient Hunt difficulty preset
- optional Unique variant choice
- smith price scaling by power
- additional meaningful gold sinks
- additional emerald sinks
- Camp visual dressing around the Tower smith area


## Immediate dependency gate (2026-10-05)

Collector implementation is complete for the known targets; retail collection is pending. Obtain its output and Better Ancient Hunt PAK, then proceed with native-field research, gameplay patching, packaging and runtime validation. No gameplay milestone is marked complete by collector tests.


## Superseding milestone update

Initial source collection received. Implemented and structurally validated HuntsEconomy-Test-v1; retail testing is next for this subset. Native Camp services, full economy coverage/completion rewards, explicit Gold Room weighting and shared-gold contracts are still open. Collect measurable run totals and arena timing before expanding/tuning. No full-design completion milestone is closed.

## Latest continuation priorities

Camp emerald chest scalar 50 -> 100 implemented and structurally validated in v2. Guaranteed random Ancient minimum added as a new requirement; it needs the separate hypermission configuration and offering/door-selection evidence, not another copy of the initial collection. Run supplemental collector, establish the authoritative generation contract, implement guaranteed random fallback/reservation and execute the acceptance matrix. Continue native paid merchant and shared-gold research; neither milestone is completed.

## Supplemental / upgrade-choice continuation

Supplemental Ancient sources received and inspected. Upgrade option/pricing/confirmation models started and tested; no native adapter, Camp UI or upgraded-item gameplay milestone closed. Exact native mutation and generator contracts are the next gate.

## October 6 revision: Ancient selection

Guaranteed random Ancient minimum is removed from active scope. Target a higher chance of one or more native Ancient encounters; zero remains possible. Keep the implemented extra waves. Identify a verified native probability/count control, compare matched offerings before/after, and measure real encounter counts; do not call extra waves, broader rune eligibility or a chance-label edit a probability increase. Paid Camp upgrades and shared gold remain unchanged goals.

## October 6 diagnostic correction

Direct Drive source is available; eight archives inspected so far. Full contents are not certified, and the largest archives exceed the connector transfer cap. The related UE4SS test crashed on retail; Rebalance's probe is withdrawn/disabled, not the next user test. Establish a compatible developer diagnostic/bridge before native calls, while continuing cooked asset research through supported transfers. See DRIVE_GAME_RESEARCH.md.
