// Decompiled with JetBrains decompiler
// Type: Klei.AI.DiseaseGrowthRules.ElementGrowthRule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Klei.AI.DiseaseGrowthRules
{
  public class ElementGrowthRule : GrowthRule
  {
    public SimHashes element;

    public ElementGrowthRule(SimHashes element) => this.element = element;

    public override bool Test(Element e) => e.id == this.element;

    public override string Name() => ElementLoader.FindElementByHash(this.element).name;
  }
}
