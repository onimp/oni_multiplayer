// Decompiled with JetBrains decompiler
// Type: FoodCometConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class FoodCometConfig : IEntityConfig
{
  public static string ID = "FoodComet";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(FoodCometConfig.ID, (string) UI.SPACEDESTINATIONS.COMETS.FOODCOMET.NAME);
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<LoopingSounds>();
    Comet comet = entity.AddOrGet<Comet>();
    comet.massRange = new Vector2(0.2f, 0.5f);
    comet.temperatureRange = new Vector2(298.15f, 303.15f);
    comet.entityDamage = 0;
    comet.totalTileDamage = 0.0f;
    comet.splashRadius = 0;
    comet.impactSound = "Meteor_Small_Impact";
    comet.flyingSoundID = 0;
    comet.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
    comet.canHitDuplicants = true;
    PrimaryElement primaryElement = entity.AddOrGet<PrimaryElement>();
    primaryElement.SetElement(SimHashes.Creature);
    primaryElement.Temperature = (float) (((double) comet.temperatureRange.x + (double) comet.temperatureRange.y) / 2.0);
    KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("meteor_sand_kanim"))
    };
    kbatchedAnimController.isMovable = true;
    kbatchedAnimController.initialAnim = "fall_loop";
    kbatchedAnimController.initialMode = (KAnim.PlayMode) 0;
    kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
    entity.AddOrGet<KCircleCollider2D>().radius = 0.5f;
    entity.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
    comet.EXHAUST_ELEMENT = SimHashes.Void;
    entity.AddTag(GameTags.Comet);
    return entity;
  }

  public void OnPrefabInit(GameObject go) => go.GetComponent<Comet>().OnImpact += (System.Action) (() =>
  {
    int num = 10;
    while (num > 0)
    {
      --num;
      Vector3 pos = Vector3.op_Addition(go.transform.position, new Vector3((float) Random.Range(-2, 3), (float) Random.Range(-2, 3), 0.0f));
      if (!Grid.Solid[Grid.PosToCell(pos)])
      {
        GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("FoodSplat")), pos);
        gameObject.SetActive(true);
        gameObject.transform.Rotate(0.0f, 0.0f, (float) Random.Range(-90, 90));
        num = 0;
      }
    }
  });

  public void OnSpawn(GameObject go)
  {
  }
}
