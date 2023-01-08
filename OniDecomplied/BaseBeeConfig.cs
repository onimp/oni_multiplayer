// Decompiled with JetBrains decompiler
// Type: BaseBeeConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public static class BaseBeeConfig
{
  public static GameObject BaseBee(
    string id,
    string name,
    string desc,
    string anim_file,
    string traitId,
    EffectorValues decor,
    bool is_baby,
    string symbolOverridePrefix = null)
  {
    string id1 = id;
    string name1 = name;
    string desc1 = desc;
    EffectorValues effectorValues = decor;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(anim_file));
    EffectorValues decor1 = effectorValues;
    EffectorValues noise = new EffectorValues();
    double freezing3 = (double) CREATURES.TEMPERATURE.FREEZING_3;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, 5f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor1, noise, defaultTemperature: ((float) freezing3));
    string str = "FlyerNavGrid1x1";
    NavType navType = NavType.Hover;
    int num1 = 5;
    if (is_baby)
    {
      str = "WalkerBabyNavGrid";
      navType = NavType.Floor;
      num1 = 1;
    }
    GameObject template = placedEntity;
    string initialTraitID = traitId;
    string NavGridName = str;
    int num2 = (int) navType;
    double moveSpeed = (double) num1;
    float freezing10 = CREATURES.TEMPERATURE.FREEZING_10;
    double freezing9 = (double) CREATURES.TEMPERATURE.FREEZING_9;
    double freezing1 = (double) CREATURES.TEMPERATURE.FREEZING_1;
    double lethalLowTemperature = (double) freezing10;
    double freezing = (double) CREATURES.TEMPERATURE.FREEZING;
    EntityTemplates.ExtendEntityToBasicCreature(template, FactionManager.FactionID.Hostile, initialTraitID, NavGridName, (NavType) num2, moveSpeed: ((float) moveSpeed), onDeathDropCount: 0, warningLowTemperature: ((float) freezing9), warningHighTemperature: ((float) freezing1), lethalLowTemperature: ((float) lethalLowTemperature), lethalHighTemperature: ((float) freezing));
    if (symbolOverridePrefix != null)
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbolOverridePrefix);
    KPrefabID component = placedEntity.GetComponent<KPrefabID>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    component.prefabInitFn += BaseBeeConfig.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (BaseBeeConfig.\u003C\u003Ec.\u003C\u003E9__0_0 = new KPrefabID.PrefabFn((object) BaseBeeConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CBaseBee\u003Eb__0_0)));
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGetDef<ThreatMonitor.Def>();
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, false);
    placedEntity.AddOrGetDef<AgeMonitor.Def>();
    Bee bee = placedEntity.AddOrGet<Bee>();
    RadiationEmitter radiationEmitter = placedEntity.AddComponent<RadiationEmitter>();
    radiationEmitter.emitRate = 0.1f;
    if (!is_baby)
    {
      component.AddTag(GameTags.Creatures.Flyer, false);
      bee.radiationOutputAmount = 240f;
      radiationEmitter.radiusProportionalToRads = false;
      radiationEmitter.emitRadiusX = (short) 3;
      radiationEmitter.emitRadiusY = (short) 3;
      radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
      placedEntity.AddOrGetDef<SubmergedMonitor.Def>();
      placedEntity.AddWeapon(2f, 3f);
    }
    else
    {
      bee.radiationOutputAmount = 120f;
      radiationEmitter.radiusProportionalToRads = false;
      radiationEmitter.emitRadiusX = (short) 2;
      radiationEmitter.emitRadiusY = (short) 2;
      radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
      placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
      placedEntity.AddOrGetDef<BeeHiveMonitor.Def>();
      placedEntity.AddOrGet<Trappable>();
      EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, true);
    }
    placedEntity.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = CREATURES.SPACE_REQUIREMENTS.TIER1;
    placedEntity.AddOrGetDef<BeeHappinessMonitor.Def>();
    ElementConsumer elementConsumer = placedEntity.AddOrGet<ElementConsumer>();
    elementConsumer.elementToConsume = SimHashes.CarbonDioxide;
    elementConsumer.consumptionRate = 0.1f;
    elementConsumer.consumptionRadius = (byte) 3;
    elementConsumer.showInStatusPanel = true;
    elementConsumer.sampleCellOffset = new Vector3(0.0f, 0.0f, 0.0f);
    elementConsumer.isRequired = false;
    elementConsumer.storeOnConsume = false;
    elementConsumer.showDescriptor = true;
    elementConsumer.EnableConsumption(false);
    placedEntity.AddOrGetDef<BeeSleepMonitor.Def>();
    placedEntity.AddOrGetDef<BeeForagingMonitor.Def>();
    placedEntity.AddOrGet<Storage>();
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new GrowUpStates.Def()).Add((StateMachine.BaseDef) new TrappedStates.Def()).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new DrowningStates.Def()).Add((StateMachine.BaseDef) new BeeSleepStates.Def()).Add((StateMachine.BaseDef) new FleeStates.Def()).Add((StateMachine.BaseDef) new AttackStates.Def("attack_pre", "attack_pst", new CellOffset[3]
    {
      new CellOffset(0, 1),
      new CellOffset(1, 1),
      new CellOffset(-1, 1)
    }), (!is_baby ? 1 : 0) != 0).Add((StateMachine.BaseDef) new FixedCaptureStates.Def()).Add((StateMachine.BaseDef) new BeeMakeHiveStates.Def()).Add((StateMachine.BaseDef) new BeeForageStates.Def(SimHashes.UraniumOre.CreateTag(), BeeHiveTuning.ORE_DELIVERY_AMOUNT)).Add((StateMachine.BaseDef) new BuzzStates.Def());
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.BeetaSpecies, symbolOverridePrefix);
    return placedEntity;
  }

  public static void SetupLoopingSounds(GameObject inst) => inst.GetComponent<LoopingSounds>().StartSound(GlobalAssets.GetSound("Bee_wings_LP"));
}
