// Decompiled with JetBrains decompiler
// Type: ClusterMapFXAnimator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ClusterMapFXAnimator : 
  GameStateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer>
{
  private KBatchedAnimController animController;
  public StateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.TargetParameter entityTarget;
  public GameStateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.State play;
  public GameStateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.State finished;
  public StateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.Signal onAnimComplete;

  public override void InitializeStates(out StateMachine.BaseState defaultState)
  {
    defaultState = (StateMachine.BaseState) this.play;
    this.play.OnSignal(this.onAnimComplete, this.finished);
    this.finished.Enter((StateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.State.Callback) (smi => smi.DestroyEntity()));
  }

  public class StatesInstance : 
    GameStateMachine<ClusterMapFXAnimator, ClusterMapFXAnimator.StatesInstance, ClusterMapVisualizer, object>.GameInstance
  {
    public StatesInstance(ClusterMapVisualizer visualizer, ClusterGridEntity entity)
      : base(visualizer)
    {
      this.sm.entityTarget.Set((KMonoBehaviour) entity, this);
      KMonoBehaviourExtensions.Subscribe(((Component) visualizer.GetFirstAnimController()).gameObject, -1061186183, new System.Action<object>(this.OnAnimQueueComplete));
    }

    private void OnAnimQueueComplete(object data) => this.sm.onAnimComplete.Trigger(this);

    public void DestroyEntity() => TracesExtesions.DeleteObject(this.sm.entityTarget.Get(this));
  }
}
