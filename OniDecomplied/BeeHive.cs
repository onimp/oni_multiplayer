// Decompiled with JetBrains decompiler
// Type: BeeHive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

public class BeeHive : 
  GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>
{
  public GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State disabled;
  public BeeHive.EnabledStates enabled;
  public StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.FloatParameter hiveGrowth = new StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.FloatParameter(1f);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    default_state = (StateMachine.BaseState) this.enabled.grownStates;
    this.root.DoTutorial(Tutorial.TutorialMessages.TM_Radiation).Enter((StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State.Callback) (smi =>
    {
      AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(smi.gameObject);
      if (amountInstance == null)
        return;
      amountInstance.hide = true;
    })).EventHandler(GameHashes.Died, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State.Callback) (smi =>
    {
      PrimaryElement component1 = smi.GetComponent<PrimaryElement>();
      Storage component2 = smi.GetComponent<Storage>();
      byte index = Db.Get().Diseases.GetIndex(Db.Get().Diseases.RadiationPoisoning.id);
      component2.AddOre(SimHashes.NuclearWaste, BeeHiveTuning.WASTE_DROPPED_ON_DEATH, component1.Temperature, index, BeeHiveTuning.GERMS_DROPPED_ON_DEATH);
      component2.DropAll(smi.master.transform.position, true, true, new Vector3());
    }));
    this.disabled.ToggleTag(GameTags.Creatures.Behaviours.DisableCreature).EventTransition(GameHashes.FoundationChanged, (GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State) this.enabled, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Transition.ConditionCallback) (smi => !smi.IsDisabled())).EventTransition(GameHashes.EntombedChanged, (GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State) this.enabled, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Transition.ConditionCallback) (smi => !smi.IsDisabled())).EventTransition(GameHashes.EnteredBreathableArea, (GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State) this.enabled, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Transition.ConditionCallback) (smi => !smi.IsDisabled()));
    this.enabled.EventTransition(GameHashes.FoundationChanged, this.disabled, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Transition.ConditionCallback) (smi => smi.IsDisabled())).EventTransition(GameHashes.EntombedChanged, this.disabled, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Transition.ConditionCallback) (smi => smi.IsDisabled())).EventTransition(GameHashes.Drowning, this.disabled, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Transition.ConditionCallback) (smi => smi.IsDisabled())).DefaultState((GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State) this.enabled.grownStates);
    this.enabled.growingStates.ParamTransition<float>((StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Parameter<float>) this.hiveGrowth, (GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State) this.enabled.grownStates, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Parameter<float>.Callback) ((smi, f) => (double) f >= 1.0)).DefaultState(this.enabled.growingStates.idle);
    this.enabled.growingStates.idle.Update((System.Action<BeeHive.StatesInstance, float>) ((smi, dt) => smi.DeltaGrowth(dt / 600f / BeeHiveTuning.HIVE_GROWTH_TIME)), (UpdateRate) 7);
    this.enabled.grownStates.ParamTransition<float>((StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Parameter<float>) this.hiveGrowth, (GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State) this.enabled.growingStates, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Parameter<float>.Callback) ((smi, f) => (double) f < 1.0)).DefaultState(this.enabled.grownStates.dayTime);
    this.enabled.grownStates.dayTime.EventTransition(GameHashes.Nighttime, (Func<BeeHive.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.enabled.grownStates.nightTime, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Transition.ConditionCallback) (smi => GameClock.Instance.IsNighttime()));
    this.enabled.grownStates.nightTime.EventTransition(GameHashes.NewDay, (Func<BeeHive.StatesInstance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.enabled.grownStates.dayTime, (StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.Transition.ConditionCallback) (smi => (double) GameClock.Instance.GetTimeSinceStartOfCycle() <= 1.0)).Exit((StateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State.Callback) (smi =>
    {
      if (GameClock.Instance.IsNighttime())
        return;
      smi.SpawnNewLarvaFromHive();
    }));
  }

  public class Def : StateMachine.BaseDef
  {
    public string beePrefabID;
    public string larvaPrefabID;
  }

  public class GrowingStates : 
    GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State
  {
    public GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State idle;
  }

  public class GrownStates : 
    GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State
  {
    public GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State dayTime;
    public GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State nightTime;
  }

  public class EnabledStates : 
    GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.State
  {
    public BeeHive.GrowingStates growingStates;
    public BeeHive.GrownStates grownStates;
  }

  public class StatesInstance : 
    GameStateMachine<BeeHive, BeeHive.StatesInstance, IStateMachineTarget, BeeHive.Def>.GameInstance
  {
    public StatesInstance(IStateMachineTarget master, BeeHive.Def def)
      : base(master, def)
    {
      this.Subscribe(1119167081, new System.Action<object>(this.OnNewGameSpawn));
      Components.BeeHives.Add(this);
    }

    public void SetUpNewHive()
    {
      double num = (double) this.sm.hiveGrowth.Set(0.0f, this);
    }

    protected override void OnCleanUp()
    {
      Components.BeeHives.Remove(this);
      base.OnCleanUp();
    }

    private void OnNewGameSpawn(object data) => this.NewGamePopulateHive();

    private void NewGamePopulateHive()
    {
      int num1 = 1;
      for (int index = 0; index < num1; ++index)
        this.SpawnNewBeeFromHive();
      int num2 = 1;
      for (int index = 0; index < num2; ++index)
        this.SpawnNewLarvaFromHive();
    }

    public bool IsFullyGrown() => (double) this.sm.hiveGrowth.Get(this) >= 1.0;

    public void DeltaGrowth(float delta)
    {
      float num1 = this.sm.hiveGrowth.Get(this) + delta;
      double num2 = (double) Mathf.Clamp01(num1);
      double num3 = (double) this.sm.hiveGrowth.Set(num1, this);
    }

    public void SpawnNewLarvaFromHive() => Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(this.def.larvaPrefabID)), TransformExtensions.GetPosition(this.transform)).SetActive(true);

    public void SpawnNewBeeFromHive() => Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(this.def.beePrefabID)), TransformExtensions.GetPosition(this.transform)).SetActive(true);

    public bool IsDisabled()
    {
      KPrefabID component = this.GetComponent<KPrefabID>();
      return component.HasTag(GameTags.Creatures.HasNoFoundation) || component.HasTag(GameTags.Entombed) || component.HasTag(GameTags.Creatures.Drowning);
    }
  }
}
