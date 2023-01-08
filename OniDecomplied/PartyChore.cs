// Decompiled with JetBrains decompiler
// Type: PartyChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using TUNING;
using UnityEngine;

public class PartyChore : Chore<PartyChore.StatesInstance>, IWorkerPrioritizable
{
  public int basePriority = RELAXATION.PRIORITY.SPECIAL_EVENT;
  public const string specificEffect = "Socialized";
  public const string trackingEffect = "RecentlySocialized";

  public PartyChore(
    IStateMachineTarget master,
    Workable chat_workable,
    Action<Chore> on_complete = null,
    Action<Chore> on_begin = null,
    Action<Chore> on_end = null)
    : base(Db.Get().ChoreTypes.Party, master, master.GetComponent<ChoreProvider>(), on_complete: on_complete, on_begin: on_begin, on_end: on_end, master_priority_class: PriorityScreen.PriorityClass.high, report_type: ReportManager.ReportType.PersonalTime)
  {
    this.smi = new PartyChore.StatesInstance(this);
    this.smi.sm.chitchatlocator.Set((KMonoBehaviour) chat_workable, this.smi);
    this.AddPrecondition(ChorePreconditions.instance.CanMoveTo, (object) chat_workable);
    this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
  }

  public override void Begin(Chore.Precondition.Context context)
  {
    this.smi.sm.partyer.Set(context.consumerState.gameObject, this.smi, false);
    base.Begin(context);
    this.smi.sm.partyer.Get(this.smi).gameObject.AddTag(GameTags.Partying);
  }

  protected override void End(string reason)
  {
    if (Object.op_Inequality((Object) this.smi.sm.partyer.Get(this.smi), (Object) null))
      this.smi.sm.partyer.Get(this.smi).gameObject.RemoveTag(GameTags.Partying);
    base.End(reason);
  }

  public bool GetWorkerPriority(Worker worker, out int priority)
  {
    priority = this.basePriority;
    return true;
  }

  public class States : GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore>
  {
    public StateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.TargetParameter partyer;
    public StateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.TargetParameter chitchatlocator;
    public GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.ApproachSubState<IApproachable> stand;
    public GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.ApproachSubState<IApproachable> chat_move;
    public GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.State chat;
    public GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.State success;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.stand;
      this.Target(this.partyer);
      this.stand.InitializeStates(this.partyer, this.masterTarget, this.chat);
      this.chat_move.InitializeStates(this.partyer, this.chitchatlocator, this.chat);
      this.chat.ToggleWork<Workable>(this.chitchatlocator, this.success, (GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.State) null, (Func<PartyChore.StatesInstance, bool>) null);
      this.success.Enter((StateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.State.Callback) (smi => this.partyer.Get(smi).gameObject.GetComponent<Effects>().Add("RecentlyPartied", true))).ReturnSuccess();
    }
  }

  public class StatesInstance : 
    GameStateMachine<PartyChore.States, PartyChore.StatesInstance, PartyChore, object>.GameInstance
  {
    public StatesInstance(PartyChore master)
      : base(master)
    {
    }
  }
}
