// Decompiled with JetBrains decompiler
// Type: ClusterGrid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClusterGrid
{
  public static ClusterGrid Instance;
  public const float NodeDistanceScale = 600f;
  private const float MAX_OFFSET_RADIUS = 0.5f;
  public int numRings;
  private ClusterFogOfWarManager.Instance m_fowManager;
  private Action<object> m_onClusterLocationChangedDelegate;
  public Dictionary<AxialI, List<ClusterGridEntity>> cellContents = new Dictionary<AxialI, List<ClusterGridEntity>>();

  public static void DestroyInstance() => ClusterGrid.Instance = (ClusterGrid) null;

  private ClusterFogOfWarManager.Instance GetFOWManager()
  {
    if (this.m_fowManager == null)
      this.m_fowManager = ((Component) SaveGame.Instance).GetSMI<ClusterFogOfWarManager.Instance>();
    return this.m_fowManager;
  }

  public bool IsValidCell(AxialI cell) => this.cellContents.ContainsKey(cell);

  public ClusterGrid(int numRings)
  {
    ClusterGrid.Instance = this;
    this.GenerateGrid(numRings);
    this.m_onClusterLocationChangedDelegate = new Action<object>(this.OnClusterLocationChanged);
  }

  public ClusterRevealLevel GetCellRevealLevel(AxialI cell) => this.GetFOWManager().GetCellRevealLevel(cell);

  public bool IsCellVisible(AxialI cell) => this.GetFOWManager().IsLocationRevealed(cell);

  public float GetRevealCompleteFraction(AxialI cell) => this.GetFOWManager().GetRevealCompleteFraction(cell);

  public bool IsVisible(ClusterGridEntity entity) => entity.IsVisible && this.IsCellVisible(entity.Location);

  public List<ClusterGridEntity> GetVisibleEntitiesAtCell(AxialI cell) => this.IsValidCell(cell) && this.GetFOWManager().IsLocationRevealed(cell) ? this.cellContents[cell].Where<ClusterGridEntity>((Func<ClusterGridEntity, bool>) (entity => entity.IsVisible)).ToList<ClusterGridEntity>() : new List<ClusterGridEntity>();

  public ClusterGridEntity GetVisibleEntityOfLayerAtCell(AxialI cell, EntityLayer entityLayer)
  {
    if (this.IsValidCell(cell) && this.GetFOWManager().IsLocationRevealed(cell))
    {
      foreach (ClusterGridEntity entityOfLayerAtCell in this.cellContents[cell])
      {
        if (entityOfLayerAtCell.IsVisible && entityOfLayerAtCell.Layer == entityLayer)
          return entityOfLayerAtCell;
      }
    }
    return (ClusterGridEntity) null;
  }

  public ClusterGridEntity GetVisibleEntityOfLayerAtAdjacentCell(
    AxialI cell,
    EntityLayer entityLayer)
  {
    return ((IEnumerable<AxialI>) AxialUtil.GetRing(cell, 1)).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetVisibleEntitiesAtCell)).FirstOrDefault<ClusterGridEntity>((Func<ClusterGridEntity, bool>) (entity => entity.Layer == entityLayer));
  }

  public List<ClusterGridEntity> GetHiddenEntitiesOfLayerAtCell(
    AxialI cell,
    EntityLayer entityLayer)
  {
    return ((IEnumerable<AxialI>) AxialUtil.GetRing(cell, 0)).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetHiddenEntitiesAtCell)).Where<ClusterGridEntity>((Func<ClusterGridEntity, bool>) (entity => entity.Layer == entityLayer)).ToList<ClusterGridEntity>();
  }

  public ClusterGridEntity GetEntityOfLayerAtCell(AxialI cell, EntityLayer entityLayer) => ((IEnumerable<AxialI>) AxialUtil.GetRing(cell, 0)).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetEntitiesOnCell)).FirstOrDefault<ClusterGridEntity>((Func<ClusterGridEntity, bool>) (entity => entity.Layer == entityLayer));

  public List<ClusterGridEntity> GetHiddenEntitiesAtCell(AxialI cell) => this.cellContents.ContainsKey(cell) && !this.GetFOWManager().IsLocationRevealed(cell) ? this.cellContents[cell].Where<ClusterGridEntity>((Func<ClusterGridEntity, bool>) (entity => entity.IsVisible)).ToList<ClusterGridEntity>() : new List<ClusterGridEntity>();

  public List<ClusterGridEntity> GetNotVisibleEntitiesAtAdjacentCell(AxialI cell) => ((IEnumerable<AxialI>) AxialUtil.GetRing(cell, 1)).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetHiddenEntitiesAtCell)).ToList<ClusterGridEntity>();

  public List<ClusterGridEntity> GetNotVisibleEntitiesOfLayerAtAdjacentCell(
    AxialI cell,
    EntityLayer entityLayer)
  {
    return ((IEnumerable<AxialI>) AxialUtil.GetRing(cell, 1)).SelectMany<AxialI, ClusterGridEntity>(new Func<AxialI, IEnumerable<ClusterGridEntity>>(this.GetHiddenEntitiesAtCell)).Where<ClusterGridEntity>((Func<ClusterGridEntity, bool>) (entity => entity.Layer == entityLayer)).ToList<ClusterGridEntity>();
  }

  public ClusterGridEntity GetAsteroidAtCell(AxialI cell) => !this.cellContents.ContainsKey(cell) ? (ClusterGridEntity) null : this.cellContents[cell].Where<ClusterGridEntity>((Func<ClusterGridEntity, bool>) (e => e.Layer == EntityLayer.Asteroid)).FirstOrDefault<ClusterGridEntity>();

  public bool HasVisibleAsteroidAtCell(AxialI cell) => Object.op_Inequality((Object) this.GetVisibleEntityOfLayerAtCell(cell, EntityLayer.Asteroid), (Object) null);

  public void RegisterEntity(ClusterGridEntity entity)
  {
    this.cellContents[entity.Location].Add(entity);
    entity.Subscribe(-1298331547, this.m_onClusterLocationChangedDelegate);
  }

  public void UnregisterEntity(ClusterGridEntity entity)
  {
    this.cellContents[entity.Location].Remove(entity);
    entity.Unsubscribe(-1298331547, this.m_onClusterLocationChangedDelegate);
  }

  public void OnClusterLocationChanged(object data)
  {
    ClusterLocationChangedEvent locationChangedEvent = (ClusterLocationChangedEvent) data;
    Debug.Assert(this.IsValidCell(locationChangedEvent.oldLocation), (object) string.Format("ChangeEntityCell move order FROM invalid location: {0} {1}", (object) locationChangedEvent.oldLocation, (object) locationChangedEvent.entity));
    Debug.Assert(this.IsValidCell(locationChangedEvent.newLocation), (object) string.Format("ChangeEntityCell move order TO invalid location: {0} {1}", (object) locationChangedEvent.newLocation, (object) locationChangedEvent.entity));
    this.cellContents[locationChangedEvent.oldLocation].Remove(locationChangedEvent.entity);
    this.cellContents[locationChangedEvent.newLocation].Add(locationChangedEvent.entity);
  }

  private AxialI GetNeighbor(AxialI cell, AxialI direction) => AxialI.op_Addition(cell, direction);

  public int GetCellRing(AxialI cell)
  {
    Vector3I cube = ((AxialI) ref cell).ToCube();
    Vector3I vector3I1;
    // ISSUE: explicit constructor call
    ((Vector3I) ref vector3I1).\u002Ector(cube.x, cube.y, cube.z);
    Vector3I vector3I2;
    // ISSUE: explicit constructor call
    ((Vector3I) ref vector3I2).\u002Ector(0, 0, 0);
    return (int) (float) ((Mathf.Abs(vector3I1.x - vector3I2.x) + Mathf.Abs(vector3I1.y - vector3I2.y) + Mathf.Abs(vector3I1.z - vector3I2.z)) / 2);
  }

  private void CleanUpGrid() => this.cellContents.Clear();

  private int GetHexDistance(AxialI a, AxialI b)
  {
    Vector3I cube1 = ((AxialI) ref a).ToCube();
    Vector3I cube2 = ((AxialI) ref b).ToCube();
    return Mathf.Max(new int[3]
    {
      Mathf.Abs(cube1.x - cube2.x),
      Mathf.Abs(cube1.y - cube2.y),
      Mathf.Abs(cube1.z - cube2.z)
    });
  }

  public List<ClusterGridEntity> GetEntitiesInRange(AxialI center, int range = 1)
  {
    List<ClusterGridEntity> entitiesInRange = new List<ClusterGridEntity>();
    foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in this.cellContents)
    {
      if (this.GetHexDistance(cellContent.Key, center) <= range)
        entitiesInRange.AddRange((IEnumerable<ClusterGridEntity>) cellContent.Value);
    }
    return entitiesInRange;
  }

  public List<ClusterGridEntity> GetEntitiesOnCell(AxialI cell) => this.cellContents[cell];

  public bool IsInRange(AxialI a, AxialI b, int range = 1) => this.GetHexDistance(a, b) <= range;

  private void GenerateGrid(int rings)
  {
    this.CleanUpGrid();
    this.numRings = rings;
    for (int index1 = -rings + 1; index1 < rings; ++index1)
    {
      for (int index2 = -rings + 1; index2 < rings; ++index2)
      {
        for (int index3 = -rings + 1; index3 < rings; ++index3)
        {
          if (index1 + index2 + index3 == 0)
          {
            AxialI key;
            // ISSUE: explicit constructor call
            ((AxialI) ref key).\u002Ector(index1, index2);
            this.cellContents.Add(key, new List<ClusterGridEntity>());
          }
        }
      }
    }
  }

  public Vector3 GetPosition(ClusterGridEntity entity)
  {
    AxialI location1 = entity.Location;
    float r = (float) ((AxialI) ref location1).R;
    AxialI location2 = entity.Location;
    float q = (float) ((AxialI) ref location2).Q;
    List<ClusterGridEntity> cellContent = this.cellContents[entity.Location];
    if (cellContent.Count <= 1 || !entity.SpaceOutInSameHex())
      return AxialUtil.AxialToWorld(r, q);
    int num1 = 0;
    int num2 = 0;
    foreach (ClusterGridEntity clusterGridEntity in cellContent)
    {
      if (Object.op_Equality((Object) entity, (Object) clusterGridEntity))
        num1 = num2;
      if (clusterGridEntity.SpaceOutInSameHex())
        ++num2;
    }
    if (cellContent.Count > num2)
    {
      num2 += 5;
      num1 += 5;
    }
    else if (num2 > 0)
    {
      ++num2;
      ++num1;
    }
    if (num2 == 0 || num2 == 1)
      return AxialUtil.AxialToWorld(r, q);
    float num3 = Mathf.Min(Mathf.Pow((float) num2, 0.5f), 1f) * 0.5f;
    float num4 = Mathf.Pow((float) num1 / (float) num2, 0.5f);
    float num5 = 0.81f;
    double num6 = 6.2831854820251465 * (double) (Mathf.Pow((float) num2, 0.5f) * num5) * (double) num4;
    float num7 = Mathf.Cos((float) num6) * num3 * num4;
    float num8 = Mathf.Sin((float) num6) * num3 * num4;
    return Vector3.op_Addition(AxialUtil.AxialToWorld(r, q), new Vector3(num7, num8, 0.0f));
  }

  public List<AxialI> GetPath(
    AxialI start,
    AxialI end,
    ClusterDestinationSelector destination_selector)
  {
    return this.GetPath(start, end, destination_selector, out string _);
  }

  public List<AxialI> GetPath(
    AxialI start,
    AxialI end,
    ClusterDestinationSelector destination_selector,
    out string fail_reason)
  {
    fail_reason = (string) null;
    if (!destination_selector.canNavigateFogOfWar && !this.IsCellVisible(end))
    {
      fail_reason = (string) UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_FOG_OF_WAR;
      return (List<AxialI>) null;
    }
    ClusterGridEntity entityOfLayerAtCell = this.GetVisibleEntityOfLayerAtCell(end, EntityLayer.Asteroid);
    if (Object.op_Inequality((Object) entityOfLayerAtCell, (Object) null) && destination_selector.requireLaunchPadOnAsteroidDestination)
    {
      bool flag = false;
      foreach (KMonoBehaviour launchPad in Components.LaunchPads)
      {
        if (AxialI.op_Equality(launchPad.GetMyWorldLocation(), entityOfLayerAtCell.Location))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
      {
        fail_reason = (string) UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_NO_LAUNCH_PAD;
        return (List<AxialI>) null;
      }
    }
    if (Object.op_Equality((Object) entityOfLayerAtCell, (Object) null) && destination_selector.requireAsteroidDestination)
    {
      fail_reason = (string) UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_REQUIRE_ASTEROID;
      return (List<AxialI>) null;
    }
    HashSet<AxialI> frontier = new HashSet<AxialI>();
    HashSet<AxialI> visited = new HashSet<AxialI>();
    HashSet<AxialI> buffer = new HashSet<AxialI>();
    Dictionary<AxialI, AxialI> cameFrom = new Dictionary<AxialI, AxialI>();
    frontier.Add(start);
    while (!frontier.Contains(end) && frontier.Count > 0)
      ExpandFrontier();
    if (frontier.Contains(end))
    {
      List<AxialI> path = new List<AxialI>();
      for (AxialI key = end; AxialI.op_Inequality(key, start); key = cameFrom[key])
        path.Add(key);
      path.Reverse();
      return path;
    }
    fail_reason = (string) UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_NO_PATH;
    return (List<AxialI>) null;

    void ExpandFrontier()
    {
      buffer.Clear();
      foreach (AxialI cell in frontier)
      {
        foreach (AxialI direction in AxialI.DIRECTIONS)
        {
          AxialI neighbor = this.GetNeighbor(cell, direction);
          if (!visited.Contains(neighbor) && this.IsValidCell(neighbor) && (this.IsCellVisible(neighbor) || destination_selector.canNavigateFogOfWar) && (!this.HasVisibleAsteroidAtCell(neighbor) || !AxialI.op_Inequality(neighbor, start) || !AxialI.op_Inequality(neighbor, end)))
          {
            buffer.Add(neighbor);
            if (!cameFrom.ContainsKey(neighbor))
              cameFrom.Add(neighbor, cell);
          }
        }
        visited.Add(cell);
      }
      HashSet<AxialI> frontier = frontier;
      frontier = buffer;
      buffer = frontier;
    }
  }

  public void GetLocationDescription(
    AxialI location,
    out Sprite sprite,
    out string label,
    out string sublabel)
  {
    ClusterGridEntity clusterGridEntity = this.GetVisibleEntitiesAtCell(location).Find((Predicate<ClusterGridEntity>) (x => x.Layer == EntityLayer.Asteroid));
    ClusterGridEntity layerAtAdjacentCell = this.GetVisibleEntityOfLayerAtAdjacentCell(location, EntityLayer.Asteroid);
    if (Object.op_Inequality((Object) clusterGridEntity, (Object) null))
    {
      sprite = clusterGridEntity.GetUISprite();
      label = clusterGridEntity.Name;
      WorldContainer component = ((Component) clusterGridEntity).GetComponent<WorldContainer>();
      sublabel = StringEntry.op_Implicit(Strings.Get(component.worldType));
    }
    else if (Object.op_Inequality((Object) layerAtAdjacentCell, (Object) null))
    {
      sprite = layerAtAdjacentCell.GetUISprite();
      label = UI.SPACEDESTINATIONS.ORBIT.NAME_FMT.Replace("{Name}", layerAtAdjacentCell.Name);
      WorldContainer component = ((Component) layerAtAdjacentCell).GetComponent<WorldContainer>();
      sublabel = StringEntry.op_Implicit(Strings.Get(component.worldType));
    }
    else if (this.IsCellVisible(location))
    {
      sprite = Assets.GetSprite(HashedString.op_Implicit("hex_unknown"));
      label = (string) UI.SPACEDESTINATIONS.EMPTY_SPACE.NAME;
      sublabel = "";
    }
    else
    {
      sprite = Assets.GetSprite(HashedString.op_Implicit("unknown_far"));
      label = (string) UI.SPACEDESTINATIONS.FOG_OF_WAR_SPACE.NAME;
      sublabel = "";
    }
  }
}
