// Decompiled with JetBrains decompiler
// Type: InfraredVisualizerData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public struct InfraredVisualizerData
{
  public KAnimControllerBase controller;
  public AmountInstance temperatureAmount;
  public HandleVector<int>.Handle structureTemperature;
  public PrimaryElement primaryElement;
  public TemperatureVulnerable temperatureVulnerable;

  public void Update()
  {
    float temperature = 0.0f;
    if (this.temperatureAmount != null)
      temperature = this.temperatureAmount.value;
    else if (this.structureTemperature.IsValid())
      temperature = ((KSplitCompactedVector<StructureTemperatureHeader, StructureTemperaturePayload>) GameComps.StructureTemperatures).GetPayload(this.structureTemperature).Temperature;
    else if (Object.op_Inequality((Object) this.primaryElement, (Object) null))
      temperature = this.primaryElement.Temperature;
    else if (Object.op_Inequality((Object) this.temperatureVulnerable, (Object) null))
      temperature = this.temperatureVulnerable.InternalTemperature;
    if ((double) temperature < 0.0)
      return;
    this.controller.OverlayColour = Color32.op_Implicit(Color32.op_Implicit(SimDebugView.Instance.NormalizedTemperature(temperature)));
  }

  public InfraredVisualizerData(GameObject go)
  {
    this.controller = (KAnimControllerBase) go.GetComponent<KBatchedAnimController>();
    if (Object.op_Inequality((Object) this.controller, (Object) null))
    {
      this.temperatureAmount = Db.Get().Amounts.Temperature.Lookup(go);
      this.structureTemperature = GameComps.StructureTemperatures.GetHandle(go);
      this.primaryElement = go.GetComponent<PrimaryElement>();
      this.temperatureVulnerable = go.GetComponent<TemperatureVulnerable>();
    }
    else
    {
      this.temperatureAmount = (AmountInstance) null;
      this.structureTemperature = HandleVector<int>.InvalidHandle;
      this.primaryElement = (PrimaryElement) null;
      this.temperatureVulnerable = (TemperatureVulnerable) null;
    }
  }
}
