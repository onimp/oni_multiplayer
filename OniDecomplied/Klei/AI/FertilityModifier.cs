// Decompiled with JetBrains decompiler
// Type: Klei.AI.FertilityModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace Klei.AI
{
  public class FertilityModifier : Resource
  {
    public string Description;
    public Tag TargetTag;
    public Func<string, string> TooltipCB;
    public FertilityModifier.FertilityModFn ApplyFunction;

    public FertilityModifier(
      string id,
      Tag targetTag,
      string name,
      string description,
      Func<string, string> tooltipCB,
      FertilityModifier.FertilityModFn applyFunction)
      : base(id, name)
    {
      this.Description = description;
      this.TargetTag = targetTag;
      this.TooltipCB = tooltipCB;
      this.ApplyFunction = applyFunction;
    }

    public string GetTooltip() => this.TooltipCB != null ? this.TooltipCB(this.Description) : this.Description;

    public delegate void FertilityModFn(FertilityMonitor.Instance inst, Tag eggTag);
  }
}
