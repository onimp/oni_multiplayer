// Decompiled with JetBrains decompiler
// Type: BasicFabricConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class BasicFabricConfig : IEntityConfig
{
  public static string ID = "BasicFabric";
  private AttributeModifier decorModifier = new AttributeModifier("Decor", 0.1f, (string) ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME, true);

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string id = BasicFabricConfig.ID;
    string name = (string) ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.NAME;
    string desc = (string) ITEMS.INDUSTRIAL_PRODUCTS.BASIC_FABRIC.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("swampreedwool_kanim"));
    int sortOrder = SORTORDER.BUILDINGELEMENTS + BasicFabricTuning.SORTORDER;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IndustrialIngredient);
    additionalTags.Add(GameTags.BuildingFiber);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, true, anim, "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.45f, true, sortOrder, additionalTags: additionalTags);
    looseEntity.AddOrGet<EntitySplitter>();
    looseEntity.AddOrGet<PrefabAttributeModifiers>().AddAttributeDescriptor(this.decorModifier);
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
