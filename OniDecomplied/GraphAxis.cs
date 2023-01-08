// Decompiled with JetBrains decompiler
// Type: GraphAxis
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

[Serializable]
public struct GraphAxis
{
  public string name;
  public float min_value;
  public float max_value;
  public float guide_frequency;

  public float range => this.max_value - this.min_value;
}
