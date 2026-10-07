# Private Camp smith asset stager

Build with .NET 8. The base mode creates nine owned actor/root/content package pairs; placement/interaction/upgrade modes create ten pairs including the Camp chest hook. All original input hashes are preserved and every output is reopened and compared semantically. Raw retail inputs/outputs stay outside git.

```text
dotnet build tools/CampSmithStager -c Release
dotnet CampSmithStager.dll --upgrade-test <private-PatchSources-root> <fresh-private-stage>
python scripts/build_camp_preview.py --upgrade-test --baseline <accepted-v2-build> --smiths <private-stage> --output <fresh-build> --packager <pinned-u4pak.py>
```

`--upgrade-test` enables native Camp interactions, eight controlled action widget templates, native upgrade/collection/choice APIs and success-guarded amount-1 currency deductions. It disables the two one-transaction actor flags only in this mode. It is an unverified gameplay prototype; production policy, custom picker completeness and persistence are not implemented/verified by these tests. See [the complete v6 guide](../../docs/CAMP_UPGRADE_TEST_V6.md).

`--placement-preview` disables NPC interaction. `--interaction-preview` enables read-only UI but blocks all eleven stock transaction-bearing configurations. Base mode has no spawn hook. Original Tower assets are unchanged. All modes retain native Function archetypes and creation preloads; every authored Boolean serializes one native byte after the v5 crash correction.

Private integration checks:

```text
python tests/test_camp_smiths.py --dotnet <dotnet> --stager <CampSmithStager.dll> --source <private-PatchSources-root> --crashed-stage <retained-v3-stage> --crashed-v5-stage <retained-v5-stage> -v
dotnet CampSmithStager.dll --self-test-paid-gateway <private-PatchSources-root>
dotnet CampSmithStager.dll --self-test-paid-buttons <private-PatchSources-root>
```

Fifteen integration tests include deliberate native payment/button rejection cases and the actual crashed v3/v5 asset checks. These execute no game functions. Other self-test modes cover presentation, physical item readers, merchant screen dispatch, placement, affordability and function loading. A correct asset round trip cannot establish engine loading or gameplay success.
