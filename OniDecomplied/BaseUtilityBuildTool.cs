// Decompiled with JetBrains decompiler
// Type: BaseUtilityBuildTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUtilityBuildTool : DragTool
{
  private IList<Tag> selectedElements;
  private BuildingDef def;
  protected List<BaseUtilityBuildTool.PathNode> path = new List<BaseUtilityBuildTool.PathNode>();
  protected IUtilityNetworkMgr conduitMgr;
  private Coroutine visUpdater;
  private int buildingCount;
  private int lastCell = -1;
  private BuildingCellVisualizer previousCellConnection;
  private int previousCell;

  protected override void OnPrefabInit()
  {
    this.buildingCount = Random.Range(1, 14);
    this.canChangeDragAxis = false;
  }

  private void Play(GameObject go, string anim) => go.GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit(anim));

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    this.visualizer = GameUtil.KInstantiate(this.def.BuildingPreview, PlayerController.GetCursorPos(KInputManager.GetMousePos()), Grid.SceneLayer.Ore, gameLayer: LayerMask.NameToLayer("Place"));
    KBatchedAnimController component = this.visualizer.GetComponent<KBatchedAnimController>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      component.visibilityType = KAnimControllerBase.VisibilityType.Always;
      component.isMovable = true;
      component.SetDirty();
    }
    this.visualizer.SetActive(true);
    this.Play(this.visualizer, "None_Place");
    ((Component) this).GetComponent<BuildToolHoverTextCard>().currentDef = this.def;
    ResourceRemainingDisplayScreen.instance.ActivateDisplay(this.visualizer);
    this.conduitMgr = this.def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>().GetNetworkManager();
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    this.StopVisUpdater();
    ResourceRemainingDisplayScreen.instance.DeactivateDisplay();
    if (Object.op_Inequality((Object) this.visualizer, (Object) null))
      Object.Destroy((Object) this.visualizer);
    base.OnDeactivateTool(new_tool);
  }

  public void Activate(BuildingDef def, IList<Tag> selected_elements)
  {
    this.selectedElements = selected_elements;
    this.def = def;
    this.viewMode = def.ViewMode;
    PlayerController.Instance.ActivateTool((InterfaceTool) this);
    ResourceRemainingDisplayScreen.instance.SetResources(selected_elements, def.CraftRecipe);
  }

  protected override void OnDragTool(int cell, int distFromOrigin)
  {
    if (this.path.Count == 0 || this.path[this.path.Count - 1].cell == cell)
      return;
    this.placeSound = GlobalAssets.GetSound("Place_building_" + this.def.AudioSize);
    EventInstance instance = SoundEvent.BeginOneShot(this.placeSound, Grid.CellToPos(cell));
    if (this.path.Count > 1 && cell == this.path[this.path.Count - 2].cell)
    {
      if (Object.op_Inequality((Object) this.previousCellConnection, (Object) null))
      {
        this.previousCellConnection.ConnectedEvent(this.previousCell);
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("OutletDisconnected"));
        this.previousCellConnection = (BuildingCellVisualizer) null;
      }
      this.previousCell = cell;
      this.CheckForConnection(cell, this.def.PrefabID, "", ref this.previousCellConnection, false);
      Object.Destroy((Object) this.path[this.path.Count - 1].visualizer);
      TileVisualizer.RefreshCell(this.path[this.path.Count - 1].cell, this.def.TileLayer, this.def.ReplacementLayer);
      this.path.RemoveAt(this.path.Count - 1);
      this.buildingCount = this.buildingCount == 1 ? (this.buildingCount = 14) : this.buildingCount - 1;
      ((EventInstance) ref instance).setParameterByName("tileCount", (float) this.buildingCount, false);
      SoundEvent.EndOneShot(instance);
    }
    else if (!this.path.Exists((Predicate<BaseUtilityBuildTool.PathNode>) (n => n.cell == cell)))
    {
      bool flag = this.CheckValidPathPiece(cell);
      this.path.Add(new BaseUtilityBuildTool.PathNode()
      {
        cell = cell,
        visualizer = (GameObject) null,
        valid = flag
      });
      this.CheckForConnection(cell, this.def.PrefabID, "OutletConnected", ref this.previousCellConnection);
      this.buildingCount = this.buildingCount % 14 + 1;
      ((EventInstance) ref instance).setParameterByName("tileCount", (float) this.buildingCount, false);
      SoundEvent.EndOneShot(instance);
    }
    this.visualizer.SetActive(this.path.Count < 2);
    ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(this.path.Count);
  }

  protected override int GetDragLength() => this.path.Count;

  private bool CheckValidPathPiece(int cell)
  {
    if (this.def.BuildLocationRule == BuildLocationRule.NotInTiles && (Object.op_Inequality((Object) Grid.Objects[cell, 9], (Object) null) || Grid.HasDoor[cell]))
      return false;
    GameObject gameObject1 = Grid.Objects[cell, (int) this.def.ObjectLayer];
    if (Object.op_Inequality((Object) gameObject1, (Object) null) && Object.op_Equality((Object) gameObject1.GetComponent<KAnimGraphTileVisualizer>(), (Object) null))
      return false;
    GameObject gameObject2 = Grid.Objects[cell, (int) this.def.TileLayer];
    return !Object.op_Inequality((Object) gameObject2, (Object) null) || !Object.op_Equality((Object) gameObject2.GetComponent<KAnimGraphTileVisualizer>(), (Object) null);
  }

  private bool CheckForConnection(
    int cell,
    string defName,
    string soundName,
    ref BuildingCellVisualizer outBcv,
    bool fireEvents = true)
  {
    outBcv = (BuildingCellVisualizer) null;
    DebugUtil.Assert(defName != null, "defName was null");
    Building building = this.GetBuilding(cell);
    if (!Object.op_Implicit((Object) building))
      return false;
    DebugUtil.Assert(Object.op_Implicit((Object) ((Component) building).gameObject), "targetBuilding.gameObject was null");
    int num1 = -1;
    int num2 = -1;
    int num3 = -1;
    if (defName.Contains("LogicWire"))
    {
      LogicPorts component = ((Component) building).gameObject.GetComponent<LogicPorts>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        if (component.inputPorts != null)
        {
          foreach (ILogicUIElement inputPort in component.inputPorts)
          {
            DebugUtil.Assert(inputPort != null, "input port was null");
            if (inputPort.GetLogicUICell() == cell)
            {
              num1 = cell;
              break;
            }
          }
        }
        if (num1 == -1 && component.outputPorts != null)
        {
          foreach (ILogicUIElement outputPort in component.outputPorts)
          {
            DebugUtil.Assert(outputPort != null, "output port was null");
            if (outputPort.GetLogicUICell() == cell)
            {
              num2 = cell;
              break;
            }
          }
        }
      }
    }
    else if (defName.Contains("Wire"))
    {
      num1 = building.GetPowerInputCell();
      num2 = building.GetPowerOutputCell();
    }
    else if (defName.Contains("Liquid"))
    {
      if (building.Def.InputConduitType == ConduitType.Liquid)
        num1 = building.GetUtilityInputCell();
      if (building.Def.OutputConduitType == ConduitType.Liquid)
        num2 = building.GetUtilityOutputCell();
      ElementFilter component = ((Component) building).GetComponent<ElementFilter>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        DebugUtil.Assert(component.portInfo != null, "elementFilter.portInfo was null A");
        if (component.portInfo.conduitType == ConduitType.Liquid)
          num3 = component.GetFilteredCell();
      }
    }
    else if (defName.Contains("Gas"))
    {
      if (building.Def.InputConduitType == ConduitType.Gas)
        num1 = building.GetUtilityInputCell();
      if (building.Def.OutputConduitType == ConduitType.Gas)
        num2 = building.GetUtilityOutputCell();
      ElementFilter component = ((Component) building).GetComponent<ElementFilter>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        DebugUtil.Assert(component.portInfo != null, "elementFilter.portInfo was null B");
        if (component.portInfo.conduitType == ConduitType.Gas)
          num3 = component.GetFilteredCell();
      }
    }
    if (cell == num1 || cell == num2 || cell == num3)
    {
      BuildingCellVisualizer component = ((Component) building).gameObject.GetComponent<BuildingCellVisualizer>();
      outBcv = component;
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        if (fireEvents)
        {
          component.ConnectedEvent(cell);
          string sound = GlobalAssets.GetSound(soundName);
          if (sound != null)
            KMonoBehaviour.PlaySound(sound);
        }
        return true;
      }
    }
    outBcv = (BuildingCellVisualizer) null;
    return false;
  }

  private Building GetBuilding(int cell)
  {
    GameObject gameObject = Grid.Objects[cell, 1];
    return Object.op_Inequality((Object) gameObject, (Object) null) ? gameObject.GetComponent<Building>() : (Building) null;
  }

  protected override DragTool.Mode GetMode() => DragTool.Mode.Brush;

  public override void OnLeftClickDown(Vector3 cursor_pos)
  {
    if (Object.op_Equality((Object) this.visualizer, (Object) null))
      return;
    this.path.Clear();
    int cell = Grid.PosToCell(cursor_pos);
    if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
    {
      bool flag = this.CheckValidPathPiece(cell);
      this.path.Add(new BaseUtilityBuildTool.PathNode()
      {
        cell = cell,
        visualizer = (GameObject) null,
        valid = flag
      });
      this.CheckForConnection(cell, this.def.PrefabID, "OutletConnected", ref this.previousCellConnection);
    }
    this.visUpdater = ((MonoBehaviour) this).StartCoroutine(this.VisUpdater());
    this.visualizer.GetComponent<KBatchedAnimController>().StopAndClear();
    ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(1);
    this.placeSound = GlobalAssets.GetSound("Place_building_" + this.def.AudioSize);
    if (this.placeSound != null)
    {
      this.buildingCount = this.buildingCount % 14 + 1;
      EventInstance instance = SoundEvent.BeginOneShot(this.placeSound, Grid.CellToPos(cell));
      if (this.def.AudioSize == "small")
        ((EventInstance) ref instance).setParameterByName("tileCount", (float) this.buildingCount, false);
      SoundEvent.EndOneShot(instance);
    }
    base.OnLeftClickDown(cursor_pos);
  }

  public override void OnLeftClickUp(Vector3 cursor_pos)
  {
    if (Object.op_Equality((Object) this.visualizer, (Object) null))
      return;
    this.BuildPath();
    this.StopVisUpdater();
    this.Play(this.visualizer, "None_Place");
    ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
    base.OnLeftClickUp(cursor_pos);
  }

  public override void OnMouseMove(Vector3 cursorPos)
  {
    base.OnMouseMove(cursorPos);
    int cell = Grid.PosToCell(cursorPos);
    if (this.lastCell != cell)
      this.lastCell = cell;
    if (!Object.op_Inequality((Object) this.visualizer, (Object) null))
      return;
    Color c = Color.white;
    float strength = 0.0f;
    if (!this.def.IsValidPlaceLocation(this.visualizer, cell, Orientation.Neutral, out string _))
    {
      c = Color.red;
      strength = 1f;
    }
    this.SetColor(this.visualizer, c, strength);
  }

  private void SetColor(GameObject root, Color c, float strength)
  {
    KBatchedAnimController component = root.GetComponent<KBatchedAnimController>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.TintColour = Color32.op_Implicit(c);
  }

  protected virtual void ApplyPathToConduitSystem() => DebugUtil.Assert(false, "I don't think this function ever runs");

  private IEnumerator VisUpdater()
  {
    while (true)
    {
      this.conduitMgr.StashVisualGrids();
      if (this.path.Count == 1)
        this.path[0] = this.CreateVisualizer(this.path[0]);
      this.ApplyPathToConduitSystem();
      for (int index = 0; index < this.path.Count; ++index)
      {
        BaseUtilityBuildTool.PathNode visualizer = this.CreateVisualizer(this.path[index]);
        this.path[index] = visualizer;
        string anim = this.conduitMgr.GetVisualizerString(visualizer.cell) + "_place";
        KBatchedAnimController component = visualizer.visualizer.GetComponent<KBatchedAnimController>();
        if (component.HasAnimation(HashedString.op_Implicit(anim)))
          visualizer.Play(anim);
        else
          visualizer.Play(this.conduitMgr.GetVisualizerString(visualizer.cell));
        component.TintColour = Color32.op_Implicit(this.def.IsValidBuildLocation((GameObject) null, visualizer.cell, Orientation.Neutral, false, out string _) ? Color.white : Color.red);
        TileVisualizer.RefreshCell(visualizer.cell, this.def.TileLayer, this.def.ReplacementLayer);
      }
      this.conduitMgr.UnstashVisualGrids();
      yield return (object) null;
    }
  }

  private void BuildPath()
  {
    this.ApplyPathToConduitSystem();
    int connectionCount = 0;
    bool flag1 = false;
    for (int index = 0; index < this.path.Count; ++index)
    {
      BaseUtilityBuildTool.PathNode pathNode = this.path[index];
      Vector3 posCbc = Grid.CellToPosCBC(pathNode.cell, Grid.SceneLayer.Building);
      UtilityConnections utilityConnections1 = (UtilityConnections) 0;
      GameObject go = Grid.Objects[pathNode.cell, (int) this.def.TileLayer];
      UtilityConnections utilityConnections2;
      if (Object.op_Equality((Object) go, (Object) null))
      {
        utilityConnections2 = this.conduitMgr.GetConnections(pathNode.cell, false);
        if ((DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild) && this.def.IsValidBuildLocation(this.visualizer, posCbc, Orientation.Neutral) && this.def.IsValidPlaceLocation(this.visualizer, posCbc, Orientation.Neutral, out string _))
        {
          go = this.def.Build(pathNode.cell, Orientation.Neutral, (Storage) null, this.selectedElements, 293.15f, timeBuilt: GameClock.Instance.GetTime());
        }
        else
        {
          go = this.def.TryPlace((GameObject) null, posCbc, Orientation.Neutral, this.selectedElements);
          if (Object.op_Inequality((Object) go, (Object) null))
          {
            if (!this.def.MaterialsAvailable(this.selectedElements, ClusterManager.Instance.activeWorld) && !DebugHandler.InstantBuildMode)
              PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, (string) UI.TOOLTIPS.NOMATERIAL, (Transform) null, posCbc);
            Constructable component = go.GetComponent<Constructable>();
            if (component.IconConnectionAnimation(0.1f * (float) connectionCount, connectionCount, "Wire", "OutletConnected_release") || component.IconConnectionAnimation(0.1f * (float) connectionCount, connectionCount, "Pipe", "OutletConnected_release"))
              ++connectionCount;
            flag1 = true;
          }
        }
      }
      else
      {
        IUtilityItem component = (IUtilityItem) go.GetComponent<KAnimGraphTileVisualizer>();
        if (component != null)
          utilityConnections1 = component.Connections;
        utilityConnections2 = utilityConnections1 | this.conduitMgr.GetConnections(pathNode.cell, false);
        if (Object.op_Inequality((Object) go.GetComponent<BuildingComplete>(), (Object) null))
          component.UpdateConnections(utilityConnections2);
      }
      if (this.def.ReplacementLayer != ObjectLayer.NumLayers && !DebugHandler.InstantBuildMode && (!Game.Instance.SandboxModeActive || !SandboxToolParameterMenu.instance.settings.InstantBuild) && this.def.IsValidBuildLocation((GameObject) null, posCbc, Orientation.Neutral))
      {
        GameObject gameObject1 = Grid.Objects[pathNode.cell, (int) this.def.TileLayer];
        GameObject gameObject2 = Grid.Objects[pathNode.cell, (int) this.def.ReplacementLayer];
        if (Object.op_Inequality((Object) gameObject1, (Object) null) && Object.op_Equality((Object) gameObject2, (Object) null))
        {
          BuildingComplete component1 = gameObject1.GetComponent<BuildingComplete>();
          bool flag2 = Tag.op_Inequality(gameObject1.GetComponent<PrimaryElement>().Element.tag, this.selectedElements[0]);
          if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) component1.Def, (Object) this.def) | flag2)
          {
            Constructable component2 = this.def.BuildingUnderConstruction.GetComponent<Constructable>();
            component2.IsReplacementTile = true;
            go = this.def.Instantiate(posCbc, Orientation.Neutral, this.selectedElements);
            component2.IsReplacementTile = false;
            if (!this.def.MaterialsAvailable(this.selectedElements, ClusterManager.Instance.activeWorld) && !DebugHandler.InstantBuildMode)
              PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, (string) UI.TOOLTIPS.NOMATERIAL, (Transform) null, posCbc);
            Grid.Objects[pathNode.cell, (int) this.def.ReplacementLayer] = go;
            IUtilityItem component3 = (IUtilityItem) go.GetComponent<KAnimGraphTileVisualizer>();
            if (component3 != null)
              utilityConnections2 = component3.Connections;
            utilityConnections2 |= this.conduitMgr.GetConnections(pathNode.cell, false);
            if (Object.op_Inequality((Object) go.GetComponent<BuildingComplete>(), (Object) null))
              component3.UpdateConnections(utilityConnections2);
            string visualizerString = this.conduitMgr.GetVisualizerString(utilityConnections2);
            string anim = visualizerString;
            if (go.GetComponent<KBatchedAnimController>().HasAnimation(HashedString.op_Implicit(visualizerString + "_place")))
              anim += "_place";
            this.Play(go, anim);
            flag1 = true;
          }
        }
      }
      if (Object.op_Inequality((Object) go, (Object) null))
      {
        if (flag1)
        {
          Prioritizable component = go.GetComponent<Prioritizable>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            if (Object.op_Inequality((Object) BuildMenu.Instance, (Object) null))
              component.SetMasterPriority(BuildMenu.Instance.GetBuildingPriority());
            if (Object.op_Inequality((Object) PlanScreen.Instance, (Object) null))
              component.SetMasterPriority(PlanScreen.Instance.GetBuildingPriority());
          }
        }
        IUtilityItem component4 = (IUtilityItem) go.GetComponent<KAnimGraphTileVisualizer>();
        if (component4 != null)
          component4.Connections = utilityConnections2;
      }
      TileVisualizer.RefreshCell(pathNode.cell, this.def.TileLayer, this.def.ReplacementLayer);
    }
    ResourceRemainingDisplayScreen.instance.SetNumberOfPendingConstructions(0);
  }

  private BaseUtilityBuildTool.PathNode CreateVisualizer(BaseUtilityBuildTool.PathNode node)
  {
    if (Object.op_Equality((Object) node.visualizer, (Object) null))
    {
      GameObject gameObject = Object.Instantiate<GameObject>(this.def.BuildingPreview, Grid.CellToPosCBC(node.cell, this.def.SceneLayer), Quaternion.identity);
      gameObject.SetActive(true);
      node.visualizer = gameObject;
    }
    return node;
  }

  private void StopVisUpdater()
  {
    for (int index = 0; index < this.path.Count; ++index)
      Object.Destroy((Object) this.path[index].visualizer);
    this.path.Clear();
    if (this.visUpdater == null)
      return;
    ((MonoBehaviour) this).StopCoroutine(this.visUpdater);
    this.visUpdater = (Coroutine) null;
  }

  protected struct PathNode
  {
    public int cell;
    public bool valid;
    public GameObject visualizer;

    public void Play(string anim) => this.visualizer.GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit(anim));
  }
}
