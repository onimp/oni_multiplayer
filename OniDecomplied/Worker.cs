// Decompiled with JetBrains decompiler
// Type: Worker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Worker")]
public class Worker : KMonoBehaviour
{
  private const float EARLIEST_REACT_TIME = 1f;
  [MyCmpGet]
  private Facing facing;
  [MyCmpGet]
  private MinionResume resume;
  private float workPendingCompletionTime;
  private int onWorkChoreDisabledHandle;
  public object workCompleteData;
  private Workable.AnimInfo animInfo;
  private KAnimSynchronizer kanimSynchronizer;
  private StatusItemGroup.Entry previousStatusItem;
  private StateMachine.Instance smi;
  private bool successFullyCompleted;
  private Vector3 workAnimOffset = Vector3.zero;
  public bool usesMultiTool = true;
  private static readonly EventSystem.IntraObjectHandler<Worker> OnChoreInterruptDelegate = new EventSystem.IntraObjectHandler<Worker>((Action<Worker, object>) ((component, data) => component.OnChoreInterrupt(data)));
  private Reactable passerbyReactable;

  public Worker.State state { get; private set; }

  public Worker.StartWorkInfo startWorkInfo { get; private set; }

  public Workable workable => this.startWorkInfo != null ? this.startWorkInfo.workable : (Workable) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.state = Worker.State.Idle;
    this.Subscribe<Worker>(1485595942, Worker.OnChoreInterruptDelegate);
  }

  private string GetWorkableDebugString() => Object.op_Equality((Object) this.workable, (Object) null) ? "Null" : ((Object) this.workable).name;

  public void CompleteWork()
  {
    this.successFullyCompleted = false;
    this.state = Worker.State.Idle;
    if (Object.op_Inequality((Object) this.workable, (Object) null))
    {
      if (this.workable.triggerWorkReactions && (double) this.workable.GetWorkTime() > 30.0)
      {
        string conversationTopic = this.workable.GetConversationTopic();
        if (!Util.IsNullOrWhiteSpace(conversationTopic))
          this.CreateCompletionReactable(conversationTopic);
      }
      this.DetachAnimOverrides();
      this.workable.CompleteWork(this);
      if (Object.op_Inequality((Object) this.workable.worker, (Object) null) && !(this.workable is Constructable) && !(this.workable is Deconstructable) && !(this.workable is Repairable) && !(this.workable is Disinfectable))
        GameplayEventManager.Instance.Trigger(1175726587, (object) new BonusEvent.GameplayEventData()
        {
          workable = this.workable,
          worker = this.workable.worker,
          building = ((Component) this.workable).GetComponent<BuildingComplete>(),
          eventTrigger = GameHashes.UseBuilding
        });
    }
    this.InternalStopWork(this.workable, false);
  }

  public Worker.WorkResult Work(float dt)
  {
    if (this.state == Worker.State.PendingCompletion)
    {
      bool flag = (double) Time.time - (double) this.workPendingCompletionTime > 10.0;
      if (!(((Component) this).GetComponent<KAnimControllerBase>().IsStopped() | flag))
        return Worker.WorkResult.InProgress;
      Navigator component = ((Component) this).GetComponent<Navigator>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        NavGrid.NavTypeData navTypeData = component.NavGrid.GetNavTypeData(component.CurrentNavType);
        if (((HashedString) ref navTypeData.idleAnim).IsValid)
          ((Component) this).GetComponent<KAnimControllerBase>().Play(navTypeData.idleAnim);
      }
      if (this.successFullyCompleted)
      {
        this.CompleteWork();
        return Worker.WorkResult.Success;
      }
      this.StopWork();
      return Worker.WorkResult.Failed;
    }
    if (this.state == Worker.State.Completing)
    {
      if (this.successFullyCompleted)
      {
        this.CompleteWork();
        return Worker.WorkResult.Success;
      }
      this.StopWork();
      return Worker.WorkResult.Failed;
    }
    if (Object.op_Inequality((Object) this.workable, (Object) null))
    {
      if (Object.op_Implicit((Object) this.facing))
      {
        if (this.workable.ShouldFaceTargetWhenWorking())
        {
          this.facing.Face(this.workable.GetFacingTarget());
        }
        else
        {
          Rotatable component = ((Component) this.workable).GetComponent<Rotatable>();
          bool flag = Object.op_Inequality((Object) component, (Object) null) && component.GetOrientation() == Orientation.FlipH;
          this.facing.Face(Vector3.op_Addition(TransformExtensions.GetPosition(this.facing.transform), flag ? Vector3.left : Vector3.right));
        }
      }
      if ((double) dt > 0.0 && Game.Instance.FastWorkersModeActive)
        dt = Mathf.Min(this.workable.WorkTimeRemaining + 0.01f, 5f);
      Klei.AI.Attribute workAttribute = this.workable.GetWorkAttribute();
      AttributeLevels component1 = ((Component) this).GetComponent<AttributeLevels>();
      if (workAttribute != null && workAttribute.IsTrainable && Object.op_Inequality((Object) component1, (Object) null))
      {
        float experienceMultiplier = this.workable.GetAttributeExperienceMultiplier();
        component1.AddExperience(workAttribute.Id, dt, experienceMultiplier);
      }
      string experienceSkillGroup = this.workable.GetSkillExperienceSkillGroup();
      if (Object.op_Inequality((Object) this.resume, (Object) null) && experienceSkillGroup != null)
      {
        float experienceMultiplier = this.workable.GetSkillExperienceMultiplier();
        this.resume.AddExperienceWithAptitude(experienceSkillGroup, dt, experienceMultiplier);
      }
      float efficiencyMultiplier = this.workable.GetEfficiencyMultiplier(this);
      if (this.workable.WorkTick(this, (float) ((double) dt * (double) efficiencyMultiplier * 1.0)) && this.state == Worker.State.Working)
      {
        this.successFullyCompleted = true;
        this.StartPlayingPostAnim();
        this.workable.OnPendingCompleteWork(this);
      }
    }
    return Worker.WorkResult.InProgress;
  }

  private void StartPlayingPostAnim()
  {
    if (Object.op_Inequality((Object) this.workable, (Object) null) && !this.workable.alwaysShowProgressBar)
      this.workable.ShowProgressBar(false);
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption, false);
    this.state = Worker.State.PendingCompletion;
    this.workPendingCompletionTime = Time.time;
    KAnimControllerBase component1 = ((Component) this).GetComponent<KAnimControllerBase>();
    HashedString[] workPstAnims = this.workable.GetWorkPstAnims(this, this.successFullyCompleted);
    if (this.smi == null)
    {
      if (workPstAnims != null && workPstAnims.Length != 0)
      {
        if (Object.op_Inequality((Object) this.workable, (Object) null) && this.workable.synchronizeAnims)
        {
          KAnimControllerBase component2 = ((Component) this.workable).GetComponent<KAnimControllerBase>();
          if (Object.op_Inequality((Object) component2, (Object) null))
            component2.Play(workPstAnims);
        }
        else
          component1.Play(workPstAnims);
      }
      else
        this.state = Worker.State.Completing;
    }
    this.Trigger(-1142962013, (object) this);
  }

  private void InternalStopWork(Workable target_workable, bool is_aborted)
  {
    this.state = Worker.State.Idle;
    ((Component) this).gameObject.RemoveTag(GameTags.PerformingWorkRequest);
    KAnimControllerBase component1 = ((Component) this).GetComponent<KAnimControllerBase>();
    component1.Offset = Vector3.op_Subtraction(component1.Offset, this.workAnimOffset);
    this.workAnimOffset = Vector3.zero;
    ((Component) this).GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
    this.DetachAnimOverrides();
    this.ClearPasserbyReactable();
    AnimEventHandler component2 = ((Component) this).GetComponent<AnimEventHandler>();
    if (Object.op_Implicit((Object) component2))
      component2.ClearContext();
    if (this.previousStatusItem.item != null)
      ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, this.previousStatusItem.item, this.previousStatusItem.data);
    if (Object.op_Inequality((Object) target_workable, (Object) null))
    {
      target_workable.Unsubscribe(this.onWorkChoreDisabledHandle);
      target_workable.StopWork(this, is_aborted);
    }
    if (this.smi != null)
    {
      this.smi.StopSM("stopping work");
      this.smi = (StateMachine.Instance) null;
    }
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    position.z = Grid.GetLayerZ(Grid.SceneLayer.Move);
    TransformExtensions.SetPosition(this.transform, position);
    this.startWorkInfo = (Worker.StartWorkInfo) null;
  }

  private void OnChoreInterrupt(object data)
  {
    if (this.state != Worker.State.Working)
      return;
    this.successFullyCompleted = false;
    this.StartPlayingPostAnim();
  }

  private void OnWorkChoreDisabled(object data)
  {
    string str = data as string;
    ChoreConsumer component = ((Component) this).GetComponent<ChoreConsumer>();
    if (!Object.op_Inequality((Object) component, (Object) null) || !Object.op_Inequality((Object) component.choreDriver, (Object) null))
      return;
    component.choreDriver.GetCurrentChore().Fail(str ?? "WorkChoreDisabled");
  }

  public void StopWork()
  {
    if (this.state == Worker.State.PendingCompletion || this.state == Worker.State.Completing)
    {
      this.state = Worker.State.Idle;
      if (this.successFullyCompleted)
        this.CompleteWork();
      else
        this.InternalStopWork(this.workable, true);
    }
    else
    {
      if (this.state != Worker.State.Working)
        return;
      if (Object.op_Inequality((Object) this.workable, (Object) null) && this.workable.synchronizeAnims)
      {
        KBatchedAnimController component = ((Component) this.workable).GetComponent<KBatchedAnimController>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          HashedString[] workPstAnims = this.workable.GetWorkPstAnims(this, false);
          if (workPstAnims != null && workPstAnims.Length != 0)
          {
            component.Play(workPstAnims);
            component.SetPositionPercent(1f);
          }
        }
      }
      this.InternalStopWork(this.workable, true);
    }
  }

  public void StartWork(Worker.StartWorkInfo start_work_info)
  {
    this.startWorkInfo = start_work_info;
    Game.Instance.StartedWork();
    if (this.state != Worker.State.Idle)
    {
      string str = "";
      if (Object.op_Inequality((Object) this.workable, (Object) null))
        str = ((Object) this.workable).name;
      Debug.LogError((object) (((Object) this).name + "." + str + ".state should be idle but instead it's:" + this.state.ToString()));
    }
    string name = ((object) this.workable).GetType().Name;
    try
    {
      ((Component) this).gameObject.AddTag(GameTags.PerformingWorkRequest);
      this.state = Worker.State.Working;
      EventExtensions.Trigger(((Component) this).gameObject, 1568504979, (object) this);
      if (Object.op_Inequality((Object) this.workable, (Object) null))
      {
        this.animInfo = this.workable.GetAnim(this);
        if (this.animInfo.smi != null)
        {
          this.smi = this.animInfo.smi;
          this.smi.StartSM();
        }
        Vector3 position = TransformExtensions.GetPosition(this.transform);
        position.z = Grid.GetLayerZ(this.workable.workLayer);
        TransformExtensions.SetPosition(this.transform, position);
        KAnimControllerBase component1 = ((Component) this).GetComponent<KAnimControllerBase>();
        if (this.animInfo.smi == null)
          this.AttachOverrideAnims(component1);
        HashedString[] workAnims = this.workable.GetWorkAnims(this);
        KAnim.PlayMode workAnimPlayMode = this.workable.GetWorkAnimPlayMode();
        Vector3 workOffset = this.workable.GetWorkOffset();
        this.workAnimOffset = workOffset;
        KAnimControllerBase kanimControllerBase = component1;
        kanimControllerBase.Offset = Vector3.op_Addition(kanimControllerBase.Offset, workOffset);
        if (this.usesMultiTool && this.animInfo.smi == null && workAnims != null && workAnims.Length != 0 && Object.op_Inequality((Object) this.resume, (Object) null))
        {
          if (this.workable.synchronizeAnims)
          {
            KAnimControllerBase component2 = ((Component) this.workable).GetComponent<KAnimControllerBase>();
            if (Object.op_Inequality((Object) component2, (Object) null))
            {
              this.kanimSynchronizer = component2.GetSynchronizer();
              if (this.kanimSynchronizer != null)
                this.kanimSynchronizer.Add(component1);
            }
            component2.Play(workAnims, workAnimPlayMode);
          }
          else
            component1.Play(workAnims, workAnimPlayMode);
        }
      }
      this.workable.StartWork(this);
      if (Object.op_Equality((Object) this.workable, (Object) null))
      {
        Debug.LogWarning((object) "Stopped work as soon as I started. This is usually a sign that a chore is open when it shouldn't be or that it's preconditions are wrong.");
      }
      else
      {
        this.onWorkChoreDisabledHandle = this.workable.Subscribe(2108245096, new Action<object>(this.OnWorkChoreDisabled));
        if (this.workable.triggerWorkReactions && (double) this.workable.WorkTimeRemaining > 10.0)
          this.CreatePasserbyReactable();
        KSelectable component = ((Component) this).GetComponent<KSelectable>();
        this.previousStatusItem = component.GetStatusItem(Db.Get().StatusItemCategories.Main);
        component.SetStatusItem(Db.Get().StatusItemCategories.Main, this.workable.GetWorkerStatusItem(), (object) this.workable);
      }
    }
    catch (Exception ex)
    {
      DebugUtil.LogErrorArgs((Object) this, new object[1]
      {
        (object) ("Exception in: Worker.StartWork(" + name + ")" + "\n" + ex.ToString())
      });
      throw;
    }
  }

  private void Update()
  {
    if (this.state != Worker.State.Working)
      return;
    this.ForceSyncAnims();
  }

  private void ForceSyncAnims()
  {
    if ((double) Time.deltaTime <= 0.0 || this.kanimSynchronizer == null)
      return;
    this.kanimSynchronizer.SyncTime();
  }

  public bool InstantlyFinish() => Object.op_Inequality((Object) this.workable, (Object) null) && this.workable.InstantlyFinish(this);

  private void AttachOverrideAnims(KAnimControllerBase worker_controller)
  {
    if (this.animInfo.overrideAnims == null || this.animInfo.overrideAnims.Length == 0)
      return;
    for (int index = 0; index < this.animInfo.overrideAnims.Length; ++index)
      worker_controller.AddAnimOverrides(this.animInfo.overrideAnims[index]);
  }

  private void DetachAnimOverrides()
  {
    KAnimControllerBase component = ((Component) this).GetComponent<KAnimControllerBase>();
    if (this.kanimSynchronizer != null)
    {
      this.kanimSynchronizer.Remove(component);
      this.kanimSynchronizer = (KAnimSynchronizer) null;
    }
    if (this.animInfo.overrideAnims == null)
      return;
    for (int index = 0; index < this.animInfo.overrideAnims.Length; ++index)
      component.RemoveAnimOverrides(this.animInfo.overrideAnims[index]);
    this.animInfo.overrideAnims = (KAnimFile[]) null;
  }

  private void CreateCompletionReactable(string topic)
  {
    if ((double) GameClock.Instance.GetTime() / 600.0 < 1.0)
      return;
    EmoteReactable oneshotReactable = OneshotReactableLocator.CreateOneshotReactable(((Component) this).gameObject, 3f, "WorkCompleteAcknowledgement", Db.Get().ChoreTypes.Emote, 9, 5, 100f);
    Emote clapCheer = Db.Get().Emotes.Minion.ClapCheer;
    oneshotReactable.SetEmote(clapCheer);
    oneshotReactable.RegisterEmoteStepCallbacks(HashedString.op_Implicit("clapcheer_pre"), new Action<GameObject>(this.GetReactionEffect), (Action<GameObject>) null).RegisterEmoteStepCallbacks(HashedString.op_Implicit("clapcheer_pst"), (Action<GameObject>) null, (Action<GameObject>) (r => EventExtensions.Trigger(r, 937885943, (object) topic)));
    Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) topic, centered: true);
    if (uiSprite == null)
      return;
    Thought thought = new Thought("Completion_" + topic, (ResourceSet) null, uiSprite.first, "mode_satisfaction", "conversation_short", "bubble_conversation", SpeechMonitor.PREFIX_HAPPY, (LocString) "", true);
    oneshotReactable.SetThought(thought);
  }

  public void CreatePasserbyReactable()
  {
    if ((double) GameClock.Instance.GetTime() / 600.0 < 1.0 || this.passerbyReactable != null)
      return;
    EmoteReactable emoteReactable = new EmoteReactable(((Component) this).gameObject, HashedString.op_Implicit("WorkPasserbyAcknowledgement"), Db.Get().ChoreTypes.Emote, 5, 5, 30f, 720f * TuningData<DupeGreetingManager.Tuning>.Get().greetingDelayMultiplier);
    Emote thumbsUp = Db.Get().Emotes.Minion.ThumbsUp;
    emoteReactable.SetEmote(thumbsUp).SetThought(Db.Get().Thoughts.Encourage).AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsOnFloor)).AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsFacingMe)).AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsntPartying));
    emoteReactable.RegisterEmoteStepCallbacks(HashedString.op_Implicit("react"), new Action<GameObject>(this.GetReactionEffect), (Action<GameObject>) null);
    this.passerbyReactable = (Reactable) emoteReactable;
  }

  private void GetReactionEffect(GameObject reactor) => ((Component) this).GetComponent<Effects>().Add("WorkEncouraged", true);

  private bool ReactorIsOnFloor(GameObject reactor, Navigator.ActiveTransition transition) => transition.end == NavType.Floor;

  private bool ReactorIsFacingMe(GameObject reactor, Navigator.ActiveTransition transition)
  {
    Facing component = reactor.GetComponent<Facing>();
    return (double) TransformExtensions.GetPosition(this.transform).x < (double) TransformExtensions.GetPosition(reactor.transform).x == component.GetFacing();
  }

  private bool ReactorIsntPartying(GameObject reactor, Navigator.ActiveTransition transition)
  {
    ChoreConsumer component = reactor.GetComponent<ChoreConsumer>();
    return component.choreDriver.HasChore() && component.choreDriver.GetCurrentChore().choreType != Db.Get().ChoreTypes.Party;
  }

  public void ClearPasserbyReactable()
  {
    if (this.passerbyReactable == null)
      return;
    this.passerbyReactable.Cleanup();
    this.passerbyReactable = (Reactable) null;
  }

  public enum State
  {
    Idle,
    Working,
    PendingCompletion,
    Completing,
  }

  public class StartWorkInfo
  {
    public Workable workable { get; set; }

    public StartWorkInfo(Workable workable) => this.workable = workable;
  }

  public enum WorkResult
  {
    Success,
    InProgress,
    Failed,
  }
}
