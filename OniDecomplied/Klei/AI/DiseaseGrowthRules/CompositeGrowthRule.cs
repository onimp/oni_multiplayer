// Decompiled with JetBrains decompiler
// Type: Klei.AI.DiseaseGrowthRules.CompositeGrowthRule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Klei.AI.DiseaseGrowthRules
{
  public class CompositeGrowthRule
  {
    public string name;
    public float underPopulationDeathRate;
    public float populationHalfLife;
    public float overPopulationHalfLife;
    public float diffusionScale;
    public float minCountPerKG;
    public float maxCountPerKG;
    public int minDiffusionCount;
    public byte minDiffusionInfestationTickCount;

    public string Name() => this.name;

    public void Overlay(GrowthRule rule)
    {
      if (rule.underPopulationDeathRate.HasValue)
        this.underPopulationDeathRate = rule.underPopulationDeathRate.Value;
      if (rule.populationHalfLife.HasValue)
        this.populationHalfLife = rule.populationHalfLife.Value;
      if (rule.overPopulationHalfLife.HasValue)
        this.overPopulationHalfLife = rule.overPopulationHalfLife.Value;
      if (rule.diffusionScale.HasValue)
        this.diffusionScale = rule.diffusionScale.Value;
      if (rule.minCountPerKG.HasValue)
        this.minCountPerKG = rule.minCountPerKG.Value;
      if (rule.maxCountPerKG.HasValue)
        this.maxCountPerKG = rule.maxCountPerKG.Value;
      if (rule.minDiffusionCount.HasValue)
        this.minDiffusionCount = rule.minDiffusionCount.Value;
      if (rule.minDiffusionInfestationTickCount.HasValue)
        this.minDiffusionInfestationTickCount = rule.minDiffusionInfestationTickCount.Value;
      this.name = rule.Name();
    }

    public float GetHalfLifeForCount(int count, float kg)
    {
      int num1 = (int) ((double) this.minCountPerKG * (double) kg);
      int num2 = (int) ((double) this.maxCountPerKG * (double) kg);
      return count < num1 || count < num2 ? this.populationHalfLife : this.overPopulationHalfLife;
    }
  }
}
