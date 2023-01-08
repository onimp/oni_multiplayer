// Decompiled with JetBrains decompiler
// Type: WormBasicFruitConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class WormBasicFruitConfig : IEntityConfig
{
  public const string ID = "WormBasicFruit";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("WormBasicFruit", (string) ITEMS.FOOD.WORMBASICFRUIT.NAME, (string) ITEMS.FOOD.WORMBASICFRUIT.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("wormwood_basic_fruit_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.7f, 0.4f, true), TUNING.FOOD.FOOD_TYPES.WORMBASICFRUIT);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
