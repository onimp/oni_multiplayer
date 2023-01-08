// Decompiled with JetBrains decompiler
// Type: MaterialsStatusItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class MaterialsStatusItem : StatusItem
{
  public MaterialsStatusItem(
    string id,
    string prefix,
    string icon,
    StatusItem.IconType icon_type,
    NotificationType notification_type,
    bool allow_multiples,
    HashedString overlay)
    : base(id, prefix, icon, icon_type, notification_type, allow_multiples, overlay)
  {
  }
}
