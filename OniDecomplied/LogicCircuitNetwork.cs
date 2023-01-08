// Decompiled with JetBrains decompiler
// Type: LogicCircuitNetwork
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class LogicCircuitNetwork : UtilityNetwork
{
  private List<LogicWire>[] wireGroups = new List<LogicWire>[2];
  private List<LogicUtilityNetworkLink>[] relevantBridges = new List<LogicUtilityNetworkLink>[2];
  private List<ILogicEventReceiver> receivers = new List<ILogicEventReceiver>();
  private List<ILogicEventSender> senders = new List<ILogicEventSender>();
  private int previousValue = -1;
  private int outputValue;
  private bool resetting;
  public static float logicSoundLastPlayedTime = 0.0f;
  private const float MIN_OVERLOAD_TIME_FOR_DAMAGE = 6f;
  private const float MIN_OVERLOAD_NOTIFICATION_DISPLAY_TIME = 5f;
  private GameObject targetOverloadedWire;
  private float timeOverloaded;
  private float timeOverloadNotificationDisplayed;
  private Notification overloadedNotification;
  public static Dictionary<int, LogicCircuitNetwork.LogicSoundPair> logicSoundRegister = new Dictionary<int, LogicCircuitNetwork.LogicSoundPair>();

  public override void AddItem(object item)
  {
    switch (item)
    {
      case LogicWire _:
        LogicWire logicWire = (LogicWire) item;
        LogicWire.BitDepth maxBitDepth = logicWire.MaxBitDepth;
        List<LogicWire> logicWireList = this.wireGroups[(int) maxBitDepth];
        if (logicWireList == null)
        {
          logicWireList = new List<LogicWire>();
          this.wireGroups[(int) maxBitDepth] = logicWireList;
        }
        logicWireList.Add(logicWire);
        break;
      case ILogicEventReceiver _:
        this.receivers.Add((ILogicEventReceiver) item);
        break;
      case ILogicEventSender _:
        this.senders.Add((ILogicEventSender) item);
        break;
    }
  }

  public override void RemoveItem(object item)
  {
    switch (item)
    {
      case LogicWire _:
        LogicWire logicWire = (LogicWire) item;
        this.wireGroups[(int) logicWire.MaxBitDepth].Remove(logicWire);
        break;
      case ILogicEventReceiver _:
        this.receivers.Remove(item as ILogicEventReceiver);
        break;
      case ILogicEventSender _:
        this.senders.Remove((ILogicEventSender) item);
        break;
    }
  }

  public override void ConnectItem(object item)
  {
    switch (item)
    {
      case ILogicEventReceiver _:
        ((ILogicNetworkConnection) item).OnLogicNetworkConnectionChanged(true);
        break;
      case ILogicEventSender _:
        ((ILogicNetworkConnection) item).OnLogicNetworkConnectionChanged(true);
        break;
    }
  }

  public override void DisconnectItem(object item)
  {
    switch (item)
    {
      case ILogicEventReceiver _:
        ILogicEventReceiver logicEventReceiver = item as ILogicEventReceiver;
        logicEventReceiver.ReceiveLogicEvent(0);
        logicEventReceiver.OnLogicNetworkConnectionChanged(false);
        break;
      case ILogicEventSender _:
        (item as ILogicEventSender).OnLogicNetworkConnectionChanged(false);
        break;
    }
  }

  public override void Reset(UtilityNetworkGridNode[] grid)
  {
    this.resetting = true;
    this.previousValue = -1;
    this.outputValue = 0;
    for (int index1 = 0; index1 < 2; ++index1)
    {
      List<LogicWire> wireGroup = this.wireGroups[index1];
      if (wireGroup != null)
      {
        for (int index2 = 0; index2 < wireGroup.Count; ++index2)
        {
          LogicWire logicWire = wireGroup[index2];
          if (Object.op_Inequality((Object) logicWire, (Object) null))
          {
            int cell = Grid.PosToCell(TransformExtensions.GetPosition(logicWire.transform));
            UtilityNetworkGridNode utilityNetworkGridNode = grid[cell] with
            {
              networkIdx = -1
            };
            grid[cell] = utilityNetworkGridNode;
          }
        }
        wireGroup.Clear();
      }
    }
    this.senders.Clear();
    this.receivers.Clear();
    this.resetting = false;
    this.RemoveOverloadedNotification();
  }

  public void UpdateLogicValue()
  {
    if (this.resetting)
      return;
    this.previousValue = this.outputValue;
    this.outputValue = 0;
    foreach (ILogicEventSender sender in this.senders)
      sender.LogicTick();
    foreach (ILogicEventSender sender in this.senders)
      this.outputValue |= sender.GetLogicValue();
  }

  public int GetBitsUsed() => this.outputValue <= 1 ? 1 : 4;

  public bool IsBitActive(int bit) => (this.OutputValue & 1 << bit) > 0;

  public static bool IsBitActive(int bit, int value) => (value & 1 << bit) > 0;

  public static int GetBitValue(int bit, int value) => value & 1 << bit;

  public void SendLogicEvents(bool force_send, int id)
  {
    if (this.resetting || !(this.outputValue != this.previousValue | force_send))
      return;
    foreach (ILogicEventReceiver receiver in this.receivers)
      receiver.ReceiveLogicEvent(this.outputValue);
    if (force_send)
      return;
    this.TriggerAudio(this.previousValue >= 0 ? this.previousValue : 0, id);
  }

  private void TriggerAudio(int old_value, int id)
  {
    SpeedControlScreen instance = SpeedControlScreen.Instance;
    if (old_value == this.outputValue || !Object.op_Inequality((Object) instance, (Object) null) || instance.IsPaused)
      return;
    int num1 = 0;
    GridArea visibleArea = GridVisibleArea.GetVisibleArea();
    List<LogicWire> logicWireList = new List<LogicWire>();
    for (int index1 = 0; index1 < 2; ++index1)
    {
      List<LogicWire> wireGroup = this.wireGroups[index1];
      if (wireGroup != null)
      {
        for (int index2 = 0; index2 < wireGroup.Count; ++index2)
        {
          ++num1;
          if (Vector2I.op_LessThanOrEqual(visibleArea.Min, Vector2.op_Implicit(TransformExtensions.GetPosition(wireGroup[index2].transform))) && Vector2I.op_LessThanOrEqual(Vector2.op_Implicit(TransformExtensions.GetPosition(wireGroup[index2].transform)), visibleArea.Max))
            logicWireList.Add(wireGroup[index2]);
        }
      }
    }
    if (logicWireList.Count <= 0)
      return;
    int index = Mathf.CeilToInt((float) (logicWireList.Count / 2));
    if (!Object.op_Inequality((Object) logicWireList[index], (Object) null))
      return;
    Vector3 position = TransformExtensions.GetPosition(logicWireList[index].transform);
    position.z = 0.0f;
    LogicCircuitNetwork.LogicSoundPair logicSoundPair = new LogicCircuitNetwork.LogicSoundPair();
    if (!LogicCircuitNetwork.logicSoundRegister.ContainsKey(id))
    {
      LogicCircuitNetwork.logicSoundRegister.Add(id, logicSoundPair);
    }
    else
    {
      logicSoundPair.playedIndex = LogicCircuitNetwork.logicSoundRegister[id].playedIndex;
      logicSoundPair.lastPlayed = LogicCircuitNetwork.logicSoundRegister[id].lastPlayed;
    }
    if (logicSoundPair.playedIndex < 2)
    {
      LogicCircuitNetwork.logicSoundRegister[id].playedIndex = logicSoundPair.playedIndex + 1;
    }
    else
    {
      LogicCircuitNetwork.logicSoundRegister[id].playedIndex = 0;
      LogicCircuitNetwork.logicSoundRegister[id].lastPlayed = Time.time;
    }
    float num2 = (float) (((double) Time.time - (double) logicSoundPair.lastPlayed) / 3.0);
    EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Logic_Circuit_Toggle"), position, 1f);
    ((EventInstance) ref eventInstance).setParameterByName("logic_volumeModifer", num2, false);
    ((EventInstance) ref eventInstance).setParameterByName("wireCount", (float) (num1 % 24), false);
    ((EventInstance) ref eventInstance).setParameterByName("enabled", (float) this.outputValue, false);
    KFMOD.EndOneShot(eventInstance);
  }

  public void UpdateOverloadTime(float dt, int bits_used)
  {
    bool flag = false;
    List<LogicWire> logicWireList = (List<LogicWire>) null;
    List<LogicUtilityNetworkLink> utilityNetworkLinkList = (List<LogicUtilityNetworkLink>) null;
    for (int rating = 0; rating < 2; ++rating)
    {
      List<LogicWire> wireGroup = this.wireGroups[rating];
      List<LogicUtilityNetworkLink> relevantBridge = this.relevantBridges[rating];
      float bitDepthAsInt = (float) LogicWire.GetBitDepthAsInt((LogicWire.BitDepth) rating);
      if ((double) bits_used > (double) bitDepthAsInt && (relevantBridge != null && relevantBridge.Count > 0 || wireGroup != null && wireGroup.Count > 0))
      {
        flag = true;
        logicWireList = wireGroup;
        utilityNetworkLinkList = relevantBridge;
        break;
      }
    }
    logicWireList?.RemoveAll((Predicate<LogicWire>) (x => Object.op_Equality((Object) x, (Object) null)));
    utilityNetworkLinkList?.RemoveAll((Predicate<LogicUtilityNetworkLink>) (x => Object.op_Equality((Object) x, (Object) null)));
    if (flag)
    {
      this.timeOverloaded += dt;
      if ((double) this.timeOverloaded <= 6.0)
        return;
      this.timeOverloaded = 0.0f;
      if (Object.op_Equality((Object) this.targetOverloadedWire, (Object) null))
      {
        if (utilityNetworkLinkList != null && utilityNetworkLinkList.Count > 0)
        {
          int index = Random.Range(0, utilityNetworkLinkList.Count);
          this.targetOverloadedWire = ((Component) utilityNetworkLinkList[index]).gameObject;
        }
        else if (logicWireList != null && logicWireList.Count > 0)
        {
          int index = Random.Range(0, logicWireList.Count);
          this.targetOverloadedWire = ((Component) logicWireList[index]).gameObject;
        }
      }
      if (Object.op_Inequality((Object) this.targetOverloadedWire, (Object) null))
        EventExtensions.Trigger(this.targetOverloadedWire, -794517298, (object) new BuildingHP.DamageSourceInfo()
        {
          damage = 1,
          source = (string) BUILDINGS.DAMAGESOURCES.LOGIC_CIRCUIT_OVERLOADED,
          popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LOGIC_CIRCUIT_OVERLOADED,
          takeDamageEffect = SpawnFXHashes.BuildingLogicOverload,
          fullDamageEffectName = "logic_ribbon_damage_kanim",
          statusItemID = Db.Get().BuildingStatusItems.LogicOverloaded.Id
        });
      if (this.overloadedNotification != null)
        return;
      this.timeOverloadNotificationDisplayed = 0.0f;
      this.overloadedNotification = new Notification((string) MISC.NOTIFICATIONS.LOGIC_CIRCUIT_OVERLOADED.NAME, NotificationType.BadMinor, click_focus: this.targetOverloadedWire.transform);
      Game.Instance.FindOrAdd<Notifier>().Add(this.overloadedNotification);
    }
    else
    {
      this.timeOverloaded = Mathf.Max(0.0f, this.timeOverloaded - dt * 0.95f);
      this.timeOverloadNotificationDisplayed += dt;
      if ((double) this.timeOverloadNotificationDisplayed <= 5.0)
        return;
      this.RemoveOverloadedNotification();
    }
  }

  private void RemoveOverloadedNotification()
  {
    if (this.overloadedNotification == null)
      return;
    Game.Instance.FindOrAdd<Notifier>().Remove(this.overloadedNotification);
    this.overloadedNotification = (Notification) null;
  }

  public void UpdateRelevantBridges(List<LogicUtilityNetworkLink>[] bridgeGroups)
  {
    LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
    for (int index1 = 0; index1 < bridgeGroups.Length; ++index1)
    {
      if (this.relevantBridges[index1] != null)
        this.relevantBridges[index1].Clear();
      for (int index2 = 0; index2 < bridgeGroups[index1].Count; ++index2)
      {
        if (logicCircuitManager.GetNetworkForCell(bridgeGroups[index1][index2].cell_one) == this || logicCircuitManager.GetNetworkForCell(bridgeGroups[index1][index2].cell_two) == this)
        {
          if (this.relevantBridges[index1] == null)
            this.relevantBridges[index1] = new List<LogicUtilityNetworkLink>();
          this.relevantBridges[index1].Add(bridgeGroups[index1][index2]);
        }
      }
    }
  }

  public int OutputValue => this.outputValue;

  public int WireCount
  {
    get
    {
      int wireCount = 0;
      for (int index = 0; index < 2; ++index)
      {
        if (this.wireGroups[index] != null)
          wireCount += this.wireGroups[index].Count;
      }
      return wireCount;
    }
  }

  public ReadOnlyCollection<ILogicEventSender> Senders => this.senders.AsReadOnly();

  public ReadOnlyCollection<ILogicEventReceiver> Receivers => this.receivers.AsReadOnly();

  public class LogicSoundPair
  {
    public int playedIndex;
    public float lastPlayed;
  }
}
