// Decompiled with JetBrains decompiler
// Type: BaseHatchConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class BaseHatchConfig
{
  public static GameObject BaseHatch(
    string id,
    string name,
    string desc,
    string anim_file,
    string traitId,
    bool is_baby,
    string symbolOverridePrefix = null)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    EffectorValues decor = tieR0;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, 100f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
    string NavGridName = "WalkerNavGrid1x1";
    if (is_baby)
      NavGridName = "WalkerBabyNavGrid";
    EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Pest, traitId, NavGridName, onDeathDropCount: 2, entombVulnerable: false);
    if (symbolOverridePrefix != null)
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbolOverridePrefix);
    placedEntity.AddOrGet<Trappable>();
    placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
    placedEntity.AddOrGetDef<BurrowMonitor.Def>();
    placedEntity.AddOrGetDef<WorldSpawnableMonitor.Def>().adjustSpawnLocationCb = new Func<int, int>(BaseHatchConfig.AdjustSpawnLocationCB);
    placedEntity.AddOrGetDef<ThreatMonitor.Def>().fleethresholdState = Health.HealthState.Dead;
    placedEntity.AddWeapon(1f, 1f);
    SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_idle", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("FloorSoundEvent", "Hatch_footstep", NOISE_POLLUTION.CREATURES.TIER1);
    SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_land", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_chew", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_hurt", NOISE_POLLUTION.CREATURES.TIER5);
    SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_voice_die", NOISE_POLLUTION.CREATURES.TIER5);
    SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_emerge", NOISE_POLLUTION.CREATURES.TIER6);
    SoundEventVolumeCache.instance.AddVolume("hatch_kanim", "Hatch_drill_hide", NOISE_POLLUTION.CREATURES.TIER6);
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, true);
    KPrefabID component = placedEntity.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Creatures.Walker, false);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    component.prefabInitFn += BaseHatchConfig.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (BaseHatchConfig.\u003C\u003Ec.\u003C\u003E9__0_0 = new KPrefabID.PrefabFn((object) BaseHatchConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CBaseHatch\u003Eb__0_0)));
    bool condition = !is_baby;
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new ExitBurrowStates.Def(), condition).Add((StateMachine.BaseDef) new PlayAnimsStates.Def(GameTags.Creatures.Burrowed, true, "idle_mound", (string) STRINGS.CREATURES.STATUSITEMS.BURROWED.NAME, (string) STRINGS.CREATURES.STATUSITEMS.BURROWED.TOOLTIP), condition).Add((StateMachine.BaseDef) new GrowUpStates.Def(), is_baby).Add((StateMachine.BaseDef) new TrappedStates.Def()).Add((StateMachine.BaseDef) new IncubatingStates.Def(), is_baby).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DrowningStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new FleeStates.Def()).Add((StateMachine.BaseDef) new AttackStates.Def(), condition).PushInterruptGroup().Add((StateMachine.BaseDef) new CreatureSleepStates.Def()).Add((StateMachine.BaseDef) new FixedCaptureStates.Def()).Add((StateMachine.BaseDef) new RanchedStates.Def(), !is_baby).Add((StateMachine.BaseDef) new PlayAnimsStates.Def(GameTags.Creatures.WantsToEnterBurrow, false, "hide", (string) STRINGS.CREATURES.STATUSITEMS.BURROWING.NAME, (string) STRINGS.CREATURES.STATUSITEMS.BURROWING.TOOLTIP), condition).Add((StateMachine.BaseDef) new LayEggStates.Def(), !is_baby).Add((StateMachine.BaseDef) new EatStates.Def()).Add((StateMachine.BaseDef) new PlayAnimsStates.Def(GameTags.Creatures.Poop, false, "poop", (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME, (string) STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP)).Add((StateMachine.BaseDef) new CallAdultStates.Def(), is_baby).PopInterruptGroup().Add((StateMachine.BaseDef) new IdleStates.Def());
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.HatchSpecies, symbolOverridePrefix);
    return placedEntity;
  }

  public static List<Diet.Info> BasicRockDiet(
    Tag poopTag,
    float caloriesPerKg,
    float producedConversionRate,
    string diseaseId,
    float diseasePerKgProduced)
  {
    HashSet<Tag> consumed_tags = new HashSet<Tag>();
    consumed_tags.Add(SimHashes.Sand.CreateTag());
    consumed_tags.Add(SimHashes.SandStone.CreateTag());
    consumed_tags.Add(SimHashes.Clay.CreateTag());
    consumed_tags.Add(SimHashes.CrushedRock.CreateTag());
    consumed_tags.Add(SimHashes.Dirt.CreateTag());
    consumed_tags.Add(SimHashes.SedimentaryRock.CreateTag());
    return new List<Diet.Info>()
    {
      new Diet.Info(consumed_tags, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
    };
  }

  public static List<Diet.Info> HardRockDiet(
    Tag poopTag,
    float caloriesPerKg,
    float producedConversionRate,
    string diseaseId,
    float diseasePerKgProduced)
  {
    HashSet<Tag> consumed_tags = new HashSet<Tag>();
    consumed_tags.Add(SimHashes.SedimentaryRock.CreateTag());
    consumed_tags.Add(SimHashes.IgneousRock.CreateTag());
    consumed_tags.Add(SimHashes.Obsidian.CreateTag());
    consumed_tags.Add(SimHashes.Granite.CreateTag());
    return new List<Diet.Info>()
    {
      new Diet.Info(consumed_tags, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
    };
  }

  public static List<Diet.Info> MetalDiet(
    Tag poopTag,
    float caloriesPerKg,
    float producedConversionRate,
    string diseaseId,
    float diseasePerKgProduced)
  {
    List<Diet.Info> infoList = new List<Diet.Info>();
    infoList.Add(new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
    {
      SimHashes.Cuprite.CreateTag()
    }), Tag.op_Equality(poopTag, GameTags.Metal) ? SimHashes.Copper.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
    infoList.Add(new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
    {
      SimHashes.GoldAmalgam.CreateTag()
    }), Tag.op_Equality(poopTag, GameTags.Metal) ? SimHashes.Gold.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
    infoList.Add(new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
    {
      SimHashes.IronOre.CreateTag()
    }), Tag.op_Equality(poopTag, GameTags.Metal) ? SimHashes.Iron.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
    infoList.Add(new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
    {
      SimHashes.Wolframite.CreateTag()
    }), Tag.op_Equality(poopTag, GameTags.Metal) ? SimHashes.Tungsten.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
    infoList.Add(new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
    {
      SimHashes.AluminumOre.CreateTag()
    }), Tag.op_Equality(poopTag, GameTags.Metal) ? SimHashes.Aluminum.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
    if (ElementLoader.FindElementByHash(SimHashes.Cobaltite) != null)
      infoList.Add(new Diet.Info(new HashSet<Tag>((IEnumerable<Tag>) new Tag[1]
      {
        SimHashes.Cobaltite.CreateTag()
      }), Tag.op_Equality(poopTag, GameTags.Metal) ? SimHashes.Cobalt.CreateTag() : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced));
    return infoList;
  }

  public static List<Diet.Info> VeggieDiet(
    Tag poopTag,
    float caloriesPerKg,
    float producedConversionRate,
    string diseaseId,
    float diseasePerKgProduced)
  {
    HashSet<Tag> consumed_tags = new HashSet<Tag>();
    consumed_tags.Add(SimHashes.Dirt.CreateTag());
    consumed_tags.Add(SimHashes.SlimeMold.CreateTag());
    consumed_tags.Add(SimHashes.Algae.CreateTag());
    consumed_tags.Add(SimHashes.Fertilizer.CreateTag());
    consumed_tags.Add(SimHashes.ToxicSand.CreateTag());
    return new List<Diet.Info>()
    {
      new Diet.Info(consumed_tags, poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)
    };
  }

  public static List<Diet.Info> FoodDiet(
    Tag poopTag,
    float caloriesPerKg,
    float producedConversionRate,
    string diseaseId,
    float diseasePerKgProduced)
  {
    List<Diet.Info> infoList = new List<Diet.Info>();
    foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
    {
      if ((double) allFoodType.CaloriesPerUnit > 0.0)
      {
        HashSet<Tag> consumed_tags = new HashSet<Tag>();
        consumed_tags.Add(new Tag(allFoodType.Id));
        infoList.Add(new Diet.Info(consumed_tags, poopTag, allFoodType.CaloriesPerUnit, producedConversionRate, diseaseId, diseasePerKgProduced));
      }
    }
    return infoList;
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

  private static int AdjustSpawnLocationCB(int cell)
  {
    int num;
    for (; !Grid.Solid[cell]; cell = num)
    {
      num = Grid.CellBelow(cell);
      if (!Grid.IsValidCell(cell))
        break;
    }
    return cell;
  }
}
