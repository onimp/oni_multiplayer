// Decompiled with JetBrains decompiler
// Type: WarpConduitSender
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class WarpConduitSender : 
  StateMachineComponent<WarpConduitSender.StatesInstance>,
  ISecondaryInput
{
  [MyCmpReq]
  private Operational operational;
  public Storage gasStorage;
  public Storage liquidStorage;
  public Storage solidStorage;
  public WarpConduitReceiver receiver;
  [SerializeField]
  public ConduitPortInfo liquidPortInfo;
  private WarpConduitSender.ConduitPort liquidPort;
  [SerializeField]
  public ConduitPortInfo gasPortInfo;
  private WarpConduitSender.ConduitPort gasPort;
  [SerializeField]
  public ConduitPortInfo solidPortInfo;
  private WarpConduitSender.ConduitPort solidPort;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Storage[] components = ((Component) this).GetComponents<Storage>();
    this.gasStorage = components[0];
    this.liquidStorage = components[1];
    this.solidStorage = components[2];
    this.gasPort = new WarpConduitSender.ConduitPort(((Component) this).gameObject, this.gasPortInfo, 1, this.gasStorage);
    this.liquidPort = new WarpConduitSender.ConduitPort(((Component) this).gameObject, this.liquidPortInfo, 2, this.liquidStorage);
    this.solidPort = new WarpConduitSender.ConduitPort(((Component) this).gameObject, this.solidPortInfo, 3, this.solidStorage);
    Vector3 position = this.liquidPort.airlock.gameObject.transform.position;
    ((Component) this.liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>()).transform.position = Vector3.op_Addition(position, new Vector3(0.0f, 0.0f, -0.1f));
    this.liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().enabled = false;
    this.liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().enabled = true;
    this.FindPartner();
    WarpConduitStatus.UpdateWarpConduitsOperational(((Component) this).gameObject, Object.op_Inequality((Object) this.receiver, (Object) null) ? ((Component) this.receiver).gameObject : (GameObject) null);
    this.smi.StartSM();
  }

  public void OnActivatedChanged(object data) => WarpConduitStatus.UpdateWarpConduitsOperational(((Component) this).gameObject, Object.op_Inequality((Object) this.receiver, (Object) null) ? ((Component) this.receiver).gameObject : (GameObject) null);

  private void FindPartner()
  {
    ((Component) SaveGame.Instance).GetComponent<WorldGenSpawner>().SpawnTag("WarpConduitReceiver");
    foreach (WarpConduitReceiver component in Object.FindObjectsOfType<WarpConduitReceiver>())
    {
      if (component.GetMyWorldId() != this.GetMyWorldId())
      {
        this.receiver = component;
        break;
      }
    }
    if (Object.op_Equality((Object) this.receiver, (Object) null))
      Debug.LogWarning((object) "No warp conduit receiver found - maybe POI stomping or failure to spawn?");
    else
      this.receiver.SetStorage(this.gasStorage, this.liquidStorage, this.solidStorage);
  }

  protected override void OnCleanUp()
  {
    Conduit.GetNetworkManager(this.liquidPortInfo.conduitType).RemoveFromNetworks(this.liquidPort.inputCell, (object) this.liquidPort.networkItem, true);
    Conduit.GetNetworkManager(this.gasPortInfo.conduitType).RemoveFromNetworks(this.gasPort.inputCell, (object) this.gasPort.networkItem, true);
    Game.Instance.solidConduitSystem.RemoveFromNetworks(this.solidPort.inputCell, (object) this.solidPort.solidConsumer, true);
    base.OnCleanUp();
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

  private class ConduitPort
  {
    public ConduitPortInfo portInfo;
    public int inputCell;
    public FlowUtilityNetwork.NetworkItem networkItem;
    private ConduitConsumer conduitConsumer;
    public SolidConduitConsumer solidConsumer;
    public MeterController airlock;
    private bool open;
    private string pre;
    private string loop;
    private string pst;

    public ConduitPort(GameObject parent, ConduitPortInfo info, int number, Storage targetStorage)
    {
      this.portInfo = info;
      this.inputCell = Grid.OffsetCell(Grid.PosToCell(parent), this.portInfo.offset);
      if (this.portInfo.conduitType != ConduitType.Solid)
      {
        ConduitConsumer conduitConsumer = parent.AddComponent<ConduitConsumer>();
        conduitConsumer.conduitType = this.portInfo.conduitType;
        conduitConsumer.useSecondaryInput = true;
        conduitConsumer.storage = targetStorage;
        conduitConsumer.capacityKG = targetStorage.capacityKg;
        conduitConsumer.alwaysConsume = false;
        this.conduitConsumer = conduitConsumer;
        this.conduitConsumer.keepZeroMassObject = false;
        IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
        this.networkItem = new FlowUtilityNetwork.NetworkItem(this.portInfo.conduitType, Endpoint.Sink, this.inputCell, parent);
        int inputCell = this.inputCell;
        FlowUtilityNetwork.NetworkItem networkItem = this.networkItem;
        networkManager.AddToNetworks(inputCell, (object) networkItem, true);
      }
      else
      {
        this.solidConsumer = parent.AddComponent<SolidConduitConsumer>();
        this.solidConsumer.useSecondaryInput = true;
        this.solidConsumer.storage = targetStorage;
        this.networkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Sink, this.inputCell, parent);
        Game.Instance.solidConduitSystem.AddToNetworks(this.inputCell, (object) this.networkItem, true);
      }
      string meter_animation = "airlock_" + number.ToString();
      string meter_target = "airlock_target_" + number.ToString();
      this.pre = "airlock_" + number.ToString() + "_pre";
      this.loop = "airlock_" + number.ToString() + "_loop";
      this.pst = "airlock_" + number.ToString() + "_pst";
      this.airlock = new MeterController((KAnimControllerBase) parent.GetComponent<KBatchedAnimController>(), meter_target, meter_animation, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
      {
        meter_target
      });
    }

    public void Update()
    {
      bool flag = false;
      if (Object.op_Inequality((Object) this.conduitConsumer, (Object) null))
        flag = this.conduitConsumer.IsConnected && this.conduitConsumer.IsSatisfied && this.conduitConsumer.consumedLastTick;
      else if (Object.op_Inequality((Object) this.solidConsumer, (Object) null))
        flag = this.solidConsumer.IsConnected && this.solidConsumer.IsConsuming;
      if (flag == this.open)
        return;
      this.open = flag;
      if (this.open)
      {
        this.airlock.meterController.Play(HashedString.op_Implicit(this.pre));
        this.airlock.meterController.Queue(HashedString.op_Implicit(this.loop), (KAnim.PlayMode) 0);
      }
      else
        this.airlock.meterController.Play(HashedString.op_Implicit(this.pst));
    }
  }

  public class StatesInstance : 
    GameStateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.GameInstance
  {
    public StatesInstance(WarpConduitSender smi)
      : base(smi)
    {
    }
  }

  public class States : 
    GameStateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender>
  {
    public GameStateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.State off;
    public WarpConduitSender.States.onStates on;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.root.EventHandler(GameHashes.BuildingActivated, (GameStateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.GameEvent.Callback) ((smi, data) => smi.master.OnActivatedChanged(data)));
      this.off.PlayAnim("off").Enter((StateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.State.Callback) (smi =>
      {
        smi.master.gasPort.Update();
        smi.master.liquidPort.Update();
        smi.master.solidPort.Update();
      })).EventTransition(GameHashes.OperationalChanged, (GameStateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.State) this.on, (StateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
      this.on.DefaultState(this.on.waiting).Update((Action<WarpConduitSender.StatesInstance, float>) ((smi, dt) =>
      {
        smi.master.gasPort.Update();
        smi.master.liquidPort.Update();
        smi.master.solidPort.Update();
      }));
      this.on.working.PlayAnim("working_pre").QueueAnim("working_loop", true).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working).Exit((StateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.State.Callback) (smi => smi.Play("working_pst")));
      this.on.waiting.QueueAnim("idle").ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal).EventTransition(GameHashes.OnStorageChange, this.on.working, (StateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.Transition.ConditionCallback) (smi => (double) smi.GetComponent<Storage>().MassStored() > 0.0));
    }

    public class onStates : 
      GameStateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.State
    {
      public GameStateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.State working;
      public GameStateMachine<WarpConduitSender.States, WarpConduitSender.StatesInstance, WarpConduitSender, object>.State waiting;
    }
  }
}
