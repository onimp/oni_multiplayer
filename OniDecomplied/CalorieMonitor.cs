// Decompiled with JetBrains decompiler
// Type: CalorieMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

public class CalorieMonitor : GameStateMachine<CalorieMonitor, CalorieMonitor.Instance>
{
  public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public CalorieMonitor.HungryState hungry;
  public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State eating;
  public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State incapacitated;
  public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State depleted;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.satisfied.Transition((GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State) this.hungry, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsHungry()));
    this.hungry.DefaultState(this.hungry.normal).Transition(this.satisfied, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsSatisfied())).EventTransition(GameHashes.BeginChore, this.eating, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsEating()));
    this.hungry.working.EventTransition(GameHashes.ScheduleBlocksChanged, this.hungry.normal, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsEatTime())).Transition(this.hungry.starving, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsStarving())).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hungry);
    this.hungry.normal.EventTransition(GameHashes.ScheduleBlocksChanged, this.hungry.working, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsEatTime())).Transition(this.hungry.starving, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsStarving())).ToggleStatusItem(Db.Get().DuplicantStatusItems.Hungry).ToggleUrge(Db.Get().Urges.Eat).ToggleExpression(Db.Get().Expressions.Hungry).ToggleThought(Db.Get().Thoughts.Starving);
    this.hungry.starving.Transition(this.hungry.normal, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsStarving())).Transition(this.depleted, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsDepleted())).ToggleStatusItem(Db.Get().DuplicantStatusItems.Starving).ToggleUrge(Db.Get().Urges.Eat).ToggleExpression(Db.Get().Expressions.Hungry).ToggleThought(Db.Get().Thoughts.Starving);
    this.eating.EventTransition(GameHashes.EndChore, this.satisfied, (StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsEating()));
    this.depleted.ToggleTag(GameTags.CaloriesDepleted).Enter((StateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.Kill()));
  }

  public class HungryState : 
    GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State working;
    public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State normal;
    public GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.State starving;
  }

  public new class Instance : 
    GameStateMachine<CalorieMonitor, CalorieMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public AmountInstance calories;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.calories = Db.Get().Amounts.Calories.Lookup(this.gameObject);
    }

    private float GetCalories0to1() => this.calories.value / this.calories.GetMax();

    public bool IsEatTime() => this.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Eat);

    public bool IsHungry() => (double) this.GetCalories0to1() < 0.824999988079071;

    public bool IsStarving() => (double) this.GetCalories0to1() < 0.25;

    public bool IsSatisfied() => (double) this.GetCalories0to1() > 0.949999988079071;

    public bool IsEating()
    {
      ChoreDriver component = this.master.GetComponent<ChoreDriver>();
      return component.HasChore() && component.GetCurrentChore().choreType.urge == Db.Get().Urges.Eat;
    }

    public bool IsDepleted() => (double) this.calories.value <= 0.0;

    public bool ShouldExitInfirmary() => !this.IsStarving();

    public void Kill()
    {
      if (this.gameObject.GetSMI<DeathMonitor.Instance>() == null)
        return;
      this.gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Starvation);
    }
  }
}
