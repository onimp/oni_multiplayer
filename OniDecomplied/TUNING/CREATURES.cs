// Decompiled with JetBrains decompiler
// Type: TUNING.CREATURES
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TUNING
{
  public class CREATURES
  {
    public const float WILD_GROWTH_RATE_MODIFIER = 0.25f;
    public const int DEFAULT_PROBING_RADIUS = 32;
    public const float FERTILITY_TIME_BY_LIFESPAN = 0.6f;
    public const float INCUBATION_TIME_BY_LIFESPAN = 0.2f;
    public const float INCUBATOR_INCUBATION_MULTIPLIER = 4f;
    public const float WILD_CALORIE_BURN_RATIO = 0.25f;
    public const float HUG_INCUBATION_MULTIPLIER = 1f;
    public const float VIABILITY_LOSS_RATE = -0.0166666675f;
    public const float STATERPILLAR_POWER_CHARGE_LOSS_RATE = -0.055555556f;

    public class HITPOINTS
    {
      public const float TIER0 = 5f;
      public const float TIER1 = 25f;
      public const float TIER2 = 50f;
      public const float TIER3 = 100f;
      public const float TIER4 = 150f;
      public const float TIER5 = 200f;
      public const float TIER6 = 400f;
    }

    public class MASS_KG
    {
      public const float TIER0 = 5f;
      public const float TIER1 = 25f;
      public const float TIER2 = 50f;
      public const float TIER3 = 100f;
      public const float TIER4 = 200f;
      public const float TIER5 = 400f;
    }

    public class TEMPERATURE
    {
      public static float FREEZING_10 = 173f;
      public static float FREEZING_9 = 183f;
      public static float FREEZING_3 = 243f;
      public static float FREEZING_2 = 253f;
      public static float FREEZING_1 = 263f;
      public static float FREEZING = 273f;
      public static float COOL = 283f;
      public static float MODERATE = 293f;
      public static float HOT = 303f;
      public static float HOT_1 = 313f;
      public static float HOT_2 = 323f;
      public static float HOT_3 = 333f;
    }

    public class LIFESPAN
    {
      public const float TIER0 = 5f;
      public const float TIER1 = 25f;
      public const float TIER2 = 75f;
      public const float TIER3 = 100f;
      public const float TIER4 = 150f;
      public const float TIER5 = 200f;
      public const float TIER6 = 400f;
    }

    public class CONVERSION_EFFICIENCY
    {
      public static float BAD_2 = 0.1f;
      public static float BAD_1 = 0.25f;
      public static float NORMAL = 0.5f;
      public static float GOOD_1 = 0.75f;
      public static float GOOD_2 = 0.95f;
      public static float GOOD_3 = 1f;
    }

    public class SPACE_REQUIREMENTS
    {
      public static int TIER1 = 4;
      public static int TIER2 = 8;
      public static int TIER3 = 12;
      public static int TIER4 = 16;
    }

    public class EGG_CHANCE_MODIFIERS
    {
      public static List<System.Action> MODIFIER_CREATORS;

      private static System.Action CreateDietaryModifier(
        string id,
        Tag eggTag,
        HashSet<Tag> foodTags,
        float modifierPerCal)
      {
        return (System.Action) (() =>
        {
          string name = (string) STRINGS.CREATURES.FERTILITY_MODIFIERS.DIET.NAME;
          string desc = (string) STRINGS.CREATURES.FERTILITY_MODIFIERS.DIET.DESC;
          Db.Get().CreateFertilityModifier(id, eggTag, name, desc, (Func<string, string>) (descStr =>
          {
            string str = string.Join(", ", ((IEnumerable<Tag>) foodTags).Select<Tag, string>((Func<Tag, string>) (t => t.ProperName())).ToArray<string>());
            descStr = string.Format(descStr, (object) str);
            return descStr;
          }), (FertilityModifier.FertilityModFn) ((inst, eggType) =>
          {
            // ISSUE: variable of a compiler-generated type
            CREATURES.EGG_CHANCE_MODIFIERS.\u003C\u003Ec__DisplayClass0_0 cDisplayClass00 = this;
            FertilityMonitor.Instance inst2 = inst;
            Tag eggType2 = eggType;
            KMonoBehaviourExtensions.Subscribe(inst2.gameObject, -2038961714, (Action<object>) (data =>
            {
              CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = (CreatureCalorieMonitor.CaloriesConsumedEvent) data;
              // ISSUE: reference to a compiler-generated field
              if (!cDisplayClass00.foodTags.Contains(caloriesConsumedEvent.tag))
                return;
              // ISSUE: reference to a compiler-generated field
              inst2.AddBreedingChance(eggType2, caloriesConsumedEvent.calories * cDisplayClass00.modifierPerCal);
            }));
          }));
        });
      }

      private static System.Action CreateDietaryModifier(
        string id,
        Tag eggTag,
        Tag foodTag,
        float modifierPerCal)
      {
        string id1 = id;
        Tag eggTag1 = eggTag;
        HashSet<Tag> foodTags = new HashSet<Tag>();
        foodTags.Add(foodTag);
        double modifierPerCal1 = (double) modifierPerCal;
        return CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier(id1, eggTag1, foodTags, (float) modifierPerCal1);
      }

      private static System.Action CreateNearbyCreatureModifier(
        string id,
        Tag eggTag,
        Tag nearbyCreature,
        float modifierPerSecond,
        bool alsoInvert)
      {
        return (System.Action) (() =>
        {
          string name = (string) ((double) modifierPerSecond < 0.0 ? STRINGS.CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE_NEG.NAME : STRINGS.CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE.NAME);
          string description = (string) ((double) modifierPerSecond < 0.0 ? STRINGS.CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE_NEG.DESC : STRINGS.CREATURES.FERTILITY_MODIFIERS.NEARBY_CREATURE.DESC);
          Db.Get().CreateFertilityModifier(id, eggTag, name, description, (Func<string, string>) (descStr => string.Format(descStr, (object) nearbyCreature.ProperName())), (FertilityModifier.FertilityModFn) ((inst, eggType) =>
          {
            // ISSUE: variable of a compiler-generated type
            CREATURES.EGG_CHANCE_MODIFIERS.\u003C\u003Ec__DisplayClass2_0 cDisplayClass20 = this;
            FertilityMonitor.Instance inst2 = inst;
            Tag eggType2 = eggType;
            NearbyCreatureMonitor.Instance instance = inst2.gameObject.GetSMI<NearbyCreatureMonitor.Instance>();
            if (instance == null)
            {
              instance = new NearbyCreatureMonitor.Instance(inst2.master);
              instance.StartSM();
            }
            instance.OnUpdateNearbyCreatures += (Action<float, List<KPrefabID>>) ((dt, creatures) =>
            {
              bool flag = false;
              foreach (KPrefabID creature in creatures)
              {
                // ISSUE: reference to a compiler-generated field
                if (Tag.op_Equality(creature.PrefabTag, cDisplayClass20.nearbyCreature))
                {
                  flag = true;
                  break;
                }
              }
              if (flag)
              {
                // ISSUE: reference to a compiler-generated field
                inst2.AddBreedingChance(eggType2, dt * cDisplayClass20.modifierPerSecond);
              }
              else
              {
                // ISSUE: reference to a compiler-generated field
                if (!cDisplayClass20.alsoInvert)
                  return;
                // ISSUE: reference to a compiler-generated field
                inst2.AddBreedingChance(eggType2, dt * -cDisplayClass20.modifierPerSecond);
              }
            });
          }));
        });
      }

      private static System.Action CreateElementCreatureModifier(
        string id,
        Tag eggTag,
        Tag element,
        float modifierPerSecond,
        bool alsoInvert,
        bool checkSubstantialLiquid,
        string tooltipOverride = null)
      {
        return (System.Action) (() =>
        {
          string name = (string) STRINGS.CREATURES.FERTILITY_MODIFIERS.LIVING_IN_ELEMENT.NAME;
          string desc = (string) STRINGS.CREATURES.FERTILITY_MODIFIERS.LIVING_IN_ELEMENT.DESC;
          Db.Get().CreateFertilityModifier(id, eggTag, name, desc, (Func<string, string>) (descStr => tooltipOverride == null ? string.Format(descStr, (object) ElementLoader.GetElement(element).name) : tooltipOverride), (FertilityModifier.FertilityModFn) ((inst, eggType) =>
          {
            // ISSUE: variable of a compiler-generated type
            CREATURES.EGG_CHANCE_MODIFIERS.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30 = this;
            FertilityMonitor.Instance inst2 = inst;
            Tag eggType2 = eggType;
            CritterElementMonitor.Instance instance = inst2.gameObject.GetSMI<CritterElementMonitor.Instance>();
            if (instance == null)
            {
              instance = new CritterElementMonitor.Instance(inst2.master);
              instance.StartSM();
            }
            instance.OnUpdateEggChances += (Action<float>) (dt =>
            {
              int cell = Grid.PosToCell((StateMachine.Instance) inst2);
              if (!Grid.IsValidCell(cell))
                return;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              if (Grid.Element[cell].HasTag(cDisplayClass30.element) && (!cDisplayClass30.checkSubstantialLiquid || Grid.IsSubstantialLiquid(cell)))
              {
                // ISSUE: reference to a compiler-generated field
                inst2.AddBreedingChance(eggType2, dt * cDisplayClass30.modifierPerSecond);
              }
              else
              {
                // ISSUE: reference to a compiler-generated field
                if (!cDisplayClass30.alsoInvert)
                  return;
                // ISSUE: reference to a compiler-generated field
                inst2.AddBreedingChance(eggType2, dt * -cDisplayClass30.modifierPerSecond);
              }
            });
          }));
        });
      }

      private static System.Action CreateCropTendedModifier(
        string id,
        Tag eggTag,
        HashSet<Tag> cropTags,
        float modifierPerEvent)
      {
        return (System.Action) (() =>
        {
          string name = (string) STRINGS.CREATURES.FERTILITY_MODIFIERS.CROPTENDING.NAME;
          string desc = (string) STRINGS.CREATURES.FERTILITY_MODIFIERS.CROPTENDING.DESC;
          Db.Get().CreateFertilityModifier(id, eggTag, name, desc, (Func<string, string>) (descStr =>
          {
            string str = string.Join(", ", ((IEnumerable<Tag>) cropTags).Select<Tag, string>((Func<Tag, string>) (t => t.ProperName())).ToArray<string>());
            descStr = string.Format(descStr, (object) str);
            return descStr;
          }), (FertilityModifier.FertilityModFn) ((inst, eggType) =>
          {
            // ISSUE: variable of a compiler-generated type
            CREATURES.EGG_CHANCE_MODIFIERS.\u003C\u003Ec__DisplayClass4_0 cDisplayClass40 = this;
            FertilityMonitor.Instance inst2 = inst;
            Tag eggType2 = eggType;
            KMonoBehaviourExtensions.Subscribe(inst2.gameObject, 90606262, (Action<object>) (data =>
            {
              // ISSUE: reference to a compiler-generated field
              if (!cDisplayClass40.cropTags.Contains(((CropTendingStates.CropTendingEventData) data).cropId))
                return;
              // ISSUE: reference to a compiler-generated field
              inst2.AddBreedingChance(eggType2, cDisplayClass40.modifierPerEvent);
            }));
          }));
        });
      }

      private static System.Action CreateTemperatureModifier(
        string id,
        Tag eggTag,
        float minTemp,
        float maxTemp,
        float modifierPerSecond,
        bool alsoInvert)
      {
        return (System.Action) (() =>
        {
          string name = (string) STRINGS.CREATURES.FERTILITY_MODIFIERS.TEMPERATURE.NAME;
          Db.Get().CreateFertilityModifier(id, eggTag, name, (string) null, (Func<string, string>) (src => string.Format((string) STRINGS.CREATURES.FERTILITY_MODIFIERS.TEMPERATURE.DESC, (object) GameUtil.GetFormattedTemperature(minTemp), (object) GameUtil.GetFormattedTemperature(maxTemp))), (FertilityModifier.FertilityModFn) ((inst, eggType) =>
          {
            // ISSUE: variable of a compiler-generated type
            CREATURES.EGG_CHANCE_MODIFIERS.\u003C\u003Ec__DisplayClass5_0 cDisplayClass50 = this;
            FertilityMonitor.Instance inst2 = inst;
            Tag eggType2 = eggType;
            TemperatureVulnerable component = inst2.master.GetComponent<TemperatureVulnerable>();
            if (Object.op_Inequality((Object) component, (Object) null))
              component.OnTemperature += (Action<float, float>) ((dt, newTemp) =>
              {
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                if ((double) newTemp > (double) cDisplayClass50.minTemp && (double) newTemp < (double) cDisplayClass50.maxTemp)
                {
                  // ISSUE: reference to a compiler-generated field
                  inst2.AddBreedingChance(eggType2, dt * cDisplayClass50.modifierPerSecond);
                }
                else
                {
                  // ISSUE: reference to a compiler-generated field
                  if (!cDisplayClass50.alsoInvert)
                    return;
                  // ISSUE: reference to a compiler-generated field
                  inst2.AddBreedingChance(eggType2, dt * -cDisplayClass50.modifierPerSecond);
                }
              });
            else
              DebugUtil.LogErrorArgs(new object[5]
              {
                (object) "Ack! Trying to add temperature modifier",
                (object) id,
                (object) "to",
                (object) inst2.master.name,
                (object) "but it's not temperature vulnerable!"
              });
          }));
        });
      }

      static EGG_CHANCE_MODIFIERS()
      {
        List<System.Action> actionList = new List<System.Action>();
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("HatchHard", TagExtensions.ToTag("HatchHardEgg"), SimHashes.SedimentaryRock.CreateTag(), 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("HatchVeggie", TagExtensions.ToTag("HatchVeggieEgg"), SimHashes.Dirt.CreateTag(), 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("HatchMetal", TagExtensions.ToTag("HatchMetalEgg"), HatchMetalConfig.METAL_ORE_TAGS, 0.05f / HatchTuning.STANDARD_CALORIES_PER_CYCLE));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateNearbyCreatureModifier("PuftAlphaBalance", TagExtensions.ToTag("PuftAlphaEgg"), TagExtensions.ToTag("PuftAlpha"), -0.00025f, true));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateNearbyCreatureModifier("PuftAlphaNearbyOxylite", TagExtensions.ToTag("PuftOxyliteEgg"), TagExtensions.ToTag("PuftAlpha"), 8.333333E-05f, false));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateNearbyCreatureModifier("PuftAlphaNearbyBleachstone", TagExtensions.ToTag("PuftBleachstoneEgg"), TagExtensions.ToTag("PuftAlpha"), 8.333333E-05f, false));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateTemperatureModifier("OilFloaterHighTemp", TagExtensions.ToTag("OilfloaterHighTempEgg"), 373.15f, 523.15f, 8.333333E-05f, false));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateTemperatureModifier("OilFloaterDecor", TagExtensions.ToTag("OilfloaterDecorEgg"), 293.15f, 333.15f, 8.333333E-05f, false));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugOrange", TagExtensions.ToTag("LightBugOrangeEgg"), TagExtensions.ToTag("GrilledPrickleFruit"), 1f / 800f));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugPurple", TagExtensions.ToTag("LightBugPurpleEgg"), TagExtensions.ToTag("FriedMushroom"), 1f / 800f));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugPink", TagExtensions.ToTag("LightBugPinkEgg"), TagExtensions.ToTag("SpiceBread"), 1f / 800f));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugBlue", TagExtensions.ToTag("LightBugBlueEgg"), TagExtensions.ToTag("Salsa"), 1f / 800f));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugBlack", TagExtensions.ToTag("LightBugBlackEgg"), SimHashes.Phosphorus.CreateTag(), 1f / 800f));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("LightBugCrystal", TagExtensions.ToTag("LightBugCrystalEgg"), TagExtensions.ToTag("CookedMeat"), 1f / 800f));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateTemperatureModifier("PacuTropical", TagExtensions.ToTag("PacuTropicalEgg"), 308.15f, 353.15f, 8.333333E-05f, false));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateTemperatureModifier("PacuCleaner", TagExtensions.ToTag("PacuCleanerEgg"), 243.15f, 278.15f, 8.333333E-05f, false));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("DreckoPlastic", TagExtensions.ToTag("DreckoPlasticEgg"), TagExtensions.ToTag("BasicSingleHarvestPlant"), 0.025f / DreckoTuning.STANDARD_CALORIES_PER_CYCLE));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateDietaryModifier("SquirrelHug", TagExtensions.ToTag("SquirrelHugEgg"), TagExtensions.ToTag(BasicFabricMaterialPlantConfig.ID), 0.025f / SquirrelTuning.STANDARD_CALORIES_PER_CYCLE));
        Tag tag = TagExtensions.ToTag("DivergentWormEgg");
        HashSet<Tag> cropTags = new HashSet<Tag>();
        cropTags.Add(TagExtensions.ToTag("WormPlant"));
        cropTags.Add(TagExtensions.ToTag("SuperWormPlant"));
        double modifierPerEvent = 0.05000000074505806 / (double) DivergentTuning.TIMES_TENDED_PER_CYCLE_FOR_EVOLUTION;
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateCropTendedModifier("DivergentWorm", tag, cropTags, (float) modifierPerEvent));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateElementCreatureModifier("PokeLumber", TagExtensions.ToTag("CrabWoodEgg"), SimHashes.Ethanol.CreateTag(), 0.00025f, true, true));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateElementCreatureModifier("PokeFreshWater", TagExtensions.ToTag("CrabFreshWaterEgg"), SimHashes.Water.CreateTag(), 0.00025f, true, true));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateTemperatureModifier("MoleDelicacy", TagExtensions.ToTag("MoleDelicacyEgg"), MoleDelicacyConfig.EGG_CHANCES_TEMPERATURE_MIN, MoleDelicacyConfig.EGG_CHANCES_TEMPERATURE_MAX, 8.333333E-05f, false));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateElementCreatureModifier("StaterpillarGas", TagExtensions.ToTag("StaterpillarGasEgg"), GameTags.Unbreathable, 0.00025f, true, false, (string) STRINGS.CREATURES.FERTILITY_MODIFIERS.LIVING_IN_ELEMENT.UNBREATHABLE));
        actionList.Add(CREATURES.EGG_CHANCE_MODIFIERS.CreateElementCreatureModifier("StaterpillarLiquid", TagExtensions.ToTag("StaterpillarLiquidEgg"), GameTags.Liquid, 0.00025f, true, false, (string) STRINGS.CREATURES.FERTILITY_MODIFIERS.LIVING_IN_ELEMENT.LIQUID));
        CREATURES.EGG_CHANCE_MODIFIERS.MODIFIER_CREATORS = actionList;
      }
    }
  }
}
