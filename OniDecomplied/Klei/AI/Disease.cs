// Decompiled with JetBrains decompiler
// Type: Klei.AI.Disease
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

namespace Klei.AI
{
  [DebuggerDisplay("{base.Id}")]
  public abstract class Disease : Resource
  {
    private StringKey name;
    public HashedString id;
    public float strength;
    public Disease.RangeInfo temperatureRange;
    public Disease.RangeInfo temperatureHalfLives;
    public Disease.RangeInfo pressureRange;
    public Disease.RangeInfo pressureHalfLives;
    public List<GrowthRule> growthRules;
    public List<ExposureRule> exposureRules;
    public ElemGrowthInfo[] elemGrowthInfo;
    public ElemExposureInfo[] elemExposureInfo;
    public string overlayColourName;
    public string overlayLegendHovertext;
    public float radiationKillRate;
    public Amount amount;
    public Attribute amountDeltaAttribute;
    public Attribute cureSpeedBase;
    public static readonly ElemGrowthInfo DEFAULT_GROWTH_INFO = new ElemGrowthInfo()
    {
      underPopulationDeathRate = 0.0f,
      populationHalfLife = float.PositiveInfinity,
      overPopulationHalfLife = float.PositiveInfinity,
      minCountPerKG = 0.0f,
      maxCountPerKG = float.PositiveInfinity,
      minDiffusionCount = 0,
      diffusionScale = 1f,
      minDiffusionInfestationTickCount = byte.MaxValue
    };
    public static ElemExposureInfo DEFAULT_EXPOSURE_INFO = new ElemExposureInfo()
    {
      populationHalfLife = float.PositiveInfinity
    };

    public string Name => StringEntry.op_Implicit(Strings.Get(this.name));

    public Disease(
      string id,
      byte strength,
      Disease.RangeInfo temperature_range,
      Disease.RangeInfo temperature_half_lives,
      Disease.RangeInfo pressure_range,
      Disease.RangeInfo pressure_half_lives,
      float radiation_kill_rate,
      bool statsOnly)
      : base(id, (ResourceSet) null, (string) null)
    {
      this.name = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".NAME");
      this.id = HashedString.op_Implicit(id);
      this.temperatureRange = temperature_range;
      this.temperatureHalfLives = temperature_half_lives;
      this.pressureRange = pressure_range;
      this.pressureHalfLives = pressure_half_lives;
      this.radiationKillRate = radiation_kill_rate;
      this.PopulateElemGrowthInfo();
      this.ApplyRules();
      if (statsOnly)
        return;
      this.overlayColourName = Assets.instance.DiseaseVisualization.GetInfo(HashedString.op_Implicit(id)).overlayColourName;
      this.overlayLegendHovertext = ((object) Strings.Get("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".LEGEND_HOVERTEXT")).ToString() + (string) DUPLICANTS.DISEASES.LEGEND_POSTAMBLE;
      Attribute min_attribute = new Attribute(id + "Min", "Minimum" + id.ToString(), "", "", 0.0f, Attribute.Display.Normal, false);
      Attribute max_attribute = new Attribute(id + "Max", "Maximum" + id.ToString(), "", "", 1E+07f, Attribute.Display.Normal, false);
      this.amountDeltaAttribute = new Attribute(id + "Delta", id.ToString(), "", "", 0.0f, Attribute.Display.Normal, false);
      this.amount = new Amount(id, id + " " + (string) DUPLICANTS.DISEASES.GERMS, id + " " + (string) DUPLICANTS.DISEASES.GERMS, min_attribute, max_attribute, this.amountDeltaAttribute, false, (Units) 0, 0.01f, true);
      Db.Get().Attributes.Add(min_attribute);
      Db.Get().Attributes.Add(max_attribute);
      Db.Get().Attributes.Add(this.amountDeltaAttribute);
      this.cureSpeedBase = new Attribute(id + "CureSpeed", false, Attribute.Display.Normal, false);
      this.cureSpeedBase.BaseValue = 1f;
      this.cureSpeedBase.SetFormatter((IAttributeFormatter) new ToPercentAttributeFormatter(1f));
      Db.Get().Attributes.Add(this.cureSpeedBase);
    }

    protected virtual void PopulateElemGrowthInfo()
    {
      this.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
      this.AddGrowthRule(new GrowthRule()
      {
        underPopulationDeathRate = new float?(0.0f),
        minCountPerKG = new float?(100f),
        populationHalfLife = new float?(float.PositiveInfinity),
        maxCountPerKG = new float?(1000f),
        overPopulationHalfLife = new float?(float.PositiveInfinity),
        minDiffusionCount = new int?(1000),
        diffusionScale = new float?(1f / 1000f),
        minDiffusionInfestationTickCount = new byte?((byte) 1)
      });
      this.InitializeElemExposureArray(ref this.elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
      this.AddExposureRule(new ExposureRule()
      {
        populationHalfLife = new float?(float.PositiveInfinity)
      });
    }

    protected void AddGrowthRule(GrowthRule g)
    {
      if (this.growthRules == null)
      {
        this.growthRules = new List<GrowthRule>();
        Debug.Assert(g.GetType() == typeof (GrowthRule), (object) "First rule must be a fully defined base rule.");
        Debug.Assert(g.underPopulationDeathRate.HasValue, (object) "First rule must be a fully defined base rule.");
        Debug.Assert(g.populationHalfLife.HasValue, (object) "First rule must be a fully defined base rule.");
        Debug.Assert(g.overPopulationHalfLife.HasValue, (object) "First rule must be a fully defined base rule.");
        Debug.Assert(g.diffusionScale.HasValue, (object) "First rule must be a fully defined base rule.");
        Debug.Assert(g.minCountPerKG.HasValue, (object) "First rule must be a fully defined base rule.");
        Debug.Assert(g.maxCountPerKG.HasValue, (object) "First rule must be a fully defined base rule.");
        Debug.Assert(g.minDiffusionCount.HasValue, (object) "First rule must be a fully defined base rule.");
        Debug.Assert(g.minDiffusionInfestationTickCount.HasValue, (object) "First rule must be a fully defined base rule.");
      }
      else
        Debug.Assert(g.GetType() != typeof (GrowthRule), (object) "Subsequent rules should not be base rules");
      this.growthRules.Add(g);
    }

    protected void AddExposureRule(ExposureRule g)
    {
      if (this.exposureRules == null)
      {
        this.exposureRules = new List<ExposureRule>();
        Debug.Assert(g.GetType() == typeof (ExposureRule), (object) "First rule must be a fully defined base rule.");
        Debug.Assert(g.populationHalfLife.HasValue, (object) "First rule must be a fully defined base rule.");
      }
      else
        Debug.Assert(g.GetType() != typeof (ExposureRule), (object) "Subsequent rules should not be base rules");
      this.exposureRules.Add(g);
    }

    public CompositeGrowthRule GetGrowthRuleForElement(Element e)
    {
      CompositeGrowthRule growthRuleForElement = new CompositeGrowthRule();
      if (this.growthRules != null)
      {
        for (int index = 0; index < this.growthRules.Count; ++index)
        {
          if (this.growthRules[index].Test(e))
            growthRuleForElement.Overlay(this.growthRules[index]);
        }
      }
      return growthRuleForElement;
    }

    public CompositeExposureRule GetExposureRuleForElement(Element e)
    {
      CompositeExposureRule exposureRuleForElement = new CompositeExposureRule();
      if (this.exposureRules != null)
      {
        for (int index = 0; index < this.exposureRules.Count; ++index)
        {
          if (this.exposureRules[index].Test(e))
            exposureRuleForElement.Overlay(this.exposureRules[index]);
        }
      }
      return exposureRuleForElement;
    }

    public TagGrowthRule GetGrowthRuleForTag(Tag t)
    {
      if (this.growthRules != null)
      {
        for (int index = 0; index < this.growthRules.Count; ++index)
        {
          if (this.growthRules[index] is TagGrowthRule growthRule && Tag.op_Equality(growthRule.tag, t))
            return growthRule;
        }
      }
      return (TagGrowthRule) null;
    }

    protected void ApplyRules()
    {
      if (this.growthRules != null)
      {
        for (int index = 0; index < this.growthRules.Count; ++index)
          this.growthRules[index].Apply(this.elemGrowthInfo);
      }
      if (this.exposureRules == null)
        return;
      for (int index = 0; index < this.exposureRules.Count; ++index)
        this.exposureRules[index].Apply(this.elemExposureInfo);
    }

    protected void InitializeElemGrowthArray(
      ref ElemGrowthInfo[] infoArray,
      ElemGrowthInfo default_value)
    {
      List<Element> elements = ElementLoader.elements;
      infoArray = new ElemGrowthInfo[elements.Count];
      for (int index = 0; index < elements.Count; ++index)
      {
        ElemGrowthInfo elemGrowthInfo = default_value;
        infoArray[index] = elemGrowthInfo;
      }
      ElemGrowthInfo[] elemGrowthInfoArray1 = infoArray;
      int elementIndex1 = (int) ElementLoader.GetElementIndex(SimHashes.Polypropylene);
      ElemGrowthInfo elemGrowthInfo1 = new ElemGrowthInfo();
      elemGrowthInfo1.underPopulationDeathRate = 2.66666675f;
      elemGrowthInfo1.populationHalfLife = 10f;
      elemGrowthInfo1.overPopulationHalfLife = 10f;
      elemGrowthInfo1.minCountPerKG = 0.0f;
      elemGrowthInfo1.maxCountPerKG = float.PositiveInfinity;
      elemGrowthInfo1.minDiffusionCount = int.MaxValue;
      elemGrowthInfo1.diffusionScale = 1f;
      elemGrowthInfo1.minDiffusionInfestationTickCount = byte.MaxValue;
      ElemGrowthInfo elemGrowthInfo2 = elemGrowthInfo1;
      elemGrowthInfoArray1[elementIndex1] = elemGrowthInfo2;
      ElemGrowthInfo[] elemGrowthInfoArray2 = infoArray;
      int elementIndex2 = (int) ElementLoader.GetElementIndex(SimHashes.Vacuum);
      elemGrowthInfo1 = new ElemGrowthInfo();
      elemGrowthInfo1.underPopulationDeathRate = 0.0f;
      elemGrowthInfo1.populationHalfLife = 0.0f;
      elemGrowthInfo1.overPopulationHalfLife = 0.0f;
      elemGrowthInfo1.minCountPerKG = 0.0f;
      elemGrowthInfo1.maxCountPerKG = float.PositiveInfinity;
      elemGrowthInfo1.diffusionScale = 0.0f;
      elemGrowthInfo1.minDiffusionInfestationTickCount = byte.MaxValue;
      ElemGrowthInfo elemGrowthInfo3 = elemGrowthInfo1;
      elemGrowthInfoArray2[elementIndex2] = elemGrowthInfo3;
    }

    protected void InitializeElemExposureArray(
      ref ElemExposureInfo[] infoArray,
      ElemExposureInfo default_value)
    {
      List<Element> elements = ElementLoader.elements;
      infoArray = new ElemExposureInfo[elements.Count];
      for (int index = 0; index < elements.Count; ++index)
      {
        ElemExposureInfo elemExposureInfo = default_value;
        infoArray[index] = elemExposureInfo;
      }
    }

    public float GetGrowthRateForTags(HashSet<Tag> tags, bool overpopulated)
    {
      float growthRateForTags = 1f;
      if (this.growthRules != null)
      {
        for (int index = 0; index < this.growthRules.Count; ++index)
        {
          if (this.growthRules[index] is TagGrowthRule growthRule && tags.Contains(growthRule.tag))
            growthRateForTags *= Disease.HalfLifeToGrowthRate((overpopulated ? growthRule.overPopulationHalfLife : growthRule.populationHalfLife).Value, 1f);
        }
      }
      return growthRateForTags;
    }

    public static float HalfLifeToGrowthRate(float half_life_in_seconds, float dt) => (double) half_life_in_seconds != 0.0 ? ((double) half_life_in_seconds != double.PositiveInfinity ? Mathf.Pow(2f, -1f / (half_life_in_seconds / dt)) : 1f) : 0.0f;

    public static float GrowthRateToHalfLife(float growth_rate) => (double) growth_rate != 0.0 ? ((double) growth_rate != 1.0 ? Mathf.Log(2f, growth_rate) : float.PositiveInfinity) : 0.0f;

    public float CalculateTemperatureHalfLife(float temperature) => Disease.CalculateRangeHalfLife(temperature, ref this.temperatureRange, ref this.temperatureHalfLives);

    public static float CalculateRangeHalfLife(
      float range_value,
      ref Disease.RangeInfo range,
      ref Disease.RangeInfo half_lives)
    {
      int idx1 = 3;
      int idx2 = 3;
      for (int idx3 = 0; idx3 < 4; ++idx3)
      {
        if ((double) range_value <= (double) range.GetValue(idx3))
        {
          idx1 = idx3 - 1;
          idx2 = idx3;
          break;
        }
      }
      if (idx1 < 0)
        idx1 = idx2;
      float f1 = half_lives.GetValue(idx1);
      float f2 = half_lives.GetValue(idx2);
      if (idx1 == 1 && idx2 == 2 || float.IsInfinity(f1) || float.IsInfinity(f2))
        return float.PositiveInfinity;
      float num1 = range.GetValue(idx1);
      double num2 = (double) range.GetValue(idx2);
      float num3 = 0.0f;
      double num4 = (double) num1;
      float num5 = (float) (num2 - num4);
      if ((double) num5 > 0.0)
        num3 = (range_value - num1) / num5;
      return Mathf.Lerp(f1, f2, num3);
    }

    public List<Descriptor> GetQuantitativeDescriptors()
    {
      List<Descriptor> quantitativeDescriptors = new List<Descriptor>();
      quantitativeDescriptors.Add(new Descriptor(string.Format((string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE, (object) GameUtil.GetFormattedTemperature(this.temperatureRange.minViable), (object) GameUtil.GetFormattedTemperature(this.temperatureRange.maxViable)), string.Format((string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.TEMPERATURE_RANGE_TOOLTIP, (object) GameUtil.GetFormattedTemperature(this.temperatureRange.minViable), (object) GameUtil.GetFormattedTemperature(this.temperatureRange.maxViable), (object) GameUtil.GetFormattedTemperature(this.temperatureRange.minGrowth), (object) GameUtil.GetFormattedTemperature(this.temperatureRange.maxGrowth)), (Descriptor.DescriptorType) 3, false));
      quantitativeDescriptors.Add(new Descriptor(string.Format((string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.PRESSURE_RANGE, (object) GameUtil.GetFormattedMass(this.pressureRange.minViable), (object) GameUtil.GetFormattedMass(this.pressureRange.maxViable)), string.Format((string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.PRESSURE_RANGE_TOOLTIP, (object) GameUtil.GetFormattedMass(this.pressureRange.minViable), (object) GameUtil.GetFormattedMass(this.pressureRange.maxViable), (object) GameUtil.GetFormattedMass(this.pressureRange.minGrowth), (object) GameUtil.GetFormattedMass(this.pressureRange.maxGrowth)), (Descriptor.DescriptorType) 3, false));
      List<GrowthRule> rules1 = new List<GrowthRule>();
      List<GrowthRule> rules2 = new List<GrowthRule>();
      List<GrowthRule> rules3 = new List<GrowthRule>();
      List<GrowthRule> rules4 = new List<GrowthRule>();
      List<GrowthRule> rules5 = new List<GrowthRule>();
      foreach (GrowthRule growthRule in this.growthRules)
      {
        if (growthRule.populationHalfLife.HasValue && growthRule.Name() != null)
        {
          if ((double) growthRule.populationHalfLife.Value < 0.0)
            rules1.Add(growthRule);
          else if ((double) growthRule.populationHalfLife.Value == double.PositiveInfinity)
            rules2.Add(growthRule);
          else if ((double) growthRule.populationHalfLife.Value >= 12000.0)
            rules3.Add(growthRule);
          else if ((double) growthRule.populationHalfLife.Value >= 1200.0)
            rules4.Add(growthRule);
          else
            rules5.Add(growthRule);
        }
      }
      quantitativeDescriptors.AddRange((IEnumerable<Descriptor>) this.BuildGrowthInfoDescriptors(rules1, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWS_ON, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWS_ON_TOOLTIP, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWS_TOOLTIP));
      quantitativeDescriptors.AddRange((IEnumerable<Descriptor>) this.BuildGrowthInfoDescriptors(rules2, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.NEUTRAL_ON, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.NEUTRAL_ON_TOOLTIP, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.NEUTRAL_TOOLTIP));
      quantitativeDescriptors.AddRange((IEnumerable<Descriptor>) this.BuildGrowthInfoDescriptors(rules3, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_SLOWLY_ON, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_SLOWLY_ON_TOOLTIP, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_SLOWLY_TOOLTIP));
      quantitativeDescriptors.AddRange((IEnumerable<Descriptor>) this.BuildGrowthInfoDescriptors(rules4, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_ON, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_ON_TOOLTIP, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_TOOLTIP));
      quantitativeDescriptors.AddRange((IEnumerable<Descriptor>) this.BuildGrowthInfoDescriptors(rules5, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_QUICKLY_ON, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_QUICKLY_ON_TOOLTIP, (string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.DIES_QUICKLY_TOOLTIP));
      return quantitativeDescriptors;
    }

    private List<Descriptor> BuildGrowthInfoDescriptors(
      List<GrowthRule> rules,
      string section_text,
      string section_tooltip,
      string item_tooltip)
    {
      List<Descriptor> descriptorList = new List<Descriptor>();
      if (rules.Count > 0)
      {
        descriptorList.Add(new Descriptor(section_text, section_tooltip, (Descriptor.DescriptorType) 3, false));
        for (int index = 0; index < rules.Count; ++index)
          descriptorList.Add(new Descriptor(string.Format((string) DUPLICANTS.DISEASES.DESCRIPTORS.INFO.GROWTH_FORMAT, (object) rules[index].Name()), string.Format(item_tooltip, (object) GameUtil.GetFormattedCycles(Mathf.Abs(rules[index].populationHalfLife.Value))), (Descriptor.DescriptorType) 3, false));
      }
      return descriptorList;
    }

    public struct RangeInfo
    {
      public float minViable;
      public float minGrowth;
      public float maxGrowth;
      public float maxViable;

      public RangeInfo(float min_viable, float min_growth, float max_growth, float max_viable)
      {
        this.minViable = min_viable;
        this.minGrowth = min_growth;
        this.maxGrowth = max_growth;
        this.maxViable = max_viable;
      }

      public void Write(BinaryWriter writer)
      {
        writer.Write(this.minViable);
        writer.Write(this.minGrowth);
        writer.Write(this.maxGrowth);
        writer.Write(this.maxViable);
      }

      public float GetValue(int idx)
      {
        switch (idx)
        {
          case 0:
            return this.minViable;
          case 1:
            return this.minGrowth;
          case 2:
            return this.maxGrowth;
          case 3:
            return this.maxViable;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      public static Disease.RangeInfo Idempotent() => new Disease.RangeInfo(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
    }
  }
}
