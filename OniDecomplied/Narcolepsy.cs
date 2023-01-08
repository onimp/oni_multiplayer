// Decompiled with JetBrains decompiler
// Type: Narcolepsy
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Narcolepsy : StateMachineComponent<Narcolepsy.StatesInstance>
{
  public static readonly Chore.Precondition IsNarcolepsingPrecondition = new Chore.Precondition()
  {
    id = nameof (IsNarcolepsingPrecondition),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_NARCOLEPSING,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Narcolepsy component = ((Component) context.consumerState.consumer).GetComponent<Narcolepsy>();
      return Object.op_Inequality((Object) component, (Object) null) && component.IsNarcolepsing();
    })
  };

  protected virtual void OnSpawn() => this.smi.StartSM();

  public bool IsNarcolepsing() => this.smi.IsNarcolepsing();

  public class StatesInstance : 
    GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy, object>.GameInstance
  {
    public StatesInstance(Narcolepsy master)
      : base(master)
    {
    }

    public bool IsSleeping()
    {
      StaminaMonitor.Instance smi = ((Component) this.master).GetSMI<StaminaMonitor.Instance>();
      return smi != null && smi.IsSleeping();
    }

    public bool IsNarcolepsing() => this.GetCurrentState() == this.sm.sleepy;

    public GameObject CreateFloorLocator()
    {
      Sleepable safeFloorLocator = SleepChore.GetSafeFloorLocator(((Component) this.master).gameObject);
      safeFloorLocator.effectName = "NarcolepticSleep";
      safeFloorLocator.stretchOnWake = false;
      return ((Component) safeFloorLocator).gameObject;
    }
  }

  public class States : GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy>
  {
    public GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy, object>.State idle;
    public GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy, object>.State sleepy;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.root.TagTransition(GameTags.Dead, (GameStateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy, object>.State) null);
      this.idle.Enter("ScheduleNextSleep", (StateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy, object>.State.Callback) (smi => smi.ScheduleGoTo(this.GetNewInterval(TUNING.TRAITS.NARCOLEPSY_INTERVAL_MIN, TUNING.TRAITS.NARCOLEPSY_INTERVAL_MAX), (StateMachine.BaseState) this.sleepy)));
      this.sleepy.Enter("Is Already Sleeping Check", (StateMachine<Narcolepsy.States, Narcolepsy.StatesInstance, Narcolepsy, object>.State.Callback) (smi =>
      {
        if (((Component) smi.master).GetSMI<StaminaMonitor.Instance>().IsSleeping())
          smi.GoTo((StateMachine.BaseState) this.idle);
        else
          smi.ScheduleGoTo(this.GetNewInterval(TUNING.TRAITS.NARCOLEPSY_SLEEPDURATION_MIN, TUNING.TRAITS.NARCOLEPSY_SLEEPDURATION_MAX), (StateMachine.BaseState) this.idle);
      })).ToggleUrge(Db.Get().Urges.Narcolepsy).ToggleChore(new Func<Narcolepsy.StatesInstance, Chore>(this.CreateNarcolepsyChore), this.idle);
    }

    private Chore CreateNarcolepsyChore(Narcolepsy.StatesInstance smi)
    {
      GameObject floorLocator = smi.CreateFloorLocator();
      SleepChore narcolepsyChore = new SleepChore(Db.Get().ChoreTypes.Narcolepsy, (IStateMachineTarget) smi.master, floorLocator, true, false);
      narcolepsyChore.AddPrecondition(Narcolepsy.IsNarcolepsingPrecondition);
      return (Chore) narcolepsyChore;
    }

    private float GetNewInterval(float min, float max)
    {
      double num = (double) Mathf.Min(Mathf.Max(Util.GaussianRandom(max - min, 1f), min), max);
      return Random.Range(min, max);
    }
  }
}
