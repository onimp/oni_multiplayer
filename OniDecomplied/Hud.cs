// Decompiled with JetBrains decompiler
// Type: Hud
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class Hud : KScreen
{
  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (!e.TryConsume((Action) 2))
      return;
    GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.ControlsScreen).gameObject, (GameObject) null, (GameScreenManager.UIRenderTarget) 2);
  }
}
