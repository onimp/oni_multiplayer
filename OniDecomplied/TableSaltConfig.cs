// Decompiled with JetBrains decompiler
// Type: TableSaltConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class TableSaltConfig : IEntityConfig
{
  public static string ID = "TableSalt";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string id = TableSaltConfig.ID;
    string name = (string) ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.NAME;
    string desc = (string) ITEMS.INDUSTRIAL_PRODUCTS.TABLE_SALT.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("seed_saltPlant_kanim"));
    int sortOrder = SORTORDER.BUILDINGELEMENTS + TableSaltTuning.SORTORDER;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Other);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, false, anim, "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.45f, true, sortOrder, SimHashes.Salt, additionalTags);
    looseEntity.AddOrGet<EntitySplitter>();
    return looseEntity;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
