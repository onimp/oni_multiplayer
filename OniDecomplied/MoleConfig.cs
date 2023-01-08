// Decompiled with JetBrains decompiler
// Type: MoleConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class MoleConfig : IEntityConfig
{
  public const string ID = "Mole";
  public const string BASE_TRAIT_ID = "MoleBaseTrait";
  public const string EGG_ID = "MoleEgg";
  private static float MIN_POOP_SIZE_IN_CALORIES = 2400000f;
  private static float CALORIES_PER_KG_OF_DIRT = 1000f;
  public static int EGG_SORT_ORDER = 800;

  public static GameObject CreateMole(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby = false)
  {
    GameObject mole = BaseMoleConfig.BaseMole(id, name, (string) STRINGS.CREATURES.SPECIES.MOLE.DESC, "MoleBaseTrait", anim_file, is_baby);
    mole.AddTag(GameTags.Creatures.Digger);
    EntityTemplates.ExtendEntityToWildCreature(mole, MoleTuning.PEN_SIZE_PER_CREATURE);
    Trait trait = Db.Get().CreateTrait("MoleBaseTrait", name, name, (string) null, false, (ChoreGroup[]) null, true, true);
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, MoleTuning.STANDARD_STOMACH_SIZE, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float) (-(double) MoleTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
    trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
    List<Tag> elementTags = new List<Tag>();
    elementTags.Add(SimHashes.Regolith.CreateTag());
    elementTags.Add(SimHashes.Dirt.CreateTag());
    elementTags.Add(SimHashes.IronOre.CreateTag());
    Diet diet = new Diet(BaseMoleConfig.SimpleOreDiet(elementTags, MoleConfig.CALORIES_PER_KG_OF_DIRT, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL).ToArray());
    CreatureCalorieMonitor.Def def = mole.AddOrGetDef<CreatureCalorieMonitor.Def>();
    def.diet = diet;
    def.minPoopSizeInCalories = MoleConfig.MIN_POOP_SIZE_IN_CALORIES;
    mole.AddOrGetDef<SolidConsumerMonitor.Def>().diet = diet;
    mole.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
    mole.AddOrGet<LoopingSounds>();
    foreach (HashedString hashedString in MoleTuning.GINGER_SYMBOL_NAMES)
      mole.GetComponent<KAnimControllerBase>().SetSymbolVisiblity(KAnimHashedString.op_Implicit(hashedString), false);
    mole.AddTag(GameTags.OriginalCreature);
    return mole;
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject mole = MoleConfig.CreateMole("Mole", (string) STRINGS.CREATURES.SPECIES.MOLE.NAME, (string) STRINGS.CREATURES.SPECIES.MOLE.DESC, "driller_kanim");
    string eggName = (string) STRINGS.CREATURES.SPECIES.MOLE.EGG_NAME;
    string desc = (string) STRINGS.CREATURES.SPECIES.MOLE.DESC;
    double eggMass = (double) MoleTuning.EGG_MASS;
    int eggSortOrder1 = MoleConfig.EGG_SORT_ORDER;
    List<FertilityMonitor.BreedingChance> eggChancesBase = MoleTuning.EGG_CHANCES_BASE;
    int eggSortOrder2 = eggSortOrder1;
    return EntityTemplates.ExtendEntityToFertileCreature(mole, "MoleEgg", eggName, desc, "egg_driller_kanim", (float) eggMass, "MoleBaby", 60.0000038f, 20f, eggChancesBase, eggSortOrder2);
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst) => MoleConfig.SetSpawnNavType(inst);

  public static void SetSpawnNavType(GameObject inst)
  {
    int cell = Grid.PosToCell(inst);
    Navigator component = inst.GetComponent<Navigator>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    if (Grid.IsSolidCell(cell))
    {
      component.SetCurrentNavType(NavType.Solid);
      TransformExtensions.SetPosition(inst.transform, Grid.CellToPosCBC(cell, Grid.SceneLayer.FXFront));
      inst.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.FXFront);
    }
    else
      inst.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
  }
}
