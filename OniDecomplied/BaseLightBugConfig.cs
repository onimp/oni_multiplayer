// Decompiled with JetBrains decompiler
// Type: BaseLightBugConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public static class BaseLightBugConfig
{
  public static GameObject BaseLightBug(
    string id,
    string name,
    string desc,
    string anim_file,
    string traitId,
    Color lightColor,
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
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id1, name1, desc1, 5f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor1, noise);
    GameObject template = placedEntity;
    string initialTraitID = traitId;
    float freezing2 = CREATURES.TEMPERATURE.FREEZING_2;
    double freezing1 = (double) CREATURES.TEMPERATURE.FREEZING_1;
    double hot1 = (double) CREATURES.TEMPERATURE.HOT_1;
    double lethalLowTemperature = (double) freezing2;
    double hot2 = (double) CREATURES.TEMPERATURE.HOT_2;
    EntityTemplates.ExtendEntityToBasicCreature(template, initialTraitID: initialTraitID, NavGridName: "FlyerNavGrid1x1", navType: NavType.Hover, onDeathDropCount: 0, warningLowTemperature: ((float) freezing1), warningHighTemperature: ((float) hot1), lethalLowTemperature: ((float) lethalLowTemperature), lethalHighTemperature: ((float) hot2));
    if (symbolOverridePrefix != null)
      placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(anim_file)), symbolOverridePrefix);
    KPrefabID component1 = placedEntity.GetComponent<KPrefabID>();
    component1.AddTag(GameTags.Creatures.Flyer, false);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    component1.prefabInitFn += BaseLightBugConfig.\u003C\u003Ec.\u003C\u003E9__0_0 ?? (BaseLightBugConfig.\u003C\u003Ec.\u003C\u003E9__0_0 = new KPrefabID.PrefabFn((object) BaseLightBugConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CBaseLightBug\u003Eb__0_0)));
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGetDef<LureableMonitor.Def>().lures = new Tag[1]
    {
      GameTags.Phosphorite
    };
    placedEntity.AddOrGetDef<ThreatMonitor.Def>();
    placedEntity.AddOrGetDef<SubmergedMonitor.Def>();
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, false);
    if (DlcManager.FeatureRadiationEnabled())
    {
      RadiationEmitter radiationEmitter = placedEntity.AddOrGet<RadiationEmitter>();
      radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
      radiationEmitter.radiusProportionalToRads = false;
      radiationEmitter.emitRadiusX = (short) 6;
      radiationEmitter.emitRadiusY = radiationEmitter.emitRadiusX;
      radiationEmitter.emitRads = 60f;
      radiationEmitter.emissionOffset = new Vector3(0.0f, 0.0f, 0.0f);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      component1.prefabSpawnFn += BaseLightBugConfig.\u003C\u003Ec.\u003C\u003E9__0_1 ?? (BaseLightBugConfig.\u003C\u003Ec.\u003C\u003E9__0_1 = new KPrefabID.PrefabFn((object) BaseLightBugConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CBaseLightBug\u003Eb__0_1)));
    }
    if (is_baby)
    {
      KBatchedAnimController component2 = placedEntity.GetComponent<KBatchedAnimController>();
      component2.animWidth = 0.5f;
      component2.animHeight = 0.5f;
    }
    if (Color.op_Inequality(lightColor, Color.black))
    {
      Light2D light2D = placedEntity.AddOrGet<Light2D>();
      light2D.Color = lightColor;
      light2D.overlayColour = LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
      light2D.Range = 5f;
      light2D.Angle = 0.0f;
      light2D.Direction = LIGHT2D.LIGHTBUG_DIRECTION;
      light2D.Offset = LIGHT2D.LIGHTBUG_OFFSET;
      light2D.shape = LightShape.Circle;
      light2D.drawOverlay = true;
      light2D.Lux = 1800;
      placedEntity.AddOrGet<LightSymbolTracker>().targetSymbol = HashedString.op_Implicit("snapTo_light_locator");
      placedEntity.AddOrGetDef<CreatureLightToggleController.Def>();
    }
    ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new AnimInterruptStates.Def()).Add((StateMachine.BaseDef) new GrowUpStates.Def(), is_baby).Add((StateMachine.BaseDef) new IncubatingStates.Def(), is_baby).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new DrowningStates.Def()).PushInterruptGroup().Add((StateMachine.BaseDef) new CreatureSleepStates.Def()).Add((StateMachine.BaseDef) new FixedCaptureStates.Def()).Add((StateMachine.BaseDef) new RanchedStates.Def(), !is_baby).Add((StateMachine.BaseDef) new LayEggStates.Def(), !is_baby).Add((StateMachine.BaseDef) new EatStates.Def()).Add((StateMachine.BaseDef) new MoveToLureStates.Def()).Add((StateMachine.BaseDef) new CallAdultStates.Def(), is_baby).PopInterruptGroup().Add((StateMachine.BaseDef) new IdleStates.Def());
    EntityTemplates.AddCreatureBrain(placedEntity, chore_table, GameTags.Creatures.Species.LightBugSpecies, symbolOverridePrefix);
    return placedEntity;
  }

  public static GameObject SetupDiet(
    GameObject prefab,
    HashSet<Tag> consumed_tags,
    Tag producedTag,
    float caloriesPerKg)
  {
    Diet diet = new Diet(new Diet.Info[1]
    {
      new Diet.Info(consumed_tags, producedTag, caloriesPerKg)
    });
    prefab.AddOrGetDef<CreatureCalorieMonitor.Def>().diet = diet;
    prefab.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
    return prefab;
  }

  public static void SetupLoopingSounds(GameObject inst) => inst.GetComponent<LoopingSounds>().StartSound(GlobalAssets.GetSound("ShineBug_wings_LP"));
}
