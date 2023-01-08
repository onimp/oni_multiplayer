// Decompiled with JetBrains decompiler
// Type: EffectorEntryDecibel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

internal struct EffectorEntryDecibel
{
  public string name;
  public int count;
  public float value;

  public EffectorEntryDecibel(string name, float value)
  {
    this.name = name;
    this.value = value;
    this.count = 1;
  }
}
