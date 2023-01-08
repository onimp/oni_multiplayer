// Decompiled with JetBrains decompiler
// Type: DebugGoToMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class DebugGoToMonitor : 
  GameStateMachine<DebugGoToMonitor, DebugGoToMonitor.Instance, IStateMachineTarget, DebugGoToMonitor.Def>
{
  public GameStateMachine<DebugGoToMonitor, DebugGoToMonitor.Instance, IStateMachineTarget, DebugGoToMonitor.Def>.State satisfied;
  public GameStateMachine<DebugGoToMonitor, DebugGoToMonitor.Instance, IStateMachineTarget, DebugGoToMonitor.Def>.State hastarget;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.DoNothing();
    this.hastarget.ToggleChore((Func<DebugGoToMonitor.Instance, Chore>) (smi => (Chore) new MoveChore(smi.master, Db.Get().ChoreTypes.DebugGoTo, (Func<MoveChore.StatesInstance, int>) (smii => DebugHandler.GetMouseCell()))), this.satisfied);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<DebugGoToMonitor, DebugGoToMonitor.Instance, IStateMachineTarget, DebugGoToMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget target, DebugGoToMonitor.Def def)
      : base(target, def)
    {
    }

    public void GoToCursor()
    {
      this.smi.GoTo((StateMachine.BaseState) this.smi.sm.satisfied);
      this.smi.GoTo((StateMachine.BaseState) this.smi.sm.hastarget);
    }
  }
}
