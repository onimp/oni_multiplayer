using HarmonyLib;
using JetBrains.Annotations;

namespace MultiplayerMod.Test.GameRuntime.Patches;

/// <summary>
/// Patches MinionIdentity.OnSpawn to suppress errors from missing Personality lookup.
/// In test environment, the Personalities DB is empty (personalitiesFile = empty TextAsset).
/// MinionIdentity.OnSpawn() calls Db.Get().Personalities.Get(personalityResourceId) which
/// logs "Could not find resource: 0x0" as Debug.LogError — converted to exception by
/// DebugLogHandlerPatch.
///
/// We suppress the DebugLogHandler exception for known test-environment errors during OnSpawn,
/// then restore normal behavior after.
/// </summary>
[UsedImplicitly]
[HarmonyPatch(typeof(MinionIdentity))]
public class MinionIdentityPatch {

    // Temporarily suppresses Debug.LogError exceptions during MinionIdentity.OnSpawn
    public static bool SuppressErrors { get; set; }

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(MinionIdentity.OnSpawn))]
    private static void MinionIdentity_OnSpawn_Prefix() {
        SuppressErrors = true;
    }

    [UsedImplicitly]
    [HarmonyPostfix]
    [HarmonyPatch(nameof(MinionIdentity.OnSpawn))]
    private static void MinionIdentity_OnSpawn_Postfix() {
        SuppressErrors = false;
    }

}
