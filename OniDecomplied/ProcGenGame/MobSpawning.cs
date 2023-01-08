// Decompiled with JetBrains decompiler
// Type: ProcGenGame.MobSpawning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using ProcGen.Map;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenGame
{
  public static class MobSpawning
  {
    public static Dictionary<TerrainCell, List<HashSet<int>>> NaturalCavities = new Dictionary<TerrainCell, List<HashSet<int>>>();
    public static HashSet<int> allNaturalCavityCells = new HashSet<int>();

    public static Dictionary<int, string> PlaceFeatureAmbientMobs(
      WorldGenSettings settings,
      TerrainCell tc,
      SeededRandom rnd,
      Sim.Cell[] cells,
      float[] bgTemp,
      Sim.DiseaseCell[] dc,
      HashSet<int> avoidCells,
      bool isDebug)
    {
      Dictionary<int, string> spawnedMobs = new Dictionary<int, string>();
      Cell node = tc.node;
      HashSet<int> alreadyOccupiedCells = new HashSet<int>();
      FeatureSettings featureSettings = (FeatureSettings) null;
      foreach (Tag featureSpecificTag in ((ProcGen.Node) node).featureSpecificTags)
      {
        if (settings.HasFeature(((Tag) ref featureSpecificTag).Name))
        {
          featureSettings = settings.GetFeature(((Tag) ref featureSpecificTag).Name);
          break;
        }
      }
      if (featureSettings == null || featureSettings.internalMobs == null || featureSettings.internalMobs.Count == 0)
        return spawnedMobs;
      List<int> spawnCellsFeature = tc.GetAvailableSpawnCellsFeature();
      tc.LogInfo(nameof (PlaceFeatureAmbientMobs), "possibleSpawnPoints", (float) spawnCellsFeature.Count);
      for (int index1 = spawnCellsFeature.Count - 1; index1 > 0; --index1)
      {
        int index2 = spawnCellsFeature[index1];
        if (ElementLoader.elements[(int) cells[index2].elementIdx].id == SimHashes.Katairite || ElementLoader.elements[(int) cells[index2].elementIdx].id == SimHashes.Unobtanium || avoidCells.Contains(index2))
          spawnCellsFeature.RemoveAt(index1);
      }
      TerrainCell terrainCell = tc;
      long nodeId = node.NodeId;
      string str = "Id:" + nodeId.ToString() + " possible cells";
      double count1 = (double) spawnCellsFeature.Count;
      terrainCell.LogInfo("mob spawns", str, (float) count1);
      if (spawnCellsFeature.Count == 0)
      {
        if (isDebug)
        {
          nodeId = tc.node.NodeId;
          Debug.LogWarning((object) ("No where to put mobs possibleSpawnPoints [" + nodeId.ToString() + "]"));
        }
        return (Dictionary<int, string>) null;
      }
      foreach (MobReference internalMob in featureSettings.internalMobs)
      {
        Mob mob = settings.GetMob(internalMob.type);
        if (mob == null)
        {
          Debug.LogError((object) ("Missing mob description for internal mob [" + internalMob.type + "]"));
        }
        else
        {
          List<int> possibleSpawnPoints = MobSpawning.GetMobPossibleSpawnPoints(mob, spawnCellsFeature, cells, alreadyOccupiedCells, rnd);
          if (possibleSpawnPoints.Count == 0)
          {
            if (!isDebug)
              ;
          }
          else
          {
            tc.LogInfo("\t\tpossible", internalMob.type + " mps: " + possibleSpawnPoints.Count.ToString() + " ps:", (float) spawnCellsFeature.Count);
            MinMax count2 = internalMob.count;
            int count3 = Mathf.RoundToInt(((MinMax) ref count2).GetRandomValueWithinRange(rnd));
            tc.LogInfo("\t\tcount", internalMob.type, (float) count3);
            Tag mobPrefab = mob.prefabName == null ? new Tag(internalMob.type) : new Tag(mob.prefabName);
            MobSpawning.SpawnCountMobs(mob, mobPrefab, count3, possibleSpawnPoints, tc, ref spawnedMobs, ref alreadyOccupiedCells);
          }
        }
      }
      return spawnedMobs;
    }

    public static Dictionary<int, string> PlaceBiomeAmbientMobs(
      WorldGenSettings settings,
      TerrainCell tc,
      SeededRandom rnd,
      Sim.Cell[] cells,
      float[] bgTemp,
      Sim.DiseaseCell[] dc,
      HashSet<int> avoidCells,
      bool isDebug)
    {
      Dictionary<int, string> spawnedMobs = new Dictionary<int, string>();
      Cell node = tc.node;
      HashSet<int> alreadyOccupiedCells = new HashSet<int>();
      List<Tag> tagList = new List<Tag>();
      if (((ProcGen.Node) node).biomeSpecificTags == null)
      {
        tc.LogInfo(nameof (PlaceBiomeAmbientMobs), "No tags", (float) node.NodeId);
        return (Dictionary<int, string>) null;
      }
      foreach (Tag biomeSpecificTag in ((ProcGen.Node) node).biomeSpecificTags)
      {
        if (settings.HasMob(((Tag) ref biomeSpecificTag).Name) && settings.GetMob(((Tag) ref biomeSpecificTag).Name) != null)
          tagList.Add(biomeSpecificTag);
      }
      if (tagList.Count <= 0)
      {
        tc.LogInfo(nameof (PlaceBiomeAmbientMobs), "No biome MOBS", (float) node.NodeId);
        return (Dictionary<int, string>) null;
      }
      List<int> possibleSpawnPoints1 = ((ProcGen.Node) node).tags.Contains(WorldGenTags.PreventAmbientMobsInFeature) ? tc.GetAvailableSpawnCellsBiome() : tc.GetAvailableSpawnCellsAll();
      tc.LogInfo("PlaceBiomAmbientMobs", "possibleSpawnPoints", (float) possibleSpawnPoints1.Count);
      for (int index1 = possibleSpawnPoints1.Count - 1; index1 > 0; --index1)
      {
        int index2 = possibleSpawnPoints1[index1];
        if (ElementLoader.elements[(int) cells[index2].elementIdx].id == SimHashes.Katairite || ElementLoader.elements[(int) cells[index2].elementIdx].id == SimHashes.Unobtanium || avoidCells.Contains(index2))
          possibleSpawnPoints1.RemoveAt(index1);
      }
      TerrainCell terrainCell1 = tc;
      long nodeId = node.NodeId;
      string str1 = "Id:" + nodeId.ToString() + " possible cells";
      double count1 = (double) possibleSpawnPoints1.Count;
      terrainCell1.LogInfo("mob spawns", str1, (float) count1);
      if (possibleSpawnPoints1.Count == 0)
      {
        if (isDebug)
        {
          nodeId = tc.node.NodeId;
          Debug.LogWarning((object) ("No where to put mobs possibleSpawnPoints [" + nodeId.ToString() + "]"));
        }
        return (Dictionary<int, string>) null;
      }
      WorldGenUtil.ShuffleSeeded<Tag>((IList<Tag>) tagList, rnd.RandomSource());
      for (int index = 0; index < tagList.Count; ++index)
      {
        WorldGenSettings worldGenSettings = settings;
        Tag tag = tagList[index];
        string name = ((Tag) ref tag).Name;
        Mob mob = worldGenSettings.GetMob(name);
        if (mob == null)
        {
          tag = tagList[index];
          Debug.LogError((object) ("Missing sample description for tag [" + ((Tag) ref tag).Name + "]"));
        }
        else
        {
          List<int> possibleSpawnPoints2 = MobSpawning.GetMobPossibleSpawnPoints(mob, possibleSpawnPoints1, cells, alreadyOccupiedCells, rnd);
          if (possibleSpawnPoints2.Count == 0)
          {
            if (!isDebug)
              ;
          }
          else
          {
            TerrainCell terrainCell2 = tc;
            tag = tagList[index];
            string str2 = tag.ToString() + " mps: " + possibleSpawnPoints2.Count.ToString() + " ps:";
            double count2 = (double) possibleSpawnPoints1.Count;
            terrainCell2.LogInfo("\t\tpossible", str2, (float) count2);
            MinMax density = ((SampleDescriber) mob).density;
            float num1 = ((MinMax) ref density).GetRandomValueWithinRange(rnd) * MobSettings.AmbientMobDensity;
            if ((double) num1 > 1.0)
            {
              if (isDebug)
              {
                tag = tagList[index];
                Debug.LogWarning((object) ("Got a mob density greater than 1.0 for " + ((Tag) ref tag).Name + ". Probably using density as spacing!"));
              }
              num1 = 1f;
            }
            tc.LogInfo("\t\tdensity:", "", num1);
            int count3 = Mathf.RoundToInt((float) possibleSpawnPoints2.Count * num1);
            TerrainCell terrainCell3 = tc;
            tag = tagList[index];
            string str3 = tag.ToString();
            double num2 = (double) count3;
            terrainCell3.LogInfo("\t\tcount", str3, (float) num2);
            Tag mobPrefab = mob.prefabName == null ? tagList[index] : new Tag(mob.prefabName);
            MobSpawning.SpawnCountMobs(mob, mobPrefab, count3, possibleSpawnPoints2, tc, ref spawnedMobs, ref alreadyOccupiedCells);
          }
        }
      }
      return spawnedMobs;
    }

    private static List<int> GetMobPossibleSpawnPoints(
      Mob mob,
      List<int> possibleSpawnPoints,
      Sim.Cell[] cells,
      HashSet<int> alreadyOccupiedCells,
      SeededRandom rnd)
    {
      List<int> all = possibleSpawnPoints.FindAll((Predicate<int>) (cell => MobSpawning.IsSuitableMobSpawnPoint(cell, mob, cells, ref alreadyOccupiedCells)));
      WorldGenUtil.ShuffleSeeded<int>((IList<int>) all, rnd.RandomSource());
      return all;
    }

    public static void SpawnCountMobs(
      Mob mobData,
      Tag mobPrefab,
      int count,
      List<int> mobPossibleSpawnPoints,
      TerrainCell tc,
      ref Dictionary<int, string> spawnedMobs,
      ref HashSet<int> alreadyOccupiedCells)
    {
      for (int index1 = 0; index1 < count && index1 < mobPossibleSpawnPoints.Count; ++index1)
      {
        int possibleSpawnPoint = mobPossibleSpawnPoints[index1];
        for (int widthIterator = 0; widthIterator < mobData.width; ++widthIterator)
        {
          for (int index2 = 0; index2 < mobData.height; ++index2)
          {
            int num = MobSpawning.MobWidthOffset(possibleSpawnPoint, widthIterator);
            alreadyOccupiedCells.Add(num);
          }
        }
        tc.AddMob(new KeyValuePair<int, Tag>(possibleSpawnPoint, mobPrefab));
        spawnedMobs.Add(possibleSpawnPoint, ((Tag) ref mobPrefab).Name);
      }
    }

    public static int MobWidthOffset(int occupiedCell, int widthIterator) => Grid.OffsetCell(occupiedCell, widthIterator % 2 == 0 ? -(widthIterator / 2) : widthIterator / 2 + widthIterator % 2, 0);

    private static bool IsSuitableMobSpawnPoint(
      int cell,
      Mob mob,
      Sim.Cell[] cells,
      ref HashSet<int> alreadyOccupiedCells)
    {
      for (int widthIterator = 0; widthIterator < mob.width; ++widthIterator)
      {
        for (int index = 0; index < mob.height; ++index)
        {
          int cell1 = MobSpawning.MobWidthOffset(cell, widthIterator);
          if (!Grid.IsValidCell(cell1) || !Grid.IsValidCell(Grid.CellAbove(cell1)) || !Grid.IsValidCell(Grid.CellBelow(cell1)) || alreadyOccupiedCells.Contains(cell1))
            return false;
        }
      }
      Element element1 = ElementLoader.elements[(int) cells[cell].elementIdx];
      Element element2 = ElementLoader.elements[(int) cells[Grid.CellAbove(cell)].elementIdx];
      Element element3 = ElementLoader.elements[(int) cells[Grid.CellBelow(cell)].elementIdx];
      switch ((int) mob.location)
      {
        case 0:
          bool flag1 = true;
          for (int y = 0; y < mob.height; ++y)
          {
            for (int x = 0; x < mob.width; ++x)
            {
              int cell2 = Grid.OffsetCell(cell, x, y);
              Element element4 = ElementLoader.elements[(int) cells[cell2].elementIdx];
              Element element5 = ElementLoader.elements[(int) cells[Grid.CellAbove(cell2)].elementIdx];
              Element element6 = ElementLoader.elements[(int) cells[Grid.CellBelow(cell2)].elementIdx];
              flag1 = flag1 && MobSpawning.isNaturalCavity(cell2) && !element4.IsSolid && !element4.IsLiquid && !element5.IsSolid;
              if (y == 0)
                flag1 = flag1 && element6.IsSolid;
              if (!flag1)
                break;
            }
            if (!flag1)
              break;
          }
          return flag1;
        case 1:
          bool flag2 = true;
          for (int y = 0; y < mob.height; ++y)
          {
            for (int x = 0; x < mob.width; ++x)
            {
              int cell3 = Grid.OffsetCell(cell, x, y);
              Element element7 = ElementLoader.elements[(int) cells[cell3].elementIdx];
              Element element8 = ElementLoader.elements[(int) cells[Grid.CellAbove(cell3)].elementIdx];
              Element element9 = ElementLoader.elements[(int) cells[Grid.CellBelow(cell3)].elementIdx];
              flag2 = flag2 && MobSpawning.isNaturalCavity(cell3) && !element7.IsSolid && !element7.IsLiquid && !element9.IsSolid;
              if (y == mob.height - 1)
                flag2 = flag2 && element8.IsSolid;
              if (!flag2)
                break;
            }
            if (!flag2)
              break;
          }
          return flag2;
        case 2:
          return !element1.IsSolid && !element2.IsSolid && !element1.IsLiquid;
        case 6:
          return !MobSpawning.isNaturalCavity(cell) && element1.IsSolid;
        case 7:
          if (element1.id != SimHashes.Water && element1.id != SimHashes.DirtyWater)
            return false;
          return element2.id == SimHashes.Water || element2.id == SimHashes.DirtyWater;
        case 9:
          bool flag3 = true;
          for (int widthIterator = 0; widthIterator < mob.width; ++widthIterator)
          {
            int cell4 = MobSpawning.MobWidthOffset(cell, widthIterator);
            Element element10 = ElementLoader.elements[(int) cells[cell4].elementIdx];
            Element element11 = ElementLoader.elements[(int) cells[Grid.CellBelow(cell4)].elementIdx];
            flag3 = flag3 && element10.id == SimHashes.Vacuum && element11.IsSolid;
          }
          return flag3;
        case 10:
          bool flag4 = true;
          for (int y = 0; y < mob.height; ++y)
          {
            for (int x = 0; x < mob.width; ++x)
            {
              int cell5 = Grid.OffsetCell(cell, x, y);
              Element element12 = ElementLoader.elements[(int) cells[cell5].elementIdx];
              Element element13 = ElementLoader.elements[(int) cells[Grid.CellAbove(cell5)].elementIdx];
              Element element14 = ElementLoader.elements[(int) cells[Grid.CellBelow(cell5)].elementIdx];
              flag4 = flag4 && MobSpawning.isNaturalCavity(cell) && !element12.IsSolid && !element13.IsSolid;
              if (y == 0)
                flag4 = flag4 && element12.IsLiquid && element14.IsSolid;
              if (!flag4)
                break;
            }
            if (!flag4)
              break;
          }
          return flag4;
        case 11:
          bool flag5 = true;
          for (int y = 0; y < mob.height; ++y)
          {
            for (int x = 0; x < mob.width; ++x)
            {
              int cell6 = Grid.OffsetCell(cell, x, y);
              Element element15 = ElementLoader.elements[(int) cells[cell6].elementIdx];
              Element element16 = ElementLoader.elements[(int) cells[Grid.CellAbove(cell6)].elementIdx];
              Element element17 = ElementLoader.elements[(int) cells[Grid.CellBelow(cell6)].elementIdx];
              flag5 = flag5 && MobSpawning.isNaturalCavity(cell) && !element15.IsSolid && !element16.IsSolid;
              if (y == 0)
                flag5 = flag5 && element17.IsSolid;
              if (!flag5)
                break;
            }
            if (!flag5)
              break;
          }
          return flag5;
        default:
          return MobSpawning.isNaturalCavity(cell) && !element1.IsSolid;
      }
    }

    public static bool isNaturalCavity(int cell) => MobSpawning.NaturalCavities != null && MobSpawning.allNaturalCavityCells.Contains(cell);

    public static void DetectNaturalCavities(
      List<TerrainCell> terrainCells,
      WorldGen.OfflineCallbackFunction updateProgressFn,
      Sim.Cell[] cells)
    {
      int num1 = updateProgressFn(UI.WORLDGEN.ANALYZINGWORLD.key, 0.0f, WorldGenProgressStages.Stages.DetectNaturalCavities) ? 1 : 0;
      MobSpawning.NaturalCavities.Clear();
      MobSpawning.allNaturalCavityCells.Clear();
      HashSet<int> invalidCells = new HashSet<int>();
      for (int index1 = 0; index1 < terrainCells.Count; ++index1)
      {
        TerrainCell terrainCell = terrainCells[index1];
        float completePercent = (float) index1 / (float) terrainCells.Count;
        int num2 = updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, completePercent, WorldGenProgressStages.Stages.DetectNaturalCavities) ? 1 : 0;
        MobSpawning.NaturalCavities.Add(terrainCell, new List<HashSet<int>>());
        invalidCells.Clear();
        List<int> allCells = terrainCell.GetAllCells();
        for (int index2 = 0; index2 < allCells.Count; ++index2)
        {
          int start_cell = allCells[index2];
          if (!ElementLoader.elements[(int) cells[start_cell].elementIdx].IsSolid && !invalidCells.Contains(start_cell))
          {
            HashSet<int> other = GameUtil.FloodCollectCells(start_cell, (Func<int, bool>) (checkCell =>
            {
              Element element = ElementLoader.elements[(int) cells[checkCell].elementIdx];
              return !invalidCells.Contains(checkCell) && !element.IsSolid;
            }), AddInvalidCellsToSet: invalidCells);
            if (other != null && other.Count > 0)
            {
              MobSpawning.NaturalCavities[terrainCell].Add(other);
              MobSpawning.allNaturalCavityCells.UnionWith((IEnumerable<int>) other);
            }
          }
        }
      }
      int num3 = updateProgressFn(UI.WORLDGEN.ANALYZINGWORLDCOMPLETE.key, 1f, WorldGenProgressStages.Stages.DetectNaturalCavities) ? 1 : 0;
    }
  }
}
