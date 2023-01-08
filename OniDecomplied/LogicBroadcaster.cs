// Decompiled with JetBrains decompiler
// Type: LogicBroadcaster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

public class LogicBroadcaster : KMonoBehaviour, ISimEveryTick
{
  public static int RANGE = 5;
  private static int INVALID_CHANNEL_ID = -1;
  public string PORT_ID = "";
  private bool wasOperational;
  [Serialize]
  private int broadcastChannelID = LogicBroadcaster.INVALID_CHANNEL_ID;
  private static readonly EventSystem.IntraObjectHandler<LogicBroadcaster> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicBroadcaster>((Action<LogicBroadcaster, object>) ((component, data) => component.OnLogicValueChanged(data)));
  public static readonly Operational.Flag spaceVisible = new Operational.Flag(nameof (spaceVisible), Operational.Flag.Type.Requirement);
  private Guid spaceNotVisibleStatusItem = Guid.Empty;
  [MyCmpGet]
  private Operational operational;
  [MyCmpGet]
  private KBatchedAnimController animController;

  public int BroadCastChannelID
  {
    get => this.broadcastChannelID;
    private set => this.broadcastChannelID = value;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Components.LogicBroadcasters.Add(this);
  }

  protected virtual void OnCleanUp()
  {
    Components.LogicBroadcasters.Remove(this);
    base.OnCleanUp();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<LogicBroadcaster>(-801688580, LogicBroadcaster.OnLogicValueChangedDelegate);
    this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
    this.operational.SetFlag(LogicBroadcaster.spaceVisible, this.IsSpaceVisible());
    this.wasOperational = !this.operational.IsOperational;
    this.OnOperationalChanged((object) null);
  }

  public bool IsSpaceVisible() => ((Component) this).gameObject.GetMyWorld().IsModuleInterior || Grid.ExposedToSunlight[Grid.PosToCell(((Component) this).gameObject)] > (byte) 0;

  public int GetCurrentValue() => ((Component) this).GetComponent<LogicPorts>().GetInputValue(HashedString.op_Implicit(this.PORT_ID));

  private void OnLogicValueChanged(object data)
  {
  }

  public void SimEveryTick(float dt)
  {
    bool flag = this.IsSpaceVisible();
    this.operational.SetFlag(LogicBroadcaster.spaceVisible, flag);
    if (!flag)
    {
      if (!(this.spaceNotVisibleStatusItem == Guid.Empty))
        return;
      this.spaceNotVisibleStatusItem = ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSurfaceSight);
    }
    else
    {
      if (!(this.spaceNotVisibleStatusItem != Guid.Empty))
        return;
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(this.spaceNotVisibleStatusItem);
      this.spaceNotVisibleStatusItem = Guid.Empty;
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
}
