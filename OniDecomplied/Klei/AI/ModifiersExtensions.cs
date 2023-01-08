// Decompiled with JetBrains decompiler
// Type: Klei.AI.ModifiersExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Klei.AI
{
  public static class ModifiersExtensions
  {
    public static Attributes GetAttributes(this KMonoBehaviour cmp) => ((Component) cmp).gameObject.GetAttributes();

    public static Attributes GetAttributes(this GameObject go)
    {
      Modifiers component = go.GetComponent<Modifiers>();
      return Object.op_Inequality((Object) component, (Object) null) ? component.attributes : (Attributes) null;
    }

    public static Amounts GetAmounts(this KMonoBehaviour cmp) => cmp is Modifiers ? ((Modifiers) cmp).amounts : ((Component) cmp).gameObject.GetAmounts();

    public static Amounts GetAmounts(this GameObject go)
    {
      Modifiers component = go.GetComponent<Modifiers>();
      return Object.op_Inequality((Object) component, (Object) null) ? component.amounts : (Amounts) null;
    }

    public static Sicknesses GetSicknesses(this KMonoBehaviour cmp) => ((Component) cmp).gameObject.GetSicknesses();

    public static Sicknesses GetSicknesses(this GameObject go)
    {
      Modifiers component = go.GetComponent<Modifiers>();
      return Object.op_Inequality((Object) component, (Object) null) ? component.sicknesses : (Sicknesses) null;
    }
  }
}
