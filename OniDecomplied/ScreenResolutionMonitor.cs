// Decompiled with JetBrains decompiler
// Type: ScreenResolutionMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ScreenResolutionMonitor : MonoBehaviour
{
  [SerializeField]
  private Vector2 previousSize;
  private static bool previousGamepadUIMode;
  private const float HIGH_DPI = 130f;

  private void Awake() => this.previousSize = new Vector2((float) Screen.width, (float) Screen.height);

  private void Update()
  {
    if (((double) this.previousSize.x != (double) Screen.width || (double) this.previousSize.y != (double) Screen.height) && Object.op_Inequality((Object) Game.Instance, (Object) null))
    {
      Game.Instance.Trigger(445618876, (object) null);
      this.previousSize.x = (float) Screen.width;
      this.previousSize.y = (float) Screen.height;
    }
    this.UpdateShouldUseGamepadUIMode();
  }

  public static bool UsingGamepadUIMode() => ScreenResolutionMonitor.previousGamepadUIMode;

  private void UpdateShouldUseGamepadUIMode()
  {
    bool flag = (double) Screen.dpi > 130.0 && Screen.height < 900 || KInputManager.currentControllerIsGamepad;
    if (flag == ScreenResolutionMonitor.previousGamepadUIMode)
      return;
    ScreenResolutionMonitor.previousGamepadUIMode = flag;
    if (Object.op_Equality((Object) Game.Instance, (Object) null))
      return;
    Game.Instance.Trigger(-442024484, (object) null);
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound(flag ? "ControllerType_ToggleOn" : "ControllerType_ToggleOff"));
  }
}
