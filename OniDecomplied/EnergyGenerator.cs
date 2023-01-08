// Decompiled with JetBrains decompiler
// Type: EnergyGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig]
public class EnergyGenerator : 
  Generator,
  IGameObjectEffectDescriptor,
  ISingleSliderControl,
  ISliderControl
{
  [MyCmpAdd]
  private Storage storage;
  [MyCmpGet]
  private ManualDeliveryKG delivery;
  [SerializeField]
  [Serialize]
  private float batteryRefillPercent = 0.5f;
  public bool ignoreBatteryRefillPercent;
  public bool hasMeter = true;
  private static StatusItem batteriesSufficientlyFull;
  public Meter.Offset meterOffset;
  [SerializeField]
  public EnergyGenerator.Formula formula;
  private MeterController meter;
  private static readonly EventSystem.IntraObjectHandler<EnergyGenerator> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<EnergyGenerator>((Action<EnergyGenerator, object>) ((component, data) => component.OnActiveChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<EnergyGenerator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<EnergyGenerator>((Action<EnergyGenerator, object>) ((component, data) => component.OnCopySettings(data)));

  public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.MANUALDELIVERYGENERATORSIDESCREEN.TITLE";

  public string SliderUnits => (string) UI.UNITSUFFIXES.PERCENT;

  public int SliderDecimalPlaces(int index) => 0;

  public float GetSliderMin(int index) => 0.0f;

  public float GetSliderMax(int index) => 100f;

  public float GetSliderValue(int index) => this.batteryRefillPercent * 100f;

  public void SetSliderValue(float value, int index) => this.batteryRefillPercent = value / 100f;

  string ISliderControl.GetSliderTooltip()
  {
    ManualDeliveryKG component = ((Component) this).GetComponent<ManualDeliveryKG>();
    return string.Format(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.UISIDESCREENS.MANUALDELIVERYGENERATORSIDESCREEN.TOOLTIP")), (object) component.RequestedItemTag.ProperName(), (object) (float) ((double) this.batteryRefillPercent * 100.0));
  }

  public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.MANUALDELIVERYGENERATORSIDESCREEN.TOOLTIP";

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    EnergyGenerator.EnsureStatusItemAvailable();
    this.Subscribe<EnergyGenerator>(824508782, EnergyGenerator.OnActiveChangedDelegate);
    if (this.ignoreBatteryRefillPercent)
      return;
    ((Component) this).gameObject.AddOrGet<CopyBuildingSettings>();
    this.Subscribe<EnergyGenerator>(-905833192, EnergyGenerator.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    EnergyGenerator component = ((GameObject) data).GetComponent<EnergyGenerator>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.batteryRefillPercent = component.batteryRefillPercent;
  }

  protected void OnActiveChanged(object data)
  {
    StatusItem status_item = ((Operational) data).IsActive ? Db.Get().BuildingStatusItems.Wattage : Db.Get().BuildingStatusItems.GeneratorOffline;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, (object) this);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if (!this.hasMeter)
      return;
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", this.meterOffset, Grid.SceneLayer.NoLayer, new string[4]
    {
      "meter_target",
      "meter_fill",
      "meter_frame",
      "meter_OL"
    });
  }

  private bool IsConvertible(float dt)
  {
    bool flag = true;
    foreach (EnergyGenerator.InputItem input in this.formula.inputs)
    {
      float massAvailable = this.storage.GetMassAvailable(input.tag);
      float num = input.consumptionRate * dt;
      flag = flag && (double) massAvailable >= (double) num;
      if (!flag)
        break;
    }
    return flag;
  }

  public override void EnergySim200ms(float dt)
  {
    base.EnergySim200ms(dt);
    if (this.hasMeter)
    {
      EnergyGenerator.InputItem input = this.formula.inputs[0];
      this.meter.SetPositionPercent(this.storage.GetMassAvailable(input.tag) / input.maxStoredMass);
    }
    ushort circuitId = this.CircuitID;
    this.operational.SetFlag(Generator.wireConnectedFlag, circuitId != ushort.MaxValue);
    bool flag1 = false;
    if (this.operational.IsOperational)
    {
      bool flag2 = false;
      List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitId);
      if (!this.ignoreBatteryRefillPercent && batteriesOnCircuit.Count > 0)
      {
        foreach (Battery battery in batteriesOnCircuit)
        {
          if ((double) this.batteryRefillPercent <= 0.0 && (double) battery.PercentFull <= 0.0)
          {
            flag2 = true;
            break;
          }
          if ((double) battery.PercentFull < (double) this.batteryRefillPercent)
          {
            flag2 = true;
            break;
          }
        }
      }
      else
        flag2 = true;
      if (!this.ignoreBatteryRefillPercent)
        this.selectable.ToggleStatusItem(EnergyGenerator.batteriesSufficientlyFull, !flag2);
      if (Object.op_Inequality((Object) this.delivery, (Object) null))
        this.delivery.Pause(!flag2, "Circuit has sufficient energy");
      if (this.formula.inputs != null)
      {
        bool flag3 = this.IsConvertible(dt);
        this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedResourceMass, !flag3, (object) this.formula);
        if (flag3)
        {
          foreach (EnergyGenerator.InputItem input in this.formula.inputs)
          {
            float amount = input.consumptionRate * dt;
            this.storage.ConsumeIgnoringDisease(input.tag, amount);
          }
          PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
          foreach (EnergyGenerator.OutputItem output in this.formula.outputs)
            this.Emit(output, dt, component);
          this.GenerateJoules(this.WattageRating * dt);
          this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, (object) this);
          flag1 = true;
        }
      }
    }
    this.operational.SetActive(flag1);
  }

  public List<Descriptor> RequirementDescriptors()
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    if (this.formula.inputs == null || this.formula.inputs.Length == 0)
      return descriptorList;
    for (int index = 0; index < this.formula.inputs.Length; ++index)
    {
      EnergyGenerator.InputItem input = this.formula.inputs[index];
      string str = input.tag.ProperName();
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMED, (object) str, (object) GameUtil.GetFormattedMass(input.consumptionRate, GameUtil.TimeSlice.PerSecond, floatFormat: "{0:0.##}")), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, (object) str, (object) GameUtil.GetFormattedMass(input.consumptionRate, GameUtil.TimeSlice.PerSecond, floatFormat: "{0:0.##}")), (Descriptor.DescriptorType) 0);
      descriptorList.Add(descriptor);
    }
    return descriptorList;
  }

  public List<Descriptor> EffectDescriptors()
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    if (this.formula.outputs == null || this.formula.outputs.Length == 0)
      return descriptorList;
    for (int index = 0; index < this.formula.outputs.Length; ++index)
    {
      EnergyGenerator.OutputItem output = this.formula.outputs[index];
      string str = ElementLoader.FindElementByHash(output.element).tag.ProperName();
      Descriptor descriptor = new Descriptor();
      if ((double) output.minTemperature > 0.0)
        ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTEMITTED_MINORENTITYTEMP, (object) str, (object) GameUtil.GetFormattedMass(output.creationRate, GameUtil.TimeSlice.PerSecond), (object) GameUtil.GetFormattedTemperature(output.minTemperature)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_MINORENTITYTEMP, (object) str, (object) GameUtil.GetFormattedMass(output.creationRate, GameUtil.TimeSlice.PerSecond), (object) GameUtil.GetFormattedTemperature(output.minTemperature)), (Descriptor.DescriptorType) 1);
      else
        ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTEMITTED_ENTITYTEMP, (object) str, (object) GameUtil.GetFormattedMass(output.creationRate, GameUtil.TimeSlice.PerSecond)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP, (object) str, (object) GameUtil.GetFormattedMass(output.creationRate, GameUtil.TimeSlice.PerSecond)), (Descriptor.DescriptorType) 1);
      descriptorList.Add(descriptor);
    }
    return descriptorList;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    foreach (Descriptor requirementDescriptor in this.RequirementDescriptors())
      descriptors.Add(requirementDescriptor);
    foreach (Descriptor effectDescriptor in this.EffectDescriptors())
      descriptors.Add(effectDescriptor);
    return descriptors;
  }

  public static StatusItem BatteriesSufficientlyFull => EnergyGenerator.batteriesSufficientlyFull;

  public static void EnsureStatusItemAvailable()
  {
    if (EnergyGenerator.batteriesSufficientlyFull != null)
      return;
    EnergyGenerator.batteriesSufficientlyFull = new StatusItem("BatteriesSufficientlyFull", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
  }

  public static EnergyGenerator.Formula CreateSimpleFormula(
    Tag input_element,
    float input_mass_rate,
    float max_stored_input_mass,
    SimHashes output_element = SimHashes.Void,
    float output_mass_rate = 0.0f,
    bool store_output_mass = true,
    CellOffset output_offset = default (CellOffset),
    float min_output_temperature = 0.0f)
  {
    EnergyGenerator.Formula simpleFormula = new EnergyGenerator.Formula();
    simpleFormula.inputs = new EnergyGenerator.InputItem[1]
    {
      new EnergyGenerator.InputItem(input_element, input_mass_rate, max_stored_input_mass)
    };
    if (output_element != SimHashes.Void)
      simpleFormula.outputs = new EnergyGenerator.OutputItem[1]
      {
        new EnergyGenerator.OutputItem(output_element, output_mass_rate, store_output_mass, output_offset, min_output_temperature)
      };
    else
      simpleFormula.outputs = (EnergyGenerator.OutputItem[]) null;
    return simpleFormula;
  }

  private void Emit(EnergyGenerator.OutputItem output, float dt, PrimaryElement root_pe)
  {
    Element elementByHash = ElementLoader.FindElementByHash(output.element);
    float num1 = output.creationRate * dt;
    if (output.store)
    {
      if (elementByHash.IsGas)
        this.storage.AddGasChunk(output.element, num1, root_pe.Temperature, byte.MaxValue, 0, true);
      else if (elementByHash.IsLiquid)
        this.storage.AddLiquid(output.element, num1, root_pe.Temperature, byte.MaxValue, 0, true);
      else
        this.storage.Store(elementByHash.substance.SpawnResource(TransformExtensions.GetPosition(this.transform), num1, root_pe.Temperature, byte.MaxValue, 0), true);
    }
    else
    {
      int num2 = Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), output.emitOffset);
      float temperature = Mathf.Max(root_pe.Temperature, output.minTemperature);
      if (elementByHash.IsGas)
        SimMessages.ModifyMass(num2, num1, byte.MaxValue, 0, CellEventLogger.Instance.EnergyGeneratorModifyMass, temperature, output.element);
      else if (elementByHash.IsLiquid)
      {
        ushort elementIndex = ElementLoader.GetElementIndex(output.element);
        FallingWater.instance.AddParticle(num2, elementIndex, num1, temperature, byte.MaxValue, 0, true);
      }
      else
        elementByHash.substance.SpawnResource(Grid.CellToPosCCC(num2, Grid.SceneLayer.Front), num1, temperature, byte.MaxValue, 0, true);
    }
  }

  [DebuggerDisplay("{tag} -{consumptionRate} kg/s")]
  [Serializable]
  public struct InputItem
  {
    public Tag tag;
    public float consumptionRate;
    public float maxStoredMass;

    public InputItem(Tag tag, float consumption_rate, float max_stored_mass)
    {
      this.tag = tag;
      this.consumptionRate = consumption_rate;
      this.maxStoredMass = max_stored_mass;
    }
  }

  [DebuggerDisplay("{element} {creationRate} kg/s")]
  [Serializable]
  public struct OutputItem
  {
    public SimHashes element;
    public float creationRate;
    public bool store;
    public CellOffset emitOffset;
    public float minTemperature;

    public OutputItem(SimHashes element, float creation_rate, bool store, float min_temperature = 0.0f)
      : this(element, creation_rate, store, CellOffset.none, min_temperature)
    {
    }

    public OutputItem(
      SimHashes element,
      float creation_rate,
      bool store,
      CellOffset emit_offset,
      float min_temperature = 0.0f)
    {
      this.element = element;
      this.creationRate = creation_rate;
      this.store = store;
      this.emitOffset = emit_offset;
      this.minTemperature = min_temperature;
    }
  }

  [Serializable]
  public struct Formula
  {
    public EnergyGenerator.InputItem[] inputs;
    public EnergyGenerator.OutputItem[] outputs;
    public Tag meterTag;
  }
}
