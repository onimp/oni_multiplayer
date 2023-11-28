using System;
using System.Collections.Generic;

namespace MultiplayerMod.Game.Chores;

public static class ChoreList {
    public static readonly HashSet<Type> DeterministicChores = new(
        new[] {
            typeof(AttackChore),
            typeof(DeliverFoodChore),
            typeof(DieChore),
            typeof(DropUnusedInventoryChore),
            typeof(EmoteChore),
            typeof(EquipChore),
            typeof(FixedCaptureChore),
            typeof(MoveChore),
            typeof(MoveToQuarantineChore),
            typeof(PartyChore),
            typeof(PeeChore),
            typeof(PutOnHatChore),
            typeof(RancherChore),
            typeof(ReactEmoteChore),
            typeof(RescueIncapacitatedChore),
            typeof(RescueSweepBotChore),
            typeof(SighChore),
            typeof(StressEmoteChore),
            typeof(StressIdleChore),
            typeof(SwitchRoleHatChore),
            typeof(TakeMedicineChore),
            typeof(TakeOffHatChore),
            typeof(UglyCryChore),
            typeof(WaterCoolerChore),
            typeof(WorkChore<>)
        }
    );
}
