// Decompiled with JetBrains decompiler
// Type: RedAlertMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RedAlertMonitor : GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance>
{
  public GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget, object>.State off;
  public GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget, object>.State on;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.off.EventTransition(GameHashes.EnteredRedAlert, (Func<RedAlertMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.on, (StateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.master.gameObject.GetMyWorld().AlertManager.IsRedAlert()));
    this.on.EventTransition(GameHashes.ExitedRedAlert, (Func<RedAlertMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) Game.Instance), this.off, (StateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.master.gameObject.GetMyWorld().AlertManager.IsRedAlert())).Enter("EnableRedAlert", (StateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.EnableRedAlert())).ToggleEffect("RedAlert").ToggleExpression(Db.Get().Expressions.RedAlert);
  }

  public new class Instance : 
    GameStateMachine<RedAlertMonitor, RedAlertMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public void EnableRedAlert()
    {
      ChoreDriver component = this.GetComponent<ChoreDriver>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      Chore currentChore = component.GetCurrentChore();
      if (currentChore == null)
        return;
      bool flag = false;
      for (int index = 0; index < currentChore.GetPreconditions().Count; ++index)
      {
        if (currentChore.GetPreconditions()[index].id == ChorePreconditions.instance.IsNotRedAlert.id)
          flag = true;
      }
      if (!flag)
        return;
      component.StopChore();
    }
  }
}
