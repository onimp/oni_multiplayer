// Decompiled with JetBrains decompiler
// Type: NewGameFlowScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public abstract class NewGameFlowScreen : KModalScreen
{
  public event System.Action OnNavigateForward;

  public event System.Action OnNavigateBackward;

  protected void NavigateBackward() => this.OnNavigateBackward();

  protected void NavigateForward() => this.OnNavigateForward();

  public override void OnKeyDown(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return;
    if (e.TryConsume((Action) 5))
      this.NavigateBackward();
    base.OnKeyDown(e);
  }
}
