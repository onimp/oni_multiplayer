// Decompiled with JetBrains decompiler
// Type: LogicOperationalController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LogicOperationalController")]
public class LogicOperationalController : KMonoBehaviour
{
  public static readonly HashedString PORT_ID = HashedString.op_Implicit("LogicOperational");
  public int unNetworkedValue = 1;
  public static readonly Operational.Flag LogicOperationalFlag = new Operational.Flag("LogicOperational", Operational.Flag.Type.Requirement);
  private static StatusItem infoStatusItem;
  [MyCmpGet]
  public Operational operational;
  private static readonly EventSystem.IntraObjectHandler<LogicOperationalController> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicOperationalController>((Action<LogicOperationalController, object>) ((component, data) => component.OnLogicValueChanged(data)));

  public static List<LogicPorts.Port> CreateSingleInputPortList(CellOffset offset) => new List<LogicPorts.Port>()
  {
    LogicPorts.Port.InputPort(LogicOperationalController.PORT_ID, offset, (string) UI.LOGIC_PORTS.CONTROL_OPERATIONAL, (string) UI.LOGIC_PORTS.CONTROL_OPERATIONAL_ACTIVE, (string) UI.LOGIC_PORTS.CONTROL_OPERATIONAL_INACTIVE)
  };

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<LogicOperationalController>(-801688580, LogicOperationalController.OnLogicValueChangedDelegate);
    if (LogicOperationalController.infoStatusItem == null)
    {
      LogicOperationalController.infoStatusItem = new StatusItem("LogicOperationalInfo", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      LogicOperationalController.infoStatusItem.resolveStringCallback = new Func<string, object, string>(LogicOperationalController.ResolveInfoStatusItemString);
    }
    this.CheckWireState();
  }

  private LogicCircuitNetwork GetNetwork() => Game.Instance.logicCircuitManager.GetNetworkForCell(((Component) this).GetComponent<LogicPorts>().GetPortCell(LogicOperationalController.PORT_ID));

  private LogicCircuitNetwork CheckWireState()
  {
    LogicCircuitNetwork network = this.GetNetwork();
    int num = network != null ? network.OutputValue : this.unNetworkedValue;
    this.operational.SetFlag(LogicOperationalController.LogicOperationalFlag, LogicCircuitNetwork.IsBitActive(0, num));
    return network;
  }

  private static string ResolveInfoStatusItemString(string format_str, object data) => (string) (((LogicOperationalController) data).operational.GetFlag(LogicOperationalController.LogicOperationalFlag) ? BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_ENABLED : BUILDING.STATUSITEMS.LOGIC.LOGIC_CONTROLLED_DISABLED);

  private void OnLogicValueChanged(object data)
  {
    if (!HashedString.op_Equality(((LogicValueChanged) data).portID, LogicOperationalController.PORT_ID))
      return;
    LogicCircuitNetwork logicCircuitNetwork = this.CheckWireState();
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(LogicOperationalController.infoStatusItem, logicCircuitNetwork != null, (object) this);
  }
}
