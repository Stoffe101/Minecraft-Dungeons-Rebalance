# Read-only native declaration collector

Research prototype for the current Dungeons upgrade implementation blocker. **This does not enable Camp NPCs, paid upgrades or a Unique picker. Dungeons Store compatibility is not yet established.**

## Run the compiled capture bundle

1. Extract the bundle into a new folder on your Desktop, outside the game installation. Do not install it in `~mods`.
2. Start Minecraft Dungeons normally and leave it at Camp. Keep only one Dungeons process running.
3. Open normal Windows x64 PowerShell in the extracted folder and run:

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\Capture-NativeContracts.ps1
   ```

4. Return the `native-contracts-<timestamp>.zip` that the command prints. Return it even if capture is incomplete; the failure report helps identify which step blocked it.

The runner downloads a checksum-pinned Microsoft .NET 8 runtime into its own folder if needed. It runs synthetic/self-process checks before attempting game reads. There is no game loader, injection, driver, game-folder installer or gameplay/save call. If access is denied, stop; do not run as administrator or attempt a protection bypass. The previous UE4SS probe remains withdrawn.

## What is collected

Only allowlisted native class, function and property declarations: names, types/targets, sizes, offsets and flags. No executable bytes, complete memory dumps, wallet balances, item values, save records, cloud IDs or process addresses are written to the report. The reader inspects PE headers and eligible non-executable module data privately to discover tables; those bytes are not exported.

The Windows handle requests exactly `PROCESS_QUERY_INFORMATION | PROCESS_VM_READ` (`0x0410`). The imported native APIs are OpenProcess, ReadProcessMemory, VirtualQueryEx and CloseHandle. Limits are 60 seconds of read work, 128 MiB of attempted reads, 2,000,000 calls, 1 MiB per read, 16 MiB per eligible section, bounded name lengths, object counts and field chains. Inaccessible reads, unsupported layouts, missing or ambiguous controls stop collection with an incomplete report.

The parser's UE4.22 legacy name/UObject/UField/UProperty layouts are **candidates**, not certified Store offsets. Names and object indices are checked first. A UFunction header is inferred uniquely from four independently observed profile getter shapes, then six profile call contracts must validate, including qualified Guid/character-save/character-slot return types and input direction. It does not use a guessed GNames/GObjects address or a fixed UFunction header offset.

Limitations: both inline and pointer-backed 128/256-chunk legacy name tables are considered, with bounded ANSI/UTF-16 names; class members are direct declarations with superclass names, not flattened inherited methods. Array/map/enum nested type details and native function bodies are not decoded. Missing classes are reported. A declaration may reveal a usable API, but it cannot prove selected-result behavior, charging, persistent item identity, rollback or replication. `Completed=true` means declaration collection and control validation only; `UpgradeSemanticsVerified` always remains false.

## Build and tests

```powershell
dotnet build tools/NativeContractReader/NativeContractReader.csproj -c Release -o .tools/NativeContractReader
dotnet .tools/NativeContractReader/NativeContractReader.dll --self-test
```

The project has no external package dependencies. Self-tests use project-owned synthetic metadata, including complete table discovery at two different function-header positions and deliberate corruption. On Windows, one additional test reads an allocated marker from the collector's own process. CI also checks PowerShell parsing, no-game incomplete reporting and preservation of an existing report. These tests establish tooling behavior, not retail game compatibility.

Source and provenance: see `docs/NATIVE_DECLARATION_RESEARCH.md` in the repository. No third-party dumper source or game source is bundled.

## v2 discovery correction

The first retail capture successfully read 6,345,728 bytes in nine calls, then stopped at the broad raw-pointer candidate limit before any name table validation. v2 sorts candidates and uses cached VirtualQueryEx region descriptions to reject free/reserved/guarded/executable regions and candidates whose complete table header crosses a region boundary. The 200,000 live-data candidate bound remains; raw collection has a separate 1,000,000-entry workspace limit, and region queries have a 4,096-call/time bound. No extra process rights are requested. Reports now include query counts and sanitized discovery diagnostics. A regression fixture supplies over 210,000 unrelated numeric candidates while still discovering the six control declarations. Retail name/layout/upgrade compatibility remains unverified until a new capture.

## v3 source-grounded layout support

v2 passed its filtering stage but found no names. Related QoL retail evidence passed names with its 256-chunk revision. Pinned Epic-authored UE4.22.3 NameTypes.h defines a 4M-entry maximum with 16,384-entry chunks, giving 256 pointer slots. v3 considers both 128 and 256 capacities and inline/pointer-backed tables. It also permits reserved name chunks beyond the live count, alternate name text/child/property-target layouts already investigated in same-owner QoL, and reserved object capacity independently from live objects. Only a unique combination matching the six independent controls is accepted. Engine UObjectArray.h preallocation can reserve 33 chunks; only chunks containing live objects are traversed.

Byte and time limits remain 128 MiB and 60 seconds, region-query limit remains 4,096. The read-call ceiling is now 2,000,000 to accommodate both header candidates plus the independently bounded live-object walk; this does not increase permitted bytes/time. Live-object bound remains 1,000,000, and reserved capacity is bounded separately at 4 Mi elements/64 chunks. Reports record the accepted name capacity/text offset, child/target layout and function header only after independent validation. This is not native upgrade-semantic verification.
