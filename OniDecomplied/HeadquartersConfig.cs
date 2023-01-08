// Decompiled with JetBrains decompiler
// Type: HeadquartersConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TUNING;
using UnityEngine;

public class HeadquartersConfig : IBuildingConfig
{
  public const string ID = "Headquarters";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR7 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER7;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR5 = TUNING.BUILDINGS.DECOR.BONUS.TIER5;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Headquarters", 4, 4, "hqbase_kanim", 250, 30f, tieR7, allMetals, 1600f, BuildLocationRule.OnFloor, tieR5, noise);
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.BaseTimeUntilRepair = 400f;
    buildingDef.ShowInBuildMenu = false;
    buildingDef.DefaultAnimState = "idle";
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_LP", NOISE_POLLUTION.NOISY.TIER3);
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_open", NOISE_POLLUTION.NOISY.TIER4);
    SoundEventVolumeCache.instance.AddVolume("hqbase_kanim", "Portal_close", NOISE_POLLUTION.NOISY.TIER4);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    LoreBearerUtil.AddLoreTo(go, LoreBearerUtil.UnlockSpecificEntry("pod_evacuation", (string) UI.USERMENUACTIONS.READLORE.SEARCH_POD));
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
    light2D.Offset = LIGHT2D.HEADQUARTERS_OFFSET;
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
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
