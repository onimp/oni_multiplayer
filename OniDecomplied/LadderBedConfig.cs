// Decompiled with JetBrains decompiler
// Type: LadderBedConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class LadderBedConfig : IBuildingConfig
{
  public static string ID = "LadderBed";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    string id = LadderBedConfig.ID;
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(id, 2, 2, "ladder_bed_kanim", 10, 10f, tieR3, refinedMetals, 1600f, BuildLocationRule.OnFloorOrBuildingAttachPoint, none2, noise);
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.PermittedRotations = PermittedRotations.FlipH;
    buildingDef.attachablePosition = new CellOffset(0, 0);
    buildingDef.AttachmentSlotTag = GameTags.LadderBed;
    buildingDef.ObjectLayer = ObjectLayer.Building;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.BedType, false);
    go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
    {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 2), GameTags.LadderBed, (AttachableBuilding) null)
    };
    go.AddOrGet<AnimTileable>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    CellOffset[] cellOffsetArray = new CellOffset[2]
    {
      new CellOffset(0, 0),
      new CellOffset(0, 1)
    };
    Ladder ladder = go.AddOrGet<Ladder>();
    ladder.upwardsMovementSpeedMultiplier = 0.75f;
    ladder.downwardsMovementSpeedMultiplier = 0.75f;
    ladder.offsets = cellOffsetArray;
    go.AddOrGetDef<LadderBed.Def>().offsets = cellOffsetArray;
    go.GetComponent<KAnimControllerBase>().initialAnim = "off";
    Bed bed = go.AddOrGet<Bed>();
    bed.effects = new string[2]
    {
      "LadderBedStamina",
      "BedHealth"
    };
    bed.workLayer = Grid.SceneLayer.BuildingFront;
    Sleepable sleepable = go.AddOrGet<Sleepable>();
    sleepable.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_ladder_bed_kanim"))
    };
    sleepable.workLayer = Grid.SceneLayer.BuildingFront;
    go.AddOrGet<Ownable>().slotID = Db.Get().AssignableSlots.Bed.Id;
    go.AddOrGetDef<RocketUsageRestriction.Def>();
  }
}
