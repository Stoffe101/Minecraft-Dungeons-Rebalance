using System.Runtime.Loader;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using CUE4Parse.FileProvider;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Versions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.Kismet;
using UAssetAPI.UnrealTypes;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (args.Length == 3 && args[0] == "--asset")
            {
                return DumpAsset(args[1], args[2], Path.GetFileName(args[1])) ? 0 : 1;
            }
            if (args.Length == 2 && args[0] == "--validate-targets")
            {
                ReadTargets(args[1]);
                Console.WriteLine("Target manifest validated.");
                return 0;
            }
            if (args.Length != 6 || args[0] != "--paks")
                throw new ArgumentException("Usage: --asset <uasset> <output-json> OR --validate-targets <manifest> OR --paks <paks> <aes-key> <new-output-directory> <inspector-libraries> <target-manifest>");
            var libraries = Path.GetFullPath(args[4]);
            AssemblyLoadContext.Default.Resolving += (_, name) =>
            {
                var path = Path.Combine(libraries, name.Name + ".dll");
                return File.Exists(path) ? AssemblyLoadContext.Default.LoadFromAssemblyPath(path) : null;
            };
            return DumpPaks(args[1], args[2], args[3], libraries, args[5]);
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }

    private static bool DumpAsset(string path, string output, string packagePath)
    {
        var asset = new UAsset(path, EngineVersion.VER_UE4_22);
        KismetSerializer.asset = asset;
        var errors = new List<string>();
        var exports = new List<object>();
        for (var i = 0; i < asset.Exports.Count; i++)
        {
            var export = asset.Exports[i];
            var entry = new Dictionary<string, object?>
            {
                ["index"] = i + 1, ["name"] = export.ObjectName.ToString(),
                ["type"] = export.GetType().Name, ["outer"] = export.OuterIndex.Index,
                ["class"] = export.ClassIndex.Index, ["super"] = export.SuperIndex.Index
            };
            try
            {
                if (export is NormalExport normal)
                    entry["data"] = JToken.Parse(asset.SerializeJsonObject(normal.Data, true));
                if (export is PropertyExport property && property.Property != null)
                    entry["property"] = JToken.Parse(asset.SerializeJsonObject(property.Property, true));
                if (export is ClassExport cls)
                {
                    entry["superStruct"] = cls.SuperStruct.Index;
                    entry["children"] = cls.Children.Select(x => x.Index).ToArray();
                    entry["classDefaultObject"] = cls.ClassDefaultObject.Index;
                    entry["flags"] = cls.ClassFlags.ToString();
                }
                if (export is FunctionExport function)
                {
                    entry["flags"] = function.FunctionFlags.ToString();
                    entry["children"] = function.Children.Select(x => x.Index).ToArray();
                    entry["script"] = function.ScriptBytecode is { Length: > 0 }
                        ? KismetSerializer.SerializeScript(function.ScriptBytecode) : null;
                }
            }
            catch (Exception ex) { errors.Add($"Export {i + 1} {export.ObjectName}: {ex.Message}"); }
            exports.Add(entry);
        }
        var report = new
        {
            schemaVersion = 1, packagePath, engine = "UE4_22", objectVersion = (int)asset.ObjectVersion,
            propertyCount = asset.Exports.OfType<PropertyExport>().Count(),
            functionCount = asset.Exports.OfType<FunctionExport>().Count(),
            imports = asset.Imports.Select((x, i) => new { index = -i - 1, name = x.ObjectName.ToString(),
                className = x.ClassName.ToString(), classPackage = x.ClassPackage.ToString(), outer = x.OuterIndex.Index }),
            exports, errors,
            note = "Legacy property metadata, imports and Kismet only. Includes tagged export defaults for reward and replication inspection; excludes opaque buffers. Native ABI and runtime behavior are not certified."
        };
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(output))!);
        File.WriteAllText(output, JsonConvert.SerializeObject(report, Formatting.Indented));
        Console.WriteLine($"{packagePath}: {report.propertyCount} properties, {report.functionCount} functions, {errors.Count} export errors");
        return errors.Count == 0;
    }

    private static string[] ReadTargets(string manifest)
    {
        var json = JObject.Parse(File.ReadAllText(manifest));
        if ((int?)json["schemaVersion"] != 1) throw new InvalidDataException("Unsupported target manifest version.");
        var packages = json["packages"]?.ToObject<string[]>() ?? throw new InvalidDataException("Missing packages.");
        var data = json["dataFiles"]?.ToObject<string[]>() ?? throw new InvalidDataException("Missing dataFiles.");
        if (packages.Length == 0 || data.Length == 0) throw new InvalidDataException("Empty target group.");
        foreach (var path in packages.Concat(data))
        {
            if (!path.StartsWith("Dungeons/Content/", StringComparison.Ordinal)
                || path.Contains('\\') || path.Contains(':') || path.Split('/').Any(x => x is "" or "." or "..")
                || !Regex.IsMatch(path, @"^[A-Za-z0-9_./-]+$"))
                throw new InvalidDataException("Invalid target path: " + path);
        }
        if (packages.Any(x => !x.EndsWith(".uasset", StringComparison.Ordinal))
            || data.Any(x => !x.EndsWith(".json", StringComparison.Ordinal)))
            throw new InvalidDataException("Only uasset and JSON targets are permitted.");
        var targets = packages.Concat(data).ToArray();
        if (targets.Length > 128 || targets.Distinct(StringComparer.OrdinalIgnoreCase).Count() != targets.Length)
            throw new InvalidDataException("Too many or duplicate targets.");
        return targets;
    }

    private static int DumpPaks(string paks, string key, string output, string libraries, string manifest)
    {
        var targets = ReadTargets(manifest);
        output = Path.GetFullPath(output);
        paks = Path.GetFullPath(paks);
        if (!Directory.Exists(paks)) throw new DirectoryNotFoundException("Paks directory does not exist.");
        var gameRoot = Directory.GetParent(paks)?.Parent?.FullName ?? paks;
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (output.Equals(gameRoot, comparison) || output.StartsWith(gameRoot + Path.DirectorySeparatorChar, comparison))
            throw new InvalidOperationException("Output must be outside the game directory.");
        if (Directory.Exists(output) || File.Exists(output)) throw new IOException("Output already exists. Choose a new directory.");
        if (OperatingSystem.IsWindows())
        {
            CUE4Parse.Compression.ZlibHelper.Initialize(Path.Combine(libraries, "zlib-ng2.dll"));
            CUE4Parse.Compression.OodleHelper.Initialize(Path.Combine(libraries, "oo2core_9_win64.dll"));
        }
        // Top-level game archives only; do not mount installed ~mods overlays.
        using var provider = new DefaultFileProvider(new DirectoryInfo(paks), SearchOption.TopDirectoryOnly, null, null);
        provider.Versions.Game = EGame.GAME_UE4_22;
        provider.Versions.Ver = EGame.GAME_UE4_22.GetVersion();
        provider.Initialize();
        var aes = new FAesKey(key);
        foreach (var archive in provider.UnloadedVfs.ToArray()) provider.SubmitKey(archive.EncryptionKeyGuid, aes);
        provider.PostMount();
        var visible = provider.Files.Keys.ToDictionary(x => x.Replace('\\', '/'), x => x, StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();
        var completed = new List<string>();
        var sourceFiles = new List<object>();
        var temp = Path.Combine(Path.GetTempPath(), "MCDRebalance-evidence-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temp);
        Directory.CreateDirectory(output);
        try
        {
            File.WriteAllLines(Path.Combine(output, "AssetList.txt"), visible.Keys.OrderBy(x => x));
            File.Copy(manifest, Path.Combine(output, "TARGETS.json"));
            for (var i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                if (!visible.TryGetValue(target, out var virtualPath)) { errors.Add("Missing target: " + target); continue; }
                var local = Path.Combine(temp, i.ToString());
                Directory.CreateDirectory(local);
                try
                {
                    if (target.EndsWith(".json", StringComparison.Ordinal))
                    {
                        var bytes = provider.SaveAsset(virtualPath);
                        // Reject invalid/non-text data before preserving a level file.
                        JToken.Parse(System.Text.Encoding.UTF8.GetString(bytes).TrimStart('\uFEFF'));
                        Preserve(target, bytes);
                    }
                    else
                    {
                        foreach (var pair in provider.SavePackage(virtualPath))
                        {
                            var filename = Path.GetFileName(pair.Key.Replace('\\', '/'));
                            File.WriteAllBytes(Path.Combine(local, filename), pair.Value);
                        }
                        var input = Path.Combine(local, Path.GetFileName(target));
                        // Keep exact originals even if one metadata export fails.
                        foreach (var extension in new[] { ".uasset", ".uexp" })
                        {
                            var source = Path.ChangeExtension(input, extension);
                            if (File.Exists(source)) Preserve(Path.ChangeExtension(target, extension).Replace('\\', '/'), File.ReadAllBytes(source));
                            else if (extension == ".uasset") throw new IOException("Missing header.");
                        }
                        var name = i.ToString("D3") + "_" + Path.GetFileNameWithoutExtension(target) + ".json";
                        if (!DumpAsset(input, Path.Combine(output, "Metadata", name), target))
                            errors.Add("Incomplete metadata: " + target);
                    }
                    completed.Add(target);
                }
                catch (Exception ex) { errors.Add(target + ": " + ex.Message); Console.Error.WriteLine(errors.Last()); }
                finally { Directory.Delete(local, true); }
            }
        }
        finally { Directory.Delete(temp, true); }
        File.WriteAllText(Path.Combine(output, "EXPORT_REPORT.json"), JsonConvert.SerializeObject(new
        {
            schemaVersion = 1, engine = "UE4_22", targetCount = targets.Length, visibleFileCount = visible.Count,
            completed, errors, sourceFiles,
            gameArchives = Directory.GetFiles(paks, "*.pak").Select(x => new { name = Path.GetFileName(x), bytes = new FileInfo(x).Length }),
            note = "Private development inputs. No saves or executables read. Raw game packages must not be committed or redistributed."
        }, Formatting.Indented));
        Console.WriteLine($"Collected {completed.Count}/{targets.Length} targets; {errors.Count} issues.");
        return errors.Count == 0 ? 0 : 1;

        void Preserve(string relative, byte[] bytes)
        {
            var destination = Path.Combine(output, "PatchSources", relative.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.WriteAllBytes(destination, bytes);
            sourceFiles.Add(new { path = "PatchSources/" + relative, bytes = bytes.Length,
                sha256 = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant() });
        }
    }
}
