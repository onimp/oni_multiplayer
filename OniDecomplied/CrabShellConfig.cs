// Decompiled with JetBrains decompiler
// Type: CrabShellConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class CrabShellConfig : IEntityConfig
{
  public const string ID = "CrabShell";
  public static readonly Tag TAG = TagManager.Create("CrabShell");
  public const float MASS = 10f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.NAME;
    string desc = (string) ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("crabshells_large_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IndustrialIngredient);
    additionalTags.Add(GameTags.Organics);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("CrabShell", name, desc, 10f, true, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, true, additionalTags: additionalTags);
    looseEntity.AddOrGet<EntitySplitter>();
    looseEntity.AddOrGet<SimpleMassStatusItem>();
    EntityTemplates.CreateAndRegisterCompostableFromPrefab(looseEntity);
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
