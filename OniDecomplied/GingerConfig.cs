// Decompiled with JetBrains decompiler
// Type: GingerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class GingerConfig : IEntityConfig
{
  public static string ID = nameof (GingerConfig);
  public static int SORTORDER = 1;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string id = GingerConfig.ID;
    string name = (string) ITEMS.INGREDIENTS.GINGER.NAME;
    string desc = (string) ITEMS.INGREDIENTS.GINGER.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("ginger_kanim"));
    int sortOrder = TUNING.SORTORDER.BUILDINGELEMENTS + GingerConfig.SORTORDER;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IndustrialIngredient);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, true, anim, "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.RECTANGLE, 0.45f, 0.4f, true, sortOrder, additionalTags: additionalTags);
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
