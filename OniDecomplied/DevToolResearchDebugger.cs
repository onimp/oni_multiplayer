// Decompiled with JetBrains decompiler
// Type: DevToolResearchDebugger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System.Collections.Generic;
using UnityEngine;

public class DevToolResearchDebugger : DevTool
{
  public DevToolResearchDebugger() => this.RequiresGameRunning = true;

  protected override void RenderTo(DevPanel panel)
  {
    TechInstance activeResearch = Research.Instance.GetActiveResearch();
    if (activeResearch == null)
    {
      ImGui.Text("No Active Research");
    }
    else
    {
      ImGui.Text("Active Research");
      ImGui.Text("ID: " + activeResearch.tech.Id);
      ImGui.Text("Name: " + Util.StripTextFormatting(activeResearch.tech.Name));
      ImGui.Separator();
      ImGui.Text("Active Research Inventory");
      foreach (KeyValuePair<string, float> keyValuePair in new Dictionary<string, float>((IDictionary<string, float>) activeResearch.progressInventory.PointsByTypeID))
      {
        if (activeResearch.tech.RequiresResearchType(keyValuePair.Key))
        {
          float num1 = activeResearch.tech.costsByResearchTypeID[keyValuePair.Key];
          float num2 = keyValuePair.Value;
          if (ImGui.Button("Fill"))
            num2 = num1;
          ImGui.SameLine();
          ImGui.SetNextItemWidth(100f);
          ImGui.InputFloat(keyValuePair.Key, ref num2, 1f, 10f);
          ImGui.SameLine();
          ImGui.Text(string.Format("of {0}", (object) num1));
          activeResearch.progressInventory.PointsByTypeID[keyValuePair.Key] = Mathf.Clamp(num2, 0.0f, num1);
        }
      }
      ImGui.Separator();
      ImGui.Text("Global Points Inventory");
      foreach (KeyValuePair<string, float> keyValuePair in Research.Instance.globalPointInventory.PointsByTypeID)
        ImGui.Text(keyValuePair.Key + ": " + keyValuePair.Value.ToString());
    }
  }
}
