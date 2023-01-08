// Decompiled with JetBrains decompiler
// Type: GravitasCreatureManipulator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GravitasCreatureManipulator : 
  GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>
{
  public GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State inoperational;
  public GravitasCreatureManipulator.ActiveStates operational;
  public StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.TargetParameter creatureTarget;
  public StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.FloatParameter cooldownTimer;
  public StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.FloatParameter workingTimer;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.inoperational;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.root.Enter((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State.Callback) (smi => smi.DropCritter())).Enter((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State.Callback) (smi => smi.UpdateMeter())).EventHandler(GameHashes.BuildingActivated, (GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.GameEvent.Callback) ((smi, activated) =>
    {
      if (!(bool) activated)
        return;
      StoryManager.Instance.BeginStoryEvent(Db.Get().Stories.CreatureManipulator);
    }));
    this.inoperational.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, this.operational.idle, (StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
    this.operational.DefaultState(this.operational.idle).EventTransition(GameHashes.OperationalChanged, this.inoperational, (StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
    this.operational.idle.PlayAnim("idle", (KAnim.PlayMode) 0).Enter(new StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State.Callback(GravitasCreatureManipulator.CheckForCritter)).ToggleMainStatusItem(Db.Get().BuildingStatusItems.CreatureManipulatorWaiting).ParamTransition<GameObject>((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.Parameter<GameObject>) this.creatureTarget, this.operational.capture, (StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Inequality((Object) p, (Object) null) && !smi.IsCritterStored)).ParamTransition<GameObject>((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.Parameter<GameObject>) this.creatureTarget, this.operational.working.pre, (StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Inequality((Object) p, (Object) null) && smi.IsCritterStored)).ParamTransition<float>((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.Parameter<float>) this.cooldownTimer, this.operational.cooldown, GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.IsGTZero);
    this.operational.capture.PlayAnim("working_capture").OnAnimQueueComplete(this.operational.working.pre);
    this.operational.working.DefaultState(this.operational.working.pre).ToggleMainStatusItem(Db.Get().BuildingStatusItems.CreatureManipulatorWorking);
    double num1;
    this.operational.working.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.operational.working.loop).Enter((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State.Callback) (smi => smi.StoreCreature())).Exit((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State.Callback) (smi => num1 = (double) smi.sm.workingTimer.Set(smi.def.workingDuration, smi))).OnTargetLost(this.creatureTarget, this.operational.idle).Target(this.creatureTarget).ToggleStationaryIdling();
    double num2;
    this.operational.working.loop.PlayAnim("working_loop", (KAnim.PlayMode) 0).Update((System.Action<GravitasCreatureManipulator.Instance, float>) ((smi, dt) => num2 = (double) smi.sm.workingTimer.DeltaClamp(-dt, 0.0f, float.MaxValue, smi)), (UpdateRate) 6).ParamTransition<float>((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.Parameter<float>) this.workingTimer, this.operational.working.pst, GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.IsLTEZero).OnTargetLost(this.creatureTarget, this.operational.idle);
    this.operational.working.pst.PlayAnim("working_pst").Enter((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State.Callback) (smi => smi.DropCritter())).OnAnimQueueComplete(this.operational.cooldown);
    double num3;
    GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State state = this.operational.cooldown.PlayAnim("working_cooldown", (KAnim.PlayMode) 0).Update((System.Action<GravitasCreatureManipulator.Instance, float>) ((smi, dt) => num3 = (double) smi.sm.cooldownTimer.DeltaClamp(-dt, 0.0f, float.MaxValue, smi)), (UpdateRate) 6).ParamTransition<float>((StateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.Parameter<float>) this.cooldownTimer, this.operational.idle, GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.IsLTEZero);
    string name = (string) CREATURES.STATUSITEMS.GRAVITAS_CREATURE_MANIPULATOR_COOLDOWN.NAME;
    string tooltip = (string) CREATURES.STATUSITEMS.GRAVITAS_CREATURE_MANIPULATOR_COOLDOWN.TOOLTIP;
    HashedString render_overlay = new HashedString();
    StatusItemCategory main = Db.Get().StatusItemCategories.Main;
    Func<string, GravitasCreatureManipulator.Instance, string> resolve_string_callback = new Func<string, GravitasCreatureManipulator.Instance, string>(GravitasCreatureManipulator.Processing);
    Func<string, GravitasCreatureManipulator.Instance, string> resolve_tooltip_callback = new Func<string, GravitasCreatureManipulator.Instance, string>(GravitasCreatureManipulator.ProcessingTooltip);
    StatusItemCategory category = main;
    state.ToggleStatusItem(name, tooltip, render_overlay: render_overlay, resolve_string_callback: resolve_string_callback, resolve_tooltip_callback: resolve_tooltip_callback, category: category);
  }

  private static string Processing(string str, GravitasCreatureManipulator.Instance smi) => str.Replace("{percent}", GameUtil.GetFormattedPercent((float) ((1.0 - (double) smi.sm.cooldownTimer.Get(smi) / (double) smi.def.cooldownDuration) * 100.0)));

  private static string ProcessingTooltip(string str, GravitasCreatureManipulator.Instance smi) => str.Replace("{timeleft}", GameUtil.GetFormattedTime(smi.sm.cooldownTimer.Get(smi)));

  private static void CheckForCritter(GravitasCreatureManipulator.Instance smi)
  {
    if (!smi.sm.creatureTarget.IsNull(smi))
      return;
    GameObject gameObject1 = Grid.Objects[smi.pickupCell, 3];
    if (!Object.op_Inequality((Object) gameObject1, (Object) null))
      return;
    ObjectLayerListItem objectLayerListItem = gameObject1.GetComponent<Pickupable>().objectLayerListItem;
    while (objectLayerListItem != null)
    {
      GameObject gameObject2 = objectLayerListItem.gameObject;
      objectLayerListItem = objectLayerListItem.nextItem;
      if (!Object.op_Equality((Object) gameObject2, (Object) null) && smi.IsAccepted(gameObject2))
      {
        smi.SetCritterTarget(gameObject2);
        break;
      }
    }
  }

  public class Def : StateMachine.BaseDef
  {
    public CellOffset pickupOffset;
    public CellOffset dropOffset;
    public int numSpeciesToUnlockMorphMode;
    public float workingDuration;
    public float cooldownDuration;
  }

  public class WorkingStates : 
    GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State
  {
    public GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State pre;
    public GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State loop;
    public GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State pst;
  }

  public class ActiveStates : 
    GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State
  {
    public GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State idle;
    public GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State capture;
    public GravitasCreatureManipulator.WorkingStates working;
    public GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.State cooldown;
  }

  public new class Instance : 
    GameStateMachine<GravitasCreatureManipulator, GravitasCreatureManipulator.Instance, IStateMachineTarget, GravitasCreatureManipulator.Def>.GameInstance
  {
    public int pickupCell;
    [MyCmpGet]
    private Storage m_storage;
    [Serialize]
    public HashSet<Tag> ScannedSpecies = new HashSet<Tag>();
    [Serialize]
    private bool m_introPopupSeen;
    [Serialize]
    private bool m_morphModeUnlocked;
    private EventInfoData eventInfo;
    private Notification m_endNotification;
    private MeterController m_progressMeter;
    private HandleVector<int>.Handle m_partitionEntry;
    private HandleVector<int>.Handle m_largeCreaturePartitionEntry;

    public Instance(IStateMachineTarget master, GravitasCreatureManipulator.Def def)
      : base(master, def)
    {
      this.pickupCell = Grid.OffsetCell(Grid.PosToCell(master.gameObject), this.smi.def.pickupOffset);
      this.m_partitionEntry = GameScenePartitioner.Instance.Add(nameof (GravitasCreatureManipulator), (object) this.gameObject, this.pickupCell, GameScenePartitioner.Instance.pickupablesChangedLayer, new System.Action<object>(this.DetectCreature));
      this.m_largeCreaturePartitionEntry = GameScenePartitioner.Instance.Add("GravitasCreatureManipulator.large", (object) this.gameObject, Grid.CellLeft(this.pickupCell), GameScenePartitioner.Instance.pickupablesChangedLayer, new System.Action<object>(this.DetectLargeCreature));
      this.m_progressMeter = new MeterController((KAnimControllerBase) this.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.UserSpecified, Grid.SceneLayer.TileFront, Array.Empty<string>());
    }

    public override void StartSM()
    {
      base.StartSM();
      this.UpdateStatusItems();
      this.UpdateMeter();
      StoryManager.Instance.ForceCreateStory(Db.Get().Stories.CreatureManipulator, this.gameObject.GetMyWorldId());
      if (this.ScannedSpecies.Count >= this.smi.def.numSpeciesToUnlockMorphMode)
        StoryManager.Instance.BeginStoryEvent(Db.Get().Stories.CreatureManipulator);
      this.TryShowCompletedNotification();
      this.Subscribe(-1503271301, new System.Action<object>(this.OnBuildingSelect));
      StoryManager.Instance.DiscoverStoryEvent(Db.Get().Stories.CreatureManipulator);
    }

    public override void StopSM(string reason)
    {
      this.Unsubscribe(-1503271301, new System.Action<object>(this.OnBuildingSelect));
      base.StopSM(reason);
    }

    private void OnBuildingSelect(object obj)
    {
      if (!(bool) obj)
        return;
      if (!this.m_introPopupSeen)
        this.ShowIntroNotification();
      if (this.m_endNotification == null)
        return;
      this.m_endNotification.customClickCallback(this.m_endNotification.customClickData);
    }

    public bool IsMorphMode => this.m_morphModeUnlocked;

    public bool IsCritterStored => this.m_storage.Count > 0;

    private void UpdateStatusItems()
    {
      KSelectable component = this.gameObject.GetComponent<KSelectable>();
      component.ToggleStatusItem(Db.Get().BuildingStatusItems.CreatureManipulatorProgress, !this.IsMorphMode, (object) this);
      component.ToggleStatusItem(Db.Get().BuildingStatusItems.CreatureManipulatorMorphMode, this.IsMorphMode, (object) this);
      component.ToggleStatusItem(Db.Get().BuildingStatusItems.CreatureManipulatorMorphModeLocked, !this.IsMorphMode, (object) this);
    }

    public void UpdateMeter() => this.m_progressMeter.SetPositionPercent(Mathf.Clamp01((float) ((double) this.ScannedSpecies.Count / (double) this.smi.def.numSpeciesToUnlockMorphMode - 0.10000000149011612)));

    public bool IsAccepted(GameObject go)
    {
      KPrefabID component = go.GetComponent<KPrefabID>();
      return component.HasTag(GameTags.Creature) && !component.HasTag(GameTags.Robot) && Tag.op_Inequality(component.PrefabTag, GameTags.Creature);
    }

    private void DetectLargeCreature(object obj)
    {
      Pickupable pickupable = obj as Pickupable;
      if (Object.op_Equality((Object) pickupable, (Object) null))
        return;
      Bounds bounds = ((Component) pickupable).GetComponent<KCollider2D>().bounds;
      if ((double) ((Bounds) ref bounds).size.x > 1.5)
      {
        this.DetectCreature(obj);
      }
      else
      {
        if (!this.smi.IsInsideState((StateMachine.BaseState) this.sm.operational.idle))
          return;
        Navigator component = ((Component) pickupable).GetComponent<Navigator>();
        if (!Object.op_Inequality((Object) component, (Object) null) || !this.IsAccepted(((Component) component).gameObject))
          return;
        component.GoTo(this.smi.pickupCell);
      }
    }

    private void DetectCreature(object obj)
    {
      Pickupable pickupable = obj as Pickupable;
      if (!Object.op_Inequality((Object) pickupable, (Object) null) || !this.IsAccepted(((Component) pickupable).gameObject) || !this.smi.sm.creatureTarget.IsNull(this.smi) || !this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.operational.idle))
        return;
      this.SetCritterTarget(((Component) pickupable).gameObject);
    }

    public void SetCritterTarget(GameObject go) => this.smi.sm.creatureTarget.Set(go.gameObject, this.smi);

    public void StoreCreature() => this.m_storage.Store(this.smi.sm.creatureTarget.Get(this.smi));

    public void DropCritter()
    {
      List<GameObject> collect_dropped_items = new List<GameObject>();
      this.m_storage.DropAll(Grid.CellToPosCBC(Grid.PosToCell((StateMachine.Instance) this.smi), Grid.SceneLayer.Creatures), offset: ((CellOffset) ref this.smi.def.dropOffset).ToVector3(), collect_dropped_items: collect_dropped_items);
      foreach (GameObject go in collect_dropped_items)
      {
        CreatureBrain component = go.GetComponent<CreatureBrain>();
        if (!Object.op_Equality((Object) component, (Object) null))
        {
          this.Scan(component.species);
          if (((Component) component).HasTag(GameTags.OriginalCreature) && this.IsMorphMode)
            this.SpawnMorph((Brain) component);
          else
            go.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("idle_loop"));
        }
      }
      this.smi.sm.creatureTarget.Set((KMonoBehaviour) null, this.smi);
    }

    private void Scan(Tag species)
    {
      if (this.ScannedSpecies.Add(species))
      {
        EventExtensions.Trigger(this.gameObject, 1980521255, (object) null);
        this.UpdateStatusItems();
        this.UpdateMeter();
        this.ShowCritterScannedNotification(species);
      }
      this.TryShowCompletedNotification();
    }

    public void SpawnMorph(Brain brain)
    {
      Tag tag1 = Tag.Invalid;
      BabyMonitor.Instance smi1 = ((Component) brain).GetSMI<BabyMonitor.Instance>();
      FertilityMonitor.Instance smi2 = ((Component) brain).GetSMI<FertilityMonitor.Instance>();
      bool flag1 = smi1 != null;
      bool flag2 = smi2 != null;
      if (flag2)
        tag1 = FertilityMonitor.EggBreedingRoll(smi2.breedingChances, true);
      else if (flag1)
        tag1 = FertilityMonitor.EggBreedingRoll(Assets.GetPrefab(smi1.def.adultPrefab).GetDef<FertilityMonitor.Def>().initialBreedingWeights, true);
      if (!((Tag) ref tag1).IsValid)
        return;
      Tag tag2 = Assets.GetPrefab(tag1).GetDef<IncubationMonitor.Def>().spawnedCreature;
      if (flag2)
        tag2 = Assets.GetPrefab(tag2).GetDef<BabyMonitor.Def>().adultPrefab;
      Vector3 position = TransformExtensions.GetPosition(brain.transform);
      position.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
      GameObject go = Util.KInstantiate(Assets.GetPrefab(tag2), position);
      go.SetActive(true);
      go.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("growup_pst"));
      foreach (AmountInstance amount in (Modifications<Amount, AmountInstance>) ((Component) brain).gameObject.GetAmounts())
      {
        AmountInstance amountInstance = amount.amount.Lookup(go);
        if (amountInstance != null)
        {
          float num = amount.value / amount.GetMax();
          amountInstance.value = num * amountInstance.GetMax();
        }
      }
      EventExtensions.Trigger(go, -2027483228, (object) ((Component) brain).gameObject);
      KSelectable component = ((Component) brain).gameObject.GetComponent<KSelectable>();
      if (Object.op_Inequality((Object) SelectTool.Instance, (Object) null) && Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) null) && Object.op_Equality((Object) SelectTool.Instance.selected, (Object) component))
        SelectTool.Instance.Select(go.GetComponent<KSelectable>());
      double num1 = (double) this.smi.sm.cooldownTimer.Set(this.smi.def.cooldownDuration, this.smi);
      TracesExtesions.DeleteObject(((Component) brain).gameObject);
    }

    public void ShowIntroNotification()
    {
      Game.Instance.unlocks.Unlock("story_trait_critter_manipulator_initial");
      this.m_introPopupSeen = true;
      EventInfoScreen.ShowPopup(EventInfoDataHelper.GenerateStoryTraitData((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.BEGIN_POPUP.NAME, (string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.BEGIN_POPUP.DESCRIPTION, (string) CODEX.STORY_TRAITS.CLOSE_BUTTON, "crittermanipulatoractivate_kanim", EventInfoDataHelper.PopupType.BEGIN));
    }

    public void ShowCritterScannedNotification(Tag species)
    {
      Game.Instance.unlocks.Unlock(GravitasCreatureManipulatorConfig.CRITTER_LORE_UNLOCK_ID.For(species), false);
      ShowCritterScannedNotificationAndWaitForClick().Then((System.Action) (() => GravitasCreatureManipulator.Instance.ShowLoreUnlockedPopup(species)));

      Promise ShowCritterScannedNotificationAndWaitForClick() => new Promise((System.Action<System.Action>) (resolve =>
      {
        Notification notification1 = new Notification((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.UNLOCK_SPECIES_NOTIFICATION.NAME, NotificationType.Event, (Func<List<Notification>, object, string>) ((notifications, obj) =>
        {
          string str = (string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.UNLOCK_SPECIES_NOTIFICATION.TOOLTIP;
          foreach (Notification notification2 in notifications)
          {
            string tooltipData = notification2.tooltipData as string;
            str = str + "\n • " + StringEntry.op_Implicit(Strings.Get("STRINGS.CREATURES.FAMILY_PLURAL." + tooltipData));
          }
          return str;
        }), (object) species.ToString().ToUpper(), false, custom_click_callback: ((Notification.ClickCallback) (obj => resolve())), clear_on_click: true);
        this.gameObject.AddOrGet<Notifier>().Add(notification1);
      }));
    }

    public static void ShowLoreUnlockedPopup(Tag species)
    {
      InfoDialogScreen infoDialogScreen = LoreBearer.ShowPopupDialog().SetHeader((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.UNLOCK_SPECIES_POPUP.NAME).AddDefaultOK();
      int num = CodexCache.GetEntryForLock(GravitasCreatureManipulatorConfig.CRITTER_LORE_UNLOCK_ID.For(species)) != null ? 1 : 0;
      Option<string> contentForSpeciesTag = GravitasCreatureManipulatorConfig.GetBodyContentForSpeciesTag(species);
      if (num != 0 && contentForSpeciesTag.HasValue)
        infoDialogScreen.AddPlainText(contentForSpeciesTag.Value).AddOption((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.UNLOCK_SPECIES_POPUP.VIEW_IN_CODEX, LoreBearerUtil.OpenCodexByEntryID("STORYTRAITCRITTERMANIPULATOR"));
      else
        infoDialogScreen.AddPlainText(GravitasCreatureManipulatorConfig.GetBodyContentForUnknownSpecies());
    }

    public void TryShowCompletedNotification()
    {
      if (this.ScannedSpecies.Count < this.smi.def.numSpeciesToUnlockMorphMode || this.IsMorphMode)
        return;
      this.eventInfo = EventInfoDataHelper.GenerateStoryTraitData((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.END_POPUP.NAME, (string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.END_POPUP.DESCRIPTION, (string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.END_POPUP.BUTTON, "crittermanipulatormorphmode_kanim", EventInfoDataHelper.PopupType.COMPLETE);
      this.m_endNotification = EventInfoScreen.CreateNotification(this.eventInfo, new Notification.ClickCallback(this.UnlockMorphMode));
      this.gameObject.AddOrGet<Notifier>().Add(this.m_endNotification);
      this.gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.AttentionRequired, (object) this.smi);
    }

    public void ClearEndNotification()
    {
      this.gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.AttentionRequired);
      if (this.m_endNotification != null)
        this.gameObject.AddOrGet<Notifier>().Remove(this.m_endNotification);
      this.m_endNotification = (Notification) null;
    }

    public void UnlockMorphMode(object _)
    {
      if (this.m_morphModeUnlocked)
        return;
      Game.Instance.unlocks.Unlock("story_trait_critter_manipulator_complete");
      if (this.m_endNotification != null)
        this.gameObject.AddOrGet<Notifier>().Remove(this.m_endNotification);
      this.m_morphModeUnlocked = true;
      this.UpdateStatusItems();
      this.ClearEndNotification();
      Vector3 posCcc = Grid.CellToPosCCC(Grid.OffsetCell(Grid.PosToCell((StateMachine.Instance) this.smi), new CellOffset(0, 2)), Grid.SceneLayer.Ore);
      StoryManager.Instance.CompleteStoryEvent(Db.Get().Stories.CreatureManipulator, this.gameObject.GetComponent<MonoBehaviour>(), new FocusTargetSequence.Data()
      {
        WorldId = this.smi.GetMyWorldId(),
        OrthographicSize = 6f,
        TargetSize = 6f,
        Target = posCcc,
        PopupData = this.eventInfo,
        CompleteCB = new System.Action(this.OnStorySequenceComplete),
        CanCompleteCB = (Func<bool>) null
      });
    }

    private void OnStorySequenceComplete()
    {
      Vector3 posCcc = Grid.CellToPosCCC(Grid.OffsetCell(Grid.PosToCell((StateMachine.Instance) this.smi), new CellOffset(-1, 1)), Grid.SceneLayer.Ore);
      StoryManager.Instance.CompleteStoryEvent(Db.Get().Stories.CreatureManipulator, posCcc);
      this.eventInfo = (EventInfoData) null;
    }

    protected override void OnCleanUp()
    {
      GameScenePartitioner.Instance.Free(ref this.m_partitionEntry);
      GameScenePartitioner.Instance.Free(ref this.m_largeCreaturePartitionEntry);
      if (this.m_endNotification == null)
        return;
      this.gameObject.AddOrGet<Notifier>().Remove(this.m_endNotification);
    }
  }
}
