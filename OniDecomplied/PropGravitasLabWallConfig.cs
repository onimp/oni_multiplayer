// Decompiled with JetBrains decompiler
// Type: PropGravitasLabWallConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class PropGravitasLabWallConfig : IBuildingConfig
{
  public const string ID = "PropGravitasLabWall";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR0_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
    string[] rawMinerals = MATERIALS.RAW_MINERALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR0_2 = DECOR.BONUS.TIER0;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("PropGravitasLabWall", 2, 3, "gravitas_lab_wall_kanim", 30, 30f, tieR0_1, rawMinerals, 1600f, BuildLocationRule.NotInTiles, tieR0_2, noise);
    buildingDef.PermittedRotations = PermittedRotations.R90;
    buildingDef.Entombable = false;
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.BaseTimeUntilRepair = -1f;
    buildingDef.DefaultAnimState = "on";
    buildingDef.ObjectLayer = ObjectLayer.Backwall;
    buildingDef.SceneLayer = Grid.SceneLayer.Backwall;
    buildingDef.ShowInBuildMenu = false;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
    go.AddComponent<ZoneTile>();
    go.GetComponent<PrimaryElement>().SetElement(SimHashes.Glass);
    go.GetComponent<PrimaryElement>().Temperature = 273f;
    go.GetComponent<KPrefabID>().AddTag(GameTags.Gravitas, false);
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
