using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerMod.Game.Chores;

public static class ChoreList {

    /**
     * Those chores are fully determined by the input.
     */
    private static readonly HashSet<Type> deterministicChores = new(
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
    private static readonly HashSet<Type> deterministicChoresOff = new(
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

    /**
     * Those chores changes its parameters dynamically via target parameters.
     */
    private static readonly HashSet<Type> dynamicTargetedChores = new(
        new[] {
            typeof(AggressiveChore),
            typeof(BeIncapacitatedChore),
            typeof(BingeEatChore),
            typeof(EatChore),
            typeof(EmoteChore),
            typeof(EntombedChore),
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
     * Those chores have some issues and are disabled for now.
     */
    private static readonly HashSet<Type> dynamicTargetedChoresOff = new(
        new[] {
            // an argument is not serializable.
            typeof(FetchAreaChore),
        }
    );

    /**
     * Those chores changes its parameters dynamically without using target parameters.
     */
    private static readonly HashSet<Type> dynamicNonTargetedChores = new(
        new[] {
            typeof(BalloonArtistChore),
            typeof(BansheeChore),
            typeof(FetchChore),
            typeof(IdleChore),
            typeof(MingleChore),
            typeof(VomitChore),
        }
    );

    public static readonly HashSet<Type> SupportedChores = new(deterministicChores.Union(dynamicTargetedChores));

    public static readonly IEnumerable<Type> AllChoreTypes = deterministicChores.Concat(deterministicChoresOff)
        .Concat(dynamicTargetedChores).Concat(dynamicTargetedChoresOff)
        .Concat(dynamicNonTargetedChores)
        .OrderBy(a => a.FullName).ToList();
}
