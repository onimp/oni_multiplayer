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
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<MoveToSafetyChore.States>(
                        StateTransitionConfig.OnMove(nameof(MoveToSafetyChore.States.move))
                    )
                )
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
                            nameof(SleepChore.States.isDisturbedByLight),
                            nameof(SleepChore.States.isDisturbedByNoise),
                            nameof(SleepChore.States.isDisturbedByMovement),
                            nameof(SleepChore.States.isScaredOfDark)
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
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<BalloonArtistChore.States>(
                        // TODO depends on HasBalloonStallCell
                        StateTransitionConfig.OnMove(nameof(BalloonArtistChore.States.goToStand))
                    )
                )
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
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<IdleChore.States>(
                        // TODO depends on HasIdleCell
                        StateTransitionConfig.OnMove(
                            $"{nameof(IdleChore.States.idle)}.{nameof(IdleChore.States.idle.move)}"
                        )
                    )
                )
            }, {
                typeof(MingleChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<MingleChore.States>(
                        StateTransitionConfig.OnMove(nameof(MingleChore.States.move)),
                        StateTransitionConfig.OnMove(nameof(MingleChore.States.walk))
                    )
                )
            }, {
                typeof(VomitChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<VomitChore.States>(
                        StateTransitionConfig.OnMove(nameof(VomitChore.States.moveto))
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

        public static StatesTransitionConfig Enabled<T>(
            params StateTransitionConfig[] stateTransitionConfigs
        ) where T : StateMachine {
            return new StatesTransitionConfig(SyncStatus.On, typeof(T), stateTransitionConfigs);
        }

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
            Update,
            Event // TODO handle this value
        }

        public static StateTransitionConfig OnEnter(string stateName, params string[] parameterName) =>
            new(TransitionTypeEnum.Enter, stateName, parameterName);

        /// Host:
        ///  - Sends command to all clients upon exit handler call.
        /// Client:
        ///  - Prevents transition to specified state.
        ///  - Transits to WaitHostState instead.
        ///  - Transits to specified state by host upon command.
        ///  - Sets values specified by host upon command.
        public static StateTransitionConfig OnExit(string stateName, params string[] parameterName) =>
            new(TransitionTypeEnum.Exit, stateName, parameterName);

        public static StateTransitionConfig OnMove(string stateName) =>
            new(TransitionTypeEnum.Move, stateName, Array.Empty<string>());

        /// Host:
        ///  - send command to all clients upon update handler call.
        /// Client:
        ///  - prevents update handler call.
        ///  - Receives values from host.
        /// TODO: Trigger command sent on the host
        /// TODO: Execute command on the client
        public static StateTransitionConfig OnUpdate(string stateName, params string[] parameterName) =>
            new(TransitionTypeEnum.Update, stateName, parameterName);

        public static StateTransitionConfig OnEventHandler(string stateName, params string[] parameterName) =>
            new(TransitionTypeEnum.Event, stateName, parameterName);

        public StateMachine.BaseState GetMonitoredState(StateMachine sm) {
            var stateName = StateToMonitorName;
            object findInObject = sm;
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
