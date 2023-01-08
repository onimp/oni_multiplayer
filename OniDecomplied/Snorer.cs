// Decompiled with JetBrains decompiler
// Type: Snorer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class Snorer : StateMachineComponent<Snorer.StatesInstance>
{
  private static readonly HashedString HeadHash = HashedString.op_Implicit("snapTo_mouth");

  protected virtual void OnSpawn() => this.smi.StartSM();

  public class StatesInstance : 
    GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.GameInstance
  {
    private SchedulerHandle snoreHandle;
    private KBatchedAnimController snoreEffect;
    private KBatchedAnimController snoreBGEffect;
    private const float BGEmissionRadius = 3f;

    public StatesInstance(Snorer master)
      : base(master)
    {
    }

    public bool IsSleeping()
    {
      StaminaMonitor.Instance smi = ((Component) this.master).GetSMI<StaminaMonitor.Instance>();
      return smi != null && smi.IsSleeping();
    }

    public void StartSmallSnore() => this.snoreHandle = GameScheduler.Instance.Schedule("snorelines", 2f, new Action<object>(this.StartSmallSnoreInternal), (object) null, (SchedulerGroup) null);

    private void StartSmallSnoreInternal(object data)
    {
      this.snoreHandle.ClearScheduler();
      bool symbolVisible;
      Matrix4x4 symbolTransform = ((Component) this.smi.master).GetComponent<KBatchedAnimController>().GetSymbolTransform(Snorer.HeadHash, out symbolVisible);
      if (!symbolVisible)
        return;
      Vector3 position = Vector4.op_Implicit(((Matrix4x4) ref symbolTransform).GetColumn(3));
      position.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
      this.snoreEffect = FXHelpers.CreateEffect("snore_fx_kanim", position);
      this.snoreEffect.destroyOnAnimComplete = true;
      this.snoreEffect.Play(HashedString.op_Implicit("snore"), (KAnim.PlayMode) 0);
    }

    public void StopSmallSnore()
    {
      this.snoreHandle.ClearScheduler();
      if (Object.op_Inequality((Object) this.snoreEffect, (Object) null))
        this.snoreEffect.PlayMode = (KAnim.PlayMode) 1;
      this.snoreEffect = (KBatchedAnimController) null;
    }

    public void StartSnoreBGEffect() => AcousticDisturbance.Emit((object) ((Component) this.smi.master).gameObject, 3);

    public void StopSnoreBGEffect()
    {
    }
  }

  public class States : GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer>
  {
    public GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State idle;
    public Snorer.States.SleepStates sleeping;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.root.TagTransition(GameTags.Dead, (GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State) null);
      this.idle.Transition((GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State) this.sleeping, (StateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.Transition.ConditionCallback) (smi => smi.IsSleeping()));
      this.sleeping.DefaultState(this.sleeping.quiet).Enter((StateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State.Callback) (smi => smi.StartSmallSnore())).Exit((StateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State.Callback) (smi => smi.StopSmallSnore())).Transition(this.idle, (StateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.Transition.ConditionCallback) (smi => !((Component) smi.master).GetSMI<StaminaMonitor.Instance>().IsSleeping()));
      this.sleeping.quiet.Enter("ScheduleNextSnore", (StateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State.Callback) (smi => smi.ScheduleGoTo(this.GetNewInterval(), (StateMachine.BaseState) this.sleeping.snoring)));
      this.sleeping.snoring.Enter((StateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State.Callback) (smi => smi.StartSnoreBGEffect())).ToggleExpression(Db.Get().Expressions.Relief).ScheduleGoTo(3f, (StateMachine.BaseState) this.sleeping.quiet).Exit((StateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State.Callback) (smi => smi.StopSnoreBGEffect()));
    }

    private float GetNewInterval() => Mathf.Min(Mathf.Max(Util.GaussianRandom(5f, 1f), 3f), 10f);

    public class SleepStates : 
      GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State
    {
      public GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State quiet;
      public GameStateMachine<Snorer.States, Snorer.StatesInstance, Snorer, object>.State snoring;
    }
  }
}
