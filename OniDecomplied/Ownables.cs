// Decompiled with JetBrains decompiler
// Type: Ownables
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

[SerializationConfig]
public class Ownables : Assignables
{
  protected override void OnSpawn() => base.OnSpawn();

  public void UnassignAll()
  {
    foreach (AssignableSlotInstance slot in this.slots)
    {
      if (Object.op_Inequality((Object) slot.assignable, (Object) null))
        slot.assignable.Unassign();
    }
  }
}
