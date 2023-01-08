// Decompiled with JetBrains decompiler
// Type: FXAnim
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FXAnim : GameStateMachine<FXAnim, FXAnim.Instance>
{
  public StateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget, object>.TargetParameter fx;
  public GameStateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget, object>.State loop;
  public GameStateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget, object>.State restart;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.loop;
    this.Target(this.fx);
    this.loop.Enter((StateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.Enter())).EventTransition(GameHashes.AnimQueueComplete, this.restart).Exit("Post", (StateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.Exit()));
    this.restart.GoTo(this.loop);
  }

  public new class Instance : 
    GameStateMachine<FXAnim, FXAnim.Instance, IStateMachineTarget, object>.GameInstance
  {
    private string anim;
    private KAnim.PlayMode mode;
    private KBatchedAnimController animController;

    public Instance(
      IStateMachineTarget master,
      string kanim_file,
      string anim,
      KAnim.PlayMode mode,
      Vector3 offset,
      Color32 tint_colour)
      : base(master)
    {
      this.animController = FXHelpers.CreateEffect(kanim_file, Vector3.op_Addition(TransformExtensions.GetPosition(this.smi.master.transform), offset), this.smi.master.transform);
      KMonoBehaviourExtensions.Subscribe(((Component) this.animController).gameObject, -1061186183, new System.Action<object>(this.OnAnimQueueComplete));
      this.animController.TintColour = tint_colour;
      this.sm.fx.Set(((Component) this.animController).gameObject, this.smi);
      this.anim = anim;
      this.mode = mode;
    }

    public void Enter() => this.animController.Play(HashedString.op_Implicit(this.anim), this.mode);

    public void Exit() => this.DestroyFX();

    private void OnAnimQueueComplete(object data) => this.DestroyFX();

    private void DestroyFX() => Util.KDestroyGameObject(this.sm.fx.Get(this.smi));
  }
}
