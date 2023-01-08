// Decompiled with JetBrains decompiler
// Type: LegendEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class LegendEntry
{
  public string name;
  public string desc;
  public string desc_arg;
  public Color colour;
  public Sprite sprite;
  public bool displaySprite;

  public LegendEntry(
    string name,
    string desc,
    Color colour,
    string desc_arg = null,
    Sprite sprite = null,
    bool displaySprite = true)
  {
    this.name = name;
    this.desc = desc;
    this.colour = colour;
    this.desc_arg = desc_arg;
    this.sprite = Object.op_Equality((Object) sprite, (Object) null) ? Assets.instance.LegendColourBox : sprite;
    this.displaySprite = displaySprite;
  }
}
