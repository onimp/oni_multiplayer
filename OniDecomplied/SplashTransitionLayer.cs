// Decompiled with JetBrains decompiler
// Type: SplashTransitionLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SplashTransitionLayer : TransitionDriver.OverrideLayer
{
  private float lastSplashTime;
  private const float SPLASH_INTERVAL = 1f;

  public SplashTransitionLayer(Navigator navigator)
    : base(navigator)
  {
    this.lastSplashTime = Time.time;
  }

  private void RefreshSplashes(Navigator navigator, Navigator.ActiveTransition transition)
  {
    if (Object.op_Equality((Object) navigator, (Object) null) || transition.end == NavType.Tube)
      return;
    Vector3 position = TransformExtensions.GetPosition(navigator.transform);
    if ((double) this.lastSplashTime + 1.0 >= (double) Time.time || !Grid.Element[Grid.PosToCell(position)].IsLiquid)
      return;
    this.lastSplashTime = Time.time;
    KBatchedAnimController effect = FXHelpers.CreateEffect("splash_step_kanim", Vector3.op_Addition(position, new Vector3(0.0f, 0.75f, -0.1f)));
    effect.Play(HashedString.op_Implicit("fx1"));
    effect.destroyOnAnimComplete = true;
  }

  public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.BeginTransition(navigator, transition);
    this.RefreshSplashes(navigator, transition);
  }

  public override void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.UpdateTransition(navigator, transition);
    this.RefreshSplashes(navigator, transition);
  }

  public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    base.EndTransition(navigator, transition);
    this.RefreshSplashes(navigator, transition);
  }
}
