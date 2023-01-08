// Decompiled with JetBrains decompiler
// Type: Face
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class Face : Resource
{
  public HashedString hash;
  public HashedString headFXHash;
  private const string SYMBOL_PREFIX = "headfx_";

  public Face(string id, string headFXSymbol = null)
    : base(id, (ResourceSet) null, (string) null)
  {
    this.hash = new HashedString(id);
    this.headFXHash = HashedString.op_Implicit(headFXSymbol);
  }
}
