// Decompiled with JetBrains decompiler
// Type: OrbitalResearchDatabankConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalResearchDatabankConfig : IEntityConfig
{
  public const string ID = "OrbitalResearchDatabank";
  public static readonly Tag TAG = TagManager.Create("OrbitalResearchDatabank");
  public const float MASS = 1f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name = (string) ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.NAME;
    string desc = (string) ITEMS.INDUSTRIAL_PRODUCTS.ORBITAL_RESEARCH_DATABANK.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("floppy_disc_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IndustrialIngredient);
    additionalTags.Add(GameTags.Experimental);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("OrbitalResearchDatabank", name, desc, 1f, true, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, true, additionalTags: additionalTags);
    looseEntity.AddOrGet<EntitySplitter>().maxStackSize = (float) TUNING.ROCKETRY.DESTINATION_RESEARCH.BASIC;
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
