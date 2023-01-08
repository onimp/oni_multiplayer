// Decompiled with JetBrains decompiler
// Type: SwampLilyFlowerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class SwampLilyFlowerConfig : IEntityConfig
{
  public static float SEEDS_PER_FRUIT = 1f;
  public static string ID = "SwampLilyFlower";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string id = SwampLilyFlowerConfig.ID;
    string name = (string) ITEMS.INGREDIENTS.SWAMPLILYFLOWER.NAME;
    string desc = (string) ITEMS.INGREDIENTS.SWAMPLILYFLOWER.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("swamplilyflower_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IndustrialIngredient);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, false, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true, additionalTags: additionalTags);
    EntityTemplates.CreateAndRegisterCompostableFromPrefab(looseEntity);
    looseEntity.AddOrGet<EntitySplitter>();
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
