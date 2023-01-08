// Decompiled with JetBrains decompiler
// Type: RocketConduitReceiver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RocketConduitReceiver : 
  StateMachineComponent<RocketConduitReceiver.StatesInstance>,
  ISecondaryOutput
{
  [SerializeField]
  public ConduitPortInfo conduitPortInfo;
  public RocketConduitReceiver.ConduitPort conduitPort;
  public Storage senderConduitStorage;
  private static readonly EventSystem.IntraObjectHandler<RocketConduitReceiver> TryFindPartner = new EventSystem.IntraObjectHandler<RocketConduitReceiver>((Action<RocketConduitReceiver, object>) ((component, data) => component.FindPartner()));
  private static readonly EventSystem.IntraObjectHandler<RocketConduitReceiver> OnLandedDelegate = new EventSystem.IntraObjectHandler<RocketConduitReceiver>((Action<RocketConduitReceiver, object>) ((component, data) => component.AddConduitPortToNetwork()));
  private static readonly EventSystem.IntraObjectHandler<RocketConduitReceiver> OnLaunchedDelegate = new EventSystem.IntraObjectHandler<RocketConduitReceiver>((Action<RocketConduitReceiver, object>) ((component, data) => component.RemoveConduitPortFromNetwork()));

  public void AddConduitPortToNetwork()
  {
    if (Object.op_Equality((Object) this.conduitPort.conduitDispenser, (Object) null))
      return;
    int cell1 = Grid.OffsetCell(Grid.PosToCell(((Component) this).gameObject), this.conduitPortInfo.offset);
    IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitPortInfo.conduitType);
    this.conduitPort.outputCell = cell1;
    this.conduitPort.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitPortInfo.conduitType, Endpoint.Source, cell1, ((Component) this).gameObject);
    int cell2 = cell1;
    FlowUtilityNetwork.NetworkItem networkItem = this.conduitPort.networkItem;
    networkManager.AddToNetworks(cell2, (object) networkItem, true);
  }

  public void RemoveConduitPortFromNetwork()
  {
    if (Object.op_Equality((Object) this.conduitPort.conduitDispenser, (Object) null))
      return;
    Conduit.GetNetworkManager(this.conduitPortInfo.conduitType).RemoveFromNetworks(this.conduitPort.outputCell, (object) this.conduitPort.networkItem, true);
  }

  private bool CanTransferFromSender()
  {
    bool flag = false;
    if (((double) this.smi.master.senderConduitStorage.MassStored() > 0.0 || this.smi.master.senderConduitStorage.items.Count > 0) && this.smi.master.conduitPort.conduitDispenser.GetConduitManager().GetPermittedFlow(this.smi.master.conduitPort.outputCell) != ConduitFlow.FlowDirections.None)
      flag = true;
    return flag;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.FindPartner();
    this.Subscribe<RocketConduitReceiver>(-1118736034, RocketConduitReceiver.TryFindPartner);
    this.Subscribe<RocketConduitReceiver>(546421097, RocketConduitReceiver.OnLaunchedDelegate);
    this.Subscribe<RocketConduitReceiver>(-735346771, RocketConduitReceiver.OnLandedDelegate);
    this.smi.StartSM();
    Components.RocketConduitReceivers.Add(this);
  }

  protected override void OnCleanUp()
  {
    this.RemoveConduitPortFromNetwork();
    base.OnCleanUp();
    Components.RocketConduitReceivers.Remove(this);
  }

  private void FindPartner()
  {
    if (Object.op_Inequality((Object) this.senderConduitStorage, (Object) null))
      return;
    RocketConduitSender rocketConduitSender = (RocketConduitSender) null;
    WorldContainer world = ClusterManager.Instance.GetWorld(((Component) this).gameObject.GetMyWorldId());
    if (Object.op_Inequality((Object) world, (Object) null) && world.IsModuleInterior)
    {
      foreach (RocketConduitSender component in ((Component) ((Component) world).GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule()).GetComponents<RocketConduitSender>())
      {
        if (component.conduitPortInfo.conduitType == this.conduitPortInfo.conduitType)
        {
          rocketConduitSender = component;
          break;
        }
      }
    }
    else
    {
      ClustercraftExteriorDoor component = ((Component) this).gameObject.GetComponent<ClustercraftExteriorDoor>();
      if (component.HasTargetWorld())
      {
        WorldContainer targetWorld = component.GetTargetWorld();
        foreach (RocketConduitSender worldItem in Components.RocketConduitSenders.GetWorldItems(targetWorld.id))
        {
          if (worldItem.conduitPortInfo.conduitType == this.conduitPortInfo.conduitType)
          {
            rocketConduitSender = worldItem;
            break;
          }
        }
      }
    }
    if (Object.op_Equality((Object) rocketConduitSender, (Object) null))
      Debug.LogWarning((object) "No warp conduit sender found?");
    else
      this.SetStorage(rocketConduitSender.conduitStorage);
  }

  public void SetStorage(Storage conduitStorage)
  {
    this.senderConduitStorage = conduitStorage;
    this.conduitPort.SetPortInfo(((Component) this).gameObject, this.conduitPortInfo, conduitStorage);
    if (!Object.op_Inequality((Object) ((Component) this).gameObject.GetMyWorld(), (Object) null))
      return;
    this.AddConduitPortToNetwork();
  }

  bool ISecondaryOutput.HasSecondaryConduitType(ConduitType type) => type == this.conduitPortInfo.conduitType;

  CellOffset ISecondaryOutput.GetSecondaryConduitOffset(ConduitType type) => type == this.conduitPortInfo.conduitType ? this.conduitPortInfo.offset : CellOffset.none;

  public struct ConduitPort
  {
    public ConduitPortInfo portInfo;
    public int outputCell;
    public FlowUtilityNetwork.NetworkItem networkItem;
    public ConduitDispenser conduitDispenser;

    public void SetPortInfo(GameObject parent, ConduitPortInfo info, Storage senderStorage)
    {
      this.portInfo = info;
      ConduitDispenser conduitDispenser = parent.AddComponent<ConduitDispenser>();
      conduitDispenser.conduitType = this.portInfo.conduitType;
      conduitDispenser.useSecondaryOutput = true;
      conduitDispenser.alwaysDispense = true;
      conduitDispenser.storage = senderStorage;
      this.conduitDispenser = conduitDispenser;
    }
  }

  public class StatesInstance : 
    GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.GameInstance
  {
    public StatesInstance(RocketConduitReceiver master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver>
  {
    public GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.State off;
    public RocketConduitReceiver.States.onStates on;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.off.EventTransition(GameHashes.OperationalFlagChanged, (GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.State) this.on, (StateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().GetFlag(WarpConduitStatus.warpConnectedFlag)));
      this.on.DefaultState(this.on.empty);
      this.on.empty.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal).Update((Action<RocketConduitReceiver.StatesInstance, float>) ((smi, dt) =>
      {
        if (!smi.master.CanTransferFromSender())
          return;
        smi.GoTo((StateMachine.BaseState) this.on.hasResources);
      }));
      this.on.hasResources.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working).Update((Action<RocketConduitReceiver.StatesInstance, float>) ((smi, dt) =>
      {
        if (smi.master.CanTransferFromSender())
          return;
        smi.GoTo((StateMachine.BaseState) this.on.empty);
      }));
    }

    public class onStates : 
      GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.State
    {
      public GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.State hasResources;
      public GameStateMachine<RocketConduitReceiver.States, RocketConduitReceiver.StatesInstance, RocketConduitReceiver, object>.State empty;
    }
  }
}
