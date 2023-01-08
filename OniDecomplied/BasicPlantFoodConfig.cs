// Decompiled with JetBrains decompiler
// Type: BasicPlantFoodConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BasicPlantFoodConfig : IEntityConfig
{
  public const string ID = "BasicPlantFood";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("BasicPlantFood", (string) ITEMS.FOOD.BASICPLANTFOOD.NAME, (string) ITEMS.FOOD.BASICPLANTFOOD.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("meallicegrain_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.25f, 0.25f, true);
    EntityTemplates.ExtendEntityToFood(looseEntity, TUNING.FOOD.FOOD_TYPES.BASICPLANTFOOD);
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
