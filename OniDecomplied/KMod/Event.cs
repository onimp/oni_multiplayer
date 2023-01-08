// Decompiled with JetBrains decompiler
// Type: KMod.Event
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

namespace KMod
{
  public struct Event
  {
    public EventType event_type;
    public Label mod;
    public string details;

    public static void GetUIStrings(EventType err_type, out string title, out string title_tooltip)
    {
      title = string.Empty;
      title_tooltip = string.Empty;
      switch (err_type)
      {
        case EventType.LoadError:
          title = (string) UI.FRONTEND.MOD_EVENTS.REQUIRED;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.REQUIRED;
          break;
        case EventType.NotFound:
          title = (string) UI.FRONTEND.MOD_EVENTS.NOT_FOUND;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.NOT_FOUND;
          break;
        case EventType.InstallInfoInaccessible:
          title = (string) UI.FRONTEND.MOD_EVENTS.INSTALL_INFO_INACCESSIBLE;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.INSTALL_INFO_INACCESSIBLE;
          break;
        case EventType.OutOfOrder:
          title = (string) UI.FRONTEND.MOD_EVENTS.OUT_OF_ORDER;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.OUT_OF_ORDER;
          break;
        case EventType.ExpectedActive:
          title = (string) UI.FRONTEND.MOD_EVENTS.EXPECTED_ENABLED;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.EXPECTED_ENABLED;
          break;
        case EventType.ExpectedInactive:
          title = (string) UI.FRONTEND.MOD_EVENTS.EXPECTED_DISABLED;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.EXPECTED_DISABLED;
          break;
        case EventType.ActiveDuringCrash:
          title = (string) UI.FRONTEND.MOD_EVENTS.ACTIVE_DURING_CRASH;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.ACTIVE_DURING_CRASH;
          break;
        case EventType.InstallFailed:
          title = (string) UI.FRONTEND.MOD_EVENTS.INSTALL_FAILED;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.INSTALL_FAILED;
          break;
        case EventType.Installed:
          title = (string) UI.FRONTEND.MOD_EVENTS.INSTALLED;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.INSTALLED;
          break;
        case EventType.Uninstalled:
          title = (string) UI.FRONTEND.MOD_EVENTS.UNINSTALLED;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.UNINSTALLED;
          break;
        case EventType.VersionUpdate:
          title = (string) UI.FRONTEND.MOD_EVENTS.VERSION_UPDATE;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.VERSION_UPDATE;
          break;
        case EventType.AvailableContentChanged:
          title = (string) UI.FRONTEND.MOD_EVENTS.AVAILABLE_CONTENT_CHANGED;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.AVAILABLE_CONTENT_CHANGED;
          break;
        case EventType.RestartRequested:
          title = (string) UI.FRONTEND.MOD_EVENTS.REQUIRES_RESTART;
          title_tooltip = (string) UI.FRONTEND.MODS.REQUIRES_RESTART;
          break;
        case EventType.BadWorldGen:
          title = (string) UI.FRONTEND.MOD_EVENTS.BAD_WORLD_GEN;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.BAD_WORLD_GEN;
          break;
        case EventType.Deactivated:
          title = (string) UI.FRONTEND.MOD_EVENTS.DEACTIVATED;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.DEACTIVATED;
          break;
        case EventType.DisabledEarlyAccess:
          title = (string) UI.FRONTEND.MOD_EVENTS.ALL_MODS_DISABLED_EARLY_ACCESS;
          title_tooltip = (string) UI.FRONTEND.MOD_EVENTS.TOOLTIPS.ALL_MODS_DISABLED_EARLY_ACCESS;
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
