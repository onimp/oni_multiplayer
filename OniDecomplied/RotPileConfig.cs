// Decompiled with JetBrains decompiler
// Type: RotPileConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using UnityEngine;

public class RotPileConfig : IEntityConfig
{
  public static string ID = "RotPile";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject looseEntity = EntityTemplates.CreateLooseEntity(RotPileConfig.ID, (string) ITEMS.FOOD.ROTPILE.NAME, (string) ITEMS.FOOD.ROTPILE.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("rotfood_kanim")), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.4f, true);
    KPrefabID component = looseEntity.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Organics, false);
    component.AddTag(GameTags.Compostable, false);
    looseEntity.AddOrGet<EntitySplitter>();
    looseEntity.AddOrGet<OccupyArea>();
    looseEntity.AddOrGet<Modifiers>();
    looseEntity.AddOrGet<RotPile>();
    looseEntity.AddComponent<DecorProvider>().SetValues(TUNING.DECOR.PENALTY.TIER2);
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst) => inst.GetComponent<DecorProvider>().overrideName = (string) ITEMS.FOOD.ROTPILE.NAME;

  public void OnSpawn(GameObject inst)
  {
  }
}
