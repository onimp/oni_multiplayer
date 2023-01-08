// Decompiled with JetBrains decompiler
// Type: SpiceGrinder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpiceGrinder : 
  GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>
{
  public static Dictionary<Tag, SpiceGrinder.Option> SettingOptions = (Dictionary<Tag, SpiceGrinder.Option>) null;
  public static Operational.Flag spiceSet = new Operational.Flag(nameof (spiceSet), Operational.Flag.Type.Requirement);
  public GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State inoperational;
  public GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State operational;
  public GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State ready;
  public StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.BoolParameter isReady;

  public static void InitializeSpices()
  {
    Spices spices = Db.Get().Spices;
    SpiceGrinder.SettingOptions = new Dictionary<Tag, SpiceGrinder.Option>();
    for (int index = 0; index < ((ResourceSet) spices).Count; ++index)
    {
      Spice spice = spices[index];
      if (DlcManager.IsDlcListValidForCurrentContent(spice.DlcIds))
        SpiceGrinder.SettingOptions.Add(Tag.op_Implicit(spice.Id), new SpiceGrinder.Option(spice));
    }
  }

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.inoperational;
    this.root.Enter(new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State.Callback(this.OnEnterRoot)).EventHandler(GameHashes.OnStorageChange, new GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.GameEvent.Callback(this.OnStorageChanged));
    this.inoperational.EventTransition(GameHashes.OperationalChanged, this.ready, new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Transition.ConditionCallback(this.IsOperational)).Enter((StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State.Callback) (smi =>
    {
      smi.Play(smi.SelectedOption != null ? "off" : "default");
      if (smi.SelectedOption != null)
        return;
      smi.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSpiceSelected);
    })).Exit((StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.State.Callback) (smi => smi.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoSpiceSelected)));
    this.operational.EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Not(new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Transition.ConditionCallback(this.IsOperational))).ParamTransition<bool>((StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Parameter<bool>) this.isReady, this.ready, new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Parameter<bool>.Callback(this.GrinderIsReady)).PlayAnim("on");
    this.ready.EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Not(new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Transition.ConditionCallback(this.IsOperational))).ParamTransition<bool>((StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Parameter<bool>) this.isReady, this.operational, new StateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.Parameter<bool>.Callback(this.GrinderNoLongerReady)).ToggleRecurringChore(new Func<SpiceGrinder.StatesInstance, Chore>(this.CreateChore));
  }

  private void OnEnterRoot(SpiceGrinder.StatesInstance smi) => smi.Initialize();

  private bool GrinderIsReady(SpiceGrinder.StatesInstance smi, bool ready) => this.isReady.Get(smi);

  private bool GrinderNoLongerReady(SpiceGrinder.StatesInstance smi, bool ready) => !this.isReady.Get(smi);

  private bool IsOperational(SpiceGrinder.StatesInstance smi) => smi.IsOperational;

  private void OnStorageChanged(SpiceGrinder.StatesInstance smi, object data)
  {
    smi.UpdateMeter();
    if (smi.SelectedOption == null)
      return;
    bool flag = (double) smi.AvailableFood > 0.0 && smi.CanSpice(smi.CurrentFood.Calories);
    smi.sm.isReady.Set(flag, smi);
    smi.UpdateFoodSymbol();
  }

  private Chore CreateChore(SpiceGrinder.StatesInstance smi) => (Chore) new WorkChore<SpiceGrinderWorkable>(Db.Get().ChoreTypes.Cook, (IStateMachineTarget) smi.workable);

  public class Option : IConfigurableConsumerOption
  {
    public readonly Tag Id;
    public readonly Spice Spice;
    private string name;
    private string fullDescription;
    private string spiceDescription;
    private string ingredientDescriptions;
    private Effect statBonus;

    public Effect StatBonus
    {
      get
      {
        if (this.statBonus == null)
          return (Effect) null;
        if (string.IsNullOrEmpty(this.spiceDescription))
        {
          this.CreateDescription();
          this.GetName();
        }
        this.statBonus.Name = this.name;
        this.statBonus.description = this.spiceDescription;
        return this.statBonus;
      }
    }

    public Option(Spice spice)
    {
      this.Id = new Tag(spice.Id);
      this.Spice = spice;
      if (spice.StatBonus == null)
        return;
      this.statBonus = new Effect(spice.Id, this.GetName(), this.spiceDescription, 600f, true, false, false);
      this.statBonus.Add(spice.StatBonus);
      Db.Get().effects.Add(this.statBonus);
    }

    public Tag GetID() => Tag.op_Implicit(this.Spice.Id);

    public string GetName()
    {
      if (string.IsNullOrEmpty(this.name))
      {
        string str = "STRINGS.ITEMS.SPICES." + this.Spice.Id.ToUpper() + ".NAME";
        StringEntry stringEntry;
        Strings.TryGet(str, ref stringEntry);
        this.name = "MISSING " + str;
        if (stringEntry != null)
          this.name = StringEntry.op_Implicit(stringEntry);
      }
      return this.name;
    }

    public string GetDetailedDescription()
    {
      if (string.IsNullOrEmpty(this.fullDescription))
        this.CreateDescription();
      return this.fullDescription;
    }

    public string GetDescription()
    {
      if (!string.IsNullOrEmpty(this.spiceDescription))
        return this.spiceDescription;
      string str = "STRINGS.ITEMS.SPICES." + this.Spice.Id.ToUpper() + ".DESC";
      StringEntry stringEntry;
      Strings.TryGet(str, ref stringEntry);
      this.spiceDescription = "MISSING " + str;
      if (stringEntry != null)
        this.spiceDescription = stringEntry.String;
      return this.spiceDescription;
    }

    private void CreateDescription()
    {
      string str = "STRINGS.ITEMS.SPICES." + this.Spice.Id.ToUpper() + ".DESC";
      StringEntry stringEntry;
      Strings.TryGet(str, ref stringEntry);
      this.spiceDescription = "MISSING " + str;
      if (stringEntry != null)
        this.spiceDescription = stringEntry.String;
      this.ingredientDescriptions = string.Format("\n\n<b>{0}</b>", (object) BUILDINGS.PREFABS.SPICEGRINDER.INGREDIENTHEADER);
      for (int index = 0; index < this.Spice.Ingredients.Length; ++index)
      {
        Spice.Ingredient ingredient = this.Spice.Ingredients[index];
        GameObject prefab = Assets.GetPrefab(ingredient.IngredientSet == null || ingredient.IngredientSet.Length == 0 ? Tag.op_Implicit((string) null) : ingredient.IngredientSet[0]);
        this.ingredientDescriptions += string.Format("\n{0}{1} {2}{3}", (object) "    • ", (object) prefab.GetProperName(), (object) ingredient.AmountKG, (object) GameUtil.GetUnitTypeMassOrUnit(prefab));
      }
      this.fullDescription = this.spiceDescription + this.ingredientDescriptions;
    }

    public Sprite GetIcon() => Assets.GetSprite(HashedString.op_Implicit(this.Spice.Image));

    public IConfigurableConsumerIngredient[] GetIngredients() => (IConfigurableConsumerIngredient[]) this.Spice.Ingredients;
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class StatesInstance : 
    GameStateMachine<SpiceGrinder, SpiceGrinder.StatesInstance, IStateMachineTarget, SpiceGrinder.Def>.GameInstance
  {
    private static string HASH_FOOD = "food";
    private KBatchedAnimController kbac;
    private KBatchedAnimController foodKBAC;
    [MyCmpReq]
    public SpiceGrinderWorkable workable;
    [Serialize]
    private int spiceHash;
    private SpiceInstance currentSpice;
    private Edible currentFood;
    private Storage seedStorage;
    private Storage foodStorage;
    private MeterController meter;
    private Tag[] foodFilter = new Tag[1];
    private FilteredStorage foodStorageFilter;
    private Operational operational;
    private ManualDeliveryKG seedDelivery;

    public bool IsOperational => Object.op_Inequality((Object) this.operational, (Object) null) && this.operational.IsOperational;

    public float AvailableFood => !Object.op_Equality((Object) this.foodStorage, (Object) null) ? this.foodStorage.MassStored() : 0.0f;

    public float AvailableSeeds => !Object.op_Equality((Object) this.seedStorage, (Object) null) ? this.seedStorage.MassStored() : 0.0f;

    public SpiceGrinder.Option SelectedOption => !Tag.op_Equality(this.currentSpice.Id, Tag.Invalid) ? SpiceGrinder.SettingOptions[this.currentSpice.Id] : (SpiceGrinder.Option) null;

    public Edible CurrentFood
    {
      get
      {
        GameObject first = this.foodStorage.FindFirst(GameTags.Edible);
        this.currentFood = Object.op_Inequality((Object) first, (Object) null) ? first.GetComponent<Edible>() : (Edible) null;
        return this.currentFood;
      }
    }

    public StatesInstance(IStateMachineTarget master, SpiceGrinder.Def def)
      : base(master, def)
    {
      this.workable.Grinder = this;
      Storage[] components = this.gameObject.GetComponents<Storage>();
      this.foodStorage = components[0];
      this.seedStorage = components[1];
      this.operational = this.GetComponent<Operational>();
      this.seedDelivery = this.GetComponent<ManualDeliveryKG>();
      this.kbac = this.GetComponent<KBatchedAnimController>();
      this.foodStorageFilter = new FilteredStorage((KMonoBehaviour) this.GetComponent<KPrefabID>(), this.foodFilter, (IUserControlledCapacity) null, false, Db.Get().ChoreTypes.CookFetch);
      this.foodStorageFilter.SetHasMeter(false);
      this.meter = new MeterController((KAnimControllerBase) this.kbac, "meter_target", nameof (meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[2]
      {
        "meter_frame",
        "meter_level"
      });
      this.SetupFoodSymbol();
      this.UpdateFoodSymbol();
      this.Subscribe(-905833192, new System.Action<object>(this.OnCopySettings));
    }

    public void Initialize()
    {
      SpiceGrinder.Option spiceOption;
      SpiceGrinder.SettingOptions.TryGetValue(new Tag(this.spiceHash), out spiceOption);
      this.OnOptionSelected(spiceOption);
      this.sm.OnStorageChanged(this, (object) null);
      this.UpdateMeter();
    }

    private void OnCopySettings(object data)
    {
      SpiceGrinderWorkable component = ((GameObject) data).GetComponent<SpiceGrinderWorkable>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      this.currentSpice = component.Grinder.currentSpice;
      SpiceGrinder.Option spiceOption;
      SpiceGrinder.SettingOptions.TryGetValue(new Tag(component.Grinder.spiceHash), out spiceOption);
      this.OnOptionSelected(spiceOption);
    }

    public void SetupFoodSymbol()
    {
      GameObject gameObject = new GameObject();
      ((Object) gameObject).name = "foodSymbol";
      gameObject.SetActive(false);
      Matrix4x4 symbolTransform = this.kbac.GetSymbolTransform(HashedString.op_Implicit(SpiceGrinder.StatesInstance.HASH_FOOD), out bool _);
      Vector3 vector3 = Vector4.op_Implicit(((Matrix4x4) ref symbolTransform).GetColumn(3));
      vector3.z = Grid.GetLayerZ(Grid.SceneLayer.Building);
      TransformExtensions.SetPosition(gameObject.transform, vector3);
      this.foodKBAC = gameObject.AddComponent<KBatchedAnimController>();
      this.foodKBAC.AnimFiles = new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit("mushbar_kanim"))
      };
      this.foodKBAC.initialAnim = "object";
      this.kbac.SetSymbolVisiblity(KAnimHashedString.op_Implicit(SpiceGrinder.StatesInstance.HASH_FOOD), false);
    }

    public void UpdateFoodSymbol()
    {
      bool flag = (double) this.AvailableFood > 0.0 && Object.op_Inequality((Object) this.CurrentFood, (Object) null);
      ((Component) this.foodKBAC).gameObject.SetActive(flag);
      if (!flag)
        return;
      this.foodKBAC.SwapAnims(((Component) this.CurrentFood).GetComponent<KBatchedAnimController>().AnimFiles);
      this.foodKBAC.Play(HashedString.op_Implicit("object"), (KAnim.PlayMode) 0);
    }

    public void UpdateMeter() => this.meter.SetPositionPercent(this.seedStorage.MassStored() / this.seedStorage.capacityKg);

    public void SpiceFood()
    {
      float num = this.CurrentFood.Calories / 1000f;
      foreach (Spice.Ingredient ingredient in SpiceGrinder.SettingOptions[this.currentSpice.Id].Spice.Ingredients)
      {
        float amount = (float) ((double) num * (double) ingredient.AmountKG / 1000.0);
        for (int index = ingredient.IngredientSet.Length - 1; (double) amount > 0.0 && index >= 0; --index)
        {
          float amount_consumed;
          this.seedStorage.ConsumeAndGetDisease(ingredient.IngredientSet[index], amount, out amount_consumed, out SimUtil.DiseaseInfo _, out float _);
          amount -= amount_consumed;
        }
      }
      this.CurrentFood.SpiceEdible(this.currentSpice, SpiceGrinderConfig.SpicedStatus);
      this.foodStorage.Drop(((Component) this.CurrentFood).gameObject, true);
      this.currentFood = (Edible) null;
      this.UpdateFoodSymbol();
      this.sm.isReady.Set(false, this);
    }

    public bool CanSpice(float kcalToSpice)
    {
      bool flag = true;
      float num1 = kcalToSpice / 1000f;
      Spice.Ingredient[] ingredients = SpiceGrinder.SettingOptions[this.currentSpice.Id].Spice.Ingredients;
      for (int index1 = 0; flag && index1 < ingredients.Length; ++index1)
      {
        Spice.Ingredient ingredient = ingredients[index1];
        float num2 = 0.0f;
        for (int index2 = 0; ingredient.IngredientSet != null && index2 < ingredient.IngredientSet.Length; ++index2)
          num2 += this.seedStorage.GetMassAvailable(ingredient.IngredientSet[index2]);
        flag = (double) num1 * (double) ingredient.AmountKG / 1000.0 <= (double) num2;
      }
      return flag;
    }

    public void OnOptionSelected(SpiceGrinder.Option spiceOption)
    {
      this.smi.GetComponent<Operational>().SetFlag(SpiceGrinder.spiceSet, spiceOption != null);
      if (spiceOption == null)
      {
        this.kbac.Play(HashedString.op_Implicit("default"));
        this.kbac.SetSymbolTint(KAnimHashedString.op_Implicit("stripe_anim2"), Color.white);
      }
      else
        this.kbac.Play(HashedString.op_Implicit(this.IsOperational ? "on" : "off"));
      this.seedDelivery.Pause(true, "Spice Changed");
      this.seedDelivery.ClearRequests();
      if (Tag.op_Inequality(this.currentSpice.Id, Tag.Invalid))
      {
        this.seedStorage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
        this.UpdateMeter();
        this.sm.isReady.Set(false, this);
      }
      if (spiceOption == null)
        return;
      this.currentSpice = new SpiceInstance()
      {
        Id = spiceOption.Id,
        TotalKG = spiceOption.Spice.TotalKG
      };
      this.SetSpiceSymbolColours(spiceOption.Spice);
      this.spiceHash = ((Tag) ref this.currentSpice.Id).GetHash();
      this.seedStorage.capacityKg = this.currentSpice.TotalKG * 10f;
      this.seedDelivery.capacity = this.seedStorage.capacityKg;
      this.seedDelivery.refillMass = this.seedStorage.capacityKg * 0.5f;
      foreach (Spice.Ingredient ingredient in spiceOption.Spice.Ingredients)
        this.seedDelivery.RequestItem(ingredient.IngredientSet, ingredient.AmountKG);
      this.foodFilter[0] = this.currentSpice.Id;
      this.foodStorageFilter.FilterChanged();
      this.seedDelivery.Pause(false, "Spice Changed");
    }

    private void SetSpiceSymbolColours(Spice spice)
    {
      this.kbac.SetSymbolTint(KAnimHashedString.op_Implicit("stripe_anim2"), spice.PrimaryColor);
      this.kbac.SetSymbolTint(KAnimHashedString.op_Implicit("stripe_anim1"), spice.SecondaryColor);
      this.kbac.SetSymbolTint(KAnimHashedString.op_Implicit("grinder"), spice.PrimaryColor);
    }
  }
}
