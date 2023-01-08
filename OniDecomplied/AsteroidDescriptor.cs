// Decompiled with JetBrains decompiler
// Type: AsteroidDescriptor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public struct AsteroidDescriptor
{
  public string text;
  public string tooltip;
  public List<Tuple<string, Color, float>> bands;
  public Color associatedColor;
  public string associatedIcon;

  public AsteroidDescriptor(
    string text,
    string tooltip,
    Color associatedColor,
    List<Tuple<string, Color, float>> bands = null,
    string associatedIcon = null)
  {
    this.text = text;
    this.tooltip = tooltip;
    this.associatedColor = associatedColor;
    this.bands = bands;
    this.associatedIcon = associatedIcon;
  }
}
