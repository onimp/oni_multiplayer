// Decompiled with JetBrains decompiler
// Type: MonumentMiddleConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using TUNING;
using UnityEngine;

public class MonumentMiddleConfig : IBuildingConfig
{
  public const string ID = "MonumentMiddle";

  public override BuildingDef CreateBuildingDef()
  {
    float[] construction_mass = new float[3]
    {
      2500f,
      2500f,
      5000f
    };
    string[] construction_materials = new string[3]
    {
      SimHashes.Ceramic.ToString(),
      SimHashes.Polypropylene.ToString(),
      SimHashes.Steel.ToString()
    };
    EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
    EffectorValues incomplete = BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE;
    EffectorValues noise = tieR2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MonumentMiddle", 5, 5, "monument_mid_a_kanim", 1000, 60f, construction_mass, construction_materials, 9999f, BuildLocationRule.BuildingAttachPoint, incomplete, noise);
    BuildingTemplates.CreateMonumentBuildingDef(buildingDef);
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.OverheatTemperature = 2273.15f;
    buildingDef.Floodable = false;
    buildingDef.AttachmentSlotTag = Tag.op_Implicit("MonumentMiddle");
    buildingDef.ObjectLayer = ObjectLayer.Building;
    buildingDef.PermittedRotations = PermittedRotations.FlipH;
    buildingDef.attachablePosition = new CellOffset(0, 0);
    buildingDef.RequiresPowerInput = false;
    buildingDef.CanMove = false;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    go.AddOrGet<LoopingSounds>();
    go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
    {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), Tag.op_Implicit("MonumentTop"), (AttachableBuilding) null)
    };
    go.AddOrGet<MonumentPart>().part = MonumentPartResource.Part.Middle;
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<KBatchedAnimController>().initialAnim = "option_a";
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabSpawnFn += MonumentMiddleConfig.\u003C\u003Ec.\u003C\u003E9__5_0 ?? (MonumentMiddleConfig.\u003C\u003Ec.\u003C\u003E9__5_0 = new KPrefabID.PrefabFn((object) MonumentMiddleConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__5_0)));
  }
}
