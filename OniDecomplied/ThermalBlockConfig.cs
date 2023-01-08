// Decompiled with JetBrains decompiler
// Type: ThermalBlockConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ThermalBlockConfig : IBuildingConfig
{
  public const string ID = "ThermalBlock";
  private static readonly CellOffset[] overrideOffsets = new CellOffset[4]
  {
    new CellOffset(-1, -1),
    new CellOffset(1, -1),
    new CellOffset(-1, 1),
    new CellOffset(1, 1)
  };

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] anyBuildable = MATERIALS.ANY_BUILDABLE;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ThermalBlock", 1, 1, "thermalblock_kanim", 30, 120f, tieR5, anyBuildable, 1600f, BuildLocationRule.NotInTiles, none2, noise);
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.BaseTimeUntilRepair = -1f;
    buildingDef.ViewMode = OverlayModes.Temperature.ID;
    buildingDef.DefaultAnimState = "off";
    buildingDef.ObjectLayer = ObjectLayer.Backwall;
    buildingDef.SceneLayer = Grid.SceneLayer.Backwall;
    buildingDef.ReplacementLayer = ObjectLayer.ReplacementBackwall;
    buildingDef.ReplacementCandidateLayers = new List<ObjectLayer>()
    {
      ObjectLayer.FoundationTile,
      ObjectLayer.Backwall
    };
    List<Tag> tagList = new List<Tag>();
    tagList.Add(GameTags.FloorTiles);
    tagList.Add(GameTags.Backwall);
    buildingDef.ReplacementTags = tagList;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<AnimTileable>().objectLayer = ObjectLayer.Backwall;
    go.AddComponent<ZoneTile>();
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    KPrefabID component = go.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Backwall, false);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    component.prefabSpawnFn += ThermalBlockConfig.\u003C\u003Ec.\u003C\u003E9__4_0 ?? (ThermalBlockConfig.\u003C\u003Ec.\u003C\u003E9__4_0 = new KPrefabID.PrefabFn((object) ThermalBlockConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__4_0)));
  }
}
