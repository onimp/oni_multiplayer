// Decompiled with JetBrains decompiler
// Type: WarpConduitReceiver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class WarpConduitReceiver : 
  StateMachineComponent<WarpConduitReceiver.StatesInstance>,
  ISecondaryOutput
{
  [SerializeField]
  public ConduitPortInfo liquidPortInfo;
  private WarpConduitReceiver.ConduitPort liquidPort;
  [SerializeField]
  public ConduitPortInfo solidPortInfo;
  private WarpConduitReceiver.ConduitPort solidPort;
  [SerializeField]
  public ConduitPortInfo gasPortInfo;
  private WarpConduitReceiver.ConduitPort gasPort;
  public Storage senderGasStorage;
  public Storage senderLiquidStorage;
  public Storage senderSolidStorage;

  private bool CanTransferFromSender()
  {
    bool flag = false;
    if (((double) this.smi.master.senderGasStorage.MassStored() > 0.0 || this.smi.master.senderGasStorage.items.Count > 0) && this.smi.master.gasPort.dispenser.GetConduitManager().GetPermittedFlow(this.smi.master.gasPort.outputCell) != ConduitFlow.FlowDirections.None)
      flag = true;
    if (((double) this.smi.master.senderLiquidStorage.MassStored() > 0.0 || this.smi.master.senderLiquidStorage.items.Count > 0) && this.smi.master.liquidPort.dispenser.GetConduitManager().GetPermittedFlow(this.smi.master.liquidPort.outputCell) != ConduitFlow.FlowDirections.None)
      flag = true;
    if (((double) this.smi.master.senderSolidStorage.MassStored() > 0.0 || this.smi.master.senderSolidStorage.items.Count > 0) && Object.op_Inequality((Object) this.smi.master.solidPort.dispenser, (Object) null) && this.smi.master.solidPort.dispenser.GetConduitManager().GetPermittedFlow(this.smi.master.solidPort.outputCell) != ConduitFlow.FlowDirections.None)
      flag = true;
    return flag;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.FindPartner();
    this.smi.StartSM();
  }

  private void FindPartner()
  {
    if (Object.op_Inequality((Object) this.senderGasStorage, (Object) null))
      return;
    WarpConduitSender warpConduitSender = (WarpConduitSender) null;
    ((Component) SaveGame.Instance).GetComponent<WorldGenSpawner>().SpawnTag("WarpConduitSender");
    foreach (WarpConduitSender component in Object.FindObjectsOfType<WarpConduitSender>())
    {
      if (component.GetMyWorldId() != this.GetMyWorldId())
      {
        warpConduitSender = component;
        break;
      }
    }
    if (Object.op_Equality((Object) warpConduitSender, (Object) null))
    {
      Debug.LogWarning((object) "No warp conduit sender found - maybe POI stomping or failure to spawn?");
    }
    else
    {
      this.SetStorage(warpConduitSender.gasStorage, warpConduitSender.liquidStorage, warpConduitSender.solidStorage);
      WarpConduitStatus.UpdateWarpConduitsOperational(((Component) warpConduitSender).gameObject, ((Component) this).gameObject);
    }
  }

  protected override void OnCleanUp()
  {
    Conduit.GetNetworkManager(this.liquidPortInfo.conduitType).RemoveFromNetworks(this.liquidPort.outputCell, (object) this.liquidPort.networkItem, true);
    if (this.gasPort.portInfo != null)
      Conduit.GetNetworkManager(this.gasPort.portInfo.conduitType).RemoveFromNetworks(this.gasPort.outputCell, (object) this.gasPort.networkItem, true);
    else
      Debug.LogWarning((object) "Conduit Receiver gasPort portInfo is null in OnCleanUp");
    Game.Instance.solidConduitSystem.RemoveFromNetworks(this.solidPort.outputCell, (object) this.solidPort.networkItem, true);
    base.OnCleanUp();
  }

  public void OnActivatedChanged(object data)
  {
    if (Object.op_Equality((Object) this.senderGasStorage, (Object) null))
      this.FindPartner();
    WarpConduitStatus.UpdateWarpConduitsOperational(Object.op_Inequality((Object) this.senderGasStorage, (Object) null) ? ((Component) this.senderGasStorage).gameObject : (GameObject) null, ((Component) this).gameObject);
  }

  public void SetStorage(Storage gasStorage, Storage liquidStorage, Storage solidStorage)
  {
    this.senderGasStorage = gasStorage;
    this.senderLiquidStorage = liquidStorage;
    this.senderSolidStorage = solidStorage;
    this.gasPort.SetPortInfo(((Component) this).gameObject, this.gasPortInfo, gasStorage, 1);
    this.liquidPort.SetPortInfo(((Component) this).gameObject, this.liquidPortInfo, liquidStorage, 2);
    this.solidPort.SetPortInfo(((Component) this).gameObject, this.solidPortInfo, solidStorage, 3);
    Vector3 position = this.liquidPort.airlock.gameObject.transform.position;
    ((Component) this.liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>()).transform.position = Vector3.op_Addition(position, new Vector3(0.0f, 0.0f, -0.1f));
    this.liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().enabled = false;
    this.liquidPort.airlock.gameObject.GetComponent<KBatchedAnimController>().enabled = true;
  }

  public bool HasSecondaryConduitType(ConduitType type) => type == this.gasPortInfo.conduitType || type == this.liquidPortInfo.conduitType || type == this.solidPortInfo.conduitType;

  public CellOffset GetSecondaryConduitOffset(ConduitType type)
  {
    if (type == this.gasPortInfo.conduitType)
      return this.gasPortInfo.offset;
    if (type == this.liquidPortInfo.conduitType)
      return this.liquidPortInfo.offset;
    return type == this.solidPortInfo.conduitType ? this.solidPortInfo.offset : CellOffset.none;
  }

  public struct ConduitPort
  {
    public ConduitPortInfo portInfo;
    public int outputCell;
    public FlowUtilityNetwork.NetworkItem networkItem;
    public ConduitDispenser dispenser;
    public MeterController airlock;
    private bool open;
    private string pre;
    private string loop;
    private string pst;

    public void SetPortInfo(
      GameObject parent,
      ConduitPortInfo info,
      Storage senderStorage,
      int number)
    {
      this.portInfo = info;
      this.outputCell = Grid.OffsetCell(Grid.PosToCell(parent), this.portInfo.offset);
      if (this.portInfo.conduitType != ConduitType.Solid)
      {
        ConduitDispenser conduitDispenser = parent.AddComponent<ConduitDispenser>();
        conduitDispenser.conduitType = this.portInfo.conduitType;
        conduitDispenser.useSecondaryOutput = true;
        conduitDispenser.alwaysDispense = true;
        conduitDispenser.storage = senderStorage;
        this.dispenser = conduitDispenser;
        IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.portInfo.conduitType);
        this.networkItem = new FlowUtilityNetwork.NetworkItem(this.portInfo.conduitType, Endpoint.Source, this.outputCell, parent);
        int outputCell = this.outputCell;
        FlowUtilityNetwork.NetworkItem networkItem = this.networkItem;
        networkManager.AddToNetworks(outputCell, (object) networkItem, true);
      }
      else
      {
        SolidConduitDispenser conduitDispenser = parent.AddComponent<SolidConduitDispenser>();
        conduitDispenser.storage = senderStorage;
        conduitDispenser.alwaysDispense = true;
        conduitDispenser.useSecondaryOutput = true;
        conduitDispenser.solidOnly = true;
        this.networkItem = new FlowUtilityNetwork.NetworkItem(ConduitType.Solid, Endpoint.Source, this.outputCell, parent);
        Game.Instance.solidConduitSystem.AddToNetworks(this.outputCell, (object) this.networkItem, true);
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

    public void UpdatePortAnim()
    {
      bool flag = this.portInfo == null || this.portInfo.conduitType != ConduitType.Solid ? Object.op_Inequality((Object) this.dispenser, (Object) null) && !this.dispenser.blocked && !this.dispenser.empty && this.dispenser.GetConduitManager().GetPermittedFlow(this.outputCell) != 0 : this.networkItem.GameObject.GetComponent<SolidConduitDispenser>().IsDispensing;
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
    GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.GameInstance
  {
    public StatesInstance(WarpConduitReceiver master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver>
  {
    public GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State off;
    public WarpConduitReceiver.States.onStates on;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.root.EventHandler(GameHashes.BuildingActivated, (GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.GameEvent.Callback) ((smi, data) => smi.master.OnActivatedChanged(data)));
      this.off.PlayAnim("off").Enter((StateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State.Callback) (smi =>
      {
        smi.master.gasPort.UpdatePortAnim();
        smi.master.liquidPort.UpdatePortAnim();
        smi.master.solidPort.UpdatePortAnim();
      })).EventTransition(GameHashes.OperationalFlagChanged, (GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State) this.on, (StateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().GetFlag(WarpConduitStatus.warpConnectedFlag)));
      this.on.DefaultState(this.on.empty).Update((Action<WarpConduitReceiver.StatesInstance, float>) ((smi, dt) =>
      {
        smi.master.gasPort.UpdatePortAnim();
        smi.master.liquidPort.UpdatePortAnim();
        smi.master.solidPort.UpdatePortAnim();
      }));
      this.on.empty.QueueAnim("idle").ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal).Update((Action<WarpConduitReceiver.StatesInstance, float>) ((smi, dt) =>
      {
        if (!smi.master.CanTransferFromSender())
          return;
        smi.GoTo((StateMachine.BaseState) this.on.hasResources);
      }));
      this.on.hasResources.PlayAnim("working_pre").QueueAnim("working_loop", true).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working).Update((Action<WarpConduitReceiver.StatesInstance, float>) ((smi, dt) =>
      {
        if (smi.master.CanTransferFromSender())
          return;
        smi.GoTo((StateMachine.BaseState) this.on.empty);
      })).Exit((StateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State.Callback) (smi => smi.Play("working_pst")));
    }

    public class onStates : 
      GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State
    {
      public GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State hasResources;
      public GameStateMachine<WarpConduitReceiver.States, WarpConduitReceiver.StatesInstance, WarpConduitReceiver, object>.State empty;
    }
  }
}
