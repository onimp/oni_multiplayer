// Decompiled with JetBrains decompiler
// Type: ProcGen.WorldLayout
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Delaunay.Geo;
using KSerialization;
using ObjectCloner;
using ProcGen.Map;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using VoronoiTree;

namespace ProcGen
{
  [SerializationConfig]
  public class WorldLayout
  {
    private Tree voronoiTree;
    [Serialize]
    public MapGraph localGraph;
    [Serialize]
    public MapGraph overworldGraph;
    [EnumFlags]
    public static WorldLayout.DebugFlags drawOptions;
    private LineSegment topEdge;
    private LineSegment bottomEdge;
    private LineSegment leftEdge;
    private LineSegment rightEdge;
    private SeededRandom myRandom;
    [Serialize]
    private WorldLayout.ExtraIO extra;

    [Serialize]
    public int mapWidth { get; private set; }

    [Serialize]
    public int mapHeight { get; private set; }

    public bool layoutOK { get; private set; }

    public static LevelLayer levelLayerGradient { get; private set; }

    public WorldGen worldGen { get; private set; }

    public WorldLayout(WorldGen worldGen, int seed)
    {
      this.worldGen = worldGen;
      this.localGraph = new MapGraph(seed);
      this.overworldGraph = new MapGraph(seed);
      this.SetSeed(seed);
    }

    public WorldLayout(WorldGen worldGen, int width, int height, int seed)
      : this(worldGen, seed)
    {
      this.mapWidth = width;
      this.mapHeight = height;
    }

    public void SetSeed(int seed)
    {
      this.myRandom = new SeededRandom(seed);
      ((Graph<Cell, Edge>) this.localGraph).SetSeed(seed);
      ((Graph<Cell, Edge>) this.overworldGraph).SetSeed(seed);
    }

    public Tree GetVoronoiTree() => this.voronoiTree;

    public static void SetLayerGradient(LevelLayer newGradient) => WorldLayout.levelLayerGradient = newGradient;

    public static string GetNodeTypeFromLayers(Vector2 point, float mapHeight, SeededRandom rnd)
    {
      Tag theVoid = WorldGenTags.TheVoid;
      string name = ((Tag) ref theVoid).Name;
      string nodeTypeFromLayers = ((Gradient<List<string>>) ((List<LayerGradient>) WorldLayout.levelLayerGradient)[((List<LayerGradient>) WorldLayout.levelLayerGradient).Count - 1]).content[rnd.RandomRange(0, ((Gradient<List<string>>) ((List<LayerGradient>) WorldLayout.levelLayerGradient)[((List<LayerGradient>) WorldLayout.levelLayerGradient).Count - 1]).content.Count)];
      for (int index1 = 0; index1 < ((List<LayerGradient>) WorldLayout.levelLayerGradient).Count; ++index1)
      {
        if ((double) point.y < (double) ((Gradient<List<string>>) ((List<LayerGradient>) WorldLayout.levelLayerGradient)[index1]).maxValue * (double) mapHeight)
        {
          int index2 = rnd.RandomRange(0, ((Gradient<List<string>>) ((List<LayerGradient>) WorldLayout.levelLayerGradient)[index1]).content.Count);
          nodeTypeFromLayers = ((Gradient<List<string>>) ((List<LayerGradient>) WorldLayout.levelLayerGradient)[index1]).content[index2];
          break;
        }
      }
      return nodeTypeFromLayers;
    }

    public Tree GenerateOverworld(bool usePD, bool isRunningDebugGen)
    {
      Debug.Assert(this.mapWidth != 0 && this.mapHeight != 0, (object) "Map size has not been set");
      Debug.Assert(this.worldGen.Settings.world != null, (object) "You need to set a world");
      Diagram.Site site = new Diagram.Site(0U, new Vector2((float) (this.mapWidth / 2), (float) (this.mapHeight / 2)), 1f);
      this.topEdge = new LineSegment(new Vector2?(new Vector2(0.0f, (float) (this.mapHeight - 5))), new Vector2?(new Vector2((float) this.mapWidth, (float) (this.mapHeight - 5))));
      this.bottomEdge = new LineSegment(new Vector2?(new Vector2(0.0f, 5f)), new Vector2?(new Vector2((float) this.mapWidth, 5f)));
      this.leftEdge = new LineSegment(new Vector2?(new Vector2(5f, 0.0f)), new Vector2?(new Vector2(5f, (float) this.mapHeight)));
      this.rightEdge = new LineSegment(new Vector2?(new Vector2((float) (this.mapWidth - 5), 0.0f)), new Vector2?(new Vector2((float) (this.mapWidth - 5), (float) this.mapHeight)));
      site.poly = new Polygon(new Rect(0.0f, 0.0f, (float) this.mapWidth, (float) this.mapHeight));
      this.voronoiTree = new Tree(site, (Tree) null, this.myRandom.seed);
      VoronoiTree.Node.maxIndex = 0U;
      float num1 = this.myRandom.RandomRange(this.worldGen.Settings.GetFloatSetting("OverworldDensityMin"), this.worldGen.Settings.GetFloatSetting("OverworldDensityMax"));
      float floatSetting = this.worldGen.Settings.GetFloatSetting("OverworldAvoidRadius");
      PointGenerator.SampleBehaviour enumSetting = this.worldGen.Settings.GetEnumSetting<PointGenerator.SampleBehaviour>("OverworldSampleBehaviour");
      Cell cell1 = (Cell) null;
      if (!string.IsNullOrEmpty(this.worldGen.Settings.world.startSubworldName))
      {
        WeightedSubworldName weightedSubworldName = this.worldGen.Settings.world.subworldFiles.Find((Predicate<WeightedSubworldName>) (x => x.name == this.worldGen.Settings.world.startSubworldName));
        Debug.Assert(weightedSubworldName != null, (object) "The start subworld must be listed in the subworld files for a world.");
        Vector2 vector2;
        ref Vector2 local = ref vector2;
        double mapWidth = (double) this.mapWidth;
        MinMax positionHorizontal = this.worldGen.Settings.world.startingBasePositionHorizontal;
        double valueWithinRange1 = (double) ((MinMax) ref positionHorizontal).GetRandomValueWithinRange(this.myRandom);
        double num2 = mapWidth * valueWithinRange1;
        double mapHeight = (double) this.mapHeight;
        MinMax positionVertical = this.worldGen.Settings.world.startingBasePositionVertical;
        double valueWithinRange2 = (double) ((MinMax) ref positionVertical).GetRandomValueWithinRange(this.myRandom);
        double num3 = mapHeight * valueWithinRange2;
        // ISSUE: explicit constructor call
        ((Vector2) ref local).\u002Ector((float) num2, (float) num3);
        cell1 = ((Graph<Cell, Edge>) this.overworldGraph).AddNode(weightedSubworldName.name, vector2);
        SubWorld subWorld = this.worldGen.Settings.GetSubWorld(weightedSubworldName.name);
        float overridePower = (double) weightedSubworldName.overridePower > 0.0 ? weightedSubworldName.overridePower : subWorld.pdWeight;
        VoronoiTree.Node vn = this.voronoiTree.AddSite(new Diagram.Site((uint) cell1.NodeId, ((Node) cell1).position, overridePower), (VoronoiTree.Node.NodeType) 1);
        vn.AddTag(WorldGenTags.AtStart);
        this.ApplySubworldToNode(vn, subWorld, overridePower);
      }
      List<Vector2> vector2List = new List<Vector2>();
      if (cell1 != null)
        vector2List.Add(((Node) cell1).position);
      List<Vector2> randomPoints = PointGenerator.GetRandomPoints(site.poly, num1, floatSetting, vector2List, enumSetting, false, this.myRandom, false, true);
      int intSetting1 = this.worldGen.Settings.GetIntSetting("OverworldMinNodes");
      int intSetting2 = this.worldGen.Settings.GetIntSetting("OverworldMaxNodes");
      if (randomPoints.Count > intSetting2)
      {
        WorldGenUtil.ShuffleSeeded<Vector2>((IList<Vector2>) randomPoints, this.myRandom.RandomSource());
        randomPoints.RemoveRange(intSetting2, randomPoints.Count - intSetting2);
      }
      if (randomPoints.Count < intSetting1)
        throw new Exception(string.Format("World layout with fewer than {0} points.", (object) intSetting1));
      for (int index = 0; index < randomPoints.Count; ++index)
      {
        MapGraph overworldGraph = this.overworldGraph;
        Tag unassignedNode = WorldGenTags.UnassignedNode;
        string name = ((Tag) ref unassignedNode).Name;
        Vector2 vector2 = randomPoints[index];
        Cell cell2 = ((Graph<Cell, Edge>) overworldGraph).AddNode(name, vector2);
        this.voronoiTree.AddSite(new Diagram.Site((uint) cell2.NodeId, ((Node) cell2).position, 1f), (VoronoiTree.Node.NodeType) 1).tags.Add(WorldGenTags.UnassignedNode);
        ((Node) cell2).tags.Add(WorldGenTags.UnassignedNode);
      }
      List<Diagram.Site> siteList = new List<Diagram.Site>();
      for (int index = 0; index < this.voronoiTree.ChildCount(); ++index)
        siteList.Add(this.voronoiTree.GetChild(index).site);
      if (usePD)
      {
        ((VoronoiTree.Node) this.voronoiTree).ComputeNode(siteList);
        ((VoronoiTree.Node) this.voronoiTree).ComputeNodePD(siteList, 500, 0.2f);
      }
      else
        this.voronoiTree.ComputeChildren(this.myRandom.seed + 1, false, false);
      this.voronoiTree.VisitAll((Action<VoronoiTree.Node>) (n => Debug.Assert(n.site.poly != null, (object) string.Format("Node {0} had a null poly after initial overworld compute!!", (object) n.site.id))));
      this.voronoiTree.AddTagToChildren(WorldGenTags.Overworld);
      this.TagTopAndBottomSites(WorldGenTags.AtSurface, WorldGenTags.AtDepths);
      this.TagEdgeSites(WorldGenTags.AtEdge, WorldGenTags.AtEdge);
      WorldLayout.ResetMapGraphFromVoronoiTree(this.voronoiTree.ImmediateChildren(), this.overworldGraph, true);
      this.PropagateDistanceTags(this.voronoiTree, WorldGenTags.DistanceTags);
      this.ConvertUnknownCells(this.myRandom, isRunningDebugGen);
      if (this.worldGen.Settings.GetOverworldAddTags() != null)
      {
        foreach (string overworldAddTag in this.worldGen.Settings.GetOverworldAddTags())
          this.voronoiTree.GetChild(this.myRandom.RandomSource().Next(this.voronoiTree.ChildCount())).AddTag(new Tag(overworldAddTag));
      }
      if (usePD)
        ((VoronoiTree.Node) this.voronoiTree).ComputeNodePD(siteList, 500, 0.2f);
      this.voronoiTree.VisitAll((Action<VoronoiTree.Node>) (n => Debug.Assert(n.site.poly != null, (object) string.Format("Node {0} had a null poly after final overworld compute!!", (object) n.site.id))));
      this.FlattenOverworld();
      return this.voronoiTree;
    }

    public static void ResetMapGraphFromVoronoiTree(List<VoronoiTree.Node> nodes, MapGraph graph, bool clear)
    {
      if (clear)
        graph.ClearEdgesAndCorners();
      for (int index = 0; index < nodes.Count; ++index)
      {
        VoronoiTree.Node node = nodes[index];
        Cell nodeById1 = ((Graph<Cell, Edge>) graph).FindNodeByID(node.site.id);
        ((Node) nodeById1).tags.Union(node.tags);
        ((Node) nodeById1).SetPosition(node.site.position);
        foreach (VoronoiTree.Node neighbor in node.GetNeighbors())
        {
          Cell nodeById2 = ((Graph<Cell, Edge>) graph).FindNodeByID(neighbor.site.id);
          if (((Graph<Cell, Edge>) graph).GetArc(nodeById1, nodeById2) == null)
          {
            int num = -1;
            LineSegment lineSegment;
            if (node.site.poly.SharesEdge(neighbor.site.poly, ref num, ref lineSegment) == 2)
            {
              Corner corner1 = graph.AddOrGetCorner(lineSegment.p0.Value);
              Corner corner2 = graph.AddOrGetCorner(lineSegment.p1.Value);
              graph.AddOrGetEdge(nodeById1, nodeById2, corner1, corner2);
            }
          }
        }
      }
    }

    public void PopulateSubworlds()
    {
      this.AddSubworldChildren();
      this.GetStartLocation();
      this.PropagateStartTag();
    }

    private void PropagateDistanceTags(Tree tree, TagSet tags)
    {
      foreach (Tag tag in tags)
      {
        Dictionary<uint, int> distanceToTag = ((Graph<Cell, Edge>) this.overworldGraph).GetDistanceToTag(tag);
        if (distanceToTag != null)
        {
          int num = 0;
          for (int index = 0; index < tree.ChildCount(); ++index)
          {
            VoronoiTree.Node child = tree.GetChild(index);
            uint id = child.site.id;
            if (distanceToTag.ContainsKey(id))
            {
              child.minDistanceToTag.Add(tag, distanceToTag[id]);
              ++num;
              if (distanceToTag[id] > 0)
                child.AddTag(new Tag(((Tag) ref tag).Name + "_Distance" + distanceToTag[id].ToString()));
            }
          }
        }
      }
    }

    private HashSet<WeightedSubWorld> GetNameFilterSet(
      VoronoiTree.Node vn,
      World.AllowedCellsFilter filter,
      List<WeightedSubWorld> subworlds)
    {
      HashSet<WeightedSubWorld> nameFilterSet = new HashSet<WeightedSubWorld>();
      switch ((int) filter.tagcommand)
      {
        case 0:
          for (int i = 0; i < filter.subworldNames.Count; i++)
            nameFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworlds.FindAll((Predicate<WeightedSubWorld>) (f => ((SampleDescriber) f.subWorld).name == filter.subworldNames[i])));
          break;
        case 1:
          if (vn.tags.Contains(Tag.op_Implicit(filter.tag)))
          {
            for (int i = 0; i < filter.subworldNames.Count; i++)
              nameFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworlds.FindAll((Predicate<WeightedSubWorld>) (f => ((SampleDescriber) f.subWorld).name == filter.subworldNames[i])));
            break;
          }
          break;
        case 2:
          if (!vn.tags.Contains(Tag.op_Implicit(filter.tag)))
          {
            for (int i = 0; i < filter.subworldNames.Count; i++)
              nameFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworlds.FindAll((Predicate<WeightedSubWorld>) (f => ((SampleDescriber) f.subWorld).name == filter.subworldNames[i])));
            break;
          }
          break;
        case 3:
          int num = vn.minDistanceToTag.ContainsKey(TagExtensions.ToTag(filter.tag)) ? 1 : 0;
          Debug.Assert(num != 0 || filter.optional, (object) ("DistanceFromTag is missing tag " + filter.tag + ", consider marking the filter optional."));
          if (num != 0 && vn.minDistanceToTag[TagExtensions.ToTag(filter.tag)] >= filter.minDistance && vn.minDistanceToTag[TagExtensions.ToTag(filter.tag)] <= filter.maxDistance)
          {
            for (int i = 0; i < filter.subworldNames.Count; i++)
              nameFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworlds.FindAll((Predicate<WeightedSubWorld>) (f => ((SampleDescriber) f.subWorld).name == filter.subworldNames[i])));
            break;
          }
          break;
      }
      return nameFilterSet;
    }

    private HashSet<WeightedSubWorld> GetZoneTypeFilterSet(
      VoronoiTree.Node vn,
      World.AllowedCellsFilter filter,
      Dictionary<string, List<WeightedSubWorld>> subworldsByZoneType)
    {
      HashSet<WeightedSubWorld> zoneTypeFilterSet = new HashSet<WeightedSubWorld>();
      switch ((int) filter.tagcommand)
      {
        case 0:
          for (int index = 0; index < filter.zoneTypes.Count; ++index)
            zoneTypeFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworldsByZoneType[filter.zoneTypes[index].ToString()]);
          break;
        case 1:
          if (vn.tags.Contains(Tag.op_Implicit(filter.tag)))
          {
            for (int index = 0; index < filter.zoneTypes.Count; ++index)
              zoneTypeFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworldsByZoneType[filter.zoneTypes[index].ToString()]);
            break;
          }
          break;
        case 2:
          if (!vn.tags.Contains(Tag.op_Implicit(filter.tag)))
          {
            for (int index = 0; index < filter.zoneTypes.Count; ++index)
              zoneTypeFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworldsByZoneType[filter.zoneTypes[index].ToString()]);
            break;
          }
          break;
        case 3:
          Debug.Assert(vn.minDistanceToTag.ContainsKey(TagExtensions.ToTag(filter.tag)), (object) filter.tag);
          if (vn.minDistanceToTag[TagExtensions.ToTag(filter.tag)] >= filter.minDistance && vn.minDistanceToTag[TagExtensions.ToTag(filter.tag)] <= filter.maxDistance)
          {
            for (int index = 0; index < filter.zoneTypes.Count; ++index)
              zoneTypeFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworldsByZoneType[filter.zoneTypes[index].ToString()]);
            break;
          }
          break;
      }
      return zoneTypeFilterSet;
    }

    private HashSet<WeightedSubWorld> GetTemperatureFilterSet(
      VoronoiTree.Node vn,
      World.AllowedCellsFilter filter,
      Dictionary<string, List<WeightedSubWorld>> subworldsByTemperature)
    {
      HashSet<WeightedSubWorld> temperatureFilterSet = new HashSet<WeightedSubWorld>();
      switch ((int) filter.tagcommand)
      {
        case 0:
          for (int index = 0; index < filter.temperatureRanges.Count; ++index)
            temperatureFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworldsByTemperature[filter.temperatureRanges[index].ToString()]);
          break;
        case 1:
          if (vn.tags.Contains(Tag.op_Implicit(filter.tag)))
          {
            for (int index = 0; index < filter.temperatureRanges.Count; ++index)
              temperatureFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworldsByTemperature[filter.temperatureRanges[index].ToString()]);
            break;
          }
          break;
        case 2:
          if (!vn.tags.Contains(Tag.op_Implicit(filter.tag)))
          {
            for (int index = 0; index < filter.temperatureRanges.Count; ++index)
              temperatureFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworldsByTemperature[filter.temperatureRanges[index].ToString()]);
            break;
          }
          break;
        case 3:
          Debug.Assert(vn.minDistanceToTag.ContainsKey(TagExtensions.ToTag(filter.tag)), (object) filter.tag);
          if (vn.minDistanceToTag[TagExtensions.ToTag(filter.tag)] >= filter.minDistance && vn.minDistanceToTag[TagExtensions.ToTag(filter.tag)] <= filter.maxDistance)
          {
            for (int index = 0; index < filter.temperatureRanges.Count; ++index)
              temperatureFilterSet.UnionWith((IEnumerable<WeightedSubWorld>) subworldsByTemperature[filter.temperatureRanges[index].ToString()]);
            break;
          }
          break;
      }
      return temperatureFilterSet;
    }

    private void RunFilterClearCommand(
      VoronoiTree.Node vn,
      World.AllowedCellsFilter filter,
      HashSet<WeightedSubWorld> allowedSubworldsSet)
    {
      switch ((int) filter.tagcommand)
      {
        case 0:
          allowedSubworldsSet.Clear();
          break;
        case 1:
          if (!vn.tags.Contains(Tag.op_Implicit(filter.tag)))
            break;
          allowedSubworldsSet.Clear();
          break;
        case 2:
          if (vn.tags.Contains(Tag.op_Implicit(filter.tag)))
            break;
          allowedSubworldsSet.Clear();
          break;
        case 3:
          Debug.Assert(vn.minDistanceToTag.ContainsKey(TagExtensions.ToTag(filter.tag)), (object) filter.tag);
          if (vn.minDistanceToTag[TagExtensions.ToTag(filter.tag)] < filter.minDistance || vn.minDistanceToTag[TagExtensions.ToTag(filter.tag)] > filter.maxDistance)
            break;
          allowedSubworldsSet.Clear();
          break;
      }
    }

    private HashSet<WeightedSubWorld> Filter(
      VoronoiTree.Node vn,
      List<WeightedSubWorld> allSubWorlds,
      Dictionary<string, List<WeightedSubWorld>> subworldsByTemperature,
      Dictionary<string, List<WeightedSubWorld>> subworldsByZoneType)
    {
      HashSet<WeightedSubWorld> allowedSubworldsSet = new HashSet<WeightedSubWorld>();
      World world = this.worldGen.Settings.world;
      string str = "";
      foreach (KeyValuePair<Tag, int> keyValuePair in vn.minDistanceToTag)
      {
        string[] strArray = new string[5]
        {
          str,
          null,
          null,
          null,
          null
        };
        Tag key = keyValuePair.Key;
        strArray[1] = ((Tag) ref key).Name;
        strArray[2] = ":";
        strArray[3] = keyValuePair.Value.ToString();
        strArray[4] = ", ";
        str = string.Concat(strArray);
      }
      foreach (World.AllowedCellsFilter cellsAllowedSubworld in world.unknownCellsAllowedSubworlds)
      {
        HashSet<WeightedSubWorld> other = new HashSet<WeightedSubWorld>();
        if (cellsAllowedSubworld.subworldNames != null && cellsAllowedSubworld.subworldNames.Count > 0)
          other.UnionWith((IEnumerable<WeightedSubWorld>) this.GetNameFilterSet(vn, cellsAllowedSubworld, allSubWorlds));
        if (cellsAllowedSubworld.temperatureRanges != null && cellsAllowedSubworld.temperatureRanges.Count > 0)
          other.UnionWith((IEnumerable<WeightedSubWorld>) this.GetTemperatureFilterSet(vn, cellsAllowedSubworld, subworldsByTemperature));
        if (cellsAllowedSubworld.zoneTypes != null && cellsAllowedSubworld.zoneTypes.Count > 0)
          other.UnionWith((IEnumerable<WeightedSubWorld>) this.GetZoneTypeFilterSet(vn, cellsAllowedSubworld, subworldsByZoneType));
        switch ((int) cellsAllowedSubworld.command)
        {
          case 0:
            this.RunFilterClearCommand(vn, cellsAllowedSubworld, allowedSubworldsSet);
            continue;
          case 1:
            if (other.Count > 0)
            {
              allowedSubworldsSet.Clear();
              allowedSubworldsSet.UnionWith((IEnumerable<WeightedSubWorld>) other);
              continue;
            }
            continue;
          case 2:
            allowedSubworldsSet.UnionWith((IEnumerable<WeightedSubWorld>) other);
            continue;
          case 3:
            allowedSubworldsSet.IntersectWith((IEnumerable<WeightedSubWorld>) other);
            continue;
          case 4:
            allowedSubworldsSet.ExceptWith((IEnumerable<WeightedSubWorld>) other);
            continue;
          case 5:
            allowedSubworldsSet.SymmetricExceptWith((IEnumerable<WeightedSubWorld>) other);
            continue;
          case 6:
            Debug.LogError((object) "Command.All is unsupported for unknownCellsAllowedSubworlds.");
            continue;
          default:
            continue;
        }
      }
      return allowedSubworldsSet;
    }

    private void ConvertUnknownCells(SeededRandom myRandom, bool isRunningDebugGen)
    {
      List<VoronoiTree.Node> nodeList = new List<VoronoiTree.Node>();
      this.voronoiTree.GetNodesWithTag(WorldGenTags.UnassignedNode, nodeList);
      WorldGenUtil.ShuffleSeeded<VoronoiTree.Node>((IList<VoronoiTree.Node>) nodeList, myRandom.RandomSource());
      List<WeightedSubWorld> subworldsForWorld = this.worldGen.Settings.GetSubworldsForWorld(new List<WeightedSubworldName>((IEnumerable<WeightedSubworldName>) this.worldGen.Settings.world.subworldFiles));
      Dictionary<string, List<WeightedSubWorld>> subworldsByTemperature = new Dictionary<string, List<WeightedSubWorld>>();
      foreach (Temperature.Range range1 in Enum.GetValues(typeof (Temperature.Range)))
      {
        Temperature.Range range = range1;
        subworldsByTemperature.Add(range.ToString(), subworldsForWorld.FindAll((Predicate<WeightedSubWorld>) (sw => sw.subWorld.temperatureRange == range)));
      }
      Dictionary<string, List<WeightedSubWorld>> subworldsByZoneType = new Dictionary<string, List<WeightedSubWorld>>();
      foreach (SubWorld.ZoneType zoneType in Enum.GetValues(typeof (SubWorld.ZoneType)))
      {
        SubWorld.ZoneType zt = zoneType;
        subworldsByZoneType.Add(zt.ToString(), subworldsForWorld.FindAll((Predicate<WeightedSubWorld>) (sw => sw.subWorld.zoneType == zt)));
      }
      foreach (VoronoiTree.Node vn in nodeList)
      {
        Node nodeById = (Node) ((Graph<Cell, Edge>) this.overworldGraph).FindNodeByID(vn.site.id);
        vn.tags.Remove(WorldGenTags.UnassignedNode);
        nodeById.tags.Remove(WorldGenTags.UnassignedNode);
        List<WeightedSubWorld> weightedSubWorldList = new List<WeightedSubWorld>((IEnumerable<WeightedSubWorld>) this.Filter(vn, subworldsForWorld, subworldsByTemperature, subworldsByZoneType));
        List<WeightedSubWorld> all = weightedSubWorldList.FindAll((Predicate<WeightedSubWorld>) (x => x.minCount > 0));
        WeightedSubWorld weightedSubWorld1;
        if (all.Count > 0)
        {
          weightedSubWorld1 = all[0];
          int priority = weightedSubWorld1.priority;
          foreach (WeightedSubWorld weightedSubWorld2 in all)
          {
            if (weightedSubWorld2.priority > priority || weightedSubWorld2.priority == priority && weightedSubWorld2.minCount > weightedSubWorld1.minCount)
            {
              weightedSubWorld1 = weightedSubWorld2;
              priority = weightedSubWorld2.priority;
            }
          }
          --weightedSubWorld1.minCount;
        }
        else
          weightedSubWorld1 = WeightedRandom.Choose<WeightedSubWorld>(weightedSubWorldList, myRandom);
        if (weightedSubWorld1 != null)
        {
          this.ApplySubworldToNode(vn, weightedSubWorld1.subWorld, weightedSubWorld1.overridePower);
          --weightedSubWorld1.maxCount;
          if (weightedSubWorld1.maxCount <= 0)
            subworldsForWorld.Remove(weightedSubWorld1);
        }
        else
        {
          string str = "";
          foreach (KeyValuePair<Tag, int> keyValuePair in vn.minDistanceToTag)
          {
            string[] strArray = new string[5]
            {
              str,
              null,
              null,
              null,
              null
            };
            Tag key = keyValuePair.Key;
            strArray[1] = ((Tag) ref key).Name;
            strArray[2] = ":";
            strArray[3] = keyValuePair.Value.ToString();
            strArray[4] = ", ";
            str = string.Concat(strArray);
          }
          DebugUtil.LogWarningArgs(new object[4]
          {
            (object) "No allowed Subworld types. Using default. ",
            (object) ((object) nodeById.tags).ToString(),
            (object) "Distances:",
            (object) str
          });
          nodeById.SetType("Default");
        }
      }
      foreach (WeightedSubWorld weightedSubWorld in subworldsForWorld)
      {
        if (weightedSubWorld.minCount > 0)
        {
          if (!isRunningDebugGen)
            throw new Exception(string.Format("Could not guarantee minCount of Subworld {0}, {1} remaining on world {2}.", (object) ((SampleDescriber) weightedSubWorld.subWorld).name, (object) weightedSubWorld.minCount, (object) this.worldGen.Settings.world.filePath));
          DebugUtil.DevLogError(string.Format("Could not guarantee minCount of Subworld {0}, {1} remaining on world {2}.", (object) ((SampleDescriber) weightedSubWorld.subWorld).name, (object) weightedSubWorld.minCount, (object) this.worldGen.Settings.world.filePath));
        }
      }
    }

    private Node ApplySubworldToNode(VoronoiTree.Node vn, SubWorld subWorld, float overridePower = -1f)
    {
      Node nodeById = (Node) ((Graph<Cell, Edge>) this.overworldGraph).FindNodeByID(vn.site.id);
      nodeById.SetType(((SampleDescriber) subWorld).name);
      vn.site.weight = (double) overridePower > 0.0 ? overridePower : subWorld.pdWeight;
      foreach (string tag in subWorld.tags)
        vn.AddTag(new Tag(tag));
      vn.AddTag(Tag.op_Implicit(subWorld.zoneType.ToString()));
      return nodeById;
    }

    private void FlattenOverworld()
    {
      try
      {
        WorldLayout.ResetMapGraphFromVoronoiTree(this.voronoiTree.ImmediateChildren(), this.overworldGraph, true);
        foreach (Edge arc in ((Graph<Cell, Edge>) this.overworldGraph).arcs)
        {
          List<Cell> nodes = ((Graph<Cell, Edge>) this.overworldGraph).GetNodes(arc);
          Cell cell1 = nodes[0];
          Cell cell2 = nodes[1];
          SubWorld subWorld1 = this.worldGen.Settings.GetSubWorld(((Node) cell1).type);
          Debug.Assert(subWorld1 != null, (object) ("SubWorld is null: " + ((Node) cell1).type));
          SubWorld subWorld2 = this.worldGen.Settings.GetSubWorld(((Node) cell2).type);
          Debug.Assert(subWorld2 != null, (object) ("other SubWorld is null: " + ((Node) cell2).type));
          if (((Node) cell1).type == ((Node) cell2).type || subWorld1.zoneType == subWorld2.zoneType)
            ((Arc) arc).tags.Add(WorldGenTags.EdgeOpen);
          else if (subWorld1.borderOverride == "NONE" || subWorld2.borderOverride == "NONE")
            ((Arc) arc).tags.Add(WorldGenTags.EdgeOpen);
          else
            ((Arc) arc).tags.Add(WorldGenTags.EdgeClosed);
        }
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ("ex: " + ex.Message + " " + ex.StackTrace));
      }
    }

    public static bool TestEdgeConsistency(MapGraph graph, Cell cell, out Edge problemEdge)
    {
      List<Edge> arcs = ((Graph<Cell, Edge>) graph).GetArcs(cell);
      foreach (Edge edge1 in arcs)
      {
        int num1 = 0;
        int num2 = 0;
        foreach (Edge edge2 in arcs)
        {
          if (edge2.corner0 == edge1.corner0 || edge2.corner1 == edge1.corner0)
            ++num1;
          if (edge2.corner1 == edge1.corner1 || edge2.corner1 == edge1.corner1)
            ++num2;
        }
        if (num1 != 2 || num2 != 2)
        {
          problemEdge = edge1;
          return false;
        }
      }
      problemEdge = (Edge) null;
      return true;
    }

    private void AddSubworldChildren()
    {
      new TagSet().Add(WorldGenTags.Overworld);
      List<string> defaultMoveTags = this.worldGen.Settings.GetDefaultMoveTags();
      if (defaultMoveTags != null)
      {
        TagSet tagSet = new TagSet((IEnumerable<string>) defaultMoveTags);
      }
      List<Feature> featureList = new List<Feature>();
      foreach (KeyValuePair<string, int> globalFeature in this.worldGen.Settings.world.globalFeatures)
      {
        for (int index = 0; index < globalFeature.Value; ++index)
          featureList.Add(new Feature()
          {
            type = globalFeature.Key
          });
      }
      Dictionary<uint, List<Feature>> dictionary = new Dictionary<uint, List<Feature>>();
      List<VoronoiTree.Node> nodeList = new List<VoronoiTree.Node>();
      this.voronoiTree.GetNodesWithoutTag(WorldGenTags.NoGlobalFeatureSpawning, nodeList);
      WorldGenUtil.ShuffleSeeded<VoronoiTree.Node>((IList<VoronoiTree.Node>) nodeList, this.myRandom.RandomSource());
      foreach (Feature feature in featureList)
      {
        if (nodeList.Count != 0)
        {
          VoronoiTree.Node node = nodeList[0];
          nodeList.RemoveAt(0);
          if (!dictionary.ContainsKey(node.site.id))
            dictionary[node.site.id] = new List<Feature>();
          dictionary[node.site.id].Add(feature);
        }
        else
          break;
      }
      this.localGraph.ClearEdgesAndCorners();
      for (int index = 0; index < this.voronoiTree.ChildCount(); ++index)
      {
        VoronoiTree.Node child1 = this.voronoiTree.GetChild(index);
        if (child1.type == 1)
        {
          Tree child = child1 as Tree;
          Node nodeById = (Node) ((Graph<Cell, Edge>) this.overworldGraph).FindNodeByID(((VoronoiTree.Node) child).site.id);
          SubWorld sw = SerializingCloner.Copy<SubWorld>(this.worldGen.Settings.GetSubWorld(nodeById.type));
          ((VoronoiTree.Node) child).AddTag(new Tag(nodeById.type));
          ((VoronoiTree.Node) child).AddTag(new Tag(sw.temperatureRange.ToString()));
          ((VoronoiTree.Node) child).AddTag(new Tag(sw.zoneType.ToString()));
          if (dictionary.ContainsKey(child1.site.id))
            sw.features.AddRange((IEnumerable<Feature>) dictionary[child1.site.id]);
          this.GenerateChildren(sw, child, this.localGraph, (float) this.mapHeight, index + this.myRandom.seed);
          child.RelaxRecursive(0, 10, 1f, this.worldGen.Settings.world.layoutMethod == 1);
          child.VisitAll((Action<VoronoiTree.Node>) (n => Debug.Assert(n.site.poly != null, (object) string.Format("Node {0}, child of {1} had a null poly after final subworld relax!!", (object) n.site.id, (object) ((VoronoiTree.Node) child).site.id))));
        }
      }
      VoronoiTree.Node.maxDepth = this.voronoiTree.MaxDepth(0);
    }

    private List<Vector2> GetPoints(
      string name,
      LoggerSSF log,
      int minPointCount,
      int maxPointCount,
      Polygon boundingArea,
      float density,
      float avoidRadius,
      List<Vector2> avoidPoints,
      PointGenerator.SampleBehaviour sampleBehaviour,
      bool testInsideBounds,
      SeededRandom rnd,
      bool doShuffle = true,
      bool testAvoidPoints = true)
    {
      int num1 = 0;
      List<Vector2> randomPoints;
      do
      {
        randomPoints = PointGenerator.GetRandomPoints(boundingArea, density, avoidRadius, avoidPoints, sampleBehaviour, testInsideBounds, rnd, doShuffle, testAvoidPoints);
        if (randomPoints.Count < minPointCount)
        {
          density *= 0.8f;
          avoidRadius *= 0.8f;
          int num2 = this.worldGen.isRunningDebugGen ? 1 : 0;
        }
        ++num1;
      }
      while (randomPoints.Count < minPointCount && randomPoints.Count <= maxPointCount && num1 < 10);
      if (randomPoints.Count > maxPointCount)
        randomPoints.RemoveRange(maxPointCount, randomPoints.Count - maxPointCount);
      return randomPoints;
    }

    public void GenerateChildren(
      SubWorld sw,
      Tree node,
      MapGraph graph,
      float worldHeight,
      int seed)
    {
      SeededRandom rnd = new SeededRandom(seed);
      List<string> defaultMoveTags = this.worldGen.Settings.GetDefaultMoveTags();
      TagSet tagSet1 = defaultMoveTags != null ? new TagSet((IEnumerable<string>) defaultMoveTags) : (TagSet) null;
      TagSet tagSet2 = new TagSet();
      if (tagSet1 != null)
      {
        for (int index = 0; index < tagSet1.Count; ++index)
        {
          Tag tag = tagSet1[index];
          if (((VoronoiTree.Node) node).tags.Contains(tag))
          {
            ((VoronoiTree.Node) node).tags.Remove(tag);
            tagSet2.Add(tag);
          }
        }
      }
      TagSet newTags = new TagSet(((VoronoiTree.Node) node).tags);
      newTags.Remove(WorldGenTags.Overworld);
      for (int index = 0; index < sw.tags.Count; ++index)
        newTags.Add(new Tag(sw.tags[index]));
      MinMax density1 = ((SampleDescriber) sw).density;
      float valueWithinRange1 = ((MinMax) ref density1).GetRandomValueWithinRange(rnd);
      List<Vector2> avoidPoints = new List<Vector2>();
      if (sw.centralFeature != null)
      {
        avoidPoints.Add(((VoronoiTree.Node) node).site.poly.Centroid());
        this.CreateTreeNodeWithFeatureAndBiome(this.worldGen.Settings, sw, node, graph, sw.centralFeature, ((VoronoiTree.Node) node).site.poly.Centroid(), newTags, -1).AddTag(WorldGenTags.CenteralFeature);
      }
      node.dontRelaxChildren = ((SampleDescriber) sw).dontRelaxChildren;
      int minPointCount = Mathf.Max(sw.features.Count + sw.extraBiomeChildren, sw.minChildCount);
      int maxPointCount = int.MaxValue;
      if (sw.singleChildCount)
      {
        minPointCount = 1;
        maxPointCount = 1;
      }
      List<Vector2> points = this.GetPoints(((SampleDescriber) sw).name, ((VoronoiTree.Node) node).log, minPointCount, maxPointCount, ((VoronoiTree.Node) node).site.poly, valueWithinRange1, ((SampleDescriber) sw).avoidRadius, avoidPoints, ((SampleDescriber) sw).sampleBehaviour, true, rnd, testAvoidPoints: ((SampleDescriber) sw).doAvoidPoints);
      Debug.Assert((points.Count >= minPointCount ? 1 : 0) != 0, (object) string.Format("Overworld node {0} of subworld {1} generated {2} points of an expected minimum {3}\nThis probably means that either:\n* sampler density is too large (lower the number for tighter samples)\n* avoid radius is too large (only applies if there is a central feature, especialy if you get 0 points generated)\n* min point count is just plain too large.", (object) ((VoronoiTree.Node) node).site.id, (object) ((SampleDescriber) sw).name, (object) points.Count, (object) minPointCount));
      for (int index = 0; index < sw.samplers.Count; ++index)
      {
        avoidPoints.AddRange((IEnumerable<Vector2>) points);
        MinMax density2 = sw.samplers[index].density;
        float valueWithinRange2 = ((MinMax) ref density2).GetRandomValueWithinRange(rnd);
        List<Vector2> randomPoints = PointGenerator.GetRandomPoints(((VoronoiTree.Node) node).site.poly, valueWithinRange2, sw.samplers[index].avoidRadius, avoidPoints, sw.samplers[index].sampleBehaviour, true, rnd, true, sw.samplers[index].doAvoidPoints);
        points.AddRange((IEnumerable<Vector2>) randomPoints);
      }
      if (points.Count > 200)
        points.RemoveRange(200, points.Count - 200);
      if (points.Count < minPointCount)
      {
        string str = "";
        for (int index = 0; index < ((VoronoiTree.Node) node).site.poly.Vertices.Count; ++index)
          str = str + ((VoronoiTree.Node) node).site.poly.Vertices[index].ToString() + ", ";
        if (!this.worldGen.isRunningDebugGen)
          return;
        Debug.Assert(points.Count >= minPointCount, (object) ("Error not enough points " + ((SampleDescriber) sw).name + " in node " + ((VoronoiTree.Node) node).site.id.ToString()));
      }
      else
      {
        int count1 = sw.features.Count;
        int count2 = points.Count;
        for (int index = 0; index < points.Count; ++index)
        {
          Feature feature = (Feature) null;
          if (index < sw.features.Count)
            feature = sw.features[index];
          this.CreateTreeNodeWithFeatureAndBiome(this.worldGen.Settings, sw, node, graph, feature, points[index], newTags, index);
        }
        node.ComputeChildren(rnd.seed + 1, false, false);
        node.VisitAll((Action<VoronoiTree.Node>) (n => Debug.Assert(n.site.poly != null, (object) string.Format("Node {0}, child of {1} had a null poly after final subworld compute!!", (object) n.site.id, (object) ((VoronoiTree.Node) node).site.id))));
        if (node.ChildCount() <= 0)
          return;
        for (int index = 0; index < tagSet2.Count; ++index)
        {
          Tag tag = tagSet2[index];
          Debug.Log((object) string.Format("Applying Moved Tag {0} to {1}", (object) ((Tag) ref tag).Name, (object) ((VoronoiTree.Node) node).site.id));
          node.GetChild(rnd.RandomSource().Next(node.ChildCount())).AddTag(tagSet2[index]);
        }
      }
    }

    private VoronoiTree.Node CreateTreeNodeWithFeatureAndBiome(
      WorldGenSettings settings,
      SubWorld sw,
      Tree node,
      MapGraph graph,
      Feature feature,
      Vector2 pos,
      TagSet newTags,
      int i)
    {
      string str = (string) null;
      bool flag = false;
      TagSet tagSet1 = new TagSet();
      TagSet tagSet2 = new TagSet();
      if (feature != null)
      {
        FeatureSettings feature1 = settings.GetFeature(feature.type);
        str = feature.type;
        tagSet2.Union(new TagSet((IEnumerable<string>) feature1.tags));
        if (feature.tags != null && feature.tags.Count > 0)
          tagSet2.Union(new TagSet((IEnumerable<string>) feature.tags));
        if (feature.excludesTags != null && feature.excludesTags.Count > 0)
          tagSet2.Remove(new TagSet((IEnumerable<string>) feature.excludesTags));
        tagSet2.Add(new Tag(feature.type));
        tagSet2.Add(WorldGenTags.Feature);
        if (feature1.forceBiome != null)
        {
          tagSet1.Add(Tag.op_Implicit(feature1.forceBiome));
          flag = true;
        }
        if (feature1.biomeTags != null)
          tagSet1.Union(new TagSet((IEnumerable<string>) feature1.biomeTags));
      }
      if (!flag && sw.biomes.Count > 0)
      {
        WeightedBiome weightedBiome = WeightedRandom.Choose<WeightedBiome>(sw.biomes, this.myRandom);
        if (str == null)
          str = weightedBiome.name;
        tagSet1.Add(Tag.op_Implicit(weightedBiome.name));
        if (weightedBiome.tags != null && weightedBiome.tags.Count > 0)
          tagSet1.Union(new TagSet((IEnumerable<string>) weightedBiome.tags));
        flag = true;
      }
      if (!flag)
      {
        str = "UNKNOWN";
        Debug.LogError((object) ("Couldn't get a biome for a cell in " + ((SampleDescriber) sw).name + ". Maybe it doesn't have any biomes configured?"));
      }
      Cell cell = ((Graph<Cell, Edge>) graph).AddNode(str, pos);
      ((Node) cell).biomeSpecificTags = new TagSet(tagSet1);
      ((Node) cell).featureSpecificTags = new TagSet(tagSet2);
      VoronoiTree.Node withFeatureAndBiome = node.AddSite(new Diagram.Site((uint) cell.NodeId, ((Node) cell).position, 1f), (VoronoiTree.Node.NodeType) 1);
      withFeatureAndBiome.tags = new TagSet(newTags);
      withFeatureAndBiome.tags.Add(Tag.op_Implicit(str));
      withFeatureAndBiome.tags.Union(tagSet1);
      withFeatureAndBiome.tags.Union(tagSet2);
      return withFeatureAndBiome;
    }

    private void TagTopAndBottomSites(Tag topTag, Tag bottomTag)
    {
      List<Diagram.Site> siteList1 = new List<Diagram.Site>();
      List<Diagram.Site> siteList2 = new List<Diagram.Site>();
      this.voronoiTree.GetIntersectingLeafSites(this.topEdge, siteList1);
      this.voronoiTree.GetIntersectingLeafSites(this.bottomEdge, siteList2);
      for (int index = 0; index < siteList1.Count; ++index)
        this.voronoiTree.GetNodeForSite(siteList1[index]).AddTag(topTag);
      for (int index = 0; index < siteList2.Count; ++index)
        this.voronoiTree.GetNodeForSite(siteList2[index]).AddTag(bottomTag);
    }

    private void TagEdgeSites(Tag leftTag, Tag rightTag)
    {
      List<Diagram.Site> siteList1 = new List<Diagram.Site>();
      List<Diagram.Site> siteList2 = new List<Diagram.Site>();
      this.voronoiTree.GetIntersectingLeafSites(this.leftEdge, siteList1);
      this.voronoiTree.GetIntersectingLeafSites(this.rightEdge, siteList2);
      for (int index = 0; index < siteList1.Count; ++index)
        this.voronoiTree.GetNodeForSite(siteList1[index]).AddTag(leftTag);
      for (int index = 0; index < siteList2.Count; ++index)
        this.voronoiTree.GetNodeForSite(siteList2[index]).AddTag(rightTag);
    }

    private bool StartAreaTooLarge(VoronoiTree.Node node) => node.tags.Contains(WorldGenTags.AtStart) && (double) node.site.poly.Area() > 2000.0;

    private void PropagateStartTag()
    {
      foreach (VoronoiTree.Node startNode in this.GetStartNodes())
      {
        startNode.AddTagToNeighbors(WorldGenTags.NearStartLocation);
        startNode.AddTag(WorldGenTags.IgnoreCaveOverride);
      }
    }

    public List<VoronoiTree.Node> GetStartNodes() => this.GetLeafNodesWithTag(WorldGenTags.StartLocation);

    public List<VoronoiTree.Node> GetLeafNodesWithTag(Tag tag)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      WorldLayout.\u003C\u003Ec__DisplayClass58_0 cDisplayClass580 = new WorldLayout.\u003C\u003Ec__DisplayClass58_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass580.tag = tag;
      List<VoronoiTree.Node> leafNodesWithTag = new List<VoronoiTree.Node>();
      // ISSUE: method pointer
      this.voronoiTree.GetLeafNodes(leafNodesWithTag, new Tree.LeafNodeTest((object) cDisplayClass580, __methodptr(\u003CGetLeafNodesWithTag\u003Eb__0)));
      return leafNodesWithTag;
    }

    public List<Node> GetTerrainNodesForTag(Tag tag)
    {
      List<Node> terrainNodesForTag = new List<Node>();
      foreach (VoronoiTree.Node node in this.GetLeafNodesWithTag(tag))
      {
        Node nodeById = (Node) ((Graph<Cell, Edge>) this.localGraph).FindNodeByID(node.site.id);
        if (nodeById != null)
          terrainNodesForTag.Add(nodeById);
      }
      return terrainNodesForTag;
    }

    private Node FindFirstNode(string nodeType) => (Node) ((Graph<Cell, Edge>) this.localGraph).FindNode((Predicate<Cell>) (node => ((Node) node).type == nodeType));

    private Node FindFirstNodeWithTag(Tag tag) => (Node) ((Graph<Cell, Edge>) this.localGraph).FindNode((Predicate<Cell>) (node => ((Node) node).tags != null && ((Node) node).tags.Contains(tag)));

    public Vector2I GetStartLocation()
    {
      if (string.IsNullOrEmpty(this.worldGen.Settings.world.startSubworldName))
        return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
      Node node1 = this.FindFirstNodeWithTag(WorldGenTags.StartLocation);
      if (node1 == null)
      {
        List<VoronoiTree.Node> nodes = this.GetStartNodes();
        if (nodes == null || nodes.Count == 0)
        {
          Debug.LogWarning((object) "Couldnt find start node");
          return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
        }
        node1 = (Node) ((Graph<Cell, Edge>) this.localGraph).FindNode((Predicate<Cell>) (node => (int) (uint) node.NodeId == (int) nodes[0].site.id));
        node1.tags.Add(WorldGenTags.StartLocation);
      }
      if (node1 != null)
        return new Vector2I((int) node1.position.x, (int) node1.position.y);
      Debug.LogWarning((object) "Couldnt find start node");
      return new Vector2I(this.mapWidth / 2, this.mapHeight / 2);
    }

    private List<Diagram.Site> GetIntersectingSites(VoronoiTree.Node intersectingSiteSource, Tree sitesSource)
    {
      List<Diagram.Site> siteList = new List<Diagram.Site>();
      List<Diagram.Site> intersectingSites = new List<Diagram.Site>();
      for (int index = 1; index < intersectingSiteSource.site.poly.Vertices.Count - 1; ++index)
      {
        LineSegment lineSegment = new LineSegment(new Vector2?(intersectingSiteSource.site.poly.Vertices[index - 1]), new Vector2?(intersectingSiteSource.site.poly.Vertices[index]));
        sitesSource.GetIntersectingLeafSites(lineSegment, intersectingSites);
      }
      LineSegment lineSegment1 = new LineSegment(new Vector2?(intersectingSiteSource.site.poly.Vertices[intersectingSiteSource.site.poly.Vertices.Count - 1]), new Vector2?(intersectingSiteSource.site.poly.Vertices[0]));
      sitesSource.GetIntersectingLeafSites(lineSegment1, intersectingSites);
      return intersectingSites;
    }

    public void GetEdgeOfMapSites(
      Tree vt,
      List<Diagram.Site> topSites,
      List<Diagram.Site> bottomSites,
      List<Diagram.Site> leftSites,
      List<Diagram.Site> rightSites)
    {
      vt.GetIntersectingLeafSites(this.topEdge, topSites);
      vt.GetIntersectingLeafSites(this.bottomEdge, bottomSites);
      vt.GetIntersectingLeafSites(this.leftEdge, leftSites);
      vt.GetIntersectingLeafSites(this.rightEdge, rightSites);
    }

    [OnSerializing]
    internal void OnSerializingMethod()
    {
      try
      {
        this.extra = new WorldLayout.ExtraIO();
        if (this.voronoiTree == null)
          return;
        this.extra.internals.Add(this.voronoiTree);
        this.voronoiTree.GetInternalNodes(this.extra.internals);
        List<VoronoiTree.Node> nodeList = new List<VoronoiTree.Node>();
        this.voronoiTree.GetLeafNodes(nodeList, (Tree.LeafNodeTest) null);
        foreach (Leaf leaf in nodeList)
        {
          Leaf ln = leaf;
          if (ln != null)
          {
            this.extra.leafInternalParent.Add(new KeyValuePair<int, int>(this.extra.leafs.Count, this.extra.internals.FindIndex(0, (Predicate<Tree>) (n => n == ((VoronoiTree.Node) ln).parent))));
            this.extra.leafs.Add(ln);
          }
        }
        for (int index1 = 0; index1 < this.extra.internals.Count; ++index1)
        {
          Tree vt = this.extra.internals[index1];
          if (((VoronoiTree.Node) vt).parent != null)
          {
            int index2 = this.extra.internals.FindIndex(0, (Predicate<Tree>) (n => n == ((VoronoiTree.Node) vt).parent));
            if (index2 >= 0)
              this.extra.internalInternalParent.Add(new KeyValuePair<int, int>(index1, index2));
          }
        }
      }
      catch (Exception ex)
      {
        WorldGenLogger.LogException(ex.Message, ex.StackTrace);
        Debug.Log((object) ("Error deserialising " + ex.Message));
      }
    }

    [OnSerialized]
    internal void OnSerializedMethod() => this.extra = (WorldLayout.ExtraIO) null;

    [OnDeserializing]
    internal void OnDeserializingMethod() => this.extra = new WorldLayout.ExtraIO();

    [OnDeserialized]
    internal void OnDeserializedMethod()
    {
      try
      {
        this.voronoiTree = this.extra.internals[0];
        for (int index = 0; index < this.extra.internalInternalParent.Count; ++index)
        {
          KeyValuePair<int, int> keyValuePair = this.extra.internalInternalParent[index];
          Tree tree = this.extra.internals[keyValuePair.Key];
          this.extra.internals[keyValuePair.Value].AddChild((VoronoiTree.Node) tree);
        }
        for (int index = 0; index < this.extra.leafInternalParent.Count; ++index)
        {
          KeyValuePair<int, int> keyValuePair = this.extra.leafInternalParent[index];
          VoronoiTree.Node leaf = (VoronoiTree.Node) this.extra.leafs[keyValuePair.Key];
          this.extra.internals[keyValuePair.Value].AddChild(leaf);
        }
      }
      catch (Exception ex)
      {
        WorldGenLogger.LogException(ex.Message, ex.StackTrace);
        Debug.Log((object) ("Error deserialising " + ex.Message));
      }
      this.extra = (WorldLayout.ExtraIO) null;
    }

    [Flags]
    public enum DebugFlags
    {
      LocalGraph = 1,
      OverworldGraph = 2,
      VoronoiTree = 4,
      PowerDiagram = 8,
    }

    [SerializationConfig]
    private class ExtraIO
    {
      public List<Leaf> leafs = new List<Leaf>();
      public List<Tree> internals = new List<Tree>();
      public List<KeyValuePair<int, int>> leafInternalParent = new List<KeyValuePair<int, int>>();
      public List<KeyValuePair<int, int>> internalInternalParent = new List<KeyValuePair<int, int>>();

      [OnDeserializing]
      internal void OnDeserializingMethod()
      {
        this.leafs = new List<Leaf>();
        this.internals = new List<Tree>();
        this.leafInternalParent = new List<KeyValuePair<int, int>>();
        this.internalInternalParent = new List<KeyValuePair<int, int>>();
      }
    }
  }
}
