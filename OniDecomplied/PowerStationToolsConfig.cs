// Decompiled with JetBrains decompiler
// Type: PowerStationToolsConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class PowerStationToolsConfig : IEntityConfig
{
  public const string ID = "PowerStationTools";
  public static readonly Tag tag = TagManager.Create("PowerStationTools");
  public const float MASS = 5f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME;
    string desc = (string) ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("kit_electrician_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.MiscPickupable);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("PowerStationTools", name, desc, 5f, true, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true, additionalTags: additionalTags);
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
