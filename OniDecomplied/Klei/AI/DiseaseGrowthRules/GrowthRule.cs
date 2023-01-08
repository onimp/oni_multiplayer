// Decompiled with JetBrains decompiler
// Type: Klei.AI.DiseaseGrowthRules.GrowthRule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Klei.AI.DiseaseGrowthRules
{
  public class GrowthRule
  {
    public float? underPopulationDeathRate;
    public float? populationHalfLife;
    public float? overPopulationHalfLife;
    public float? diffusionScale;
    public float? minCountPerKG;
    public float? maxCountPerKG;
    public int? minDiffusionCount;
    public byte? minDiffusionInfestationTickCount;

    public void Apply(ElemGrowthInfo[] infoList)
    {
      List<Element> elements = ElementLoader.elements;
      for (int index = 0; index < elements.Count; ++index)
      {
        Element e = elements[index];
        if (e.id != SimHashes.Vacuum && this.Test(e))
        {
          ElemGrowthInfo info = infoList[index];
          if (this.underPopulationDeathRate.HasValue)
            info.underPopulationDeathRate = this.underPopulationDeathRate.Value;
          if (this.populationHalfLife.HasValue)
            info.populationHalfLife = this.populationHalfLife.Value;
          if (this.overPopulationHalfLife.HasValue)
            info.overPopulationHalfLife = this.overPopulationHalfLife.Value;
          if (this.diffusionScale.HasValue)
            info.diffusionScale = this.diffusionScale.Value;
          if (this.minCountPerKG.HasValue)
            info.minCountPerKG = this.minCountPerKG.Value;
          if (this.maxCountPerKG.HasValue)
            info.maxCountPerKG = this.maxCountPerKG.Value;
          if (this.minDiffusionCount.HasValue)
            info.minDiffusionCount = this.minDiffusionCount.Value;
          if (this.minDiffusionInfestationTickCount.HasValue)
            info.minDiffusionInfestationTickCount = this.minDiffusionInfestationTickCount.Value;
          infoList[index] = info;
        }
      }
    }

    public virtual bool Test(Element e) => true;

    public virtual string Name() => (string) null;
  }
}
