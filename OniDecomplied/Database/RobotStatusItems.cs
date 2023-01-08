// Decompiled with JetBrains decompiler
// Type: Database.RobotStatusItems
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

namespace Database
{
  public class RobotStatusItems : StatusItems
  {
    public StatusItem LowBattery;
    public StatusItem LowBatteryNoCharge;
    public StatusItem DeadBattery;
    public StatusItem CantReachStation;
    public StatusItem DustBinFull;
    public StatusItem Working;
    public StatusItem UnloadingStorage;
    public StatusItem ReactPositive;
    public StatusItem ReactNegative;
    public StatusItem MovingToChargeStation;

    public RobotStatusItems(ResourceSet parent)
      : base(nameof (RobotStatusItems), parent)
    {
      this.CreateStatusItems();
    }

    private void CreateStatusItems()
    {
      this.CantReachStation = new StatusItem("CantReachStation", "ROBOTS", "status_item_exclamation", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false);
      this.CantReachStation.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject go = (GameObject) data;
        return str.Replace("{0}", go.GetProperName());
      });
      this.LowBattery = new StatusItem("LowBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false);
      this.LowBattery.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject go = (GameObject) data;
        return str.Replace("{0}", go.GetProperName());
      });
      this.LowBatteryNoCharge = new StatusItem("LowBatteryNoCharge", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false);
      this.LowBatteryNoCharge.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject go = (GameObject) data;
        return str.Replace("{0}", go.GetProperName());
      });
      this.DeadBattery = new StatusItem("DeadBattery", "ROBOTS", "status_item_need_power", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, false);
      this.DeadBattery.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject go = (GameObject) data;
        return str.Replace("{0}", go.GetProperName());
      });
      this.DustBinFull = new StatusItem("DustBinFull", "ROBOTS", "status_item_pending_clear", StatusItem.IconType.Custom, NotificationType.Neutral, false, OverlayModes.None.ID, false);
      this.DustBinFull.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject go = (GameObject) data;
        return str.Replace("{0}", go.GetProperName());
      });
      this.Working = new StatusItem("Working", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
      this.Working.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject go = (GameObject) data;
        return str.Replace("{0}", go.GetProperName());
      });
      this.MovingToChargeStation = new StatusItem("MovingToChargeStation", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
      this.MovingToChargeStation.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject go = (GameObject) data;
        return str.Replace("{0}", go.GetProperName());
      });
      this.UnloadingStorage = new StatusItem("UnloadingStorage", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
      this.UnloadingStorage.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        GameObject go = (GameObject) data;
        return str.Replace("{0}", go.GetProperName());
      });
      this.ReactPositive = new StatusItem("ReactPositive", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
      this.ReactPositive.resolveStringCallback = (Func<string, object, string>) ((str, data) => str);
      this.ReactNegative = new StatusItem("ReactNegative", "ROBOTS", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
      this.ReactNegative.resolveStringCallback = (Func<string, object, string>) ((str, data) => str);
    }
  }
}
