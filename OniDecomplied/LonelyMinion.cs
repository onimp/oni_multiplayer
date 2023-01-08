// Decompiled with JetBrains decompiler
// Type: LonelyMinion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LonelyMinion : 
  GameStateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>
{
  public StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.TargetParameter Mail;
  public StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.BoolParameter Active;
  public GameStateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State Idle;
  public GameStateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State Inactive;
  public LonelyMinion.MailStates CheckMail;

  private bool HahCheckedMail(LonelyMinion.Instance smi)
  {
    if (HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.CHECK_MAIL))
    {
      if (Object.op_Equality((Object) this.Mail.Get(smi), (Object) smi.gameObject))
      {
        this.Mail.Set((GameObject) null, smi, true);
        smi.AnimController.Play(LonelyMinionConfig.CHECK_MAIL_FAILURE);
        return false;
      }
      this.CheckForMail(smi);
      return false;
    }
    if ((HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.FOOD_FAILURE) ? 1 : (HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.FOOD_DUPLICATE) ? 1 : 0)) != 0)
    {
      smi.Drop();
      return false;
    }
    return HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.CHECK_MAIL_FAILURE) || HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.CHECK_MAIL_SUCCESS) || HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.CHECK_MAIL_DUPLICATE);
  }

  private void CheckForMail(LonelyMinion.Instance smi)
  {
    Tag prefabTag = this.Mail.Get(smi).GetComponent<KPrefabID>().PrefabTag;
    QuestInstance instance = QuestManager.GetInstance(smi.def.QuestOwnerId, Db.Get().Quests.LonelyMinionFoodQuest);
    Debug.Assert(instance != null);
    Quest.ItemData data1 = new Quest.ItemData()
    {
      CriteriaId = LonelyMinionConfig.FoodCriteriaId,
      SatisfyingItem = prefabTag,
      QualifyingTag = GameTags.Edible,
      CurrentValue = (float) EdiblesManager.GetFoodInfo(((Tag) ref prefabTag).Name).Quality
    };
    LonelyMinion.MailStates currentState = smi.GetCurrentState() as LonelyMinion.MailStates;
    bool dataSatisfies;
    bool itemIsRedundant;
    instance.TrackProgress(data1, out dataSatisfies, out itemIsRedundant);
    StateMachine.BaseState state = (StateMachine.BaseState) currentState.Success;
    string name = (string) CODEX.STORY_TRAITS.LONELYMINION.GIFTRESPONSE_POPUP.TASTYFOOD.NAME;
    string tooltip = (string) CODEX.STORY_TRAITS.LONELYMINION.GIFTRESPONSE_POPUP.TASTYFOOD.TOOLTIP;
    if (itemIsRedundant)
    {
      state = (StateMachine.BaseState) currentState.Duplicate;
      name = (string) CODEX.STORY_TRAITS.LONELYMINION.GIFTRESPONSE_POPUP.REPEATEDFOOD.NAME;
      tooltip = (string) CODEX.STORY_TRAITS.LONELYMINION.GIFTRESPONSE_POPUP.REPEATEDFOOD.TOOLTIP;
    }
    else if (!dataSatisfies)
    {
      state = (StateMachine.BaseState) currentState.Failure;
      name = (string) CODEX.STORY_TRAITS.LONELYMINION.GIFTRESPONSE_POPUP.CRAPPYFOOD.NAME;
      tooltip = (string) CODEX.STORY_TRAITS.LONELYMINION.GIFTRESPONSE_POPUP.CRAPPYFOOD.TOOLTIP;
    }
    Pickupable component = this.Mail.Get(smi).GetComponent<Pickupable>();
    smi.Pickup(component, state != currentState.Success);
    smi.ScheduleGoTo(0.016f, state);
    Notification notification = new Notification(name, NotificationType.Event, (Func<List<Notification>, object, string>) ((notificationList, data) => data as string), (object) tooltip, false, click_focus: smi.transform.parent, clear_on_click: true, show_dismiss_button: true);
    ((Component) smi.transform.parent).gameObject.AddOrGet<Notifier>().Add(notification);
  }

  private void EvaluateCurrentDecor(LonelyMinion.Instance smi, float dt)
  {
    QuestInstance instance = QuestManager.GetInstance(smi.def.QuestOwnerId, Db.Get().Quests.LonelyMinionDecorQuest);
    if (smi.GetCurrentState() == this.Inactive || instance.IsComplete)
      return;
    float averageDecor = LonelyMinionHouse.CalculateAverageDecor(smi.def.DecorInspectionArea);
    bool flag = (double) averageDecor >= 0.0 || (double) averageDecor > (double) smi.StartingAverageDecor && 1.0 - (double) averageDecor / (double) smi.StartingAverageDecor > 0.10000000149011612;
    if (!instance.IsStarted && !flag)
      return;
    instance.TrackProgress(new Quest.ItemData()
    {
      CriteriaId = LonelyMinionConfig.DecorCriteriaId,
      CurrentValue = averageDecor
    }, out bool _, out bool _);
  }

  private void DelayIdle(LonelyMinion.Instance smi, float dt)
  {
    if (HashedString.op_Inequality(smi.AnimController.currentAnim, HashedString.op_Implicit(smi.AnimController.defaultAnim)))
      return;
    if ((double) smi.IdleDelayTimer > 0.0)
      smi.IdleDelayTimer -= dt;
    if ((double) smi.IdleDelayTimer > 0.0)
      return;
    this.PlayIdle(smi, smi.ChooseIdle());
    smi.IdleDelayTimer = Random.Range(20f, 40f);
  }

  private void PlayIdle(LonelyMinion.Instance smi, HashedString idleAnim)
  {
    if (!((HashedString) ref idleAnim).IsValid)
      return;
    if (HashedString.op_Equality(idleAnim, LonelyMinionConfig.CHECK_MAIL))
    {
      this.Mail.Set(smi.gameObject, smi, false);
    }
    else
    {
      QuestInstance instance = QuestManager.GetInstance(smi.def.QuestOwnerId, Db.Get().Quests.LonelyMinionFoodQuest);
      int num = instance.GetCurrentCount(LonelyMinionConfig.FoodCriteriaId) - 1;
      if (HashedString.op_Equality(idleAnim, LonelyMinionConfig.FOOD_IDLE) && num > 0)
      {
        KBatchedAnimController component = Assets.GetPrefab(instance.GetSatisfyingItem(LonelyMinionConfig.FoodCriteriaId, Random.Range(0, num))).GetComponent<KBatchedAnimController>();
        smi.PackageSnapPoint.SwapAnims(component.AnimFiles);
        smi.PackageSnapPoint.Play(HashedString.op_Implicit("object"), (KAnim.PlayMode) 0);
      }
      if ((HashedString.op_Equality(idleAnim, LonelyMinionConfig.FOOD_IDLE) || HashedString.op_Equality(idleAnim, LonelyMinionConfig.DECOR_IDLE) ? 1 : (HashedString.op_Equality(idleAnim, LonelyMinionConfig.POWER_IDLE) ? 1 : 0)) == 0)
      {
        LonelyMinionHouse.Instance smi1 = ((Component) smi.transform.parent).GetSMI<LonelyMinionHouse.Instance>();
        smi.AnimController.GetSynchronizer().Remove((KAnimControllerBase) smi1.AnimController);
        if (HashedString.op_Equality(idleAnim, LonelyMinionConfig.BLINDS_IDLE_0))
          smi1.BlindsController.Play(LonelyMinionConfig.BLINDS_IDLE_0);
      }
      smi.AnimController.Play(idleAnim);
    }
  }

  private void OnIdleAnimComplete(LonelyMinion.Instance smi)
  {
    if (HashedString.op_Equality(smi.AnimController.currentAnim, HashedString.op_Implicit(smi.AnimController.defaultAnim)))
      return;
    if ((HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.FOOD_IDLE) || HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.DECOR_IDLE) ? 1 : (HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.POWER_IDLE) ? 1 : 0)) == 0)
    {
      LonelyMinionHouse.Instance smi1 = ((Component) smi.transform.parent).GetSMI<LonelyMinionHouse.Instance>();
      smi.AnimController.GetSynchronizer().Add((KAnimControllerBase) smi1.AnimController);
      if (HashedString.op_Equality(smi.AnimController.currentAnim, LonelyMinionConfig.BLINDS_IDLE_0))
        smi1.BlindsController.Play(HashedString.op_Implicit(string.Format("{0}_{1}", (object) "meter_blinds", (object) 0)), (KAnim.PlayMode) 2);
    }
    smi.AnimController.Play(HashedString.op_Implicit(smi.AnimController.defaultAnim), smi.AnimController.initialMode);
    if (!this.Active.Get(smi) || !Object.op_Inequality((Object) this.Mail.Get(smi), (Object) null))
      return;
    smi.ScheduleGoTo(0.0f, (StateMachine.BaseState) this.CheckMail);
  }

  private void OnBecomeInactive(LonelyMinion.Instance smi)
  {
    smi.AnimController.GetSynchronizer().Clear();
    smi.AnimController.Play(HashedString.op_Implicit(smi.AnimController.initialAnim), smi.AnimController.initialMode);
  }

  private void OnBecomeActive(LonelyMinion.Instance smi)
  {
    LonelyMinionHouse.Instance smi1 = ((Component) smi.transform.parent).GetSMI<LonelyMinionHouse.Instance>();
    smi.AnimController.GetSynchronizer().Add((KAnimControllerBase) smi1.AnimController);
    if ((double) smi.StartingAverageDecor != double.NegativeInfinity)
      return;
    smi.StartingAverageDecor = LonelyMinionHouse.CalculateAverageDecor(smi.def.DecorInspectionArea);
  }

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.Inactive;
    this.root.ParamTransition<bool>((StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.Parameter<bool>) this.Active, this.Inactive, (StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.Parameter<bool>.Callback) ((smi, p) => !this.Active.Get(smi))).ParamTransition<bool>((StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.Parameter<bool>) this.Active, this.Idle, (StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.Parameter<bool>.Callback) ((smi, p) => this.Active.Get(smi))).Update(new System.Action<LonelyMinion.Instance, float>(this.EvaluateCurrentDecor), (UpdateRate) 6);
    this.Inactive.Enter(new StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback(this.OnBecomeInactive)).Exit(new StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback(this.OnBecomeActive));
    this.Idle.ParamTransition<GameObject>((StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.Parameter<GameObject>) this.Mail, (GameStateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State) this.CheckMail, (StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.Parameter<GameObject>.Callback) ((smi, p) => HashedString.op_Equality(smi.AnimController.currentAnim, HashedString.op_Implicit(smi.AnimController.defaultAnim)) && Object.op_Inequality((Object) this.Mail.Get(smi), (Object) null))).Update(new System.Action<LonelyMinion.Instance, float>(this.DelayIdle), (UpdateRate) 3).EventHandler(GameHashes.AnimQueueComplete, new StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback(this.OnIdleAnimComplete)).Exit(new StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback(this.OnIdleAnimComplete));
    this.CheckMail.Enter(new StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback(LonelyMinion.MailStates.OnEnter)).EventTransition(GameHashes.AnimQueueComplete, this.Idle, new StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.Transition.ConditionCallback(this.HahCheckedMail)).Exit(new StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback(LonelyMinion.MailStates.OnExit));
    this.CheckMail.Success.Enter((StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback) (smi => LonelyMinion.MailStates.PlayAnims(smi, LonelyMinionConfig.FOOD_SUCCESS))).EventHandler(GameHashes.AnimQueueComplete, (StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback) (smi => LonelyMinion.MailStates.PlayAnims(smi, LonelyMinionConfig.CHECK_MAIL_SUCCESS)));
    this.CheckMail.Failure.Enter((StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback) (smi => LonelyMinion.MailStates.PlayAnims(smi, LonelyMinionConfig.FOOD_FAILURE))).EventHandler(GameHashes.AnimQueueComplete, (StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback) (smi => LonelyMinion.MailStates.PlayAnims(smi, LonelyMinionConfig.CHECK_MAIL_FAILURE)));
    this.CheckMail.Duplicate.Enter((StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback) (smi => LonelyMinion.MailStates.PlayAnims(smi, LonelyMinionConfig.FOOD_DUPLICATE))).EventHandler(GameHashes.AnimQueueComplete, (StateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State.Callback) (smi => LonelyMinion.MailStates.PlayAnims(smi, LonelyMinionConfig.CHECK_MAIL_DUPLICATE)));
  }

  public class Def : StateMachine.BaseDef
  {
    public Personality Personality;
    public HashedString QuestOwnerId;
    public Extents DecorInspectionArea;
  }

  public new class Instance : 
    GameStateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.GameInstance
  {
    public float IdleDelayTimer;
    private KBatchedAnimController[] animControllers;
    private Storage storage;
    private const int maxIdles = 8;
    private List<HashedString> availableIdles = new List<HashedString>(8);
    public float StartingAverageDecor = float.NegativeInfinity;

    public KBatchedAnimController AnimController => this.animControllers[0];

    public KBatchedAnimController PackageSnapPoint => this.animControllers[1];

    public Instance(StateMachineController master, LonelyMinion.Def def)
      : base(master, def)
    {
      this.animControllers = this.gameObject.GetComponentsInChildren<KBatchedAnimController>(true);
      this.storage = this.GetComponent<Storage>();
      Debug.Assert(def.Personality != null);
      Accessorizer component = this.GetComponent<Accessorizer>();
      component.ApplyMinionPersonality(def.Personality);
      LonelyMinionConfig.ApplyAccessoryOverrides(component);
      StoryManager.Instance.GetStoryInstance(Db.Get().Stories.LonelyMinion.HashId).StoryStateChanged += new System.Action<StoryInstance.State>(this.OnStoryStateChanged);
    }

    public override void StartSM()
    {
      LonelyMinionHouse.Instance smi = ((Component) this.smi.transform.parent).GetSMI<LonelyMinionHouse.Instance>();
      this.smi.AnimController.GetSynchronizer().Add((KAnimControllerBase) smi.AnimController);
      QuestManager.GetInstance(this.def.QuestOwnerId, Db.Get().Quests.LonelyMinionGreetingQuest).QuestProgressChanged += new System.Action<QuestInstance, Quest.State, float>(this.ShowQuestCompleteNotification);
      this.smi.IdleDelayTimer = Random.Range(20f, 40f);
      this.InitializeIdles();
      base.StartSM();
    }

    public override void StopSM(string reason)
    {
      QuestManager.GetInstance(this.def.QuestOwnerId, Db.Get().Quests.LonelyMinionGreetingQuest).QuestProgressChanged -= new System.Action<QuestInstance, Quest.State, float>(this.ShowQuestCompleteNotification);
      this.StoryCleanUp();
      base.StopSM(reason);
    }

    public HashedString ChooseIdle()
    {
      if (this.availableIdles.Count > 1)
        Util.Shuffle<HashedString>((IList<HashedString>) this.availableIdles);
      return this.availableIdles[0];
    }

    public void Pickup(Pickupable pickupable, bool store)
    {
      ((Component) pickupable.storage).GetComponent<SingleEntityReceptacle>().OrderRemoveOccupant();
      this.PackageSnapPoint.Play(HashedString.op_Implicit("object"), (KAnim.PlayMode) 0);
      if (store)
        this.storage.Store(((Component) pickupable).gameObject, true, true, false);
      else
        Object.Destroy((Object) ((Component) pickupable).gameObject);
    }

    public void Drop() => this.storage.DropAll(((Component) this.PackageSnapPoint).transform.position, offset: new Vector3());

    private void OnStoryStateChanged(StoryInstance.State state)
    {
      if (state != StoryInstance.State.COMPLETE)
        return;
      this.StoryCleanUp();
    }

    private void StoryCleanUp()
    {
      this.AnimController.GetSynchronizer().Clear();
      StoryManager.Instance.GetStoryInstance(Db.Get().Stories.LonelyMinion.HashId).StoryStateChanged -= new System.Action<StoryInstance.State>(this.OnStoryStateChanged);
    }

    private void InitializeIdles()
    {
      QuestInstance instance = QuestManager.GetInstance(this.def.QuestOwnerId, Db.Get().Quests.LonelyMinionFoodQuest);
      if (instance.IsStarted)
      {
        this.availableIdles.Add(LonelyMinionConfig.FOOD_IDLE);
        if (!instance.IsComplete)
          this.availableIdles.Add(LonelyMinionConfig.CHECK_MAIL);
      }
      if (QuestManager.GetInstance(this.def.QuestOwnerId, Db.Get().Quests.LonelyMinionDecorQuest).IsStarted)
        this.availableIdles.Add(LonelyMinionConfig.DECOR_IDLE);
      if (QuestManager.GetInstance(this.def.QuestOwnerId, Db.Get().Quests.LonelyMinionPowerQuest).IsStarted)
        this.availableIdles.Add(LonelyMinionConfig.POWER_IDLE);
      LonelyMinionHouse.Instance smi = ((Component) this.transform.parent).GetSMI<LonelyMinionHouse.Instance>();
      float num1 = 3f * (smi.GetStateMachine() as LonelyMinionHouse).QuestProgress.Get(smi);
      int num2 = Mathf.Approximately((float) Mathf.CeilToInt(num1), num1) ? Mathf.CeilToInt(num1) : Mathf.FloorToInt(num1);
      if (num2 == 0)
      {
        this.availableIdles.Add(LonelyMinionConfig.BLINDS_IDLE_0);
      }
      else
      {
        for (int index = 1; index <= num2 && index != 3; ++index)
          this.availableIdles.Add(HashedString.op_Implicit(string.Format("{0}_{1}", (object) "idle_blinds", (object) index)));
      }
    }

    public void UnlockQuestIdle(QuestInstance quest, Quest.State prevState, float delta)
    {
      if ((prevState != Quest.State.NotStarted ? 0 : (quest.IsStarted ? 1 : 0)) != 0)
      {
        if (HashedString.op_Equality(quest.Id, Db.Get().Quests.LonelyMinionFoodQuest.IdHash))
          this.availableIdles.Add(LonelyMinionConfig.FOOD_IDLE);
        else if (HashedString.op_Equality(quest.Id, Db.Get().Quests.LonelyMinionDecorQuest.IdHash))
          this.availableIdles.Add(LonelyMinionConfig.DECOR_IDLE);
        else
          this.availableIdles.Add(LonelyMinionConfig.POWER_IDLE);
      }
      if (!quest.IsComplete)
        return;
      if (HashedString.op_Equality(quest.Id, Db.Get().Quests.LonelyMinionFoodQuest.IdHash))
        this.availableIdles.Remove(LonelyMinionConfig.CHECK_MAIL);
      LonelyMinionHouse.Instance smi = ((Component) this.transform.parent).GetSMI<LonelyMinionHouse.Instance>();
      float num1 = 3f * (smi.GetStateMachine() as LonelyMinionHouse).QuestProgress.Get(smi);
      int num2 = Mathf.Approximately((float) Mathf.CeilToInt(num1), num1) ? Mathf.CeilToInt(num1) : Mathf.FloorToInt(num1);
      if (num2 > 0 && num2 < 3)
        this.availableIdles.Add(HashedString.op_Implicit(string.Format("{0}_{1}", (object) "idle_blinds", (object) num2)));
      this.availableIdles.Remove(LonelyMinionConfig.BLINDS_IDLE_0);
    }

    public void ShowQuestCompleteNotification(
      QuestInstance quest,
      Quest.State prevState,
      float delta = 0.0f)
    {
      if (!quest.IsComplete)
        return;
      string unlockID = string.Empty;
      if (HashedString.op_Inequality(quest.Id, Db.Get().Quests.LonelyMinionGreetingQuest.IdHash))
      {
        unlockID = "story_trait_lonelyminion_" + quest.Name.ToLower();
        Game.Instance.unlocks.Unlock(unlockID, false);
      }
      Notification notification = new Notification((string) CODEX.STORY_TRAITS.LONELYMINION.QUESTCOMPLETE_POPUP.NAME, NotificationType.Event, expires: false, custom_click_callback: new Notification.ClickCallback(this.ShowQuestCompletePopup), custom_click_data: ((object) new Tuple<string, string>(unlockID, quest.CompletionText)), clear_on_click: true, show_dismiss_button: true);
      ((Component) this.transform.parent).gameObject.AddOrGet<Notifier>().Add(notification);
    }

    private void ShowQuestCompletePopup(object data)
    {
      Tuple<string, string> tuple = data as Tuple<string, string>;
      InfoDialogScreen infoDialogScreen = LoreBearer.ShowPopupDialog().SetHeader((string) CODEX.STORY_TRAITS.LONELYMINION.QUESTCOMPLETE_POPUP.NAME).AddPlainText(tuple.second).AddDefaultOK();
      if (string.IsNullOrEmpty(tuple.first))
        return;
      infoDialogScreen.AddOption((string) CODEX.STORY_TRAITS.LONELYMINION.QUESTCOMPLETE_POPUP.VIEW_IN_CODEX, LoreBearerUtil.OpenCodexByLockKeyID(tuple.first, true));
    }
  }

  public class MailStates : 
    GameStateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State
  {
    public GameStateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State Success;
    public GameStateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State Failure;
    public GameStateMachine<LonelyMinion, LonelyMinion.Instance, StateMachineController, LonelyMinion.Def>.State Duplicate;

    public static void OnEnter(LonelyMinion.Instance smi)
    {
      KBatchedAnimController component = smi.sm.Mail.Get(smi).GetComponent<KBatchedAnimController>();
      ((Component) smi.PackageSnapPoint).gameObject.SetActive(Object.op_Inequality((Object) ((Component) component).gameObject, (Object) smi.gameObject));
      if (((Component) smi.PackageSnapPoint).gameObject.activeSelf)
        smi.PackageSnapPoint.SwapAnims(component.AnimFiles);
      smi.AnimController.Play(LonelyMinionConfig.CHECK_MAIL);
    }

    public static void OnExit(LonelyMinion.Instance smi) => smi.ScheduleNextFrame(new System.Action<object>(LonelyMinion.MailStates.ResetState), (object) smi);

    private static void ResetState(object data)
    {
      LonelyMinion.Instance instance = data as LonelyMinion.Instance;
      instance.AnimController.Play(HashedString.op_Implicit(instance.AnimController.initialAnim), instance.AnimController.initialMode);
      instance.Drop();
    }

    public static void PlayAnims(LonelyMinion.Instance smi, HashedString anim)
    {
      if (((HashedString) ref anim).IsValid)
        smi.AnimController.Queue(anim);
      else
        smi.GoTo((StateMachine.BaseState) smi.sm.Idle);
    }
  }
}
