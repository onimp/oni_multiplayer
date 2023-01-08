// Decompiled with JetBrains decompiler
// Type: Klei.AI.RadiationPoisoning
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI.DiseaseGrowthRules;

namespace Klei.AI
{
  public class RadiationPoisoning : Disease
  {
    public const string ID = "RadiationSickness";

    public RadiationPoisoning(bool statsOnly)
      : base("RadiationSickness", (byte) 100, Disease.RangeInfo.Idempotent(), Disease.RangeInfo.Idempotent(), Disease.RangeInfo.Idempotent(), Disease.RangeInfo.Idempotent(), 0.0f, statsOnly)
    {
    }

    protected override void PopulateElemGrowthInfo()
    {
      this.InitializeElemGrowthArray(ref this.elemGrowthInfo, Disease.DEFAULT_GROWTH_INFO);
      this.AddGrowthRule(new GrowthRule()
      {
        underPopulationDeathRate = new float?(0.0f),
        minCountPerKG = new float?(0.0f),
        populationHalfLife = new float?(600f),
        maxCountPerKG = new float?(float.PositiveInfinity),
        overPopulationHalfLife = new float?(600f),
        minDiffusionCount = new int?(10000),
        diffusionScale = new float?(0.0f),
        minDiffusionInfestationTickCount = new byte?((byte) 1)
      });
      this.InitializeElemExposureArray(ref this.elemExposureInfo, Disease.DEFAULT_EXPOSURE_INFO);
    }
  }
}
