// Decompiled with JetBrains decompiler
// Type: DevToolStringsTable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System.Collections.Generic;
using UnityEngine;

public class DevToolStringsTable : DevTool
{
  private List<(string id, string value)> m_cached_entries;
  private const int MAX_ENTRIES_TO_DRAW = 3000;
  private string m_search_filter = "";

  protected override void RenderTo(DevPanel panel)
  {
    if (this.m_cached_entries == null)
    {
      this.m_cached_entries = new List<(string, string)>();
      DevToolStringsTable.RegenerateCacheWithFilter(this.m_cached_entries, this.m_search_filter);
    }
    if (!ImGui.CollapsingHeader(string.Format("Entries ({0})###ID_LocStringEntries", (object) this.m_cached_entries.Count), (ImGuiTreeNodeFlags) 32))
      return;
    if (ImGuiEx.InputFilter("Filter", ref this.m_search_filter, 50U))
      DevToolStringsTable.RegenerateCacheWithFilter(this.m_cached_entries, this.m_search_filter);
    ImGui.Columns(2, "LocStrings");
    ImGui.Text("Key");
    ImGui.NextColumn();
    ImGui.Text("Value");
    ImGui.NextColumn();
    ImGui.Separator();
    int num = Mathf.Min(3000, this.m_cached_entries.Count);
    for (int index = 0; index < num; ++index)
    {
      (string id, string value) = this.m_cached_entries[index];
      if (ImGui.Selectable(string.Format("{0}###ID_{1}_key", (object) id, (object) index)))
      {
        this.m_search_filter = id;
        DevToolStringsTable.RegenerateCacheWithFilter(this.m_cached_entries, this.m_search_filter);
        break;
      }
      ImGuiEx.TooltipForPrevious(id ?? "");
      ImGui.NextColumn();
      if (ImGui.Selectable(string.Format("{0}###ID_{1}_value", (object) value, (object) index)))
      {
        this.m_search_filter = value;
        DevToolStringsTable.RegenerateCacheWithFilter(this.m_cached_entries, this.m_search_filter);
        break;
      }
      ImGuiEx.TooltipForPrevious(value ?? "");
      ImGui.NextColumn();
    }
    ImGui.Columns(1);
    if (this.m_cached_entries.Count <= 3000)
      return;
    ImGui.Separator();
    ImGui.Text(string.Format("* Stopped drawing entries because there are too many to draw (limit: {0}, current: {1}) *", (object) 3000, (object) this.m_cached_entries.Count));
  }

  public static void RegenerateCacheWithFilter(
    List<(string id, string value)> cached_entries,
    string filter)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    DevToolStringsTable.\u003C\u003Ec__DisplayClass4_0 cDisplayClass40 = new DevToolStringsTable.\u003C\u003Ec__DisplayClass4_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass40.cached_entries = cached_entries;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass40.cached_entries.Clear();
    if (!string.IsNullOrWhiteSpace(filter))
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: method pointer
      Strings.VisitEntries(new StringTable.EntryVisitor((object) new DevToolStringsTable.\u003C\u003Ec__DisplayClass4_1()
      {
        CS\u0024\u003C\u003E8__locals1 = cDisplayClass40,
        normalized_filter = filter.ToLowerInvariant().Trim()
      }, __methodptr(\u003CRegenerateCacheWithFilter\u003Eb__1)));
    }
    else
    {
      // ISSUE: method pointer
      Strings.VisitEntries(new StringTable.EntryVisitor((object) cDisplayClass40, __methodptr(\u003CRegenerateCacheWithFilter\u003Eb__0)));
    }
  }
}
