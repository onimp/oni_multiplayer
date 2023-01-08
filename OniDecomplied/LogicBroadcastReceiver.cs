// Decompiled with JetBrains decompiler
// Type: LogicBroadcastReceiver
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LogicBroadcastReceiver : KMonoBehaviour, ISimEveryTick
{
  [Serialize]
  private Ref<LogicBroadcaster> channel = new Ref<LogicBroadcaster>();
  public string PORT_ID = "";
  private List<int> channelEventListeners = new List<int>();
  private bool syncToChannelComplete;
  public static readonly Operational.Flag spaceVisible = new Operational.Flag(nameof (spaceVisible), Operational.Flag.Type.Requirement);
  public static readonly Operational.Flag validChannelInRange = new Operational.Flag(nameof (validChannelInRange), Operational.Flag.Type.Requirement);
  [MyCmpGet]
  private Operational operational;
  private bool wasOperational;
  [MyCmpGet]
  private KBatchedAnimController animController;
  private Guid rangeStatusItem = Guid.Empty;
  private Guid spaceNotVisibleStatusItem = Guid.Empty;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
    this.SetChannel(this.channel.Get());
    this.operational.SetFlag(LogicBroadcastReceiver.spaceVisible, this.IsSpaceVisible());
    this.operational.SetFlag(LogicBroadcastReceiver.validChannelInRange, this.CheckChannelValid() && LogicBroadcastReceiver.CheckRange(((Component) this.channel.Get()).gameObject, ((Component) this).gameObject));
    this.wasOperational = !this.operational.IsOperational;
    this.OnOperationalChanged((object) null);
  }

  public void SimEveryTick(float dt)
  {
    bool flag1 = this.IsSpaceVisible();
    this.operational.SetFlag(LogicBroadcastReceiver.spaceVisible, flag1);
    if (!flag1)
    {
      if (this.spaceNotVisibleStatusItem == Guid.Empty)
        this.spaceNotVisibleStatusItem = ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSurfaceSight);
    }
    else if (this.spaceNotVisibleStatusItem != Guid.Empty)
    {
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(this.spaceNotVisibleStatusItem);
      this.spaceNotVisibleStatusItem = Guid.Empty;
    }
    bool flag2 = this.CheckChannelValid() && LogicBroadcastReceiver.CheckRange(((Component) this.channel.Get()).gameObject, ((Component) this).gameObject);
    this.operational.SetFlag(LogicBroadcastReceiver.validChannelInRange, flag2);
    if (!flag2 || this.syncToChannelComplete)
      return;
    this.SyncWithBroadcast();
  }

  public bool IsSpaceVisible() => ((Component) this).gameObject.GetMyWorld().IsModuleInterior || Grid.ExposedToSunlight[Grid.PosToCell(((Component) this).gameObject)] > (byte) 0;

  private bool CheckChannelValid() => Object.op_Inequality((Object) this.channel.Get(), (Object) null) && ((Component) this.channel.Get()).GetComponent<LogicPorts>().inputPorts != null;

  public LogicBroadcaster GetChannel() => this.channel.Get();

  public void SetChannel(LogicBroadcaster broadcaster)
  {
    this.ClearChannel();
    if (Object.op_Equality((Object) broadcaster, (Object) null))
      return;
    this.channel.Set(broadcaster);
    this.syncToChannelComplete = false;
    this.channelEventListeners.Add(KMonoBehaviourExtensions.Subscribe(((Component) this.channel.Get()).gameObject, -801688580, new Action<object>(this.OnChannelLogicEvent)));
    this.channelEventListeners.Add(KMonoBehaviourExtensions.Subscribe(((Component) this.channel.Get()).gameObject, -592767678, new Action<object>(this.OnChannelLogicEvent)));
    this.SyncWithBroadcast();
  }

  private void ClearChannel()
  {
    if (this.CheckChannelValid())
    {
      for (int index = 0; index < this.channelEventListeners.Count; ++index)
        KMonoBehaviourExtensions.Unsubscribe(((Component) this.channel.Get()).gameObject, this.channelEventListeners[index]);
    }
    this.channelEventListeners.Clear();
  }

  private void OnChannelLogicEvent(object data)
  {
    if (!((Component) this.channel.Get()).GetComponent<Operational>().IsOperational)
      return;
    this.SyncWithBroadcast();
  }

  private void SyncWithBroadcast()
  {
    if (!this.CheckChannelValid())
      return;
    bool inRange = LogicBroadcastReceiver.CheckRange(((Component) this.channel.Get()).gameObject, ((Component) this).gameObject);
    this.UpdateRangeStatus(inRange);
    if (!inRange)
      return;
    ((Component) this).GetComponent<LogicPorts>().SendSignal(HashedString.op_Implicit(this.PORT_ID), this.channel.Get().GetCurrentValue());
    this.syncToChannelComplete = true;
  }

  public static bool CheckRange(GameObject broadcaster, GameObject receiver) => AxialUtil.GetDistance(broadcaster.GetMyWorldLocation(), receiver.GetMyWorldLocation()) <= LogicBroadcaster.RANGE;

  private void UpdateRangeStatus(bool inRange)
  {
    if (!inRange && this.rangeStatusItem == Guid.Empty)
    {
      this.rangeStatusItem = ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.BroadcasterOutOfRange);
    }
    else
    {
      if (!(this.rangeStatusItem != Guid.Empty))
        return;
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(this.rangeStatusItem);
      this.rangeStatusItem = Guid.Empty;
    }
  }

  private void OnOperationalChanged(object data)
  {
    if (this.operational.IsOperational)
    {
      if (this.wasOperational)
        return;
      this.wasOperational = true;
      this.animController.Queue(HashedString.op_Implicit("on_pre"));
      this.animController.Queue(HashedString.op_Implicit("on"), (KAnim.PlayMode) 0);
    }
    else
    {
      if (!this.wasOperational)
        return;
      this.wasOperational = false;
      this.animController.Queue(HashedString.op_Implicit("on_pst"));
      this.animController.Queue(HashedString.op_Implicit("off"), (KAnim.PlayMode) 0);
    }
  }

  protected virtual void OnCleanUp()
  {
    this.ClearChannel();
    base.OnCleanUp();
  }
}
