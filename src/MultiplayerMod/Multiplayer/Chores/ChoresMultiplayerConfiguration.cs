using static MultiplayerMod.Multiplayer.Chores.Dsl.ChoresConfigurationDsl;

namespace MultiplayerMod.Multiplayer.Chores;

public static class ChoresMultiplayerConfiguration {

    public static readonly ChoreConfiguration[] Configuration = [
        Synchronize<AttackChore>(),
        Synchronize<DeliverFoodChore>(),
        Synchronize<DieChore>(),
        Synchronize<DropUnusedInventoryChore>(),
        Synchronize<EntombedChore>(),
        Synchronize<MoveChore>(),
        Synchronize<MoveToQuarantineChore>(),
        Synchronize<PartyChore>(),
        Synchronize<PeeChore>(),
        Synchronize<PutOnHatChore>(),
        Synchronize<SighChore>(),
        Synchronize<StressIdleChore>(),
        Synchronize<SwitchRoleHatChore>(),
        Synchronize<TakeOffHatChore>(),
        Synchronize<UglyCryChore>(),
        Synchronize<WaterCoolerChore>(),
        Synchronize<ReactEmoteChore>(),
        Synchronize<EmoteChore>(),
        Synchronize<StressEmoteChore>(),
        Synchronize<EquipChore>(),
        Synchronize<FixedCaptureChore>(),
        Synchronize<RancherChore>(),
        Synchronize<RescueIncapacitatedChore>(),
        Synchronize<RescueSweepBotChore>(),
        Synchronize<TakeMedicineChore>(),
        Synchronize<FetchChore>()
    ];

}
