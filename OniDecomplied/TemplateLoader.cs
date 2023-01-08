// Decompiled with JetBrains decompiler
// Type: TemplateLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using TemplateClasses;
using UnityEngine;

public static class TemplateLoader
{
  private static List<TemplateLoader.ActiveStamp> activeStamps = new List<TemplateLoader.ActiveStamp>();

  public static void Stamp(
    TemplateContainer template,
    Vector2 rootLocation,
    System.Action on_complete_callback)
  {
    TemplateLoader.ActiveStamp activeStamp = new TemplateLoader.ActiveStamp(template, rootLocation, on_complete_callback);
    TemplateLoader.activeStamps.Add(activeStamp);
  }

  private static void StampComplete(TemplateLoader.ActiveStamp stamp) => TemplateLoader.activeStamps.Remove(stamp);

  private static void BuildPhase1(
    int baseX,
    int baseY,
    TemplateContainer template,
    System.Action callback)
  {
    if (Grid.WidthInCells < 16)
      return;
    if (template.cells == null)
    {
      callback();
    }
    else
    {
      CellOffset[] cellOffsetArray = new CellOffset[template.cells.Count];
      for (int index = 0; index < template.cells.Count; ++index)
        cellOffsetArray[index] = new CellOffset(template.cells[index].location_x, template.cells[index].location_y);
      TemplateLoader.ClearPickups(baseX, baseY, cellOffsetArray);
      if (template.cells.Count > 0)
      {
        TemplateLoader.ApplyGridProperties(baseX, baseY, template);
        TemplateLoader.PlaceCells(baseX, baseY, template, callback);
        TemplateLoader.ClearEntities<Crop>(baseX, baseY, cellOffsetArray);
        TemplateLoader.ClearEntities<Health>(baseX, baseY, cellOffsetArray);
        TemplateLoader.ClearEntities<Geyser>(baseX, baseY, cellOffsetArray);
      }
      else
        callback();
    }
  }

  private static void BuildPhase2(
    int baseX,
    int baseY,
    TemplateContainer template,
    System.Action callback)
  {
    int num = Grid.OffsetCell(0, baseX, baseY);
    if (template == null)
      Debug.LogError((object) "No stamp template");
    if (template.buildings != null)
    {
      for (int index = 0; index < template.buildings.Count; ++index)
        TemplateLoader.PlaceBuilding(template.buildings[index], num);
    }
    HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(callback));
    SimMessages.ReplaceElement(num, ElementLoader.elements[(int) Grid.ElementIdx[num]].id, CellEventLogger.Instance.TemplateLoader, Grid.Mass[num], Grid.Temperature[num], Grid.DiseaseIdx[num], Grid.DiseaseCount[num], handle.index);
    handle.index = -1;
  }

  public static GameObject PlaceBuilding(Prefab prefab, int root_cell)
  {
    if (prefab == null || prefab.id == "")
      return (GameObject) null;
    BuildingDef buildingDef = Assets.GetBuildingDef(prefab.id);
    if (Object.op_Equality((Object) buildingDef, (Object) null))
      return (GameObject) null;
    int locationX = prefab.location_x;
    int locationY = prefab.location_y;
    if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, locationX, locationY)))
      return (GameObject) null;
    if (buildingDef.WidthInCells >= 3)
      --locationX;
    GameObject gameObject1 = Scenario.PlaceBuilding(root_cell, locationX, locationY, prefab.id, prefab.element);
    if (Object.op_Equality((Object) gameObject1, (Object) null))
    {
      Debug.LogWarning((object) ("Null prefab for " + prefab.id));
      return gameObject1;
    }
    BuildingComplete component1 = gameObject1.GetComponent<BuildingComplete>();
    gameObject1.GetComponent<KPrefabID>().AddTag(GameTags.TemplateBuilding, true);
    Components.TemplateBuildings.Add(component1);
    Rotatable component2 = gameObject1.GetComponent<Rotatable>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      component2.SetOrientation(prefab.rotationOrientation);
    PrimaryElement component3 = ((Component) component1).GetComponent<PrimaryElement>();
    if ((double) prefab.temperature > 0.0)
      component3.Temperature = prefab.temperature;
    component3.AddDisease(Db.Get().Diseases.GetIndex(HashedString.op_Implicit(prefab.diseaseName)), prefab.diseaseCount, "TemplateLoader.PlaceBuilding");
    if (prefab.id == "Door")
    {
      for (int index = 0; index < component1.PlacementCells.Length; ++index)
        SimMessages.ReplaceElement(component1.PlacementCells[index], SimHashes.Vacuum, CellEventLogger.Instance.TemplateLoader, 0.0f, 0.0f);
    }
    if (prefab.amounts != null)
    {
      foreach (Prefab.template_amount_value amount in prefab.amounts)
      {
        try
        {
          if (Db.Get().Amounts.Get(amount.id) != null)
            gameObject1.GetAmounts().SetValue(amount.id, amount.value);
        }
        catch
        {
          Debug.LogWarning((object) string.Format("Building does not have amount with ID {0}", (object) amount.id));
        }
      }
    }
    if (prefab.other_values != null)
    {
      foreach (Prefab.template_amount_value otherValue in prefab.other_values)
      {
        switch (otherValue.id)
        {
          case "joulesAvailable":
            Battery component4 = gameObject1.GetComponent<Battery>();
            if (Object.op_Implicit((Object) component4))
            {
              component4.AddEnergy(otherValue.value);
              break;
            }
            break;
          case "sealedDoorDirection":
            Unsealable component5 = gameObject1.GetComponent<Unsealable>();
            if (Object.op_Implicit((Object) component5))
            {
              component5.facingRight = (double) otherValue.value != 0.0;
              break;
            }
            break;
          case "switchSetting":
            LogicSwitch s = gameObject1.GetComponent<LogicSwitch>();
            if (Object.op_Implicit((Object) s) && (s.IsSwitchedOn && (double) otherValue.value == 0.0 || !s.IsSwitchedOn && (double) otherValue.value == 1.0))
            {
              s.SetFirstFrameCallback((System.Action) (() => s.HandleToggle()));
              break;
            }
            break;
        }
      }
    }
    if (prefab.storage != null && prefab.storage.Count > 0)
    {
      Storage component6 = ((Component) component1).gameObject.GetComponent<Storage>();
      if (Object.op_Equality((Object) component6, (Object) null))
        Debug.LogWarning((object) ("No storage component on stampTemplate building " + prefab.id + ". Saved storage contents will be ignored."));
      for (int index = 0; index < prefab.storage.Count; ++index)
      {
        StorageItem storageItem = prefab.storage[index];
        string id = storageItem.id;
        GameObject go;
        if (storageItem.isOre)
        {
          go = ElementLoader.FindElementByHash(storageItem.element).substance.SpawnResource(Vector3.zero, storageItem.units, storageItem.temperature, Db.Get().Diseases.GetIndex(HashedString.op_Implicit(storageItem.diseaseName)), storageItem.diseaseCount);
        }
        else
        {
          go = Scenario.SpawnPrefab(root_cell, 0, 0, id);
          if (Object.op_Equality((Object) go, (Object) null))
          {
            Debug.LogWarning((object) ("Null prefab for " + id));
            continue;
          }
          go.SetActive(true);
          PrimaryElement component7 = go.GetComponent<PrimaryElement>();
          component7.Units = storageItem.units;
          component7.Temperature = storageItem.temperature;
          component7.AddDisease(Db.Get().Diseases.GetIndex(HashedString.op_Implicit(storageItem.diseaseName)), storageItem.diseaseCount, "TemplateLoader.PlaceBuilding");
          Rottable.Instance smi = go.GetSMI<Rottable.Instance>();
          if (smi != null)
            smi.RotValue = storageItem.rottable.rotAmount;
        }
        GameObject gameObject2 = component6.Store(go, true, true);
        if (Object.op_Inequality((Object) gameObject2, (Object) null))
          gameObject2.GetComponent<Pickupable>().OnStore((object) component6);
      }
    }
    if (prefab.connections != 0)
      TemplateLoader.PlaceUtilityConnection(gameObject1, prefab, root_cell);
    return gameObject1;
  }

  public static void PlaceUtilityConnection(GameObject spawned, Prefab bc, int root_cell)
  {
    int cell = Grid.OffsetCell(root_cell, bc.location_x, bc.location_y);
    UtilityConnections connection = (UtilityConnections) bc.connections;
    string id = bc.id;
    if (id == null)
      return;
    // ISSUE: reference to a compiler-generated method
    switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(id))
    {
      case 301047391:
        if (!(id == "WireRefined"))
          return;
        break;
      case 379600269:
        if (!(id == "LiquidConduit"))
          return;
        goto label_27;
      case 609727380:
        if (!(id == "GasConduit"))
          return;
        goto label_26;
      case 848332507:
        if (!(id == "InsulatedGasConduit"))
          return;
        goto label_26;
      case 1213766155:
        if (!(id == "TravelTube"))
          return;
        spawned.GetComponent<TravelTube>().SetFirstFrameCallback((System.Action) (() =>
        {
          Game.Instance.travelTubeSystem.SetConnections(connection, cell, true);
          KAnimGraphTileVisualizer component = spawned.GetComponent<KAnimGraphTileVisualizer>();
          if (!Object.op_Inequality((Object) component, (Object) null))
            return;
          component.Refresh();
        }));
        return;
      case 1827504487:
        if (!(id == "InsulatedWire"))
          return;
        break;
      case 1938276536:
        if (!(id == "Wire"))
          return;
        break;
      case 3228988836:
        if (!(id == "LogicWire"))
          return;
        spawned.GetComponent<LogicWire>().SetFirstFrameCallback((System.Action) (() =>
        {
          Game.Instance.logicCircuitSystem.SetConnections(connection, cell, true);
          KAnimGraphTileVisualizer component = spawned.GetComponent<KAnimGraphTileVisualizer>();
          if (!Object.op_Inequality((Object) component, (Object) null))
            return;
          component.Refresh();
        }));
        return;
      case 3711470516:
        if (!(id == "InsulatedLiquidConduit"))
          return;
        goto label_27;
      case 3716494409:
        if (!(id == "HighWattageWire"))
          return;
        break;
      case 4113070310:
        if (!(id == "SolidConduit"))
          return;
        spawned.GetComponent<SolidConduit>().SetFirstFrameCallback((System.Action) (() =>
        {
          Game.Instance.solidConduitSystem.SetConnections(connection, cell, true);
          KAnimGraphTileVisualizer component = spawned.GetComponent<KAnimGraphTileVisualizer>();
          if (!Object.op_Inequality((Object) component, (Object) null))
            return;
          component.Refresh();
        }));
        return;
      default:
        return;
    }
    spawned.GetComponent<Wire>().SetFirstFrameCallback((System.Action) (() =>
    {
      Game.Instance.electricalConduitSystem.SetConnections(connection, cell, true);
      KAnimGraphTileVisualizer component = spawned.GetComponent<KAnimGraphTileVisualizer>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.Refresh();
    }));
    return;
label_26:
    spawned.GetComponent<Conduit>().SetFirstFrameCallback((System.Action) (() =>
    {
      Game.Instance.gasConduitSystem.SetConnections(connection, cell, true);
      KAnimGraphTileVisualizer component = spawned.GetComponent<KAnimGraphTileVisualizer>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.Refresh();
    }));
    return;
label_27:
    spawned.GetComponent<Conduit>().SetFirstFrameCallback((System.Action) (() =>
    {
      Game.Instance.liquidConduitSystem.SetConnections(connection, cell, true);
      KAnimGraphTileVisualizer component = spawned.GetComponent<KAnimGraphTileVisualizer>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.Refresh();
    }));
  }

  public static GameObject PlacePickupables(Prefab prefab, int root_cell)
  {
    int locationX = prefab.location_x;
    int locationY = prefab.location_y;
    if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, locationX, locationY)))
      return (GameObject) null;
    GameObject go = Scenario.SpawnPrefab(root_cell, locationX, locationY, prefab.id);
    if (Object.op_Equality((Object) go, (Object) null))
    {
      Debug.LogWarning((object) ("Null prefab for " + prefab.id));
      return (GameObject) null;
    }
    go.SetActive(true);
    if ((double) prefab.units != 0.0)
    {
      PrimaryElement component = go.GetComponent<PrimaryElement>();
      component.Units = prefab.units;
      component.Temperature = (double) prefab.temperature > 0.0 ? prefab.temperature : component.Element.defaultValues.temperature;
      component.AddDisease(Db.Get().Diseases.GetIndex(HashedString.op_Implicit(prefab.diseaseName)), prefab.diseaseCount, "TemplateLoader.PlacePickupables");
    }
    Rottable.Instance smi = go.GetSMI<Rottable.Instance>();
    if (smi != null)
      smi.RotValue = prefab.rottable.rotAmount;
    return go;
  }

  public static GameObject PlaceOtherEntities(Prefab prefab, int root_cell)
  {
    int locationX = prefab.location_x;
    int locationY = prefab.location_y;
    if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, locationX, locationY)))
      return (GameObject) null;
    GameObject prefab1 = Assets.GetPrefab(new Tag(prefab.id));
    if (Object.op_Equality((Object) prefab1, (Object) null))
      return (GameObject) null;
    Grid.SceneLayer scene_layer = Grid.SceneLayer.Front;
    KBatchedAnimController component = prefab1.GetComponent<KBatchedAnimController>();
    if (Object.op_Inequality((Object) component, (Object) null))
      scene_layer = component.sceneLayer;
    GameObject go = Scenario.SpawnPrefab(root_cell, locationX, locationY, prefab.id, scene_layer);
    if (Object.op_Equality((Object) go, (Object) null))
    {
      Debug.LogWarning((object) ("Null prefab for " + prefab.id));
      return (GameObject) null;
    }
    go.SetActive(true);
    if (prefab.amounts != null)
    {
      foreach (Prefab.template_amount_value amount in prefab.amounts)
      {
        try
        {
          go.GetAmounts().SetValue(amount.id, amount.value);
        }
        catch
        {
          Debug.LogWarning((object) string.Format("Entity {0} does not have amount with ID {1}", (object) go.GetProperName(), (object) amount.id));
        }
      }
    }
    return go;
  }

  public static GameObject PlaceElementalOres(Prefab prefab, int root_cell)
  {
    int locationX = prefab.location_x;
    int locationY = prefab.location_y;
    if (!Grid.IsValidCell(Grid.OffsetCell(root_cell, locationX, locationY)))
      return (GameObject) null;
    Substance substance = ElementLoader.FindElementByHash(prefab.element).substance;
    Vector3 posCcc = Grid.CellToPosCCC(Grid.OffsetCell(root_cell, locationX, locationY), Grid.SceneLayer.Ore);
    byte index = Db.Get().Diseases.GetIndex(HashedString.op_Implicit(prefab.diseaseName));
    if ((double) prefab.temperature <= 0.0)
    {
      Debug.LogWarning((object) "Template trying to spawn zero temperature substance!");
      prefab.temperature = 300f;
    }
    Vector3 position = posCcc;
    double units = (double) prefab.units;
    double temperature = (double) prefab.temperature;
    int disease_idx = (int) index;
    int diseaseCount = prefab.diseaseCount;
    return substance.SpawnResource(position, (float) units, (float) temperature, (byte) disease_idx, diseaseCount);
  }

  private static void BuildPhase3(
    int baseX,
    int baseY,
    TemplateContainer template,
    System.Action callback)
  {
    if (template != null)
    {
      int root_cell = Grid.OffsetCell(0, baseX, baseY);
      foreach (Component component1 in Components.BuildingCompletes.Items)
      {
        KAnimGraphTileVisualizer component2 = component1.GetComponent<KAnimGraphTileVisualizer>();
        if (Object.op_Inequality((Object) component2, (Object) null))
          component2.Refresh();
      }
      if (template.pickupables != null)
      {
        for (int index = 0; index < template.pickupables.Count; ++index)
        {
          if (template.pickupables[index] != null && !(template.pickupables[index].id == ""))
            TemplateLoader.PlacePickupables(template.pickupables[index], root_cell);
        }
      }
      if (template.elementalOres != null)
      {
        for (int index = 0; index < template.elementalOres.Count; ++index)
        {
          if (template.elementalOres[index] != null && !(template.elementalOres[index].id == ""))
            TemplateLoader.PlaceElementalOres(template.elementalOres[index], root_cell);
        }
      }
    }
    if (callback == null)
      return;
    callback();
  }

  private static void BuildPhase4(
    int baseX,
    int baseY,
    TemplateContainer template,
    System.Action callback)
  {
    if (template != null)
    {
      int root_cell = Grid.OffsetCell(0, baseX, baseY);
      if (template.otherEntities != null)
      {
        for (int index = 0; index < template.otherEntities.Count; ++index)
        {
          if (template.otherEntities[index] != null && !(template.otherEntities[index].id == ""))
            TemplateLoader.PlaceOtherEntities(template.otherEntities[index], root_cell);
        }
      }
      template = (TemplateContainer) null;
    }
    if (callback == null)
      return;
    callback();
  }

  private static void ClearPickups(int baseX, int baseY, CellOffset[] template_as_offsets)
  {
    if (Object.op_Inequality((Object) SaveGame.Instance.worldGenSpawner, (Object) null))
      SaveGame.Instance.worldGenSpawner.ClearSpawnersInArea(new Vector2((float) baseX, (float) baseY), template_as_offsets);
    foreach (Pickupable pickupable in Components.Pickupables.Items)
    {
      if (Grid.IsCellOffsetOf(Grid.XYToCell(baseX, baseY), ((Component) pickupable).gameObject, template_as_offsets))
        Util.KDestroyGameObject(((Component) pickupable).gameObject);
    }
  }

  private static void ClearEntities<T>(int rootX, int rootY, CellOffset[] TemplateOffsets) where T : KMonoBehaviour
  {
    foreach (T obj in (T[]) Object.FindObjectsOfType(typeof (T)))
    {
      if (Grid.IsCellOffsetOf(Grid.PosToCell(((Component) (object) obj).gameObject), Grid.XYToCell(rootX, rootY), TemplateOffsets))
        Util.KDestroyGameObject(((Component) (object) obj).gameObject);
    }
  }

  private static void PlaceCells(
    int baseX,
    int baseY,
    TemplateContainer template,
    System.Action callback)
  {
    if (template == null)
      Debug.LogError((object) "Template Loader does not have template.");
    if (template.cells == null)
    {
      callback();
    }
    else
    {
      HandleVector<Game.CallbackInfo>.Handle handle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(callback));
      for (int index1 = 0; index1 < template.cells.Count; ++index1)
      {
        int cell = Grid.XYToCell(template.cells[index1].location_x + baseX, template.cells[index1].location_y + baseY);
        if (!Grid.IsValidCell(cell))
          Debug.LogError((object) string.Format("Trying to replace invalid cells cell{0} root{1}:{2} offset{3}:{4}", (object) cell, (object) baseX, (object) baseY, (object) template.cells[index1].location_x, (object) template.cells[index1].location_y));
        SimHashes element = template.cells[index1].element;
        float mass = template.cells[index1].mass;
        float temperature = template.cells[index1].temperature;
        byte index2 = Db.Get().Diseases.GetIndex(HashedString.op_Implicit(template.cells[index1].diseaseName));
        int diseaseCount = template.cells[index1].diseaseCount;
        SimMessages.ReplaceElement(cell, element, CellEventLogger.Instance.TemplateLoader, mass, temperature, index2, diseaseCount, handle.index);
        handle.index = -1;
      }
    }
  }

  public static void ApplyGridProperties(int baseX, int baseY, TemplateContainer template)
  {
    if (template.cells == null)
      return;
    for (int index = 0; index < template.cells.Count; ++index)
    {
      int cell = Grid.XYToCell(template.cells[index].location_x + baseX, template.cells[index].location_y + baseY);
      if (Grid.IsValidCell(cell) && template.cells[index].preventFoWReveal)
      {
        Grid.PreventFogOfWarReveal[cell] = true;
        Grid.Visible[cell] = (byte) 0;
      }
    }
  }

  private class ActiveStamp
  {
    private TemplateContainer m_template;
    private Vector2I m_rootLocation;
    private System.Action m_onCompleteCallback;
    private int currentPhase;

    public ActiveStamp(TemplateContainer template, Vector2 rootLocation, System.Action onCompleteCallback)
    {
      this.m_template = template;
      this.m_rootLocation = new Vector2I((int) rootLocation.x, (int) rootLocation.y);
      this.m_onCompleteCallback = onCompleteCallback;
      this.NextPhase();
    }

    private void NextPhase()
    {
      ++this.currentPhase;
      switch (this.currentPhase)
      {
        case 1:
          TemplateLoader.BuildPhase1(this.m_rootLocation.x, this.m_rootLocation.y, this.m_template, new System.Action(this.NextPhase));
          break;
        case 2:
          TemplateLoader.BuildPhase2(this.m_rootLocation.x, this.m_rootLocation.y, this.m_template, new System.Action(this.NextPhase));
          break;
        case 3:
          TemplateLoader.BuildPhase3(this.m_rootLocation.x, this.m_rootLocation.y, this.m_template, new System.Action(this.NextPhase));
          break;
        case 4:
          TemplateLoader.BuildPhase4(this.m_rootLocation.x, this.m_rootLocation.y, this.m_template, new System.Action(this.NextPhase));
          break;
        case 5:
          this.m_onCompleteCallback();
          TemplateLoader.StampComplete(this);
          break;
        default:
          Debug.Assert(false, (object) "How did we get here?? Something's wrong!");
          break;
      }
    }
  }
}
