// Decompiled with JetBrains decompiler
// Type: ProcGenGame.TerrainCell
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Delaunay.Geo;
using KSerialization;
using ProcGen;
using ProcGen.Map;
using System;
using System.Collections.Generic;
using UnityEngine;
using VoronoiTree;

namespace ProcGenGame
{
  [SerializationConfig]
  public class TerrainCell
  {
    private const float MASS_VARIATION = 0.2f;
    public List<KeyValuePair<int, Tag>> terrainPositions;
    public List<KeyValuePair<int, Tag>> poi;
    public List<int> neighbourTerrainCells = new List<int>();
    private float finalSize;
    private bool debugMode;
    private List<int> allCells;
    private HashSet<int> availableTerrainPoints;
    private HashSet<int> featureSpawnPoints;
    private HashSet<int> availableSpawnPoints;
    public const int DONT_SET_TEMPERATURE_DEFAULTS = -1;
    private static readonly Tag[] noPOISpawnTags = new Tag[5]
    {
      WorldGenTags.StartLocation,
      WorldGenTags.AtStart,
      WorldGenTags.NearStartLocation,
      WorldGenTags.POI,
      WorldGenTags.Feature
    };
    private static readonly TagSet noPOISpawnTagSet = new TagSet(TerrainCell.noPOISpawnTags);
    private static readonly Tag[] relaxedNoPOISpawnTags = new Tag[4]
    {
      WorldGenTags.StartLocation,
      WorldGenTags.AtStart,
      WorldGenTags.NearStartLocation,
      WorldGenTags.POI
    };
    private static readonly TagSet relaxedNoPOISpawnTagSet = new TagSet(TerrainCell.relaxedNoPOISpawnTags);

    public Polygon poly => this.site.poly;

    public Cell node { get; private set; }

    public Diagram.Site site { get; private set; }

    public Dictionary<Tag, int> distancesToTags { get; private set; }

    public bool HasMobs => this.mobs != null && this.mobs.Count > 0;

    public List<KeyValuePair<int, Tag>> mobs { get; private set; }

    protected TerrainCell()
    {
    }

    protected TerrainCell(Cell node, Diagram.Site site, Dictionary<Tag, int> distancesToTags)
    {
      this.node = node;
      this.site = site;
      this.distancesToTags = distancesToTags;
    }

    public virtual void LogInfo(string evt, string param, float value) => Debug.Log((object) (evt + ":" + param + "=" + value.ToString()));

    public void InitializeCells(HashSet<int> claimedCells)
    {
      if (this.allCells != null)
        return;
      this.allCells = new List<int>();
      this.availableTerrainPoints = new HashSet<int>();
      this.availableSpawnPoints = new HashSet<int>();
      for (int y = 0; y < Grid.HeightInCells; ++y)
      {
        for (int x = 0; x < Grid.WidthInCells; ++x)
        {
          if (this.poly.Contains(new Vector2((float) x, (float) y)))
          {
            int cell = Grid.XYToCell(x, y);
            this.availableTerrainPoints.Add(cell);
            this.availableSpawnPoints.Add(cell);
            if (claimedCells.Add(cell))
              this.allCells.Add(cell);
          }
        }
      }
      this.LogInfo("Initialise cells", "", (float) this.allCells.Count);
    }

    public List<int> GetAllCells() => new List<int>((IEnumerable<int>) this.allCells);

    public List<int> GetAvailableSpawnCellsAll()
    {
      List<int> availableSpawnCellsAll = new List<int>();
      foreach (int availableSpawnPoint in this.availableSpawnPoints)
        availableSpawnCellsAll.Add(availableSpawnPoint);
      return availableSpawnCellsAll;
    }

    public List<int> GetAvailableSpawnCellsFeature()
    {
      List<int> spawnCellsFeature = new List<int>();
      HashSet<int> intSet = new HashSet<int>((IEnumerable<int>) this.availableSpawnPoints);
      intSet.ExceptWith((IEnumerable<int>) this.availableTerrainPoints);
      foreach (int num in intSet)
        spawnCellsFeature.Add(num);
      return spawnCellsFeature;
    }

    public List<int> GetAvailableSpawnCellsBiome()
    {
      List<int> availableSpawnCellsBiome = new List<int>();
      HashSet<int> intSet = new HashSet<int>((IEnumerable<int>) this.availableSpawnPoints);
      intSet.ExceptWith((IEnumerable<int>) this.featureSpawnPoints);
      foreach (int num in intSet)
        availableSpawnCellsBiome.Add(num);
      return availableSpawnCellsBiome;
    }

    private bool RemoveFromAvailableSpawnCells(int cell) => this.availableSpawnPoints.Remove(cell);

    public void AddMobs(IEnumerable<KeyValuePair<int, Tag>> newMobs)
    {
      foreach (KeyValuePair<int, Tag> newMob in newMobs)
        this.AddMob(newMob);
    }

    private void AddMob(int cellIdx, string tag) => this.AddMob(new KeyValuePair<int, Tag>(cellIdx, new Tag(tag)));

    public void AddMob(KeyValuePair<int, Tag> mob)
    {
      if (this.mobs == null)
        this.mobs = new List<KeyValuePair<int, Tag>>();
      this.mobs.Add(mob);
      bool flag = this.RemoveFromAvailableSpawnCells(mob.Key);
      Tag tag = mob.Value;
      this.LogInfo("\t\t\tRemoveFromAvailableCells", ((Tag) ref tag).Name + ": " + (flag ? "success" : "failed"), (float) mob.Key);
      if (flag)
        return;
      if (!this.allCells.Contains(mob.Key))
      {
        string[] strArray = new string[5]
        {
          "Couldnt find cell [",
          mob.Key.ToString(),
          "] we dont own, to remove for mob [",
          null,
          null
        };
        tag = mob.Value;
        strArray[3] = ((Tag) ref tag).Name;
        strArray[4] = "]";
        Debug.Assert(false, (object) string.Concat(strArray));
      }
      else
      {
        string[] strArray = new string[5]
        {
          "Couldnt find cell [",
          mob.Key.ToString(),
          "] to remove for mob [",
          null,
          null
        };
        tag = mob.Value;
        strArray[3] = ((Tag) ref tag).Name;
        strArray[4] = "]";
        Debug.Assert(false, (object) string.Concat(strArray));
      }
    }

    protected string GetSubWorldType(WorldGen worldGen)
    {
      Vector2I pos;
      // ISSUE: explicit constructor call
      ((Vector2I) ref pos).\u002Ector((int) this.site.poly.Centroid().x, (int) this.site.poly.Centroid().y);
      return worldGen.GetSubWorldType(pos);
    }

    protected Temperature.Range GetTemperatureRange(WorldGen worldGen)
    {
      string subWorldType = this.GetSubWorldType(worldGen);
      if (subWorldType == null)
        return (Temperature.Range) 5;
      return !worldGen.Settings.HasSubworld(subWorldType) ? (Temperature.Range) 5 : worldGen.Settings.GetSubWorld(subWorldType).temperatureRange;
    }

    protected void GetTemperatureRange(WorldGen worldGen, ref float min, ref float range)
    {
      Temperature.Range temperatureRange = this.GetTemperatureRange(worldGen);
      min = SettingsCache.temperatures[temperatureRange].min;
      range = SettingsCache.temperatures[temperatureRange].max - min;
    }

    protected float GetDensityMassForCell(Chunk world, int cellIdx, float mass)
    {
      if (!Grid.IsValidCell(cellIdx))
        return 0.0f;
      Debug.Assert((double) world.density[cellIdx] >= 0.0 && (double) world.density[cellIdx] <= 1.0, (object) ("Density [" + world.density[cellIdx].ToString() + "] out of range [0-1]"));
      float num = (float) (0.20000000298023224 * ((double) world.density[cellIdx] - 0.5));
      float densityMassForCell = mass + mass * num;
      if ((double) densityMassForCell > 10000.0)
        densityMassForCell = 10000f;
      return densityMassForCell;
    }

    private void HandleSprinkleOfElement(
      WorldGenSettings settings,
      Tag targetTag,
      Chunk world,
      TerrainCell.SetValuesFunction SetValues,
      float temperatureMin,
      float temperatureRange,
      SeededRandom rnd)
    {
      Element elementByName = ElementLoader.FindElementByName(settings.GetFeature(((Tag) ref targetTag).Name).GetOneWeightedSimHash("SprinkleOfElementChoices", rnd).element);
      Room room = (Room) null;
      SettingsCache.rooms.TryGetValue(((Tag) ref targetTag).Name, ref room);
      SampleDescriber sampleDescriber = (SampleDescriber) room;
      Sim.PhysicsData defaultValues = elementByName.defaultValues;
      Sim.DiseaseCell invalid = Sim.DiseaseCell.Invalid;
      for (int index1 = 0; index1 < this.terrainPositions.Count; ++index1)
      {
        if (!Tag.op_Inequality(this.terrainPositions[index1].Value, targetTag))
        {
          SeededRandom seededRandom = rnd;
          MinMax blobSize = sampleDescriber.blobSize;
          double min = (double) ((MinMax) ref blobSize).min;
          blobSize = sampleDescriber.blobSize;
          double max = (double) ((MinMax) ref blobSize).max;
          float num = seededRandom.RandomRange((float) min, (float) max);
          List<Vector2I> filledCircle = Util.GetFilledCircle(Vector2.op_Implicit(Grid.CellToPos2D(this.terrainPositions[index1].Key)), num);
          for (int index2 = 0; index2 < filledCircle.Count; ++index2)
          {
            int cell = Grid.XYToCell(filledCircle[index2].x, filledCircle[index2].y);
            if (Grid.IsValidCell(cell))
            {
              defaultValues.mass = this.GetDensityMassForCell(world, cell, elementByName.defaultValues.mass);
              defaultValues.temperature = temperatureMin + world.heatOffset[cell] * temperatureRange;
              SetValues(cell, (object) elementByName, defaultValues, invalid);
            }
          }
        }
      }
    }

    private HashSet<Vector2I> DigFeature(
      Room.Shape shape,
      float size,
      List<int> bordersWidths,
      SeededRandom rnd,
      out List<Vector2I> featureCenterPoints,
      out List<List<Vector2I>> featureBorders)
    {
      HashSet<Vector2I> vector2ISet = new HashSet<Vector2I>();
      featureCenterPoints = new List<Vector2I>();
      featureBorders = new List<List<Vector2I>>();
      if ((double) size < 1.0)
        return vector2ISet;
      Vector2 vector2 = this.site.poly.Centroid();
      this.finalSize = size;
      switch ((int) shape)
      {
        case 0:
          featureCenterPoints = Util.GetFilledCircle(vector2, this.finalSize);
          break;
        case 2:
          featureCenterPoints = Util.GetBlob(vector2, this.finalSize, rnd.RandomSource());
          break;
        case 4:
          featureCenterPoints = Util.GetFilledRectangle(vector2, this.finalSize, this.finalSize, rnd, 2f, 2f);
          break;
        case 5:
          featureCenterPoints = Util.GetFilledRectangle(vector2, this.finalSize / 4f, this.finalSize, rnd, 2f, 2f);
          break;
        case 6:
          featureCenterPoints = Util.GetFilledRectangle(vector2, this.finalSize, this.finalSize / 4f, rnd, 2f, 2f);
          break;
        case 9:
          featureCenterPoints = Util.GetSplat(vector2, this.finalSize, rnd.RandomSource());
          break;
      }
      vector2ISet.UnionWith((IEnumerable<Vector2I>) featureCenterPoints);
      if (featureCenterPoints.Count == 0)
        Debug.LogWarning((object) ("Room has no centerpoints. Terrain Cell [ shape: " + shape.ToString() + " size: " + this.finalSize.ToString() + "] [" + this.node.NodeId.ToString() + " " + ((ProcGen.Node) this.node).type + " " + ((ProcGen.Node) this.node).position.ToString() + "]"));
      else if (bordersWidths != null && bordersWidths.Count > 0 && bordersWidths[0] > 0)
      {
        for (int index = 0; index < bordersWidths.Count && bordersWidths[index] > 0; ++index)
        {
          featureBorders.Add(Util.GetBorder(vector2ISet, bordersWidths[index]));
          vector2ISet.UnionWith((IEnumerable<Vector2I>) featureBorders[index]);
        }
      }
      return vector2ISet;
    }

    public static TerrainCell.ElementOverride GetElementOverride(
      string element,
      SampleDescriber.Override overrides)
    {
      Debug.Assert(element != null && element.Length > 0);
      TerrainCell.ElementOverride elementOverride = new TerrainCell.ElementOverride();
      elementOverride.element = ElementLoader.FindElementByName(element);
      Debug.Assert(elementOverride.element != null, (object) ("Couldn't find an element called " + element));
      elementOverride.pdelement = elementOverride.element.defaultValues;
      elementOverride.dc = Sim.DiseaseCell.Invalid;
      elementOverride.mass = elementOverride.element.defaultValues.mass;
      elementOverride.temperature = elementOverride.element.defaultValues.temperature;
      if (overrides == null)
        return elementOverride;
      elementOverride.overrideMass = false;
      elementOverride.overrideTemperature = false;
      elementOverride.overrideDiseaseIdx = false;
      elementOverride.overrideDiseaseAmount = false;
      float? nullable = overrides.massOverride;
      if (nullable.HasValue)
      {
        ref TerrainCell.ElementOverride local = ref elementOverride;
        nullable = overrides.massOverride;
        double num = (double) nullable.Value;
        local.mass = (float) num;
        elementOverride.overrideMass = true;
      }
      nullable = overrides.massMultiplier;
      if (nullable.HasValue)
      {
        ref float local = ref elementOverride.mass;
        double num1 = (double) local;
        nullable = overrides.massMultiplier;
        double num2 = (double) nullable.Value;
        local = (float) (num1 * num2);
        elementOverride.overrideMass = true;
      }
      nullable = overrides.temperatureOverride;
      if (nullable.HasValue)
      {
        ref TerrainCell.ElementOverride local = ref elementOverride;
        nullable = overrides.temperatureOverride;
        double num = (double) nullable.Value;
        local.temperature = (float) num;
        elementOverride.overrideTemperature = true;
      }
      nullable = overrides.temperatureMultiplier;
      if (nullable.HasValue)
      {
        ref float local = ref elementOverride.temperature;
        double num3 = (double) local;
        nullable = overrides.temperatureMultiplier;
        double num4 = (double) nullable.Value;
        local = (float) (num3 * num4);
        elementOverride.overrideTemperature = true;
      }
      if (overrides.diseaseOverride != null)
      {
        elementOverride.diseaseIdx = WorldGen.diseaseStats.GetIndex(HashedString.op_Implicit(overrides.diseaseOverride));
        elementOverride.overrideDiseaseIdx = true;
      }
      int? diseaseAmountOverride = overrides.diseaseAmountOverride;
      if (diseaseAmountOverride.HasValue)
      {
        ref TerrainCell.ElementOverride local = ref elementOverride;
        diseaseAmountOverride = overrides.diseaseAmountOverride;
        int num = diseaseAmountOverride.Value;
        local.diseaseAmount = num;
        elementOverride.overrideDiseaseAmount = true;
      }
      if (elementOverride.overrideTemperature)
        elementOverride.pdelement.temperature = elementOverride.temperature;
      if (elementOverride.overrideMass)
        elementOverride.pdelement.mass = elementOverride.mass;
      if (elementOverride.overrideDiseaseIdx)
        elementOverride.dc.diseaseIdx = elementOverride.diseaseIdx;
      if (elementOverride.overrideDiseaseAmount)
        elementOverride.dc.elementCount = elementOverride.diseaseAmount;
      return elementOverride;
    }

    private bool IsFeaturePointContainedInBorder(Vector2 point, WorldGen worldGen)
    {
      if (!((ProcGen.Node) this.node).tags.Contains(WorldGenTags.AllowExceedNodeBorders) || this.poly.Contains(point))
        return true;
      TerrainCell terrainCell = worldGen.TerrainCells.Find((Predicate<TerrainCell>) (x => x.poly.Contains(point)));
      return terrainCell == null || worldGen.Settings.GetSubWorld(((ProcGen.Node) this.node).GetSubworld()).zoneType == worldGen.Settings.GetSubWorld(((ProcGen.Node) terrainCell.node).GetSubworld()).zoneType;
    }

    private void ApplyPlaceElementForRoom(
      FeatureSettings feature,
      string group,
      List<Vector2I> cells,
      WorldGen worldGen,
      Chunk world,
      TerrainCell.SetValuesFunction SetValues,
      float temperatureMin,
      float temperatureRange,
      SeededRandom rnd,
      HashSet<int> highPriorityClaims)
    {
      if (cells == null || cells.Count == 0 || !feature.HasGroup(group))
        return;
      switch (feature.ElementChoiceGroups[group].selectionMethod - 3)
      {
        case 0:
        case 2:
          for (int index = 0; index < cells.Count; ++index)
          {
            int cell = Grid.XYToCell(cells[index].x, cells[index].y);
            if (Grid.IsValidCell(cell) && !highPriorityClaims.Contains(cell) && this.IsFeaturePointContainedInBorder(Vector2I.op_Implicit(cells[index]), worldGen))
            {
              WeightedSimHash oneWeightedSimHash = feature.GetOneWeightedSimHash(group, rnd);
              TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(oneWeightedSimHash.element, oneWeightedSimHash.overrides);
              if (!elementOverride.overrideTemperature)
                elementOverride.pdelement.temperature = temperatureMin + world.heatOffset[cell] * temperatureRange;
              if (!elementOverride.overrideMass)
                elementOverride.pdelement.mass = this.GetDensityMassForCell(world, cell, elementOverride.mass);
              SetValues(cell, (object) elementOverride.element, elementOverride.pdelement, elementOverride.dc);
            }
          }
          break;
        case 4:
          int num1 = int.MaxValue;
          int num2 = int.MinValue;
          for (int index = 0; index < cells.Count; ++index)
          {
            num1 = Mathf.Min(cells[index].y, num1);
            num2 = Mathf.Max(cells[index].y, num2);
          }
          int num3 = num2 - num1;
          for (int index = 0; index < cells.Count; ++index)
          {
            int cell = Grid.XYToCell(cells[index].x, cells[index].y);
            if (Grid.IsValidCell(cell) && !highPriorityClaims.Contains(cell) && this.IsFeaturePointContainedInBorder(Vector2I.op_Implicit(cells[index]), worldGen))
            {
              float num4 = (float) (1.0 - (double) (cells[index].y - num1) / (double) num3);
              WeightedSimHash weightedSimHashAtChoice = feature.GetWeightedSimHashAtChoice(group, num4);
              TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(weightedSimHashAtChoice.element, weightedSimHashAtChoice.overrides);
              if (!elementOverride.overrideTemperature)
                elementOverride.pdelement.temperature = temperatureMin + world.heatOffset[cell] * temperatureRange;
              if (!elementOverride.overrideMass)
                elementOverride.pdelement.mass = this.GetDensityMassForCell(world, cell, elementOverride.mass);
              SetValues(cell, (object) elementOverride.element, elementOverride.pdelement, elementOverride.dc);
            }
          }
          break;
        default:
          WeightedSimHash oneWeightedSimHash1 = feature.GetOneWeightedSimHash(group, rnd);
          for (int index = 0; index < cells.Count; ++index)
          {
            int cell = Grid.XYToCell(cells[index].x, cells[index].y);
            if (Grid.IsValidCell(cell) && !highPriorityClaims.Contains(cell) && this.IsFeaturePointContainedInBorder(Vector2I.op_Implicit(cells[index]), worldGen))
            {
              TerrainCell.ElementOverride elementOverride = TerrainCell.GetElementOverride(oneWeightedSimHash1.element, oneWeightedSimHash1.overrides);
              if (!elementOverride.overrideTemperature)
                elementOverride.pdelement.temperature = temperatureMin + world.heatOffset[cell] * temperatureRange;
              if (!elementOverride.overrideMass)
                elementOverride.pdelement.mass = this.GetDensityMassForCell(world, cell, elementOverride.mass);
              SetValues(cell, (object) elementOverride.element, elementOverride.pdelement, elementOverride.dc);
            }
          }
          break;
      }
    }

    private int GetIndexForLocation(List<Vector2I> points, Mob.Location location, SeededRandom rnd)
    {
      int index1 = -1;
      if (points == null || points.Count == 0)
        return index1;
      if (location == 2 || location == 6)
        return rnd.RandomRange(0, points.Count);
      for (int index2 = 0; index2 < points.Count; ++index2)
      {
        if (Grid.IsValidCell(Grid.XYToCell(points[index2].x, points[index2].y)))
        {
          if (index1 == -1)
            index1 = index2;
          else if (location != null)
          {
            if (location == 1 && points[index2].y > points[index1].y)
              index1 = index2;
          }
          else if (points[index2].y < points[index1].y)
            index1 = index2;
        }
      }
      return index1;
    }

    private void PlaceMobsInRoom(
      WorldGenSettings settings,
      List<MobReference> mobTags,
      List<Vector2I> points,
      SeededRandom rnd)
    {
      if (points == null)
        return;
      if (this.mobs == null)
        this.mobs = new List<KeyValuePair<int, Tag>>();
      for (int index1 = 0; index1 < mobTags.Count; ++index1)
      {
        if (!settings.HasMob(mobTags[index1].type))
        {
          Debug.LogError((object) ("Missing sample description for tag [" + mobTags[index1].type + "]"));
        }
        else
        {
          Mob mob = settings.GetMob(mobTags[index1].type);
          MinMax count = mobTags[index1].count;
          int num = Mathf.RoundToInt(((MinMax) ref count).GetRandomValueWithinRange(rnd));
          for (int index2 = 0; index2 < num; ++index2)
          {
            int indexForLocation = this.GetIndexForLocation(points, mob.location, rnd);
            if (indexForLocation != -1)
            {
              if (points.Count <= indexForLocation)
                return;
              int cell = Grid.XYToCell(points[indexForLocation].x, points[indexForLocation].y);
              points.RemoveAt(indexForLocation);
              this.AddMob(cell, mobTags[index1].type);
            }
            else
              break;
          }
        }
      }
    }

    private int[] ConvertNoiseToPoints(float[] basenoise, float minThreshold = 0.9f, float maxThreshold = 1f)
    {
      if (basenoise == null)
        return (int[]) null;
      List<int> intList = new List<int>();
      Rect bounds1 = this.site.poly.bounds;
      float width = ((Rect) ref bounds1).width;
      Rect bounds2 = this.site.poly.bounds;
      float height = ((Rect) ref bounds2).height;
      for (float num1 = this.site.position.y - height / 2f; (double) num1 < (double) this.site.position.y + (double) height / 2.0; ++num1)
      {
        for (float num2 = this.site.position.x - width / 2f; (double) num2 < (double) this.site.position.x + (double) width / 2.0; ++num2)
        {
          int cell = Grid.PosToCell(new Vector2(num2, num1));
          if (this.site.poly.Contains(new Vector2(num2, num1)))
          {
            float num3 = (float) (int) basenoise[cell];
            if ((double) num3 >= (double) minThreshold && (double) num3 <= (double) maxThreshold && !intList.Contains(cell))
              intList.Add(Grid.PosToCell(new Vector2(num2, num1)));
          }
        }
      }
      return intList.ToArray();
    }

    private void ApplyForeground(
      WorldGen worldGen,
      Chunk world,
      TerrainCell.SetValuesFunction SetValues,
      float temperatureMin,
      float temperatureRange,
      SeededRandom rnd)
    {
      bool flag = ((ProcGen.Node) this.node).tags != null;
      this.LogInfo("Apply foregreound", flag.ToString(), (float) (((ProcGen.Node) this.node).tags != null ? (double) ((ProcGen.Node) this.node).tags.Count : 0.0));
      if (((ProcGen.Node) this.node).tags == null)
        return;
      FeatureSettings feature = worldGen.Settings.TryGetFeature(((ProcGen.Node) this.node).type);
      flag = feature != null;
      this.LogInfo("\tFeature?", flag.ToString(), 0.0f);
      if (feature == null && ((ProcGen.Node) this.node).tags != null)
      {
        List<Tag> tagList = new List<Tag>();
        foreach (Tag tag in ((ProcGen.Node) this.node).tags)
        {
          if (worldGen.Settings.HasFeature(((Tag) ref tag).Name))
            tagList.Add(tag);
        }
        this.LogInfo("\tNo feature, checking possible feature tags, found", "", (float) tagList.Count);
        if (tagList.Count > 0)
        {
          Tag tag = tagList[rnd.RandomSource().Next(tagList.Count)];
          feature = worldGen.Settings.GetFeature(((Tag) ref tag).Name);
          this.LogInfo("\tPicked feature", ((Tag) ref tag).Name, 0.0f);
        }
      }
      if (feature == null)
        return;
      this.LogInfo("APPLY FOREGROUND", ((ProcGen.Node) this.node).type, 0.0f);
      MinMax blobSize = feature.blobSize;
      float size = ((MinMax) ref blobSize).GetRandomValueWithinRange(rnd);
      float closestEdge = this.poly.DistanceToClosestEdge(new Vector2?());
      if (!((ProcGen.Node) this.node).tags.Contains(WorldGenTags.AllowExceedNodeBorders) && (double) closestEdge < (double) size)
      {
        if (this.debugMode)
          Debug.LogWarning((object) (((ProcGen.Node) this.node).type + " " + feature.shape.ToString() + "  blob size too large to fit in node. Size reduced. " + size.ToString() + "->" + (closestEdge - 6f).ToString()));
        size = closestEdge - 6f;
      }
      if ((double) size <= 0.0)
        return;
      List<Vector2I> featureCenterPoints;
      List<List<Vector2I>> featureBorders;
      HashSet<Vector2I> vector2ISet = this.DigFeature(feature.shape, size, feature.borders, rnd, out featureCenterPoints, out featureBorders);
      this.featureSpawnPoints = new HashSet<int>();
      foreach (Vector2I vector2I in vector2ISet)
        this.featureSpawnPoints.Add(Grid.XYToCell(vector2I.x, vector2I.y));
      this.LogInfo("\t\t", "claimed points", (float) this.featureSpawnPoints.Count);
      this.availableTerrainPoints.ExceptWith((IEnumerable<int>) this.featureSpawnPoints);
      this.ApplyPlaceElementForRoom(feature, "RoomCenterElements", featureCenterPoints, worldGen, world, SetValues, temperatureMin, temperatureRange, rnd, worldGen.HighPriorityClaimedCells);
      if (featureBorders != null)
      {
        for (int index = 0; index < featureBorders.Count; ++index)
          this.ApplyPlaceElementForRoom(feature, "RoomBorderChoices" + index.ToString(), featureBorders[index], worldGen, world, SetValues, temperatureMin, temperatureRange, rnd, worldGen.HighPriorityClaimedCells);
      }
      List<string> tags = feature.tags;
      Tag highPriorityFeature = WorldGenTags.HighPriorityFeature;
      string name = ((Tag) ref highPriorityFeature).Name;
      if (!tags.Contains(name))
        return;
      worldGen.AddHighPriorityCells(this.featureSpawnPoints);
    }

    private void ApplyBackground(
      WorldGen worldGen,
      Chunk world,
      TerrainCell.SetValuesFunction SetValues,
      float temperatureMin,
      float temperatureRange,
      SeededRandom rnd)
    {
      this.LogInfo("Apply Background", ((ProcGen.Node) this.node).type, 0.0f);
      float floatSetting1 = worldGen.Settings.GetFloatSetting("CaveOverrideMaxValue");
      float floatSetting2 = worldGen.Settings.GetFloatSetting("CaveOverrideSliverValue");
      Leaf leafForTerrainCell = worldGen.GetLeafForTerrainCell(this);
      bool flag1 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.IgnoreCaveOverride);
      bool flag2 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.CaveVoidSliver);
      bool flag3 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.ErodePointToCentroid);
      bool flag4 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.ErodePointToCentroidInv);
      bool flag5 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.ErodePointToEdge);
      bool flag6 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.ErodePointToEdgeInv);
      bool flag7 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.ErodePointToBorder);
      bool flag8 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.ErodePointToBorderWeak);
      bool flag9 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.ErodePointToBorderInv);
      bool flag10 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.ErodePointToWorldTop);
      bool flag11 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.ErodePointToWorldTopOrSide);
      bool flag12 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.DistFunctionPointCentroid);
      bool flag13 = ((VoronoiTree.Node) leafForTerrainCell).tags.Contains(WorldGenTags.DistFunctionPointEdge);
      this.LogInfo("Getting Element Bands", ((ProcGen.Node) this.node).type, 0.0f);
      ElementBandConfiguration elementBandForBiome1 = worldGen.Settings.GetElementBandForBiome(((ProcGen.Node) this.node).type);
      if (elementBandForBiome1 == null && ((ProcGen.Node) this.node).biomeSpecificTags != null)
      {
        this.LogInfo("\tType is not a biome, checking tags", "", (float) ((ProcGen.Node) this.node).tags.Count);
        List<ElementBandConfiguration> bandConfigurationList = new List<ElementBandConfiguration>();
        foreach (Tag biomeSpecificTag in ((ProcGen.Node) this.node).biomeSpecificTags)
        {
          ElementBandConfiguration elementBandForBiome2 = worldGen.Settings.GetElementBandForBiome(((Tag) ref biomeSpecificTag).Name);
          if (elementBandForBiome2 != null)
          {
            bandConfigurationList.Add(elementBandForBiome2);
            this.LogInfo("\tFound biome", ((Tag) ref biomeSpecificTag).Name, 0.0f);
          }
        }
        if (bandConfigurationList.Count > 0)
        {
          int index = rnd.RandomSource().Next(bandConfigurationList.Count);
          elementBandForBiome1 = bandConfigurationList[index];
          this.LogInfo("\tPicked biome", "", (float) index);
        }
      }
      DebugUtil.Assert(elementBandForBiome1 != null, "A node didn't get assigned a biome! ", ((ProcGen.Node) this.node).type);
      foreach (int availableTerrainPoint in this.availableTerrainPoints)
      {
        Vector2I xy = Grid.CellToXY(availableTerrainPoint);
        if (!worldGen.HighPriorityClaimedCells.Contains(availableTerrainPoint))
        {
          float num1 = world.overrides[availableTerrainPoint];
          if (!flag1 && (double) num1 >= 100.0)
          {
            if ((double) num1 >= 300.0)
              SetValues(availableTerrainPoint, (object) WorldGen.voidElement, WorldGen.voidElement.defaultValues, Sim.DiseaseCell.Invalid);
            else if ((double) num1 >= 200.0)
              SetValues(availableTerrainPoint, (object) WorldGen.unobtaniumElement, WorldGen.unobtaniumElement.defaultValues, Sim.DiseaseCell.Invalid);
            else
              SetValues(availableTerrainPoint, (object) WorldGen.katairiteElement, WorldGen.katairiteElement.defaultValues, Sim.DiseaseCell.Invalid);
          }
          else
          {
            float erode = 1f;
            Vector2 vector2_1;
            // ISSUE: explicit constructor call
            ((Vector2) ref vector2_1).\u002Ector((float) xy.x, (float) xy.y);
            if (flag3 | flag4)
            {
              float num2 = 15f;
              if (flag13)
              {
                float num3 = 0.0f;
                MathUtil.Pair<Vector2, Vector2> closestEdge = this.poly.GetClosestEdge(vector2_1, ref num3);
                num2 = Vector2.Distance(Vector2.op_Addition(closestEdge.First, Vector2.op_Multiply(Vector2.op_Subtraction(closestEdge.Second, closestEdge.First), num3)), vector2_1);
              }
              erode = Mathf.Max(0.0f, Mathf.Min(1f, Vector2.Distance(this.poly.Centroid(), vector2_1) / num2));
              if (flag4)
                erode = 1f - erode;
            }
            if (flag6 | flag5)
            {
              float num4 = 0.0f;
              MathUtil.Pair<Vector2, Vector2> closestEdge = this.poly.GetClosestEdge(vector2_1, ref num4);
              Vector2 vector2_2 = Vector2.op_Addition(closestEdge.First, Vector2.op_Multiply(Vector2.op_Subtraction(closestEdge.Second, closestEdge.First), num4));
              float num5 = 15f;
              if (flag12)
                num5 = Vector2.Distance(this.poly.Centroid(), vector2_1);
              Vector2 vector2_3 = vector2_1;
              erode = Mathf.Max(0.0f, Mathf.Min(1f, Vector2.Distance(vector2_2, vector2_3) / num5));
              if (flag6)
                erode = 1f - erode;
            }
            if (flag9 | flag7)
            {
              List<Edge> edgesWithTag = worldGen.WorldLayout.overworldGraph.GetEdgesWithTag(WorldGenTags.EdgeClosed);
              float num6 = float.MaxValue;
              foreach (Edge edge in edgesWithTag)
              {
                MathUtil.Pair<Vector2, Vector2> pair = new MathUtil.Pair<Vector2, Vector2>(edge.corner0.position, edge.corner1.position);
                float num7 = 0.0f;
                Vector2 vector2_4 = vector2_1;
                ref float local = ref num7;
                num6 = Mathf.Min(Mathf.Abs(MathUtil.GetClosestPointBetweenPointAndLineSegment(pair, vector2_4, ref local)), num6);
              }
              float num8 = flag8 ? 7f : 20f;
              if (flag12)
                num8 = Vector2.Distance(this.poly.Centroid(), vector2_1);
              erode = Mathf.Max(0.0f, Mathf.Min(1f, num6 / num8));
              if (flag9)
                erode = 1f - erode;
            }
            if (flag10)
            {
              int y = worldGen.WorldSize.y;
              float num9 = 38f;
              float num10 = 58f;
              float num11 = (float) y - vector2_1.y;
              erode = (double) num11 >= (double) num9 ? ((double) num11 >= (double) num10 ? 1f : Mathf.Clamp01((float) (((double) num11 - (double) num9) / ((double) num10 - (double) num9)))) : 0.0f;
            }
            if (flag11)
            {
              int y = worldGen.WorldSize.y;
              int x = worldGen.WorldSize.x;
              float num12 = 2f;
              float num13 = 10f;
              float num14 = Mathf.Min(new float[3]
              {
                (float) y - vector2_1.y,
                vector2_1.x,
                (float) x - vector2_1.x
              });
              erode = (double) num14 >= (double) num12 ? ((double) num14 >= (double) num13 ? 1f : Mathf.Clamp01((float) (((double) num14 - (double) num12) / ((double) num13 - (double) num12)))) : 0.0f;
            }
            Element element;
            Sim.PhysicsData pd;
            Sim.DiseaseCell dc;
            worldGen.GetElementForBiomePoint(world, elementBandForBiome1, xy, out element, out pd, out dc, erode);
            pd.mass += (float) ((double) pd.mass * 0.20000000298023224 * ((double) world.density[xy.x + world.size.x * xy.y] - 0.5));
            if (!element.IsVacuum && element.id != SimHashes.Katairite && element.id != SimHashes.Unobtanium)
            {
              float num15 = temperatureMin;
              if (element.lowTempTransition != null && (double) temperatureMin < (double) element.lowTemp)
                num15 = element.lowTemp;
              pd.temperature = num15 + world.heatOffset[availableTerrainPoint] * temperatureRange;
            }
            if (element.IsSolid && !flag1 && (double) num1 > (double) floatSetting1 && (double) num1 < 100.0)
            {
              element = !flag2 || (double) num1 <= (double) floatSetting2 ? WorldGen.vacuumElement : WorldGen.voidElement;
              pd = element.defaultValues;
            }
            SetValues(availableTerrainPoint, (object) element, pd, dc);
          }
        }
      }
      if (((ProcGen.Node) this.node).tags.Contains(WorldGenTags.SprinkleOfOxyRock))
        this.HandleSprinkleOfElement(worldGen.Settings, WorldGenTags.SprinkleOfOxyRock, world, SetValues, temperatureMin, temperatureRange, rnd);
      if (!((ProcGen.Node) this.node).tags.Contains(WorldGenTags.SprinkleOfMetal))
        return;
      this.HandleSprinkleOfElement(worldGen.Settings, WorldGenTags.SprinkleOfMetal, world, SetValues, temperatureMin, temperatureRange, rnd);
    }

    private void GenerateActionCells(
      WorldGenSettings settings,
      Tag tag,
      HashSet<int> possiblePoints,
      SeededRandom rnd)
    {
      Room room = (Room) null;
      SettingsCache.rooms.TryGetValue(((Tag) ref tag).Name, ref room);
      SampleDescriber sampleDescriber = (SampleDescriber) room;
      if (sampleDescriber == null && settings.HasMob(((Tag) ref tag).Name))
        sampleDescriber = (SampleDescriber) settings.GetMob(((Tag) ref tag).Name);
      if (sampleDescriber == null)
        return;
      HashSet<int> intSet = new HashSet<int>();
      MinMax density = sampleDescriber.density;
      float valueWithinRange = ((MinMax) ref density).GetRandomValueWithinRange(rnd);
      SampleDescriber.PointSelectionMethod selectMethod = sampleDescriber.selectMethod;
      List<Vector2> vector2List;
      if (selectMethod != null)
      {
        if (selectMethod == 1)
          ;
        vector2List = new List<Vector2>();
        vector2List.Add(((ProcGen.Node) this.node).position);
      }
      else
        vector2List = PointGenerator.GetRandomPoints(this.poly, valueWithinRange, 0.0f, (List<Vector2>) null, sampleDescriber.sampleBehaviour, true, rnd, true, true);
      foreach (Vector2 vector2 in vector2List)
      {
        int cell = Grid.XYToCell((int) vector2.x, (int) vector2.y);
        if (possiblePoints.Contains(cell))
          intSet.Add(cell);
      }
      if (room == null || room.mobselection != null)
        return;
      if (this.terrainPositions == null)
        this.terrainPositions = new List<KeyValuePair<int, Tag>>();
      foreach (int num in intSet)
      {
        if (Grid.IsValidCell(num))
          this.terrainPositions.Add(new KeyValuePair<int, Tag>(num, tag));
      }
    }

    private void DoProcess(
      WorldGen worldGen,
      Chunk world,
      TerrainCell.SetValuesFunction SetValues,
      SeededRandom rnd)
    {
      float min = 265f;
      float range = 30f;
      this.InitializeCells(worldGen.ClaimedCells);
      this.GetTemperatureRange(worldGen, ref min, ref range);
      this.ApplyForeground(worldGen, world, SetValues, min, range, rnd);
      for (int index = 0; index < ((ProcGen.Node) this.node).tags.Count; ++index)
        this.GenerateActionCells(worldGen.Settings, ((ProcGen.Node) this.node).tags[index], this.availableTerrainPoints, rnd);
      this.ApplyBackground(worldGen, world, SetValues, min, range, rnd);
    }

    public void Process(
      WorldGen worldGen,
      Sim.Cell[] cells,
      float[] bgTemp,
      Sim.DiseaseCell[] dcs,
      Chunk world,
      SeededRandom rnd)
    {
      TerrainCell.SetValuesFunction SetValues = (TerrainCell.SetValuesFunction) ((index, elem, pd, dc) =>
      {
        if (Grid.IsValidCell(index))
        {
          if ((double) pd.temperature == 0.0 || (elem as Element).HasTag(GameTags.Special))
            bgTemp[index] = -1f;
          cells[index].SetValues(elem as Element, pd, ElementLoader.elements);
          dcs[index] = dc;
        }
        else
          Debug.LogError((object) ("Process::SetValuesFunction Index [" + index.ToString() + "] is not valid. cells.Length [" + cells.Length.ToString() + "]"));
      });
      this.DoProcess(worldGen, world, SetValues, rnd);
    }

    public void Process(WorldGen worldGen, Chunk world, SeededRandom rnd)
    {
      TerrainCell.SetValuesFunction SetValues = (TerrainCell.SetValuesFunction) ((index, elem, pd, dc) => SimMessages.ModifyCell(index, (elem as Element).idx, pd.temperature, pd.mass, dc.diseaseIdx, dc.elementCount, SimMessages.ReplaceType.Replace));
      this.DoProcess(worldGen, world, SetValues, rnd);
    }

    public int DistanceToTag(Tag tag)
    {
      int tag1;
      if (!this.distancesToTags.TryGetValue(tag, out tag1))
        DebugUtil.DevLogError(string.Format("DistanceToTag could not find tag '{0}', did forget to include a start template?", (object) tag));
      return tag1;
    }

    public bool IsSafeToSpawnPOI(List<TerrainCell> allCells, bool log = true) => this.IsSafeToSpawnPOI(allCells, TerrainCell.noPOISpawnTags, TerrainCell.noPOISpawnTagSet, log);

    public bool IsSafeToSpawnPOIRelaxed(List<TerrainCell> allCells, bool log = true) => this.IsSafeToSpawnPOI(allCells, TerrainCell.relaxedNoPOISpawnTags, TerrainCell.relaxedNoPOISpawnTagSet, log);

    private bool IsSafeToSpawnPOI(
      List<TerrainCell> allCells,
      Tag[] noSpawnTags,
      TagSet noSpawnTagSet,
      bool log)
    {
      return !((ProcGen.Node) this.node).tags.ContainsOne(noSpawnTagSet);
    }

    public delegate void SetValuesFunction(
      int index,
      object elem,
      Sim.PhysicsData pd,
      Sim.DiseaseCell dc);

    public struct ElementOverride
    {
      public Element element;
      public Sim.PhysicsData pdelement;
      public Sim.DiseaseCell dc;
      public float mass;
      public float temperature;
      public byte diseaseIdx;
      public int diseaseAmount;
      public bool overrideMass;
      public bool overrideTemperature;
      public bool overrideDiseaseIdx;
      public bool overrideDiseaseAmount;
    }
  }
}
