// Decompiled with JetBrains decompiler
// Type: LonelyMinionMailboxConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LonelyMinionMailboxConfig : IBuildingConfig
{
  public const string ID = "LonelyMailBox";
  public static readonly HashedString IdHash = HashedString.op_Implicit("LonelyMailBox");
  public static readonly HashedString CleanupAnimation = HashedString.op_Implicit("");

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR2_2 = BUILDINGS.DECOR.PENALTY.TIER2;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LonelyMailBox", 2, 2, "parcel_delivery_kanim", 10, 30f, tieR2_1, allMetals, 800f, BuildLocationRule.OnFloor, tieR2_2, noise);
    buildingDef.SceneLayer = Grid.SceneLayer.BuildingBack;
    buildingDef.DefaultAnimState = "idle";
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.ShowInBuildMenu = false;
    buildingDef.ViewMode = OverlayModes.None.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "small";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    SingleEntityReceptacle entityReceptacle = go.AddComponent<SingleEntityReceptacle>();
    entityReceptacle.AddDepositTag(GameTags.Edible);
    ((Behaviour) entityReceptacle).enabled = false;
    go.AddComponent<LonelyMinionMailbox>();
    go.GetComponent<Deconstructable>().allowDeconstruction = false;
    Storage storage = go.AddOrGet<Storage>();
    storage.allowItemRemoval = false;
    storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Seal,
      Storage.StoredItemModifier.Preserve
    });
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
