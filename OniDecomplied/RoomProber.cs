// Decompiled with JetBrains decompiler
// Type: RoomProber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomProber : ISim1000ms
{
  public List<Room> rooms = new List<Room>();
  private KCompactedVector<CavityInfo> cavityInfos = new KCompactedVector<CavityInfo>(1024);
  private HandleVector<int>.Handle[] CellCavityID;
  private bool dirty = true;
  private HashSet<int> solidChanges = new HashSet<int>();
  private HashSet<int> visitedCells = new HashSet<int>();
  private HashSet<int> floodFillSet = new HashSet<int>();
  private HashSet<HandleVector<int>.Handle> releasedIDs = new HashSet<HandleVector<int>.Handle>();
  private RoomProber.CavityFloodFiller floodFiller;
  private List<KPrefabID> releasedCritters = new List<KPrefabID>();

  public RoomProber()
  {
    this.CellCavityID = new HandleVector<int>.Handle[Grid.CellCount];
    this.floodFiller = new RoomProber.CavityFloodFiller(this.CellCavityID);
    for (int index = 0; index < this.CellCavityID.Length; ++index)
      this.solidChanges.Add(index);
    this.ProcessSolidChanges();
    this.RefreshRooms();
    World.Instance.OnSolidChanged += new Action<int>(this.SolidChangedEvent);
    GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[1], new Action<int, object>(this.OnBuildingsChanged));
    GameScenePartitioner.Instance.AddGlobalLayerListener(GameScenePartitioner.Instance.objectLayers[2], new Action<int, object>(this.OnBuildingsChanged));
  }

  public void Refresh()
  {
    this.ProcessSolidChanges();
    this.RefreshRooms();
  }

  private void SolidChangedEvent(int cell) => this.SolidChangedEvent(cell, true);

  private void OnBuildingsChanged(int cell, object building)
  {
    if (this.GetCavityForCell(cell) == null)
      return;
    this.solidChanges.Add(cell);
    this.dirty = true;
  }

  public void SolidChangedEvent(int cell, bool ignoreDoors)
  {
    if (ignoreDoors && Grid.HasDoor[cell])
      return;
    this.solidChanges.Add(cell);
    this.dirty = true;
  }

  private CavityInfo CreateNewCavity()
  {
    CavityInfo newCavity = new CavityInfo();
    newCavity.handle = this.cavityInfos.Allocate(newCavity);
    return newCavity;
  }

  private unsafe void ProcessSolidChanges()
  {
    int* numPtr = stackalloc int[5];
    numPtr[0] = 0;
    numPtr[1] = -Grid.WidthInCells;
    numPtr[2] = -1;
    numPtr[3] = 1;
    numPtr[4] = Grid.WidthInCells;
    foreach (int solidChange in this.solidChanges)
    {
      for (int index = 0; index < 5; ++index)
      {
        int cell = solidChange + numPtr[index];
        if (Grid.IsValidCell(cell))
        {
          this.floodFillSet.Add(cell);
          HandleVector<int>.Handle handle = this.CellCavityID[cell];
          if (handle.IsValid())
          {
            this.CellCavityID[cell] = HandleVector<int>.InvalidHandle;
            this.releasedIDs.Add(handle);
          }
        }
      }
    }
    CavityInfo newCavity = this.CreateNewCavity();
    foreach (int floodFill in this.floodFillSet)
    {
      if (!this.visitedCells.Contains(floodFill) && !this.CellCavityID[floodFill].IsValid())
      {
        CavityInfo cavityInfo = newCavity;
        this.floodFiller.Reset(cavityInfo.handle);
        GameUtil.FloodFillConditional(floodFill, new Func<int, bool>(this.floodFiller.ShouldContinue), (ICollection<int>) this.visitedCells);
        if (this.floodFiller.NumCells > 0)
        {
          cavityInfo.numCells = this.floodFiller.NumCells;
          cavityInfo.minX = this.floodFiller.MinX;
          cavityInfo.minY = this.floodFiller.MinY;
          cavityInfo.maxX = this.floodFiller.MaxX;
          cavityInfo.maxY = this.floodFiller.MaxY;
          newCavity = this.CreateNewCavity();
        }
      }
    }
    if (newCavity.numCells == 0)
      this.releasedIDs.Add(newCavity.handle);
    foreach (HandleVector<int>.Handle releasedId in this.releasedIDs)
    {
      CavityInfo data = this.cavityInfos.GetData(releasedId);
      this.releasedCritters.AddRange((IEnumerable<KPrefabID>) data.creatures);
      if (data.room != null)
        this.ClearRoom(data.room);
      this.cavityInfos.Free(releasedId);
    }
    this.RebuildDirtyCavities((ICollection<int>) this.visitedCells);
    this.releasedIDs.Clear();
    this.visitedCells.Clear();
    this.solidChanges.Clear();
    this.floodFillSet.Clear();
  }

  private void RebuildDirtyCavities(ICollection<int> visited_cells)
  {
    int maxRoomSize = TuningData<RoomProber.Tuning>.Get().maxRoomSize;
    foreach (int visitedCell in (IEnumerable<int>) visited_cells)
    {
      HandleVector<int>.Handle handle = this.CellCavityID[visitedCell];
      if (handle.IsValid())
      {
        CavityInfo data = this.cavityInfos.GetData(handle);
        if (0 < data.numCells && data.numCells <= maxRoomSize)
        {
          GameObject gameObject = Grid.Objects[visitedCell, 1];
          if (Object.op_Inequality((Object) gameObject, (Object) null))
          {
            KPrefabID component = gameObject.GetComponent<KPrefabID>();
            bool flag = false;
            foreach (KPrefabID building in data.buildings)
            {
              if (component.InstanceID == building.InstanceID)
              {
                flag = true;
                break;
              }
            }
            foreach (KPrefabID plant in data.plants)
            {
              if (component.InstanceID == plant.InstanceID)
              {
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              if (Object.op_Implicit((Object) ((Component) component).GetComponent<Deconstructable>()))
                data.AddBuilding(component);
              else if (component.HasTag(GameTags.Plant) && !component.IsPrefabID(TagExtensions.ToTag("ForestTreeBranch")))
                data.AddPlants(component);
            }
          }
        }
      }
    }
    visited_cells.Clear();
  }

  public void Sim1000ms(float dt)
  {
    if (!this.dirty)
      return;
    this.ProcessSolidChanges();
    this.RefreshRooms();
  }

  private void CreateRoom(CavityInfo cavity)
  {
    Debug.Assert(cavity.room == null);
    Room room = new Room();
    room.cavity = cavity;
    cavity.room = room;
    this.rooms.Add(room);
    room.roomType = Db.Get().RoomTypes.GetRoomType(room);
    this.AssignBuildingsToRoom(room);
  }

  private void ClearRoom(Room room)
  {
    this.UnassignBuildingsToRoom(room);
    room.CleanUp();
    this.rooms.Remove(room);
  }

  private void RefreshRooms()
  {
    int maxRoomSize = TuningData<RoomProber.Tuning>.Get().maxRoomSize;
    foreach (CavityInfo data in this.cavityInfos.GetDataList())
    {
      if (data.dirty)
      {
        Debug.Assert(data.room == null, (object) "I expected info.room to always be null by this point");
        if (data.numCells > 0)
        {
          if (data.numCells <= maxRoomSize)
            this.CreateRoom(data);
          foreach (KMonoBehaviour building in data.buildings)
            building.Trigger(144050788, (object) data.room);
          foreach (KMonoBehaviour plant in data.plants)
            plant.Trigger(144050788, (object) data.room);
        }
        data.dirty = false;
      }
    }
    foreach (KPrefabID releasedCritter in this.releasedCritters)
    {
      if (Object.op_Inequality((Object) releasedCritter, (Object) null))
        ((Component) releasedCritter).GetSMI<OvercrowdingMonitor.Instance>()?.RoomRefreshUpdateCavity();
    }
    this.releasedCritters.Clear();
    this.dirty = false;
  }

  private void AssignBuildingsToRoom(Room room)
  {
    Debug.Assert(room != null);
    RoomType roomType = room.roomType;
    if (roomType == Db.Get().RoomTypes.Neutral)
      return;
    foreach (KPrefabID building in room.buildings)
    {
      if (!Object.op_Equality((Object) building, (Object) null) && !building.HasTag(GameTags.NotRoomAssignable))
      {
        Assignable component = ((Component) building).GetComponent<Assignable>();
        if (Object.op_Inequality((Object) component, (Object) null) && (roomType.primary_constraint == null || !roomType.primary_constraint.building_criteria(((Component) building).GetComponent<KPrefabID>())))
          component.Assign((IAssignableIdentity) room);
      }
    }
  }

  private void UnassignKPrefabIDs(Room room, List<KPrefabID> list)
  {
    foreach (KPrefabID kprefabId in list)
    {
      if (!Object.op_Equality((Object) kprefabId, (Object) null))
      {
        ((KMonoBehaviour) kprefabId).Trigger(144050788, (object) null);
        Assignable component = ((Component) kprefabId).GetComponent<Assignable>();
        if (Object.op_Inequality((Object) component, (Object) null) && component.assignee == room)
          component.Unassign();
      }
    }
  }

  private void UnassignBuildingsToRoom(Room room)
  {
    Debug.Assert(room != null);
    this.UnassignKPrefabIDs(room, room.buildings);
    this.UnassignKPrefabIDs(room, room.plants);
  }

  public void UpdateRoom(CavityInfo cavity)
  {
    if (cavity == null)
      return;
    if (cavity.room != null)
    {
      this.ClearRoom(cavity.room);
      cavity.room = (Room) null;
    }
    this.CreateRoom(cavity);
    foreach (KPrefabID building in cavity.buildings)
    {
      if (Object.op_Inequality((Object) building, (Object) null))
        ((KMonoBehaviour) building).Trigger(144050788, (object) cavity.room);
    }
    foreach (KPrefabID plant in cavity.plants)
    {
      if (Object.op_Inequality((Object) plant, (Object) null))
        ((KMonoBehaviour) plant).Trigger(144050788, (object) cavity.room);
    }
  }

  public Room GetRoomOfGameObject(GameObject go)
  {
    if (Object.op_Equality((Object) go, (Object) null))
      return (Room) null;
    int cell = Grid.PosToCell(go);
    if (!Grid.IsValidCell(cell))
      return (Room) null;
    return this.GetCavityForCell(cell)?.room;
  }

  public bool IsInRoomType(GameObject go, RoomType checkType)
  {
    Room roomOfGameObject = this.GetRoomOfGameObject(go);
    if (roomOfGameObject == null)
      return false;
    RoomType roomType = roomOfGameObject.roomType;
    return checkType == roomType;
  }

  private CavityInfo GetCavityInfo(HandleVector<int>.Handle id)
  {
    CavityInfo cavityInfo = (CavityInfo) null;
    if (id.IsValid())
      cavityInfo = this.cavityInfos.GetData(id);
    return cavityInfo;
  }

  public CavityInfo GetCavityForCell(int cell) => !Grid.IsValidCell(cell) ? (CavityInfo) null : this.GetCavityInfo(this.CellCavityID[cell]);

  public class Tuning : TuningData<RoomProber.Tuning>
  {
    public int maxRoomSize;
  }

  private class CavityFloodFiller
  {
    private HandleVector<int>.Handle[] grid;
    private HandleVector<int>.Handle cavityID;
    private int numCells;
    private int minX;
    private int minY;
    private int maxX;
    private int maxY;

    public CavityFloodFiller(HandleVector<int>.Handle[] grid) => this.grid = grid;

    public void Reset(HandleVector<int>.Handle search_id)
    {
      this.cavityID = search_id;
      this.numCells = 0;
      this.minX = int.MaxValue;
      this.minY = int.MaxValue;
      this.maxX = 0;
      this.maxY = 0;
    }

    private static bool IsWall(int cell) => (Grid.BuildMasks[cell] & (Grid.BuildFlags.Solid | Grid.BuildFlags.Foundation)) != 0 || Grid.HasDoor[cell];

    public bool ShouldContinue(int flood_cell)
    {
      if (RoomProber.CavityFloodFiller.IsWall(flood_cell))
      {
        this.grid[flood_cell] = HandleVector<int>.InvalidHandle;
        return false;
      }
      this.grid[flood_cell] = this.cavityID;
      int x;
      int y;
      Grid.CellToXY(flood_cell, out x, out y);
      this.minX = Math.Min(x, this.minX);
      this.minY = Math.Min(y, this.minY);
      this.maxX = Math.Max(x, this.maxX);
      this.maxY = Math.Max(y, this.maxY);
      ++this.numCells;
      return true;
    }

    public int NumCells => this.numCells;

    public int MinX => this.minX;

    public int MinY => this.minY;

    public int MaxX => this.maxX;

    public int MaxY => this.maxY;
  }
}
