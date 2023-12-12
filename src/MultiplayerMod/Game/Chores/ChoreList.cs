using System;
using System.Collections.Generic;

namespace MultiplayerMod.Game.Chores;

public static class ChoreList {

    public static readonly Dictionary<Type, ChoreSyncConfig> Config =
        new() {
            {
                typeof(AttackChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(DeliverFoodChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(DieChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(DropUnusedInventoryChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(EntombedChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(MoveChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(MoveToQuarantineChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(PartyChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(PeeChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(PutOnHatChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(SighChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(StressIdleChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(SwitchRoleHatChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(TakeOffHatChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(UglyCryChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(WaterCoolerChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(AggressiveChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.On,
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
                new ChoreSyncConfig(
                    CreationStatusEnum.On,
                    new StateTransitionConfig(
                        typeof(BingeEatChore.States),
                        nameof(BingeEatChore.States.findfood),
                        new[] { nameof(BingeEatChore.States.ediblesource) }
                    )
                )
            }, {
                typeof(FleeChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.On,
                    new StateTransitionConfig(
                        typeof(FleeChore.States),
                        nameof(FleeChore.States.planFleeRoute),
                        new[] { nameof(FleeChore.States.fleeToTarget) }
                    )
                )
            }, {
                typeof(BeIncapacitatedChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(EmoteChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(StressEmoteChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(ReactEmoteChore),
                new ChoreSyncConfig(
                    //  an argument is not serializable.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(FoodFightChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(MournChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(MoveToSafetyChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(RecoverBreathChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(EatChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(MovePickupableChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(SleepChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(EquipChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(FixedCaptureChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(RancherChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(RescueIncapacitatedChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(RescueSweepBotChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(TakeMedicineChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(WorkChore<>),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(FetchAreaChore),
                new ChoreSyncConfig(
                    //  an argument is not serializable.
                    // fully determined by the input.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(BalloonArtistChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(BansheeChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(FetchChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(IdleChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(MingleChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }, {
                typeof(VomitChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off,
                    StateTransitionConfig.Disabled()
                )
            }
        };

    public record ChoreSyncConfig(
        CreationStatusEnum CreationSync,
        StateTransitionConfig StateTransitionSync
    );

    public enum CreationStatusEnum {
        On,
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
    };
}
