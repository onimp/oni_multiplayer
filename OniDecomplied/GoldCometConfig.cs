// Decompiled with JetBrains decompiler
// Type: GoldCometConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class GoldCometConfig : IEntityConfig
{
  public static string ID = "GoldComet";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(GoldCometConfig.ID, (string) UI.SPACEDESTINATIONS.COMETS.GOLDCOMET.NAME);
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<LoopingSounds>();
    Comet comet = entity.AddOrGet<Comet>();
    comet.massRange = new Vector2(3f, 20f);
    comet.temperatureRange = new Vector2(323.15f, 423.15f);
    comet.explosionOreCount = new Vector2I(2, 4);
    comet.entityDamage = 15;
    comet.totalTileDamage = 0.5f;
    comet.splashRadius = 1;
    comet.impactSound = "Meteor_Medium_Impact";
    comet.flyingSoundID = 1;
    comet.explosionEffectHash = SpawnFXHashes.MeteorImpactMetal;
    PrimaryElement primaryElement = entity.AddOrGet<PrimaryElement>();
    primaryElement.SetElement(SimHashes.GoldAmalgam);
    primaryElement.Temperature = (float) (((double) comet.temperatureRange.x + (double) comet.temperatureRange.y) / 2.0);
    KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("meteor_gold_kanim"))
    };
    kbatchedAnimController.isMovable = true;
    kbatchedAnimController.initialAnim = "fall_loop";
    kbatchedAnimController.initialMode = (KAnim.PlayMode) 0;
    kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
    entity.AddOrGet<KCircleCollider2D>().radius = 0.5f;
    entity.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
    entity.AddTag(GameTags.Comet);
    return entity;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject go)
  {
  }
}
