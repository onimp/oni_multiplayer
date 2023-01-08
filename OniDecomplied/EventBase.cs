// Decompiled with JetBrains decompiler
// Type: EventBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class EventBase : Resource
{
  public int hash;

  public EventBase(string id)
    : base(id, id)
  {
    this.hash = Hash.SDBMLower(id);
  }

  public virtual string GetDescription(EventInstanceBase ev) => "";
}
