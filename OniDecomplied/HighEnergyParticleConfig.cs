// Decompiled with JetBrains decompiler
// Type: HighEnergyParticleConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class HighEnergyParticleConfig : IEntityConfig
{
  public const int PARTICLE_SPEED = 8;
  public const float PARTICLE_COLLISION_SIZE = 0.2f;
  public const float PER_CELL_FALLOFF = 0.1f;
  public const float FALLOUT_RATIO = 0.5f;
  public const int MAX_PAYLOAD = 500;
  public const int EXPLOSION_FALLOUT_TEMPERATURE = 5000;
  public const float EXPLOSION_FALLOUT_MASS_PER_PARTICLE = 0.001f;
  public const float EXPLOSION_EMIT_DURRATION = 1f;
  public const short EXPLOSION_EMIT_RADIUS = 6;
  public const string ID = "HighEnergyParticle";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject basicEntity = EntityTemplates.CreateBasicEntity("HighEnergyParticle", (string) ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, (string) ITEMS.RADIATION.HIGHENERGYPARITCLE.DESC, 1f, false, Assets.GetAnim(HashedString.op_Implicit("spark_radial_high_energy_particles_kanim")), "travel_pre", Grid.SceneLayer.FXFront2);
    EntityTemplates.AddCollision(basicEntity, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f);
    basicEntity.AddOrGet<LoopingSounds>();
    RadiationEmitter radiationEmitter = basicEntity.AddOrGet<RadiationEmitter>();
    radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
    radiationEmitter.radiusProportionalToRads = false;
    radiationEmitter.emitRadiusX = (short) 3;
    radiationEmitter.emitRadiusY = (short) 3;
    radiationEmitter.emitRads = (float) (0.40000000596046448 * ((double) radiationEmitter.emitRadiusX / 6.0));
    basicEntity.AddComponent<HighEnergyParticle>().speed = 8f;
    return basicEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
