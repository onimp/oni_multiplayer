// Decompiled with JetBrains decompiler
// Type: KMod.EventType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace KMod
{
  public enum EventType
  {
    LoadError,
    NotFound,
    InstallInfoInaccessible,
    OutOfOrder,
    ExpectedActive,
    ExpectedInactive,
    ActiveDuringCrash,
    InstallFailed,
    Installed,
    Uninstalled,
    VersionUpdate,
    AvailableContentChanged,
    RestartRequested,
    BadWorldGen,
    Deactivated,
    DisabledEarlyAccess,
  }
}
