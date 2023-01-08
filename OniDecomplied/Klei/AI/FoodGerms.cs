// Decompiled with JetBrains decompiler
// Type: Klei.AI.FoodGerms
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
  public class FoodGerms : Disease
  {
    public const string ID = "FoodPoisoning";
    private const float VOMIT_FREQUENCY = 200f;

    public FoodGerms(bool statsOnly)
      : base("FoodPoisoning", (byte) 10, new Disease.RangeInfo(248.15f, 278.15f, 313.15f, 348.15f), new Disease.RangeInfo(10f, 1200f, 1200f, 10f), new Disease.RangeInfo(0.0f, 0.0f, 1000f, 1000f), Disease.RangeInfo.Idempotent(), 2.5f, statsOnly)
    {
    }

    protected override void PopulateElemGrowthInfo()
    {
      this.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
      this.AddGrowthRule(new GrowthRule()
      {
        underPopulationDeathRate = new float?(2.66666675f),
        minCountPerKG = new float?(0.4f),
        populationHalfLife = new float?(12000f),
        maxCountPerKG = new float?(1000f),
        overPopulationHalfLife = new float?(3000f),
        minDiffusionCount = new int?(1000),
        diffusionScale = new float?(1f / 1000f),
        minDiffusionInfestationTickCount = new byte?((byte) 1)
      });
      StateGrowthRule g1 = new StateGrowthRule(Element.State.Solid);
      g1.minCountPerKG = new float?(0.4f);
      g1.populationHalfLife = new float?(300f);
      g1.overPopulationHalfLife = new float?(10f);
      g1.minDiffusionCount = new int?(1000000);
      this.AddGrowthRule((GrowthRule) g1);
      ElementGrowthRule g2 = new ElementGrowthRule(SimHashes.ToxicSand);
      g2.populationHalfLife = new float?(float.PositiveInfinity);
      g2.overPopulationHalfLife = new float?(12000f);
      this.AddGrowthRule((GrowthRule) g2);
      ElementGrowthRule g3 = new ElementGrowthRule(SimHashes.Creature);
      g3.populationHalfLife = new float?(float.PositiveInfinity);
      g3.maxCountPerKG = new float?(4000f);
      g3.overPopulationHalfLife = new float?(3000f);
      this.AddGrowthRule((GrowthRule) g3);
      ElementGrowthRule g4 = new ElementGrowthRule(SimHashes.BleachStone);
      g4.populationHalfLife = new float?(10f);
      g4.overPopulationHalfLife = new float?(10f);
      g4.diffusionScale = new float?(1f / 1000f);
      this.AddGrowthRule((GrowthRule) g4);
      StateGrowthRule g5 = new StateGrowthRule(Element.State.Gas);
      g5.minCountPerKG = new float?(250f);
      g5.populationHalfLife = new float?(1200f);
      g5.overPopulationHalfLife = new float?(300f);
      g5.diffusionScale = new float?(0.01f);
      this.AddGrowthRule((GrowthRule) g5);
      ElementGrowthRule g6 = new ElementGrowthRule(SimHashes.ContaminatedOxygen);
      g6.populationHalfLife = new float?(12000f);
      g6.maxCountPerKG = new float?(10000f);
      g6.overPopulationHalfLife = new float?(3000f);
      g6.diffusionScale = new float?(0.05f);
      this.AddGrowthRule((GrowthRule) g6);
      ElementGrowthRule g7 = new ElementGrowthRule(SimHashes.ChlorineGas);
      g7.populationHalfLife = new float?(10f);
      g7.overPopulationHalfLife = new float?(10f);
      g7.minDiffusionCount = new int?(1000000);
      this.AddGrowthRule((GrowthRule) g7);
      StateGrowthRule g8 = new StateGrowthRule(Element.State.Liquid);
      g8.minCountPerKG = new float?(0.4f);
      g8.populationHalfLife = new float?(12000f);
      g8.maxCountPerKG = new float?(5000f);
      g8.diffusionScale = new float?(0.2f);
      this.AddGrowthRule((GrowthRule) g8);
      ElementGrowthRule g9 = new ElementGrowthRule(SimHashes.DirtyWater);
      g9.populationHalfLife = new float?(-12000f);
      g9.overPopulationHalfLife = new float?(12000f);
      this.AddGrowthRule((GrowthRule) g9);
      TagGrowthRule g10 = new TagGrowthRule(GameTags.Edible);
      g10.populationHalfLife = new float?(-12000f);
      g10.overPopulationHalfLife = new float?(float.PositiveInfinity);
      this.AddGrowthRule((GrowthRule) g10);
      TagGrowthRule g11 = new TagGrowthRule(GameTags.Pickled);
      g11.populationHalfLife = new float?(10f);
      g11.overPopulationHalfLife = new float?(10f);
      this.AddGrowthRule((GrowthRule) g11);
      this.InitializeElemExposureArray(ref this.elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
      this.AddExposureRule(new ExposureRule()
      {
        populationHalfLife = new float?(float.PositiveInfinity)
      });
      ElementExposureRule g12 = new ElementExposureRule(SimHashes.DirtyWater);
      g12.populationHalfLife = new float?(-12000f);
      this.AddExposureRule((ExposureRule) g12);
      ElementExposureRule g13 = new ElementExposureRule(SimHashes.ContaminatedOxygen);
      g13.populationHalfLife = new float?(-12000f);
      this.AddExposureRule((ExposureRule) g13);
      ElementExposureRule g14 = new ElementExposureRule(SimHashes.ChlorineGas);
      g14.populationHalfLife = new float?(10f);
      this.AddExposureRule((ExposureRule) g14);
    }
  }
}
