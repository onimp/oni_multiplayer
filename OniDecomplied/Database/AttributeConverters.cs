// Decompiled with JetBrains decompiler
// Type: Database.AttributeConverters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System.Collections.Generic;

namespace Database
{
  public class AttributeConverters : ResourceSet<AttributeConverter>
  {
    public AttributeConverter MovementSpeed;
    public AttributeConverter ConstructionSpeed;
    public AttributeConverter DiggingSpeed;
    public AttributeConverter MachinerySpeed;
    public AttributeConverter HarvestSpeed;
    public AttributeConverter PlantTendSpeed;
    public AttributeConverter CompoundingSpeed;
    public AttributeConverter ResearchSpeed;
    public AttributeConverter TrainingSpeed;
    public AttributeConverter CookingSpeed;
    public AttributeConverter ArtSpeed;
    public AttributeConverter DoctorSpeed;
    public AttributeConverter TidyingSpeed;
    public AttributeConverter AttackDamage;
    public AttributeConverter PilotingSpeed;
    public AttributeConverter ImmuneLevelBoost;
    public AttributeConverter ToiletSpeed;
    public AttributeConverter CarryAmountFromStrength;
    public AttributeConverter TemperatureInsulation;
    public AttributeConverter SeedHarvestChance;
    public AttributeConverter RanchingEffectDuration;
    public AttributeConverter FarmedEffectDuration;
    public AttributeConverter PowerTinkerEffectDuration;
    public AttributeConverter CapturableSpeed;
    public AttributeConverter GeotuningSpeed;

    public AttributeConverter Create(
      string id,
      string name,
      string description,
      Attribute attribute,
      float multiplier,
      float base_value,
      IAttributeFormatter formatter,
      string[] available_dlcs)
    {
      AttributeConverter attributeConverter = new AttributeConverter(id, name, description, multiplier, base_value, attribute, formatter);
      if (DlcManager.IsDlcListValidForCurrentContent(available_dlcs))
      {
        this.Add(attributeConverter);
        attribute.converters.Add(attributeConverter);
      }
      return attributeConverter;
    }

    public AttributeConverters()
    {
      ToPercentAttributeFormatter formatter1 = new ToPercentAttributeFormatter(1f);
      StandardAttributeFormatter formatter2 = new StandardAttributeFormatter(GameUtil.UnitClass.Mass, GameUtil.TimeSlice.None);
      this.MovementSpeed = this.Create(nameof (MovementSpeed), "Movement Speed", (string) DUPLICANTS.ATTRIBUTES.ATHLETICS.SPEEDMODIFIER, Db.Get().Attributes.Athletics, 0.1f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.ConstructionSpeed = this.Create(nameof (ConstructionSpeed), "Construction Speed", (string) DUPLICANTS.ATTRIBUTES.CONSTRUCTION.SPEEDMODIFIER, Db.Get().Attributes.Construction, 0.25f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.DiggingSpeed = this.Create(nameof (DiggingSpeed), "Digging Speed", (string) DUPLICANTS.ATTRIBUTES.DIGGING.SPEEDMODIFIER, Db.Get().Attributes.Digging, 0.25f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.MachinerySpeed = this.Create(nameof (MachinerySpeed), "Machinery Speed", (string) DUPLICANTS.ATTRIBUTES.MACHINERY.SPEEDMODIFIER, Db.Get().Attributes.Machinery, 0.1f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.HarvestSpeed = this.Create(nameof (HarvestSpeed), "Harvest Speed", (string) DUPLICANTS.ATTRIBUTES.BOTANIST.HARVEST_SPEED_MODIFIER, Db.Get().Attributes.Botanist, 0.05f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.PlantTendSpeed = this.Create(nameof (PlantTendSpeed), "Plant Tend Speed", (string) DUPLICANTS.ATTRIBUTES.BOTANIST.TINKER_MODIFIER, Db.Get().Attributes.Botanist, 0.025f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.CompoundingSpeed = this.Create(nameof (CompoundingSpeed), "Compounding Speed", (string) DUPLICANTS.ATTRIBUTES.CARING.FABRICATE_SPEEDMODIFIER, Db.Get().Attributes.Caring, 0.1f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.ResearchSpeed = this.Create(nameof (ResearchSpeed), "Research Speed", (string) DUPLICANTS.ATTRIBUTES.LEARNING.RESEARCHSPEED, Db.Get().Attributes.Learning, 0.4f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.TrainingSpeed = this.Create(nameof (TrainingSpeed), "Training Speed", (string) DUPLICANTS.ATTRIBUTES.LEARNING.SPEEDMODIFIER, Db.Get().Attributes.Learning, 0.1f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.CookingSpeed = this.Create(nameof (CookingSpeed), "Cooking Speed", (string) DUPLICANTS.ATTRIBUTES.COOKING.SPEEDMODIFIER, Db.Get().Attributes.Cooking, 0.05f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.ArtSpeed = this.Create(nameof (ArtSpeed), "Art Speed", (string) DUPLICANTS.ATTRIBUTES.ART.SPEEDMODIFIER, Db.Get().Attributes.Art, 0.1f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.DoctorSpeed = this.Create(nameof (DoctorSpeed), "Doctor Speed", (string) DUPLICANTS.ATTRIBUTES.CARING.SPEEDMODIFIER, Db.Get().Attributes.Caring, 0.2f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.TidyingSpeed = this.Create(nameof (TidyingSpeed), "Tidying Speed", (string) DUPLICANTS.ATTRIBUTES.STRENGTH.SPEEDMODIFIER, Db.Get().Attributes.Strength, 0.25f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.AttackDamage = this.Create(nameof (AttackDamage), "Attack Damage", (string) DUPLICANTS.ATTRIBUTES.DIGGING.ATTACK_MODIFIER, Db.Get().Attributes.Digging, 0.05f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.PilotingSpeed = this.Create(nameof (PilotingSpeed), "Piloting Speed", (string) DUPLICANTS.ATTRIBUTES.SPACENAVIGATION.SPEED_MODIFIER, Db.Get().Attributes.SpaceNavigation, 0.025f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_EXPANSION1_ONLY);
      this.ImmuneLevelBoost = this.Create(nameof (ImmuneLevelBoost), "Immune Level Boost", (string) DUPLICANTS.ATTRIBUTES.IMMUNITY.BOOST_MODIFIER, Db.Get().Attributes.Immunity, 1f / 600f, 0.0f, (IAttributeFormatter) new ToPercentAttributeFormatter(100f, GameUtil.TimeSlice.PerCycle), DlcManager.AVAILABLE_ALL_VERSIONS);
      this.ToiletSpeed = this.Create(nameof (ToiletSpeed), "Toilet Speed", "", Db.Get().Attributes.ToiletEfficiency, 1f, -1f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.CarryAmountFromStrength = this.Create(nameof (CarryAmountFromStrength), "Carry Amount", (string) DUPLICANTS.ATTRIBUTES.STRENGTH.CARRYMODIFIER, Db.Get().Attributes.Strength, 40f, 0.0f, (IAttributeFormatter) formatter2, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.TemperatureInsulation = this.Create(nameof (TemperatureInsulation), "Temperature Insulation", (string) DUPLICANTS.ATTRIBUTES.INSULATION.SPEEDMODIFIER, Db.Get().Attributes.Insulation, 0.1f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.SeedHarvestChance = this.Create(nameof (SeedHarvestChance), "Seed Harvest Chance", (string) DUPLICANTS.ATTRIBUTES.BOTANIST.BONUS_SEEDS, Db.Get().Attributes.Botanist, 0.033f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.CapturableSpeed = this.Create(nameof (CapturableSpeed), "Capturable Speed", (string) DUPLICANTS.ATTRIBUTES.RANCHING.CAPTURABLESPEED, Db.Get().Attributes.Ranching, 0.05f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.GeotuningSpeed = this.Create(nameof (GeotuningSpeed), "Geotuning Speed", (string) DUPLICANTS.ATTRIBUTES.LEARNING.GEOTUNER_SPEED_MODIFIER, Db.Get().Attributes.Learning, 0.05f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.RanchingEffectDuration = this.Create(nameof (RanchingEffectDuration), "Ranching Effect Duration", (string) DUPLICANTS.ATTRIBUTES.RANCHING.EFFECTMODIFIER, Db.Get().Attributes.Ranching, 0.1f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.FarmedEffectDuration = this.Create(nameof (FarmedEffectDuration), "Farmer's Touch Duration", (string) DUPLICANTS.ATTRIBUTES.BOTANIST.TINKER_EFFECT_MODIFIER, Db.Get().Attributes.Botanist, 0.1f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
      this.PowerTinkerEffectDuration = this.Create(nameof (PowerTinkerEffectDuration), "Engie's Tune-Up Effect Duration", (string) DUPLICANTS.ATTRIBUTES.MACHINERY.TINKER_EFFECT_MODIFIER, Db.Get().Attributes.Machinery, 0.025f, 0.0f, (IAttributeFormatter) formatter1, DlcManager.AVAILABLE_ALL_VERSIONS);
    }

    public List<AttributeConverter> GetConvertersForAttribute(Attribute attrib)
    {
      List<AttributeConverter> convertersForAttribute = new List<AttributeConverter>();
      foreach (AttributeConverter resource in this.resources)
      {
        if (resource.attribute == attrib)
          convertersForAttribute.Add(resource);
      }
      return convertersForAttribute;
    }
  }
}
