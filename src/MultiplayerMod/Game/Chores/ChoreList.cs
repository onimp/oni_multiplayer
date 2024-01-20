using System;
using System.Collections.Generic;
using MultiplayerMod.Game.Chores.Types;

namespace MultiplayerMod.Game.Chores;

public static class ChoreList {

    public static readonly Dictionary<Type, ChoreSyncConfig> Config =
        new() {
            {
                typeof(AttackChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(DeliverFoodChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(DieChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(DropUnusedInventoryChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(EntombedChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(MoveChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(MoveToQuarantineChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(PartyChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(PeeChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(PutOnHatChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(SighChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(StressIdleChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(SwitchRoleHatChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(TakeOffHatChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(UglyCryChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(WaterCoolerChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(ReactEmoteChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(AggressiveChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<AggressiveChore.States>(
                        StateTransitionConfig.OnExit(
                            nameof(AggressiveChore.States.findbreakable),
                            nameof(AggressiveChore.States.moveToWallTarget),
                            nameof(AggressiveChore.States.breakable)
                        )
                    )
                )
            }, {
                typeof(BingeEatChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<BingeEatChore.States>(
                        StateTransitionConfig.OnExit(
                            nameof(BingeEatChore.States.findfood),
                            nameof(BingeEatChore.States.ediblesource)
                        )
                    )
                )
            }, {
                typeof(FleeChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<FleeChore.States>(
                        StateTransitionConfig.OnExit(
                            nameof(FleeChore.States.planFleeRoute),
                            nameof(FleeChore.States.fleeToTarget)
                        )
                    )
                )
            }, {
                typeof(MournChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<MournChore.States>(
                        StateTransitionConfig.OnExit(
                            nameof(MournChore.States.findOffset),
                            nameof(MournChore.States.locator)
                        )
                    )
                )
            }, {
                typeof(BeIncapacitatedChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<BeIncapacitatedChore.States>(
                        StateTransitionConfig.OnExit(
                            $"{nameof(BeIncapacitatedChore.States.incapacitation_root)}.{nameof(BeIncapacitatedChore.States.incapacitation_root.lookingForBed)}",
                            nameof(BeIncapacitatedChore.States.clinic)
                        )
                    )
                )
            }, {
                typeof(EmoteChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(StressEmoteChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(FoodFightChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<FoodFightChore.States>(
                        StateTransitionConfig.OnExit(
                            nameof(FoodFightChore.States.emoteRoar),
                            nameof(FoodFightChore.States.attackableTarget)
                        )
                    )
                )
            }, {
                typeof(MoveToSafetyChore),
                // TODO sync SafeCellSensor instead
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(RecoverBreathChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<RecoverBreathChore.States>(
                        StateTransitionConfig.OnUpdate(
                            nameof(RecoverBreathChore.States.root),
                            nameof(RecoverBreathChore.States.locator)
                        )
                    )
                )
            }, {
                typeof(EatChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<EatChore.States>(
                        StateTransitionConfig.OnEnter(
                            nameof(EatChore.States.root),
                            nameof(EatChore.States.messstation)
                        ),
                        StateTransitionConfig.OnEventHandler(
                            nameof(EatChore.States.root),
                            GameHashes.AssignablesChanged,
                            nameof(EatChore.States.messstation)
                        ),
                        StateTransitionConfig.OnEnter(
                            nameof(EatChore.States.eatonfloorstate),
                            nameof(EatChore.States.locator)
                        )
                    )
                )
            }, {
                typeof(MovePickupableChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<MovePickupableChore.States>(
                        StateTransitionConfig.OnExit(
                            nameof(MovePickupableChore.States.success),
                            nameof(MovePickupableChore.States.actualamount),
                            nameof(MovePickupableChore.States.pickupablesource),
                            nameof(MovePickupableChore.States.requestedamount)
                        )
                    )
                )
            }, {
                typeof(SleepChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<SleepChore.States>(
                        StateTransitionConfig.OnEventHandler(
                            nameof(SleepChore.States.sleep),
                            GameHashes.SleepDisturbedByLight,
                            nameof(SleepChore.States.isDisturbedByLight)
                        ),
                        StateTransitionConfig.OnEventHandler(
                            nameof(SleepChore.States.sleep),
                            GameHashes.SleepDisturbedByNoise,
                            nameof(SleepChore.States.isDisturbedByNoise)
                        ),
                        StateTransitionConfig.OnEventHandler(
                            nameof(SleepChore.States.sleep),
                            GameHashes.SleepDisturbedByFearOfDark,
                            nameof(SleepChore.States.isScaredOfDark)
                        ),
                        StateTransitionConfig.OnEventHandler(
                            nameof(SleepChore.States.sleep),
                            GameHashes.SleepDisturbedByMovement,
                            nameof(SleepChore.States.isDisturbedByMovement)
                        )
                    )
                )
            }, {
                typeof(EquipChore),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(FixedCaptureChore),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(RancherChore),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(RescueIncapacitatedChore),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(RescueSweepBotChore),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(TakeMedicineChore),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(WorkChore<>),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(FetchAreaChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<FetchAreaChore.States>(
                        new[] {
                            StateTransitionConfig.OnExit(
                                $"{nameof(FetchAreaChore.States.delivering)}.{nameof(FetchAreaChore.States.delivering.next)}",
                                nameof(FetchAreaChore.States.deliveryDestination),
                                nameof(FetchAreaChore.States.deliveryObject)
                            ),
                            StateTransitionConfig.OnExit(
                                $"{nameof(FetchAreaChore.States.fetching)}.{nameof(FetchAreaChore.States.fetching.next)}",
                                nameof(FetchAreaChore.States.fetchTarget),
                                nameof(FetchAreaChore.States.fetchResultTarget),
                                nameof(FetchAreaChore.States.fetchAmount)
                            ),
                        }
                    )
                )
            }, {
                typeof(BalloonArtistChore),
                // TODO sync balloonArtistCellSensor instead
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(BansheeChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<BansheeChore.States>(
                        StateTransitionConfig.OnExit(
                            nameof(BansheeChore.States.findAudience),
                            nameof(BansheeChore.States.targetWailLocation)
                        ),
                        StateTransitionConfig.OnMove(nameof(BansheeChore.States.wander))
                    )
                )
            }, {
                typeof(FetchChore),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(IdleChore),
                // TODO sync idleCellSensor instead
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(MingleChore),
                // TODO sync mingleCellSensor instead
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(VomitChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<VomitChore.States>(
                        StateTransitionConfig.OnMove(nameof(VomitChore.States.moveto))
                    )
                )
            }
        };

}
