// Decompiled with JetBrains decompiler
// Type: StaterpillarLiquidConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class StaterpillarLiquidConfig : IEntityConfig
{
  public const string ID = "StaterpillarLiquid";
  public const string BASE_TRAIT_ID = "StaterpillarLiquidBaseTrait";
  public const string EGG_ID = "StaterpillarLiquidEgg";
  public const int EGG_SORT_ORDER = 2;
  private static float KG_ORE_EATEN_PER_CYCLE = 30f;
  private static float CALORIES_PER_KG_OF_ORE = StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE / StaterpillarLiquidConfig.KG_ORE_EATEN_PER_CYCLE;
  private static float STORAGE_CAPACITY = 1000f;
  private static float COOLDOWN_MIN = 20f;
  private static float COOLDOWN_MAX = 40f;
  private static float CONSUMPTION_RATE = 10f;
  private static float INHALE_TIME = 6f;
  private static float LETHAL_LOW_TEMPERATURE = 243.15f;
  private static float LETHAL_HIGH_TEMPERATURE = 363.15f;
  private static float WARNING_LOW_TEMPERATURE = StaterpillarLiquidConfig.LETHAL_LOW_TEMPERATURE + 20f;
  private static float WARNING_HIGH_TEMPERATURE = StaterpillarLiquidConfig.LETHAL_HIGH_TEMPERATURE - 20f;

  public static GameObject CreateStaterpillarLiquid(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    InhaleStates.Def inhaleDef = new InhaleStates.Def()
    {
      inhaleSound = "wtr_Staterpillar_intake",
      behaviourTag = GameTags.Creatures.WantsToStore,
      inhaleAnimPre = "liquid_consume_pre",
      inhaleAnimLoop = "liquid_consume_loop",
      inhaleAnimPst = "liquid_consume_pst",
      useStorage = true,
      alwaysPlayPstAnim = true,
      inhaleTime = StaterpillarLiquidConfig.INHALE_TIME,
      storageStatusItem = Db.Get().CreatureStatusItems.LookingForLiquid
    };
    GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseStaterpillarConfig.BaseStaterpillar(id, name, desc, anim_file, "StaterpillarLiquidBaseTrait", is_baby, ObjectLayer.LiquidConduit, StaterpillarLiquidConnectorConfig.ID, GameTags.Unbreathable, "wtr_", StaterpillarLiquidConfig.WARNING_LOW_TEMPERATURE, StaterpillarLiquidConfig.WARNING_HIGH_TEMPERATURE, StaterpillarLiquidConfig.LETHAL_LOW_TEMPERATURE, StaterpillarLiquidConfig.LETHAL_HIGH_TEMPERATURE, inhaleDef), TUNING.CREATURES.SPACE_REQUIREMENTS.TIER3);
    if (!is_baby)
    {
      GasAndLiquidConsumerMonitor.Def def = wildCreature.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>();
      def.behaviourTag = GameTags.Creatures.WantsToStore;
      def.consumableElementTag = GameTags.Liquid;
      def.transitionTag = new Tag[1]{ GameTags.Creature };
      def.minCooldown = StaterpillarLiquidConfig.COOLDOWN_MIN;
      def.maxCooldown = StaterpillarLiquidConfig.COOLDOWN_MAX;
      def.consumptionRate = StaterpillarLiquidConfig.CONSUMPTION_RATE;
    }
    Trait trait = Db.Get().CreateTrait("StaterpillarLiquidBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, StaterpillarTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
    List<Diet.Info> diet_infos = new List<Diet.Info>();
    diet_infos.AddRange((IEnumerable<Diet.Info>) BaseStaterpillarConfig.RawMetalDiet(SimHashes.Hydrogen.CreateTag(), StaterpillarLiquidConfig.CALORIES_PER_KG_OF_ORE, StaterpillarTuning.POOP_CONVERSTION_RATE, (string) null, 0.0f));
    diet_infos.AddRange((IEnumerable<Diet.Info>) BaseStaterpillarConfig.RefinedMetalDiet(SimHashes.Hydrogen.CreateTag(), StaterpillarLiquidConfig.CALORIES_PER_KG_OF_ORE, StaterpillarTuning.POOP_CONVERSTION_RATE, (string) null, 0.0f));
    GameObject staterpillarLiquid = BaseStaterpillarConfig.SetupDiet(wildCreature, diet_infos);
    staterpillarLiquid.AddComponent<Storage>().capacityKg = StaterpillarLiquidConfig.STORAGE_CAPACITY;
    return staterpillarLiquid;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public virtual GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(StaterpillarLiquidConfig.CreateStaterpillarLiquid("StaterpillarLiquid", (string) STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.NAME, (string) STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.DESC, "caterpillar_kanim", false), "StaterpillarLiquidEgg", (string) STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.EGG_NAME, (string) STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.DESC, "egg_caterpillar_kanim", StaterpillarTuning.EGG_MASS, "StaterpillarLiquidBaby", 60.0000038f, 20f, StaterpillarTuning.EGG_CHANCES_LIQUID, 2);

  public void OnPrefabInit(GameObject prefab)
  {
    KBatchedAnimController component = prefab.GetComponent<KBatchedAnimController>();
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("electric_bolt_c_bloom"), false);
    component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("gulp"), false);
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
