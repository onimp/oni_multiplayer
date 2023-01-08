// Decompiled with JetBrains decompiler
// Type: Klei.AI.DiseaseGrowthRules.ElemGrowthInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Klei.AI.DiseaseGrowthRules
{
  public struct ElemGrowthInfo
  {
    public float underPopulationDeathRate;
    public float populationHalfLife;
    public float overPopulationHalfLife;
    public float diffusionScale;
    public float minCountPerKG;
    public float maxCountPerKG;
    public int minDiffusionCount;
    public byte minDiffusionInfestationTickCount;

    public void Write(BinaryWriter writer)
    {
      writer.Write(this.underPopulationDeathRate);
      writer.Write(this.populationHalfLife);
      writer.Write(this.overPopulationHalfLife);
      writer.Write(this.diffusionScale);
      writer.Write(this.minCountPerKG);
      writer.Write(this.maxCountPerKG);
      writer.Write(this.minDiffusionCount);
      writer.Write(this.minDiffusionInfestationTickCount);
    }

    public static void SetBulk(
      ElemGrowthInfo[] info,
      Func<Element, bool> test,
      ElemGrowthInfo settings)
    {
      List<Element> elements = ElementLoader.elements;
      for (int index = 0; index < elements.Count; ++index)
      {
        if (test(elements[index]))
          info[index] = settings;
      }
    }

    public float CalculateDiseaseCountDelta(int disease_count, float kg, float dt)
    {
      float num1 = this.minCountPerKG * kg;
      float num2 = this.maxCountPerKG * kg;
      return (double) num1 > (double) disease_count || (double) disease_count > (double) num2 ? ((double) disease_count >= (double) num1 ? (Klei.AI.Disease.HalfLifeToGrowthRate(this.overPopulationHalfLife, dt) - 1f) * (float) disease_count : -this.underPopulationDeathRate * dt) : (Klei.AI.Disease.HalfLifeToGrowthRate(this.populationHalfLife, dt) - 1f) * (float) disease_count;
    }
  }
}
