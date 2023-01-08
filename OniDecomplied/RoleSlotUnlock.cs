// Decompiled with JetBrains decompiler
// Type: RoleSlotUnlock
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public class RoleSlotUnlock
{
  public string id { get; protected set; }

  public string name { get; protected set; }

  public string description { get; protected set; }

  public List<Tuple<string, int>> slots { get; protected set; }

  public Func<bool> isSatisfied { get; protected set; }

  public RoleSlotUnlock(
    string id,
    string name,
    string description,
    List<Tuple<string, int>> slots,
    Func<bool> isSatisfied)
  {
    this.id = id;
    this.name = name;
    this.description = description;
    this.slots = slots;
    this.isSatisfied = isSatisfied;
  }
}
