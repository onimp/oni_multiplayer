// Decompiled with JetBrains decompiler
// Type: BaseOilFloaterConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class BaseOilFloaterConfig
{
  public static GameObject BaseOilFloater(
    string id,
    string name,
    string desc,
    string anim_file,
    string traitId,
    float warnLowTemp,
    float warnHighTemp,
    bool is_baby,
    string symbolOverridePrefix = null)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    EffectorValues tieR1 = DECOR.BONUS.TIER1;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    EffectorValues decor = tieR1;
    EffectorValues noise = new EffectorValues();
    double defaultTemperature = ((double) warnLowTemp + (double) warnHighTemp) / 2.0;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, 50f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise, defaultTemperature: ((float) defaultTemperature));
    placedEntity.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Hoverer, false);
    EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Pest, traitId, "FloaterNavGrid", NavType.Hover, onDeathDropCount: 2, entombVulnerable: false, warningLowTemperature: warnLowTemp, warningHighTemperature: warnHighTemp, lethalLowTemperature: (warnLowTemp - 15f), lethalHighTemperature: (warnHighTemp + 20f));
    if (!string.IsNullOrEmpty(symbolOverridePrefix))
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbolOverridePrefix);
    placedEntity.AddOrGet<Trappable>();
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGetDef<ThreatMonitor.Def>();
    placedEntity.AddOrGetDef<SubmergedMonitor.Def>();
    placedEntity.AddOrGetDef<CreatureFallMonitor.Def>().canSwim = true;
    placedEntity.AddWeapon(1f, 1f);
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, false);
    string str1 = "OilFloater_intake_air";
    if (is_baby)
      str1 = "OilFloaterBaby_intake_air";
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new GrowUpStates.Def(), is_baby).Add((StateMachine.BaseDef) new TrappedStates.Def()).Add((StateMachine.BaseDef) new IncubatingStates.Def(), is_baby).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DrowningStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).PushInterruptGroup().Add((StateMachine.BaseDef) new CreatureSleepStates.Def()).Add((StateMachine.BaseDef) new FixedCaptureStates.Def()).Add((StateMachine.BaseDef) new RanchedStates.Def(), !is_baby).Add((StateMachine.BaseDef) new LayEggStates.Def(), !is_baby).Add((StateMachine.BaseDef) new InhaleStates.Def()
    {
      inhaleSound = str1
    }).Add((StateMachine.BaseDef) new SameSpotPoopStates.Def()).Add((StateMachine.BaseDef) new CallAdultStates.Def(), is_baby).PopInterruptGroup().Add((StateMachine.BaseDef) new IdleStates.Def());
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.OilFloaterSpecies, symbolOverridePrefix);
    string str2 = "OilFloater_move_LP";
    if (is_baby)
      str2 = "OilFloaterBaby_move_LP";
    placedEntity.AddOrGet<OilFloaterMovementSound>().sound = str2;
    return placedEntity;
  }

  public static GameObject SetupDiet(
    GameObject prefab,
    Tag consumed_tag,
    Tag producedTag,
    float caloriesPerKg,
    float producedConversionRate,
    string diseaseId,
    float diseasePerKgProduced,
    float minPoopSizeInKg)
  {
    HashSet<Tag> consumed_tags = new HashSet<Tag>();
    consumed_tags.Add(consumed_tag);
    Diet diet = new Diet(new Diet.Info[1]
    {
      new Diet.Info(consumed_tags, producedTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
    });
    CreatureCalorieMonitor.Def def = prefab.AddOrGetDef<CreatureCalorieMonitor.Def>();
    def.diet = diet;
    def.minPoopSizeInCalories = minPoopSizeInKg * caloriesPerKg;
    prefab.AddOrGetDef<GasAndLiquidConsumerMonitor.Def>().diet = diet;
    return prefab;
  }
}
