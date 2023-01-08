// Decompiled with JetBrains decompiler
// Type: RailGun
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RailGun : StateMachineComponent<RailGun.StatesInstance>, ISim200ms, ISecondaryInput
{
  [Serialize]
  public float launchMass = 200f;
  public float MinLaunchMass = 2f;
  [MyCmpGet]
  private Operational operational;
  [MyCmpGet]
  private KAnimControllerBase kac;
  [MyCmpGet]
  public HighEnergyParticleStorage hepStorage;
  public Storage resourceStorage;
  private MeterController resourceMeter;
  private HighEnergyParticleStorage particleStorage;
  private MeterController particleMeter;
  private ClusterDestinationSelector destinationSelector;
  public static readonly Operational.Flag noSurfaceSight = new Operational.Flag(nameof (noSurfaceSight), Operational.Flag.Type.Requirement);
  private static StatusItem noSurfaceSightStatusItem;
  public static readonly Operational.Flag noDestination = new Operational.Flag(nameof (noDestination), Operational.Flag.Type.Requirement);
  private static StatusItem noDestinationStatusItem;
  [SerializeField]
  public ConduitPortInfo liquidPortInfo;
  private int liquidInputCell = -1;
  private FlowUtilityNetwork.NetworkItem liquidNetworkItem;
  private ConduitConsumer liquidConsumer;
  [SerializeField]
  public ConduitPortInfo gasPortInfo;
  private int gasInputCell = -1;
  private FlowUtilityNetwork.NetworkItem gasNetworkItem;
  private ConduitConsumer gasConsumer;
  [SerializeField]
  public ConduitPortInfo solidPortInfo;
  private int solidInputCell = -1;
  private FlowUtilityNetwork.NetworkItem solidNetworkItem;
  private SolidConduitConsumer solidConsumer;
  public static readonly HashedString PORT_ID = HashedString.op_Implicit("LogicLaunching");
  private bool hasLogicWire;
  private bool isLogicActive;
  private static StatusItem infoStatusItemLogic;
  public bool FreeStartHex;
  public bool FreeDestinationHex;
  private static readonly EventSystem.IntraObjectHandler<RailGun> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<RailGun>((Action<RailGun, object>) ((component, data) => component.OnLogicValueChanged(data)));

  public float MaxLaunchMass => 200f;

  public float EnergyCost => this.smi.EnergyCost();

  public float CurrentEnergy => this.hepStorage.Particles;

  public bool AllowLaunchingFromLogic
  {
    get
    {
      if (!this.hasLogicWire)
        return true;
      return this.hasLogicWire && this.isLogicActive;
    }
  }

  public bool HasLogicWire => this.hasLogicWire;

  public bool IsLogicActive => this.isLogicActive;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.destinationSelector = ((Component) this).GetComponent<ClusterDestinationSelector>();
    this.resourceStorage = ((Component) this).GetComponent<Storage>();
    this.particleStorage = ((Component) this).GetComponent<HighEnergyParticleStorage>();
    if (RailGun.noSurfaceSightStatusItem == null)
      RailGun.noSurfaceSightStatusItem = new StatusItem("RAILGUN_PATH_NOT_CLEAR", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID);
    if (RailGun.noDestinationStatusItem == null)
      RailGun.noDestinationStatusItem = new StatusItem("RAILGUN_NO_DESTINATION", "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID);
    this.gasInputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.gasPortInfo.offset);
    this.gasConsumer = this.CreateConduitConsumer(ConduitType.Gas, this.gasInputCell, out this.gasNetworkItem);
    this.liquidInputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.liquidPortInfo.offset);
    this.liquidConsumer = this.CreateConduitConsumer(ConduitType.Liquid, this.liquidInputCell, out this.liquidNetworkItem);
    this.solidInputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.solidPortInfo.offset);
    this.solidConsumer = this.CreateSolidConduitConsumer(this.solidInputCell, out this.solidNetworkItem);
    this.CreateMeters();
    this.smi.StartSM();
    if (RailGun.infoStatusItemLogic == null)
    {
      RailGun.infoStatusItemLogic = new StatusItem("LogicOperationalInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      RailGun.infoStatusItemLogic.resolveStringCallback = new Func<string, object, string>(RailGun.ResolveInfoStatusItemString);
    }
    this.CheckLogicWireState();
    this.Subscribe<RailGun>(-801688580, RailGun.OnLogicValueChangedDelegate);
  }

  protected override void OnCleanUp()
  {
    Conduit.GetNetworkManager(this.liquidPortInfo.conduitType).RemoveFromNetworks(this.liquidInputCell, (object) this.liquidNetworkItem, true);
    Conduit.GetNetworkManager(this.gasPortInfo.conduitType).RemoveFromNetworks(this.gasInputCell, (object) this.gasNetworkItem, true);
    Game.Instance.solidConduitSystem.RemoveFromNetworks(this.solidInputCell, (object) this.solidConsumer, true);
    base.OnCleanUp();
  }

  private void CreateMeters()
  {
    this.resourceMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_storage_target", "meter_storage", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
    this.particleMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_orb_target", "meter_orb", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
  }

  bool ISecondaryInput.HasSecondaryConduitType(ConduitType type) => this.liquidPortInfo.conduitType == type || this.gasPortInfo.conduitType == type || this.solidPortInfo.conduitType == type;

  public CellOffset GetSecondaryConduitOffset(ConduitType type)
  {
    if (this.liquidPortInfo.conduitType == type)
      return this.liquidPortInfo.offset;
    if (this.gasPortInfo.conduitType == type)
      return this.gasPortInfo.offset;
    return this.solidPortInfo.conduitType == type ? this.solidPortInfo.offset : CellOffset.none;
  }

  private LogicCircuitNetwork GetNetwork() => Game.Instance.logicCircuitManager.GetNetworkForCell(((Component) this).GetComponent<LogicPorts>().GetPortCell(RailGun.PORT_ID));

  private void CheckLogicWireState()
  {
    LogicCircuitNetwork network = this.GetNetwork();
    this.hasLogicWire = network != null;
    this.isLogicActive = LogicCircuitNetwork.IsBitActive(0, network != null ? network.OutputValue : 1);
    this.smi.sm.allowedFromLogic.Set(this.AllowLaunchingFromLogic, this.smi);
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(RailGun.infoStatusItemLogic, network != null, (object) this);
  }

  private void OnLogicValueChanged(object data)
  {
    if (!HashedString.op_Equality(((LogicValueChanged) data).portID, RailGun.PORT_ID))
      return;
    this.CheckLogicWireState();
  }

  private static string ResolveInfoStatusItemString(string format_str, object data)
  {
    RailGun railGun = (RailGun) data;
    Operational operational = railGun.operational;
    return (string) (railGun.AllowLaunchingFromLogic ? BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_ENABLED : BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_DISABLED);
  }

  public void Sim200ms(float dt)
  {
    WorldContainer myWorld = this.GetMyWorld();
    Extents extents = ((Component) this).GetComponent<Building>().GetExtents();
    int x1 = extents.x;
    int x2 = extents.x + extents.width - 2;
    int y1 = extents.y + extents.height;
    int cell1 = Grid.XYToCell(x1, y1);
    int y2 = y1;
    int cell2 = Grid.XYToCell(x2, y2);
    bool flag = true;
    int y3 = (int) myWorld.maximumBounds.y;
    for (int index1 = cell1; index1 <= cell2; ++index1)
    {
      for (int index2 = index1; Grid.CellRow(index2) <= y3; index2 = Grid.CellAbove(index2))
      {
        if (!Grid.IsValidCell(index2) || Grid.Solid[index2])
          flag = false;
      }
    }
    this.operational.SetFlag(RailGun.noSurfaceSight, flag);
    this.operational.SetFlag(RailGun.noDestination, this.destinationSelector.GetDestinationWorld() >= 0);
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.ToggleStatusItem(RailGun.noSurfaceSightStatusItem, !flag);
    component.ToggleStatusItem(RailGun.noDestinationStatusItem, this.destinationSelector.GetDestinationWorld() < 0);
    this.UpdateMeters();
  }

  private void UpdateMeters()
  {
    this.resourceMeter.SetPositionPercent(Mathf.Clamp01(this.resourceStorage.MassStored() / this.resourceStorage.capacityKg));
    this.particleMeter.SetPositionPercent(Mathf.Clamp01(this.particleStorage.Particles / this.particleStorage.capacity));
  }

  private void LaunchProjectile()
  {
    Extents extents = ((Component) this).GetComponent<Building>().GetExtents();
    Vector2I xy = Grid.PosToXY(this.transform.position);
    xy.y += extents.height + 1;
    int cell = Grid.XYToCell(xy.x, xy.y);
    GameObject go = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("RailGunPayload")), Grid.CellToPosCBC(cell, Grid.SceneLayer.Front));
    float num1 = 0.0f;
    while ((double) num1 < (double) this.launchMass && (double) this.resourceStorage.MassStored() > 0.0)
      num1 += this.resourceStorage.Transfer(go.GetComponent<Storage>(), GameTags.Stored, this.launchMass - num1, hide_popups: true);
    double num2 = (double) this.particleStorage.ConsumeAndGet(this.smi.EnergyCost());
    go.SetActive(true);
    if (this.destinationSelector.GetDestinationWorld() < 0)
      return;
    RailGunPayload.StatesInstance smi = go.GetSMI<RailGunPayload.StatesInstance>();
    smi.takeoffVelocity = 35f;
    smi.StartSM();
    smi.Launch(((Component) this).gameObject.GetMyWorldLocation(), this.destinationSelector.GetDestination());
  }

  private ConduitConsumer CreateConduitConsumer(
    ConduitType inputType,
    int inputCell,
    out FlowUtilityNetwork.NetworkItem flowNetworkItem)
  {
    ConduitConsumer conduitConsumer = ((Component) this).gameObject.AddComponent<ConduitConsumer>();
    conduitConsumer.conduitType = inputType;
    conduitConsumer.useSecondaryInput = true;
    IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(inputType);
    flowNetworkItem = new FlowUtilityNetwork.NetworkItem(inputType, Endpoint.Sink, inputCell, ((Component) this).gameObject);
    int cell = inputCell;
    FlowUtilityNetwork.NetworkItem networkItem = flowNetworkItem;
    networkManager.AddToNetworks(cell, (object) networkItem, true);
    return conduitConsumer;
  }

  private SolidConduitConsumer CreateSolidConduitConsumer(
    int inputCell,
    out FlowUtilityNetwork.NetworkItem flowNetworkItem)
  {
    SolidConduitConsumer solidConduitConsumer = ((Component) this).gameObject.AddComponent<SolidConduitConsumer>();
    solidConduitConsumer.useSecondaryInput = true;
    flowNetworkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Sink, inputCell, ((Component) this).gameObject);
    Game.Instance.solidConduitSystem.AddToNetworks(inputCell, (object) flowNetworkItem, true);
    return solidConduitConsumer;
  }

  public class StatesInstance : 
    GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.GameInstance
  {
    public const int INVALID_PATH_LENGTH = -1;
    private List<AxialI> m_cachedPath;

    public StatesInstance(RailGun smi)
      : base(smi)
    {
    }

    public bool HasResources() => (double) this.smi.master.resourceStorage.MassStored() >= (double) this.smi.master.launchMass;

    public bool HasEnergy() => (double) this.smi.master.particleStorage.Particles > (double) this.EnergyCost();

    public bool HasDestination() => this.smi.master.destinationSelector.GetDestinationWorld() != this.smi.master.GetMyWorldId();

    public bool IsDestinationReachable(bool forceRefresh = false)
    {
      if (forceRefresh)
        this.UpdatePath();
      return this.smi.master.destinationSelector.GetDestinationWorld() != this.smi.master.GetMyWorldId() && this.PathLength() != -1;
    }

    public int PathLength()
    {
      if (this.smi.m_cachedPath == null)
        this.UpdatePath();
      if (this.smi.m_cachedPath == null)
        return -1;
      int count = this.smi.m_cachedPath.Count;
      if (this.master.FreeStartHex)
        --count;
      if (this.master.FreeDestinationHex)
        --count;
      return count;
    }

    public void UpdatePath() => this.m_cachedPath = ClusterGrid.Instance.GetPath(this.gameObject.GetMyWorldLocation(), this.smi.master.destinationSelector.GetDestination(), this.smi.master.destinationSelector);

    public float EnergyCost() => Mathf.Max(0.0f, (float) (0.0 + (double) this.PathLength() * 10.0));

    public bool MayTurnOn() => this.HasEnergy() && this.IsDestinationReachable() && this.master.operational.IsOperational && this.sm.allowedFromLogic.Get(this);
  }

  public class States : GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun>
  {
    public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State off;
    public RailGun.States.OnStates on;
    public StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.FloatParameter cooldownTimer;
    public StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.IntParameter payloadsFiredSinceCooldown;
    public StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.BoolParameter allowedFromLogic;
    public StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.BoolParameter updatePath;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.serializable = StateMachine.SerializeType.ParamsOnly;
      this.root.EventHandler(GameHashes.ClusterDestinationChanged, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State.Callback) (smi => smi.UpdatePath()));
      this.off.PlayAnim("off").EventTransition(GameHashes.OnParticleStorageChanged, (GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State) this.on, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => smi.MayTurnOn())).EventTransition(GameHashes.ClusterDestinationChanged, (GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State) this.on, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => smi.MayTurnOn())).EventTransition(GameHashes.OperationalChanged, (GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State) this.on, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => smi.MayTurnOn())).ParamTransition<bool>((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<bool>) this.allowedFromLogic, (GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State) this.on, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<bool>.Callback) ((smi, p) => smi.MayTurnOn()));
      this.on.DefaultState(this.on.power_on).EventTransition(GameHashes.OperationalChanged, this.on.power_off, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational)).EventTransition(GameHashes.ClusterDestinationChanged, this.on.power_off, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => !smi.IsDestinationReachable())).EventTransition(GameHashes.ClusterFogOfWarRevealed, (Func<RailGun.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.on.power_off, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => !smi.IsDestinationReachable(true))).EventTransition(GameHashes.OnParticleStorageChanged, this.on.power_off, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => !smi.MayTurnOn())).ParamTransition<bool>((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<bool>) this.allowedFromLogic, this.on.power_off, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<bool>.Callback) ((smi, p) => !p)).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal);
      this.on.power_on.PlayAnim("power_on").OnAnimQueueComplete(this.on.wait_for_storage);
      this.on.power_off.PlayAnim("power_off").OnAnimQueueComplete(this.off);
      this.on.wait_for_storage.PlayAnim("on", (KAnim.PlayMode) 0).EventTransition(GameHashes.ClusterDestinationChanged, this.on.power_off, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => !smi.HasEnergy())).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State) this.on.working, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => smi.HasResources() && (double) smi.sm.cooldownTimer.Get(smi) <= 0.0)).EventTransition(GameHashes.OperationalChanged, (GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State) this.on.working, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Transition.ConditionCallback) (smi => smi.HasResources() && (double) smi.sm.cooldownTimer.Get(smi) <= 0.0)).ParamTransition<float>((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<float>) this.cooldownTimer, (GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State) this.on.cooldown, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<float>.Callback) ((smi, p) => (double) p > 0.0));
      this.on.working.DefaultState(this.on.working.pre).Enter((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Exit((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State.Callback) (smi => smi.master.operational.SetActive(false)));
      this.on.working.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.on.working.loop);
      this.on.working.loop.PlayAnim("working_loop").OnAnimQueueComplete(this.on.working.fire);
      this.on.working.fire.Enter((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State.Callback) (smi =>
      {
        if (!smi.IsDestinationReachable())
          return;
        smi.master.LaunchProjectile();
        smi.sm.payloadsFiredSinceCooldown.Delta(1, smi);
        if (smi.sm.payloadsFiredSinceCooldown.Get(smi) < 6)
          return;
        double num = (double) smi.sm.cooldownTimer.Set(30f, smi);
      })).GoTo(this.on.working.bounce);
      this.on.working.bounce.ParamTransition<float>((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<float>) this.cooldownTimer, this.on.working.pst, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<float>.Callback) ((smi, p) => (double) p > 0.0 || !smi.HasResources())).ParamTransition<int>((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<int>) this.payloadsFiredSinceCooldown, this.on.working.loop, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<int>.Callback) ((smi, p) => p < 6 && smi.HasResources()));
      this.on.working.pst.PlayAnim("working_pst").OnAnimQueueComplete(this.on.wait_for_storage);
      this.on.cooldown.DefaultState(this.on.cooldown.pre).ToggleMainStatusItem(Db.Get().BuildingStatusItems.RailGunCooldown);
      this.on.cooldown.pre.PlayAnim("cooldown_pre").OnAnimQueueComplete(this.on.cooldown.loop);
      double num1;
      this.on.cooldown.loop.PlayAnim("cooldown_loop", (KAnim.PlayMode) 0).ParamTransition<float>((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<float>) this.cooldownTimer, this.on.cooldown.pst, (StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.Parameter<float>.Callback) ((smi, p) => (double) p <= 0.0)).Update((Action<RailGun.StatesInstance, float>) ((smi, dt) => num1 = (double) this.cooldownTimer.Delta(-dt, smi)), (UpdateRate) 6);
      this.on.cooldown.pst.PlayAnim("cooldown_pst").OnAnimQueueComplete(this.on.wait_for_storage).Exit((StateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State.Callback) (smi => smi.sm.payloadsFiredSinceCooldown.Set(0, smi)));
    }

    public class WorkingStates : 
      GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State
    {
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State pre;
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State loop;
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State fire;
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State bounce;
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State pst;
    }

    public class CooldownStates : 
      GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State
    {
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State pre;
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State loop;
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State pst;
    }

    public class OnStates : 
      GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State
    {
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State power_on;
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State wait_for_storage;
      public GameStateMachine<RailGun.States, RailGun.StatesInstance, RailGun, object>.State power_off;
      public RailGun.States.WorkingStates working;
      public RailGun.States.CooldownStates cooldown;
    }
  }
}
