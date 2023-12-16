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
                    new StateTransitionConfig(
                        typeof(AggressiveChore.States),
                        nameof(AggressiveChore.States.findbreakable),
                        new[] {
                            nameof(AggressiveChore.States.moveToWallTarget), nameof(AggressiveChore.States.breakable)
                        }
                    )
                )
            }, {
                typeof(BingeEatChore),
                ChoreSyncConfig.Dynamic(
                    new StateTransitionConfig(
                        typeof(BingeEatChore.States),
                        nameof(BingeEatChore.States.findfood),
                        new[] { nameof(BingeEatChore.States.ediblesource) }
                    )
                )
            }, {
                typeof(FleeChore),
                ChoreSyncConfig.Dynamic(
                    new StateTransitionConfig(
                        typeof(FleeChore.States),
                        nameof(FleeChore.States.planFleeRoute),
                        new[] { nameof(FleeChore.States.fleeToTarget) }
                    )
                )
            }, {
                typeof(MournChore),
                ChoreSyncConfig.Dynamic(
                    new StateTransitionConfig(
                        typeof(MournChore.States),
                        nameof(MournChore.States.findOffset),
                        new[] { nameof(MournChore.States.locator) }
                    )
                )
            }, {
                typeof(BeIncapacitatedChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(EmoteChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(StressEmoteChore),
                ChoreSyncConfig.FullyDeterminedByInput()
            }, {
                typeof(FoodFightChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(MoveToSafetyChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(RecoverBreathChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(EatChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(MovePickupableChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(SleepChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
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
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(BalloonArtistChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(BansheeChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(FetchChore),
                ChoreSyncConfig.FullyDeterminedByInput(GlobalChoreProviderStatusEnum.OnTodo)
            }, {
                typeof(IdleChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(MingleChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }, {
                typeof(VomitChore),
                ChoreSyncConfig.Dynamic(StateTransitionConfig.TODO_SpecifyConfig())
            }
        };

    public record ChoreSyncConfig(
        CreationStatusEnum CreationSync,
        GlobalChoreProviderStatusEnum GlobalChoreProviderSync,
        StateTransitionConfig StateTransitionSync
    ) {
        public static ChoreSyncConfig FullyDeterminedByInput(
            GlobalChoreProviderStatusEnum globalChoreStatus = GlobalChoreProviderStatusEnum.Off
        ) {
            return new ChoreSyncConfig(
                CreationStatusEnum.On,
                globalChoreStatus,
                StateTransitionConfig.Disabled()
            );
        }

        public static ChoreSyncConfig Dynamic(StateTransitionConfig stateTransitionConfig) {
            return new ChoreSyncConfig(CreationStatusEnum.On, GlobalChoreProviderStatusEnum.Off, stateTransitionConfig);
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

    public record StateTransitionConfig(
        StateTransitionConfig.SyncStatus Status,
        Type StateType,
        string StateToMonitorName,
        string[] ParameterName
    ) {
        public enum SyncStatus {
            On,
            Off
        }

        public StateTransitionConfig(
            Type StateType,
            string StateToMonitorName,
            string[] ParameterName
        ) : this(SyncStatus.On, StateType, StateToMonitorName, ParameterName) { }

        public static StateTransitionConfig Disabled() {
            return new StateTransitionConfig(SyncStatus.Off, null!, null!, null!);
        }

        public static StateTransitionConfig TODO_SpecifyConfig() => Disabled();
    };
}
