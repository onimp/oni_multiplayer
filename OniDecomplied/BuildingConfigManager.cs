// Decompiled with JetBrains decompiler
// Type: BuildingConfigManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BuildingConfigManager")]
public class BuildingConfigManager : KMonoBehaviour
{
  public static BuildingConfigManager Instance;
  private GameObject baseTemplate;
  private Dictionary<IBuildingConfig, BuildingDef> configTable = new Dictionary<IBuildingConfig, BuildingDef>();
  private string[] NonBuildableBuildings = new string[1]
  {
    "Headquarters"
  };
  private HashSet<System.Type> defaultKComponents = new HashSet<System.Type>();
  private HashSet<System.Type> defaultBuildingCompleteKComponents = new HashSet<System.Type>();
  private Dictionary<System.Type, HashSet<Tag>> ignoredDefaultKComponents = new Dictionary<System.Type, HashSet<Tag>>();
  private Dictionary<Tag, HashSet<System.Type>> buildingCompleteKComponents = new Dictionary<Tag, HashSet<System.Type>>();

  protected virtual void OnPrefabInit()
  {
    BuildingConfigManager.Instance = this;
    this.baseTemplate = new GameObject("BuildingTemplate");
    this.baseTemplate.SetActive(false);
    this.baseTemplate.AddComponent<KPrefabID>();
    this.baseTemplate.AddComponent<KSelectable>();
    this.baseTemplate.AddComponent<Modifiers>();
    this.baseTemplate.AddComponent<PrimaryElement>();
    this.baseTemplate.AddComponent<BuildingComplete>();
    this.baseTemplate.AddComponent<StateMachineController>();
    this.baseTemplate.AddComponent<Deconstructable>();
    this.baseTemplate.AddComponent<SaveLoadRoot>();
    this.baseTemplate.AddComponent<OccupyArea>();
    this.baseTemplate.AddComponent<DecorProvider>();
    this.baseTemplate.AddComponent<Operational>();
    this.baseTemplate.AddComponent<BuildingEnabledButton>();
    this.baseTemplate.AddComponent<Prioritizable>();
    this.baseTemplate.AddComponent<BuildingHP>();
    this.baseTemplate.AddComponent<LoopingSounds>();
    this.baseTemplate.AddComponent<InvalidPortReporter>();
    this.defaultBuildingCompleteKComponents.Add(typeof (RequiresFoundation));
  }

  public static string GetUnderConstructionName(string name) => name + "UnderConstruction";

  public void RegisterBuilding(IBuildingConfig config)
  {
    if (!DlcManager.IsDlcListValidForCurrentContent(config.GetDlcIds()))
      return;
    BuildingDef buildingDef = config.CreateBuildingDef();
    buildingDef.RequiredDlcIds = config.GetDlcIds();
    this.configTable[config] = buildingDef;
    GameObject go = Object.Instantiate<GameObject>(this.baseTemplate);
    Object.DontDestroyOnLoad((Object) go);
    go.GetComponent<KPrefabID>().PrefabTag = buildingDef.Tag;
    ((Object) go).name = buildingDef.PrefabID + "Template";
    go.GetComponent<Building>().Def = buildingDef;
    go.GetComponent<OccupyArea>().OccupiedCellsOffsets = buildingDef.PlacementOffsets;
    if (buildingDef.Deprecated)
      go.GetComponent<KPrefabID>().AddTag(GameTags.DeprecatedContent, false);
    config.ConfigureBuildingTemplate(go, buildingDef.Tag);
    buildingDef.BuildingComplete = BuildingLoader.Instance.CreateBuildingComplete(go, buildingDef);
    bool flag = true;
    for (int index = 0; index < this.NonBuildableBuildings.Length; ++index)
    {
      if (buildingDef.PrefabID == this.NonBuildableBuildings[index])
      {
        flag = false;
        break;
      }
    }
    if (flag)
    {
      buildingDef.BuildingUnderConstruction = BuildingLoader.Instance.CreateBuildingUnderConstruction(buildingDef);
      ((Object) buildingDef.BuildingUnderConstruction).name = BuildingConfigManager.GetUnderConstructionName(((Object) buildingDef.BuildingUnderConstruction).name);
      buildingDef.BuildingPreview = BuildingLoader.Instance.CreateBuildingPreview(buildingDef);
      GameObject buildingPreview = buildingDef.BuildingPreview;
      ((Object) buildingPreview).name = ((Object) buildingPreview).name + "Preview";
    }
    buildingDef.PostProcess();
    config.DoPostConfigureComplete(buildingDef.BuildingComplete);
    if (flag)
    {
      config.DoPostConfigurePreview(buildingDef, buildingDef.BuildingPreview);
      config.DoPostConfigureUnderConstruction(buildingDef.BuildingUnderConstruction);
    }
    Assets.AddBuildingDef(buildingDef);
  }

  public void ConfigurePost()
  {
    foreach (KeyValuePair<IBuildingConfig, BuildingDef> keyValuePair in this.configTable)
      keyValuePair.Key.ConfigurePost(keyValuePair.Value);
  }

  public void IgnoreDefaultKComponent(System.Type type_to_ignore, Tag building_tag)
  {
    HashSet<Tag> tagSet;
    if (!this.ignoredDefaultKComponents.TryGetValue(type_to_ignore, out tagSet))
    {
      tagSet = new HashSet<Tag>();
      this.ignoredDefaultKComponents[type_to_ignore] = tagSet;
    }
    tagSet.Add(building_tag);
  }

  private bool IsIgnoredDefaultKComponent(Tag building_tag, System.Type type)
  {
    bool flag = false;
    HashSet<Tag> tagSet;
    if (this.ignoredDefaultKComponents.TryGetValue(type, out tagSet) && tagSet.Contains(building_tag))
      flag = true;
    return flag;
  }

  public void AddBuildingCompleteKComponents(GameObject go, Tag prefab_tag)
  {
    foreach (System.Type completeKcomponent in this.defaultBuildingCompleteKComponents)
    {
      if (!this.IsIgnoredDefaultKComponent(prefab_tag, completeKcomponent))
        GameComps.GetKComponentManager(completeKcomponent).Add(go);
    }
    HashSet<System.Type> typeSet;
    if (!this.buildingCompleteKComponents.TryGetValue(prefab_tag, out typeSet))
      return;
    foreach (System.Type kcomponent_type in typeSet)
      GameComps.GetKComponentManager(kcomponent_type).Add(go);
  }

  public void DestroyBuildingCompleteKComponents(GameObject go, Tag prefab_tag)
  {
    foreach (System.Type completeKcomponent in this.defaultBuildingCompleteKComponents)
    {
      if (!this.IsIgnoredDefaultKComponent(prefab_tag, completeKcomponent))
        GameComps.GetKComponentManager(completeKcomponent).Remove(go);
    }
    HashSet<System.Type> typeSet;
    if (!this.buildingCompleteKComponents.TryGetValue(prefab_tag, out typeSet))
      return;
    foreach (System.Type kcomponent_type in typeSet)
      GameComps.GetKComponentManager(kcomponent_type).Remove(go);
  }

  public void AddDefaultBuildingCompleteKComponent(System.Type kcomponent_type) => this.defaultKComponents.Add(kcomponent_type);

  public void AddBuildingCompleteKComponent(Tag prefab_tag, System.Type kcomponent_type)
  {
    HashSet<System.Type> typeSet;
    if (!this.buildingCompleteKComponents.TryGetValue(prefab_tag, out typeSet))
    {
      typeSet = new HashSet<System.Type>();
      this.buildingCompleteKComponents[prefab_tag] = typeSet;
    }
    typeSet.Add(kcomponent_type);
  }
}
