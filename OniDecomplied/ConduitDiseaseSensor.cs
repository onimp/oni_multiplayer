// Decompiled with JetBrains decompiler
// Type: ConduitDiseaseSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig]
public class ConduitDiseaseSensor : ConduitThresholdSensor, IThresholdSwitch
{
  private const float rangeMin = 0.0f;
  private const float rangeMax = 100000f;
  [Serialize]
  private float lastValue;
  private static readonly HashedString TINT_SYMBOL = HashedString.op_Implicit("germs");

  protected override void UpdateVisualState(bool force = false)
  {
    if (!(this.wasOn != this.switchedOn | force))
      return;
    this.wasOn = this.switchedOn;
    if (this.switchedOn)
    {
      this.animController.Play(ConduitSensor.ON_ANIMS, (KAnim.PlayMode) 0);
      int diseaseIdx;
      this.GetContentsDisease(out diseaseIdx, out int _, out bool _);
      Color32 color32 = Color32.op_Implicit(Color.white);
      if (diseaseIdx != (int) byte.MaxValue)
        color32 = GlobalAssets.Instance.colorSet.GetColorByName(Db.Get().Diseases[diseaseIdx].overlayColourName);
      this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(ConduitDiseaseSensor.TINT_SYMBOL), Color32.op_Implicit(color32));
    }
    else
      this.animController.Play(ConduitSensor.OFF_ANIMS);
  }

  private void GetContentsDisease(out int diseaseIdx, out int diseaseCount, out bool hasMass)
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (this.conduitType == ConduitType.Liquid || this.conduitType == ConduitType.Gas)
    {
      ConduitFlow.ConduitContents contents = Conduit.GetFlowManager(this.conduitType).GetContents(cell);
      diseaseIdx = (int) contents.diseaseIdx;
      diseaseCount = contents.diseaseCount;
      hasMass = (double) contents.mass > 0.0;
    }
    else
    {
      SolidConduitFlow flowManager = SolidConduit.GetFlowManager();
      Pickupable pickupable = flowManager.GetPickupable(flowManager.GetContents(cell).pickupableHandle);
      if (Object.op_Inequality((Object) pickupable, (Object) null) && (double) pickupable.PrimaryElement.Mass > 0.0)
      {
        diseaseIdx = (int) pickupable.PrimaryElement.DiseaseIdx;
        diseaseCount = pickupable.PrimaryElement.DiseaseCount;
        hasMass = true;
      }
      else
      {
        diseaseIdx = 0;
        diseaseCount = 0;
        hasMass = false;
      }
    }
  }

  public override float CurrentValue
  {
    get
    {
      int diseaseCount;
      bool hasMass;
      this.GetContentsDisease(out int _, out diseaseCount, out hasMass);
      if (hasMass)
        this.lastValue = (float) diseaseCount;
      return this.lastValue;
    }
  }

  public float RangeMin => 0.0f;

  public float RangeMax => 100000f;

  public float GetRangeMinInputField() => 0.0f;

  public float GetRangeMaxInputField() => 100000f;

  public LocString Title => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;

  public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE;

  public string AboveToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_ABOVE;

  public string BelowToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_BELOW;

  public string Format(float value, bool units) => GameUtil.GetFormattedInt((float) (int) value);

  public float ProcessedSliderValue(float input) => input;

  public float ProcessedInputValue(float input) => input;

  public LocString ThresholdValueUnits() => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_UNITS;

  public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

  public int IncrementScale => 1;

  public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(this.RangeMax);
}
