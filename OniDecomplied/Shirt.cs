// Decompiled with JetBrains decompiler
// Type: Shirt
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class Shirt : Resource
{
  public HashedString hash;

  public Shirt(string id)
    : base(id, (ResourceSet) null, (string) null)
  {
    this.hash = new HashedString(id);
  }
}
