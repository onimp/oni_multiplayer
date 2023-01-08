// Decompiled with JetBrains decompiler
// Type: RockCometConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class RockCometConfig : IEntityConfig
{
  public static readonly string ID = "RockComet";
  private const SimHashes element = SimHashes.Regolith;
  private const int ADDED_CELLS = 6;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(RockCometConfig.ID, (string) UI.SPACEDESTINATIONS.COMETS.ROCKCOMET.NAME);
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<LoopingSounds>();
    Comet comet = entity.AddOrGet<Comet>();
    float mass = ElementLoader.FindElementByHash(SimHashes.Regolith).defaultValues.mass;
    comet.massRange = new Vector2((float) ((double) mass * 0.800000011920929 * 6.0), (float) ((double) mass * 1.2000000476837158 * 6.0));
    comet.temperatureRange = new Vector2(323.15f, 423.15f);
    comet.addTiles = 6;
    comet.addTilesMinHeight = 2;
    comet.addTilesMaxHeight = 8;
    comet.entityDamage = 20;
    comet.totalTileDamage = 0.0f;
    comet.splashRadius = 1;
    comet.impactSound = "Meteor_Large_Impact";
    comet.flyingSoundID = 2;
    comet.explosionEffectHash = SpawnFXHashes.MeteorImpactDirt;
    PrimaryElement primaryElement = entity.AddOrGet<PrimaryElement>();
    primaryElement.SetElement(SimHashes.Regolith);
    primaryElement.Temperature = (float) (((double) comet.temperatureRange.x + (double) comet.temperatureRange.y) / 2.0);
    KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("meteor_rock_kanim"))
    };
    kbatchedAnimController.isMovable = true;
    kbatchedAnimController.initialAnim = "fall_loop";
    kbatchedAnimController.initialMode = (KAnim.PlayMode) 0;
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
