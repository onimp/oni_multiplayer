// Decompiled with JetBrains decompiler
// Type: MoveToLocationMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

public class MoveToLocationMonitor : 
  GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance>
{
  public GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance, IStateMachineTarget, object>.State moving;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.DoNothing();
    this.moving.ToggleChore((Func<MoveToLocationMonitor.Instance, Chore>) (smi => (Chore) new MoveChore(smi.master, Db.Get().ChoreTypes.MoveTo, (Func<MoveChore.StatesInstance, int>) (smii => smi.targetCell))), this.satisfied);
  }

  public new class Instance : 
    GameStateMachine<MoveToLocationMonitor, MoveToLocationMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public int targetCell;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      master.Subscribe(493375141, new System.Action<object>(this.OnRefreshUserMenu));
    }

    private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_control", (string) UI.USERMENUACTIONS.MOVETOLOCATION.NAME, new System.Action(this.OnClickMoveToLocation), tooltipText: ((string) UI.USERMENUACTIONS.MOVETOLOCATION.TOOLTIP)), 0.2f);

    private void OnClickMoveToLocation() => MoveToLocationTool.Instance.Activate(this.GetComponent<Navigator>());

    public void MoveToLocation(int cell)
    {
      this.targetCell = cell;
      this.smi.GoTo((StateMachine.BaseState) this.smi.sm.satisfied);
      this.smi.GoTo((StateMachine.BaseState) this.smi.sm.moving);
    }

    public override void StopSM(string reason)
    {
      this.master.Unsubscribe(493375141, new System.Action<object>(this.OnRefreshUserMenu));
      base.StopSM(reason);
    }
  }
}
