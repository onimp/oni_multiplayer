using HarmonyLib;

namespace DedicatedServer.Game.Patches;

/// <summary>
/// Suppress RotPile food-rot UI notifications in headless.
///
/// ROOT CAUSE:
///   <c>RotPile.TryCreateNotification()</c> calls:
///   <code>
///     WorldContainer myWorld = base.smi.master.GetMyWorld();
///     if (myWorld != null &amp;&amp; myWorld.worldInventory.IsReachable(pickupable))
///   </code>
///   The headless <c>WorldContainer</c> (created manually in <c>WorldBuilder</c>)
///   has no <c>WorldInventory</c> component. <c>WorldContainer.OnPrefabInit()</c>
///   sets <c>worldInventory = GetComponent&lt;WorldInventory&gt;()</c> — null when
///   the component is absent. When <c>GetMyWorld()</c> returns non-null (RotPile
///   is at a valid grid cell with a valid WorldIdx), the call
///   <c>null.IsReachable(pickupable)</c> throws <c>NullReferenceException</c>.
///
///   Because the exception fires inside a <c>GameStateMachine.States.Enter()</c>
///   delegate, the StateMachine framework catches it and sets
///   <c>StateMachine.Instance.error = true</c>, halting ALL state machines.
///
/// FIX:
///   Notifications are UI-only (they display "Food is rotting" alerts to players
///   via the <c>NotificationManager</c>). In headless mode there are no players
///   and <c>KScreenManager.Instance</c> is null — <c>Notifier.Add()</c> would
///   already return early even if we reached it. Skipping both
///   <c>TryCreateNotification</c> and <c>TryClearNotification</c> entirely is
///   correct and safe. Game logic (decomposing timer, conversion to ToxicSand via
///   <c>ConvertToElement()</c>) is unaffected.
/// </summary>
[HarmonyPatch(typeof(RotPile))]
public static class RotPilePatches {

    /// <summary>
    /// Skip food-rot notification creation — UI-only, no-op in headless.
    /// Prevents <c>worldInventory.IsReachable()</c> NPE that sets globalSMError=true.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch("TryCreateNotification")]
    static bool TryCreateNotification_Prefix() => false;

    /// <summary>
    /// Skip food-rot notification removal — UI-only, no-op in headless.
    /// Symmetric with <c>TryCreateNotification</c>; notification was never created.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch("TryClearNotification")]
    static bool TryClearNotification_Prefix() => false;
}
