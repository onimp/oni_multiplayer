// Decompiled with JetBrains decompiler
// Type: NavGridUpdater
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class NavGridUpdater
{
  public static int InvalidHandle = -1;
  public static int InvalidIdx = -1;
  public static int InvalidCell = -1;

  public static void InitializeNavGrid(
    NavTable nav_table,
    NavTableValidator[] validators,
    CellOffset[] bounding_offsets,
    int max_links_per_cell,
    NavGrid.Link[] links,
    NavGrid.Transition[][] transitions_by_nav_type)
  {
    NavGridUpdater.MarkValidCells(nav_table, validators, bounding_offsets);
    NavGridUpdater.CreateLinks(nav_table, max_links_per_cell, links, transitions_by_nav_type, new Dictionary<int, int>());
  }

  public static void UpdateNavGrid(
    NavTable nav_table,
    NavTableValidator[] validators,
    CellOffset[] bounding_offsets,
    int max_links_per_cell,
    NavGrid.Link[] links,
    NavGrid.Transition[][] transitions_by_nav_type,
    Dictionary<int, int> teleport_transitions,
    HashSet<int> dirty_nav_cells)
  {
    NavGridUpdater.UpdateValidCells(dirty_nav_cells, nav_table, validators, bounding_offsets);
    NavGridUpdater.UpdateLinks(dirty_nav_cells, nav_table, max_links_per_cell, links, transitions_by_nav_type, teleport_transitions);
  }

  private static void UpdateValidCells(
    HashSet<int> dirty_solid_cells,
    NavTable nav_table,
    NavTableValidator[] validators,
    CellOffset[] bounding_offsets)
  {
    foreach (int dirtySolidCell in dirty_solid_cells)
    {
      foreach (NavTableValidator validator in validators)
        validator.UpdateCell(dirtySolidCell, nav_table, bounding_offsets);
    }
  }

  private static void CreateLinksForCell(
    int cell,
    NavTable nav_table,
    int max_links_per_cell,
    NavGrid.Link[] links,
    NavGrid.Transition[][] transitions_by_nav_type,
    Dictionary<int, int> teleport_transitions)
  {
    NavGridUpdater.CreateLinks(cell, nav_table, max_links_per_cell, links, transitions_by_nav_type, teleport_transitions);
  }

  private static void UpdateLinks(
    HashSet<int> dirty_nav_cells,
    NavTable nav_table,
    int max_links_per_cell,
    NavGrid.Link[] links,
    NavGrid.Transition[][] transitions_by_nav_type,
    Dictionary<int, int> teleport_transitions)
  {
    foreach (int dirtyNavCell in dirty_nav_cells)
      NavGridUpdater.CreateLinksForCell(dirtyNavCell, nav_table, max_links_per_cell, links, transitions_by_nav_type, teleport_transitions);
  }

  private static void CreateLinks(
    NavTable nav_table,
    int max_links_per_cell,
    NavGrid.Link[] links,
    NavGrid.Transition[][] transitions_by_nav_type,
    Dictionary<int, int> teleport_transitions)
  {
    WorkItemCollection<NavGridUpdater.CreateLinkWorkItem, object> workItemCollection = new WorkItemCollection<NavGridUpdater.CreateLinkWorkItem, object>();
    workItemCollection.Reset((object) null);
    for (int index = 0; index < Grid.HeightInCells; ++index)
      workItemCollection.Add(new NavGridUpdater.CreateLinkWorkItem(Grid.OffsetCell(0, new CellOffset(0, index)), nav_table, max_links_per_cell, links, transitions_by_nav_type, teleport_transitions));
    GlobalJobManager.Run((IWorkItemCollection) workItemCollection);
  }

  private static void CreateLinks(
    int cell,
    NavTable nav_table,
    int max_links_per_cell,
    NavGrid.Link[] links,
    NavGrid.Transition[][] transitions_by_nav_type,
    Dictionary<int, int> teleport_transitions)
  {
    int index1 = cell * max_links_per_cell;
    int num = 0;
    for (int index2 = 0; index2 < 11; ++index2)
    {
      NavType nav_type = (NavType) index2;
      NavGrid.Transition[] transitionArray = transitions_by_nav_type[index2];
      if (transitionArray != null && nav_table.IsValid(cell, nav_type))
      {
        foreach (NavGrid.Transition transition1 in transitionArray)
        {
          NavGrid.Transition transition2;
          if ((transition2 = transition1).start == NavType.Teleport && teleport_transitions.ContainsKey(cell))
          {
            int x1;
            int y1;
            Grid.CellToXY(cell, out x1, out y1);
            int teleportTransition = teleport_transitions[cell];
            int x2;
            int y2;
            Grid.CellToXY(teleport_transitions[cell], out x2, out y2);
            transition2.x = x2 - x1;
            transition2.y = y2 - y1;
          }
          int link = transition2.IsValid(cell, nav_table);
          if (link != Grid.InvalidCell)
          {
            links[index1] = new NavGrid.Link(link, transition2.start, transition2.end, transition2.id, transition2.cost);
            ++index1;
            ++num;
          }
        }
      }
    }
    if (num >= max_links_per_cell)
      Debug.LogError((object) ("Out of nav links. Need to increase maxLinksPerCell:" + max_links_per_cell.ToString()));
    links[index1].link = Grid.InvalidCell;
  }

  private static void MarkValidCells(
    NavTable nav_table,
    NavTableValidator[] validators,
    CellOffset[] bounding_offsets)
  {
    WorkItemCollection<NavGridUpdater.MarkValidCellWorkItem, object> workItemCollection = new WorkItemCollection<NavGridUpdater.MarkValidCellWorkItem, object>();
    workItemCollection.Reset((object) null);
    for (int index = 0; index < Grid.HeightInCells; ++index)
      workItemCollection.Add(new NavGridUpdater.MarkValidCellWorkItem(Grid.OffsetCell(0, new CellOffset(0, index)), nav_table, bounding_offsets, validators));
    GlobalJobManager.Run((IWorkItemCollection) workItemCollection);
  }

  public static void DebugDrawPath(int start_cell, int end_cell)
  {
    Grid.CellToPosCCF(start_cell, Grid.SceneLayer.Move);
    Grid.CellToPosCCF(end_cell, Grid.SceneLayer.Move);
  }

  public static void DebugDrawPath(PathFinder.Path path)
  {
    if (path.nodes == null)
      return;
    for (int index = 0; index < path.nodes.Count - 1; ++index)
      NavGridUpdater.DebugDrawPath(path.nodes[index].cell, path.nodes[index + 1].cell);
  }

  private struct CreateLinkWorkItem : IWorkItem<object>
  {
    private int startCell;
    private NavTable navTable;
    private int maxLinksPerCell;
    private NavGrid.Link[] links;
    private NavGrid.Transition[][] transitionsByNavType;
    private Dictionary<int, int> teleportTransitions;

    public CreateLinkWorkItem(
      int start_cell,
      NavTable nav_table,
      int max_links_per_cell,
      NavGrid.Link[] links,
      NavGrid.Transition[][] transitions_by_nav_type,
      Dictionary<int, int> teleport_transitions)
    {
      this.startCell = start_cell;
      this.navTable = nav_table;
      this.maxLinksPerCell = max_links_per_cell;
      this.links = links;
      this.transitionsByNavType = transitions_by_nav_type;
      this.teleportTransitions = teleport_transitions;
    }

    public void Run(object shared_data)
    {
      for (int index = 0; index < Grid.WidthInCells; ++index)
        NavGridUpdater.CreateLinksForCell(this.startCell + index, this.navTable, this.maxLinksPerCell, this.links, this.transitionsByNavType, this.teleportTransitions);
    }
  }

  private struct MarkValidCellWorkItem : IWorkItem<object>
  {
    private NavTable navTable;
    private CellOffset[] boundingOffsets;
    private NavTableValidator[] validators;
    private int startCell;

    public MarkValidCellWorkItem(
      int start_cell,
      NavTable nav_table,
      CellOffset[] bounding_offsets,
      NavTableValidator[] validators)
    {
      this.startCell = start_cell;
      this.navTable = nav_table;
      this.boundingOffsets = bounding_offsets;
      this.validators = validators;
    }

    public void Run(object shared_data)
    {
      for (int index = 0; index < Grid.WidthInCells; ++index)
      {
        int cell = this.startCell + index;
        foreach (NavTableValidator validator in this.validators)
          validator.UpdateCell(cell, this.navTable, this.boundingOffsets);
      }
    }
  }
}
