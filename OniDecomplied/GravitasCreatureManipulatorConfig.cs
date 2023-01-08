// Decompiled with JetBrains decompiler
// Type: GravitasCreatureManipulatorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TUNING;
using UnityEngine;

public class GravitasCreatureManipulatorConfig : IBuildingConfig
{
  public const string ID = "GravitasCreatureManipulator";
  public const string CODEX_ENTRY_ID = "STORYTRAITCRITTERMANIPULATOR";
  public const string INITIAL_LORE_UNLOCK_ID = "story_trait_critter_manipulator_initial";
  public const string PARKING_LORE_UNLOCK_ID = "story_trait_critter_manipulator_parking";
  public const string COMPLETED_LORE_UNLOCK_ID = "story_trait_critter_manipulator_complete";
  private const int HEIGHT = 4;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR5_1 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues tieR5_2 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues tieR2 = TUNING.BUILDINGS.DECOR.BONUS.TIER2;
    EffectorValues noise = tieR5_2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GravitasCreatureManipulator", 3, 4, "gravitas_critter_manipulator_kanim", 250, 120f, tieR5_1, refinedMetals, 3200f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.0f;
    buildingDef.Floodable = false;
    buildingDef.Entombable = true;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "medium";
    buildingDef.ForegroundLayer = Grid.SceneLayer.Ground;
    buildingDef.ShowInBuildMenu = false;
    return buildingDef;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    PrimaryElement component = go.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.Steel);
    component.Temperature = 294.15f;
    BuildingTemplates.ExtendBuildingToGravitas(go);
    go.AddComponent<Storage>();
    Activatable activatable = go.AddComponent<Activatable>();
    activatable.synchronizeAnims = false;
    activatable.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_use_remote_kanim"))
    };
    activatable.SetWorkTime(30f);
    GravitasCreatureManipulator.Def def1 = go.AddOrGetDef<GravitasCreatureManipulator.Def>();
    def1.pickupOffset = new CellOffset(-1, 0);
    def1.dropOffset = new CellOffset(1, 0);
    def1.numSpeciesToUnlockMorphMode = 5;
    def1.workingDuration = 15f;
    def1.cooldownDuration = 540f;
    MakeBaseSolid.Def def2 = go.AddOrGetDef<MakeBaseSolid.Def>();
    def2.solidOffsets = new CellOffset[4];
    for (int index = 0; index < 4; ++index)
      def2.solidOffsets[index] = new CellOffset(0, index);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabInitFn += GravitasCreatureManipulatorConfig.\u003C\u003Ec.\u003C\u003E9__8_0 ?? (GravitasCreatureManipulatorConfig.\u003C\u003Ec.\u003C\u003E9__8_0 = new KPrefabID.PrefabFn((object) GravitasCreatureManipulatorConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__8_0)));
  }

  public static Option<string> GetBodyContentForSpeciesTag(Tag species)
  {
    Option<string> nameForSpeciesTag = GravitasCreatureManipulatorConfig.GetNameForSpeciesTag(species);
    Option<string> descriptionForSpeciesTag = GravitasCreatureManipulatorConfig.GetDescriptionForSpeciesTag(species);
    return nameForSpeciesTag.HasValue && descriptionForSpeciesTag.HasValue ? (Option<string>) GravitasCreatureManipulatorConfig.GetBodyContent(nameForSpeciesTag.Value, descriptionForSpeciesTag.Value) : (Option<string>) Option.None;
  }

  public static string GetBodyContentForUnknownSpecies() => GravitasCreatureManipulatorConfig.GetBodyContent((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.UNKNOWN_TITLE, (string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.UNKNOWN);

  public static string GetBodyContent(string name, string desc) => "<size=125%><b>" + name + "</b></size><line-height=150%>\n</line-height>" + desc;

  public static Option<string> GetNameForSpeciesTag(Tag species)
  {
    if (Tag.op_Equality(species, GameTags.Creatures.Species.HatchSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.HATCHSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.LightBugSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.LIGHTBUGSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.OilFloaterSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.OILFLOATERSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.DreckoSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.DRECKOSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.GlomSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.GLOMSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.PuftSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.PUFTSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.PacuSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.PACUSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.MooSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.MOOSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.MoleSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.MOLESPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.SquirrelSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.SQUIRRELSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.CrabSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.CRABSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.DivergentSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.DIVERGENTSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.StaterpillarSpecies))
      return Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.STATERPILLARSPECIES);
    return Tag.op_Equality(species, GameTags.Creatures.Species.BeetaSpecies) ? Option.Some<string>((string) STRINGS.CREATURES.FAMILY_PLURAL.BEETASPECIES) : (Option<string>) Option.None;
  }

  public static Option<string> GetDescriptionForSpeciesTag(Tag species)
  {
    if (Tag.op_Equality(species, GameTags.Creatures.Species.HatchSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.HATCH);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.LightBugSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.LIGHTBUG);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.OilFloaterSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.OILFLOATER);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.DreckoSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.DRECKO);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.GlomSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.GLOM);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.PuftSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.PUFT);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.PacuSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.PACU);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.MooSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.MOO);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.MoleSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.MOLE);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.SquirrelSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.SQUIRREL);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.CrabSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.CRAB);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.DivergentSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.DIVERGENTSPECIES);
    if (Tag.op_Equality(species, GameTags.Creatures.Species.StaterpillarSpecies))
      return Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.STATERPILLAR);
    return Tag.op_Equality(species, GameTags.Creatures.Species.BeetaSpecies) ? Option.Some<string>((string) CODEX.STORY_TRAITS.CRITTER_MANIPULATOR.SPECIES_ENTRIES.BEETA) : (Option<string>) Option.None;
  }

  public static class CRITTER_LORE_UNLOCK_ID
  {
    public static string For(Tag species) => "story_trait_critter_manipulator_" + species.ToString().ToLower();
  }
}
