// Decompiled with JetBrains decompiler
// Type: Klei.AI.ZombieSpores
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
  public class ZombieSpores : Disease
  {
    public const string ID = "ZombieSpores";

    public ZombieSpores(bool statsOnly)
      : base(nameof (ZombieSpores), (byte) 50, new Disease.RangeInfo(168.15f, 258.15f, 513.15f, 563.15f), new Disease.RangeInfo(10f, 1200f, 1200f, 10f), new Disease.RangeInfo(0.0f, 0.0f, 1000f, 1000f), Disease.RangeInfo.Idempotent(), 1f, statsOnly)
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
      SimHashes[] simHashesArray1 = new SimHashes[2]
      {
        SimHashes.Carbon,
        SimHashes.Diamond
      };
      foreach (SimHashes element in simHashesArray1)
      {
        ElementGrowthRule g2 = new ElementGrowthRule(element);
        g2.underPopulationDeathRate = new float?(0.0f);
        g2.populationHalfLife = new float?(float.PositiveInfinity);
        g2.overPopulationHalfLife = new float?(3000f);
        g2.maxCountPerKG = new float?(1000f);
        g2.diffusionScale = new float?(0.005f);
        this.AddGrowthRule((GrowthRule) g2);
      }
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
      SimHashes[] simHashesArray2 = new SimHashes[3]
      {
        SimHashes.CarbonDioxide,
        SimHashes.Methane,
        SimHashes.SourGas
      };
      foreach (SimHashes element in simHashesArray2)
      {
        ElementGrowthRule g5 = new ElementGrowthRule(element);
        g5.underPopulationDeathRate = new float?(0.0f);
        g5.populationHalfLife = new float?(float.PositiveInfinity);
        g5.overPopulationHalfLife = new float?(6000f);
        this.AddGrowthRule((GrowthRule) g5);
      }
      ElementGrowthRule g6 = new ElementGrowthRule(SimHashes.ChlorineGas);
      g6.populationHalfLife = new float?(10f);
      g6.overPopulationHalfLife = new float?(10f);
      g6.minDiffusionCount = new int?(100000);
      g6.diffusionScale = new float?(1f / 1000f);
      this.AddGrowthRule((GrowthRule) g6);
      StateGrowthRule g7 = new StateGrowthRule(Element.State.Liquid);
      g7.minCountPerKG = new float?(0.4f);
      g7.populationHalfLife = new float?(1200f);
      g7.overPopulationHalfLife = new float?(300f);
      g7.maxCountPerKG = new float?(100f);
      g7.diffusionScale = new float?(0.01f);
      this.AddGrowthRule((GrowthRule) g7);
      SimHashes[] simHashesArray3 = new SimHashes[4]
      {
        SimHashes.CrudeOil,
        SimHashes.Petroleum,
        SimHashes.Naphtha,
        SimHashes.LiquidMethane
      };
      foreach (SimHashes element in simHashesArray3)
      {
        ElementGrowthRule g8 = new ElementGrowthRule(element);
        g8.populationHalfLife = new float?(float.PositiveInfinity);
        g8.overPopulationHalfLife = new float?(6000f);
        g8.maxCountPerKG = new float?(1000f);
        g8.diffusionScale = new float?(0.005f);
        this.AddGrowthRule((GrowthRule) g8);
      }
      ElementGrowthRule g9 = new ElementGrowthRule(SimHashes.Chlorine);
      g9.populationHalfLife = new float?(10f);
      g9.overPopulationHalfLife = new float?(10f);
      g9.minDiffusionCount = new int?(100000);
      g9.diffusionScale = new float?(1f / 1000f);
      this.AddGrowthRule((GrowthRule) g9);
      this.InitializeElemExposureArray(ref this.elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
      this.AddExposureRule(new ExposureRule()
      {
        populationHalfLife = new float?(float.PositiveInfinity)
      });
      ElementExposureRule g10 = new ElementExposureRule(SimHashes.Chlorine);
      g10.populationHalfLife = new float?(10f);
      this.AddExposureRule((ExposureRule) g10);
      ElementExposureRule g11 = new ElementExposureRule(SimHashes.ChlorineGas);
      g11.populationHalfLife = new float?(10f);
      this.AddExposureRule((ExposureRule) g11);
    }
  }
}
