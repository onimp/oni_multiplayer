// Decompiled with JetBrains decompiler
// Type: SleepChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using UnityEngine;

public class SleepChore : Chore<SleepChore.StatesInstance>
{
  public static readonly Chore.Precondition IsOkayTimeToSleep = new Chore.Precondition()
  {
    id = nameof (IsOkayTimeToSleep),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_OKAY_TIME_TO_SLEEP,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Narcolepsy component = ((Component) context.consumerState.consumer).GetComponent<Narcolepsy>();
      int num1 = Object.op_Inequality((Object) component, (Object) null) ? (component.IsNarcolepsing() ? 1 : 0) : 0;
      StaminaMonitor.Instance smi = ((Component) context.consumerState.consumer).GetSMI<StaminaMonitor.Instance>();
      bool flag = smi != null && smi.NeedsToSleep();
      int num2 = ChorePreconditions.instance.IsScheduledTime.fn(ref context, (object) Db.Get().ScheduleBlockTypes.Sleep) ? 1 : 0;
      return (num1 | num2 | (flag ? 1 : 0)) != 0;
    })
  };

  public SleepChore(
    ChoreType choreType,
    IStateMachineTarget target,
    GameObject bed,
    bool bedIsLocator,
    bool isInterruptable)
    : base(choreType, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.personalNeeds, report_type: ReportManager.ReportType.PersonalTime)
  {
    this.smi = new SleepChore.StatesInstance(this, target.gameObject, bed, bedIsLocator, isInterruptable);
    if (isInterruptable)
      this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
    this.AddPrecondition(SleepChore.IsOkayTimeToSleep);
    Operational component = bed.GetComponent<Operational>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.AddPrecondition(ChorePreconditions.instance.IsOperational, (object) component);
  }

  public static Sleepable GetSafeFloorLocator(GameObject sleeper)
  {
    int cell = sleeper.GetComponent<Sensors>().GetSensor<SafeCellSensor>().GetSleepCellQuery();
    if (cell == Grid.InvalidCell)
      cell = Grid.PosToCell(TransformExtensions.GetPosition(sleeper.transform));
    return ChoreHelpers.CreateSleepLocator(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move)).GetComponent<Sleepable>();
  }

  public static bool IsDarkAtCell(int cell) => Grid.LightIntensity[cell] <= 0;

  public class StatesInstance : 
    GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.GameInstance
  {
    public bool hadPeacefulSleep;
    public bool hadNormalSleep;
    public bool hadBadSleep;
    public bool hadTerribleSleep;
    public int lastEvaluatedDay = -1;
    public float wakeUpBuffer = 2f;
    public string stateChangeNoiseSource;
    private GameObject locator;

    public StatesInstance(
      SleepChore master,
      GameObject sleeper,
      GameObject bed,
      bool bedIsLocator,
      bool isInterruptable)
      : base(master)
    {
      this.sm.sleeper.Set(sleeper, this.smi, false);
      this.sm.isInterruptable.Set(isInterruptable, this.smi);
      Traits component = sleeper.GetComponent<Traits>();
      if (Object.op_Inequality((Object) component, (Object) null))
        this.sm.needsNightLight.Set(component.HasTrait("NightLight"), this.smi);
      if (bedIsLocator)
        this.AddLocator(bed);
      else
        this.sm.bed.Set(bed, this.smi, false);
    }

    public void CheckLightLevel()
    {
      GameObject go = this.sm.sleeper.Get(this.smi);
      int cell = Grid.PosToCell(go);
      if (!Grid.IsValidCell(cell))
        return;
      bool flag = SleepChore.IsDarkAtCell(cell);
      if (this.sm.needsNightLight.Get(this.smi))
      {
        if (!flag)
          return;
        EventExtensions.Trigger(go, -1307593733, (object) null);
      }
      else
      {
        if (flag || this.IsLoudSleeper() || this.IsGlowStick())
          return;
        EventExtensions.Trigger(go, -1063113160, (object) null);
      }
    }

    public bool IsLoudSleeper() => Object.op_Inequality((Object) this.sm.sleeper.Get(this.smi).GetComponent<Snorer>(), (Object) null);

    public bool IsGlowStick() => Object.op_Inequality((Object) this.sm.sleeper.Get(this.smi).GetComponent<GlowStick>(), (Object) null);

    public void EvaluateSleepQuality()
    {
    }

    public void AddLocator(GameObject sleepable)
    {
      this.locator = sleepable;
      int cell = Grid.PosToCell(this.locator);
      Grid.Reserved[cell] = true;
      this.sm.bed.Set(this.locator, this, false);
    }

    public void DestroyLocator()
    {
      if (!Object.op_Inequality((Object) this.locator, (Object) null))
        return;
      Grid.Reserved[Grid.PosToCell(this.locator)] = false;
      ChoreHelpers.DestroyLocator(this.locator);
      this.sm.bed.Set((KMonoBehaviour) null, this);
      this.locator = (GameObject) null;
    }

    public void SetAnim()
    {
      Sleepable sleepable = this.sm.bed.Get<Sleepable>(this.smi);
      if (!Object.op_Equality((Object) ((Component) sleepable).GetComponent<Building>(), (Object) null))
        return;
      string str;
      switch (this.sm.sleeper.Get<Navigator>(this.smi).CurrentNavType)
      {
        case NavType.Ladder:
          str = "anim_sleep_ladder_kanim";
          break;
        case NavType.Pole:
          str = "anim_sleep_pole_kanim";
          break;
        default:
          str = "anim_sleep_floor_kanim";
          break;
      }
      sleepable.overrideAnims = new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit(str))
      };
    }
  }

  public class States : GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore>
  {
    public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter sleeper;
    public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.TargetParameter bed;
    public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isInterruptable;
    public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isDisturbedByNoise;
    public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isDisturbedByLight;
    public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isDisturbedByMovement;
    public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter isScaredOfDark;
    public StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.BoolParameter needsNightLight;
    public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.ApproachSubState<IApproachable> approach;
    public SleepChore.States.SleepStates sleep;
    public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State success;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.approach;
      this.Target(this.sleeper);
      this.root.Exit("DestroyLocator", (StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi => smi.DestroyLocator()));
      this.approach.InitializeStates(this.sleeper, this.bed, (GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State) this.sleep);
      this.sleep.Enter("SetAnims", (StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi => smi.SetAnim())).DefaultState(this.sleep.normal).ToggleTag(GameTags.Asleep).DoSleep(this.sleeper, this.bed, this.success, (GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State) null).TriggerOnExit(GameHashes.SleepFinished).EventHandler(GameHashes.SleepDisturbedByLight, (StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi => this.isDisturbedByLight.Set(true, smi))).EventHandler(GameHashes.SleepDisturbedByNoise, (StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi => this.isDisturbedByNoise.Set(true, smi))).EventHandler(GameHashes.SleepDisturbedByFearOfDark, (StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi => this.isScaredOfDark.Set(true, smi))).EventHandler(GameHashes.SleepDisturbedByMovement, (StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi => this.isDisturbedByMovement.Set(true, smi)));
      this.sleep.uninterruptable.DoNothing();
      this.sleep.normal.ParamTransition<bool>((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.Parameter<bool>) this.isInterruptable, this.sleep.uninterruptable, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsFalse).ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.Sleeping).QueueAnim("working_loop", true).ParamTransition<bool>((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.Parameter<bool>) this.isDisturbedByNoise, this.sleep.interrupt_noise, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsTrue).ParamTransition<bool>((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.Parameter<bool>) this.isDisturbedByLight, this.sleep.interrupt_light, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsTrue).ParamTransition<bool>((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.Parameter<bool>) this.isScaredOfDark, this.sleep.interrupt_scared, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsTrue).ParamTransition<bool>((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.Parameter<bool>) this.isDisturbedByMovement, this.sleep.interrupt_movement, GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.IsTrue).Update((Action<SleepChore.StatesInstance, float>) ((smi, dt) => smi.CheckLightLevel()));
      this.sleep.interrupt_scared.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByFearOfDark).QueueAnim("interrupt_afraid").OnAnimQueueComplete(this.sleep.interrupt_scared_transition);
      this.sleep.interrupt_scared_transition.Enter((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi =>
      {
        if (!smi.master.GetComponent<Effects>().HasEffect(Db.Get().effects.Get("TerribleSleep")))
          smi.master.GetComponent<Effects>().Add(Db.Get().effects.Get("BadSleepAfraidOfDark"), true);
        GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State state = smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? this.sleep.normal : this.success;
        this.isScaredOfDark.Set(false, smi);
        smi.GoTo((StateMachine.BaseState) state);
      }));
      this.sleep.interrupt_movement.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByMovement).PlayAnim("interrupt_light").OnAnimQueueComplete(this.sleep.interrupt_movement_transition).Enter((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi =>
      {
        GameObject gameObject = smi.sm.bed.Get(smi);
        if (!Object.op_Inequality((Object) gameObject, (Object) null))
          return;
        EventExtensions.Trigger(gameObject, -717201811, (object) null);
      }));
      this.sleep.interrupt_movement_transition.Enter((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi =>
      {
        if (!smi.master.GetComponent<Effects>().HasEffect(Db.Get().effects.Get("TerribleSleep")))
          smi.master.GetComponent<Effects>().Add(Db.Get().effects.Get("BadSleepMovement"), true);
        GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State state = smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? this.sleep.normal : this.success;
        this.isDisturbedByMovement.Set(false, smi);
        smi.GoTo((StateMachine.BaseState) state);
      }));
      this.sleep.interrupt_noise.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByNoise).QueueAnim("interrupt_light").OnAnimQueueComplete(this.sleep.interrupt_noise_transition);
      this.sleep.interrupt_noise_transition.Enter((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi =>
      {
        Effects component = smi.master.GetComponent<Effects>();
        component.Add(Db.Get().effects.Get("TerribleSleep"), true);
        if (component.HasEffect(Db.Get().effects.Get("BadSleep")))
          component.Remove(Db.Get().effects.Get("BadSleep"));
        this.isDisturbedByNoise.Set(false, smi);
        GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State state = smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? this.sleep.normal : this.success;
        smi.GoTo((StateMachine.BaseState) state);
      }));
      this.sleep.interrupt_light.ToggleCategoryStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().DuplicantStatusItems.SleepingInterruptedByLight).QueueAnim("interrupt").OnAnimQueueComplete(this.sleep.interrupt_light_transition);
      this.sleep.interrupt_light_transition.Enter((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi =>
      {
        if (!smi.master.GetComponent<Effects>().HasEffect(Db.Get().effects.Get("TerribleSleep")))
          smi.master.GetComponent<Effects>().Add(Db.Get().effects.Get("BadSleep"), true);
        GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State state = smi.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Sleep) ? this.sleep.normal : this.success;
        this.isDisturbedByLight.Set(false, smi);
        smi.GoTo((StateMachine.BaseState) state);
      }));
      this.success.Enter((StateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State.Callback) (smi => smi.EvaluateSleepQuality())).ReturnSuccess();
    }

    public class SleepStates : 
      GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State
    {
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State condition_transition;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State condition_transition_pre;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State uninterruptable;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State normal;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_noise;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_noise_transition;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_light;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_light_transition;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_scared;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_scared_transition;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_movement;
      public GameStateMachine<SleepChore.States, SleepChore.StatesInstance, SleepChore, object>.State interrupt_movement_transition;
    }
  }
}
