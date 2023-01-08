// Decompiled with JetBrains decompiler
// Type: TransitionDriver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDriver
{
  private static Navigator.ActiveTransition emptyTransition = new Navigator.ActiveTransition();
  public static ObjectPool<Navigator.ActiveTransition> TransitionPool = new ObjectPool<Navigator.ActiveTransition>((Func<Navigator.ActiveTransition>) (() => new Navigator.ActiveTransition()), 128);
  private Stack<TransitionDriver.InterruptOverrideLayer> interruptOverrideStack = new Stack<TransitionDriver.InterruptOverrideLayer>(8);
  private Navigator.ActiveTransition transition;
  private Navigator navigator;
  private Vector3 targetPos;
  private bool isComplete;
  private Brain brain;
  public List<TransitionDriver.OverrideLayer> overrideLayers = new List<TransitionDriver.OverrideLayer>();
  private LoggerFS log;

  public Navigator.ActiveTransition GetTransition => this.transition;

  public TransitionDriver(Navigator navigator) => this.log = new LoggerFS(nameof (TransitionDriver), 35);

  public void BeginTransition(
    Navigator navigator,
    NavGrid.Transition transition,
    float defaultSpeed)
  {
    Navigator.ActiveTransition instance = TransitionDriver.TransitionPool.GetInstance();
    instance.Init(transition, defaultSpeed);
    this.BeginTransition(navigator, instance);
  }

  private void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    bool flag1 = this.interruptOverrideStack.Count != 0;
    foreach (TransitionDriver.OverrideLayer overrideLayer in this.overrideLayers)
    {
      if (!flag1 || !(overrideLayer is TransitionDriver.InterruptOverrideLayer))
        overrideLayer.BeginTransition(navigator, transition);
    }
    this.navigator = navigator;
    this.transition = transition;
    this.isComplete = false;
    Grid.SceneLayer layer1 = navigator.sceneLayer;
    if (transition.navGridTransition.start == NavType.Tube || transition.navGridTransition.end == NavType.Tube)
      layer1 = Grid.SceneLayer.BuildingUse;
    else if (transition.navGridTransition.start == NavType.Solid && transition.navGridTransition.end == NavType.Solid)
    {
      KBatchedAnimController component = ((Component) navigator).GetComponent<KBatchedAnimController>();
      layer1 = Grid.SceneLayer.FXFront;
      int layer2 = (int) layer1;
      component.SetSceneLayer((Grid.SceneLayer) layer2);
    }
    else if (transition.navGridTransition.start == NavType.Solid || transition.navGridTransition.end == NavType.Solid)
      ((Component) navigator).GetComponent<KBatchedAnimController>().SetSceneLayer(layer1);
    this.targetPos = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) navigator), transition.x, transition.y), layer1);
    if (transition.isLooping)
    {
      KAnimControllerBase component = ((Component) navigator).GetComponent<KAnimControllerBase>();
      component.PlaySpeedMultiplier = transition.animSpeed;
      bool flag2 = HashedString.op_Inequality(transition.preAnim, HashedString.op_Implicit(""));
      bool flag3 = component.CurrentAnim != null && HashedString.op_Equality(HashedString.op_Implicit(component.CurrentAnim.name), transition.anim);
      if ((!flag2 || component.CurrentAnim == null ? 0 : (HashedString.op_Equality(HashedString.op_Implicit(component.CurrentAnim.name), transition.preAnim) ? 1 : 0)) != 0)
      {
        component.ClearQueue();
        component.Queue(transition.anim, (KAnim.PlayMode) 0);
      }
      else if (flag3)
      {
        if (component.PlayMode != null)
        {
          component.ClearQueue();
          component.Queue(transition.anim, (KAnim.PlayMode) 0);
        }
      }
      else if (flag2)
      {
        component.Play(transition.preAnim);
        component.Queue(transition.anim, (KAnim.PlayMode) 0);
      }
      else
        component.Play(transition.anim, (KAnim.PlayMode) 0);
    }
    else if (HashedString.op_Inequality(transition.anim, HashedString.op_Implicit((string) null)))
    {
      KAnimControllerBase component = ((Component) navigator).GetComponent<KAnimControllerBase>();
      component.PlaySpeedMultiplier = transition.animSpeed;
      component.Play(transition.anim);
      navigator.Subscribe(-1061186183, new Action<object>(this.OnAnimComplete));
    }
    if (transition.navGridTransition.y != 0)
    {
      if (transition.navGridTransition.start == NavType.RightWall)
        ((Component) navigator).GetComponent<Facing>().SetFacing(transition.navGridTransition.y < 0);
      else if (transition.navGridTransition.start == NavType.LeftWall)
        ((Component) navigator).GetComponent<Facing>().SetFacing(transition.navGridTransition.y > 0);
    }
    if (transition.navGridTransition.x != 0)
    {
      if (transition.navGridTransition.start == NavType.Ceiling)
        ((Component) navigator).GetComponent<Facing>().SetFacing(transition.navGridTransition.x > 0);
      else if (transition.navGridTransition.start != NavType.LeftWall && transition.navGridTransition.start != NavType.RightWall)
        ((Component) navigator).GetComponent<Facing>().SetFacing(transition.navGridTransition.x < 0);
    }
    this.brain = ((Component) navigator).GetComponent<Brain>();
  }

  public void UpdateTransition(float dt)
  {
    if (Object.op_Equality((Object) this.navigator, (Object) null))
      return;
    foreach (TransitionDriver.OverrideLayer overrideLayer in this.overrideLayers)
    {
      if (!(this.interruptOverrideStack.Count != 0 & overrideLayer is TransitionDriver.InterruptOverrideLayer) || this.interruptOverrideStack.Peek() == overrideLayer)
        overrideLayer.UpdateTransition(this.navigator, this.transition);
    }
    if (!this.isComplete && this.transition.isCompleteCB != null)
      this.isComplete = this.transition.isCompleteCB();
    if (Object.op_Inequality((Object) this.brain, (Object) null))
    {
      int num = this.isComplete ? 1 : 0;
    }
    if (this.transition.isLooping)
    {
      float speed = this.transition.speed;
      Vector3 position = TransformExtensions.GetPosition(this.navigator.transform);
      int cell1 = Grid.PosToCell(position);
      if (this.transition.x > 0)
      {
        position.x += dt * speed;
        if ((double) position.x > (double) this.targetPos.x)
          this.isComplete = true;
      }
      else if (this.transition.x < 0)
      {
        position.x -= dt * speed;
        if ((double) position.x < (double) this.targetPos.x)
          this.isComplete = true;
      }
      else
        position.x = this.targetPos.x;
      if (this.transition.y > 0)
      {
        position.y += dt * speed;
        if ((double) position.y > (double) this.targetPos.y)
          this.isComplete = true;
      }
      else if (this.transition.y < 0)
      {
        position.y -= dt * speed;
        if ((double) position.y < (double) this.targetPos.y)
          this.isComplete = true;
      }
      else
        position.y = this.targetPos.y;
      TransformExtensions.SetPosition(this.navigator.transform, position);
      int cell2 = Grid.PosToCell(position);
      if (cell2 != cell1)
        this.navigator.Trigger(915392638, (object) cell2);
    }
    if (!this.isComplete)
      return;
    this.isComplete = false;
    Navigator navigator = this.navigator;
    navigator.SetCurrentNavType(this.transition.end);
    TransformExtensions.SetPosition(navigator.transform, this.targetPos);
    this.EndTransition();
    navigator.AdvancePath();
  }

  public void EndTransition()
  {
    if (!Object.op_Inequality((Object) this.navigator, (Object) null))
      return;
    this.interruptOverrideStack.Clear();
    foreach (TransitionDriver.OverrideLayer overrideLayer in this.overrideLayers)
      overrideLayer.EndTransition(this.navigator, this.transition);
    ((Component) this.navigator).GetComponent<KAnimControllerBase>().PlaySpeedMultiplier = 1f;
    this.navigator.Unsubscribe(-1061186183, new Action<object>(this.OnAnimComplete));
    if (Object.op_Inequality((Object) this.brain, (Object) null))
      this.brain.Resume("move_handler");
    if (Object.op_Inequality((Object) this.navigator.animEventHandler, (Object) null))
      this.navigator.animEventHandler.SetDirty();
    TransitionDriver.TransitionPool.ReleaseInstance(this.transition);
    this.transition = (Navigator.ActiveTransition) null;
    this.navigator = (Navigator) null;
    this.brain = (Brain) null;
  }

  private void OnAnimComplete(object data)
  {
    if (Object.op_Inequality((Object) this.navigator, (Object) null))
      this.navigator.Unsubscribe(-1061186183, new Action<object>(this.OnAnimComplete));
    this.isComplete = true;
  }

  public static Navigator.ActiveTransition SwapTransitionWithEmpty(Navigator.ActiveTransition src)
  {
    Navigator.ActiveTransition instance = TransitionDriver.TransitionPool.GetInstance();
    instance.Copy(src);
    src.Copy(TransitionDriver.emptyTransition);
    return instance;
  }

  public class OverrideLayer
  {
    public OverrideLayer(Navigator navigator)
    {
    }

    public virtual void Destroy()
    {
    }

    public virtual void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
    {
    }

    public virtual void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
    {
    }

    public virtual void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
    {
    }
  }

  public class InterruptOverrideLayer : TransitionDriver.OverrideLayer
  {
    protected Navigator.ActiveTransition originalTransition;
    protected TransitionDriver driver;

    protected bool InterruptInProgress => this.originalTransition != null;

    public InterruptOverrideLayer(Navigator navigator)
      : base(navigator)
    {
      this.driver = navigator.transitionDriver;
    }

    public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
    {
      this.driver.interruptOverrideStack.Push(this);
      this.originalTransition = TransitionDriver.SwapTransitionWithEmpty(transition);
    }

    public override void UpdateTransition(
      Navigator navigator,
      Navigator.ActiveTransition transition)
    {
      if (!this.IsOverrideComplete())
        return;
      this.driver.interruptOverrideStack.Pop();
      transition.Copy(this.originalTransition);
      TransitionDriver.TransitionPool.ReleaseInstance(this.originalTransition);
      this.originalTransition = (Navigator.ActiveTransition) null;
      this.EndTransition(navigator, transition);
      this.driver.BeginTransition(navigator, transition);
    }

    public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
    {
      base.EndTransition(navigator, transition);
      if (this.originalTransition == null)
        return;
      TransitionDriver.TransitionPool.ReleaseInstance(this.originalTransition);
      this.originalTransition = (Navigator.ActiveTransition) null;
    }

    protected virtual bool IsOverrideComplete() => this.originalTransition != null && this.driver.interruptOverrideStack.Count != 0 && this.driver.interruptOverrideStack.Peek() == this;
  }
}
