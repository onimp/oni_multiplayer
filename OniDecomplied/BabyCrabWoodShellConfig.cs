// Decompiled with JetBrains decompiler
// Type: BabyCrabWoodShellConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class BabyCrabWoodShellConfig : IEntityConfig
{
  public const string ID = "BabyCrabWoodShell";
  public static readonly Tag TAG = TagManager.Create("BabyCrabWoodShell");
  public const float MASS = 10f;
  public const string symbolPrefix = "wood_";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) ITEMS.INDUSTRIAL_PRODUCTS.BABY_CRAB_SHELL.VARIANT_WOOD.NAME;
    string desc = (string) ITEMS.INDUSTRIAL_PRODUCTS.BABY_CRAB_SHELL.VARIANT_WOOD.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("crabshells_small_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IndustrialIngredient);
    additionalTags.Add(GameTags.Organics);
    additionalTags.Add(GameTags.MoltShell);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("BabyCrabWoodShell", name, desc, 10f, true, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.9f, 0.6f, true, additionalTags: additionalTags);
    looseEntity.AddOrGet<EntitySplitter>();
    looseEntity.AddOrGet<SimpleMassStatusItem>().symbolPrefix = "wood_";
    SymbolOverrideControllerUtil.AddToPrefab(looseEntity).ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit("crabshells_small_kanim")), "wood_");
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
