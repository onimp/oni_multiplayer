// Decompiled with JetBrains decompiler
// Type: SwampDelightsConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class SwampDelightsConfig : IEntityConfig
{
  public const string ID = "SwampDelights";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("SwampDelights", (string) ITEMS.FOOD.SWAMPDELIGHTS.NAME, (string) ITEMS.FOOD.SWAMPDELIGHTS.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("swamp_delights_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.7f, true), TUNING.FOOD.FOOD_TYPES.SWAMP_DELIGHTS);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
