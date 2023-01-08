// Decompiled with JetBrains decompiler
// Type: GassyMooCometConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class GassyMooCometConfig : IEntityConfig
{
  public static string ID = "GassyMoo";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(GassyMooCometConfig.ID, (string) UI.SPACEDESTINATIONS.COMETS.GASSYMOOCOMET.NAME);
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<LoopingSounds>();
    GassyMooComet gassyMooComet = entity.AddOrGet<GassyMooComet>();
    gassyMooComet.massRange = new Vector2(100f, 200f);
    gassyMooComet.EXHAUST_ELEMENT = SimHashes.Methane;
    gassyMooComet.temperatureRange = new Vector2(296.15f, 318.15f);
    gassyMooComet.entityDamage = 0;
    gassyMooComet.explosionOreCount = new Vector2I(0, 0);
    gassyMooComet.totalTileDamage = 0.0f;
    gassyMooComet.splashRadius = 1;
    gassyMooComet.impactSound = "Meteor_GassyMoo_Impact";
    gassyMooComet.flyingSoundID = 4;
    gassyMooComet.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
    gassyMooComet.addTiles = 0;
    gassyMooComet.destroyOnExplode = false;
    gassyMooComet.craterPrefabs = new string[1]{ "Moo" };
    PrimaryElement primaryElement = entity.AddOrGet<PrimaryElement>();
    primaryElement.SetElement(SimHashes.Creature);
    primaryElement.Temperature = (float) (((double) gassyMooComet.temperatureRange.x + (double) gassyMooComet.temperatureRange.y) / 2.0);
    KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("meteor_gassymoo_kanim"))
    };
    kbatchedAnimController.isMovable = true;
    kbatchedAnimController.initialAnim = "fall_loop";
    kbatchedAnimController.initialMode = (KAnim.PlayMode) 0;
    kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
    entity.AddOrGet<KCircleCollider2D>().radius = 0.5f;
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
