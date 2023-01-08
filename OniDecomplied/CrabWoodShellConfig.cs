// Decompiled with JetBrains decompiler
// Type: CrabWoodShellConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class CrabWoodShellConfig : IEntityConfig
{
  public const string ID = "CrabWoodShell";
  public static readonly Tag TAG = TagManager.Create("CrabWoodShell");
  public const float MASS = 100f;
  public const string symbolPrefix = "wood_";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.VARIANT_WOOD.NAME;
    string desc = (string) ITEMS.INDUSTRIAL_PRODUCTS.CRAB_SHELL.VARIANT_WOOD.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("crabshells_large_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IndustrialIngredient);
    additionalTags.Add(GameTags.Organics);
    additionalTags.Add(GameTags.MoltShell);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("CrabWoodShell", name, desc, 100f, true, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, true, additionalTags: additionalTags);
    looseEntity.AddOrGet<EntitySplitter>();
    looseEntity.AddOrGet<SimpleMassStatusItem>().symbolPrefix = "wood_";
    SymbolOverrideControllerUtil.AddToPrefab(looseEntity).ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit("crabshells_large_kanim")), "wood_");
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
