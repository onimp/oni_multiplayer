// Decompiled with JetBrains decompiler
// Type: EquippableBalloon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using TUNING;
using UnityEngine;

public class EquippableBalloon : StateMachineComponent<EquippableBalloon.StatesInstance>
{
  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.transitionTime = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.JOY_REACTION_DURATION;
    this.smi.StartSM();
  }

  protected override void OnCleanUp() => base.OnCleanUp();

  public class StatesInstance : 
    GameStateMachine<EquippableBalloon.States, EquippableBalloon.StatesInstance, EquippableBalloon, object>.GameInstance
  {
    [Serialize]
    public float transitionTime;

    public StatesInstance(EquippableBalloon master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<EquippableBalloon.States, EquippableBalloon.StatesInstance, EquippableBalloon>
  {
    public GameStateMachine<EquippableBalloon.States, EquippableBalloon.StatesInstance, EquippableBalloon, object>.State destroy;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.root;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.root.Transition(this.destroy, (StateMachine<EquippableBalloon.States, EquippableBalloon.StatesInstance, EquippableBalloon, object>.Transition.ConditionCallback) (smi => (double) GameClock.Instance.GetTime() >= (double) smi.transitionTime));
      this.destroy.Enter((StateMachine<EquippableBalloon.States, EquippableBalloon.StatesInstance, EquippableBalloon, object>.State.Callback) (smi => ((Component) smi.master).GetComponent<Equippable>().Unassign()));
    }
  }
}
