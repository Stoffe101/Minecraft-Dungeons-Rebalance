# Observed native call index

Observed import candidates and argument expressions only. No function parameter direction, native implementation, payment atomicity, or authority contract is certified.

| Native import candidate | Observed argument counts | Call sites |
| --- | --- | --- |
| `/Script/Dungeons.ActorQuery.GetGameBP` | 1 | 1 |
| `/Script/Dungeons.ActorQuery.GetPlayerCharactersInRange` | 5 | 1 |
| `/Script/Dungeons.BaseCharacter.IsFrozenSolid` | 0 | 1 |
| `/Script/Dungeons.BasePlayerController.GetControlledPlayerCharacter` | 0 | 8 |
| `/Script/Dungeons.BasePlayerController.GetItemStashComponent` | 0 | 1 |
| `/Script/Dungeons.BasePlayerController.IsHUDCreated` | 0 | 1 |
| `/Script/Dungeons.BasePlayerController.IsInputCapturedByTeleport` | 0 | 3 |
| `/Script/Dungeons.BasePlayerController.IsInputTypeAllowed` | 2 | 40 |
| `/Script/Dungeons.BasePlayerController.IsTeleportListOpen` | 0 | 7 |
| `/Script/Dungeons.BasePlayerController.IsTowerPlayer` | 0 | 1 |
| `/Script/Dungeons.BasePlayerController.OnAnyPlayerDamaged` | 0 | 1 |
| `/Script/Dungeons.BasePlayerController.OnDebugPointer` | 1 | 2 |
| `/Script/Dungeons.BasePlayerController.OnDebugState` | 0 | 1 |
| `/Script/Dungeons.BasePlayerController.OnDodgeButton` | 1 | 2 |
| `/Script/Dungeons.BasePlayerController.OnDodgeForwardButton` | 1 | 2 |
| `/Script/Dungeons.BasePlayerController.OnMoveButton` | 1 | 2 |
| `/Script/Dungeons.BasePlayerController.OnRangedAttackButton` | 1 | 2 |
| `/Script/Dungeons.BasePlayerController.OnRootPlayer` | 1 | 2 |
| `/Script/Dungeons.BasePlayerController.SetInputCapturedByUI` | 3 | 4 |
| `/Script/Dungeons.BasePlayerController.SetPlayerIsImmovable` | 1 | 1 |
| `/Script/Dungeons.ControllerTypeManager.GetControllerType` | 1 | 2 |
| `/Script/Dungeons.ControllerTypeManager.HideTouchControl` | 0 | 1 |
| `/Script/Dungeons.ControllerTypeManager.ShowTouchControl` | 1 | 1 |
| `/Script/Dungeons.DungeonHUD.SetHotbarBackgroundVisibility` | 1 | 1 |
| `/Script/Dungeons.DungeonHUD.SetIsVisible` | 1 | 1 |
| `/Script/Dungeons.DungeonsGameInstance.GetCachedUIWidget` | 1 | 1 |
| `/Script/Dungeons.DungeonsGameInstance.GetControllerTypeManager` | 0 | 6 |
| `/Script/Dungeons.DungeonsGameInstance.GetLocalPlayerUserCount` | 0 | 3 |
| `/Script/Dungeons.DungeonsGameInstance.GetTowerManager` | 0 | 2 |
| `/Script/Dungeons.DungeonsGameInstance.GetUserManager` | 0 | 2 |
| `/Script/Dungeons.DungeonsGameInstance.HandleLocalPlayerLeave` | 1 | 1 |
| `/Script/Dungeons.DungeonsGameInstance.SetCachedUIWidget` | 1 | 1 |
| `/Script/Dungeons.DungeonsGameState.IsAnyPlayerMatchingAliveState` | 1 | 1 |
| `/Script/Dungeons.DungeonsUserManager.GetAllLocalPlayerControllers` | 0 | 2 |
| `/Script/Dungeons.DungeonsVisiblityRootWidget.GetVisibleRecursive` | 0 | 1 |
| `/Script/Dungeons.DungeonsWidgetSwitcher.GetActiveWidget` | 0 | 4 |
| `/Script/Dungeons.GameBP.RingAtObjective` | 0 | 1 |
| `/Script/Dungeons.GameVersion.PlatformCheckMatch` | 1 | 1 |
| `/Script/Dungeons.HealthComponent.Kill` | 0 | 1 |
| `/Script/Dungeons.InteractableComponent.DisableInteraction` | 0 | 1 |
| `/Script/Dungeons.ItemFunctionLibrary.BreakItemId` | 1 | 1 |
| `/Script/Dungeons.ItemStashComponent.GetCanOpenWithKeyCommand` | 0 | 1 |
| `/Script/Dungeons.MerchantActor.SimulateInteraction` | 1 | 1 |
| `/Script/Dungeons.MerchantActorUtil.GetFirstSelectMissionOfferingsMerchant` | 2 | 1 |
| `/Script/Dungeons.MerchantBaseWidget.GetCurrencyComponent` | 0 | 2 |
| `/Script/Dungeons.MerchantBaseWidget.GetDisplayName` | 0 | 4 |
| `/Script/Dungeons.MerchantBaseWidget.GetMerchantActorOwner` | 0 | 2 |
| `/Script/Dungeons.MerchantBaseWidget.InitialBindingUpdate` | 0 | 2 |
| `/Script/Dungeons.MerchantBaseWidget.IsTowerWidget` | 0 | 1 |
| `/Script/Dungeons.MerchantBaseWidget.OnOpenClose` | 1 | 2 |
| `/Script/Dungeons.MerchantBaseWidget.ShouldShowEnchantmentPoints` | 0 | 1 |
| `/Script/Dungeons.MerchantTransactionBase.QueryProblemStatus` | 1 | 2 |
| `/Script/Dungeons.MerchantTransactionUtil.GetTransactionReasonText` | 2 | 1 |
| `/Script/Dungeons.MissionChancesUtil.GetMissionProbabilities` | 1 | 1 |
| `/Script/Dungeons.MissionDefs.GetMissionDisplayName` | 1 | 3 |
| `/Script/Dungeons.MissionDefs.GetMissionRequiresOfferings` | 1 | 1 |
| `/Script/Dungeons.MissionDefs.IsTowerMission` | 1 | 4 |
| `/Script/Dungeons.MissionOfferingsTransactionBase.GetIsItemBeingOffered` | 1 | 1 |
| `/Script/Dungeons.MissionOfferingsTransactionBase.GetMission` | 0 | 2 |
| `/Script/Dungeons.MissionOfferingsTransactionBase.GetMissionOfferings` | 0 | 2 |
| `/Script/Dungeons.MissionOfferingsUtil.GetOfferingsArchetypeCounts` | 1 | 1 |
| `/Script/Dungeons.MissionProgressComponent.ClearMissionState` | 2 | 1 |
| `/Script/Dungeons.MissionProgressComponent.IsMissionUnlocked` | 2 | 1 |
| `/Script/Dungeons.MissionQuery.GetLevelName` | 1 | 4 |
| `/Script/Dungeons.MissionRequestUtil.CreateMissionRequest` | 5 | 1 |
| `/Script/Dungeons.MissionRequestUtil.GetMissionState` | 1 | 8 |
| `/Script/Dungeons.MissionStateUtil.GetBulletPoints` | 1 | 1 |
| `/Script/Dungeons.MissionStateUtil.GetDifficulty` | 1 | 1 |
| `/Script/Dungeons.MissionStateUtil.GetLevelName` | 1 | 7 |
| `/Script/Dungeons.OnlineUtil.IsOnlineSession` | 0 | 5 |
| `/Script/Dungeons.PlayerCharacter.GetPlayerColor` | 0 | 1 |
| `/Script/Dungeons.PlayerCharacter.GetTeleportCandidates` | 0 | 1 |
| `/Script/Dungeons.PlayerCharacter.HasPendingRewardItem` | 0 | 1 |
| `/Script/Dungeons.PlayerCharacterFunctionLibrary.CountLocalPlayerCharacters` | 1 | 1 |
| `/Script/Dungeons.PlayerCharacterFunctionLibrary.GetLocalPlayerCharacters` | 1 | 4 |
| `/Script/Dungeons.PlayerControllerBase.IsOwnedByInitialLocalPlayer` | 0 | 20 |
| `/Script/Dungeons.ProjectileFunctionLibrary.SpawnHitParticleEffect` | 4 | 1 |
| `/Script/Dungeons.SettingsBlueprintFunctionLibrary.GetSettingFromSave` | 3 | 1 |
| `/Script/Dungeons.SoundMixManager.PopSoundMix` | 1 | 2 |
| `/Script/Dungeons.SoundMixManager.PushSoundMix` | 1 | 2 |
| `/Script/Dungeons.TemporalUtils.DilateTime` | 5 | 1 |
| `/Script/Dungeons.TowerFunctionLibrary.HasRewardsToGive` | 4 | 1 |
| `/Script/Dungeons.TowerMerchantUtil.OpenTowerMerchant` | 2 | 3 |
| `/Script/Dungeons.TowerRewardSelectorComponent.CanShowTimer` | 0 | 1 |
| `/Script/Dungeons.TowerRewardSelectorComponent.GetCurrentSelectingPlayerController` | 0 | 2 |
| `/Script/Dungeons.TowerRewardSelectorComponent.GetSelectionTimeLeft` | 0 | 1 |
| `/Script/Dungeons.WalkPickupComponent.ResetPickup` | 0 | 1 |
| `/Script/Dungeons.WidgetHelper.DestroyObject` | 1 | 1 |
| `/Script/Dungeons.WidgetHelper.GetAllDescendentWidgetsOfClass` | 2 | 1 |
| `/Script/Dungeons.WidgetHelper.GetAllDescendentWidgetsWithInterface` | 2 | 11 |
| `/Script/Dungeons.WidgetHelper.GetFirstParentWidgetWithInterface` | 2 | 1 |
| `/Script/Dungeons.WidgetHelper.LoadClassAsset_Blocking` | 1 | 4 |
| `/Script/Dungeons.WidgetHelper.SetOwnerRecursive` | 2 | 3 |

Inputs: 39 cooked-package metadata reports. JSON companion records exact export/statement paths, receiver expressions, arguments and metadata hashes.

22 call sites contain an unresolved pointer/name marker in the supplied serializer output. These are retained and flagged, not repaired by guessing.

Counts refer to serialized caller arguments; these may include out parameters. A unique import candidate does not prove native eligibility, mutation behavior or multiplayer authority.
