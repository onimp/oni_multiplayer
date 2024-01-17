using System;
using System.Collections.Generic;

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
                    new StatesTransitionConfig(
                        typeof(AggressiveChore.States),
                        new[] {
                            StateTransitionConfig.OnExit(
                                nameof(AggressiveChore.States.findbreakable),
                                nameof(AggressiveChore.States.moveToWallTarget),
                                nameof(AggressiveChore.States.breakable)
                            )
                        }
                    )
                )
            }, {
                typeof(BingeEatChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(BingeEatChore.States),
                        new[] {
                            StateTransitionConfig.OnExit(
                                nameof(BingeEatChore.States.findfood),
                                nameof(BingeEatChore.States.ediblesource)
                            )
                        }
                    )
                )
            }, {
                typeof(FleeChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(FleeChore.States),
                        new[] {
                            StateTransitionConfig.OnExit(
                                nameof(FleeChore.States.planFleeRoute),
                                nameof(FleeChore.States.fleeToTarget)
                            )
                        }
                    )
                )
            }, {
                typeof(MournChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(MournChore.States),
                        new[] {
                            StateTransitionConfig.OnExit(
                                nameof(MournChore.States.findOffset),
                                nameof(MournChore.States.locator)
                            )
                        }
                    )
                )
            }, {
                typeof(BeIncapacitatedChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(BeIncapacitatedChore.States),
                        new[] {
                            StateTransitionConfig.OnExit(
                                $"{nameof(BeIncapacitatedChore.States.incapacitation_root)}.{nameof(BeIncapacitatedChore.States.incapacitation_root.lookingForBed)}",
                                nameof(BeIncapacitatedChore.States.clinic)
                            )
                        }
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
                    new StatesTransitionConfig(
                        typeof(FoodFightChore.States),
                        new[] {
                            StateTransitionConfig.OnExit(
                                nameof(FoodFightChore.States.emoteRoar),
                                nameof(FoodFightChore.States.attackableTarget)
                            )
                        }
                    )
                )
            }, {
                typeof(MoveToSafetyChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(MoveToSafetyChore.States),
                        new[] {
                            StateTransitionConfig.OnMove(nameof(MoveToSafetyChore.States.move))
                        }
                    )
                )
            }, {
                typeof(RecoverBreathChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(RecoverBreathChore.States),
                        new[] {
                            StateTransitionConfig.OnUpdate(
                                nameof(RecoverBreathChore.States.root),
                                nameof(RecoverBreathChore.States.locator)
                            )
                        }
                    )
                )
            }, {
                typeof(EatChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(EatChore.States),
                        new[] {
                            StateTransitionConfig.OnEnter(
                                nameof(EatChore.States.root),
                                nameof(EatChore.States.messstation)
                            ),
                            StateTransitionConfig.OnEventHandler(
                                nameof(EatChore.States.root),
                                nameof(EatChore.States.messstation)
                            ),
                            StateTransitionConfig.OnEnter(
                                nameof(EatChore.States.eatonfloorstate),
                                nameof(EatChore.States.locator)
                            )
                        }
                    )
                )
            }, {
                typeof(MovePickupableChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(MovePickupableChore.States),
                        new[] {
                            StateTransitionConfig.OnExit(
                                nameof(MovePickupableChore.States.success),
                                nameof(MovePickupableChore.States.actualamount),
                                nameof(MovePickupableChore.States.pickupablesource),
                                nameof(MovePickupableChore.States.requestedamount)
                            )
                        }
                    )
                )
            }, {
                typeof(SleepChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(SleepChore.States),
                        new[] {
                            StateTransitionConfig.OnEventHandler(
                                nameof(SleepChore.States.sleep),
                                nameof(SleepChore.States.isDisturbedByLight),
                                nameof(SleepChore.States.isDisturbedByNoise),
                                nameof(SleepChore.States.isDisturbedByMovement),
                                nameof(SleepChore.States.isScaredOfDark)
                            )
                        }
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
                    new StatesTransitionConfig(
                        typeof(FetchAreaChore.States),
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
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(BalloonArtistChore.States),
                        // TODO depends on HasBalloonStallCell
                        new[] { StateTransitionConfig.OnMove(nameof(BalloonArtistChore.States.goToStand)) }
                    )
                )
            }, {
                typeof(BansheeChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(BansheeChore.States),
                        new[] {
                            StateTransitionConfig.OnExit(
                                nameof(BansheeChore.States.findAudience),
                                nameof(BansheeChore.States.targetWailLocation)
                            ),
                            StateTransitionConfig.OnMove(nameof(BansheeChore.States.wander))
                        }
                    )
                )
            }, {
                typeof(FetchChore),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(IdleChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(IdleChore.States),
                        // TODO depends on HasIdleCell
                        new[] {
                            StateTransitionConfig.OnMove(
                                $"{nameof(IdleChore.States.idle)}.{nameof(IdleChore.States.idle.move)}"
                            )
                        }
                    )
                )
            }, {
                typeof(MingleChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(MingleChore.States),
                        new[] {
                            // TODO depends on hasMingleCell
                            StateTransitionConfig.OnMove(nameof(MingleChore.States.move)),
                            StateTransitionConfig.OnMove(nameof(MingleChore.States.walk))
                        }
                    )
                )
            }, {
                typeof(VomitChore),
                ChoreSyncConfig.Dynamic(
                    new StatesTransitionConfig(
                        typeof(VomitChore.States),
                        new[] { StateTransitionConfig.OnMove(nameof(VomitChore.States.moveto)) }
                    )
                )
            }
        };

    public record ChoreSyncConfig(
        CreationStatusEnum CreationSync,
        GlobalChoreProviderStatusEnum GlobalChoreProviderSync,
        StatesTransitionConfig StatesTransitionSync
    ) {
        public static ChoreSyncConfig FullyDeterminedByInput(
            GlobalChoreProviderStatusEnum globalChoreStatus = GlobalChoreProviderStatusEnum.Off
        ) {
            return new ChoreSyncConfig(
                CreationStatusEnum.On,
                globalChoreStatus,
                StatesTransitionConfig.Disabled()
            );
        }

        public static ChoreSyncConfig Dynamic(StatesTransitionConfig statesTransitionConfig) {
            return new ChoreSyncConfig(
                CreationStatusEnum.On,
                GlobalChoreProviderStatusEnum.Off,
                statesTransitionConfig
            );
        }
    }

    public enum CreationStatusEnum {
        On,
        Off
    }

    public enum GlobalChoreProviderStatusEnum {
        // TODO implement sync using this value.
        OnTodo,
        Off
    }

    public record StatesTransitionConfig(
        StatesTransitionConfig.SyncStatus Status,
        Type StateType,
        StateTransitionConfig[] StateTransitionConfigs
    ) {
        public enum SyncStatus {
            On,
            Off
        }

        public StatesTransitionConfig(
            Type StateType,
            StateTransitionConfig[] StateTransitionConfigs
        ) : this(SyncStatus.On, StateType, StateTransitionConfigs) { }

        public static StatesTransitionConfig Disabled() {
            return new StatesTransitionConfig(SyncStatus.Off, null!, Array.Empty<StateTransitionConfig>());
        }
    }

    public record StateTransitionConfig(
        StateTransitionConfig.TransitionTypeEnum TransitionType,
        string StateToMonitorName,
        string[] ParameterName
    ) {
        public enum TransitionTypeEnum {
            Enter, // TODO handle this value
            Exit,
            Move, // TODO handle this value
            Update, // TODO handle this value
            Event // TODO handle this value
        }

        public static StateTransitionConfig OnEnter(string stateName, params string[] parameterName) =>
            new(TransitionTypeEnum.Enter, stateName, parameterName);

        public static StateTransitionConfig OnExit(string stateName, params string[] parameterName) =>
            new(TransitionTypeEnum.Exit, stateName, parameterName);

        public static StateTransitionConfig OnMove(string stateName) =>
            new(TransitionTypeEnum.Move, stateName, Array.Empty<string>());

        public static StateTransitionConfig OnUpdate(string stateName, params string[] parameterName) =>
            new(TransitionTypeEnum.Update, stateName, parameterName);

        public static StateTransitionConfig OnEventHandler(string stateName, params string[] parameterName) =>
            new(TransitionTypeEnum.Event, stateName, parameterName);

        public StateMachine.BaseState GetMonitoredState(StateMachine.Instance smi) {
            var stateName = StateToMonitorName;
            object findInObject = smi;
            while (stateName.Contains(".")) {
                var firstSplit = StateToMonitorName.IndexOf('.');
                findInObject = findInObject.GetType().GetField(stateName.Substring(0, firstSplit))
                    .GetValue(findInObject);
                stateName = stateName.Substring(firstSplit + 1);
            }
            return (StateMachine.BaseState) findInObject.GetType().GetField(stateName).GetValue(findInObject);
        }
    }
}
