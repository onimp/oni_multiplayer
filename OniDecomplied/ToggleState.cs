// Decompiled with JetBrains decompiler
// Type: ToggleState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public struct ToggleState
{
  public string Name;
  public string on_click_override_sound_path;
  public string on_release_override_sound_path;
  public Sprite sprite;
  public Color color;
  public Color color_on_hover;
  public bool use_color_on_hover;
  public bool use_rect_margins;
  public Vector2 rect_margins;
  public StatePresentationSetting[] additional_display_settings;
}
