// Decompiled with JetBrains decompiler
// Type: DevToolSaveGameInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using UnityEngine;

public class DevToolSaveGameInfo : DevTool
{
  private string clSearch = "";

  protected override void RenderTo(DevPanel panel)
  {
    if (Object.op_Equality((Object) Game.Instance, (Object) null))
    {
      ImGui.Text("No game loaded");
    }
    else
    {
      ImGui.Text("Seed: " + CustomGameSettings.Instance.GetSettingsCoordinate());
      ImGui.Text("Generated: " + Game.Instance.dateGenerated);
      ImGui.PushItemWidth(100f);
      ImGui.NewLine();
      ImGui.Text("Changelists played on");
      ImGui.InputText("Search", ref this.clSearch, 10U);
      ImGui.PopItemWidth();
      foreach (uint num in Game.Instance.changelistsPlayedOn)
      {
        if (Util.IsNullOrWhiteSpace(this.clSearch) || num.ToString().Contains(this.clSearch))
          ImGui.Text(num.ToString());
      }
      ImGui.NewLine();
      if (!ImGui.Button("Open Story Manager"))
        return;
      DevToolUtil.Open<DevToolStoryManager>();
    }
  }
}
