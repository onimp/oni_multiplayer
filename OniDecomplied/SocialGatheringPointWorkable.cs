// Decompiled with JetBrains decompiler
// Type: SocialGatheringPointWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/SocialGatheringPointWorkable")]
public class SocialGatheringPointWorkable : Workable, IWorkerPrioritizable
{
  private GameObject lastTalker;
  public int basePriority;
  public string specificEffect;
  public int timesConversed;

  private SocialGatheringPointWorkable() => this.SetReportType(ReportManager.ReportType.PersonalTime);

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_generic_convo_kanim"))
    };
    this.workAnims = new HashedString[1]
    {
      HashedString.op_Implicit("idle")
    };
    this.faceTargetWhenWorking = true;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Socializing;
    this.synchronizeAnims = false;
    this.showProgressBar = false;
    this.resetProgressOnStop = true;
    this.lightEfficiencyBonus = false;
  }

  public override Vector3 GetFacingTarget() => Object.op_Inequality((Object) this.lastTalker, (Object) null) ? TransformExtensions.GetPosition(this.lastTalker.transform) : base.GetFacingTarget();

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if (!((Component) worker).GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation))
    {
      Effects component = ((Component) worker).GetComponent<Effects>();
      if (string.IsNullOrEmpty(this.specificEffect) || component.HasEffect(this.specificEffect))
        return true;
    }
    return false;
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    ((Component) worker).GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse, false);
    worker.Subscribe(-594200555, new Action<object>(this.OnStartedTalking));
    worker.Subscribe(25860745, new Action<object>(this.OnStoppedTalking));
    this.timesConversed = 0;
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    ((Component) worker).GetComponent<KPrefabID>().RemoveTag(GameTags.AlwaysConverse);
    worker.Unsubscribe(-594200555, new Action<object>(this.OnStartedTalking));
    worker.Unsubscribe(25860745, new Action<object>(this.OnStoppedTalking));
  }

  protected override void OnCompleteWork(Worker worker)
  {
    if (this.timesConversed <= 0)
      return;
    Effects component = ((Component) worker).GetComponent<Effects>();
    if (string.IsNullOrEmpty(this.specificEffect))
      return;
    component.Add(this.specificEffect, true);
  }

  private void OnStartedTalking(object data)
  {
    if (!(data is ConversationManager.StartedTalkingEvent startedTalkingEvent))
      return;
    GameObject talker = startedTalkingEvent.talker;
    if (Object.op_Equality((Object) talker, (Object) ((Component) this.worker).gameObject))
    {
      KBatchedAnimController component = ((Component) this.worker).GetComponent<KBatchedAnimController>();
      component.Play(HashedString.op_Implicit(startedTalkingEvent.anim + Random.Range(1, 9).ToString()));
      component.Queue(HashedString.op_Implicit("idle"), (KAnim.PlayMode) 0);
    }
    else
    {
      ((Component) this.worker).GetComponent<Facing>().Face(TransformExtensions.GetPosition(talker.transform));
      this.lastTalker = talker;
    }
    ++this.timesConversed;
  }

  private void OnStoppedTalking(object data)
  {
  }

  public bool GetWorkerPriority(Worker worker, out int priority)
  {
    priority = this.basePriority;
    if (!string.IsNullOrEmpty(this.specificEffect) && ((Component) worker).GetComponent<Effects>().HasEffect(this.specificEffect))
      priority = RELAXATION.PRIORITY.RECENTLY_USED;
    return true;
  }
}
