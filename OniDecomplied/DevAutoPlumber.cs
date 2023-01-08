// Decompiled with JetBrains decompiler
// Type: DevAutoPlumber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class DevAutoPlumber
{
  public static void AutoPlumbBuilding(Building building)
  {
    DevAutoPlumber.DoElectricalPlumbing(building);
    DevAutoPlumber.DoLiquidAndGasPlumbing(building);
    DevAutoPlumber.SetupSolidOreDelivery(building);
  }

  public static void DoElectricalPlumbing(Building building)
  {
    if (!building.Def.RequiresPowerInput)
      return;
    int cell = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) building), building.Def.PowerInputOffset);
    GameObject gameObject = Grid.Objects[cell, 26];
    if (Object.op_Inequality((Object) gameObject, (Object) null))
      EventExtensions.Trigger(gameObject, -790448070, (object) null);
    DevAutoPlumber.PlaceSourceAndUtilityConduit(building, Assets.GetBuildingDef("DevGenerator"), Assets.GetBuildingDef("WireRefined"), (IUtilityNetworkMgr) Game.Instance.electricalConduitSystem, new int[2]
    {
      26,
      29
    }, DevAutoPlumber.PortSelection.PowerInput);
  }

  public static void DoLiquidAndGasPlumbing(Building building)
  {
    DevAutoPlumber.SetupPlumbingInput(building);
    DevAutoPlumber.SetupPlumbingOutput(building);
  }

  public static void SetupSolidOreDelivery(Building building)
  {
    ManualDeliveryKG component = ((Component) building).GetComponent<ManualDeliveryKG>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      Object.op_Equality((Object) DevAutoPlumber.TrySpawnElementOreFromTag(component.RequestedItemTag, Grid.PosToCell((KMonoBehaviour) building), component.Capacity), (Object) null);
    }
    else
    {
      foreach (ComplexRecipe recipe in ComplexRecipeManager.Get().recipes)
      {
        foreach (Tag fabricator in recipe.fabricators)
        {
          if (Tag.op_Equality(fabricator, Tag.op_Implicit(building.Def.PrefabID)))
          {
            foreach (ComplexRecipe.RecipeElement ingredient in recipe.ingredients)
              Object.op_Equality((Object) DevAutoPlumber.TrySpawnElementOreFromTag(ingredient.material, Grid.PosToCell((KMonoBehaviour) building), ingredient.amount * 10f), (Object) null);
          }
        }
      }
    }
  }

  private static GameObject TrySpawnElementOreFromTag(Tag t, int cell, float amount)
  {
    Element element = ElementLoader.GetElement(t) ?? ElementLoader.elements.Find((Predicate<Element>) (match => match.HasTag(t)));
    return element?.substance.SpawnResource(Grid.CellToPos(cell), amount, element.defaultValues.temperature, byte.MaxValue, 0);
  }

  private static void SetupPlumbingInput(Building building)
  {
    ConduitConsumer component = ((Component) building).GetComponent<ConduitConsumer>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    BuildingDef sourceDef = (BuildingDef) null;
    BuildingDef conduitDef = (BuildingDef) null;
    int[] conduitTypeLayers = (int[]) null;
    UtilityNetworkManager<FlowUtilityNetwork, Vent> utlityNetworkManager = (UtilityNetworkManager<FlowUtilityNetwork, Vent>) null;
    switch (component.ConduitType)
    {
      case ConduitType.Gas:
        conduitDef = Assets.GetBuildingDef("InsulatedGasConduit");
        sourceDef = Assets.GetBuildingDef("DevPumpGas");
        utlityNetworkManager = Game.Instance.gasConduitSystem;
        conduitTypeLayers = new int[2]{ 12, 15 };
        break;
      case ConduitType.Liquid:
        conduitDef = Assets.GetBuildingDef("InsulatedLiquidConduit");
        sourceDef = Assets.GetBuildingDef("DevPumpLiquid");
        utlityNetworkManager = Game.Instance.liquidConduitSystem;
        conduitTypeLayers = new int[2]{ 16, 19 };
        break;
    }
    GameObject gameObject = DevAutoPlumber.PlaceSourceAndUtilityConduit(building, sourceDef, conduitDef, (IUtilityNetworkMgr) utlityNetworkManager, conduitTypeLayers, DevAutoPlumber.PortSelection.UtilityInput);
    Element element = DevAutoPlumber.GuessMostRelevantElementForPump(building);
    if (element != null)
      gameObject.GetComponent<DevPump>().SelectedTag = element.tag;
    else
      gameObject.GetComponent<DevPump>().SelectedTag = ElementLoader.FindElementByHash(SimHashes.Vacuum).tag;
  }

  private static void SetupPlumbingOutput(Building building)
  {
    ConduitDispenser component = ((Component) building).GetComponent<ConduitDispenser>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    BuildingDef sourceDef = (BuildingDef) null;
    BuildingDef conduitDef = (BuildingDef) null;
    int[] conduitTypeLayers = (int[]) null;
    UtilityNetworkManager<FlowUtilityNetwork, Vent> utlityNetworkManager = (UtilityNetworkManager<FlowUtilityNetwork, Vent>) null;
    switch (component.ConduitType)
    {
      case ConduitType.Gas:
        conduitDef = Assets.GetBuildingDef("InsulatedGasConduit");
        sourceDef = Assets.GetBuildingDef("GasVent");
        utlityNetworkManager = Game.Instance.gasConduitSystem;
        conduitTypeLayers = new int[2]{ 12, 15 };
        break;
      case ConduitType.Liquid:
        conduitDef = Assets.GetBuildingDef("InsulatedLiquidConduit");
        sourceDef = Assets.GetBuildingDef("LiquidVent");
        utlityNetworkManager = Game.Instance.liquidConduitSystem;
        conduitTypeLayers = new int[2]{ 16, 19 };
        break;
    }
    DevAutoPlumber.PlaceSourceAndUtilityConduit(building, sourceDef, conduitDef, (IUtilityNetworkMgr) utlityNetworkManager, conduitTypeLayers, DevAutoPlumber.PortSelection.UtilityOutput);
  }

  private static Element GuessMostRelevantElementForPump(Building destinationBuilding)
  {
    ConduitConsumer consumer = ((Component) destinationBuilding).GetComponent<ConduitConsumer>();
    Tag targetTag = ((Component) destinationBuilding).GetComponent<ConduitConsumer>().capacityTag;
    ElementConverter elementConverter = ((Component) destinationBuilding).GetComponent<ElementConverter>();
    ElementConsumer elementConsumer = ((Component) destinationBuilding).GetComponent<ElementConsumer>();
    RocketEngineCluster rocketEngineCluster = ((Component) destinationBuilding).GetComponent<RocketEngineCluster>();
    return ElementLoader.elements.Find((Predicate<Element>) (match =>
    {
      if (Object.op_Inequality((Object) elementConverter, (Object) null))
      {
        bool flag = false;
        for (int index = 0; index < elementConverter.consumedElements.Length; ++index)
        {
          if (Tag.op_Equality(elementConverter.consumedElements[index].Tag, match.tag))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
          return false;
      }
      else if (Object.op_Inequality((Object) elementConsumer, (Object) null))
      {
        bool flag = false;
        if (Tag.op_Equality(ElementLoader.FindElementByHash(elementConsumer.elementToConsume).tag, match.tag))
          flag = true;
        if (!flag)
          return false;
      }
      else if (Object.op_Inequality((Object) rocketEngineCluster, (Object) null))
      {
        bool flag = false;
        if (Tag.op_Equality(rocketEngineCluster.fuelTag, match.tag))
          flag = true;
        if (!flag)
          return false;
      }
      return (consumer.ConduitType != ConduitType.Liquid || match.IsLiquid) && (consumer.ConduitType != ConduitType.Gas || match.IsGas) && (match.HasTag(targetTag) || !Tag.op_Inequality(targetTag, GameTags.Any));
    }));
  }

  private static GameObject PlaceSourceAndUtilityConduit(
    Building destinationBuilding,
    BuildingDef sourceDef,
    BuildingDef conduitDef,
    IUtilityNetworkMgr utlityNetworkManager,
    int[] conduitTypeLayers,
    DevAutoPlumber.PortSelection portSelection)
  {
    Building building = (Building) null;
    List<int> rejectLocations = new List<int>();
    int placementLocation = DevAutoPlumber.FindClearPlacementLocation(Grid.PosToCell((KMonoBehaviour) destinationBuilding), new List<int>((IEnumerable<int>) conduitTypeLayers)
    {
      1
    }.ToArray(), rejectLocations);
    bool flag = false;
    int num = 10;
    while (!flag)
    {
      --num;
      building = DevAutoPlumber.PlaceConduitSourceBuilding(placementLocation, sourceDef);
      if (Object.op_Equality((Object) building, (Object) null))
        return (GameObject) null;
      List<int> clearConduitPath = DevAutoPlumber.GenerateClearConduitPath(building, destinationBuilding, conduitTypeLayers, portSelection);
      if (clearConduitPath == null)
      {
        rejectLocations.Add(Grid.PosToCell((KMonoBehaviour) building));
        building.Trigger(-790448070, (object) null);
      }
      else
      {
        flag = true;
        DevAutoPlumber.BuildConduits(clearConduitPath, conduitDef, (object) utlityNetworkManager);
      }
    }
    return ((Component) building).gameObject;
  }

  private static int FindClearPlacementLocation(
    int nearStartingCell,
    int[] placementBlockingObjectLayers,
    List<int> rejectLocations)
  {
    Func<int, object, bool> fn = (Func<int, object, bool>) ((test, unusedData) =>
    {
      int[] numArray = new int[6]
      {
        test,
        Grid.OffsetCell(test, 1, 0),
        Grid.OffsetCell(test, 1, -1),
        Grid.OffsetCell(test, 0, -1),
        Grid.OffsetCell(test, 0, 1),
        Grid.OffsetCell(test, 1, 1)
      };
      foreach (int num in numArray)
      {
        if (!Grid.IsValidCell(num) || Grid.Solid[num] || Grid.ObjectLayers[1].ContainsKey(num))
          return false;
        foreach (int blockingObjectLayer in placementBlockingObjectLayers)
        {
          if (Grid.ObjectLayers[blockingObjectLayer].ContainsKey(num))
            return false;
        }
        if (rejectLocations.Contains(test))
          return false;
      }
      return true;
    });
    int num1 = 20;
    int start_cell = nearStartingCell;
    int max_depth = num1;
    return GameUtil.FloodFillFind<object>(fn, (object) null, start_cell, max_depth, false, false);
  }

  private static List<int> GenerateClearConduitPath(
    Building sourceBuilding,
    Building destinationBuilding,
    int[] conduitTypeLayers,
    DevAutoPlumber.PortSelection portSelection)
  {
    List<int> intList = new List<int>();
    if (Object.op_Equality((Object) sourceBuilding, (Object) null))
      return (List<int>) null;
    int conduitStart = -1;
    int conduitEnd = -1;
    switch (portSelection)
    {
      case DevAutoPlumber.PortSelection.UtilityInput:
        conduitStart = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) sourceBuilding), sourceBuilding.Def.UtilityOutputOffset);
        conduitEnd = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) destinationBuilding), destinationBuilding.Def.UtilityInputOffset);
        break;
      case DevAutoPlumber.PortSelection.UtilityOutput:
        conduitStart = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) destinationBuilding), destinationBuilding.Def.UtilityOutputOffset);
        conduitEnd = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) sourceBuilding), sourceBuilding.Def.UtilityInputOffset);
        break;
      case DevAutoPlumber.PortSelection.PowerInput:
        conduitStart = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) sourceBuilding), sourceBuilding.Def.PowerOutputOffset);
        conduitEnd = Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) destinationBuilding), destinationBuilding.Def.PowerInputOffset);
        break;
    }
    return DevAutoPlumber.GetGridPath(conduitStart, conduitEnd, (Func<int, bool>) (cell =>
    {
      if (!Grid.IsValidCell(cell))
        return false;
      foreach (int conduitTypeLayer in conduitTypeLayers)
      {
        GameObject gameObject = Grid.Objects[cell, conduitTypeLayer];
        bool flag1 = Object.op_Equality((Object) gameObject, (Object) ((Component) sourceBuilding).gameObject) || Object.op_Equality((Object) gameObject, (Object) ((Component) destinationBuilding).gameObject);
        bool flag2 = cell == conduitEnd || cell == conduitStart;
        if (Object.op_Inequality((Object) gameObject, (Object) null) && (!flag1 || flag1 && !flag2))
          return false;
      }
      return true;
    }));
  }

  private static Building PlaceConduitSourceBuilding(int cell, BuildingDef def)
  {
    List<Tag> tagList = new List<Tag>();
    tagList.Add(SimHashes.Cuprite.CreateTag());
    List<Tag> selected_elements = tagList;
    return def.Build(cell, Orientation.Neutral, (Storage) null, (IList<Tag>) selected_elements, 273.15f, timeBuilt: GameClock.Instance.GetTime()).GetComponent<Building>();
  }

  private static void BuildConduits(List<int> path, BuildingDef conduitDef, object utilityNetwork)
  {
    List<Tag> tagList = new List<Tag>();
    tagList.Add(SimHashes.Cuprite.CreateTag());
    List<Tag> selected_elements = tagList;
    List<GameObject> gameObjectList = new List<GameObject>();
    for (int index = 0; index < path.Count; ++index)
      gameObjectList.Add(conduitDef.Build(path[index], Orientation.Neutral, (Storage) null, (IList<Tag>) selected_elements, 273.15f, timeBuilt: GameClock.Instance.GetTime()));
    if (gameObjectList.Count < 2)
      return;
    IUtilityNetworkMgr utilityNetworkMgr = (IUtilityNetworkMgr) utilityNetwork;
    for (int index = 1; index < gameObjectList.Count; ++index)
    {
      UtilityConnections cell = UtilityConnectionsExtensions.DirectionFromToCell(Grid.PosToCell(gameObjectList[index - 1]), Grid.PosToCell(gameObjectList[index]));
      utilityNetworkMgr.AddConnection(cell, Grid.PosToCell(gameObjectList[index - 1]), true);
      utilityNetworkMgr.AddConnection(cell.InverseDirection(), Grid.PosToCell(gameObjectList[index]), true);
      gameObjectList[index].GetComponent<KAnimGraphTileVisualizer>()?.UpdateConnections(utilityNetworkMgr.GetConnections(Grid.PosToCell(gameObjectList[index]), true));
    }
  }

  private static List<int> GetGridPath(
    int startCell,
    int endCell,
    Func<int, bool> testFunction,
    int maxDepth = 20)
  {
    List<int> gridPath = new List<int>();
    List<int> frontier = new List<int>();
    List<int> touched = new List<int>();
    Dictionary<int, int> crumbs = new Dictionary<int, int>();
    frontier.Add(startCell);
    List<int> newFrontier = new List<int>();
    int num1 = 0;
    while (!touched.Contains(endCell))
    {
      ++num1;
      if (num1 <= maxDepth && frontier.Count != 0)
      {
        foreach (int fromCell in frontier)
          _ExpandFrontier(fromCell);
        frontier.Clear();
        foreach (int num2 in newFrontier)
          frontier.Add(num2);
        newFrontier.Clear();
      }
      else
        break;
    }
    int key = endCell;
    gridPath.Add(key);
    while (crumbs.ContainsKey(key))
    {
      key = crumbs[key];
      gridPath.Add(key);
    }
    gridPath.Reverse();
    return gridPath;

    void _ExpandFrontier(int fromCell)
    {
      int[] numArray = new int[4]
      {
        Grid.CellAbove(fromCell),
        Grid.CellBelow(fromCell),
        Grid.CellLeft(fromCell),
        Grid.CellRight(fromCell)
      };
      foreach (int key in numArray)
      {
        if (!newFrontier.Contains(key) && !frontier.Contains(key) && !touched.Contains(key) && testFunction(key))
        {
          newFrontier.Add(key);
          crumbs.Add(key, fromCell);
        }
        touched.Add(key);
        if (key == endCell)
          break;
      }
      touched.Add(fromCell);
    }
  }

  private enum PortSelection
  {
    UtilityInput,
    UtilityOutput,
    PowerInput,
  }
}
