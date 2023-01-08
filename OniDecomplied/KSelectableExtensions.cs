// Decompiled with JetBrains decompiler
// Type: KSelectableExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class KSelectableExtensions
{
  public static string GetProperName(this Component cmp) => Object.op_Inequality((Object) cmp, (Object) null) && Object.op_Inequality((Object) cmp.gameObject, (Object) null) ? cmp.gameObject.GetProperName() : "";

  public static string GetProperName(this GameObject go)
  {
    if (Object.op_Inequality((Object) go, (Object) null))
    {
      KSelectable component = go.GetComponent<KSelectable>();
      if (Object.op_Inequality((Object) component, (Object) null))
        return component.GetName();
    }
    return "";
  }

  public static string GetProperName(this KSelectable cmp) => Object.op_Inequality((Object) cmp, (Object) null) ? cmp.GetName() : "";
}
