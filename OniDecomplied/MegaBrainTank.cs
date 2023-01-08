// Decompiled with JetBrains decompiler
// Type: MegaBrainTank
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MegaBrainTank : StateMachineComponent<MegaBrainTank.StatesInstance>
{
  [Serialize]
  private bool introDisplayed;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    StoryManager.Instance.ForceCreateStory(Db.Get().Stories.MegaBrainTank, ((Component) this).gameObject.GetMyWorldId());
    this.smi.StartSM();
    this.Subscribe(-1503271301, new Action<object>(this.OnBuildingSelect));
    ((Component) this).GetComponent<Activatable>().SetWorkTime(5f);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    this.Unsubscribe(-1503271301);
  }

  private void OnBuildingSelect(object obj)
  {
    if (!(bool) obj)
      return;
    if (!this.introDisplayed)
    {
      this.introDisplayed = true;
      EventInfoScreen.ShowPopup(EventInfoDataHelper.GenerateStoryTraitData((string) CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.BEGIN_POPUP.NAME, (string) CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.BEGIN_POPUP.DESCRIPTION, (string) CODEX.STORY_TRAITS.CLOSE_BUTTON, "braintankdiscovered_kanim", EventInfoDataHelper.PopupType.BEGIN, callback: new System.Action(this.DoInitialUnlock)));
    }
    this.smi.ShowEventCompleteUI();
  }

  private void DoInitialUnlock() => Game.Instance.unlocks.Unlock("story_trait_mega_brain_tank_initial");

  public class States : 
    GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank>
  {
    public Operational.Flag activationCost = new Operational.Flag("brains restored", Operational.Flag.Type.Requirement);
    public MegaBrainTank.States.BrainState brain;
    public StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.BoolParameter dormantParam;
    public StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.BoolParameter activeParam;
    public Effect StatBonus;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      this.serializable = StateMachine.SerializeType.ParamsOnly;
      default_state = (StateMachine.BaseState) this.brain;
      this.brain.Initialize(this);
      this.brain.DefaultState((GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State) this.brain.inactive);
      this.brain.active.ParamTransition<bool>((StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>) this.activeParam, (GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State) this.brain.inactive, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsInactive));
      this.brain.active.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State) this.brain.inactive, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Transition.ConditionCallback(this.brain.IsInactive));
      this.brain.inactive.ParamTransition<bool>((StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>) this.activeParam, (GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State) this.brain.active.dormant, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsDormant));
      this.brain.inactive.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State) this.brain.active.dormant, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Transition.ConditionCallback(this.brain.IsDormant));
      this.brain.active.conscious.ParamTransition<bool>((StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>) this.dormantParam, (GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State) this.brain.active.dormant, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsDormant));
      this.brain.inactive.ParamTransition<bool>((StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>) this.activeParam, (GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State) this.brain.active.conscious, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsConscious));
      this.brain.inactive.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State) this.brain.active.conscious, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Transition.ConditionCallback(this.brain.IsConscious));
      this.brain.active.dormant.ParamTransition<bool>((StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>) this.dormantParam, (GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State) this.brain.active.conscious, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsConscious));
      this.StatBonus = new Effect("MegaBrainTankBonus", (string) DUPLICANTS.MODIFIERS.MEGABRAINTANKBONUS.NAME, (string) DUPLICANTS.MODIFIERS.MEGABRAINTANKBONUS.TOOLTIP, 0.0f, true, true, false);
      object[,] statBonuses = MegaBrainTankConfig.STAT_BONUSES;
      int length = statBonuses.GetLength(0);
      for (int index = 0; index < length; ++index)
        this.StatBonus.Add(new AttributeModifier(statBonuses[index, 0] as string, ModifierSet.ConvertValue(((float?) statBonuses[index, 1]).Value, ((Units?) statBonuses[index, 2]).Value), (string) DUPLICANTS.MODIFIERS.MEGABRAINTANKBONUS.NAME));
    }

    public class BrainState : BrainTankState
    {
      public MegaBrainTank.States.InactiveState inactive;
      public MegaBrainTank.States.ActiveState active;

      public bool IsInactive(MegaBrainTank.StatesInstance smi, bool _) => !smi.IsActive;

      public bool IsDormant(MegaBrainTank.StatesInstance smi, bool _) => smi.IsActive && smi.IsHungry;

      public bool IsConscious(MegaBrainTank.StatesInstance smi, bool _) => smi.IsActive && !smi.IsHungry;

      public bool IsInactive(MegaBrainTank.StatesInstance smi) => this.IsInactive(smi, false);

      public bool IsDormant(MegaBrainTank.StatesInstance smi) => this.IsDormant(smi, false);

      public bool IsConscious(MegaBrainTank.StatesInstance smi) => this.IsConscious(smi, false);

      public override void Initialize(MegaBrainTank.States sm)
      {
        this.EventHandler(GameHashes.BuildingActivated, new GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.GameEvent.Callback(this.OnActivatableChanged));
        this.EventHandler(GameHashes.OperationalChanged, new GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.GameEvent.Callback(this.OnBuildingOperationalChanged));
        this.Update(new Action<MegaBrainTank.StatesInstance, float>(((BrainTankState) this).OnUpdate), (UpdateRate) 4, false);
        this.inactive.Initialize(sm);
        this.active.Initialize(sm);
      }

      public override void OnUpdate(MegaBrainTank.StatesInstance smi, float dt)
      {
        smi.IncrementMeter(dt);
        if (smi.UnitsFromLastStore == (short) 0)
          return;
        smi.ShelveJournals(dt);
      }

      public override void OnAnimComplete(
        MegaBrainTank.StatesInstance smi,
        HashedString completedAnim)
      {
        if (HashedString.op_Inequality(completedAnim, MegaBrainTankConfig.KACHUNK))
          return;
        smi.StoreJournals();
      }

      private void OnBuildingOperationalChanged(MegaBrainTank.StatesInstance smi, object _)
      {
        if (!smi.IsActive)
          return;
        smi.Operational.SetActive(true);
      }

      private void OnActivatableChanged(MegaBrainTank.StatesInstance smi, object data)
      {
        if (!(bool) data)
          return;
        if (!this.sm.activeParam.Get(smi))
        {
          StoryManager.Instance.BeginStoryEvent(Db.Get().Stories.MegaBrainTank);
          smi.Selectable.AddStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankActivationProgress, (object) smi);
        }
        else
          smi.Selectable.AddStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankComplete, (object) smi);
      }
    }

    public class InactiveState : BrainTankState
    {
      public override void Initialize(MegaBrainTank.States sm)
      {
        this.Enter(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(((BrainTankState) this).OnEnter));
        this.Update(new Action<MegaBrainTank.StatesInstance, float>(((BrainTankState) this).OnUpdate), (UpdateRate) 4, false);
        this.Exit(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(((BrainTankState) this).OnExit));
      }

      public override void OnEnter(MegaBrainTank.StatesInstance smi)
      {
        smi.SetBonusActive(false);
        smi.ElementConverter.SetAllConsumedActive(false);
        smi.Selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankDreamAnalysis);
        smi.ElementConverter.SetConsumedElementActive(DreamJournalConfig.ID, false);
        ((Behaviour) ((Component) smi.master).GetComponent<Light2D>()).enabled = false;
      }

      public override void OnExit(MegaBrainTank.StatesInstance smi)
      {
        smi.ElementConverter.SetConsumedElementActive(DreamJournalConfig.ID, true);
        RequireInputs component = smi.GetComponent<RequireInputs>();
        component.requireConduitHasMass = true;
        component.visualizeRequirements = RequireInputs.Requirements.All;
      }

      public override void OnUpdate(MegaBrainTank.StatesInstance smi, float dt) => smi.ActivateBrains(dt);

      public override void OnAnimComplete(
        MegaBrainTank.StatesInstance smi,
        HashedString completedAnim)
      {
        if (HashedString.op_Inequality(completedAnim, smi.CurrentActivationAnim))
          return;
        smi.CompleteBrainActivation();
      }
    }

    public class ActiveState : BrainTankState
    {
      public MegaBrainTank.States.DormantState dormant;
      public MegaBrainTank.States.ConsciousState conscious;

      public override void Initialize(MegaBrainTank.States sm)
      {
        this.Enter(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(((BrainTankState) this).OnEnter));
        this.Exit(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(((BrainTankState) this).OnExit));
        this.dormant.Initialize(sm);
        this.conscious.Initialize(sm);
      }

      public override void OnEnter(MegaBrainTank.StatesInstance smi) => ((Behaviour) ((Component) smi.master).GetComponent<Light2D>()).enabled = false;

      public override void OnExit(MegaBrainTank.StatesInstance smi) => smi.ElementConverter.SetConsumedElementActive(DreamJournalConfig.ID, false);
    }

    public class DormantState : BrainTankState
    {
      public override void Initialize(MegaBrainTank.States sm)
      {
        this.EventHandler(GameHashes.OnStorageChange, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(this.OnStorageChanged));
        this.Enter(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(((BrainTankState) this).OnEnter));
      }

      public override void OnEnter(MegaBrainTank.StatesInstance smi)
      {
        smi.CleanTank();
        bool flag = smi.ElementConverter.HasEnoughMass(GameTags.Oxygen, true);
        smi.Selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.MegaBrainNotEnoughOxygen, !flag);
        ((Behaviour) ((Component) smi.master).GetComponent<Light2D>()).enabled = false;
      }

      private void OnStorageChanged(MegaBrainTank.StatesInstance smi)
      {
        double massAvailable1 = (double) smi.BrainStorage.GetMassAvailable(GameTags.Oxygen);
        float massAvailable2 = smi.BrainStorage.GetMassAvailable(DreamJournalConfig.ID);
        bool flag = massAvailable1 >= 1.0;
        smi.sm.dormantParam.Set((double) massAvailable2 <= 0.0 || !flag, smi);
        smi.Selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.MegaBrainNotEnoughOxygen, !flag);
      }
    }

    public class ConsciousState : BrainTankState
    {
      public override void Initialize(MegaBrainTank.States sm)
      {
        this.Enter(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(((BrainTankState) this).OnEnter));
        this.Update(new Action<MegaBrainTank.StatesInstance, float>(((BrainTankState) this).OnUpdate), (UpdateRate) 4, false);
      }

      public override void OnEnter(MegaBrainTank.StatesInstance smi)
      {
        smi.CleanTank();
        smi.Selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MegaBrainNotEnoughOxygen);
        ((Behaviour) ((Component) smi.master).GetComponent<Light2D>()).enabled = true;
      }

      public override void OnUpdate(MegaBrainTank.StatesInstance smi, float dt) => smi.Digest(dt);

      public override void OnAnimComplete(
        MegaBrainTank.StatesInstance smi,
        HashedString completedAnim)
      {
        if (HashedString.op_Inequality(completedAnim, MegaBrainTankConfig.ACTIVATE_ALL))
          return;
        smi.CompleteBrainActivation();
      }
    }
  }

  public class StatesInstance : 
    GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.GameInstance
  {
    private static List<Effects> minionEffects;
    public short UnitsFromLastStore;
    private float meterFill = 0.04f;
    private float targetProgress;
    private float timeTilDigested;
    private float journalActivationTimer;
    private float lastRemainingTime;
    private byte activatedJournals;
    private bool currentlyActivating;
    private short nextActiveBrain = 1;
    private string brainHum;
    private KBatchedAnimController[] controllers;
    private KAnimLink fxLink;
    private MeterController meter;
    private EventInfoData eventInfo;
    private Notification eventComplete;
    private Notifier notifier;

    public KBatchedAnimController BrainController => this.controllers[0];

    public KBatchedAnimController ShelfController => this.controllers[1];

    public Storage BrainStorage { get; private set; }

    public KSelectable Selectable { get; private set; }

    public Operational Operational { get; private set; }

    public ElementConverter ElementConverter { get; private set; }

    public ManualDeliveryKG JournalDelivery { get; private set; }

    public LoopingSounds BrainSounds { get; private set; }

    public bool IsActive => this.Operational.IsOperational && this.sm.activeParam.Get(this);

    public bool IsHungry => !this.ElementConverter.HasEnoughMassToStartConverting(true);

    public int TimeTilDigested => (int) this.timeTilDigested;

    public int ActivationProgress => (int) (25.0 * (double) this.meterFill);

    public HashedString CurrentActivationAnim => MegaBrainTankConfig.ACTIVATION_ANIMS[(int) this.nextActiveBrain - 1];

    private HashedString currentActivationLoop
    {
      get
      {
        int index = (int) this.nextActiveBrain - 1 + 5;
        return MegaBrainTankConfig.ACTIVATION_ANIMS[index];
      }
    }

    public StatesInstance(MegaBrainTank master)
      : base(master)
    {
      this.BrainSounds = this.GetComponent<LoopingSounds>();
      this.BrainStorage = this.GetComponent<Storage>();
      this.ElementConverter = this.GetComponent<ElementConverter>();
      this.JournalDelivery = this.GetComponent<ManualDeliveryKG>();
      this.Operational = this.GetComponent<Operational>();
      this.Selectable = this.GetComponent<KSelectable>();
      this.notifier = this.GetComponent<Notifier>();
      this.controllers = this.gameObject.GetComponentsInChildren<KBatchedAnimController>();
      this.meter = new MeterController((KAnimControllerBase) this.BrainController, "meter_oxygen_target", nameof (meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, MegaBrainTankConfig.METER_SYMBOLS);
      this.fxLink = new KAnimLink((KAnimControllerBase) this.BrainController, (KAnimControllerBase) this.ShelfController);
    }

    public override void StartSM()
    {
      this.InitializeEffectsList();
      base.StartSM();
      this.BrainController.onAnimComplete += new KAnimControllerBase.KAnimEvent(this.OnAnimComplete);
      this.ShelfController.onAnimComplete += new KAnimControllerBase.KAnimEvent(this.OnAnimComplete);
      Storage brainStorage = this.BrainStorage;
      brainStorage.OnWorkableEventCB = brainStorage.OnWorkableEventCB + new Action<Workable, Workable.WorkableEvent>(this.OnJournalDeliveryStateChanged);
      this.brainHum = GlobalAssets.GetSound("MegaBrainTank_brain_wave_LP");
      StoryManager.Instance.DiscoverStoryEvent(Db.Get().Stories.MegaBrainTank);
      bool flag = this.sm.activeParam.Get(this);
      this.Operational.SetFlag(this.sm.activationCost, flag);
      float unitsAvailable = this.BrainStorage.GetUnitsAvailable(DreamJournalConfig.ID);
      if (!flag)
      {
        this.meterFill = this.targetProgress = unitsAvailable / 25f;
        this.meter.SetPositionPercent(this.meterFill);
        short num = (short) (5.0 * (double) this.meterFill);
        if (num <= (short) 0)
          return;
        this.nextActiveBrain = num;
        this.BrainSounds.StartSound(this.brainHum);
        this.BrainSounds.SetParameter(this.brainHum, HashedString.op_Implicit("BrainTankProgress"), (float) num);
        this.CompleteBrainActivation();
      }
      else
      {
        this.timeTilDigested = unitsAvailable * 60f;
        this.meterFill = this.timeTilDigested - this.timeTilDigested % 0.04f;
        this.meterFill /= 1500f;
        this.meter.SetPositionPercent(this.meterFill);
        StoryManager.Instance.BeginStoryEvent(Db.Get().Stories.MegaBrainTank);
        this.nextActiveBrain = (short) 5;
        this.CompleteBrainActivation();
      }
    }

    public override void StopSM(string reason)
    {
      this.BrainController.onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.OnAnimComplete);
      this.ShelfController.onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.OnAnimComplete);
      Storage brainStorage = this.BrainStorage;
      brainStorage.OnWorkableEventCB = brainStorage.OnWorkableEventCB - new Action<Workable, Workable.WorkableEvent>(this.OnJournalDeliveryStateChanged);
      base.StopSM(reason);
    }

    private void InitializeEffectsList()
    {
      Components.Cmps<MinionIdentity> minionIdentities = Components.LiveMinionIdentities;
      minionIdentities.OnAdd += new Action<MinionIdentity>(this.OnLiveMinionIdAdded);
      minionIdentities.OnRemove += new Action<MinionIdentity>(this.OnLiveMinionIdRemoved);
      MegaBrainTank.StatesInstance.minionEffects = new List<Effects>(minionIdentities.Count > 32 ? minionIdentities.Count : 32);
      for (int idx = 0; idx < minionIdentities.Count; ++idx)
        this.OnLiveMinionIdAdded(minionIdentities[idx]);
    }

    private void OnLiveMinionIdAdded(MinionIdentity id)
    {
      Effects component = ((Component) id).GetComponent<Effects>();
      MegaBrainTank.StatesInstance.minionEffects.Add(component);
      if (!this.sm.activeParam.Get(this) || !this.sm.dormantParam.Get(this) || !this.IsActive)
        return;
      component.Add(this.sm.StatBonus, false);
    }

    private void OnLiveMinionIdRemoved(MinionIdentity id)
    {
      Effects component = ((Component) id).GetComponent<Effects>();
      MegaBrainTank.StatesInstance.minionEffects.Remove(component);
    }

    public void SetBonusActive(bool active)
    {
      for (int index = 0; index < MegaBrainTank.StatesInstance.minionEffects.Count; ++index)
      {
        if (active)
          MegaBrainTank.StatesInstance.minionEffects[index].Add(this.sm.StatBonus, false);
        else
          MegaBrainTank.StatesInstance.minionEffects[index].Remove(this.sm.StatBonus);
      }
    }

    private void OnAnimComplete(HashedString anim)
    {
      if (!(this.GetCurrentState() is BrainTankState brainTankState))
        return;
      for (; brainTankState != null; brainTankState = brainTankState.parent as BrainTankState)
        brainTankState.OnAnimComplete(this, anim);
    }

    private void OnJournalDeliveryStateChanged(Workable w, Workable.WorkableEvent state)
    {
      switch (state)
      {
        case Workable.WorkableEvent.WorkStarted:
          FetchAreaChore.StatesInstance smi = ((Component) w.worker).GetSMI<FetchAreaChore.StatesInstance>();
          if (smi.IsNullOrStopped())
            break;
          Pickupable component = smi.sm.deliveryObject.Get(smi).GetComponent<Pickupable>();
          this.UnitsFromLastStore = (short) component.PrimaryElement.Units;
          this.BrainStorage.SetWorkTime(Mathf.Clamp01(component.PrimaryElement.Units / 5f) * this.BrainStorage.storageWorkTime);
          break;
        case Workable.WorkableEvent.WorkStopped:
          break;
        default:
          this.ShelfController.Play(MegaBrainTankConfig.KACHUNK);
          break;
      }
    }

    public void ShelveJournals(float dt)
    {
      float num1 = this.lastRemainingTime - this.BrainStorage.WorkTimeRemaining;
      if ((double) num1 <= 0.0)
        num1 = this.BrainStorage.storageWorkTime - this.BrainStorage.WorkTimeRemaining;
      this.lastRemainingTime = this.BrainStorage.WorkTimeRemaining;
      if ((double) this.BrainStorage.storageWorkTime / 5.0 - (double) this.journalActivationTimer > 1.0 / 1000.0)
      {
        this.journalActivationTimer += num1;
      }
      else
      {
        int index1 = -1;
        this.journalActivationTimer = 0.0f;
        for (int index2 = 0; index2 < MegaBrainTankConfig.JOURNAL_SYMBOLS.Length; ++index2)
        {
          byte num2 = (byte) (1 << index2);
          int num3 = ((int) this.activatedJournals & (int) num2) == 0 ? 1 : 0;
          if (num3 != 0 && index1 == -1)
            index1 = index2;
          if ((num3 & ((double) Random.Range(0.0f, 1f) >= 0.5 ? 1 : 0)) != 0)
          {
            index1 = -1;
            this.activatedJournals |= num2;
            this.ShelfController.SetSymbolVisiblity(KAnimHashedString.op_Implicit(MegaBrainTankConfig.JOURNAL_SYMBOLS[index2]), true);
            break;
          }
        }
        if (index1 != -1)
          this.ShelfController.SetSymbolVisiblity(KAnimHashedString.op_Implicit(MegaBrainTankConfig.JOURNAL_SYMBOLS[index1]), true);
        --this.UnitsFromLastStore;
      }
    }

    public void StoreJournals()
    {
      this.lastRemainingTime = 0.0f;
      this.activatedJournals = (byte) 0;
      for (int index = 0; index < MegaBrainTankConfig.JOURNAL_SYMBOLS.Length; ++index)
        this.ShelfController.SetSymbolVisiblity(KAnimHashedString.op_Implicit(MegaBrainTankConfig.JOURNAL_SYMBOLS[index]), false);
      this.ShelfController.PlayMode = (KAnim.PlayMode) 2;
      this.ShelfController.SetPositionPercent(0.0f);
      this.targetProgress = Mathf.Clamp01(this.BrainStorage.GetUnitsAvailable(DreamJournalConfig.ID) / 25f);
    }

    public void ActivateBrains(float dt)
    {
      if (this.currentlyActivating)
        return;
      this.currentlyActivating = (double) this.nextActiveBrain / 5.0 - (double) this.meterFill <= 1.0 / 1000.0;
      if (!this.currentlyActivating)
        return;
      this.BrainController.QueueAndSyncTransition(this.CurrentActivationAnim);
      if (this.nextActiveBrain <= (short) 0)
        return;
      this.BrainSounds.StartSound(this.brainHum);
      this.BrainSounds.SetParameter(this.brainHum, HashedString.op_Implicit("BrainTankProgress"), (float) this.nextActiveBrain);
    }

    public void CompleteBrainActivation()
    {
      this.BrainController.Play(this.currentActivationLoop, (KAnim.PlayMode) 0);
      ++this.nextActiveBrain;
      this.currentlyActivating = false;
      if (this.nextActiveBrain <= (short) 5)
        return;
      this.timeTilDigested = this.BrainStorage.GetUnitsAvailable(DreamJournalConfig.ID) * 60f;
      this.Operational.SetFlag(this.sm.activationCost, true);
      this.CompleteEvent();
    }

    public void Digest(float dt)
    {
      this.timeTilDigested = this.BrainStorage.GetUnitsAvailable(DreamJournalConfig.ID) * 60f;
      this.sm.dormantParam.Set(this.IsHungry, this);
      if ((double) this.targetProgress - (double) this.meterFill > (double) Mathf.Epsilon)
        return;
      this.targetProgress = 0.0f;
      float num = this.meterFill - this.timeTilDigested / 1500f;
      if ((double) num < 0.039999999105930328)
        return;
      this.meterFill -= num - num % 0.04f;
      this.meter.SetPositionPercent(this.meterFill);
    }

    public void CleanTank()
    {
      bool flag = !this.sm.dormantParam.Get(this);
      this.SetBonusActive(flag);
      this.Selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankDreamAnalysis, flag, (object) this);
      this.ElementConverter.SetAllConsumedActive(flag);
      if (flag)
      {
        this.nextActiveBrain = (short) 5;
        this.BrainController.QueueAndSyncTransition(MegaBrainTankConfig.ACTIVATE_ALL);
        this.BrainSounds.StartSound(this.brainHum);
        this.BrainSounds.SetParameter(this.brainHum, HashedString.op_Implicit("BrainTankProgress"), (float) this.nextActiveBrain);
      }
      else
      {
        if ((double) this.timeTilDigested < 0.01666666753590107)
        {
          this.BrainStorage.ConsumeAllIgnoringDisease(DreamJournalConfig.ID);
          this.timeTilDigested = 0.0f;
          this.meterFill = 0.0f;
          this.meter.SetPositionPercent(this.meterFill);
        }
        this.BrainController.QueueAndSyncTransition(MegaBrainTankConfig.DEACTIVATE_ALL);
        this.BrainSounds.StopSound(this.brainHum);
      }
    }

    public bool IncrementMeter(float dt)
    {
      if ((double) this.targetProgress - (double) this.meterFill <= (double) Mathf.Epsilon)
        return false;
      this.meterFill += Mathf.Lerp(0.0f, 1f, 0.04f * dt);
      if (1.0 - (double) this.meterFill <= 1.0 / 1000.0)
        this.meterFill = 1f;
      this.meter.SetPositionPercent(this.meterFill);
      return (double) this.targetProgress - (double) this.meterFill > 1.0 / 1000.0;
    }

    public void CompleteEvent()
    {
      this.Selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankActivationProgress);
      this.Selectable.AddStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankComplete, (object) this.smi);
      StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.MegaBrainTank.HashId);
      if (storyInstance == null || this.sm.activeParam.Get(this) && storyInstance.CurrentState == StoryInstance.State.COMPLETE)
        return;
      this.eventInfo = EventInfoDataHelper.GenerateStoryTraitData((string) CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.END_POPUP.NAME, (string) CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.END_POPUP.DESCRIPTION, (string) CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.END_POPUP.BUTTON, "braintankcomplete_kanim", EventInfoDataHelper.PopupType.COMPLETE);
      this.smi.Selectable.AddStatusItem(Db.Get().MiscStatusItems.AttentionRequired, (object) this.smi);
      this.eventComplete = EventInfoScreen.CreateNotification(this.eventInfo, new Notification.ClickCallback(this.ShowEventCompleteUI));
      this.notifier.Add(this.eventComplete);
    }

    public void ShowEventCompleteUI(object _ = null)
    {
      if (this.eventComplete == null)
        return;
      this.smi.Selectable.RemoveStatusItem(Db.Get().MiscStatusItems.AttentionRequired);
      this.notifier.Remove(this.eventComplete);
      this.eventComplete = (Notification) null;
      Game.Instance.unlocks.Unlock("story_trait_mega_brain_tank_competed");
      Vector3 posCcc = Grid.CellToPosCCC(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this.master), new CellOffset(0, 3)), Grid.SceneLayer.Ore);
      StoryManager.Instance.CompleteStoryEvent(Db.Get().Stories.MegaBrainTank, (MonoBehaviour) this.master, new FocusTargetSequence.Data()
      {
        WorldId = this.master.GetMyWorldId(),
        OrthographicSize = 6f,
        TargetSize = 6f,
        Target = posCcc,
        PopupData = this.eventInfo,
        CompleteCB = new System.Action(this.OnCompleteStorySequence),
        CanCompleteCB = (Func<bool>) null
      });
    }

    private void OnCompleteStorySequence()
    {
      Vector3 posCcc = Grid.CellToPosCCC(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this.master), new CellOffset(0, 2)), Grid.SceneLayer.Ore);
      StoryManager.Instance.CompleteStoryEvent(Db.Get().Stories.MegaBrainTank, posCcc);
      this.eventInfo = (EventInfoData) null;
      this.sm.dormantParam.Set(this.smi.IsHungry, this.smi);
      this.sm.activeParam.Set(true, this);
    }
  }
}
