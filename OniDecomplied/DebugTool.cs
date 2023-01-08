// Decompiled with JetBrains decompiler
// Type: DebugTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugTool : DragTool
{
  public static DebugTool Instance;
  public DebugTool.Type type;

  public static void DestroyInstance() => DebugTool.Instance = (DebugTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    DebugTool.Instance = this;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  public void Activate(DebugTool.Type type)
  {
    this.type = type;
    this.Activate();
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    PlayerController.Instance.ToolDeactivated((InterfaceTool) this);
  }

  protected override void OnDragTool(int cell, int distFromOrigin)
  {
    if (!Grid.IsValidCell(cell))
      return;
    switch (this.type)
    {
      case DebugTool.Type.ReplaceSubstance:
        this.DoReplaceSubstance(cell);
        break;
      case DebugTool.Type.FillReplaceSubstance:
        GameUtil.FloodFillNext.Clear();
        GameUtil.FloodFillVisited.Clear();
        SimHashes elem_hash = Grid.Element[cell].id;
        GameUtil.FloodFillConditional(cell, (Func<int, bool>) (check_cell =>
        {
          bool flag = false;
          if (Grid.Element[check_cell].id == elem_hash)
          {
            flag = true;
            this.DoReplaceSubstance(check_cell);
          }
          return flag;
        }), (ICollection<int>) GameUtil.FloodFillVisited);
        break;
      case DebugTool.Type.Clear:
        this.ClearCell(cell);
        break;
      case DebugTool.Type.AddSelection:
        DebugBaseTemplateButton.Instance.AddToSelection(cell);
        break;
      case DebugTool.Type.RemoveSelection:
        DebugBaseTemplateButton.Instance.RemoveFromSelection(cell);
        break;
      case DebugTool.Type.Deconstruct:
        this.DeconstructCell(cell);
        break;
      case DebugTool.Type.Destroy:
        this.DestroyCell(cell);
        break;
      case DebugTool.Type.Sample:
        DebugPaintElementScreen.Instance.SampleCell(cell);
        break;
      case DebugTool.Type.StoreSubstance:
        this.DoStoreSubstance(cell);
        break;
      case DebugTool.Type.Dig:
        SimMessages.Dig(cell);
        break;
      case DebugTool.Type.Heat:
        SimMessages.ModifyEnergy(cell, 10000f, 10000f, SimMessages.EnergySourceID.DebugHeat);
        break;
      case DebugTool.Type.Cool:
        SimMessages.ModifyEnergy(cell, -10000f, 10000f, SimMessages.EnergySourceID.DebugCool);
        break;
      case DebugTool.Type.AddPressure:
        SimMessages.ModifyMass(cell, 10000f, byte.MaxValue, 0, CellEventLogger.Instance.DebugToolModifyMass, 293f, SimHashes.Oxygen);
        break;
      case DebugTool.Type.RemovePressure:
        SimMessages.ModifyMass(cell, -10000f, byte.MaxValue, 0, CellEventLogger.Instance.DebugToolModifyMass, 0.0f, SimHashes.Oxygen);
        break;
    }
  }

  public void DoReplaceSubstance(int cell)
  {
    if (!Grid.IsValidBuildingCell(cell))
      return;
    Element element = (DebugPaintElementScreen.Instance.paintElement.isOn ? ElementLoader.FindElementByHash(DebugPaintElementScreen.Instance.element) : ElementLoader.elements[(int) Grid.ElementIdx[cell]]) ?? ElementLoader.FindElementByHash(SimHashes.Vacuum);
    byte num1 = DebugPaintElementScreen.Instance.paintDisease.isOn ? DebugPaintElementScreen.Instance.diseaseIdx : Grid.DiseaseIdx[cell];
    float temperature = DebugPaintElementScreen.Instance.paintTemperature.isOn ? DebugPaintElementScreen.Instance.temperature : Grid.Temperature[cell];
    float mass = DebugPaintElementScreen.Instance.paintMass.isOn ? DebugPaintElementScreen.Instance.mass : Grid.Mass[cell];
    int num2 = DebugPaintElementScreen.Instance.paintDiseaseCount.isOn ? DebugPaintElementScreen.Instance.diseaseCount : Grid.DiseaseCount[cell];
    if ((double) temperature == -1.0)
      temperature = element.defaultValues.temperature;
    if ((double) mass == -1.0)
      mass = element.defaultValues.mass;
    if (DebugPaintElementScreen.Instance.affectCells.isOn)
    {
      SimMessages.ReplaceElement(cell, element.id, CellEventLogger.Instance.DebugTool, mass, temperature, num1, num2);
      if (DebugPaintElementScreen.Instance.set_prevent_fow_reveal)
      {
        Grid.Visible[cell] = (byte) 0;
        Grid.PreventFogOfWarReveal[cell] = true;
      }
      else if (DebugPaintElementScreen.Instance.set_allow_fow_reveal && Grid.PreventFogOfWarReveal[cell])
        Grid.PreventFogOfWarReveal[cell] = false;
    }
    if (!DebugPaintElementScreen.Instance.affectBuildings.isOn)
      return;
    List<GameObject> gameObjectList = new List<GameObject>();
    gameObjectList.Add(Grid.Objects[cell, 1]);
    gameObjectList.Add(Grid.Objects[cell, 2]);
    gameObjectList.Add(Grid.Objects[cell, 9]);
    gameObjectList.Add(Grid.Objects[cell, 16]);
    gameObjectList.Add(Grid.Objects[cell, 12]);
    gameObjectList.Add(Grid.Objects[cell, 16]);
    gameObjectList.Add(Grid.Objects[cell, 26]);
    foreach (GameObject gameObject in gameObjectList)
    {
      if (Object.op_Inequality((Object) gameObject, (Object) null))
      {
        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
        if ((double) temperature > 0.0)
          component.Temperature = temperature;
        if (num2 > 0 && num1 != byte.MaxValue)
        {
          component.ModifyDiseaseCount(int.MinValue, "DebugTool.DoReplaceSubstance");
          component.AddDisease(num1, num2, "DebugTool.DoReplaceSubstance");
        }
      }
    }
  }

  public void DeconstructCell(int cell)
  {
    int num = DebugHandler.InstantBuildMode ? 1 : 0;
    DebugHandler.InstantBuildMode = true;
    DeconstructTool.Instance.DeconstructCell(cell);
    if (num != 0)
      return;
    DebugHandler.InstantBuildMode = false;
  }

  public void DestroyCell(int cell)
  {
    List<GameObject> gameObjectList = new List<GameObject>();
    gameObjectList.Add(Grid.Objects[cell, 2]);
    gameObjectList.Add(Grid.Objects[cell, 1]);
    gameObjectList.Add(Grid.Objects[cell, 12]);
    gameObjectList.Add(Grid.Objects[cell, 16]);
    gameObjectList.Add(Grid.Objects[cell, 0]);
    gameObjectList.Add(Grid.Objects[cell, 26]);
    foreach (GameObject gameObject in gameObjectList)
    {
      if (Object.op_Inequality((Object) gameObject, (Object) null))
        Object.Destroy((Object) gameObject);
    }
    this.ClearCell(cell);
    if (ElementLoader.elements[(int) Grid.ElementIdx[cell]].id == SimHashes.Void)
      SimMessages.ReplaceElement(cell, SimHashes.Void, CellEventLogger.Instance.DebugTool, 0.0f, 0.0f);
    else
      SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.DebugTool, 0.0f, 0.0f);
  }

  public void ClearCell(int cell)
  {
    Vector2I xy = Grid.CellToXY(cell);
    ListPool<ScenePartitionerEntry, DebugTool>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, DebugTool>.Allocate();
    GameScenePartitioner.Instance.GatherEntries(xy.x, xy.y, 1, 1, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
    for (int index = 0; index < ((List<ScenePartitionerEntry>) gathered_entries).Count; ++index)
    {
      Pickupable pickupable = ((List<ScenePartitionerEntry>) gathered_entries)[index].obj as Pickupable;
      if (Object.op_Inequality((Object) pickupable, (Object) null) && Object.op_Equality((Object) ((Component) pickupable).GetComponent<MinionBrain>(), (Object) null))
        Util.KDestroyGameObject(((Component) pickupable).gameObject);
    }
    gathered_entries.Recycle();
  }

  public void DoStoreSubstance(int cell)
  {
    if (!Grid.IsValidBuildingCell(cell))
      return;
    GameObject gameObject = Grid.Objects[cell, 1];
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    Storage component = gameObject.GetComponent<Storage>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    Element element = (DebugPaintElementScreen.Instance.paintElement.isOn ? ElementLoader.FindElementByHash(DebugPaintElementScreen.Instance.element) : ElementLoader.elements[(int) Grid.ElementIdx[cell]]) ?? ElementLoader.FindElementByHash(SimHashes.Vacuum);
    byte disease_idx = DebugPaintElementScreen.Instance.paintDisease.isOn ? DebugPaintElementScreen.Instance.diseaseIdx : Grid.DiseaseIdx[cell];
    float temperature = DebugPaintElementScreen.Instance.paintTemperature.isOn ? DebugPaintElementScreen.Instance.temperature : element.defaultValues.temperature;
    float mass = DebugPaintElementScreen.Instance.paintMass.isOn ? DebugPaintElementScreen.Instance.mass : element.defaultValues.mass;
    if ((double) temperature == -1.0)
      temperature = element.defaultValues.temperature;
    if ((double) mass == -1.0)
      mass = element.defaultValues.mass;
    int disease_count = DebugPaintElementScreen.Instance.paintDiseaseCount.isOn ? DebugPaintElementScreen.Instance.diseaseCount : 0;
    if (element.IsGas)
      component.AddGasChunk(element.id, mass, temperature, disease_idx, disease_count, false);
    else if (element.IsLiquid)
    {
      component.AddLiquid(element.id, mass, temperature, disease_idx, disease_count);
    }
    else
    {
      if (!element.IsSolid)
        return;
      component.AddOre(element.id, mass, temperature, disease_idx, disease_count);
    }
  }

  public enum Type
  {
    ReplaceSubstance,
    FillReplaceSubstance,
    Clear,
    AddSelection,
    RemoveSelection,
    Deconstruct,
    Destroy,
    Sample,
    StoreSubstance,
    Dig,
    Heat,
    Cool,
    AddPressure,
    RemovePressure,
    PaintPlant,
  }
}
