// Decompiled with JetBrains decompiler
// Type: DevToolPrintingPodDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using UnityEngine;

public class DevToolPrintingPodDebug : DevTool
{
  protected override void RenderTo(DevPanel panel)
  {
    if (Object.op_Inequality((Object) Immigration.Instance, (Object) null))
      this.ShowButtons();
    else
      ImGui.Text("Game not available");
  }

  private void ShowButtons()
  {
    if (Components.Telepads.Count == 0)
    {
      ImGui.Text("No printing pods available");
    }
    else
    {
      ImGui.Text("Time until next print available: " + Mathf.CeilToInt(Immigration.Instance.timeBeforeSpawn).ToString() + "s");
      if (ImGui.Button("Activate now"))
        Immigration.Instance.timeBeforeSpawn = 0.0f;
      if (!ImGui.Button("Shuffle Options"))
        return;
      if (Object.op_Equality((Object) ImmigrantScreen.instance.Telepad, (Object) null))
        ImmigrantScreen.InitializeImmigrantScreen(Components.Telepads[0]);
      else
        ImmigrantScreen.instance.DebugShuffleOptions();
    }
  }
}
