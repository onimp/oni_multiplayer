// Decompiled with JetBrains decompiler
// Type: RocketConduitSender
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RocketConduitSender : 
  StateMachineComponent<RocketConduitSender.StatesInstance>,
  ISecondaryInput
{
  public Storage conduitStorage;
  [SerializeField]
  public ConduitPortInfo conduitPortInfo;
  private RocketConduitSender.ConduitPort conduitPort;
  private RocketConduitReceiver partnerReceiver;
  private static readonly EventSystem.IntraObjectHandler<RocketConduitSender> TryFindPartnerDelegate = new EventSystem.IntraObjectHandler<RocketConduitSender>((Action<RocketConduitSender, object>) ((component, data) => component.FindPartner()));
  private static readonly EventSystem.IntraObjectHandler<RocketConduitSender> OnLandedDelegate = new EventSystem.IntraObjectHandler<RocketConduitSender>((Action<RocketConduitSender, object>) ((component, data) => component.AddConduitPortToNetwork()));
  private static readonly EventSystem.IntraObjectHandler<RocketConduitSender> OnLaunchedDelegate = new EventSystem.IntraObjectHandler<RocketConduitSender>((Action<RocketConduitSender, object>) ((component, data) => component.RemoveConduitPortFromNetwork()));

  public void AddConduitPortToNetwork()
  {
    if (this.conduitPort == null)
      return;
    int cell1 = Grid.OffsetCell(Grid.PosToCell(((Component) this).gameObject), this.conduitPortInfo.offset);
    IUtilityNetworkMgr networkManager = Conduit.GetNetworkManager(this.conduitPortInfo.conduitType);
    this.conduitPort.inputCell = cell1;
    this.conduitPort.networkItem = new FlowUtilityNetwork.NetworkItem(this.conduitPortInfo.conduitType, Endpoint.Sink, cell1, ((Component) this).gameObject);
    int cell2 = cell1;
    FlowUtilityNetwork.NetworkItem networkItem = this.conduitPort.networkItem;
    networkManager.AddToNetworks(cell2, (object) networkItem, true);
  }

  public void RemoveConduitPortFromNetwork()
  {
    if (this.conduitPort == null)
      return;
    Conduit.GetNetworkManager(this.conduitPortInfo.conduitType).RemoveFromNetworks(this.conduitPort.inputCell, (object) this.conduitPort.networkItem, true);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.FindPartner();
    this.Subscribe<RocketConduitSender>(-1118736034, RocketConduitSender.TryFindPartnerDelegate);
    this.Subscribe<RocketConduitSender>(546421097, RocketConduitSender.OnLaunchedDelegate);
    this.Subscribe<RocketConduitSender>(-735346771, RocketConduitSender.OnLandedDelegate);
    this.smi.StartSM();
    Components.RocketConduitSenders.Add(this);
  }

  protected override void OnCleanUp()
  {
    this.RemoveConduitPortFromNetwork();
    base.OnCleanUp();
    Components.RocketConduitSenders.Remove(this);
  }

  private void FindPartner()
  {
    WorldContainer world = ClusterManager.Instance.GetWorld(((Component) this).gameObject.GetMyWorldId());
    if (Object.op_Inequality((Object) world, (Object) null) && world.IsModuleInterior)
    {
      foreach (RocketConduitReceiver component in ((Component) ((Component) world).GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule()).GetComponents<RocketConduitReceiver>())
      {
        if (component.conduitPortInfo.conduitType == this.conduitPortInfo.conduitType)
        {
          this.partnerReceiver = component;
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
        foreach (RocketConduitReceiver worldItem in Components.RocketConduitReceivers.GetWorldItems(targetWorld.id))
        {
          if (worldItem.conduitPortInfo.conduitType == this.conduitPortInfo.conduitType)
          {
            this.partnerReceiver = worldItem;
            break;
          }
        }
      }
    }
    if (Object.op_Equality((Object) this.partnerReceiver, (Object) null))
    {
      Debug.LogWarning((object) "No rocket conduit receiver found?");
    }
    else
    {
      this.conduitPort = new RocketConduitSender.ConduitPort(((Component) this).gameObject, this.conduitPortInfo, this.conduitStorage);
      if (Object.op_Inequality((Object) world, (Object) null))
        this.AddConduitPortToNetwork();
      this.partnerReceiver.SetStorage(this.conduitStorage);
    }
  }

  bool ISecondaryInput.HasSecondaryConduitType(ConduitType type) => this.conduitPortInfo.conduitType == type;

  CellOffset ISecondaryInput.GetSecondaryConduitOffset(ConduitType type) => this.conduitPortInfo.conduitType == type ? this.conduitPortInfo.offset : CellOffset.none;

  private class ConduitPort
  {
    public ConduitPortInfo conduitPortInfo;
    public int inputCell;
    public FlowUtilityNetwork.NetworkItem networkItem;
    private ConduitConsumer conduitConsumer;

    public ConduitPort(GameObject parent, ConduitPortInfo info, Storage targetStorage)
    {
      this.conduitPortInfo = info;
      ConduitConsumer conduitConsumer = parent.AddComponent<ConduitConsumer>();
      conduitConsumer.conduitType = this.conduitPortInfo.conduitType;
      conduitConsumer.useSecondaryInput = true;
      conduitConsumer.storage = targetStorage;
      conduitConsumer.capacityKG = targetStorage.capacityKg;
      conduitConsumer.alwaysConsume = true;
      conduitConsumer.forceAlwaysSatisfied = true;
      this.conduitConsumer = conduitConsumer;
      this.conduitConsumer.keepZeroMassObject = false;
    }
  }

  public class StatesInstance : 
    GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.GameInstance
  {
    public StatesInstance(RocketConduitSender smi)
      : base(smi)
    {
    }
  }

  public class States : 
    GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender>
  {
    public RocketConduitSender.States.onStates on;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.on;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.on.DefaultState(this.on.waiting);
      this.on.waiting.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Normal).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State) this.on.working, (StateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.Transition.ConditionCallback) (smi => (double) smi.GetComponent<Storage>().MassStored() > 0.0));
      this.on.working.ToggleMainStatusItem(Db.Get().BuildingStatusItems.Working).DefaultState(this.on.working.ground);
      this.on.working.notOnGround.Enter((StateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State.Callback) (smi => smi.gameObject.GetSMI<AutoStorageDropper.Instance>().SetInvertElementFilter(true))).UpdateTransition(this.on.working.ground, (Func<RocketConduitSender.StatesInstance, float, bool>) ((smi, f) =>
      {
        WorldContainer myWorld = smi.master.GetMyWorld();
        return Object.op_Implicit((Object) myWorld) && myWorld.IsModuleInterior && !((Component) ((Component) myWorld).GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule()).HasTag(GameTags.RocketNotOnGround);
      })).Exit((StateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.gameObject, (Object) null))
          return;
        smi.gameObject.GetSMI<AutoStorageDropper.Instance>()?.SetInvertElementFilter(false);
      }));
      this.on.working.ground.Enter((StateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State.Callback) (smi => smi.master.partnerReceiver.conduitPort.conduitDispenser.alwaysDispense = true)).UpdateTransition(this.on.working.notOnGround, (Func<RocketConduitSender.StatesInstance, float, bool>) ((smi, f) =>
      {
        WorldContainer myWorld = smi.master.GetMyWorld();
        return Object.op_Implicit((Object) myWorld) && myWorld.IsModuleInterior && ((Component) ((Component) myWorld).GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule()).HasTag(GameTags.RocketNotOnGround);
      })).Exit((StateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State.Callback) (smi => smi.master.partnerReceiver.conduitPort.conduitDispenser.alwaysDispense = false));
    }

    public class onStates : 
      GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State
    {
      public RocketConduitSender.States.workingStates working;
      public GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State waiting;
    }

    public class workingStates : 
      GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State
    {
      public GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State notOnGround;
      public GameStateMachine<RocketConduitSender.States, RocketConduitSender.StatesInstance, RocketConduitSender, object>.State ground;
    }
  }
}
