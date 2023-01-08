// Decompiled with JetBrains decompiler
// Type: ClusterMapBallisticAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ClusterMapBallisticAnimator : 
  GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer>
{
  public StateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.TargetParameter entityTarget;
  public GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.State launching;
  public GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.State moving;
  public GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.State landing;

  public override void InitializeStates(out StateMachine.BaseState defaultState)
  {
    defaultState = (StateMachine.BaseState) this.moving;
    this.root.Target(this.entityTarget).TagTransition(GameTags.BallisticEntityLaunching, this.launching).TagTransition(GameTags.BallisticEntityLanding, this.landing).TagTransition(GameTags.BallisticEntityMoving, this.moving);
    this.moving.Enter((StateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback) (smi => smi.PlayVisAnim("inflight_loop", (KAnim.PlayMode) 0)));
    this.landing.Enter((StateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback) (smi => smi.PlayVisAnim("landing", (KAnim.PlayMode) 0)));
    this.launching.Enter((StateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback) (smi => smi.PlayVisAnim("launching", (KAnim.PlayMode) 0)));
  }

  public class StatesInstance : 
    GameStateMachine<ClusterMapBallisticAnimator, ClusterMapBallisticAnimator.StatesInstance, ClusterMapVisualizer, object>.GameInstance
  {
    public ClusterGridEntity entity;
    private int animCompleteHandle = -1;
    private GameObject animCompleteSubscriber;

    public StatesInstance(ClusterMapVisualizer master, ClusterGridEntity entity)
      : base(master)
    {
      this.entity = entity;
      this.sm.entityTarget.Set((KMonoBehaviour) entity, this);
    }

    public void PlayVisAnim(string animName, KAnim.PlayMode playMode) => this.GetComponent<ClusterMapVisualizer>().PlayAnim(animName, playMode);

    public void ToggleVisAnim(bool on)
    {
      ClusterMapVisualizer component = this.GetComponent<ClusterMapVisualizer>();
      if (on)
        return;
      component.GetFirstAnimController().Play(HashedString.op_Implicit("grounded"));
    }

    public void SubscribeOnVisAnimComplete(System.Action<object> action)
    {
      ClusterMapVisualizer component = this.GetComponent<ClusterMapVisualizer>();
      this.UnsubscribeOnVisAnimComplete();
      this.animCompleteSubscriber = ((Component) component.GetFirstAnimController()).gameObject;
      this.animCompleteHandle = KMonoBehaviourExtensions.Subscribe(this.animCompleteSubscriber, -1061186183, action);
    }

    public void UnsubscribeOnVisAnimComplete()
    {
      if (this.animCompleteHandle == -1)
        return;
      DebugUtil.DevAssert(Object.op_Inequality((Object) this.animCompleteSubscriber, (Object) null), "ClustermapBallisticAnimator animCompleteSubscriber GameObject is null. Whatever the previous gameObject in this variable was, it may not have unsubscribed from an event properly", (Object) null);
      KMonoBehaviourExtensions.Unsubscribe(this.animCompleteSubscriber, this.animCompleteHandle);
      this.animCompleteHandle = -1;
    }

    protected override void OnCleanUp()
    {
      base.OnCleanUp();
      this.UnsubscribeOnVisAnimComplete();
    }
  }
}
