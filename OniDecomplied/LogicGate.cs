// Decompiled with JetBrains decompiler
// Type: LogicGate
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
public class LogicGate : LogicGateBase, ILogicEventSender, ILogicNetworkConnection
{
  private static readonly LogicGate.LogicGateDescriptions.Description INPUT_ONE_SINGLE_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_SINGLE_INPUT_ONE_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description INPUT_ONE_MULTI_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_ONE_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description INPUT_TWO_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_TWO_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_TWO_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_TWO_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description INPUT_THREE_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_THREE_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_THREE_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_THREE_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description INPUT_FOUR_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_FOUR_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_FOUR_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTI_INPUT_FOUR_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description OUTPUT_ONE_SINGLE_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_SINGLE_OUTPUT_ONE_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_SINGLE_OUTPUT_ONE_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_SINGLE_OUTPUT_ONE_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description OUTPUT_ONE_MULTI_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_ONE_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_ONE_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_ONE_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description OUTPUT_TWO_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_TWO_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_TWO_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_TWO_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description OUTPUT_THREE_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_THREE_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_THREE_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_THREE_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description OUTPUT_FOUR_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_FOUR_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_FOUR_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTI_OUTPUT_FOUR_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description CONTROL_ONE_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_ONE_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_ONE_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_ONE_INACTIVE
  };
  private static readonly LogicGate.LogicGateDescriptions.Description CONTROL_TWO_DESCRIPTION = new LogicGate.LogicGateDescriptions.Description()
  {
    name = (string) UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_TWO_NAME,
    active = (string) UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_TWO_ACTIVE,
    inactive = (string) UI.LOGIC_PORTS.GATE_MULTIPLEXER_CONTROL_TWO_INACTIVE
  };
  private LogicGate.LogicGateDescriptions descriptions;
  private LogicEventSender[] additionalOutputs;
  private const bool IS_CIRCUIT_ENDPOINT = true;
  private bool connected;
  protected bool cleaningUp;
  private int lastAnimState = -1;
  [Serialize]
  protected int outputValueOne;
  [Serialize]
  protected int outputValueTwo;
  [Serialize]
  protected int outputValueThree;
  [Serialize]
  protected int outputValueFour;
  private LogicEventHandler inputOne;
  private LogicEventHandler inputTwo;
  private LogicEventHandler inputThree;
  private LogicEventHandler inputFour;
  private LogicPortVisualizer outputOne;
  private LogicPortVisualizer outputTwo;
  private LogicPortVisualizer outputThree;
  private LogicPortVisualizer outputFour;
  private LogicEventSender outputTwoSender;
  private LogicEventSender outputThreeSender;
  private LogicEventSender outputFourSender;
  private LogicEventHandler controlOne;
  private LogicEventHandler controlTwo;
  private static readonly EventSystem.IntraObjectHandler<LogicGate> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<LogicGate>((Action<LogicGate, object>) ((component, data) => component.OnBuildingBroken(data)));
  private static readonly EventSystem.IntraObjectHandler<LogicGate> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<LogicGate>((Action<LogicGate, object>) ((component, data) => component.OnBuildingFullyRepaired(data)));
  private static KAnimHashedString INPUT1_SYMBOL = KAnimHashedString.op_Implicit("input1");
  private static KAnimHashedString INPUT2_SYMBOL = KAnimHashedString.op_Implicit("input2");
  private static KAnimHashedString INPUT3_SYMBOL = KAnimHashedString.op_Implicit("input3");
  private static KAnimHashedString INPUT4_SYMBOL = KAnimHashedString.op_Implicit("input4");
  private static KAnimHashedString OUTPUT1_SYMBOL = KAnimHashedString.op_Implicit("output1");
  private static KAnimHashedString OUTPUT2_SYMBOL = KAnimHashedString.op_Implicit("output2");
  private static KAnimHashedString OUTPUT3_SYMBOL = KAnimHashedString.op_Implicit("output3");
  private static KAnimHashedString OUTPUT4_SYMBOL = KAnimHashedString.op_Implicit("output4");
  private static KAnimHashedString INPUT1_SYMBOL_BLM_RED = KAnimHashedString.op_Implicit("input1_red_bloom");
  private static KAnimHashedString INPUT1_SYMBOL_BLM_GRN = KAnimHashedString.op_Implicit("input1_green_bloom");
  private static KAnimHashedString INPUT2_SYMBOL_BLM_RED = KAnimHashedString.op_Implicit("input2_red_bloom");
  private static KAnimHashedString INPUT2_SYMBOL_BLM_GRN = KAnimHashedString.op_Implicit("input2_green_bloom");
  private static KAnimHashedString INPUT3_SYMBOL_BLM_RED = KAnimHashedString.op_Implicit("input3_red_bloom");
  private static KAnimHashedString INPUT3_SYMBOL_BLM_GRN = KAnimHashedString.op_Implicit("input3_green_bloom");
  private static KAnimHashedString INPUT4_SYMBOL_BLM_RED = KAnimHashedString.op_Implicit("input4_red_bloom");
  private static KAnimHashedString INPUT4_SYMBOL_BLM_GRN = KAnimHashedString.op_Implicit("input4_green_bloom");
  private static KAnimHashedString OUTPUT1_SYMBOL_BLM_RED = KAnimHashedString.op_Implicit("output1_red_bloom");
  private static KAnimHashedString OUTPUT1_SYMBOL_BLM_GRN = KAnimHashedString.op_Implicit("output1_green_bloom");
  private static KAnimHashedString OUTPUT2_SYMBOL_BLM_RED = KAnimHashedString.op_Implicit("output2_red_bloom");
  private static KAnimHashedString OUTPUT2_SYMBOL_BLM_GRN = KAnimHashedString.op_Implicit("output2_green_bloom");
  private static KAnimHashedString OUTPUT3_SYMBOL_BLM_RED = KAnimHashedString.op_Implicit("output3_red_bloom");
  private static KAnimHashedString OUTPUT3_SYMBOL_BLM_GRN = KAnimHashedString.op_Implicit("output3_green_bloom");
  private static KAnimHashedString OUTPUT4_SYMBOL_BLM_RED = KAnimHashedString.op_Implicit("output4_red_bloom");
  private static KAnimHashedString OUTPUT4_SYMBOL_BLM_GRN = KAnimHashedString.op_Implicit("output4_green_bloom");
  private static KAnimHashedString LINE_LEFT_1_SYMBOL = KAnimHashedString.op_Implicit("line_left_1");
  private static KAnimHashedString LINE_LEFT_2_SYMBOL = KAnimHashedString.op_Implicit("line_left_2");
  private static KAnimHashedString LINE_LEFT_3_SYMBOL = KAnimHashedString.op_Implicit("line_left_3");
  private static KAnimHashedString LINE_LEFT_4_SYMBOL = KAnimHashedString.op_Implicit("line_left_4");
  private static KAnimHashedString LINE_RIGHT_1_SYMBOL = KAnimHashedString.op_Implicit("line_right_1");
  private static KAnimHashedString LINE_RIGHT_2_SYMBOL = KAnimHashedString.op_Implicit("line_right_2");
  private static KAnimHashedString LINE_RIGHT_3_SYMBOL = KAnimHashedString.op_Implicit("line_right_3");
  private static KAnimHashedString LINE_RIGHT_4_SYMBOL = KAnimHashedString.op_Implicit("line_right_4");
  private static KAnimHashedString FLIPPER_1_SYMBOL = KAnimHashedString.op_Implicit("flipper1");
  private static KAnimHashedString FLIPPER_2_SYMBOL = KAnimHashedString.op_Implicit("flipper2");
  private static KAnimHashedString FLIPPER_3_SYMBOL = KAnimHashedString.op_Implicit("flipper3");
  private static KAnimHashedString INPUT_SYMBOL = KAnimHashedString.op_Implicit("input");
  private static KAnimHashedString OUTPUT_SYMBOL = KAnimHashedString.op_Implicit("output");
  private static KAnimHashedString INPUT1_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("input1_bloom");
  private static KAnimHashedString INPUT2_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("input2_bloom");
  private static KAnimHashedString INPUT3_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("input3_bloom");
  private static KAnimHashedString INPUT4_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("input4_bloom");
  private static KAnimHashedString OUTPUT1_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("output1_bloom");
  private static KAnimHashedString OUTPUT2_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("output2_bloom");
  private static KAnimHashedString OUTPUT3_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("output3_bloom");
  private static KAnimHashedString OUTPUT4_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("output4_bloom");
  private static KAnimHashedString LINE_LEFT_1_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("line_left_1_bloom");
  private static KAnimHashedString LINE_LEFT_2_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("line_left_2_bloom");
  private static KAnimHashedString LINE_LEFT_3_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("line_left_3_bloom");
  private static KAnimHashedString LINE_LEFT_4_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("line_left_4_bloom");
  private static KAnimHashedString LINE_RIGHT_1_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("line_right_1_bloom");
  private static KAnimHashedString LINE_RIGHT_2_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("line_right_2_bloom");
  private static KAnimHashedString LINE_RIGHT_3_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("line_right_3_bloom");
  private static KAnimHashedString LINE_RIGHT_4_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("line_right_4_bloom");
  private static KAnimHashedString FLIPPER_1_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("flipper1_bloom");
  private static KAnimHashedString FLIPPER_2_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("flipper2_bloom");
  private static KAnimHashedString FLIPPER_3_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("flipper3_bloom");
  private static KAnimHashedString INPUT_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("input_bloom");
  private static KAnimHashedString OUTPUT_SYMBOL_BLOOM = KAnimHashedString.op_Implicit("output_bloom");
  private static KAnimHashedString[][] multiplexerSymbolPaths = new KAnimHashedString[4][]
  {
    new KAnimHashedString[5]
    {
      LogicGate.LINE_LEFT_1_SYMBOL_BLOOM,
      LogicGate.FLIPPER_1_SYMBOL_BLOOM,
      LogicGate.LINE_RIGHT_1_SYMBOL_BLOOM,
      LogicGate.FLIPPER_3_SYMBOL_BLOOM,
      LogicGate.OUTPUT_SYMBOL_BLOOM
    },
    new KAnimHashedString[5]
    {
      LogicGate.LINE_LEFT_2_SYMBOL_BLOOM,
      LogicGate.FLIPPER_1_SYMBOL_BLOOM,
      LogicGate.LINE_RIGHT_1_SYMBOL_BLOOM,
      LogicGate.FLIPPER_3_SYMBOL_BLOOM,
      LogicGate.OUTPUT_SYMBOL_BLOOM
    },
    new KAnimHashedString[5]
    {
      LogicGate.LINE_LEFT_3_SYMBOL_BLOOM,
      LogicGate.FLIPPER_2_SYMBOL_BLOOM,
      LogicGate.LINE_RIGHT_2_SYMBOL_BLOOM,
      LogicGate.FLIPPER_3_SYMBOL_BLOOM,
      LogicGate.OUTPUT_SYMBOL_BLOOM
    },
    new KAnimHashedString[5]
    {
      LogicGate.LINE_LEFT_4_SYMBOL_BLOOM,
      LogicGate.FLIPPER_2_SYMBOL_BLOOM,
      LogicGate.LINE_RIGHT_2_SYMBOL_BLOOM,
      LogicGate.FLIPPER_3_SYMBOL_BLOOM,
      LogicGate.OUTPUT_SYMBOL_BLOOM
    }
  };
  private static KAnimHashedString[] multiplexerSymbols = new KAnimHashedString[10]
  {
    LogicGate.LINE_LEFT_1_SYMBOL,
    LogicGate.LINE_LEFT_2_SYMBOL,
    LogicGate.LINE_LEFT_3_SYMBOL,
    LogicGate.LINE_LEFT_4_SYMBOL,
    LogicGate.LINE_RIGHT_1_SYMBOL,
    LogicGate.LINE_RIGHT_2_SYMBOL,
    LogicGate.FLIPPER_1_SYMBOL,
    LogicGate.FLIPPER_2_SYMBOL,
    LogicGate.FLIPPER_3_SYMBOL,
    LogicGate.OUTPUT_SYMBOL
  };
  private static KAnimHashedString[] multiplexerBloomSymbols = new KAnimHashedString[10]
  {
    LogicGate.LINE_LEFT_1_SYMBOL_BLOOM,
    LogicGate.LINE_LEFT_2_SYMBOL_BLOOM,
    LogicGate.LINE_LEFT_3_SYMBOL_BLOOM,
    LogicGate.LINE_LEFT_4_SYMBOL_BLOOM,
    LogicGate.LINE_RIGHT_1_SYMBOL_BLOOM,
    LogicGate.LINE_RIGHT_2_SYMBOL_BLOOM,
    LogicGate.FLIPPER_1_SYMBOL_BLOOM,
    LogicGate.FLIPPER_2_SYMBOL_BLOOM,
    LogicGate.FLIPPER_3_SYMBOL_BLOOM,
    LogicGate.OUTPUT_SYMBOL_BLOOM
  };
  private static KAnimHashedString[][] demultiplexerSymbolPaths = new KAnimHashedString[4][]
  {
    new KAnimHashedString[4]
    {
      LogicGate.INPUT_SYMBOL_BLOOM,
      LogicGate.LINE_LEFT_1_SYMBOL_BLOOM,
      LogicGate.LINE_RIGHT_1_SYMBOL_BLOOM,
      LogicGate.OUTPUT1_SYMBOL
    },
    new KAnimHashedString[4]
    {
      LogicGate.INPUT_SYMBOL_BLOOM,
      LogicGate.LINE_LEFT_1_SYMBOL_BLOOM,
      LogicGate.LINE_RIGHT_2_SYMBOL_BLOOM,
      LogicGate.OUTPUT2_SYMBOL
    },
    new KAnimHashedString[4]
    {
      LogicGate.INPUT_SYMBOL_BLOOM,
      LogicGate.LINE_LEFT_2_SYMBOL_BLOOM,
      LogicGate.LINE_RIGHT_3_SYMBOL_BLOOM,
      LogicGate.OUTPUT3_SYMBOL
    },
    new KAnimHashedString[4]
    {
      LogicGate.INPUT_SYMBOL_BLOOM,
      LogicGate.LINE_LEFT_2_SYMBOL_BLOOM,
      LogicGate.LINE_RIGHT_4_SYMBOL_BLOOM,
      LogicGate.OUTPUT4_SYMBOL
    }
  };
  private static KAnimHashedString[] demultiplexerSymbols = new KAnimHashedString[7]
  {
    LogicGate.INPUT_SYMBOL,
    LogicGate.LINE_LEFT_1_SYMBOL,
    LogicGate.LINE_LEFT_2_SYMBOL,
    LogicGate.LINE_RIGHT_1_SYMBOL,
    LogicGate.LINE_RIGHT_2_SYMBOL,
    LogicGate.LINE_RIGHT_3_SYMBOL,
    LogicGate.LINE_RIGHT_4_SYMBOL
  };
  private static KAnimHashedString[] demultiplexerBloomSymbols = new KAnimHashedString[7]
  {
    LogicGate.INPUT_SYMBOL_BLOOM,
    LogicGate.LINE_LEFT_1_SYMBOL_BLOOM,
    LogicGate.LINE_LEFT_2_SYMBOL_BLOOM,
    LogicGate.LINE_RIGHT_1_SYMBOL_BLOOM,
    LogicGate.LINE_RIGHT_2_SYMBOL_BLOOM,
    LogicGate.LINE_RIGHT_3_SYMBOL_BLOOM,
    LogicGate.LINE_RIGHT_4_SYMBOL_BLOOM
  };
  private static KAnimHashedString[] demultiplexerOutputSymbols = new KAnimHashedString[4]
  {
    LogicGate.OUTPUT1_SYMBOL,
    LogicGate.OUTPUT2_SYMBOL,
    LogicGate.OUTPUT3_SYMBOL,
    LogicGate.OUTPUT4_SYMBOL
  };
  private static KAnimHashedString[] demultiplexerOutputRedSymbols = new KAnimHashedString[4]
  {
    LogicGate.OUTPUT1_SYMBOL_BLM_RED,
    LogicGate.OUTPUT2_SYMBOL_BLM_RED,
    LogicGate.OUTPUT3_SYMBOL_BLM_RED,
    LogicGate.OUTPUT4_SYMBOL_BLM_RED
  };
  private static KAnimHashedString[] demultiplexerOutputGreenSymbols = new KAnimHashedString[4]
  {
    LogicGate.OUTPUT1_SYMBOL_BLM_GRN,
    LogicGate.OUTPUT2_SYMBOL_BLM_GRN,
    LogicGate.OUTPUT3_SYMBOL_BLM_GRN,
    LogicGate.OUTPUT4_SYMBOL_BLM_GRN
  };
  private Color activeTintColor = new Color(0.5411765f, 0.9882353f, 0.298039228f);
  private Color inactiveTintColor = Color.red;

  protected virtual void OnSpawn()
  {
    this.inputOne = new LogicEventHandler(this.InputCellOne, new Action<int>(this.UpdateState), (Action<int, bool>) null, LogicPortSpriteType.Input);
    if (this.RequiresTwoInputs)
      this.inputTwo = new LogicEventHandler(this.InputCellTwo, new Action<int>(this.UpdateState), (Action<int, bool>) null, LogicPortSpriteType.Input);
    else if (this.RequiresFourInputs)
    {
      this.inputTwo = new LogicEventHandler(this.InputCellTwo, new Action<int>(this.UpdateState), (Action<int, bool>) null, LogicPortSpriteType.Input);
      this.inputThree = new LogicEventHandler(this.InputCellThree, new Action<int>(this.UpdateState), (Action<int, bool>) null, LogicPortSpriteType.Input);
      this.inputFour = new LogicEventHandler(this.InputCellFour, new Action<int>(this.UpdateState), (Action<int, bool>) null, LogicPortSpriteType.Input);
    }
    if (this.RequiresControlInputs)
    {
      this.controlOne = new LogicEventHandler(this.ControlCellOne, new Action<int>(this.UpdateState), (Action<int, bool>) null, LogicPortSpriteType.ControlInput);
      this.controlTwo = new LogicEventHandler(this.ControlCellTwo, new Action<int>(this.UpdateState), (Action<int, bool>) null, LogicPortSpriteType.ControlInput);
    }
    if (this.RequiresFourOutputs)
    {
      this.outputTwo = new LogicPortVisualizer(this.OutputCellTwo, LogicPortSpriteType.Output);
      this.outputThree = new LogicPortVisualizer(this.OutputCellThree, LogicPortSpriteType.Output);
      this.outputFour = new LogicPortVisualizer(this.OutputCellFour, LogicPortSpriteType.Output);
      this.outputTwoSender = new LogicEventSender(LogicGateBase.OUTPUT_TWO_PORT_ID, this.OutputCellTwo, (Action<int>) (new_value =>
      {
        if (!Object.op_Inequality((Object) this, (Object) null))
          return;
        this.OnAdditionalOutputsLogicValueChanged(LogicGateBase.OUTPUT_TWO_PORT_ID, new_value);
      }), (Action<int, bool>) null, LogicPortSpriteType.Output);
      this.outputThreeSender = new LogicEventSender(LogicGateBase.OUTPUT_THREE_PORT_ID, this.OutputCellThree, (Action<int>) (new_value =>
      {
        if (!Object.op_Inequality((Object) this, (Object) null))
          return;
        this.OnAdditionalOutputsLogicValueChanged(LogicGateBase.OUTPUT_THREE_PORT_ID, new_value);
      }), (Action<int, bool>) null, LogicPortSpriteType.Output);
      this.outputFourSender = new LogicEventSender(LogicGateBase.OUTPUT_FOUR_PORT_ID, this.OutputCellFour, (Action<int>) (new_value =>
      {
        if (!Object.op_Inequality((Object) this, (Object) null))
          return;
        this.OnAdditionalOutputsLogicValueChanged(LogicGateBase.OUTPUT_FOUR_PORT_ID, new_value);
      }), (Action<int, bool>) null, LogicPortSpriteType.Output);
    }
    this.Subscribe<LogicGate>(774203113, LogicGate.OnBuildingBrokenDelegate);
    this.Subscribe<LogicGate>(-1735440190, LogicGate.OnBuildingFullyRepairedDelegate);
    BuildingHP component = ((Component) this).GetComponent<BuildingHP>();
    if (!Object.op_Equality((Object) component, (Object) null) && component.IsBroken)
      return;
    this.Connect();
  }

  protected virtual void OnCleanUp()
  {
    this.cleaningUp = true;
    this.Disconnect();
    this.Unsubscribe<LogicGate>(774203113, LogicGate.OnBuildingBrokenDelegate, false);
    this.Unsubscribe<LogicGate>(-1735440190, LogicGate.OnBuildingFullyRepairedDelegate, false);
    base.OnCleanUp();
  }

  private void OnBuildingBroken(object data) => this.Disconnect();

  private void OnBuildingFullyRepaired(object data) => this.Connect();

  private void Connect()
  {
    if (this.connected)
      return;
    LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
    UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
    this.connected = true;
    int outputCellOne = this.OutputCellOne;
    logicCircuitSystem.AddToNetworks(outputCellOne, (object) this, true);
    this.outputOne = new LogicPortVisualizer(outputCellOne, LogicPortSpriteType.Output);
    logicCircuitManager.AddVisElem((ILogicUIElement) this.outputOne);
    if (this.RequiresFourOutputs)
    {
      this.outputTwo = new LogicPortVisualizer(this.OutputCellTwo, LogicPortSpriteType.Output);
      logicCircuitSystem.AddToNetworks(this.OutputCellTwo, (object) this.outputTwoSender, true);
      logicCircuitManager.AddVisElem((ILogicUIElement) this.outputTwo);
      this.outputThree = new LogicPortVisualizer(this.OutputCellThree, LogicPortSpriteType.Output);
      logicCircuitSystem.AddToNetworks(this.OutputCellThree, (object) this.outputThreeSender, true);
      logicCircuitManager.AddVisElem((ILogicUIElement) this.outputThree);
      this.outputFour = new LogicPortVisualizer(this.OutputCellFour, LogicPortSpriteType.Output);
      logicCircuitSystem.AddToNetworks(this.OutputCellFour, (object) this.outputFourSender, true);
      logicCircuitManager.AddVisElem((ILogicUIElement) this.outputFour);
    }
    int inputCellOne = this.InputCellOne;
    logicCircuitSystem.AddToNetworks(inputCellOne, (object) this.inputOne, true);
    logicCircuitManager.AddVisElem((ILogicUIElement) this.inputOne);
    if (this.RequiresTwoInputs)
    {
      int inputCellTwo = this.InputCellTwo;
      logicCircuitSystem.AddToNetworks(inputCellTwo, (object) this.inputTwo, true);
      logicCircuitManager.AddVisElem((ILogicUIElement) this.inputTwo);
    }
    else if (this.RequiresFourInputs)
    {
      logicCircuitSystem.AddToNetworks(this.InputCellTwo, (object) this.inputTwo, true);
      logicCircuitManager.AddVisElem((ILogicUIElement) this.inputTwo);
      logicCircuitSystem.AddToNetworks(this.InputCellThree, (object) this.inputThree, true);
      logicCircuitManager.AddVisElem((ILogicUIElement) this.inputThree);
      logicCircuitSystem.AddToNetworks(this.InputCellFour, (object) this.inputFour, true);
      logicCircuitManager.AddVisElem((ILogicUIElement) this.inputFour);
    }
    if (this.RequiresControlInputs)
    {
      logicCircuitSystem.AddToNetworks(this.ControlCellOne, (object) this.controlOne, true);
      logicCircuitManager.AddVisElem((ILogicUIElement) this.controlOne);
      logicCircuitSystem.AddToNetworks(this.ControlCellTwo, (object) this.controlTwo, true);
      logicCircuitManager.AddVisElem((ILogicUIElement) this.controlTwo);
    }
    this.RefreshAnimation();
  }

  private void Disconnect()
  {
    if (!this.connected)
      return;
    LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
    UtilityNetworkManager<LogicCircuitNetwork, LogicWire> logicCircuitSystem = Game.Instance.logicCircuitSystem;
    this.connected = false;
    int outputCellOne = this.OutputCellOne;
    logicCircuitSystem.RemoveFromNetworks(outputCellOne, (object) this, true);
    logicCircuitManager.RemoveVisElem((ILogicUIElement) this.outputOne);
    this.outputOne = (LogicPortVisualizer) null;
    if (this.RequiresFourOutputs)
    {
      logicCircuitSystem.RemoveFromNetworks(this.OutputCellTwo, (object) this.outputTwoSender, true);
      logicCircuitManager.RemoveVisElem((ILogicUIElement) this.outputTwo);
      this.outputTwo = (LogicPortVisualizer) null;
      logicCircuitSystem.RemoveFromNetworks(this.OutputCellThree, (object) this.outputThreeSender, true);
      logicCircuitManager.RemoveVisElem((ILogicUIElement) this.outputThree);
      this.outputThree = (LogicPortVisualizer) null;
      logicCircuitSystem.RemoveFromNetworks(this.OutputCellFour, (object) this.outputFourSender, true);
      logicCircuitManager.RemoveVisElem((ILogicUIElement) this.outputFour);
      this.outputFour = (LogicPortVisualizer) null;
    }
    int inputCellOne = this.InputCellOne;
    logicCircuitSystem.RemoveFromNetworks(inputCellOne, (object) this.inputOne, true);
    logicCircuitManager.RemoveVisElem((ILogicUIElement) this.inputOne);
    this.inputOne = (LogicEventHandler) null;
    if (this.RequiresTwoInputs)
    {
      int inputCellTwo = this.InputCellTwo;
      logicCircuitSystem.RemoveFromNetworks(inputCellTwo, (object) this.inputTwo, true);
      logicCircuitManager.RemoveVisElem((ILogicUIElement) this.inputTwo);
      this.inputTwo = (LogicEventHandler) null;
    }
    else if (this.RequiresFourInputs)
    {
      logicCircuitSystem.RemoveFromNetworks(this.InputCellTwo, (object) this.inputTwo, true);
      logicCircuitManager.RemoveVisElem((ILogicUIElement) this.inputTwo);
      this.inputTwo = (LogicEventHandler) null;
      logicCircuitSystem.RemoveFromNetworks(this.InputCellThree, (object) this.inputThree, true);
      logicCircuitManager.RemoveVisElem((ILogicUIElement) this.inputThree);
      this.inputThree = (LogicEventHandler) null;
      logicCircuitSystem.RemoveFromNetworks(this.InputCellFour, (object) this.inputFour, true);
      logicCircuitManager.RemoveVisElem((ILogicUIElement) this.inputFour);
      this.inputFour = (LogicEventHandler) null;
    }
    if (this.RequiresControlInputs)
    {
      logicCircuitSystem.RemoveFromNetworks(this.ControlCellOne, (object) this.controlOne, true);
      logicCircuitManager.RemoveVisElem((ILogicUIElement) this.controlOne);
      this.controlOne = (LogicEventHandler) null;
      logicCircuitSystem.RemoveFromNetworks(this.ControlCellTwo, (object) this.controlTwo, true);
      logicCircuitManager.RemoveVisElem((ILogicUIElement) this.controlTwo);
      this.controlTwo = (LogicEventHandler) null;
    }
    this.RefreshAnimation();
  }

  private void UpdateState(int new_value)
  {
    if (this.cleaningUp)
      return;
    int val1 = this.inputOne.Value;
    int val2 = this.inputTwo != null ? this.inputTwo.Value : 0;
    int num1 = this.inputThree != null ? this.inputThree.Value : 0;
    int num2 = this.inputFour != null ? this.inputFour.Value : 0;
    int num3 = this.controlOne != null ? this.controlOne.Value : 0;
    int num4 = this.controlTwo != null ? this.controlTwo.Value : 0;
    if (this.RequiresFourInputs && this.RequiresControlInputs)
    {
      this.outputValueOne = 0;
      if (this.op == LogicGateBase.Op.Multiplexer)
        this.outputValueOne = LogicCircuitNetwork.IsBitActive(0, num4) ? (LogicCircuitNetwork.IsBitActive(0, num3) ? num2 : num1) : (LogicCircuitNetwork.IsBitActive(0, num3) ? val2 : val1);
    }
    if (this.RequiresFourOutputs && this.RequiresControlInputs)
    {
      this.outputValueOne = 0;
      this.outputValueTwo = 0;
      this.outputTwoSender.SetValue(0);
      this.outputValueThree = 0;
      this.outputThreeSender.SetValue(0);
      this.outputValueFour = 0;
      this.outputFourSender.SetValue(0);
      if (this.op == LogicGateBase.Op.Demultiplexer)
      {
        if (!LogicCircuitNetwork.IsBitActive(0, num3))
        {
          if (!LogicCircuitNetwork.IsBitActive(0, num4))
          {
            this.outputValueOne = val1;
          }
          else
          {
            this.outputValueTwo = val1;
            this.outputTwoSender.SetValue(val1);
          }
        }
        else if (!LogicCircuitNetwork.IsBitActive(0, num4))
        {
          this.outputValueThree = val1;
          this.outputThreeSender.SetValue(val1);
        }
        else
        {
          this.outputValueFour = val1;
          this.outputFourSender.SetValue(val1);
        }
      }
    }
    switch (this.op)
    {
      case LogicGateBase.Op.And:
        this.outputValueOne = val1 & val2;
        break;
      case LogicGateBase.Op.Or:
        this.outputValueOne = val1 | val2;
        break;
      case LogicGateBase.Op.Not:
        LogicWire.BitDepth bitDepth = LogicWire.BitDepth.NumRatings;
        int inputCellOne = this.InputCellOne;
        GameObject gameObject = Grid.Objects[inputCellOne, 31];
        if (Object.op_Inequality((Object) gameObject, (Object) null))
        {
          LogicWire component = gameObject.GetComponent<LogicWire>();
          if (Object.op_Inequality((Object) component, (Object) null))
            bitDepth = component.MaxBitDepth;
        }
        this.outputValueOne = bitDepth == LogicWire.BitDepth.OneBit || bitDepth != LogicWire.BitDepth.FourBit ? (val1 == 0 ? 1 : 0) : (int) ((uint) ~val1 & 15U);
        break;
      case LogicGateBase.Op.Xor:
        this.outputValueOne = val1 ^ val2;
        break;
      case LogicGateBase.Op.CustomSingle:
        this.outputValueOne = this.GetCustomValue(val1, val2);
        break;
    }
    this.RefreshAnimation();
  }

  private void OnAdditionalOutputsLogicValueChanged(HashedString port_id, int new_value)
  {
    if (!Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null))
      return;
    EventExtensions.Trigger(((Component) this).gameObject, -801688580, (object) new LogicValueChanged()
    {
      portID = port_id,
      newValue = new_value
    });
  }

  public virtual void LogicTick()
  {
  }

  protected virtual int GetCustomValue(int val1, int val2) => val1;

  public int GetPortValue(LogicGateBase.PortId port)
  {
    switch (port)
    {
      case LogicGateBase.PortId.InputOne:
        return this.inputOne.Value;
      case LogicGateBase.PortId.InputTwo:
        return this.RequiresTwoInputs || this.RequiresFourInputs ? this.inputTwo.Value : 0;
      case LogicGateBase.PortId.InputThree:
        return !this.RequiresFourInputs ? 0 : this.inputThree.Value;
      case LogicGateBase.PortId.InputFour:
        return !this.RequiresFourInputs ? 0 : this.inputFour.Value;
      case LogicGateBase.PortId.OutputOne:
        return this.outputValueOne;
      case LogicGateBase.PortId.OutputTwo:
        return this.outputValueTwo;
      case LogicGateBase.PortId.OutputThree:
        return this.outputValueThree;
      case LogicGateBase.PortId.OutputFour:
        return this.outputValueFour;
      case LogicGateBase.PortId.ControlOne:
        return this.controlOne.Value;
      case LogicGateBase.PortId.ControlTwo:
        return this.controlTwo.Value;
      default:
        return this.outputValueOne;
    }
  }

  public bool GetPortConnected(LogicGateBase.PortId port) => (port != LogicGateBase.PortId.InputTwo || this.RequiresTwoInputs || this.RequiresFourInputs) && (port != LogicGateBase.PortId.InputThree || this.RequiresFourInputs) && (port != LogicGateBase.PortId.InputFour || this.RequiresFourInputs) && Game.Instance.logicCircuitManager.GetNetworkForCell(this.PortCell(port)) != null;

  public void SetPortDescriptions(LogicGate.LogicGateDescriptions descriptions) => this.descriptions = descriptions;

  public LogicGate.LogicGateDescriptions.Description GetPortDescription(LogicGateBase.PortId port)
  {
    switch (port)
    {
      case LogicGateBase.PortId.InputOne:
        if (this.descriptions.inputOne != null)
          return this.descriptions.inputOne;
        return !this.RequiresTwoInputs && !this.RequiresFourInputs ? LogicGate.INPUT_ONE_SINGLE_DESCRIPTION : LogicGate.INPUT_ONE_MULTI_DESCRIPTION;
      case LogicGateBase.PortId.InputTwo:
        return this.descriptions.inputTwo == null ? LogicGate.INPUT_TWO_DESCRIPTION : this.descriptions.inputTwo;
      case LogicGateBase.PortId.InputThree:
        return this.descriptions.inputThree == null ? LogicGate.INPUT_THREE_DESCRIPTION : this.descriptions.inputThree;
      case LogicGateBase.PortId.InputFour:
        return this.descriptions.inputFour == null ? LogicGate.INPUT_FOUR_DESCRIPTION : this.descriptions.inputFour;
      case LogicGateBase.PortId.OutputOne:
        if (this.descriptions.inputOne != null)
          return this.descriptions.inputOne;
        return !this.RequiresFourOutputs ? LogicGate.OUTPUT_ONE_SINGLE_DESCRIPTION : LogicGate.OUTPUT_ONE_MULTI_DESCRIPTION;
      case LogicGateBase.PortId.OutputTwo:
        return this.descriptions.outputTwo == null ? LogicGate.OUTPUT_TWO_DESCRIPTION : this.descriptions.outputTwo;
      case LogicGateBase.PortId.OutputThree:
        return this.descriptions.outputThree == null ? LogicGate.OUTPUT_THREE_DESCRIPTION : this.descriptions.outputThree;
      case LogicGateBase.PortId.OutputFour:
        return this.descriptions.outputFour == null ? LogicGate.OUTPUT_FOUR_DESCRIPTION : this.descriptions.outputFour;
      case LogicGateBase.PortId.ControlOne:
        return this.descriptions.controlOne == null ? LogicGate.CONTROL_ONE_DESCRIPTION : this.descriptions.controlOne;
      case LogicGateBase.PortId.ControlTwo:
        return this.descriptions.controlTwo == null ? LogicGate.CONTROL_TWO_DESCRIPTION : this.descriptions.controlTwo;
      default:
        return this.descriptions.outputOne;
    }
  }

  public int GetLogicValue() => this.outputValueOne;

  public int GetLogicCell() => this.GetLogicUICell();

  public int GetLogicUICell() => this.OutputCellOne;

  public bool IsLogicInput() => false;

  private LogicEventHandler GetInputFromControlValue(int val)
  {
    switch (val)
    {
      case 1:
        return this.inputTwo;
      case 2:
        return this.inputThree;
      case 3:
        return this.inputFour;
      default:
        return this.inputOne;
    }
  }

  private void ShowSymbolConditionally(
    bool showAnything,
    bool active,
    KBatchedAnimController kbac,
    KAnimHashedString ifTrue,
    KAnimHashedString ifFalse)
  {
    if (!showAnything)
    {
      kbac.SetSymbolVisiblity(ifTrue, false);
      kbac.SetSymbolVisiblity(ifFalse, false);
    }
    else
    {
      kbac.SetSymbolVisiblity(ifTrue, active);
      kbac.SetSymbolVisiblity(ifFalse, !active);
    }
  }

  private void TintSymbolConditionally(
    bool tintAnything,
    bool condition,
    KBatchedAnimController kbac,
    KAnimHashedString symbol,
    Color ifTrue,
    Color ifFalse)
  {
    if (tintAnything)
      kbac.SetSymbolTint(symbol, condition ? ifTrue : ifFalse);
    else
      kbac.SetSymbolTint(symbol, Color.white);
  }

  private void SetBloomSymbolShowing(
    bool showing,
    KBatchedAnimController kbac,
    KAnimHashedString symbol,
    KAnimHashedString bloomSymbol)
  {
    kbac.SetSymbolVisiblity(bloomSymbol, showing);
    kbac.SetSymbolVisiblity(symbol, !showing);
  }

  protected void RefreshAnimation()
  {
    if (this.cleaningUp)
      return;
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    if (this.op == LogicGateBase.Op.Multiplexer)
    {
      int val = LogicCircuitNetwork.GetBitValue(0, this.controlOne.Value) + LogicCircuitNetwork.GetBitValue(0, this.controlTwo.Value) * 2;
      if (this.lastAnimState != val)
      {
        if (this.lastAnimState == -1)
          component.Play(HashedString.op_Implicit(val.ToString()));
        else
          component.Play(HashedString.op_Implicit(this.lastAnimState.ToString() + "_" + val.ToString()));
      }
      this.lastAnimState = val;
      LogicEventHandler fromControlValue = this.GetInputFromControlValue(val);
      KAnimHashedString[] multiplexerSymbolPath = LogicGate.multiplexerSymbolPaths[val];
      LogicCircuitNetwork networkForCell1 = Game.Instance.logicCircuitSystem.GetNetworkForCell(fromControlValue.GetLogicCell()) as LogicCircuitNetwork;
      UtilityNetwork networkForCell2 = Game.Instance.logicCircuitSystem.GetNetworkForCell(this.InputCellOne);
      UtilityNetwork networkForCell3 = Game.Instance.logicCircuitSystem.GetNetworkForCell(this.InputCellTwo);
      UtilityNetwork networkForCell4 = Game.Instance.logicCircuitSystem.GetNetworkForCell(this.InputCellThree);
      UtilityNetwork networkForCell5 = Game.Instance.logicCircuitSystem.GetNetworkForCell(this.InputCellFour);
      this.ShowSymbolConditionally(networkForCell2 != null, this.inputOne.Value == 0, component, LogicGate.INPUT1_SYMBOL_BLM_RED, LogicGate.INPUT1_SYMBOL_BLM_GRN);
      this.ShowSymbolConditionally(networkForCell3 != null, this.inputTwo.Value == 0, component, LogicGate.INPUT2_SYMBOL_BLM_RED, LogicGate.INPUT2_SYMBOL_BLM_GRN);
      this.ShowSymbolConditionally(networkForCell4 != null, this.inputThree.Value == 0, component, LogicGate.INPUT3_SYMBOL_BLM_RED, LogicGate.INPUT3_SYMBOL_BLM_GRN);
      this.ShowSymbolConditionally(networkForCell5 != null, this.inputFour.Value == 0, component, LogicGate.INPUT4_SYMBOL_BLM_RED, LogicGate.INPUT4_SYMBOL_BLM_GRN);
      this.ShowSymbolConditionally(networkForCell1 != null, fromControlValue.Value == 0, component, LogicGate.OUTPUT1_SYMBOL_BLM_RED, LogicGate.OUTPUT1_SYMBOL_BLM_GRN);
      this.TintSymbolConditionally(networkForCell2 != null, this.inputOne.Value == 0, component, LogicGate.INPUT1_SYMBOL, this.inactiveTintColor, this.activeTintColor);
      this.TintSymbolConditionally(networkForCell3 != null, this.inputTwo.Value == 0, component, LogicGate.INPUT2_SYMBOL, this.inactiveTintColor, this.activeTintColor);
      this.TintSymbolConditionally(networkForCell4 != null, this.inputThree.Value == 0, component, LogicGate.INPUT3_SYMBOL, this.inactiveTintColor, this.activeTintColor);
      this.TintSymbolConditionally(networkForCell5 != null, this.inputFour.Value == 0, component, LogicGate.INPUT4_SYMBOL, this.inactiveTintColor, this.activeTintColor);
      this.TintSymbolConditionally(Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) != null && networkForCell1 != null, fromControlValue.Value == 0, component, LogicGate.OUTPUT1_SYMBOL, this.inactiveTintColor, this.activeTintColor);
      for (int index = 0; index < LogicGate.multiplexerSymbols.Length; ++index)
      {
        KAnimHashedString multiplexerSymbol = LogicGate.multiplexerSymbols[index];
        KAnimHashedString multiplexerBloomSymbol = LogicGate.multiplexerBloomSymbols[index];
        bool showing = Array.IndexOf<KAnimHashedString>(multiplexerSymbolPath, multiplexerBloomSymbol) != -1 && networkForCell1 != null;
        this.SetBloomSymbolShowing(showing, component, multiplexerSymbol, multiplexerBloomSymbol);
        if (showing)
          component.SetSymbolTint(multiplexerBloomSymbol, fromControlValue.Value == 0 ? this.inactiveTintColor : this.activeTintColor);
      }
    }
    else if (this.op == LogicGateBase.Op.Demultiplexer)
    {
      int index1 = LogicCircuitNetwork.GetBitValue(0, this.controlOne.Value) * 2 + LogicCircuitNetwork.GetBitValue(0, this.controlTwo.Value);
      if (this.lastAnimState != index1)
      {
        if (this.lastAnimState == -1)
          component.Play(HashedString.op_Implicit(index1.ToString()));
        else
          component.Play(HashedString.op_Implicit(this.lastAnimState.ToString() + "_" + index1.ToString()));
      }
      this.lastAnimState = index1;
      KAnimHashedString[] demultiplexerSymbolPath = LogicGate.demultiplexerSymbolPaths[index1];
      LogicCircuitNetwork networkForCell6 = Game.Instance.logicCircuitSystem.GetNetworkForCell(this.inputOne.GetLogicCell()) as LogicCircuitNetwork;
      for (int index2 = 0; index2 < LogicGate.demultiplexerSymbols.Length; ++index2)
      {
        KAnimHashedString demultiplexerSymbol = LogicGate.demultiplexerSymbols[index2];
        KAnimHashedString demultiplexerBloomSymbol = LogicGate.demultiplexerBloomSymbols[index2];
        bool showing = Array.IndexOf<KAnimHashedString>(demultiplexerSymbolPath, demultiplexerBloomSymbol) != -1 && networkForCell6 != null;
        this.SetBloomSymbolShowing(showing, component, demultiplexerSymbol, demultiplexerBloomSymbol);
        if (showing)
          component.SetSymbolTint(demultiplexerBloomSymbol, this.inputOne.Value == 0 ? this.inactiveTintColor : this.activeTintColor);
      }
      this.ShowSymbolConditionally(networkForCell6 != null, this.inputOne.Value == 0, component, LogicGate.INPUT1_SYMBOL_BLM_RED, LogicGate.INPUT1_SYMBOL_BLM_GRN);
      if (networkForCell6 != null)
        component.SetSymbolTint(LogicGate.INPUT1_SYMBOL_BLOOM, this.inputOne.Value == 0 ? this.inactiveTintColor : this.activeTintColor);
      int[] numArray = new int[4]
      {
        this.OutputCellOne,
        this.OutputCellTwo,
        this.OutputCellThree,
        this.OutputCellFour
      };
      for (int index3 = 0; index3 < LogicGate.demultiplexerOutputSymbols.Length; ++index3)
      {
        KAnimHashedString demultiplexerOutputSymbol = LogicGate.demultiplexerOutputSymbols[index3];
        bool flag = Array.IndexOf<KAnimHashedString>(demultiplexerSymbolPath, demultiplexerOutputSymbol) == -1 || this.inputOne.Value == 0;
        UtilityNetwork networkForCell7 = Game.Instance.logicCircuitSystem.GetNetworkForCell(numArray[index3]);
        this.TintSymbolConditionally(networkForCell6 != null && networkForCell7 != null, flag, component, demultiplexerOutputSymbol, this.inactiveTintColor, this.activeTintColor);
        this.ShowSymbolConditionally(networkForCell6 != null && networkForCell7 != null, flag, component, LogicGate.demultiplexerOutputRedSymbols[index3], LogicGate.demultiplexerOutputGreenSymbols[index3]);
      }
    }
    else if (this.op == LogicGateBase.Op.And || this.op == LogicGateBase.Op.Xor || this.op == LogicGateBase.Op.Not || this.op == LogicGateBase.Op.Or)
    {
      if (!(Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) is LogicCircuitNetwork))
        component.Play(HashedString.op_Implicit("off"));
      else if (this.RequiresTwoInputs)
      {
        int num = this.inputOne.Value * 2 + this.inputTwo.Value;
        if (this.lastAnimState == num)
          return;
        if (this.lastAnimState == -1)
          component.Play(HashedString.op_Implicit(num.ToString()));
        else
          component.Play(HashedString.op_Implicit(this.lastAnimState.ToString() + "_" + num.ToString()));
        this.lastAnimState = num;
      }
      else
      {
        int num = this.inputOne.Value;
        if (this.lastAnimState == num)
          return;
        if (this.lastAnimState == -1)
          component.Play(HashedString.op_Implicit(num.ToString()));
        else
          component.Play(HashedString.op_Implicit(this.lastAnimState.ToString() + "_" + num.ToString()));
        this.lastAnimState = num;
      }
    }
    else if (!(Game.Instance.logicCircuitSystem.GetNetworkForCell(this.OutputCellOne) is LogicCircuitNetwork))
      component.Play(HashedString.op_Implicit("off"));
    else if (this.RequiresTwoInputs)
    {
      int num = this.inputOne.Value + this.inputTwo.Value * 2 + this.outputValueOne * 4;
      component.Play(HashedString.op_Implicit("on_" + num.ToString()));
    }
    else
    {
      int num = this.inputOne.Value + this.outputValueOne * 4;
      component.Play(HashedString.op_Implicit("on_" + num.ToString()));
    }
  }

  public void OnLogicNetworkConnectionChanged(bool connected)
  {
  }

  public class LogicGateDescriptions
  {
    public LogicGate.LogicGateDescriptions.Description inputOne;
    public LogicGate.LogicGateDescriptions.Description inputTwo;
    public LogicGate.LogicGateDescriptions.Description inputThree;
    public LogicGate.LogicGateDescriptions.Description inputFour;
    public LogicGate.LogicGateDescriptions.Description outputOne;
    public LogicGate.LogicGateDescriptions.Description outputTwo;
    public LogicGate.LogicGateDescriptions.Description outputThree;
    public LogicGate.LogicGateDescriptions.Description outputFour;
    public LogicGate.LogicGateDescriptions.Description controlOne;
    public LogicGate.LogicGateDescriptions.Description controlTwo;

    public class Description
    {
      public string name;
      public string active;
      public string inactive;
    }
  }
}
