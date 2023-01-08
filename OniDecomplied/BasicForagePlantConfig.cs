// Decompiled with JetBrains decompiler
// Type: BasicForagePlantConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BasicForagePlantConfig : IEntityConfig
{
  public const string ID = "BasicForagePlant";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFood(EntityTemplates.CreateLooseEntity("BasicForagePlant", (string) ITEMS.FOOD.BASICFORAGEPLANT.NAME, (string) ITEMS.FOOD.BASICFORAGEPLANT.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("muckrootvegetable_kanim")), "object", Grid.SceneLayer.BuildingBack, EntityTemplates.CollisionShape.CIRCLE, 0.3f, 0.3f, true), TUNING.FOOD.FOOD_TYPES.BASICFORAGEPLANT);

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
