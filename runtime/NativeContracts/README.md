# Withdrawn diagnostic source

Do not install UE4SS to run this probe on the user's Store installation. The related QoL UE4SS 3.0.1 test crashed at startup. Its supplied evidence has no generated headers, object dump or completed capture. The exact native fault is not established.

The Lua source is disabled by default and retained for historical API-shim tests. This does **not** prevent or repair UE4SS's native startup hooks: they execute before the Lua mod can disable itself. No installer, loader DLL or runtime dependency is bundled with Rebalance. The gameplay v2 PAK does not use this probe.

Any future runtime diagnostic route must first establish compatibility in a developer environment. Do not re-enable this source, guess engine/signature settings or repeat installation on the user's game as the next step. See docs/DRIVE_GAME_RESEARCH.md and docs/NATIVE_RUNTIME.md.
