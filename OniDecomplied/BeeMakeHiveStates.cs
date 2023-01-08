// Decompiled with JetBrains decompiler
// Type: BeeMakeHiveStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class BeeMakeHiveStates : 
  GameStateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>
{
  public GameStateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>.State findBuildLocation;
  public GameStateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>.State moveToBuildLocation;
  public GameStateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>.State doBuild;
  public GameStateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.findBuildLocation;
    this.root.DoNothing();
    this.findBuildLocation.Enter((StateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>.State.Callback) (smi =>
    {
      this.FindBuildLocation(smi);
      if (smi.targetBuildCell != Grid.InvalidCell)
        smi.GoTo((StateMachine.BaseState) this.moveToBuildLocation);
      else
        smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
    }));
    this.moveToBuildLocation.MoveTo((Func<BeeMakeHiveStates.Instance, int>) (smi => smi.targetBuildCell), this.doBuild, this.behaviourcomplete);
    this.doBuild.PlayAnim("hive_grow_pre").EventHandler(GameHashes.AnimQueueComplete, (StateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>.State.Callback) (smi =>
    {
      if (Object.op_Equality((Object) smi.gameObject.GetComponent<Bee>().FindHiveInRoom(), (Object) null))
      {
        smi.builtHome = true;
        smi.BuildHome();
      }
      smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
    }));
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToMakeHome).Exit((StateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>.State.Callback) (smi =>
    {
      if (!smi.builtHome)
        return;
      Util.KDestroyGameObject(smi.master.gameObject);
    }));
  }

  private void FindBuildLocation(BeeMakeHiveStates.Instance smi)
  {
    smi.targetBuildCell = Grid.InvalidCell;
    GameObject prefab = Assets.GetPrefab(TagExtensions.ToTag("BeeHive"));
    BuildingPlacementQuery query = PathFinderQueries.buildingPlacementQuery.Reset(1, prefab);
    smi.GetComponent<Navigator>().RunQuery((PathFinderQuery) query);
    if (query.result_cells.Count <= 0)
      return;
    smi.targetBuildCell = query.result_cells[Random.Range(0, query.result_cells.Count)];
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<BeeMakeHiveStates, BeeMakeHiveStates.Instance, IStateMachineTarget, BeeMakeHiveStates.Def>.GameInstance
  {
    public int targetBuildCell;
    public bool builtHome;

    public Instance(Chore<BeeMakeHiveStates.Instance> chore, BeeMakeHiveStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsToMakeHome);
    }

    public void BuildHome()
    {
      Vector3 pos = Grid.CellToPos(this.targetBuildCell, (CellAlignment) 1, Grid.SceneLayer.Creatures);
      GameObject go = Util.KInstantiate(Assets.GetPrefab(TagExtensions.ToTag("BeeHive")), pos, Quaternion.identity, (GameObject) null, (string) null, true, 0);
      PrimaryElement component = go.GetComponent<PrimaryElement>();
      component.ElementID = SimHashes.Creature;
      component.Temperature = this.gameObject.GetComponent<PrimaryElement>().Temperature;
      go.SetActive(true);
      go.GetSMI<BeeHive.StatesInstance>().SetUpNewHive();
    }
  }
}
