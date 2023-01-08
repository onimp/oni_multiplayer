// Decompiled with JetBrains decompiler
// Type: Klei.AI.AttributeConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Klei.AI
{
  public class AttributeConverter : Resource
  {
    public string description;
    public float multiplier;
    public float baseValue;
    public Attribute attribute;
    public IAttributeFormatter formatter;

    public AttributeConverter(
      string id,
      string name,
      string description,
      float multiplier,
      float base_value,
      Attribute attribute,
      IAttributeFormatter formatter = null)
      : base(id, name)
    {
      this.description = description;
      this.multiplier = multiplier;
      this.baseValue = base_value;
      this.attribute = attribute;
      this.formatter = formatter;
    }

    public AttributeConverterInstance Lookup(Component cmp) => this.Lookup(cmp.gameObject);

    public AttributeConverterInstance Lookup(GameObject go)
    {
      AttributeConverters component = go.GetComponent<AttributeConverters>();
      return Object.op_Inequality((Object) component, (Object) null) ? component.Get(this) : (AttributeConverterInstance) null;
    }

    public string DescriptionFromAttribute(float value, GameObject go)
    {
      string text = this.formatter == null ? (this.attribute.formatter == null ? GameUtil.GetFormattedSimple(value) : this.attribute.formatter.GetFormattedValue(value, this.attribute.formatter.DeltaTimeSlice)) : this.formatter.GetFormattedValue(value, this.formatter.DeltaTimeSlice);
      return text != null ? string.Format(this.description, (object) GameUtil.AddPositiveSign(text, (double) value > 0.0)) : (string) null;
    }
  }
}
