// Decompiled with JetBrains decompiler
// Type: DrowningStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

public class DrowningStates : 
  GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>
{
  public GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.State drown;
  public GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.State drown_pst;
  public GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.State move_to_safe;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.drown;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.DROWNING.NAME, (string) CREATURES.STATUSITEMS.DROWNING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).TagTransition(GameTags.Creatures.Drowning, (GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.State) null, true);
    this.drown.PlayAnim("drown_pre").QueueAnim("drown_loop", true).Transition(this.drown_pst, new StateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.Transition.ConditionCallback(this.UpdateSafeCell), (UpdateRate) 6);
    this.drown_pst.PlayAnim("drown_pst").OnAnimQueueComplete(this.move_to_safe);
    this.move_to_safe.MoveTo((Func<DrowningStates.Instance, int>) (smi => smi.safeCell));
  }

  public bool UpdateSafeCell(DrowningStates.Instance smi)
  {
    Navigator component = smi.GetComponent<Navigator>();
    DrowningStates.EscapeCellQuery escapeCellQuery = new DrowningStates.EscapeCellQuery(smi.GetComponent<DrowningMonitor>());
    DrowningStates.EscapeCellQuery query = escapeCellQuery;
    component.RunQuery((PathFinderQuery) query);
    smi.safeCell = escapeCellQuery.GetResultCell();
    return smi.safeCell != Grid.InvalidCell;
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<DrowningStates, DrowningStates.Instance, IStateMachineTarget, DrowningStates.Def>.GameInstance
  {
    public int safeCell = Grid.InvalidCell;

    public Instance(Chore<DrowningStates.Instance> chore, DrowningStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.HasTag, (object) GameTags.Creatures.Drowning);
    }
  }

  public class EscapeCellQuery : PathFinderQuery
  {
    private DrowningMonitor monitor;

    public EscapeCellQuery(DrowningMonitor monitor) => this.monitor = monitor;

    public override bool IsMatch(int cell, int parent_cell, int cost) => this.monitor.IsCellSafe(cell);
  }
}
