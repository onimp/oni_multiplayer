// Decompiled with JetBrains decompiler
// Type: StampTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using TemplateClasses;
using UnityEngine;

public class StampTool : InterfaceTool
{
  public static StampTool Instance;
  private static HashMapObjectPool<Tag, Building> previewPool = new HashMapObjectPool<Tag, Building>(new Func<Tag, Building>(StampTool.InstantiatePreview));
  private static GameObjectPool placerPool = new GameObjectPool(new Func<GameObject>(StampTool.InstantiatePlacer));
  private static Transform previewPoolTransform = (Transform) null;
  private static Transform placerPoolTransform = (Transform) null;
  public TemplateContainer stampTemplate;
  public GameObject PlacerPrefab;
  private bool ready = true;
  private int placementCell = Grid.InvalidCell;
  private bool selectAffected;
  private bool deactivateOnStamp;
  private GameObject rootCellPlacer;
  private List<GameObject> childCellPlacers = new List<GameObject>();
  private List<Building> buildingPreviews = new List<Building>();

  public static void DestroyInstance() => StampTool.Instance = (StampTool) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    StampTool.Instance = this;
  }

  private void Update() => this.RefreshPreview(Grid.PosToCell(this.GetCursorPos()));

  public void Activate(TemplateContainer template, bool SelectAffected = false, bool DeactivateOnStamp = false)
  {
    this.selectAffected = SelectAffected;
    this.deactivateOnStamp = DeactivateOnStamp;
    if (this.stampTemplate == template)
      return;
    this.stampTemplate = template;
    PlayerController.Instance.ActivateTool((InterfaceTool) this);
    ((MonoBehaviour) this).StartCoroutine(this.InitializePlacementVisual());
  }

  private Vector3 GetCursorPos() => PlayerController.GetCursorPos(KInputManager.GetMousePos());

  public override void OnLeftClickDown(Vector3 cursor_pos)
  {
    base.OnLeftClickDown(cursor_pos);
    this.Stamp(Vector2.op_Implicit(cursor_pos));
  }

  private void Stamp(Vector2 pos)
  {
    if (!this.ready)
      return;
    int cell1 = Grid.PosToCell(pos);
    Vector2f size = this.stampTemplate.info.size;
    int x1 = Mathf.FloorToInt((float) (-(double) ((Vector2f) ref size).X / 2.0));
    int cell2 = Grid.OffsetCell(cell1, x1, 0);
    int cell3 = Grid.PosToCell(pos);
    size = this.stampTemplate.info.size;
    int x2 = Mathf.FloorToInt(((Vector2f) ref size).X / 2f);
    int cell4 = Grid.OffsetCell(cell3, x2, 0);
    int cell5 = Grid.PosToCell(pos);
    size = this.stampTemplate.info.size;
    int y1 = 1 + Mathf.FloorToInt((float) (-(double) ((Vector2f) ref size).Y / 2.0));
    int cell6 = Grid.OffsetCell(cell5, 0, y1);
    int cell7 = Grid.PosToCell(pos);
    size = this.stampTemplate.info.size;
    int y2 = 1 + Mathf.FloorToInt(((Vector2f) ref size).Y / 2f);
    int cell8 = Grid.OffsetCell(cell7, 0, y2);
    if (!Grid.IsValidBuildingCell(cell2) || !Grid.IsValidBuildingCell(cell4) || !Grid.IsValidBuildingCell(cell8) || !Grid.IsValidBuildingCell(cell6))
      return;
    this.ready = false;
    bool pauseOnComplete = SpeedControlScreen.Instance.IsPaused;
    if (SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Unpause();
    if (this.stampTemplate.cells != null)
    {
      for (int index = 0; index < this.buildingPreviews.Count; ++index)
        StampTool.ClearTilePreview(this.buildingPreviews[index]);
      List<GameObject> gameObjectList = new List<GameObject>();
      for (int index = 0; index < this.stampTemplate.cells.Count; ++index)
      {
        for (int layer = 0; layer < 34; ++layer)
        {
          GameObject gameObject = Grid.Objects[Grid.XYToCell((int) ((double) pos.x + (double) this.stampTemplate.cells[index].location_x), (int) ((double) pos.y + (double) this.stampTemplate.cells[index].location_y)), layer];
          if (Object.op_Inequality((Object) gameObject, (Object) null) && !gameObjectList.Contains(gameObject))
            gameObjectList.Add(gameObject);
        }
      }
      if (gameObjectList != null)
      {
        foreach (GameObject gameObject in gameObjectList)
        {
          if (Object.op_Inequality((Object) gameObject, (Object) null))
            Util.KDestroyGameObject(gameObject);
        }
      }
    }
    TemplateLoader.Stamp(this.stampTemplate, pos, (System.Action) (() => this.CompleteStamp(pauseOnComplete)));
    if (this.selectAffected)
    {
      DebugBaseTemplateButton.Instance.ClearSelection();
      if (this.stampTemplate.cells != null)
      {
        for (int index = 0; index < this.stampTemplate.cells.Count; ++index)
          DebugBaseTemplateButton.Instance.AddToSelection(Grid.XYToCell((int) ((double) pos.x + (double) this.stampTemplate.cells[index].location_x), (int) ((double) pos.y + (double) this.stampTemplate.cells[index].location_y)));
      }
    }
    if (!this.deactivateOnStamp)
      return;
    this.DeactivateTool();
  }

  private void CompleteStamp(bool pause)
  {
    if (pause)
      SpeedControlScreen.Instance.Pause();
    this.ready = true;
    this.OnDeactivateTool((InterfaceTool) null);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    if (((Component) this).gameObject.activeSelf)
      return;
    this.ReleasePlacementVisual();
    this.placementCell = Grid.InvalidCell;
    this.stampTemplate = (TemplateContainer) null;
  }

  private IEnumerator InitializePlacementVisual()
  {
    this.ReleasePlacementVisual();
    this.rootCellPlacer = StampTool.placerPool.GetInstance();
    for (int index = 0; index < this.stampTemplate.cells.Count; ++index)
    {
      TemplateClasses.Cell cell = this.stampTemplate.cells[index];
      if (cell.location_x != 0 || cell.location_y != 0)
      {
        GameObject instance = StampTool.placerPool.GetInstance();
        instance.transform.SetParent(this.rootCellPlacer.transform);
        instance.transform.localPosition = new Vector3((float) cell.location_x, (float) cell.location_y);
        instance.SetActive(true);
        this.childCellPlacers.Add(instance);
      }
    }
    if (this.stampTemplate.buildings != null)
      yield return (object) this.InitializeBuildingPlacementVisuals();
  }

  private IEnumerator InitializeBuildingPlacementVisuals()
  {
    foreach (Prefab building in this.stampTemplate.buildings)
    {
      Building instance = StampTool.previewPool.GetInstance(Tag.op_Implicit(building.id));
      Rotatable component = ((Component) instance).GetComponent<Rotatable>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.SetOrientation(building.rotationOrientation);
      instance.transform.SetParent(this.rootCellPlacer.transform);
      TransformExtensions.SetLocalPosition(instance.transform, Vector2.op_Implicit(new Vector2((float) building.location_x, (float) building.location_y)));
      ((Component) instance).gameObject.SetActive(true);
      this.buildingPreviews.Add(instance);
    }
    yield return (object) null;
    for (int index = 0; index < this.stampTemplate.buildings.Count; ++index)
    {
      Prefab building = this.stampTemplate.buildings[index];
      Building buildingPreview = this.buildingPreviews[index];
      string str1 = "";
      if ((building.connections & 1) != 0)
        str1 += "L";
      if ((building.connections & 2) != 0)
        str1 += "R";
      if ((building.connections & 4) != 0)
        str1 += "U";
      if ((building.connections & 8) != 0)
        str1 += "D";
      if (str1 == "")
        str1 = "None";
      KBatchedAnimController component = ((Component) buildingPreview).GetComponent<KBatchedAnimController>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.HasAnimation(HashedString.op_Implicit(str1)))
      {
        string str2 = str1 + "_place";
        bool flag = component.HasAnimation(HashedString.op_Implicit(str2));
        component.Play(HashedString.op_Implicit(flag ? str2 : str1), (KAnim.PlayMode) 0);
      }
    }
  }

  private void ReleasePlacementVisual()
  {
    if (Object.op_Equality((Object) this.rootCellPlacer, (Object) null))
      return;
    this.rootCellPlacer.SetActive(false);
    for (int index = this.childCellPlacers.Count - 1; index >= 0; --index)
    {
      GameObject childCellPlacer = this.childCellPlacers[index];
      childCellPlacer.transform.SetParent(StampTool.placerPoolTransform);
      childCellPlacer.transform.localPosition = Vector3.zero;
      childCellPlacer.SetActive(false);
      StampTool.placerPool.ReleaseInstance(childCellPlacer);
      this.childCellPlacers.RemoveAt(index);
    }
    for (int index = this.buildingPreviews.Count - 1; index >= 0; --index)
    {
      Building buildingPreview = this.buildingPreviews[index];
      StampTool.ClearTilePreview(buildingPreview);
      buildingPreview.transform.SetParent(StampTool.previewPoolTransform);
      buildingPreview.transform.localPosition = Vector3.zero;
      ((Component) buildingPreview).gameObject.SetActive(false);
      StampTool.previewPool.ReleaseInstance(Tag.op_Implicit(buildingPreview.Def.PrefabID), buildingPreview);
      this.buildingPreviews.RemoveAt(index);
    }
    this.rootCellPlacer.transform.SetParent(StampTool.placerPoolTransform);
    this.rootCellPlacer.transform.position = Vector3.zero;
    StampTool.placerPool.ReleaseInstance(this.rootCellPlacer);
    this.rootCellPlacer = (GameObject) null;
  }

  private static void ClearTilePreview(Building b)
  {
    int cell = Grid.PosToCell(b.transform.position);
    if (!b.Def.IsTilePiece || !Grid.IsValidBuildingCell(cell))
      return;
    if (Object.op_Equality((Object) ((Component) b).gameObject, (Object) Grid.Objects[cell, (int) b.Def.TileLayer]))
      Grid.Objects[cell, (int) b.Def.TileLayer] = (GameObject) null;
    if (!b.Def.isKAnimTile)
      return;
    if (Object.op_Inequality((Object) b.Def.BlockTileAtlas, (Object) null))
      World.Instance.blockTileRenderer.RemoveBlock(b.Def, false, SimHashes.Void, cell);
    TileVisualizer.RefreshCell(cell, b.Def.TileLayer, ObjectLayer.NumLayers);
  }

  private static void UpdateTileRendering(int newCell, Building b)
  {
    StampTool.ClearTilePreview(b);
    if (!b.Def.IsTilePiece || !Grid.IsValidBuildingCell(newCell))
      return;
    if (Object.op_Equality((Object) Grid.Objects[newCell, (int) b.Def.TileLayer], (Object) null))
      Grid.Objects[newCell, (int) b.Def.TileLayer] = ((Component) b).gameObject;
    if (!b.Def.isKAnimTile)
      return;
    if (Object.op_Inequality((Object) b.Def.BlockTileAtlas, (Object) null))
      World.Instance.blockTileRenderer.AddBlock(((Component) b).gameObject.layer, b.Def, false, SimHashes.Void, newCell);
    TileVisualizer.RefreshCell(newCell, b.Def.TileLayer, ObjectLayer.NumLayers);
  }

  public void RefreshPreview(int new_placement_cell)
  {
    if (!Grid.IsValidCell(new_placement_cell) || new_placement_cell == this.placementCell)
      return;
    for (int index = 0; index < this.buildingPreviews.Count; ++index)
    {
      Building buildingPreview = this.buildingPreviews[index];
      Vector3 localPosition = buildingPreview.transform.localPosition;
      StampTool.UpdateTileRendering(Grid.OffsetCell(new_placement_cell, (int) localPosition.x, (int) localPosition.y), buildingPreview);
    }
    this.placementCell = new_placement_cell;
    TransformExtensions.SetPosition(this.rootCellPlacer.transform, Grid.CellToPosCBC(this.placementCell, this.visualizerLayer));
    this.rootCellPlacer.SetActive(true);
  }

  private static Building InstantiatePreview(Tag previewId)
  {
    GameObject prefab = Assets.TryGetPrefab(previewId);
    if (Object.op_Equality((Object) prefab, (Object) null))
      return (Building) null;
    Building component1 = prefab.GetComponent<Building>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return (Building) null;
    if (Object.op_Equality((Object) StampTool.previewPoolTransform, (Object) null))
      StampTool.previewPoolTransform = new GameObject("Preview Pool").transform;
    GameObject buildingPreview = component1.Def.BuildingPreview;
    if (Object.op_Equality((Object) buildingPreview, (Object) null))
      buildingPreview = BuildingLoader.Instance.CreateBuildingPreview(component1.Def);
    int layer = LayerMask.NameToLayer("Place");
    Building component2 = GameUtil.KInstantiate(buildingPreview, Vector3.zero, Grid.SceneLayer.Ore, gameLayer: layer).GetComponent<Building>();
    KBatchedAnimController component3 = ((Component) component2).GetComponent<KBatchedAnimController>();
    if (Object.op_Inequality((Object) component3, (Object) null))
    {
      component3.visibilityType = KAnimControllerBase.VisibilityType.Always;
      component3.isMovable = true;
      component3.Offset = component1.Def.GetVisualizerOffset();
      ((Object) component3).name = ((Component) component3).GetComponent<KPrefabID>().GetDebugName() + "_visualizer";
      component3.TintColour = Color32.op_Implicit(Color.white);
      component3.SetLayer(layer);
    }
    component2.transform.SetParent(StampTool.previewPoolTransform);
    ((Component) component2).gameObject.SetActive(false);
    return component2;
  }

  private static GameObject InstantiatePlacer()
  {
    if (Object.op_Equality((Object) StampTool.placerPoolTransform, (Object) null))
      StampTool.placerPoolTransform = new GameObject("Stamp Placer Pool").transform;
    GameObject gameObject = Util.KInstantiate(StampTool.Instance.PlacerPrefab, ((Component) StampTool.placerPoolTransform).gameObject, (string) null);
    gameObject.SetActive(false);
    return gameObject;
  }
}
