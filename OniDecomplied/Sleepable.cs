// Decompiled with JetBrains decompiler
// Type: Sleepable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Sleepable")]
public class Sleepable : Workable
{
  private const float STRECH_CHANCE = 0.33f;
  [MyCmpGet]
  private Operational operational;
  public string effectName = "Sleep";
  public List<string> wakeEffects;
  public bool stretchOnWake = true;
  private float wakeTime;
  private bool isDoneSleeping;
  public ClinicDreamable Dreamable;
  private static readonly HashedString[] normalWorkAnims = new HashedString[2]
  {
    HashedString.op_Implicit("working_pre"),
    HashedString.op_Implicit("working_loop")
  };
  private static readonly HashedString[] hatWorkAnims = new HashedString[2]
  {
    HashedString.op_Implicit("hat_pre"),
    HashedString.op_Implicit("working_loop")
  };
  private static readonly HashedString[] normalWorkPstAnim = new HashedString[1]
  {
    HashedString.op_Implicit("working_pst")
  };
  private static readonly HashedString[] hatWorkPstAnim = new HashedString[1]
  {
    HashedString.op_Implicit("hat_pst")
  };

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetReportType(ReportManager.ReportType.PersonalTime);
    this.showProgressBar = false;
    this.workerStatusItem = (StatusItem) null;
    this.synchronizeAnims = false;
    this.triggerWorkReactions = false;
    this.lightEfficiencyBonus = false;
  }

  protected override void OnSpawn()
  {
    Components.Sleepables.Add(this);
    this.SetWorkTime(float.PositiveInfinity);
  }

  public override HashedString[] GetWorkAnims(Worker worker)
  {
    MinionResume component = ((Component) worker).GetComponent<MinionResume>();
    return Object.op_Inequality((Object) ((Component) this).GetComponent<Building>(), (Object) null) && Object.op_Inequality((Object) component, (Object) null) && component.CurrentHat != null ? Sleepable.hatWorkAnims : Sleepable.normalWorkAnims;
  }

  public override HashedString[] GetWorkPstAnims(Worker worker, bool successfully_completed)
  {
    MinionResume component = ((Component) worker).GetComponent<MinionResume>();
    return Object.op_Inequality((Object) ((Component) this).GetComponent<Building>(), (Object) null) && Object.op_Inequality((Object) component, (Object) null) && component.CurrentHat != null ? Sleepable.hatWorkPstAnim : Sleepable.normalWorkPstAnim;
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    KAnimControllerBase component = ((Component) this).GetComponent<KAnimControllerBase>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      component.Play(HashedString.op_Implicit("working_pre"));
      component.Queue(HashedString.op_Implicit("working_loop"), (KAnim.PlayMode) 0);
    }
    this.Subscribe(((Component) worker).gameObject, -1142962013, new Action<object>(this.PlayPstAnim));
    if (Object.op_Inequality((Object) this.operational, (Object) null))
      this.operational.SetActive(true);
    worker.Trigger(-1283701846, (object) this);
    ((Component) worker).GetComponent<Effects>().Add(this.effectName, false);
    this.isDoneSleeping = false;
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if (this.isDoneSleeping)
      return (double) Time.time > (double) this.wakeTime;
    if (Object.op_Inequality((Object) this.Dreamable, (Object) null) && !this.Dreamable.DreamIsDisturbed)
      this.Dreamable.WorkTick(worker, dt);
    if (((Component) worker).GetSMI<StaminaMonitor.Instance>().ShouldExitSleep())
    {
      this.isDoneSleeping = true;
      this.wakeTime = Time.time + Random.value * 3f;
    }
    return false;
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    if (Object.op_Inequality((Object) this.operational, (Object) null))
      this.operational.SetActive(false);
    this.Unsubscribe(((Component) worker).gameObject, -1142962013, new Action<object>(this.PlayPstAnim));
    if (!Object.op_Inequality((Object) worker, (Object) null))
      return;
    Effects component = ((Component) worker).GetComponent<Effects>();
    component.Remove(this.effectName);
    if (this.wakeEffects != null)
    {
      foreach (string wakeEffect in this.wakeEffects)
        component.Add(wakeEffect, true);
    }
    if (this.stretchOnWake && (double) Random.value < 0.33000001311302185)
    {
      EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) ((Component) worker).GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, Db.Get().Emotes.Minion.MorningStretch);
    }
    if ((double) worker.GetAmounts().Get(Db.Get().Amounts.Stamina).value >= (double) worker.GetAmounts().Get(Db.Get().Amounts.Stamina).GetMax())
      return;
    worker.Trigger(1338475637, (object) this);
  }

  public override bool InstantlyFinish(Worker worker) => false;

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.Sleepables.Remove(this);
  }

  private void PlayPstAnim(object data)
  {
    Worker worker = (Worker) data;
    if (!Object.op_Inequality((Object) worker, (Object) null) || !Object.op_Inequality((Object) worker.workable, (Object) null))
      return;
    KAnimControllerBase component = ((Component) worker.workable).gameObject.GetComponent<KAnimControllerBase>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.Play(HashedString.op_Implicit("working_pst"));
  }
}
