// Decompiled with JetBrains decompiler
// Type: Klei.AI.SlimeGerms
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
  public class SlimeGerms : Disease
  {
    private const float COUGH_FREQUENCY = 20f;
    private const int DISEASE_AMOUNT = 1000;
    public const string ID = "SlimeLung";

    public SlimeGerms(bool statsOnly)
      : base("SlimeLung", (byte) 20, new Disease.RangeInfo(283.15f, 293.15f, 363.15f, 373.15f), new Disease.RangeInfo(10f, 1200f, 1200f, 10f), new Disease.RangeInfo(0.0f, 0.0f, 1000f, 1000f), Disease.RangeInfo.Idempotent(), 2.5f, statsOnly)
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
        maxCountPerKG = new float?(500f),
        overPopulationHalfLife = new float?(1200f),
        minDiffusionCount = new int?(1000),
        diffusionScale = new float?(1f / 1000f),
        minDiffusionInfestationTickCount = new byte?((byte) 1)
      });
      StateGrowthRule g1 = new StateGrowthRule(Element.State.Solid);
      g1.minCountPerKG = new float?(0.4f);
      g1.populationHalfLife = new float?(3000f);
      g1.overPopulationHalfLife = new float?(1200f);
      g1.diffusionScale = new float?(1E-06f);
      g1.minDiffusionCount = new int?(1000000);
      this.AddGrowthRule((GrowthRule) g1);
      ElementGrowthRule g2 = new ElementGrowthRule(SimHashes.SlimeMold);
      g2.underPopulationDeathRate = new float?(0.0f);
      g2.populationHalfLife = new float?(-3000f);
      g2.overPopulationHalfLife = new float?(3000f);
      g2.maxCountPerKG = new float?(4500f);
      g2.diffusionScale = new float?(0.05f);
      this.AddGrowthRule((GrowthRule) g2);
      ElementGrowthRule g3 = new ElementGrowthRule(SimHashes.BleachStone);
      g3.populationHalfLife = new float?(10f);
      g3.overPopulationHalfLife = new float?(10f);
      g3.minDiffusionCount = new int?(100000);
      g3.diffusionScale = new float?(1f / 1000f);
      this.AddGrowthRule((GrowthRule) g3);
      StateGrowthRule g4 = new StateGrowthRule(Element.State.Gas);
      g4.minCountPerKG = new float?(250f);
      g4.populationHalfLife = new float?(12000f);
      g4.overPopulationHalfLife = new float?(1200f);
      g4.maxCountPerKG = new float?(10000f);
      g4.minDiffusionCount = new int?(5100);
      g4.diffusionScale = new float?(0.005f);
      this.AddGrowthRule((GrowthRule) g4);
      ElementGrowthRule g5 = new ElementGrowthRule(SimHashes.ContaminatedOxygen);
      g5.underPopulationDeathRate = new float?(0.0f);
      g5.populationHalfLife = new float?(-300f);
      g5.overPopulationHalfLife = new float?(1200f);
      this.AddGrowthRule((GrowthRule) g5);
      ElementGrowthRule g6 = new ElementGrowthRule(SimHashes.Oxygen);
      g6.populationHalfLife = new float?(1200f);
      g6.overPopulationHalfLife = new float?(10f);
      this.AddGrowthRule((GrowthRule) g6);
      ElementGrowthRule g7 = new ElementGrowthRule(SimHashes.ChlorineGas);
      g7.populationHalfLife = new float?(10f);
      g7.overPopulationHalfLife = new float?(10f);
      g7.minDiffusionCount = new int?(100000);
      g7.diffusionScale = new float?(1f / 1000f);
      this.AddGrowthRule((GrowthRule) g7);
      StateGrowthRule g8 = new StateGrowthRule(Element.State.Liquid);
      g8.minCountPerKG = new float?(0.4f);
      g8.populationHalfLife = new float?(1200f);
      g8.overPopulationHalfLife = new float?(300f);
      g8.maxCountPerKG = new float?(100f);
      g8.diffusionScale = new float?(0.01f);
      this.AddGrowthRule((GrowthRule) g8);
      this.InitializeElemExposureArray(ref this.elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
      this.AddExposureRule(new ExposureRule()
      {
        populationHalfLife = new float?(float.PositiveInfinity)
      });
      ElementExposureRule g9 = new ElementExposureRule(SimHashes.DirtyWater);
      g9.populationHalfLife = new float?(-12000f);
      this.AddExposureRule((ExposureRule) g9);
      ElementExposureRule g10 = new ElementExposureRule(SimHashes.ContaminatedOxygen);
      g10.populationHalfLife = new float?(-12000f);
      this.AddExposureRule((ExposureRule) g10);
      ElementExposureRule g11 = new ElementExposureRule(SimHashes.Oxygen);
      g11.populationHalfLife = new float?(3000f);
      this.AddExposureRule((ExposureRule) g11);
      ElementExposureRule g12 = new ElementExposureRule(SimHashes.ChlorineGas);
      g12.populationHalfLife = new float?(10f);
      this.AddExposureRule((ExposureRule) g12);
    }
  }
}
