// Decompiled with JetBrains decompiler
// Type: FallWhenDeadMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FallWhenDeadMonitor : 
  GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance>
{
  public GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.State standing;
  public GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.State falling;
  public GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.State entombed;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.standing;
    this.standing.Transition(this.entombed, (StateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsEntombed())).Transition(this.falling, (StateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsFalling()));
    this.falling.ToggleGravity(this.standing);
    this.entombed.Transition(this.standing, (StateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsEntombed()));
  }

  public new class Instance : 
    GameStateMachine<FallWhenDeadMonitor, FallWhenDeadMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public bool IsEntombed()
    {
      Pickupable component = this.GetComponent<Pickupable>();
      return Object.op_Inequality((Object) component, (Object) null) && component.IsEntombed;
    }

    public bool IsFalling()
    {
      int num = Grid.CellBelow(Grid.PosToCell(TransformExtensions.GetPosition(this.master.transform)));
      return Grid.IsValidCell(num) && !Grid.Solid[num];
    }
  }
}
