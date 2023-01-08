// Decompiled with JetBrains decompiler
// Type: RoomTypeCategory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class RoomTypeCategory : Resource
{
  public string colorName { get; private set; }

  public string icon { get; private set; }

  public RoomTypeCategory(string id, string name, string colorName, string icon)
    : base(id, name)
  {
    this.colorName = colorName;
    this.icon = icon;
  }
}
