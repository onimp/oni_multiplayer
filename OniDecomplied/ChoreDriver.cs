// Decompiled with JetBrains decompiler
// Type: ChoreDriver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class ChoreDriver : StateMachineComponent<ChoreDriver.StatesInstance>
{
  [MyCmpAdd]
  private User user;
  private Chore.Precondition.Context context;

  public Chore GetCurrentChore() => this.smi.GetCurrentChore();

  public bool HasChore() => this.smi.GetCurrentChore() != null;

  public void StopChore() => this.smi.sm.stop.Trigger(this.smi);

  public void SetChore(Chore.Precondition.Context context)
  {
    Chore currentChore = this.smi.GetCurrentChore();
    if (currentChore == context.chore)
      return;
    this.StopChore();
    if (context.chore.IsValid())
    {
      context.chore.PrepareChore(ref context);
      this.context = context;
      this.smi.sm.nextChore.Set(context.chore, this.smi);
    }
    else
    {
      string str1 = "Null";
      string str2 = "Null";
      if (currentChore != null)
        str1 = currentChore.GetType().Name;
      if (context.chore != null)
        str2 = context.chore.GetType().Name;
      Debug.LogWarning((object) ("Stopping chore " + str1 + " to start " + str2 + " but stopping the first chore cancelled the second one."));
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public class StatesInstance : 
    GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.GameInstance
  {
    private ChoreConsumer choreConsumer;

    public string masterProperName { get; private set; }

    public KPrefabID masterPrefabId { get; private set; }

    public Navigator navigator { get; private set; }

    public Worker worker { get; private set; }

    public StatesInstance(ChoreDriver master)
      : base(master)
    {
      this.masterProperName = ((Component) this.master).GetProperName();
      this.masterPrefabId = ((Component) this.master).GetComponent<KPrefabID>();
      this.navigator = ((Component) this.master).GetComponent<Navigator>();
      this.worker = ((Component) this.master).GetComponent<Worker>();
      this.choreConsumer = this.GetComponent<ChoreConsumer>();
      this.choreConsumer.choreRulesChanged += new System.Action(this.OnChoreRulesChanged);
    }

    public void BeginChore()
    {
      Chore data = this.smi.sm.currentChore.Set(this.GetNextChore(), this.smi);
      if (data != null && data.IsPreemptable && Object.op_Inequality((Object) data.driver, (Object) null))
        data.Fail("Preemption!");
      this.smi.sm.nextChore.Set((Chore) null, this.smi);
      data.onExit += new Action<Chore>(this.OnChoreExit);
      data.Begin(this.master.context);
      this.Trigger(-1988963660, (object) data);
    }

    public void EndChore(string reason)
    {
      if (this.GetCurrentChore() == null)
        return;
      Chore currentChore = this.GetCurrentChore();
      this.smi.sm.currentChore.Set((Chore) null, this.smi);
      currentChore.onExit -= new Action<Chore>(this.OnChoreExit);
      currentChore.Fail(reason);
      this.Trigger(1745615042, (object) currentChore);
    }

    private void OnChoreExit(Chore chore) => this.smi.sm.stop.Trigger(this.smi);

    public Chore GetNextChore() => this.smi.sm.nextChore.Get(this.smi);

    public Chore GetCurrentChore() => this.smi.sm.currentChore.Get(this.smi);

    private void OnChoreRulesChanged()
    {
      Chore currentChore = this.GetCurrentChore();
      if (currentChore == null || this.choreConsumer.IsPermittedOrEnabled(currentChore.choreType, currentChore))
        return;
      this.EndChore("Permissions changed");
    }
  }

  public class States : GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver>
  {
    public StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.ObjectParameter<Chore> currentChore;
    public StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.ObjectParameter<Chore> nextChore;
    public StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.Signal stop;
    public GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.State nochore;
    public GameStateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.State haschore;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.nochore;
      this.saveHistory = true;
      this.nochore.Update((Action<ChoreDriver.StatesInstance, float>) ((smi, dt) =>
      {
        if (!smi.masterPrefabId.IsPrefabID(GameTags.Minion) || smi.masterPrefabId.HasTag(GameTags.Dead))
          return;
        ReportManager.Instance.ReportValue(ReportManager.ReportType.WorkTime, dt, string.Format((string) UI.ENDOFDAYREPORT.NOTES.TIME_SPENT, (object) DUPLICANTS.CHORES.THINKING.NAME), ((Component) smi.master).GetProperName());
      })).ParamTransition<Chore>((StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.Parameter<Chore>) this.nextChore, this.haschore, (StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.Parameter<Chore>.Callback) ((smi, next_chore) => next_chore != null));
      this.haschore.Enter("BeginChore", (StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.State.Callback) (smi => smi.BeginChore())).Update((Action<ChoreDriver.StatesInstance, float>) ((smi, dt) =>
      {
        if (!smi.masterPrefabId.IsPrefabID(GameTags.Minion) || smi.masterPrefabId.HasTag(GameTags.Dead))
          return;
        Chore chore = this.currentChore.Get(smi);
        if (chore == null)
          return;
        if (smi.navigator.IsMoving())
        {
          ReportManager.Instance.ReportValue(ReportManager.ReportType.TravelTime, dt, GameUtil.GetChoreName(chore, (object) null), ((Component) smi.master).GetProperName());
        }
        else
        {
          ReportManager.ReportType reportType1 = chore.GetReportType();
          Workable workable = smi.worker.workable;
          if (Object.op_Inequality((Object) workable, (Object) null))
          {
            ReportManager.ReportType reportType2 = workable.GetReportType();
            if (reportType1 != reportType2)
              reportType1 = reportType2;
          }
          ReportManager.Instance.ReportValue(reportType1, dt, string.Format((string) UI.ENDOFDAYREPORT.NOTES.WORK_TIME, (object) GameUtil.GetChoreName(chore, (object) null)), ((Component) smi.master).GetProperName());
        }
      })).Exit("EndChore", (StateMachine<ChoreDriver.States, ChoreDriver.StatesInstance, ChoreDriver, object>.State.Callback) (smi => smi.EndChore("ChoreDriver.SignalStop"))).OnSignal(this.stop, this.nochore);
    }
  }
}
