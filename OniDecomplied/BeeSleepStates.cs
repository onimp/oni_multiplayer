// Decompiled with JetBrains decompiler
// Type: BeeSleepStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class BeeSleepStates : 
  GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>
{
  public BeeSleepStates.SleepStates sleep;
  public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State findSleepLocation;
  public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State moveToSleepLocation;
  public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.findSleepLocation;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.SLEEPING.NAME, (string) CREATURES.STATUSITEMS.SLEEPING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.findSleepLocation.Enter((StateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State.Callback) (smi =>
    {
      BeeSleepStates.FindSleepLocation(smi);
      if (smi.targetSleepCell != Grid.InvalidCell)
        smi.GoTo((StateMachine.BaseState) this.moveToSleepLocation);
      else
        smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
    }));
    this.moveToSleepLocation.MoveTo((Func<BeeSleepStates.Instance, int>) (smi => smi.targetSleepCell), this.sleep.pre, this.behaviourcomplete);
    this.sleep.Enter("EnableGravity", (StateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State.Callback) (smi => GameComps.Gravities.Add(smi.gameObject, Vector2.zero, (System.Action) (() =>
    {
      if (!((KComponentManager<GravityComponent>) GameComps.Gravities).Has((object) smi.gameObject))
        return;
      GameComps.Gravities.Remove(smi.gameObject);
    })))).TriggerOnEnter(GameHashes.SleepStarted).TriggerOnExit(GameHashes.SleepFinished).Transition(this.sleep.pst, new StateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.Transition.ConditionCallback(BeeSleepStates.ShouldWakeUp), (UpdateRate) 6);
    this.sleep.pre.QueueAnim("sleep_pre").OnAnimQueueComplete(this.sleep.loop);
    this.sleep.loop.Enter((StateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State.Callback) (smi => smi.GetComponent<LoopingSounds>().PauseSound(GlobalAssets.GetSound("Bee_wings_LP"), true))).QueueAnim("sleep_loop", true).Exit((StateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State.Callback) (smi => smi.GetComponent<LoopingSounds>().PauseSound(GlobalAssets.GetSound("Bee_wings_LP"), false)));
    this.sleep.pst.QueueAnim("sleep_pst").OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.BeeWantsToSleep);
  }

  private static void FindSleepLocation(BeeSleepStates.Instance smi)
  {
    smi.targetSleepCell = Grid.InvalidCell;
    FloorCellQuery query = PathFinderQueries.floorCellQuery.Reset(1);
    smi.GetComponent<Navigator>().RunQuery((PathFinderQuery) query);
    if (query.result_cells.Count <= 0)
      return;
    smi.targetSleepCell = query.result_cells[Random.Range(0, query.result_cells.Count)];
  }

  public static bool ShouldWakeUp(BeeSleepStates.Instance smi) => (double) smi.GetSMI<BeeSleepMonitor.Instance>().CO2Exposure <= 0.0;

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.GameInstance
  {
    public int targetSleepCell;
    public float co2Exposure;

    public Instance(Chore<BeeSleepStates.Instance> chore, BeeSleepStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.BeeWantsToSleep);
    }
  }

  public class SleepStates : 
    GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State
  {
    public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State pre;
    public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State loop;
    public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State pst;
  }
}
