// Decompiled with JetBrains decompiler
// Type: JettisonableCargoModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class JettisonableCargoModule : 
  GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>
{
  public StateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.BoolParameter hasCargo;
  public StateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.Signal emptyCargo;
  public JettisonableCargoModule.GroundedStates grounded;
  public JettisonableCargoModule.NotGroundedStates not_grounded;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.grounded;
    this.root.Enter((StateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State.Callback) (smi => smi.CheckIfLoaded())).EventHandler(GameHashes.OnStorageChange, (StateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State.Callback) (smi => smi.CheckIfLoaded()));
    this.grounded.DefaultState(this.grounded.loaded).TagTransition(GameTags.RocketNotOnGround, (GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State) this.not_grounded);
    this.grounded.loaded.PlayAnim("loaded").ParamTransition<bool>((StateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.Parameter<bool>) this.hasCargo, this.grounded.empty, GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.IsFalse);
    this.grounded.empty.PlayAnim("deployed").ParamTransition<bool>((StateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.Parameter<bool>) this.hasCargo, this.grounded.loaded, GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.IsTrue);
    this.not_grounded.DefaultState(this.not_grounded.loaded).TagTransition(GameTags.RocketNotOnGround, (GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State) this.grounded, true);
    this.not_grounded.loaded.PlayAnim("loaded").ParamTransition<bool>((StateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.Parameter<bool>) this.hasCargo, this.not_grounded.empty, GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.IsFalse).OnSignal(this.emptyCargo, this.not_grounded.emptying);
    this.not_grounded.emptying.PlayAnim("deploying").Update((System.Action<JettisonableCargoModule.StatesInstance, float>) ((smi, dt) =>
    {
      if (!smi.CheckReadyForFinalDeploy())
        return;
      smi.FinalDeploy();
      smi.GoTo((StateMachine.BaseState) smi.sm.not_grounded.empty);
    })).EventTransition(GameHashes.ClusterLocationChanged, (Func<JettisonableCargoModule.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), (GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State) this.not_grounded).Exit((StateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State.Callback) (smi => smi.CancelPendingDeploy()));
    this.not_grounded.empty.PlayAnim("deployed").ParamTransition<bool>((StateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.Parameter<bool>) this.hasCargo, this.not_grounded.loaded, GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.IsTrue);
  }

  public class Def : StateMachine.BaseDef
  {
    public DefComponent<Storage> landerContainer;
    public Tag landerPrefabID;
    public Vector3 cargoDropOffset;
    public string clusterMapFXPrefabID;
  }

  public class GroundedStates : 
    GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State
  {
    public GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State loaded;
    public GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State empty;
  }

  public class NotGroundedStates : 
    GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State
  {
    public GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State loaded;
    public GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State emptying;
    public GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.State empty;
  }

  public class StatesInstance : 
    GameStateMachine<JettisonableCargoModule, JettisonableCargoModule.StatesInstance, IStateMachineTarget, JettisonableCargoModule.Def>.GameInstance,
    IEmptyableCargo
  {
    private Storage landerContainer;
    private bool landerPlaced;
    private MinionIdentity chosenDuplicant;
    private int landerPlacementCell;
    public GameObject clusterMapFX;

    public StatesInstance(IStateMachineTarget master, JettisonableCargoModule.Def def)
      : base(master, def)
    {
      this.landerContainer = def.landerContainer.Get((StateMachine.Instance) this);
    }

    private void ChooseLanderLocation()
    {
      ClusterGridEntity stableOrbitAsteroid = ((Component) this.master.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>().GetStableOrbitAsteroid();
      if (!Object.op_Inequality((Object) stableOrbitAsteroid, (Object) null))
        return;
      WorldContainer component1 = ((Component) stableOrbitAsteroid).GetComponent<WorldContainer>();
      Placeable component2 = this.landerContainer.FindFirst(this.def.landerPrefabID).GetComponent<Placeable>();
      component2.restrictWorldId = component1.id;
      component1.LookAtSurface();
      ClusterManager.Instance.SetActiveWorld(component1.id);
      ManagementMenu.Instance.CloseAll();
      PlaceTool.Instance.Activate(component2, new System.Action<Placeable, int>(this.OnLanderPlaced));
    }

    private void OnLanderPlaced(Placeable lander, int cell)
    {
      this.landerPlaced = true;
      this.landerPlacementCell = cell;
      if (Object.op_Inequality((Object) ((Component) lander).GetComponent<MinionStorage>(), (Object) null))
        this.OpenMoveChoreForChosenDuplicant();
      ManagementMenu.Instance.ToggleClusterMap();
      this.sm.emptyCargo.Trigger(this.smi);
      ClusterMapScreen.Instance.SelectEntity(((Component) this.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<ClusterGridEntity>(), true);
    }

    private void OpenMoveChoreForChosenDuplicant()
    {
      Clustercraft craft = ((Component) this.master.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>();
      MinionStorage storage = this.landerContainer.FindFirst(this.def.landerPrefabID).GetComponent<MinionStorage>();
      this.EnableTeleport(true);
      ((Component) this.ChosenDuplicant).GetSMI<RocketPassengerMonitor.Instance>().SetModuleDeployChore(this.landerPlacementCell, (System.Action<Chore>) (obj =>
      {
        Game.Instance.assignmentManager.RemoveFromWorld((IAssignableIdentity) this.ChosenDuplicant.assignableProxy.Get(), craft.ModuleInterface.GetInteriorWorld().id);
        craft.ModuleInterface.GetPassengerModule().RemoveRocketPassenger(this.ChosenDuplicant);
        storage.SerializeMinion(((Component) this.ChosenDuplicant).gameObject);
        this.EnableTeleport(false);
      }));
    }

    private void EnableTeleport(bool enable)
    {
      ClustercraftExteriorDoor component1 = ((Component) ((Component) this.master.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>().ModuleInterface.GetPassengerModule()).GetComponent<ClustercraftExteriorDoor>();
      ClustercraftInteriorDoor interiorDoor = component1.GetInteriorDoor();
      AccessControl component2 = ((Component) component1.GetInteriorDoor()).GetComponent<AccessControl>();
      NavTeleporter component3 = this.GetComponent<NavTeleporter>();
      if (enable)
      {
        component3.SetOverrideCell(this.landerPlacementCell);
        ((Component) interiorDoor).GetComponent<NavTeleporter>().SetTarget(component3);
        component3.SetTarget(((Component) interiorDoor).GetComponent<NavTeleporter>());
        foreach (MinionIdentity worldItem in Components.MinionIdentities.GetWorldItems(interiorDoor.GetMyWorldId()))
          component2.SetPermission(worldItem.assignableProxy.Get(), Object.op_Equality((Object) worldItem, (Object) this.ChosenDuplicant) ? AccessControl.Permission.Both : AccessControl.Permission.Neither);
      }
      else
      {
        component3.SetOverrideCell(-1);
        ((Component) interiorDoor).GetComponent<NavTeleporter>().SetTarget((NavTeleporter) null);
        component3.SetTarget((NavTeleporter) null);
        component2.SetPermission(this.ChosenDuplicant.assignableProxy.Get(), AccessControl.Permission.Neither);
      }
    }

    public void FinalDeploy()
    {
      this.landerPlaced = false;
      Placeable component1 = this.landerContainer.FindFirst(this.def.landerPrefabID).GetComponent<Placeable>();
      this.landerContainer.FindFirst(this.def.landerPrefabID);
      this.landerContainer.Drop(((Component) component1).gameObject, true);
      TreeFilterable component2 = this.GetComponent<TreeFilterable>();
      TreeFilterable component3 = ((Component) component1).GetComponent<TreeFilterable>();
      if (Object.op_Inequality((Object) component3, (Object) null))
        component3.UpdateFilters(component2.AcceptedTags);
      Storage component4 = ((Component) component1).GetComponent<Storage>();
      if (Object.op_Inequality((Object) component4, (Object) null))
      {
        foreach (Storage component5 in this.gameObject.GetComponents<Storage>())
          component5.Transfer(component4, hide_popups: true);
      }
      Vector3 posCbc = Grid.CellToPosCBC(this.landerPlacementCell, Grid.SceneLayer.Building);
      TransformExtensions.SetPosition(component1.transform, posCbc);
      ((Component) component1).gameObject.SetActive(true);
      EventExtensions.Trigger(((Component) ((Component) this.master.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>()).gameObject, 1792516731, (object) component1);
      component1.Trigger(1792516731, (object) this.gameObject);
      GameObject prefab = Assets.TryGetPrefab(Tag.op_Implicit(this.smi.def.clusterMapFXPrefabID));
      if (!Object.op_Inequality((Object) prefab, (Object) null))
        return;
      this.clusterMapFX = GameUtil.KInstantiate(prefab, Grid.SceneLayer.Background);
      this.clusterMapFX.SetActive(true);
      this.clusterMapFX.GetComponent<ClusterFXEntity>().Init(component1.GetMyWorldLocation(), Vector3.zero);
      component1.Subscribe(1969584890, (System.Action<object>) (data =>
      {
        if (Util.IsNullOrDestroyed((object) this.clusterMapFX))
          return;
        Util.KDestroyGameObject(this.clusterMapFX);
      }));
      component1.Subscribe(1591811118, (System.Action<object>) (data =>
      {
        if (Util.IsNullOrDestroyed((object) this.clusterMapFX))
          return;
        Util.KDestroyGameObject(this.clusterMapFX);
      }));
    }

    public bool CheckReadyForFinalDeploy()
    {
      MinionStorage component = this.landerContainer.FindFirst(this.def.landerPrefabID).GetComponent<MinionStorage>();
      return !Object.op_Inequality((Object) component, (Object) null) || component.GetStoredMinionInfo().Count > 0;
    }

    public void CancelPendingDeploy()
    {
      this.landerPlaced = false;
      if (!Object.op_Inequality((Object) this.ChosenDuplicant, (Object) null) || !this.CheckIfLoaded())
        return;
      ((Component) this.ChosenDuplicant).GetSMI<RocketPassengerMonitor.Instance>().CancelModuleDeployChore();
    }

    public bool CheckIfLoaded()
    {
      bool flag = false;
      foreach (GameObject go in this.landerContainer.items)
      {
        if (Tag.op_Equality(go.PrefabID(), this.def.landerPrefabID))
        {
          flag = true;
          break;
        }
      }
      if (flag != this.sm.hasCargo.Get(this))
        this.sm.hasCargo.Set(flag, this);
      return flag;
    }

    public bool IsValidDropLocation() => Object.op_Inequality((Object) ((Component) this.GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>().GetStableOrbitAsteroid(), (Object) null);

    public bool AutoDeploy { get; set; }

    public bool CanAutoDeploy => false;

    public void EmptyCargo() => this.ChooseLanderLocation();

    public bool CanEmptyCargo() => this.sm.hasCargo.Get(this.smi) && this.IsValidDropLocation() && (!this.ChooseDuplicant || Object.op_Inequality((Object) this.ChosenDuplicant, (Object) null)) && !this.landerPlaced;

    public bool ChooseDuplicant
    {
      get
      {
        GameObject first = this.landerContainer.FindFirst(this.def.landerPrefabID);
        return !Object.op_Equality((Object) first, (Object) null) && Object.op_Inequality((Object) first.GetComponent<MinionStorage>(), (Object) null);
      }
    }

    public bool ModuleDeployed => this.landerPlaced;

    public MinionIdentity ChosenDuplicant
    {
      get => this.chosenDuplicant;
      set => this.chosenDuplicant = value;
    }
  }
}
