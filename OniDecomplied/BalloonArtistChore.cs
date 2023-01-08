// Decompiled with JetBrains decompiler
// Type: BalloonArtistChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using TUNING;
using UnityEngine;

public class BalloonArtistChore : Chore<BalloonArtistChore.StatesInstance>, IWorkerPrioritizable
{
  private int basePriority = RELAXATION.PRIORITY.TIER1;
  private Chore.Precondition HasBalloonStallCell = new Chore.Precondition()
  {
    id = nameof (HasBalloonStallCell),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.HAS_BALLOON_STALL_CELL,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => ((Chore<BalloonArtistChore.StatesInstance>) data).smi.HasBalloonStallCell())
  };

  public BalloonArtistChore(IStateMachineTarget target)
    : base(Db.Get().ChoreTypes.JoyReaction, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.high, report_type: ReportManager.ReportType.PersonalTime)
  {
    this.showAvailabilityInHoverText = false;
    this.smi = new BalloonArtistChore.StatesInstance(this, target.gameObject);
    this.AddPrecondition(this.HasBalloonStallCell, (object) this);
    this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
    this.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, (object) Db.Get().ScheduleBlockTypes.Recreation);
    this.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) this);
  }

  public bool GetWorkerPriority(Worker worker, out int priority)
  {
    priority = this.basePriority;
    return true;
  }

  public class States : 
    GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore>
  {
    public StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.TargetParameter balloonArtist;
    public StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.IntParameter balloonsGivenOut = new StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.IntParameter(0);
    public StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.Signal giveBalloonOut;
    public GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State idle;
    public GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State goToStand;
    public BalloonArtistChore.States.BalloonStandStates balloonStand;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.goToStand;
      this.Target(this.balloonArtist);
      this.root.EventTransition(GameHashes.ScheduleBlocksChanged, this.idle, (StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.Transition.ConditionCallback) (smi => !smi.IsRecTime()));
      this.idle.DoNothing();
      this.goToStand.Transition((GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State) null, (StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.Transition.ConditionCallback) (smi => !smi.HasBalloonStallCell())).MoveTo((Func<BalloonArtistChore.StatesInstance, int>) (smi => smi.GetBalloonStallCell()), (GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State) this.balloonStand);
      this.balloonStand.ToggleAnims("anim_interacts_balloon_artist_kanim").Enter((StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State.Callback) (smi => smi.SpawnBalloonStand())).Exit((StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State.Callback) (smi => smi.DestroyBalloonStand())).DefaultState(this.balloonStand.idle);
      this.balloonStand.idle.PlayAnim("working_pre").QueueAnim("working_loop", true).OnSignal(this.giveBalloonOut, this.balloonStand.giveBalloon);
      this.balloonStand.giveBalloon.PlayAnim("working_pst").OnAnimQueueComplete(this.balloonStand.idle);
    }

    public class BalloonStandStates : 
      GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State
    {
      public GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State idle;
      public GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State giveBalloon;
    }
  }

  public class StatesInstance : 
    GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.GameInstance
  {
    private BalloonStandCellSensor balloonArtistCellSensor;
    private GameObject balloonArtist;
    private GameObject balloonStand;

    public StatesInstance(BalloonArtistChore master, GameObject balloonArtist)
      : base(master)
    {
      this.balloonArtist = balloonArtist;
      this.sm.balloonArtist.Set(balloonArtist, this.smi, false);
    }

    public bool IsRecTime() => this.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);

    public int GetBalloonStallCell() => this.balloonArtistCellSensor.GetCell();

    public int GetBalloonStallTargetCell() => this.balloonArtistCellSensor.GetStandCell();

    public bool HasBalloonStallCell()
    {
      if (this.balloonArtistCellSensor == null)
        this.balloonArtistCellSensor = this.GetComponent<Sensors>().GetSensor<BalloonStandCellSensor>();
      return this.balloonArtistCellSensor.GetCell() != Grid.InvalidCell;
    }

    public bool IsSameRoom()
    {
      CavityInfo cavityForCell1 = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(this.balloonArtist));
      CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(this.GetBalloonStallCell());
      return cavityForCell1 != null && cavityForCell2 != null && HandleVector<int>.Handle.op_Equality(cavityForCell1.handle, cavityForCell2.handle);
    }

    public void SpawnBalloonStand()
    {
      Vector3 pos = Grid.CellToPos(this.GetBalloonStallTargetCell());
      this.balloonArtist.GetComponent<Facing>().Face(pos);
      this.balloonStand = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("BalloonStand")), pos, Quaternion.identity, (GameObject) null, (string) null, true, 0);
      this.balloonStand.SetActive(true);
      this.balloonStand.GetComponent<GetBalloonWorkable>().SetBalloonArtist(this.smi);
    }

    public void DestroyBalloonStand() => TracesExtesions.DeleteObject(this.balloonStand);

    public void GiveBalloon()
    {
      this.balloonArtist.GetSMI<BalloonArtist.Instance>().GiveBalloon();
      this.smi.sm.giveBalloonOut.Trigger(this.smi);
    }
  }
}
