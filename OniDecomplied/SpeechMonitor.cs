// Decompiled with JetBrains decompiler
// Type: SpeechMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using UnityEngine;

public class SpeechMonitor : 
  GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>
{
  public GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State satisfied;
  public GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State talking;
  public static string PREFIX_SAD = "sad";
  public static string PREFIX_HAPPY = "happy";
  public static string PREFIX_SINGER = "sing";
  public StateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.TargetParameter mouth;
  private static HashedString HASH_SNAPTO_MOUTH = HashedString.op_Implicit("snapto_mouth");
  private static KAnimHashedString ANIM_HASH_HEAD_ANIM = KAnimHashedString.op_Implicit("head_anim");

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.root.Enter(new StateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State.Callback(SpeechMonitor.CreateMouth)).Exit(new StateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State.Callback(SpeechMonitor.DestroyMouth));
    this.satisfied.DoNothing();
    this.talking.Enter(new StateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State.Callback(SpeechMonitor.BeginTalking)).Update(new System.Action<SpeechMonitor.Instance, float>(SpeechMonitor.UpdateTalking), (UpdateRate) 0).Target(this.mouth).OnAnimQueueComplete(this.satisfied).Exit(new StateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.State.Callback(SpeechMonitor.EndTalking));
  }

  private static void CreateMouth(SpeechMonitor.Instance smi)
  {
    smi.mouth = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(MouthAnimation.ID)), (GameObject) null, (string) null).GetComponent<KBatchedAnimController>();
    ((Component) smi.mouth).gameObject.SetActive(true);
    smi.sm.mouth.Set(((Component) smi.mouth).gameObject, smi);
  }

  private static void DestroyMouth(SpeechMonitor.Instance smi)
  {
    if (!Object.op_Inequality((Object) smi.mouth, (Object) null))
      return;
    Util.KDestroyGameObject((Component) smi.mouth);
    smi.mouth = (KBatchedAnimController) null;
  }

  private static string GetRandomSpeechAnim(string speech_prefix) => speech_prefix + Random.Range(1, TuningData<SpeechMonitor.Tuning>.Get().speechCount).ToString();

  public static bool IsAllowedToPlaySpeech(GameObject go)
  {
    if (go.HasTag(GameTags.Dead))
      return false;
    KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
    KAnim.Anim currentAnim = component.GetCurrentAnim();
    if (currentAnim == null)
      return true;
    return GameAudioSheets.Get().IsAnimAllowedToPlaySpeech(currentAnim) && SpeechMonitor.CanOverrideHead(component);
  }

  private static bool CanOverrideHead(KBatchedAnimController kbac)
  {
    bool flag = true;
    KAnim.Anim currentAnim = kbac.GetCurrentAnim();
    if (currentAnim == null)
      return false;
    int currentFrameIndex = kbac.GetCurrentFrameIndex();
    if (currentFrameIndex <= 0)
      return false;
    KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(currentAnim.animFile.animBatchTag);
    KAnim.Anim.Frame frame = batchGroupData.GetFrame(currentFrameIndex);
    for (int index = 0; index < frame.numElements; ++index)
    {
      if (KAnimHashedString.op_Equality(batchGroupData.GetFrameElement(frame.firstElementIdx + index).folder, SpeechMonitor.ANIM_HASH_HEAD_ANIM))
      {
        flag = false;
        break;
      }
    }
    return flag;
  }

  public static void BeginTalking(SpeechMonitor.Instance smi)
  {
    ((EventInstance) ref smi.ev).clearHandle();
    if (smi.voiceEvent != null)
      smi.ev = VoiceSoundEvent.PlayVoice(smi.voiceEvent, smi.GetComponent<KBatchedAnimController>(), 0.0f, false);
    if (((EventInstance) ref smi.ev).isValid())
    {
      smi.mouth.Play(HashedString.op_Implicit(SpeechMonitor.GetRandomSpeechAnim(smi.speechPrefix)));
      smi.mouth.Queue(HashedString.op_Implicit(SpeechMonitor.GetRandomSpeechAnim(smi.speechPrefix)));
      smi.mouth.Queue(HashedString.op_Implicit(SpeechMonitor.GetRandomSpeechAnim(smi.speechPrefix)));
      smi.mouth.Queue(HashedString.op_Implicit(SpeechMonitor.GetRandomSpeechAnim(smi.speechPrefix)));
    }
    else
    {
      smi.mouth.Play(HashedString.op_Implicit(SpeechMonitor.GetRandomSpeechAnim(smi.speechPrefix)));
      smi.mouth.Queue(HashedString.op_Implicit(SpeechMonitor.GetRandomSpeechAnim(smi.speechPrefix)));
    }
    SpeechMonitor.UpdateTalking(smi, 0.0f);
  }

  public static void EndTalking(SpeechMonitor.Instance smi) => smi.GetComponent<SymbolOverrideController>().RemoveSymbolOverride(SpeechMonitor.HASH_SNAPTO_MOUTH, 3);

  public static KAnim.Anim.FrameElement GetFirstFrameElement(KBatchedAnimController controller)
  {
    KAnim.Anim.FrameElement firstFrameElement = new KAnim.Anim.FrameElement();
    firstFrameElement.symbol = KAnimHashedString.op_Implicit(HashedString.Invalid);
    int currentFrameIndex = controller.GetCurrentFrameIndex();
    KAnimBatch batch = controller.GetBatch();
    if (currentFrameIndex == -1 || batch == null)
      return firstFrameElement;
    KAnim.Anim.Frame frame = controller.GetBatch().group.data.GetFrame(currentFrameIndex);
    if (KAnim.Anim.Frame.op_Equality(frame, KAnim.Anim.Frame.InvalidFrame))
      return firstFrameElement;
    for (int index1 = 0; index1 < frame.numElements; ++index1)
    {
      int index2 = frame.firstElementIdx + index1;
      if (index2 < batch.group.data.frameElements.Count)
      {
        KAnim.Anim.FrameElement frameElement = batch.group.data.frameElements[index2];
        if (!KAnimHashedString.op_Equality(frameElement.symbol, HashedString.Invalid))
        {
          firstFrameElement = frameElement;
          break;
        }
      }
    }
    return firstFrameElement;
  }

  public static void UpdateTalking(SpeechMonitor.Instance smi, float dt)
  {
    if (((EventInstance) ref smi.ev).isValid())
    {
      PLAYBACK_STATE playbackState;
      ((EventInstance) ref smi.ev).getPlaybackState(ref playbackState);
      if (playbackState == 4 || playbackState == 2)
      {
        smi.GoTo((StateMachine.BaseState) smi.sm.satisfied);
        ((EventInstance) ref smi.ev).clearHandle();
        return;
      }
    }
    KAnim.Anim.FrameElement firstFrameElement = SpeechMonitor.GetFirstFrameElement(smi.mouth);
    if (KAnimHashedString.op_Equality(firstFrameElement.symbol, HashedString.Invalid))
      return;
    smi.Get<SymbolOverrideController>().AddSymbolOverride(SpeechMonitor.HASH_SNAPTO_MOUTH, smi.mouth.AnimFiles[0].GetData().build.GetSymbol(firstFrameElement.symbol), 3);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class Tuning : TuningData<SpeechMonitor.Tuning>
  {
    public float randomSpeechIntervalMin;
    public float randomSpeechIntervalMax;
    public int speechCount;
  }

  public new class Instance : 
    GameStateMachine<SpeechMonitor, SpeechMonitor.Instance, IStateMachineTarget, SpeechMonitor.Def>.GameInstance
  {
    public KBatchedAnimController mouth;
    public string speechPrefix = "happy";
    public string voiceEvent;
    public EventInstance ev;

    public Instance(IStateMachineTarget master, SpeechMonitor.Def def)
      : base(master, def)
    {
    }

    public bool IsPlayingSpeech() => this.IsInsideState((StateMachine.BaseState) this.sm.talking);

    public void PlaySpeech(string speech_prefix, string voice_event)
    {
      this.speechPrefix = speech_prefix;
      this.voiceEvent = voice_event;
      this.GoTo((StateMachine.BaseState) this.sm.talking);
    }

    public void DrawMouth()
    {
      KAnim.Anim.FrameElement firstFrameElement = SpeechMonitor.GetFirstFrameElement(this.smi.mouth);
      if (KAnimHashedString.op_Equality(firstFrameElement.symbol, HashedString.Invalid))
        return;
      KAnim.Build.Symbol symbol1 = this.smi.mouth.AnimFiles[0].GetData().build.GetSymbol(firstFrameElement.symbol);
      KBatchedAnimController component = this.GetComponent<KBatchedAnimController>();
      this.GetComponent<SymbolOverrideController>().AddSymbolOverride(SpeechMonitor.HASH_SNAPTO_MOUTH, this.smi.mouth.AnimFiles[0].GetData().build.GetSymbol(firstFrameElement.symbol), 3);
      KAnim.Build.Symbol symbol2 = KAnimBatchManager.Instance().GetBatchGroupData(component.batchGroupID).GetSymbol(KAnimHashedString.op_Implicit(SpeechMonitor.HASH_SNAPTO_MOUTH));
      KAnim.Build.SymbolFrameInstance symbolFrameInstance = KAnimBatchManager.Instance().GetBatchGroupData(symbol1.build.batchTag).symbolFrameInstances[symbol1.firstFrameIdx + firstFrameElement.frame];
      symbolFrameInstance.buildImageIdx = this.GetComponent<SymbolOverrideController>().GetAtlasIdx(symbol1.build.GetTexture(0));
      component.SetSymbolOverride(symbol2.firstFrameIdx, ref symbolFrameInstance);
    }
  }
}
