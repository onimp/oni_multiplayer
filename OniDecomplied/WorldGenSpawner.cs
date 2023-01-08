// Decompiled with JetBrains decompiler
// Type: WorldGenSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using TemplateClasses;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/WorldGenSpawner")]
public class WorldGenSpawner : KMonoBehaviour
{
  [Serialize]
  private Prefab[] spawnInfos;
  [Serialize]
  private bool hasPlacedTemplates;
  private List<WorldGenSpawner.Spawnable> spawnables = new List<WorldGenSpawner.Spawnable>();

  public bool SpawnsRemain() => this.spawnables.Count > 0;

  public void SpawnEverything()
  {
    for (int index = 0; index < this.spawnables.Count; ++index)
      this.spawnables[index].TrySpawn();
  }

  public void SpawnTag(string id)
  {
    for (int index = 0; index < this.spawnables.Count; ++index)
    {
      if (this.spawnables[index].spawnInfo.id == id)
        this.spawnables[index].TrySpawn();
    }
  }

  public void ClearSpawnersInArea(Vector2 root_position, CellOffset[] area)
  {
    for (int index = 0; index < this.spawnables.Count; ++index)
    {
      if (Grid.IsCellOffsetOf(Grid.PosToCell(root_position), this.spawnables[index].cell, area))
        this.spawnables[index].FreeResources();
    }
  }

  public IReadOnlyList<WorldGenSpawner.Spawnable> GetSpawnables() => (IReadOnlyList<WorldGenSpawner.Spawnable>) this.spawnables;

  protected virtual void OnSpawn()
  {
    if (!this.hasPlacedTemplates)
    {
      Debug.Assert(SaveLoader.Instance.ClusterLayout != null, (object) "Trying to place templates for an already-loaded save, no worldgen data available");
      this.DoReveal(SaveLoader.Instance.ClusterLayout);
      this.PlaceTemplates(SaveLoader.Instance.ClusterLayout);
      this.hasPlacedTemplates = true;
    }
    if (this.spawnInfos == null)
      return;
    for (int index = 0; index < this.spawnInfos.Length; ++index)
      this.AddSpawnable(this.spawnInfos[index]);
  }

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing()
  {
    List<Prefab> prefabList = new List<Prefab>();
    for (int index = 0; index < this.spawnables.Count; ++index)
    {
      WorldGenSpawner.Spawnable spawnable = this.spawnables[index];
      if (!spawnable.isSpawned)
        prefabList.Add(spawnable.spawnInfo);
    }
    this.spawnInfos = prefabList.ToArray();
  }

  private void AddSpawnable(Prefab prefab) => this.spawnables.Add(new WorldGenSpawner.Spawnable(prefab));

  public void AddLegacySpawner(Tag tag, int cell)
  {
    Vector2I xy = Grid.CellToXY(cell);
    this.AddSpawnable(new Prefab(((Tag) ref tag).Name, Prefab.Type.Other, xy.x, xy.y, SimHashes.Carbon));
  }

  public List<Tag> GetUnspawnedWithType<T>(int worldID) where T : KMonoBehaviour
  {
    List<Tag> unspawnedWithType = new List<Tag>();
    foreach (WorldGenSpawner.Spawnable spawnable in this.spawnables.FindAll((Predicate<WorldGenSpawner.Spawnable>) (match => !match.isSpawned && (int) Grid.WorldIdx[match.cell] == worldID && Object.op_Inequality((Object) Assets.GetPrefab(Tag.op_Implicit(match.spawnInfo.id)), (Object) null) && Object.op_Inequality((Object) (object) Assets.GetPrefab(Tag.op_Implicit(match.spawnInfo.id)).GetComponent<T>(), (Object) null))))
      unspawnedWithType.Add(Tag.op_Implicit(spawnable.spawnInfo.id));
    return unspawnedWithType;
  }

  public List<Tag> GetSpawnersWithTag(Tag tag, int worldID, bool includeSpawned = false)
  {
    List<Tag> spawnersWithTag = new List<Tag>();
    foreach (WorldGenSpawner.Spawnable spawnable in this.spawnables.FindAll((Predicate<WorldGenSpawner.Spawnable>) (match => (includeSpawned || !match.isSpawned) && (int) Grid.WorldIdx[match.cell] == worldID && Tag.op_Equality(Tag.op_Implicit(match.spawnInfo.id), tag))))
      spawnersWithTag.Add(Tag.op_Implicit(spawnable.spawnInfo.id));
    return spawnersWithTag;
  }

  private void PlaceTemplates(Cluster clusterLayout)
  {
    this.spawnables = new List<WorldGenSpawner.Spawnable>();
    foreach (WorldGen world in clusterLayout.worlds)
    {
      foreach (Prefab building in world.SpawnData.buildings)
      {
        building.location_x += world.data.world.offset.x;
        building.location_y += world.data.world.offset.y;
        building.type = Prefab.Type.Building;
        this.AddSpawnable(building);
      }
      foreach (Prefab elementalOre in world.SpawnData.elementalOres)
      {
        elementalOre.location_x += world.data.world.offset.x;
        elementalOre.location_y += world.data.world.offset.y;
        elementalOre.type = Prefab.Type.Ore;
        this.AddSpawnable(elementalOre);
      }
      foreach (Prefab otherEntity in world.SpawnData.otherEntities)
      {
        otherEntity.location_x += world.data.world.offset.x;
        otherEntity.location_y += world.data.world.offset.y;
        otherEntity.type = Prefab.Type.Other;
        this.AddSpawnable(otherEntity);
      }
      foreach (Prefab pickupable in world.SpawnData.pickupables)
      {
        pickupable.location_x += world.data.world.offset.x;
        pickupable.location_y += world.data.world.offset.y;
        pickupable.type = Prefab.Type.Pickupable;
        this.AddSpawnable(pickupable);
      }
      world.SpawnData.buildings.Clear();
      world.SpawnData.elementalOres.Clear();
      world.SpawnData.otherEntities.Clear();
      world.SpawnData.pickupables.Clear();
    }
  }

  private void DoReveal(Cluster clusterLayout)
  {
    foreach (WorldGen world in clusterLayout.worlds)
      Game.Instance.Reset(world.SpawnData, world.WorldOffset);
    for (int i = 0; i < Grid.CellCount; ++i)
    {
      Grid.Revealed[i] = false;
      Grid.Spawnable[i] = (byte) 0;
    }
    float innerRadius = 16.5f;
    int radius = 18;
    Vector2I vector2I = Vector2I.op_Addition(clusterLayout.currentWorld.SpawnData.baseStartPos, clusterLayout.currentWorld.WorldOffset);
    GridVisibility.Reveal(vector2I.x, vector2I.y, radius, innerRadius);
  }

  public class Spawnable
  {
    private HandleVector<int>.Handle fogOfWarPartitionerEntry;
    private HandleVector<int>.Handle solidChangedPartitionerEntry;

    public Prefab spawnInfo { get; private set; }

    public bool isSpawned { get; private set; }

    public int cell { get; private set; }

    public Spawnable(Prefab spawn_info)
    {
      this.spawnInfo = spawn_info;
      int num = Grid.XYToCell(this.spawnInfo.location_x, this.spawnInfo.location_y);
      GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(spawn_info.id));
      if (Object.op_Inequality((Object) prefab, (Object) null))
      {
        WorldSpawnableMonitor.Def def = prefab.GetDef<WorldSpawnableMonitor.Def>();
        if (def != null && def.adjustSpawnLocationCb != null)
          num = def.adjustSpawnLocationCb(num);
      }
      this.cell = num;
      Debug.Assert(Grid.IsValidCell(this.cell));
      if (Grid.Spawnable[this.cell] > (byte) 0)
        this.TrySpawn();
      else
        this.fogOfWarPartitionerEntry = GameScenePartitioner.Instance.Add("WorldGenSpawner.OnReveal", (object) this, this.cell, GameScenePartitioner.Instance.fogOfWarChangedLayer, new Action<object>(this.OnReveal));
    }

    private void OnReveal(object data)
    {
      if (Grid.Spawnable[this.cell] <= (byte) 0)
        return;
      this.TrySpawn();
    }

    private void OnSolidChanged(object data)
    {
      if (Grid.Solid[this.cell])
        return;
      GameScenePartitioner.Instance.Free(ref this.solidChangedPartitionerEntry);
      ((Component) Game.Instance).GetComponent<EntombedItemVisualizer>().RemoveItem(this.cell);
      this.Spawn();
    }

    public void FreeResources()
    {
      if (this.solidChangedPartitionerEntry.IsValid())
      {
        GameScenePartitioner.Instance.Free(ref this.solidChangedPartitionerEntry);
        if (Object.op_Inequality((Object) Game.Instance, (Object) null))
          ((Component) Game.Instance).GetComponent<EntombedItemVisualizer>().RemoveItem(this.cell);
      }
      GameScenePartitioner.Instance.Free(ref this.fogOfWarPartitionerEntry);
      this.isSpawned = true;
    }

    public void TrySpawn()
    {
      if (this.isSpawned || this.solidChangedPartitionerEntry.IsValid())
        return;
      WorldContainer world = ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[this.cell]);
      bool flag1 = Object.op_Inequality((Object) world, (Object) null) && world.IsDiscovered;
      GameObject prefab = Assets.GetPrefab(this.GetPrefabTag());
      if (Object.op_Inequality((Object) prefab, (Object) null))
      {
        if (!(flag1 | prefab.HasTag(GameTags.WarpTech)))
          return;
        GameScenePartitioner.Instance.Free(ref this.fogOfWarPartitionerEntry);
        bool flag2 = false;
        if (Object.op_Inequality((Object) prefab.GetComponent<Pickupable>(), (Object) null) && !prefab.HasTag(GameTags.Creatures.Digger))
          flag2 = true;
        else if (prefab.GetDef<BurrowMonitor.Def>() != null)
          flag2 = true;
        if (flag2 && Grid.Solid[this.cell])
        {
          this.solidChangedPartitionerEntry = GameScenePartitioner.Instance.Add("WorldGenSpawner.OnSolidChanged", (object) this, this.cell, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
          ((Component) Game.Instance).GetComponent<EntombedItemVisualizer>().AddItem(this.cell);
        }
        else
          this.Spawn();
      }
      else
      {
        if (!flag1)
          return;
        GameScenePartitioner.Instance.Free(ref this.fogOfWarPartitionerEntry);
        this.Spawn();
      }
    }

    private Tag GetPrefabTag()
    {
      Mob mob = SettingsCache.mobs.GetMob(this.spawnInfo.id);
      return mob != null && mob.prefabName != null ? new Tag(mob.prefabName) : new Tag(this.spawnInfo.id);
    }

    private void Spawn()
    {
      this.isSpawned = true;
      GameObject gameObject = WorldGenSpawner.Spawnable.GetSpawnableCallback(this.spawnInfo.type)(this.spawnInfo, 0);
      if (Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Implicit((Object) gameObject))
      {
        gameObject.SetActive(true);
        EventExtensions.Trigger(gameObject, 1119167081, (object) null);
      }
      this.FreeResources();
    }

    public static WorldGenSpawner.Spawnable.PlaceEntityFn GetSpawnableCallback(Prefab.Type type)
    {
      switch (type)
      {
        case Prefab.Type.Building:
          return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlaceBuilding);
        case Prefab.Type.Ore:
          return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlaceElementalOres);
        case Prefab.Type.Pickupable:
          return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlacePickupables);
        case Prefab.Type.Other:
          return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlaceOtherEntities);
        default:
          return new WorldGenSpawner.Spawnable.PlaceEntityFn(TemplateLoader.PlaceOtherEntities);
      }
    }

    public delegate GameObject PlaceEntityFn(Prefab prefab, int root_cell);
  }
}
