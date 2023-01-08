// Decompiled with JetBrains decompiler
// Type: DevTool_StoryTraits_Reveal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System.Collections.Generic;
using UnityEngine;

public class DevTool_StoryTraits_Reveal : DevTool
{
  protected override void RenderTo(DevPanel panel)
  {
    Option<int> forUniqueBuilding = DevToolUtil.GetCellIndexForUniqueBuilding("Headquarters");
    bool hasValue1 = forUniqueBuilding.HasValue;
    if (ImGuiEx.Button("Focus on headquaters", hasValue1))
      DevToolUtil.FocusCameraOnCell((int) forUniqueBuilding);
    if (!hasValue1)
      ImGuiEx.TooltipForPrevious("Couldn't find headquaters");
    if (!ImGui.CollapsingHeader("Search world for entity", (ImGuiTreeNodeFlags) 32))
      return;
    Option<IReadOnlyList<WorldGenSpawner.Spawnable>> allSpawnables = this.GetAllSpawnables();
    if (!allSpawnables.HasValue)
    {
      ImGui.Text("Couldn't find a list of spawnables");
    }
    else
    {
      foreach (string prefabId in this.GetPrefabIDsToSearchFor())
      {
        Option<int> indexForSpawnable = this.GetCellIndexForSpawnable(prefabId, allSpawnables.Value);
        string str = "\"" + prefabId + "\"";
        bool hasValue2 = indexForSpawnable.HasValue;
        if (ImGuiEx.Button("Reveal and focus on " + str, hasValue2))
          DevToolUtil.RevealAndFocusAt(indexForSpawnable.Value);
        if (!hasValue2)
          ImGuiEx.TooltipForPrevious("Couldn't find a cell that contained a spawnable with component " + str);
      }
    }
  }

  public IEnumerable<string> GetPrefabIDsToSearchFor()
  {
    yield return "MegaBrainTank";
    yield return "GravitasCreatureManipulator";
  }

  private Option<ClusterManager> GetClusterManager() => Object.op_Equality((Object) ClusterManager.Instance, (Object) null) ? (Option<ClusterManager>) Option.None : (Option<ClusterManager>) ClusterManager.Instance;

  private Option<int> GetCellIndexForSpawnable(
    string prefabId,
    IReadOnlyList<WorldGenSpawner.Spawnable> spawnablesToSearch)
  {
    foreach (WorldGenSpawner.Spawnable spawnable in (IEnumerable<WorldGenSpawner.Spawnable>) spawnablesToSearch)
    {
      if (prefabId == spawnable.spawnInfo.id)
        return (Option<int>) spawnable.cell;
    }
    return (Option<int>) Option.None;
  }

  private Option<IReadOnlyList<WorldGenSpawner.Spawnable>> GetAllSpawnables()
  {
    WorldGenSpawner objectOfType = Object.FindObjectOfType<WorldGenSpawner>(true);
    if (Object.op_Equality((Object) objectOfType, (Object) null))
      return (Option<IReadOnlyList<WorldGenSpawner.Spawnable>>) Option.None;
    IReadOnlyList<WorldGenSpawner.Spawnable> spawnables = objectOfType.GetSpawnables();
    return spawnables == null ? (Option<IReadOnlyList<WorldGenSpawner.Spawnable>>) Option.None : Option.Some<IReadOnlyList<WorldGenSpawner.Spawnable>>(spawnables);
  }
}
