// Decompiled with JetBrains decompiler
// Type: ContactConductivePipeBridge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ContactConductivePipeBridge : 
  GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>
{
  private const string loopAnimName = "on";
  private GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>.State idle;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.idle.PlayAnim("on", (KAnim.PlayMode) 0).Update("", new System.Action<ContactConductivePipeBridge.Instance, float>(ContactConductivePipeBridge.Flow200ms));
  }

  private static void Flow200ms(ContactConductivePipeBridge.Instance smi, float dt)
  {
    if (!Object.op_Inequality((Object) smi.storage, (Object) null) || smi.storage.items.Count <= 0)
      return;
    ContactConductivePipeBridge.ExchangeStorageTemperatureWithBuilding200ms(smi, smi.storage, smi.building, smi.tag, dt);
    List<GameObject> items = smi.storage.items;
    for (int index = 0; index < items.Count; ++index)
    {
      PrimaryElement component = items[index].GetComponent<PrimaryElement>();
      if ((double) component.Mass > 0.0)
      {
        float num1 = (smi.def.type == ConduitType.Liquid ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow).AddElement(smi.outputCell, component.ElementID, component.Mass, component.Temperature, component.DiseaseIdx, component.DiseaseCount);
        component.KeepZeroMassObject = true;
        float num2 = num1 / component.Mass;
        int num3 = (int) ((double) component.DiseaseCount * (double) num2);
        component.Mass -= num1;
        component.ModifyDiseaseCount(-num3, "ContactConductivePipeBridge.Flow200ms");
      }
    }
  }

  private static void ExchangeStorageTemperatureWithBuilding200ms(
    ContactConductivePipeBridge.Instance smi,
    Storage storage,
    Building building,
    Tag tag,
    float dt)
  {
    List<GameObject> items = storage.items;
    for (int index = 0; index < items.Count; ++index)
    {
      PrimaryElement component1 = items[index].GetComponent<PrimaryElement>();
      if ((double) component1.Mass > 0.0 && ((Component) component1).HasTag(tag))
      {
        PrimaryElement primaryElement = component1;
        PrimaryElement component2 = ((Component) building).GetComponent<PrimaryElement>();
        float content_heat_capacity = primaryElement.Mass * primaryElement.Element.specificHeatCapacity;
        float building_heat_capacity = building.Def.MassForTemperatureModification * component2.Element.specificHeatCapacity;
        float temperature1 = component2.Temperature;
        float temperature2 = primaryElement.Temperature;
        float contentTemperature = ContactConductivePipeBridge.GetFinalContentTemperature(ContactConductivePipeBridge.GetKilloJoulesTransfered(ContactConductivePipeBridge.CalculateMaxWattsTransfered(temperature1, component2.Element.thermalConductivity, temperature2, primaryElement.Element.thermalConductivity), dt, temperature1, building_heat_capacity, temperature2, content_heat_capacity), temperature1, building_heat_capacity, temperature2, content_heat_capacity);
        float buildingTemperature = ContactConductivePipeBridge.GetFinalBuildingTemperature(temperature2, contentTemperature, content_heat_capacity, temperature1, building_heat_capacity);
        if (((double) buildingTemperature - (double) temperature1) * (double) building_heat_capacity + (double) ((contentTemperature - temperature2) * content_heat_capacity) != 0.0)
          ;
        if ((((double) buildingTemperature < 0.0 ? 0 : ((double) buildingTemperature <= 10000.0 ? 1 : 0)) & ((double) contentTemperature < 0.0 ? 0 : ((double) contentTemperature <= 10000.0 ? 1 : 0))) != 0)
        {
          primaryElement.Temperature = contentTemperature;
          component2.Temperature = buildingTemperature;
        }
      }
    }
  }

  private static float CalculateMaxWattsTransfered(
    float buildingTemperature,
    float building_thermal_conductivity,
    float content_temperature,
    float content_thermal_conductivity)
  {
    float num1 = 1f;
    float num2 = 1f;
    float num3 = 50f;
    return (float) (((double) content_temperature - (double) buildingTemperature) * (((double) content_thermal_conductivity + (double) building_thermal_conductivity) * 0.5)) * num1 * num3 / num2;
  }

  private static float GetKilloJoulesTransfered(
    float maxWattsTransfered,
    float dt,
    float building_Temperature,
    float building_heat_capacity,
    float content_temperature,
    float content_heat_capacity)
  {
    float num1 = (float) ((double) maxWattsTransfered * (double) dt / 1000.0);
    float num2 = Mathf.Min(content_temperature, building_Temperature);
    float num3 = Mathf.Max(content_temperature, building_Temperature);
    double num4 = (double) content_temperature - (double) num1 / (double) content_heat_capacity;
    float num5 = building_Temperature + num1 / building_heat_capacity;
    double num6 = (double) num2;
    double num7 = (double) num3;
    double num8 = (double) Mathf.Clamp((float) num4, (float) num6, (float) num7);
    float num9 = Mathf.Clamp(num5, num2, num3);
    double num10 = (double) content_temperature;
    float num11 = Mathf.Abs((float) (num8 - num10));
    double num12 = (double) Mathf.Abs(num9 - building_Temperature);
    float num13 = num11 * content_heat_capacity;
    double num14 = (double) building_heat_capacity;
    float num15 = (float) (num12 * num14);
    return Mathf.Min(num13, num15) * Mathf.Sign(maxWattsTransfered);
  }

  private static float GetFinalContentTemperature(
    float KJT,
    float building_Temperature,
    float building_heat_capacity,
    float content_temperature,
    float content_heat_capacity)
  {
    float num1 = -KJT;
    float num2 = Mathf.Max(0.0f, content_temperature + num1 / content_heat_capacity);
    float num3 = Mathf.Max(0.0f, building_Temperature - num1 / building_heat_capacity);
    return ((double) content_temperature - (double) building_Temperature) * ((double) num2 - (double) num3) < 0.0 ? (float) ((double) content_temperature * (double) content_heat_capacity / ((double) content_heat_capacity + (double) building_heat_capacity) + (double) building_Temperature * (double) building_heat_capacity / ((double) content_heat_capacity + (double) building_heat_capacity)) : num2;
  }

  private static float GetFinalBuildingTemperature(
    float content_temperature,
    float content_final_temperature,
    float content_heat_capacity,
    float building_temperature,
    float building_heat_capacity)
  {
    double num1 = ((double) content_temperature - (double) content_final_temperature) * (double) content_heat_capacity;
    float num2 = Mathf.Min(content_temperature, building_temperature);
    float num3 = Mathf.Max(content_temperature, building_temperature);
    double num4 = (double) building_heat_capacity;
    float num5 = (float) (num1 / num4);
    return Mathf.Clamp(building_temperature + num5, num2, num3);
  }

  public class Def : StateMachine.BaseDef
  {
    public ConduitType type = ConduitType.Liquid;
    public float pumpKGRate;
  }

  public new class Instance : 
    GameStateMachine<ContactConductivePipeBridge, ContactConductivePipeBridge.Instance, IStateMachineTarget, ContactConductivePipeBridge.Def>.GameInstance
  {
    public ConduitType type = ConduitType.Liquid;
    public HandleVector<int>.Handle structureHandle;
    public int outputCell = -1;
    [MyCmpGet]
    public Storage storage;
    [MyCmpGet]
    public Building building;
    [MyCmpGet]
    public ConduitDispenser conduitDispenser;

    public Tag tag => this.type != ConduitType.Liquid ? GameTags.Gas : GameTags.Liquid;

    public Instance(IStateMachineTarget master, ContactConductivePipeBridge.Def def)
      : base(master, def)
    {
    }

    public override void StartSM()
    {
      base.StartSM();
      this.outputCell = this.building.GetUtilityOutputCell();
      this.structureHandle = GameComps.StructureTemperatures.GetHandle(this.gameObject);
    }

    protected override void OnCleanUp()
    {
      base.OnCleanUp();
      this.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
    }
  }
}
