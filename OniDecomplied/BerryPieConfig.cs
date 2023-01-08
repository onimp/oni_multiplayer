// Decompiled with JetBrains decompiler
// Type: BerryPieConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BerryPieConfig : IEntityConfig
{
  public const string ID = "BerryPie";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("BerryPie", (string) ITEMS.FOOD.BERRYPIE.NAME, (string) ITEMS.FOOD.BERRYPIE.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("wormwood_berry_pie_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.55f, true), TUNING.FOOD.FOOD_TYPES.BERRY_PIE);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
