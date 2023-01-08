// Decompiled with JetBrains decompiler
// Type: BaseDivergentConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public static class BaseDivergentConfig
{
  public const float CROP_TENDED_MULTIPLIER_DURATION = 600f;
  public const float CROP_TENDED_MULTIPLIER_EFFECT = 0.05f;
  public static string[] ignoreEffectGroup = new string[2]
  {
    "DivergentCropTended",
    "DivergentCropTendedWorm"
  };

  public static GameObject BaseDivergent(
    string id,
    string name,
    string desc,
    float mass,
    string anim_file,
    string traitId,
    bool is_baby,
    float num_tended_per_cycle = 8f,
    string symbolOverridePrefix = null,
    string cropTendingEffect = "DivergentCropTended",
    int meatAmount = 1,
    bool is_pacifist = true)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    double mass1 = (double) mass;
    EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    EffectorValues decor = tieR0;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, (float) mass1, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
    string NavGridName = "WalkerNavGrid1x1";
    if (is_baby)
      NavGridName = "WalkerBabyNavGrid";
    EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Pest, traitId, NavGridName, onDeathDropCount: meatAmount, entombVulnerable: false);
    if (symbolOverridePrefix != null)
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbolOverridePrefix);
    placedEntity.AddOrGet<Trappable>();
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
    placedEntity.AddOrGetDef<BurrowMonitor.Def>();
    placedEntity.AddOrGetDef<CropTendingMonitor.Def>().numCropsTendedPerCycle = num_tended_per_cycle;
    placedEntity.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, true);
    KPrefabID component = placedEntity.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Creatures.Walker, false);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    component.prefabInitFn += BaseDivergentConfig.\u003C\u003Ec.\u003C\u003E9__3_0 ?? (BaseDivergentConfig.\u003C\u003Ec.\u003C\u003E9__3_0 = new KPrefabID.PrefabFn((object) BaseDivergentConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CBaseDivergent\u003Eb__3_0)));
    CropTendingStates.Def def = new CropTendingStates.Def();
    def.effectId = cropTendingEffect;
    def.interests.Add(Tag.op_Implicit("WormPlant"), 10);
    def.animSetOverrides.Add(Tag.op_Implicit("WormPlant"), new CropTendingStates.AnimSet()
    {
      crop_tending_pre = "wormwood_tending_pre",
      crop_tending = "wormwood_tending",
      crop_tending_pst = "wormwood_tending_pst",
      hide_symbols_after_pre = new string[2]
      {
        "flower",
        "flower_wilted"
      }
    });
    def.ignoreEffectGroup = BaseDivergentConfig.ignoreEffectGroup;
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new GrowUpStates.Def(), is_baby).Add((StateMachine.BaseDef) new TrappedStates.Def()).Add((StateMachine.BaseDef) new IncubatingStates.Def(), is_baby).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DrowningStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new FleeStates.Def()).Add((StateMachine.BaseDef) new AttackStates.Def(), !is_baby && !is_pacifist).PushInterruptGroup().Add((StateMachine.BaseDef) new CreatureSleepStates.Def()).Add((StateMachine.BaseDef) new FixedCaptureStates.Def()).Add((StateMachine.BaseDef) new RanchedStates.Def(), !is_baby).Add((StateMachine.BaseDef) new LayEggStates.Def(), !is_baby).Add((StateMachine.BaseDef) new EatStates.Def()).Add((StateMachine.BaseDef) new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP)).Add((StateMachine.BaseDef) new CallAdultStates.Def(), is_baby).Add((StateMachine.BaseDef) def, !is_baby).PopInterruptGroup().Add((StateMachine.BaseDef) new IdleStates.Def());
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.DivergentSpecies, symbolOverridePrefix);
    return placedEntity;
  }

  public static List<Diet.Info> BasicSulfurDiet(
    Tag poopTag,
    float caloriesPerKg,
    float producedConversionRate,
    string diseaseId,
    float diseasePerKgProduced)
  {
    HashSet<Tag> consumed_tags = new HashSet<Tag>();
    consumed_tags.Add(SimHashes.Sulfur.CreateTag());
    return new List<Diet.Info>()
    {
      new Diet.Info(consumed_tags, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
    };
  }

  public static GameObject SetupDiet(
    GameObject prefab,
    List<Diet.Info> diet_infos,
    float referenceCaloriesPerKg,
    float minPoopSizeInKg)
  {
    Diet diet = new Diet(diet_infos.ToArray());
    CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
    def.diet = diet;
    def.minPoopSizeInCalories = referenceCaloriesPerKg * minPoopSizeInKg;
    prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
    return prefab;
  }
}
