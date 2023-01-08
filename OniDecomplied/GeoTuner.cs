// Decompiled with JetBrains decompiler
// Type: GeoTuner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GeoTuner : 
  GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>
{
  private StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Signal geyserSwitchSignal;
  private GeoTuner.NonOperationalState nonOperational;
  private GeoTuner.OperationalState operational;
  private StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.TargetParameter FutureGeyser;
  private StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.TargetParameter AssignedGeyser;
  public StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.BoolParameter hasBeenWorkedByResearcher;
  public StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.FloatParameter expirationTimer;
  public static string liquidGeyserTuningSoundPath = GlobalAssets.GetSound("GeoTuner_Tuning_Geyser");
  public static string gasGeyserTuningSoundPath = GlobalAssets.GetSound("GeoTuner_Tuning_Vent");
  public static string metalGeyserTuningSoundPath = GlobalAssets.GetSound("GeoTuner_Tuning_Volcano");
  public const string anim_switchGeyser_down = "geyser_down";
  public const string anim_switchGeyser_up = "geyser_up";
  private const string BroadcastingOnHoldAnimationName = "on";
  private const string OnAnimName = "on";
  private const string OffAnimName = "off";
  private const string BroadcastingAnimationName = "broadcasting";

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.operational;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.root.Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.RefreshAnimationGeyserSymbolType));
    this.nonOperational.DefaultState(this.nonOperational.off).OnSignal(this.geyserSwitchSignal, this.nonOperational.switchingGeyser).Enter((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback) (smi => smi.RefreshLogicOutput())).TagTransition(GameTags.Operational, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.operational);
    this.nonOperational.off.PlayAnim("off");
    this.nonOperational.switchingGeyser.QueueAnim("geyser_down").OnAnimQueueComplete(this.nonOperational.down);
    this.nonOperational.down.PlayAnim("geyser_up").Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.RefreshAnimationGeyserSymbolType)).Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.TriggerSoundsForGeyserChange)).ScheduleActionNextFrame("Switch Animation Completed", (System.Action<GeoTuner.Instance>) (smi => smi.GoTo((StateMachine.BaseState) this.nonOperational)));
    this.operational.PlayAnim("on").Enter((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback) (smi => smi.RefreshLogicOutput())).DefaultState(this.operational.idle).TagTransition(GameTags.Operational, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.nonOperational, true);
    this.operational.idle.ParamTransition<GameObject>((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Parameter<GameObject>) this.AssignedGeyser, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.operational.geyserSelected, GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.IsNotNull).ParamTransition<GameObject>((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Parameter<GameObject>) this.AssignedGeyser, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.operational.noGeyserSelected, GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.IsNull);
    this.operational.noGeyserSelected.ToggleMainStatusItem(Db.Get().BuildingStatusItems.GeoTunerNoGeyserSelected).ParamTransition<GameObject>((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Parameter<GameObject>) this.AssignedGeyser, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.operational.geyserSelected.switchingGeyser, GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.IsNotNull).Enter((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback) (smi => smi.RefreshLogicOutput())).Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.DropStorage)).Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.RefreshStorageRequirements)).Exit(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.ForgetWorkDoneByDupe)).Exit(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.ResetExpirationTimer)).QueueAnim("geyser_down").OnAnimQueueComplete(this.operational.noGeyserSelected.idle);
    this.operational.noGeyserSelected.idle.PlayAnim("geyser_up").QueueAnim("on").Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.RefreshAnimationGeyserSymbolType)).Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.TriggerSoundsForGeyserChange));
    this.operational.geyserSelected.DefaultState(this.operational.geyserSelected.idle).ToggleStatusItem(Db.Get().BuildingStatusItems.GeoTunerGeyserStatus).ParamTransition<GameObject>((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Parameter<GameObject>) this.AssignedGeyser, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.operational.noGeyserSelected, GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.IsNull).OnSignal(this.geyserSwitchSignal, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.operational.geyserSelected.switchingGeyser).Enter((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback) (smi => smi.RefreshLogicOutput()));
    this.operational.geyserSelected.idle.ParamTransition<bool>((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Parameter<bool>) this.hasBeenWorkedByResearcher, this.operational.geyserSelected.broadcasting.active, GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.IsTrue).ParamTransition<bool>((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Parameter<bool>) this.hasBeenWorkedByResearcher, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.operational.geyserSelected.researcherInteractionNeeded, GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.IsFalse);
    this.operational.geyserSelected.switchingGeyser.Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.DropStorageIfNotMatching)).Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.ForgetWorkDoneByDupe)).Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.ResetExpirationTimer)).Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.RefreshStorageRequirements)).Enter((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback) (smi => smi.RefreshLogicOutput())).QueueAnim("geyser_down").OnAnimQueueComplete(this.operational.geyserSelected.switchingGeyser.down);
    this.operational.geyserSelected.switchingGeyser.down.QueueAnim("geyser_up").QueueAnim("on").Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.RefreshAnimationGeyserSymbolType)).Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.TriggerSoundsForGeyserChange)).ScheduleActionNextFrame("Switch Animation Completed", (System.Action<GeoTuner.Instance>) (smi => smi.GoTo((StateMachine.BaseState) this.operational.geyserSelected.idle)));
    this.operational.geyserSelected.researcherInteractionNeeded.EventTransition(GameHashes.UpdateRoom, this.operational.geyserSelected.researcherInteractionNeeded.blocked, (StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Transition.ConditionCallback) (smi => !GeoTuner.WorkRequirementsMet(smi))).EventTransition(GameHashes.UpdateRoom, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.operational.geyserSelected.researcherInteractionNeeded.available, new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Transition.ConditionCallback(GeoTuner.WorkRequirementsMet)).EventTransition(GameHashes.OnStorageChange, this.operational.geyserSelected.researcherInteractionNeeded.blocked, (StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Transition.ConditionCallback) (smi => !GeoTuner.WorkRequirementsMet(smi))).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State) this.operational.geyserSelected.researcherInteractionNeeded.available, new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Transition.ConditionCallback(GeoTuner.WorkRequirementsMet)).ParamTransition<bool>((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Parameter<bool>) this.hasBeenWorkedByResearcher, this.operational.geyserSelected.broadcasting.active, GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.IsTrue).Exit(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.ResetExpirationTimer));
    this.operational.geyserSelected.researcherInteractionNeeded.blocked.ToggleMainStatusItem(Db.Get().BuildingStatusItems.GeoTunerResearchNeeded).DoNothing();
    this.operational.geyserSelected.researcherInteractionNeeded.available.DefaultState(this.operational.geyserSelected.researcherInteractionNeeded.available.waitingForDupe).ToggleRecurringChore(new Func<GeoTuner.Instance, Chore>(this.CreateResearchChore)).WorkableCompleteTransition((Func<GeoTuner.Instance, Workable>) (smi => (Workable) smi.workable), this.operational.geyserSelected.researcherInteractionNeeded.completed);
    this.operational.geyserSelected.researcherInteractionNeeded.available.waitingForDupe.ToggleMainStatusItem(Db.Get().BuildingStatusItems.GeoTunerResearchNeeded).WorkableStartTransition((Func<GeoTuner.Instance, Workable>) (smi => (Workable) smi.workable), this.operational.geyserSelected.researcherInteractionNeeded.available.inProgress);
    this.operational.geyserSelected.researcherInteractionNeeded.available.inProgress.ToggleMainStatusItem(Db.Get().BuildingStatusItems.GeoTunerResearchInProgress).WorkableStopTransition((Func<GeoTuner.Instance, Workable>) (smi => (Workable) smi.workable), this.operational.geyserSelected.researcherInteractionNeeded.available.waitingForDupe);
    this.operational.geyserSelected.researcherInteractionNeeded.completed.Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.OnResearchCompleted));
    this.operational.geyserSelected.broadcasting.ToggleMainStatusItem(Db.Get().BuildingStatusItems.GeoTunerBroadcasting).Toggle("Tuning", new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.ApplyTuning), new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.RemoveTuning));
    this.operational.geyserSelected.broadcasting.onHold.PlayAnim("on").UpdateTransition(this.operational.geyserSelected.broadcasting.active, (Func<GeoTuner.Instance, float, bool>) ((smi, dt) => !GeoTuner.GeyserExitEruptionTransition(smi, dt)));
    this.operational.geyserSelected.broadcasting.active.Toggle("EnergyConsumption", (StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback) (smi => smi.operational.SetActive(true)), (StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback) (smi => smi.operational.SetActive(false))).Toggle("BroadcastingAnimations", new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.PlayBroadcastingAnimation), new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.StopPlayingBroadcastingAnimation)).Update(new System.Action<GeoTuner.Instance, float>(GeoTuner.ExpirationTimerUpdate)).UpdateTransition(this.operational.geyserSelected.broadcasting.onHold, new Func<GeoTuner.Instance, float, bool>(GeoTuner.GeyserExitEruptionTransition)).ParamTransition<float>((StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.Parameter<float>) this.expirationTimer, this.operational.geyserSelected.broadcasting.expired, GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.IsLTEZero);
    this.operational.geyserSelected.broadcasting.expired.Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.ForgetWorkDoneByDupe)).Enter(new StateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State.Callback(GeoTuner.ResetExpirationTimer)).ScheduleActionNextFrame("Expired", (System.Action<GeoTuner.Instance>) (smi => smi.GoTo((StateMachine.BaseState) this.operational.geyserSelected.researcherInteractionNeeded)));
  }

  private static void TriggerSoundsForGeyserChange(GeoTuner.Instance smi)
  {
    Geyser assignedGeyser = smi.GetAssignedGeyser();
    if (!Object.op_Inequality((Object) assignedGeyser, (Object) null))
      return;
    EventInstance instance = new EventInstance();
    switch (assignedGeyser.configuration.geyserType.shape)
    {
      case GeyserConfigurator.GeyserShape.Gas:
        instance = SoundEvent.BeginOneShot(GeoTuner.gasGeyserTuningSoundPath, TransformExtensions.GetPosition(smi.transform));
        break;
      case GeyserConfigurator.GeyserShape.Liquid:
        instance = SoundEvent.BeginOneShot(GeoTuner.liquidGeyserTuningSoundPath, TransformExtensions.GetPosition(smi.transform));
        break;
      case GeyserConfigurator.GeyserShape.Molten:
        instance = SoundEvent.BeginOneShot(GeoTuner.metalGeyserTuningSoundPath, TransformExtensions.GetPosition(smi.transform));
        break;
    }
    SoundEvent.EndOneShot(instance);
  }

  private static void RefreshStorageRequirements(GeoTuner.Instance smi)
  {
    Geyser assignedGeyser = smi.GetAssignedGeyser();
    if (Object.op_Equality((Object) assignedGeyser, (Object) null))
    {
      smi.storage.capacityKg = 0.0f;
      smi.storage.storageFilters = (List<Tag>) null;
      smi.manualDelivery.capacity = 0.0f;
      smi.manualDelivery.refillMass = 0.0f;
      smi.manualDelivery.RequestedItemTag = Tag.op_Implicit((string) null);
      smi.manualDelivery.AbortDelivery("No geyser is selected for tuning");
    }
    else
    {
      GeoTunerConfig.GeotunedGeyserSettings settingsForGeyser = smi.def.GetSettingsForGeyser(assignedGeyser);
      smi.storage.capacityKg = settingsForGeyser.quantity;
      Storage storage = smi.storage;
      List<Tag> tagList = new List<Tag>();
      tagList.Add(settingsForGeyser.material);
      storage.storageFilters = tagList;
      smi.manualDelivery.AbortDelivery("Switching to new delivery request");
      smi.manualDelivery.capacity = settingsForGeyser.quantity;
      smi.manualDelivery.refillMass = settingsForGeyser.quantity;
      smi.manualDelivery.MinimumMass = settingsForGeyser.quantity;
      smi.manualDelivery.RequestedItemTag = settingsForGeyser.material;
    }
  }

  private static void DropStorage(GeoTuner.Instance smi) => smi.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);

  private static void DropStorageIfNotMatching(GeoTuner.Instance smi)
  {
    Geyser assignedGeyser = smi.GetAssignedGeyser();
    if (Object.op_Inequality((Object) assignedGeyser, (Object) null))
    {
      GeoTunerConfig.GeotunedGeyserSettings settingsForGeyser = smi.def.GetSettingsForGeyser(assignedGeyser);
      List<GameObject> items = smi.storage.GetItems();
      if (smi.storage.GetItems() == null || items.Count <= 0)
        return;
      Tag tag = items[0].PrefabID();
      PrimaryElement component = items[0].GetComponent<PrimaryElement>();
      if (Tag.op_Inequality(tag, settingsForGeyser.material))
      {
        smi.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
      }
      else
      {
        float amount = component.Mass - settingsForGeyser.quantity;
        if ((double) amount <= 0.0)
          return;
        smi.storage.DropSome(tag, amount, offset: new Vector3());
      }
    }
    else
      smi.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
  }

  private static bool GeyserExitEruptionTransition(GeoTuner.Instance smi, float dt)
  {
    Geyser assignedGeyser = smi.GetAssignedGeyser();
    return Object.op_Inequality((Object) assignedGeyser, (Object) null) && assignedGeyser.smi.GetCurrentState().parent != assignedGeyser.smi.sm.erupt;
  }

  public static void OnResearchCompleted(GeoTuner.Instance smi)
  {
    smi.storage.ConsumeAllIgnoringDisease();
    smi.sm.hasBeenWorkedByResearcher.Set(true, smi);
  }

  public static void PlayBroadcastingAnimation(GeoTuner.Instance smi) => smi.animController.Play(HashedString.op_Implicit("broadcasting"), (KAnim.PlayMode) 0);

  public static void StopPlayingBroadcastingAnimation(GeoTuner.Instance smi) => smi.animController.Play(HashedString.op_Implicit("broadcasting"));

  public static void RefreshAnimationGeyserSymbolType(GeoTuner.Instance smi) => smi.RefreshGeyserSymbol();

  public static float GetRemainingExpiraionTime(GeoTuner.Instance smi) => smi.sm.expirationTimer.Get(smi);

  private static void ExpirationTimerUpdate(GeoTuner.Instance smi, float dt)
  {
    float num1 = GeoTuner.GetRemainingExpiraionTime(smi) - dt;
    double num2 = (double) smi.sm.expirationTimer.Set(num1, smi);
  }

  private static void ResetExpirationTimer(GeoTuner.Instance smi)
  {
    Geyser assignedGeyser = smi.GetAssignedGeyser();
    if (Object.op_Inequality((Object) assignedGeyser, (Object) null))
    {
      double num1 = (double) smi.sm.expirationTimer.Set(smi.def.GetSettingsForGeyser(assignedGeyser).duration, smi);
    }
    else
    {
      double num2 = (double) smi.sm.expirationTimer.Set(0.0f, smi);
    }
  }

  private static void ForgetWorkDoneByDupe(GeoTuner.Instance smi)
  {
    smi.sm.hasBeenWorkedByResearcher.Set(false, smi);
    smi.workable.WorkTimeRemaining = smi.workable.GetWorkTime();
  }

  private Chore CreateResearchChore(GeoTuner.Instance smi) => (Chore) new WorkChore<GeoTunerWorkable>(Db.Get().ChoreTypes.Research, (IStateMachineTarget) smi.workable);

  private static void ApplyTuning(GeoTuner.Instance smi) => smi.GetAssignedGeyser().AddModification(smi.currentGeyserModification);

  private static void RemoveTuning(GeoTuner.Instance smi)
  {
    Geyser assignedGeyser = smi.GetAssignedGeyser();
    if (!Object.op_Inequality((Object) assignedGeyser, (Object) null))
      return;
    assignedGeyser.RemoveModification(smi.currentGeyserModification);
  }

  public static bool WorkRequirementsMet(GeoTuner.Instance smi) => GeoTuner.IsInLabRoom(smi) && (double) smi.storage.MassStored() == (double) smi.storage.capacityKg;

  public static bool IsInLabRoom(GeoTuner.Instance smi) => smi.roomTracker.IsInCorrectRoom();

  public class Def : StateMachine.BaseDef
  {
    public string OUTPUT_LOGIC_PORT_ID;
    public Dictionary<HashedString, GeoTunerConfig.GeotunedGeyserSettings> geotunedGeyserSettings;
    public GeoTunerConfig.GeotunedGeyserSettings defaultSetting;

    public GeoTunerConfig.GeotunedGeyserSettings GetSettingsForGeyser(Geyser geyser)
    {
      GeoTunerConfig.GeotunedGeyserSettings settingsForGeyser;
      if (this.geotunedGeyserSettings.TryGetValue(geyser.configuration.typeId, out settingsForGeyser))
        return settingsForGeyser;
      DebugUtil.DevLogError(string.Format("Geyser {0} is missing a Geotuner setting, using default", (object) geyser.configuration.typeId));
      return this.defaultSetting;
    }
  }

  public class BroadcastingState : 
    GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State
  {
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State active;
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State onHold;
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State expired;
  }

  public class ResearchProgress : 
    GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State
  {
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State waitingForDupe;
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State inProgress;
  }

  public class ResearchState : 
    GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State
  {
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State blocked;
    public GeoTuner.ResearchProgress available;
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State completed;
  }

  public class SwitchingGeyser : 
    GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State
  {
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State down;
  }

  public class GeyserSelectedState : 
    GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State
  {
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State idle;
    public GeoTuner.SwitchingGeyser switchingGeyser;
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State resourceNeeded;
    public GeoTuner.ResearchState researcherInteractionNeeded;
    public GeoTuner.BroadcastingState broadcasting;
  }

  public class SimpleIdleState : 
    GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State
  {
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State idle;
  }

  public class NonOperationalState : 
    GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State
  {
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State off;
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State switchingGeyser;
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State down;
  }

  public class OperationalState : 
    GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State
  {
    public GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.State idle;
    public GeoTuner.SimpleIdleState noGeyserSelected;
    public GeoTuner.GeyserSelectedState geyserSelected;
  }

  public enum GeyserAnimTypeSymbols
  {
    meter_gas,
    meter_metal,
    meter_liquid,
    meter_board,
  }

  public new class Instance : 
    GameStateMachine<GeoTuner, GeoTuner.Instance, IStateMachineTarget, GeoTuner.Def>.GameInstance
  {
    [MyCmpReq]
    public Operational operational;
    [MyCmpReq]
    public Storage storage;
    [MyCmpReq]
    public ManualDeliveryKG manualDelivery;
    [MyCmpReq]
    public GeoTunerWorkable workable;
    [MyCmpReq]
    public GeoTunerSwitchGeyserWorkable switchGeyserWorkable;
    [MyCmpReq]
    public LogicPorts logicPorts;
    [MyCmpReq]
    public RoomTracker roomTracker;
    [MyCmpReq]
    public KBatchedAnimController animController;
    public MeterController switchGeyserMeter;
    public string originID;
    public float enhancementDuration;
    public Geyser.GeyserModification currentGeyserModification;
    private Chore switchGeyserChore;

    public Instance(IStateMachineTarget master, GeoTuner.Def def)
      : base(master, def)
    {
      this.originID = UI.StripLinkFormatting(nameof (GeoTuner)) + " [" + ((Object) this.gameObject).GetInstanceID().ToString() + "]";
      this.switchGeyserMeter = new MeterController((KAnimControllerBase) this.animController, "geyser_target", this.GetAnimationSymbol().ToString(), Meter.Offset.Behind, Grid.SceneLayer.NoLayer, Array.Empty<string>());
    }

    public override void StartSM()
    {
      base.StartSM();
      Components.GeoTuners.Add(this.smi.GetMyWorldId(), this);
      Geyser assignedGeyser = this.GetAssignedGeyser();
      if (Object.op_Inequality((Object) assignedGeyser, (Object) null))
      {
        assignedGeyser.Subscribe(-593169791, new System.Action<object>(this.OnEruptionStateChanged));
        this.RefreshModification();
      }
      this.RefreshLogicOutput();
      this.AssignFutureGeyser(this.GetFutureGeyser());
      KMonoBehaviourExtensions.Subscribe(this.gameObject, -905833192, new System.Action<object>(this.OnCopySettings));
    }

    public Geyser GetFutureGeyser() => this.smi.sm.FutureGeyser.IsNull(this) ? (Geyser) null : this.sm.FutureGeyser.Get(this).GetComponent<Geyser>();

    public Geyser GetAssignedGeyser() => this.smi.sm.AssignedGeyser.IsNull(this) ? (Geyser) null : this.sm.AssignedGeyser.Get(this).GetComponent<Geyser>();

    public void AssignFutureGeyser(Geyser newFutureGeyser)
    {
      int num = Object.op_Inequality((Object) newFutureGeyser, (Object) this.GetFutureGeyser()) ? 1 : 0;
      bool flag = Object.op_Inequality((Object) this.GetAssignedGeyser(), (Object) newFutureGeyser);
      this.sm.FutureGeyser.Set((KMonoBehaviour) newFutureGeyser, this);
      if (num != 0)
      {
        if (flag)
        {
          this.RecreateSwitchGeyserChore();
        }
        else
        {
          if (this.switchGeyserChore == null)
            return;
          this.AbortSwitchGeyserChore("Future Geyser was set to current Geyser");
        }
      }
      else
      {
        if (!(this.switchGeyserChore == null & flag))
          return;
        this.RecreateSwitchGeyserChore();
      }
    }

    private void AbortSwitchGeyserChore(string reason = "Aborting Switch Geyser Chore")
    {
      if (this.switchGeyserChore != null)
      {
        this.switchGeyserChore.onComplete -= new System.Action<Chore>(this.OnSwitchGeyserChoreCompleted);
        this.switchGeyserChore.Cancel(reason);
        this.switchGeyserChore = (Chore) null;
      }
      this.switchGeyserChore = (Chore) null;
    }

    private Chore RecreateSwitchGeyserChore()
    {
      this.AbortSwitchGeyserChore("Recreating Chore");
      this.switchGeyserChore = (Chore) new WorkChore<GeoTunerSwitchGeyserWorkable>(Db.Get().ChoreTypes.Toggle, (IStateMachineTarget) this.switchGeyserWorkable, on_begin: new System.Action<Chore>(this.ShowSwitchingGeyserStatusItem), on_end: new System.Action<Chore>(this.HideSwitchingGeyserStatusItem), only_when_operational: false);
      this.switchGeyserChore.onComplete += new System.Action<Chore>(this.OnSwitchGeyserChoreCompleted);
      return this.switchGeyserChore;
    }

    private void ShowSwitchingGeyserStatusItem(Chore chore) => this.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingSwitchToggle);

    private void HideSwitchingGeyserStatusItem(Chore chore) => this.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingSwitchToggle);

    private void OnSwitchGeyserChoreCompleted(Chore chore)
    {
      this.GetCurrentState();
      GeoTuner.NonOperationalState nonOperational = this.sm.nonOperational;
      Geyser futureGeyser = this.GetFutureGeyser();
      bool flag = Object.op_Inequality((Object) this.GetAssignedGeyser(), (Object) futureGeyser);
      if (chore.isComplete & flag)
        this.AssignGeyser(futureGeyser);
      this.Trigger(1980521255);
    }

    public void AssignGeyser(Geyser geyser)
    {
      Geyser assignedGeyser = this.GetAssignedGeyser();
      if (Object.op_Inequality((Object) assignedGeyser, (Object) null) && Object.op_Inequality((Object) assignedGeyser, (Object) geyser))
      {
        GeoTuner.RemoveTuning(this.smi);
        assignedGeyser.Unsubscribe(-593169791, new System.Action<object>(this.smi.OnEruptionStateChanged));
      }
      Geyser geyser1 = assignedGeyser;
      this.sm.AssignedGeyser.Set((KMonoBehaviour) geyser, this);
      this.RefreshModification();
      if (!Object.op_Inequality((Object) geyser1, (Object) geyser))
        return;
      if (Object.op_Inequality((Object) geyser, (Object) null))
      {
        geyser.Subscribe(-593169791, new System.Action<object>(this.OnEruptionStateChanged));
        geyser.Trigger(1763323737, (object) null);
      }
      if (Object.op_Inequality((Object) geyser1, (Object) null))
        geyser1.Trigger(1763323737, (object) null);
      this.sm.geyserSwitchSignal.Trigger(this);
    }

    private void RefreshModification()
    {
      Geyser assignedGeyser = this.GetAssignedGeyser();
      if (Object.op_Inequality((Object) assignedGeyser, (Object) null))
      {
        GeoTunerConfig.GeotunedGeyserSettings settingsForGeyser = this.def.GetSettingsForGeyser(assignedGeyser);
        this.currentGeyserModification = settingsForGeyser.template;
        this.currentGeyserModification.originID = this.originID;
        this.enhancementDuration = settingsForGeyser.duration;
        assignedGeyser.Trigger(1763323737, (object) null);
      }
      GeoTuner.RefreshStorageRequirements(this);
      GeoTuner.DropStorageIfNotMatching(this);
    }

    public void RefreshGeyserSymbol() => this.switchGeyserMeter.meterController.Play(HashedString.op_Implicit(this.GetAnimationSymbol().ToString()));

    private GeoTuner.GeyserAnimTypeSymbols GetAnimationSymbol()
    {
      GeoTuner.GeyserAnimTypeSymbols animationSymbol = GeoTuner.GeyserAnimTypeSymbols.meter_board;
      Geyser assignedGeyser = this.smi.GetAssignedGeyser();
      if (Object.op_Inequality((Object) assignedGeyser, (Object) null))
      {
        switch (assignedGeyser.configuration.geyserType.shape)
        {
          case GeyserConfigurator.GeyserShape.Gas:
            animationSymbol = GeoTuner.GeyserAnimTypeSymbols.meter_gas;
            break;
          case GeyserConfigurator.GeyserShape.Liquid:
            animationSymbol = GeoTuner.GeyserAnimTypeSymbols.meter_liquid;
            break;
          case GeyserConfigurator.GeyserShape.Molten:
            animationSymbol = GeoTuner.GeyserAnimTypeSymbols.meter_metal;
            break;
        }
      }
      return animationSymbol;
    }

    public void OnEruptionStateChanged(object data)
    {
      int num = (bool) data ? 1 : 0;
      this.RefreshLogicOutput();
    }

    public void RefreshLogicOutput()
    {
      Geyser assignedGeyser = this.GetAssignedGeyser();
      int num1 = this.GetCurrentState() != this.smi.sm.nonOperational ? 1 : 0;
      bool flag1 = Object.op_Inequality((Object) assignedGeyser, (Object) null) && this.GetCurrentState() != this.smi.sm.operational.noGeyserSelected;
      bool flag2 = Object.op_Inequality((Object) assignedGeyser, (Object) null) && assignedGeyser.smi.GetCurrentState() != null && (assignedGeyser.smi.GetCurrentState() == assignedGeyser.smi.sm.erupt || assignedGeyser.smi.GetCurrentState().parent == assignedGeyser.smi.sm.erupt);
      int num2 = flag1 ? 1 : 0;
      bool is_visible = (num1 & num2 & (flag2 ? 1 : 0)) != 0;
      this.logicPorts.SendSignal(HashedString.op_Implicit(this.def.OUTPUT_LOGIC_PORT_ID), is_visible ? 1 : 0);
      this.switchGeyserMeter.meterController.SetSymbolVisiblity(KAnimHashedString.op_Implicit("light_bloom"), is_visible);
    }

    public void OnCopySettings(object data)
    {
      GameObject go = (GameObject) data;
      if (!Object.op_Inequality((Object) go, (Object) null))
        return;
      GeoTuner.Instance smi = go.GetSMI<GeoTuner.Instance>();
      if (smi == null || !Object.op_Inequality((Object) smi.GetFutureGeyser(), (Object) this.GetFutureGeyser()))
        return;
      Geyser futureGeyser = smi.GetFutureGeyser();
      if (!Object.op_Inequality((Object) futureGeyser, (Object) null) || futureGeyser.GetAmountOfGeotunersPointingOrWillPointAtThisGeyser() >= 5)
        return;
      this.AssignFutureGeyser(smi.GetFutureGeyser());
    }

    protected override void OnCleanUp()
    {
      Geyser assignedGeyser = this.GetAssignedGeyser();
      Components.GeoTuners.Remove(this.smi.GetMyWorldId(), this);
      if (Object.op_Inequality((Object) assignedGeyser, (Object) null))
        assignedGeyser.Unsubscribe(-593169791, new System.Action<object>(this.smi.OnEruptionStateChanged));
      GeoTuner.RemoveTuning(this);
    }
  }
}
