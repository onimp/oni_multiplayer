// Decompiled with JetBrains decompiler
// Type: LogicDiseaseSensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
public class LogicDiseaseSensor : Switch, ISaveLoadable, IThresholdSwitch, ISim200ms
{
  [SerializeField]
  [Serialize]
  private float threshold;
  [SerializeField]
  [Serialize]
  private bool activateAboveThreshold = true;
  private KBatchedAnimController animController;
  private bool wasOn;
  private const float rangeMin = 0.0f;
  private const float rangeMax = 100000f;
  private const int WINDOW_SIZE = 8;
  private int[] samples = new int[8];
  private int sampleIdx;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<LogicDiseaseSensor> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicDiseaseSensor>((Action<LogicDiseaseSensor, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly HashedString[] ON_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("on_pre"),
    HashedString.op_Implicit("on_loop")
  };
  private static readonly HashedString[] OFF_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("on_pst"),
    HashedString.op_Implicit("off")
  };
  private static readonly HashedString TINT_SYMBOL = HashedString.op_Implicit("germs");

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LogicDiseaseSensor>(-905833192, LogicDiseaseSensor.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    LogicDiseaseSensor component = ((GameObject) data).GetComponent<LogicDiseaseSensor>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.Threshold = component.Threshold;
    this.ActivateAboveThreshold = component.ActivateAboveThreshold;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.animController = ((Component) this).GetComponent<KBatchedAnimController>();
    this.OnToggle += new Action<bool>(this.OnSwitchToggled);
    this.UpdateLogicCircuit();
    this.UpdateVisualState(true);
    this.wasOn = this.switchedOn;
  }

  public void Sim200ms(float dt)
  {
    if (this.sampleIdx < 8)
    {
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      if ((double) Grid.Mass[cell] <= 0.0)
        return;
      this.samples[this.sampleIdx] = Grid.DiseaseCount[cell];
      ++this.sampleIdx;
    }
    else
    {
      this.sampleIdx = 0;
      float currentValue = this.CurrentValue;
      if (this.activateAboveThreshold)
      {
        if ((double) currentValue > (double) this.threshold && !this.IsSwitchedOn || (double) currentValue <= (double) this.threshold && this.IsSwitchedOn)
          this.Toggle();
      }
      else if ((double) currentValue > (double) this.threshold && this.IsSwitchedOn || (double) currentValue <= (double) this.threshold && !this.IsSwitchedOn)
        this.Toggle();
      this.animController.SetSymbolVisiblity(KAnimHashedString.op_Implicit(LogicDiseaseSensor.TINT_SYMBOL), (double) currentValue > 0.0);
    }
  }

  private void OnSwitchToggled(bool toggled_on)
  {
    this.UpdateLogicCircuit();
    this.UpdateVisualState();
  }

  public float Threshold
  {
    get => this.threshold;
    set => this.threshold = value;
  }

  public bool ActivateAboveThreshold
  {
    get => this.activateAboveThreshold;
    set => this.activateAboveThreshold = value;
  }

  public float CurrentValue
  {
    get
    {
      float num = 0.0f;
      for (int index = 0; index < 8; ++index)
        num += (float) this.samples[index];
      return num / 8f;
    }
  }

  public float RangeMin => 0.0f;

  public float RangeMax => 100000f;

  public float GetRangeMinInputField() => 0.0f;

  public float GetRangeMaxInputField() => 100000f;

  public LocString ThresholdValueName => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE;

  public string AboveToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_ABOVE;

  public string BelowToolTip => (string) UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TOOLTIP_BELOW;

  public string Format(float value, bool units) => GameUtil.GetFormattedInt((float) (int) value);

  public float ProcessedSliderValue(float input) => input;

  public float ProcessedInputValue(float input) => input;

  public LocString ThresholdValueUnits() => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_UNITS;

  public ThresholdScreenLayoutType LayoutType => ThresholdScreenLayoutType.SliderBar;

  public int IncrementScale => 100;

  public NonLinearSlider.Range[] GetRanges => NonLinearSlider.GetDefaultRange(this.RangeMax);

  private void UpdateLogicCircuit() => ((Component) this).GetComponent<LogicPorts>().SendSignal(LogicSwitch.PORT_ID, this.switchedOn ? 1 : 0);

  private void UpdateVisualState(bool force = false)
  {
    if (!(this.wasOn != this.switchedOn | force))
      return;
    this.wasOn = this.switchedOn;
    if (this.switchedOn)
    {
      this.animController.Play(LogicDiseaseSensor.ON_ANIMS, (KAnim.PlayMode) 0);
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      byte num = Grid.DiseaseIdx[cell];
      Color32 color32 = Color32.op_Implicit(Color.white);
      if (num != byte.MaxValue)
        color32 = GlobalAssets.Instance.colorSet.GetColorByName(Db.Get().Diseases[(int) num].overlayColourName);
      this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(LogicDiseaseSensor.TINT_SYMBOL), Color32.op_Implicit(color32));
    }
    else
      this.animController.Play(LogicDiseaseSensor.OFF_ANIMS);
  }

  protected override void UpdateSwitchStatus()
  {
    StatusItem status_item = this.switchedOn ? Db.Get().BuildingStatusItems.LogicSensorStatusActive : Db.Get().BuildingStatusItems.LogicSensorStatusInactive;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item);
  }

  public LocString Title => UI.UISIDESCREENS.THRESHOLD_SWITCH_SIDESCREEN.DISEASE_TITLE;
}
