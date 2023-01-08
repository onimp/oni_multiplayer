// Decompiled with JetBrains decompiler
// Type: Database.Amounts
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

namespace Database
{
  public class Amounts : ResourceSet<Amount>
  {
    public Amount Stamina;
    public Amount Calories;
    public Amount ImmuneLevel;
    public Amount ExternalTemperature;
    public Amount Breath;
    public Amount Stress;
    public Amount Toxicity;
    public Amount Bladder;
    public Amount Decor;
    public Amount RadiationBalance;
    public Amount Temperature;
    public Amount HitPoints;
    public Amount AirPressure;
    public Amount Maturity;
    public Amount OldAge;
    public Amount Age;
    public Amount Fertilization;
    public Amount Illumination;
    public Amount Irrigation;
    public Amount CreatureCalories;
    public Amount Fertility;
    public Amount Viability;
    public Amount PowerCharge;
    public Amount Wildness;
    public Amount Incubation;
    public Amount ScaleGrowth;
    public Amount ElementGrowth;
    public Amount InternalBattery;
    public Amount InternalChemicalBattery;
    public Amount Rot;

    public void Load()
    {
      this.Stamina = this.CreateAmount("Stamina", 0.0f, 100f, false, (Units) 0, 0.35f, true, "STRINGS.DUPLICANTS", "ui_icon_stamina", "attribute_stamina", "mod_stamina");
      this.Stamina.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle));
      this.Calories = this.CreateAmount("Calories", 0.0f, 0.0f, false, (Units) 0, 4000f, true, "STRINGS.DUPLICANTS", "ui_icon_calories", "attribute_calories", "mod_calories");
      this.Calories.SetDisplayer((IAmountDisplayer) new CaloriesDisplayer());
      this.ExternalTemperature = this.CreateAmount("ExternalTemperature", 0.0f, 10000f, false, (Units) 3, 0.5f, true, "STRINGS.DUPLICANTS");
      this.ExternalTemperature.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
      this.Breath = this.CreateAmount("Breath", 0.0f, 100f, false, (Units) 0, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_breath", uiFullColourSprite: "mod_breath");
      this.Breath.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
      this.Stress = this.CreateAmount("Stress", 0.0f, 100f, false, (Units) 0, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_stress", "attribute_stress", "mod_stress");
      this.Stress.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.Toxicity = this.CreateAmount("Toxicity", 0.0f, 100f, true, (Units) 0, 0.5f, true, "STRINGS.DUPLICANTS");
      this.Toxicity.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle));
      this.Bladder = this.CreateAmount("Bladder", 0.0f, 100f, false, (Units) 0, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_bladder", uiFullColourSprite: "mod_bladder");
      this.Bladder.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerCycle));
      this.Decor = this.CreateAmount("Decor", -1000f, 1000f, false, (Units) 0, 0.0166666675f, true, "STRINGS.DUPLICANTS", "ui_icon_decor", uiFullColourSprite: "mod_decor");
      this.Decor.SetDisplayer((IAmountDisplayer) new DecorDisplayer());
      this.RadiationBalance = this.CreateAmount("RadiationBalance", 0.0f, 10000f, false, (Units) 0, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_radiation", uiFullColourSprite: "mod_health");
      this.RadiationBalance.SetDisplayer((IAmountDisplayer) new RadiationBalanceDisplayer());
      this.Temperature = this.CreateAmount("Temperature", 0.0f, 10000f, false, (Units) 3, 0.5f, true, "STRINGS.DUPLICANTS", "ui_icon_temperature");
      this.Temperature.SetDisplayer((IAmountDisplayer) new DuplicantTemperatureDeltaAsEnergyAmountDisplayer(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.PerSecond));
      this.HitPoints = this.CreateAmount("HitPoints", 0.0f, 0.0f, true, (Units) 0, 0.1675f, true, "STRINGS.DUPLICANTS", "ui_icon_hitpoints", "attribute_hitpoints", "mod_health");
      this.HitPoints.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle, tense: GameUtil.IdentityDescriptorTense.Possessive));
      this.AirPressure = this.CreateAmount("AirPressure", 0.0f, 1E+09f, false, (Units) 0, 0.0f, true, "STRINGS.CREATURES");
      this.AirPressure.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));
      this.Maturity = this.CreateAmount("Maturity", 0.0f, 0.0f, true, (Units) 0, 0.0009166667f, true, "STRINGS.CREATURES", "ui_icon_maturity");
      this.Maturity.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Cycles, GameUtil.TimeSlice.None));
      this.OldAge = this.CreateAmount("OldAge", 0.0f, 0.0f, false, (Units) 0, 0.0f, false, "STRINGS.CREATURES");
      this.Fertilization = this.CreateAmount("Fertilization", 0.0f, 100f, true, (Units) 0, 0.1675f, true, "STRINGS.CREATURES");
      this.Fertilization.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
      this.Fertility = this.CreateAmount("Fertility", 0.0f, 100f, true, (Units) 0, 0.008375f, true, "STRINGS.CREATURES", "ui_icon_fertility");
      this.Fertility.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.Wildness = this.CreateAmount("Wildness", 0.0f, 100f, true, (Units) 0, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_wildness");
      this.Wildness.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.Incubation = this.CreateAmount("Incubation", 0.0f, 100f, true, (Units) 0, 0.01675f, true, "STRINGS.CREATURES", "ui_icon_incubation");
      this.Incubation.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.Viability = this.CreateAmount("Viability", 0.0f, 100f, true, (Units) 0, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_viability");
      this.Viability.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.PowerCharge = this.CreateAmount("PowerCharge", 0.0f, 100f, true, (Units) 0, 0.1675f, true, "STRINGS.CREATURES");
      this.PowerCharge.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.Age = this.CreateAmount("Age", 0.0f, 0.0f, true, (Units) 0, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_age");
      this.Age.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.PerCycle));
      this.Irrigation = this.CreateAmount("Irrigation", 0.0f, 1f, true, (Units) 0, 0.1675f, true, "STRINGS.CREATURES");
      this.Irrigation.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.PerSecond));
      this.ImmuneLevel = this.CreateAmount("ImmuneLevel", 0.0f, 100f, true, (Units) 0, 0.1675f, true, "STRINGS.DUPLICANTS", "ui_icon_immunelevel", "attribute_immunelevel");
      this.ImmuneLevel.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.Rot = this.CreateAmount("Rot", 0.0f, 0.0f, false, (Units) 0, 0.0f, true, "STRINGS.CREATURES");
      this.Rot.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.Illumination = this.CreateAmount("Illumination", 0.0f, 1f, false, (Units) 0, 0.0f, true, "STRINGS.CREATURES");
      this.Illumination.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None));
      this.ScaleGrowth = this.CreateAmount("ScaleGrowth", 0.0f, 100f, true, (Units) 0, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_scale_growth");
      this.ScaleGrowth.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.ElementGrowth = this.CreateAmount("ElementGrowth", 0.0f, 100f, true, (Units) 0, 0.1675f, true, "STRINGS.CREATURES", "ui_icon_scale_growth");
      this.ElementGrowth.SetDisplayer((IAmountDisplayer) new AsPercentAmountDisplayer(GameUtil.TimeSlice.PerCycle));
      this.InternalBattery = this.CreateAmount("InternalBattery", 0.0f, 0.0f, true, (Units) 0, 4000f, true, "STRINGS.ROBOTS", "ui_icon_battery");
      this.InternalBattery.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Energy, GameUtil.TimeSlice.PerSecond));
      this.InternalChemicalBattery = this.CreateAmount("InternalChemicalBattery", 0.0f, 0.0f, true, (Units) 0, 4000f, true, "STRINGS.ROBOTS", "ui_icon_battery");
      this.InternalChemicalBattery.SetDisplayer((IAmountDisplayer) new StandardAmountDisplayer(GameUtil.UnitClass.Energy, GameUtil.TimeSlice.PerSecond));
    }

    public Amount CreateAmount(
      string id,
      float min,
      float max,
      bool show_max,
      Units units,
      float delta_threshold,
      bool show_in_ui,
      string string_root,
      string uiSprite = null,
      string thoughtSprite = null,
      string uiFullColourSprite = null)
    {
      string name1 = StringEntry.op_Implicit(Strings.Get(string.Format("{1}.STATS.{0}.NAME", (object) id.ToUpper(), (object) string_root.ToUpper())));
      string description = StringEntry.op_Implicit(Strings.Get(string.Format("{1}.STATS.{0}.TOOLTIP", (object) id.ToUpper(), (object) string_root.ToUpper())));
      Attribute.Display show_in_ui1 = show_in_ui ? Attribute.Display.Normal : Attribute.Display.Never;
      string str1 = id + "Min";
      StringEntry stringEntry1;
      string name2 = Strings.TryGet(new StringKey(string.Format("{1}.ATTRIBUTES.{0}.NAME", (object) str1.ToUpper(), (object) string_root)), ref stringEntry1) ? stringEntry1.String : "Minimum" + name1;
      StringEntry stringEntry2;
      string attribute_description1 = Strings.TryGet(new StringKey(string.Format("{1}.ATTRIBUTES.{0}.DESC", (object) str1.ToUpper(), (object) string_root)), ref stringEntry2) ? stringEntry2.String : "Minimum" + name1;
      Attribute min_attribute = new Attribute(id + "Min", name2, "", attribute_description1, min, show_in_ui1, false, uiFullColourSprite: uiFullColourSprite);
      string str2 = id + "Max";
      StringEntry stringEntry3;
      string name3 = Strings.TryGet(new StringKey(string.Format("{1}.ATTRIBUTES.{0}.NAME", (object) str2.ToUpper(), (object) string_root)), ref stringEntry3) ? stringEntry3.String : "Maximum" + name1;
      StringEntry stringEntry4;
      string attribute_description2 = Strings.TryGet(new StringKey(string.Format("{1}.ATTRIBUTES.{0}.DESC", (object) str2.ToUpper(), (object) string_root)), ref stringEntry4) ? stringEntry4.String : "Maximum" + name1;
      Attribute max_attribute = new Attribute(id + "Max", name3, "", attribute_description2, max, show_in_ui1, false, uiFullColourSprite: uiFullColourSprite);
      string id1 = id + "Delta";
      string name4 = StringEntry.op_Implicit(Strings.Get(string.Format("{1}.ATTRIBUTES.{0}.NAME", (object) id1.ToUpper(), (object) string_root)));
      string attribute_description3 = StringEntry.op_Implicit(Strings.Get(string.Format("{1}.ATTRIBUTES.{0}.DESC", (object) id1.ToUpper(), (object) string_root)));
      Attribute delta_attribute = new Attribute(id1, name4, "", attribute_description3, 0.0f, Attribute.Display.Normal, false, uiFullColourSprite: uiFullColourSprite);
      Amount amount = new Amount(id, name1, description, min_attribute, max_attribute, delta_attribute, show_max, units, delta_threshold, show_in_ui, uiSprite, thoughtSprite);
      Db.Get().Attributes.Add(min_attribute);
      Db.Get().Attributes.Add(max_attribute);
      Db.Get().Attributes.Add(delta_attribute);
      this.Add(amount);
      return amount;
    }
  }
}
