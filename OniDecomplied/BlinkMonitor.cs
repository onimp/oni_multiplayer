// Decompiled with JetBrains decompiler
// Type: BlinkMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class BlinkMonitor : 
  GameStateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>
{
  public GameStateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.State satisfied;
  public GameStateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.State blinking;
  public StateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.TargetParameter eyes;
  private static HashedString HASH_SNAPTO_EYES = HashedString.op_Implicit("snapto_eyes");

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.root.Enter(new StateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.State.Callback(BlinkMonitor.CreateEyes)).Exit(new StateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.State.Callback(BlinkMonitor.DestroyEyes));
    this.satisfied.ScheduleGoTo(new Func<BlinkMonitor.Instance, float>(BlinkMonitor.GetRandomBlinkTime), (StateMachine.BaseState) this.blinking);
    this.blinking.EnterTransition(this.satisfied, GameStateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.Not(new StateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.Transition.ConditionCallback(BlinkMonitor.CanBlink))).Enter(new StateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.State.Callback(BlinkMonitor.BeginBlinking)).Update(new System.Action<BlinkMonitor.Instance, float>(BlinkMonitor.UpdateBlinking), (UpdateRate) 0).Target(this.eyes).OnAnimQueueComplete(this.satisfied).Exit(new StateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.State.Callback(BlinkMonitor.EndBlinking));
  }

  private static bool CanBlink(BlinkMonitor.Instance smi) => SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject) && smi.Get<Navigator>().CurrentNavType != NavType.Ladder;

  private static float GetRandomBlinkTime(BlinkMonitor.Instance smi) => Random.Range(TuningData<BlinkMonitor.Tuning>.Get().randomBlinkIntervalMin, TuningData<BlinkMonitor.Tuning>.Get().randomBlinkIntervalMax);

  private static void CreateEyes(BlinkMonitor.Instance smi)
  {
    smi.eyes = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EyeAnimation.ID)), (GameObject) null, (string) null).GetComponent<KBatchedAnimController>();
    ((Component) smi.eyes).gameObject.SetActive(true);
    smi.sm.eyes.Set(((Component) smi.eyes).gameObject, smi);
  }

  private static void DestroyEyes(BlinkMonitor.Instance smi)
  {
    if (!Object.op_Inequality((Object) smi.eyes, (Object) null))
      return;
    Util.KDestroyGameObject((Component) smi.eyes);
    smi.eyes = (KBatchedAnimController) null;
  }

  public static void BeginBlinking(BlinkMonitor.Instance smi)
  {
    string str = "eyes1";
    smi.eyes.Play(HashedString.op_Implicit(str));
    BlinkMonitor.UpdateBlinking(smi, 0.0f);
  }

  public static void EndBlinking(BlinkMonitor.Instance smi) => smi.GetComponent<SymbolOverrideController>().RemoveSymbolOverride(BlinkMonitor.HASH_SNAPTO_EYES, 3);

  public static void UpdateBlinking(BlinkMonitor.Instance smi, float dt)
  {
    int currentFrameIndex = smi.eyes.GetCurrentFrameIndex();
    KAnimBatch batch = smi.eyes.GetBatch();
    if (currentFrameIndex == -1 || batch == null)
      return;
    KAnim.Anim.Frame frame = smi.eyes.GetBatch().group.data.GetFrame(currentFrameIndex);
    if (KAnim.Anim.Frame.op_Equality(frame, KAnim.Anim.Frame.InvalidFrame))
      return;
    HashedString hashedString = HashedString.Invalid;
    for (int index1 = 0; index1 < frame.numElements; ++index1)
    {
      int index2 = frame.firstElementIdx + index1;
      if (index2 < batch.group.data.frameElements.Count)
      {
        KAnim.Anim.FrameElement frameElement = batch.group.data.frameElements[index2];
        if (!KAnimHashedString.op_Equality(frameElement.symbol, HashedString.Invalid))
        {
          hashedString = HashedString.op_Implicit(frameElement.symbol);
          break;
        }
      }
    }
    smi.GetComponent<SymbolOverrideController>().AddSymbolOverride(BlinkMonitor.HASH_SNAPTO_EYES, smi.eyes.AnimFiles[0].GetData().build.GetSymbol(KAnimHashedString.op_Implicit(hashedString)), 3);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class Tuning : TuningData<BlinkMonitor.Tuning>
  {
    public float randomBlinkIntervalMin;
    public float randomBlinkIntervalMax;
  }

  public new class Instance : 
    GameStateMachine<BlinkMonitor, BlinkMonitor.Instance, IStateMachineTarget, BlinkMonitor.Def>.GameInstance
  {
    public KBatchedAnimController eyes;

    public Instance(IStateMachineTarget master, BlinkMonitor.Def def)
      : base(master, def)
    {
    }

    public bool IsBlinking() => this.IsInsideState((StateMachine.BaseState) this.sm.blinking);

    public void Blink() => this.GoTo((StateMachine.BaseState) this.sm.blinking);
  }
}
