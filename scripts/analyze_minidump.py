"""Extract a bounded, sanitized x64 crash summary; no symbolization claim."""
import argparse
import hashlib
import json
from pathlib import Path, PureWindowsPath
import struct
import xml.etree.ElementTree as ET


def analyze(dump, context):
    data = dump.read_bytes()
    def unpack(fmt, offset):
        size = struct.calcsize(fmt)
        if offset < 0 or offset + size > len(data):
            raise ValueError('Truncated minidump record')
        return struct.unpack_from(fmt, data, offset)
    def u32(offset):
        return unpack('<I', offset)[0]
    def u64(offset):
        return unpack('<Q', offset)[0]
    if data[:4] != b'MDMP':
        raise ValueError('Not a minidump')
    count, directory = u32(8), u32(12)
    if count > 1024:
        raise ValueError('Unexpected stream count')
    streams = {}
    for i in range(count):
        kind, size, offset = unpack('<III', directory + i * 12)
        if offset + size > len(data):
            raise ValueError('Stream extends past dump')
        streams[kind] = (size, offset)
    if 6 not in streams or streams[6][0] < 168:
        raise ValueError('Exception stream missing')
    exception = streams[6][1]
    parameter_count = u32(exception + 32)
    if parameter_count > 15:
        raise ValueError('Invalid exception parameter count')
    parameters = [u64(exception + 40 + i * 8) for i in range(parameter_count)]
    address = u64(exception + 24)
    modules = []
    if 4 in streams:
        size, offset = streams[4]
        module_count = u32(offset)
        if module_count * 108 + 4 > size:
            raise ValueError('Module list truncated')
        for i in range(module_count):
            entry = offset + 4 + i * 108
            name_at = u32(entry + 20)
            name_size = u32(name_at)
            if name_at + 4 + name_size > len(data):
                raise ValueError('Module name truncated')
            name = PureWindowsPath(data[name_at + 4:name_at + 4 + name_size].decode('utf-16-le')).name
            base, length = u64(entry), u32(entry + 8)
            if base <= address < base + length:
                modules.append(dict(name=name, rva=f'0x{address-base:x}'))
    root = ET.parse(context).getroot()
    find = lambda tag: root.findtext('.//' + tag)
    return dict(schema='rebalance-crash-summary-v1', dumpSha256=hashlib.sha256(data).hexdigest(),
                contextSha256=hashlib.sha256(context.read_bytes()).hexdigest(),
                engineVersion=find('EngineVersion'), secondsSinceStart=int(find('SecondsSinceStart') or 0),
                exceptionCode=f'0x{u32(exception+8):08x}',
                memoryAccess={0:'read', 1:'write', 8:'execute'}.get(parameters[0], 'unknown') if parameters else 'unknown',
                targetAddress=f'0x{parameters[1]:x}' if len(parameters)>1 else None,
                faultModules=modules, privateModulePathsOmitted=True, symbolsAvailable=False)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--dump', type=Path, required=True)
    parser.add_argument('--context', type=Path, required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    if args.output.exists():
        raise ValueError('Output already exists')
    result = analyze(args.dump, args.context)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(result, indent=2) + '\n')
    print(json.dumps(result))


if __name__ == '__main__':
    main()
