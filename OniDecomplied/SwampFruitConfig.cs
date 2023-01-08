// Decompiled with JetBrains decompiler
// Type: SwampFruitConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class SwampFruitConfig : IEntityConfig
{
  public static string ID = "SwampFruit";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity(SwampFruitConfig.ID, (string) ITEMS.FOOD.SWAMPFRUIT.NAME, (string) ITEMS.FOOD.SWAMPFRUIT.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("swampcrop_fruit_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, height: 0.72f, isPickupable: true), TUNING.FOOD.FOOD_TYPES.SWAMPFRUIT);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
