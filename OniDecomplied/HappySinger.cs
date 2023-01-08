// Decompiled with JetBrains decompiler
// Type: HappySinger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class HappySinger : GameStateMachine<HappySinger, HappySinger.Instance>
{
  private Vector3 offset = new Vector3(0.0f, 0.0f, 0.1f);
  public GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State neutral;
  public HappySinger.OverjoyedStates overjoyed;
  public string soundPath = GlobalAssets.GetSound("DupeSinging_NotesFX_LP");

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.neutral;
    this.root.TagTransition(GameTags.Dead, (GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State) null);
    this.neutral.TagTransition(GameTags.Overjoyed, (GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State) this.overjoyed);
    this.overjoyed.DefaultState(this.overjoyed.idle).TagTransition(GameTags.Overjoyed, this.neutral, true).ToggleEffect("IsJoySinger").ToggleLoopingSound(this.soundPath).ToggleAnims("anim_loco_singer_kanim").ToggleAnims("anim_idle_singer_kanim").EventHandler(GameHashes.TagsChanged, (GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.GameEvent.Callback) ((smi, obj) => smi.musicParticleFX.SetActive(!smi.HasTag(GameTags.Asleep)))).Enter((StateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      smi.musicParticleFX = Util.KInstantiate(EffectPrefabs.Instance.HappySingerFX, Vector3.op_Addition(TransformExtensions.GetPosition(smi.master.transform), this.offset));
      smi.musicParticleFX.transform.SetParent(smi.master.transform);
      smi.CreatePasserbyReactable();
      smi.musicParticleFX.SetActive(!smi.HasTag(GameTags.Asleep));
    })).Update((System.Action<HappySinger.Instance, float>) ((smi, dt) =>
    {
      if (smi.GetSpeechMonitor().IsPlayingSpeech() || !SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
        return;
      smi.GetSpeechMonitor().PlaySpeech(Db.Get().Thoughts.CatchyTune.speechPrefix, Db.Get().Thoughts.CatchyTune.sound);
    }), (UpdateRate) 6).Exit((StateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      Util.KDestroyGameObject(smi.musicParticleFX);
      smi.ClearPasserbyReactable();
      smi.musicParticleFX.SetActive(false);
    }));
  }

  public class OverjoyedStates : 
    GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State idle;
    public GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.State moving;
  }

  public new class Instance : 
    GameStateMachine<HappySinger, HappySinger.Instance, IStateMachineTarget, object>.GameInstance
  {
    private Reactable passerbyReactable;
    public GameObject musicParticleFX;
    public SpeechMonitor.Instance speechMonitor;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public void CreatePasserbyReactable()
    {
      if (this.passerbyReactable != null)
        return;
      EmoteReactable emoteReactable = new EmoteReactable(this.gameObject, HashedString.op_Implicit("WorkPasserbyAcknowledgement"), Db.Get().ChoreTypes.Emote, 5, 5, localCooldown: 600f);
      Emote sing = Db.Get().Emotes.Minion.Sing;
      emoteReactable.SetEmote(sing).SetThought(Db.Get().Thoughts.CatchyTune).AddPrecondition(new Reactable.ReactablePrecondition(this.ReactorIsOnFloor));
      emoteReactable.RegisterEmoteStepCallbacks(HashedString.op_Implicit("react"), new System.Action<GameObject>(this.AddReactionEffect), (System.Action<GameObject>) null);
      this.passerbyReactable = (Reactable) emoteReactable;
    }

    public SpeechMonitor.Instance GetSpeechMonitor()
    {
      if (this.speechMonitor == null)
        this.speechMonitor = this.master.gameObject.GetSMI<SpeechMonitor.Instance>();
      return this.speechMonitor;
    }

    private void AddReactionEffect(GameObject reactor) => EventExtensions.Trigger(reactor, -1278274506, (object) null);

    private bool ReactorIsOnFloor(GameObject reactor, Navigator.ActiveTransition transition) => transition.end == NavType.Floor;

    public void ClearPasserbyReactable()
    {
      if (this.passerbyReactable == null)
        return;
      this.passerbyReactable.Cleanup();
      this.passerbyReactable = (Reactable) null;
    }
  }
}
