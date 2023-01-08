// Decompiled with JetBrains decompiler
// Type: LogicPorts
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/LogicPorts")]
public class LogicPorts : KMonoBehaviour, IGameObjectEffectDescriptor, IRenderEveryTick
{
  [SerializeField]
  public LogicPorts.Port[] outputPortInfo;
  [SerializeField]
  public LogicPorts.Port[] inputPortInfo;
  public List<ILogicUIElement> outputPorts;
  public List<ILogicUIElement> inputPorts;
  private int cell = -1;
  private Orientation orientation = Orientation.NumRotations;
  [Serialize]
  private int[] serializedOutputValues;
  private bool isPhysical;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.autoRegisterSimRender = false;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.isPhysical = Object.op_Inequality((Object) ((Component) this).GetComponent<BuildingComplete>(), (Object) null);
    if ((this.isPhysical ? 0 : (Object.op_Equality((Object) ((Component) this).GetComponent<BuildingUnderConstruction>(), (Object) null) ? 1 : 0)) != 0)
    {
      OverlayScreen.Instance.OnOverlayChanged += new Action<HashedString>(this.OnOverlayChanged);
      this.OnOverlayChanged(OverlayScreen.Instance.mode);
      this.CreateVisualizers();
      SimAndRenderScheduler.instance.Add((object) this, false);
    }
    else if (this.isPhysical)
    {
      this.UpdateMissingWireIcon();
      this.CreatePhysicalPorts();
    }
    else
      this.CreateVisualizers();
  }

  protected virtual void OnCleanUp()
  {
    OverlayScreen.Instance.OnOverlayChanged -= new Action<HashedString>(this.OnOverlayChanged);
    this.DestroyVisualizers();
    if (this.isPhysical)
      this.DestroyPhysicalPorts();
    base.OnCleanUp();
  }

  public void RenderEveryTick(float dt) => this.CreateVisualizers();

  public void HackRefreshVisualizers() => this.CreateVisualizers();

  private void CreateVisualizers()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    bool flag = cell != this.cell;
    this.cell = cell;
    if (!flag)
    {
      Rotatable component = ((Component) this).GetComponent<Rotatable>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        Orientation orientation = component.GetOrientation();
        flag = orientation != this.orientation;
        this.orientation = orientation;
      }
    }
    if (!flag)
      return;
    this.DestroyVisualizers();
    if (this.outputPortInfo != null)
    {
      this.outputPorts = new List<ILogicUIElement>();
      for (int index = 0; index < this.outputPortInfo.Length; ++index)
      {
        LogicPorts.Port port = this.outputPortInfo[index];
        LogicPortVisualizer elem = new LogicPortVisualizer(this.GetActualCell(port.cellOffset), port.spriteType);
        this.outputPorts.Add((ILogicUIElement) elem);
        Game.Instance.logicCircuitManager.AddVisElem((ILogicUIElement) elem);
      }
    }
    if (this.inputPortInfo == null)
      return;
    this.inputPorts = new List<ILogicUIElement>();
    for (int index = 0; index < this.inputPortInfo.Length; ++index)
    {
      LogicPorts.Port port = this.inputPortInfo[index];
      LogicPortVisualizer elem = new LogicPortVisualizer(this.GetActualCell(port.cellOffset), port.spriteType);
      this.inputPorts.Add((ILogicUIElement) elem);
      Game.Instance.logicCircuitManager.AddVisElem((ILogicUIElement) elem);
    }
  }

  private void DestroyVisualizers()
  {
    if (this.outputPorts != null)
    {
      foreach (ILogicUIElement outputPort in this.outputPorts)
        Game.Instance.logicCircuitManager.RemoveVisElem(outputPort);
    }
    if (this.inputPorts == null)
      return;
    foreach (ILogicUIElement inputPort in this.inputPorts)
      Game.Instance.logicCircuitManager.RemoveVisElem(inputPort);
  }

  private void CreatePhysicalPorts(bool forceCreate = false)
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    if (cell == this.cell && !forceCreate)
      return;
    this.cell = cell;
    this.DestroyVisualizers();
    if (this.outputPortInfo != null)
    {
      this.outputPorts = new List<ILogicUIElement>();
      for (int index = 0; index < this.outputPortInfo.Length; ++index)
      {
        LogicPorts.Port info = this.outputPortInfo[index];
        LogicEventSender elem = new LogicEventSender(info.id, this.GetActualCell(info.cellOffset), (Action<int>) (new_value =>
        {
          if (!Object.op_Inequality((Object) this, (Object) null))
            return;
          this.OnLogicValueChanged(info.id, new_value);
        }), new Action<int, bool>(this.OnLogicNetworkConnectionChanged), info.spriteType);
        this.outputPorts.Add((ILogicUIElement) elem);
        Game.Instance.logicCircuitManager.AddVisElem((ILogicUIElement) elem);
        Game.Instance.logicCircuitSystem.AddToNetworks(elem.GetLogicUICell(), (object) elem, true);
      }
      if (this.serializedOutputValues != null && this.serializedOutputValues.Length == this.outputPorts.Count)
      {
        for (int index = 0; index < this.outputPorts.Count; ++index)
          (this.outputPorts[index] as LogicEventSender).SetValue(this.serializedOutputValues[index]);
      }
    }
    this.serializedOutputValues = (int[]) null;
    if (this.inputPortInfo == null)
      return;
    this.inputPorts = new List<ILogicUIElement>();
    for (int index = 0; index < this.inputPortInfo.Length; ++index)
    {
      LogicPorts.Port info = this.inputPortInfo[index];
      LogicEventHandler elem = new LogicEventHandler(this.GetActualCell(info.cellOffset), (Action<int>) (new_value =>
      {
        if (!Object.op_Inequality((Object) this, (Object) null))
          return;
        this.OnLogicValueChanged(info.id, new_value);
      }), new Action<int, bool>(this.OnLogicNetworkConnectionChanged), info.spriteType);
      this.inputPorts.Add((ILogicUIElement) elem);
      Game.Instance.logicCircuitManager.AddVisElem((ILogicUIElement) elem);
      Game.Instance.logicCircuitSystem.AddToNetworks(elem.GetLogicUICell(), (object) elem, true);
    }
  }

  private bool ShowMissingWireIcon()
  {
    LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
    if (this.outputPortInfo != null)
    {
      for (int index = 0; index < this.outputPortInfo.Length; ++index)
      {
        LogicPorts.Port port = this.outputPortInfo[index];
        if (port.requiresConnection)
        {
          int portCell = this.GetPortCell(port.id);
          if (logicCircuitManager.GetNetworkForCell(portCell) == null)
            return true;
        }
      }
    }
    if (this.inputPortInfo != null)
    {
      for (int index = 0; index < this.inputPortInfo.Length; ++index)
      {
        LogicPorts.Port port = this.inputPortInfo[index];
        if (port.requiresConnection)
        {
          int portCell = this.GetPortCell(port.id);
          if (logicCircuitManager.GetNetworkForCell(portCell) == null)
            return true;
        }
      }
    }
    return false;
  }

  public void OnMove()
  {
    this.DestroyPhysicalPorts();
    this.CreatePhysicalPorts();
  }

  private void OnLogicNetworkConnectionChanged(int cell, bool connected) => this.UpdateMissingWireIcon();

  private void UpdateMissingWireIcon() => LogicCircuitManager.ToggleNoWireConnected(this.ShowMissingWireIcon(), ((Component) this).gameObject);

  private void DestroyPhysicalPorts()
  {
    if (this.outputPorts != null)
    {
      foreach (ILogicEventSender outputPort in this.outputPorts)
        Game.Instance.logicCircuitSystem.RemoveFromNetworks(outputPort.GetLogicCell(), (object) outputPort, true);
    }
    if (this.inputPorts == null)
      return;
    for (int index = 0; index < this.inputPorts.Count; ++index)
    {
      if (this.inputPorts[index] is LogicEventHandler inputPort)
        Game.Instance.logicCircuitSystem.RemoveFromNetworks(inputPort.GetLogicCell(), (object) inputPort, true);
    }
  }

  private void OnLogicValueChanged(HashedString port_id, int new_value)
  {
    if (!Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null))
      return;
    EventExtensions.Trigger(((Component) this).gameObject, -801688580, (object) new LogicValueChanged()
    {
      portID = port_id,
      newValue = new_value
    });
  }

  private int GetActualCell(CellOffset offset)
  {
    Rotatable component = ((Component) this).GetComponent<Rotatable>();
    if (Object.op_Inequality((Object) component, (Object) null))
      offset = component.GetRotatedCellOffset(offset);
    return Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), offset);
  }

  public bool TryGetPortAtCell(int cell, out LogicPorts.Port port, out bool isInput)
  {
    foreach (LogicPorts.Port port1 in this.inputPortInfo)
    {
      if (this.GetActualCell(port1.cellOffset) == cell)
      {
        port = port1;
        isInput = true;
        return true;
      }
    }
    foreach (LogicPorts.Port port2 in this.outputPortInfo)
    {
      if (this.GetActualCell(port2.cellOffset) == cell)
      {
        port = port2;
        isInput = false;
        return true;
      }
    }
    port = new LogicPorts.Port();
    isInput = false;
    return false;
  }

  public void SendSignal(HashedString port_id, int new_value)
  {
    if (this.outputPortInfo != null && this.outputPorts == null)
      this.CreatePhysicalPorts(true);
    foreach (LogicEventSender outputPort in this.outputPorts)
    {
      if (HashedString.op_Equality(outputPort.ID, port_id))
      {
        outputPort.SetValue(new_value);
        break;
      }
    }
  }

  public int GetPortCell(HashedString port_id)
  {
    foreach (LogicPorts.Port port in this.inputPortInfo)
    {
      if (HashedString.op_Equality(port.id, port_id))
        return this.GetActualCell(port.cellOffset);
    }
    foreach (LogicPorts.Port port in this.outputPortInfo)
    {
      if (HashedString.op_Equality(port.id, port_id))
        return this.GetActualCell(port.cellOffset);
    }
    return -1;
  }

  public int GetInputValue(HashedString port_id)
  {
    for (int index = 0; index < this.inputPortInfo.Length && this.inputPorts != null; ++index)
    {
      if (HashedString.op_Equality(this.inputPortInfo[index].id, port_id))
        return !(this.inputPorts[index] is LogicEventHandler inputPort) ? 0 : inputPort.Value;
    }
    return 0;
  }

  public int GetOutputValue(HashedString port_id)
  {
    for (int index = 0; index < this.outputPorts.Count && this.outputPorts[index] is LogicEventSender outputPort; ++index)
    {
      if (HashedString.op_Equality(outputPort.ID, port_id))
        return outputPort.GetLogicValue();
    }
    return 0;
  }

  public bool IsPortConnected(HashedString port_id) => Game.Instance.logicCircuitManager.GetNetworkForCell(this.GetPortCell(port_id)) != null;

  private void OnOverlayChanged(HashedString mode)
  {
    if (HashedString.op_Equality(mode, OverlayModes.Logic.ID))
    {
      ((Behaviour) this).enabled = true;
      this.CreateVisualizers();
    }
    else
    {
      ((Behaviour) this).enabled = false;
      this.DestroyVisualizers();
    }
  }

  public LogicWire.BitDepth GetConnectedWireBitDepth(HashedString port_id)
  {
    LogicWire.BitDepth connectedWireBitDepth = LogicWire.BitDepth.NumRatings;
    int portCell = this.GetPortCell(port_id);
    GameObject gameObject = Grid.Objects[portCell, 31];
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      LogicWire component = gameObject.GetComponent<LogicWire>();
      if (Object.op_Inequality((Object) component, (Object) null))
        connectedWireBitDepth = component.MaxBitDepth;
    }
    return connectedWireBitDepth;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    LogicPorts component = go.GetComponent<LogicPorts>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      if (component.inputPortInfo != null && component.inputPortInfo.Length != 0)
      {
        Descriptor descriptor;
        // ISSUE: explicit constructor call
        ((Descriptor) ref descriptor).\u002Ector((string) UI.LOGIC_PORTS.INPUT_PORTS, (string) UI.LOGIC_PORTS.INPUT_PORTS_TOOLTIP, (Descriptor.DescriptorType) 1, false);
        descriptors.Add(descriptor);
        foreach (LogicPorts.Port port in component.inputPortInfo)
        {
          string str = string.Format((string) UI.LOGIC_PORTS.INPUT_PORT_TOOLTIP, (object) port.activeDescription, (object) port.inactiveDescription);
          // ISSUE: explicit constructor call
          ((Descriptor) ref descriptor).\u002Ector(port.description, str, (Descriptor.DescriptorType) 1, false);
          ((Descriptor) ref descriptor).IncreaseIndent();
          descriptors.Add(descriptor);
        }
      }
      if (component.outputPortInfo != null && component.outputPortInfo.Length != 0)
      {
        Descriptor descriptor;
        // ISSUE: explicit constructor call
        ((Descriptor) ref descriptor).\u002Ector((string) UI.LOGIC_PORTS.OUTPUT_PORTS, (string) UI.LOGIC_PORTS.OUTPUT_PORTS_TOOLTIP, (Descriptor.DescriptorType) 1, false);
        descriptors.Add(descriptor);
        foreach (LogicPorts.Port port in component.outputPortInfo)
        {
          string str = string.Format((string) UI.LOGIC_PORTS.OUTPUT_PORT_TOOLTIP, (object) port.activeDescription, (object) port.inactiveDescription);
          // ISSUE: explicit constructor call
          ((Descriptor) ref descriptor).\u002Ector(port.description, str, (Descriptor.DescriptorType) 1, false);
          ((Descriptor) ref descriptor).IncreaseIndent();
          descriptors.Add(descriptor);
        }
      }
    }
    return descriptors;
  }

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing()
  {
    if (!this.isPhysical || this.outputPorts == null)
      return;
    this.serializedOutputValues = new int[this.outputPorts.Count];
    for (int index = 0; index < this.outputPorts.Count; ++index)
    {
      LogicEventSender outputPort = this.outputPorts[index] as LogicEventSender;
      this.serializedOutputValues[index] = outputPort.GetLogicValue();
    }
  }

  [System.Runtime.Serialization.OnSerialized]
  private void OnSerialized() => this.serializedOutputValues = (int[]) null;

  [Serializable]
  public struct Port
  {
    public HashedString id;
    public CellOffset cellOffset;
    public string description;
    public string activeDescription;
    public string inactiveDescription;
    public bool requiresConnection;
    public LogicPortSpriteType spriteType;
    public bool displayCustomName;

    public Port(
      HashedString id,
      CellOffset cell_offset,
      string description,
      string activeDescription,
      string inactiveDescription,
      bool show_wire_missing_icon,
      LogicPortSpriteType sprite_type,
      bool display_custom_name = false)
    {
      this.id = id;
      this.cellOffset = cell_offset;
      this.description = description;
      this.activeDescription = activeDescription;
      this.inactiveDescription = inactiveDescription;
      this.requiresConnection = show_wire_missing_icon;
      this.spriteType = sprite_type;
      this.displayCustomName = display_custom_name;
    }

    public static LogicPorts.Port InputPort(
      HashedString id,
      CellOffset cell_offset,
      string description,
      string activeDescription,
      string inactiveDescription,
      bool show_wire_missing_icon = false,
      bool display_custom_name = false)
    {
      return new LogicPorts.Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.Input, display_custom_name);
    }

    public static LogicPorts.Port OutputPort(
      HashedString id,
      CellOffset cell_offset,
      string description,
      string activeDescription,
      string inactiveDescription,
      bool show_wire_missing_icon = false,
      bool display_custom_name = false)
    {
      return new LogicPorts.Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.Output, display_custom_name);
    }

    public static LogicPorts.Port RibbonInputPort(
      HashedString id,
      CellOffset cell_offset,
      string description,
      string activeDescription,
      string inactiveDescription,
      bool show_wire_missing_icon = false,
      bool display_custom_name = false)
    {
      return new LogicPorts.Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.RibbonInput, display_custom_name);
    }

    public static LogicPorts.Port RibbonOutputPort(
      HashedString id,
      CellOffset cell_offset,
      string description,
      string activeDescription,
      string inactiveDescription,
      bool show_wire_missing_icon = false,
      bool display_custom_name = false)
    {
      return new LogicPorts.Port(id, cell_offset, description, activeDescription, inactiveDescription, show_wire_missing_icon, LogicPortSpriteType.RibbonOutput, display_custom_name);
    }
  }
}
