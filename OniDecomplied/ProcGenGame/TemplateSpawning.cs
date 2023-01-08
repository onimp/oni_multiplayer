// Decompiled with JetBrains decompiler
// Type: ProcGenGame.TemplateSpawning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenGame
{
  public class TemplateSpawning
  {
    private static float s_minProgressPercent;
    private static float s_maxProgressPercent;
    private static int s_poiPadding;
    private const int TEMPERATURE_PADDING = 3;
    private const float EXTREME_POI_OVERLAP_TEMPERATURE_RANGE = 100f;

    public static List<KeyValuePair<Vector2I, TemplateContainer>> DetermineTemplatesForWorld(
      WorldGenSettings settings,
      List<TerrainCell> terrainCells,
      SeededRandom myRandom,
      ref List<RectInt> placedPOIBounds,
      bool isRunningDebugGen,
      ref List<WorldTrait> placedStoryTraits,
      WorldGen.OfflineCallbackFunction successCallbackFn)
    {
      int num1 = successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, 0.0f, WorldGenProgressStages.Stages.PlaceTemplates) ? 1 : 0;
      List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets = new List<KeyValuePair<Vector2I, TemplateContainer>>();
      TemplateSpawning.s_poiPadding = settings.GetIntSetting("POIPadding");
      TemplateSpawning.s_minProgressPercent = 0.0f;
      TemplateSpawning.s_maxProgressPercent = 0.33f;
      TemplateSpawning.SpawnStartingTemplate(settings, terrainCells, ref templateSpawnTargets, ref placedPOIBounds, isRunningDebugGen, successCallbackFn);
      TemplateSpawning.s_minProgressPercent = TemplateSpawning.s_maxProgressPercent;
      TemplateSpawning.s_maxProgressPercent = 0.66f;
      TemplateSpawning.SpawnTemplatesFromTemplateRules(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, isRunningDebugGen, successCallbackFn);
      TemplateSpawning.s_minProgressPercent = TemplateSpawning.s_maxProgressPercent;
      TemplateSpawning.s_maxProgressPercent = 1f;
      TemplateSpawning.SpawnStoryTraitTemplates(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, ref placedStoryTraits, isRunningDebugGen, successCallbackFn);
      int num2 = successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, 1f, WorldGenProgressStages.Stages.PlaceTemplates) ? 1 : 0;
      return templateSpawnTargets;
    }

    private static float ProgressPercent(float stagePercent) => MathUtil.ReRange(stagePercent, 0.0f, 1f, TemplateSpawning.s_minProgressPercent, TemplateSpawning.s_maxProgressPercent);

    private static void SpawnStartingTemplate(
      WorldGenSettings settings,
      List<TerrainCell> terrainCells,
      ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets,
      ref List<RectInt> placedPOIBounds,
      bool isRunningDebugGen,
      WorldGen.OfflineCallbackFunction successCallbackFn)
    {
      TerrainCell terrainCell = terrainCells.Find((Predicate<TerrainCell>) (tc => ((ProcGen.Node) tc.node).tags.Contains(WorldGenTags.StartLocation)));
      if (Util.IsNullOrWhiteSpace(settings.world.startingBaseTemplate))
        return;
      TemplateContainer template = TemplateCache.GetTemplate(settings.world.startingBaseTemplate);
      KeyValuePair<Vector2I, TemplateContainer> keyValuePair = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int) terrainCell.poly.Centroid().x, (int) terrainCell.poly.Centroid().y), template);
      RectInt templateBounds = template.GetTemplateBounds(keyValuePair.Key, TemplateSpawning.s_poiPadding);
      if (TemplateSpawning.IsPOIOverlappingBounds(placedPOIBounds, templateBounds))
      {
        string message = "TemplateSpawning: Starting template overlaps world boundaries in world '" + settings.world.filePath + "'";
        DebugUtil.DevLogError(message);
        if (!isRunningDebugGen)
          throw new Exception(message);
      }
      int num = successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, TemplateSpawning.ProgressPercent(1f), WorldGenProgressStages.Stages.PlaceTemplates) ? 1 : 0;
      templateSpawnTargets.Add(keyValuePair);
      placedPOIBounds.Add(templateBounds);
    }

    private static void SpawnTemplatesFromTemplateRules(
      WorldGenSettings settings,
      List<TerrainCell> terrainCells,
      SeededRandom myRandom,
      ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets,
      ref List<RectInt> placedPOIBounds,
      bool isRunningDebugGen,
      WorldGen.OfflineCallbackFunction successCallbackFn)
    {
      List<World.TemplateSpawnRules> templateSpawnRulesList = new List<World.TemplateSpawnRules>();
      if (settings.world.worldTemplateRules != null)
        templateSpawnRulesList.AddRange((IEnumerable<World.TemplateSpawnRules>) settings.world.worldTemplateRules);
      foreach (WeightedSubworldName subworldFile in settings.world.subworldFiles)
      {
        SubWorld subWorld = settings.GetSubWorld(subworldFile.name);
        if (subWorld.subworldTemplateRules != null)
          templateSpawnRulesList.AddRange((IEnumerable<World.TemplateSpawnRules>) subWorld.subworldTemplateRules);
      }
      if (templateSpawnRulesList.Count == 0)
        return;
      int num1 = 0;
      float count = (float) templateSpawnRulesList.Count;
      templateSpawnRulesList.Sort((Comparison<World.TemplateSpawnRules>) ((a, b) => b.priority.CompareTo(a.priority)));
      HashSet<string> usedTemplates = new HashSet<string>();
      foreach (World.TemplateSpawnRules rule in templateSpawnRulesList)
      {
        int num2 = successCallbackFn(UI.WORLDGEN.PLACINGTEMPLATES.key, TemplateSpawning.ProgressPercent((float) num1++ / count), WorldGenProgressStages.Stages.PlaceTemplates) ? 1 : 0;
        string errorMessage;
        if (!TemplateSpawning.ApplyTemplateRule(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, rule, ref usedTemplates, out errorMessage, out List<KeyValuePair<Vector2I, TemplateContainer>> _))
        {
          DebugUtil.LogErrorArgs(new object[1]
          {
            (object) errorMessage
          });
          if (!isRunningDebugGen)
            throw new TemplateSpawningException(errorMessage, (string) UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FAILURE);
        }
      }
    }

    private static void SpawnStoryTraitTemplates(
      WorldGenSettings settings,
      List<TerrainCell> terrainCells,
      SeededRandom myRandom,
      ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets,
      ref List<RectInt> placedPOIBounds,
      ref List<WorldTrait> placedStoryTraits,
      bool isRunningDebugGen,
      WorldGen.OfflineCallbackFunction successCallbackFn)
    {
      Queue<WorldTrait> worldTraitQueue = new Queue<WorldTrait>((IEnumerable<WorldTrait>) settings.GetStoryTraitCandiates());
      int count = worldTraitQueue.Count;
      List<WorldTrait> worldTraitList = new List<WorldTrait>();
      HashSet<string> usedTemplates = new HashSet<string>();
      while (worldTraitQueue.Count > 0 && worldTraitList.Count < count)
      {
        WorldTrait worldTrait = worldTraitQueue.Dequeue();
        bool flag = false;
        List<KeyValuePair<Vector2I, TemplateContainer>> newTemplateSpawnTargets = new List<KeyValuePair<Vector2I, TemplateContainer>>();
        string errorMessage = "";
        List<World.TemplateSpawnRules> templateSpawnRulesList = new List<World.TemplateSpawnRules>();
        templateSpawnRulesList.AddRange((IEnumerable<World.TemplateSpawnRules>) worldTrait.additionalWorldTemplateRules);
        templateSpawnRulesList.Sort((Comparison<World.TemplateSpawnRules>) ((a, b) => b.priority.CompareTo(a.priority)));
        foreach (World.TemplateSpawnRules rule in templateSpawnRulesList)
        {
          flag = TemplateSpawning.ApplyTemplateRule(settings, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, rule, ref usedTemplates, out errorMessage, out newTemplateSpawnTargets);
          if (!flag)
          {
            flag = false;
            break;
          }
        }
        if (flag)
        {
          placedStoryTraits.Add(worldTrait);
          worldTraitList.Add(worldTrait);
          settings.ApplyStoryTrait(worldTrait);
          DebugUtil.LogArgs(new object[1]
          {
            (object) ("Applied story trait '" + worldTrait.filePath + "'")
          });
        }
        else
        {
          foreach (KeyValuePair<Vector2I, TemplateContainer> keyValuePair in newTemplateSpawnTargets)
          {
            KeyValuePair<Vector2I, TemplateContainer> partialTemplate = keyValuePair;
            templateSpawnTargets.RemoveAll((Predicate<KeyValuePair<Vector2I, TemplateContainer>>) (x => Vector2I.op_Equality(x.Key, partialTemplate.Key)));
            usedTemplates.Remove(partialTemplate.Value.name);
            placedPOIBounds.RemoveAll((Predicate<RectInt>) (bound => Vector2.op_Equality(((RectInt) ref bound).center, Vector2I.op_Implicit(partialTemplate.Key))));
          }
          if (DlcManager.FeatureClusterSpaceEnabled())
            DebugUtil.LogArgs(new object[1]
            {
              (object) ("Cannot place story trait on '" + worldTrait.filePath + "' and will try another world. error='" + errorMessage + "'.")
            });
          else
            DebugUtil.LogArgs(new object[1]
            {
              (object) ("Cannot place story trait '" + worldTrait.filePath + "' error='" + errorMessage + "'")
            });
        }
      }
    }

    private static bool ApplyTemplateRule(
      WorldGenSettings settings,
      List<TerrainCell> terrainCells,
      SeededRandom myRandom,
      ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets,
      ref List<RectInt> placedPOIBounds,
      World.TemplateSpawnRules rule,
      ref HashSet<string> usedTemplates,
      out string errorMessage,
      out List<KeyValuePair<Vector2I, TemplateContainer>> newTemplateSpawnTargets)
    {
      newTemplateSpawnTargets = new List<KeyValuePair<Vector2I, TemplateContainer>>();
      for (int index = 0; index < rule.times; ++index)
      {
        ListPool<string, TemplateSpawning>.PooledList pooledList = ListPool<string, TemplateSpawning>.Allocate();
        if (!rule.allowDuplicates)
        {
          foreach (string name in rule.names)
          {
            if (!usedTemplates.Contains(name))
            {
              if (!TemplateCache.TemplateExists(name))
                DebugUtil.DevLogError("TemplateSpawning: Missing template '" + name + "' in world '" + settings.world.filePath + "'");
              else
                ((List<string>) pooledList).Add(name);
            }
          }
        }
        else
          ((List<string>) pooledList).AddRange((IEnumerable<string>) rule.names);
        WorldGenUtil.ShuffleSeeded<string>((IList<string>) pooledList, myRandom.RandomSource());
        if (((List<string>) pooledList).Count == 0)
        {
          pooledList.Recycle();
        }
        else
        {
          int num1 = 0;
          int num2 = 0;
          switch ((int) rule.listRule)
          {
            case 0:
              num1 = 1;
              num2 = 1;
              break;
            case 1:
              num1 = rule.someCount;
              num2 = rule.someCount;
              break;
            case 2:
              num1 = rule.someCount;
              num2 = rule.someCount + rule.moreCount;
              break;
            case 3:
              num1 = ((List<string>) pooledList).Count;
              num2 = ((List<string>) pooledList).Count;
              break;
            case 4:
              num2 = 1;
              break;
            case 5:
              num2 = rule.someCount;
              break;
            case 6:
              num2 = ((List<string>) pooledList).Count;
              break;
          }
          string str1 = "";
          foreach (string templatePath in (List<string>) pooledList)
          {
            if (num2 > 0)
            {
              TemplateContainer template = TemplateCache.GetTemplate(templatePath);
              if (template != null)
              {
                bool guarantee = num1 > 0;
                TerrainCell targetForTemplate = TemplateSpawning.FindTargetForTemplate(template, rule, terrainCells, myRandom, ref templateSpawnTargets, ref placedPOIBounds, guarantee, settings);
                if (targetForTemplate != null)
                {
                  KeyValuePair<Vector2I, TemplateContainer> keyValuePair = new KeyValuePair<Vector2I, TemplateContainer>(new Vector2I((int) targetForTemplate.poly.Centroid().x + rule.overrideOffset.x, (int) targetForTemplate.poly.Centroid().y + rule.overrideOffset.y), template);
                  templateSpawnTargets.Add(keyValuePair);
                  newTemplateSpawnTargets.Add(keyValuePair);
                  placedPOIBounds.Add(template.GetTemplateBounds(keyValuePair.Key, TemplateSpawning.s_poiPadding));
                  ((ProcGen.Node) targetForTemplate.node).templateTag = TagExtensions.ToTag(templatePath);
                  ((ProcGen.Node) targetForTemplate.node).tags.Add(TagExtensions.ToTag(templatePath));
                  ((ProcGen.Node) targetForTemplate.node).tags.Add(WorldGenTags.POI);
                  usedTemplates.Add(templatePath);
                  --num2;
                  --num1;
                }
                else
                  str1 = str1 + "\n    - " + templatePath;
              }
            }
            else
              break;
          }
          pooledList.Recycle();
          if (num1 > 0)
          {
            string str2 = string.Join(", ", settings.GetWorldTraitIDs());
            string str3 = string.Join(", ", settings.GetStoryTraitIDs());
            errorMessage = "TemplateSpawning: Guaranteed placement failure on " + settings.world.filePath + "\n" + string.Format("    listRule={0} someCount={1} moreCount={2} count={3}\n", (object) rule.listRule, (object) rule.someCount, (object) rule.moreCount, (object) ((List<string>) pooledList).Count) + "    Could not place templates:" + str1 + "\n    world traits=" + str2 + "\n    story traits=" + str3;
            return false;
          }
        }
      }
      errorMessage = "";
      return true;
    }

    private static TerrainCell FindTargetForTemplate(
      TemplateContainer template,
      World.TemplateSpawnRules rule,
      List<TerrainCell> terrainCells,
      SeededRandom myRandom,
      ref List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets,
      ref List<RectInt> placedPOIBounds,
      bool guarantee,
      WorldGenSettings settings)
    {
      List<TerrainCell> filteredTerrainCells = rule.useRelaxedFiltering ? terrainCells.FindAll((Predicate<TerrainCell>) (tc =>
      {
        tc.LogInfo("Filtering Relaxed (replace features)", template.name, 0.0f);
        return tc.IsSafeToSpawnPOIRelaxed(terrainCells) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
      })) : terrainCells.FindAll((Predicate<TerrainCell>) (tc =>
      {
        tc.LogInfo("Filtering", template.name, 0.0f);
        return tc.IsSafeToSpawnPOI(terrainCells) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
      }));
      TemplateSpawning.RemoveOverlappingPOIs(ref filteredTerrainCells, ref terrainCells, ref placedPOIBounds, template, settings, rule.allowExtremeTemperatureOverlap, Vector2I.op_Implicit(rule.overrideOffset));
      if (filteredTerrainCells.Count == 0)
      {
        if (guarantee && !rule.useRelaxedFiltering)
        {
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) ("Could not place " + template.name + " using normal rules, trying relaxed")
          });
          filteredTerrainCells = terrainCells.FindAll((Predicate<TerrainCell>) (tc =>
          {
            tc.LogInfo("Filtering Relaxed", template.name, 0.0f);
            return tc.IsSafeToSpawnPOIRelaxed(terrainCells) && TemplateSpawning.DoesCellMatchFilters(tc, rule.allowedCellsFilter);
          }));
          TemplateSpawning.RemoveOverlappingPOIs(ref filteredTerrainCells, ref terrainCells, ref placedPOIBounds, template, settings, rule.allowExtremeTemperatureOverlap, Vector2I.op_Implicit(rule.overrideOffset));
        }
        if (filteredTerrainCells.Count == 0)
          return (TerrainCell) null;
      }
      WorldGenUtil.ShuffleSeeded<TerrainCell>((IList<TerrainCell>) filteredTerrainCells, myRandom.RandomSource());
      return filteredTerrainCells[filteredTerrainCells.Count - 1];
    }

    private static bool IsPOIOverlappingBounds(
      List<RectInt> placedPOIBounds,
      RectInt templateBounds)
    {
      foreach (RectInt placedPoiBound in placedPOIBounds)
      {
        if (((RectInt) ref templateBounds).Overlaps(placedPoiBound))
          return true;
      }
      return false;
    }

    private static bool IsPOIOverlappingHighTemperatureDelta(
      RectInt paddedTemplateBounds,
      SubWorld subworld,
      ref List<TerrainCell> allCells,
      WorldGenSettings settings)
    {
      Vector2 vector2_1 = Vector2.op_Multiply(Vector2.op_Multiply(2f, Vector2.one), (float) TemplateSpawning.s_poiPadding);
      Vector2 vector2_2 = Vector2.op_Multiply(Vector2.op_Multiply(2f, Vector2.one), 3f);
      Rect rect;
      // ISSUE: explicit constructor call
      ((Rect) ref rect).\u002Ector(Vector2Int.op_Implicit(((RectInt) ref paddedTemplateBounds).position), Vector2.op_Addition(Vector2.op_Subtraction(Vector2Int.op_Implicit(((RectInt) ref paddedTemplateBounds).size), vector2_1), vector2_2));
      Temperature temperature1 = SettingsCache.temperatures[subworld.temperatureRange];
      foreach (TerrainCell terrainCell in allCells)
      {
        SubWorld subWorld = settings.GetSubWorld(((ProcGen.Node) terrainCell.node).GetSubworld());
        Temperature temperature2 = SettingsCache.temperatures[subWorld.temperatureRange];
        if (subWorld.temperatureRange != subworld.temperatureRange)
        {
          float num1 = Mathf.Min(temperature1.min, temperature2.min);
          float num2 = Mathf.Max(temperature1.max, temperature2.max) - num1;
          if (((Rect) ref rect).Overlaps(terrainCell.poly.bounds) & (double) num2 > 100.0)
            return true;
        }
      }
      return false;
    }

    private static void RemoveOverlappingPOIs(
      ref List<TerrainCell> filteredTerrainCells,
      ref List<TerrainCell> allCells,
      ref List<RectInt> placedPOIBounds,
      TemplateContainer container,
      WorldGenSettings settings,
      bool allowExtremeTemperatureOverlap,
      Vector2 poiOffset)
    {
      for (int index1 = filteredTerrainCells.Count - 1; index1 >= 0; --index1)
      {
        TerrainCell terrainCell = filteredTerrainCells[index1];
        int index2 = index1;
        SubWorld subWorld = settings.GetSubWorld(((ProcGen.Node) terrainCell.node).GetSubworld());
        RectInt templateBounds = container.GetTemplateBounds(Vector2.op_Addition(terrainCell.poly.Centroid(), poiOffset), TemplateSpawning.s_poiPadding);
        bool flag = false;
        if (TemplateSpawning.IsPOIOverlappingBounds(placedPOIBounds, templateBounds))
        {
          terrainCell.LogInfo("-> Removed due to overlapping POIs", "", 0.0f);
          flag = true;
        }
        else if (!allowExtremeTemperatureOverlap && TemplateSpawning.IsPOIOverlappingHighTemperatureDelta(templateBounds, subWorld, ref allCells, settings))
        {
          terrainCell.LogInfo("-> Removed due to overlapping extreme temperature delta", "", 0.0f);
          flag = true;
        }
        if (flag)
          filteredTerrainCells.RemoveAt(index2);
      }
    }

    private static bool DoesCellMatchFilters(
      TerrainCell cell,
      List<World.AllowedCellsFilter> filters)
    {
      bool flag1 = false;
      foreach (World.AllowedCellsFilter filter in filters)
      {
        bool applied;
        bool flag2 = TemplateSpawning.DoesCellMatchFilter(cell, filter, out applied);
        if (applied)
        {
          World.AllowedCellsFilter.Command command = filter.command;
          switch ((int) command)
          {
            case 0:
              flag1 = false;
              break;
            case 1:
              flag1 = flag2;
              break;
            case 2:
              flag1 = flag2 | flag1;
              break;
            case 3:
              flag1 = flag2 & flag1;
              break;
            case 4:
            case 5:
              if (flag2)
              {
                flag1 = false;
                break;
              }
              break;
            case 6:
              flag1 = true;
              break;
          }
          TerrainCell terrainCell = cell;
          command = filter.command;
          string evt = "-> DoesCellMatchFilter " + command.ToString();
          string str = flag2 ? "1" : "0";
          double num = flag1 ? 1.0 : 0.0;
          terrainCell.LogInfo(evt, str, (float) num);
        }
      }
      cell.LogInfo("> Final match", flag1 ? "true" : "false", 0.0f);
      return flag1;
    }

    private static bool DoesCellMatchFilter(
      TerrainCell cell,
      World.AllowedCellsFilter filter,
      out bool applied)
    {
      applied = true;
      if (!TemplateSpawning.ValidateFilter(filter))
        return false;
      if (filter.tagcommand != null)
      {
        switch ((int) filter.tagcommand)
        {
          case 0:
            return true;
          case 1:
            return ((ProcGen.Node) cell.node).tags.Contains(Tag.op_Implicit(filter.tag));
          case 2:
            return !((ProcGen.Node) cell.node).tags.Contains(Tag.op_Implicit(filter.tag));
          case 3:
            int num = cell.distancesToTags.ContainsKey(TagExtensions.ToTag(filter.tag)) ? 1 : 0;
            Debug.Assert(num != 0 || filter.optional, (object) ("DistanceFromTag is missing tag " + filter.tag + ", consider marking the filter optional."));
            if (num != 0)
            {
              int tag = cell.DistanceToTag(Tag.op_Implicit(filter.tag));
              return tag >= filter.minDistance && tag <= filter.maxDistance;
            }
            applied = false;
            return true;
        }
      }
      else
      {
        if (filter.subworldNames != null && filter.subworldNames.Count > 0)
        {
          foreach (string subworldName in filter.subworldNames)
          {
            if (((ProcGen.Node) cell.node).tags.Contains(Tag.op_Implicit(subworldName)))
              return true;
          }
          return false;
        }
        if (filter.zoneTypes != null && filter.zoneTypes.Count > 0)
        {
          foreach (SubWorld.ZoneType zoneType in filter.zoneTypes)
          {
            if (((ProcGen.Node) cell.node).tags.Contains(Tag.op_Implicit(zoneType.ToString())))
              return true;
          }
          return false;
        }
        if (filter.temperatureRanges != null && filter.temperatureRanges.Count > 0)
        {
          foreach (Temperature.Range temperatureRange in filter.temperatureRanges)
          {
            if (((ProcGen.Node) cell.node).tags.Contains(Tag.op_Implicit(temperatureRange.ToString())))
              return true;
          }
          return false;
        }
      }
      return true;
    }

    private static bool ValidateFilter(World.AllowedCellsFilter filter)
    {
      if (filter.command == 6)
        return true;
      int num = 0;
      if (filter.tagcommand != null)
        ++num;
      if (filter.subworldNames != null && filter.subworldNames.Count > 0)
        ++num;
      if (filter.zoneTypes != null && filter.zoneTypes.Count > 0)
        ++num;
      if (filter.temperatureRanges != null && filter.temperatureRanges.Count > 0)
        ++num;
      if (num == 1)
        return true;
      string str = "BAD ALLOWED CELLS FILTER in FEATURE RULES!" + "\nA filter can only specify one of `tagcommand`, `subworldNames`, `zoneTypes`, or `temperatureRanges`." + "\nFound a filter with the following:";
      if (filter.tagcommand != null)
        str = str + "\ntagcommand:\n\t" + filter.tagcommand.ToString() + "\ntag:\n\t" + filter.tag;
      if (filter.subworldNames != null && filter.subworldNames.Count > 0)
        str = str + "\nsubworldNames:\n\t" + string.Join(", ", (IEnumerable<string>) filter.subworldNames);
      if (filter.zoneTypes != null && filter.zoneTypes.Count > 0)
        str = str + "\nzoneTypes:\n" + string.Join<SubWorld.ZoneType>(", ", (IEnumerable<SubWorld.ZoneType>) filter.zoneTypes);
      if (filter.temperatureRanges != null && filter.temperatureRanges.Count > 0)
        str = str + "\ntemperatureRanges:\n" + string.Join<Temperature.Range>(", ", (IEnumerable<Temperature.Range>) filter.temperatureRanges);
      Debug.LogError((object) str);
      return false;
    }
  }
}
