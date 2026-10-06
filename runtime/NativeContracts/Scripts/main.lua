-- Developer-only UE4SS reflection inventory. No item/currency setters, hooks or native transactions.
-- Loader compatibility with this Store build is NOT established. Not part of the gameplay PAK.
-- Withdrawn after the related QoL UE4SS 3.0.1 startup crash. This cannot repair a loader crash.
local diagnosticEnabled = false
local prefix = "[RebalanceContracts] "
local function log(kind, value)
    print(prefix .. kind .. " " .. tostring(value):gsub("[\r\n]", " ") .. "\n")
end
if not diagnosticEnabled then
    log("DISABLED", "Diagnostic withdrawn; do not install UE4SS for this probe")
    return
end
if type(StaticFindObject) ~= "function" or type(RegisterKeyBind) ~= "function"
    or type(ExecuteInGameThread) ~= "function" or type(Key) ~= "table" or not Key.F8 then
    log("DISABLED", "Required documented UE4SS APIs unavailable")
    return
end
-- All classes below occur as actual /Script/Dungeons Class imports in the supplied evidence.
local names = {
    "InventoryItem", "InventoryItemSlot", "ItemStashComponent", "ItemFunctionLibrary",
    "MerchantTransactionBase", "MerchantTransactionUtil", "MerchantCurrencyComponent",
    "UniqueCollectItem", "GildItem", "UpgradeTowerItem", "TowerMerchantUtil",
    "TowerArtisanMerchantDef", "TowerBlacksmithMerchantDef", "TowerGilderMerchantDef",
    "MissionChancesUtil", "MissionStateUtil", "MissionRequestUtil", "MissionSelectorComponent",
    "MissionDefs", "MissionOfferingsUtil", "OfferHyperMissionOfferings", "StorableItem",
    "PickupStorableComponent", "MerchantBaseWidget", "MerchantActor", "MerchantActorUtil",
    "MissionOfferingsTransactionBase", "MissionProgressComponent", "WalkPickupComponent",
    "PlayerCharacter", "BasePlayerController", "TowerFunctionLibrary"
}
local running = false
local function dump()
    if running then return end
    running = true
    local ok, err = pcall(function()
        log("BEGIN", "read-only class/function/property names; no ABI certification")
        local seen = {}
        for _, name in ipairs(names) do
            local path = "/Script/Dungeons." .. name
            local found, cls = pcall(StaticFindObject, path)
            if not found or not cls or not cls:IsValid() then
                log("MISSING", path)
            else
                for depth = 1, 12 do
                    if not cls or not cls:IsValid() then break end
                    local full = cls:GetFullName()
                    if seen[full] then break end
                    seen[full] = true
                    log("CLASS", full)
                    local count = 0
                    local funcs_ok, funcs_err = pcall(function()
                        cls:ForEachFunction(function(fn)
                            count = count + 1
                            log("FUNCTION", fn:GetFullName() .. " flags=" .. tostring(fn:GetFunctionFlags()))
                            if count >= 256 then log("TRUNCATED", "functions " .. full); return true end
                        end)
                    end)
                    if not funcs_ok then log("FUNCTION_ERROR", funcs_err) end
                    count = 0
                    local props_ok, props_err = pcall(function()
                        cls:ForEachProperty(function(prop)
                            count = count + 1
                            log("PROPERTY", prop:GetFullName())
                            if count >= 256 then log("TRUNCATED", "properties " .. full); return true end
                        end)
                    end)
                    if not props_ok then log("PROPERTY_ERROR", props_err) end
                    cls = cls:GetSuperStruct()
                end
            end
        end
        log("END", "Names/flags only; offsets, parameter direction and semantics remain unverified")
    end)
    running = false
    if not ok then log("ERROR", err) end
end
RegisterKeyBind(Key.F8, function() ExecuteInGameThread(dump) end)
log("READY", "F8 requests read-only contract discovery on the game thread")
