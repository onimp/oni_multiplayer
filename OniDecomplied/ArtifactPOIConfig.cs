// Decompiled with JetBrains decompiler
// Type: ArtifactPOIConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactPOIConfig : IMultiEntityConfig
{
  public const string GravitasSpaceStation1 = "GravitasSpaceStation1";
  public const string GravitasSpaceStation2 = "GravitasSpaceStation2";
  public const string GravitasSpaceStation3 = "GravitasSpaceStation3";
  public const string GravitasSpaceStation4 = "GravitasSpaceStation4";
  public const string GravitasSpaceStation5 = "GravitasSpaceStation5";
  public const string GravitasSpaceStation6 = "GravitasSpaceStation6";
  public const string GravitasSpaceStation7 = "GravitasSpaceStation7";
  public const string GravitasSpaceStation8 = "GravitasSpaceStation8";
  public const string RussellsTeapot = "RussellsTeapot";

  public List<GameObject> CreatePrefabs()
  {
    List<GameObject> prefabs = new List<GameObject>();
    foreach (ArtifactPOIConfig.ArtifactPOIParams config in this.GenerateConfigs())
      prefabs.Add(ArtifactPOIConfig.CreateArtifactPOI(config.id, config.anim, StringEntry.op_Implicit(Strings.Get(config.nameStringKey)), StringEntry.op_Implicit(Strings.Get(config.descStringKey)), config.poiType.idHash));
    return prefabs;
  }

  public static GameObject CreateArtifactPOI(
    string id,
    string anim,
    string name,
    string desc,
    HashedString poiType)
  {
    GameObject entity = EntityTemplates.CreateEntity(id, id);
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<ArtifactPOIConfigurator>().presetType = poiType;
    ArtifactPOIClusterGridEntity clusterGridEntity = entity.AddOrGet<ArtifactPOIClusterGridEntity>();
    clusterGridEntity.m_name = name;
    clusterGridEntity.m_Anim = anim;
    entity.AddOrGetDef<ArtifactPOIStates.Def>();
    LoreBearerUtil.AddLoreTo(entity, new LoreBearerAction(LoreBearerUtil.UnlockNextSpaceEntry));
    return entity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }

  private List<ArtifactPOIConfig.ArtifactPOIParams> GenerateConfigs()
  {
    List<ArtifactPOIConfig.ArtifactPOIParams> configs = new List<ArtifactPOIConfig.ArtifactPOIParams>();
    configs.Add(new ArtifactPOIConfig.ArtifactPOIParams("station_1", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation1")));
    configs.Add(new ArtifactPOIConfig.ArtifactPOIParams("station_2", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation2")));
    configs.Add(new ArtifactPOIConfig.ArtifactPOIParams("station_3", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation3")));
    configs.Add(new ArtifactPOIConfig.ArtifactPOIParams("station_4", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation4")));
    configs.Add(new ArtifactPOIConfig.ArtifactPOIParams("station_5", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation5")));
    configs.Add(new ArtifactPOIConfig.ArtifactPOIParams("station_6", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation6")));
    configs.Add(new ArtifactPOIConfig.ArtifactPOIParams("station_7", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation7")));
    configs.Add(new ArtifactPOIConfig.ArtifactPOIParams("station_8", new ArtifactPOIConfigurator.ArtifactPOIType("GravitasSpaceStation8")));
    configs.Add(new ArtifactPOIConfig.ArtifactPOIParams("russels_teapot", new ArtifactPOIConfigurator.ArtifactPOIType("RussellsTeapot", "artifact_TeaPot", true)));
    configs.RemoveAll((Predicate<ArtifactPOIConfig.ArtifactPOIParams>) (poi => !Util.IsNullOrWhiteSpace(poi.poiType.dlcID) && !DlcManager.IsContentActive(poi.poiType.dlcID)));
    return configs;
  }

  public struct ArtifactPOIParams
  {
    public string id;
    public string anim;
    public StringKey nameStringKey;
    public StringKey descStringKey;
    public ArtifactPOIConfigurator.ArtifactPOIType poiType;

    public ArtifactPOIParams(string anim, ArtifactPOIConfigurator.ArtifactPOIType poiType)
    {
      this.id = "ArtifactSpacePOI_" + poiType.id;
      this.anim = anim;
      this.nameStringKey = new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + poiType.id.ToUpper() + ".NAME");
      this.descStringKey = new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + poiType.id.ToUpper() + ".DESC");
      this.poiType = poiType;
    }
  }
}
