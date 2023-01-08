// Decompiled with JetBrains decompiler
// Type: Klei.AI.DiseaseGrowthRules.StateGrowthRule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Klei.AI.DiseaseGrowthRules
{
  public class StateGrowthRule : GrowthRule
  {
    public Element.State state;

    public StateGrowthRule(Element.State state) => this.state = state;

    public override bool Test(Element e) => e.IsState(this.state);

    public override string Name() => Element.GetStateString(this.state);
  }
}
