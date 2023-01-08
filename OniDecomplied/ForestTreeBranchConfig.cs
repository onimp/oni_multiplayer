// Decompiled with JetBrains decompiler
// Type: ForestTreeBranchConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ForestTreeBranchConfig : IEntityConfig
{
  public const string ID = "ForestTreeBranch";
  public const float WOOD_AMOUNT = 300f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME;
    string desc = (string) STRINGS.CREATURES.SPECIES.WOOD_TREE.DESC;
    EffectorValues tieR1 = TUNING.DECOR.BONUS.TIER1;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("tree_kanim"));
    EffectorValues decor = tieR1;
    EffectorValues noise = new EffectorValues();
    List<Tag> additionalTags = new List<Tag>();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("ForestTreeBranch", name, desc, 8f, anim, "idle_empty", Grid.SceneLayer.BuildingFront, 1, 1, decor, noise, additionalTags: additionalTags, defaultTemperature: 298.15f);
    EntityTemplates.ExtendEntityToBasicPlant(placedEntity, 258.15f, 288.15f, 313.15f, 448.15f, crop_id: "WoodLog", require_solid_tile: false, max_age: 12000f, max_radiation: 9800f, baseTraitId: "ForestTreeBranchOriginal", baseTraitName: ((string) STRINGS.CREATURES.SPECIES.WOOD_TREE.NAME));
    placedEntity.AddOrGet<TreeBud>();
    placedEntity.AddOrGet<StandardCropPlant>();
    placedEntity.AddOrGet<BudUprootedMonitor>();
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
