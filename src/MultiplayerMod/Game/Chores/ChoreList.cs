using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MultiplayerMod.Game.Chores.Types;

namespace MultiplayerMod.Game.Chores;

[Obsolete]
public static class ChoreList {

    private static readonly Regex stateNameRegex = new(@"nameof\(.*?\..*?\.(.*?)\)"); // Chore.State.(chained.name)

    // ReSharper disable once EntityNameCapturedOnly.Local
    private static string State(string value, [CallerArgumentExpression(nameof(value))] string expression = default!) =>
        stateNameRegex.Match(expression).Groups[1].Value;

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
                            State(nameof(BeIncapacitatedChore.States.incapacitation_root.lookingForBed)),
                            nameof(BeIncapacitatedChore.States.clinic)
                        )
                    )
                )
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
                        StateTransitionConfig.OnEnter(
                            nameof(RecoverBreathChore.States.root),
                            nameof(RecoverBreathChore.States.locator)
                        ),
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
                typeof(FetchAreaChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<FetchAreaChore.States>(
                        StateTransitionConfig.OnExit(
                            State(nameof(FetchAreaChore.States.delivering.next)),
                            nameof(FetchAreaChore.States.deliveryDestination),
                            nameof(FetchAreaChore.States.deliveryObject)
                        ),
                        StateTransitionConfig.OnExit(
                            State(nameof(FetchAreaChore.States.fetching.next)),
                            nameof(FetchAreaChore.States.fetchTarget),
                            nameof(FetchAreaChore.States.fetchResultTarget),
                            nameof(FetchAreaChore.States.fetchAmount)
                        )
                    )
                )
            }, {
                typeof(BalloonArtistChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<BalloonArtistChore.States>(
                        StateTransitionConfig.OnTransition(nameof(BalloonArtistChore.States.goToStand)),
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
                typeof(IdleChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<IdleChore.States>(
                        // To prevent update callback on client
                        StateTransitionConfig.OnUpdate(
                            State(nameof(IdleChore.States.idle.ontube))
                        ),
                        StateTransitionConfig.OnExit(
                            State(nameof(IdleChore.States.idle.ontube))
                        ),
                        StateTransitionConfig.OnTransition(
                            State(nameof(IdleChore.States.idle.move))
                        ),
                        StateTransitionConfig.OnMove(
                            State(nameof(IdleChore.States.idle.move))
                        )
                    )
                )
            }, {
                typeof(MingleChore),
                ChoreSyncConfig.Dynamic(
                    StatesTransitionConfig.Enabled<MingleChore.States>(
                        StateTransitionConfig.OnTransition(nameof(MingleChore.States.mingle)),
                        StateTransitionConfig.OnTransition(nameof(MingleChore.States.move)),
                        StateTransitionConfig.OnTransition(nameof(MingleChore.States.walk)),
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

}
