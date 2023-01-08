// Decompiled with JetBrains decompiler
// Type: EntityTemplates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class EntityTemplates
{
  private static GameObject selectableEntityTemplate;
  private static GameObject unselectableEntityTemplate;
  private static GameObject baseEntityTemplate;
  private static GameObject placedEntityTemplate;
  private static GameObject baseOreTemplate;

  public static void CreateTemplates()
  {
    EntityTemplates.unselectableEntityTemplate = new GameObject("unselectableEntityTemplate");
    EntityTemplates.unselectableEntityTemplate.SetActive(false);
    EntityTemplates.unselectableEntityTemplate.AddComponent<KPrefabID>();
    Object.DontDestroyOnLoad((Object) EntityTemplates.unselectableEntityTemplate);
    EntityTemplates.selectableEntityTemplate = Object.Instantiate<GameObject>(EntityTemplates.unselectableEntityTemplate);
    ((Object) EntityTemplates.selectableEntityTemplate).name = "selectableEntityTemplate";
    EntityTemplates.selectableEntityTemplate.AddComponent<KSelectable>();
    Object.DontDestroyOnLoad((Object) EntityTemplates.selectableEntityTemplate);
    EntityTemplates.baseEntityTemplate = Object.Instantiate<GameObject>(EntityTemplates.selectableEntityTemplate);
    ((Object) EntityTemplates.baseEntityTemplate).name = "baseEntityTemplate";
    EntityTemplates.baseEntityTemplate.AddComponent<KBatchedAnimController>();
    EntityTemplates.baseEntityTemplate.AddComponent<SaveLoadRoot>();
    EntityTemplates.baseEntityTemplate.AddComponent<StateMachineController>();
    EntityTemplates.baseEntityTemplate.AddComponent<PrimaryElement>();
    EntityTemplates.baseEntityTemplate.AddComponent<SimTemperatureTransfer>();
    EntityTemplates.baseEntityTemplate.AddComponent<InfoDescription>();
    EntityTemplates.baseEntityTemplate.AddComponent<Notifier>();
    Object.DontDestroyOnLoad((Object) EntityTemplates.baseEntityTemplate);
    EntityTemplates.placedEntityTemplate = Object.Instantiate<GameObject>(EntityTemplates.baseEntityTemplate);
    ((Object) EntityTemplates.placedEntityTemplate).name = "placedEntityTemplate";
    EntityTemplates.placedEntityTemplate.AddComponent<KBoxCollider2D>();
    EntityTemplates.placedEntityTemplate.AddComponent<OccupyArea>();
    EntityTemplates.placedEntityTemplate.AddComponent<Modifiers>();
    EntityTemplates.placedEntityTemplate.AddComponent<DecorProvider>();
    Object.DontDestroyOnLoad((Object) EntityTemplates.placedEntityTemplate);
  }

  private static void ConfigEntity(
    GameObject template,
    string id,
    string name,
    bool is_selectable = true)
  {
    ((Object) template).name = id;
    template.AddOrGet<KPrefabID>().PrefabTag = TagManager.Create(id, name);
    if (!is_selectable)
      return;
    template.AddOrGet<KSelectable>().SetName(name);
  }

  public static GameObject CreateEntity(string id, string name, bool is_selectable = true)
  {
    GameObject template = !is_selectable ? Object.Instantiate<GameObject>(EntityTemplates.unselectableEntityTemplate) : Object.Instantiate<GameObject>(EntityTemplates.selectableEntityTemplate);
    Object.DontDestroyOnLoad((Object) template);
    EntityTemplates.ConfigEntity(template, id, name, is_selectable);
    return template;
  }

  public static GameObject ConfigBasicEntity(
    GameObject template,
    string id,
    string name,
    string desc,
    float mass,
    bool unitMass,
    KAnimFile anim,
    string initialAnim,
    Grid.SceneLayer sceneLayer,
    SimHashes element = SimHashes.Creature,
    List<Tag> additionalTags = null,
    float defaultTemperature = 293f)
  {
    EntityTemplates.ConfigEntity(template, id, name);
    KPrefabID kprefabId = template.AddOrGet<KPrefabID>();
    if (additionalTags != null)
    {
      foreach (Tag additionalTag in additionalTags)
        kprefabId.AddTag(additionalTag, false);
    }
    KBatchedAnimController kbatchedAnimController = template.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      anim
    };
    kbatchedAnimController.sceneLayer = sceneLayer;
    kbatchedAnimController.initialAnim = initialAnim;
    template.AddOrGet<StateMachineController>();
    PrimaryElement primaryElement = template.AddOrGet<PrimaryElement>();
    primaryElement.ElementID = element;
    primaryElement.Temperature = defaultTemperature;
    if (unitMass)
    {
      primaryElement.MassPerUnit = mass;
      primaryElement.Units = 1f;
      GameTags.DisplayAsUnits.Add(kprefabId.PrefabTag);
    }
    else
      primaryElement.Mass = mass;
    template.AddOrGet<InfoDescription>().description = desc;
    template.AddOrGet<Notifier>();
    return template;
  }

  public static GameObject CreateBasicEntity(
    string id,
    string name,
    string desc,
    float mass,
    bool unitMass,
    KAnimFile anim,
    string initialAnim,
    Grid.SceneLayer sceneLayer,
    SimHashes element = SimHashes.Creature,
    List<Tag> additionalTags = null,
    float defaultTemperature = 293f)
  {
    GameObject template = Object.Instantiate<GameObject>(EntityTemplates.baseEntityTemplate);
    Object.DontDestroyOnLoad((Object) template);
    EntityTemplates.ConfigBasicEntity(template, id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, element, additionalTags, defaultTemperature);
    return template;
  }

  private static GameObject ConfigPlacedEntity(
    GameObject template,
    string id,
    string name,
    string desc,
    float mass,
    KAnimFile anim,
    string initialAnim,
    Grid.SceneLayer sceneLayer,
    int width,
    int height,
    EffectorValues decor,
    EffectorValues noise = default (EffectorValues),
    SimHashes element = SimHashes.Creature,
    List<Tag> additionalTags = null,
    float defaultTemperature = 293f)
  {
    if (Object.op_Equality((Object) anim, (Object) null))
      Debug.LogErrorFormat("Cant create [{0}] entity without an anim", new object[1]
      {
        (object) name
      });
    EntityTemplates.ConfigBasicEntity(template, id, name, desc, mass, true, anim, initialAnim, sceneLayer, element, additionalTags, defaultTemperature);
    KBoxCollider2D kboxCollider2D = template.AddOrGet<KBoxCollider2D>();
    kboxCollider2D.size = Vector2f.op_Implicit(new Vector2f(width, height));
    float num = 0.5f * (float) ((width + 1) % 2);
    kboxCollider2D.offset = Vector2f.op_Implicit(new Vector2f(num, (float) height / 2f));
    template.GetComponent<KBatchedAnimController>().Offset = new Vector3(num, 0.0f, 0.0f);
    template.AddOrGet<OccupyArea>().OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(width, height);
    DecorProvider decorProvider = template.AddOrGet<DecorProvider>();
    decorProvider.SetValues(decor);
    decorProvider.overrideName = name;
    return template;
  }

  public static GameObject CreatePlacedEntity(
    string id,
    string name,
    string desc,
    float mass,
    KAnimFile anim,
    string initialAnim,
    Grid.SceneLayer sceneLayer,
    int width,
    int height,
    EffectorValues decor,
    EffectorValues noise = default (EffectorValues),
    SimHashes element = SimHashes.Creature,
    List<Tag> additionalTags = null,
    float defaultTemperature = 293f)
  {
    GameObject template = Object.Instantiate<GameObject>(EntityTemplates.placedEntityTemplate);
    Object.DontDestroyOnLoad((Object) template);
    EntityTemplates.ConfigPlacedEntity(template, id, name, desc, mass, anim, initialAnim, sceneLayer, width, height, decor, noise, element, additionalTags, defaultTemperature);
    return template;
  }

  public static GameObject MakeHangingOffsets(GameObject template, int width, int height)
  {
    KBoxCollider2D component1 = template.GetComponent<KBoxCollider2D>();
    if (Object.op_Implicit((Object) component1))
    {
      component1.size = Vector2f.op_Implicit(new Vector2f(width, height));
      float num = 0.5f * (float) ((width + 1) % 2);
      component1.offset = Vector2f.op_Implicit(new Vector2f(num, (float) ((double) -height / 2.0 + 1.0)));
    }
    OccupyArea component2 = template.GetComponent<OccupyArea>();
    if (Object.op_Implicit((Object) component2))
      component2.OccupiedCellsOffsets = EntityTemplates.GenerateHangingOffsets(width, height);
    return template;
  }

  public static GameObject ExtendEntityToBasicPlant(
    GameObject template,
    float temperature_lethal_low = 218.15f,
    float temperature_warning_low = 283.15f,
    float temperature_warning_high = 303.15f,
    float temperature_lethal_high = 398.15f,
    SimHashes[] safe_elements = null,
    bool pressure_sensitive = true,
    float pressure_lethal_low = 0.0f,
    float pressure_warning_low = 0.15f,
    string crop_id = null,
    bool can_drown = true,
    bool can_tinker = true,
    bool require_solid_tile = true,
    bool should_grow_old = true,
    float max_age = 2400f,
    float min_radiation = 0.0f,
    float max_radiation = 2200f,
    string baseTraitId = null,
    string baseTraitName = null)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    EntityTemplates.\u003C\u003Ec__DisplayClass12_0 cDisplayClass120 = new EntityTemplates.\u003C\u003Ec__DisplayClass12_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass120.crop_id = crop_id;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass120.safe_elements = safe_elements;
    Modifiers component1 = template.GetComponent<Modifiers>();
    Trait trait = Db.Get().CreateTrait(baseTraitId, baseTraitName, baseTraitName, (string) null, false, (ChoreGroup[]) null, true, true);
    template.AddTag(GameTags.Plant);
    template.AddOrGet<EntombVulnerable>();
    PressureVulnerable pressureVulnerable = template.AddOrGet<PressureVulnerable>();
    if (pressure_sensitive)
    {
      // ISSUE: reference to a compiler-generated field
      pressureVulnerable.Configure(pressure_warning_low, pressure_lethal_low, safeAtmospheres: cDisplayClass120.safe_elements);
    }
    else
    {
      // ISSUE: reference to a compiler-generated field
      pressureVulnerable.Configure(cDisplayClass120.safe_elements);
    }
    template.AddOrGet<WiltCondition>();
    template.AddOrGet<Prioritizable>();
    template.AddOrGet<Uprootable>();
    template.AddOrGet<Effects>();
    if (require_solid_tile)
      template.AddOrGet<UprootedMonitor>();
    template.AddOrGet<ReceptacleMonitor>();
    template.AddOrGet<Notifier>();
    if (can_drown)
      template.AddOrGet<DrowningMonitor>();
    template.AddOrGet<KAnimControllerBase>().randomiseLoopedOffset = true;
    component1.initialAttributes.Add(Db.Get().PlantAttributes.WiltTempRangeMod.Id);
    template.AddOrGet<TemperatureVulnerable>().Configure(temperature_warning_low, temperature_lethal_low, temperature_warning_high, temperature_lethal_high);
    if (DlcManager.FeaturePlantMutationsEnabled())
    {
      component1.initialAttributes.Add(Db.Get().PlantAttributes.MinRadiationThreshold.Id);
      if ((double) min_radiation != 0.0)
        trait.Add(new AttributeModifier(Db.Get().PlantAttributes.MinRadiationThreshold.Id, min_radiation, baseTraitName));
      component1.initialAttributes.Add(Db.Get().PlantAttributes.MaxRadiationThreshold.Id);
      trait.Add(new AttributeModifier(Db.Get().PlantAttributes.MaxRadiationThreshold.Id, max_radiation, baseTraitName));
      template.AddOrGetDef<RadiationVulnerable.Def>();
    }
    template.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
    {
      ObjectLayer.Building
    };
    KPrefabID component2 = template.GetComponent<KPrefabID>();
    // ISSUE: reference to a compiler-generated field
    if (cDisplayClass120.crop_id != null)
    {
      GeneratedBuildings.RegisterWithOverlay(OverlayScreen.HarvestableIDs, component2.PrefabID().ToString());
      // ISSUE: reference to a compiler-generated method
      Crop.CropVal cropval = TUNING.CROPS.CROP_TYPES.Find(new Predicate<Crop.CropVal>(cDisplayClass120.\u003CExtendEntityToBasicPlant\u003Eb__1));
      Debug.Assert(baseTraitId != null && baseTraitName != null, (object) ("Extending " + ((Object) template).name + " to a crop plant failed because the base trait wasn't specified."));
      component1.initialAttributes.Add(Db.Get().PlantAttributes.YieldAmount.Id);
      component1.initialAmounts.Add(Db.Get().Amounts.Maturity.Id);
      trait.Add(new AttributeModifier(Db.Get().PlantAttributes.YieldAmount.Id, (float) cropval.numProduced, baseTraitName));
      trait.Add(new AttributeModifier(Db.Get().Amounts.Maturity.maxAttribute.Id, cropval.cropDuration / 600f, baseTraitName));
      if (DlcManager.FeaturePlantMutationsEnabled())
      {
        template.AddOrGet<MutantPlant>().SpeciesID = component2.PrefabTag;
        SymbolOverrideControllerUtil.AddToPrefab(template);
      }
      template.AddOrGet<Crop>().Configure(cropval);
      Growing growing = template.AddOrGet<Growing>();
      growing.shouldGrowOld = should_grow_old;
      growing.maxAge = max_age;
      template.AddOrGet<Harvestable>();
      template.AddOrGet<HarvestDesignatable>();
    }
    if (trait.SelfModifiers != null && trait.SelfModifiers.Count > 0)
    {
      template.AddOrGet<Traits>();
      component1.initialTraits.Add(baseTraitId);
    }
    // ISSUE: method pointer
    component2.prefabInitFn += new KPrefabID.PrefabFn((object) cDisplayClass120, __methodptr(\u003CExtendEntityToBasicPlant\u003Eb__0));
    if (can_tinker)
      Tinkerable.MakeFarmTinkerable(template);
    return template;
  }

  public static GameObject ExtendEntityToWildCreature(
    GameObject prefab,
    int space_required_per_creature)
  {
    prefab.AddOrGetDef<AgeMonitor.Def>();
    prefab.AddOrGetDef<HappinessMonitor.Def>();
    Tag prefabTag = prefab.GetComponent<KPrefabID>().PrefabTag;
    WildnessMonitor.Def def = prefab.AddOrGetDef<WildnessMonitor.Def>();
    def.wildEffect = new Effect("Wild" + ((Tag) ref prefabTag).Name, (string) STRINGS.CREATURES.MODIFIERS.WILD.NAME, (string) STRINGS.CREATURES.MODIFIERS.WILD.TOOLTIP, 0.0f, true, true, false);
    def.wildEffect.Add(new AttributeModifier(Db.Get().Amounts.Wildness.deltaAttribute.Id, 0.008333334f, (string) STRINGS.CREATURES.MODIFIERS.WILD.NAME));
    def.wildEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, 25f, (string) STRINGS.CREATURES.MODIFIERS.WILD.NAME));
    def.wildEffect.Add(new AttributeModifier(Db.Get().Amounts.ScaleGrowth.deltaAttribute.Id, -0.75f, (string) STRINGS.CREATURES.MODIFIERS.WILD.NAME, true));
    def.tameEffect = new Effect("Tame" + ((Tag) ref prefabTag).Name, (string) STRINGS.CREATURES.MODIFIERS.TAME.NAME, (string) STRINGS.CREATURES.MODIFIERS.TAME.TOOLTIP, 0.0f, true, true, false);
    def.tameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -1f, (string) STRINGS.CREATURES.MODIFIERS.TAME.NAME));
    def.tameEffect.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, 100f, (string) STRINGS.CREATURES.MODIFIERS.TAME.NAME));
    prefab.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = space_required_per_creature;
    return prefab;
  }

  public static GameObject ExtendEntityToFertileCreature(
    GameObject prefab,
    string eggId,
    string eggName,
    string eggDesc,
    string egg_anim,
    float egg_mass,
    string baby_id,
    float fertility_cycles,
    float incubation_cycles,
    List<FertilityMonitor.BreedingChance> egg_chances,
    int eggSortOrder = -1,
    bool is_ranchable = true,
    bool add_fish_overcrowding_monitor = false,
    bool add_fixed_capturable_monitor = true,
    float egg_anim_scale = 1f,
    bool deprecated = false)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    EntityTemplates.\u003C\u003Ec__DisplayClass14_0 cDisplayClass140 = new EntityTemplates.\u003C\u003Ec__DisplayClass14_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass140.eggId = eggId;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass140.baby_id = baby_id;
    FertilityMonitor.Def def = prefab.AddOrGetDef<FertilityMonitor.Def>();
    def.baseFertileCycles = fertility_cycles;
    DebugUtil.DevAssert(eggSortOrder > -1, "Added a fertile creature without an egg sort order!", (Object) null);
    float base_incubation_rate = (float) (100.0 / (600.0 * (double) incubation_cycles));
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    GameObject egg = EggConfig.CreateEgg(cDisplayClass140.eggId, eggName, eggDesc, Tag.op_Implicit(cDisplayClass140.baby_id), egg_anim, egg_mass, eggSortOrder, base_incubation_rate);
    // ISSUE: reference to a compiler-generated field
    def.eggPrefab = new Tag(cDisplayClass140.eggId);
    def.initialBreedingWeights = egg_chances;
    if ((double) egg_anim_scale != 1.0)
    {
      KBatchedAnimController component = egg.GetComponent<KBatchedAnimController>();
      component.animWidth = egg_anim_scale;
      component.animHeight = egg_anim_scale;
    }
    // ISSUE: reference to a compiler-generated field
    cDisplayClass140.egg_prefab_id = egg.GetComponent<KPrefabID>();
    SymbolOverrideController prefab1 = SymbolOverrideControllerUtil.AddToPrefab(egg);
    string symbolPrefix = prefab.GetComponent<CreatureBrain>().symbolPrefix;
    if (!string.IsNullOrEmpty(symbolPrefix))
      prefab1.ApplySymbolOverridesByAffix(Assets.GetAnim(HashedString.op_Implicit(egg_anim)), symbolPrefix);
    // ISSUE: reference to a compiler-generated field
    cDisplayClass140.creature_prefab_id = prefab.GetComponent<KPrefabID>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    cDisplayClass140.creature_prefab_id.prefabSpawnFn += new KPrefabID.PrefabFn((object) cDisplayClass140, __methodptr(\u003CExtendEntityToFertileCreature\u003Eb__0));
    if (is_ranchable)
      prefab.AddOrGetDef<RanchableMonitor.Def>();
    if (add_fixed_capturable_monitor)
      prefab.AddOrGetDef<FixedCapturableMonitor.Def>();
    if (add_fish_overcrowding_monitor)
      egg.AddOrGetDef<FishOvercrowdingMonitor.Def>();
    if (deprecated)
    {
      egg.AddTag(GameTags.DeprecatedContent);
      prefab.AddTag(GameTags.DeprecatedContent);
    }
    return prefab;
  }

  public static GameObject ExtendEntityToBeingABaby(
    GameObject prefab,
    Tag adult_prefab_id,
    string on_grow_item_drop_id = null,
    bool force_adult_nav_type = false,
    float adult_threshold = 5f)
  {
    prefab.AddOrGetDef<BabyMonitor.Def>().adultPrefab = adult_prefab_id;
    prefab.AddOrGetDef<BabyMonitor.Def>().onGrowDropID = on_grow_item_drop_id;
    prefab.AddOrGetDef<BabyMonitor.Def>().forceAdultNavType = force_adult_nav_type;
    prefab.AddOrGetDef<BabyMonitor.Def>().adultThreshold = adult_threshold;
    prefab.AddOrGetDef<IncubatorMonitor.Def>();
    prefab.AddOrGetDef<CreatureSleepMonitor.Def>();
    prefab.AddOrGetDef<CallAdultMonitor.Def>();
    prefab.AddOrGetDef<AgeMonitor.Def>().maxAgePercentOnSpawn = 0.01f;
    return prefab;
  }

  public static GameObject ExtendEntityToBasicCreature(
    GameObject template,
    FactionManager.FactionID faction = FactionManager.FactionID.Prey,
    string initialTraitID = null,
    string NavGridName = "WalkerNavGrid1x1",
    NavType navType = NavType.Floor,
    int max_probing_radius = 32,
    float moveSpeed = 2f,
    string onDeathDropID = "Meat",
    int onDeathDropCount = 1,
    bool drownVulnerable = true,
    bool entombVulnerable = true,
    float warningLowTemperature = 283.15f,
    float warningHighTemperature = 293.15f,
    float lethalLowTemperature = 243.15f,
    float lethalHighTemperature = 343.15f)
  {
    template.GetComponent<KBatchedAnimController>().isMovable = true;
    template.AddOrGet<KPrefabID>().AddTag(GameTags.Creature, false);
    Modifiers modifiers = template.AddOrGet<Modifiers>();
    if (initialTraitID != null)
      modifiers.initialTraits.Add(initialTraitID);
    modifiers.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
    template.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_pivot"), false);
    template.AddOrGet<Pickupable>();
    template.AddOrGet<Clearable>().isClearable = false;
    template.AddOrGet<Traits>();
    template.AddOrGet<Health>();
    template.AddOrGet<CharacterOverlay>();
    template.AddOrGet<RangedAttackable>();
    template.AddOrGet<FactionAlignment>().Alignment = faction;
    template.AddOrGet<Prioritizable>();
    template.AddOrGet<Effects>();
    template.AddOrGetDef<CreatureDebugGoToMonitor.Def>();
    template.AddOrGetDef<DeathMonitor.Def>();
    template.AddOrGetDef<AnimInterruptMonitor.Def>();
    template.AddOrGet<AnimEventHandler>();
    SymbolOverrideControllerUtil.AddToPrefab(template);
    template.AddOrGet<TemperatureVulnerable>().Configure(warningLowTemperature, lethalLowTemperature, warningHighTemperature, lethalHighTemperature);
    if (drownVulnerable)
      template.AddOrGet<DrowningMonitor>();
    if (entombVulnerable)
      template.AddOrGet<EntombVulnerable>();
    if (onDeathDropCount > 0 && onDeathDropID != "")
    {
      string[] drops = new string[onDeathDropCount];
      for (int index = 0; index < drops.Length; ++index)
        drops[index] = onDeathDropID;
      template.AddOrGet<Butcherable>().SetDrops(drops);
    }
    Navigator navigator = template.AddOrGet<Navigator>();
    navigator.NavGridName = NavGridName;
    navigator.CurrentNavType = navType;
    navigator.defaultSpeed = moveSpeed;
    navigator.updateProber = true;
    navigator.maxProbingRadius = max_probing_radius;
    navigator.sceneLayer = Grid.SceneLayer.Creatures;
    return template;
  }

  public static void AddCreatureBrain(
    GameObject prefab,
    ChoreTable.Builder chore_table,
    Tag species,
    string symbol_prefix)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    EntityTemplates.\u003C\u003Ec__DisplayClass17_0 cDisplayClass170 = new EntityTemplates.\u003C\u003Ec__DisplayClass17_0();
    CreatureBrain creatureBrain = prefab.AddOrGet<CreatureBrain>();
    creatureBrain.species = species;
    creatureBrain.symbolPrefix = symbol_prefix;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass170.chore_consumer = prefab.AddOrGet<ChoreConsumer>();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass170.chore_consumer.choreTable = chore_table.CreateTable();
    KPrefabID kprefabId = prefab.AddOrGet<KPrefabID>();
    kprefabId.AddTag(GameTags.CreatureBrain, false);
    // ISSUE: method pointer
    kprefabId.instantiateFn += new KPrefabID.PrefabFn((object) cDisplayClass170, __methodptr(\u003CAddCreatureBrain\u003Eb__0));
  }

  public static Tag GetBaggedCreatureTag(Tag tag) => TagManager.Create("Bagged" + ((Tag) ref tag).Name);

  public static Tag GetUnbaggedCreatureTag(Tag bagged_tag) => TagManager.Create(((Tag) ref bagged_tag).Name.Substring(6));

  public static string GetBaggedCreatureID(string name) => "Bagged" + name;

  public static GameObject CreateAndRegisterBaggedCreature(
    GameObject creature,
    bool must_stand_on_top_for_pickup,
    bool allow_mark_for_capture,
    bool use_gun_for_pickup = false)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    EntityTemplates.\u003C\u003Ec__DisplayClass21_0 cDisplayClass210 = new EntityTemplates.\u003C\u003Ec__DisplayClass21_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass210.creature_prefab_id = creature.GetComponent<KPrefabID>();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass210.creature_prefab_id.AddTag(GameTags.BagableCreature, false);
    Baggable baggable = creature.AddOrGet<Baggable>();
    baggable.mustStandOntopOfTrapForPickup = must_stand_on_top_for_pickup;
    baggable.useGunForPickup = use_gun_for_pickup;
    creature.AddOrGet<Capturable>().allowCapture = allow_mark_for_capture;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    cDisplayClass210.creature_prefab_id.prefabSpawnFn += new KPrefabID.PrefabFn((object) cDisplayClass210, __methodptr(\u003CCreateAndRegisterBaggedCreature\u003Eb__0));
    return creature;
  }

  public static GameObject CreateLooseEntity(
    string id,
    string name,
    string desc,
    float mass,
    bool unitMass,
    KAnimFile anim,
    string initialAnim,
    Grid.SceneLayer sceneLayer,
    EntityTemplates.CollisionShape collisionShape,
    float width = 1f,
    float height = 1f,
    bool isPickupable = false,
    int sortOrder = 0,
    SimHashes element = SimHashes.Creature,
    List<Tag> additionalTags = null)
  {
    GameObject go = EntityTemplates.AddCollision(EntityTemplates.CreateBasicEntity(id, name, desc, mass, unitMass, anim, initialAnim, sceneLayer, element, additionalTags), collisionShape, width, height);
    go.GetComponent<KBatchedAnimController>().isMovable = true;
    go.AddOrGet<Modifiers>();
    if (isPickupable)
    {
      Pickupable pickupable = go.AddOrGet<Pickupable>();
      pickupable.SetWorkTime(5f);
      pickupable.sortOrder = sortOrder;
    }
    return go;
  }

  public static void CreateBaseOreTemplates()
  {
    EntityTemplates.baseOreTemplate = new GameObject("OreTemplate");
    Object.DontDestroyOnLoad((Object) EntityTemplates.baseOreTemplate);
    EntityTemplates.baseOreTemplate.SetActive(false);
    EntityTemplates.baseOreTemplate.AddComponent<KPrefabID>();
    EntityTemplates.baseOreTemplate.AddComponent<PrimaryElement>();
    EntityTemplates.baseOreTemplate.AddComponent<Pickupable>();
    EntityTemplates.baseOreTemplate.AddComponent<KSelectable>();
    EntityTemplates.baseOreTemplate.AddComponent<SaveLoadRoot>();
    EntityTemplates.baseOreTemplate.AddComponent<StateMachineController>();
    EntityTemplates.baseOreTemplate.AddComponent<Clearable>();
    EntityTemplates.baseOreTemplate.AddComponent<Prioritizable>();
    EntityTemplates.baseOreTemplate.AddComponent<KBatchedAnimController>();
    EntityTemplates.baseOreTemplate.AddComponent<SimTemperatureTransfer>();
    EntityTemplates.baseOreTemplate.AddComponent<Modifiers>();
    EntityTemplates.baseOreTemplate.AddOrGet<OccupyArea>().OccupiedCellsOffsets = new CellOffset[1];
    DecorProvider decorProvider = EntityTemplates.baseOreTemplate.AddOrGet<DecorProvider>();
    decorProvider.baseDecor = -10f;
    decorProvider.baseRadius = 1f;
    EntityTemplates.baseOreTemplate.AddOrGet<ElementChunk>();
  }

  public static void DestroyBaseOreTemplates()
  {
    Object.Destroy((Object) EntityTemplates.baseOreTemplate);
    EntityTemplates.baseOreTemplate = (GameObject) null;
  }

  public static GameObject CreateOreEntity(
    SimHashes elementID,
    EntityTemplates.CollisionShape shape,
    float width,
    float height,
    List<Tag> additionalTags = null,
    float default_temperature = 293f)
  {
    Element elementByHash = ElementLoader.FindElementByHash(elementID);
    GameObject gameObject = Object.Instantiate<GameObject>(EntityTemplates.baseOreTemplate);
    ((Object) gameObject).name = elementByHash.name;
    Object.DontDestroyOnLoad((Object) gameObject);
    KPrefabID kprefabId = gameObject.AddOrGet<KPrefabID>();
    kprefabId.PrefabTag = elementByHash.tag;
    kprefabId.InitializeTags(false);
    if (additionalTags != null)
    {
      foreach (Tag additionalTag in additionalTags)
        kprefabId.AddTag(additionalTag, false);
    }
    if ((double) elementByHash.lowTemp < 296.14999389648438 && (double) elementByHash.highTemp > 296.14999389648438)
      kprefabId.AddTag(GameTags.PedestalDisplayable, false);
    PrimaryElement primaryElement = gameObject.AddOrGet<PrimaryElement>();
    primaryElement.SetElement(elementID);
    primaryElement.Mass = 1f;
    primaryElement.Temperature = default_temperature;
    Pickupable pickupable = gameObject.AddOrGet<Pickupable>();
    pickupable.SetWorkTime(5f);
    pickupable.sortOrder = elementByHash.buildMenuSort;
    gameObject.AddOrGet<KSelectable>().SetName(elementByHash.name);
    KBatchedAnimController kbatchedAnimController = gameObject.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      elementByHash.substance.anim
    };
    kbatchedAnimController.sceneLayer = Grid.SceneLayer.Front;
    kbatchedAnimController.initialAnim = "idle1";
    kbatchedAnimController.isMovable = true;
    return EntityTemplates.AddCollision(gameObject, shape, width, height);
  }

  public static GameObject CreateSolidOreEntity(SimHashes elementId, List<Tag> additionalTags = null) => EntityTemplates.CreateOreEntity(elementId, EntityTemplates.CollisionShape.CIRCLE, 0.5f, 0.5f, additionalTags);

  public static GameObject CreateLiquidOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
  {
    GameObject oreEntity = EntityTemplates.CreateOreEntity(elementId, EntityTemplates.CollisionShape.RECTANGLE, 0.5f, 0.6f, additionalTags);
    oreEntity.AddOrGet<Dumpable>().SetWorkTime(5f);
    oreEntity.AddOrGet<SubstanceChunk>();
    return oreEntity;
  }

  public static GameObject CreateGasOreEntity(SimHashes elementId, List<Tag> additionalTags = null)
  {
    GameObject oreEntity = EntityTemplates.CreateOreEntity(elementId, EntityTemplates.CollisionShape.RECTANGLE, 0.5f, 0.6f, additionalTags);
    oreEntity.AddOrGet<Dumpable>().SetWorkTime(5f);
    oreEntity.AddOrGet<SubstanceChunk>();
    return oreEntity;
  }

  public static GameObject ExtendEntityToFood(GameObject template, EdiblesManager.FoodInfo foodInfo)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    EntityTemplates.\u003C\u003Ec__DisplayClass31_0 cDisplayClass310 = new EntityTemplates.\u003C\u003Ec__DisplayClass31_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass310.foodInfo = foodInfo;
    template.AddOrGet<EntitySplitter>();
    // ISSUE: reference to a compiler-generated field
    if (cDisplayClass310.foodInfo.CanRot)
    {
      Rottable.Def def = template.AddOrGetDef<Rottable.Def>();
      // ISSUE: reference to a compiler-generated field
      def.preserveTemperature = cDisplayClass310.foodInfo.PreserveTemperature;
      // ISSUE: reference to a compiler-generated field
      def.rotTemperature = cDisplayClass310.foodInfo.RotTemperature;
      // ISSUE: reference to a compiler-generated field
      def.spoilTime = cDisplayClass310.foodInfo.SpoilTime;
      // ISSUE: reference to a compiler-generated field
      def.staleTime = cDisplayClass310.foodInfo.StaleTime;
      EntityTemplates.CreateAndRegisterCompostableFromPrefab(template);
    }
    KPrefabID component = template.GetComponent<KPrefabID>();
    component.AddTag(GameTags.PedestalDisplayable, false);
    // ISSUE: reference to a compiler-generated field
    if ((double) cDisplayClass310.foodInfo.CaloriesPerUnit > 0.0)
    {
      component.AddTag(GameTags.Edible, false);
      // ISSUE: reference to a compiler-generated field
      template.AddOrGet<Edible>().FoodInfo = cDisplayClass310.foodInfo;
      // ISSUE: method pointer
      component.instantiateFn += new KPrefabID.PrefabFn((object) cDisplayClass310, __methodptr(\u003CExtendEntityToFood\u003Eb__0));
      GameTags.DisplayAsCalories.Add(component.PrefabTag);
    }
    else
    {
      component.AddTag(GameTags.CookingIngredient, false);
      template.AddOrGet<HasSortOrder>();
    }
    return template;
  }

  public static GameObject ExtendEntityToMedicine(GameObject template, MedicineInfo medicineInfo)
  {
    template.AddOrGet<EntitySplitter>();
    KPrefabID component = template.GetComponent<KPrefabID>();
    Debug.Assert(Tag.op_Equality(component.PrefabID(), Tag.op_Implicit(medicineInfo.id)), (object) "Tried assigning a medicine info to a non-matching prefab!");
    MedicinalPill medicinalPill = template.AddOrGet<MedicinalPill>();
    medicinalPill.info = medicineInfo;
    if (medicineInfo.doctorStationId == null)
    {
      template.AddOrGet<MedicinalPillWorkable>().pill = medicinalPill;
      component.AddTag(GameTags.Medicine, false);
    }
    else
    {
      component.AddTag(GameTags.MedicalSupplies, false);
      component.AddTag(medicineInfo.GetSupplyTag(), false);
    }
    return template;
  }

  public static GameObject ExtendPlantToFertilizable(
    GameObject template,
    PlantElementAbsorber.ConsumeInfo[] fertilizers)
  {
    template.GetComponent<Modifiers>().initialAttributes.Add(Db.Get().PlantAttributes.FertilizerUsageMod.Id);
    HashedString idHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
    foreach (PlantElementAbsorber.ConsumeInfo fertilizer in fertilizers)
    {
      ManualDeliveryKG manualDeliveryKg = template.AddComponent<ManualDeliveryKG>();
      manualDeliveryKg.RequestedItemTag = fertilizer.tag;
      manualDeliveryKg.capacity = (float) ((double) fertilizer.massConsumptionRate * 600.0 * 3.0);
      manualDeliveryKg.refillMass = (float) ((double) fertilizer.massConsumptionRate * 600.0 * 0.5);
      manualDeliveryKg.MinimumMass = (float) ((double) fertilizer.massConsumptionRate * 600.0 * 0.5);
      manualDeliveryKg.operationalRequirement = Operational.State.Functional;
      manualDeliveryKg.choreTypeIDHash = idHash;
    }
    KPrefabID component = template.GetComponent<KPrefabID>();
    FertilizationMonitor.Def def = template.AddOrGetDef<FertilizationMonitor.Def>();
    def.wrongFertilizerTestTag = GameTags.Solid;
    def.consumedElements = fertilizers;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    KPrefabID.PrefabFn prefabFn = EntityTemplates.\u003C\u003Ec.\u003C\u003E9__33_0 ?? (EntityTemplates.\u003C\u003Ec.\u003C\u003E9__33_0 = new KPrefabID.PrefabFn((object) EntityTemplates.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CExtendPlantToFertilizable\u003Eb__33_0)));
    component.prefabInitFn += prefabFn;
    return template;
  }

  public static GameObject ExtendPlantToIrrigated(
    GameObject template,
    PlantElementAbsorber.ConsumeInfo info)
  {
    return EntityTemplates.ExtendPlantToIrrigated(template, new PlantElementAbsorber.ConsumeInfo[1]
    {
      info
    });
  }

  public static GameObject ExtendPlantToIrrigated(
    GameObject template,
    PlantElementAbsorber.ConsumeInfo[] consume_info)
  {
    template.GetComponent<Modifiers>().initialAttributes.Add(Db.Get().PlantAttributes.FertilizerUsageMod.Id);
    HashedString idHash = Db.Get().ChoreTypes.FarmFetch.IdHash;
    foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in consume_info)
    {
      ManualDeliveryKG manualDeliveryKg = template.AddComponent<ManualDeliveryKG>();
      manualDeliveryKg.RequestedItemTag = consumeInfo.tag;
      manualDeliveryKg.capacity = (float) ((double) consumeInfo.massConsumptionRate * 600.0 * 3.0);
      manualDeliveryKg.refillMass = (float) ((double) consumeInfo.massConsumptionRate * 600.0 * 0.5);
      manualDeliveryKg.MinimumMass = (float) ((double) consumeInfo.massConsumptionRate * 600.0 * 0.5);
      manualDeliveryKg.operationalRequirement = Operational.State.Functional;
      manualDeliveryKg.choreTypeIDHash = idHash;
    }
    IrrigationMonitor.Def def = template.AddOrGetDef<IrrigationMonitor.Def>();
    def.wrongIrrigationTestTag = GameTags.Liquid;
    def.consumedElements = consume_info;
    return template;
  }

  public static GameObject CreateAndRegisterCompostableFromPrefab(GameObject original)
  {
    if (Object.op_Inequality((Object) original.GetComponent<Compostable>(), (Object) null))
      return (GameObject) null;
    original.AddComponent<Compostable>().isMarkedForCompost = false;
    KPrefabID component = original.GetComponent<KPrefabID>();
    GameObject compostableFromPrefab = Object.Instantiate<GameObject>(original);
    Object.DontDestroyOnLoad((Object) compostableFromPrefab);
    string str = "Compost" + ((Tag) ref component.PrefabTag).Name;
    string name = MISC.TAGS.COMPOST_FORMAT.Replace("{Item}", component.PrefabTag.ProperName());
    compostableFromPrefab.GetComponent<KPrefabID>().PrefabTag = TagManager.Create(str, name);
    compostableFromPrefab.GetComponent<KPrefabID>().AddTag(GameTags.Compostable, false);
    ((Object) compostableFromPrefab).name = name;
    compostableFromPrefab.GetComponent<Compostable>().isMarkedForCompost = true;
    compostableFromPrefab.GetComponent<KSelectable>().SetName(name);
    compostableFromPrefab.GetComponent<Compostable>().originalPrefab = original;
    compostableFromPrefab.GetComponent<Compostable>().compostPrefab = compostableFromPrefab;
    original.GetComponent<Compostable>().originalPrefab = original;
    original.GetComponent<Compostable>().compostPrefab = compostableFromPrefab;
    Assets.AddPrefab(compostableFromPrefab.GetComponent<KPrefabID>());
    return compostableFromPrefab;
  }

  public static GameObject CreateAndRegisterSeedForPlant(
    GameObject plant,
    SeedProducer.ProductionType productionType,
    string id,
    string name,
    string desc,
    KAnimFile anim,
    string initialAnim = "object",
    int numberOfSeeds = 1,
    List<Tag> additionalTags = null,
    SingleEntityReceptacle.ReceptacleDirection planterDirection = SingleEntityReceptacle.ReceptacleDirection.Top,
    Tag replantGroundTag = default (Tag),
    int sortOrder = 0,
    string domesticatedDescription = "",
    EntityTemplates.CollisionShape collisionShape = EntityTemplates.CollisionShape.CIRCLE,
    float width = 0.25f,
    float height = 0.25f,
    Recipe.Ingredient[] recipe_ingredients = null,
    string recipe_description = "",
    bool ignoreDefaultSeedTag = false)
  {
    GameObject looseEntity = EntityTemplates.CreateLooseEntity(id, name, desc, 1f, true, anim, initialAnim, Grid.SceneLayer.Front, collisionShape, width, height, true, SORTORDER.SEEDS + sortOrder);
    looseEntity.AddOrGet<EntitySplitter>();
    GameObject compostableFromPrefab = EntityTemplates.CreateAndRegisterCompostableFromPrefab(looseEntity);
    PlantableSeed plantableSeed = looseEntity.AddOrGet<PlantableSeed>();
    plantableSeed.PlantID = new Tag(((Object) plant).name);
    plantableSeed.replantGroundTag = replantGroundTag;
    plantableSeed.domesticatedDescription = domesticatedDescription;
    plantableSeed.direction = planterDirection;
    KPrefabID component1 = looseEntity.GetComponent<KPrefabID>();
    foreach (Tag additionalTag in additionalTags)
      component1.AddTag(additionalTag, false);
    if (!ignoreDefaultSeedTag)
      component1.AddTag(GameTags.Seed, false);
    component1.AddTag(GameTags.PedestalDisplayable, false);
    MutantPlant component2 = plant.GetComponent<MutantPlant>();
    if (Object.op_Inequality((Object) component2, (Object) null))
    {
      MutantPlant mutantPlant1 = looseEntity.AddOrGet<MutantPlant>();
      MutantPlant mutantPlant2 = compostableFromPrefab.AddOrGet<MutantPlant>();
      mutantPlant1.SpeciesID = component2.SpeciesID;
      Tag speciesId = component2.SpeciesID;
      mutantPlant2.SpeciesID = speciesId;
    }
    Assets.AddPrefab(component1);
    plant.AddOrGet<SeedProducer>().Configure(id, productionType, numberOfSeeds);
    return looseEntity;
  }

  public static GameObject CreateAndRegisterPreview(
    string id,
    KAnimFile anim,
    string initial_anim,
    ObjectLayer object_layer,
    int width,
    int height)
  {
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id, id, id, 1f, anim, initial_anim, Grid.SceneLayer.Front, width, height, TUNING.BUILDINGS.DECOR.NONE);
    Util.UpdateComponentRequirement<KSelectable>(placedEntity, false);
    Util.UpdateComponentRequirement<SaveLoadRoot>(placedEntity, false);
    placedEntity.AddOrGet<EntityPreview>().objectLayer = object_layer;
    OccupyArea occupyArea = placedEntity.AddOrGet<OccupyArea>();
    occupyArea.objectLayers = new ObjectLayer[1]
    {
      object_layer
    };
    occupyArea.ApplyToCells = false;
    placedEntity.AddOrGet<Storage>();
    Assets.AddPrefab(placedEntity.GetComponent<KPrefabID>());
    return placedEntity;
  }

  public static GameObject CreateAndRegisterPreviewForPlant(
    GameObject seed,
    string id,
    KAnimFile anim,
    string initialAnim,
    int width,
    int height)
  {
    GameObject andRegisterPreview = EntityTemplates.CreateAndRegisterPreview(id, anim, initialAnim, ObjectLayer.Building, width, height);
    seed.GetComponent<PlantableSeed>().PreviewID = TagManager.Create(id);
    return andRegisterPreview;
  }

  public static CellOffset[] GenerateOffsets(int width, int height)
  {
    int num1 = width / 2;
    int startX = num1 - width + 1;
    int num2 = 0;
    int num3 = height - 1;
    int startY = num2;
    int endX = num1;
    int endY = num3;
    return EntityTemplates.GenerateOffsets(startX, startY, endX, endY);
  }

  private static CellOffset[] GenerateOffsets(int startX, int startY, int endX, int endY)
  {
    List<CellOffset> cellOffsetList = new List<CellOffset>();
    for (int index1 = startY; index1 <= endY; ++index1)
    {
      for (int index2 = startX; index2 <= endX; ++index2)
        cellOffsetList.Add(new CellOffset()
        {
          x = index2,
          y = index1
        });
    }
    return cellOffsetList.ToArray();
  }

  public static CellOffset[] GenerateHangingOffsets(int width, int height)
  {
    int num1 = width / 2;
    int startX = num1 - width + 1;
    int num2 = -height + 1;
    int num3 = 0;
    int startY = num2;
    int endX = num1;
    int endY = num3;
    return EntityTemplates.GenerateOffsets(startX, startY, endX, endY);
  }

  public static GameObject AddCollision(
    GameObject template,
    EntityTemplates.CollisionShape shape,
    float width,
    float height)
  {
    switch (shape)
    {
      case EntityTemplates.CollisionShape.RECTANGLE:
        template.AddOrGet<KBoxCollider2D>().size = Vector2f.op_Implicit(new Vector2f(width, height));
        break;
      case EntityTemplates.CollisionShape.POLYGONAL:
        template.AddOrGet<PolygonCollider2D>();
        break;
      default:
        template.AddOrGet<KCircleCollider2D>().radius = Mathf.Max(width, height);
        break;
    }
    return template;
  }

  public enum CollisionShape
  {
    CIRCLE,
    RECTANGLE,
    POLYGONAL,
  }
}
