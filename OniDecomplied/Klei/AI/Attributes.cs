// Decompiled with JetBrains decompiler
// Type: Klei.AI.Attributes
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  public class Attributes
  {
    public List<AttributeInstance> AttributeTable = new List<AttributeInstance>();
    public GameObject gameObject;

    public IEnumerator<AttributeInstance> GetEnumerator() => (IEnumerator<AttributeInstance>) this.AttributeTable.GetEnumerator();

    public int Count => this.AttributeTable.Count;

    public Attributes(GameObject game_object) => this.gameObject = game_object;

    public AttributeInstance Add(Attribute attribute)
    {
      AttributeInstance attributeInstance = this.Get(attribute.Id);
      if (attributeInstance == null)
      {
        attributeInstance = new AttributeInstance(this.gameObject, attribute);
        this.AttributeTable.Add(attributeInstance);
      }
      return attributeInstance;
    }

    public void Add(AttributeModifier modifier) => this.Get(modifier.AttributeId)?.Add(modifier);

    public void Remove(AttributeModifier modifier)
    {
      if (modifier == null)
        return;
      this.Get(modifier.AttributeId)?.Remove(modifier);
    }

    public float GetValuePercent(string attribute_id)
    {
      float valuePercent = 1f;
      AttributeInstance attributeInstance = this.Get(attribute_id);
      if (attributeInstance != null)
        valuePercent = attributeInstance.GetTotalValue() / attributeInstance.GetBaseValue();
      else
        Debug.LogError((object) ("Could not find attribute " + attribute_id));
      return valuePercent;
    }

    public AttributeInstance Get(string attribute_id)
    {
      for (int index = 0; index < this.AttributeTable.Count; ++index)
      {
        if (this.AttributeTable[index].Id == attribute_id)
          return this.AttributeTable[index];
      }
      return (AttributeInstance) null;
    }

    public AttributeInstance Get(Attribute attribute) => this.Get(attribute.Id);

    public float GetValue(string id)
    {
      float num = 0.0f;
      AttributeInstance attributeInstance = this.Get(id);
      if (attributeInstance != null)
        num = attributeInstance.GetTotalValue();
      else
        Debug.LogError((object) ("Could not find attribute " + id));
      return num;
    }

    public AttributeInstance GetProfession()
    {
      AttributeInstance profession = (AttributeInstance) null;
      foreach (AttributeInstance attributeInstance in this)
      {
        if (attributeInstance.modifier.IsProfession)
        {
          if (profession == null)
            profession = attributeInstance;
          else if ((double) profession.GetTotalValue() < (double) attributeInstance.GetTotalValue())
            profession = attributeInstance;
        }
      }
      return profession;
    }

    public string GetProfessionString(bool longform = true)
    {
      AttributeInstance profession = this.GetProfession();
      return (int) profession.GetTotalValue() == 0 ? string.Format((string) (longform ? UI.ATTRIBUTELEVEL : UI.ATTRIBUTELEVEL_SHORT), (object) 0, (object) DUPLICANTS.ATTRIBUTES.UNPROFESSIONAL_NAME) : string.Format((string) (longform ? UI.ATTRIBUTELEVEL : UI.ATTRIBUTELEVEL_SHORT), (object) (int) profession.GetTotalValue(), (object) profession.modifier.ProfessionName);
    }

    public string GetProfessionDescriptionString()
    {
      AttributeInstance profession = this.GetProfession();
      return (int) profession.GetTotalValue() == 0 ? (string) DUPLICANTS.ATTRIBUTES.UNPROFESSIONAL_DESC : string.Format((string) DUPLICANTS.ATTRIBUTES.PROFESSION_DESC, (object) profession.modifier.Name);
    }
  }
}
