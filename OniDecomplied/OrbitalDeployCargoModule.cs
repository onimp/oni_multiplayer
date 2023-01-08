// Decompiled with JetBrains decompiler
// Type: OrbitalDeployCargoModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

public class OrbitalDeployCargoModule : 
  GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>
{
  public StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.BoolParameter hasCargo;
  public StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.Signal emptyCargo;
  public OrbitalDeployCargoModule.GroundedStates grounded;
  public OrbitalDeployCargoModule.NotGroundedStates not_grounded;
  public StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IntParameter numVisualCapsules;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.grounded;
    this.root.Enter((StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State.Callback) (smi => smi.CheckIfLoaded())).EventHandler(GameHashes.OnStorageChange, (StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State.Callback) (smi => smi.CheckIfLoaded())).EventHandler(GameHashes.ClusterDestinationReached, (StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State.Callback) (smi =>
    {
      if (!smi.AutoDeploy || !smi.IsValidDropLocation())
        return;
      smi.DeployCargoPods();
    }));
    this.grounded.DefaultState(this.grounded.loaded).TagTransition(GameTags.RocketNotOnGround, (GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State) this.not_grounded);
    this.grounded.loading.PlayAnim((Func<OrbitalDeployCargoModule.StatesInstance, string>) (smi => smi.GetLoadingAnimName())).ParamTransition<bool>((StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.Parameter<bool>) this.hasCargo, this.grounded.empty, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsFalse).OnAnimQueueComplete(this.grounded.loaded);
    this.grounded.loaded.ParamTransition<bool>((StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.Parameter<bool>) this.hasCargo, this.grounded.empty, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsFalse).EventTransition(GameHashes.OnStorageChange, this.grounded.loading, (StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.Transition.ConditionCallback) (smi => smi.NeedsVisualUpdate()));
    this.grounded.empty.Enter((StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State.Callback) (smi => this.numVisualCapsules.Set(0, smi))).PlayAnim("deployed").ParamTransition<bool>((StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.Parameter<bool>) this.hasCargo, this.grounded.loaded, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsTrue);
    this.not_grounded.DefaultState(this.not_grounded.loaded).TagTransition(GameTags.RocketNotOnGround, (GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State) this.grounded, true);
    this.not_grounded.loaded.PlayAnim("loaded").ParamTransition<bool>((StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.Parameter<bool>) this.hasCargo, this.not_grounded.empty, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsFalse).OnSignal(this.emptyCargo, this.not_grounded.emptying);
    this.not_grounded.emptying.PlayAnim("deploying").GoTo(this.not_grounded.empty);
    this.not_grounded.empty.PlayAnim("deployed").ParamTransition<bool>((StateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.Parameter<bool>) this.hasCargo, this.not_grounded.loaded, GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.IsTrue);
  }

  public class Def : StateMachine.BaseDef
  {
    public float numCapsules;
  }

  public class GroundedStates : 
    GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State
  {
    public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State loading;
    public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State loaded;
    public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State empty;
  }

  public class NotGroundedStates : 
    GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State
  {
    public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State loaded;
    public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State emptying;
    public GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.State empty;
  }

  public class StatesInstance : 
    GameStateMachine<OrbitalDeployCargoModule, OrbitalDeployCargoModule.StatesInstance, IStateMachineTarget, OrbitalDeployCargoModule.Def>.GameInstance,
    IEmptyableCargo
  {
    private Storage storage;
    [Serialize]
    private bool autoDeploy;

    public StatesInstance(IStateMachineTarget master, OrbitalDeployCargoModule.Def def)
      : base(master, def)
    {
      this.storage = this.GetComponent<Storage>();
      this.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition) new LoadingCompleteCondition(this.storage));
    }

    public bool NeedsVisualUpdate()
    {
      if (this.sm.numVisualCapsules.Get(this) >= Mathf.FloorToInt(this.storage.MassStored() / 200f))
        return false;
      this.sm.numVisualCapsules.Delta(1, this);
      return true;
    }

    public string GetLoadingAnimName()
    {
      int num1 = this.sm.numVisualCapsules.Get(this);
      int num2 = Mathf.RoundToInt(this.storage.capacityKg / 200f);
      if (num1 == num2)
        return "loading6_full";
      if (num1 == num2 - 1)
        return "loading5";
      if (num1 == num2 - 2)
        return "loading4";
      if (num1 == num2 - 3 || num1 > 2)
        return "loading3_repeat";
      if (num1 == 2)
        return "loading2";
      return num1 == 1 ? "loading1" : "deployed";
    }

    public void DeployCargoPods()
    {
      Clustercraft component1 = ((Component) this.master.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>();
      ClusterGridEntity orbitAsteroid = component1.GetOrbitAsteroid();
      if (Object.op_Inequality((Object) orbitAsteroid, (Object) null))
      {
        WorldContainer component2 = ((Component) orbitAsteroid).GetComponent<WorldContainer>();
        int id = component2.id;
        Vector3 vector3;
        // ISSUE: explicit constructor call
        ((Vector3) ref vector3).\u002Ector(component2.minimumBounds.x + 1f, component2.maximumBounds.y, Grid.GetLayerZ(Grid.SceneLayer.Front));
        while ((double) this.storage.MassStored() > 0.0)
        {
          GameObject go = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("RailGunPayload")), vector3);
          go.GetComponent<Pickupable>().deleteOffGrid = false;
          float num = 0.0f;
          while ((double) num < 200.0 && (double) this.storage.MassStored() > 0.0)
            num += this.storage.Transfer(go.GetComponent<Storage>(), GameTags.Stored, 200f - num, hide_popups: true);
          go.SetActive(true);
          go.GetSMI<RailGunPayload.StatesInstance>().Travel(component1.Location, component2.GetMyWorldLocation());
        }
      }
      this.CheckIfLoaded();
    }

    public bool CheckIfLoaded()
    {
      bool flag = (double) this.storage.MassStored() > 0.0;
      if (flag != this.sm.hasCargo.Get(this))
        this.sm.hasCargo.Set(flag, this);
      return flag;
    }

    public bool IsValidDropLocation() => Object.op_Inequality((Object) ((Component) this.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>().GetOrbitAsteroid(), (Object) null);

    public bool AutoDeploy
    {
      get => this.autoDeploy;
      set => this.autoDeploy = value;
    }

    public bool CanAutoDeploy => true;

    public void EmptyCargo() => this.DeployCargoPods();

    public bool CanEmptyCargo() => this.sm.hasCargo.Get(this.smi) && this.IsValidDropLocation();

    public bool ChooseDuplicant => false;

    public MinionIdentity ChosenDuplicant
    {
      get => (MinionIdentity) null;
      set
      {
      }
    }

    public bool ModuleDeployed => false;
  }
}
