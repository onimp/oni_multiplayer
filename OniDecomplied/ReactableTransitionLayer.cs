// Decompiled with JetBrains decompiler
// Type: ReactableTransitionLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ReactableTransitionLayer : TransitionDriver.InterruptOverrideLayer
{
  private ReactionMonitor.Instance reactionMonitor;

  public ReactableTransitionLayer(Navigator navigator)
    : base(navigator)
  {
  }

  protected override bool IsOverrideComplete() => !this.reactionMonitor.IsReacting() && base.IsOverrideComplete();

  public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
  {
    if (this.reactionMonitor == null)
      this.reactionMonitor = ((Component) navigator).GetSMI<ReactionMonitor.Instance>();
    this.reactionMonitor.PollForReactables(transition);
    if (!this.reactionMonitor.IsReacting())
      return;
    base.BeginTransition(navigator, transition);
    transition.start = this.originalTransition.start;
    transition.end = this.originalTransition.end;
  }
}
