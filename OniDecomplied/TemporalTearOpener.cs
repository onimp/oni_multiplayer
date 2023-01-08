// Decompiled with JetBrains decompiler
// Type: TemporalTearOpener
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TemporalTearOpener : 
  GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>
{
  private const float MIN_SUNLIGHT_EXPOSURE = 15f;
  private static StatusItem s_noLosStatus = new StatusItem("Temporal_Tear_Opener_No_Los", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
  private static StatusItem s_insufficient_colonies = TemporalTearOpener.CreateColoniesStatusItem();
  private static StatusItem s_noTargetStatus = new StatusItem("Temporal_Tear_Opener_No_Target", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
  private static StatusItem s_progressStatus = TemporalTearOpener.CreateProgressStatusItem();
  private TemporalTearOpener.CheckRequirementsState check_requirements;
  private TemporalTearOpener.ChargingState charging;
  private GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State opening_tear_beam_pre;
  private GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State opening_tear_beam;
  private GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State opening_tear_finish;
  private GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State ready;

  private static StatusItem CreateColoniesStatusItem() => new StatusItem("Temporal_Tear_Opener_Insufficient_Colonies", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false)
  {
    resolveStringCallback = (Func<string, object, string>) ((str, data) =>
    {
      TemporalTearOpener.Instance instance = (TemporalTearOpener.Instance) data;
      str = str.Replace("{progress}", string.Format("({0}/{1})", (object) instance.CountColonies(), (object) EstablishColonies.BASE_COUNT));
      return str;
    })
  };

  private static StatusItem CreateProgressStatusItem() => new StatusItem("Temporal_Tear_Opener_Progress", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false)
  {
    resolveStringCallback = (Func<string, object, string>) ((str, data) =>
    {
      TemporalTearOpener.Instance instance = (TemporalTearOpener.Instance) data;
      str = str.Replace("{progress}", GameUtil.GetFormattedPercent(instance.GetPercentComplete()));
      return str;
    })
  };

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.Enter((StateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State.Callback) (smi =>
    {
      smi.UpdateMeter();
      if (ClusterManager.Instance.GetClusterPOIManager().IsTemporalTearOpen())
        smi.GoTo((StateMachine.BaseState) this.opening_tear_finish);
      else
        smi.GoTo((StateMachine.BaseState) this.check_requirements);
    })).PlayAnim("off");
    this.check_requirements.DefaultState(this.check_requirements.has_target).Enter((StateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State.Callback) (smi =>
    {
      smi.GetComponent<HighEnergyParticleStorage>().receiverOpen = false;
      smi.GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("port_close"));
      smi.GetComponent<KBatchedAnimController>().Queue(HashedString.op_Implicit("off"), (KAnim.PlayMode) 0);
    }));
    this.check_requirements.has_target.ToggleStatusItem(TemporalTearOpener.s_noTargetStatus).UpdateTransition(this.check_requirements.has_los, (Func<TemporalTearOpener.Instance, float, bool>) ((smi, dt) => ClusterManager.Instance.GetClusterPOIManager().IsTemporalTearRevealed()));
    this.check_requirements.has_los.ToggleStatusItem(TemporalTearOpener.s_noLosStatus).UpdateTransition(this.check_requirements.enough_colonies, (Func<TemporalTearOpener.Instance, float, bool>) ((smi, dt) => smi.HasLineOfSight()));
    this.check_requirements.enough_colonies.ToggleStatusItem(TemporalTearOpener.s_insufficient_colonies).UpdateTransition((GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State) this.charging, (Func<TemporalTearOpener.Instance, float, bool>) ((smi, dt) => smi.HasSufficientColonies()));
    this.charging.DefaultState(this.charging.idle).ToggleStatusItem(TemporalTearOpener.s_progressStatus, (Func<TemporalTearOpener.Instance, object>) (smi => (object) smi)).UpdateTransition(this.check_requirements.has_los, (Func<TemporalTearOpener.Instance, float, bool>) ((smi, dt) => !smi.HasLineOfSight())).UpdateTransition(this.check_requirements.enough_colonies, (Func<TemporalTearOpener.Instance, float, bool>) ((smi, dt) => !smi.HasSufficientColonies())).Enter((StateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State.Callback) (smi =>
    {
      smi.GetComponent<HighEnergyParticleStorage>().receiverOpen = true;
      smi.GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("port_open"));
      smi.GetComponent<KBatchedAnimController>().Queue(HashedString.op_Implicit("inert"), (KAnim.PlayMode) 0);
    }));
    this.charging.idle.EventTransition(GameHashes.OnParticleStorageChanged, this.charging.consuming, (StateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<HighEnergyParticleStorage>().IsEmpty()));
    this.charging.consuming.EventTransition(GameHashes.OnParticleStorageChanged, this.charging.idle, (StateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<HighEnergyParticleStorage>().IsEmpty())).UpdateTransition(this.ready, (Func<TemporalTearOpener.Instance, float, bool>) ((smi, dt) => smi.ConsumeParticlesAndCheckComplete(dt)));
    this.ready.ToggleNotification((Func<TemporalTearOpener.Instance, Notification>) (smi => new Notification((string) BUILDING.STATUSITEMS.TEMPORAL_TEAR_OPENER_READY.NOTIFICATION, NotificationType.Good, (Func<List<Notification>, object, string>) ((a, b) => (string) BUILDING.STATUSITEMS.TEMPORAL_TEAR_OPENER_READY.NOTIFICATION_TOOLTIP), expires: false)));
    this.opening_tear_beam_pre.PlayAnim("working_pre", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.opening_tear_beam);
    this.opening_tear_beam.Enter((StateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State.Callback) (smi => smi.CreateBeamFX())).PlayAnim("working_loop", (KAnim.PlayMode) 0).ScheduleGoTo(5f, (StateMachine.BaseState) this.opening_tear_finish);
    this.opening_tear_finish.PlayAnim("working_pst").Enter((StateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State.Callback) (smi => smi.OpenTemporalTear()));
  }

  public class Def : StateMachine.BaseDef
  {
    public float consumeRate;
    public float numParticlesToOpen;
  }

  private class ChargingState : 
    GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State
  {
    public GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State idle;
    public GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State consuming;
  }

  private class CheckRequirementsState : 
    GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State
  {
    public GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State has_target;
    public GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State has_los;
    public GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.State enough_colonies;
  }

  public new class Instance : 
    GameStateMachine<TemporalTearOpener, TemporalTearOpener.Instance, IStateMachineTarget, TemporalTearOpener.Def>.GameInstance,
    ISidescreenButtonControl
  {
    [Serialize]
    private float m_particlesConsumed;
    private MeterController m_meter;

    public Instance(IStateMachineTarget master, TemporalTearOpener.Def def)
      : base(master, def)
    {
      this.m_meter = new MeterController((KAnimControllerBase) this.gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
      EnterTemporalTearSequence.tearOpenerGameObject = this.gameObject;
    }

    protected override void OnCleanUp()
    {
      if (Object.op_Equality((Object) EnterTemporalTearSequence.tearOpenerGameObject, (Object) this.gameObject))
        EnterTemporalTearSequence.tearOpenerGameObject = (GameObject) null;
      base.OnCleanUp();
    }

    public bool HasLineOfSight()
    {
      Extents extents = this.GetComponent<Building>().GetExtents();
      int x1 = extents.x;
      int num = extents.x + extents.width - 1;
      for (int x2 = x1; x2 <= num; ++x2)
      {
        int cell = Grid.XYToCell(x2, extents.y);
        if ((double) Grid.ExposedToSunlight[cell] < 15.0)
          return false;
      }
      return true;
    }

    public bool HasSufficientColonies() => this.CountColonies() >= EstablishColonies.BASE_COUNT;

    public int CountColonies()
    {
      int num = 0;
      for (int idx = 0; idx < Components.Telepads.Count; ++idx)
      {
        Activatable component = ((Component) Components.Telepads[idx]).GetComponent<Activatable>();
        if (Object.op_Equality((Object) component, (Object) null) || component.IsActivated)
          ++num;
      }
      return num;
    }

    public bool ConsumeParticlesAndCheckComplete(float dt)
    {
      float amount = Mathf.Min(dt * this.def.consumeRate, this.def.numParticlesToOpen - this.m_particlesConsumed);
      this.m_particlesConsumed += this.GetComponent<HighEnergyParticleStorage>().ConsumeAndGet(amount);
      this.UpdateMeter();
      return (double) this.m_particlesConsumed >= (double) this.def.numParticlesToOpen;
    }

    public void UpdateMeter() => this.m_meter.SetPositionPercent(this.GetAmountComplete());

    private float GetAmountComplete() => Mathf.Min(this.m_particlesConsumed / this.def.numParticlesToOpen, 1f);

    public float GetPercentComplete() => this.GetAmountComplete() * 100f;

    public void CreateBeamFX()
    {
      Vector3 position = this.gameObject.transform.position;
      position.y += 3.25f;
      Quaternion quaternion = Quaternion.Euler(-90f, 90f, 0.0f);
      Util.KInstantiate(EffectPrefabs.Instance.OpenTemporalTearBeam, position, quaternion, this.gameObject, (string) null, true, 0);
    }

    public void OpenTemporalTear()
    {
      ClusterManager.Instance.GetClusterPOIManager().RevealTemporalTear();
      ClusterManager.Instance.GetClusterPOIManager().OpenTemporalTear(this.GetMyWorldId());
    }

    public string SidescreenButtonText => (string) BUILDINGS.PREFABS.TEMPORALTEAROPENER.SIDESCREEN.TEXT;

    public string SidescreenButtonTooltip => (string) BUILDINGS.PREFABS.TEMPORALTEAROPENER.SIDESCREEN.TOOLTIP;

    public bool SidescreenEnabled() => this.GetCurrentState() == this.sm.ready || DebugHandler.InstantBuildMode;

    public bool SidescreenButtonInteractable() => this.GetCurrentState() == this.sm.ready || DebugHandler.InstantBuildMode;

    public void OnSidescreenButtonPressed()
    {
      ConfirmDialogScreen component = ((Component) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, (GameObject) null, (GameScreenManager.UIRenderTarget) 2)).GetComponent<ConfirmDialogScreen>();
      string confirmPopupMessage = (string) UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.CONFIRM_POPUP_MESSAGE;
      System.Action on_confirm = (System.Action) (() => this.FireTemporalTearOpener(this.smi));
      string confirmPopupConfirm = (string) UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.CONFIRM_POPUP_CONFIRM;
      string confirmPopupCancel = (string) UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.CONFIRM_POPUP_CANCEL;
      string confirmPopupTitle = (string) UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.CONFIRM_POPUP_TITLE;
      string confirm_text = confirmPopupConfirm;
      string cancel_text = confirmPopupCancel;
      component.PopupConfirmDialog(confirmPopupMessage, on_confirm, (System.Action) (() => { }), title_text: confirmPopupTitle, confirm_text: confirm_text, cancel_text: cancel_text);
    }

    private void FireTemporalTearOpener(TemporalTearOpener.Instance smi) => smi.GoTo((StateMachine.BaseState) this.sm.opening_tear_beam_pre);

    public int ButtonSideScreenSortOrder() => 20;

    public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();
  }
}
