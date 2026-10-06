param([string]$OutputDirectory, [string]$DotNetPath)
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'EvidenceCommon.ps1')
if ([Environment]::OSVersion.Platform -ne [PlatformID]::Win32NT -or -not [Environment]::Is64BitProcess) {
    throw 'Use Windows x64 PowerShell.'
}
$root = Get-ProjectRoot
$binary = Join-Path $root '.tools/NativeContractReader/NativeContractReader.dll'
if (-not (Test-Path $binary)) { throw 'Use the compiled native collector bundle; NativeContractReader.dll is missing.' }
if (-not $OutputDirectory) { $OutputDirectory = Join-Path $root ('native-contracts-' + [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss')) }
$out = [System.IO.Path]::GetFullPath($OutputDirectory)
if ((Test-Path $out) -or (Test-Path "$out.zip")) { throw 'Choose a fresh output directory.' }
New-Item -ItemType Directory -Path $out | Out-Null
$issues = New-Object System.Collections.Generic.List[string]
try {
    if (-not $DotNetPath) {
        $cfg = Get-Content (Join-Path $root 'config/evidence-tool.json') -Raw | ConvertFrom-Json
        $runtime = Join-Path $root ('.tools/native-dotnet-' + $cfg.runtimeVersion)
        $DotNetPath = Join-Path $runtime 'dotnet.exe'
        if (-not (Test-Path $DotNetPath)) {
            $archive = "$runtime.zip"
            Invoke-WebRequest $cfg.runtimeUrl -UseBasicParsing -OutFile $archive
            if ((Get-FileHash $archive -Algorithm SHA512).Hash.ToLowerInvariant() -ne $cfg.runtimeSha512) { throw 'Runtime checksum mismatch.' }
            Expand-Archive $archive $runtime
        }
    }
    $code = Invoke-EvidenceProcess $DotNetPath @($binary, '--self-test') (Join-Path $out 'SelfTest.log')
    if ($code -ne 0) { throw 'Collector self-test failed; no Dungeons capture attempted.' }
    Write-Host 'Leave Minecraft Dungeons at Camp. Reading declarations only; no upgrades or save calls are executed.'
    $code = Invoke-EvidenceProcess $DotNetPath @($binary, '--capture', (Join-Path $out 'NativeContracts.json')) (Join-Path $out 'Capture.log')
    if ($code -ne 0) { $issues.Add('Native declaration capture incomplete; report/log retained. Do not elevate or install a protection fallback.') }
} catch { $issues.Add($_.Exception.Message) }
[ordered]@{
    schema = 'rebalance-native-capture-runner-v1'
    collectedUtc = [DateTime]::UtcNow.ToString('o')
    issues = @($issues.ToArray())
    upgradeFeaturesEnabled = $false
    note = 'Research only. Declaration capture does not certify payment, item mutation, persistence, or Dungeons runtime compatibility.'
} | ConvertTo-Json | Set-Content (Join-Path $out 'REPORT.json')
Compress-Archive -Path (Join-Path $out '*') -DestinationPath "$out.zip"
Write-Host "Capture archive: $out.zip"
if ($issues.Count -gt 0) { Write-Warning ($issues -join '; '); exit 1 }
exit 0
