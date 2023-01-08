// Decompiled with JetBrains decompiler
// Type: RationMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RationMonitor : GameStateMachine<RationMonitor, RationMonitor.Instance>
{
  public StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.FloatParameter rationsAteToday;
  public RationMonitor.RationsAvailableState rationsavailable;
  public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.HungrySubState outofrations;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.rationsavailable;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.root.EventHandler(GameHashes.EatCompleteEater, (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback) ((smi, d) => smi.OnEatComplete(d))).EventHandler(GameHashes.NewDay, (Func<RationMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), (StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.OnNewDay())).ParamTransition<float>((StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.rationsAteToday, (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable, (StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => smi.HasRationsAvailable())).ParamTransition<float>((StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.rationsAteToday, (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.outofrations, (StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => !smi.HasRationsAvailable()));
    this.rationsavailable.DefaultState((GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable.noediblesavailable);
    this.rationsavailable.noediblesavailable.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.NoRationsAvailable).EventTransition(GameHashes.ColonyHasRationsChanged, new Func<RationMonitor.Instance, KMonoBehaviour>(RationMonitor.GetSaveGame), (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable.ediblesunreachable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.AreThereAnyEdibles));
    this.rationsavailable.ediblereachablebutnotpermitted.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.RationsNotPermitted).EventTransition(GameHashes.ColonyHasRationsChanged, new Func<RationMonitor.Instance, KMonoBehaviour>(RationMonitor.GetSaveGame), (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable.noediblesavailable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.AreThereNoEdibles)).EventTransition(GameHashes.ClosestEdibleChanged, (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable.ediblesunreachable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.NotIsEdibleInReachButNotPermitted));
    this.rationsavailable.ediblesunreachable.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.RationsUnreachable).EventTransition(GameHashes.ColonyHasRationsChanged, new Func<RationMonitor.Instance, KMonoBehaviour>(RationMonitor.GetSaveGame), (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable.noediblesavailable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.AreThereNoEdibles)).EventTransition(GameHashes.ClosestEdibleChanged, (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable.edibleavailable, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.IsEdibleAvailable)).EventTransition(GameHashes.ClosestEdibleChanged, (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable.ediblereachablebutnotpermitted, new StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(RationMonitor.IsEdibleInReachButNotPermitted));
    this.rationsavailable.edibleavailable.ToggleChore((Func<RationMonitor.Instance, Chore>) (smi => (Chore) new EatChore(smi.master)), (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable.noediblesavailable).DefaultState(this.rationsavailable.edibleavailable.readytoeat);
    this.rationsavailable.edibleavailable.readytoeat.EventTransition(GameHashes.ClosestEdibleChanged, (GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State) this.rationsavailable.noediblesavailable).EventTransition(GameHashes.BeginChore, this.rationsavailable.edibleavailable.eating, (StateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsEating()));
    this.rationsavailable.edibleavailable.eating.DoNothing();
    this.outofrations.InitializeStates(this.masterTarget, Db.Get().DuplicantStatusItems.DailyRationLimitReached);
  }

  private static bool AreThereNoEdibles(RationMonitor.Instance smi) => !RationMonitor.AreThereAnyEdibles(smi);

  private static bool AreThereAnyEdibles(RationMonitor.Instance smi)
  {
    if (Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
    {
      ColonyRationMonitor.Instance smi1 = ((Component) SaveGame.Instance).GetSMI<ColonyRationMonitor.Instance>();
      if (smi1 != null)
        return !smi1.IsOutOfRations();
    }
    return false;
  }

  private static KMonoBehaviour GetSaveGame(RationMonitor.Instance smi) => (KMonoBehaviour) SaveGame.Instance;

  private static bool IsEdibleAvailable(RationMonitor.Instance smi) => Object.op_Inequality((Object) smi.GetEdible(), (Object) null);

  private static bool NotIsEdibleInReachButNotPermitted(RationMonitor.Instance smi) => !RationMonitor.IsEdibleInReachButNotPermitted(smi);

  private static bool IsEdibleInReachButNotPermitted(RationMonitor.Instance smi) => smi.GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().edibleInReachButNotPermitted;

  public class EdibleAvailablestate : 
    GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State readytoeat;
    public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State eating;
  }

  public class RationsAvailableState : 
    GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.HungrySubState noediblesavailable;
    public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.HungrySubState ediblereachablebutnotpermitted;
    public GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.HungrySubState ediblesunreachable;
    public RationMonitor.EdibleAvailablestate edibleavailable;
  }

  public new class Instance : 
    GameStateMachine<RationMonitor, RationMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    private ChoreDriver choreDriver;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.choreDriver = master.GetComponent<ChoreDriver>();
    }

    public Edible GetEdible() => this.GetComponent<Sensors>().GetSensor<ClosestEdibleSensor>().GetEdible();

    public bool HasRationsAvailable() => true;

    public float GetRationsAteToday() => this.sm.rationsAteToday.Get(this.smi);

    public float GetRationsRemaining() => 1f;

    public bool IsEating() => this.choreDriver.HasChore() && this.choreDriver.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;

    public void OnNewDay()
    {
      double num = (double) this.smi.sm.rationsAteToday.Set(0.0f, this.smi);
    }

    public void OnEatComplete(object data)
    {
      Edible edible = (Edible) data;
      double num = (double) this.sm.rationsAteToday.Delta(edible.caloriesConsumed, this.smi);
      RationTracker.Get().RegisterRationsConsumed(edible);
    }
  }
}
