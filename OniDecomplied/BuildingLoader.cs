// Decompiled with JetBrains decompiler
// Type: BuildingLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BuildingLoader")]
public class BuildingLoader : KMonoBehaviour
{
  private GameObject previewTemplate;
  private GameObject constructionTemplate;
  public static BuildingLoader Instance;

  public static void DestroyInstance() => BuildingLoader.Instance = (BuildingLoader) null;

  protected virtual void OnPrefabInit()
  {
    BuildingLoader.Instance = this;
    this.previewTemplate = this.CreatePreviewTemplate();
    this.constructionTemplate = this.CreateConstructionTemplate();
    Object.DontDestroyOnLoad((Object) this.previewTemplate);
  }

  private GameObject CreateTemplate()
  {
    GameObject go = new GameObject();
    go.SetActive(false);
    go.AddOrGet<KPrefabID>();
    go.AddOrGet<KSelectable>();
    go.AddOrGet<StateMachineController>();
    PrimaryElement primaryElement = go.AddOrGet<PrimaryElement>();
    primaryElement.Mass = 1f;
    primaryElement.Temperature = 293f;
    return go;
  }

  private GameObject CreatePreviewTemplate()
  {
    GameObject template = this.CreateTemplate();
    template.AddComponent<BuildingPreview>();
    return template;
  }

  private GameObject CreateConstructionTemplate()
  {
    GameObject template = this.CreateTemplate();
    template.AddOrGet<BuildingUnderConstruction>();
    template.AddOrGet<Constructable>();
    template.AddComponent<Storage>().doDiseaseTransfer = false;
    template.AddOrGet<Prioritizable>();
    template.AddOrGet<Notifier>();
    template.AddOrGet<SaveLoadRoot>();
    return template;
  }

  public GameObject CreateBuilding(BuildingDef def, GameObject go, GameObject parent = null)
  {
    go = Object.Instantiate<GameObject>(go);
    ((Object) go).name = def.PrefabID;
    if (Object.op_Inequality((Object) parent, (Object) null))
      go.transform.parent = parent.transform;
    go.GetComponent<Building>().Def = def;
    return go;
  }

  private static bool Add2DComponents(
    BuildingDef def,
    GameObject go,
    string initialAnimState = null,
    bool no_collider = false,
    int layer = -1)
  {
    bool required = def.AnimFiles != null && def.AnimFiles.Length != 0;
    if (layer == -1)
      layer = LayerMask.NameToLayer("Default");
    go.layer = layer;
    KBatchedAnimController[] components = go.GetComponents<KBatchedAnimController>();
    if (components.Length > 1)
    {
      for (int index = 2; index < components.Length; ++index)
        Object.DestroyImmediate((Object) components[index]);
    }
    if (Object.op_Equality((Object) def.BlockTileAtlas, (Object) null))
    {
      KBatchedAnimController kbatchedAnimController = BuildingLoader.UpdateComponentRequirement<KBatchedAnimController>(go, required);
      if (Object.op_Inequality((Object) kbatchedAnimController, (Object) null))
      {
        kbatchedAnimController.AnimFiles = def.AnimFiles;
        if (def.isKAnimTile)
        {
          kbatchedAnimController.initialAnim = (string) null;
        }
        else
        {
          if (def.isUtility && initialAnimState == null)
            initialAnimState = "idle";
          else if (Object.op_Inequality((Object) go.GetComponent<Door>(), (Object) null))
            initialAnimState = "closed";
          kbatchedAnimController.initialAnim = initialAnimState != null ? initialAnimState : def.DefaultAnimState;
          kbatchedAnimController.defaultAnim = kbatchedAnimController.initialAnim;
        }
        kbatchedAnimController.SetFGLayer(def.ForegroundLayer);
        kbatchedAnimController.materialType = (KAnimBatchGroup.MaterialType) 0;
      }
    }
    KBoxCollider2D kboxCollider2D = BuildingLoader.UpdateComponentRequirement<KBoxCollider2D>(go, required && !no_collider);
    if (Object.op_Inequality((Object) kboxCollider2D, (Object) null))
    {
      kboxCollider2D.offset = Vector2.op_Implicit(new Vector3(0.0f, 0.5f * (float) def.HeightInCells, 0.0f));
      kboxCollider2D.size = Vector2.op_Implicit(new Vector3((float) def.WidthInCells, (float) def.HeightInCells, 0.0f));
    }
    if (def.AnimFiles == null)
      Debug.LogError((object) (def.Name + " Def missing anim files"));
    return required;
  }

  private static T UpdateComponentRequirement<T>(GameObject go, bool required) where T : Component
  {
    T obj = go.GetComponent(typeof (T)) as T;
    if (!required && Object.op_Inequality((Object) (object) obj, (Object) null))
    {
      Object.DestroyImmediate((Object) (object) obj, true);
      obj = default (T);
    }
    else if (required && Object.op_Equality((Object) (object) obj, (Object) null))
      obj = go.AddComponent(typeof (T)) as T;
    return obj;
  }

  public static KPrefabID AddID(GameObject go, string str)
  {
    KPrefabID kprefabId = go.GetComponent<KPrefabID>();
    if (Object.op_Equality((Object) kprefabId, (Object) null))
      kprefabId = go.AddComponent<KPrefabID>();
    kprefabId.PrefabTag = new Tag(str);
    kprefabId.SaveLoadTag = kprefabId.PrefabTag;
    kprefabId.InitializeTags(true);
    return kprefabId;
  }

  public GameObject CreateBuildingUnderConstruction(BuildingDef def)
  {
    GameObject building = this.CreateBuilding(def, this.constructionTemplate);
    Object.DontDestroyOnLoad((Object) building);
    building.GetComponent<KSelectable>().SetName(def.Name);
    for (int index = 0; index < def.Mass.Length; ++index)
      building.GetComponent<PrimaryElement>().MassPerUnit += def.Mass[index];
    KPrefabID prefab = BuildingLoader.AddID(building, def.PrefabID + "UnderConstruction");
    prefab.AddTag(GameTags.UnderConstruction, false);
    BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(building, def.CheckRequiresBuildingCellVisualizer());
    building.GetComponent<Constructable>().SetWorkTime(def.ConstructionTime);
    if (def.Cancellable)
      building.AddOrGet<Cancellable>();
    building.AddComponent<BuildingFacade>();
    Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(building, def.PermittedRotations != 0);
    if (Object.op_Implicit((Object) rotatable))
      rotatable.permittedRotations = def.PermittedRotations;
    int layer = LayerMask.NameToLayer("Construction");
    prefab.defaultLayer = layer;
    BuildingLoader.Add2DComponents(def, building, "place", layer: layer);
    BuildingLoader.UpdateComponentRequirement<Vent>(building, false);
    bool required = Object.op_Inequality((Object) def.BuildingComplete.GetComponent<AnimTileable>(), (Object) null);
    BuildingLoader.UpdateComponentRequirement<AnimTileable>(building, required);
    if (def.RequiresPowerInput && def.AddLogicPowerPort)
      GeneratedBuildings.RegisterSingleLogicInputPort(building);
    Assets.AddPrefab(prefab);
    Util.PreInit(building);
    GeneratedBuildings.InitializeHighEnergyParticlePorts(building, def);
    GeneratedBuildings.InitializeLogicPorts(building, def);
    return building;
  }

  public GameObject CreateBuildingComplete(GameObject go, BuildingDef def)
  {
    ((Object) go).name = def.PrefabID + "Complete";
    TransformExtensions.SetPosition(go.transform, new Vector3(0.0f, 0.0f, Grid.GetLayerZ(def.SceneLayer)));
    go.GetComponent<KSelectable>().SetName(def.Name);
    PrimaryElement component = go.GetComponent<PrimaryElement>();
    component.MassPerUnit = 0.0f;
    for (int index = 0; index < def.Mass.Length; ++index)
      component.MassPerUnit += def.Mass[index];
    component.Temperature = 273.15f;
    BuildingHP buildingHp = go.AddOrGet<BuildingHP>();
    if (def.Invincible)
      buildingHp.invincible = true;
    buildingHp.SetHitPoints(def.HitPoints);
    if (def.Repairable)
      BuildingLoader.UpdateComponentRequirement<Repairable>(go, true);
    int layer = LayerMask.NameToLayer("Default");
    go.layer = layer;
    go.GetComponent<BuildingComplete>().Def = def;
    if (def.InputConduitType != ConduitType.None || def.OutputConduitType != ConduitType.None)
      go.AddComponent<BuildingConduitEndpoints>();
    if (!BuildingLoader.Add2DComponents(def, go))
      Debug.Log((object) (def.Name + " is not yet a 2d building!"));
    go.AddOrGet<BuildingFacade>();
    BuildingLoader.UpdateComponentRequirement<EnergyConsumer>(go, def.RequiresPowerInput);
    Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(go, def.PermittedRotations != 0);
    if (Object.op_Implicit((Object) rotatable))
      rotatable.permittedRotations = def.PermittedRotations;
    if (def.Breakable)
      go.AddComponent<Breakable>();
    ConduitConsumer conduitConsumer = BuildingLoader.UpdateComponentRequirement<ConduitConsumer>(go, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
    if (Object.op_Inequality((Object) conduitConsumer, (Object) null))
      conduitConsumer.SetConduitData(def.InputConduitType);
    bool required = def.RequiresPowerInput || def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid;
    RequireInputs requireInputs = BuildingLoader.UpdateComponentRequirement<RequireInputs>(go, required);
    if (Object.op_Inequality((Object) requireInputs, (Object) null))
      requireInputs.SetRequirements(def.RequiresPowerInput, def.InputConduitType == ConduitType.Gas || def.InputConduitType == ConduitType.Liquid);
    BuildingLoader.UpdateComponentRequirement<RequireOutputs>(go, def.OutputConduitType != 0);
    BuildingLoader.UpdateComponentRequirement<Operational>(go, !def.isUtility);
    if (def.Floodable)
      go.AddComponent<Floodable>();
    if (def.Disinfectable)
    {
      go.AddOrGet<AutoDisinfectable>();
      go.AddOrGet<Disinfectable>();
    }
    if (def.Overheatable)
    {
      Overheatable overheatable = go.AddComponent<Overheatable>();
      overheatable.baseOverheatTemp = def.OverheatTemperature;
      overheatable.baseFatalTemp = def.FatalHot;
    }
    if (def.Entombable)
      go.AddOrGet<Structure>();
    if (def.RequiresPowerInput && def.AddLogicPowerPort)
    {
      GeneratedBuildings.RegisterSingleLogicInputPort(go);
      go.AddOrGet<LogicOperationalController>();
    }
    BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(go, def.CheckRequiresBuildingCellVisualizer());
    if ((double) def.BaseDecor != 0.0)
    {
      DecorProvider decorProvider = BuildingLoader.UpdateComponentRequirement<DecorProvider>(go, true);
      decorProvider.baseDecor = def.BaseDecor;
      decorProvider.baseRadius = def.BaseDecorRadius;
    }
    if (Tag.op_Inequality(def.AttachmentSlotTag, Tag.Invalid))
      BuildingLoader.UpdateComponentRequirement<AttachableBuilding>(go, true).attachableToTag = def.AttachmentSlotTag;
    KPrefabID prefab = BuildingLoader.AddID(go, def.PrefabID);
    prefab.defaultLayer = layer;
    Assets.AddPrefab(prefab);
    Util.PreInit(go);
    GeneratedBuildings.InitializeHighEnergyParticlePorts(go, def);
    GeneratedBuildings.InitializeLogicPorts(go, def);
    return go;
  }

  public GameObject CreateBuildingPreview(BuildingDef def)
  {
    GameObject building = this.CreateBuilding(def, this.previewTemplate);
    Object.DontDestroyOnLoad((Object) building);
    int layer = LayerMask.NameToLayer("Place");
    TransformExtensions.SetPosition(building.transform, new Vector3(0.0f, 0.0f, Grid.GetLayerZ(def.SceneLayer)));
    BuildingLoader.Add2DComponents(def, building, "place", true, layer);
    KAnimControllerBase component1 = building.GetComponent<KAnimControllerBase>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      component1.fgLayer = Grid.SceneLayer.NoLayer;
    building.AddComponent<BuildingFacade>();
    Rotatable rotatable = BuildingLoader.UpdateComponentRequirement<Rotatable>(building, def.PermittedRotations != 0);
    if (Object.op_Implicit((Object) rotatable))
      rotatable.permittedRotations = def.PermittedRotations;
    BuildingLoader.AddID(building, def.PrefabID + "Preview").defaultLayer = layer;
    building.GetComponent<KSelectable>().SetName(def.Name);
    BuildingLoader.UpdateComponentRequirement<BuildingCellVisualizer>(building, def.CheckRequiresBuildingCellVisualizer());
    KAnimGraphTileVisualizer component2 = building.GetComponent<KAnimGraphTileVisualizer>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      Object.DestroyImmediate((Object) component2);
    if (def.RequiresPowerInput && def.AddLogicPowerPort)
      GeneratedBuildings.RegisterSingleLogicInputPort(building);
    Util.PreInit(building);
    GeneratedBuildings.InitializeHighEnergyParticlePorts(building, def);
    Assets.AddPrefab(building.GetComponent<KPrefabID>());
    GeneratedBuildings.InitializeLogicPorts(building, def);
    return building;
  }
}
