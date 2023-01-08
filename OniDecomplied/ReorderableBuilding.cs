// Decompiled with JetBrains decompiler
// Type: ReorderableBuilding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class ReorderableBuilding : KMonoBehaviour
{
  public string templateBuildingID = "UnconstructedRocketModule";
  private bool cancelShield;
  private bool reorderingAnimUnderway;
  private KBatchedAnimController animController;
  public List<SelectModuleCondition> buildConditions = new List<SelectModuleCondition>();
  private KBatchedAnimController reorderArmController;
  private KAnimLink m_animLink;
  [MyCmpAdd]
  private LoopingSounds loopingSounds;
  private string reorderSound = "RocketModuleSwitchingArm_moving_LP";
  private static List<ReorderableBuilding> toBeRemoved = new List<ReorderableBuilding>();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.animController = ((Component) this).GetComponent<KBatchedAnimController>();
    this.Subscribe(2127324410, new Action<object>(this.OnCancel));
    GameObject go = new GameObject();
    ((Object) go).name = "ReorderArm";
    go.transform.SetParent(this.transform);
    TransformExtensions.SetLocalPosition(go.transform, Vector3.op_Multiply(Vector3.op_Multiply(Vector3.up, Grid.CellSizeInMeters), (float) ((double) ((Component) this).GetComponent<Building>().Def.HeightInCells / 2.0 - 0.5)));
    TransformExtensions.SetPosition(go.transform, new Vector3(go.transform.position.x, go.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.BuildingBack)));
    go.SetActive(false);
    this.reorderArmController = go.AddComponent<KBatchedAnimController>();
    this.reorderArmController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("rocket_module_switching_arm_kanim"))
    };
    this.reorderArmController.initialAnim = "off";
    go.SetActive(true);
    this.ShowReorderArm(Grid.IsValidCell(Grid.PosToCell(go)));
    RocketModuleCluster component = ((Component) this).GetComponent<RocketModuleCluster>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      LaunchPad currentPad = component.CraftInterface.CurrentPad;
      if (Object.op_Inequality((Object) currentPad, (Object) null))
        this.m_animLink = new KAnimLink(((Component) currentPad).GetComponent<KAnimControllerBase>(), (KAnimControllerBase) this.reorderArmController);
    }
    if (this.m_animLink != null)
      return;
    this.m_animLink = new KAnimLink(((Component) this).GetComponent<KAnimControllerBase>(), (KAnimControllerBase) this.reorderArmController);
  }

  private void OnCancel(object data)
  {
    if (!Object.op_Inequality((Object) ((Component) this).GetComponent<BuildingUnderConstruction>(), (Object) null) || this.cancelShield || ReorderableBuilding.toBeRemoved.Contains(this))
      return;
    ReorderableBuilding.toBeRemoved.Add(this);
  }

  public GameObject AddModule(BuildingDef def, IList<Tag> buildMaterials) => Assets.GetPrefab(((Component) this).GetComponent<KPrefabID>().PrefabID()).GetComponent<ReorderableBuilding>().buildConditions.Find((Predicate<SelectModuleCondition>) (match => match is TopOnly)) != null || def.BuildingComplete.GetComponent<ReorderableBuilding>().buildConditions.Find((Predicate<SelectModuleCondition>) (match => match is EngineOnBottom)) != null ? this.AddModuleBelow(def, buildMaterials) : this.AddModuleAbove(def, buildMaterials);

  private GameObject AddModuleAbove(BuildingDef def, IList<Tag> buildMaterials)
  {
    BuildingAttachPoint component = ((Component) this).GetComponent<BuildingAttachPoint>();
    if (Object.op_Equality((Object) component, (Object) null))
      return (GameObject) null;
    BuildingAttachPoint.HardPoint point = component.points[0];
    int cell = Grid.OffsetCell(Grid.PosToCell(((Component) this).gameObject), point.position);
    int heightInCells = def.HeightInCells;
    if (Object.op_Inequality((Object) point.attachedBuilding, (Object) null))
    {
      if (!((Component) point.attachedBuilding).GetComponent<ReorderableBuilding>().CanMoveVertically(heightInCells))
        return (GameObject) null;
      ((Component) point.attachedBuilding).GetComponent<ReorderableBuilding>().MoveVertical(heightInCells);
    }
    return this.AddModuleCommon(def, buildMaterials, cell);
  }

  private GameObject AddModuleBelow(BuildingDef def, IList<Tag> buildMaterials)
  {
    int cell = Grid.PosToCell(((Component) this).gameObject);
    int heightInCells = def.HeightInCells;
    if (!this.CanMoveVertically(heightInCells))
      return (GameObject) null;
    this.MoveVertical(heightInCells);
    return this.AddModuleCommon(def, buildMaterials, cell);
  }

  private GameObject AddModuleCommon(BuildingDef def, IList<Tag> buildMaterials, int cell)
  {
    GameObject gameObject = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild ? def.Build(cell, Orientation.Neutral, (Storage) null, buildMaterials, 273.15f, timeBuilt: GameClock.Instance.GetTime()) : def.TryPlace((GameObject) null, Grid.CellToPosCBC(cell, def.SceneLayer), Orientation.Neutral, buildMaterials);
    ReorderableBuilding.RebuildNetworks();
    this.RocketSpecificPostAdd(gameObject, cell);
    return gameObject;
  }

  private void RocketSpecificPostAdd(GameObject obj, int cell)
  {
    RocketModuleCluster component1 = ((Component) this).GetComponent<RocketModuleCluster>();
    RocketModuleCluster component2 = obj.GetComponent<RocketModuleCluster>();
    if (!Object.op_Inequality((Object) component1, (Object) null) || !Object.op_Inequality((Object) component2, (Object) null))
      return;
    component1.CraftInterface.AddModule(component2);
  }

  public void RemoveModule()
  {
    BuildingAttachPoint component1 = ((Component) this).GetComponent<BuildingAttachPoint>();
    AttachableBuilding attachableBuilding = (AttachableBuilding) null;
    if (Object.op_Inequality((Object) component1, (Object) null))
      attachableBuilding = component1.points[0].attachedBuilding;
    int heightInCells = ((Component) this).GetComponent<Building>().Def.HeightInCells;
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<Deconstructable>(), (Object) null))
      ((Component) this).GetComponent<Deconstructable>().CompleteWork((Worker) null);
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<BuildingUnderConstruction>(), (Object) null))
      TracesExtesions.DeleteObject((Component) this);
    Building component2 = ((Component) this).GetComponent<Building>();
    component2.Def.UnmarkArea(Grid.PosToCell((KMonoBehaviour) this), component2.Orientation, component2.Def.ObjectLayer, ((Component) this).gameObject);
    if (!Object.op_Inequality((Object) attachableBuilding, (Object) null))
      return;
    ((Component) attachableBuilding).GetComponent<ReorderableBuilding>().MoveVertical(-heightInCells);
  }

  public void LateUpdate()
  {
    this.cancelShield = false;
    ReorderableBuilding.ProcessToBeRemoved();
    if (!this.reorderingAnimUnderway)
      return;
    float num = 10f;
    if ((double) Mathf.Abs(this.animController.Offset.y) < (double) Time.unscaledDeltaTime * (double) num)
    {
      this.animController.Offset = new Vector3(this.animController.Offset.x, 0.0f, this.animController.Offset.z);
      this.reorderingAnimUnderway = false;
      string str = ((Component) this).GetComponent<Building>().Def.WidthInCells.ToString() + "x" + ((Component) this).GetComponent<Building>().Def.HeightInCells.ToString() + "_ungrab";
      if (!this.reorderArmController.HasAnimation(HashedString.op_Implicit(str)))
        str = "3x3_ungrab";
      this.reorderArmController.Play(HashedString.op_Implicit(str));
      this.reorderArmController.Queue(HashedString.op_Implicit("off"));
      this.loopingSounds.StopSound(GlobalAssets.GetSound(this.reorderSound));
    }
    else if ((double) this.animController.Offset.y > 0.0)
      this.animController.Offset = new Vector3(this.animController.Offset.x, this.animController.Offset.y - Time.unscaledDeltaTime * num, this.animController.Offset.z);
    else if ((double) this.animController.Offset.y < 0.0)
      this.animController.Offset = new Vector3(this.animController.Offset.x, this.animController.Offset.y + Time.unscaledDeltaTime * num, this.animController.Offset.z);
    this.reorderArmController.Offset = this.animController.Offset;
  }

  private static void ProcessToBeRemoved()
  {
    if (ReorderableBuilding.toBeRemoved.Count <= 0)
      return;
    ReorderableBuilding.toBeRemoved.Sort((Comparison<ReorderableBuilding>) ((a, b) => (double) a.transform.position.y < (double) b.transform.position.y ? -1 : 1));
    for (int index = 0; index < ReorderableBuilding.toBeRemoved.Count; ++index)
      ReorderableBuilding.toBeRemoved[index].RemoveModule();
    ReorderableBuilding.toBeRemoved.Clear();
  }

  public void MoveVertical(int amount)
  {
    if (amount == 0)
      return;
    this.cancelShield = true;
    List<GameObject> buildings = new List<GameObject>();
    buildings.Add(((Component) this).gameObject);
    AttachableBuilding.GetAttachedAbove(((Component) this).GetComponent<AttachableBuilding>(), ref buildings);
    if (amount > 0)
      buildings.Reverse();
    foreach (GameObject go in buildings)
    {
      ReorderableBuilding.UnmarkBuilding(go, (AttachableBuilding) null);
      TransformExtensions.SetPosition(go.transform, Grid.CellToPos(Grid.OffsetCell(Grid.PosToCell(go), 0, amount), (CellAlignment) 1, Grid.SceneLayer.BuildingFront));
      ReorderableBuilding.MarkBuilding(go, (AttachableBuilding) null);
      go.GetComponent<ReorderableBuilding>().ApplyAnimOffset((float) -amount);
    }
    if (amount <= 0)
      return;
    foreach (GameObject gameObject in buildings)
      gameObject.GetComponent<AttachableBuilding>().RegisterWithAttachPoint(true);
  }

  public void SwapWithAbove(bool selectOnComplete = true)
  {
    BuildingAttachPoint component1 = ((Component) this).GetComponent<BuildingAttachPoint>();
    if (Object.op_Equality((Object) component1, (Object) null) || Object.op_Equality((Object) component1.points[0].attachedBuilding, (Object) null))
      return;
    int cell = Grid.PosToCell(((Component) this).gameObject);
    ReorderableBuilding.UnmarkBuilding(((Component) this).gameObject, (AttachableBuilding) null);
    AttachableBuilding attachedBuilding = component1.points[0].attachedBuilding;
    BuildingAttachPoint component2 = ((Component) attachedBuilding).GetComponent<BuildingAttachPoint>();
    AttachableBuilding aboveBuilding = Object.op_Inequality((Object) component2, (Object) null) ? component2.points[0].attachedBuilding : (AttachableBuilding) null;
    ReorderableBuilding.UnmarkBuilding(((Component) attachedBuilding).gameObject, aboveBuilding);
    Building component3 = ((Component) attachedBuilding).GetComponent<Building>();
    TransformExtensions.SetPosition(attachedBuilding.transform, Grid.CellToPos(cell, (CellAlignment) 1, Grid.SceneLayer.BuildingFront));
    ReorderableBuilding.MarkBuilding(((Component) attachedBuilding).gameObject, (AttachableBuilding) null);
    TransformExtensions.SetPosition(this.transform, Grid.CellToPos(Grid.OffsetCell(cell, 0, component3.Def.HeightInCells), (CellAlignment) 1, Grid.SceneLayer.BuildingFront));
    ReorderableBuilding.MarkBuilding(((Component) this).gameObject, aboveBuilding);
    ReorderableBuilding.RebuildNetworks();
    this.ApplyAnimOffset((float) -component3.Def.HeightInCells);
    Building component4 = ((Component) this).GetComponent<Building>();
    ((Component) component3).GetComponent<ReorderableBuilding>().ApplyAnimOffset((float) component4.Def.HeightInCells);
    if (!selectOnComplete)
      return;
    SelectTool.Instance.Select(((Component) component4).GetComponent<KSelectable>());
  }

  protected virtual void OnCleanUp()
  {
    if (Object.op_Equality((Object) ((Component) this).GetComponent<BuildingUnderConstruction>(), (Object) null) && !((Component) this).HasTag(GameTags.RocketInSpace))
      this.RemoveModule();
    if (this.m_animLink != null)
      this.m_animLink.Unregister();
    base.OnCleanUp();
  }

  private void ApplyAnimOffset(float amount)
  {
    this.animController.Offset = new Vector3(this.animController.Offset.x, this.animController.Offset.y + amount, this.animController.Offset.z);
    this.reorderArmController.Offset = this.animController.Offset;
    string str = ((Component) this).GetComponent<Building>().Def.WidthInCells.ToString() + "x" + ((Component) this).GetComponent<Building>().Def.HeightInCells.ToString() + "_grab";
    if (!this.reorderArmController.HasAnimation(HashedString.op_Implicit(str)))
      str = "3x3_grab";
    this.reorderArmController.Play(HashedString.op_Implicit(str));
    this.reorderArmController.onAnimComplete += new KAnimControllerBase.KAnimEvent(this.StartReorderingAnim);
  }

  private void StartReorderingAnim(HashedString data)
  {
    this.loopingSounds.StartSound(GlobalAssets.GetSound(this.reorderSound));
    this.reorderingAnimUnderway = true;
    this.reorderArmController.onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.StartReorderingAnim);
  }

  public void SwapWithBelow(bool selectOnComplete = true)
  {
    if (Object.op_Equality((Object) ((Component) this).GetComponent<AttachableBuilding>(), (Object) null) || Object.op_Equality((Object) ((Component) this).GetComponent<AttachableBuilding>().GetAttachedTo(), (Object) null))
      return;
    ((Component) ((Component) this).GetComponent<AttachableBuilding>().GetAttachedTo()).GetComponent<ReorderableBuilding>().SwapWithAbove(!selectOnComplete);
    if (!selectOnComplete)
      return;
    SelectTool.Instance.Select(((Component) this).GetComponent<KSelectable>());
  }

  public bool CanMoveVertically(int moveAmount, GameObject ignoreBuilding = null)
  {
    if (moveAmount == 0)
      return true;
    BuildingAttachPoint component1 = ((Component) this).GetComponent<BuildingAttachPoint>();
    AttachableBuilding component2 = ((Component) this).GetComponent<AttachableBuilding>();
    if (moveAmount > 0)
    {
      if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) component1.points[0].attachedBuilding, (Object) null) && Object.op_Inequality((Object) ((Component) component1.points[0].attachedBuilding).gameObject, (Object) ignoreBuilding) && !((Component) component1.points[0].attachedBuilding).GetComponent<ReorderableBuilding>().CanMoveVertically(moveAmount))
        return false;
    }
    else if (Object.op_Inequality((Object) component2, (Object) null))
    {
      BuildingAttachPoint attachedTo = component2.GetAttachedTo();
      if (Object.op_Inequality((Object) attachedTo, (Object) null) && Object.op_Inequality((Object) ((Component) attachedTo).gameObject, (Object) ignoreBuilding) && !((Component) component2.GetAttachedTo()).GetComponent<ReorderableBuilding>().CanMoveVertically(moveAmount))
        return false;
    }
    foreach (CellOffset occupiedOffset in this.GetOccupiedOffsets())
    {
      if (!ReorderableBuilding.CheckCellClear(Grid.OffsetCell(Grid.OffsetCell(Grid.PosToCell(((Component) this).gameObject), occupiedOffset), 0, moveAmount), ((Component) this).gameObject))
        return false;
    }
    return true;
  }

  public static bool CheckCellClear(int checkCell, GameObject ignoreObject = null) => Grid.IsValidCell(checkCell) && Grid.IsValidBuildingCell(checkCell) && !Grid.Solid[checkCell] && (int) Grid.WorldIdx[checkCell] != (int) ClusterManager.INVALID_WORLD_IDX && (!Object.op_Inequality((Object) Grid.Objects[checkCell, 1], (Object) null) || !Object.op_Inequality((Object) Grid.Objects[checkCell, 1], (Object) ignoreObject) || !Object.op_Equality((Object) Grid.Objects[checkCell, 1].GetComponent<ReorderableBuilding>(), (Object) null));

  public GameObject ConvertModule(BuildingDef toModule, IList<Tag> materials)
  {
    int cell = Grid.PosToCell(((Component) this).gameObject);
    int amount1 = toModule.HeightInCells - ((Component) this).GetComponent<Building>().Def.HeightInCells;
    ((Component) this).gameObject.GetComponent<Building>();
    BuildingAttachPoint component1 = ((Component) this).gameObject.GetComponent<BuildingAttachPoint>();
    GameObject gameObject1 = (GameObject) null;
    if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) component1.points[0].attachedBuilding, (Object) null))
    {
      gameObject1 = ((Component) component1.points[0].attachedBuilding).gameObject;
      component1.points[0].attachedBuilding = (AttachableBuilding) null;
      Components.BuildingAttachPoints.Remove(component1);
    }
    ReorderableBuilding.UnmarkBuilding(((Component) this).gameObject, (AttachableBuilding) null);
    if (amount1 != 0 && Object.op_Inequality((Object) gameObject1, (Object) null))
      gameObject1.GetComponent<ReorderableBuilding>().MoveVertical(amount1);
    string fail_reason;
    if (!DebugHandler.InstantBuildMode && !toModule.IsValidPlaceLocation(((Component) this).gameObject, cell, Orientation.Neutral, out fail_reason))
    {
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, fail_reason, this.transform);
      if (amount1 != 0 && Object.op_Inequality((Object) gameObject1, (Object) null))
      {
        int amount2 = amount1 * -1;
        gameObject1.GetComponent<ReorderableBuilding>().MoveVertical(amount2);
      }
      ReorderableBuilding.MarkBuilding(((Component) this).gameObject, Object.op_Inequality((Object) gameObject1, (Object) null) ? gameObject1.GetComponent<AttachableBuilding>() : (AttachableBuilding) null);
      if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) gameObject1, (Object) null))
      {
        component1.points[0].attachedBuilding = gameObject1.GetComponent<AttachableBuilding>();
        Components.BuildingAttachPoints.Add(component1);
      }
      return (GameObject) null;
    }
    if (materials == null)
      materials = (IList<Tag>) toModule.DefaultElements();
    GameObject gameObject2 = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive && SandboxToolParameterMenu.instance.settings.InstantBuild ? toModule.Build(cell, Orientation.Neutral, (Storage) null, materials, 273.15f, timeBuilt: GameClock.Instance.GetTime()) : toModule.TryPlace(((Component) this).gameObject, Grid.CellToPosCBC(cell, toModule.SceneLayer), Orientation.Neutral, materials);
    RocketModuleCluster component2 = ((Component) this).GetComponent<RocketModuleCluster>();
    RocketModuleCluster component3 = gameObject2.GetComponent<RocketModuleCluster>();
    if (Object.op_Inequality((Object) component2, (Object) null) && Object.op_Inequality((Object) component3, (Object) null))
      component2.CraftInterface.AddModule(component3);
    Util.KDestroyGameObject(((Component) this).gameObject);
    return gameObject2;
  }

  private CellOffset[] GetOccupiedOffsets()
  {
    OccupyArea component = ((Component) this).GetComponent<OccupyArea>();
    return Object.op_Inequality((Object) component, (Object) null) ? component.OccupiedCellsOffsets : ((Component) this).GetComponent<BuildingUnderConstruction>().Def.PlacementOffsets;
  }

  public bool CanChangeModule()
  {
    string str = !Object.op_Inequality((Object) ((Component) this).GetComponent<BuildingUnderConstruction>(), (Object) null) ? ((Component) this).GetComponent<Building>().Def.PrefabID : ((Component) this).GetComponent<BuildingUnderConstruction>().Def.PrefabID;
    RocketModuleCluster component = ((Component) this).GetComponent<RocketModuleCluster>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      if (Object.op_Inequality((Object) component.CraftInterface, (Object) null))
      {
        if (((Component) component.CraftInterface).GetComponent<Clustercraft>().Status != Clustercraft.CraftStatus.Grounded)
          return false;
      }
      else if (Object.op_Inequality((Object) component.conditionManager, (Object) null) && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(component.conditionManager).state != Spacecraft.MissionState.Grounded)
        return false;
    }
    return str != this.templateBuildingID && Tag.op_Inequality(Tag.op_Implicit(str), Assets.GetBuildingDef(this.templateBuildingID).BuildingUnderConstruction.PrefabID());
  }

  public bool CanRemoveModule() => true;

  public bool CanSwapUp(bool alsoCheckAboveCanSwapDown = true)
  {
    BuildingAttachPoint component = ((Component) this).GetComponent<BuildingAttachPoint>();
    if (Object.op_Equality((Object) component, (Object) null) || Object.op_Equality((Object) ((Component) this).GetComponent<AttachableBuilding>(), (Object) null) || Object.op_Inequality((Object) ((Component) this).GetComponent<RocketEngineCluster>(), (Object) null))
      return false;
    AttachableBuilding attachedBuilding = component.points[0].attachedBuilding;
    return !Object.op_Equality((Object) attachedBuilding, (Object) null) && !Object.op_Equality((Object) ((Component) attachedBuilding).GetComponent<BuildingAttachPoint>(), (Object) null) && !((Component) attachedBuilding).HasTag(GameTags.NoseRocketModule) && this.CanMoveVertically(((Component) attachedBuilding).GetComponent<Building>().Def.HeightInCells, ((Component) attachedBuilding).gameObject) && (!alsoCheckAboveCanSwapDown || ((Component) attachedBuilding).GetComponent<ReorderableBuilding>().CanSwapDown(false));
  }

  public bool CanSwapDown(bool alsoCheckBelowCanSwapUp = true)
  {
    if (((Component) this).gameObject.HasTag(GameTags.NoseRocketModule))
      return false;
    AttachableBuilding component = ((Component) this).GetComponent<AttachableBuilding>();
    if (Object.op_Equality((Object) component, (Object) null))
      return false;
    BuildingAttachPoint attachedTo = component.GetAttachedTo();
    return !Object.op_Equality((Object) attachedTo, (Object) null) && !Object.op_Equality((Object) ((Component) this).GetComponent<BuildingAttachPoint>(), (Object) null) && !Object.op_Equality((Object) ((Component) attachedTo).GetComponent<AttachableBuilding>(), (Object) null) && !Object.op_Inequality((Object) ((Component) attachedTo).GetComponent<RocketEngineCluster>(), (Object) null) && this.CanMoveVertically(((Component) attachedTo).GetComponent<Building>().Def.HeightInCells * -1, ((Component) attachedTo).gameObject) && (!alsoCheckBelowCanSwapUp || ((Component) attachedTo).GetComponent<ReorderableBuilding>().CanSwapUp(false));
  }

  public void ShowReorderArm(bool show)
  {
    if (!Object.op_Inequality((Object) this.reorderArmController, (Object) null))
      return;
    ((Component) this.reorderArmController).gameObject.SetActive(show);
  }

  private static void RebuildNetworks()
  {
    Game.Instance.logicCircuitSystem.ForceRebuildNetworks();
    Game.Instance.gasConduitSystem.ForceRebuildNetworks();
    Game.Instance.liquidConduitSystem.ForceRebuildNetworks();
    Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
    Game.Instance.solidConduitSystem.ForceRebuildNetworks();
  }

  private static void UnmarkBuilding(GameObject go, AttachableBuilding aboveBuilding)
  {
    int cell = Grid.PosToCell(go);
    Building component1 = go.GetComponent<Building>();
    component1.Def.UnmarkArea(cell, component1.Orientation, component1.Def.ObjectLayer, go);
    AttachableBuilding component2 = go.GetComponent<AttachableBuilding>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      component2.RegisterWithAttachPoint(false);
    if (Object.op_Inequality((Object) aboveBuilding, (Object) null))
      aboveBuilding.RegisterWithAttachPoint(false);
    RocketModule component3 = go.GetComponent<RocketModule>();
    if (Object.op_Inequality((Object) component3, (Object) null))
      component3.DeregisterComponents();
    RocketConduitSender[] components1 = go.GetComponents<RocketConduitSender>();
    if (components1.Length != 0)
    {
      foreach (RocketConduitSender rocketConduitSender in components1)
        rocketConduitSender.RemoveConduitPortFromNetwork();
    }
    RocketConduitReceiver[] components2 = go.GetComponents<RocketConduitReceiver>();
    if (components2.Length == 0)
      return;
    foreach (RocketConduitReceiver rocketConduitReceiver in components2)
      rocketConduitReceiver.RemoveConduitPortFromNetwork();
  }

  private static void MarkBuilding(GameObject go, AttachableBuilding aboveBuilding)
  {
    int cell = Grid.PosToCell(go);
    Building component1 = go.GetComponent<Building>();
    component1.Def.MarkArea(cell, component1.Orientation, component1.Def.ObjectLayer, go);
    if (Object.op_Inequality((Object) ((Component) component1).GetComponent<OccupyArea>(), (Object) null))
      ((Component) component1).GetComponent<OccupyArea>().UpdateOccupiedArea();
    LogicPorts component2 = ((Component) component1).GetComponent<LogicPorts>();
    if (Object.op_Implicit((Object) component2) && Object.op_Inequality((Object) go.GetComponent<BuildingComplete>(), (Object) null))
      component2.OnMove();
    ((Component) component1).GetComponent<AttachableBuilding>().RegisterWithAttachPoint(true);
    if (Object.op_Inequality((Object) aboveBuilding, (Object) null))
      aboveBuilding.RegisterWithAttachPoint(true);
    RocketModule component3 = go.GetComponent<RocketModule>();
    if (Object.op_Inequality((Object) component3, (Object) null))
      component3.RegisterComponents();
    VerticalModuleTiler component4 = go.GetComponent<VerticalModuleTiler>();
    if (Object.op_Inequality((Object) component4, (Object) null))
      component4.PostReorderMove();
    RocketConduitSender[] components1 = go.GetComponents<RocketConduitSender>();
    if (components1.Length != 0)
    {
      foreach (RocketConduitSender rocketConduitSender in components1)
        rocketConduitSender.AddConduitPortToNetwork();
    }
    RocketConduitReceiver[] components2 = go.GetComponents<RocketConduitReceiver>();
    if (components2.Length == 0)
      return;
    foreach (RocketConduitReceiver rocketConduitReceiver in components2)
      rocketConduitReceiver.AddConduitPortToNetwork();
  }

  public enum MoveSource
  {
    Push,
    Pull,
  }
}
