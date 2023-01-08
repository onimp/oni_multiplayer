// Decompiled with JetBrains decompiler
// Type: Edible
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Edible")]
public class Edible : Workable, IGameObjectEffectDescriptor, ISaveLoadable, IExtendSplitting
{
  private PrimaryElement primaryElement;
  public string FoodID;
  private EdiblesManager.FoodInfo foodInfo;
  public float unitsConsumed = float.NaN;
  public float caloriesConsumed = float.NaN;
  private float totalFeedingTime = float.NaN;
  private float totalUnits = float.NaN;
  private float totalConsumableCalories = float.NaN;
  [Serialize]
  private List<SpiceInstance> spices = new List<SpiceInstance>();
  private AttributeModifier caloriesModifier = new AttributeModifier("CaloriesDelta", 50000f, (string) DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, uiOnly: true);
  private AttributeModifier caloriesLitSpaceModifier = new AttributeModifier("CaloriesDelta", 57500f, (string) DUPLICANTS.MODIFIERS.EATINGCALORIES.NAME, uiOnly: true);
  private AttributeModifier currentModifier;
  private static readonly EventSystem.IntraObjectHandler<Edible> OnCraftDelegate = new EventSystem.IntraObjectHandler<Edible>((Action<Edible, object>) ((component, data) => component.OnCraft(data)));
  private static readonly HashedString[] normalWorkAnims = new HashedString[2]
  {
    HashedString.op_Implicit("working_pre"),
    HashedString.op_Implicit("working_loop")
  };
  private static readonly HashedString[] hatWorkAnims = new HashedString[2]
  {
    HashedString.op_Implicit("hat_pre"),
    HashedString.op_Implicit("working_loop")
  };
  private static readonly HashedString[] saltWorkAnims = new HashedString[2]
  {
    HashedString.op_Implicit("salt_pre"),
    HashedString.op_Implicit("salt_loop")
  };
  private static readonly HashedString[] saltHatWorkAnims = new HashedString[2]
  {
    HashedString.op_Implicit("salt_hat_pre"),
    HashedString.op_Implicit("salt_hat_loop")
  };
  private static readonly HashedString[] normalWorkPstAnim = new HashedString[1]
  {
    HashedString.op_Implicit("working_pst")
  };
  private static readonly HashedString[] hatWorkPstAnim = new HashedString[1]
  {
    HashedString.op_Implicit("hat_pst")
  };
  private static readonly HashedString[] saltWorkPstAnim = new HashedString[1]
  {
    HashedString.op_Implicit("salt_pst")
  };
  private static readonly HashedString[] saltHatWorkPstAnim = new HashedString[1]
  {
    HashedString.op_Implicit("salt_hat_pst")
  };
  private static Dictionary<int, string> qualityEffects = new Dictionary<int, string>()
  {
    {
      -1,
      "EdibleMinus3"
    },
    {
      0,
      "EdibleMinus2"
    },
    {
      1,
      "EdibleMinus1"
    },
    {
      2,
      "Edible0"
    },
    {
      3,
      "Edible1"
    },
    {
      4,
      "Edible2"
    },
    {
      5,
      "Edible3"
    }
  };

  public float Units
  {
    get => this.primaryElement.Units;
    set => this.primaryElement.Units = value;
  }

  public float MassPerUnit => this.primaryElement.MassPerUnit;

  public float Calories
  {
    get => this.Units * this.foodInfo.CaloriesPerUnit;
    set => this.Units = value / this.foodInfo.CaloriesPerUnit;
  }

  public EdiblesManager.FoodInfo FoodInfo
  {
    get => this.foodInfo;
    set
    {
      this.foodInfo = value;
      this.FoodID = this.foodInfo.Id;
    }
  }

  public bool isBeingConsumed { get; private set; }

  protected override void OnPrefabInit()
  {
    this.primaryElement = ((Component) this).GetComponent<PrimaryElement>();
    this.SetReportType(ReportManager.ReportType.PersonalTime);
    this.showProgressBar = false;
    this.SetOffsetTable(OffsetGroups.InvertedStandardTable);
    this.shouldTransferDiseaseWithWorker = false;
    base.OnPrefabInit();
    if (this.foodInfo == null)
    {
      if (this.FoodID == null)
        Debug.LogError((object) "No food FoodID");
      this.foodInfo = EdiblesManager.GetFoodInfo(this.FoodID);
    }
    this.Subscribe<Edible>(748399584, Edible.OnCraftDelegate);
    this.Subscribe<Edible>(1272413801, Edible.OnCraftDelegate);
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Eating;
    this.synchronizeAnims = false;
    Components.Edibles.Add(this);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.ToggleGenericSpicedTag(((Component) this).gameObject.HasTag(GameTags.SpicedFood));
    if (this.spices != null)
    {
      for (int index = 0; index < this.spices.Count; ++index)
        this.ApplySpiceEffects(this.spices[index], SpiceGrinderConfig.SpicedStatus);
    }
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().MiscStatusItems.Edible, (object) this);
  }

  public override HashedString[] GetWorkAnims(Worker worker)
  {
    EatChore.StatesInstance smi = ((Component) worker).GetSMI<EatChore.StatesInstance>();
    bool flag = smi != null && smi.UseSalt();
    MinionResume component = ((Component) worker).GetComponent<MinionResume>();
    return Object.op_Inequality((Object) component, (Object) null) && component.CurrentHat != null ? (!flag ? Edible.hatWorkAnims : Edible.saltHatWorkAnims) : (!flag ? Edible.normalWorkAnims : Edible.saltWorkAnims);
  }

  public override HashedString[] GetWorkPstAnims(Worker worker, bool successfully_completed)
  {
    EatChore.StatesInstance smi = ((Component) worker).GetSMI<EatChore.StatesInstance>();
    bool flag = smi != null && smi.UseSalt();
    MinionResume component = ((Component) worker).GetComponent<MinionResume>();
    return Object.op_Inequality((Object) component, (Object) null) && component.CurrentHat != null ? (!flag ? Edible.hatWorkPstAnim : Edible.saltHatWorkPstAnim) : (!flag ? Edible.normalWorkPstAnim : Edible.saltWorkPstAnim);
  }

  private void OnCraft(object data) => RationTracker.Get().RegisterCaloriesProduced(this.Calories);

  public float GetFeedingTime(Worker worker)
  {
    float feedingTime = this.Calories * 2E-05f;
    if (Object.op_Inequality((Object) worker, (Object) null))
    {
      BingeEatChore.StatesInstance smi = ((Component) worker).GetSMI<BingeEatChore.StatesInstance>();
      if (smi != null && smi.IsBingeEating())
        feedingTime /= 2f;
    }
    return feedingTime;
  }

  protected override void OnStartWork(Worker worker)
  {
    this.totalFeedingTime = this.GetFeedingTime(worker);
    this.SetWorkTime(this.totalFeedingTime);
    this.caloriesConsumed = 0.0f;
    this.unitsConsumed = 0.0f;
    this.totalUnits = this.Units;
    ((Component) worker).GetComponent<KPrefabID>().AddTag(GameTags.AlwaysConverse, false);
    this.totalConsumableCalories = this.Units * this.foodInfo.CaloriesPerUnit;
    this.StartConsuming();
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if (this.currentlyLit)
    {
      if (this.currentModifier != this.caloriesLitSpaceModifier)
      {
        worker.GetAttributes().Remove(this.currentModifier);
        worker.GetAttributes().Add(this.caloriesLitSpaceModifier);
        this.currentModifier = this.caloriesLitSpaceModifier;
      }
    }
    else if (this.currentModifier != this.caloriesModifier)
    {
      worker.GetAttributes().Remove(this.currentModifier);
      worker.GetAttributes().Add(this.caloriesModifier);
      this.currentModifier = this.caloriesModifier;
    }
    return this.OnTickConsume(worker, dt);
  }

  protected override void OnStopWork(Worker worker)
  {
    if (this.currentModifier != null)
    {
      worker.GetAttributes().Remove(this.currentModifier);
      this.currentModifier = (AttributeModifier) null;
    }
    ((Component) worker).GetComponent<KPrefabID>().RemoveTag(GameTags.AlwaysConverse);
    this.StopConsuming(worker);
  }

  private bool OnTickConsume(Worker worker, float dt)
  {
    if (!this.isBeingConsumed)
    {
      DebugUtil.DevLogError("OnTickConsume while we're not eating, this would set a NaN mass on this Edible");
      return true;
    }
    bool flag = false;
    float num1 = dt / this.totalFeedingTime;
    float num2 = num1 * this.totalConsumableCalories;
    if ((double) this.caloriesConsumed + (double) num2 > (double) this.totalConsumableCalories)
      num2 = this.totalConsumableCalories - this.caloriesConsumed;
    this.caloriesConsumed += num2;
    worker.GetAmounts().Get("Calories").value += num2;
    float num3 = this.totalUnits * num1;
    if ((double) this.Units - (double) num3 < 0.0)
      num3 = this.Units;
    this.Units -= num3;
    this.unitsConsumed += num3;
    if ((double) this.Units <= 0.0)
      flag = true;
    return flag;
  }

  public void SpiceEdible(SpiceInstance spice, StatusItem status)
  {
    this.spices.Add(spice);
    this.ApplySpiceEffects(spice, status);
  }

  protected virtual void ApplySpiceEffects(SpiceInstance spice, StatusItem status)
  {
    ((Component) this).GetComponent<KPrefabID>().AddTag(spice.Id, true);
    this.ToggleGenericSpicedTag(true);
    ((Component) this).GetComponent<KSelectable>().AddStatusItem(status, (object) this.spices);
    if (spice.FoodModifier != null)
      ((Component) this).gameObject.GetAttributes().Add(spice.FoodModifier);
    if (spice.CalorieModifier == null)
      return;
    this.Calories += spice.CalorieModifier.Value;
  }

  private void ToggleGenericSpicedTag(bool isSpiced)
  {
    KPrefabID component = ((Component) this).GetComponent<KPrefabID>();
    if (isSpiced)
    {
      component.RemoveTag(GameTags.UnspicedFood);
      component.AddTag(GameTags.SpicedFood, true);
    }
    else
    {
      component.RemoveTag(GameTags.SpicedFood);
      component.AddTag(GameTags.UnspicedFood, false);
    }
  }

  public bool CanAbsorb(Edible other)
  {
    bool flag = this.spices.Count == other.spices.Count;
    for (int index1 = 0; flag && index1 < this.spices.Count; ++index1)
    {
      for (int index2 = 0; flag && index2 < other.spices.Count; ++index2)
        flag = Tag.op_Equality(this.spices[index1].Id, other.spices[index2].Id);
    }
    return flag;
  }

  private void StartConsuming()
  {
    DebugUtil.DevAssert(!this.isBeingConsumed, "Can't StartConsuming()...we've already started", (Object) null);
    this.isBeingConsumed = true;
    this.worker.Trigger(1406130139, (object) this);
  }

  private void StopConsuming(Worker worker)
  {
    DebugUtil.DevAssert(this.isBeingConsumed, "StopConsuming() called without StartConsuming()", (Object) null);
    this.isBeingConsumed = false;
    if (Object.op_Inequality((Object) this.primaryElement, (Object) null) && this.primaryElement.DiseaseCount > 0)
    {
      EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) ((Component) worker).GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, Db.Get().Emotes.Minion.FoodPoisoning);
    }
    for (int index = 0; index < this.foodInfo.Effects.Count; ++index)
      ((Component) worker).GetComponent<Effects>().Add(this.foodInfo.Effects[index], true);
    ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, -this.caloriesConsumed, StringFormatter.Replace((string) UI.ENDOFDAYREPORT.NOTES.EATEN, "{0}", ((Component) this).GetProperName()), ((Component) worker).GetProperName());
    this.AddOnConsumeEffects(worker);
    worker.Trigger(1121894420, (object) this);
    this.Trigger(-10536414, (object) ((Component) worker).gameObject);
    this.unitsConsumed = float.NaN;
    this.caloriesConsumed = float.NaN;
    this.totalUnits = float.NaN;
    if ((double) this.Units > 0.0)
      return;
    TracesExtesions.DeleteObject(((Component) this).gameObject);
  }

  public static string GetEffectForFoodQuality(int qualityLevel)
  {
    qualityLevel = Mathf.Clamp(qualityLevel, -1, 5);
    return Edible.qualityEffects[qualityLevel];
  }

  private void AddOnConsumeEffects(Worker worker)
  {
    int qualityLevel = this.FoodInfo.Quality + Mathf.RoundToInt(worker.GetAttributes().Add(Db.Get().Attributes.FoodExpectation).GetTotalValue());
    Effects component = ((Component) worker).GetComponent<Effects>();
    component.Add(Edible.GetEffectForFoodQuality(qualityLevel), true);
    for (int index = 0; index < this.spices.Count; ++index)
    {
      Effect statBonus = this.spices[index].StatBonus;
      if (statBonus != null)
      {
        float duration = statBonus.duration;
        statBonus.duration = (float) ((double) this.caloriesConsumed * (1.0 / 1000.0) / 1000.0 * 600.0);
        component.Add(statBonus, true);
        statBonus.duration = duration;
      }
    }
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.Edibles.Remove(this);
  }

  public int GetQuality() => this.foodInfo.Quality;

  public override List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.CALORIES, (object) GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit)), string.Format((string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.CALORIES, (object) GameUtil.GetFormattedCalories(this.foodInfo.CaloriesPerUnit)), (Descriptor.DescriptorType) 3, false));
    descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, (object) GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality)), string.Format((string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.FOOD_QUALITY, (object) GameUtil.GetFormattedFoodQuality(this.foodInfo.Quality)), (Descriptor.DescriptorType) 1, false));
    foreach (string effect in this.foodInfo.Effects)
    {
      string str = "";
      foreach (AttributeModifier selfModifier in Db.Get().effects.Get(effect).SelfModifiers)
        str = str + "\n    • " + StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + selfModifier.AttributeId.ToUpper() + ".NAME")) + ": " + selfModifier.GetFormattedString();
      descriptors.Add(new Descriptor(StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + effect.ToUpper() + ".NAME")), StringEntry.op_Implicit(Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + effect.ToUpper() + ".DESCRIPTION")) + str, (Descriptor.DescriptorType) 1, false));
    }
    return descriptors;
  }

  public void OnSplitTick(Pickupable thePieceTaken)
  {
    Edible component = ((Component) thePieceTaken).GetComponent<Edible>();
    if (this.spices == null || !Object.op_Inequality((Object) component, (Object) null))
      return;
    for (int index = 0; index < this.spices.Count; ++index)
    {
      SpiceInstance spice = this.spices[index];
      component.SpiceEdible(spice, SpiceGrinderConfig.SpicedStatus);
    }
  }

  public class EdibleStartWorkInfo : Worker.StartWorkInfo
  {
    public float amount { get; private set; }

    public EdibleStartWorkInfo(Workable workable, float amount)
      : base(workable)
    {
      this.amount = amount;
    }
  }
}
