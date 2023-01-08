// Decompiled with JetBrains decompiler
// Type: BaseMooConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public static class BaseMooConfig
{
  public static GameObject BaseMoo(
    string id,
    string name,
    string desc,
    string traitId,
    string anim_file,
    bool is_baby,
    string symbol_override_prefix)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    EffectorValues decor = tieR0;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, 50f, anim, "idle_loop", Grid.SceneLayer.Creatures, 2, 2, decor, noise);
    EntityTemplates.ExtendEntityToBasicCreature(placedEntity, initialTraitID: traitId, NavGridName: "FlyerNavGrid2x2", navType: NavType.Hover, onDeathDropCount: 10, warningLowTemperature: 123.149994f, warningHighTemperature: 423.15f, lethalLowTemperature: 73.1499939f, lethalHighTemperature: 473.15f);
    if (!string.IsNullOrEmpty(symbol_override_prefix))
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbol_override_prefix);
    KPrefabID component = placedEntity.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Creatures.Flyer, false);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    component.prefabInitFn += BaseMooConfig.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (BaseMooConfig.\u003C\u003Ec.\u003C\u003E9__0_0 = new KPrefabID.PrefabFn((object) BaseMooConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CBaseMoo\u003Eb__0_0)));
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[1]
    {
      SimHashes.BleachStone.CreateTag()
    };
    placedEntity.AddOrGetDef<ThreatMonitor.Def>();
    placedEntity.AddOrGetDef<SubmergedMonitor.Def>();
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, false);
    placedEntity.AddOrGetDef<RanchableMonitor.Def>();
    placedEntity.AddOrGetDef<FixedCapturableMonitor.Def>();
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new DrowningStates.Def()).PushInterruptGroup().Add((StateMachine.BaseDef) new CreatureSleepStates.Def()).Add((StateMachine.BaseDef) new FixedCaptureStates.Def()).Add((StateMachine.BaseDef) new RanchedStates.Def()
    {
      WaitCellOffset = 2
    }).Add((StateMachine.BaseDef) new EatStates.Def()).Add((StateMachine.BaseDef) new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_GAS.NAME, (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_GAS.TOOLTIP)).Add((StateMachine.BaseDef) new MoveToLureStates.Def()).PopInterruptGroup().Add((StateMachine.BaseDef) new IdleStates.Def()
    {
      customIdleAnim = new IdleStates.Def.IdleAnimCallback(BaseMooConfig.CustomIdleAnim)
    });
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.MooSpecies, symbol_override_prefix);
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
    prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
    return prefab;
  }

  private static HashedString CustomIdleAnim(IdleStates.Instance smi, ref HashedString pre_anim)
  {
    CreatureCalorieMonitor.Instance smi1 = smi.GetSMI<CreatureCalorieMonitor.Instance>();
    return HashedString.op_Implicit(smi1 == null || !smi1.stomach.IsReadyToPoop() ? "idle_loop" : "idle_loop_full");
  }

  public static void OnSpawn(GameObject inst)
  {
    Navigator component = inst.GetComponent<Navigator>();
    component.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer) new FullPuftTransitionLayer(component));
  }
}
