// Decompiled with JetBrains decompiler
// Type: CreatureCalorieMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureCalorieMonitor : 
  GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>
{
  public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State normal;
  private CreatureCalorieMonitor.HungryStates hungry;
  private Effect outOfCaloriesTame;
  public StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.FloatParameter starvationStartTime;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.normal;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.root.EventHandler(GameHashes.CaloriesConsumed, (GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.GameEvent.Callback) ((smi, data) => smi.OnCaloriesConsumed(data))).ToggleBehaviour(GameTags.Creatures.Poop, new StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.Transition.ConditionCallback(CreatureCalorieMonitor.ReadyToPoop), (System.Action<CreatureCalorieMonitor.Instance>) (smi => smi.Poop())).Update(new System.Action<CreatureCalorieMonitor.Instance, float>(CreatureCalorieMonitor.UpdateMetabolismCalorieModifier));
    this.normal.Transition((GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State) this.hungry, (StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.Transition.ConditionCallback) (smi => smi.IsHungry()), (UpdateRate) 6);
    this.hungry.DefaultState(this.hungry.hungry).ToggleTag(GameTags.Creatures.Hungry).EventTransition(GameHashes.CaloriesConsumed, this.normal, (StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.Transition.ConditionCallback) (smi => !smi.IsHungry()));
    this.hungry.hungry.Transition(this.normal, (StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.Transition.ConditionCallback) (smi => !smi.IsHungry()), (UpdateRate) 6).Transition((GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State) this.hungry.outofcalories, (StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.Transition.ConditionCallback) (smi => smi.IsOutOfCalories()), (UpdateRate) 6).ToggleStatusItem(Db.Get().CreatureStatusItems.Hungry);
    this.hungry.outofcalories.DefaultState(this.hungry.outofcalories.wild).Transition(this.hungry.hungry, (StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.Transition.ConditionCallback) (smi => !smi.IsOutOfCalories()), (UpdateRate) 6);
    this.hungry.outofcalories.wild.TagTransition(GameTags.Creatures.Wild, this.hungry.outofcalories.tame, true).ToggleStatusItem(Db.Get().CreatureStatusItems.Hungry);
    double num;
    this.hungry.outofcalories.tame.Enter("StarvationStartTime", new StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State.Callback(CreatureCalorieMonitor.StarvationStartTime)).Exit("ClearStarvationTime", (StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State.Callback) (smi => num = (double) this.starvationStartTime.Set(0.0f, smi))).Transition(this.hungry.outofcalories.starvedtodeath, (StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.Transition.ConditionCallback) (smi => (double) smi.GetDeathTimeRemaining() <= 0.0), (UpdateRate) 6).TagTransition(GameTags.Creatures.Wild, this.hungry.outofcalories.wild).ToggleStatusItem((string) STRINGS.CREATURES.STATUSITEMS.STARVING.NAME, (string) STRINGS.CREATURES.STATUSITEMS.STARVING.TOOLTIP, notification_type: NotificationType.BadMinor, render_overlay: new HashedString(), resolve_string_callback: ((Func<string, CreatureCalorieMonitor.Instance, string>) ((str, smi) => str.Replace("{TimeUntilDeath}", GameUtil.GetFormattedCycles(smi.GetDeathTimeRemaining()))))).ToggleNotification((Func<CreatureCalorieMonitor.Instance, Notification>) (smi => new Notification((string) STRINGS.CREATURES.STATUSITEMS.STARVING.NOTIFICATION_NAME, NotificationType.BadMinor, (Func<List<Notification>, object, string>) ((notifications, data) => (string) STRINGS.CREATURES.STATUSITEMS.STARVING.NOTIFICATION_TOOLTIP + notifications.ReduceMessages(false))))).ToggleEffect((Func<CreatureCalorieMonitor.Instance, Effect>) (smi => this.outOfCaloriesTame));
    this.hungry.outofcalories.starvedtodeath.Enter((StateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State.Callback) (smi => smi.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Starvation)));
    this.outOfCaloriesTame = new Effect("OutOfCaloriesTame", (string) STRINGS.CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME, (string) STRINGS.CREATURES.MODIFIERS.OUT_OF_CALORIES.TOOLTIP, 0.0f, false, false, false);
    this.outOfCaloriesTame.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, -10f, (string) STRINGS.CREATURES.MODIFIERS.OUT_OF_CALORIES.NAME));
  }

  private static bool ReadyToPoop(CreatureCalorieMonitor.Instance smi) => smi.stomach.IsReadyToPoop() && (double) Time.time - (double) smi.lastMealOrPoopTime >= (double) smi.def.minimumTimeBeforePooping;

  private static void UpdateMetabolismCalorieModifier(CreatureCalorieMonitor.Instance smi, float dt) => smi.deltaCalorieMetabolismModifier.SetValue((float) (1.0 - (double) smi.metabolism.GetTotalValue() / 100.0));

  private static void StarvationStartTime(CreatureCalorieMonitor.Instance smi)
  {
    if ((double) smi.sm.starvationStartTime.Get(smi) != 0.0)
      return;
    double num = (double) smi.sm.starvationStartTime.Set(GameClock.Instance.GetTime(), smi);
  }

  public struct CaloriesConsumedEvent
  {
    public Tag tag;
    public float calories;
  }

  public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
  {
    public Diet diet;
    public float minPoopSizeInCalories = 100f;
    public float minimumTimeBeforePooping = 10f;
    public float deathTimer = 6000f;
    public bool storePoop;

    public override void Configure(GameObject prefab) => prefab.GetComponent<Modifiers>().initialAmounts.Add(Db.Get().Amounts.Calories.Id);

    public List<Descriptor> GetDescriptors(GameObject obj)
    {
      List<Descriptor> descriptors = new List<Descriptor>();
      descriptors.Add(new Descriptor((string) UI.BUILDINGEFFECTS.DIET_HEADER, (string) UI.BUILDINGEFFECTS.TOOLTIPS.DIET_HEADER, (Descriptor.DescriptorType) 1, false));
      float dailyPlantGrowthConsumption = 1f;
      if (this.diet.consumedTags.Count > 0)
      {
        float calorie_loss_per_second = 0.0f;
        foreach (AttributeModifier selfModifier in Db.Get().traits.Get(obj.GetComponent<Modifiers>().initialTraits[0]).SelfModifiers)
        {
          if (selfModifier.AttributeId == Db.Get().Amounts.Calories.deltaAttribute.Id)
            calorie_loss_per_second = selfModifier.Value;
        }
        string newValue1 = string.Join(", ", ((IEnumerable<KeyValuePair<Tag, float>>) this.diet.consumedTags).Select<KeyValuePair<Tag, float>, string>((Func<KeyValuePair<Tag, float>, string>) (t => t.Key.ProperName())).ToArray<string>());
        string newValue2 = !this.diet.eatsPlantsDirectly ? string.Join("\n", ((IEnumerable<KeyValuePair<Tag, float>>) this.diet.consumedTags).Select<KeyValuePair<Tag, float>, string>((Func<KeyValuePair<Tag, float>, string>) (t => UI.BUILDINGEFFECTS.DIET_CONSUMED_ITEM.text.Replace("{Food}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(-calorie_loss_per_second / t.Value, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram)))).ToArray<string>()) : string.Join("\n", ((IEnumerable<KeyValuePair<Tag, float>>) this.diet.consumedTags).Select<KeyValuePair<Tag, float>, string>((Func<KeyValuePair<Tag, float>, string>) (t =>
        {
          dailyPlantGrowthConsumption = -calorie_loss_per_second / t.Value;
          Crop crop = Assets.GetPrefab(Tag.op_Implicit(t.Key.ToString())).GetComponent<Crop>();
          float num = 1f / (TUNING.CROPS.CROP_TYPES.Find((Predicate<Crop.CropVal>) (m => m.cropId == crop.cropId)).cropDuration / 600f);
          return UI.BUILDINGEFFECTS.DIET_CONSUMED_ITEM.text.Replace("{Food}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedPlantGrowth((float) (-(double) calorie_loss_per_second / (double) t.Value * (double) num * 100.0), GameUtil.TimeSlice.PerCycle));
        })).ToArray<string>());
        descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_CONSUMED.text.Replace("{Foodlist}", newValue1), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_CONSUMED.text.Replace("{Foodlist}", newValue2), (Descriptor.DescriptorType) 1, false));
      }
      if (this.diet.producedTags.Count > 0)
      {
        string newValue3 = string.Join(", ", ((IEnumerable<KeyValuePair<Tag, float>>) this.diet.producedTags).Select<KeyValuePair<Tag, float>, string>((Func<KeyValuePair<Tag, float>, string>) (t => t.Key.ProperName())).ToArray<string>());
        string newValue4 = !this.diet.eatsPlantsDirectly ? string.Join("\n", ((IEnumerable<KeyValuePair<Tag, float>>) this.diet.producedTags).Select<KeyValuePair<Tag, float>, string>((Func<KeyValuePair<Tag, float>, string>) (t => UI.BUILDINGEFFECTS.DIET_PRODUCED_ITEM.text.Replace("{Item}", t.Key.ProperName()).Replace("{Percent}", GameUtil.GetFormattedPercent(t.Value * 100f)))).ToArray<string>()) : string.Join("\n", ((IEnumerable<KeyValuePair<Tag, float>>) this.diet.producedTags).Select<KeyValuePair<Tag, float>, string>((Func<KeyValuePair<Tag, float>, string>) (t => UI.BUILDINGEFFECTS.DIET_PRODUCED_ITEM_FROM_PLANT.text.Replace("{Item}", t.Key.ProperName()).Replace("{Amount}", GameUtil.GetFormattedMass(t.Value * dailyPlantGrowthConsumption, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.Kilogram)))).ToArray<string>());
        descriptors.Add(new Descriptor(UI.BUILDINGEFFECTS.DIET_PRODUCED.text.Replace("{Items}", newValue3), UI.BUILDINGEFFECTS.TOOLTIPS.DIET_PRODUCED.text.Replace("{Items}", newValue4), (Descriptor.DescriptorType) 1, false));
      }
      return descriptors;
    }
  }

  public class HungryStates : 
    GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State
  {
    public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State hungry;
    public CreatureCalorieMonitor.HungryStates.OutOfCaloriesState outofcalories;

    public class OutOfCaloriesState : 
      GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State
    {
      public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State wild;
      public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State tame;
      public GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.State starvedtodeath;
    }
  }

  [SerializationConfig]
  public class Stomach
  {
    [Serialize]
    private List<CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry> caloriesConsumed = new List<CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry>();
    private float minPoopSizeInCalories;
    private GameObject owner;
    private bool storePoop;

    public Diet diet { get; private set; }

    public Stomach(Diet diet, GameObject owner, float min_poop_size_in_calories, bool storePoop)
    {
      this.diet = diet;
      this.owner = owner;
      this.minPoopSizeInCalories = min_poop_size_in_calories;
      this.storePoop = storePoop;
    }

    public void Poop()
    {
      float num1 = 0.0f;
      Tag tag = Tag.Invalid;
      byte disease_idx = byte.MaxValue;
      int num2 = 0;
      bool flag = false;
      for (int index = 0; index < this.caloriesConsumed.Count; ++index)
      {
        CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry = this.caloriesConsumed[index];
        if ((double) caloriesConsumedEntry.calories > 0.0)
        {
          Diet.Info dietInfo = this.diet.GetDietInfo(caloriesConsumedEntry.tag);
          if (dietInfo != null && (!Tag.op_Inequality(tag, Tag.Invalid) || !Tag.op_Inequality(tag, dietInfo.producedElement)))
          {
            num1 += dietInfo.ConvertConsumptionMassToProducedMass(dietInfo.ConvertCaloriesToConsumptionMass(caloriesConsumedEntry.calories));
            tag = dietInfo.producedElement;
            disease_idx = dietInfo.diseaseIdx;
            num2 = (int) ((double) dietInfo.diseasePerKgProduced * (double) num1);
            caloriesConsumedEntry.calories = 0.0f;
            this.caloriesConsumed[index] = caloriesConsumedEntry;
            flag = flag || dietInfo.produceSolidTile;
          }
        }
      }
      if ((double) num1 <= 0.0 || Tag.op_Equality(tag, Tag.Invalid))
        return;
      Element element = ElementLoader.GetElement(tag);
      Debug.Assert(element != null, (object) "TODO: implement non-element tag spawning");
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.owner.transform));
      float temperature = this.owner.GetComponent<PrimaryElement>().Temperature;
      DebugUtil.DevAssert(!(this.storePoop & flag), "Stomach cannot both store poop & create a solid tile.", (Object) null);
      if (this.storePoop)
      {
        Storage component = this.owner.GetComponent<Storage>();
        if (element.IsLiquid)
          component.AddLiquid(element.id, num1, temperature, disease_idx, num2);
        else if (element.IsGas)
          component.AddGasChunk(element.id, num1, temperature, disease_idx, num2, false);
        else
          component.AddOre(element.id, num1, temperature, disease_idx, num2);
      }
      else if (element.IsLiquid)
        FallingWater.instance.AddParticle(cell, element.idx, num1, temperature, disease_idx, num2, true);
      else if (element.IsGas)
        SimMessages.AddRemoveSubstance(cell, element.idx, CellEventLogger.Instance.ElementConsumerSimUpdate, num1, temperature, disease_idx, num2);
      else if (flag)
      {
        int num3 = this.owner.GetComponent<Facing>().GetFrontCell();
        if (!Grid.IsValidCell(num3))
        {
          Debug.LogWarningFormat("{0} attemping to Poop {1} on invalid cell {2} from cell {3}", new object[4]
          {
            (object) this.owner,
            (object) element.name,
            (object) num3,
            (object) cell
          });
          num3 = cell;
        }
        SimMessages.AddRemoveSubstance(num3, element.idx, CellEventLogger.Instance.ElementConsumerSimUpdate, num1, temperature, disease_idx, num2);
      }
      else
        element.substance.SpawnResource(Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore), num1, temperature, disease_idx, num2);
      KPrefabID component1 = this.owner.GetComponent<KPrefabID>();
      if (!Game.Instance.savedInfo.creaturePoopAmount.ContainsKey(component1.PrefabTag))
        Game.Instance.savedInfo.creaturePoopAmount.Add(component1.PrefabTag, 0.0f);
      Game.Instance.savedInfo.creaturePoopAmount[component1.PrefabTag] += num1;
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, element.name, this.owner.transform);
    }

    public List<CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry> GetCalorieEntries() => this.caloriesConsumed;

    public float GetTotalConsumedCalories()
    {
      float consumedCalories = 0.0f;
      foreach (CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry in this.caloriesConsumed)
      {
        if ((double) caloriesConsumedEntry.calories > 0.0)
        {
          Diet.Info dietInfo = this.diet.GetDietInfo(caloriesConsumedEntry.tag);
          if (dietInfo != null && !Tag.op_Equality(dietInfo.producedElement, Tag.Invalid))
            consumedCalories += caloriesConsumedEntry.calories;
        }
      }
      return consumedCalories;
    }

    public float GetFullness() => this.GetTotalConsumedCalories() / this.minPoopSizeInCalories;

    public bool IsReadyToPoop()
    {
      float consumedCalories = this.GetTotalConsumedCalories();
      return (double) consumedCalories > 0.0 && (double) consumedCalories >= (double) this.minPoopSizeInCalories;
    }

    public void Consume(Tag tag, float calories)
    {
      for (int index = 0; index < this.caloriesConsumed.Count; ++index)
      {
        CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry = this.caloriesConsumed[index];
        if (Tag.op_Equality(caloriesConsumedEntry.tag, tag))
        {
          caloriesConsumedEntry.calories += calories;
          this.caloriesConsumed[index] = caloriesConsumedEntry;
          return;
        }
      }
      this.caloriesConsumed.Add(new CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry()
      {
        tag = tag,
        calories = calories
      });
    }

    public Tag GetNextPoopEntry()
    {
      for (int index = 0; index < this.caloriesConsumed.Count; ++index)
      {
        CreatureCalorieMonitor.Stomach.CaloriesConsumedEntry caloriesConsumedEntry = this.caloriesConsumed[index];
        if ((double) caloriesConsumedEntry.calories > 0.0)
        {
          Diet.Info dietInfo = this.diet.GetDietInfo(caloriesConsumedEntry.tag);
          if (dietInfo != null && !Tag.op_Equality(dietInfo.producedElement, Tag.Invalid))
            return dietInfo.producedElement;
        }
      }
      return Tag.Invalid;
    }

    [Serializable]
    public struct CaloriesConsumedEntry
    {
      public Tag tag;
      public float calories;
    }
  }

  public new class Instance : 
    GameStateMachine<CreatureCalorieMonitor, CreatureCalorieMonitor.Instance, IStateMachineTarget, CreatureCalorieMonitor.Def>.GameInstance
  {
    public const float HUNGRY_RATIO = 0.9f;
    public AmountInstance calories;
    [Serialize]
    public CreatureCalorieMonitor.Stomach stomach;
    public float lastMealOrPoopTime;
    public AttributeInstance metabolism;
    public AttributeModifier deltaCalorieMetabolismModifier;

    public Instance(IStateMachineTarget master, CreatureCalorieMonitor.Def def)
      : base(master, def)
    {
      this.calories = Db.Get().Amounts.Calories.Lookup(this.gameObject);
      this.calories.value = this.calories.GetMax() * 0.9f;
      this.stomach = new CreatureCalorieMonitor.Stomach(def.diet, master.gameObject, def.minPoopSizeInCalories, def.storePoop);
      this.metabolism = this.gameObject.GetAttributes().Add(Db.Get().CritterAttributes.Metabolism);
      this.deltaCalorieMetabolismModifier = new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, 1f, (string) DUPLICANTS.MODIFIERS.METABOLISM_CALORIE_MODIFIER.NAME, true, is_readonly: false);
      this.calories.deltaAttribute.Add(this.deltaCalorieMetabolismModifier);
    }

    public void OnCaloriesConsumed(object data)
    {
      CreatureCalorieMonitor.CaloriesConsumedEvent caloriesConsumedEvent = (CreatureCalorieMonitor.CaloriesConsumedEvent) data;
      this.calories.value += caloriesConsumedEvent.calories;
      this.stomach.Consume(caloriesConsumedEvent.tag, caloriesConsumedEvent.calories);
      this.lastMealOrPoopTime = Time.time;
    }

    public float GetDeathTimeRemaining() => this.smi.def.deathTimer - (GameClock.Instance.GetTime() - this.sm.starvationStartTime.Get(this.smi));

    public void Poop()
    {
      this.lastMealOrPoopTime = Time.time;
      this.stomach.Poop();
    }

    public float GetCalories0to1() => this.calories.value / this.calories.GetMax();

    public bool IsHungry() => (double) this.GetCalories0to1() < 0.89999997615814209;

    public bool IsOutOfCalories() => (double) this.GetCalories0to1() <= 0.0;
  }
}
