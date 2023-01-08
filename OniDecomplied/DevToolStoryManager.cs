// Decompiled with JetBrains decompiler
// Type: DevToolStoryManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System.Collections.Generic;
using UnityEngine;

public class DevToolStoryManager : DevTool
{
  protected override void RenderTo(DevPanel panel)
  {
    if (ImGui.CollapsingHeader("Story Instance Data", (ImGuiTreeNodeFlags) 32))
      this.DrawStoryInstanceData();
    ImGui.Spacing();
    if (!ImGui.CollapsingHeader("Story Telemetry Data", (ImGuiTreeNodeFlags) 32))
      return;
    this.DrawTelemetryData();
  }

  private void DrawStoryInstanceData()
  {
    if (Object.op_Equality((Object) StoryManager.Instance, (Object) null))
    {
      ImGui.Text("Couldn't find StoryManager instance");
    }
    else
    {
      ImGui.Text(string.Format("Stories (count: {0})", (object) StoryManager.Instance.GetStoryInstances().Count));
      ImGui.Text("Highest generated: " + (StoryManager.Instance.GetHighestCoordinate() == -2 ? "Before stories" : StoryManager.Instance.GetHighestCoordinate().ToString()));
      foreach (KeyValuePair<int, StoryInstance> storyInstance in StoryManager.Instance.GetStoryInstances())
        ImGui.Text(" - " + storyInstance.Value.storyId + ": " + storyInstance.Value.CurrentState.ToString());
      if (StoryManager.Instance.GetStoryInstances().Count != 0)
        return;
      ImGui.Text(" - No stories");
    }
  }

  private void DrawTelemetryData() => ImGuiEx.DrawObjectTable<StoryManager.StoryTelemetry>("ID_telemetry", (IEnumerable<StoryManager.StoryTelemetry>) StoryManager.GetTelemetry(), new ImGuiTableFlags?());
}
