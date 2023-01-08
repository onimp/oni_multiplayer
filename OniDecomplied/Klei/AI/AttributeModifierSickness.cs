// Decompiled with JetBrains decompiler
// Type: Klei.AI.AttributeModifierSickness
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  public class AttributeModifierSickness : Sickness.SicknessComponent
  {
    private AttributeModifier[] attributeModifiers;

    public AttributeModifierSickness(AttributeModifier[] attribute_modifiers) => this.attributeModifiers = attribute_modifiers;

    public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
    {
      Attributes attributes = go.GetAttributes();
      for (int index = 0; index < this.attributeModifiers.Length; ++index)
      {
        AttributeModifier attributeModifier = this.attributeModifiers[index];
        attributes.Add(attributeModifier);
      }
      return (object) null;
    }

    public override void OnCure(GameObject go, object instance_data)
    {
      Attributes attributes = go.GetAttributes();
      for (int index = 0; index < this.attributeModifiers.Length; ++index)
      {
        AttributeModifier attributeModifier = this.attributeModifiers[index];
        attributes.Remove(attributeModifier);
      }
    }

    public AttributeModifier[] Modifers => this.attributeModifiers;

    public override List<Descriptor> GetSymptoms()
    {
      List<Descriptor> symptoms = new List<Descriptor>();
      foreach (AttributeModifier attributeModifier in this.attributeModifiers)
      {
        Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
        symptoms.Add(new Descriptor(string.Format((string) DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS, (object) attribute.Name, (object) attributeModifier.GetFormattedString()), string.Format((string) DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS_TOOLTIP, (object) attribute.Name, (object) attributeModifier.GetFormattedString()), (Descriptor.DescriptorType) 6, false));
      }
      return symptoms;
    }
  }
}
