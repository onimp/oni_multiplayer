// Decompiled with JetBrains decompiler
// Type: LogicRibbonReader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/LogicRibbonReader")]
public class LogicRibbonReader : KMonoBehaviour, ILogicRibbonBitSelector, IRender200ms
{
  public static readonly HashedString INPUT_PORT_ID = new HashedString("LogicRibbonReaderInput");
  public static readonly HashedString OUTPUT_PORT_ID = new HashedString("LogicRibbonReaderOutput");
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<LogicRibbonReader> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<LogicRibbonReader>((Action<LogicRibbonReader, object>) ((component, data) => component.OnLogicValueChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<LogicRibbonReader> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<LogicRibbonReader>((Action<LogicRibbonReader, object>) ((component, data) => component.OnCopySettings(data)));
  private KAnimHashedString BIT_ONE_SYMBOL = KAnimHashedString.op_Implicit("bit1_bloom");
  private KAnimHashedString BIT_TWO_SYMBOL = KAnimHashedString.op_Implicit("bit2_bloom");
  private KAnimHashedString BIT_THREE_SYMBOL = KAnimHashedString.op_Implicit("bit3_bloom");
  private KAnimHashedString BIT_FOUR_SYMBOL = KAnimHashedString.op_Implicit("bit4_bloom");
  private KAnimHashedString OUTPUT_SYMBOL = KAnimHashedString.op_Implicit("output_light_bloom");
  private KBatchedAnimController kbac;
  private Color colorOn = new Color(0.34117648f, 0.7254902f, 0.368627459f);
  private Color colorOff = new Color(0.9529412f, 0.2901961f, 0.2784314f);
  private LogicPorts ports;
  public int bitDepth = 4;
  [Serialize]
  public int selectedBit;
  [Serialize]
  private int currentValue;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<LogicRibbonReader>(-801688580, LogicRibbonReader.OnLogicValueChangedDelegate);
    this.ports = ((Component) this).GetComponent<LogicPorts>();
    this.kbac = ((Component) this).GetComponent<KBatchedAnimController>();
    this.kbac.Play(HashedString.op_Implicit("idle"));
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LogicRibbonReader>(-905833192, LogicRibbonReader.OnCopySettingsDelegate);
  }

  public void OnLogicValueChanged(object data)
  {
    LogicValueChanged logicValueChanged = (LogicValueChanged) data;
    if (HashedString.op_Inequality(logicValueChanged.portID, LogicRibbonReader.INPUT_PORT_ID))
      return;
    this.currentValue = logicValueChanged.newValue;
    this.UpdateLogicCircuit();
    this.UpdateVisuals();
  }

  private void OnCopySettings(object data)
  {
    LogicRibbonReader component = ((GameObject) data).GetComponent<LogicRibbonReader>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.SetBitSelection(component.selectedBit);
  }

  private void UpdateLogicCircuit()
  {
    LogicPorts component1 = ((Component) this).GetComponent<LogicPorts>();
    LogicWire.BitDepth bitDepth = LogicWire.BitDepth.NumRatings;
    int portCell = component1.GetPortCell(LogicRibbonReader.OUTPUT_PORT_ID);
    GameObject gameObject = Grid.Objects[portCell, 31];
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      LogicWire component2 = gameObject.GetComponent<LogicWire>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        bitDepth = component2.MaxBitDepth;
    }
    if (bitDepth != LogicWire.BitDepth.OneBit && bitDepth == LogicWire.BitDepth.FourBit)
    {
      int new_value = this.currentValue >> this.selectedBit;
      component1.SendSignal(LogicRibbonReader.OUTPUT_PORT_ID, new_value);
    }
    else
    {
      int num = this.currentValue & 1 << this.selectedBit;
      component1.SendSignal(LogicRibbonReader.OUTPUT_PORT_ID, num > 0 ? 1 : 0);
    }
    this.UpdateVisuals();
  }

  public void Render200ms(float dt) => this.UpdateVisuals();

  public void SetBitSelection(int bit)
  {
    this.selectedBit = bit;
    this.UpdateLogicCircuit();
  }

  public int GetBitSelection() => this.selectedBit;

  public int GetBitDepth() => this.bitDepth;

  public string SideScreenTitle => "STRINGS.UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.RIBBON_READER_TITLE";

  public string SideScreenDescription => (string) UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.RIBBON_READER_DESCRIPTION;

  public bool SideScreenDisplayWriterDescription() => false;

  public bool SideScreenDisplayReaderDescription() => true;

  public bool IsBitActive(int bit)
  {
    LogicCircuitNetwork logicCircuitNetwork = (LogicCircuitNetwork) null;
    if (Object.op_Inequality((Object) this.ports, (Object) null))
      logicCircuitNetwork = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(LogicRibbonReader.INPUT_PORT_ID));
    return logicCircuitNetwork != null && logicCircuitNetwork.IsBitActive(bit);
  }

  public int GetInputValue()
  {
    LogicPorts component = ((Component) this).GetComponent<LogicPorts>();
    return !Object.op_Inequality((Object) component, (Object) null) ? 0 : component.GetInputValue(LogicRibbonReader.INPUT_PORT_ID);
  }

  public int GetOutputValue()
  {
    LogicPorts component = ((Component) this).GetComponent<LogicPorts>();
    return !Object.op_Inequality((Object) component, (Object) null) ? 0 : component.GetOutputValue(LogicRibbonReader.OUTPUT_PORT_ID);
  }

  private LogicCircuitNetwork GetInputNetwork()
  {
    LogicCircuitNetwork inputNetwork = (LogicCircuitNetwork) null;
    if (Object.op_Inequality((Object) this.ports, (Object) null))
      inputNetwork = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(LogicRibbonReader.INPUT_PORT_ID));
    return inputNetwork;
  }

  private LogicCircuitNetwork GetOutputNetwork()
  {
    LogicCircuitNetwork outputNetwork = (LogicCircuitNetwork) null;
    if (Object.op_Inequality((Object) this.ports, (Object) null))
      outputNetwork = Game.Instance.logicCircuitManager.GetNetworkForCell(this.ports.GetPortCell(LogicRibbonReader.OUTPUT_PORT_ID));
    return outputNetwork;
  }

  public void UpdateVisuals()
  {
    LogicCircuitNetwork inputNetwork = this.GetInputNetwork();
    LogicCircuitNetwork outputNetwork = this.GetOutputNetwork();
    this.GetInputValue();
    int num = 0;
    if (inputNetwork != null)
    {
      num += 4;
      this.kbac.SetSymbolTint(this.BIT_ONE_SYMBOL, this.IsBitActive(0) ? this.colorOn : this.colorOff);
      this.kbac.SetSymbolTint(this.BIT_TWO_SYMBOL, this.IsBitActive(1) ? this.colorOn : this.colorOff);
      this.kbac.SetSymbolTint(this.BIT_THREE_SYMBOL, this.IsBitActive(2) ? this.colorOn : this.colorOff);
      this.kbac.SetSymbolTint(this.BIT_FOUR_SYMBOL, this.IsBitActive(3) ? this.colorOn : this.colorOff);
    }
    if (outputNetwork != null)
    {
      ++num;
      this.kbac.SetSymbolTint(this.OUTPUT_SYMBOL, LogicCircuitNetwork.IsBitActive(0, this.GetOutputValue()) ? this.colorOn : this.colorOff);
    }
    this.kbac.Play(HashedString.op_Implicit(num.ToString() + "_" + (this.GetBitSelection() + 1).ToString()));
  }
}
