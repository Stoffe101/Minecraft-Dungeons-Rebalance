"""Upgrade choice/confirmation core. Runtime integration must supply a verified native adapter.

Never infer Unique families from asset suffixes or reconstruct saved items. This model is
not loaded by Minecraft Dungeons and makes no claim that native mutations work yet.
"""
from dataclasses import dataclass
from enum import Enum
from typing import Protocol
import time
import uuid

class UpgradeError(ValueError):
    pass

class Service(str, Enum):
    RARE = 'common_to_rare'
    UNIQUE = 'rare_to_unique'
    GILD = 'add_gilded'
    REROLL = 'reroll_gilded'

@dataclass(frozen=True)
class UniqueOption:
    type_id: str
    family_id: str
    name: str
    icon_asset: str
    description: str
    innate_effects: tuple[str, ...]
    preview_stats: tuple[tuple[str, str], ...] = ()

@dataclass(frozen=True)
class ItemSnapshot:
    owner_id: str
    instance_id: str
    revision: str
    type_id: str
    family_id: str
    rarity: str
    gilded: bool
    power: str
    preservation_fingerprint: str
    location: str = 'inventory'

@dataclass(frozen=True)
class NativeView:
    item: ItemSnapshot
    options: tuple[UniqueOption, ...]
    emeralds: int
    gold: int
    catalog_revision: str
    catalog_authoritative: bool
    eligible_services: tuple[Service, ...] = ()

@dataclass(frozen=True)
class Capabilities:
    # These flags require audited native contracts; existence of a class name is insufficient.
    atomic_charge_and_mutate: bool = False
    selected_unique_result: bool = False
    preserves_item_state: bool = False
    idempotent_receipts: bool = False
    camp_persistent_inventory: bool = False

@dataclass(frozen=True)
class Quote:
    token: str
    owner_id: str
    service: Service
    item: ItemSnapshot
    options: tuple[UniqueOption, ...]
    catalog_revision: str
    currency: str
    price: int
    expires_at: float
    purchase_enabled: bool

    def choice_rows(self):
        # View model for native UMG rows. Icons/text must be supplied by native item presentation.
        return tuple(dict(typeId=o.type_id, name=o.name, iconAsset=o.icon_asset,
                          description=o.description, innateEffects=o.innate_effects,
                          previewStats=dict(o.preview_stats)) for o in self.options)

@dataclass(frozen=True)
class Purchase:
    request_id: str
    quote: Quote
    selected_type_id: str

@dataclass(frozen=True)
class Receipt:
    request_id: str
    success: bool
    result_type_id: str
    charged: int
    reason: str = ''

class NativeAdapter(Protocol):
    capabilities: Capabilities
    def read_view(self, owner_id: str, instance_id: str) -> NativeView: ...
    def execute_atomic(self, purchase: Purchase) -> Receipt:
        """Revalidate ownership/revision/catalog/balance atomically on native authority.

        Must preserve the existing item, apply EXACT selected Unique type, charge only
        on success, save/notify natively, and deduplicate request_id. No random fallback.
        Failure must leave both item and wallet unchanged. Not implemented in retail yet.
        """
        ...

class UpgradeCore:
    def __init__(self, adapter: NativeAdapter, balance: dict, clock=time.monotonic):
        self.adapter, self.clock = adapter, clock
        # Copy agreed prices: later caller/config mutation cannot alter a confirmation.
        c = balance['smiths']
        self.prices = {Service.RARE: ('emeralds', c['commonToRareEmeralds']),
                       Service.UNIQUE: ('emeralds', c['rareToUniqueEmeralds']),
                       Service.GILD: ('gold', c['addGildedGold']),
                       Service.REROLL: ('gold', c['rerollGildedGold'])}
        if any(type(p) is not int or p <= 0 for _, p in self.prices.values()):
            raise UpgradeError('Invalid smith price')
        self.quotes, self.receipts, self.pending = {}, {}, set()

    def _view(self, owner, instance):
        v = self.adapter.read_view(owner, instance)
        if v.item.owner_id != owner or v.item.instance_id != instance or v.item.location != 'inventory':
            raise UpgradeError('Item ownership/location changed')
        if not all([v.item.revision, v.item.preservation_fingerprint, v.item.family_id,
                    v.catalog_revision]) or not v.catalog_authoritative:
            raise UpgradeError('Native item/catalog data unresolved')
        return v

    def _enabled(self, service):
        caps = self.adapter.capabilities
        return all([caps.atomic_charge_and_mutate, caps.preserves_item_state,
                    caps.idempotent_receipts, caps.camp_persistent_inventory,
                    service is not Service.UNIQUE or caps.selected_unique_result])

    def quote(self, owner, instance, service: Service):
        service = Service(service)
        v = self._view(owner, instance)
        i = v.item
        if service not in v.eligible_services:
            raise UpgradeError('Native eligibility does not permit this service')
        if ((service is Service.RARE and i.rarity != 'common') or
            (service is Service.UNIQUE and i.rarity != 'rare') or
            (service is Service.GILD and i.gilded) or
            (service is Service.REROLL and not i.gilded)):
            raise UpgradeError('Service not valid for this item')
        options = tuple(v.options) if service is Service.UNIQUE else ()
        if service is Service.UNIQUE:
            if not options or len({o.type_id for o in options}) != len(options):
                raise UpgradeError('Missing/duplicate native Unique outcomes')
            if any(o.family_id != i.family_id or not all([o.type_id, o.name, o.icon_asset,
                                                        o.description]) for o in options):
                raise UpgradeError('Invalid/incomplete native Unique outcome')
        currency, price = self.prices[service]
        enabled = self._enabled(service)
        q = Quote(uuid.uuid4().hex, owner, service, i, options, v.catalog_revision,
                  currency, price, self.clock() + 120, enabled)
        self.quotes[q.token] = q
        return q

    def confirm(self, owner, token, selected_type_id=None):
        q = self.quotes.get(token)
        if q is None or q.owner_id != owner:
            raise UpgradeError('Unknown confirmation')
        if token in self.receipts:
            receipt, selected = self.receipts[token]
            if selected_type_id != selected:
                raise UpgradeError('Confirmation selection changed')
            return receipt
        if token in self.pending:
            raise UpgradeError('Confirmation pending native receipt; do not retry mutation')
        if self.clock() >= q.expires_at:
            raise UpgradeError('Confirmation expired')
        if not q.purchase_enabled or not self._enabled(q.service):
            raise UpgradeError('Native Camp upgrade contract not verified; preview only')
        if q.service is Service.UNIQUE:
            if selected_type_id not in {o.type_id for o in q.options}:
                raise UpgradeError('Choose an available Unique result explicitly')
            target = selected_type_id
        else:
            if selected_type_id is not None:
                raise UpgradeError('This service has no Unique choice')
            target = q.item.type_id
        view = self._view(owner, q.item.instance_id)
        if q.service not in view.eligible_services:
            raise UpgradeError('Native service eligibility changed')
        if view.item != q.item or view.catalog_revision != q.catalog_revision:
            raise UpgradeError('Item/catalog changed; request a fresh confirmation')
        if q.service is Service.UNIQUE and view.options != q.options:
            raise UpgradeError('Unique outcomes changed')
        if getattr(view, q.currency) < q.price:
            raise UpgradeError('Insufficient currency')
        self.pending.add(token)
        # The native adapter must revalidate inside its atomic transaction, not trust this snapshot.
        receipt = self.adapter.execute_atomic(Purchase(token, q, target))
        if (receipt.request_id != token or
            (receipt.success and (receipt.result_type_id != target or receipt.charged != q.price)) or
            (not receipt.success and receipt.charged != 0)):
            # An ambiguous/bad native result must never trigger a second mutation or a guessed refund.
            raise UpgradeError('Native receipt mismatch; confirmation retained pending')
        self.pending.remove(token)
        self.receipts[token] = (receipt, selected_type_id)
        return receipt
