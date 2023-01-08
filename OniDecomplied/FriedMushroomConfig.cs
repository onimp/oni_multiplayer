// Decompiled with JetBrains decompiler
// Type: FriedMushroomConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class FriedMushroomConfig : IEntityConfig
{
  public const string ID = "FriedMushroom";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("FriedMushroom", (string) ITEMS.FOOD.FRIEDMUSHROOM.NAME, (string) ITEMS.FOOD.FRIEDMUSHROOM.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("funguscapfried_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.6f, true), TUNING.FOOD.FOOD_TYPES.FRIED_MUSHROOM);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
