// Decompiled with JetBrains decompiler
// Type: ToiletTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class ToiletTracker : WorldTracker
{
  public ToiletTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData() => throw new NotImplementedException();

  public override string FormatValueString(float value) => value.ToString();
}
