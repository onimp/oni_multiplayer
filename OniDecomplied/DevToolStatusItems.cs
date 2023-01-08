// Decompiled with JetBrains decompiler
// Type: DevToolStatusItems
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DevToolStatusItems : DevTool
{
  private (string header, Action<StatusItemGroup.Entry> draw)[] columns = new (string, Action<StatusItemGroup.Entry>)[4]
  {
    ("Text", (Action<StatusItemGroup.Entry>) (entry => ImGui.Text(entry.GetName()))),
    ("Id Name", (Action<StatusItemGroup.Entry>) (entry => ImGui.Text(entry.item.Id))),
    ("Notification Type", (Action<StatusItemGroup.Entry>) (entry => ImGui.Text(entry.item.notificationType.ToString()))),
    ("Category", (Action<StatusItemGroup.Entry>) (entry => ImGui.Text(entry.category?.Name ?? "<no category>")))
  };

  public DevToolStatusItems() => this.RequiresGameRunning = true;

  protected override void RenderTo(DevPanel panel)
  {
    if (Object.op_Equality((Object) SelectTool.Instance, (Object) null))
    {
      ImGui.Text("no select tool instance");
    }
    else
    {
      KSelectable selected = SelectTool.Instance.selected;
      if (Object.op_Equality((Object) selected, (Object) null) || !Object.op_Implicit((Object) selected))
      {
        ImGui.Text("no object is selected");
      }
      else
      {
        StatusItemGroup statusItemGroup = selected.GetStatusItemGroup();
        if (statusItemGroup == null)
          ImGui.Text("object doesn't have a StatusItemGroup");
        else
          DevToolStatusItems.DrawTable<StatusItemGroup.Entry>("status_items", this.columns, statusItemGroup.GetEnumerator());
      }
    }
  }

  public static void DrawTable<T>(
    string string_id,
    (string header, Action<T> draw)[] columns,
    IEnumerator<T> data)
  {
    ImGuiTableFlags imGuiTableFlags = (ImGuiTableFlags) 50341825;
    if (!ImGui.BeginTable(string_id, columns.Length, imGuiTableFlags))
      return;
    foreach ((string header, Action<T> draw) column in columns)
      ImGui.TableSetupColumn(column.header);
    ImGui.TableHeadersRow();
    using (data)
    {
      while (data.MoveNext())
      {
        T current = data.Current;
        ImGui.TableNextRow();
        foreach ((string header, Action<T> draw) column in columns)
        {
          ImGui.TableNextColumn();
          try
          {
            column.draw(current);
          }
          catch (Exception ex)
          {
            ImGui.PushStyleColor((ImGuiCol) 0, new Vector4(1f, 0.0f, 0.0f, 1f));
            ImGui.Text("<ERROR: " + ex.Message + ">");
            if (ImGui.IsItemHovered())
            {
              ImGui.BeginTooltip();
              ImGui.SetTooltip(ex.ToString());
              ImGui.EndTooltip();
            }
            ImGui.PopStyleColor(1);
          }
        }
      }
    }
    ImGui.EndTable();
  }
}
