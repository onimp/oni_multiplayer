// Decompiled with JetBrains decompiler
// Type: GasGrassHarvestedConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class GasGrassHarvestedConfig : IEntityConfig
{
  public const string ID = "GasGrassHarvested";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) CREATURES.SPECIES.GASGRASS.NAME;
    string desc = (string) CREATURES.SPECIES.GASGRASS.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("harvested_gassygrass_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Other);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("GasGrassHarvested", name, desc, 1f, false, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, true, additionalTags: additionalTags);
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
