// Decompiled with JetBrains decompiler
// Type: RocketModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RocketModule")]
public class RocketModule : KMonoBehaviour
{
  public LaunchConditionManager conditionManager;
  public Dictionary<ProcessCondition.ProcessConditionType, List<ProcessCondition>> moduleConditions = new Dictionary<ProcessCondition.ProcessConditionType, List<ProcessCondition>>();
  public static Operational.Flag landedFlag = new Operational.Flag("landed", Operational.Flag.Type.Requirement);
  public bool operationalLandedRequired = true;
  private string rocket_module_bg_base_string = "{0}{1}";
  private string rocket_module_bg_affix = "BG";
  private string rocket_module_bg_anim = "on";
  [SerializeField]
  private KAnimFile bgAnimFile;
  protected string parentRocketName = (string) UI.STARMAP.DEFAULT_NAME;
  private static readonly EventSystem.IntraObjectHandler<RocketModule> DEBUG_OnDestroyDelegate = new EventSystem.IntraObjectHandler<RocketModule>((Action<RocketModule, object>) ((component, data) => component.DEBUG_OnDestroy(data)));
  private static readonly EventSystem.IntraObjectHandler<RocketModule> OnRocketOnGroundTagDelegate = GameUtil.CreateHasTagHandler<RocketModule>(GameTags.RocketOnGround, (Action<RocketModule, object>) ((component, data) => component.OnRocketOnGroundTag(data)));
  private static readonly EventSystem.IntraObjectHandler<RocketModule> OnRocketNotOnGroundTagDelegate = GameUtil.CreateHasTagHandler<RocketModule>(GameTags.RocketNotOnGround, (Action<RocketModule, object>) ((component, data) => component.OnRocketNotOnGroundTag(data)));

  public ProcessCondition AddModuleCondition(
    ProcessCondition.ProcessConditionType conditionType,
    ProcessCondition condition)
  {
    if (!this.moduleConditions.ContainsKey(conditionType))
      this.moduleConditions.Add(conditionType, new List<ProcessCondition>());
    if (!this.moduleConditions[conditionType].Contains(condition))
      this.moduleConditions[conditionType].Add(condition);
    return condition;
  }

  public List<ProcessCondition> GetConditionSet(
    ProcessCondition.ProcessConditionType conditionType)
  {
    List<ProcessCondition> conditionSet = new List<ProcessCondition>();
    if (conditionType == ProcessCondition.ProcessConditionType.All)
    {
      foreach (KeyValuePair<ProcessCondition.ProcessConditionType, List<ProcessCondition>> moduleCondition in this.moduleConditions)
        conditionSet.AddRange((IEnumerable<ProcessCondition>) moduleCondition.Value);
    }
    else if (this.moduleConditions.ContainsKey(conditionType))
      conditionSet = this.moduleConditions[conditionType];
    return conditionSet;
  }

  public void SetBGKAnim(KAnimFile anim_file) => this.bgAnimFile = anim_file;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    GameUtil.SubscribeToTags<RocketModule>(this, RocketModule.OnRocketOnGroundTagDelegate, false);
    GameUtil.SubscribeToTags<RocketModule>(this, RocketModule.OnRocketNotOnGroundTagDelegate, false);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!DlcManager.FeatureClusterSpaceEnabled())
    {
      this.conditionManager = this.FindLaunchConditionManager();
      Spacecraft conditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.conditionManager);
      if (conditionManager != null)
        this.SetParentRocketName(conditionManager.GetRocketName());
      this.RegisterWithConditionManager();
    }
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.AddStatusItem(Db.Get().BuildingStatusItems.RocketName, (object) this);
    this.Subscribe<RocketModule>(1502190696, RocketModule.DEBUG_OnDestroyDelegate);
    this.FixSorting();
    ((Component) this).GetComponent<AttachableBuilding>().onAttachmentNetworkChanged += new Action<object>(this.OnAttachmentNetworkChanged);
    if (!Object.op_Inequality((Object) this.bgAnimFile, (Object) null))
      return;
    this.AddBGGantry();
  }

  public void FixSorting()
  {
    int num = 0;
    AttachableBuilding component1 = ((Component) this).GetComponent<AttachableBuilding>();
    while (Object.op_Inequality((Object) component1, (Object) null))
    {
      BuildingAttachPoint attachedTo = component1.GetAttachedTo();
      if (Object.op_Inequality((Object) attachedTo, (Object) null))
      {
        component1 = ((Component) attachedTo).GetComponent<AttachableBuilding>();
        ++num;
      }
      else
        break;
    }
    Vector3 localPosition = TransformExtensions.GetLocalPosition(this.transform);
    localPosition.z = Grid.GetLayerZ(Grid.SceneLayer.Building) - (float) num * 0.01f;
    TransformExtensions.SetLocalPosition(this.transform, localPosition);
    KBatchedAnimController component2 = ((Component) this).GetComponent<KBatchedAnimController>();
    if (!component2.enabled)
      return;
    component2.enabled = false;
    component2.enabled = true;
  }

  private void OnAttachmentNetworkChanged(object ab) => this.FixSorting();

  private void AddBGGantry()
  {
    KAnimControllerBase component = ((Component) this).GetComponent<KAnimControllerBase>();
    GameObject go = new GameObject();
    ((Object) go).name = string.Format(this.rocket_module_bg_base_string, (object) ((Object) this).name, (object) this.rocket_module_bg_affix);
    go.SetActive(false);
    Vector3 position = TransformExtensions.GetPosition(((Component) component).transform);
    position.z = Grid.GetLayerZ(Grid.SceneLayer.InteriorWall);
    TransformExtensions.SetPosition(go.transform, position);
    go.transform.parent = this.transform;
    KBatchedAnimController kbatchedAnimController = go.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      this.bgAnimFile
    };
    kbatchedAnimController.initialAnim = this.rocket_module_bg_anim;
    kbatchedAnimController.fgLayer = Grid.SceneLayer.NoLayer;
    kbatchedAnimController.initialMode = (KAnim.PlayMode) 2;
    kbatchedAnimController.FlipX = component.FlipX;
    kbatchedAnimController.FlipY = component.FlipY;
    go.SetActive(true);
  }

  private void DEBUG_OnDestroy(object data)
  {
    if (!Object.op_Inequality((Object) this.conditionManager, (Object) null) || App.IsExiting || KMonoBehaviour.isLoadingScene)
      return;
    Spacecraft conditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.conditionManager);
    this.conditionManager.DEBUG_TraceModuleDestruction(((Object) this).name, conditionManager == null ? "null spacecraft" : conditionManager.state.ToString(), new StackTrace(true).ToString());
  }

  private void OnRocketOnGroundTag(object data)
  {
    this.RegisterComponents();
    Operational component = ((Component) this).GetComponent<Operational>();
    if (!this.operationalLandedRequired || !Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetFlag(RocketModule.landedFlag, true);
  }

  private void OnRocketNotOnGroundTag(object data)
  {
    this.DeregisterComponents();
    Operational component = ((Component) this).GetComponent<Operational>();
    if (!this.operationalLandedRequired || !Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetFlag(RocketModule.landedFlag, false);
  }

  public void DeregisterComponents()
  {
    KSelectable component1 = ((Component) this).GetComponent<KSelectable>();
    component1.IsSelectable = false;
    BuildingComplete component2 = ((Component) this).GetComponent<BuildingComplete>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      component2.UpdatePosition();
    if (Object.op_Equality((Object) SelectTool.Instance.selected, (Object) component1))
      SelectTool.Instance.Select((KSelectable) null);
    Deconstructable component3 = ((Component) this).GetComponent<Deconstructable>();
    if (Object.op_Inequality((Object) component3, (Object) null))
      component3.SetAllowDeconstruction(false);
    HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(((Component) this).gameObject);
    if (handle.IsValid())
      GameComps.StructureTemperatures.Disable(handle);
    FakeFloorAdder component4 = ((Component) this).GetComponent<FakeFloorAdder>();
    if (Object.op_Inequality((Object) component4, (Object) null))
      component4.SetFloor(false);
    AccessControl component5 = ((Component) this).GetComponent<AccessControl>();
    if (Object.op_Inequality((Object) component5, (Object) null))
      component5.SetRegistered(false);
    foreach (ManualDeliveryKG component6 in ((Component) this).GetComponents<ManualDeliveryKG>())
    {
      DebugUtil.DevAssert(!component6.IsPaused, "RocketModule ManualDeliver chore was already paused, when this rocket lands it will re-enable it.", (Object) null);
      component6.Pause(true, "Rocket heading to space");
    }
    foreach (BuildingConduitEndpoints component7 in ((Component) this).GetComponents<BuildingConduitEndpoints>())
      component7.RemoveEndPoint();
    ReorderableBuilding component8 = ((Component) this).GetComponent<ReorderableBuilding>();
    if (Object.op_Inequality((Object) component8, (Object) null))
      component8.ShowReorderArm(false);
    Workable component9 = ((Component) this).GetComponent<Workable>();
    if (Object.op_Inequality((Object) component9, (Object) null))
      component9.RefreshReachability();
    Structure component10 = ((Component) this).GetComponent<Structure>();
    if (Object.op_Inequality((Object) component10, (Object) null))
      component10.UpdatePosition();
    WireUtilitySemiVirtualNetworkLink component11 = ((Component) this).GetComponent<WireUtilitySemiVirtualNetworkLink>();
    if (Object.op_Inequality((Object) component11, (Object) null))
      component11.SetLinkConnected(false);
    PartialLightBlocking component12 = ((Component) this).GetComponent<PartialLightBlocking>();
    if (!Object.op_Inequality((Object) component12, (Object) null))
      return;
    component12.ClearLightBlocking();
  }

  public void RegisterComponents()
  {
    ((Component) this).GetComponent<KSelectable>().IsSelectable = true;
    BuildingComplete component1 = ((Component) this).GetComponent<BuildingComplete>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      component1.UpdatePosition();
    Deconstructable component2 = ((Component) this).GetComponent<Deconstructable>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      component2.SetAllowDeconstruction(true);
    HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(((Component) this).gameObject);
    if (handle.IsValid())
      GameComps.StructureTemperatures.Enable(handle);
    foreach (Storage component3 in ((Component) this).GetComponents<Storage>())
      component3.UpdateStoredItemCachedCells();
    FakeFloorAdder component4 = ((Component) this).GetComponent<FakeFloorAdder>();
    if (Object.op_Inequality((Object) component4, (Object) null))
      component4.SetFloor(true);
    AccessControl component5 = ((Component) this).GetComponent<AccessControl>();
    if (Object.op_Inequality((Object) component5, (Object) null))
      component5.SetRegistered(true);
    foreach (ManualDeliveryKG component6 in ((Component) this).GetComponents<ManualDeliveryKG>())
      component6.Pause(false, "Landing on world");
    foreach (BuildingConduitEndpoints component7 in ((Component) this).GetComponents<BuildingConduitEndpoints>())
      component7.AddEndpoint();
    ReorderableBuilding component8 = ((Component) this).GetComponent<ReorderableBuilding>();
    if (Object.op_Inequality((Object) component8, (Object) null))
      component8.ShowReorderArm(true);
    Workable component9 = ((Component) this).GetComponent<Workable>();
    if (Object.op_Inequality((Object) component9, (Object) null))
      component9.RefreshReachability();
    Structure component10 = ((Component) this).GetComponent<Structure>();
    if (Object.op_Inequality((Object) component10, (Object) null))
      component10.UpdatePosition();
    WireUtilitySemiVirtualNetworkLink component11 = ((Component) this).GetComponent<WireUtilitySemiVirtualNetworkLink>();
    if (Object.op_Inequality((Object) component11, (Object) null))
      component11.SetLinkConnected(true);
    PartialLightBlocking component12 = ((Component) this).GetComponent<PartialLightBlocking>();
    if (!Object.op_Inequality((Object) component12, (Object) null))
      return;
    component12.SetLightBlocking();
  }

  private void ToggleComponent(System.Type cmpType, bool enabled)
  {
    MonoBehaviour component = (MonoBehaviour) ((Component) this).GetComponent(cmpType);
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    ((Behaviour) component).enabled = enabled;
  }

  public void RegisterWithConditionManager()
  {
    Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
    if (!Object.op_Inequality((Object) this.conditionManager, (Object) null))
      return;
    this.conditionManager.RegisterRocketModule(this);
  }

  protected virtual void OnCleanUp()
  {
    if (Object.op_Inequality((Object) this.conditionManager, (Object) null))
      this.conditionManager.UnregisterRocketModule(this);
    base.OnCleanUp();
  }

  public virtual LaunchConditionManager FindLaunchConditionManager()
  {
    if (!DlcManager.FeatureClusterSpaceEnabled())
    {
      foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this).GetComponent<AttachableBuilding>()))
      {
        LaunchConditionManager component = gameObject.GetComponent<LaunchConditionManager>();
        if (Object.op_Inequality((Object) component, (Object) null))
          return component;
      }
    }
    return (LaunchConditionManager) null;
  }

  public void SetParentRocketName(string newName)
  {
    this.parentRocketName = newName;
    NameDisplayScreen.Instance.UpdateName(((Component) this).gameObject);
  }

  public virtual string GetParentRocketName() => this.parentRocketName;

  public void MoveToSpace()
  {
    Prioritizable component1 = ((Component) this).GetComponent<Prioritizable>();
    if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) component1.GetMyWorld(), (Object) null))
      component1.GetMyWorld().RemoveTopPriorityPrioritizable(component1);
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    Building component2 = ((Component) this).GetComponent<Building>();
    component2.Def.UnmarkArea(cell, component2.Orientation, component2.Def.ObjectLayer, ((Component) this).gameObject);
    Vector3 vector3;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3).\u002Ector(-1f, -1f, 0.0f);
    TransformExtensions.SetPosition(((Component) this).gameObject.transform, vector3);
    LogicPorts component3 = ((Component) this).GetComponent<LogicPorts>();
    if (Object.op_Inequality((Object) component3, (Object) null))
      component3.OnMove();
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.Entombed, false, (object) this);
  }

  public void MoveToPad(int newCell)
  {
    TransformExtensions.SetPosition(((Component) this).gameObject.transform, Grid.CellToPos(newCell, (CellAlignment) 1, Grid.SceneLayer.Building));
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    Building component1 = ((Component) this).GetComponent<Building>();
    component1.RefreshCells();
    component1.Def.MarkArea(cell, component1.Orientation, component1.Def.ObjectLayer, ((Component) this).gameObject);
    LogicPorts component2 = ((Component) this).GetComponent<LogicPorts>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      component2.OnMove();
    Prioritizable component3 = ((Component) this).GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component3, (Object) null) || !component3.IsTopPriority())
      return;
    component3.GetMyWorld().AddTopPriorityPrioritizable(component3);
  }
}
