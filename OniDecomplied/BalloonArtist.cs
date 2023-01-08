// Decompiled with JetBrains decompiler
// Type: BalloonArtist
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Runtime.Serialization;
using TUNING;

public class BalloonArtist : GameStateMachine<BalloonArtist, BalloonArtist.Instance>
{
  public StateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.IntParameter balloonsGivenOut;
  public GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State neutral;
  public BalloonArtist.OverjoyedStates overjoyed;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.neutral;
    this.root.TagTransition(GameTags.Dead, (GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State) null);
    this.neutral.TagTransition(GameTags.Overjoyed, (GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State) this.overjoyed);
    this.overjoyed.TagTransition(GameTags.Overjoyed, this.neutral, true).DefaultState(this.overjoyed.idle).ParamTransition<int>((StateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.Parameter<int>) this.balloonsGivenOut, this.overjoyed.exitEarly, (StateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.Parameter<int>.Callback) ((smi, p) => p >= TRAITS.JOY_REACTIONS.BALLOON_ARTIST.NUM_BALLOONS_TO_GIVE)).Exit((StateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      smi.numBalloonsGiven = 0;
      this.balloonsGivenOut.Set(0, smi);
    }));
    this.overjoyed.idle.Enter((StateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      if (!smi.IsRecTime())
        return;
      smi.GoTo((StateMachine.BaseState) this.overjoyed.balloon_stand);
    })).ToggleStatusItem(Db.Get().DuplicantStatusItems.BalloonArtistPlanning).EventTransition(GameHashes.ScheduleBlocksChanged, this.overjoyed.balloon_stand, (StateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsRecTime()));
    this.overjoyed.balloon_stand.ToggleStatusItem(Db.Get().DuplicantStatusItems.BalloonArtistHandingOut).EventTransition(GameHashes.ScheduleBlocksChanged, this.overjoyed.idle, (StateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsRecTime())).ToggleChore((Func<BalloonArtist.Instance, Chore>) (smi => (Chore) new BalloonArtistChore(smi.master)), this.overjoyed.idle);
    this.overjoyed.exitEarly.Enter((StateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.ExitJoyReactionEarly()));
  }

  public class OverjoyedStates : 
    GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State idle;
    public GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State balloon_stand;
    public GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State exitEarly;
  }

  public new class Instance : 
    GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.GameInstance
  {
    [Serialize]
    public int numBalloonsGiven;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    [OnDeserialized]
    private void OnDeserialized() => this.smi.sm.balloonsGivenOut.Set(this.numBalloonsGiven, this.smi);

    public bool IsRecTime() => this.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);

    public void GiveBalloon()
    {
      ++this.numBalloonsGiven;
      this.smi.sm.balloonsGivenOut.Set(this.numBalloonsGiven, this.smi);
    }

    public void ExitJoyReactionEarly()
    {
      JoyBehaviourMonitor.Instance smi = this.master.gameObject.GetSMI<JoyBehaviourMonitor.Instance>();
      smi.sm.exitEarly.Trigger(smi);
    }
  }
}
