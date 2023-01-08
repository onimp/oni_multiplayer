// Decompiled with JetBrains decompiler
// Type: InspirationEffectMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class InspirationEffectMonitor : 
  GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>
{
  public StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.BoolParameter shouldCatchyTune;
  public StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.FloatParameter inspirationTimeRemaining;
  public GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.State idle;
  public GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.State catchyTune;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    default_state = (StateMachine.BaseState) this.idle;
    this.idle.EventHandler(GameHashes.CatchyTune, new GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.GameEvent.Callback(this.OnCatchyTune)).ParamTransition<bool>((StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.Parameter<bool>) this.shouldCatchyTune, this.catchyTune, (StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.Parameter<bool>.Callback) ((smi, shouldCatchyTune) => shouldCatchyTune));
    this.catchyTune.Exit((StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.State.Callback) (smi => this.shouldCatchyTune.Set(false, smi))).ToggleEffect("HeardJoySinger").ToggleThought(Db.Get().Thoughts.CatchyTune).EventHandler(GameHashes.StartWork, new GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.GameEvent.Callback(this.TryThinkCatchyTune)).Enter((StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.State.Callback) (smi => this.SingCatchyTune(smi))).Update((System.Action<InspirationEffectMonitor.Instance, float>) ((smi, dt) =>
    {
      this.TryThinkCatchyTune(smi, (object) null);
      double num = (double) this.inspirationTimeRemaining.Delta(-dt, smi);
    }), (UpdateRate) 7).ParamTransition<float>((StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.Parameter<float>) this.inspirationTimeRemaining, this.idle, (StateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.Parameter<float>.Callback) ((smi, p) => (double) p <= 0.0));
  }

  private void OnCatchyTune(InspirationEffectMonitor.Instance smi, object data)
  {
    double num = (double) this.inspirationTimeRemaining.Set(600f, smi);
    this.shouldCatchyTune.Set(true, smi);
  }

  private void TryThinkCatchyTune(InspirationEffectMonitor.Instance smi, object data)
  {
    if (Random.Range(1, 101) <= 66)
      return;
    this.SingCatchyTune(smi);
  }

  private void SingCatchyTune(InspirationEffectMonitor.Instance smi)
  {
    smi.master.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.CatchyTune);
    if (smi.GetSpeechMonitor().IsPlayingSpeech() || !SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
      return;
    smi.GetSpeechMonitor().PlaySpeech(Db.Get().Thoughts.CatchyTune.speechPrefix, Db.Get().Thoughts.CatchyTune.sound);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<InspirationEffectMonitor, InspirationEffectMonitor.Instance, IStateMachineTarget, InspirationEffectMonitor.Def>.GameInstance
  {
    public SpeechMonitor.Instance speechMonitor;

    public Instance(IStateMachineTarget master, InspirationEffectMonitor.Def def)
      : base(master, def)
    {
    }

    public SpeechMonitor.Instance GetSpeechMonitor()
    {
      if (this.speechMonitor == null)
        this.speechMonitor = this.master.gameObject.GetSMI<SpeechMonitor.Instance>();
      return this.speechMonitor;
    }
  }
}
