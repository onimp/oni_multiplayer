// Decompiled with JetBrains decompiler
// Type: ExposureType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class ExposureType
{
  public string germ_id;
  public string sickness_id;
  public string infection_effect;
  public int exposure_threshold;
  public bool infect_immediately;
  public List<string> required_traits;
  public List<string> excluded_traits;
  public List<string> excluded_effects;
  public int base_resistance;
}
