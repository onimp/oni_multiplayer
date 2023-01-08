// Decompiled with JetBrains decompiler
// Type: WoodLogConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class WoodLogConfig : IEntityConfig
{
  public const string ID = "WoodLog";
  public static readonly Tag TAG = TagManager.Create("WoodLog");

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) ITEMS.INDUSTRIAL_PRODUCTS.WOOD.NAME;
    string desc = (string) ITEMS.INDUSTRIAL_PRODUCTS.WOOD.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("wood_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IndustrialIngredient);
    additionalTags.Add(GameTags.Organics);
    additionalTags.Add(GameTags.BuildingWood);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("WoodLog", name, desc, 1f, false, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, true, additionalTags: additionalTags);
    looseEntity.AddOrGet<EntitySplitter>();
    looseEntity.AddOrGet<SimpleMassStatusItem>();
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
