// Decompiled with JetBrains decompiler
// Type: VignetteManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class VignetteManager : GameStateMachine<VignetteManager, VignetteManager.Instance>
{
  public GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State off;
  public VignetteManager.OnStates on;
  public StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter isRedAlert = new StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter();
  public StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter isYellowAlert = new StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter();
  public StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter isOn = new StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.BoolParameter();

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.off.ParamTransition<bool>((StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.Parameter<bool>) this.isOn, (GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State) this.on, GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.IsTrue);
    this.on.Exit("VignetteOff", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => Vignette.Instance.Reset())).ParamTransition<bool>((StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.Parameter<bool>) this.isRedAlert, this.on.red, (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.Parameter<bool>.Callback) ((smi, p) => this.isRedAlert.Get(smi))).ParamTransition<bool>((StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.Parameter<bool>) this.isRedAlert, this.on.yellow, (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.Parameter<bool>.Callback) ((smi, p) => this.isYellowAlert.Get(smi) && !this.isRedAlert.Get(smi))).ParamTransition<bool>((StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.Parameter<bool>) this.isYellowAlert, this.on.yellow, (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.Parameter<bool>.Callback) ((smi, p) => this.isYellowAlert.Get(smi) && !this.isRedAlert.Get(smi))).ParamTransition<bool>((StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.Parameter<bool>) this.isOn, this.off, GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.IsFalse);
    this.on.red.Enter("EnterEvent", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => Game.Instance.Trigger(1585324898, (object) null))).Exit("ExitEvent", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => Game.Instance.Trigger(-1393151672, (object) null))).Enter("EnableVignette", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => Vignette.Instance.SetColor(new Color(1f, 0.0f, 0.0f, 0.3f)))).Enter("SoundsOnRedAlert", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_ON")))).Exit("SoundsOffRedAlert", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("RedAlert_OFF")))).ToggleLoopingSound(GlobalAssets.GetSound("RedAlert_LP"), enable_culling: false).ToggleNotification((Func<VignetteManager.Instance, Notification>) (smi => smi.redAlertNotification));
    this.on.yellow.Enter("EnterEvent", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => Game.Instance.Trigger(-741654735, (object) null))).Exit("ExitEvent", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => Game.Instance.Trigger(-2062778933, (object) null))).Enter("EnableVignette", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => Vignette.Instance.SetColor(new Color(1f, 1f, 0.0f, 0.3f)))).Enter("SoundsOnYellowAlert", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_ON")))).Exit("SoundsOffRedAlert", (StateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State.Callback) (smi => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("YellowAlert_OFF")))).ToggleLoopingSound(GlobalAssets.GetSound("YellowAlert_LP"), enable_culling: false);
  }

  public class OnStates : 
    GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State yellow;
    public GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.State red;
  }

  public new class Instance : 
    GameStateMachine<VignetteManager, VignetteManager.Instance, IStateMachineTarget, object>.GameInstance
  {
    private static VignetteManager.Instance instance;
    private bool isToggled;
    private bool hasTopPriorityChore;
    public Notification redAlertNotification = new Notification((string) MISC.NOTIFICATIONS.REDALERT.NAME, NotificationType.Bad, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) MISC.NOTIFICATIONS.REDALERT.TOOLTIP), expires: false);

    public static void DestroyInstance() => VignetteManager.Instance.instance = (VignetteManager.Instance) null;

    public static VignetteManager.Instance Get() => VignetteManager.Instance.instance;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      VignetteManager.Instance.instance = this;
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

    public void HasTopPriorityChore(bool on)
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
