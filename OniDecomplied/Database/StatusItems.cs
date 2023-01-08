// Decompiled with JetBrains decompiler
// Type: Database.StatusItems
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Diagnostics;

namespace Database
{
  public class StatusItems : ResourceSet<StatusItem>
  {
    public StatusItems(string id, ResourceSet parent)
      : base(id, parent)
    {
    }

    [DebuggerDisplay("{Id}")]
    public class StatusItemInfo : Resource
    {
      public string Type;
      public string Tooltip;
      public bool IsIconTinted;
      public StatusItem.IconType IconType;
      public string Icon;
      public string SoundPath;
      public bool ShouldNotify;
      public float NotificationDelay;
      public NotificationType NotificationType;
      public bool AllowMultiples;
      public string Effect;
      public HashedString Overlay;
      public HashedString SecondOverlay;
    }
  }
}
