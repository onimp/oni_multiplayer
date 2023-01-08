// Decompiled with JetBrains decompiler
// Type: GameOverScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class GameOverScreen : KModalScreen
{
  public KButton DismissButton;
  public KButton QuitButton;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Init();
  }

  private void Init()
  {
    if (Object.op_Implicit((Object) this.QuitButton))
      this.QuitButton.onClick += (System.Action) (() => this.Quit());
    if (!Object.op_Implicit((Object) this.DismissButton))
      return;
    this.DismissButton.onClick += (System.Action) (() => this.Dismiss());
  }

  private void Quit() => PauseScreen.TriggerQuitGame();

  private void Dismiss() => this.Show(false);
}
