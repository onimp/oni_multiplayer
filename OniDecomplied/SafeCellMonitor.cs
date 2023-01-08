// Decompiled with JetBrains decompiler
// Type: SafeCellMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class SafeCellMonitor : GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance>
{
  public GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, object>.State danger;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.serializable = StateMachine.SerializeType.Never;
    this.root.ToggleUrge(Db.Get().Urges.MoveToSafety);
    this.satisfied.EventTransition(GameHashes.SafeCellDetected, this.danger, (StateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsAreaUnsafe()));
    this.danger.EventTransition(GameHashes.SafeCellLost, this.satisfied, (StateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsAreaUnsafe())).ToggleChore((Func<SafeCellMonitor.Instance, Chore>) (smi => (Chore) new MoveToSafetyChore(smi.master)), this.satisfied);
  }

  public new class Instance : 
    GameStateMachine<SafeCellMonitor, SafeCellMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    private SafeCellSensor safeCellSensor;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.safeCellSensor = this.GetComponent<Sensors>().GetSensor<SafeCellSensor>();
    }

    public bool IsAreaUnsafe() => this.safeCellSensor.HasSafeCell();
  }
}
