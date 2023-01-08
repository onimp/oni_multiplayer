// Decompiled with JetBrains decompiler
// Type: Klei.AI.DiseaseGrowthRules.TagGrowthRule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Klei.AI.DiseaseGrowthRules
{
  public class TagGrowthRule : GrowthRule
  {
    public Tag tag;

    public TagGrowthRule(Tag tag) => this.tag = tag;

    public override bool Test(Element e) => e.HasTag(this.tag);

    public override string Name() => this.tag.ProperName();
  }
}
