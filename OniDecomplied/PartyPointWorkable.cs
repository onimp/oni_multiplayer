// Decompiled with JetBrains decompiler
// Type: PartyPointWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using TUNING;
using UnityEngine;

public class PartyPointWorkable : Workable, IWorkerPrioritizable
{
  private GameObject lastTalker;
  public int basePriority;
  public string specificEffect;
  public KAnimFile[][] workerOverrideAnims;
  private PartyPointWorkable.ActivityType activity;

  private PartyPointWorkable() => this.SetReportType(ReportManager.ReportType.PersonalTime);

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_generic_convo_kanim"))
    };
    this.workAnimPlayMode = (KAnim.PlayMode) 0;
    this.faceTargetWhenWorking = true;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Socializing;
    this.synchronizeAnims = false;
    this.showProgressBar = false;
    this.resetProgressOnStop = true;
    this.lightEfficiencyBonus = false;
    this.activity = (double) Random.Range(0.0f, 100f) <= 80.0 ? PartyPointWorkable.ActivityType.Talk : PartyPointWorkable.ActivityType.Dance;
    switch (this.activity)
    {
      case PartyPointWorkable.ActivityType.Talk:
        this.workAnims = new HashedString[1]
        {
          HashedString.op_Implicit("idle")
        };
        this.workerOverrideAnims = new KAnimFile[1][]
        {
          new KAnimFile[1]
          {
            Assets.GetAnim(HashedString.op_Implicit("anim_generic_convo_kanim"))
          }
        };
        break;
      case PartyPointWorkable.ActivityType.Dance:
        this.workAnims = new HashedString[1]
        {
          HashedString.op_Implicit("working_loop")
        };
        this.workerOverrideAnims = new KAnimFile[3][]
        {
          new KAnimFile[1]
          {
            Assets.GetAnim(HashedString.op_Implicit("anim_interacts_phonobox_danceone_kanim"))
          },
          new KAnimFile[1]
          {
            Assets.GetAnim(HashedString.op_Implicit("anim_interacts_phonobox_dancetwo_kanim"))
          },
          new KAnimFile[1]
          {
            Assets.GetAnim(HashedString.op_Implicit("anim_interacts_phonobox_dancethree_kanim"))
          }
        };
        break;
    }
  }

  public override Workable.AnimInfo GetAnim(Worker worker)
  {
    this.overrideAnims = this.workerOverrideAnims[Random.Range(0, this.workerOverrideAnims.Length)];
    return base.GetAnim(worker);
  }

  public override Vector3 GetFacingTarget() => Object.op_Inequality((Object) this.lastTalker, (Object) null) ? TransformExtensions.GetPosition(this.lastTalker.transform) : base.GetFacingTarget();

  protected override bool OnWorkTick(Worker worker, float dt) => false;

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    ((Component) worker).GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse, false);
    worker.Subscribe(-594200555, new Action<object>(this.OnStartedTalking));
    worker.Subscribe(25860745, new Action<object>(this.OnStoppedTalking));
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
      if (this.activity != PartyPointWorkable.ActivityType.Talk)
        return;
      KBatchedAnimController component = ((Component) this.worker).GetComponent<KBatchedAnimController>();
      component.Play(HashedString.op_Implicit(startedTalkingEvent.anim + Random.Range(1, 9).ToString()));
      component.Queue(HashedString.op_Implicit("idle"), (KAnim.PlayMode) 0);
    }
    else
    {
      if (this.activity == PartyPointWorkable.ActivityType.Talk)
        ((Component) this.worker).GetComponent<Facing>().Face(TransformExtensions.GetPosition(talker.transform));
      this.lastTalker = talker;
    }
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

  private enum ActivityType
  {
    Talk,
    Dance,
    LENGTH,
  }
}
