// Decompiled with JetBrains decompiler
// Type: LogicMemory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/LogicMemory")]
public class LogicMemory : KMonoBehaviour
{
  [MyCmpGet]
  private LogicPorts ports;
  [Serialize]
  private int value;
  private static StatusItem infoStatusItem;
  public static readonly HashedString READ_PORT_ID = new HashedString("LogicMemoryRead");
  public static readonly HashedString SET_PORT_ID = new HashedString("LogicMemorySet");
  public static readonly HashedString RESET_PORT_ID = new HashedString("LogicMemoryReset");
  private static readonly EventSystem.IntraObjectHandler<LogicMemory> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicMemory>((Action<LogicMemory, object>) ((component, data) => component.OnLogicValueChanged(data)));

  protected virtual void OnSpawn()
  {
    if (LogicMemory.infoStatusItem == null)
    {
      LogicMemory.infoStatusItem = new StatusItem("StoredValue", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      LogicMemory.infoStatusItem.resolveStringCallback = new Func<string, object, string>(LogicMemory.ResolveInfoStatusItemString);
    }
    this.Subscribe<LogicMemory>(-801688580, LogicMemory.OnLogicValueChangedDelegate);
  }

  public void OnLogicValueChanged(object data)
  {
    if (Object.op_Equality((Object) this.ports, (Object) null) || Object.op_Equality((Object) ((Component) this).gameObject, (Object) null) || Object.op_Equality((Object) this, (Object) null) || !HashedString.op_Inequality(((LogicValueChanged) data).portID, LogicMemory.READ_PORT_ID))
      return;
    int inputValue1 = this.ports.GetInputValue(LogicMemory.SET_PORT_ID);
    int inputValue2 = this.ports.GetInputValue(LogicMemory.RESET_PORT_ID);
    int num = this.value;
    if (LogicCircuitNetwork.IsBitActive(0, inputValue2))
      num = 0;
    else if (LogicCircuitNetwork.IsBitActive(0, inputValue1))
      num = 1;
    if (num == this.value)
      return;
    this.value = num;
    this.ports.SendSignal(LogicMemory.READ_PORT_ID, this.value);
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.Play(HashedString.op_Implicit(LogicCircuitNetwork.IsBitActive(0, this.value) ? "on" : "off"));
  }

  private static string ResolveInfoStatusItemString(string format_str, object data)
  {
    int outputValue = ((LogicMemory) data).ports.GetOutputValue(LogicMemory.READ_PORT_ID);
    return string.Format((string) BUILDINGS.PREFABS.LOGICMEMORY.STATUS_ITEM_VALUE, (object) outputValue);
  }
}
