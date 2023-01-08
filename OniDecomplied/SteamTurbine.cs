// Decompiled with JetBrains decompiler
// Type: SteamTurbine
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using UnityEngine;

public class SteamTurbine : Generator
{
  private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
  public SimHashes srcElem;
  public SimHashes destElem;
  public float requiredMass = 1f / 1000f;
  public float minActiveTemperature = 398.15f;
  public float idealSourceElementTemperature = 473.15f;
  public float maxBuildingTemperature = 373.15f;
  public float outputElementTemperature = 368.15f;
  public float minConvertMass;
  public float pumpKGRate;
  public float maxSelfHeat;
  public float wasteHeatToTurbinePercent;
  private static readonly HashedString TINT_SYMBOL = new HashedString("meter_fill");
  [Serialize]
  private float storedMass;
  [Serialize]
  private float storedTemperature;
  [Serialize]
  private byte diseaseIdx = byte.MaxValue;
  [Serialize]
  private int diseaseCount;
  private static StatusItem inputBlockedStatusItem;
  private static StatusItem inputPartiallyBlockedStatusItem;
  private static StatusItem insufficientMassStatusItem;
  private static StatusItem insufficientTemperatureStatusItem;
  private static StatusItem activeWattageStatusItem;
  private static StatusItem buildingTooHotItem;
  private static StatusItem activeStatusItem;
  private const Sim.Cell.Properties floorCellProperties = Sim.Cell.Properties.GasImpermeable | Sim.Cell.Properties.LiquidImpermeable | Sim.Cell.Properties.SolidImpermeable | Sim.Cell.Properties.Opaque;
  private MeterController meter;
  private HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle simEmitCBHandle = HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.InvalidHandle;
  private SteamTurbine.Instance smi;
  private int[] srcCells;
  private Storage gasStorage;
  private Storage liquidStorage;
  private ElementConsumer consumer;
  private Guid statusHandle;
  private HandleVector<int>.Handle structureTemperature;
  private float lastSampleTime = -1f;

  public int BlockedInputs { get; private set; }

  public int TotalInputs => this.srcCells.Length;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.accumulator = Game.Instance.accumulators.Add("Power", (KMonoBehaviour) this);
    this.structureTemperature = GameComps.StructureTemperatures.GetHandle(((Component) this).gameObject);
    this.simEmitCBHandle = Game.Instance.massEmitCallbackManager.Add(new Action<Sim.MassEmittedCallback, object>(SteamTurbine.OnSimEmittedCallback), (object) this, "SteamTurbineEmit");
    BuildingDef def = ((Component) this).GetComponent<BuildingComplete>().Def;
    this.srcCells = new int[def.WidthInCells];
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    for (int index = 0; index < def.WidthInCells; ++index)
    {
      int num = index - (def.WidthInCells - 1) / 2;
      this.srcCells[index] = Grid.OffsetCell(cell, new CellOffset(num, -2));
    }
    this.smi = new SteamTurbine.Instance(this);
    this.smi.StartSM();
    this.CreateMeter();
  }

  private void CreateMeter() => this.meter = new MeterController((KAnimControllerBase) ((Component) this).gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[3]
  {
    "meter_OL",
    "meter_frame",
    "meter_fill"
  });

  protected override void OnCleanUp()
  {
    if (this.smi != null)
      this.smi.StopSM("cleanup");
    Game.Instance.massEmitCallbackManager.Release(this.simEmitCBHandle, nameof (SteamTurbine));
    this.simEmitCBHandle.Clear();
    base.OnCleanUp();
  }

  private void Pump(float dt)
  {
    float num = this.pumpKGRate * dt / (float) this.srcCells.Length;
    foreach (int srcCell in this.srcCells)
    {
      HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(new Action<Sim.MassConsumedCallback, object>(SteamTurbine.OnSimConsumeCallback), (object) this, "SteamTurbineConsume");
      int srcElem = (int) this.srcElem;
      double mass = (double) num;
      int index = handle.index;
      SimMessages.ConsumeMass(srcCell, (SimHashes) srcElem, (float) mass, (byte) 1, index);
    }
  }

  private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data) => ((SteamTurbine) data).OnSimConsume(mass_cb_info);

  private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
  {
    if ((double) mass_cb_info.mass <= 0.0)
      return;
    this.storedTemperature = SimUtil.CalculateFinalTemperature(this.storedMass, this.storedTemperature, mass_cb_info.mass, mass_cb_info.temperature);
    this.storedMass += mass_cb_info.mass;
    SimUtil.DiseaseInfo finalDiseaseInfo = SimUtil.CalculateFinalDiseaseInfo(this.diseaseIdx, this.diseaseCount, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
    this.diseaseIdx = finalDiseaseInfo.idx;
    this.diseaseCount = finalDiseaseInfo.count;
    if ((double) this.storedMass <= (double) this.minConvertMass || !this.simEmitCBHandle.IsValid())
      return;
    Game.Instance.massEmitCallbackManager.GetItem(this.simEmitCBHandle);
    this.gasStorage.AddGasChunk(this.srcElem, this.storedMass, this.storedTemperature, this.diseaseIdx, this.diseaseCount, true);
    this.storedMass = 0.0f;
    this.storedTemperature = 0.0f;
    this.diseaseIdx = byte.MaxValue;
    this.diseaseCount = 0;
  }

  private static void OnSimEmittedCallback(Sim.MassEmittedCallback info, object data) => ((SteamTurbine) data).OnSimEmitted(info);

  private void OnSimEmitted(Sim.MassEmittedCallback info)
  {
    if (info.suceeded == (byte) 1)
      return;
    this.storedTemperature = SimUtil.CalculateFinalTemperature(this.storedMass, this.storedTemperature, info.mass, info.temperature);
    this.storedMass += info.mass;
    if (info.diseaseIdx == byte.MaxValue)
      return;
    SimUtil.DiseaseInfo diseaseInfo = new SimUtil.DiseaseInfo();
    diseaseInfo.idx = this.diseaseIdx;
    diseaseInfo.count = this.diseaseCount;
    SimUtil.DiseaseInfo a = diseaseInfo;
    diseaseInfo = new SimUtil.DiseaseInfo();
    diseaseInfo.idx = info.diseaseIdx;
    diseaseInfo.count = info.diseaseCount;
    SimUtil.DiseaseInfo b = diseaseInfo;
    SimUtil.DiseaseInfo finalDiseaseInfo = SimUtil.CalculateFinalDiseaseInfo(a, b);
    this.diseaseIdx = finalDiseaseInfo.idx;
    this.diseaseCount = finalDiseaseInfo.count;
  }

  public static void InitializeStatusItems()
  {
    SteamTurbine.activeStatusItem = new StatusItem("TURBINE_ACTIVE", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Good, false, OverlayModes.None.ID);
    SteamTurbine.inputBlockedStatusItem = new StatusItem("TURBINE_BLOCKED_INPUT", "BUILDING", "status_item_vent_disabled", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID);
    SteamTurbine.inputPartiallyBlockedStatusItem = new StatusItem("TURBINE_PARTIALLY_BLOCKED_INPUT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, false, OverlayModes.None.ID);
    SteamTurbine.inputPartiallyBlockedStatusItem.resolveStringCallback = new Func<string, object, string>(SteamTurbine.ResolvePartialBlockedStatus);
    SteamTurbine.insufficientMassStatusItem = new StatusItem("TURBINE_INSUFFICIENT_MASS", "BUILDING", "status_item_resource_unavailable", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID);
    SteamTurbine.insufficientMassStatusItem.resolveStringCallback = new Func<string, object, string>(SteamTurbine.ResolveStrings);
    SteamTurbine.buildingTooHotItem = new StatusItem("TURBINE_TOO_HOT", "BUILDING", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID);
    SteamTurbine.buildingTooHotItem.resolveTooltipCallback = new Func<string, object, string>(SteamTurbine.ResolveStrings);
    SteamTurbine.insufficientTemperatureStatusItem = new StatusItem("TURBINE_INSUFFICIENT_TEMPERATURE", "BUILDING", "status_item_plant_temperature", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.Power.ID);
    SteamTurbine.insufficientTemperatureStatusItem.resolveStringCallback = new Func<string, object, string>(SteamTurbine.ResolveStrings);
    SteamTurbine.insufficientTemperatureStatusItem.resolveTooltipCallback = new Func<string, object, string>(SteamTurbine.ResolveStrings);
    SteamTurbine.activeWattageStatusItem = new StatusItem("TURBINE_ACTIVE_WATTAGE", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.Power.ID);
    SteamTurbine.activeWattageStatusItem.resolveStringCallback = new Func<string, object, string>(SteamTurbine.ResolveWattageStatus);
  }

  private static string ResolveWattageStatus(string str, object data)
  {
    SteamTurbine steamTurbine = (SteamTurbine) data;
    float num = Game.Instance.accumulators.GetAverageRate(steamTurbine.accumulator) / steamTurbine.WattageRating;
    return str.Replace("{Wattage}", GameUtil.GetFormattedWattage(steamTurbine.CurrentWattage)).Replace("{Max_Wattage}", GameUtil.GetFormattedWattage(steamTurbine.WattageRating)).Replace("{Efficiency}", GameUtil.GetFormattedPercent(num * 100f)).Replace("{Src_Element}", ElementLoader.FindElementByHash(steamTurbine.srcElem).name);
  }

  private static string ResolvePartialBlockedStatus(string str, object data)
  {
    SteamTurbine steamTurbine = (SteamTurbine) data;
    return str.Replace("{Blocked}", steamTurbine.BlockedInputs.ToString()).Replace("{Total}", steamTurbine.TotalInputs.ToString());
  }

  private static string ResolveStrings(string str, object data)
  {
    SteamTurbine steamTurbine = (SteamTurbine) data;
    str = str.Replace("{Src_Element}", ElementLoader.FindElementByHash(steamTurbine.srcElem).name);
    str = str.Replace("{Dest_Element}", ElementLoader.FindElementByHash(steamTurbine.destElem).name);
    str = str.Replace("{Overheat_Temperature}", GameUtil.GetFormattedTemperature(steamTurbine.maxBuildingTemperature));
    str = str.Replace("{Active_Temperature}", GameUtil.GetFormattedTemperature(steamTurbine.minActiveTemperature));
    str = str.Replace("{Min_Mass}", GameUtil.GetFormattedMass(steamTurbine.requiredMass));
    return str;
  }

  public void SetStorage(Storage steamStorage, Storage waterStorage)
  {
    this.gasStorage = steamStorage;
    this.liquidStorage = waterStorage;
  }

  public override void EnergySim200ms(float dt)
  {
    base.EnergySim200ms(dt);
    ushort circuitId = this.CircuitID;
    this.operational.SetFlag(Generator.wireConnectedFlag, circuitId != ushort.MaxValue);
    if (!this.operational.IsOperational)
      return;
    float num1 = 0.0f;
    if (Object.op_Inequality((Object) this.gasStorage, (Object) null) && this.gasStorage.items.Count > 0)
    {
      GameObject first = this.gasStorage.FindFirst(ElementLoader.FindElementByHash(this.srcElem).tag);
      if (Object.op_Inequality((Object) first, (Object) null))
      {
        PrimaryElement component = first.GetComponent<PrimaryElement>();
        float num2 = 0.1f;
        if ((double) component.Mass > (double) num2)
        {
          float mass = Mathf.Min(component.Mass, this.pumpKGRate * dt);
          num1 = Mathf.Min(this.JoulesToGenerate(component) * (mass / this.pumpKGRate), this.WattageRating * dt);
          float num3 = this.HeatFromCoolingSteam(component) * (mass / component.Mass);
          float num4 = mass / component.Mass;
          int disease_count = Mathf.RoundToInt((float) component.DiseaseCount * num4);
          component.Mass -= mass;
          component.ModifyDiseaseCount(-disease_count, "SteamTurbine.EnergySim200ms");
          float display_dt = (double) this.lastSampleTime > 0.0 ? Time.time - this.lastSampleTime : 1f;
          this.lastSampleTime = Time.time;
          GameComps.StructureTemperatures.ProduceEnergy(this.structureTemperature, num3 * this.wasteHeatToTurbinePercent, (string) BUILDINGS.PREFABS.STEAMTURBINE2.HEAT_SOURCE, display_dt);
          this.liquidStorage.AddLiquid(this.destElem, mass, this.outputElementTemperature, component.DiseaseIdx, disease_count, true);
        }
      }
    }
    float num5 = Mathf.Clamp(num1, 0.0f, this.WattageRating);
    Game.Instance.accumulators.Accumulate(this.accumulator, num5);
    if ((double) num5 > 0.0)
      this.GenerateJoules(num5);
    this.meter.SetPositionPercent(Game.Instance.accumulators.GetAverageRate(this.accumulator) / this.WattageRating);
    this.meter.SetSymbolTint(KAnimHashedString.op_Implicit(SteamTurbine.TINT_SYMBOL), Color32.op_Implicit(Color.Lerp(Color.red, Color.green, Game.Instance.accumulators.GetAverageRate(this.accumulator) / this.WattageRating)));
  }

  public float HeatFromCoolingSteam(PrimaryElement steam)
  {
    float temperature = steam.Temperature;
    return -GameUtil.CalculateEnergyDeltaForElement(steam, temperature, this.outputElementTemperature);
  }

  public float JoulesToGenerate(PrimaryElement steam) => this.WattageRating * (float) Math.Pow(((double) steam.Temperature - (double) this.outputElementTemperature) / ((double) this.idealSourceElementTemperature - (double) this.outputElementTemperature), 1.0);

  public float CurrentWattage => Game.Instance.accumulators.GetAverageRate(this.accumulator);

  public class States : GameStateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine>
  {
    public GameStateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.State inoperational;
    public SteamTurbine.States.OperationalStates operational;
    private static readonly HashedString[] ACTIVE_ANIMS = new HashedString[2]
    {
      HashedString.op_Implicit("working_pre"),
      HashedString.op_Implicit("working_loop")
    };
    private static readonly HashedString[] TOOHOT_ANIMS = new HashedString[1]
    {
      HashedString.op_Implicit("working_pre")
    };

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      SteamTurbine.InitializeStatusItems();
      default_state = (StateMachine.BaseState) this.operational;
      this.root.Update("UpdateBlocked", (Action<SteamTurbine.Instance, float>) ((smi, dt) => smi.UpdateBlocked(dt)));
      this.inoperational.EventTransition(GameHashes.OperationalChanged, this.operational.active, (StateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.Transition.ConditionCallback) (smi => ((Component) smi.master).GetComponent<Operational>().IsOperational)).QueueAnim("off");
      this.operational.DefaultState(this.operational.active).EventTransition(GameHashes.OperationalChanged, this.inoperational, (StateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.Transition.ConditionCallback) (smi => !((Component) smi.master).GetComponent<Operational>().IsOperational)).Update("UpdateOperational", (Action<SteamTurbine.Instance, float>) ((smi, dt) => smi.UpdateState(dt))).Exit((StateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.State.Callback) (smi => smi.DisableStatusItems()));
      this.operational.idle.QueueAnim("on");
      this.operational.active.Update("UpdateActive", (Action<SteamTurbine.Instance, float>) ((smi, dt) => smi.master.Pump(dt))).ToggleStatusItem((Func<SteamTurbine.Instance, StatusItem>) (smi => SteamTurbine.activeStatusItem), (Func<SteamTurbine.Instance, object>) (smi => (object) smi.master)).Enter((StateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.State.Callback) (smi =>
      {
        smi.GetComponent<KAnimControllerBase>().Play(SteamTurbine.States.ACTIVE_ANIMS, (KAnim.PlayMode) 0);
        smi.GetComponent<Operational>().SetActive(true);
      })).Exit((StateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.State.Callback) (smi =>
      {
        ((Component) smi.master).GetComponent<Generator>().ResetJoules();
        smi.GetComponent<Operational>().SetActive(false);
      }));
      this.operational.tooHot.Enter((StateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.State.Callback) (smi => smi.GetComponent<KAnimControllerBase>().Play(SteamTurbine.States.TOOHOT_ANIMS, (KAnim.PlayMode) 0)));
    }

    public class OperationalStates : 
      GameStateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.State
    {
      public GameStateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.State idle;
      public GameStateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.State active;
      public GameStateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.State tooHot;
    }
  }

  public class Instance : 
    GameStateMachine<SteamTurbine.States, SteamTurbine.Instance, SteamTurbine, object>.GameInstance
  {
    public bool insufficientMass;
    public bool insufficientTemperature;
    public bool buildingTooHot;
    private Guid inputBlockedHandle = Guid.Empty;
    private Guid inputPartiallyBlockedHandle = Guid.Empty;
    private Guid insufficientMassHandle = Guid.Empty;
    private Guid insufficientTemperatureHandle = Guid.Empty;
    private Guid buildingTooHotHandle = Guid.Empty;
    private Guid activeWattageHandle = Guid.Empty;

    public Instance(SteamTurbine master)
      : base(master)
    {
    }

    public void UpdateBlocked(float dt)
    {
      this.master.BlockedInputs = 0;
      for (int index = 0; index < this.master.TotalInputs; ++index)
      {
        int srcCell = this.master.srcCells[index];
        Element element = Grid.Element[srcCell];
        if (element.IsLiquid || element.IsSolid)
          ++this.master.BlockedInputs;
      }
      KSelectable component = this.GetComponent<KSelectable>();
      this.inputBlockedHandle = this.UpdateStatusItem(SteamTurbine.inputBlockedStatusItem, this.master.BlockedInputs == this.master.TotalInputs, this.inputBlockedHandle, component);
      this.inputPartiallyBlockedHandle = this.UpdateStatusItem(SteamTurbine.inputPartiallyBlockedStatusItem, this.master.BlockedInputs > 0 && this.master.BlockedInputs < this.master.TotalInputs, this.inputPartiallyBlockedHandle, component);
    }

    public void UpdateState(float dt)
    {
      bool flag = this.CanSteamFlow(ref this.insufficientMass, ref this.insufficientTemperature);
      int num = this.IsTooHot(ref this.buildingTooHot) ? 1 : 0;
      this.UpdateStatusItems();
      StateMachine.BaseState currentState = this.smi.GetCurrentState();
      if (num != 0)
      {
        if (currentState == this.sm.operational.tooHot)
          return;
        this.smi.GoTo((StateMachine.BaseState) this.sm.operational.tooHot);
      }
      else if (flag)
      {
        if (currentState == this.sm.operational.active)
          return;
        this.smi.GoTo((StateMachine.BaseState) this.sm.operational.active);
      }
      else
      {
        if (currentState == this.sm.operational.idle)
          return;
        this.smi.GoTo((StateMachine.BaseState) this.sm.operational.idle);
      }
    }

    private bool IsTooHot(ref bool building_too_hot)
    {
      building_too_hot = (double) this.gameObject.GetComponent<PrimaryElement>().Temperature > (double) this.smi.master.maxBuildingTemperature;
      return building_too_hot;
    }

    private bool CanSteamFlow(ref bool insufficient_mass, ref bool insufficient_temperature)
    {
      float num1 = 0.0f;
      float num2 = 0.0f;
      for (int index = 0; index < this.master.srcCells.Length; ++index)
      {
        int srcCell = this.master.srcCells[index];
        float num3 = Grid.Mass[srcCell];
        if (Grid.Element[srcCell].id == this.master.srcElem)
        {
          num1 = Mathf.Max(num1, num3);
          float num4 = Grid.Temperature[srcCell];
          num2 = Mathf.Max(num2, num4);
        }
      }
      insufficient_mass = (double) num1 < (double) this.master.requiredMass;
      insufficient_temperature = (double) num2 < (double) this.master.minActiveTemperature;
      return !insufficient_mass && !insufficient_temperature;
    }

    public void UpdateStatusItems()
    {
      KSelectable component = this.GetComponent<KSelectable>();
      this.insufficientMassHandle = this.UpdateStatusItem(SteamTurbine.insufficientMassStatusItem, this.insufficientMass, this.insufficientMassHandle, component);
      this.insufficientTemperatureHandle = this.UpdateStatusItem(SteamTurbine.insufficientTemperatureStatusItem, this.insufficientTemperature, this.insufficientTemperatureHandle, component);
      this.buildingTooHotHandle = this.UpdateStatusItem(SteamTurbine.buildingTooHotItem, this.buildingTooHot, this.buildingTooHotHandle, component);
      StatusItem status_item = this.master.operational.IsActive ? SteamTurbine.activeWattageStatusItem : Db.Get().BuildingStatusItems.GeneratorOffline;
      this.activeWattageHandle = component.SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, (object) this.master);
    }

    private Guid UpdateStatusItem(
      StatusItem item,
      bool show,
      Guid current_handle,
      KSelectable ksel)
    {
      Guid guid = current_handle;
      if (show != (current_handle != Guid.Empty))
        guid = !show ? ksel.RemoveStatusItem(current_handle) : ksel.AddStatusItem(item, (object) this.master);
      return guid;
    }

    public void DisableStatusItems()
    {
      KSelectable component = this.GetComponent<KSelectable>();
      component.RemoveStatusItem(this.buildingTooHotHandle);
      component.RemoveStatusItem(this.insufficientMassHandle);
      component.RemoveStatusItem(this.insufficientTemperatureHandle);
      component.RemoveStatusItem(this.activeWattageHandle);
    }
  }
}
