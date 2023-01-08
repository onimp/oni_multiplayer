// Decompiled with JetBrains decompiler
// Type: GraveConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GraveConfig : IBuildingConfig
{
  public const string ID = "Grave";
  private static KAnimFile[] STORAGE_OVERRIDE_ANIM_FILES;
  private static readonly HashedString[] STORAGE_WORK_ANIMS = new HashedString[1]
  {
    HashedString.op_Implicit("working_pre")
  };
  private static readonly HashedString STORAGE_PST_ANIM = HashedString.Invalid;
  private static readonly List<Storage.StoredItemModifier> StorageModifiers = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Preserve
  };

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] rawMinerals = MATERIALS.RAW_MINERALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Grave", 1, 2, "gravestone_kanim", 30, 120f, tieR5, rawMinerals, 1600f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.Overheatable = false;
    buildingDef.Floodable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.BaseTimeUntilRepair = -1f;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    GraveConfig.STORAGE_OVERRIDE_ANIM_FILES = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_bury_dupe_kanim"))
    };
    Storage storage = go.AddOrGet<Storage>();
    storage.showInUI = true;
    storage.SetDefaultStoredItemModifiers(GraveConfig.StorageModifiers);
    storage.overrideAnims = GraveConfig.STORAGE_OVERRIDE_ANIM_FILES;
    storage.workAnims = GraveConfig.STORAGE_WORK_ANIMS;
    storage.workingPstComplete = new HashedString[1]
    {
      GraveConfig.STORAGE_PST_ANIM
    };
    storage.synchronizeAnims = false;
    storage.useGunForDelivery = false;
    storage.workAnimPlayMode = (KAnim.PlayMode) 1;
    go.AddOrGet<Grave>();
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
