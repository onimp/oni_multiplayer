// Decompiled with JetBrains decompiler
// Type: WarpConduitStatus
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class WarpConduitStatus
{
  public static readonly Operational.Flag warpConnectedFlag = new Operational.Flag("warp_conduit_connected", Operational.Flag.Type.Requirement);

  public static void UpdateWarpConduitsOperational(GameObject sender, GameObject receiver)
  {
    int num = !Object.op_Inequality((Object) sender, (Object) null) ? 0 : (sender.GetComponent<Activatable>().IsActivated ? 1 : 0);
    bool flag1 = Object.op_Inequality((Object) receiver, (Object) null) && receiver.GetComponent<Activatable>().IsActivated;
    bool flag2 = (num & (flag1 ? 1 : 0)) != 0;
    int data = 0;
    if (num != 0)
      ++data;
    if (flag1)
      ++data;
    if (Object.op_Inequality((Object) sender, (Object) null))
    {
      sender.GetComponent<Operational>().SetFlag(WarpConduitStatus.warpConnectedFlag, flag2);
      if (data != 2)
      {
        sender.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
        sender.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, (object) data);
      }
      else
        sender.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
    }
    if (!Object.op_Inequality((Object) receiver, (Object) null))
      return;
    receiver.GetComponent<Operational>().SetFlag(WarpConduitStatus.warpConnectedFlag, flag2);
    if (data != 2)
    {
      receiver.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
      receiver.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled, (object) data);
    }
    else
      receiver.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.WarpConduitPartnerDisabled);
  }
}
