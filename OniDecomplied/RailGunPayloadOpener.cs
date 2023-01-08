// Decompiled with JetBrains decompiler
// Type: RailGunPayloadOpener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RailGunPayloadOpener : 
  StateMachineComponent<RailGunPayloadOpener.StatesInstance>,
  ISecondaryOutput
{
  public static float delivery_time = 10f;
  [SerializeField]
  public ConduitPortInfo liquidPortInfo;
  private int liquidOutputCell = -1;
  private FlowUtilityNetwork.NetworkItem liquidNetworkItem;
  private ConduitDispenser liquidDispenser;
  [SerializeField]
  public ConduitPortInfo gasPortInfo;
  private int gasOutputCell = -1;
  private FlowUtilityNetwork.NetworkItem gasNetworkItem;
  private ConduitDispenser gasDispenser;
  [SerializeField]
  public ConduitPortInfo solidPortInfo;
  private int solidOutputCell = -1;
  private FlowUtilityNetwork.NetworkItem solidNetworkItem;
  private SolidConduitDispenser solidDispenser;
  public Storage payloadStorage;
  public Storage resourceStorage;
  private ManualDeliveryKG[] deliveryComponents;
  private MeterController payloadMeter;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.gasOutputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.gasPortInfo.offset);
    this.gasDispenser = this.CreateConduitDispenser(ConduitType.Gas, this.gasOutputCell, out this.gasNetworkItem);
    this.liquidOutputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.liquidPortInfo.offset);
    this.liquidDispenser = this.CreateConduitDispenser(ConduitType.Liquid, this.liquidOutputCell, out this.liquidNetworkItem);
    this.solidOutputCell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.solidPortInfo.offset);
    this.solidDispenser = this.CreateSolidConduitDispenser(this.solidOutputCell, out this.solidNetworkItem);
    this.deliveryComponents = ((Component) this).GetComponents<ManualDeliveryKG>();
    this.payloadStorage.gunTargetOffset = new Vector2(-1f, 1.5f);
    this.payloadMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_storage_target", "meter_storage", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
    this.smi.StartSM();
  }

  protected override void OnCleanUp()
  {
    Conduit.GetNetworkManager(this.liquidPortInfo.conduitType).RemoveFromNetworks(this.liquidOutputCell, (object) this.liquidNetworkItem, true);
    Conduit.GetNetworkManager(this.gasPortInfo.conduitType).RemoveFromNetworks(this.gasOutputCell, (object) this.gasNetworkItem, true);
    Game.Instance.solidConduitSystem.RemoveFromNetworks(this.solidOutputCell, (object) this.solidDispenser, true);
    base.OnCleanUp();
  }

  private ConduitDispenser CreateConduitDispenser(
    ConduitType outputType,
    int outputCell,
    out FlowUtilityNetwork.NetworkItem flowNetworkItem)
  {
    ConduitDispenser conduitDispenser = ((Component) this).gameObject.AddComponent<ConduitDispenser>();
    conduitDispenser.conduitType = outputType;
    conduitDispenser.useSecondaryOutput = true;
    conduitDispenser.alwaysDispense = true;
    conduitDispenser.storage = this.resourceStorage;
    IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(outputType);
    flowNetworkItem = new FlowUtilityNetwork.NetworkItem(outputType, Endpoint.Source, outputCell, ((Component) this).gameObject);
    int cell = outputCell;
    FlowUtilityNetwork.NetworkItem networkItem = flowNetworkItem;
    networkManager.AddToNetworks(cell, (object) networkItem, true);
    return conduitDispenser;
  }

  private SolidConduitDispenser CreateSolidConduitDispenser(
    int outputCell,
    out FlowUtilityNetwork.NetworkItem flowNetworkItem)
  {
    SolidConduitDispenser conduitDispenser = ((Component) this).gameObject.AddComponent<SolidConduitDispenser>();
    conduitDispenser.storage = this.resourceStorage;
    conduitDispenser.alwaysDispense = true;
    conduitDispenser.useSecondaryOutput = true;
    conduitDispenser.solidOnly = true;
    flowNetworkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Source, outputCell, ((Component) this).gameObject);
    Game.Instance.solidConduitSystem.AddToNetworks(outputCell, (object) flowNetworkItem, true);
    return conduitDispenser;
  }

  public void EmptyPayload()
  {
    Storage component = ((Component) this).GetComponent<Storage>();
    if (!Object.op_Inequality((Object) component, (Object) null) || component.items.Count <= 0)
      return;
    GameObject gameObject = this.payloadStorage.items[0];
    gameObject.GetComponent<Storage>().Transfer(this.resourceStorage);
    Util.KDestroyGameObject(gameObject);
    component.ConsumeIgnoringDisease(this.payloadStorage.items[0]);
  }

  public bool PowerOperationalChanged()
  {
    EnergyConsumer component = ((Component) this).GetComponent<EnergyConsumer>();
    return Object.op_Inequality((Object) component, (Object) null) && component.IsPowered;
  }

  bool ISecondaryOutput.HasSecondaryConduitType(ConduitType type) => type == this.gasPortInfo.conduitType || type == this.liquidPortInfo.conduitType || type == this.solidPortInfo.conduitType;

  CellOffset ISecondaryOutput.GetSecondaryConduitOffset(ConduitType type)
  {
    if (type == this.gasPortInfo.conduitType)
      return this.gasPortInfo.offset;
    if (type == this.liquidPortInfo.conduitType)
      return this.liquidPortInfo.offset;
    return type != this.solidPortInfo.conduitType ? CellOffset.none : this.solidPortInfo.offset;
  }

  public class StatesInstance : 
    GameStateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.GameInstance
  {
    public StatesInstance(RailGunPayloadOpener master)
      : base(master)
    {
    }

    public bool HasPayload() => this.smi.master.payloadStorage.items.Count > 0;

    public bool HasResources() => (double) this.smi.master.resourceStorage.MassStored() > 0.0;
  }

  public class States : 
    GameStateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener>
  {
    public GameStateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State unoperational;
    public RailGunPayloadOpener.States.OperationalStates operational;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.unoperational;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.unoperational.PlayAnim("off").EventTransition(GameHashes.OperationalFlagChanged, (GameStateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State) this.operational, (StateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.Transition.ConditionCallback) (smi => smi.master.PowerOperationalChanged())).Enter((StateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State.Callback) (smi =>
      {
        smi.GetComponent<Operational>().SetActive(false, true);
        smi.GetComponent<ManualDeliveryKG>().Pause(true, "no_power");
      }));
      this.operational.Enter((StateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State.Callback) (smi => smi.GetComponent<ManualDeliveryKG>().Pause(false, "power"))).EventTransition(GameHashes.OperationalFlagChanged, this.unoperational, (StateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.Transition.ConditionCallback) (smi => !smi.master.PowerOperationalChanged())).DefaultState(this.operational.idle).EventHandler(GameHashes.OnStorageChange, (StateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State.Callback) (smi => smi.master.payloadMeter.SetPositionPercent(Mathf.Clamp01((float) smi.master.payloadStorage.items.Count / smi.master.payloadStorage.capacityKg))));
      this.operational.idle.PlayAnim("on").EventTransition(GameHashes.OnStorageChange, this.operational.pre, (StateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.Transition.ConditionCallback) (smi => smi.HasPayload()));
      this.operational.pre.Enter((StateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(true, true))).PlayAnim("working_pre").OnAnimQueueComplete(this.operational.loop);
      this.operational.loop.PlayAnim("working_loop", (KAnim.PlayMode) 0).ScheduleGoTo(10f, (StateMachine.BaseState) this.operational.pst);
      this.operational.pst.PlayAnim("working_pst").Exit((StateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State.Callback) (smi =>
      {
        smi.master.EmptyPayload();
        smi.GetComponent<Operational>().SetActive(false, true);
      })).OnAnimQueueComplete(this.operational.idle);
    }

    public class OperationalStates : 
      GameStateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State
    {
      public GameStateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State idle;
      public GameStateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State pre;
      public GameStateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State loop;
      public GameStateMachine<RailGunPayloadOpener.States, RailGunPayloadOpener.StatesInstance, RailGunPayloadOpener, object>.State pst;
    }
  }
}
