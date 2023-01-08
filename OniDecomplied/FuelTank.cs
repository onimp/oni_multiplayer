// Decompiled with JetBrains decompiler
// Type: FuelTank
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank : KMonoBehaviour, IUserControlledCapacity, IFuelTank
{
  public global::Storage storage;
  private MeterController meter;
  [Serialize]
  public float targetFillMass = -1f;
  [SerializeField]
  public float physicalFuelCapacity;
  public bool consumeFuelOnLand;
  [SerializeField]
  private Tag fuelType;
  private static readonly EventSystem.IntraObjectHandler<FuelTank> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<FuelTank>((Action<FuelTank, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly EventSystem.IntraObjectHandler<FuelTank> OnRocketLandedDelegate = new EventSystem.IntraObjectHandler<FuelTank>((Action<FuelTank, object>) ((component, data) => component.OnRocketLanded(data)));
  private static readonly EventSystem.IntraObjectHandler<FuelTank> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<FuelTank>((Action<FuelTank, object>) ((component, data) => component.OnStorageChange(data)));

  public IStorage Storage => (IStorage) this.storage;

  public bool ConsumeFuelOnLand => this.consumeFuelOnLand;

  public float UserMaxCapacity
  {
    get => this.targetFillMass;
    set
    {
      this.targetFillMass = value;
      this.storage.capacityKg = this.targetFillMass;
      ConduitConsumer component1 = ((Component) this).GetComponent<ConduitConsumer>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        component1.capacityKG = this.targetFillMass;
      ManualDeliveryKG component2 = ((Component) this).GetComponent<ManualDeliveryKG>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        component2.capacity = component2.refillMass = this.targetFillMass;
      this.Trigger(-945020481, (object) this);
    }
  }

  public float MinCapacity => 0.0f;

  public float MaxCapacity => this.physicalFuelCapacity;

  public float AmountStored => this.storage.MassStored();

  public bool WholeValues => false;

  public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

  public Tag FuelType
  {
    get => this.fuelType;
    set
    {
      this.fuelType = value;
      if (this.storage.storageFilters == null)
        this.storage.storageFilters = new List<Tag>();
      this.storage.storageFilters.Add(this.fuelType);
      ManualDeliveryKG component = ((Component) this).GetComponent<ManualDeliveryKG>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.RequestedItemTag = this.fuelType;
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<FuelTank>(-905833192, FuelTank.OnCopySettingsDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if ((double) this.targetFillMass == -1.0)
      this.targetFillMass = this.physicalFuelCapacity;
    ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("grounded"), (KAnim.PlayMode) 0);
    if (DlcManager.FeatureClusterSpaceEnabled())
      ((Component) this).GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition) new ConditionProperlyFueled((IFuelTank) this));
    this.Subscribe<FuelTank>(-887025858, FuelTank.OnRocketLandedDelegate);
    this.UserMaxCapacity = this.UserMaxCapacity;
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[4]
    {
      "meter_target",
      "meter_fill",
      "meter_frame",
      "meter_OL"
    });
    this.meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
    this.OnStorageChange((object) null);
    this.Subscribe<FuelTank>(-1697596308, FuelTank.OnStorageChangedDelegate);
  }

  private void OnStorageChange(object data) => this.meter.SetPositionPercent(this.storage.MassStored() / this.storage.capacityKg);

  private void OnRocketLanded(object data)
  {
    if (!this.ConsumeFuelOnLand)
      return;
    this.storage.ConsumeAllIgnoringDisease();
  }

  private void OnCopySettings(object data)
  {
    FuelTank component = ((GameObject) data).GetComponent<FuelTank>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.UserMaxCapacity = component.UserMaxCapacity;
  }

  public void DEBUG_FillTank()
  {
    if (DlcManager.FeatureClusterSpaceEnabled())
    {
      RocketEngineCluster rocketEngineCluster = (RocketEngineCluster) null;
      foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this).GetComponent<AttachableBuilding>()))
      {
        rocketEngineCluster = gameObject.GetComponent<RocketEngineCluster>();
        if (Object.op_Inequality((Object) rocketEngineCluster, (Object) null))
        {
          if (rocketEngineCluster.mainEngine)
            break;
        }
      }
      if (Object.op_Inequality((Object) rocketEngineCluster, (Object) null))
      {
        Element element = ElementLoader.GetElement(rocketEngineCluster.fuelTag);
        if (element.IsLiquid)
          this.storage.AddLiquid(element.id, this.targetFillMass - this.storage.MassStored(), element.defaultValues.temperature, (byte) 0, 0);
        else if (element.IsGas)
          this.storage.AddGasChunk(element.id, this.targetFillMass - this.storage.MassStored(), element.defaultValues.temperature, (byte) 0, 0, false);
        else if (element.IsSolid)
          this.storage.AddOre(element.id, this.targetFillMass - this.storage.MassStored(), element.defaultValues.temperature, (byte) 0, 0);
        ((Component) ((Component) rocketEngineCluster).GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>().UpdateStatusItem();
      }
      else
        Debug.LogWarning((object) "Fuel tank couldn't find rocket engine");
    }
    else
    {
      RocketEngine rocketEngine = (RocketEngine) null;
      foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this).GetComponent<AttachableBuilding>()))
      {
        rocketEngine = gameObject.GetComponent<RocketEngine>();
        if (Object.op_Inequality((Object) rocketEngine, (Object) null))
        {
          if (rocketEngine.mainEngine)
            break;
        }
      }
      if (Object.op_Inequality((Object) rocketEngine, (Object) null))
      {
        Element element = ElementLoader.GetElement(rocketEngine.fuelTag);
        if (element.IsLiquid)
          this.storage.AddLiquid(element.id, this.targetFillMass - this.storage.MassStored(), element.defaultValues.temperature, (byte) 0, 0);
        else if (element.IsGas)
        {
          this.storage.AddGasChunk(element.id, this.targetFillMass - this.storage.MassStored(), element.defaultValues.temperature, (byte) 0, 0, false);
        }
        else
        {
          if (!element.IsSolid)
            return;
          this.storage.AddOre(element.id, this.targetFillMass - this.storage.MassStored(), element.defaultValues.temperature, (byte) 0, 0);
        }
      }
      else
        Debug.LogWarning((object) "Fuel tank couldn't find rocket engine");
    }
  }
}
