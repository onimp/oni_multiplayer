// Decompiled with JetBrains decompiler
// Type: Database.Attributes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

namespace Database
{
  public class Attributes : ResourceSet<Attribute>
  {
    public Attribute Construction;
    public Attribute Digging;
    public Attribute Machinery;
    public Attribute Athletics;
    public Attribute Learning;
    public Attribute Cooking;
    public Attribute Caring;
    public Attribute Strength;
    public Attribute Art;
    public Attribute Botanist;
    public Attribute Ranching;
    public Attribute LifeSupport;
    public Attribute Toggle;
    public Attribute PowerTinker;
    public Attribute FarmTinker;
    public Attribute SpaceNavigation;
    public Attribute Immunity;
    public Attribute GermResistance;
    public Attribute Insulation;
    public Attribute ThermalConductivityBarrier;
    public Attribute Decor;
    public Attribute FoodQuality;
    public Attribute ScaldingThreshold;
    public Attribute GeneratorOutput;
    public Attribute MachinerySpeed;
    public Attribute RadiationResistance;
    public Attribute RadiationRecovery;
    public Attribute DecorExpectation;
    public Attribute FoodExpectation;
    public Attribute RoomTemperaturePreference;
    public Attribute QualityOfLifeExpectation;
    public Attribute AirConsumptionRate;
    public Attribute MaxUnderwaterTravelCost;
    public Attribute ToiletEfficiency;
    public Attribute Sneezyness;
    public Attribute DiseaseCureSpeed;
    public Attribute DoctoredLevel;
    public Attribute CarryAmount;
    public Attribute QualityOfLife;

    public Attributes(ResourceSet parent)
      : base(nameof (Attributes), parent)
    {
      this.Construction = this.Add(new Attribute(nameof (Construction), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_construction"));
      this.Construction.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Digging = this.Add(new Attribute(nameof (Digging), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_excavation"));
      this.Digging.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Machinery = this.Add(new Attribute(nameof (Machinery), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_machinery"));
      this.Machinery.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Athletics = this.Add(new Attribute(nameof (Athletics), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_athletics"));
      this.Athletics.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Learning = this.Add(new Attribute(nameof (Learning), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_science"));
      this.Learning.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Cooking = this.Add(new Attribute(nameof (Cooking), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_cusine"));
      this.Cooking.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Caring = this.Add(new Attribute(nameof (Caring), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_medicine"));
      this.Caring.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Strength = this.Add(new Attribute(nameof (Strength), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_strength"));
      this.Strength.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Art = this.Add(new Attribute(nameof (Art), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_creativity"));
      this.Art.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Botanist = this.Add(new Attribute(nameof (Botanist), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_agriculture"));
      this.Botanist.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Ranching = this.Add(new Attribute(nameof (Ranching), true, Attribute.Display.Skill, true, uiFullColourSprite: "mod_husbandry"));
      this.Ranching.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.PowerTinker = this.Add(new Attribute(nameof (PowerTinker), true, Attribute.Display.Normal, true));
      this.PowerTinker.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.FarmTinker = this.Add(new Attribute(nameof (FarmTinker), true, Attribute.Display.Normal, true));
      this.FarmTinker.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      if (DlcManager.IsExpansion1Active())
      {
        this.SpaceNavigation = this.Add(new Attribute(nameof (SpaceNavigation), true, Attribute.Display.Skill, true));
        this.SpaceNavigation.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      }
      else
      {
        this.SpaceNavigation = this.Add(new Attribute(nameof (SpaceNavigation), true, Attribute.Display.Normal, true));
        this.SpaceNavigation.SetFormatter((IAttributeFormatter) new PercentAttributeFormatter());
      }
      this.Immunity = this.Add(new Attribute(nameof (Immunity), true, Attribute.Display.Details, false));
      this.Immunity.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.ThermalConductivityBarrier = this.Add(new Attribute(nameof (ThermalConductivityBarrier), false, Attribute.Display.Details, false));
      this.ThermalConductivityBarrier.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Distance, GameUtil.TimeSlice.None));
      this.Insulation = this.Add(new Attribute(nameof (Insulation), false, Attribute.Display.General, true));
      this.Decor = this.Add(new Attribute(nameof (Decor), false, Attribute.Display.General, false));
      this.Decor.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.FoodQuality = this.Add(new Attribute(nameof (FoodQuality), false, Attribute.Display.General, false));
      this.FoodQuality.SetFormatter((IAttributeFormatter) new FoodQualityAttributeFormatter());
      this.ScaldingThreshold = this.Add(new Attribute(nameof (ScaldingThreshold), false, Attribute.Display.General, false));
      this.ScaldingThreshold.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
      this.GeneratorOutput = this.Add(new Attribute(nameof (GeneratorOutput), false, Attribute.Display.General, false));
      this.GeneratorOutput.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Percent, GameUtil.TimeSlice.None));
      this.MachinerySpeed = this.Add(new Attribute(nameof (MachinerySpeed), false, Attribute.Display.General, false, 1f));
      this.MachinerySpeed.SetFormatter((IAttributeFormatter) new PercentAttributeFormatter());
      this.RadiationResistance = this.Add(new Attribute(nameof (RadiationResistance), false, Attribute.Display.Details, false, overrideDLCIDs: DlcManager.AVAILABLE_EXPANSION1_ONLY));
      this.RadiationResistance.SetFormatter((IAttributeFormatter) new PercentAttributeFormatter());
      this.RadiationRecovery = this.Add(new Attribute(nameof (RadiationRecovery), false, Attribute.Display.Details, false, overrideDLCIDs: DlcManager.AVAILABLE_EXPANSION1_ONLY));
      this.RadiationRecovery.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Radiation, GameUtil.TimeSlice.PerCycle));
      this.DecorExpectation = this.Add(new Attribute(nameof (DecorExpectation), false, Attribute.Display.Expectation, false));
      this.DecorExpectation.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.FoodExpectation = this.Add(new Attribute(nameof (FoodExpectation), false, Attribute.Display.Expectation, false));
      this.FoodExpectation.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.RoomTemperaturePreference = this.Add(new Attribute(nameof (RoomTemperaturePreference), false, Attribute.Display.Normal, false));
      this.RoomTemperaturePreference.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Temperature, GameUtil.TimeSlice.None));
      this.QualityOfLifeExpectation = this.Add(new Attribute(nameof (QualityOfLifeExpectation), false, Attribute.Display.Normal, false));
      this.QualityOfLifeExpectation.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.AirConsumptionRate = this.Add(new Attribute(nameof (AirConsumptionRate), false, Attribute.Display.Normal, false));
      this.AirConsumptionRate.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.PerSecond));
      this.MaxUnderwaterTravelCost = this.Add(new Attribute(nameof (MaxUnderwaterTravelCost), false, Attribute.Display.Normal, false));
      this.ToiletEfficiency = this.Add(new Attribute(nameof (ToiletEfficiency), false, Attribute.Display.Details, false));
      this.ToiletEfficiency.SetFormatter((IAttributeFormatter) new ToPercentAttributeFormatter(1f));
      this.Sneezyness = this.Add(new Attribute(nameof (Sneezyness), false, Attribute.Display.Details, false));
      this.DiseaseCureSpeed = this.Add(new Attribute(nameof (DiseaseCureSpeed), false, Attribute.Display.Normal, false));
      this.DiseaseCureSpeed.BaseValue = 1f;
      this.DiseaseCureSpeed.SetFormatter((IAttributeFormatter) new ToPercentAttributeFormatter(1f));
      this.DoctoredLevel = this.Add(new Attribute(nameof (DoctoredLevel), false, Attribute.Display.Never, false));
      this.CarryAmount = this.Add(new Attribute(nameof (CarryAmount), false, Attribute.Display.Details, false));
      this.CarryAmount.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.None));
      this.QualityOfLife = this.Add(new Attribute(nameof (QualityOfLife), false, Attribute.Display.Details, false, uiSprite: "ui_icon_qualityoflife", thoughtSprite: "attribute_qualityoflife", uiFullColourSprite: "mod_morale"));
      this.QualityOfLife.SetFormatter((IAttributeFormatter) new QualityOfLifeAttributeFormatter());
      this.GermResistance = this.Add(new Attribute(nameof (GermResistance), false, Attribute.Display.Details, false, uiSprite: "ui_icon_immunelevel", thoughtSprite: "attribute_immunelevel", uiFullColourSprite: "mod_germresistance"));
      this.GermResistance.SetFormatter((IAttributeFormatter) new GermResistanceAttributeFormatter());
      this.LifeSupport = this.Add(new Attribute(nameof (LifeSupport), true, Attribute.Display.Never, false));
      this.LifeSupport.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
      this.Toggle = this.Add(new Attribute(nameof (Toggle), true, Attribute.Display.Never, false));
      this.Toggle.SetFormatter((IAttributeFormatter) new StandardAttributeFormatter(GameUtil.UnitClass.SimpleInteger, GameUtil.TimeSlice.None));
    }
  }
}
