"""Policy/choice/receipt tests with synthetic adapter; NOT retail native mutation tests."""
import json,sys,unittest
from dataclasses import replace
from pathlib import Path
sys.path.insert(0,str(Path(__file__).resolve().parents[1]/'src'))
from rebalance.upgrades import *
CFG=json.loads((Path(__file__).resolve().parents[1]/'config/balance.json').read_text())
class FakeNative:
    def __init__(self):
        self.capabilities=Capabilities(True,True,True,True,True)
        self.item=ItemSnapshot('owner','instance','revision1','base','family','rare',False,'150','enchants+gild+favorite')
        self.options=(UniqueOption('first','family','First result','/Game/Fixture/First','First description',('first effect',)),UniqueOption('second','family','Second result','/Game/Fixture/Second','Second description',('second effect',)))
        self.view=NativeView(self.item,self.options,5000,500,'catalog1',True,tuple(Service))
        self.calls=[];self.fail=False;self.throw=False;self.wrong=False
    def read_view(self,owner,instance):return self.view
    def execute_atomic(self,p):
        self.calls.append(p)
        if self.throw:raise RuntimeError('Unknown native completion')
        if self.fail:return Receipt(p.request_id,False,'',0,'rejected')
        return Receipt(p.request_id,True,'unexpected' if self.wrong else p.selected_type_id,p.quote.price)
class UpgradeTests(unittest.TestCase):
    def setUp(self):
        self.native=FakeNative();self.now=10;self.core=UpgradeCore(self.native,CFG,clock=lambda:self.now)
    def quote(self):return self.core.quote('owner','instance',Service.UNIQUE)
    def test_names_icons_information_and_chosen_second_result(self):
        q=self.quote();rows=q.choice_rows();self.assertEqual([r['name'] for r in rows],['First result','Second result']);self.assertEqual(rows[1]['iconAsset'],'/Game/Fixture/Second');self.assertEqual(rows[1]['innateEffects'],('second effect',))
        r=self.core.confirm('owner',q.token,'second');self.assertTrue(r.success);self.assertEqual(r.result_type_id,'second');self.assertEqual(r.charged,2500);self.assertEqual(self.native.calls[0].quote.item.preservation_fingerprint,self.native.item.preservation_fingerprint)
    def test_missing_or_other_family_choice_never_purchases(self):
        q=self.quote()
        for chosen in [None,'unrelated']:
            with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,chosen)
        self.assertEqual(self.native.calls,[])
    def test_unverified_bridge_previews_but_cannot_buy(self):
        self.native.capabilities=Capabilities();q=self.quote();self.assertFalse(q.purchase_enabled);self.assertEqual(len(q.choice_rows()),2)
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
        self.assertEqual(self.native.calls,[])
    def test_native_capability_revoked_after_confirmation_blocks_purchase(self):
        q=self.quote();self.native.capabilities=Capabilities()
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
        self.assertEqual(self.native.calls,[])
    def test_changed_item_and_reused_slot_rejected(self):
        q=self.quote();self.native.view=replace(self.native.view,item=replace(self.native.item,revision='revision2'))
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
        self.native.view=replace(self.native.view,item=replace(self.native.item,instance_id='replacement'))
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
        self.assertEqual(self.native.calls,[])
    def test_cross_player_confirmation_rejected(self):
        q=self.quote()
        with self.assertRaises(UpgradeError):self.core.confirm('other',q.token,'first')
        self.assertEqual(self.native.calls,[])
    def test_balance_rechecked_before_commit(self):
        q=self.quote();self.native.view=replace(self.native.view,emeralds=2499)
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
        self.assertEqual(self.native.calls,[])
    def test_catalog_changed_rejected(self):
        q=self.quote();self.native.view=replace(self.native.view,catalog_revision='catalog2')
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
        self.assertEqual(self.native.calls,[])
    def test_outcomes_changed_rejected(self):
        q=self.quote();self.native.view=replace(self.native.view,options=self.native.options[:1])
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
    def test_duplicate_confirm_returns_same_receipt_without_second_call(self):
        q=self.quote();r=self.core.confirm('owner',q.token,'first');self.assertEqual(self.core.confirm('owner',q.token,'first'),r);self.assertEqual(len(self.native.calls),1)
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'second')
    def test_unknown_completion_and_wrong_result_block_retries(self):
        for field in ['throw','wrong']:
            with self.subTest(field=field):
                self.setUp();setattr(self.native,field,True);q=self.quote()
                with self.assertRaises((RuntimeError,UpgradeError)):self.core.confirm('owner',q.token,'first')
                with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
                self.assertEqual(len(self.native.calls),1)
    def test_native_failure_records_no_charge(self):
        self.native.fail=True;q=self.quote();r=self.core.confirm('owner',q.token,'first');self.assertFalse(r.success);self.assertEqual(r.charged,0)
    def test_expired_quote_rejected(self):
        q=self.quote();self.now=130
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
    def test_invalid_catalog_and_missing_presentation_rejected(self):
        for options in [(),self.native.options*2,(replace(self.native.options[0],family_id='other'),),(replace(self.native.options[0],icon_asset=''),)]:
            self.native.view=replace(self.native.view,options=options)
            with self.assertRaises(UpgradeError):self.quote()
        self.native.view=replace(self.native.view,options=self.native.options,catalog_authoritative=False)
        with self.assertRaises(UpgradeError):self.quote()
    def test_service_eligibility_and_agreed_prices(self):
        self.native.view=replace(self.native.view,item=replace(self.native.item,rarity='common'))
        q=self.core.quote('owner','instance',Service.RARE);self.assertEqual(q.price,750);self.assertEqual(q.currency,'emeralds')
        q=self.core.quote('owner','instance',Service.GILD);self.assertEqual(q.price,150);self.assertEqual(q.currency,'gold')
        with self.assertRaises(UpgradeError):self.quote()
        with self.assertRaises(UpgradeError):self.core.quote('owner','instance',Service.REROLL)
        self.native.view=replace(self.native.view,item=replace(self.native.item,gilded=True));q=self.core.quote('owner','instance',Service.REROLL);self.assertEqual(q.price,250)
    def test_price_config_mutation_does_not_change_confirmation(self):
        cfg=json.loads(json.dumps(CFG));core=UpgradeCore(self.native,cfg);q=core.quote('owner','instance',Service.UNIQUE);cfg['smiths']['rareToUniqueEmeralds']=1;self.assertEqual(q.price,2500);self.assertEqual(core.quote('owner','instance',Service.UNIQUE).price,2500)
    def test_native_ineligible_artifact_or_changed_eligibility_blocks_purchase(self):
        q=self.quote();self.native.view=replace(self.native.view,eligible_services=())
        with self.assertRaises(UpgradeError):self.core.confirm('owner',q.token,'first')
        with self.assertRaises(UpgradeError):self.quote()
        self.assertEqual(self.native.calls,[])
if __name__=='__main__':unittest.main()
