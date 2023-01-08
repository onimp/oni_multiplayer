// Decompiled with JetBrains decompiler
// Type: Grid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class Grid
{
  public static readonly CellOffset[] DefaultOffset = new CellOffset[1];
  public static float WidthInMeters;
  public static float HeightInMeters;
  public static int WidthInCells;
  public static int HeightInCells;
  public static float CellSizeInMeters;
  public static float InverseCellSizeInMeters;
  public static float HalfCellSizeInMeters;
  public static int CellCount;
  public static int InvalidCell = -1;
  public static int TopBorderHeight = 2;
  public static Dictionary<int, GameObject>[] ObjectLayers;
  public static Action<int> OnReveal;
  public static Grid.BuildFlags[] BuildMasks;
  public static Grid.BuildFlagsFoundationIndexer Foundation;
  public static Grid.BuildFlagsSolidIndexer Solid;
  public static Grid.BuildFlagsDupeImpassableIndexer DupeImpassable;
  public static Grid.BuildFlagsFakeFloorIndexer FakeFloor;
  public static Grid.BuildFlagsDupePassableIndexer DupePassable;
  public static Grid.BuildFlagsImpassableIndexer CritterImpassable;
  public static Grid.BuildFlagsDoorIndexer HasDoor;
  public static Grid.VisFlags[] VisMasks;
  public static Grid.VisFlagsRevealedIndexer Revealed;
  public static Grid.VisFlagsPreventFogOfWarRevealIndexer PreventFogOfWarReveal;
  public static Grid.VisFlagsRenderedByWorldIndexer RenderedByWorld;
  public static Grid.VisFlagsAllowPathfindingIndexer AllowPathfinding;
  public static Grid.NavValidatorFlags[] NavValidatorMasks;
  public static Grid.NavValidatorFlagsLadderIndexer HasLadder;
  public static Grid.NavValidatorFlagsPoleIndexer HasPole;
  public static Grid.NavValidatorFlagsTubeIndexer HasTube;
  public static Grid.NavValidatorFlagsNavTeleporterIndexer HasNavTeleporter;
  public static Grid.NavValidatorFlagsUnderConstructionIndexer IsTileUnderConstruction;
  public static Grid.NavFlags[] NavMasks;
  private static Grid.NavFlagsAccessDoorIndexer HasAccessDoor;
  public static Grid.NavFlagsTubeEntranceIndexer HasTubeEntrance;
  public static Grid.NavFlagsPreventIdleTraversalIndexer PreventIdleTraversal;
  public static Grid.NavFlagsReservedIndexer Reserved;
  public static Grid.NavFlagsSuitMarkerIndexer HasSuitMarker;
  private static Dictionary<int, Grid.Restriction> restrictions = new Dictionary<int, Grid.Restriction>();
  private static Dictionary<int, Grid.TubeEntrance> tubeEntrances = new Dictionary<int, Grid.TubeEntrance>();
  private static Dictionary<int, Grid.SuitMarker> suitMarkers = new Dictionary<int, Grid.SuitMarker>();
  public static unsafe ushort* elementIdx;
  public static unsafe float* temperature;
  public static unsafe float* radiation;
  public static unsafe float* mass;
  public static unsafe byte* properties;
  public static unsafe byte* strengthInfo;
  public static unsafe byte* insulation;
  public static unsafe byte* diseaseIdx;
  public static unsafe int* diseaseCount;
  public static unsafe byte* exposedToSunlight;
  public static unsafe float* AccumulatedFlowValues = (float*) null;
  public static byte[] Visible;
  public static byte[] Spawnable;
  public static float[] Damage;
  public static float[] Decor;
  public static bool[] GravitasFacility;
  public static byte[] WorldIdx;
  public static float[] Loudness;
  public static global::Element[] Element;
  public static int[] LightCount;
  public static Grid.PressureIndexer Pressure;
  public static Grid.TransparentIndexer Transparent;
  public static Grid.ElementIdxIndexer ElementIdx;
  public static Grid.TemperatureIndexer Temperature;
  public static Grid.RadiationIndexer Radiation;
  public static Grid.MassIndexer Mass;
  public static Grid.PropertiesIndexer Properties;
  public static Grid.ExposedToSunlightIndexer ExposedToSunlight;
  public static Grid.StrengthInfoIndexer StrengthInfo;
  public static Grid.Insulationndexer Insulation;
  public static Grid.DiseaseIdxIndexer DiseaseIdx;
  public static Grid.DiseaseCountIndexer DiseaseCount;
  public static Grid.LightIntensityIndexer LightIntensity;
  public static Grid.AccumulatedFlowIndexer AccumulatedFlow;
  public static Grid.ObjectLayerIndexer Objects;
  public static float LayerMultiplier = 1f;
  private static readonly Func<int, bool> VisibleBlockingDelegate = (Func<int, bool>) (cell => Grid.VisibleBlockingCB(cell));
  private static readonly Func<int, bool> PhysicalBlockingDelegate = (Func<int, bool>) (cell => Grid.PhysicalBlockingCB(cell));

  private static void UpdateBuildMask(int i, Grid.BuildFlags flag, bool state)
  {
    if (state)
      Grid.BuildMasks[i] |= flag;
    else
      Grid.BuildMasks[i] &= ~flag;
  }

  public static void SetSolid(int cell, bool solid, CellSolidEvent ev) => Grid.UpdateBuildMask(cell, Grid.BuildFlags.Solid, solid);

  private static void UpdateVisMask(int i, Grid.VisFlags flag, bool state)
  {
    if (state)
      Grid.VisMasks[i] |= flag;
    else
      Grid.VisMasks[i] &= ~flag;
  }

  private static void UpdateNavValidatorMask(int i, Grid.NavValidatorFlags flag, bool state)
  {
    if (state)
      Grid.NavValidatorMasks[i] |= flag;
    else
      Grid.NavValidatorMasks[i] &= ~flag;
  }

  private static void UpdateNavMask(int i, Grid.NavFlags flag, bool state)
  {
    if (state)
      Grid.NavMasks[i] |= flag;
    else
      Grid.NavMasks[i] &= ~flag;
  }

  public static void ResetNavMasksAndDetails()
  {
    Grid.NavMasks = (Grid.NavFlags[]) null;
    Grid.tubeEntrances.Clear();
    Grid.restrictions.Clear();
    Grid.suitMarkers.Clear();
  }

  public static bool DEBUG_GetRestrictions(int cell, out Grid.Restriction restriction) => Grid.restrictions.TryGetValue(cell, out restriction);

  public static void RegisterRestriction(int cell, Grid.Restriction.Orientation orientation)
  {
    Grid.HasAccessDoor[cell] = true;
    Grid.restrictions[cell] = new Grid.Restriction()
    {
      DirectionMasksForMinionInstanceID = new Dictionary<int, Grid.Restriction.Directions>(),
      orientation = orientation
    };
  }

  public static void UnregisterRestriction(int cell)
  {
    Grid.restrictions.Remove(cell);
    Grid.HasAccessDoor[cell] = false;
  }

  public static void SetRestriction(
    int cell,
    int minionInstanceID,
    Grid.Restriction.Directions directions)
  {
    Grid.restrictions[cell].DirectionMasksForMinionInstanceID[minionInstanceID] = directions;
  }

  public static void ClearRestriction(int cell, int minionInstanceID) => Grid.restrictions[cell].DirectionMasksForMinionInstanceID.Remove(minionInstanceID);

  public static bool HasPermission(
    int cell,
    int minionInstanceID,
    int fromCell,
    NavType fromNavType)
  {
    if (!Grid.HasAccessDoor[cell])
      return true;
    Grid.Restriction restriction = Grid.restrictions[cell];
    Vector2I xy1 = Grid.CellToXY(cell);
    Vector2I xy2 = Grid.CellToXY(fromCell);
    Grid.Restriction.Directions directions1 = (Grid.Restriction.Directions) 0;
    int num1 = xy1.x - xy2.x;
    int num2 = xy1.y - xy2.y;
    switch (restriction.orientation)
    {
      case Grid.Restriction.Orientation.Vertical:
        if (num1 < 0)
          directions1 |= Grid.Restriction.Directions.Left;
        if (num1 > 0)
        {
          directions1 |= Grid.Restriction.Directions.Right;
          break;
        }
        break;
      case Grid.Restriction.Orientation.Horizontal:
        if (num2 > 0)
          directions1 |= Grid.Restriction.Directions.Left;
        if (num2 < 0)
        {
          directions1 |= Grid.Restriction.Directions.Right;
          break;
        }
        break;
      case Grid.Restriction.Orientation.SingleCell:
        if (Math.Abs(num1) != 1 && Math.Abs(num2) != 1 && fromNavType != NavType.Teleport)
        {
          directions1 |= Grid.Restriction.Directions.Teleport;
          break;
        }
        break;
    }
    Grid.Restriction.Directions directions2 = (Grid.Restriction.Directions) 0;
    return !restriction.DirectionMasksForMinionInstanceID.TryGetValue(minionInstanceID, out directions2) && !restriction.DirectionMasksForMinionInstanceID.TryGetValue(-1, out directions2) || (directions2 & directions1) == (Grid.Restriction.Directions) 0;
  }

  public static void RegisterTubeEntrance(int cell, int reservationCapacity)
  {
    DebugUtil.Assert(!Grid.tubeEntrances.ContainsKey(cell));
    Grid.HasTubeEntrance[cell] = true;
    Grid.tubeEntrances[cell] = new Grid.TubeEntrance()
    {
      reservationCapacity = reservationCapacity,
      reservedInstanceIDs = new HashSet<int>()
    };
  }

  public static void UnregisterTubeEntrance(int cell)
  {
    DebugUtil.Assert(Grid.tubeEntrances.ContainsKey(cell));
    Grid.HasTubeEntrance[cell] = false;
    Grid.tubeEntrances.Remove(cell);
  }

  public static bool ReserveTubeEntrance(int cell, int minionInstanceID, bool reserve)
  {
    Grid.TubeEntrance tubeEntrance = Grid.tubeEntrances[cell];
    HashSet<int> reservedInstanceIds = tubeEntrance.reservedInstanceIDs;
    if (!reserve)
      return reservedInstanceIds.Remove(minionInstanceID);
    DebugUtil.Assert(Grid.HasTubeEntrance[cell]);
    if (reservedInstanceIds.Count == tubeEntrance.reservationCapacity)
      return false;
    DebugUtil.Assert(reservedInstanceIds.Add(minionInstanceID));
    return true;
  }

  public static void SetTubeEntranceReservationCapacity(int cell, int newReservationCapacity)
  {
    DebugUtil.Assert(Grid.HasTubeEntrance[cell]);
    Grid.TubeEntrance tubeEntrance = Grid.tubeEntrances[cell] with
    {
      reservationCapacity = newReservationCapacity
    };
    Grid.tubeEntrances[cell] = tubeEntrance;
  }

  public static bool HasUsableTubeEntrance(int cell, int minionInstanceID)
  {
    if (!Grid.HasTubeEntrance[cell])
      return false;
    Grid.TubeEntrance tubeEntrance = Grid.tubeEntrances[cell];
    if (!tubeEntrance.operational)
      return false;
    HashSet<int> reservedInstanceIds = tubeEntrance.reservedInstanceIDs;
    return reservedInstanceIds.Count < tubeEntrance.reservationCapacity || reservedInstanceIds.Contains(minionInstanceID);
  }

  public static bool HasReservedTubeEntrance(int cell, int minionInstanceID)
  {
    DebugUtil.Assert(Grid.HasTubeEntrance[cell]);
    return Grid.tubeEntrances[cell].reservedInstanceIDs.Contains(minionInstanceID);
  }

  public static void SetTubeEntranceOperational(int cell, bool operational)
  {
    DebugUtil.Assert(Grid.HasTubeEntrance[cell]);
    Grid.TubeEntrance tubeEntrance = Grid.tubeEntrances[cell] with
    {
      operational = operational
    };
    Grid.tubeEntrances[cell] = tubeEntrance;
  }

  public static void RegisterSuitMarker(int cell)
  {
    DebugUtil.Assert(!Grid.HasSuitMarker[cell]);
    Grid.HasSuitMarker[cell] = true;
    Grid.suitMarkers[cell] = new Grid.SuitMarker()
    {
      suitCount = 0,
      lockerCount = 0,
      flags = Grid.SuitMarker.Flags.Operational,
      minionIDsWithSuitReservations = new HashSet<int>(),
      minionIDsWithEmptyLockerReservations = new HashSet<int>()
    };
  }

  public static void UnregisterSuitMarker(int cell)
  {
    DebugUtil.Assert(Grid.HasSuitMarker[cell]);
    Grid.HasSuitMarker[cell] = false;
    Grid.suitMarkers.Remove(cell);
  }

  public static bool ReserveSuit(int cell, int minionInstanceID, bool reserve)
  {
    DebugUtil.Assert(Grid.HasSuitMarker[cell]);
    Grid.SuitMarker suitMarker = Grid.suitMarkers[cell];
    HashSet<int> suitReservations = suitMarker.minionIDsWithSuitReservations;
    if (!reserve)
      return suitReservations.Remove(minionInstanceID);
    if (suitReservations.Count == suitMarker.suitCount)
      return false;
    DebugUtil.Assert(suitReservations.Add(minionInstanceID));
    return true;
  }

  public static bool ReserveEmptyLocker(int cell, int minionInstanceID, bool reserve)
  {
    DebugUtil.Assert(Grid.HasSuitMarker[cell]);
    Grid.SuitMarker suitMarker = Grid.suitMarkers[cell];
    HashSet<int> lockerReservations = suitMarker.minionIDsWithEmptyLockerReservations;
    if (!reserve)
      return lockerReservations.Remove(minionInstanceID);
    if (lockerReservations.Count == suitMarker.emptyLockerCount)
      return false;
    DebugUtil.Assert(lockerReservations.Add(minionInstanceID));
    return true;
  }

  public static void UpdateSuitMarker(
    int cell,
    int fullLockerCount,
    int emptyLockerCount,
    Grid.SuitMarker.Flags flags,
    PathFinder.PotentialPath.Flags pathFlags)
  {
    DebugUtil.Assert(Grid.HasSuitMarker[cell]);
    Grid.SuitMarker suitMarker = Grid.suitMarkers[cell] with
    {
      suitCount = fullLockerCount,
      lockerCount = fullLockerCount + emptyLockerCount,
      flags = flags,
      pathFlags = pathFlags
    };
    Grid.suitMarkers[cell] = suitMarker;
  }

  public static bool TryGetSuitMarkerFlags(
    int cell,
    out Grid.SuitMarker.Flags flags,
    out PathFinder.PotentialPath.Flags pathFlags)
  {
    if (Grid.HasSuitMarker[cell])
    {
      flags = Grid.suitMarkers[cell].flags;
      pathFlags = Grid.suitMarkers[cell].pathFlags;
      return true;
    }
    flags = (Grid.SuitMarker.Flags) 0;
    pathFlags = PathFinder.PotentialPath.Flags.None;
    return false;
  }

  public static bool HasSuit(int cell, int minionInstanceID)
  {
    if (!Grid.HasSuitMarker[cell])
      return false;
    Grid.SuitMarker suitMarker = Grid.suitMarkers[cell];
    HashSet<int> suitReservations = suitMarker.minionIDsWithSuitReservations;
    return suitReservations.Count < suitMarker.suitCount || suitReservations.Contains(minionInstanceID);
  }

  public static bool HasEmptyLocker(int cell, int minionInstanceID)
  {
    if (!Grid.HasSuitMarker[cell])
      return false;
    Grid.SuitMarker suitMarker = Grid.suitMarkers[cell];
    HashSet<int> lockerReservations = suitMarker.minionIDsWithEmptyLockerReservations;
    return lockerReservations.Count < suitMarker.emptyLockerCount || lockerReservations.Contains(minionInstanceID);
  }

  public static unsafe void InitializeCells()
  {
    for (int index1 = 0; index1 != Grid.WidthInCells * Grid.HeightInCells; ++index1)
    {
      ushort index2 = Grid.elementIdx[index1];
      global::Element element = ElementLoader.elements[(int) index2];
      Grid.Element[index1] = element;
      if (element.IsSolid)
        Grid.BuildMasks[index1] |= Grid.BuildFlags.Solid;
      else
        Grid.BuildMasks[index1] &= Grid.BuildFlags.FakeFloor | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable;
      Grid.RenderedByWorld[index1] = element.substance != null && element.substance.renderedByWorld && Object.op_Equality((Object) Grid.Objects[index1, 9], (Object) null);
    }
  }

  public static unsafe bool IsInitialized() => (IntPtr) Grid.mass != IntPtr.Zero;

  public static int GetCellInDirection(int cell, Direction d)
  {
    switch (d)
    {
      case Direction.Up:
        return Grid.CellAbove(cell);
      case Direction.Right:
        return Grid.CellRight(cell);
      case Direction.Down:
        return Grid.CellBelow(cell);
      case Direction.Left:
        return Grid.CellLeft(cell);
      case Direction.None:
        return cell;
      default:
        return -1;
    }
  }

  public static bool Raycast(
    int cell,
    Vector2I direction,
    out int hitDistance,
    int maxDistance = 100,
    Grid.BuildFlags layerMask = Grid.BuildFlags.Any)
  {
    bool flag = false;
    Vector2I vector2I1 = Grid.CellToXY(cell);
    Vector2I vector2I2 = Vector2I.op_Addition(vector2I1, Vector2I.op_Multiply(direction, maxDistance));
    int cell1 = cell;
    int cell2 = Grid.XYToCell(vector2I2.x, vector2I2.y);
    int num1 = 0;
    int num2 = 0;
    for (float num3 = (float) maxDistance * 0.5f; (double) num1 < (double) num3; ++num1)
    {
      if (!Grid.IsValidCell(cell1) || (Grid.BuildMasks[cell1] & layerMask) != ~Grid.BuildFlags.Any)
      {
        flag = true;
        break;
      }
      if (!Grid.IsValidCell(cell2) || (Grid.BuildMasks[cell2] & layerMask) != ~Grid.BuildFlags.Any)
        num2 = maxDistance - num1;
      vector2I1 = Vector2I.op_Addition(vector2I1, direction);
      vector2I2 = Vector2I.op_Subtraction(vector2I2, direction);
      cell1 = Grid.XYToCell(vector2I1.x, vector2I1.y);
      cell2 = Grid.XYToCell(vector2I2.x, vector2I2.y);
    }
    if (!flag && maxDistance % 2 == 0)
      flag = !Grid.IsValidCell(cell2) || (Grid.BuildMasks[cell2] & layerMask) != 0;
    hitDistance = flag ? num1 : (num2 > 0 ? num2 : maxDistance);
    return flag | hitDistance == num2;
  }

  public static int CellAbove(int cell) => cell + Grid.WidthInCells;

  public static int CellBelow(int cell) => cell - Grid.WidthInCells;

  public static int CellLeft(int cell) => cell % Grid.WidthInCells <= 0 ? -1 : cell - 1;

  public static int CellRight(int cell) => cell % Grid.WidthInCells >= Grid.WidthInCells - 1 ? -1 : cell + 1;

  public static CellOffset GetOffset(int cell)
  {
    int x = 0;
    int y = 0;
    Grid.CellToXY(cell, out x, out y);
    return new CellOffset(x, y);
  }

  public static int CellUpLeft(int cell)
  {
    int num = -1;
    if (cell < (Grid.HeightInCells - 1) * Grid.WidthInCells && cell % Grid.WidthInCells > 0)
      num = cell - 1 + Grid.WidthInCells;
    return num;
  }

  public static int CellUpRight(int cell)
  {
    int num = -1;
    if (cell < (Grid.HeightInCells - 1) * Grid.WidthInCells && cell % Grid.WidthInCells < Grid.WidthInCells - 1)
      num = cell + 1 + Grid.WidthInCells;
    return num;
  }

  public static int CellDownLeft(int cell)
  {
    int num = -1;
    if (cell > Grid.WidthInCells && cell % Grid.WidthInCells > 0)
      num = cell - 1 - Grid.WidthInCells;
    return num;
  }

  public static int CellDownRight(int cell)
  {
    int num = -1;
    if (cell >= Grid.WidthInCells && cell % Grid.WidthInCells < Grid.WidthInCells - 1)
      num = cell + 1 - Grid.WidthInCells;
    return num;
  }

  public static bool IsCellLeftOf(int cell, int other_cell) => Grid.CellColumn(cell) < Grid.CellColumn(other_cell);

  public static bool IsCellOffsetOf(int cell, int target_cell, CellOffset[] target_offsets)
  {
    int length = target_offsets.Length;
    for (int index = 0; index < length; ++index)
    {
      if (cell == Grid.OffsetCell(target_cell, target_offsets[index]))
        return true;
    }
    return false;
  }

  public static bool IsCellOffsetOf(int cell, GameObject target, CellOffset[] target_offsets)
  {
    int cell1 = Grid.PosToCell(target);
    return Grid.IsCellOffsetOf(cell, cell1, target_offsets);
  }

  public static int GetCellDistance(int cell_a, int cell_b)
  {
    CellOffset offset = Grid.GetOffset(cell_a, cell_b);
    return Math.Abs(offset.x) + Math.Abs(offset.y);
  }

  public static int GetCellRange(int cell_a, int cell_b)
  {
    CellOffset offset = Grid.GetOffset(cell_a, cell_b);
    return Math.Max(Math.Abs(offset.x), Math.Abs(offset.y));
  }

  public static CellOffset GetOffset(int base_cell, int offset_cell)
  {
    int x1;
    int y1;
    Grid.CellToXY(base_cell, out x1, out y1);
    int x2;
    int y2;
    Grid.CellToXY(offset_cell, out x2, out y2);
    return new CellOffset(x2 - x1, y2 - y1);
  }

  public static int OffsetCell(int cell, CellOffset offset) => cell + offset.x + offset.y * Grid.WidthInCells;

  public static int OffsetCell(int cell, int x, int y) => cell + x + y * Grid.WidthInCells;

  public static bool IsCellOffsetValid(int cell, int x, int y)
  {
    int x1;
    int y1;
    Grid.CellToXY(cell, out x1, out y1);
    return x1 + x >= 0 && x1 + x < Grid.WidthInCells && y1 + y >= 0 && y1 + y < Grid.HeightInCells;
  }

  public static bool IsCellOffsetValid(int cell, CellOffset offset) => Grid.IsCellOffsetValid(cell, offset.x, offset.y);

  public static int PosToCell(StateMachine.Instance smi) => Grid.PosToCell(TransformExtensions.GetPosition(smi.transform));

  public static int PosToCell(GameObject go) => Grid.PosToCell(TransformExtensions.GetPosition(go.transform));

  public static int PosToCell(KMonoBehaviour cmp) => Grid.PosToCell(TransformExtensions.GetPosition(cmp.transform));

  public static bool IsValidBuildingCell(int cell)
  {
    if (!Grid.IsWorldValidCell(cell))
      return false;
    WorldContainer world = ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[cell]);
    if (Object.op_Equality((Object) world, (Object) null))
      return false;
    Vector2I xy = Grid.CellToXY(cell);
    return (double) xy.x >= (double) world.minimumBounds.x && (double) xy.x <= (double) world.maximumBounds.x && (double) xy.y >= (double) world.minimumBounds.y && (double) xy.y <= (double) world.maximumBounds.y - (double) Grid.TopBorderHeight;
  }

  public static bool IsWorldValidCell(int cell) => Grid.IsValidCell(cell) && (int) Grid.WorldIdx[cell] != (int) ClusterManager.INVALID_WORLD_IDX;

  public static bool IsValidCell(int cell) => cell >= 0 && cell < Grid.CellCount;

  public static bool IsValidCellInWorld(int cell, int world) => cell >= 0 && cell < Grid.CellCount && (int) Grid.WorldIdx[cell] == world;

  public static bool IsActiveWorld(int cell) => Object.op_Inequality((Object) ClusterManager.Instance, (Object) null) && ClusterManager.Instance.activeWorldId == (int) Grid.WorldIdx[cell];

  public static bool AreCellsInSameWorld(int cell, int world_cell) => Grid.IsValidCell(cell) && Grid.IsValidCell(world_cell) && (int) Grid.WorldIdx[cell] == (int) Grid.WorldIdx[world_cell];

  public static bool IsCellOpenToSpace(int cell) => !Grid.IsSolidCell(cell) && !Object.op_Inequality((Object) Grid.Objects[cell, 2], (Object) null) && World.Instance.zoneRenderData.GetSubWorldZoneType(cell) == 7;

  public static int PosToCell(Vector2 pos)
  {
    float x = pos.x;
    int num1 = (int) ((double) pos.y + 0.05000000074505806);
    int num2 = (int) x;
    int widthInCells = Grid.WidthInCells;
    return num1 * widthInCells + num2;
  }

  public static int PosToCell(Vector3 pos)
  {
    float x = pos.x;
    int num1 = (int) ((double) pos.y + 0.05000000074505806);
    int num2 = (int) x;
    int widthInCells = Grid.WidthInCells;
    return num1 * widthInCells + num2;
  }

  public static void PosToXY(Vector3 pos, out int x, out int y) => Grid.CellToXY(Grid.PosToCell(pos), out x, out y);

  public static void PosToXY(Vector3 pos, out Vector2I xy) => Grid.CellToXY(Grid.PosToCell(pos), out xy.x, out xy.y);

  public static Vector2I PosToXY(Vector3 pos)
  {
    Vector2I xy;
    Grid.CellToXY(Grid.PosToCell(pos), out xy.x, out xy.y);
    return xy;
  }

  public static int XYToCell(int x, int y) => x + y * Grid.WidthInCells;

  public static void CellToXY(int cell, out int x, out int y)
  {
    x = Grid.CellColumn(cell);
    y = Grid.CellRow(cell);
  }

  public static Vector2I CellToXY(int cell) => new Vector2I(Grid.CellColumn(cell), Grid.CellRow(cell));

  public static Vector3 CellToPos(int cell, float x_offset, float y_offset, float z_offset)
  {
    int widthInCells = Grid.WidthInCells;
    double num1 = (double) Grid.CellSizeInMeters * (double) (cell % widthInCells);
    float num2 = Grid.CellSizeInMeters * (float) (cell / widthInCells);
    double num3 = (double) x_offset;
    return new Vector3((float) (num1 + num3), num2 + y_offset, z_offset);
  }

  public static Vector3 CellToPos(int cell)
  {
    int widthInCells = Grid.WidthInCells;
    return new Vector3(Grid.CellSizeInMeters * (float) (cell % widthInCells), Grid.CellSizeInMeters * (float) (cell / widthInCells), 0.0f);
  }

  public static Vector3 CellToPos2D(int cell)
  {
    int widthInCells = Grid.WidthInCells;
    return Vector2.op_Implicit(new Vector2(Grid.CellSizeInMeters * (float) (cell % widthInCells), Grid.CellSizeInMeters * (float) (cell / widthInCells)));
  }

  public static int CellRow(int cell) => cell / Grid.WidthInCells;

  public static int CellColumn(int cell) => cell % Grid.WidthInCells;

  public static int ClampX(int x) => Math.Min(Math.Max(x, 0), Grid.WidthInCells - 1);

  public static int ClampY(int y) => Math.Min(Math.Max(y, 0), Grid.HeightInCells - 1);

  public static Vector2I Constrain(Vector2I val)
  {
    val.x = Mathf.Max(0, Mathf.Min(val.x, Grid.WidthInCells - 1));
    val.y = Mathf.Max(0, Mathf.Min(val.y, Grid.HeightInCells - 1));
    return val;
  }

  public static void Reveal(int cell, byte visibility = 255, bool forceReveal = false)
  {
    int num = Grid.Spawnable[cell] != (byte) 0 ? 0 : (visibility > (byte) 0 ? 1 : 0);
    Grid.Spawnable[cell] = Math.Max(visibility, Grid.Visible[cell]);
    if (forceReveal || !Grid.PreventFogOfWarReveal[cell])
      Grid.Visible[cell] = Math.Max(visibility, Grid.Visible[cell]);
    if (num == 0 || Grid.OnReveal == null)
      return;
    Grid.OnReveal(cell);
  }

  public static ObjectLayer GetObjectLayerForConduitType(ConduitType conduit_type)
  {
    switch (conduit_type)
    {
      case ConduitType.Gas:
        return ObjectLayer.GasConduitConnection;
      case ConduitType.Liquid:
        return ObjectLayer.LiquidConduitConnection;
      case ConduitType.Solid:
        return ObjectLayer.SolidConduitConnection;
      default:
        throw new ArgumentException("Invalid value.", nameof (conduit_type));
    }
  }

  public static Vector3 CellToPos(int cell, CellAlignment alignment, Grid.SceneLayer layer)
  {
    switch ((int) alignment)
    {
      case 1:
        return Grid.CellToPosCBC(cell, layer);
      case 2:
        return Grid.CellToPosCTC(cell, layer);
      case 3:
        return Grid.CellToPosLCC(cell, layer);
      case 4:
        return Grid.CellToPosRCC(cell, layer);
      case 5:
        Vector3 vector3;
        // ISSUE: explicit constructor call
        ((Vector3) ref vector3).\u002Ector(Random.Range(-0.3f, 0.3f), 0.0f, 0.0f);
        return Vector3.op_Addition(Grid.CellToPosCCC(cell, layer), vector3);
      default:
        return Grid.CellToPosCCC(cell, layer);
    }
  }

  public static float GetLayerZ(Grid.SceneLayer layer) => (float) (-(double) Grid.HalfCellSizeInMeters - (double) Grid.CellSizeInMeters * (double) layer * (double) Grid.LayerMultiplier);

  public static Vector3 CellToPosCCC(int cell, Grid.SceneLayer layer) => Grid.CellToPos(cell, Grid.HalfCellSizeInMeters, Grid.HalfCellSizeInMeters, Grid.GetLayerZ(layer));

  public static Vector3 CellToPosCBC(int cell, Grid.SceneLayer layer) => Grid.CellToPos(cell, Grid.HalfCellSizeInMeters, 0.01f, Grid.GetLayerZ(layer));

  public static Vector3 CellToPosCCF(int cell, Grid.SceneLayer layer) => Grid.CellToPos(cell, Grid.HalfCellSizeInMeters, Grid.HalfCellSizeInMeters, -Grid.CellSizeInMeters * (float) layer * Grid.LayerMultiplier);

  public static Vector3 CellToPosLCC(int cell, Grid.SceneLayer layer) => Grid.CellToPos(cell, 0.01f, Grid.HalfCellSizeInMeters, Grid.GetLayerZ(layer));

  public static Vector3 CellToPosRCC(int cell, Grid.SceneLayer layer) => Grid.CellToPos(cell, Grid.CellSizeInMeters - 0.01f, Grid.HalfCellSizeInMeters, Grid.GetLayerZ(layer));

  public static Vector3 CellToPosCTC(int cell, Grid.SceneLayer layer) => Grid.CellToPos(cell, Grid.HalfCellSizeInMeters, Grid.CellSizeInMeters - 0.01f, Grid.GetLayerZ(layer));

  public static bool IsSolidCell(int cell) => Grid.IsValidCell(cell) && Grid.Solid[cell];

  public static unsafe bool IsSubstantialLiquid(int cell, float threshold = 0.35f)
  {
    if (Grid.IsValidCell(cell))
    {
      ushort index = Grid.elementIdx[cell];
      if ((int) index < ElementLoader.elements.Count)
      {
        global::Element element = ElementLoader.elements[(int) index];
        if (element.IsLiquid && (double) Grid.mass[cell] >= (double) element.defaultValues.mass * (double) threshold)
          return true;
      }
    }
    return false;
  }

  public static bool IsVisiblyInLiquid(Vector2 pos)
  {
    int cell1 = Grid.PosToCell(pos);
    if (Grid.IsValidCell(cell1) && Grid.IsLiquid(cell1))
    {
      int cell2 = Grid.CellAbove(cell1);
      if (Grid.IsValidCell(cell2) && Grid.IsLiquid(cell2))
        return true;
      if ((double) Grid.Mass[cell1] / 1000.0 <= (double) ((float) (int) pos.y - pos.y))
        return true;
    }
    return false;
  }

  public static bool IsLiquid(int cell) => ElementLoader.elements[(int) Grid.ElementIdx[cell]].IsLiquid;

  public static bool IsGas(int cell) => ElementLoader.elements[(int) Grid.ElementIdx[cell]].IsGas;

  public static void GetVisibleExtents(out int min_x, out int min_y, out int max_x, out int max_y)
  {
    Vector3 worldPoint1 = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, TransformExtensions.GetPosition(((Component) Camera.main).transform).z));
    Vector3 worldPoint2 = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, TransformExtensions.GetPosition(((Component) Camera.main).transform).z));
    min_y = (int) worldPoint2.y;
    max_y = (int) ((double) worldPoint1.y + 0.5);
    min_x = (int) worldPoint2.x;
    max_x = (int) ((double) worldPoint1.x + 0.5);
  }

  public static void GetVisibleExtents(out Vector2I min, out Vector2I max) => Grid.GetVisibleExtents(out min.x, out min.y, out max.x, out max.y);

  public static bool IsVisible(int cell) => Grid.Visible[cell] > (byte) 0 || !PropertyTextures.IsFogOfWarEnabled;

  public static bool VisibleBlockingCB(int cell) => !Grid.Transparent[cell] && Grid.IsSolidCell(cell);

  public static bool VisibilityTest(int x, int y, int x2, int y2, bool blocking_tile_visible = false) => Grid.TestLineOfSight(x, y, x2, y2, Grid.VisibleBlockingDelegate, blocking_tile_visible);

  public static bool VisibilityTest(int cell, int target_cell, bool blocking_tile_visible = false)
  {
    int x1 = 0;
    int y1 = 0;
    Grid.CellToXY(cell, out x1, out y1);
    int x2 = 0;
    int y2 = 0;
    Grid.CellToXY(target_cell, out x2, out y2);
    return Grid.VisibilityTest(x1, y1, x2, y2, blocking_tile_visible);
  }

  public static bool PhysicalBlockingCB(int cell) => Grid.Solid[cell];

  public static bool IsPhysicallyAccessible(
    int x,
    int y,
    int x2,
    int y2,
    bool blocking_tile_visible = false)
  {
    return Grid.TestLineOfSight(x, y, x2, y2, Grid.PhysicalBlockingDelegate, blocking_tile_visible);
  }

  public static void CollectCellsInLine(int startCell, int endCell, HashSet<int> outputCells)
  {
    int num1 = 2;
    int cellDistance = Grid.GetCellDistance(startCell, endCell);
    Vector3 vector3 = Vector3.op_Subtraction(Grid.CellToPos(endCell), Grid.CellToPos(startCell));
    Vector2 vector2 = Vector2.op_Implicit(((Vector3) ref vector3).normalized);
    for (float num2 = 0.0f; (double) num2 < (double) cellDistance; num2 = Mathf.Min(num2 + 1f / (float) num1, (float) cellDistance))
    {
      int cell = Grid.PosToCell(Vector3.op_Addition(Grid.CellToPos(startCell), Vector2.op_Implicit(Vector2.op_Multiply(vector2, num2))));
      if (Grid.GetCellDistance(startCell, cell) <= cellDistance)
        outputCells.Add(cell);
    }
  }

  public static bool IsRangeExposedToSunlight(
    int cell,
    int scanRadius,
    CellOffset scanShape,
    out int cellsClear,
    int clearThreshold = 1)
  {
    cellsClear = 0;
    if (Grid.IsValidCell(cell) && (int) Grid.ExposedToSunlight[cell] >= clearThreshold)
      ++cellsClear;
    bool flag1 = true;
    bool flag2 = true;
    for (int index = 1; index <= scanRadius && flag1 | flag2; ++index)
    {
      int num1 = Grid.OffsetCell(cell, scanShape.x * index, scanShape.y * index);
      int num2 = Grid.OffsetCell(cell, -scanShape.x * index, scanShape.y * index);
      if (Grid.IsValidCell(num1) && (int) Grid.ExposedToSunlight[num1] >= clearThreshold)
        ++cellsClear;
      if (Grid.IsValidCell(num2) && (int) Grid.ExposedToSunlight[num2] >= clearThreshold)
        ++cellsClear;
    }
    return cellsClear > 0;
  }

  public static bool TestLineOfSight(
    int x,
    int y,
    int x2,
    int y2,
    Func<int, bool> blocking_cb,
    bool blocking_tile_visible = false)
  {
    int num1 = x;
    int num2 = y;
    int num3 = x2 - x;
    int num4 = y2 - y;
    int num5 = 0;
    int num6 = 0;
    int num7 = 0;
    int num8 = 0;
    if (num3 < 0)
      num5 = -1;
    else if (num3 > 0)
      num5 = 1;
    if (num4 < 0)
      num6 = -1;
    else if (num4 > 0)
      num6 = 1;
    if (num3 < 0)
      num7 = -1;
    else if (num3 > 0)
      num7 = 1;
    int num9 = Math.Abs(num3);
    int num10 = Math.Abs(num4);
    if (num9 <= num10)
    {
      num9 = Math.Abs(num4);
      num10 = Math.Abs(num3);
      if (num4 < 0)
        num8 = -1;
      else if (num4 > 0)
        num8 = 1;
      num7 = 0;
    }
    int num11 = num9 >> 1;
    for (int index = 0; index <= num9; ++index)
    {
      int cell = Grid.XYToCell(x, y);
      if (!Grid.IsValidCell(cell))
        return false;
      bool flag = blocking_cb(cell);
      if (((x != num1 ? 1 : (y != num2 ? 1 : 0)) & (flag ? 1 : 0)) != 0)
        return blocking_tile_visible && x == x2 && y == y2;
      num11 += num10;
      if (num11 >= num9)
      {
        num11 -= num9;
        x += num5;
        y += num6;
      }
      else
      {
        x += num7;
        y += num8;
      }
    }
    return true;
  }

  public static bool GetFreeGridSpace(Vector2I size, out Vector2I offset)
  {
    Vector2I gridOffset = BestFit.GetGridOffset(ClusterManager.Instance.WorldContainers, size, out offset);
    if (((Vector2I) ref gridOffset).X > Grid.WidthInCells || ((Vector2I) ref gridOffset).Y > Grid.HeightInCells)
      return false;
    SimMessages.SimDataResizeGridAndInitializeVacuumCells(gridOffset, size.x, size.y, offset.x, offset.y);
    Game.Instance.roomProber.Refresh();
    return true;
  }

  public static void FreeGridSpace(Vector2I size, Vector2I offset)
  {
    SimMessages.SimDataFreeCells(size.x, size.y, offset.x, offset.y);
    for (int y = offset.y; y < size.y + offset.y + 1; ++y)
    {
      for (int x = offset.x - 1; x < size.x + offset.x + 1; ++x)
      {
        int cell = Grid.XYToCell(x, y);
        if (Grid.IsValidCell(cell))
          Grid.Element[cell] = ElementLoader.FindElementByHash(SimHashes.Vacuum);
      }
    }
    Game.Instance.roomProber.Refresh();
  }

  [Conditional("UNITY_EDITOR")]
  public static void DrawBoxOnCell(int cell, Color color, float offset = 0.0f) => Vector3.op_Addition(Grid.CellToPos(cell), new Vector3(0.5f, 0.5f, 0.0f));

  [Flags]
  public enum BuildFlags : byte
  {
    Solid = 1,
    Foundation = 2,
    Door = 4,
    DupePassable = 8,
    DupeImpassable = 16, // 0x10
    CritterImpassable = 32, // 0x20
    FakeFloor = 192, // 0xC0
    Any = FakeFloor | CritterImpassable | DupeImpassable | DupePassable | Door | Foundation | Solid, // 0xFF
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct BuildFlagsFoundationIndexer
  {
    public bool this[int i]
    {
      get => (Grid.BuildMasks[i] & Grid.BuildFlags.Foundation) != 0;
      set => Grid.UpdateBuildMask(i, Grid.BuildFlags.Foundation, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct BuildFlagsSolidIndexer
  {
    public bool this[int i] => (Grid.BuildMasks[i] & Grid.BuildFlags.Solid) != 0;
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct BuildFlagsDupeImpassableIndexer
  {
    public bool this[int i]
    {
      get => (Grid.BuildMasks[i] & Grid.BuildFlags.DupeImpassable) != 0;
      set => Grid.UpdateBuildMask(i, Grid.BuildFlags.DupeImpassable, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct BuildFlagsFakeFloorIndexer
  {
    public bool this[int i] => (Grid.BuildMasks[i] & Grid.BuildFlags.FakeFloor) != 0;

    public void Add(int i)
    {
      Grid.BuildFlags buildMask = Grid.BuildMasks[i];
      int num = Math.Min(((int) (buildMask & Grid.BuildFlags.FakeFloor) >> 6) + 1, 3);
      Grid.BuildMasks[i] = buildMask & (Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable) | (Grid.BuildFlags) ((int) (byte) (num << 6) & 192);
    }

    public void Remove(int i)
    {
      Grid.BuildFlags buildMask = Grid.BuildMasks[i];
      int num = Math.Max(((int) (buildMask & Grid.BuildFlags.FakeFloor) >> 6) - 1, 0);
      Grid.BuildMasks[i] = buildMask & (Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation | Grid.BuildFlags.Door | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable) | (Grid.BuildFlags) ((int) (byte) (num << 6) & 192);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct BuildFlagsDupePassableIndexer
  {
    public bool this[int i]
    {
      get => (Grid.BuildMasks[i] & Grid.BuildFlags.DupePassable) != 0;
      set => Grid.UpdateBuildMask(i, Grid.BuildFlags.DupePassable, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct BuildFlagsImpassableIndexer
  {
    public bool this[int i]
    {
      get => (Grid.BuildMasks[i] & Grid.BuildFlags.CritterImpassable) != 0;
      set => Grid.UpdateBuildMask(i, Grid.BuildFlags.CritterImpassable, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct BuildFlagsDoorIndexer
  {
    public bool this[int i]
    {
      get => (Grid.BuildMasks[i] & Grid.BuildFlags.Door) != 0;
      set => Grid.UpdateBuildMask(i, Grid.BuildFlags.Door, value);
    }
  }

  [Flags]
  public enum VisFlags : byte
  {
    Revealed = 1,
    PreventFogOfWarReveal = 2,
    RenderedByWorld = 4,
    AllowPathfinding = 8,
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct VisFlagsRevealedIndexer
  {
    public bool this[int i]
    {
      get => (Grid.VisMasks[i] & Grid.VisFlags.Revealed) != 0;
      set => Grid.UpdateVisMask(i, Grid.VisFlags.Revealed, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct VisFlagsPreventFogOfWarRevealIndexer
  {
    public bool this[int i]
    {
      get => (Grid.VisMasks[i] & Grid.VisFlags.PreventFogOfWarReveal) != 0;
      set => Grid.UpdateVisMask(i, Grid.VisFlags.PreventFogOfWarReveal, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct VisFlagsRenderedByWorldIndexer
  {
    public bool this[int i]
    {
      get => (Grid.VisMasks[i] & Grid.VisFlags.RenderedByWorld) != 0;
      set => Grid.UpdateVisMask(i, Grid.VisFlags.RenderedByWorld, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct VisFlagsAllowPathfindingIndexer
  {
    public bool this[int i]
    {
      get => (Grid.VisMasks[i] & Grid.VisFlags.AllowPathfinding) != 0;
      set => Grid.UpdateVisMask(i, Grid.VisFlags.AllowPathfinding, value);
    }
  }

  [Flags]
  public enum NavValidatorFlags : byte
  {
    Ladder = 1,
    Pole = 2,
    Tube = 4,
    NavTeleporter = 8,
    UnderConstruction = 16, // 0x10
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavValidatorFlagsLadderIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.Ladder) != 0;
      set => Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.Ladder, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavValidatorFlagsPoleIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.Pole) != 0;
      set => Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.Pole, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavValidatorFlagsTubeIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.Tube) != 0;
      set => Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.Tube, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavValidatorFlagsNavTeleporterIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.NavTeleporter) != 0;
      set => Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.NavTeleporter, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavValidatorFlagsUnderConstructionIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavValidatorMasks[i] & Grid.NavValidatorFlags.UnderConstruction) != 0;
      set => Grid.UpdateNavValidatorMask(i, Grid.NavValidatorFlags.UnderConstruction, value);
    }
  }

  [Flags]
  public enum NavFlags : byte
  {
    AccessDoor = 1,
    TubeEntrance = 2,
    PreventIdleTraversal = 4,
    Reserved = 8,
    SuitMarker = 16, // 0x10
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavFlagsAccessDoorIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavMasks[i] & Grid.NavFlags.AccessDoor) != 0;
      set => Grid.UpdateNavMask(i, Grid.NavFlags.AccessDoor, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavFlagsTubeEntranceIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavMasks[i] & Grid.NavFlags.TubeEntrance) != 0;
      set => Grid.UpdateNavMask(i, Grid.NavFlags.TubeEntrance, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavFlagsPreventIdleTraversalIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavMasks[i] & Grid.NavFlags.PreventIdleTraversal) != 0;
      set => Grid.UpdateNavMask(i, Grid.NavFlags.PreventIdleTraversal, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavFlagsReservedIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavMasks[i] & Grid.NavFlags.Reserved) != 0;
      set => Grid.UpdateNavMask(i, Grid.NavFlags.Reserved, value);
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct NavFlagsSuitMarkerIndexer
  {
    public bool this[int i]
    {
      get => (Grid.NavMasks[i] & Grid.NavFlags.SuitMarker) != 0;
      set => Grid.UpdateNavMask(i, Grid.NavFlags.SuitMarker, value);
    }
  }

  public struct Restriction
  {
    public const int DefaultID = -1;
    public Dictionary<int, Grid.Restriction.Directions> DirectionMasksForMinionInstanceID;
    public Grid.Restriction.Orientation orientation;

    [Flags]
    public enum Directions : byte
    {
      Left = 1,
      Right = 2,
      Teleport = 4,
    }

    public enum Orientation : byte
    {
      Vertical,
      Horizontal,
      SingleCell,
    }
  }

  private struct TubeEntrance
  {
    public bool operational;
    public int reservationCapacity;
    public HashSet<int> reservedInstanceIDs;
  }

  public struct SuitMarker
  {
    public int suitCount;
    public int lockerCount;
    public Grid.SuitMarker.Flags flags;
    public PathFinder.PotentialPath.Flags pathFlags;
    public HashSet<int> minionIDsWithSuitReservations;
    public HashSet<int> minionIDsWithEmptyLockerReservations;

    public int emptyLockerCount => this.lockerCount - this.suitCount;

    [Flags]
    public enum Flags : byte
    {
      OnlyTraverseIfUnequipAvailable = 1,
      Operational = 2,
      Rotated = 4,
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct ObjectLayerIndexer
  {
    public GameObject this[int cell, int layer]
    {
      get
      {
        GameObject gameObject = (GameObject) null;
        Grid.ObjectLayers[layer].TryGetValue(cell, out gameObject);
        return gameObject;
      }
      set
      {
        if (Object.op_Equality((Object) value, (Object) null))
          Grid.ObjectLayers[layer].Remove(cell);
        else
          Grid.ObjectLayers[layer][cell] = value;
        GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.objectLayers[layer], (object) value);
      }
    }
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct PressureIndexer
  {
    public unsafe float this[int i] => Grid.mass[i] * 101.3f;
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct TransparentIndexer
  {
    public unsafe bool this[int i] => ((uint) Grid.properties[i] & 16U) > 0U;
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct ElementIdxIndexer
  {
    public unsafe ushort this[int i] => Grid.elementIdx[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct TemperatureIndexer
  {
    public unsafe float this[int i] => Grid.temperature[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct RadiationIndexer
  {
    public unsafe float this[int i] => Grid.radiation[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct MassIndexer
  {
    public unsafe float this[int i] => Grid.mass[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct PropertiesIndexer
  {
    public unsafe byte this[int i] => Grid.properties[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct ExposedToSunlightIndexer
  {
    public unsafe byte this[int i] => Grid.exposedToSunlight[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct StrengthInfoIndexer
  {
    public unsafe byte this[int i] => Grid.strengthInfo[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct Insulationndexer
  {
    public unsafe byte this[int i] => Grid.insulation[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct DiseaseIdxIndexer
  {
    public unsafe byte this[int i] => Grid.diseaseIdx[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct DiseaseCountIndexer
  {
    public unsafe int this[int i] => Grid.diseaseCount[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct AccumulatedFlowIndexer
  {
    public unsafe float this[int i] => Grid.AccumulatedFlowValues[i];
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct LightIntensityIndexer
  {
    public unsafe int this[int i]
    {
      get
      {
        float sunlightIntensity = Game.Instance.currentFallbackSunlightIntensity;
        WorldContainer world = ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[i]);
        if (Object.op_Inequality((Object) world, (Object) null))
          sunlightIntensity = world.currentSunlightIntensity;
        return (int) ((double) Grid.exposedToSunlight[i] / (double) byte.MaxValue * (double) sunlightIntensity) + Grid.LightCount[i];
      }
    }
  }

  public enum SceneLayer
  {
    WorldSelection = -3, // 0xFFFFFFFD
    NoLayer = -2, // 0xFFFFFFFE
    Background = -1, // 0xFFFFFFFF
    Backwall = 1,
    Gas = 2,
    GasConduits = 3,
    GasConduitBridges = 4,
    LiquidConduits = 5,
    LiquidConduitBridges = 6,
    SolidConduits = 7,
    SolidConduitContents = 8,
    SolidConduitBridges = 9,
    Wires = 10, // 0x0000000A
    WireBridges = 11, // 0x0000000B
    WireBridgesFront = 12, // 0x0000000C
    LogicWires = 13, // 0x0000000D
    LogicGates = 14, // 0x0000000E
    LogicGatesFront = 15, // 0x0000000F
    InteriorWall = 16, // 0x00000010
    GasFront = 17, // 0x00000011
    BuildingBack = 18, // 0x00000012
    Building = 19, // 0x00000013
    BuildingUse = 20, // 0x00000014
    BuildingFront = 21, // 0x00000015
    TransferArm = 22, // 0x00000016
    Ore = 23, // 0x00000017
    Creatures = 24, // 0x00000018
    Move = 25, // 0x00000019
    Front = 26, // 0x0000001A
    GlassTile = 27, // 0x0000001B
    Liquid = 28, // 0x0000001C
    Ground = 29, // 0x0000001D
    TileMain = 30, // 0x0000001E
    TileFront = 31, // 0x0000001F
    FXFront = 32, // 0x00000020
    FXFront2 = 33, // 0x00000021
    SceneMAX = 34, // 0x00000022
  }
}
