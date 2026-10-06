using System.Runtime.InteropServices;

// Entirely project-owned synthetic metadata. No retail executable/save/asset bytes.
static class Fixtures {
    public static void Run() {
        int passed = 0;
        void Check(string name, Action test) { test(); passed++; Console.WriteLine("PASS " + name); }
        void Reject(string name, Action test) => Check(name, () => {
            try { test(); } catch (InvalidDataException) { return; }
            throw new Exception("Expected rejection: " + name);
        });
        foreach (int delta in new[] { 4, 6 }) foreach (int offset in new[] { 0x98, 0xb0 }) Check("bootstrap and six controls at header " + offset + "/" + delta, () => {
            var arena = new Arena(offset, delta);
            var reflection = Reflection.Discover(arena, [arena.Module]);
            if (reflection.FunctionHeaderOffset != offset || reflection.FunctionNumParmsDelta != delta) throw new Exception("Wrong inferred function header.");
            Contracts.Validate(reflection.Capture());
        });
        Reject("ambiguous function headers", () => {
            var arena = new Arena(0x98);
            foreach (var (p, size) in arena.Functions.Where(f => f.Size is 4 or 16)) arena.Header(p, 0xb0, 1, size, 0);
            Reflection.Discover(arena, [arena.Module]);
        });
        Reject("cyclic child chain", () => {
            var a = new Arena(0x98); a.U64(a.Functions[0].Address + 0x28, a.Functions[0].Address);
            Reflection.Discover(a, [a.Module]);
        });
        Reject("wrong Guid target", () => {
            var a = new Arena(0x98); a.U64(a.GuidResult + 0x70, a.Obj("Vector", a.StructClass, 0));
            Reflection.Discover(a, [a.Module]);
        });
        Reject("parameter count mismatch", () => {
            var a = new Arena(0x98); a.Raw[(int)(a.Functions[0].Address - Arena.Base + 0x98 + 6)] = 0;
            Reflection.Discover(a, [a.Module]);
        });
        Reject("child owner mismatch", () => {
            var a = new Arena(0x98); a.U64(a.GuidResult + 32, a.StructClass);
            Reflection.Discover(a, [a.Module]);
        });
        Reject("input out-only", () => {
            var a = new Arena(0x98); a.U64(a.FirstInput + 0x38, 0x180);
            Reflection.Discover(a, [a.Module]);
        });
        Reject("name entry identity mismatch", () => {
            var a = new Arena(0x98); a.I32(a.NoneEntry + 8, 2);
            Reflection.Discover(a, [a.Module]);
        });
        Reject("object index identity mismatch", () => {
            var a = new Arena(0x98); a.I32(a.StructClass + 12, 900);
            Reflection.Discover(a, [a.Module]);
        });
        Reject("null read", () => Bytes.Range(0, 4));
        Reject("overflow/out-of-range read", () => Bytes.Range(ulong.MaxValue, 4));
        Reject("oversized read", () => Bytes.Range(Arena.Base, 1048577));
        Check("unaligned byte reads permitted", () => Bytes.Range(Arena.Base + 3, 1));
        Check("capture cannot certify upgrades", () => {
            var report = new CaptureReport();
            if (report.Completed || report.GameplayFunctionsInvoked || report.GameMemoryWritten || report.UpgradeSemanticsVerified)
                throw new Exception("Unsafe report defaults.");
        });
        Check("only query/read rights", () => { if (ProcessMemory.Access != 0x0410) throw new Exception("Unexpected process rights."); });
        Check("large numeric noise filtered before table limit", () => {
            var a = new Arena(0x98);
            var noisy = new byte[210001 * 8];
            for (int i = 0; i < 210001; i++) BitConverter.GetBytes(0x100000000UL + (ulong)i * 8).CopyTo(noisy, i * 8);
            var diagnostics = new DiscoveryDiagnostics();
            Contracts.Validate(Reflection.Discover(a, [a.Module, noisy], diagnostics).Capture());
            if (diagnostics.RawPointerCandidates <= 200000 || diagnostics.MappedDataCandidates > 200000 || a.Queries > 10)
                throw new Exception("Noisy candidates were not removed/cached efficiently.");
        });
        Check("guarded/executable/reserved regions rejected", () => {
            foreach (uint p in new uint[] { 0x01, 0x10, 0x20, 0x40, 0x80, 0x104 })
                if (new MemoryRegion(Arena.Base, 4096, 0x1000, p).ReadableData) throw new Exception("Unsafe region admitted.");
            if (new MemoryRegion(Arena.Base, 4096, 0x2000, 0x04).ReadableData) throw new Exception("Uncommitted region admitted.");
        });
        Check("readable regions and boundary filtering", () => {
            var a = new Arena(0x98); var r = a.Query(Arena.Base);
            if (CandidateFilter.Select(a, [Arena.Base, Arena.Base + (ulong)a.Raw.Length - 8]).Length != 1) throw new Exception("Boundary candidate admitted.");
            foreach (uint p in new uint[] { 0x02, 0x04, 0x08 }) if (!new MemoryRegion(Arena.Base, 4096, 0x1000, p).ReadableData) throw new Exception("Readable data rejected.");
        });
        if (OperatingSystem.IsWindows()) Check("Windows read API on own process", () => {
            var pointer = Marshal.AllocHGlobal(8);
            try {
                Marshal.WriteInt64(pointer, 0x1122334455667788);
                using var memory = new ProcessMemory(Environment.ProcessId);
                if (!memory.Query((ulong)pointer.ToInt64()).ReadableData) throw new Exception("Own marker region not readable data.");
                if (Bytes.U64(memory.Read((ulong)pointer.ToInt64(), 8), 0) != 0x1122334455667788)
                    throw new Exception("Read API marker mismatch.");
            } finally { Marshal.FreeHGlobal(pointer); }
        });
        Console.WriteLine($"{passed} checks passed. Synthetic/self-process tests do not establish Dungeons compatibility.");
    }
}

sealed class Arena : IRegionMemory {
    public const ulong Base = 0x100000;
    public readonly byte[] Raw = new byte[4 * 1024 * 1024];
    public readonly byte[] Module = new byte[4096];
    readonly ulong table = Base + 0x1000, names = Base + 0x2000, chunks = Base + 0x30000, items = Base + 0x31000;
    ulong next = Base + 0x40000, nameNext = Base + 0x60000;
    int objects, namesCount;
    readonly int numParmsDelta;
    readonly Dictionary<string, int> ids = new();
    readonly Dictionary<string, ulong> types = new();
    public readonly List<(ulong Address, int Size)> Functions = [];
    public ulong GuidResult, FirstInput, NoneEntry, StructClass;
    public int Queries;
    public MemoryRegion Query(ulong address) {
        Queries++;
        if (address >= Base && address < Base + (ulong)Raw.Length) return new(Base, (ulong)Raw.Length, 0x1000, 0x04);
        if (address >= 0x100000000 && address < 0x110000000) return new(0x100000000, 0x10000000, 0x10000, 0);
        return new(address & ~0xfffUL, 4096, 0x10000, 0);
    }
    public Arena(int header, int delta = 6) {
        numParmsDelta = delta;
        U64(table, names);
        NameId("None"); NoneEntry = Bytes.U64(Read(names, 8), 0);
        foreach (string n in new[] { "ByteProperty", "IntProperty", "BoolProperty", "FloatProperty", "ObjectProperty", "NameProperty", "DelegateProperty" }) NameId(n);
        var classClass = Obj("Class", 0, 0); U64(classClass + 16, classClass);
        var functionClass = Obj("Function", classClass, 0);
        StructClass = Obj("ScriptStruct", classClass, 0);
        foreach (string t in new[] { "StructProperty", "IntProperty", "ObjectProperty", "BoolProperty" }) types[t] = Obj(t, classClass, 0);
        var package = Obj("/Script/Dungeons", classClass, 0);
        var core = Obj("/Script/CoreUObject", classClass, 0);
        var guid = Obj("Guid", StructClass, core);
        var save = Obj("CharacterSaveData", classClass, package);
        var slot = Obj("PlayerCharacterSaveSlot", classClass, package);
        var controller = Obj("PlayerControllerBase", classClass, package);
        var last = new Dictionary<ulong, ulong>();
        foreach (var c in Contracts.Controls) {
            ulong owner = c.Owner == "PlayerCharacterSaveSlot" ? slot : controller;
            var function = Obj(c.Method, functionClass, owner);
            if (last.TryGetValue(owner, out var previous)) U64(previous + 0x28, function); else U64(owner + 0x48, function);
            last[owner] = function;
            var parameters = new List<ulong>(); int offset = 0;
            foreach (string input in c.Inputs) {
                int size = input == "IntProperty" ? 4 : 1;
                var p = Prop("Input" + parameters.Count, input, function, size, offset, 0x80, 0);
                parameters.Add(p); offset += size;
                if (FirstInput == 0) FirstInput = p;
            }
            offset = (offset + c.Size - 1) / c.Size * c.Size;
            ulong target = c.Target switch { "/Script/CoreUObject.Guid" => guid, "/Script/Dungeons.CharacterSaveData" => save, "/Script/Dungeons.PlayerCharacterSaveSlot" => slot, _ => 0 };
            var result = Prop("ReturnValue", c.Type, function, c.Size, offset, 0x580, target);
            if (c.Method == "GetCloudPlayerId") GuidResult = result;
            parameters.Add(result); U64(function + 0x48, parameters[0]);
            for (int i = 0; i < parameters.Count - 1; i++) U64(parameters[i] + 0x28, parameters[i + 1]);
            Header(function, header, parameters.Count, offset + c.Size, offset);
            Functions.Add((function, offset + c.Size));
        }
        // Remaining object slots are intentionally null, matching supported table holes.
        U64(chunks, items);
        WriteModule(0, table); WriteModule(64, chunks);
        BitConverter.GetBytes(65536).CopyTo(Module, 80);
        BitConverter.GetBytes(100).CopyTo(Module, 84);
        BitConverter.GetBytes(1).CopyTo(Module, 88); BitConverter.GetBytes(1).CopyTo(Module, 92);
        I32(table + 1024, namesCount); I32(table + 1028, 1);
    }
    public byte[] Read(ulong address, int size) {
        Bytes.Range(address, size);
        if (address < Base || address - Base + (ulong)size > (ulong)Raw.Length) throw new InvalidDataException("Outside fixture arena.");
        return Raw.AsSpan((int)(address - Base), size).ToArray();
    }
    void WriteModule(int offset, ulong value) => BitConverter.GetBytes(value).CopyTo(Module, offset);
    public void U64(ulong address, ulong value) => BitConverter.GetBytes(value).CopyTo(Raw, (int)(address - Base));
    public void I32(ulong address, int value) => BitConverter.GetBytes(value).CopyTo(Raw, (int)(address - Base));
    int NameId(string name) {
        if (ids.TryGetValue(name, out int id)) return id;
        id = namesCount++; ids[name] = id;
        ulong entry = nameNext; nameNext += 512;
        U64(names + (ulong)(id * 8), entry); I32(entry + 8, id << 1);
        System.Text.Encoding.ASCII.GetBytes(name).CopyTo(Raw, (int)(entry + 12 - Base));
        return id;
    }
    public ulong Obj(string name, ulong type, ulong outer) {
        ulong p = next; next += 512;
        I32(p + 12, objects); U64(p + 16, type); I32(p + 24, NameId(name)); U64(p + 32, outer);
        U64(items + (ulong)(objects++ * 24), p); I32(table + 1024, namesCount);
        return p;
    }
    ulong Prop(string name, string type, ulong owner, int size, int offset, ulong flags, ulong target) {
        ulong p = Obj(name, types[type], owner);
        I32(p + 0x30, 1); I32(p + 0x34, size); U64(p + 0x38, flags); I32(p + 0x44, offset); U64(p + 0x70, target);
        return p;
    }
    public void Header(ulong p, int offset, int count, int size, int ret) {
        I32(p + (ulong)offset, 0x400); Raw[(int)(p - Base) + offset + numParmsDelta] = (byte)count;
        BitConverter.GetBytes((ushort)size).CopyTo(Raw, (int)(p - Base) + offset + numParmsDelta + 2);
        BitConverter.GetBytes((ushort)ret).CopyTo(Raw, (int)(p - Base) + offset + numParmsDelta + 4);
    }
}
