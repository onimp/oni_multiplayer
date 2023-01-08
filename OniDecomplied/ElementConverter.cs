// Decompiled with JetBrains decompiler
// Type: ElementConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[SerializationConfig]
public class ElementConverter : 
  StateMachineComponent<ElementConverter.StatesInstance>,
  IGameObjectEffectDescriptor
{
  [MyCmpGet]
  private Operational operational;
  [MyCmpReq]
  private Storage storage;
  public Action<float> onConvertMass;
  private float totalDiseaseWeight = float.MaxValue;
  public Operational.State OperationalRequirement = Operational.State.Active;
  private AttributeInstance machinerySpeedAttribute;
  private float workSpeedMultiplier = 1f;
  public bool showDescriptors = true;
  private const float BASE_INTERVAL = 1f;
  public ElementConverter.ConsumedElement[] consumedElements;
  public ElementConverter.OutputElement[] outputElements;
  public bool ShowInUI = true;
  private float outputMultiplier = 1f;
  private Dictionary<Tag, Guid> consumedElementStatusHandles = new Dictionary<Tag, Guid>();
  private Dictionary<SimHashes, Guid> outputElementStatusHandles = new Dictionary<SimHashes, Guid>();
  private static StatusItem ElementConverterInput;
  private static StatusItem ElementConverterOutput;

  public void SetWorkSpeedMultiplier(float speed) => this.workSpeedMultiplier = speed;

  public void SetConsumedElementActive(Tag elementId, bool active)
  {
    for (int index = 0; index < this.consumedElements.Length; ++index)
    {
      if (!Tag.op_Inequality(this.consumedElements[index].Tag, elementId))
      {
        this.consumedElements[index].IsActive = active;
        if (!this.ShowInUI)
          break;
        ElementConverter.ConsumedElement consumedElement = this.consumedElements[index];
        if (active)
        {
          this.smi.AddStatusItem<ElementConverter.ConsumedElement, Tag>(consumedElement, consumedElement.Tag, ElementConverter.ElementConverterInput, this.consumedElementStatusHandles);
          break;
        }
        this.smi.RemoveStatusItem<Tag>(consumedElement.Tag, this.consumedElementStatusHandles);
        break;
      }
    }
  }

  public void SetOutputElementActive(SimHashes element, bool active)
  {
    for (int index = 0; index < this.outputElements.Length; ++index)
    {
      if (this.outputElements[index].elementHash == element)
      {
        this.outputElements[index].IsActive = active;
        ElementConverter.OutputElement outputElement = this.outputElements[index];
        if (active)
        {
          this.smi.AddStatusItem<ElementConverter.OutputElement, SimHashes>(outputElement, outputElement.elementHash, ElementConverter.ElementConverterOutput, this.outputElementStatusHandles);
          break;
        }
        this.smi.RemoveStatusItem<SimHashes>(outputElement.elementHash, this.outputElementStatusHandles);
        break;
      }
    }
  }

  public void SetStorage(Storage storage) => this.storage = storage;

  public float OutputMultiplier
  {
    get => this.outputMultiplier;
    set => this.outputMultiplier = value;
  }

  public float AverageConvertRate => Game.Instance.accumulators.GetAverageRate(this.outputElements[0].accumulator);

  public bool HasEnoughMass(Tag tag, bool includeInactive = false)
  {
    bool flag = false;
    List<GameObject> items = this.storage.items;
    foreach (ElementConverter.ConsumedElement consumedElement in this.consumedElements)
    {
      if (!Tag.op_Inequality(tag, consumedElement.Tag) && (includeInactive || consumedElement.IsActive))
      {
        float num = 0.0f;
        for (int index = 0; index < items.Count; ++index)
        {
          GameObject go = items[index];
          if (!Object.op_Equality((Object) go, (Object) null) && go.HasTag(tag))
            num += go.GetComponent<PrimaryElement>().Mass;
        }
        flag = (double) num >= (double) consumedElement.MassConsumptionRate;
        break;
      }
    }
    return flag;
  }

  public bool HasEnoughMassToStartConverting(bool includeInactive = false)
  {
    float num1 = 1f * this.GetSpeedMultiplier();
    bool flag1 = includeInactive || this.consumedElements.Length == 0;
    bool flag2 = true;
    List<GameObject> items = this.storage.items;
    for (int index1 = 0; index1 < this.consumedElements.Length; ++index1)
    {
      ElementConverter.ConsumedElement consumedElement = this.consumedElements[index1];
      flag1 |= consumedElement.IsActive;
      if (includeInactive || consumedElement.IsActive)
      {
        float num2 = 0.0f;
        for (int index2 = 0; index2 < items.Count; ++index2)
        {
          GameObject go = items[index2];
          if (!Object.op_Equality((Object) go, (Object) null) && go.HasTag(consumedElement.Tag))
            num2 += go.GetComponent<PrimaryElement>().Mass;
        }
        if ((double) num2 < (double) consumedElement.MassConsumptionRate * (double) num1)
        {
          flag2 = false;
          break;
        }
      }
    }
    return flag1 & flag2;
  }

  public bool CanConvertAtAll()
  {
    bool flag1 = this.consumedElements.Length == 0;
    bool flag2 = true;
    List<GameObject> items = this.storage.items;
    for (int index1 = 0; index1 < this.consumedElements.Length; ++index1)
    {
      ElementConverter.ConsumedElement consumedElement = this.consumedElements[index1];
      flag1 |= consumedElement.IsActive;
      if (consumedElement.IsActive)
      {
        bool flag3 = false;
        for (int index2 = 0; index2 < items.Count; ++index2)
        {
          GameObject go = items[index2];
          if (!Object.op_Equality((Object) go, (Object) null) && go.HasTag(consumedElement.Tag) && (double) go.GetComponent<PrimaryElement>().Mass > 0.0)
          {
            flag3 = true;
            break;
          }
        }
        if (!flag3)
        {
          flag2 = false;
          break;
        }
      }
    }
    return flag1 & flag2;
  }

  private float GetSpeedMultiplier() => this.machinerySpeedAttribute.GetTotalValue() * this.workSpeedMultiplier;

  private void ConvertMass()
  {
    float num1 = 1f * this.GetSpeedMultiplier();
    bool flag = this.consumedElements.Length == 0;
    float num2 = 1f;
    for (int index1 = 0; index1 < this.consumedElements.Length; ++index1)
    {
      ElementConverter.ConsumedElement consumedElement = this.consumedElements[index1];
      flag |= consumedElement.IsActive;
      if (consumedElement.IsActive)
      {
        float num3 = consumedElement.MassConsumptionRate * num1 * num2;
        if ((double) num3 <= 0.0)
        {
          num2 = 0.0f;
          break;
        }
        float num4 = 0.0f;
        for (int index2 = 0; index2 < this.storage.items.Count; ++index2)
        {
          GameObject go = this.storage.items[index2];
          if (!Object.op_Equality((Object) go, (Object) null) && go.HasTag(consumedElement.Tag))
          {
            PrimaryElement component = go.GetComponent<PrimaryElement>();
            float num5 = Mathf.Min(num3, component.Mass);
            num4 += num5 / num3;
          }
        }
        num2 = Mathf.Min(num2, num4);
      }
    }
    if (!flag || (double) num2 <= 0.0)
      return;
    SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid with
    {
      idx = byte.MaxValue,
      count = 0
    };
    float num6 = 0.0f;
    float num7 = 0.0f;
    float num8 = 0.0f;
    for (int index3 = 0; index3 < this.consumedElements.Length; ++index3)
    {
      ElementConverter.ConsumedElement consumedElement = this.consumedElements[index3];
      if (consumedElement.IsActive)
      {
        float amount = consumedElement.MassConsumptionRate * num1 * num2;
        Game.Instance.accumulators.Accumulate(consumedElement.Accumulator, amount);
        for (int index4 = 0; index4 < this.storage.items.Count; ++index4)
        {
          GameObject go = this.storage.items[index4];
          if (!Object.op_Equality((Object) go, (Object) null))
          {
            if (go.HasTag(consumedElement.Tag))
            {
              PrimaryElement component = go.GetComponent<PrimaryElement>();
              component.KeepZeroMassObject = true;
              float num9 = Mathf.Min(amount, component.Mass);
              int src2_count = (int) ((double) num9 / (double) component.Mass * (double) component.DiseaseCount);
              float num10 = num9 * component.Element.specificHeatCapacity;
              num8 += num10;
              num7 += num10 * component.Temperature;
              component.Mass -= num9;
              component.ModifyDiseaseCount(-src2_count, "ElementConverter.ConvertMass");
              num6 += num9;
              diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo.idx, diseaseInfo.count, component.DiseaseIdx, src2_count);
              amount -= num9;
              if ((double) amount <= 0.0)
                break;
            }
            if ((double) amount <= 0.0)
              Debug.Assert((double) amount <= 0.0);
          }
        }
      }
    }
    float num11 = (double) num8 > 0.0 ? num7 / num8 : 0.0f;
    if (this.onConvertMass != null && (double) num6 > 0.0)
      this.onConvertMass(num6);
    for (int index = 0; index < this.outputElements.Length; ++index)
    {
      ElementConverter.OutputElement outputElement = this.outputElements[index];
      if (outputElement.IsActive)
      {
        SimUtil.DiseaseInfo a = diseaseInfo;
        if ((double) this.totalDiseaseWeight <= 0.0)
        {
          a.idx = byte.MaxValue;
          a.count = 0;
        }
        else
        {
          float num12 = outputElement.diseaseWeight / this.totalDiseaseWeight;
          a.count = (int) ((double) a.count * (double) num12);
        }
        if (outputElement.addedDiseaseIdx != byte.MaxValue)
          a = SimUtil.CalculateFinalDiseaseInfo(a, new SimUtil.DiseaseInfo()
          {
            idx = outputElement.addedDiseaseIdx,
            count = outputElement.addedDiseaseCount
          });
        float num13 = outputElement.massGenerationRate * this.OutputMultiplier * num1 * num2;
        Game.Instance.accumulators.Accumulate(outputElement.accumulator, num13);
        float temperature = outputElement.useEntityTemperature || (double) num11 == 0.0 && (double) outputElement.minOutputTemperature == 0.0 ? ((Component) this).GetComponent<PrimaryElement>().Temperature : Mathf.Max(outputElement.minOutputTemperature, num11);
        Element elementByHash = ElementLoader.FindElementByHash(outputElement.elementHash);
        if (outputElement.storeOutput)
        {
          PrimaryElement primaryElement = this.storage.AddToPrimaryElement(outputElement.elementHash, num13, temperature);
          if (Object.op_Equality((Object) primaryElement, (Object) null))
          {
            if (elementByHash.IsGas)
              this.storage.AddGasChunk(outputElement.elementHash, num13, temperature, a.idx, a.count, true);
            else if (elementByHash.IsLiquid)
              this.storage.AddLiquid(outputElement.elementHash, num13, temperature, a.idx, a.count, true);
            else
              this.storage.Store(elementByHash.substance.SpawnResource(TransformExtensions.GetPosition(this.transform), num13, temperature, a.idx, a.count, true), true);
          }
          else
            primaryElement.AddDisease(a.idx, a.count, "ElementConverter.ConvertMass");
        }
        else
        {
          Vector3 vector3;
          // ISSUE: explicit constructor call
          ((Vector3) ref vector3).\u002Ector(TransformExtensions.GetPosition(this.transform).x + outputElement.outputElementOffset.x, TransformExtensions.GetPosition(this.transform).y + outputElement.outputElementOffset.y, 0.0f);
          int cell = Grid.PosToCell(vector3);
          if (elementByHash.IsLiquid)
            FallingWater.instance.AddParticle(cell, elementByHash.idx, num13, temperature, a.idx, a.count, true);
          else if (elementByHash.IsSolid)
            elementByHash.substance.SpawnResource(vector3, num13, temperature, a.idx, a.count);
          else
            SimMessages.AddRemoveSubstance(cell, outputElement.elementHash, CellEventLogger.Instance.OxygenModifierSimUpdate, num13, temperature, a.idx, a.count);
        }
        if (outputElement.elementHash == SimHashes.Oxygen || outputElement.elementHash == SimHashes.ContaminatedOxygen)
          ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num13, ((Component) this).gameObject.GetProperName());
      }
    }
    this.storage.Trigger(-1697596308, (object) ((Component) this).gameObject);
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.machinerySpeedAttribute = ((Component) this).gameObject.GetAttributes().Add(Db.Get().Attributes.MachinerySpeed);
    if (ElementConverter.ElementConverterInput == null)
      ElementConverter.ElementConverterInput = new StatusItem("ElementConverterInput", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID).SetResolveStringCallback((Func<string, object, string>) ((str, data) =>
      {
        ElementConverter.ConsumedElement consumedElement = (ElementConverter.ConsumedElement) data;
        str = str.Replace("{ElementTypes}", consumedElement.Name);
        str = str.Replace("{FlowRate}", GameUtil.GetFormattedByTag(consumedElement.Tag, consumedElement.Rate, GameUtil.TimeSlice.PerSecond));
        return str;
      }));
    if (ElementConverter.ElementConverterOutput != null)
      return;
    ElementConverter.ElementConverterOutput = new StatusItem("ElementConverterOutput", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID).SetResolveStringCallback((Func<string, object, string>) ((str, data) =>
    {
      ElementConverter.OutputElement outputElement = (ElementConverter.OutputElement) data;
      str = str.Replace("{ElementTypes}", outputElement.Name);
      str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(outputElement.Rate, GameUtil.TimeSlice.PerSecond));
      return str;
    }));
  }

  public void SetAllConsumedActive(bool active)
  {
    for (int index = 0; index < this.consumedElements.Length; ++index)
      this.consumedElements[index].IsActive = active;
    this.smi.sm.canConvert.Set(active, this.smi);
  }

  public void SetConsumedActive(Tag id, bool active)
  {
    bool flag = this.consumedElements.Length == 0;
    for (int index = 0; index < this.consumedElements.Length; ++index)
    {
      ref ElementConverter.ConsumedElement local = ref this.consumedElements[index];
      if (Tag.op_Equality(local.Tag, id))
      {
        local.IsActive = active;
        if (active)
        {
          flag = true;
          break;
        }
      }
      flag |= local.IsActive;
    }
    this.smi.sm.canConvert.Set(flag, this.smi);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    for (int index = 0; index < this.consumedElements.Length; ++index)
      this.consumedElements[index].Accumulator = Game.Instance.accumulators.Add("ElementsConsumed", (KMonoBehaviour) this);
    this.totalDiseaseWeight = 0.0f;
    for (int index = 0; index < this.outputElements.Length; ++index)
    {
      this.outputElements[index].accumulator = Game.Instance.accumulators.Add("OutputElements", (KMonoBehaviour) this);
      this.totalDiseaseWeight += this.outputElements[index].diseaseWeight;
    }
    this.smi.StartSM();
  }

  protected override void OnCleanUp()
  {
    for (int index = 0; index < this.consumedElements.Length; ++index)
      Game.Instance.accumulators.Remove(this.consumedElements[index].Accumulator);
    for (int index = 0; index < this.outputElements.Length; ++index)
      Game.Instance.accumulators.Remove(this.outputElements[index].accumulator);
    base.OnCleanUp();
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    if (!this.showDescriptors)
      return descriptors;
    if (this.consumedElements != null)
    {
      foreach (ElementConverter.ConsumedElement consumedElement in this.consumedElements)
      {
        if (consumedElement.IsActive)
        {
          Descriptor descriptor = new Descriptor();
          ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMED, (object) consumedElement.Name, (object) GameUtil.GetFormattedMass(consumedElement.MassConsumptionRate, GameUtil.TimeSlice.PerSecond, floatFormat: "{0:0.##}")), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, (object) consumedElement.Name, (object) GameUtil.GetFormattedMass(consumedElement.MassConsumptionRate, GameUtil.TimeSlice.PerSecond, floatFormat: "{0:0.##}")), (Descriptor.DescriptorType) 0);
          descriptors.Add(descriptor);
        }
      }
    }
    if (this.outputElements != null)
    {
      foreach (ElementConverter.OutputElement outputElement in this.outputElements)
      {
        if (outputElement.IsActive)
        {
          LocString format1 = UI.BUILDINGEFFECTS.ELEMENTEMITTED_INPUTTEMP;
          LocString format2 = UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_INPUTTEMP;
          if (outputElement.useEntityTemperature)
          {
            format1 = UI.BUILDINGEFFECTS.ELEMENTEMITTED_ENTITYTEMP;
            format2 = UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP;
          }
          else if ((double) outputElement.minOutputTemperature > 0.0)
          {
            format1 = UI.BUILDINGEFFECTS.ELEMENTEMITTED_MINTEMP;
            format2 = UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_MINTEMP;
          }
          Descriptor descriptor;
          // ISSUE: explicit constructor call
          ((Descriptor) ref descriptor).\u002Ector(string.Format((string) format1, (object) outputElement.Name, (object) GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, floatFormat: "{0:0.##}"), (object) GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature)), string.Format((string) format2, (object) outputElement.Name, (object) GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, floatFormat: "{0:0.##}"), (object) GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature)), (Descriptor.DescriptorType) 1, false);
          descriptors.Add(descriptor);
        }
      }
    }
    return descriptors;
  }

  [DebuggerDisplay("{tag} {massConsumptionRate}")]
  [Serializable]
  public struct ConsumedElement
  {
    public Tag Tag;
    public float MassConsumptionRate;
    public bool IsActive;
    public HandleVector<int>.Handle Accumulator;

    public ConsumedElement(Tag tag, float kgPerSecond, bool isActive = true)
    {
      this.Tag = tag;
      this.MassConsumptionRate = kgPerSecond;
      this.IsActive = isActive;
      this.Accumulator = HandleVector<int>.InvalidHandle;
    }

    public string Name => this.Tag.ProperName();

    public float Rate => Game.Instance.accumulators.GetAverageRate(this.Accumulator);
  }

  [Serializable]
  public struct OutputElement
  {
    public bool IsActive;
    public SimHashes elementHash;
    public float minOutputTemperature;
    public bool useEntityTemperature;
    public float massGenerationRate;
    public bool storeOutput;
    public Vector2 outputElementOffset;
    public HandleVector<int>.Handle accumulator;
    public float diseaseWeight;
    public byte addedDiseaseIdx;
    public int addedDiseaseCount;

    public OutputElement(
      float kgPerSecond,
      SimHashes element,
      float minOutputTemperature,
      bool useEntityTemperature = false,
      bool storeOutput = false,
      float outputElementOffsetx = 0.0f,
      float outputElementOffsety = 0.5f,
      float diseaseWeight = 1f,
      byte addedDiseaseIdx = 255,
      int addedDiseaseCount = 0,
      bool isActive = true)
    {
      this.elementHash = element;
      this.minOutputTemperature = minOutputTemperature;
      this.useEntityTemperature = useEntityTemperature;
      this.storeOutput = storeOutput;
      this.massGenerationRate = kgPerSecond;
      this.outputElementOffset = new Vector2(outputElementOffsetx, outputElementOffsety);
      this.accumulator = HandleVector<int>.InvalidHandle;
      this.diseaseWeight = diseaseWeight;
      this.addedDiseaseIdx = addedDiseaseIdx;
      this.addedDiseaseCount = addedDiseaseCount;
      this.IsActive = isActive;
    }

    public string Name => ElementLoader.FindElementByHash(this.elementHash).tag.ProperName();

    public float Rate => Game.Instance.accumulators.GetAverageRate(this.accumulator);
  }

  public class StatesInstance : 
    GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.GameInstance
  {
    private KSelectable selectable;

    public StatesInstance(ElementConverter smi)
      : base(smi)
    {
      this.selectable = this.GetComponent<KSelectable>();
    }

    public void AddStatusItems()
    {
      if (!this.master.ShowInUI)
        return;
      foreach (ElementConverter.ConsumedElement consumedElement in this.master.consumedElements)
      {
        if (consumedElement.IsActive)
          this.AddStatusItem<ElementConverter.ConsumedElement, Tag>(consumedElement, consumedElement.Tag, ElementConverter.ElementConverterInput, this.master.consumedElementStatusHandles);
      }
      foreach (ElementConverter.OutputElement outputElement in this.master.outputElements)
      {
        if (outputElement.IsActive)
          this.AddStatusItem<ElementConverter.OutputElement, SimHashes>(outputElement, outputElement.elementHash, ElementConverter.ElementConverterOutput, this.master.outputElementStatusHandles);
      }
    }

    public void RemoveStatusItems()
    {
      if (!this.master.ShowInUI)
        return;
      for (int index = 0; index < this.master.consumedElements.Length; ++index)
        this.RemoveStatusItem<Tag>(this.master.consumedElements[index].Tag, this.master.consumedElementStatusHandles);
      for (int index = 0; index < this.master.outputElements.Length; ++index)
        this.RemoveStatusItem<SimHashes>(this.master.outputElements[index].elementHash, this.master.outputElementStatusHandles);
      this.master.consumedElementStatusHandles.Clear();
      this.master.outputElementStatusHandles.Clear();
    }

    public void AddStatusItem<ElementType, IDType>(
      ElementType element,
      IDType id,
      StatusItem status,
      Dictionary<IDType, Guid> collection)
    {
      Guid guid = this.selectable.AddStatusItem(status, (object) element);
      collection[id] = guid;
    }

    public void RemoveStatusItem<IDType>(IDType id, Dictionary<IDType, Guid> collection)
    {
      Guid guid;
      if (!collection.TryGetValue(id, out guid))
        return;
      this.selectable.RemoveStatusItem(guid);
    }

    public void OnOperationalRequirementChanged(object data)
    {
      Operational operational = data as Operational;
      this.sm.canConvert.Set(Object.op_Equality((Object) operational, (Object) null) ? (bool) data : operational.IsActive, this);
    }
  }

  public class States : 
    GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter>
  {
    public GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State disabled;
    public GameStateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State converting;
    public StateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.BoolParameter canConvert;

    private bool ValidateStateTransition(ElementConverter.StatesInstance smi, bool _)
    {
      bool flag1 = smi.GetCurrentState() == smi.sm.disabled;
      if (Object.op_Equality((Object) smi.master.operational, (Object) null))
        return flag1;
      bool flag2 = smi.master.consumedElements.Length == 0;
      bool flag3 = this.canConvert.Get(smi);
      for (int index = 0; !flag2 && index < smi.master.consumedElements.Length; ++index)
        flag2 = smi.master.consumedElements[index].IsActive;
      if (!flag3 || flag2)
        return smi.master.operational.MeetsRequirements(smi.master.OperationalRequirement) == flag1;
      this.canConvert.Set(false, smi, true);
      return false;
    }

    private void OnEnterRoot(ElementConverter.StatesInstance smi)
    {
      int eventForState = (int) Operational.GetEventForState(smi.master.OperationalRequirement);
      smi.Subscribe(eventForState, new Action<object>(smi.OnOperationalRequirementChanged));
    }

    private void OnExitRoot(ElementConverter.StatesInstance smi)
    {
      int eventForState = (int) Operational.GetEventForState(smi.master.OperationalRequirement);
      smi.Unsubscribe(eventForState, new Action<object>(smi.OnOperationalRequirementChanged));
    }

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.disabled;
      this.root.Enter(new StateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State.Callback(this.OnEnterRoot)).Exit(new StateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State.Callback(this.OnExitRoot));
      this.disabled.ParamTransition<bool>((StateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.Parameter<bool>) this.canConvert, this.converting, new StateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.Parameter<bool>.Callback(this.ValidateStateTransition));
      this.converting.Enter("AddStatusItems", (StateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State.Callback) (smi => smi.AddStatusItems())).Exit("RemoveStatusItems", (StateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.State.Callback) (smi => smi.RemoveStatusItems())).ParamTransition<bool>((StateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.Parameter<bool>) this.canConvert, this.disabled, new StateMachine<ElementConverter.States, ElementConverter.StatesInstance, ElementConverter, object>.Parameter<bool>.Callback(this.ValidateStateTransition)).Update("ConvertMass", (Action<ElementConverter.StatesInstance, float>) ((smi, dt) => smi.master.ConvertMass()), (UpdateRate) 6, true);
    }
  }
}
