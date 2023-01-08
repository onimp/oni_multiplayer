// Decompiled with JetBrains decompiler
// Type: Death
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class Death : Resource
{
  public string preAnim;
  public string loopAnim;
  public string sound;
  public string description;

  public Death(
    string id,
    ResourceSet parent,
    string name,
    string description,
    string pre_anim,
    string loop_anim)
    : base(id, parent, name)
  {
    this.preAnim = pre_anim;
    this.loopAnim = loop_anim;
    this.description = description;
  }
}
