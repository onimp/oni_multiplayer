// Decompiled with JetBrains decompiler
// Type: WormSuperFoodConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class WormSuperFoodConfig : IEntityConfig
{
  public const string ID = "WormSuperFood";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("WormSuperFood", (string) ITEMS.FOOD.WORMSUPERFOOD.NAME, (string) ITEMS.FOOD.WORMSUPERFOOD.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("wormwood_preserved_berries_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.7f, 0.6f, true), TUNING.FOOD.FOOD_TYPES.WORMSUPERFOOD);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
