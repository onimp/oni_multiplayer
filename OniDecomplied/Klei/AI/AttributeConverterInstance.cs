// Decompiled with JetBrains decompiler
// Type: Klei.AI.AttributeConverterInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Klei.AI
{
  public class AttributeConverterInstance : ModifierInstance<AttributeConverter>
  {
    public AttributeConverter converter;
    public AttributeInstance attributeInstance;

    public AttributeConverterInstance(
      GameObject game_object,
      AttributeConverter converter,
      AttributeInstance attribute_instance)
      : base(game_object, converter)
    {
      this.converter = converter;
      this.attributeInstance = attribute_instance;
    }

    public float Evaluate() => this.converter.multiplier * this.attributeInstance.GetTotalValue() + this.converter.baseValue;

    public string DescriptionFromAttribute(float value, GameObject go) => this.converter.DescriptionFromAttribute(this.Evaluate(), go);
  }
}
