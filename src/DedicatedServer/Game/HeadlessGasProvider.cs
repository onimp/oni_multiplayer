namespace DedicatedServer.Game;

/// <summary>
/// No-op <see cref="OxygenBreather.IGasProvider"/> stub for the headless dedicated server.
/// <para>
/// In headless mode there is no gas simulation — <see cref="OxygenBreather.Sim200ms"/> finds
/// no provider with <see cref="HasOxygen"/> = true and, after a 2-second hysteresis timer,
/// flips <c>hasAir</c> to false.  That fires <c>GameHashes.OxygenBreatherHasAirChanged</c>,
/// which causes <c>SuffocationMonitor</c> to transition <c>satisfied → noOxygen</c>,
/// draining <c>breath.value</c> to 0 and ultimately calling
/// <c>DeathMonitor.Instance.Kill(Deaths.Suffocation)</c> → <c>DieChore</c>.
/// </para>
/// <para>
/// Adding this provider via <see cref="OxygenBreather.AddGasProvider"/> keeps
/// <see cref="HasOxygen"/> = true, so <c>hasAir</c> stays true and no suffocation occurs.
/// </para>
/// <list type="bullet">
///   <item><c>ConsumeGas</c> returns true — satisfies the O2-consumption path (no actual gas removed).</item>
///   <item><c>ShouldEmitCO2</c> / <c>ShouldStoreCO2</c> return false — no CO2 accumulation.</item>
///   <item><c>IsLowOxygen</c> returns false — no low-oxygen status item.</item>
///   <item><c>IsBlocked</c> returns false — provider is always active.</item>
/// </list>
/// </summary>
public class HeadlessGasProvider : OxygenBreather.IGasProvider {
    public void OnSetOxygenBreather(OxygenBreather oxygen_breather) { }
    public void OnClearOxygenBreather(OxygenBreather oxygen_breather) { }
    public bool ConsumeGas(OxygenBreather oxygen_breather, float amount) => true;
    public bool ShouldEmitCO2()  => false;
    public bool ShouldStoreCO2() => false;
    public bool IsLowOxygen()    => false;
    public bool HasOxygen()      => true;
    public bool IsBlocked()      => false;
}
