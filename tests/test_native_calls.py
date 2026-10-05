"""Verify evidence indexing, not native invocation or gameplay."""
import importlib.util
from pathlib import Path
import unittest

spec = importlib.util.spec_from_file_location('index_native_calls', Path(__file__).resolve().parents[1] / 'scripts/index_native_calls.py')
module = importlib.util.module_from_spec(spec)
spec.loader.exec_module(module)


def fixture(script):
    return {'packagePath': 'fixture.uasset', 'errors': [], 'imports': [
        {'index': -1, 'name': '/Script/Dungeons', 'outer': 0, 'className': 'Package'},
        {'index': -2, 'name': 'FixtureUtil', 'outer': -1, 'className': 'Class'},
        {'index': -3, 'name': 'Query', 'outer': -2, 'className': 'Function'},
    ], 'exports': [{'index': 1, 'name': 'Caller', 'outer': 0, 'script': script}]}


class NativeCallTests(unittest.TestCase):
    def test_nested_receiver_keeps_its_own_context(self):
        inner = {'Inst': 'FinalFunction', 'Function': 'Query', 'Parameters': []}
        source = fixture([{'Inst': 'Context', 'Context': inner, 'Expression': {
            'Inst': 'FinalFunction', 'Function': 'Query', 'Parameters': [{'Inst': 'IntConst', 'Value': 1}]}}])
        calls = module.index_report(source)['calls']
        self.assertEqual(len(calls), 2)
        self.assertIsNone(calls[0]['receiverExpression'])
        self.assertEqual(calls[1]['receiverExpression'], inner)
        self.assertEqual(calls[1]['importCandidates'], ['/Script/Dungeons.FixtureUtil.Query'])
        self.assertFalse(calls[1]['nativeSignatureVerified'])

    def test_same_name_imports_remain_ambiguous(self):
        source = fixture([{'Inst': 'FinalFunction', 'Function': 'Query', 'Parameters': []}])
        source['imports'] += [
            {'index': -4, 'name': 'OtherUtil', 'outer': -1, 'className': 'Class'},
            {'index': -5, 'name': 'Query', 'outer': -4, 'className': 'Function'}]
        self.assertEqual(len(module.index_report(source)['calls'][0]['importCandidates']), 2)

    def test_argument_call_does_not_inherit_callee_receiver(self):
        source = fixture([{'Inst': 'Context', 'Context': {'Inst': 'Self'},
                           'Expression': {'Inst': 'FinalFunction', 'Function': 'Query', 'Parameters': [
                               {'Inst': 'LocalFinalFunction', 'Function': 'Query', 'Parameters': []}]}}])
        calls = module.index_report(source)['calls']
        self.assertEqual(calls[0]['receiverExpression'], {'Inst': 'Self'})
        self.assertIsNone(calls[1]['receiverExpression'])

    def test_serializer_pointer_errors_are_flagged(self):
        source = fixture([{'Inst': 'Context', 'Context': {'Inst': 'Self'},
                           'RValuePropertyOuter': '#Pointer Error#', 'Expression': {
                               'Inst': 'FinalFunction', 'Function': 'Query', 'Parameters': []}}])
        self.assertTrue(module.index_report(source)['calls'][0]['hasUnresolvedMetadata'])

    def test_broken_evidence_is_rejected(self):
        source = fixture([])
        source['errors'] = ['failed export']
        with self.assertRaises(ValueError): module.index_report(source)
        source['errors'] = []
        source['imports'][1]['outer'] = -2
        with self.assertRaises(ValueError): module.index_report(source)


if __name__ == '__main__': unittest.main()
