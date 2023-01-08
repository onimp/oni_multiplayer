// Decompiled with JetBrains decompiler
// Type: DemoOverScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class DemoOverScreen : KModalScreen
{
  public KButton QuitButton;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Init();
    PlayerController.Instance.ActivateTool((InterfaceTool) SelectTool.Instance);
    SelectTool.Instance.Select((KSelectable) null);
  }

  private void Init() => this.QuitButton.onClick += (System.Action) (() => this.Quit());

  private void Quit() => PauseScreen.TriggerQuitGame();
}
