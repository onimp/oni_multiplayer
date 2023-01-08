// Decompiled with JetBrains decompiler
// Type: Klei.AI.DiseaseGrowthRules.CompositeExposureRule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Klei.AI.DiseaseGrowthRules
{
  public class CompositeExposureRule
  {
    public string name;
    public float populationHalfLife;

    public string Name() => this.name;

    public void Overlay(ExposureRule rule)
    {
      if (rule.populationHalfLife.HasValue)
        this.populationHalfLife = rule.populationHalfLife.Value;
      this.name = rule.Name();
    }

    public float GetHalfLifeForCount(int count) => this.populationHalfLife;
  }
}
