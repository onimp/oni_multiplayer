// Decompiled with JetBrains decompiler
// Type: BuildingTemplates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingTemplates
{
  public static BuildingDef CreateBuildingDef(
    string id,
    int width,
    int height,
    string anim,
    int hitpoints,
    float construction_time,
    float[] construction_mass,
    string[] construction_materials,
    float melting_point,
    BuildLocationRule build_location_rule,
    EffectorValues decor,
    EffectorValues noise,
    float temperature_modification_mass_scale = 0.2f)
  {
    BuildingDef instance = ScriptableObject.CreateInstance<BuildingDef>();
    instance.PrefabID = id;
    instance.InitDef();
    ((Object) instance).name = id;
    instance.Mass = construction_mass;
    instance.MassForTemperatureModification = construction_mass[0] * temperature_modification_mass_scale;
    instance.WidthInCells = width;
    instance.HeightInCells = height;
    instance.HitPoints = hitpoints;
    instance.ConstructionTime = construction_time;
    instance.SceneLayer = Grid.SceneLayer.Building;
    instance.MaterialCategory = construction_materials;
    instance.BaseMeltingPoint = melting_point;
    switch (build_location_rule)
    {
      case BuildLocationRule.Anywhere:
      case BuildLocationRule.Tile:
      case BuildLocationRule.Conduit:
      case BuildLocationRule.LogicBridge:
      case BuildLocationRule.WireBridge:
        instance.ContinuouslyCheckFoundation = false;
        break;
      default:
        instance.ContinuouslyCheckFoundation = true;
        break;
    }
    instance.BuildLocationRule = build_location_rule;
    instance.ObjectLayer = ObjectLayer.Building;
    instance.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit(anim))
    };
    instance.GenerateOffsets();
    instance.BaseDecor = (float) decor.amount;
    instance.BaseDecorRadius = (float) decor.radius;
    instance.BaseNoisePollution = noise.amount;
    instance.BaseNoisePollutionRadius = noise.radius;
    return instance;
  }

  public static void CreateStandardBuildingDef(BuildingDef def) => def.Breakable = true;

  public static void CreateFoundationTileDef(BuildingDef def)
  {
    def.IsFoundation = true;
    def.TileLayer = ObjectLayer.FoundationTile;
    def.ReplacementLayer = ObjectLayer.ReplacementTile;
    def.ReplacementCandidateLayers = new List<ObjectLayer>()
    {
      ObjectLayer.FoundationTile,
      ObjectLayer.LadderTile,
      ObjectLayer.Backwall
    };
    BuildingDef buildingDef = def;
    List<Tag> tagList = new List<Tag>();
    tagList.Add(GameTags.FloorTiles);
    tagList.Add(GameTags.Ladders);
    tagList.Add(GameTags.Backwall);
    buildingDef.ReplacementTags = tagList;
    def.EquivalentReplacementLayers = new List<ObjectLayer>()
    {
      ObjectLayer.ReplacementLadder
    };
  }

  public static void CreateLadderDef(BuildingDef def)
  {
    def.TileLayer = ObjectLayer.LadderTile;
    def.ReplacementLayer = ObjectLayer.ReplacementLadder;
    BuildingDef buildingDef = def;
    List<Tag> tagList = new List<Tag>();
    tagList.Add(GameTags.Ladders);
    buildingDef.ReplacementTags = tagList;
    def.EquivalentReplacementLayers = new List<ObjectLayer>()
    {
      ObjectLayer.ReplacementTile
    };
  }

  public static void CreateElectricalBuildingDef(BuildingDef def)
  {
    BuildingTemplates.CreateStandardBuildingDef(def);
    def.RequiresPowerInput = true;
    def.ViewMode = OverlayModes.Power.ID;
    def.AudioCategory = "HollowMetal";
  }

  public static void CreateRocketBuildingDef(BuildingDef def)
  {
    BuildingTemplates.CreateStandardBuildingDef(def);
    def.Invincible = true;
    def.DefaultAnimState = "grounded";
    def.UseStructureTemperature = false;
  }

  public static void CreateMonumentBuildingDef(BuildingDef def)
  {
    BuildingTemplates.CreateStandardBuildingDef(def);
    def.Invincible = true;
  }

  public static Storage CreateDefaultStorage(GameObject go, bool forceCreate = false)
  {
    Storage defaultStorage = forceCreate ? go.AddComponent<Storage>() : go.AddOrGet<Storage>();
    defaultStorage.capacityKg = 2000f;
    return defaultStorage;
  }

  public static void CreateComplexFabricatorStorage(GameObject go, ComplexFabricator fabricator)
  {
    fabricator.inStorage = go.AddComponent<Storage>();
    fabricator.inStorage.capacityKg = 20000f;
    fabricator.inStorage.showInUI = true;
    fabricator.inStorage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
    fabricator.buildStorage = go.AddComponent<Storage>();
    fabricator.buildStorage.capacityKg = 20000f;
    fabricator.buildStorage.showInUI = true;
    fabricator.buildStorage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
    fabricator.outStorage = go.AddComponent<Storage>();
    fabricator.outStorage.capacityKg = 20000f;
    fabricator.outStorage.showInUI = true;
    fabricator.outStorage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
  }

  public static void DoPostConfigure(GameObject go)
  {
  }

  public static GameObject ExtendBuildingToRocketModule(
    GameObject template,
    string vanillaBGAnim,
    bool clusterRocket = false)
  {
    template.AddTag(GameTags.RocketModule);
    RocketModule rocketModule = !clusterRocket ? template.AddOrGet<RocketModule>() : (RocketModule) template.AddOrGet<RocketModuleCluster>();
    if (vanillaBGAnim != null)
      rocketModule.SetBGKAnim(Assets.GetAnim(HashedString.op_Implicit(vanillaBGAnim)));
    KBatchedAnimController component1 = template.GetComponent<KBatchedAnimController>();
    component1.isMovable = true;
    component1.initialMode = (KAnim.PlayMode) 0;
    BuildingDef def = template.GetComponent<Building>().Def;
    def.ShowInBuildMenu = def.ShowInBuildMenu && !DlcManager.FeatureClusterSpaceEnabled();
    if (def.WidthInCells == 3)
      template.AddOrGet<VerticalModuleTiler>();
    GameObject underConstruction = def.BuildingUnderConstruction;
    if (clusterRocket)
      underConstruction.AddOrGet<RocketModuleCluster>();
    else
      underConstruction.AddOrGet<RocketModule>();
    AttachableBuilding component2 = template.GetComponent<AttachableBuilding>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      underConstruction.AddOrGet<AttachableBuilding>().attachableToTag = component2.attachableToTag;
    BuildingAttachPoint component3 = template.GetComponent<BuildingAttachPoint>();
    if (Object.op_Inequality((Object) component3, (Object) null))
      underConstruction.AddOrGet<BuildingAttachPoint>().points = component3.points;
    template.GetComponent<Building>().Def.ThermalConductivity = 0.1f;
    Storage component4 = template.GetComponent<Storage>();
    if (Object.op_Inequality((Object) component4, (Object) null))
      component4.showUnreachableStatus = true;
    return template;
  }

  public static GameObject ExtendBuildingToRocketModuleCluster(
    GameObject template,
    string vanillaBGAnim,
    int burden,
    float enginePower = 0.0f,
    float fuelCostPerDistance = 0.0f)
  {
    template.AddTag(GameTags.RocketModule);
    template = BuildingTemplates.ExtendBuildingToRocketModule(template, vanillaBGAnim, true);
    BuildingDef def = template.GetComponent<Building>().Def;
    GameObject underConstruction = def.BuildingUnderConstruction;
    DebugUtil.Assert(Array.IndexOf<string>(def.RequiredDlcIds, "EXPANSION1_ID") != -1, "Only expansion1 rocket engines should be expanded to Cluster Modules.");
    template.AddOrGet<ReorderableBuilding>();
    underConstruction.AddOrGet<ReorderableBuilding>();
    if (def.Cancellable)
      Debug.LogError((object) (def.Name + " Def should be marked 'Cancellable = false' as they implement their own cancel logic in ReorderableBuilding"));
    template.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition) new ResearchCompleted());
    template.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition) new MaterialsAvailable());
    template.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition) new PlaceSpaceAvailable());
    template.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition) new RocketHeightLimit());
    if (Object.op_Implicit((Object) template.GetComponent<RocketEngineCluster>()))
    {
      template.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition) new LimitOneEngine());
      template.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition) new EngineOnBottom());
    }
    if (Object.op_Implicit((Object) template.GetComponent<PassengerRocketModule>()))
      template.GetComponent<ReorderableBuilding>().buildConditions.Add((SelectModuleCondition) new NoFreeRocketInterior());
    if (Object.op_Implicit((Object) template.GetComponent<CargoBay>()))
      template.AddOrGet<CargoBayConduit>();
    RocketModulePerformance modulePerformance = new RocketModulePerformance((float) burden, fuelCostPerDistance, enginePower);
    template.GetComponent<RocketModuleCluster>().performanceStats = modulePerformance;
    template.GetComponent<Building>().Def.BuildingUnderConstruction.GetComponent<RocketModuleCluster>().performanceStats = modulePerformance;
    return template;
  }

  public static GameObject ExtendBuildingToClusterCargoBay(
    GameObject template,
    float capacity,
    List<Tag> storageFilters,
    CargoBay.CargoType cargoType)
  {
    Storage storage = template.AddOrGet<Storage>();
    storage.capacityKg = capacity;
    storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    storage.showCapacityStatusItem = true;
    storage.storageFilters = storageFilters;
    storage.allowSettingOnlyFetchMarkedItems = false;
    CargoBayCluster cargoBayCluster = template.AddOrGet<CargoBayCluster>();
    cargoBayCluster.storage = storage;
    cargoBayCluster.storageType = cargoType;
    TreeFilterable treeFilterable = template.AddOrGet<TreeFilterable>();
    treeFilterable.dropIncorrectOnFilterChange = false;
    treeFilterable.autoSelectStoredOnLoad = false;
    return template;
  }

  public static void ExtendBuildingToGravitas(GameObject template)
  {
    template.GetComponent<Deconstructable>().allowDeconstruction = false;
    template.AddOrGet<Demolishable>();
  }
}
