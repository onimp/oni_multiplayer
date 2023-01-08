// Decompiled with JetBrains decompiler
// Type: AnimInterruptMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class AnimInterruptMonitor : 
  GameStateMachine<AnimInterruptMonitor, AnimInterruptMonitor.Instance, IStateMachineTarget, AnimInterruptMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.Creatures.Behaviours.PlayInterruptAnim, new StateMachine<AnimInterruptMonitor, AnimInterruptMonitor.Instance, IStateMachineTarget, AnimInterruptMonitor.Def>.Transition.ConditionCallback(AnimInterruptMonitor.ShoulPlayAnim), new System.Action<AnimInterruptMonitor.Instance>(AnimInterruptMonitor.ClearAnim));
  }

  private static bool ShoulPlayAnim(AnimInterruptMonitor.Instance smi) => smi.anims != null;

  private static void ClearAnim(AnimInterruptMonitor.Instance smi) => smi.anims = (HashedString[]) null;

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<AnimInterruptMonitor, AnimInterruptMonitor.Instance, IStateMachineTarget, AnimInterruptMonitor.Def>.GameInstance
  {
    public HashedString[] anims;

    public Instance(IStateMachineTarget master, AnimInterruptMonitor.Def def)
      : base(master, def)
    {
    }

    public void PlayAnim(HashedString anim) => this.PlayAnimSequence(new HashedString[1]
    {
      anim
    });

    public void PlayAnimSequence(HashedString[] anims)
    {
      this.anims = anims;
      this.GetComponent<CreatureBrain>().UpdateBrain();
    }
  }
}
