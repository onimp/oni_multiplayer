// Decompiled with JetBrains decompiler
// Type: MushroomWrapConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class MushroomWrapConfig : IEntityConfig
{
  public const string ID = "MushroomWrap";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("MushroomWrap", (string) ITEMS.FOOD.MUSHROOMWRAP.NAME, (string) ITEMS.FOOD.MUSHROOMWRAP.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("mushroom_wrap_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.5f, true), TUNING.FOOD.FOOD_TYPES.MUSHROOM_WRAP);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
