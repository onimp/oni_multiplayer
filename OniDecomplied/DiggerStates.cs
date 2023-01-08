// Decompiled with JetBrains decompiler
// Type: DiggerStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class DiggerStates : 
  GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>
{
  public GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.State move;
  public GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.State hide;
  public GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.State behaviourcomplete;

  private static bool ShouldStopHiding(DiggerStates.Instance smi) => !GameplayEventManager.Instance.IsGameplayEventRunningWithTag(GameTags.SpaceDanger);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.move;
    this.move.MoveTo((Func<DiggerStates.Instance, int>) (smi => smi.GetTunnelCell()), this.hide, this.behaviourcomplete);
    this.hide.Transition(this.behaviourcomplete, new StateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.Transition.ConditionCallback(DiggerStates.ShouldStopHiding), (UpdateRate) 7);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Tunnel);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<DiggerStates, DiggerStates.Instance, IStateMachineTarget, DiggerStates.Def>.GameInstance
  {
    public Instance(Chore<DiggerStates.Instance> chore, DiggerStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Tunnel);
    }

    public int GetTunnelCell()
    {
      DiggerMonitor.Instance smi = this.smi.GetSMI<DiggerMonitor.Instance>();
      return smi != null ? smi.lastDigCell : -1;
    }
  }
}
