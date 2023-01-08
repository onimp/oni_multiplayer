// Decompiled with JetBrains decompiler
// Type: SweepBotTrappedStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SweepBotTrappedStates : 
  GameStateMachine<SweepBotTrappedStates, SweepBotTrappedStates.Instance, IStateMachineTarget, SweepBotTrappedStates.Def>
{
  public SweepBotTrappedStates.BlockedStates blockedStates;
  public GameStateMachine<SweepBotTrappedStates, SweepBotTrappedStates.Instance, IStateMachineTarget, SweepBotTrappedStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.blockedStates.evaluating;
    this.blockedStates.ToggleStatusItem(Db.Get().RobotStatusItems.CantReachStation, (Func<SweepBotTrappedStates.Instance, object>) (smi => (object) smi.gameObject), Db.Get().StatusItemCategories.Main).TagTransition(GameTags.Robots.Behaviours.TrappedBehaviour, this.behaviourcomplete, true);
    this.blockedStates.evaluating.Enter((StateMachine<SweepBotTrappedStates, SweepBotTrappedStates.Instance, IStateMachineTarget, SweepBotTrappedStates.Def>.State.Callback) (smi =>
    {
      if (Object.op_Equality((Object) smi.sm.GetSweepLocker(smi), (Object) null))
        smi.GoTo((StateMachine.BaseState) this.blockedStates.noHome);
      else
        smi.GoTo((StateMachine.BaseState) this.blockedStates.blocked);
    }));
    this.blockedStates.blocked.ToggleChore((Func<SweepBotTrappedStates.Instance, Chore>) (smi => (Chore) new RescueSweepBotChore(smi.master, smi.master.gameObject, ((Component) smi.sm.GetSweepLocker(smi)).gameObject)), this.behaviourcomplete, this.blockedStates.evaluating).PlayAnim("react_stuck", (KAnim.PlayMode) 0);
    this.blockedStates.noHome.PlayAnim("react_stuck", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.blockedStates.evaluating);
    this.behaviourcomplete.BehaviourComplete(GameTags.Robots.Behaviours.TrappedBehaviour);
  }

  public Storage GetSweepLocker(SweepBotTrappedStates.Instance smi)
  {
    StorageUnloadMonitor.Instance smi1 = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
    return smi1?.sm.sweepLocker.Get(smi1);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<SweepBotTrappedStates, SweepBotTrappedStates.Instance, IStateMachineTarget, SweepBotTrappedStates.Def>.GameInstance
  {
    public Instance(Chore<SweepBotTrappedStates.Instance> chore, SweepBotTrappedStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Robots.Behaviours.TrappedBehaviour);
    }
  }

  public class BlockedStates : 
    GameStateMachine<SweepBotTrappedStates, SweepBotTrappedStates.Instance, IStateMachineTarget, SweepBotTrappedStates.Def>.State
  {
    public GameStateMachine<SweepBotTrappedStates, SweepBotTrappedStates.Instance, IStateMachineTarget, SweepBotTrappedStates.Def>.State evaluating;
    public GameStateMachine<SweepBotTrappedStates, SweepBotTrappedStates.Instance, IStateMachineTarget, SweepBotTrappedStates.Def>.State blocked;
    public GameStateMachine<SweepBotTrappedStates, SweepBotTrappedStates.Instance, IStateMachineTarget, SweepBotTrappedStates.Def>.State noHome;
  }
}
