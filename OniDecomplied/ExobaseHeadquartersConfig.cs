// Decompiled with JetBrains decompiler
// Type: ExobaseHeadquartersConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class ExobaseHeadquartersConfig : IBuildingConfig
{
  public const string ID = "ExobaseHeadquarters";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
    string[] allMinerals = MATERIALS.ALL_MINERALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR5 = BUILDINGS.DECOR.BONUS.TIER5;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ExobaseHeadquarters", 3, 3, "porta_pod_y_kanim", 250, 30f, tieR1, allMinerals, 1600f, BuildLocationRule.OnFloor, tieR5, noise);
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.BaseTimeUntilRepair = 400f;
    buildingDef.ShowInBuildMenu = true;
    buildingDef.DefaultAnimState = "idle";
    buildingDef.OnePerWorld = true;
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_LP", NOISE_POLLUTION.NOISY.TIER3);
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_open", NOISE_POLLUTION.NOISY.TIER4);
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_close", NOISE_POLLUTION.NOISY.TIER4);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    LoreBearerUtil.AddLoreTo(go);
    Telepad telepad = go.AddOrGet<Telepad>();
    go.GetComponent<KPrefabID>().AddTag(GameTags.Telepad, false);
    telepad.startingSkillPoints = 1f;
    SocialGatheringPoint socialGatheringPoint = go.AddOrGet<SocialGatheringPoint>();
    socialGatheringPoint.choreOffsets = new CellOffset[6]
    {
      new CellOffset(-1, 0),
      new CellOffset(-2, 0),
      new CellOffset(2, 0),
      new CellOffset(3, 0),
      new CellOffset(0, 0),
      new CellOffset(1, 0)
    };
    socialGatheringPoint.choreCount = 4;
    socialGatheringPoint.basePriority = RELAXATION.PRIORITY.TIER0;
    Light2D light2D = go.AddOrGet<Light2D>();
    light2D.Color = LIGHT2D.HEADQUARTERS_COLOR;
    light2D.Range = 5f;
    light2D.Offset = LIGHT2D.EXOBASE_HEADQUARTERS_OFFSET;
    light2D.overlayColour = LIGHT2D.HEADQUARTERS_OVERLAYCOLOR;
    light2D.shape = LightShape.Circle;
    light2D.drawOverlay = true;
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource, false);
    go.GetComponent<KPrefabID>().AddTag(GameTags.Experimental, false);
    RoleStation roleStation = go.AddOrGet<RoleStation>();
    roleStation.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_hqbase_skill_upgrade_kanim"))
    };
    roleStation.workAnims = new HashedString[1]
    {
      HashedString.op_Implicit("upgrade")
    };
    roleStation.workingPstComplete = (HashedString[]) null;
    roleStation.workingPstFailed = (HashedString[]) null;
    Activatable activatable = go.AddOrGet<Activatable>();
    activatable.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_porta_pod_kanim"))
    };
    activatable.workAnims = new HashedString[2]
    {
      HashedString.op_Implicit("activate_pre"),
      HashedString.op_Implicit("activate_loop")
    };
    activatable.workingPstComplete = new HashedString[1]
    {
      HashedString.op_Implicit("activate_pst")
    };
    activatable.workingPstFailed = new HashedString[1]
    {
      HashedString.op_Implicit("activate_pre")
    };
    activatable.SetWorkTime(15f);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
