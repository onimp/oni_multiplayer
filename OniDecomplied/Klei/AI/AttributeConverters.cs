// Decompiled with JetBrains decompiler
// Type: Klei.AI.AttributeConverters
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  [AddComponentMenu("KMonoBehaviour/scripts/AttributeConverters")]
  public class AttributeConverters : KMonoBehaviour
  {
    public List<AttributeConverterInstance> converters = new List<AttributeConverterInstance>();

    public int Count => this.converters.Count;

    protected virtual void OnPrefabInit()
    {
      foreach (AttributeInstance attribute in this.GetAttributes())
      {
        foreach (AttributeConverter converter in attribute.Attribute.converters)
          this.converters.Add(new AttributeConverterInstance(((Component) this).gameObject, converter, attribute));
      }
    }

    public AttributeConverterInstance Get(AttributeConverter converter)
    {
      foreach (AttributeConverterInstance converter1 in this.converters)
      {
        if (converter1.converter == converter)
          return converter1;
      }
      return (AttributeConverterInstance) null;
    }

    public AttributeConverterInstance GetConverter(string id)
    {
      foreach (AttributeConverterInstance converter in this.converters)
      {
        if (converter.converter.Id == id)
          return converter;
      }
      return (AttributeConverterInstance) null;
    }
  }
}
