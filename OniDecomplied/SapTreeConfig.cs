// Decompiled with JetBrains decompiler
// Type: SapTreeConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SapTreeConfig : IEntityConfig
{
  public const string ID = "SapTree";
  public static readonly EffectorValues POSITIVE_DECOR_EFFECT = TUNING.DECOR.BONUS.TIER5;
  private const int WIDTH = 5;
  private const int HEIGHT = 5;
  private const int ATTACK_RADIUS = 2;
  public const float MASS_EAT_RATE = 0.05f;
  public const float KCAL_TO_KG_RATIO = 0.005f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name = (string) STRINGS.CREATURES.SPECIES.SAPTREE.NAME;
    string desc = (string) STRINGS.CREATURES.SPECIES.SAPTREE.DESC;
    EffectorValues positiveDecorEffect = SapTreeConfig.POSITIVE_DECOR_EFFECT;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("gravitas_sap_tree_kanim"));
    EffectorValues decor = positiveDecorEffect;
    EffectorValues noise = new EffectorValues();
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Decoration);
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("SapTree", name, desc, 1f, anim, "idle", Grid.SceneLayer.BuildingFront, 5, 5, decor, noise, additionalTags: additionalTags);
    SapTree.Def def = placedEntity.AddOrGetDef<SapTree.Def>();
    def.foodSenseArea = new Vector2I(5, 1);
    def.massEatRate = 0.05f;
    def.kcalorieToKGConversionRatio = 0.005f;
    def.stomachSize = 5f;
    def.oozeRate = 2f;
    List<Vector3> vector3List = new List<Vector3>();
    vector3List.Add(new Vector3(-2f, 2f));
    vector3List.Add(new Vector3(2f, 1f));
    def.oozeOffsets = vector3List;
    def.attackSenseArea = new Vector2I(5, 5);
    def.attackCooldown = 5f;
    placedEntity.AddOrGet<Storage>();
    FactionAlignment factionAlignment = placedEntity.AddOrGet<FactionAlignment>();
    factionAlignment.Alignment = FactionManager.FactionID.Hostile;
    factionAlignment.canBePlayerTargeted = false;
    placedEntity.AddOrGet<RangedAttackable>();
    placedEntity.AddWeapon(5f, 5f, targetType: AttackProperties.TargetType.AreaOfEffect, aoeRadius: 2f);
    placedEntity.AddOrGet<WiltCondition>();
    placedEntity.AddOrGet<TemperatureVulnerable>().Configure(173.15f, 0.0f, 373.15f, 1023.15f);
    placedEntity.AddOrGet<EntombVulnerable>();
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
    {
      ObjectLayer.Building
    };
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
