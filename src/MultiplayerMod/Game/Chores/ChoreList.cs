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
                    CreationStatusEnum.On
                )
            }, {
                typeof(DeliverFoodChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(DieChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(DropUnusedInventoryChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(EntombedChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(MoveChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(MoveToQuarantineChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(PartyChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(PeeChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(PutOnHatChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(SighChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(StressIdleChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(SwitchRoleHatChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(TakeOffHatChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(UglyCryChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(WaterCoolerChore),
                new ChoreSyncConfig(
                    // fully determined by the input.
                    CreationStatusEnum.On
                )
            }, {
                typeof(AggressiveChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(BingeEatChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(FleeChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(BeIncapacitatedChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(EmoteChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(StressEmoteChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(ReactEmoteChore),
                new ChoreSyncConfig(
                    //  an argument is not serializable.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(FoodFightChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(MournChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(MoveToSafetyChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(RecoverBreathChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(EatChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(MovePickupableChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(SleepChore),
                new ChoreSyncConfig(
                    CreationStatusEnum.Off
                )
            }, {
                typeof(EquipChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(FixedCaptureChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(RancherChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(RescueIncapacitatedChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(RescueSweepBotChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(TakeMedicineChore),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(WorkChore<>),
                new ChoreSyncConfig(
                    // using global providers. Hence they depends on consumer who will pick them up.
                    // fully determined by the input.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(FetchAreaChore),
                new ChoreSyncConfig(
                    //  an argument is not serializable.
                    // fully determined by the input.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(BalloonArtistChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(BansheeChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(FetchChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(IdleChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(MingleChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off
                )
            }, {
                typeof(VomitChore),
                new ChoreSyncConfig(
                    // changes its parameters dynamically without using target parameters.
                    CreationStatusEnum.Off
                )
            }
        };

    public record ChoreSyncConfig(CreationStatusEnum CreationSync);

    public enum CreationStatusEnum {
        On,
        Off
    }
}
