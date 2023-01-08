// Decompiled with JetBrains decompiler
// Type: GlomConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using TUNING;
using UnityEngine;

public class GlomConfig : IEntityConfig
{
  public const string ID = "Glom";
  public const string BASE_TRAIT_ID = "GlomBaseTrait";
  public const SimHashes dirtyEmitElement = SimHashes.ContaminatedOxygen;
  public const float dirtyProbabilityPercent = 25f;
  public const float dirtyCellToTargetMass = 1f;
  public const float dirtyMassPerDirty = 0.2f;
  public const float dirtyMassReleaseOnDeath = 3f;
  public const string emitDisease = "SlimeLung";
  public const int emitDiseasePerKg = 1000;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name1 = (string) STRINGS.CREATURES.SPECIES.GLOM.NAME;
    string name2 = name1;
    string desc = (string) STRINGS.CREATURES.SPECIES.GLOM.DESC;
    EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("glom_kanim"));
    EffectorValues decor = tieR0;
    EffectorValues noise = new EffectorValues();
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("Glom", name2, desc, 25f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
    Db.Get().CreateTrait("GlomBaseTrait", name1, name1, (string) null, false, (ChoreGroup[]) null, true, true).Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name1));
    KPrefabID component = placedEntity.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Creatures.Walker, false);
    component.AddTag(GameTags.OriginalCreature, false);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    component.prefabInitFn += GlomConfig.\u003C\u003Ec.\u003C\u003E9__10_0 ?? (GlomConfig.\u003C\u003Ec.\u003C\u003E9__10_0 = new KPrefabID.PrefabFn((object) GlomConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CCreatePrefab\u003Eb__10_0)));
    EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Pest, "GlomBaseTrait", onDeathDropID: "", onDeathDropCount: 0, warningLowTemperature: 293.15f, warningHighTemperature: 393.15f, lethalLowTemperature: 273.15f, lethalHighTemperature: 423.15f);
    placedEntity.AddWeapon(1f, 1f);
    placedEntity.AddOrGet<Trappable>();
    placedEntity.AddOrGetDef<ThreatMonitor.Def>();
    placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();
    ElementDropperMonitor.Def def = placedEntity.AddOrGetDef<ElementDropperMonitor.Def>();
    def.dirtyEmitElement = SimHashes.ContaminatedOxygen;
    def.dirtyProbabilityPercent = 25f;
    def.dirtyCellToTargetMass = 1f;
    def.dirtyMassPerDirty = 0.2f;
    def.dirtyMassReleaseOnDeath = 3f;
    def.emitDiseaseIdx = Db.Get().Diseases.GetIndex(HashedString.op_Implicit("SlimeLung"));
    def.emitDiseasePerKg = 1000f;
    placedEntity.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.GetComponent<LoopingSounds>().updatePosition = true;
    placedEntity.AddOrGet<DiseaseSourceVisualizer>().alwaysShowDisease = "SlimeLung";
    SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_movement_short", NOISE_POLLUTION.CREATURES.TIER2);
    SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_jump", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_land", NOISE_POLLUTION.CREATURES.TIER3);
    SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_expel", NOISE_POLLUTION.CREATURES.TIER4);
    EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, false);
    EntityTemplates.AddCreatureBrain(placedEntity, new ChoreTable.Builder().Add((StateMachine.BaseDef) new DeathStates.Def()).Add((StateMachine.BaseDef) new TrappedStates.Def()).Add((StateMachine.BaseDef) new BaggedStates.Def()).Add((StateMachine.BaseDef) new FallStates.Def()).Add((StateMachine.BaseDef) new StunnedStates.Def()).Add((StateMachine.BaseDef) new DrowningStates.Def()).Add((StateMachine.BaseDef) new DebugGoToStates.Def()).Add((StateMachine.BaseDef) new FleeStates.Def()).Add((StateMachine.BaseDef) new DropElementStates.Def()).Add((StateMachine.BaseDef) new IdleStates.Def()), GameTags.Creatures.Species.GlomSpecies, (string) null);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
