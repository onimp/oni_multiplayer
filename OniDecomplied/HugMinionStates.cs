// Decompiled with JetBrains decompiler
// Type: HugMinionStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class HugMinionStates : 
  GameStateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>
{
  public GameStateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>.ApproachSubState<EggIncubator> moving;
  public GameStateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>.State waiting;
  public GameStateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>.State behaviourcomplete;
  public StateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>.FloatParameter timeout;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.moving;
    this.moving.MoveTo(new Func<HugMinionStates.Instance, int>(HugMinionStates.FindFlopLocation), this.waiting, this.behaviourcomplete);
    double num;
    this.waiting.Enter((StateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>.State.Callback) (smi => smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor))).ParamTransition<float>((StateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>.Parameter<float>) this.timeout, this.behaviourcomplete, (StateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>.Parameter<float>.Callback) ((smi, p) => (double) p > 60.0 && !smi.GetSMI<HugMonitor.Instance>().IsHugging())).Update((System.Action<HugMinionStates.Instance, float>) ((smi, dt) => num = (double) smi.sm.timeout.Delta(dt, smi))).PlayAnim("waiting_pre").QueueAnim("waiting_loop", true).ToggleStatusItem((string) CREATURES.STATUSITEMS.HUGMINIONWAITING.NAME, (string) CREATURES.STATUSITEMS.HUGMINIONWAITING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsAHug);
  }

  private static int FindFlopLocation(HugMinionStates.Instance smi)
  {
    Navigator component = smi.GetComponent<Navigator>();
    FloorCellQuery floorCellQuery = PathFinderQueries.floorCellQuery.Reset(1, 1);
    FloorCellQuery query = floorCellQuery;
    component.RunQuery((PathFinderQuery) query);
    smi.targetFlopCell = floorCellQuery.result_cells.Count <= 0 ? Grid.InvalidCell : floorCellQuery.result_cells[Random.Range(0, floorCellQuery.result_cells.Count)];
    return smi.targetFlopCell;
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<HugMinionStates, HugMinionStates.Instance, IStateMachineTarget, HugMinionStates.Def>.GameInstance
  {
    public int targetFlopCell;

    public Instance(Chore<HugMinionStates.Instance> chore, HugMinionStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsAHug);
    }
  }
}
