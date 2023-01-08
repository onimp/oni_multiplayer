// Decompiled with JetBrains decompiler
// Type: Database.PermitRarityExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public static class PermitRarityExtensions
  {
    public static string GetLocStringName(this PermitRarity rarity)
    {
      switch (rarity)
      {
        case PermitRarity.Unknown:
          return (string) UI.PERMIT_RARITY.UNKNOWN;
        case PermitRarity.Universal:
          return (string) UI.PERMIT_RARITY.UNIVERSAL;
        case PermitRarity.Loyalty:
          return (string) UI.PERMIT_RARITY.LOYALTY;
        case PermitRarity.Common:
          return (string) UI.PERMIT_RARITY.COMMON;
        case PermitRarity.Decent:
          return (string) UI.PERMIT_RARITY.DECENT;
        case PermitRarity.Nifty:
          return (string) UI.PERMIT_RARITY.NIFTY;
        case PermitRarity.Splendid:
          return (string) UI.PERMIT_RARITY.SPLENDID;
        default:
          DebugUtil.DevAssert(false, string.Format("Couldn't get name for rarity {0}", (object) rarity), (Object) null);
          return "-";
      }
    }
  }
}
