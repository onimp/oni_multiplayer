// Decompiled with JetBrains decompiler
// Type: SocialGatheringPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class SocialGatheringPoint : StateMachineComponent<SocialGatheringPoint.StatesInstance>
{
  public CellOffset[] choreOffsets = new CellOffset[2]
  {
    new CellOffset(0, 0),
    new CellOffset(1, 0)
  };
  public int choreCount = 2;
  public int basePriority;
  public string socialEffect;
  public float workTime = 15f;
  public System.Action OnSocializeBeginCB;
  public System.Action OnSocializeEndCB;
  private SocialChoreTracker tracker;
  private SocialGatheringPointWorkable[] workables;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.workables = new SocialGatheringPointWorkable[this.choreOffsets.Length];
    for (int index = 0; index < this.workables.Length; ++index)
    {
      SocialGatheringPointWorkable gatheringPointWorkable = ChoreHelpers.CreateLocator("SocialGatheringPointWorkable", Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.choreOffsets[index]), Grid.SceneLayer.Move)).AddOrGet<SocialGatheringPointWorkable>();
      gatheringPointWorkable.basePriority = this.basePriority;
      gatheringPointWorkable.specificEffect = this.socialEffect;
      gatheringPointWorkable.OnWorkableEventCB = new Action<Workable, Workable.WorkableEvent>(this.OnWorkableEvent);
      gatheringPointWorkable.SetWorkTime(this.workTime);
      this.workables[index] = gatheringPointWorkable;
    }
    this.tracker = new SocialChoreTracker(((Component) this).gameObject, this.choreOffsets);
    this.tracker.choreCount = this.choreCount;
    this.tracker.CreateChoreCB = new Func<int, Chore>(this.CreateChore);
    this.smi.StartSM();
  }

  protected override void OnCleanUp()
  {
    if (this.tracker != null)
    {
      this.tracker.Clear();
      this.tracker = (SocialChoreTracker) null;
    }
    if (this.workables != null)
    {
      for (int index = 0; index < this.workables.Length; ++index)
      {
        if (Object.op_Implicit((Object) this.workables[index]))
        {
          Util.KDestroyGameObject((Component) this.workables[index]);
          this.workables[index] = (SocialGatheringPointWorkable) null;
        }
      }
    }
    base.OnCleanUp();
  }

  private Chore CreateChore(int i)
  {
    Workable workable = (Workable) this.workables[i];
    ChoreType relax = Db.Get().ChoreTypes.Relax;
    Workable target = workable;
    ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
    Action<Chore> on_end = new Action<Chore>(this.OnSocialChoreEnd);
    ScheduleBlockType schedule_block = recreation;
    WorkChore<SocialGatheringPointWorkable> chore = new WorkChore<SocialGatheringPointWorkable>(relax, (IStateMachineTarget) target, on_end: on_end, allow_in_red_alert: false, schedule_block: schedule_block, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high, add_to_daily_report: false);
    chore.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
    chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) workable);
    chore.AddPrecondition(ChorePreconditions.instance.IsNotARobot, (object) workable);
    return (Chore) chore;
  }

  private void OnSocialChoreEnd(Chore chore)
  {
    if (!this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.on))
      return;
    this.tracker.Update();
  }

  private void OnWorkableEvent(Workable workable, Workable.WorkableEvent workable_event)
  {
    switch (workable_event)
    {
      case Workable.WorkableEvent.WorkStarted:
        if (this.OnSocializeBeginCB == null)
          break;
        this.OnSocializeBeginCB();
        break;
      case Workable.WorkableEvent.WorkStopped:
        if (this.OnSocializeEndCB == null)
          break;
        this.OnSocializeEndCB();
        break;
    }
  }

  public class States : 
    GameStateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint>
  {
    public GameStateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint, object>.State off;
    public GameStateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint, object>.State on;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.off;
      this.root.DoNothing();
      this.off.TagTransition(GameTags.Operational, this.on);
      this.on.TagTransition(GameTags.Operational, this.off, true).Enter("CreateChore", (StateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint, object>.State.Callback) (smi => smi.master.tracker.Update())).Exit("CancelChore", (StateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint, object>.State.Callback) (smi => smi.master.tracker.Update(false)));
    }
  }

  public class StatesInstance : 
    GameStateMachine<SocialGatheringPoint.States, SocialGatheringPoint.StatesInstance, SocialGatheringPoint, object>.GameInstance
  {
    public StatesInstance(SocialGatheringPoint smi)
      : base(smi)
    {
    }
  }
}
