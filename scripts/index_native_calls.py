#!/usr/bin/env python3
"""Index observed Kismet calls. This does not infer native signatures or semantics."""
import argparse
import hashlib
import json
from pathlib import Path


def index_report(report):
    if report.get('errors'):
        raise ValueError('Collector reported export errors')
    objects = {x['index']: x for x in report['imports'] + report['exports']}

    def path(index, seen=()):
        if not index:
            return ''
        if index in seen or index not in objects:
            raise ValueError('Invalid or cyclic object reference')
        obj = objects[index]
        parent = path(obj['outer'], seen + (index,))
        return parent + ('.' if parent else '') + obj['name']

    functions = {}
    for obj in report['imports']:
        if obj['className'] == 'Function':
            functions.setdefault(obj['name'], []).append(path(obj['index']))
    calls = []

    def unresolved(value):
        if isinstance(value, str):
            return '#Pointer Error#' in value or value == '^^^^^'
        if isinstance(value, dict):
            return any(unresolved(child) for child in value.values())
        if isinstance(value, list):
            return any(unresolved(child) for child in value)
        return False

    def walk(node, location, context=None, result=None):
        if isinstance(node, list):
            for i, child in enumerate(node):
                walk(child, f'{location}/{i}', context, result)
        elif isinstance(node, dict):
            instruction = node.get('Inst', '')
            if instruction in ('Context', 'Context_FailSilent', 'ClassContext'):
                # A context applies only to its Expression, never its receiver expression.
                walk(node.get('Context'), location + '/Context', context, result)
                walk(node.get('Expression'), location + '/Expression', node.get('Context'), {
                    key: node[key] for key in ('RValuePropertyOuter', 'RValuePropertyName') if key in node
                })
                return
            if instruction in ('FinalFunction', 'LocalFinalFunction', 'VirtualFunction', 'LocalVirtualFunction'):
                name = node.get('Function', node.get('FunctionName'))
                candidates = functions.get(name, [])
                native = [candidate for candidate in candidates if candidate.startswith('/Script/Dungeons.')]
                if native:
                    calls.append({'location': location, 'instruction': instruction,
                                  'function': name, 'importCandidates': sorted(candidates),
                                  'receiverExpression': context, 'resultProperty': result,
                                  'argumentExpressions': node.get('Parameters', []),
                                  'hasUnresolvedMetadata': unresolved([context, result, node]),
                                  'nativeSignatureVerified': False})
            for key, child in node.items():
                if isinstance(child, (dict, list)):
                    # Calls inside argument expressions evaluate independently of the callee receiver.
                    walk(child, location + '/' + key,
                         None if key == 'Parameters' else context,
                         None if key == 'Parameters' else result)

    for export in report['exports']:
        if export.get('script'):
            walk(export['script'], f"exports/{export['index']}:{export['name']}/script")
    classes = sorted(path(x['index']) for x in report['imports']
                     if x['className'] == 'Class' and path(x['index']).startswith('/Script/Dungeons.'))
    return {'package': report['packagePath'], 'nativeClasses': classes, 'calls': calls}


def generate(inputs):
    packages = {}
    for directory in inputs:
        for file in sorted(Path(directory).glob('*.json')):
            raw = file.read_bytes()
            report = json.loads(raw)
            indexed = index_report(report)
            indexed['metadataSha256'] = hashlib.sha256(raw).hexdigest()
            package = indexed['package']
            if package in packages and packages[package] != indexed:
                raise ValueError(f'Conflicting metadata for {package}')
            packages[package] = indexed
    if not packages:
        raise ValueError('No metadata reports found')
    return {'schemaVersion': 1, 'nativeSignatureVerified': False,
            'note': 'Observed import candidates and argument expressions only. No function parameter direction, native implementation, payment atomicity, or authority contract is certified.',
            'packages': [packages[key] for key in sorted(packages)]}


def markdown(data):
    lines = ['# Observed native call index', '', data['note'], '',
             '| Native import candidate | Observed argument counts | Call sites |',
             '| --- | --- | --- |']
    grouped = {}
    for package in data['packages']:
        for call in package['calls']:
            for candidate in call['importCandidates']:
                if candidate.startswith('/Script/Dungeons.'):
                    counts = grouped.setdefault(candidate, [])
                    counts.append(len(call['argumentExpressions']))
    for candidate, counts in sorted(grouped.items()):
        lines.append(f"| `{candidate}` | {', '.join(map(str, sorted(set(counts))))} | {len(counts)} |")
    degraded = sum(call['hasUnresolvedMetadata'] for package in data['packages'] for call in package['calls'])
    lines += ['', f"Inputs: {len(data['packages'])} cooked-package metadata reports. JSON companion records exact export/statement paths, receiver expressions, arguments and metadata hashes.", '',
              f'{degraded} call sites contain an unresolved pointer/name marker in the supplied serializer output. These are retained and flagged, not repaired by guessing.', '',
              'Counts refer to serialized caller arguments; these may include out parameters. A unique import candidate does not prove native eligibility, mutation behavior or multiplayer authority.', '']
    return '\n'.join(lines)


if __name__ == '__main__':
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--metadata', type=Path, action='append', required=True)
    parser.add_argument('--output', type=Path, required=True)
    args = parser.parse_args()
    data = generate(args.metadata)
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(json.dumps(data, indent=2) + '\n')
    args.output.with_suffix('.md').write_text(markdown(data))
    print(f"Indexed {len(data['packages'])} packages, {sum(len(p['calls']) for p in data['packages'])} observed native calls")
