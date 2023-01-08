// Decompiled with JetBrains decompiler
// Type: CellEventInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;

[SerializationConfig]
public class CellEventInstance : EventInstanceBase, ISaveLoadable
{
  [Serialize]
  public int cell;
  [Serialize]
  public int data;
  [Serialize]
  public int data2;

  public CellEventInstance(int cell, int data, int data2, CellEvent ev)
    : base((EventBase) ev)
  {
    this.cell = cell;
    this.data = data;
    this.data2 = data2;
  }
}
