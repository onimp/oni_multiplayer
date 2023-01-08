// Decompiled with JetBrains decompiler
// Type: Klei.AI.DiseaseGrowthRules.ExposureRule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Klei.AI.DiseaseGrowthRules
{
  public class ExposureRule
  {
    public float? populationHalfLife;

    public void Apply(ElemExposureInfo[] infoList)
    {
      List<Element> elements = ElementLoader.elements;
      for (int index = 0; index < elements.Count; ++index)
      {
        if (this.Test(elements[index]))
        {
          ElemExposureInfo info = infoList[index];
          if (this.populationHalfLife.HasValue)
            info.populationHalfLife = this.populationHalfLife.Value;
          infoList[index] = info;
        }
      }
    }

    public virtual bool Test(Element e) => true;

    public virtual string Name() => (string) null;
  }
}
