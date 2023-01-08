// Decompiled with JetBrains decompiler
// Type: SicknessExposureInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

[Serializable]
public struct SicknessExposureInfo
{
  public string sicknessID;
  public string sourceInfo;

  public SicknessExposureInfo(string id, string infection_source_info)
  {
    this.sicknessID = id;
    this.sourceInfo = infection_source_info;
  }
}
