// Decompiled with JetBrains decompiler
// Type: HeatCubeConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class HeatCubeConfig : IEntityConfig
{
  public const string ID = "HeatCube";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("artifacts_kanim"));
    int buildingelements = SORTORDER.BUILDINGELEMENTS;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.MiscPickupable);
    additionalTags.Add(GameTags.IndustrialIngredient);
    return EntityTemplates.CreateLooseEntity("HeatCube", "Heat Cube", "A cube that holds heat.", 1000f, true, anim, "idle_tallstone", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, isPickupable: true, sortOrder: buildingelements, element: SimHashes.Diamond, additionalTags: additionalTags);
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
