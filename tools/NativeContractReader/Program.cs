using System.Buffers.Binary;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

if (args.SequenceEqual(new[] { "--self-test" })) { Fixtures.Run(); return 0; }
if (args.Length != 2 || args[0] != "--capture") {
    Console.Error.WriteLine("Usage: NativeContractReader --capture <fresh-report.json> | --self-test"); return 2;
}
var output = Path.GetFullPath(args[1]);
if (File.Exists(output) || Directory.Exists(output)) { Console.Error.WriteLine("Output already exists."); return 2; }
var report = new CaptureReport();
try {
    if (!OperatingSystem.IsWindows() || !Environment.Is64BitProcess)
        throw new InvalidOperationException("Capture requires Windows x64. No capture was attempted.");
    var processes = Process.GetProcesses().Where(p => p.ProcessName is "Dungeons" or "Dungeons-Win64-Shipping").ToArray();
    try {
        if (processes.Length != 1) throw new InvalidOperationException("Run exactly one Minecraft Dungeons process and leave it at Camp.");
        using var memory = new ProcessMemory(processes[0].Id);
        try {
            var module = processes[0].MainModule ?? throw new InvalidOperationException("Main module is unavailable.");
            var regions = ModuleData.Read(memory, (ulong)module.BaseAddress.ToInt64());
            var reflection = Reflection.Discover(memory, regions, report.Discovery);
            report.FunctionHeaderOffset = reflection.FunctionHeaderOffset;
            report.FunctionNumParmsDelta = reflection.FunctionNumParmsDelta;
            report.ChildrenOffset = reflection.ChildrenOffset;
            report.PropertyTargetOffset = reflection.PropertyTargetOffset;
            report.NameCharactersOffset = reflection.NameCharactersOffset;
            report.NameChunkCapacity = reflection.NameChunkCapacity;
            report.Classes = reflection.Capture();
            Contracts.Validate(report.Classes);
            report.ControlContractsPassed = true;
            report.Completed = true;
            report.MissingAllowlistedClasses = Reflection.Allowlist.Except(report.Classes.Select(c => c.Name)).ToArray();
        } finally { report.BytesRead = memory.BytesRead; report.ReadCalls = memory.ReadCalls; report.RegionQueries = memory.RegionQueries; }
    } finally { foreach (var process in processes) process.Dispose(); }
} catch (Exception error) { report.Error = error.Message; }
Directory.CreateDirectory(Path.GetDirectoryName(output)!);
// CreateNew prevents an existing report from being replaced even if it appeared during capture.
using (var stream = new FileStream(output, FileMode.CreateNew, FileAccess.Write))
    JsonSerializer.Serialize(stream, report, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(report.Completed ? "Declarations captured; native upgrade semantics are NOT verified." : "Capture incomplete: " + report.Error);
Console.WriteLine(output);
return report.Completed ? 0 : 1;

sealed class CaptureReport {
    public string Schema { get; init; } = "rebalance-native-declarations-v1";
    public string EngineLayoutCandidate { get; init; } = "UE4.22 x64 legacy UProperty; independently validated control declarations required";
    public bool Completed { get; set; }
    public bool ControlContractsPassed { get; set; }
    public bool GameplayFunctionsInvoked { get; init; } = false;
    public bool GameMemoryWritten { get; init; } = false;
    public bool UpgradeSemanticsVerified { get; init; } = false;
    public bool DungeonsRuntimeCompatibilityPreviouslyVerified { get; init; } = false;
    public int? FunctionHeaderOffset { get; set; }
    public int? FunctionNumParmsDelta { get; set; }
    public int? ChildrenOffset { get; set; }
    public int? PropertyTargetOffset { get; set; }
    public int? NameCharactersOffset { get; set; }
    public int? NameChunkCapacity { get; set; }
    public long BytesRead { get; set; }
    public int ReadCalls { get; set; }
    public int RegionQueries { get; set; }
    public DiscoveryDiagnostics Discovery { get; init; } = new();
    public string? Error { get; set; }
    public string[] MissingAllowlistedClasses { get; set; } = [];
    public List<ClassDeclaration> Classes { get; set; } = [];
}
record PropertyDeclaration(string Name, string Type, string? Target, int ArrayDim, int Size, int Offset, ulong Flags);
record FunctionDeclaration(string Name, uint Flags, int ParameterSize, int ReturnOffset, List<PropertyDeclaration> Parameters);
record ClassDeclaration(string Name, string? Super, List<PropertyDeclaration> Fields, List<FunctionDeclaration> Functions);

interface IMemory { byte[] Read(ulong address, int size); }
interface IRegionMemory : IMemory { MemoryRegion Query(ulong address); }
record MemoryRegion(ulong Start, ulong Size, uint State, uint Protection) {
    public bool Contains(ulong address, int size) => address >= Start && Size > 0 && address - Start <= Size && (ulong)size <= Size - (address - Start);
    public bool ReadableData => State == 0x1000 && (Protection & 0x100) == 0 && (Protection & 0xff) is 0x02 or 0x04 or 0x08;
}
sealed class DiscoveryDiagnostics {
    public int RawPointerCandidates { get; set; }
    public int MappedDataCandidates { get; set; }
    public int ValidatedNameTables { get; set; }
    public int InlineNameHeaderCandidates { get; set; }
    public int PointerNameHeaderCandidates { get; set; }
    public int ObjectTableShapeCandidates { get; set; }
    public List<string> RejectedObjectReasons { get; init; } = [];
}
static class CandidateFilter {
    public static ulong[] Select(IMemory memory, HashSet<ulong> raw) {
        var ordered = raw.Order().ToArray();
        if (memory is not IRegionMemory regions) throw new InvalidDataException("Memory-region queries are required for candidate filtering.");
        var selected = new List<ulong>(); MemoryRegion? region = null;
        foreach (var p in ordered) {
            if (region == null || !region.Contains(p, 1)) region = regions.Query(p);
            if (!region.Contains(p, 1)) throw new InvalidDataException("Memory-region query did not contain its requested address.");
            if (region.ReadableData && region.Contains(p, 1032)) selected.Add(p);
            if (selected.Count > 200000) throw new InvalidDataException("Too many mapped non-executable data candidates; bounded discovery stopped.");
        }
        return selected.ToArray();
    }
}
static class Bytes {
    public static int I32(byte[] b, int o) => BinaryPrimitives.ReadInt32LittleEndian(b.AsSpan(o, 4));
    public static ushort U16(byte[] b, int o) => BinaryPrimitives.ReadUInt16LittleEndian(b.AsSpan(o, 2));
    public static uint U32(byte[] b, int o) => BinaryPrimitives.ReadUInt32LittleEndian(b.AsSpan(o, 4));
    public static ulong U64(byte[] b, int o) => BinaryPrimitives.ReadUInt64LittleEndian(b.AsSpan(o, 8));
    public static bool Pointer(ulong p) => p >= 0x10000 && p <= 0x00007fffffff0000 && p % 8 == 0;
    public static void Range(ulong p, int size) {
        if (p < 0x10000 || size <= 0 || size > 1024 * 1024 || p > 0x00007fffffff0000 - (ulong)size)
            throw new InvalidDataException("Read address/size outside allowed range.");
    }
}
sealed class ProcessMemory : IRegionMemory, IDisposable {
    public const uint Access = 0x0410; // PROCESS_QUERY_INFORMATION | PROCESS_VM_READ. No write/execute rights.
    readonly IntPtr handle;
    readonly Stopwatch clock = Stopwatch.StartNew();
    public long BytesRead { get; private set; }
    public int ReadCalls { get; private set; }
    public int RegionQueries { get; private set; }
    public ProcessMemory(int pid) {
        handle = OpenProcess(Access, false, pid);
        if (handle == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error(), "Query/read access denied or unavailable; stop without elevation or a protection fallback.");
    }
    public byte[] Read(ulong address, int size) {
        Bytes.Range(address, size);
        // Two bounded name-header variants plus up to 1M live UObject headers require more than the old 500K call ceiling.
        if (clock.Elapsed.TotalSeconds > 60 || ReadCalls >= 2000000 || BytesRead + size > 128L * 1024 * 1024)
            throw new BudgetExceededException();
        ReadCalls++; BytesRead += size;
        var result = new byte[size];
        if (!ReadProcessMemory(handle, (IntPtr)(long)address, result, (nuint)size, out var read) || read != (nuint)size)
            throw new InvalidDataException("An exact metadata read failed; the process/layout may have changed.");
        return result;
    }
    public MemoryRegion Query(ulong address) {
        Bytes.Range(address, 1);
        if (clock.Elapsed.TotalSeconds > 60 || RegionQueries >= 4096) throw new BudgetExceededException();
        RegionQueries++;
        if (VirtualQueryEx(handle, (IntPtr)(long)address, out var info, (nuint)Marshal.SizeOf<MemoryInfo>()) != (nuint)Marshal.SizeOf<MemoryInfo>())
            throw new InvalidDataException("Memory-region query failed; no fallback was attempted.");
        return new((ulong)info.BaseAddress, (ulong)info.RegionSize, info.State, info.Protect);
    }
    public void Dispose() => CloseHandle(handle);
    [DllImport("kernel32.dll", SetLastError = true)] static extern IntPtr OpenProcess(uint access, bool inherit, int pid);
    [DllImport("kernel32.dll", SetLastError = true)] static extern bool ReadProcessMemory(IntPtr process, IntPtr address, [Out] byte[] buffer, nuint size, out nuint read);
    [DllImport("kernel32.dll")] static extern bool CloseHandle(IntPtr handle);
    [StructLayout(LayoutKind.Sequential)] struct MemoryInfo {
        public nuint BaseAddress, AllocationBase;
        public uint AllocationProtect;
        public ushort PartitionId, Padding;
        public nuint RegionSize;
        public uint State, Protect, Type, Padding2;
    }
    [DllImport("kernel32.dll", SetLastError = true)] static extern nuint VirtualQueryEx(IntPtr process, IntPtr address, out MemoryInfo info, nuint size);
}
sealed class BudgetExceededException : Exception { public BudgetExceededException() : base("The bounded read/time budget was exhausted. No offsets or declarations were guessed.") { } }

static class ModuleData {
    public static List<ModuleRegion> Read(IMemory memory, ulong image) {
        var header = memory.Read(image, 4096);
        var pe = Bytes.I32(header, 0x3c);
        if (header[0] != 'M' || header[1] != 'Z' || pe < 0x40 || pe > 2048 || Bytes.U32(header, pe) != 0x4550 || Bytes.U16(header, pe + 4) != 0x8664)
            throw new InvalidDataException("Expected a bounded x64 PE header.");
        int count = Bytes.U16(header, pe + 6), optional = Bytes.U16(header, pe + 20);
        if (count is < 1 or > 96 || optional < 112 || Bytes.U16(header, pe + 24) != 0x20b || pe + 24 + optional + count * 40 > header.Length)
            throw new InvalidDataException("Invalid PE section table.");
        uint imageSize = Bytes.U32(header, pe + 24 + 56);
        var result = new List<ModuleRegion>();
        for (int i = 0; i < count; i++) {
            int s = pe + 24 + optional + i * 40;
            uint flags = Bytes.U32(header, s + 36), size = Bytes.U32(header, s + 8), rva = Bytes.U32(header, s + 12);
            // Only readable/writable initialized, non-executable image data. No executable bytes exported.
            if ((flags & 0xc0000040) != 0xc0000040 || (flags & 0x20000000) != 0) continue;
            if (size == 0 || size > 16 * 1024 * 1024 || rva > imageSize || size > imageSize - rva)
                throw new InvalidDataException("Module data section outside bounded range.");
            var data = new byte[(int)size];
            for (int j = 0; j < data.Length; j += 1024 * 1024) {
                var block = memory.Read(image + rva + (ulong)j, Math.Min(1024 * 1024, data.Length - j));
                block.CopyTo(data, j);
            }
            result.Add(new(image + rva, data));
        }
        if (result.Count == 0) throw new InvalidDataException("No eligible module data section.");
        return result;
    }
}
record ModuleRegion(ulong Address, byte[] Data);

sealed class Names {
    readonly IMemory memory; readonly ulong table; readonly int count;
    public int CharactersOffset { get; }
    public int ChunkCapacity { get; }
    readonly Dictionary<int, string> cache = new();
    public Names(IMemory memory, ulong table, int charactersOffset, int capacity) {
        this.memory = memory; this.table = table;
        CharactersOffset = charactersOffset;
        ChunkCapacity = capacity;
        var header = memory.Read(table + (ulong)capacity * 8, 8);
        count = Bytes.I32(header, 0); int chunks = Bytes.I32(header, 4);
        if (count is < 8 or > 2000000 || chunks < (count + 16383) / 16384 || chunks > capacity)
            throw new InvalidDataException("Invalid legacy name table dimensions.");
        if (Get(0) != "None" || Enumerable.Range(1, 7).Count(i => Get(i).EndsWith("Property", StringComparison.Ordinal)) < 4)
            throw new InvalidDataException("Legacy name table controls failed.");
    }
    public string Get(int index) {
        if (index < 0 || index >= count) throw new InvalidDataException("Name index outside table.");
        if (cache.TryGetValue(index, out var name)) return name;
        ulong chunk = Bytes.U64(memory.Read(table + (ulong)(index / 16384 * 8), 8), 0);
        if (!Bytes.Pointer(chunk)) throw new InvalidDataException("Invalid name chunk pointer.");
        ulong entry = Bytes.U64(memory.Read(chunk + (ulong)(index % 16384 * 8), 8), 0);
        if (!Bytes.Pointer(entry)) throw new InvalidDataException("Invalid name entry pointer.");
        var prefix = memory.Read(entry, 12); uint encoded = Bytes.U32(prefix, 8);
        if ((encoded >> 1) != index) throw new InvalidDataException("Mismatched legacy name entry.");
        bool wide = (encoded & 1) != 0;
        // Small bounded blocks avoid reading an entire 256-byte buffer beyond a valid entry/page.
        var bytes = new List<byte>();
        for (int i = 0; i < 256; i++) {
            var part = memory.Read(entry + (ulong)CharactersOffset + (ulong)i * (wide ? 2UL : 1UL), wide ? 2 : 1);
            if (part.All(b => b == 0)) {
                string text = wide ? new UnicodeEncoding(false, false, true).GetString(bytes.ToArray()) : Encoding.ASCII.GetString(bytes.ToArray());
                if (text.Length == 0 || text.Any(char.IsControl)) throw new InvalidDataException("Invalid name characters.");
                return cache[index] = text;
            }
            if (!wide && part[0] is < 32 or > 126) throw new InvalidDataException("Unsupported ANSI name characters.");
            bytes.AddRange(part);
        }
        throw new InvalidDataException("Name exceeds bounded length.");
    }
}

sealed class Reflection {
    public static readonly string[] Allowlist = ["PlayerCharacterSaveSlot", "PlayerControllerBase", "InventoryItem", "InventoryItemSlot", "ItemStashComponent", "WalletComponent", "MerchantTransactionBase", "MerchantTransactionUtil", "TowerMerchantUtil", "TowerArtisanMerchantDef", "TowerBlacksmithMerchantDef", "TowerGilderMerchantDef", "UniqueCollectItem", "GildItem", "UpgradeTowerItem", "InventoryItemDataFunctionLibrary", "ItemFunctionLibrary", "TowerFunctionLibrary"];
    readonly IMemory memory; readonly Names names;
    readonly Dictionary<string, ulong> classes;
    public int FunctionHeaderOffset { get; private set; }
    public int FunctionNumParmsDelta { get; private set; }
    public int ChildrenOffset { get; private set; }
    public int PropertyTargetOffset { get; private set; }
    public int NameCharactersOffset => names.CharactersOffset;
    public int NameChunkCapacity => names.ChunkCapacity;
    Reflection(IMemory memory, Names names, Dictionary<string, ulong> classes) { this.memory = memory; this.names = names; this.classes = classes; }
    public static Reflection Discover(IMemory memory, List<ModuleRegion> regions, DiscoveryDiagnostics? diagnostics = null) {
        diagnostics ??= new();
        var candidates = new HashSet<ulong>();
        var inline = new HashSet<(ulong Address, int Capacity)>();
        foreach (var region in regions) for (int i = 0; i + 8 <= region.Data.Length; i += 8) {
            var data = region.Data;
            ulong p = Bytes.U64(data, i);
            if (Bytes.Pointer(p)) candidates.Add(p);
            if (candidates.Count > 1000000) throw new InvalidDataException("Raw candidate workspace limit exceeded.");
            foreach (int capacity in new[] { 128, 256 }) if (i + capacity * 8 + 8 <= data.Length) {
                int count = Bytes.I32(data, i + capacity * 8), chunks = Bytes.I32(data, i + capacity * 8 + 4);
                if (count is >= 8 and <= 2000000 && chunks >= (count + 16383) / 16384 && chunks <= capacity
                    && Bytes.Pointer(p)) inline.Add((region.Address + (ulong)i, capacity));
            }
        }
        diagnostics.RawPointerCandidates = candidates.Count;
        var filtered = CandidateFilter.Select(memory, candidates);
        diagnostics.MappedDataCandidates = filtered.Length;
        diagnostics.InlineNameHeaderCandidates = inline.Count;
        var validated = new List<Names>();
        foreach (var specification in inline.Union(filtered.SelectMany(address => new[] { (address, 128), (address, 256) }))) {
            ulong candidate = specification.Item1; int capacity = specification.Item2;
            try {
                var dimensions = memory.Read(candidate + (ulong)capacity * 8, 8);
                int count = Bytes.I32(dimensions, 0), chunks = Bytes.I32(dimensions, 4);
                if (count is < 8 or > 2000000 || chunks < (count + 16383) / 16384 || chunks > capacity) continue;
                if (!inline.Contains(specification)) diagnostics.PointerNameHeaderCandidates++;
                foreach (int offset in new[] { 12, 16 }) {
                    try { validated.Add(new Names(memory, candidate, offset, capacity)); } catch (InvalidDataException) { }
                }
            } catch (InvalidDataException) { }
        }
        diagnostics.ValidatedNameTables = validated.Count;
        if (validated.Count != 1) throw new InvalidDataException("Expected one independently validated legacy name table, found " + validated.Count + ".");
        var nameTable = validated.Single();
        var matches = new List<Reflection>();
        foreach (var region in regions) for (int i = 0; i + 32 <= region.Data.Length; i += 8) {
            var data = region.Data;
            ulong chunks = Bytes.U64(data, i);
            int max = Bytes.I32(data, i + 16), count = Bytes.I32(data, i + 20), maxChunks = Bytes.I32(data, i + 24), usedChunks = Bytes.I32(data, i + 28);
            if (!Bytes.Pointer(chunks) || count is < 100 or > 1000000 || max < count || max > 4 * 1024 * 1024
                || usedChunks < (count + 65535) / 65536 || maxChunks < usedChunks || maxChunks > 64
                || max != maxChunks * 65536) continue;
            diagnostics.ObjectTableShapeCandidates++;
            try {
                var objects = ReadClasses(memory, nameTable, chunks, count, (count + 65535) / 65536);
                if (!objects.ContainsKey("PlayerCharacterSaveSlot") || !objects.ContainsKey("PlayerControllerBase")) continue;
                foreach (int childOffset in new[] { 0x38, 0x48 }) foreach (int targetOffset in new[] { 0x70, 0x78, 0x80 }) {
                    try {
                        var candidate = new Reflection(memory, nameTable, objects) { ChildrenOffset = childOffset, PropertyTargetOffset = targetOffset };
                        (candidate.FunctionHeaderOffset, candidate.FunctionNumParmsDelta) = candidate.InferFunctionHeader();
                        Contracts.Validate(candidate.CaptureControls());
                        matches.Add(candidate);
                    } catch (InvalidDataException error) { RecordRejection(error.Message); }
                }
            } catch (InvalidDataException error) {
                RecordRejection(error.Message);
            }
        }
        if (matches.Count != 1) throw new InvalidDataException("Expected one validated object/layout candidate, found " + matches.Count + ".");
        return matches.Single();
        void RecordRejection(string message) {
            if (diagnostics.RejectedObjectReasons.Count < 8 && !diagnostics.RejectedObjectReasons.Contains(message)) diagnostics.RejectedObjectReasons.Add(message);
        }
    }
    static Dictionary<string, ulong> ReadClasses(IMemory memory, Names names, ulong chunks, int count, int usedChunks) {
        var result = new Dictionary<string, ulong>();
        var pointers = memory.Read(chunks, usedChunks * 8);
        int checkedHeaders = 0;
        // Validate table identity before traversing it.
        ulong first = Bytes.U64(pointers, 0);
        if (!Bytes.Pointer(first)) throw new InvalidDataException("Invalid object chunk.");
        var initial = memory.Read(first, 16 * 24);
        for (int i = 0; i < 16; i++) {
            ulong obj = Bytes.U64(initial, i * 24); if (obj == 0) continue;
            if (!Bytes.Pointer(obj) || Bytes.I32(memory.Read(obj, 40), 12) != i) throw new InvalidDataException("Object index control failed.");
            checkedHeaders++;
        }
        if (checkedHeaders < 3) throw new InvalidDataException("Insufficient object controls.");
        var typeCache = new Dictionary<ulong, string>();
        for (int c = 0; c < usedChunks; c++) {
            ulong chunk = Bytes.U64(pointers, c * 8);
            if (!Bytes.Pointer(chunk)) throw new InvalidDataException("Invalid object chunk.");
            int remaining = Math.Min(65536, count - c * 65536);
            for (int start = 0; start < remaining; start += 16384) {
                int batch = Math.Min(16384, remaining - start);
                var items = memory.Read(chunk + (ulong)(start * 24), batch * 24);
                for (int j = 0; j < batch; j++) {
                    ulong obj = Bytes.U64(items, j * 24); if (obj == 0) continue;
                    if (!Bytes.Pointer(obj)) throw new InvalidDataException("Invalid object pointer.");
                    var h = memory.Read(obj, 40);
                    if (Bytes.I32(h, 12) != c * 65536 + start + j) throw new InvalidDataException("Object index changed during capture.");
                    ulong type = Bytes.U64(h, 16);
                    if (!Bytes.Pointer(type)) throw new InvalidDataException("Invalid object class.");
                    if (!typeCache.TryGetValue(type, out var typeName)) typeCache[type] = typeName = names.Get(Bytes.I32(memory.Read(type, 40), 24));
                    if (typeName != "Class") continue;
                    string name = names.Get(Bytes.I32(h, 24));
                    if (!Allowlist.Contains(name)) continue;
                    ulong outer = Bytes.U64(h, 32);
                    if (!Bytes.Pointer(outer) || names.Get(Bytes.I32(memory.Read(outer, 40), 24)) != "/Script/Dungeons") continue;
                    if (!result.TryAdd(name, obj)) throw new InvalidDataException("Duplicate allowlisted native class.");
                }
            }
        }
        return result;
    }
    string Name(ulong obj) => names.Get(Bytes.I32(memory.Read(obj, 40), 24));
    string Type(ulong obj) { var p = Bytes.U64(memory.Read(obj + 16, 8), 0); return Name(p); }
    IEnumerable<ulong> Children(ulong obj) {
        ulong p = Bytes.U64(memory.Read(obj + (ulong)ChildrenOffset, 8), 0); var visited = new HashSet<ulong>();
        while (p != 0) {
            if (!Bytes.Pointer(p) || !visited.Add(p) || visited.Count > 4096) throw new InvalidDataException("Invalid/cyclic legacy UField chain.");
            if (Bytes.U64(memory.Read(p + 32, 8), 0) != obj) throw new InvalidDataException("Legacy child owner mismatch.");
            yield return p;
            p = Bytes.U64(memory.Read(p + 0x28, 8), 0);
        }
    }
    (int Offset, int NumParmsDelta) InferFunctionHeader() {
        var controls = new[] { ("PlayerCharacterSaveSlot", "GetCloudPlayerId", 16), ("PlayerControllerBase", "GetRecentSaveDataIndex", 4), ("PlayerControllerBase", "GetNumProfiles", 4), ("PlayerControllerBase", "GetSaveLocalUserNum", 4) };
        var headers = controls.Select(c => {
            var functions = Children(classes[c.Item1]).Where(p => Type(p) == "Function" && Name(p) == c.Item2).ToArray();
            if (functions.Length != 1) throw new InvalidDataException("Missing/duplicate header control: " + c.Item2);
            return (Header: memory.Read(functions[0], 0xd0), Size: c.Item3);
        }).ToArray();
        // Both legacy header variants are candidates. Choose neither unless the independent controls select one uniquely.
        var offsets = (from o in Enumerable.Range(0, 17).Select(i => 0x80 + i * 4)
            from delta in new[] { 4, 6 }
            where headers.All(h => (Bytes.U32(h.Header, o) & 0x400) != 0 && h.Header[o + delta] == 1
                && Bytes.U16(h.Header, o + delta + 2) == h.Size && Bytes.U16(h.Header, o + delta + 4) == 0)
            select (o, delta)).ToArray();
        if (offsets.Length != 1) throw new InvalidDataException("Function header inference is missing or ambiguous.");
        return offsets.Single();
    }
    PropertyDeclaration Property(ulong p) {
        var h = memory.Read(p, 0x90); string type = Type(p); string? target = null;
        int targetOffset = PropertyTargetOffset + (type == "ClassProperty" ? 8 : 0);
        if (type is "ObjectProperty" or "ClassProperty" or "StructProperty" or "ByteProperty" or "InterfaceProperty" or "WeakObjectProperty" or "SoftObjectProperty") {
            ulong reference = Bytes.U64(h, targetOffset);
            if (reference != 0) {
                var referenced = memory.Read(reference, 40);
                ulong owner = Bytes.U64(referenced, 32);
                target = (owner == 0 ? "" : Name(owner) + ".") + Name(reference);
            }
        }
        int dim = Bytes.I32(h, 0x30), size = Bytes.I32(h, 0x34), offset = Bytes.I32(h, 0x44);
        if (dim is < 1 or > 65536 || size is < 1 or > 1048576 || offset is < 0 or > 16777216) throw new InvalidDataException("Invalid legacy property layout.");
        return new(Name(p), type, target, dim, size, offset, Bytes.U64(h, 0x38));
    }
    FunctionDeclaration Function(ulong p) {
        var h = memory.Read(p, 0xd0); int o = FunctionHeaderOffset, delta = FunctionNumParmsDelta;
        var parameters = Children(p).Where(child => Type(child).EndsWith("Property", StringComparison.Ordinal)).Select(Property).Where(prop => (prop.Flags & 0x80) != 0).ToList();
        int size = Bytes.U16(h, o + delta + 2), ret = Bytes.U16(h, o + delta + 4);
        if (parameters.Count != h[o + delta] || parameters.Any(prop => (long)prop.Offset + (long)prop.Size * prop.ArrayDim > size) || (ret != 65535 && ret >= size))
            throw new InvalidDataException("Function parameter/header bounds disagree.");
        return new(Name(p), Bytes.U32(h, o), size, ret, parameters);
    }
    public List<ClassDeclaration> Capture() => classes.OrderBy(k => k.Key).Select(k => {
        var children = Children(k.Value).ToArray(); ulong super = Bytes.U64(memory.Read(k.Value + (ulong)ChildrenOffset - 8, 8), 0);
        return new ClassDeclaration(k.Key, super == 0 ? null : Name(super),
            children.Where(p => Type(p).EndsWith("Property", StringComparison.Ordinal)).Select(Property).ToList(),
            children.Where(p => Type(p) == "Function").Select(Function).ToList());
    }).ToList();
    List<ClassDeclaration> CaptureControls() => classes.Where(k => Contracts.Controls.Any(c => c.Owner == k.Key)).Select(k =>
        new ClassDeclaration(k.Key, null, [], Children(k.Value).Where(p => Type(p) == "Function" && Contracts.Controls.Any(c => c.Owner == k.Key && c.Method == Name(p))).Select(Function).ToList())).ToList();
}

static class Contracts {
    // Independently observed cooked caller shapes from QoL's retail profile evidence.
    public static readonly (string Owner, string Method, string Type, string? Target, int Size, string[] Inputs)[] Controls = [
        ("PlayerCharacterSaveSlot", "GetCloudPlayerId", "StructProperty", "/Script/CoreUObject.Guid", 16, []),
        ("PlayerControllerBase", "GetRecentSaveDataIndex", "IntProperty", null, 4, []),
        ("PlayerControllerBase", "GetNumProfiles", "IntProperty", null, 4, []),
        ("PlayerControllerBase", "GetSaveLocalUserNum", "IntProperty", null, 4, []),
        ("PlayerControllerBase", "GetAvailableSaveDataByIndex", "ObjectProperty", "/Script/Dungeons.CharacterSaveData", 8, ["IntProperty"]),
        ("PlayerControllerBase", "GetCharacterSlotByIndex", "ObjectProperty", "/Script/Dungeons.PlayerCharacterSaveSlot", 8, ["IntProperty", "BoolProperty"])
    ];
    public static void Validate(List<ClassDeclaration> classes) {
        foreach (var c in Controls) {
            var owners = classes.Where(o => o.Name == c.Owner).ToArray();
            if (owners.Length != 1) throw new InvalidDataException("Missing/duplicate control owner: " + c.Owner);
            var functions = owners[0].Functions.Where(f => f.Name == c.Method).ToArray();
            if (functions.Length != 1) throw new InvalidDataException("Missing/duplicate control method: " + c.Method);
            var f = functions[0]; var returns = f.Parameters.Where(p => (p.Flags & 0x400) != 0).ToArray();
            var inputs = f.Parameters.Where(p => (p.Flags & 0x400) == 0).OrderBy(p => p.Offset).ToArray();
            if ((f.Flags & 0x400) == 0 || returns.Length != 1 || inputs.Length != c.Inputs.Length)
                throw new InvalidDataException("Control native/parameter shape mismatch: " + c.Method);
            var r = returns[0];
            if (r.Type != c.Type || r.Target != c.Target || r.Size != c.Size || r.ArrayDim != 1 || r.Offset != f.ReturnOffset || (r.Flags & 0x80) == 0)
                throw new InvalidDataException("Control return shape mismatch: " + c.Method);
            for (int i = 0; i < inputs.Length; i++) {
                var p = inputs[i]; int expectedSize = c.Inputs[i] == "IntProperty" ? 4 : 1;
                if (p.Type != c.Inputs[i] || p.ArrayDim != 1 || p.Size != expectedSize || (p.Flags & 0x80) == 0 || (p.Flags & 0x100) != 0)
                    throw new InvalidDataException("Control input shape mismatch: " + c.Method);
            }
        }
    }
}
