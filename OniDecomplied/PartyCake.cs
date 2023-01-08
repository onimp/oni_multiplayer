// Decompiled with JetBrains decompiler
// Type: PartyCake
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public class PartyCake : 
  GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>
{
  public PartyCake.CreatingStates creating;
  public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State ready_to_party;
  public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State party;
  public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State complete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.creating.ready;
    this.creating.ready.PlayAnim("base").GoTo((GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State) this.creating.tier1);
    this.creating.tier1.InitializeStates(this.masterTarget, "tier_1", (GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State) this.creating.tier2);
    this.creating.tier2.InitializeStates(this.masterTarget, "tier_2", (GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State) this.creating.tier3);
    this.creating.tier3.InitializeStates(this.masterTarget, "tier_3", this.ready_to_party);
    this.ready_to_party.PlayAnim("unlit");
    this.party.PlayAnim("lit");
    this.complete.PlayAnim("finished");
  }

  private static Chore CreateFetchChore(PartyCake.StatesInstance smi)
  {
    ChoreType farmFetch = Db.Get().ChoreTypes.FarmFetch;
    Storage component = smi.GetComponent<Storage>();
    HashSet<Tag> tags = new HashSet<Tag>();
    tags.Add(TagExtensions.ToTag("MushBar"));
    Tag invalid = Tag.Invalid;
    return (Chore) new FetchChore(farmFetch, component, 10f, tags, FetchChore.MatchCriteria.MatchID, invalid, operational_requirement: Operational.State.Functional);
  }

  private static Chore CreateWorkChore(PartyCake.StatesInstance smi)
  {
    Workable component = (Workable) smi.master.GetComponent<PartyCakeWorkable>();
    return (Chore) new WorkChore<PartyCakeWorkable>(Db.Get().ChoreTypes.Cook, (IStateMachineTarget) component, allow_in_red_alert: false, schedule_block: Db.Get().ScheduleBlockTypes.Work, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class CreatingStates : 
    GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State
  {
    public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State ready;
    public PartyCake.CreatingStates.Tier tier1;
    public PartyCake.CreatingStates.Tier tier2;
    public PartyCake.CreatingStates.Tier tier3;
    public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State finish;

    public class Tier : 
      GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State
    {
      public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State fetch;
      public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State work;

      public GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State InitializeStates(
        StateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.TargetParameter targ,
        string animPrefix,
        GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State success)
      {
        this.root.Target(targ).DefaultState(this.fetch);
        this.fetch.PlayAnim(animPrefix + "_ready").ToggleChore(new Func<PartyCake.StatesInstance, Chore>(PartyCake.CreateFetchChore), this.work);
        this.work.Enter((StateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State.Callback) (smi =>
        {
          KBatchedAnimController component = smi.GetComponent<KBatchedAnimController>();
          component.Play(HashedString.op_Implicit(animPrefix + "_working"));
          component.SetPositionPercent(0.0f);
        })).ToggleChore(new Func<PartyCake.StatesInstance, Chore>(PartyCake.CreateWorkChore), success, this.work);
        return (GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.State) this;
      }
    }
  }

  public class StatesInstance : 
    GameStateMachine<PartyCake, PartyCake.StatesInstance, IStateMachineTarget, PartyCake.Def>.GameInstance
  {
    public StatesInstance(IStateMachineTarget smi, PartyCake.Def def)
      : base(smi, def)
    {
    }
  }
}
