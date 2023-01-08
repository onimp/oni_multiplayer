// Decompiled with JetBrains decompiler
// Type: Dream
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class Dream : Resource
{
  public string BackgroundAnim;
  public Sprite[] Icons;
  public float secondPerImage = 2.4f;

  public Dream(string id, ResourceSet parent, string background, string[] icons_sprite_names)
    : base(id, parent, (string) null)
  {
    this.Icons = new Sprite[icons_sprite_names.Length];
    this.BackgroundAnim = background;
    for (int index = 0; index < icons_sprite_names.Length; ++index)
      this.Icons[index] = Assets.GetSprite(HashedString.op_Implicit(icons_sprite_names[index]));
  }

  public Dream(
    string id,
    ResourceSet parent,
    string background,
    string[] icons_sprite_names,
    float durationPerImage)
    : this(id, parent, background, icons_sprite_names)
  {
    this.secondPerImage = durationPerImage;
  }
}
