// Decompiled with JetBrains decompiler
// Type: DevToolWarning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using STRINGS;
using UnityEngine;

public class DevToolWarning
{
  private bool showAgain;
  public string Name;
  public bool ShouldDrawWindow;

  public DevToolWarning() => this.Name = (string) UI.FRONTEND.DEVTOOLS.TITLE;

  public void DrawMenuBar()
  {
    if (!ImGui.BeginMainMenuBar())
      return;
    ImGui.Checkbox(this.Name, ref this.ShouldDrawWindow);
    ImGui.EndMainMenuBar();
  }

  public void DrawWindow(out bool isOpen)
  {
    ImGuiWindowFlags imGuiWindowFlags = (ImGuiWindowFlags) 0;
    isOpen = true;
    if (!ImGui.Begin(this.Name + "###ID_DevToolWarning", ref isOpen, imGuiWindowFlags))
      return;
    if (!isOpen)
    {
      ImGui.End();
    }
    else
    {
      ImGui.SetWindowSize(new Vector2(500f, 250f));
      ImGui.TextWrapped((string) UI.FRONTEND.DEVTOOLS.WARNING);
      ImGui.Spacing();
      ImGui.Spacing();
      ImGui.Spacing();
      ImGui.Spacing();
      ImGui.Checkbox((string) UI.FRONTEND.DEVTOOLS.DONTSHOW, ref this.showAgain);
      if (ImGui.Button((string) UI.FRONTEND.DEVTOOLS.BUTTON))
      {
        if (this.showAgain)
          KPlayerPrefs.SetInt("ShowDevtools", 1);
        DevToolManager.Instance.UserAcceptedWarning = true;
        isOpen = false;
      }
      ImGui.End();
    }
  }
}
