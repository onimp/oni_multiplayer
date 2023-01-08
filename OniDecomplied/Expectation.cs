// Decompiled with JetBrains decompiler
// Type: Expectation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class Expectation
{
  public string id { get; protected set; }

  public string name { get; protected set; }

  public string description { get; protected set; }

  public Action<MinionResume> OnApply { get; protected set; }

  public Action<MinionResume> OnRemove { get; protected set; }

  public Expectation(
    string id,
    string name,
    string description,
    Action<MinionResume> OnApply,
    Action<MinionResume> OnRemove)
  {
    this.id = id;
    this.name = name;
    this.description = description;
    this.OnApply = OnApply;
    this.OnRemove = OnRemove;
  }
}
