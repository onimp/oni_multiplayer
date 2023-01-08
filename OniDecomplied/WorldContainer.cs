// Decompiled with JetBrains decompiler
// Type: WorldContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Delaunay.Geo;
using Klei;
using KSerialization;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using TemplateClasses;
using TUNING;
using UnityEngine;

[SerializationConfig]
public class WorldContainer : KMonoBehaviour
{
  [Serialize]
  public int id = -1;
  [Serialize]
  public Tag prefabTag;
  [Serialize]
  private Vector2I worldOffset;
  [Serialize]
  private Vector2I worldSize;
  [Serialize]
  private bool fullyEnclosedBorder;
  [Serialize]
  private bool isModuleInterior;
  [Serialize]
  private WorldDetailSave.OverworldCell overworldCell;
  [Serialize]
  private bool isDiscovered;
  [Serialize]
  private bool isStartWorld;
  [Serialize]
  private bool isDupeVisited;
  [Serialize]
  private float dupeVisitedTimestamp;
  [Serialize]
  private float discoveryTimestamp;
  [Serialize]
  private bool isRoverVisited;
  [Serialize]
  private bool isSurfaceRevealed;
  [Serialize]
  public string worldName;
  [Serialize]
  public string[] nameTables;
  [Serialize]
  public string worldType;
  [Serialize]
  public string worldDescription;
  [Serialize]
  public int sunlight = FIXEDTRAITS.SUNLIGHT.DEFAULT_VALUE;
  [Serialize]
  public int cosmicRadiation = FIXEDTRAITS.COSMICRADIATION.DEFAULT_VALUE;
  [Serialize]
  public float currentSunlightIntensity;
  [Serialize]
  public float currentCosmicIntensity = (float) FIXEDTRAITS.COSMICRADIATION.DEFAULT_VALUE;
  [Serialize]
  public string sunlightFixedTrait;
  [Serialize]
  public string cosmicRadiationFixedTrait;
  [Serialize]
  public int fixedTraitsUpdateVersion = 1;
  private Dictionary<string, int> sunlightFixedTraits = new Dictionary<string, int>()
  {
    {
      FIXEDTRAITS.SUNLIGHT.NAME.NONE,
      FIXEDTRAITS.SUNLIGHT.NONE
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_LOW,
      FIXEDTRAITS.SUNLIGHT.VERY_VERY_LOW
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.VERY_LOW,
      FIXEDTRAITS.SUNLIGHT.VERY_LOW
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.LOW,
      FIXEDTRAITS.SUNLIGHT.LOW
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.MED_LOW,
      FIXEDTRAITS.SUNLIGHT.MED_LOW
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.MED,
      FIXEDTRAITS.SUNLIGHT.MED
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.MED_HIGH,
      FIXEDTRAITS.SUNLIGHT.MED_HIGH
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.HIGH,
      FIXEDTRAITS.SUNLIGHT.HIGH
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.VERY_HIGH,
      FIXEDTRAITS.SUNLIGHT.VERY_HIGH
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_HIGH,
      FIXEDTRAITS.SUNLIGHT.VERY_VERY_HIGH
    },
    {
      FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_VERY_HIGH,
      FIXEDTRAITS.SUNLIGHT.VERY_VERY_VERY_HIGH
    }
  };
  private Dictionary<string, int> cosmicRadiationFixedTraits = new Dictionary<string, int>()
  {
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.NONE,
      FIXEDTRAITS.COSMICRADIATION.NONE
    },
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.VERY_VERY_LOW,
      FIXEDTRAITS.COSMICRADIATION.VERY_VERY_LOW
    },
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.VERY_LOW,
      FIXEDTRAITS.COSMICRADIATION.VERY_LOW
    },
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.LOW,
      FIXEDTRAITS.COSMICRADIATION.LOW
    },
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.MED_LOW,
      FIXEDTRAITS.COSMICRADIATION.MED_LOW
    },
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.MED,
      FIXEDTRAITS.COSMICRADIATION.MED
    },
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.MED_HIGH,
      FIXEDTRAITS.COSMICRADIATION.MED_HIGH
    },
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.HIGH,
      FIXEDTRAITS.COSMICRADIATION.HIGH
    },
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.VERY_HIGH,
      FIXEDTRAITS.COSMICRADIATION.VERY_HIGH
    },
    {
      FIXEDTRAITS.COSMICRADIATION.NAME.VERY_VERY_HIGH,
      FIXEDTRAITS.COSMICRADIATION.VERY_VERY_HIGH
    }
  };
  [Serialize]
  private List<string> m_seasonIds;
  [Serialize]
  private List<string> m_subworldNames;
  [Serialize]
  private List<string> m_worldTraitIds;
  [Serialize]
  private List<string> m_storyTraitIds;
  private WorldParentChangedEventArgs parentChangeArgs = new WorldParentChangedEventArgs();
  [MySmiReq]
  private AlertStateManager.Instance m_alertManager;
  private List<Prioritizable> yellowAlertTasks = new List<Prioritizable>();

  [Serialize]
  public WorldInventory worldInventory { get; private set; }

  public Dictionary<Tag, float> materialNeeds { get; private set; }

  public bool IsModuleInterior => this.isModuleInterior;

  public bool IsDiscovered => this.isDiscovered || DebugHandler.RevealFogOfWar;

  public bool IsStartWorld => this.isStartWorld;

  public bool IsDupeVisited => this.isDupeVisited;

  public float DupeVisitedTimestamp => this.dupeVisitedTimestamp;

  public float DiscoveryTimestamp => this.discoveryTimestamp;

  public bool IsRoverVisted => this.isRoverVisited;

  public bool IsSurfaceRevealed => this.isSurfaceRevealed;

  public Dictionary<string, int> SunlightFixedTraits => this.sunlightFixedTraits;

  public Dictionary<string, int> CosmicRadiationFixedTraits => this.cosmicRadiationFixedTraits;

  public List<string> Biomes => this.m_subworldNames;

  public List<string> WorldTraitIds => this.m_worldTraitIds;

  public List<string> StoryTraitIds => this.m_storyTraitIds;

  public AlertStateManager.Instance AlertManager
  {
    get
    {
      if (this.m_alertManager == null)
        this.m_alertManager = ((Component) this).GetComponent<StateMachineController>().GetSMI<AlertStateManager.Instance>();
      Debug.Assert(this.m_alertManager != null, (object) "AlertStateManager should never be null.");
      return this.m_alertManager;
    }
  }

  public void AddTopPriorityPrioritizable(Prioritizable prioritizable)
  {
    if (!this.yellowAlertTasks.Contains(prioritizable))
      this.yellowAlertTasks.Add(prioritizable);
    this.RefreshHasTopPriorityChore();
  }

  public void RemoveTopPriorityPrioritizable(Prioritizable prioritizable)
  {
    for (int index = this.yellowAlertTasks.Count - 1; index >= 0; --index)
    {
      if (Object.op_Equality((Object) this.yellowAlertTasks[index], (Object) prioritizable) || ((object) this.yellowAlertTasks[index]).Equals((object) null))
        this.yellowAlertTasks.RemoveAt(index);
    }
    this.RefreshHasTopPriorityChore();
  }

  public int ParentWorldId { get; private set; }

  public Quadrant[] GetQuadrantOfCell(int cell, int depth = 1)
  {
    Vector2 vector2_1;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_1).\u002Ector((float) this.WorldSize.x * Grid.CellSizeInMeters, (float) this.worldSize.y * Grid.CellSizeInMeters);
    Vector2 vector2_2 = Vector2.op_Implicit(Grid.CellToPos2D(Grid.XYToCell(this.WorldOffset.x, this.WorldOffset.y)));
    Vector2 vector2_3 = Vector2.op_Implicit(Grid.CellToPos2D(cell));
    Quadrant[] quadrantOfCell = new Quadrant[depth];
    Vector2 vector2_4;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_4).\u002Ector(vector2_2.x, (float) this.worldOffset.y + vector2_1.y);
    Vector2 vector2_5;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_5).\u002Ector(vector2_2.x + vector2_1.x, (float) this.worldOffset.y);
    for (int index = 0; index < depth; ++index)
    {
      float num1 = vector2_5.x - vector2_4.x;
      double num2 = (double) vector2_4.y - (double) vector2_5.y;
      float num3 = num1 * 0.5f;
      float num4 = (float) (num2 * 0.5);
      if ((double) vector2_3.x >= (double) vector2_4.x + (double) num3 && (double) vector2_3.y >= (double) vector2_5.y + (double) num4)
        quadrantOfCell[index] = Quadrant.NE;
      if ((double) vector2_3.x >= (double) vector2_4.x + (double) num3 && (double) vector2_3.y < (double) vector2_5.y + (double) num4)
        quadrantOfCell[index] = Quadrant.SE;
      if ((double) vector2_3.x < (double) vector2_4.x + (double) num3 && (double) vector2_3.y < (double) vector2_5.y + (double) num4)
        quadrantOfCell[index] = Quadrant.SW;
      if ((double) vector2_3.x < (double) vector2_4.x + (double) num3 && (double) vector2_3.y >= (double) vector2_5.y + (double) num4)
        quadrantOfCell[index] = Quadrant.NW;
      switch (quadrantOfCell[index])
      {
        case Quadrant.NE:
          vector2_4.x += num3;
          vector2_5.y += num4;
          break;
        case Quadrant.NW:
          vector2_5.x -= num3;
          vector2_5.y += num4;
          break;
        case Quadrant.SW:
          vector2_4.y -= num4;
          vector2_5.x -= num3;
          break;
        case Quadrant.SE:
          vector2_4.x += num3;
          vector2_4.y -= num4;
          break;
      }
    }
    return quadrantOfCell;
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized() => this.ParentWorldId = this.id;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.worldInventory = ((Component) this).GetComponent<WorldInventory>();
    this.materialNeeds = new Dictionary<Tag, float>();
    ClusterManager.Instance.RegisterWorldContainer(this);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this).gameObject.AddOrGet<InfoDescription>().DescriptionLocString = this.worldDescription;
    this.RefreshHasTopPriorityChore();
    this.UpgradeFixedTraits();
    this.RefreshFixedTraits();
    if (!DlcManager.IsPureVanilla())
      return;
    this.isStartWorld = true;
    this.isDupeVisited = true;
  }

  protected virtual void OnCleanUp()
  {
    SaveGame.Instance.materialSelectorSerializer.WipeWorldSelectionData(this.id);
    base.OnCleanUp();
  }

  private void UpgradeFixedTraits()
  {
    if (this.sunlightFixedTrait == null || this.sunlightFixedTrait == "")
      new Dictionary<int, string>()
      {
        {
          160000,
          FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_HIGH
        },
        {
          0,
          FIXEDTRAITS.SUNLIGHT.NAME.NONE
        },
        {
          10000,
          FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_LOW
        },
        {
          20000,
          FIXEDTRAITS.SUNLIGHT.NAME.VERY_LOW
        },
        {
          30000,
          FIXEDTRAITS.SUNLIGHT.NAME.LOW
        },
        {
          35000,
          FIXEDTRAITS.SUNLIGHT.NAME.MED_LOW
        },
        {
          40000,
          FIXEDTRAITS.SUNLIGHT.NAME.MED
        },
        {
          50000,
          FIXEDTRAITS.SUNLIGHT.NAME.MED_HIGH
        },
        {
          60000,
          FIXEDTRAITS.SUNLIGHT.NAME.HIGH
        },
        {
          80000,
          FIXEDTRAITS.SUNLIGHT.NAME.VERY_HIGH
        },
        {
          120000,
          FIXEDTRAITS.SUNLIGHT.NAME.VERY_VERY_HIGH
        }
      }.TryGetValue(this.sunlight, out this.sunlightFixedTrait);
    if (this.cosmicRadiationFixedTrait != null && !(this.cosmicRadiationFixedTrait == ""))
      return;
    new Dictionary<int, string>()
    {
      {
        0,
        FIXEDTRAITS.COSMICRADIATION.NAME.NONE
      },
      {
        6,
        FIXEDTRAITS.COSMICRADIATION.NAME.VERY_VERY_LOW
      },
      {
        12,
        FIXEDTRAITS.COSMICRADIATION.NAME.VERY_LOW
      },
      {
        18,
        FIXEDTRAITS.COSMICRADIATION.NAME.LOW
      },
      {
        21,
        FIXEDTRAITS.COSMICRADIATION.NAME.MED_LOW
      },
      {
        25,
        FIXEDTRAITS.COSMICRADIATION.NAME.MED
      },
      {
        31,
        FIXEDTRAITS.COSMICRADIATION.NAME.MED_HIGH
      },
      {
        37,
        FIXEDTRAITS.COSMICRADIATION.NAME.HIGH
      },
      {
        50,
        FIXEDTRAITS.COSMICRADIATION.NAME.VERY_HIGH
      },
      {
        75,
        FIXEDTRAITS.COSMICRADIATION.NAME.VERY_VERY_HIGH
      }
    }.TryGetValue(this.cosmicRadiation, out this.cosmicRadiationFixedTrait);
  }

  private void RefreshFixedTraits()
  {
    this.sunlight = this.GetSunlightValueFromFixedTrait();
    this.cosmicRadiation = this.GetCosmicRadiationValueFromFixedTrait();
  }

  private void RefreshHasTopPriorityChore()
  {
    if (this.AlertManager == null)
      return;
    this.AlertManager.SetHasTopPriorityChore(this.yellowAlertTasks.Count > 0);
  }

  public List<string> GetSeasonIds() => this.m_seasonIds;

  public bool IsRedAlert() => this.m_alertManager.IsRedAlert();

  public bool IsYellowAlert() => this.m_alertManager.IsYellowAlert();

  public string GetRandomName() => GameUtil.GenerateRandomWorldName(this.nameTables);

  public void SetID(int id)
  {
    this.id = id;
    this.ParentWorldId = id;
  }

  public void SetParentIdx(int parentIdx)
  {
    this.parentChangeArgs.lastParentId = this.ParentWorldId;
    this.parentChangeArgs.world = this;
    this.ParentWorldId = parentIdx;
    Game.Instance.Trigger(880851192, (object) this.parentChangeArgs);
    this.parentChangeArgs.lastParentId = (int) ClusterManager.INVALID_WORLD_IDX;
  }

  public Vector2 minimumBounds => new Vector2((float) this.worldOffset.x, (float) this.worldOffset.y);

  public Vector2 maximumBounds => new Vector2((float) (this.worldOffset.x + (this.worldSize.x - 1)), (float) (this.worldOffset.y + (this.worldSize.y - 1)));

  public Vector2I WorldSize => this.worldSize;

  public Vector2I WorldOffset => this.worldOffset;

  public bool FullyEnclosedBorder => this.fullyEnclosedBorder;

  public int Height => this.worldSize.y;

  public int Width => this.worldSize.x;

  public void SetDiscovered(bool reveal_surface = false)
  {
    if (!this.isDiscovered)
      this.discoveryTimestamp = GameUtil.GetCurrentTimeInCycles();
    this.isDiscovered = true;
    if (reveal_surface)
      this.LookAtSurface();
    Game.Instance.Trigger(-521212405, (object) this);
  }

  public void SetDupeVisited()
  {
    if (this.isDupeVisited)
      return;
    this.dupeVisitedTimestamp = GameUtil.GetCurrentTimeInCycles();
    this.isDupeVisited = true;
    Game.Instance.Trigger(-434755240, (object) this);
  }

  public void SetRoverLanded() => this.isRoverVisited = true;

  public void SetRocketInteriorWorldDetails(int world_id, Vector2I size, Vector2I offset)
  {
    this.SetID(world_id);
    this.fullyEnclosedBorder = true;
    this.worldOffset = offset;
    this.worldSize = size;
    this.isDiscovered = true;
    this.isModuleInterior = true;
    this.m_seasonIds = new List<string>();
  }

  private static int IsClockwise(Vector2 first, Vector2 second, Vector2 origin)
  {
    if (Vector2.op_Equality(first, second))
      return 0;
    Vector2 vector2_1 = Vector2.op_Subtraction(first, origin);
    Vector2 vector2_2 = Vector2.op_Subtraction(second, origin);
    float num1 = Mathf.Atan2(vector2_1.x, vector2_1.y);
    float num2 = Mathf.Atan2(vector2_2.x, vector2_2.y);
    return (double) num1 < (double) num2 || (double) num1 <= (double) num2 && (double) ((Vector2) ref vector2_1).sqrMagnitude < (double) ((Vector2) ref vector2_2).sqrMagnitude ? 1 : -1;
  }

  public void PlaceInteriorTemplate(string template_name, System.Action callback)
  {
    TemplateContainer template = TemplateCache.GetTemplate(template_name);
    Vector2 pos = new Vector2((float) (this.worldSize.x / 2 + this.worldOffset.x), (float) (this.worldSize.y / 2 + this.worldOffset.y));
    TemplateLoader.Stamp(template, pos, callback);
    Vector2f size1 = template.info.size;
    float val1 = ((Vector2f) ref size1).X / 2f;
    Vector2f size2 = template.info.size;
    float val2 = ((Vector2f) ref size2).Y / 2f;
    float num1 = Math.Max(val1, val2);
    GridVisibility.Reveal((int) pos.x, (int) pos.y, (int) num1 + 3 + 5, num1 + 3f);
    WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
    this.overworldCell = new WorldDetailSave.OverworldCell();
    List<Vector2> vector2List = new List<Vector2>(template.cells.Count);
    foreach (Prefab building in template.buildings)
    {
      if (building.id == "RocketWallTile")
      {
        Vector2 vector2;
        // ISSUE: explicit constructor call
        ((Vector2) ref vector2).\u002Ector((float) building.location_x + pos.x, (float) building.location_y + pos.y);
        if ((double) vector2.x > (double) pos.x)
          vector2.x += 0.5f;
        if ((double) vector2.y > (double) pos.y)
          vector2.y += 0.5f;
        vector2List.Add(vector2);
      }
    }
    vector2List.Sort((Comparison<Vector2>) ((v1, v2) => WorldContainer.IsClockwise(v1, v2, pos)));
    this.overworldCell.poly = new Polygon(vector2List);
    this.overworldCell.zoneType = (SubWorld.ZoneType) 14;
    WorldDetailSave.OverworldCell overworldCell = this.overworldCell;
    TagSet tagSet = new TagSet();
    tagSet.Add(WorldGenTags.RocketInterior);
    overworldCell.tags = tagSet;
    clusterDetailSave.overworldCells.Add(this.overworldCell);
    Rect rect;
    ref Rect local = ref rect;
    double num2 = (double) pos.x - (double) val1 + 1.0;
    double num3 = (double) pos.y - (double) val2 + 1.0;
    Vector2f size3 = template.info.size;
    double x = (double) ((Vector2f) ref size3).X;
    size3 = template.info.size;
    double y = (double) ((Vector2f) ref size3).Y;
    // ISSUE: explicit constructor call
    ((Rect) ref local).\u002Ector((float) num2, (float) num3, (float) x, (float) y);
    for (int yMin = (int) ((Rect) ref rect).yMin; (double) yMin < (double) ((Rect) ref rect).yMax; ++yMin)
    {
      for (int xMin = (int) ((Rect) ref rect).xMin; (double) xMin < (double) ((Rect) ref rect).xMax; ++xMin)
        SimMessages.ModifyCellWorldZone(Grid.XYToCell(xMin, yMin), (byte) 0);
    }
  }

  private string GetSunlightFromFixedTraits(WorldGen world)
  {
    foreach (string fixedTrait in world.Settings.world.fixedTraits)
    {
      if (this.sunlightFixedTraits.ContainsKey(fixedTrait))
        return fixedTrait;
    }
    return FIXEDTRAITS.SUNLIGHT.NAME.DEFAULT;
  }

  private string GetCosmicRadiationFromFixedTraits(WorldGen world)
  {
    foreach (string fixedTrait in world.Settings.world.fixedTraits)
    {
      if (this.cosmicRadiationFixedTraits.ContainsKey(fixedTrait))
        return fixedTrait;
    }
    return FIXEDTRAITS.COSMICRADIATION.NAME.DEFAULT;
  }

  private int GetSunlightValueFromFixedTrait()
  {
    if (this.sunlightFixedTrait == null)
      this.sunlightFixedTrait = FIXEDTRAITS.SUNLIGHT.NAME.DEFAULT;
    return this.sunlightFixedTraits.ContainsKey(this.sunlightFixedTrait) ? this.sunlightFixedTraits[this.sunlightFixedTrait] : FIXEDTRAITS.SUNLIGHT.DEFAULT_VALUE;
  }

  private int GetCosmicRadiationValueFromFixedTrait()
  {
    if (this.cosmicRadiationFixedTrait == null)
      this.cosmicRadiationFixedTrait = FIXEDTRAITS.COSMICRADIATION.NAME.DEFAULT;
    return this.cosmicRadiationFixedTraits.ContainsKey(this.cosmicRadiationFixedTrait) ? this.cosmicRadiationFixedTraits[this.cosmicRadiationFixedTrait] : FIXEDTRAITS.COSMICRADIATION.DEFAULT_VALUE;
  }

  public void SetWorldDetails(WorldGen world)
  {
    if (world != null)
    {
      this.fullyEnclosedBorder = world.Settings.GetBoolSetting("DrawWorldBorder") && world.Settings.GetBoolSetting("DrawWorldBorderOverVacuum");
      this.worldOffset = world.GetPosition();
      this.worldSize = world.GetSize();
      this.isDiscovered = world.isStartingWorld;
      this.isStartWorld = world.isStartingWorld;
      this.worldName = world.Settings.world.filePath;
      this.nameTables = world.Settings.world.nameTables;
      this.worldDescription = world.Settings.world.description;
      this.worldType = world.Settings.world.name;
      this.isModuleInterior = world.Settings.world.moduleInterior;
      this.m_seasonIds = new List<string>((IEnumerable<string>) world.Settings.world.seasons);
      this.sunlightFixedTrait = this.GetSunlightFromFixedTraits(world);
      this.cosmicRadiationFixedTrait = this.GetCosmicRadiationFromFixedTraits(world);
      this.sunlight = this.GetSunlightValueFromFixedTrait();
      this.cosmicRadiation = this.GetCosmicRadiationValueFromFixedTrait();
      this.currentCosmicIntensity = (float) this.cosmicRadiation;
      this.m_subworldNames = new List<string>();
      foreach (WeightedSubworldName subworldFile in world.Settings.world.subworldFiles)
      {
        string name = subworldFile.name;
        string str = name.Substring(0, name.LastIndexOf('/'));
        this.m_subworldNames.Add(str.Substring(str.LastIndexOf('/') + 1, str.Length - (str.LastIndexOf('/') + 1)));
      }
      this.m_worldTraitIds = new List<string>();
      this.m_worldTraitIds.AddRange((IEnumerable<string>) world.Settings.GetWorldTraitIDs());
      this.m_storyTraitIds = new List<string>();
      this.m_storyTraitIds.AddRange((IEnumerable<string>) world.Settings.GetStoryTraitIDs());
    }
    else
    {
      this.fullyEnclosedBorder = false;
      this.worldOffset = Vector2I.zero;
      this.worldSize = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
      this.isDiscovered = true;
      this.isStartWorld = true;
      this.isDupeVisited = true;
      this.m_seasonIds = new List<string>()
      {
        Db.Get().GameplaySeasons.MeteorShowers.Id
      };
    }
  }

  public bool ContainsPoint(Vector2 point) => (double) point.x >= (double) this.worldOffset.x && (double) point.y >= (double) this.worldOffset.y && (double) point.x < (double) (this.worldOffset.x + this.worldSize.x) && (double) point.y < (double) (this.worldOffset.y + this.worldSize.y);

  public void LookAtSurface()
  {
    if (!this.IsDupeVisited)
      this.RevealSurface();
    Vector3? nullable = this.SetSurfaceCameraPos();
    if (ClusterManager.Instance.activeWorldId != this.id || !nullable.HasValue)
      return;
    CameraController.Instance.SnapTo(nullable.Value);
  }

  public void RevealSurface()
  {
    if (this.isSurfaceRevealed)
      return;
    this.isSurfaceRevealed = true;
    for (int index1 = 0; index1 < this.worldSize.x; ++index1)
    {
      for (int index2 = this.worldSize.y - 1; index2 >= 0; --index2)
      {
        int cell = Grid.XYToCell(index1 + this.worldOffset.x, index2 + this.worldOffset.y);
        if (Grid.IsValidCell(cell) && !Grid.IsSolidCell(cell) && !Grid.IsLiquid(cell))
          GridVisibility.Reveal(index1 + ((Vector2I) ref this.worldOffset).X, index2 + this.worldOffset.y, 7, 1f);
        else
          break;
      }
    }
  }

  private Vector3? SetSurfaceCameraPos()
  {
    if (!Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
      return new Vector3?();
    int val1 = (int) this.maximumBounds.y;
    for (int index1 = 0; index1 < ((Vector2I) ref this.worldSize).X; ++index1)
    {
      for (int index2 = this.worldSize.y - 1; index2 >= 0; --index2)
      {
        int num = index2 + this.worldOffset.y;
        int cell = Grid.XYToCell(index1 + this.worldOffset.x, num);
        if (Grid.IsValidCell(cell) && (Grid.Solid[cell] || Grid.IsLiquid(cell)))
        {
          val1 = Math.Min(val1, num);
          break;
        }
      }
    }
    int num1 = (val1 + this.worldOffset.y + this.worldSize.y) / 2;
    Vector3 start_pos;
    // ISSUE: explicit constructor call
    ((Vector3) ref start_pos).\u002Ector((float) (this.WorldOffset.x + this.Width / 2), (float) num1, 0.0f);
    ((Component) SaveGame.Instance).GetComponent<UserNavigation>().SetWorldCameraStartPosition(this.id, start_pos);
    return new Vector3?(start_pos);
  }

  public void EjectAllDupes(Vector3 spawn_pos)
  {
    foreach (KMonoBehaviour worldItem in Components.MinionIdentities.GetWorldItems(this.id))
      TransformExtensions.SetLocalPosition(worldItem.transform, spawn_pos);
  }

  public void SpacePodAllDupes(AxialI sourceLocation, SimHashes podElement)
  {
    foreach (MinionIdentity worldItem in Components.MinionIdentities.GetWorldItems(this.id))
    {
      if (!((Component) worldItem).HasTag(GameTags.Dead))
      {
        Vector3 vector3;
        // ISSUE: explicit constructor call
        ((Vector3) ref vector3).\u002Ector(-1f, -1f, 0.0f);
        GameObject go = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("EscapePod")), vector3);
        go.GetComponent<PrimaryElement>().SetElement(podElement);
        go.SetActive(true);
        go.GetComponent<MinionStorage>().SerializeMinion(((Component) worldItem).gameObject);
        TravellingCargoLander.StatesInstance smi = go.GetSMI<TravellingCargoLander.StatesInstance>();
        smi.StartSM();
        smi.Travel(sourceLocation, ClusterUtil.ClosestVisibleAsteroidToLocation(sourceLocation).Location);
      }
    }
  }

  public void DestroyWorldBuildings(out HashSet<int> noRefundTiles)
  {
    this.TransferBuildingMaterials(out noRefundTiles);
    foreach (Component worldItem in Components.ClusterCraftInteriorDoors.GetWorldItems(this.id))
      TracesExtesions.DeleteObject(worldItem);
    this.ClearWorldZones();
  }

  public void TransferResourcesToParentWorld(Vector3 spawn_pos, HashSet<int> noRefundTiles)
  {
    this.TransferPickupables(spawn_pos);
    this.TransferLiquidsSolidsAndGases(spawn_pos, noRefundTiles);
  }

  public void TransferResourcesToDebris(
    AxialI sourceLocation,
    HashSet<int> noRefundTiles,
    SimHashes debrisContainerElement)
  {
    List<Storage> debrisObjects = new List<Storage>();
    this.TransferPickupablesToDebris(ref debrisObjects, debrisContainerElement);
    this.TransferLiquidsSolidsAndGasesToDebris(ref debrisObjects, noRefundTiles, debrisContainerElement);
    foreach (Component cmp in debrisObjects)
    {
      RailGunPayload.StatesInstance smi = cmp.GetSMI<RailGunPayload.StatesInstance>();
      smi.StartSM();
      smi.Travel(sourceLocation, ClusterUtil.ClosestVisibleAsteroidToLocation(sourceLocation).Location);
    }
  }

  private void TransferBuildingMaterials(out HashSet<int> noRefundTiles)
  {
    HashSet<int> retTemplateFoundationCells = new HashSet<int>();
    ListPool<ScenePartitionerEntry, ClusterManager>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ClusterManager>.Allocate();
    GameScenePartitioner.Instance.GatherEntries((int) this.minimumBounds.x, (int) this.minimumBounds.y, this.Width, this.Height, GameScenePartitioner.Instance.completeBuildings, (List<ScenePartitionerEntry>) gathered_entries);
    foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
    {
      BuildingComplete cmp = partitionerEntry.obj as BuildingComplete;
      if (Object.op_Inequality((Object) cmp, (Object) null))
      {
        Deconstructable component1 = ((Component) cmp).GetComponent<Deconstructable>();
        if (Object.op_Inequality((Object) component1, (Object) null) && !((Component) cmp).HasTag(GameTags.NoRocketRefund))
        {
          PrimaryElement component2 = ((Component) cmp).GetComponent<PrimaryElement>();
          float temperature = component2.Temperature;
          byte diseaseIdx = component2.DiseaseIdx;
          int diseaseCount = component2.DiseaseCount;
          for (int index1 = 0; index1 < component1.constructionElements.Length && cmp.Def.Mass.Length > index1; ++index1)
          {
            Element element = ElementLoader.GetElement(component1.constructionElements[index1]);
            if (element != null)
            {
              element.substance.SpawnResource(TransformExtensions.GetPosition(cmp.transform), cmp.Def.Mass[index1], temperature, diseaseIdx, diseaseCount);
            }
            else
            {
              GameObject prefab = Assets.GetPrefab(component1.constructionElements[index1]);
              for (int index2 = 0; (double) index2 < (double) cmp.Def.Mass[index1]; ++index2)
                GameUtil.KInstantiate(prefab, TransformExtensions.GetPosition(cmp.transform), Grid.SceneLayer.Ore).SetActive(true);
            }
          }
        }
        SimCellOccupier component3 = ((Component) cmp).GetComponent<SimCellOccupier>();
        if (Object.op_Inequality((Object) component3, (Object) null) && component3.doReplaceElement)
          cmp.RunOnArea((Action<int>) (cell => retTemplateFoundationCells.Add(cell)));
        Storage component4 = ((Component) cmp).GetComponent<Storage>();
        if (Object.op_Inequality((Object) component4, (Object) null))
          component4.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
        PlantablePlot component5 = ((Component) cmp).GetComponent<PlantablePlot>();
        if (Object.op_Inequality((Object) component5, (Object) null))
        {
          SeedProducer seedProducer = Object.op_Inequality((Object) component5.Occupant, (Object) null) ? component5.Occupant.GetComponent<SeedProducer>() : (SeedProducer) null;
          if (Object.op_Inequality((Object) seedProducer, (Object) null))
            seedProducer.DropSeed();
        }
        TracesExtesions.DeleteObject((Component) cmp);
      }
    }
    ((List<ScenePartitionerEntry>) gathered_entries).Clear();
    noRefundTiles = retTemplateFoundationCells;
  }

  private void TransferPickupables(Vector3 pos)
  {
    int cell = Grid.PosToCell(pos);
    ListPool<ScenePartitionerEntry, ClusterManager>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ClusterManager>.Allocate();
    GameScenePartitioner.Instance.GatherEntries((int) this.minimumBounds.x, (int) this.minimumBounds.y, this.Width, this.Height, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
    foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
    {
      if (partitionerEntry.obj != null)
      {
        Pickupable pickupable = partitionerEntry.obj as Pickupable;
        if (Object.op_Inequality((Object) pickupable, (Object) null))
          TransformExtensions.SetLocalPosition(((Component) pickupable).gameObject.transform, Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
      }
    }
    gathered_entries.Recycle();
  }

  private void TransferLiquidsSolidsAndGases(Vector3 pos, HashSet<int> noRefundTiles)
  {
    for (int x = (int) this.minimumBounds.x; (double) x <= (double) this.maximumBounds.x; ++x)
    {
      for (int y = (int) this.minimumBounds.y; (double) y <= (double) this.maximumBounds.y; ++y)
      {
        int cell = Grid.XYToCell(x, y);
        if (!noRefundTiles.Contains(cell))
        {
          Element element = Grid.Element[cell];
          if (element != null && !element.IsVacuum)
            element.substance.SpawnResource(pos, Grid.Mass[cell], Grid.Temperature[cell], Grid.DiseaseIdx[cell], Grid.DiseaseCount[cell]);
        }
      }
    }
  }

  private void TransferPickupablesToDebris(
    ref List<Storage> debrisObjects,
    SimHashes debrisContainerElement)
  {
    ListPool<ScenePartitionerEntry, ClusterManager>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ClusterManager>.Allocate();
    GameScenePartitioner.Instance.GatherEntries((int) this.minimumBounds.x, (int) this.minimumBounds.y, this.Width, this.Height, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
    foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
    {
      if (partitionerEntry.obj != null)
      {
        Pickupable cmp = partitionerEntry.obj as Pickupable;
        if (Object.op_Inequality((Object) cmp, (Object) null))
        {
          if (((Component) cmp).IsPrefabID(GameTags.Minion))
          {
            Util.KDestroyGameObject(((Component) cmp).gameObject);
          }
          else
          {
            cmp.PrimaryElement.Units = (float) Mathf.Max(1, Mathf.RoundToInt(cmp.PrimaryElement.Units * 0.5f));
            if ((debrisObjects.Count == 0 || (double) debrisObjects[debrisObjects.Count - 1].RemainingCapacity() == 0.0) && (double) cmp.PrimaryElement.Mass > 0.0)
              debrisObjects.Add(CraftModuleInterface.SpawnRocketDebris(" from World Objects", debrisContainerElement));
            Storage storage = debrisObjects[debrisObjects.Count - 1];
            while ((double) cmp.PrimaryElement.Mass > (double) storage.RemainingCapacity())
            {
              Pickupable pickupable = cmp.Take(storage.RemainingCapacity());
              storage.Store(((Component) pickupable).gameObject);
              storage = CraftModuleInterface.SpawnRocketDebris(" from World Objects", debrisContainerElement);
              debrisObjects.Add(storage);
            }
            if ((double) cmp.PrimaryElement.Mass > 0.0)
              storage.Store(((Component) cmp).gameObject);
          }
        }
      }
    }
    gathered_entries.Recycle();
  }

  private void TransferLiquidsSolidsAndGasesToDebris(
    ref List<Storage> debrisObjects,
    HashSet<int> noRefundTiles,
    SimHashes debrisContainerElement)
  {
    for (int x = (int) this.minimumBounds.x; (double) x <= (double) this.maximumBounds.x; ++x)
    {
      for (int y = (int) this.minimumBounds.y; (double) y <= (double) this.maximumBounds.y; ++y)
      {
        int cell = Grid.XYToCell(x, y);
        if (!noRefundTiles.Contains(cell))
        {
          Element element = Grid.Element[cell];
          if (element != null && !element.IsVacuum)
          {
            float num = Grid.Mass[cell] * 0.5f;
            if ((debrisObjects.Count == 0 || (double) debrisObjects[debrisObjects.Count - 1].RemainingCapacity() == 0.0) && (double) num > 0.0)
              debrisObjects.Add(CraftModuleInterface.SpawnRocketDebris(" from World Tiles", debrisContainerElement));
            Storage storage = debrisObjects[debrisObjects.Count - 1];
            while ((double) num > 0.0)
            {
              float mass = Mathf.Min(num, storage.RemainingCapacity());
              num -= mass;
              storage.AddOre(element.id, mass, Grid.Temperature[cell], Grid.DiseaseIdx[cell], Grid.DiseaseCount[cell]);
              if ((double) num > 0.0)
              {
                storage = CraftModuleInterface.SpawnRocketDebris(" from World Tiles", debrisContainerElement);
                debrisObjects.Add(storage);
              }
            }
          }
        }
      }
    }
  }

  public void CancelChores()
  {
    for (int layer = 0; layer < 44; ++layer)
    {
      for (int x = (int) this.minimumBounds.x; (double) x <= (double) this.maximumBounds.x; ++x)
      {
        for (int y = (int) this.minimumBounds.y; (double) y <= (double) this.maximumBounds.y; ++y)
        {
          int cell = Grid.XYToCell(x, y);
          GameObject gameObject = Grid.Objects[cell, layer];
          if (Object.op_Inequality((Object) gameObject, (Object) null))
            EventExtensions.Trigger(gameObject, 2127324410, (object) true);
        }
      }
    }
    List<Chore> choreList;
    GlobalChoreProvider.Instance.choreWorldMap.TryGetValue(this.id, out choreList);
    for (int index = 0; choreList != null && index < choreList.Count; ++index)
    {
      Chore chore = choreList[index];
      if (chore != null && chore.target != null && !chore.isNull)
        chore.Cancel("World destroyed");
    }
    List<FetchChore> fetchChoreList;
    GlobalChoreProvider.Instance.fetchMap.TryGetValue(this.id, out fetchChoreList);
    for (int index = 0; fetchChoreList != null && index < fetchChoreList.Count; ++index)
    {
      FetchChore fetchChore = fetchChoreList[index];
      if (fetchChore != null && fetchChore.target != null && !fetchChore.isNull)
        fetchChore.Cancel("World destroyed");
    }
  }

  public void ClearWorldZones()
  {
    if (this.overworldCell != null)
    {
      WorldDetailSave clusterDetailSave = SaveLoader.Instance.clusterDetailSave;
      int index1 = -1;
      for (int index2 = 0; index2 < SaveLoader.Instance.clusterDetailSave.overworldCells.Count; ++index2)
      {
        WorldDetailSave.OverworldCell overworldCell = SaveLoader.Instance.clusterDetailSave.overworldCells[index2];
        if (overworldCell.zoneType == this.overworldCell.zoneType && overworldCell.tags != null && this.overworldCell.tags != null && overworldCell.tags.ContainsAll(this.overworldCell.tags) && Rect.op_Equality(overworldCell.poly.bounds, this.overworldCell.poly.bounds))
        {
          index1 = index2;
          break;
        }
      }
      if (index1 >= 0)
        clusterDetailSave.overworldCells.RemoveAt(index1);
    }
    for (int y = (int) this.minimumBounds.y; (double) y <= (double) this.maximumBounds.y; ++y)
    {
      for (int x = (int) this.minimumBounds.x; (double) x <= (double) this.maximumBounds.x; ++x)
        SimMessages.ModifyCellWorldZone(Grid.XYToCell(x, y), byte.MaxValue);
    }
  }

  public int GetSafeCell()
  {
    if (this.IsModuleInterior)
    {
      foreach (RocketControlStation rocketControlStation in Components.RocketControlStations.Items)
      {
        if (rocketControlStation.GetMyWorldId() == this.id)
          return Grid.PosToCell((KMonoBehaviour) rocketControlStation);
      }
    }
    else
    {
      foreach (Telepad telepad in Components.Telepads.Items)
      {
        if (telepad.GetMyWorldId() == this.id)
          return Grid.PosToCell((KMonoBehaviour) telepad);
      }
    }
    return Grid.XYToCell(this.worldOffset.x + this.worldSize.x / 2, this.worldOffset.y + this.worldSize.y / 2);
  }

  public string GetStatus() => ColonyDiagnosticUtility.Instance.GetWorldDiagnosticResultStatus(this.id);
}
