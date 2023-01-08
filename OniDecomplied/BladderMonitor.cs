// Decompiled with JetBrains decompiler
// Type: BladderMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

public class BladderMonitor : GameStateMachine<BladderMonitor, BladderMonitor.Instance>
{
  public GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public BladderMonitor.WantsToPeeStates urgentwant;
  public BladderMonitor.WantsToPeeStates breakwant;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.Transition((GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State) this.urgentwant, (StateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.NeedsToPee())).Transition((GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State) this.breakwant, (StateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.WantsToPee()));
    this.urgentwant.InitializeStates(this.satisfied).ToggleThought(Db.Get().Thoughts.FullBladder).ToggleExpression(Db.Get().Expressions.FullBladder).ToggleStateMachine((Func<BladderMonitor.Instance, StateMachine.Instance>) (smi => (StateMachine.Instance) new PeeChoreMonitor.Instance(smi.master))).ToggleEffect("FullBladder");
    this.breakwant.InitializeStates(this.satisfied);
    this.breakwant.wanting.Transition((GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State) this.urgentwant, (StateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.NeedsToPee())).EventTransition(GameHashes.ScheduleBlocksChanged, this.satisfied, (StateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.WantsToPee()));
    this.breakwant.peeing.ToggleThought(Db.Get().Thoughts.BreakBladder);
  }

  public class WantsToPeeStates : 
    GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State wanting;
    public GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State peeing;

    public GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State InitializeStates(
      GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State donePeeingState)
    {
      this.DefaultState(this.wanting).ToggleUrge(Db.Get().Urges.Pee).ToggleStateMachine((Func<BladderMonitor.Instance, StateMachine.Instance>) (smi => (StateMachine.Instance) new ToiletMonitor.Instance(smi.master)));
      this.wanting.EventTransition(GameHashes.BeginChore, this.peeing, (StateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsPeeing()));
      this.peeing.EventTransition(GameHashes.EndChore, donePeeingState, (StateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsPeeing()));
      return (GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.State) this;
    }
  }

  public new class Instance : 
    GameStateMachine<BladderMonitor, BladderMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    private AmountInstance bladder;
    private ChoreDriver choreDriver;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.bladder = Db.Get().Amounts.Bladder.Lookup(master.gameObject);
      this.choreDriver = this.GetComponent<ChoreDriver>();
    }

    public bool NeedsToPee()
    {
      DebugUtil.DevAssert(this.master != null, "master ref null", (Object) null);
      DebugUtil.DevAssert(!this.master.isNull, "master isNull", (Object) null);
      KPrefabID component = this.master.GetComponent<KPrefabID>();
      DebugUtil.DevAssert(Object.op_Implicit((Object) component), "kpid was null", (Object) null);
      return !component.HasTag(GameTags.Asleep) && (double) this.bladder.value >= 100.0;
    }

    public bool WantsToPee()
    {
      if (this.NeedsToPee())
        return true;
      return this.IsPeeTime() && (double) this.bladder.value >= 40.0;
    }

    public bool IsPeeing() => this.choreDriver.HasChore() && this.choreDriver.GetCurrentChore().SatisfiesUrge(Db.Get().Urges.Pee);

    public bool IsPeeTime() => this.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Hygiene);
  }
}
