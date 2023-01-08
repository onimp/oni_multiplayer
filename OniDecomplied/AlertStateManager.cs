// Decompiled with JetBrains decompiler
// Type: AlertStateManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;

public class AlertStateManager : 
  GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>
{
  public GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State off;
  public AlertStateManager.OnStates on;
  public StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter isRedAlert = new StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter();
  public StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter isYellowAlert = new StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter();
  public StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter isOn = new StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.BoolParameter();

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.off.ParamTransition<bool>((StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.Parameter<bool>) this.isOn, (GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State) this.on, GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.IsTrue);
    this.on.Exit("VignetteOff", (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State.Callback) (smi => Vignette.Instance.Reset())).ParamTransition<bool>((StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.Parameter<bool>) this.isRedAlert, this.on.red, (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.Parameter<bool>.Callback) ((smi, p) => this.isRedAlert.Get(smi))).ParamTransition<bool>((StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.Parameter<bool>) this.isRedAlert, this.on.yellow, (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.Parameter<bool>.Callback) ((smi, p) => this.isYellowAlert.Get(smi) && !this.isRedAlert.Get(smi))).ParamTransition<bool>((StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.Parameter<bool>) this.isYellowAlert, this.on.yellow, (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.Parameter<bool>.Callback) ((smi, p) => this.isYellowAlert.Get(smi) && !this.isRedAlert.Get(smi))).ParamTransition<bool>((StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.Parameter<bool>) this.isOn, this.off, GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.IsFalse);
    this.on.red.Enter("EnterEvent", (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State.Callback) (smi => Game.Instance.Trigger(1585324898, (object) null))).Exit("ExitEvent", (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State.Callback) (smi => Game.Instance.Trigger(-1393151672, (object) null))).Enter("SoundsOnRedAlert", (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State.Callback) (smi => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_ON")))).Exit("SoundsOffRedAlert", (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State.Callback) (smi => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_OFF")))).ToggleNotification((Func<AlertStateManager.Instance, Notification>) (smi => smi.redAlertNotification));
    this.on.yellow.Enter("EnterEvent", (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State.Callback) (smi => Game.Instance.Trigger(-741654735, (object) null))).Exit("ExitEvent", (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State.Callback) (smi => Game.Instance.Trigger(-2062778933, (object) null))).Enter("SoundsOnYellowAlert", (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State.Callback) (smi => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_ON")))).Exit("SoundsOffRedAlert", (StateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State.Callback) (smi => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_OFF"))));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class OnStates : 
    GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State
  {
    public GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State yellow;
    public GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.State red;
  }

  public new class Instance : 
    GameStateMachine<AlertStateManager, AlertStateManager.Instance, IStateMachineTarget, AlertStateManager.Def>.GameInstance
  {
    private bool isToggled;
    private bool hasTopPriorityChore;
    public Notification redAlertNotification = new Notification((string) MISC.NOTIFICATIONS.REDALERT.NAME, NotificationType.Bad, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) MISC.NOTIFICATIONS.REDALERT.TOOLTIP), expires: false);

    public Instance(IStateMachineTarget master, AlertStateManager.Def def)
      : base(master, def)
    {
    }

    public void UpdateState(float dt)
    {
      if (this.IsRedAlert())
        this.smi.GoTo((StateMachine.BaseState) this.sm.on.red);
      else if (this.IsYellowAlert())
      {
        this.smi.GoTo((StateMachine.BaseState) this.sm.on.yellow);
      }
      else
      {
        if (this.IsOn())
          return;
        this.smi.GoTo((StateMachine.BaseState) this.sm.off);
      }
    }

    public bool IsOn() => this.sm.isYellowAlert.Get(this.smi) || this.sm.isRedAlert.Get(this.smi);

    public bool IsRedAlert() => this.sm.isRedAlert.Get(this.smi);

    public bool IsYellowAlert() => this.sm.isYellowAlert.Get(this.smi);

    public bool IsRedAlertToggledOn() => this.isToggled;

    public void ToggleRedAlert(bool on)
    {
      this.isToggled = on;
      this.Refresh();
    }

    public void SetHasTopPriorityChore(bool on)
    {
      this.hasTopPriorityChore = on;
      this.Refresh();
    }

    private void Refresh()
    {
      this.sm.isYellowAlert.Set(this.hasTopPriorityChore, this.smi);
      this.sm.isRedAlert.Set(this.isToggled, this.smi);
      this.sm.isOn.Set(this.hasTopPriorityChore || this.isToggled, this.smi);
    }
  }
}
