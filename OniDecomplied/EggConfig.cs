// Decompiled with JetBrains decompiler
// Type: EggConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class EggConfig
{
  public static GameObject CreateEgg(
    string id,
    string name,
    string desc,
    Tag creature_id,
    string anim,
    float mass,
    int egg_sort_order,
    float base_incubation_rate)
  {
    GameObject looseEntity = EntityTemplates.CreateLooseEntity(id, name, desc, mass, true, Assets.GetAnim(HashedString.op_Implicit(anim)), "idle", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 0.8f, true);
    looseEntity.AddOrGet<KBoxCollider2D>().offset = Vector2f.op_Implicit(new Vector2f(0.0f, 0.36f));
    looseEntity.AddOrGet<Pickupable>().sortOrder = SORTORDER.EGGS + egg_sort_order;
    looseEntity.AddOrGet<Effects>();
    KPrefabID kprefabId = looseEntity.AddOrGet<KPrefabID>();
    kprefabId.AddTag(GameTags.Egg, false);
    kprefabId.AddTag(GameTags.IncubatableEgg, false);
    kprefabId.AddTag(GameTags.PedestalDisplayable, false);
    IncubationMonitor.Def def = looseEntity.AddOrGetDef<IncubationMonitor.Def>();
    def.spawnedCreature = creature_id;
    def.baseIncubationRate = base_incubation_rate;
    looseEntity.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
    Object.Destroy((Object) looseEntity.GetComponent<EntitySplitter>());
    Assets.AddPrefab(looseEntity.GetComponent<KPrefabID>());
    string str1 = string.Format((string) STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RESULT_DESCRIPTION, (object) name);
    ComplexRecipe.RecipeElement[] recipeElementArray1 = new ComplexRecipe.RecipeElement[1]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit(id), 1f)
    };
    ComplexRecipe.RecipeElement[] recipeElementArray2 = new ComplexRecipe.RecipeElement[2]
    {
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("RawEgg"), 0.5f * mass, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature),
      new ComplexRecipe.RecipeElement(Tag.op_Implicit("EggShell"), 0.5f * mass, ComplexRecipe.RecipeElement.TemperatureOperation.AverageTemperature)
    };
    string obsolete_id = ComplexRecipeManager.MakeObsoleteRecipeID(id, Tag.op_Implicit("RawEgg"));
    string str2 = ComplexRecipeManager.MakeRecipeID("EggCracker", (IList<ComplexRecipe.RecipeElement>) recipeElementArray1, (IList<ComplexRecipe.RecipeElement>) recipeElementArray2);
    ComplexRecipe complexRecipe = new ComplexRecipe(str2, recipeElementArray1, recipeElementArray2);
    complexRecipe.description = string.Format((string) STRINGS.BUILDINGS.PREFABS.EGGCRACKER.RECIPE_DESCRIPTION, (object) name, (object) str1);
    List<Tag> tagList = new List<Tag>();
    tagList.Add(Tag.op_Implicit("EggCracker"));
    complexRecipe.fabricators = tagList;
    complexRecipe.time = 5f;
    ComplexRecipeManager.Get().AddObsoleteIDMapping(obsolete_id, str2);
    return looseEntity;
  }
}
