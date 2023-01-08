// Decompiled with JetBrains decompiler
// Type: DevToolNavGrid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Linq;
using UnityEngine;

public class DevToolNavGrid : DevTool
{
  private const string INVALID_OVERLAY_MODE_STR = "None";
  private string[] navGridNames;
  private int selectedNavGrid;
  public static DevToolNavGrid Instance;
  private int[] linkStats;
  private int highestLinkCell;
  private int highestLinkCount;
  private int selectedCell;

  public DevToolNavGrid() => DevToolNavGrid.Instance = this;

  private bool Init()
  {
    if (Object.op_Equality((Object) Pathfinding.Instance, (Object) null))
      return false;
    if (this.navGridNames != null)
      return true;
    this.navGridNames = Pathfinding.Instance.GetNavGrids().Select<NavGrid, string>((Func<NavGrid, string>) (x => x.id)).ToArray<string>();
    return true;
  }

  protected override void RenderTo(DevPanel panel)
  {
    if (this.Init())
      this.Contents();
    else
      ImGui.Text("Game not initialized");
  }

  public void SetCell(int cell) => this.selectedCell = cell;

  private void Contents()
  {
    ImGui.Combo("Nav Grid ID", ref this.selectedNavGrid, this.navGridNames, this.navGridNames.Length);
    NavGrid navGrid = Pathfinding.Instance.GetNavGrid(this.navGridNames[this.selectedNavGrid]);
    ImGui.Text("Max Links per cell: " + navGrid.maxLinksPerCell.ToString());
    ImGui.Spacing();
    if (ImGui.Button("Calculate Stats"))
    {
      this.linkStats = new int[navGrid.maxLinksPerCell];
      this.highestLinkCell = 0;
      this.highestLinkCount = 0;
      for (int index1 = 0; index1 < Grid.CellCount; ++index1)
      {
        int index2 = 0;
        for (int index3 = 0; index3 < navGrid.maxLinksPerCell; ++index3)
        {
          int index4 = index1 * navGrid.maxLinksPerCell + index3;
          if (navGrid.Links[index4].link != Grid.InvalidCell)
            ++index2;
          else
            break;
        }
        if (index2 > this.highestLinkCount)
        {
          this.highestLinkCell = index1;
          this.highestLinkCount = index2;
        }
        ++this.linkStats[index2];
      }
    }
    ImGui.SameLine();
    if (ImGui.Button("Clear"))
      this.linkStats = (int[]) null;
    if (this.linkStats != null)
    {
      ImGui.Text("Highest link count: " + this.highestLinkCount.ToString());
      ImGui.Text(string.Format("Utilized percentage: {0} %", (object) (float) ((double) this.highestLinkCount / (double) navGrid.maxLinksPerCell * 100.0)));
      ImGui.SameLine();
      if (ImGui.Button(string.Format("Select {0}", (object) this.highestLinkCell)))
        this.selectedCell = this.highestLinkCell;
      for (int index = 0; index < this.linkStats.Length; ++index)
      {
        if (this.linkStats[index] > 0)
          ImGui.Text(string.Format("\t{0}: {1}", (object) index, (object) this.linkStats[index]));
      }
    }
    ImGui.Spacing();
    int x;
    int y;
    Grid.CellToXY(this.selectedCell, out x, out y);
    ImGui.Text(string.Format("Selected Cell: {0} ({1},{2})", (object) this.selectedCell, (object) x, (object) y));
    if (!Grid.IsValidCell(this.selectedCell) || navGrid.Links == null || navGrid.Links.Length <= navGrid.maxLinksPerCell * this.selectedCell)
      return;
    for (int idx = 0; idx < navGrid.maxLinksPerCell; ++idx)
    {
      int index = this.selectedCell * navGrid.maxLinksPerCell + idx;
      NavGrid.Link link = navGrid.Links[index];
      if (link.link == Grid.InvalidCell)
        break;
      this.DrawLink(idx, link, navGrid);
    }
  }

  private void DrawLink(int idx, NavGrid.Link l, NavGrid navGrid)
  {
    NavGrid.Transition transition = navGrid.transitions[(int) l.transitionId];
    ImGui.Text(string.Format("   {0} -> {1} x:{2} y:{3} anim:{4} cost:{5}", (object) transition.start, (object) transition.end, (object) transition.x, (object) transition.y, (object) transition.anim, (object) transition.cost));
  }
}
