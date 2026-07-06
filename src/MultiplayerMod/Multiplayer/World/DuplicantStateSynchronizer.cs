using System.Collections.Generic;
using Klei.AI;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative replication of duplicant internal state (sicknesses / effects / health). Each
// machine simulates these independently, so a dupe can be food-poisoned, debuffed or dying on the host
// while fine on the client until the daily hard-sync corrects it - the most visible mid-game divergence.
// This streams the host's authoritative state to clients continuously instead (see SyncDuplicantState
// for the model and the applied-state details).
//
// There are only a handful of dupes, so - unlike SimStateSynchronizer's per-cell round-robin - we send
// every live dupe in a single command each cadence tick. The payload is tiny (a few short lists per dupe)
// and rides the reliable Gameplay lane. Gated Host-only + level-active + at least one client, on a ~1 s
// cadence; a client that hasn't replicated a given dupe yet simply skips it (graceful resolve).
public class DuplicantStateSynchronizer : MultiplayerKMonoBehaviour, IRenderEveryTick {

    private const float sendPeriod = 1.0f;

    [InjectDependency]
    private readonly IMultiplayerServer server = null!;

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private readonly ExecutionLevelManager manager = null!;

    private float lastTime;

    public void RenderEveryTick(float dt) {
        if (!manager.LevelIsActive(ExecutionLevel.Multiplayer) || multiplayer.Mode != MultiplayerMode.Host)
            return;
        if (server.Clients.Count == 0)
            return;
        if (GameClock.Instance.GetTime() - lastTime < sendPeriod)
            return;

        lastTime = GameClock.Instance.GetTime();

        var entries = BuildEntries();
        if (entries.Length > 0)
            server.Send(new SyncDuplicantState(entries));
    }

    private static SyncDuplicantState.DuplicantEntry[] BuildEntries() {
        var identities = global::Components.LiveMinionIdentities.Items;
        var list = new List<SyncDuplicantState.DuplicantEntry>(identities.Count);
        foreach (var identity in identities) {
            if (identity == null)
                continue;

            var gameObject = identity.gameObject;
            var entry = new SyncDuplicantState.DuplicantEntry {
                Reference = identity.GetReference<MinionIdentity>(),
                Sicknesses = ReadSicknesses(gameObject),
                Effects = ReadEffects(gameObject),
                Immunities = ReadImmunities(gameObject)
            };

            var health = gameObject.GetComponent<Health>();
            if (health != null) {
                entry.HasHealth = true;
                entry.HitPoints = health.hitPoints;
                entry.CanBeIncapacitated = health.canBeIncapacitated;
            }

            entry.Amounts = ReadAmounts(gameObject);
            entry.AttributeLevels = ReadAttributeLevels(gameObject);

            var resume = gameObject.GetComponent<MinionResume>();
            if (resume != null) {
                entry.HasResume = true;
                entry.TotalExperienceGained = resume.totalExperienceGained;
            }

            var element = gameObject.GetComponent<PrimaryElement>();
            if (element != null) {
                entry.GermIdx = element.DiseaseIdx;
                entry.GermCount = element.DiseaseCount;
            }

            list.Add(entry);
        }
        return list.ToArray();
    }

    // The vitals shown on the dupe panel - each an ISim200ms amount integrated independently per machine.
    // Cached because the ids never change within a session (Db is fixed on the pinned game version).
    private static string[]? vitalAmountIds;
    private static string[] VitalAmountIds => vitalAmountIds ??= new[] {
        Db.Get().Amounts.Calories.Id,
        Db.Get().Amounts.Stress.Id,
        Db.Get().Amounts.Stamina.Id,
        Db.Get().Amounts.Bladder.Id,
        Db.Get().Amounts.Breath.Id
    };

    private static SyncDuplicantState.AmountEntry[] ReadAmounts(UnityEngine.GameObject gameObject) {
        var modifiers = gameObject.GetComponent<Klei.AI.Modifiers>();
        if (modifiers?.amounts == null)
            return System.Array.Empty<SyncDuplicantState.AmountEntry>();

        var result = new List<SyncDuplicantState.AmountEntry>(VitalAmountIds.Length);
        foreach (var id in VitalAmountIds) {
            var instance = modifiers.amounts.Get(id);
            if (instance != null)
                result.Add(new SyncDuplicantState.AmountEntry { Id = id, Value = instance.value });
        }
        return result.ToArray();
    }

    private static SyncDuplicantState.AttributeLevelEntry[] ReadAttributeLevels(UnityEngine.GameObject gameObject) {
        var levels = gameObject.GetComponent<Klei.AI.AttributeLevels>();
        if (levels == null)
            return System.Array.Empty<SyncDuplicantState.AttributeLevelEntry>();

        var result = new List<SyncDuplicantState.AttributeLevelEntry>();
        foreach (var level in levels) {
            result.Add(new SyncDuplicantState.AttributeLevelEntry {
                Id = level.attribute.Id,
                Experience = level.experience,
                Level = level.level
            });
        }
        return result.ToArray();
    }

    private static SyncDuplicantState.SicknessEntry[] ReadSicknesses(UnityEngine.GameObject gameObject) {
        var sicknesses = gameObject.GetSicknesses();
        if (sicknesses == null)
            return System.Array.Empty<SyncDuplicantState.SicknessEntry>();

        var result = new SyncDuplicantState.SicknessEntry[sicknesses.Count];
        for (var i = 0; i < sicknesses.Count; i++) {
            var instance = sicknesses[i];
            result[i] = new SyncDuplicantState.SicknessEntry {
                Id = instance.modifier.Id,
                Source = instance.ExposureInfo.sourceInfo,
                PercentCured = instance.GetPercentCured()
            };
        }
        return result;
    }

    private static SyncDuplicantState.EffectEntry[] ReadEffects(UnityEngine.GameObject gameObject) {
        var effects = gameObject.GetComponent<Effects>();
        if (effects == null)
            return System.Array.Empty<SyncDuplicantState.EffectEntry>();

        var saved = effects.GetAllEffectsForSerialization();
        var result = new SyncDuplicantState.EffectEntry[saved.Count];
        for (var i = 0; i < saved.Count; i++) {
            result[i] = new SyncDuplicantState.EffectEntry {
                Id = saved[i].id,
                TimeRemaining = saved[i].timeRemaining,
                Saved = saved[i].saved
            };
        }
        return result;
    }

    private static SyncDuplicantState.ImmunityEntry[] ReadImmunities(UnityEngine.GameObject gameObject) {
        var effects = gameObject.GetComponent<Effects>();
        if (effects == null)
            return System.Array.Empty<SyncDuplicantState.ImmunityEntry>();

        var saved = effects.GetAllImmunitiesForSerialization();
        var result = new SyncDuplicantState.ImmunityEntry[saved.Count];
        for (var i = 0; i < saved.Count; i++) {
            result[i] = new SyncDuplicantState.ImmunityEntry {
                EffectId = saved[i].effectID,
                GiverId = saved[i].giverID,
                Saved = saved[i].saved
            };
        }
        return result;
    }

}
