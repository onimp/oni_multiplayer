// Decompiled with JetBrains decompiler
// Type: DevRadiationEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class DevRadiationEmitter : KMonoBehaviour, ISingleSliderControl, ISliderControl
{
  [MyCmpReq]
  private RadiationEmitter radiationEmitter;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!Object.op_Inequality((Object) this.radiationEmitter, (Object) null))
      return;
    this.radiationEmitter.SetEmitting(true);
  }

  public string SliderTitleKey => (string) BUILDINGS.PREFABS.DEVRADIATIONGENERATOR.NAME;

  public string SliderUnits => (string) UI.UNITSUFFIXES.RADIATION.RADS;

  public float GetSliderMax(int index) => 5000f;

  public float GetSliderMin(int index) => 0.0f;

  public string GetSliderTooltip() => "";

  public string GetSliderTooltipKey(int index) => "";

  public float GetSliderValue(int index) => this.radiationEmitter.emitRads;

  public void SetSliderValue(float value, int index)
  {
    this.radiationEmitter.emitRads = value;
    this.radiationEmitter.Refresh();
  }

  public int SliderDecimalPlaces(int index) => 0;
}
