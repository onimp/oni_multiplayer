using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerMod.Game.Chores;

public static class ChoreList {

    /**
     * Those chores are fully determined by the input.
     */
    public static readonly HashSet<Type> DeterministicChores = new(
        new[] {
            typeof(AttackChore),
            typeof(DeliverFoodChore),
            typeof(DieChore),
            typeof(DropUnusedInventoryChore),
            typeof(MoveToQuarantineChore),
            typeof(PartyChore),
            typeof(PeeChore),
            typeof(PutOnHatChore),
            typeof(SighChore),
            typeof(StressIdleChore),
            typeof(SwitchRoleHatChore),
            typeof(TakeOffHatChore),
            typeof(UglyCryChore),
            typeof(WaterCoolerChore),
        }
    );

    /**
     * Those chores have some issues and are disabled for now.
     */
    public static readonly HashSet<Type> DeterministicChoresOff = new(
        new[] {
            // No usage has been found in the game dlls.
            // one argument of type EmoteReactable is not serializable.
            typeof(ReactEmoteChore),
            // Chores below using global providers. Hence they depends on consumer who will pick them up.
            typeof(EquipChore),
            typeof(FixedCaptureChore),
            typeof(RancherChore),
            typeof(RescueIncapacitatedChore),
            typeof(RescueSweepBotChore),
            typeof(TakeMedicineChore),
            typeof(WorkChore<>),
        }
    );

    public static readonly HashSet<Type> SupportedChores = new(DeterministicChores);

    /**
     * Those chores changes its parameters dynamically via target parameters.
     */
    public static readonly HashSet<Type> DynamicTargetedChores = new(
        new[] {
            typeof(AggressiveChore),
            typeof(BeIncapacitatedChore),
            typeof(BingeEatChore),
            typeof(EatChore),
            typeof(EmoteChore),
            typeof(EntombedChore),
            typeof(FetchAreaChore),
            typeof(FleeChore),
            typeof(FoodFightChore),
            typeof(MoveChore),
            typeof(MournChore),
            typeof(MovePickupableChore),
            typeof(MoveToSafetyChore),
            typeof(RecoverBreathChore),
            typeof(SleepChore),
            typeof(StressEmoteChore),
        }
    );

    /**
     * Those chores changes its parameters dynamically without using target parameters.
     */
    public static readonly HashSet<Type> DynamicNonTargetedChores = new(
        new[] {
            typeof(BalloonArtistChore),
            typeof(BansheeChore),
            typeof(FetchChore),
            typeof(IdleChore),
            typeof(MingleChore),
            typeof(VomitChore),
        }
    );

    public static readonly IEnumerable<Type> AllChoreTypes = DeterministicChores.Concat(DeterministicChoresOff)
        .Concat(DynamicTargetedChores).Concat(DynamicNonTargetedChores).OrderBy(a => a.FullName).ToList();
}
