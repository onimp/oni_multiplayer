// Decompiled with JetBrains decompiler
// Type: Comet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Comet")]
public class Comet : KMonoBehaviour, ISim33ms
{
  public SimHashes EXHAUST_ELEMENT = SimHashes.CarbonDioxide;
  public float EXHAUST_RATE = 50f;
  public Vector2 spawnVelocity = new Vector2(12f, 15f);
  public Vector2 spawnAngle = new Vector2(-100f, -80f);
  public Vector2 massRange;
  public Vector2 temperatureRange;
  public SpawnFXHashes explosionEffectHash;
  public int splashRadius = 1;
  public int addTiles;
  public int addTilesMinHeight;
  public int addTilesMaxHeight;
  public int entityDamage = 1;
  public float totalTileDamage = 0.2f;
  private float addTileMass;
  public int addDiseaseCount;
  public byte diseaseIdx = byte.MaxValue;
  public Vector2 elementReplaceTileTemperatureRange = new Vector2(800f, 1000f);
  public Vector2I explosionOreCount = new Vector2I(0, 0);
  private float explosionMass;
  public Vector2 explosionTemperatureRange = new Vector2(500f, 700f);
  public Vector2 explosionSpeedRange = new Vector2(8f, 14f);
  public float windowDamageMultiplier = 5f;
  public float bunkerDamageMultiplier;
  public string impactSound;
  public string flyingSound;
  public int flyingSoundID;
  private HashedString FLYING_SOUND_ID_PARAMETER = HashedString.op_Implicit("meteorType");
  [Serialize]
  protected Vector2 velocity;
  [Serialize]
  private float remainingTileDamage;
  private Vector3 previousPosition;
  private bool hasExploded;
  public bool canHitDuplicants;
  public string[] craterPrefabs;
  public bool destroyOnExplode = true;
  private float age;
  public System.Action OnImpact;
  public Ref<KPrefabID> ignoreObstacleForDamage = new Ref<KPrefabID>();
  private LoopingSounds loopingSounds;
  private List<GameObject> damagedEntities = new List<GameObject>();
  private List<int> destroyedCells = new List<int>();
  private const float MAX_DISTANCE_TEST = 6f;

  public Vector2 Velocity
  {
    get => this.velocity;
    set => this.velocity = value;
  }

  private float GetVolume(GameObject gameObject)
  {
    float volume = 1f;
    GameObject gameObject1 = gameObject;
    if (Object.op_Inequality((Object) gameObject1, (Object) null) && Object.op_Inequality((Object) gameObject1.GetComponent<KSelectable>(), (Object) null) && gameObject1.GetComponent<KSelectable>().IsSelected)
      volume = 1f;
    return volume;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.remainingTileDamage = this.totalTileDamage;
    this.loopingSounds = ((Component) this).gameObject.GetComponent<LoopingSounds>();
    this.flyingSound = GlobalAssets.GetSound("Meteor_LP");
    this.RandomizeVelocity();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.RandomizeMassAndTemperature();
    this.StartLoopingSound();
  }

  public virtual void RandomizeVelocity()
  {
    float num1 = Random.Range(this.spawnAngle.x, this.spawnAngle.y);
    float num2 = (float) ((double) num1 * 3.1415927410125732 / 180.0);
    float num3 = Random.Range(this.spawnVelocity.x, this.spawnVelocity.y);
    this.velocity = new Vector2(-Mathf.Cos(num2) * num3, Mathf.Sin(num2) * num3);
    ((Component) this).GetComponent<KBatchedAnimController>().Rotation = (float) (-(double) num1 - 90.0);
  }

  public void RandomizeMassAndTemperature()
  {
    float num1 = Random.Range(this.massRange.x, this.massRange.y);
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    component.Mass = num1;
    component.Temperature = Random.Range(this.temperatureRange.x, this.temperatureRange.y);
    if (this.addTiles > 0)
    {
      float num2 = Random.Range(0.95f, 0.98f);
      this.explosionMass = num1 * (1f - num2);
      this.addTileMass = num1 * num2;
    }
    else
    {
      this.explosionMass = num1;
      this.addTileMass = 0.0f;
    }
  }

  [ContextMenu("Explode")]
  private void Explode(Vector3 pos, int cell1, int prev_cell, Element element)
  {
    int world = (int) Grid.WorldIdx[cell1];
    this.PlayImpactSound(pos);
    Vector3 pos1 = pos;
    pos1.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);
    Game.Instance.SpawnFX(this.explosionEffectHash, pos1, 0.0f);
    Substance substance = element.substance;
    int num1 = Random.Range(this.explosionOreCount.x, this.explosionOreCount.y + 1);
    Vector2 vector2_1 = Vector2.op_UnaryNegation(((Vector2) ref this.velocity).normalized);
    Vector2 vector2_2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_2).\u002Ector(vector2_1.y, -vector2_1.x);
    ListPool<ScenePartitionerEntry, Comet>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, Comet>.Allocate();
    GameScenePartitioner.Instance.GatherEntries((int) pos.x - 3, (int) pos.y - 3, 6, 6, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
    foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
    {
      GameObject gameObject = ((Component) (partitionerEntry.obj as Pickupable)).gameObject;
      if (!Object.op_Inequality((Object) gameObject.GetComponent<MinionIdentity>(), (Object) null) && gameObject.GetDef<CreatureFallMonitor.Def>() == null)
      {
        Vector2 vector2_3 = Vector2.op_Implicit(Vector3.op_Subtraction(TransformExtensions.GetPosition(gameObject.transform), pos));
        vector2_3 = ((Vector2) ref vector2_3).normalized;
        Vector2 initial_velocity = Vector2.op_Multiply(Vector2.op_Addition(vector2_3, new Vector2(0.0f, 0.55f)), 0.5f * Random.Range(this.explosionSpeedRange.x, this.explosionSpeedRange.y));
        if (((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) gameObject))
          ((KGameObjectComponentManager<FallerComponent>) GameComps.Fallers).Remove(gameObject);
        if (((KComponentManager<GravityComponent>) GameComps.Gravities).Has((object) gameObject))
          GameComps.Gravities.Remove(gameObject);
        GameComps.Fallers.Add(gameObject, initial_velocity);
      }
    }
    gathered_entries.Recycle();
    int num2 = this.splashRadius + 1;
    for (int y = -num2; y <= num2; ++y)
    {
      for (int x = -num2; x <= num2; ++x)
      {
        int cell = Grid.OffsetCell(cell1, x, y);
        if (Grid.IsValidCellInWorld(cell, world) && !this.destroyedCells.Contains(cell))
        {
          float num3 = (float) ((1.0 - (double) Mathf.Abs(x) / (double) num2) * (1.0 - (double) Mathf.Abs(y) / (double) num2));
          if ((double) num3 > 0.0)
          {
            double num4 = (double) this.DamageTiles(cell, prev_cell, (float) ((double) num3 * (double) this.totalTileDamage * 0.5));
          }
        }
      }
    }
    float mass1 = num1 > 0 ? this.explosionMass / (float) num1 : 1f;
    float temperature = Random.Range(this.explosionTemperatureRange.x, this.explosionTemperatureRange.y);
    for (int index = 0; index < num1; ++index)
    {
      Vector2 vector2_4 = Vector2.op_Addition(vector2_1, Vector2.op_Multiply(vector2_2, Random.Range(-1f, 1f)));
      Vector2 normalized = ((Vector2) ref vector2_4).normalized;
      Vector3 vector3 = Vector2.op_Implicit(Vector2.op_Multiply(normalized, Random.Range(this.explosionSpeedRange.x, this.explosionSpeedRange.y)));
      Vector3 position = Vector3.op_Addition(Vector3.op_Addition(Vector2.op_Implicit(Vector2.op_Multiply(((Vector2) ref normalized).normalized, 0.75f)), new Vector3(0.0f, 0.55f, 0.0f)), pos);
      GameObject go = substance.SpawnResource(position, mass1, temperature, byte.MaxValue, 0);
      if (((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) go))
        ((KGameObjectComponentManager<FallerComponent>) GameComps.Fallers).Remove(go);
      GameComps.Fallers.Add(go, Vector2.op_Implicit(vector3));
    }
    if (this.addTiles > 0)
    {
      int depthOfElement = this.GetDepthOfElement(cell1, element, world);
      float num5 = 1f;
      int addTilesMinHeight = this.addTilesMinHeight;
      float f = (float) (depthOfElement - addTilesMinHeight) / (float) (this.addTilesMaxHeight - this.addTilesMinHeight);
      if (!float.IsNaN(f))
        num5 -= f;
      int num6 = Mathf.Min(this.addTiles, Mathf.Clamp(Mathf.RoundToInt((float) this.addTiles * num5), 1, this.addTiles));
      HashSetPool<int, Comet>.PooledHashSet valid_cells = HashSetPool<int, Comet>.Allocate();
      HashSetPool<int, Comet>.PooledHashSet visited_cells = HashSetPool<int, Comet>.Allocate();
      QueuePool<GameUtil.FloodFillInfo, Comet>.PooledQueue queue = QueuePool<GameUtil.FloodFillInfo, Comet>.Allocate();
      int num7 = -1;
      int num8 = 1;
      if ((double) this.velocity.x < 0.0)
      {
        num7 *= -1;
        num8 *= -1;
      }
      QueuePool<GameUtil.FloodFillInfo, Comet>.PooledQueue pooledQueue1 = queue;
      GameUtil.FloodFillInfo floodFillInfo1 = new GameUtil.FloodFillInfo();
      floodFillInfo1.cell = prev_cell;
      floodFillInfo1.depth = 0;
      GameUtil.FloodFillInfo floodFillInfo2 = floodFillInfo1;
      ((Queue<GameUtil.FloodFillInfo>) pooledQueue1).Enqueue(floodFillInfo2);
      QueuePool<GameUtil.FloodFillInfo, Comet>.PooledQueue pooledQueue2 = queue;
      floodFillInfo1 = new GameUtil.FloodFillInfo();
      floodFillInfo1.cell = Grid.OffsetCell(prev_cell, new CellOffset(num7, 0));
      floodFillInfo1.depth = 0;
      GameUtil.FloodFillInfo floodFillInfo3 = floodFillInfo1;
      ((Queue<GameUtil.FloodFillInfo>) pooledQueue2).Enqueue(floodFillInfo3);
      QueuePool<GameUtil.FloodFillInfo, Comet>.PooledQueue pooledQueue3 = queue;
      floodFillInfo1 = new GameUtil.FloodFillInfo();
      floodFillInfo1.cell = Grid.OffsetCell(prev_cell, new CellOffset(num8, 0));
      floodFillInfo1.depth = 0;
      GameUtil.FloodFillInfo floodFillInfo4 = floodFillInfo1;
      ((Queue<GameUtil.FloodFillInfo>) pooledQueue3).Enqueue(floodFillInfo4);
      Func<int, bool> condition = (Func<int, bool>) (cell2 => Grid.IsValidCellInWorld(cell2, world) && !Grid.Solid[cell2]);
      GameUtil.FloodFillConditional((Queue<GameUtil.FloodFillInfo>) queue, condition, (ICollection<int>) visited_cells, (ICollection<int>) valid_cells, 10);
      float mass2 = num6 > 0 ? this.addTileMass / (float) this.addTiles : 1f;
      int disease_count = this.addDiseaseCount / num6;
      if (element.HasTag(GameTags.Unstable))
      {
        UnstableGroundManager component = ((Component) World.Instance).GetComponent<UnstableGroundManager>();
        foreach (int cell in (HashSet<int>) valid_cells)
        {
          if (num6 > 0)
          {
            component.Spawn(cell, element, mass2, temperature, byte.MaxValue, 0);
            --num6;
          }
          else
            break;
        }
      }
      else
      {
        foreach (int gameCell in (HashSet<int>) valid_cells)
        {
          if (num6 > 0)
          {
            SimMessages.AddRemoveSubstance(gameCell, element.id, CellEventLogger.Instance.ElementEmitted, mass2, temperature, this.diseaseIdx, disease_count);
            --num6;
          }
          else
            break;
        }
      }
      valid_cells.Recycle();
      visited_cells.Recycle();
      queue.Recycle();
    }
    this.SpawnCraterPrefabs();
    if (this.OnImpact == null)
      return;
    this.OnImpact();
  }

  protected virtual void SpawnCraterPrefabs()
  {
    if (this.craterPrefabs == null || this.craterPrefabs.Length == 0)
      return;
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(this.craterPrefabs[Random.Range(0, this.craterPrefabs.Length)])), Grid.CellToPos(Grid.PosToCell((KMonoBehaviour) this)));
    gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -19.5f);
    gameObject.SetActive(true);
  }

  private int GetDepthOfElement(int cell, Element element, int world)
  {
    int depthOfElement = 0;
    for (int cell1 = Grid.CellBelow(cell); Grid.IsValidCellInWorld(cell1, world) && Grid.Element[cell1] == element; cell1 = Grid.CellBelow(cell1))
      ++depthOfElement;
    return depthOfElement;
  }

  [ContextMenu("DamageTiles")]
  private float DamageTiles(int cell, int prev_cell, float input_damage)
  {
    GameObject tile_go = Grid.Objects[cell, 9];
    float num1 = 1f;
    bool flag = false;
    if (Object.op_Inequality((Object) tile_go, (Object) null))
    {
      if (tile_go.GetComponent<KPrefabID>().HasTag(GameTags.Window))
        num1 = this.windowDamageMultiplier;
      else if (tile_go.GetComponent<KPrefabID>().HasTag(GameTags.Bunker))
      {
        num1 = this.bunkerDamageMultiplier;
        if (Object.op_Inequality((Object) tile_go.GetComponent<Door>(), (Object) null))
          Game.Instance.savedInfo.blockedCometWithBunkerDoor = true;
      }
      SimCellOccupier component = tile_go.GetComponent<SimCellOccupier>();
      if (Object.op_Inequality((Object) component, (Object) null) && !component.doReplaceElement)
        flag = true;
    }
    Element element = !flag ? Grid.Element[cell] : tile_go.GetComponent<PrimaryElement>().Element;
    if ((double) element.strength == 0.0)
      return 0.0f;
    float amount = input_damage * num1 / element.strength;
    this.PlayTileDamageSound(element, Grid.CellToPos(cell), tile_go);
    if ((double) amount == 0.0)
      return 0.0f;
    float num2;
    if (flag)
    {
      BuildingHP component = tile_go.GetComponent<BuildingHP>();
      double num3 = (double) component.HitPoints / (double) component.MaxHitPoints;
      float num4 = amount * (float) component.MaxHitPoints;
      EventExtensions.Trigger(((Component) component).gameObject, -794517298, (object) new BuildingHP.DamageSourceInfo()
      {
        damage = Mathf.RoundToInt(num4),
        source = (string) BUILDINGS.DAMAGESOURCES.COMET,
        popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.COMET
      });
      double num5 = (double) amount;
      num2 = Mathf.Min((float) num3, (float) num5);
    }
    else
      num2 = WorldDamage.Instance.ApplyDamage(cell, amount, prev_cell, (string) BUILDINGS.DAMAGESOURCES.COMET, (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.COMET);
    this.destroyedCells.Add(cell);
    float num6 = num2 / amount;
    return input_damage * (1f - num6);
  }

  private void DamageThings(Vector3 pos, int cell, int damage, GameObject ignoreObject = null)
  {
    if (!Grid.IsValidCell(cell))
      return;
    GameObject building_go = Grid.Objects[cell, 1];
    if (Object.op_Inequality((Object) building_go, (Object) null) && Object.op_Inequality((Object) building_go, (Object) ignoreObject))
    {
      BuildingHP component1 = building_go.GetComponent<BuildingHP>();
      Building component2 = building_go.GetComponent<Building>();
      if (Object.op_Inequality((Object) component1, (Object) null) && !this.damagedEntities.Contains(building_go))
      {
        float num = building_go.GetComponent<KPrefabID>().HasTag(GameTags.Bunker) ? (float) damage * this.bunkerDamageMultiplier : (float) damage;
        if (Object.op_Inequality((Object) component2, (Object) null) && Object.op_Inequality((Object) component2.Def, (Object) null))
          this.PlayBuildingDamageSound(component2.Def, Grid.CellToPos(cell), building_go);
        EventExtensions.Trigger(((Component) component1).gameObject, -794517298, (object) new BuildingHP.DamageSourceInfo()
        {
          damage = Mathf.RoundToInt(num),
          source = (string) BUILDINGS.DAMAGESOURCES.COMET,
          popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.COMET
        });
        this.damagedEntities.Add(building_go);
      }
    }
    ListPool<ScenePartitionerEntry, Comet>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, Comet>.Allocate();
    GameScenePartitioner.Instance.GatherEntries((int) pos.x, (int) pos.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
    foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
    {
      Pickupable pickupable = partitionerEntry.obj as Pickupable;
      Health component = ((Component) pickupable).GetComponent<Health>();
      if (Object.op_Inequality((Object) component, (Object) null) && !this.damagedEntities.Contains(((Component) pickupable).gameObject))
      {
        float amount = ((Component) pickupable).GetComponent<KPrefabID>().HasTag(GameTags.Bunker) ? (float) damage * this.bunkerDamageMultiplier : (float) damage;
        component.Damage(amount);
        this.damagedEntities.Add(((Component) pickupable).gameObject);
      }
    }
    gathered_entries.Recycle();
  }

  private float GetDistanceFromImpact()
  {
    float num1 = this.velocity.x / this.velocity.y;
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    float num2 = 0.0f;
    while ((double) num2 > -6.0)
    {
      float num3 = num2 - 1f;
      num2 = Mathf.Ceil(position.y + num3) - 0.2f - position.y;
      float num4 = num2 * num1;
      Vector3 vector3;
      // ISSUE: explicit constructor call
      ((Vector3) ref vector3).\u002Ector(num4, num2, 0.0f);
      int cell = Grid.PosToCell(Vector3.op_Addition(position, vector3));
      if (Grid.IsValidCell(cell) && Grid.Solid[cell])
        return ((Vector3) ref vector3).magnitude;
    }
    return 6f;
  }

  public float GetSoundDistance() => this.GetDistanceFromImpact();

  private void PlayTileDamageSound(Element element, Vector3 pos, GameObject tile_go)
  {
    string sound = GlobalAssets.GetSound("MeteorDamage_" + (element.substance.GetMiningBreakSound() ?? (!element.HasTag(GameTags.RefinedMetal) ? (!element.HasTag(GameTags.Metal) ? "Rock" : "RawMetal") : "RefinedMetal")));
    if (!Object.op_Implicit((Object) CameraController.Instance) || !CameraController.Instance.IsAudibleSound(pos, HashedString.op_Implicit(sound)))
      return;
    float volume = this.GetVolume(tile_go);
    KFMOD.PlayOneShot(sound, CameraController.Instance.GetVerticallyScaledPosition(pos), volume);
  }

  private void PlayBuildingDamageSound(BuildingDef def, Vector3 pos, GameObject building_go)
  {
    if (!Object.op_Inequality((Object) def, (Object) null))
      return;
    string str = GlobalAssets.GetSound(StringFormatter.Combine("MeteorDamage_Building_", def.AudioCategory)) ?? GlobalAssets.GetSound("MeteorDamage_Building_Metal");
    if (str == null || !Object.op_Implicit((Object) CameraController.Instance) || !CameraController.Instance.IsAudibleSound(pos, HashedString.op_Implicit(str)))
      return;
    float volume = this.GetVolume(building_go);
    KFMOD.PlayOneShot(str, CameraController.Instance.GetVerticallyScaledPosition(pos), volume);
  }

  public void Sim33ms(float dt)
  {
    if (this.hasExploded)
      return;
    Vector2 vector2_1 = Vector2.op_Multiply(new Vector2((float) Grid.WidthInCells, (float) Grid.HeightInCells), -0.1f);
    Vector2 vector2_2 = Vector2.op_Multiply(new Vector2((float) Grid.WidthInCells, (float) Grid.HeightInCells), 1.1f);
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    Vector3 pos = Vector3.op_Addition(position, new Vector3(this.velocity.x * dt, this.velocity.y * dt, 0.0f));
    int cell1 = Grid.PosToCell(pos);
    this.loopingSounds.UpdateVelocity(this.flyingSound, Vector2.op_Implicit(Vector3.op_Subtraction(pos, position)));
    Element elementByHash = ElementLoader.FindElementByHash(this.EXHAUST_ELEMENT);
    if (this.EXHAUST_ELEMENT != SimHashes.Void && Grid.IsValidCell(cell1) && !Grid.Solid[cell1])
      SimMessages.EmitMass(cell1, elementByHash.idx, dt * this.EXHAUST_RATE, elementByHash.defaultValues.temperature, this.diseaseIdx, Mathf.RoundToInt((float) this.addDiseaseCount * dt));
    if ((double) pos.x < (double) vector2_1.x || (double) vector2_2.x < (double) pos.x || (double) pos.y < (double) vector2_1.y)
      Util.KDestroyGameObject(((Component) this).gameObject);
    int cell2 = Grid.PosToCell((KMonoBehaviour) this);
    int cell3 = Grid.PosToCell(this.previousPosition);
    if (cell2 != cell3)
    {
      if (Grid.IsValidCell(cell2) && Grid.Solid[cell2])
      {
        PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
        this.remainingTileDamage = this.DamageTiles(cell2, cell3, this.remainingTileDamage);
        if ((double) this.remainingTileDamage <= 0.0)
        {
          this.Explode(position, cell2, cell3, component.Element);
          this.hasExploded = true;
          if (!this.destroyOnExplode)
            return;
          Util.KDestroyGameObject(((Component) this).gameObject);
          return;
        }
      }
      else
      {
        GameObject ignoreObject = Object.op_Equality((Object) this.ignoreObstacleForDamage.Get(), (Object) null) ? (GameObject) null : ((Component) this.ignoreObstacleForDamage.Get()).gameObject;
        this.DamageThings(position, cell2, this.entityDamage, ignoreObject);
      }
    }
    if (this.canHitDuplicants && (double) this.age > 0.25 && Object.op_Inequality((Object) Grid.Objects[Grid.PosToCell(position), 0], (Object) null))
    {
      this.transform.position = Grid.CellToPos(Grid.PosToCell(position));
      this.Explode(position, cell2, cell3, ((Component) this).GetComponent<PrimaryElement>().Element);
      if (!this.destroyOnExplode)
        return;
      Util.KDestroyGameObject(((Component) this).gameObject);
    }
    else
    {
      this.previousPosition = position;
      TransformExtensions.SetPosition(this.transform, pos);
      this.age += dt;
    }
  }

  private void PlayImpactSound(Vector3 pos)
  {
    if (this.impactSound == null)
      this.impactSound = "Meteor_Large_Impact";
    this.loopingSounds.StopSound(this.flyingSound);
    string sound = GlobalAssets.GetSound(this.impactSound);
    if (!CameraController.Instance.IsAudibleSound(pos, HashedString.op_Implicit(sound)))
      return;
    float volume = this.GetVolume(((Component) this).gameObject);
    pos.z = 0.0f;
    EventInstance eventInstance = KFMOD.BeginOneShot(sound, pos, volume);
    ((EventInstance) ref eventInstance).setParameterByName("userVolume_SFX", KPlayerPrefs.GetFloat("Volume_SFX"), false);
    KFMOD.EndOneShot(eventInstance);
  }

  private void StartLoopingSound()
  {
    this.loopingSounds.StartSound(this.flyingSound);
    this.loopingSounds.UpdateFirstParameter(this.flyingSound, this.FLYING_SOUND_ID_PARAMETER, (float) this.flyingSoundID);
  }
}
