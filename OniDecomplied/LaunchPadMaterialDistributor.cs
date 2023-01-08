// Decompiled with JetBrains decompiler
// Type: LaunchPadMaterialDistributor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class LaunchPadMaterialDistributor : 
  GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>
{
  public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State inoperational;
  public LaunchPadMaterialDistributor.OperationalStates operational;
  private StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.TargetParameter attachedRocket;
  private StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.BoolParameter emptyComplete;
  private StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.BoolParameter fillComplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.inoperational;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.inoperational.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State) this.operational, (StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
    this.operational.DefaultState(this.operational.noRocket).EventTransition(GameHashes.OperationalChanged, this.inoperational, (StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).EventHandler(GameHashes.ChainedNetworkChanged, (GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.GameEvent.Callback) ((smi, data) => this.SetAttachedRocket(smi.GetLandedRocketFromPad(), smi)));
    this.operational.noRocket.Enter((StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State.Callback) (smi => this.SetAttachedRocket(smi.GetLandedRocketFromPad(), smi))).EventHandler(GameHashes.RocketLanded, (GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.GameEvent.Callback) ((smi, data) => this.SetAttachedRocket(smi.GetLandedRocketFromPad(), smi))).EventHandler(GameHashes.RocketCreated, (GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.GameEvent.Callback) ((smi, data) => this.SetAttachedRocket(smi.GetLandedRocketFromPad(), smi))).ParamTransition<GameObject>((StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<GameObject>) this.attachedRocket, this.operational.rocketLanding, (StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Inequality((Object) p, (Object) null)));
    this.operational.rocketLanding.EventTransition(GameHashes.RocketLaunched, this.operational.rocketLost).OnTargetLost(this.attachedRocket, this.operational.rocketLost).Target(this.attachedRocket).TagTransition(GameTags.RocketOnGround, (GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State) this.operational.hasRocket).Target(this.masterTarget);
    this.operational.hasRocket.DefaultState((GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State) this.operational.hasRocket.transferring).Update((System.Action<LaunchPadMaterialDistributor.Instance, float>) ((smi, dt) => smi.EmptyRocket(dt)), (UpdateRate) 6).Update((System.Action<LaunchPadMaterialDistributor.Instance, float>) ((smi, dt) => smi.FillRocket(dt)), (UpdateRate) 6).EventTransition(GameHashes.RocketLaunched, this.operational.rocketLost).OnTargetLost(this.attachedRocket, this.operational.rocketLost).Target(this.attachedRocket).EventTransition(GameHashes.DoLaunchRocket, this.operational.rocketLost).Target(this.masterTarget);
    this.operational.hasRocket.transferring.DefaultState(this.operational.hasRocket.transferring.actual).ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoEmptying).ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFilling);
    this.operational.hasRocket.transferring.actual.ParamTransition<bool>((StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<bool>) this.emptyComplete, this.operational.hasRocket.transferring.delay, (StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<bool>.Callback) ((smi, p) => this.emptyComplete.Get(smi) && this.fillComplete.Get(smi))).ParamTransition<bool>((StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<bool>) this.fillComplete, this.operational.hasRocket.transferring.delay, (StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<bool>.Callback) ((smi, p) => this.emptyComplete.Get(smi) && this.fillComplete.Get(smi)));
    this.operational.hasRocket.transferring.delay.ParamTransition<bool>((StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<bool>) this.fillComplete, this.operational.hasRocket.transferring.actual, GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.IsFalse).ParamTransition<bool>((StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<bool>) this.emptyComplete, this.operational.hasRocket.transferring.actual, GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.IsFalse).ScheduleGoTo(4f, (StateMachine.BaseState) this.operational.hasRocket.transferComplete);
    this.operational.hasRocket.transferComplete.ToggleStatusItem(Db.Get().BuildingStatusItems.RocketCargoFull).ToggleTag(GameTags.TransferringCargoComplete).ParamTransition<bool>((StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<bool>) this.fillComplete, (GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State) this.operational.hasRocket.transferring, GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.IsFalse).ParamTransition<bool>((StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.Parameter<bool>) this.emptyComplete, (GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State) this.operational.hasRocket.transferring, GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.IsFalse);
    this.operational.rocketLost.Enter((StateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State.Callback) (smi =>
    {
      this.emptyComplete.Set(false, smi);
      this.fillComplete.Set(false, smi);
      this.SetAttachedRocket((RocketModuleCluster) null, smi);
    })).GoTo(this.operational.noRocket);
  }

  private void SetAttachedRocket(
    RocketModuleCluster attached,
    LaunchPadMaterialDistributor.Instance smi)
  {
    HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
    smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
    foreach (StateMachine.Instance smi1 in (HashSet<ChainedBuilding.StatesInstance>) chain)
      smi1.GetSMI<ModularConduitPortController.Instance>()?.SetRocket(Object.op_Inequality((Object) attached, (Object) null));
    this.attachedRocket.Set((KMonoBehaviour) attached, smi);
    chain.Recycle();
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class HasRocketStates : 
    GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State
  {
    public LaunchPadMaterialDistributor.HasRocketStates.TransferringStates transferring;
    public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State transferComplete;

    public class TransferringStates : 
      GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State
    {
      public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State actual;
      public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State delay;
    }
  }

  public class OperationalStates : 
    GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State
  {
    public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State noRocket;
    public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State rocketLanding;
    public LaunchPadMaterialDistributor.HasRocketStates hasRocket;
    public GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.State rocketLost;
  }

  public new class Instance : 
    GameStateMachine<LaunchPadMaterialDistributor, LaunchPadMaterialDistributor.Instance, IStateMachineTarget, LaunchPadMaterialDistributor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, LaunchPadMaterialDistributor.Def def)
      : base(master, def)
    {
    }

    public RocketModuleCluster GetLandedRocketFromPad() => this.GetComponent<LaunchPad>().LandedRocket;

    public void EmptyRocket(float dt)
    {
      CraftModuleInterface craftInterface = this.sm.attachedRocket.Get<RocketModuleCluster>(this.smi).CraftInterface;
      DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.Allocate();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
      foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) craftInterface.ClusterModules)
      {
        CargoBayCluster component = ((Component) clusterModule.Get()).GetComponent<CargoBayCluster>();
        if (Object.op_Inequality((Object) component, (Object) null) && component.storageType != CargoBay.CargoType.Entities && (double) component.storage.MassStored() > 0.0)
          ((List<CargoBayCluster>) ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[component.storageType]).Add(component);
      }
      bool flag = false;
      HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
      this.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
      foreach (ChainedBuilding.StatesInstance smi1 in (HashSet<ChainedBuilding.StatesInstance>) chain)
      {
        ModularConduitPortController.Instance smi2 = smi1.GetSMI<ModularConduitPortController.Instance>();
        IConduitDispenser component1 = smi1.GetComponent<IConduitDispenser>();
        Operational component2 = smi1.GetComponent<Operational>();
        bool isUnloading = false;
        if (component1 != null && (smi2 == null || smi2.SelectedMode == ModularConduitPortController.Mode.Unload || smi2.SelectedMode == ModularConduitPortController.Mode.Both) && (Object.op_Equality((Object) component2, (Object) null) || component2.IsOperational))
        {
          smi2.SetRocket(true);
          TreeFilterable component3 = smi1.GetComponent<TreeFilterable>();
          float amount = component1.Storage.RemainingCapacity();
          foreach (CargoBayCluster cargoBayCluster in (List<CargoBayCluster>) ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBayConduit.ElementToCargoMap[component1.ConduitType]])
          {
            if (cargoBayCluster.storage.Count != 0)
            {
              for (int index = cargoBayCluster.storage.items.Count - 1; index >= 0; --index)
              {
                GameObject go = cargoBayCluster.storage.items[index];
                if (component3.AcceptedTags.Contains(go.PrefabID()))
                {
                  isUnloading = true;
                  flag = true;
                  if ((double) amount > 0.0)
                  {
                    Pickupable pickupable = go.GetComponent<Pickupable>().Take(amount);
                    if (Object.op_Inequality((Object) pickupable, (Object) null))
                    {
                      component1.Storage.Store(((Component) pickupable).gameObject);
                      amount -= pickupable.PrimaryElement.Mass;
                    }
                  }
                  else
                    break;
                }
              }
            }
          }
        }
        smi2?.SetUnloading(isUnloading);
      }
      chain.Recycle();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Solids].Recycle();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Liquids].Recycle();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Gasses].Recycle();
      pooledDictionary.Recycle();
      this.sm.emptyComplete.Set(!flag, this);
    }

    public void FillRocket(float dt)
    {
      CraftModuleInterface craftInterface = this.sm.attachedRocket.Get<RocketModuleCluster>(this.smi).CraftInterface;
      DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.PooledDictionary pooledDictionary = DictionaryPool<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList, LaunchPadMaterialDistributor>.Allocate();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Solids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Liquids] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Gasses] = ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.Allocate();
      foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) craftInterface.ClusterModules)
      {
        CargoBayCluster component = ((Component) clusterModule.Get()).GetComponent<CargoBayCluster>();
        if (Object.op_Inequality((Object) component, (Object) null) && component.storageType != CargoBay.CargoType.Entities && (double) component.RemainingCapacity > 0.0)
          ((List<CargoBayCluster>) ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[component.storageType]).Add(component);
      }
      bool flag = false;
      HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
      this.smi.GetSMI<ChainedBuilding.StatesInstance>().GetLinkedBuildings(ref chain);
      foreach (ChainedBuilding.StatesInstance smi1 in (HashSet<ChainedBuilding.StatesInstance>) chain)
      {
        ModularConduitPortController.Instance smi2 = smi1.GetSMI<ModularConduitPortController.Instance>();
        IConduitConsumer component = smi1.GetComponent<IConduitConsumer>();
        bool isLoading = false;
        if (component != null && (smi2 == null || smi2.SelectedMode == ModularConduitPortController.Mode.Load || smi2.SelectedMode == ModularConduitPortController.Mode.Both))
        {
          smi2.SetRocket(true);
          for (int index = component.Storage.items.Count - 1; index >= 0; --index)
          {
            GameObject go = component.Storage.items[index];
            foreach (CargoBayCluster cargoBayCluster in (List<CargoBayCluster>) ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBayConduit.ElementToCargoMap[component.ConduitType]])
            {
              float remainingCapacity = cargoBayCluster.RemainingCapacity;
              float num1 = component.Storage.MassStored();
              if ((double) remainingCapacity > 0.0 && (double) num1 > 0.0 && ((Component) cargoBayCluster).GetComponent<TreeFilterable>().AcceptedTags.Contains(go.PrefabID()))
              {
                isLoading = true;
                flag = true;
                Pickupable pickupable = go.GetComponent<Pickupable>().Take(remainingCapacity);
                if (Object.op_Inequality((Object) pickupable, (Object) null))
                {
                  cargoBayCluster.storage.Store(((Component) pickupable).gameObject);
                  float num2 = remainingCapacity - pickupable.PrimaryElement.Mass;
                }
              }
            }
          }
        }
        smi2?.SetLoading(isLoading);
      }
      chain.Recycle();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Solids].Recycle();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Liquids].Recycle();
      ((Dictionary<CargoBay.CargoType, ListPool<CargoBayCluster, LaunchPadMaterialDistributor>.PooledList>) pooledDictionary)[CargoBay.CargoType.Gasses].Recycle();
      pooledDictionary.Recycle();
      this.sm.fillComplete.Set(!flag, this.smi);
    }
  }
}
