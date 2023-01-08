// Decompiled with JetBrains decompiler
// Type: AssignableSlot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{Id}")]
[Serializable]
public class AssignableSlot : Resource
{
  public bool showInUI = true;

  public AssignableSlot(string id, string name, bool showInUI = true)
    : base(id, name)
  {
    this.showInUI = showInUI;
  }

  public AssignableSlotInstance Lookup(GameObject go)
  {
    Assignables component = go.GetComponent<Assignables>();
    return Object.op_Inequality((Object) component, (Object) null) ? component.GetSlot(this) : (AssignableSlotInstance) null;
  }
}
