// Decompiled with JetBrains decompiler
// Type: DivergentWormConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

[EntityConfigOrder(1)]
public class DivergentWormConfig : IEntityConfig
{
  public const string ID = "DivergentWorm";
  public const string BASE_TRAIT_ID = "DivergentWormBaseTrait";
  public const string EGG_ID = "DivergentWormEgg";
  private const float LIFESPAN = 150f;
  public const float CROP_TENDED_MULTIPLIER_EFFECT = 0.5f;
  public const float CROP_TENDED_MULTIPLIER_DURATION = 600f;
  private const int NUM_SEGMENTS = 5;
  private const SimHashes EMIT_ELEMENT = SimHashes.Mud;
  private static float KG_ORE_EATEN_PER_CYCLE = 50f;
  private static float KG_SUCROSE_EATEN_PER_CYCLE = 30f;
  private static float CALORIES_PER_KG_OF_ORE = DivergentTuning.STANDARD_CALORIES_PER_CYCLE / DivergentWormConfig.KG_ORE_EATEN_PER_CYCLE;
  private static float CALORIES_PER_KG_OF_SUCROSE = DivergentTuning.STANDARD_CALORIES_PER_CYCLE / DivergentWormConfig.KG_SUCROSE_EATEN_PER_CYCLE;
  public static int EGG_SORT_ORDER = 0;
  private static float MINI_POOP_SIZE_IN_KG = 4f;

  public static GameObject CreateWorm(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseDivergentConfig.BaseDivergent(id, name, desc, 200f, anim_file, "DivergentWormBaseTrait", is_baby, cropTendingEffect: "DivergentCropTendedWorm", meatAmount: 3, is_pacifist: false), DivergentTuning.PEN_SIZE_PER_CREATURE_WORM);
    Trait trait = Db.Get().CreateTrait("DivergentWormBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, DivergentTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) DivergentTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name));
    wildCreature.AddWeapon(2f, 3f);
    List<Diet.Info> diet_infos = BaseDivergentConfig.BasicSulfurDiet(SimHashes.Mud.CreateTag(), DivergentWormConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.BAD_2, (string) null, 0.0f);
    List<Diet.Info> infoList = diet_infos;
    HashSet<Tag> consumed_tags = new HashSet<Tag>();
    consumed_tags.Add(SimHashes.Sucrose.CreateTag());
    Diet.Info info = new Diet.Info(consumed_tags, SimHashes.Mud.CreateTag(), DivergentWormConfig.CALORIES_PER_KG_OF_SUCROSE);
    infoList.Add(info);
    GameObject go = BaseDivergentConfig.SetupDiet(wildCreature, diet_infos, DivergentWormConfig.CALORIES_PER_KG_OF_ORE, DivergentWormConfig.MINI_POOP_SIZE_IN_KG);
    SegmentedCreature.Def def = go.AddOrGetDef<SegmentedCreature.Def>();
    def.segmentTrackerSymbol = new HashedString("segmenttracker");
    def.numBodySegments = 5;
    def.midAnim = Assets.GetAnim(HashedString.op_Implicit("worm_torso_kanim"));
    def.tailAnim = Assets.GetAnim(HashedString.op_Implicit("worm_tail_kanim"));
    def.animFrameOffset = 2;
    def.pathSpacing = 0.2f;
    def.numPathNodes = 15;
    def.minSegmentSpacing = 0.1f;
    def.maxSegmentSpacing = 0.4f;
    def.retractionSegmentSpeed = 1f;
    def.retractionPathSpeed = 2f;
    def.compressedMaxScale = 0.25f;
    def.headOffset = new Vector3(0.12f, 0.4f, 0.0f);
    return go;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(DivergentWormConfig.CreateWorm("DivergentWorm", (string) STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.NAME, (string) STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.DESC, "worm_head_kanim", false), "DivergentWormEgg", (string) STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.DESC, "egg_worm_kanim", DivergentTuning.EGG_MASS, "DivergentWormBaby", 90f, 30f, DivergentTuning.EGG_CHANCES_WORM, DivergentWormConfig.EGG_SORT_ORDER);

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
