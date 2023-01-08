// Decompiled with JetBrains decompiler
// Type: ConditionFlightPathIsClear
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ConditionFlightPathIsClear : ProcessCondition
{
  private GameObject module;
  private int bufferWidth;
  private bool hasClearSky;
  private int obstructedTile = -1;
  public const int MAXIMUM_ROCKET_HEIGHT = 35;

  public ConditionFlightPathIsClear(GameObject module, int bufferWidth)
  {
    this.module = module;
    this.bufferWidth = bufferWidth;
  }

  public override ProcessCondition.Status EvaluateCondition()
  {
    this.Update();
    return !this.hasClearSky ? ProcessCondition.Status.Failure : ProcessCondition.Status.Ready;
  }

  public override StatusItem GetStatusItem(ProcessCondition.Status status) => status == ProcessCondition.Status.Failure ? Db.Get().BuildingStatusItems.PathNotClear : (StatusItem) null;

  public override string GetStatusMessage(ProcessCondition.Status status)
  {
    if (DlcManager.FeatureClusterSpaceEnabled())
      return (string) (status == ProcessCondition.Status.Ready ? UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.STATUS.READY : UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.STATUS.FAILURE);
    if (status != ProcessCondition.Status.Ready)
      return Db.Get().BuildingStatusItems.PathNotClear.notificationText;
    Debug.LogError((object) "ConditionFlightPathIsClear: You'll need to add new strings/status items if you want to show the ready state");
    return "";
  }

  public override string GetStatusTooltip(ProcessCondition.Status status)
  {
    if (DlcManager.FeatureClusterSpaceEnabled())
      return (string) (status == ProcessCondition.Status.Ready ? UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.TOOLTIP.READY : UI.STARMAP.LAUNCHCHECKLIST.FLIGHT_PATH_CLEAR.TOOLTIP.FAILURE);
    if (status != ProcessCondition.Status.Ready)
      return Db.Get().BuildingStatusItems.PathNotClear.notificationTooltipText;
    Debug.LogError((object) "ConditionFlightPathIsClear: You'll need to add new strings/status items if you want to show the ready state");
    return "";
  }

  public override bool ShowInUI() => DlcManager.FeatureClusterSpaceEnabled();

  public void Update()
  {
    Extents extents = this.module.GetComponent<Building>().GetExtents();
    int x1 = extents.x - this.bufferWidth;
    int x2 = extents.x + extents.width - 1 + this.bufferWidth;
    int y1 = extents.y;
    int cell1 = Grid.XYToCell(x1, y1);
    int y2 = y1;
    int cell2 = Grid.XYToCell(x2, y2);
    this.hasClearSky = true;
    this.obstructedTile = -1;
    for (int startCell = cell1; startCell <= cell2; ++startCell)
    {
      if (!ConditionFlightPathIsClear.CanReachSpace(startCell, out this.obstructedTile))
      {
        this.hasClearSky = false;
        break;
      }
    }
  }

  public static int PadTopEdgeDistanceToCeilingEdge(GameObject launchpad)
  {
    Vector2 maximumBounds = launchpad.GetMyWorld().maximumBounds;
    int y1 = (int) launchpad.GetMyWorld().maximumBounds.y;
    int y2 = Grid.CellToXY(launchpad.GetComponent<LaunchPad>().RocketBottomPosition).y;
    int topBorderHeight = Grid.TopBorderHeight;
    return y1 - topBorderHeight - y2 + 1;
  }

  public static bool CheckFlightPathClear(
    CraftModuleInterface craft,
    GameObject launchpad,
    out int obstruction)
  {
    Vector2I xy = Grid.CellToXY(launchpad.GetComponent<LaunchPad>().RocketBottomPosition);
    int ceilingEdge = ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(launchpad);
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) craft.ClusterModules)
    {
      Building component = ((Component) clusterModule.Get()).GetComponent<Building>();
      int widthInCells = component.Def.WidthInCells;
      int verticalPosition = craft.GetModuleRelativeVerticalPosition(((Component) clusterModule.Get()).gameObject);
      if (verticalPosition + component.Def.HeightInCells > ceilingEdge)
      {
        int cell = Grid.XYToCell(xy.x, verticalPosition + xy.y);
        obstruction = cell;
        return false;
      }
      for (int index1 = verticalPosition; index1 < ceilingEdge; ++index1)
      {
        for (int index2 = 0; index2 < widthInCells; ++index2)
        {
          int cell = Grid.XYToCell(index2 + (xy.x - widthInCells / 2), index1 + xy.y);
          bool flag = Grid.Solid[cell];
          if (((!Grid.IsValidCell(cell) ? 1 : ((int) Grid.WorldIdx[cell] != (int) Grid.WorldIdx[launchpad.GetComponent<LaunchPad>().RocketBottomPosition] ? 1 : 0)) | (flag ? 1 : 0)) != 0)
          {
            obstruction = cell;
            return false;
          }
        }
      }
    }
    obstruction = -1;
    return true;
  }

  private static bool CanReachSpace(int startCell, out int obstruction)
  {
    WorldContainer worldContainer = startCell >= 0 ? ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[startCell]) : (WorldContainer) null;
    int num = Object.op_Equality((Object) worldContainer, (Object) null) ? Grid.HeightInCells : (int) worldContainer.maximumBounds.y;
    obstruction = -1;
    for (int index = startCell; Grid.CellRow(index) < num; index = Grid.CellAbove(index))
    {
      if (!Grid.IsValidCell(index) || Grid.Solid[index])
      {
        obstruction = index;
        return false;
      }
    }
    return true;
  }

  public string GetObstruction()
  {
    if (this.obstructedTile == -1)
      return (string) null;
    return Object.op_Inequality((Object) Grid.Objects[this.obstructedTile, 1], (Object) null) ? Grid.Objects[this.obstructedTile, 1].GetComponent<Building>().Def.Name : string.Format((string) BUILDING.STATUSITEMS.PATH_NOT_CLEAR.TILE_FORMAT, (object) Grid.Element[this.obstructedTile].tag.ProperName());
  }
}
