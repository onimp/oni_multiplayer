// Decompiled with JetBrains decompiler
// Type: ScreenResize
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ScreenResize : MonoBehaviour
{
  public System.Action OnResize;
  public static ScreenResize Instance;
  private int Width;
  private int Height;
  private bool isFullscreen;

  private void Awake()
  {
    ScreenResize.Instance = this;
    this.isFullscreen = Screen.fullScreen;
    this.OnResize += new System.Action(this.SaveResolutionToPrefs);
  }

  private void LateUpdate()
  {
    if (Screen.width == this.Width && Screen.height == this.Height && this.isFullscreen == Screen.fullScreen)
      return;
    this.Width = Screen.width;
    this.Height = Screen.height;
    this.isFullscreen = Screen.fullScreen;
    this.TriggerResize();
  }

  public void TriggerResize()
  {
    if (this.OnResize == null)
      return;
    this.OnResize();
  }

  private void SaveResolutionToPrefs() => GraphicsOptionsScreen.OnResize();
}
