// Decompiled with JetBrains decompiler
// Type: GasBreatherFromWorldProvider
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class GasBreatherFromWorldProvider : OxygenBreather.IGasProvider
{
  private SuffocationMonitor.Instance suffocationMonitor;
  private SafeCellMonitor.Instance safeCellMonitor;
  private OxygenBreather oxygenBreather;
  private Navigator nav;

  public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
  {
    this.suffocationMonitor = new SuffocationMonitor.Instance(oxygen_breather);
    this.suffocationMonitor.StartSM();
    this.safeCellMonitor = new SafeCellMonitor.Instance((IStateMachineTarget) oxygen_breather);
    this.safeCellMonitor.StartSM();
    this.oxygenBreather = oxygen_breather;
    this.nav = ((Component) this.oxygenBreather).GetComponent<Navigator>();
  }

  public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
  {
    this.suffocationMonitor.StopSM("Removed gas provider");
    this.safeCellMonitor.StopSM("Removed gas provider");
  }

  public bool ShouldEmitCO2() => this.nav.CurrentNavType != NavType.Tube;

  public bool ShouldStoreCO2() => false;

  public bool ConsumeGas(OxygenBreather oxygen_breather, float gas_consumed)
  {
    if (this.nav.CurrentNavType != NavType.Tube)
    {
      SimHashes breathableElement = oxygen_breather.GetBreathableElement;
      if (breathableElement == SimHashes.Vacuum)
        return false;
      HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(new Action<Sim.MassConsumedCallback, object>(GasBreatherFromWorldProvider.OnSimConsumeCallback), (object) this, nameof (GasBreatherFromWorldProvider));
      SimMessages.ConsumeMass(oxygen_breather.mouthCell, breathableElement, gas_consumed, (byte) 3, handle.index);
    }
    return true;
  }

  private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data) => ((GasBreatherFromWorldProvider) data).OnSimConsume(mass_cb_info);

  private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
  {
    if (Object.op_Equality((Object) this.oxygenBreather, (Object) null) || ((Component) this.oxygenBreather).GetComponent<KPrefabID>().HasTag(GameTags.Dead))
      return;
    if (ElementLoader.elements[(int) mass_cb_info.elemIdx].id == SimHashes.ContaminatedOxygen)
      this.oxygenBreather.Trigger(-935848905, (object) mass_cb_info);
    Game.Instance.accumulators.Accumulate(this.oxygenBreather.O2Accumulator, mass_cb_info.mass);
    ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, -mass_cb_info.mass, ((Component) this.oxygenBreather).GetProperName());
    this.oxygenBreather.Consume(mass_cb_info);
  }
}
