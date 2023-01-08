// Decompiled with JetBrains decompiler
// Type: Klei.AI.AttributeInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
  [DebuggerDisplay("{Attribute.Id}")]
  public class AttributeInstance : ModifierInstance<Attribute>
  {
    public Attribute Attribute;
    public System.Action OnDirty;
    public ArrayRef<AttributeModifier> Modifiers;
    public bool hide;

    public string Id => this.Attribute.Id;

    public string Name => this.Attribute.Name;

    public string Description => this.Attribute.Description;

    public float GetBaseValue() => this.Attribute.BaseValue;

    public float GetTotalDisplayValue()
    {
      float baseValue = this.Attribute.BaseValue;
      float num = 0.0f;
      for (int index = 0; index != this.Modifiers.Count; ++index)
      {
        AttributeModifier modifier = this.Modifiers[index];
        if (!modifier.IsMultiplier)
          baseValue += modifier.Value;
        else
          num += modifier.Value;
      }
      if ((double) num != 0.0)
        baseValue += Mathf.Abs(baseValue) * num;
      return baseValue;
    }

    public float GetTotalValue()
    {
      float baseValue = this.Attribute.BaseValue;
      float num = 0.0f;
      for (int index = 0; index != this.Modifiers.Count; ++index)
      {
        AttributeModifier modifier = this.Modifiers[index];
        if (!modifier.UIOnly)
        {
          if (!modifier.IsMultiplier)
            baseValue += modifier.Value;
          else
            num += modifier.Value;
        }
      }
      if ((double) num != 0.0)
        baseValue += Mathf.Abs(baseValue) * num;
      return baseValue;
    }

    public static float GetTotalDisplayValue(Attribute attribute, List<AttributeModifier> modifiers)
    {
      float baseValue = attribute.BaseValue;
      float num = 0.0f;
      for (int index = 0; index != modifiers.Count; ++index)
      {
        AttributeModifier modifier = modifiers[index];
        if (!modifier.IsMultiplier)
          baseValue += modifier.Value;
        else
          num += modifier.Value;
      }
      if ((double) num != 0.0)
        baseValue += Mathf.Abs(baseValue) * num;
      return baseValue;
    }

    public static float GetTotalValue(Attribute attribute, List<AttributeModifier> modifiers)
    {
      float baseValue = attribute.BaseValue;
      float num = 0.0f;
      for (int index = 0; index != modifiers.Count; ++index)
      {
        AttributeModifier modifier = modifiers[index];
        if (!modifier.UIOnly)
        {
          if (!modifier.IsMultiplier)
            baseValue += modifier.Value;
          else
            num += modifier.Value;
        }
      }
      if ((double) num != 0.0)
        baseValue += Mathf.Abs(baseValue) * num;
      return baseValue;
    }

    public float GetModifierContribution(AttributeModifier testModifier)
    {
      if (!testModifier.IsMultiplier)
        return testModifier.Value;
      float baseValue = this.Attribute.BaseValue;
      for (int index = 0; index != this.Modifiers.Count; ++index)
      {
        AttributeModifier modifier = this.Modifiers[index];
        if (!modifier.IsMultiplier)
          baseValue += modifier.Value;
      }
      return baseValue * testModifier.Value;
    }

    public AttributeInstance(GameObject game_object, Attribute attribute)
      : base(game_object, attribute)
    {
      DebugUtil.Assert(attribute != null);
      this.Attribute = attribute;
    }

    public void Add(AttributeModifier modifier)
    {
      this.Modifiers.Add(modifier);
      if (this.OnDirty == null)
        return;
      this.OnDirty();
    }

    public void Remove(AttributeModifier modifier)
    {
      for (int index = 0; index < this.Modifiers.Count; ++index)
      {
        if (this.Modifiers[index] == modifier)
        {
          this.Modifiers.RemoveAt(index);
          if (this.OnDirty == null)
            break;
          this.OnDirty();
          break;
        }
      }
    }

    public void ClearModifiers()
    {
      if (this.Modifiers.Count <= 0)
        return;
      this.Modifiers.Clear();
      if (this.OnDirty == null)
        return;
      this.OnDirty();
    }

    public string GetDescription() => string.Format((string) DUPLICANTS.ATTRIBUTES.VALUE, (object) this.Name, (object) this.GetFormattedValue());

    public string GetFormattedValue() => this.Attribute.formatter.GetFormattedAttribute(this);

    public string GetAttributeValueTooltip() => this.Attribute.GetTooltip(this);
  }
}
