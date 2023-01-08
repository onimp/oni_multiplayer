// Decompiled with JetBrains decompiler
// Type: PickledMealConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class PickledMealConfig : IEntityConfig
{
  public const string ID = "PickledMeal";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject food = EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("PickledMeal", (string) ITEMS.FOOD.PICKLEDMEAL.NAME, (string) ITEMS.FOOD.PICKLEDMEAL.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("pickledmeal_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.6f, 0.7f, true), TUNING.FOOD.FOOD_TYPES.PICKLEDMEAL);
    food.GetComponent<KPrefabID>().AddTag(GameTags.Pickled, false);
    return food;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
