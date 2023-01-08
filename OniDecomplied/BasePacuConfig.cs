// Decompiled with JetBrains decompiler
// Type: BasePacuConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class BasePacuConfig
{
  private static float KG_ORE_EATEN_PER_CYCLE = 140f;
  private static float CALORIES_PER_KG_OF_ORE = PacuTuning.STANDARD_CALORIES_PER_CYCLE / BasePacuConfig.KG_ORE_EATEN_PER_CYCLE;
  private static float MIN_POOP_SIZE_IN_KG = 25f;

  public static GameObject CreatePrefab(
    string id,
    string base_trait_id,
    string name,
    string description,
    string anim_file,
    bool is_baby,
    string symbol_prefix,
    float warnLowTemp,
    float warnHighTemp)
  {
    string id1 = id;
    string name1 = name;
    string desc = description;
    EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    EffectorValues decor = tieR0;
    EffectorValues noise = new EffectorValues();
    double defaultTemperature = ((double) warnLowTemp + (double) warnHighTemp) / 2.0;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc, 200f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise, defaultTemperature: ((float) defaultTemperature));
    KPrefabID component1 = placedEntity.GetComponent<KPrefabID>();
    component1.AddTag(GameTags.SwimmingCreature, false);
    component1.AddTag(GameTags.Creatures.Swimmer, false);
    Trait trait = Db.Get().CreateTrait(base_trait_id, name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, PacuTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) PacuTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, name));
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, false, false, true);
    EntityTemplates.ExtendEntityToBasicCreature(placedEntity, initialTraitID: base_trait_id, NavGridName: "SwimmerNavGrid", navType: NavType.Swim, onDeathDropID: "FishMeat", drownVulnerable: false, entombVulnerable: false, warningLowTemperature: warnLowTemp, warningHighTemperature: warnHighTemp, lethalLowTemperature: (warnLowTemp - 20f), lethalHighTemperature: (warnHighTemp + 20f));
    if (is_baby)
    {
      KBatchedAnimController component2 = placedEntity.GetComponent<KBatchedAnimController>();
      component2.animWidth = 0.5f;
      component2.animHeight = 0.5f;
    }
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new GrowUpStates.Def(), is_baby).Add((StateMachine.BaseDef) new TrappedStates.Def()).Add((StateMachine.BaseDef) new IncubatingStates.Def(), is_baby).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()
    {
      getLandAnim = new Func<FallStates.Instance, string>(BasePacuConfig.GetLandAnim)
    }).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new FlopStates.Def()).PushInterruptGroup().Add((StateMachine.BaseDef) new FixedCaptureStates.Def()).Add((StateMachine.BaseDef) new LayEggStates.Def(), !is_baby).Add((StateMachine.BaseDef) new EatStates.Def()).Add((StateMachine.BaseDef) new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "lay_egg_pre", (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP)).Add((StateMachine.BaseDef) new MoveToLureStates.Def()).PopInterruptGroup().Add((StateMachine.BaseDef) new IdleStates.Def());
    CreatureFallMonitor.Def def1 = placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
    def1.canSwim = true;
    def1.checkHead = false;
    placedEntity.AddOrGetDef<FlopMonitor.Def>();
    placedEntity.AddOrGetDef<FishOvercrowdingMonitor.Def>();
    placedEntity.AddOrGet<Trappable>();
    placedEntity.AddOrGet<LoopingSounds>();
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.PacuSpecies, symbol_prefix);
    Tag tag = SimHashes.ToxicSand.CreateTag();
    HashSet<Tag> consumed_tags = new HashSet<Tag>();
    consumed_tags.Add(SimHashes.Algae.CreateTag());
    List<Diet.Info> infoList = new List<Diet.Info>();
    infoList.Add(new Diet.Info(consumed_tags, tag, BasePacuConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL));
    infoList.AddRange((IEnumerable<Diet.Info>) BasePacuConfig.SeedDiet(tag, (float) ((double) BasePacuConfig.CALORIES_PER_KG_OF_ORE * (double) BasePacuConfig.KG_ORE_EATEN_PER_CYCLE * 4.0), TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL));
    Diet diet = new Diet(infoList.ToArray());
    CreatureCalorieMonitor.Def def2 = placedEntity.AddOrGetDef<CreatureCalorieMonitor.Def>();
    def2.diet = diet;
    def2.minPoopSizeInCalories = BasePacuConfig.CALORIES_PER_KG_OF_ORE * BasePacuConfig.MIN_POOP_SIZE_IN_KG;
    placedEntity.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
    placedEntity.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[1]
    {
      GameTags.Creatures.FishTrapLure
    };
    if (!string.IsNullOrEmpty(symbol_prefix))
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbol_prefix);
    return placedEntity;
  }

  public static List<Diet.Info> SeedDiet(
    Tag poopTag,
    float caloriesPerSeed,
    float producedConversionRate)
  {
    List<Diet.Info> infoList = new List<Diet.Info>();
    foreach (GameObject gameObject in Assets.GetPrefabsWithTag(GameTags.Seed))
    {
      HashSet<Tag> consumed_tags = new HashSet<Tag>();
      consumed_tags.Add(new Tag(gameObject.GetComponent<KPrefabID>().PrefabID()));
      infoList.Add(new Diet.Info(consumed_tags, poopTag, caloriesPerSeed, producedConversionRate));
    }
    return infoList;
  }

  private static string GetLandAnim(FallStates.Instance smi) => smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation() ? "idle_loop" : "flop_loop";
}
