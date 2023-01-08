// Decompiled with JetBrains decompiler
// Type: NuclearWasteCometConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class NuclearWasteCometConfig : IEntityConfig
{
  public static string ID = "NuclearWasteComet";
  public static float MASS = 1f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(NuclearWasteCometConfig.ID, (string) UI.SPACEDESTINATIONS.COMETS.NUCLEAR_WASTE.NAME);
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<LoopingSounds>();
    Comet comet = entity.AddOrGet<Comet>();
    comet.massRange = new Vector2(NuclearWasteCometConfig.MASS, NuclearWasteCometConfig.MASS);
    comet.EXHAUST_ELEMENT = SimHashes.Fallout;
    comet.EXHAUST_RATE = NuclearWasteCometConfig.MASS * 0.2f;
    comet.temperatureRange = new Vector2(473.15f, 573.15f);
    comet.entityDamage = 2;
    comet.totalTileDamage = 0.45f;
    comet.splashRadius = 0;
    comet.impactSound = "Meteor_Nuclear_Impact";
    comet.flyingSoundID = 3;
    comet.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
    comet.addTiles = 1;
    comet.diseaseIdx = Db.Get().Diseases.GetIndex(HashedString.op_Implicit(Db.Get().Diseases.RadiationPoisoning.Id));
    comet.addDiseaseCount = 1000000;
    PrimaryElement primaryElement = entity.AddOrGet<PrimaryElement>();
    primaryElement.SetElement(SimHashes.Corium);
    primaryElement.Temperature = (float) (((double) comet.temperatureRange.x + (double) comet.temperatureRange.y) / 2.0);
    KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("nuclear_metldown_comet_fx_kanim"))
    };
    kbatchedAnimController.isMovable = true;
    kbatchedAnimController.initialAnim = "fall_loop";
    kbatchedAnimController.initialMode = (KAnim.PlayMode) 0;
    kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
    entity.AddOrGet<KCircleCollider2D>().radius = 0.5f;
    entity.transform.localScale = new Vector3(0.3f, 0.3f, 1f);
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
