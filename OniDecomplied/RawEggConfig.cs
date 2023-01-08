// Decompiled with JetBrains decompiler
// Type: RawEggConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class RawEggConfig : IEntityConfig
{
  public const string ID = "RawEgg";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("RawEgg", (string) ITEMS.FOOD.RAWEGG.NAME, (string) ITEMS.FOOD.RAWEGG.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("rawegg_kanim")), "object", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true);
    EntityTemplates.ExtendEntityToFood(looseEntity, TUNING.FOOD.FOOD_TYPES.RAWEGG);
    TemperatureCookable temperatureCookable = looseEntity.AddOrGet<TemperatureCookable>();
    temperatureCookable.cookTemperature = 344.15f;
    temperatureCookable.cookedID = "CookedEgg";
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
