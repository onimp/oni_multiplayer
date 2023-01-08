// Decompiled with JetBrains decompiler
// Type: JoyBehaviourMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using TUNING;
using UnityEngine;

public class JoyBehaviourMonitor : 
  GameStateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance>
{
  public StateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance, IStateMachineTarget, object>.Signal exitEarly;
  public GameStateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance, IStateMachineTarget, object>.State neutral;
  public GameStateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance, IStateMachineTarget, object>.State overjoyed;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.neutral;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.root.TagTransition(GameTags.Dead, (GameStateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance, IStateMachineTarget, object>.State) null);
    this.neutral.EventHandler(GameHashes.TagsChanged, (GameStateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance, IStateMachineTarget, object>.GameEvent.Callback) ((smi, data) =>
    {
      TagChangedEventData changedEventData = (TagChangedEventData) data;
      if (!changedEventData.added)
        return;
      if (Tag.op_Equality(changedEventData.tag, GameTags.PleasantConversation) && (double) Random.Range(0.0f, 100f) <= 1.0)
        smi.GoToOverjoyed();
      smi.GetComponent<KPrefabID>().RemoveTag(GameTags.PleasantConversation);
    })).EventHandler(GameHashes.SleepFinished, (StateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      if (!smi.ShouldBeOverjoyed())
        return;
      smi.GoToOverjoyed();
    }));
    this.overjoyed.Transition(this.neutral, (StateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => (double) GameClock.Instance.GetTime() >= (double) smi.transitionTime)).ToggleExpression((Func<JoyBehaviourMonitor.Instance, Expression>) (smi => smi.happyExpression)).ToggleAnims((Func<JoyBehaviourMonitor.Instance, HashedString>) (smi => HashedString.op_Implicit(smi.happyLocoAnim))).ToggleAnims((Func<JoyBehaviourMonitor.Instance, HashedString>) (smi => HashedString.op_Implicit(smi.happyLocoWalkAnim))).ToggleTag(GameTags.Overjoyed).Exit((StateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.GetComponent<KPrefabID>().RemoveTag(GameTags.PleasantConversation))).OnSignal(this.exitEarly, this.neutral);
  }

  public new class Instance : 
    GameStateMachine<JoyBehaviourMonitor, JoyBehaviourMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public string happyLocoAnim = "";
    public string happyLocoWalkAnim = "";
    public Expression happyExpression;
    [Serialize]
    public float transitionTime;
    private AttributeInstance expectationAttribute;
    private AttributeInstance qolAttribute;

    public Instance(
      IStateMachineTarget master,
      string happy_loco_anim,
      string happy_loco_walk_anim,
      Expression happy_expression)
      : base(master)
    {
      this.happyLocoAnim = happy_loco_anim;
      this.happyLocoWalkAnim = happy_loco_walk_anim;
      this.happyExpression = happy_expression;
      this.expectationAttribute = this.gameObject.GetAttributes().Add(Db.Get().Attributes.QualityOfLifeExpectation);
      this.qolAttribute = Db.Get().Attributes.QualityOfLife.Lookup(this.gameObject);
    }

    public bool ShouldBeOverjoyed()
    {
      float num1 = this.qolAttribute.GetTotalValue() - this.expectationAttribute.GetTotalValue();
      if ((double) num1 < (double) TRAITS.JOY_REACTIONS.MIN_MORALE_EXCESS)
        return false;
      float num2 = MathUtil.ReRange(num1, TRAITS.JOY_REACTIONS.MIN_MORALE_EXCESS, TRAITS.JOY_REACTIONS.MAX_MORALE_EXCESS, TRAITS.JOY_REACTIONS.MIN_REACTION_CHANCE, TRAITS.JOY_REACTIONS.MAX_REACTION_CHANCE);
      return (double) Random.Range(0.0f, 100f) <= (double) num2;
    }

    public void GoToOverjoyed()
    {
      this.smi.transitionTime = GameClock.Instance.GetTime() + TRAITS.JOY_REACTIONS.JOY_REACTION_DURATION;
      this.smi.GoTo((StateMachine.BaseState) this.smi.sm.overjoyed);
    }
  }
}
