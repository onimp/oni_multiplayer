// Decompiled with JetBrains decompiler
// Type: EmoteReactable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

public class EmoteReactable : Reactable
{
  private KBatchedAnimController kbac;
  public Expression expression;
  public Thought thought;
  public Emote emote;
  private HandleVector<EmoteStep.Callbacks>.Handle[] callbackHandles;
  protected KAnimFile overrideAnimSet;
  private int currentStep = -1;
  private float elapsed;

  public EmoteReactable(
    GameObject gameObject,
    HashedString id,
    ChoreType chore_type,
    int range_width = 15,
    int range_height = 8,
    float globalCooldown = 0.0f,
    float localCooldown = 20f,
    float lifeSpan = float.PositiveInfinity,
    float max_initial_delay = 0.0f)
    : base(gameObject, id, chore_type, range_width, range_height, true, globalCooldown, localCooldown, lifeSpan, max_initial_delay)
  {
  }

  public EmoteReactable SetEmote(Emote emote)
  {
    this.emote = emote;
    return this;
  }

  public EmoteReactable RegisterEmoteStepCallbacks(
    HashedString stepName,
    Action<GameObject> startedCb,
    Action<GameObject> finishedCb)
  {
    if (this.callbackHandles == null)
      this.callbackHandles = new HandleVector<EmoteStep.Callbacks>.Handle[this.emote.StepCount];
    int stepIndex = this.emote.GetStepIndex(stepName);
    this.callbackHandles[stepIndex] = this.emote[stepIndex].RegisterCallbacks(startedCb, finishedCb);
    return this;
  }

  public EmoteReactable SetExpression(Expression expression)
  {
    this.expression = expression;
    return this;
  }

  public EmoteReactable SetThought(Thought thought)
  {
    this.thought = thought;
    return this;
  }

  public EmoteReactable SetOverideAnimSet(string animSet)
  {
    this.overrideAnimSet = Assets.GetAnim(HashedString.op_Implicit(animSet));
    return this;
  }

  public override bool InternalCanBegin(
    GameObject new_reactor,
    Navigator.ActiveTransition transition)
  {
    if (Object.op_Inequality((Object) this.reactor, (Object) null) || Object.op_Equality((Object) new_reactor, (Object) null))
      return false;
    Navigator component = new_reactor.GetComponent<Navigator>();
    return !Object.op_Equality((Object) component, (Object) null) && component.IsMoving() && (-257 & 1 << (int) (component.CurrentNavType & (NavType) 31)) != 0 && Object.op_Inequality((Object) this.gameObject, (Object) new_reactor);
  }

  public override void Update(float dt)
  {
    if (this.emote == null || !this.emote.IsValidStep(this.currentStep))
      return;
    if (Object.op_Inequality((Object) this.gameObject, (Object) null) && Object.op_Inequality((Object) this.reactor, (Object) null))
    {
      Facing component = this.reactor.GetComponent<Facing>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.Face(TransformExtensions.GetPosition(this.gameObject.transform));
    }
    float timeout = this.emote[this.currentStep].timeout;
    if ((double) timeout > 0.0 && (double) timeout < (double) this.elapsed)
      this.NextStep(HashedString.op_Implicit((string) null));
    else
      this.elapsed += dt;
  }

  protected override void InternalBegin()
  {
    this.kbac = this.reactor.GetComponent<KBatchedAnimController>();
    this.emote.ApplyAnimOverrides(this.kbac, this.overrideAnimSet);
    if (this.expression != null)
      this.reactor.GetComponent<FaceGraph>().AddExpression(this.expression);
    if (this.thought != null)
      this.reactor.GetSMI<ThoughtGraph.Instance>().AddThought(this.thought);
    this.NextStep(HashedString.op_Implicit((string) null));
  }

  protected override void InternalEnd()
  {
    if (Object.op_Inequality((Object) this.kbac, (Object) null))
    {
      this.kbac.onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.NextStep);
      this.emote.RemoveAnimOverrides(this.kbac, this.overrideAnimSet);
      this.kbac = (KBatchedAnimController) null;
    }
    if (Object.op_Inequality((Object) this.reactor, (Object) null))
    {
      if (this.expression != null)
        this.reactor.GetComponent<FaceGraph>().RemoveExpression(this.expression);
      if (this.thought != null)
        this.reactor.GetSMI<ThoughtGraph.Instance>().RemoveThought(this.thought);
    }
    this.currentStep = -1;
  }

  protected override void InternalCleanup()
  {
    if (this.emote == null || this.callbackHandles == null)
      return;
    for (int stepIdx = 0; this.emote.IsValidStep(stepIdx); ++stepIdx)
      this.emote[stepIdx].UnregisterCallbacks(this.callbackHandles[stepIdx]);
  }

  private void NextStep(HashedString finishedAnim)
  {
    if (this.emote.IsValidStep(this.currentStep) && (double) this.emote[this.currentStep].timeout <= 0.0)
    {
      this.kbac.onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.NextStep);
      if (this.callbackHandles != null)
        this.emote[this.currentStep].OnStepFinished(this.callbackHandles[this.currentStep], this.reactor);
    }
    ++this.currentStep;
    if (!this.emote.IsValidStep(this.currentStep) || Object.op_Equality((Object) this.kbac, (Object) null))
    {
      this.End();
    }
    else
    {
      EmoteStep emoteStep = this.emote[this.currentStep];
      if (HashedString.op_Inequality(emoteStep.anim, HashedString.Invalid))
      {
        this.kbac.Play(emoteStep.anim, emoteStep.mode);
        if (this.kbac.IsStopped())
          emoteStep.timeout = 0.25f;
      }
      if ((double) emoteStep.timeout <= 0.0)
        this.kbac.onAnimComplete += new KAnimControllerBase.KAnimEvent(this.NextStep);
      else
        this.elapsed = 0.0f;
      if (this.callbackHandles == null)
        return;
      emoteStep.OnStepStarted(this.callbackHandles[this.currentStep], this.reactor);
    }
  }
}
