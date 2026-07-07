using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using MultiplayerMod.Core.Collections;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Game.Chores;
using MultiplayerMod.Game.Mechanics.Minions;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Players;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Ownership;

/// <summary>
/// Host-side chore precondition that makes owned duplicants prefer their owning player's work. Mirrors
/// <c>MultiplayerDriverChores</c>'s static-precondition pattern. Added only to work chores on the host
/// (see <c>ChoresPatcher</c>); clients never evaluate it.
///
/// Two behaviours in one precondition:
/// <list type="bullet">
/// <item><b>Boost</b> — a chore owned by the evaluating duplicant's own player gets a large
/// <c>personalPriority</c> bump, so it outranks every other <see cref="PriorityScreen.PriorityClass.basic"/>
/// chore and the duplicant picks it next. We deliberately do NOT touch <c>priority_class</c>: emergencies
/// (red alert) and the <c>!!</c> top-priority button (<see cref="PriorityScreen.PriorityClass.topPriority"/>)
/// are a higher class, which <see cref="Chore.Precondition.Context.CompareTo"/> checks first, so they always
/// win for everyone. Running at sortOrder -1 (after vanilla <c>IsMoreSatisfyingEarly</c> at -2) means the
/// boost decides the <i>next</i> pick rather than interrupting an in-progress errand.</item>
/// <item><b>Exclude</b> — another player's owned work fails the precondition (unavailable) unless the
/// duplicant has been idle past a short grace ("borrowing" — keeps foreign work from starving).</item>
/// </list>
/// Unowned work stays available and unboosted; an unassigned duplicant behaves vanilla. When the feature is
/// disabled the precondition passes everything with no boost (vanilla behaviour).
/// </summary>
[Core.Dependency.Dependency, UsedImplicitly]
public class ChoreOwnershipPreconditions {

    private const float BorrowGraceSeconds = 3f;

    // Added to an owned chore's personalPriority. Far above the natural range (1-9) so owned work dominates
    // all other basic-class work, but personalPriority is ranked below priority_class, so a higher class
    // (emergency / !!) still wins.
    private const int OwnerPriorityBoost = 1000;

    private static ChoreOwnershipPreconditions? instance;
    private static DuplicantOwnershipRegistry duplicants = null!;
    private static ChoreOwnershipRegistry chores = null!;

    // Per-duplicant (keyed on the minion GameObject) idle-since timestamp and cached proxy id.
    private readonly ConditionalWeakTable<GameObject, BoxedValue<float>> idleSince = new();
    private readonly ConditionalWeakTable<GameObject, BoxedValue<MultiplayerId>> proxyIdCache = new();

    public static Chore.Precondition PrefersOwnerChore = new() {
        id = nameof(PrefersOwnerChore),
        description = "Duplicant prefers its owning player's work",
        // Never throw out of chore scoring - an exception here could stall the host's whole chore system.
        // On any unexpected error, fall back to "available" (vanilla behaviour).
        fn = (ref Chore.Precondition.Context context, object _) => instance == null || instance.PassesSafe(ref context),
        // -1: after vanilla IsMoreSatisfyingEarly (-2) so the personalPriority boost affects the next pick
        // and final ranking, not the "interrupt current errand" decision. Must stay > -2.
        sortOrder = -1
    };

    public ChoreOwnershipPreconditions(DuplicantOwnershipRegistry duplicants, ChoreOwnershipRegistry chores) {
        instance = this;
        ChoreOwnershipPreconditions.duplicants = duplicants;
        ChoreOwnershipPreconditions.chores = chores;
        ChoreDriverEvents.ChoreSetting += OnChoreSetting;
    }

    private bool PassesSafe(ref Chore.Precondition.Context context) {
        try {
            return Passes(ref context);
        } catch {
            return true;
        }
    }

    private bool Passes(ref Chore.Precondition.Context context) {
        if (!duplicants.Enabled)
            return true;

        var targetOwner = chores.GetOwner(context.chore?.gameObject);
        if (targetOwner == null)
            return true; // unowned work is available to anyone, unboosted

        var dupe = context.consumerState.gameObject;
        var dupeOwner = GetDuplicantOwner(dupe);

        if (dupeOwner != null && targetOwner.Equals(dupeOwner)) {
            // The duplicant's own player's work: boost it above all other basic-class work so it is picked
            // next. priority_class is untouched, so red alert / !! still win (see class summary).
            context.personalPriority += OwnerPriorityBoost;
            return true;
        }

        if (dupeOwner == null)
            return true; // unassigned duplicant does anything, unboosted

        return IsBorrowing(dupe); // foreign work only while idle past the grace
    }

    private PlayerIdentity? GetDuplicantOwner(GameObject dupe) {
        var proxyId = GetProxyId(dupe);
        return proxyId == null ? null : duplicants.GetOwner(proxyId);
    }

    private MultiplayerId? GetProxyId(GameObject dupe) {
        if (proxyIdCache.TryGetValue(dupe, out var cached))
            return cached.Value;

        MultiplayerId? id = null;
        try {
            id = dupe.GetComponent<MinionIdentity>()?.GetMultiplayerInstance().Id;
        } catch {
            // Proxy not ready yet - don't cache; retry next evaluation.
        }
        if (id != null)
            proxyIdCache.Add(dupe, new BoxedValue<MultiplayerId>(id));
        return id;
    }

    // Idle fallback: when the host assigns a duplicant an IdleChore it has nothing owned/unowned to do, so
    // after a grace it becomes eligible to "borrow" a foreign player's work. Any real assignment clears it.
    private void OnChoreSetting(ChoreDriver driver, Chore? previousChore, ref Chore.Precondition.Context context) {
        var dupe = driver.gameObject;
        if (context.chore is IdleChore)
            MarkIdle(dupe);
        else
            idleSince.Remove(dupe);
    }

    private void MarkIdle(GameObject dupe) {
        if (!idleSince.TryGetValue(dupe, out _))
            idleSince.Add(dupe, new BoxedValue<float>(Time.time));
    }

    private bool IsBorrowing(GameObject dupe) =>
        idleSince.TryGetValue(dupe, out var since) && Time.time - since.Value >= BorrowGraceSeconds;

}
