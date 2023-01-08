// Decompiled with JetBrains decompiler
// Type: NavOffset
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public struct NavOffset
{
  public NavType navType;
  public CellOffset offset;

  public NavOffset(NavType nav_type, int x, int y)
  {
    this.navType = nav_type;
    this.offset.x = x;
    this.offset.y = y;
  }
}
