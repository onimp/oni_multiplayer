// Decompiled with JetBrains decompiler
// Type: PixelPack
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/PixelPack")]
public class PixelPack : KMonoBehaviour, ISaveLoadable
{
  protected KBatchedAnimController animController;
  private static readonly EventSystem.IntraObjectHandler<PixelPack> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<PixelPack>((Action<PixelPack, object>) ((component, data) => component.OnLogicValueChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<PixelPack> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<PixelPack>((Action<PixelPack, object>) ((component, data) => component.OnOperationalChanged(data)));
  public static readonly HashedString PORT_ID = new HashedString("PixelPackInput");
  public static readonly HashedString SYMBOL_ONE_NAME = HashedString.op_Implicit("screen1");
  public static readonly HashedString SYMBOL_TWO_NAME = HashedString.op_Implicit("screen2");
  public static readonly HashedString SYMBOL_THREE_NAME = HashedString.op_Implicit("screen3");
  public static readonly HashedString SYMBOL_FOUR_NAME = HashedString.op_Implicit("screen4");
  [MyCmpGet]
  private Operational operational;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<PixelPack> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<PixelPack>((Action<PixelPack, object>) ((component, data) => component.OnCopySettings(data)));
  public int logicValue;
  [Serialize]
  public List<PixelPack.ColorPair> colorSettings;
  private Color defaultActive = new Color(0.345098048f, 0.847058833f, 0.329411775f);
  private Color defaultStandby = new Color(0.972549f, 0.470588237f, 0.345098048f);
  protected static readonly HashedString[] ON_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("on_pre"),
    HashedString.op_Implicit("on")
  };
  protected static readonly HashedString[] OFF_ANIMS = new HashedString[2]
  {
    HashedString.op_Implicit("off_pre"),
    HashedString.op_Implicit("off")
  };

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<PixelPack>(-905833192, PixelPack.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    PixelPack component = ((GameObject) data).GetComponent<PixelPack>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      for (int index = 0; index < component.colorSettings.Count; ++index)
        this.colorSettings[index] = component.colorSettings[index];
    }
    this.UpdateColors();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.animController = ((Component) this).GetComponent<KBatchedAnimController>();
    this.Subscribe<PixelPack>(-801688580, PixelPack.OnLogicValueChangedDelegate);
    this.Subscribe<PixelPack>(-592767678, PixelPack.OnOperationalChangedDelegate);
    if (this.colorSettings != null)
      return;
    PixelPack.ColorPair colorPair1 = new PixelPack.ColorPair()
    {
      activeColor = this.defaultActive,
      standbyColor = this.defaultStandby
    };
    PixelPack.ColorPair colorPair2 = new PixelPack.ColorPair();
    colorPair2.activeColor = this.defaultActive;
    colorPair2.standbyColor = this.defaultStandby;
    PixelPack.ColorPair colorPair3 = colorPair2;
    colorPair2 = new PixelPack.ColorPair();
    colorPair2.activeColor = this.defaultActive;
    colorPair2.standbyColor = this.defaultStandby;
    PixelPack.ColorPair colorPair4 = colorPair2;
    colorPair2 = new PixelPack.ColorPair();
    colorPair2.activeColor = this.defaultActive;
    colorPair2.standbyColor = this.defaultStandby;
    PixelPack.ColorPair colorPair5 = colorPair2;
    this.colorSettings = new List<PixelPack.ColorPair>();
    this.colorSettings.Add(colorPair1);
    this.colorSettings.Add(colorPair3);
    this.colorSettings.Add(colorPair4);
    this.colorSettings.Add(colorPair5);
  }

  private void OnLogicValueChanged(object data)
  {
    LogicValueChanged logicValueChanged = (LogicValueChanged) data;
    if (!HashedString.op_Equality(logicValueChanged.portID, PixelPack.PORT_ID))
      return;
    this.logicValue = logicValueChanged.newValue;
    this.UpdateColors();
  }

  private void OnOperationalChanged(object data)
  {
    if (this.operational.IsOperational)
    {
      this.UpdateColors();
      this.animController.Play(PixelPack.ON_ANIMS);
    }
    else
      this.animController.Play(PixelPack.OFF_ANIMS);
    this.operational.SetActive(this.operational.IsOperational);
  }

  public void UpdateColors()
  {
    if (!this.operational.IsOperational)
      return;
    LogicPorts component = ((Component) this).GetComponent<LogicPorts>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    switch (component.GetConnectedWireBitDepth(PixelPack.PORT_ID))
    {
      case LogicWire.BitDepth.OneBit:
        this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(PixelPack.SYMBOL_ONE_NAME), LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[0].activeColor : this.colorSettings[0].standbyColor);
        this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(PixelPack.SYMBOL_TWO_NAME), LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[1].activeColor : this.colorSettings[1].standbyColor);
        this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(PixelPack.SYMBOL_THREE_NAME), LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[2].activeColor : this.colorSettings[2].standbyColor);
        this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(PixelPack.SYMBOL_FOUR_NAME), LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[3].activeColor : this.colorSettings[3].standbyColor);
        break;
      case LogicWire.BitDepth.FourBit:
        this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(PixelPack.SYMBOL_ONE_NAME), LogicCircuitNetwork.IsBitActive(0, this.logicValue) ? this.colorSettings[0].activeColor : this.colorSettings[0].standbyColor);
        this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(PixelPack.SYMBOL_TWO_NAME), LogicCircuitNetwork.IsBitActive(1, this.logicValue) ? this.colorSettings[1].activeColor : this.colorSettings[1].standbyColor);
        this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(PixelPack.SYMBOL_THREE_NAME), LogicCircuitNetwork.IsBitActive(2, this.logicValue) ? this.colorSettings[2].activeColor : this.colorSettings[2].standbyColor);
        this.animController.SetSymbolTint(KAnimHashedString.op_Implicit(PixelPack.SYMBOL_FOUR_NAME), LogicCircuitNetwork.IsBitActive(3, this.logicValue) ? this.colorSettings[3].activeColor : this.colorSettings[3].standbyColor);
        break;
    }
  }

  public struct ColorPair
  {
    public Color activeColor;
    public Color standbyColor;
  }
}
