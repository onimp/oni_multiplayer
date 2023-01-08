// Decompiled with JetBrains decompiler
// Type: Battery
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
[DebuggerDisplay("{name}")]
[AddComponentMenu("KMonoBehaviour/scripts/Battery")]
public class Battery : 
  KMonoBehaviour,
  IEnergyConsumer,
  ICircuitConnected,
  IGameObjectEffectDescriptor,
  IEnergyProducer
{
  [SerializeField]
  public float capacity;
  [SerializeField]
  public float chargeWattage = float.PositiveInfinity;
  [Serialize]
  private float joulesAvailable;
  [MyCmpGet]
  protected Operational operational;
  [MyCmpGet]
  public PowerTransformer powerTransformer;
  protected MeterController meter;
  public float joulesLostPerSecond;
  [SerializeField]
  public int powerSortOrder;
  private float PreviousJoulesAvailable;
  private CircuitManager.ConnectionStatus connectionStatus;
  public static readonly Tag[] DEFAULT_CONNECTED_TAGS = new Tag[1]
  {
    GameTags.Operational
  };
  [SerializeField]
  public Tag[] connectedTags = Battery.DEFAULT_CONNECTED_TAGS;
  private static readonly EventSystem.IntraObjectHandler<Battery> OnTagsChangedDelegate = new EventSystem.IntraObjectHandler<Battery>((Action<Battery, object>) ((component, data) => component.OnTagsChanged(data)));
  private float dt;
  private float joulesConsumed;

  public float WattsUsed { get; private set; }

  public float WattsNeededWhenActive => 0.0f;

  public float PercentFull => this.joulesAvailable / this.capacity;

  public float PreviousPercentFull => this.PreviousJoulesAvailable / this.capacity;

  public float JoulesAvailable => this.joulesAvailable;

  public float Capacity => this.capacity;

  public float ChargeCapacity { get; private set; }

  public int PowerSortOrder => this.powerSortOrder;

  public string Name => ((Component) this).GetComponent<KSelectable>().GetName();

  public int PowerCell { get; private set; }

  public ushort CircuitID => Game.Instance.circuitManager.GetCircuitID((ICircuitConnected) this);

  public bool IsConnected => this.connectionStatus != 0;

  public bool IsPowered => this.connectionStatus == CircuitManager.ConnectionStatus.Powered;

  public bool IsVirtual { get; protected set; }

  public object VirtualCircuitKey { get; protected set; }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.Batteries.Add(this);
    this.PowerCell = ((Component) this).GetComponent<Building>().GetPowerInputCell();
    this.Subscribe<Battery>(-1582839653, Battery.OnTagsChangedDelegate);
    this.OnTagsChanged((object) null);
    MeterController meterController;
    if (!Object.op_Implicit((Object) ((Component) this).GetComponent<PowerTransformer>()))
      meterController = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[4]
      {
        "meter_target",
        "meter_fill",
        "meter_frame",
        "meter_OL"
      });
    else
      meterController = (MeterController) null;
    this.meter = meterController;
    Game.Instance.circuitManager.Connect((IEnergyConsumer) this);
    Game.Instance.energySim.AddBattery(this);
  }

  private void OnTagsChanged(object data)
  {
    if (((Component) this).HasAllTags(this.connectedTags))
    {
      Game.Instance.circuitManager.Connect((IEnergyConsumer) this);
      ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.JoulesAvailable, (object) this);
    }
    else
    {
      Game.Instance.circuitManager.Disconnect((IEnergyConsumer) this, false);
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.JoulesAvailable);
    }
  }

  protected virtual void OnCleanUp()
  {
    Game.Instance.energySim.RemoveBattery(this);
    Game.Instance.circuitManager.Disconnect((IEnergyConsumer) this, true);
    Components.Batteries.Remove(this);
    base.OnCleanUp();
  }

  public virtual void EnergySim200ms(float dt)
  {
    this.dt = dt;
    this.joulesConsumed = 0.0f;
    this.WattsUsed = 0.0f;
    this.ChargeCapacity = this.chargeWattage * dt;
    if (this.meter != null)
      this.meter.SetPositionPercent(this.PercentFull);
    this.UpdateSounds();
    this.PreviousJoulesAvailable = this.JoulesAvailable;
    this.ConsumeEnergy(this.joulesLostPerSecond * dt, true);
  }

  private void UpdateSounds()
  {
    float previousPercentFull = this.PreviousPercentFull;
    double percentFull = (double) this.PercentFull;
    if (percentFull == 0.0 && (double) previousPercentFull != 0.0)
      ((Component) this).GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryDischarged);
    if (percentFull > 0.99900001287460327 && (double) previousPercentFull <= 0.99900001287460327)
      ((Component) this).GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryFull);
    if (percentFull >= 0.25 || (double) previousPercentFull < 0.25)
      return;
    ((Component) this).GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryWarning);
  }

  public void SetConnectionStatus(CircuitManager.ConnectionStatus status)
  {
    this.connectionStatus = status;
    if (status == CircuitManager.ConnectionStatus.NotConnected)
      this.operational.SetActive(false);
    else
      this.operational.SetActive(this.operational.IsOperational && (double) this.JoulesAvailable > 0.0);
  }

  public void AddEnergy(float joules)
  {
    this.joulesAvailable = Mathf.Min(this.capacity, this.JoulesAvailable + joules);
    this.joulesConsumed += joules;
    this.ChargeCapacity -= joules;
    this.WattsUsed = this.joulesConsumed / this.dt;
  }

  public void ConsumeEnergy(float joules, bool report = false)
  {
    if (report)
      ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, -Mathf.Min(this.JoulesAvailable, joules), StringFormatter.Replace((string) BUILDINGS.PREFABS.BATTERY.CHARGE_LOSS, "{Battery}", ((Component) this).GetProperName()));
    this.joulesAvailable = Mathf.Max(0.0f, this.JoulesAvailable - joules);
  }

  public void ConsumeEnergy(float joules) => this.ConsumeEnergy(joules, false);

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    if (Object.op_Equality((Object) this.powerTransformer, (Object) null))
    {
      descriptors.Add(new Descriptor((string) UI.BUILDINGEFFECTS.REQUIRESPOWERGENERATOR, (string) UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWERGENERATOR, (Descriptor.DescriptorType) 0, false));
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.BATTERYCAPACITY, (object) GameUtil.GetFormattedJoules(this.capacity, "")), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.BATTERYCAPACITY, (object) GameUtil.GetFormattedJoules(this.capacity, "")), (Descriptor.DescriptorType) 1, false));
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.BATTERYLEAK, (object) GameUtil.GetFormattedJoules(this.joulesLostPerSecond, timeSlice: GameUtil.TimeSlice.PerCycle)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.BATTERYLEAK, (object) GameUtil.GetFormattedJoules(this.joulesLostPerSecond, timeSlice: GameUtil.TimeSlice.PerCycle)), (Descriptor.DescriptorType) 1, false));
    }
    else
    {
      descriptors.Add(new Descriptor((string) UI.BUILDINGEFFECTS.TRANSFORMER_INPUT_WIRE, (string) UI.BUILDINGEFFECTS.TOOLTIPS.TRANSFORMER_INPUT_WIRE, (Descriptor.DescriptorType) 0, false));
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.TRANSFORMER_OUTPUT_WIRE, (object) GameUtil.GetFormattedWattage(this.capacity)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.TRANSFORMER_OUTPUT_WIRE, (object) GameUtil.GetFormattedWattage(this.capacity)), (Descriptor.DescriptorType) 0, false));
    }
    return descriptors;
  }

  [ContextMenu("Refill Power")]
  public void DEBUG_RefillPower() => this.joulesAvailable = this.capacity;
}
