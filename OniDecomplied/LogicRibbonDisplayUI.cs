// Decompiled with JetBrains decompiler
// Type: LogicRibbonDisplayUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/LogicRibbonDisplayUI")]
public class LogicRibbonDisplayUI : KMonoBehaviour
{
  [SerializeField]
  private Image wire1;
  [SerializeField]
  private Image wire2;
  [SerializeField]
  private Image wire3;
  [SerializeField]
  private Image wire4;
  [SerializeField]
  private LogicModeUI uiAsset;
  private Color32 colourOn;
  private Color32 colourOff;
  private Color32 colourDisconnected = Color32.op_Implicit(new Color((float) byte.MaxValue, (float) byte.MaxValue, (float) byte.MaxValue, (float) byte.MaxValue));
  private int bitDepth = 4;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.colourOn = GlobalAssets.Instance.colorSet.logicOn;
    this.colourOff = GlobalAssets.Instance.colorSet.logicOff;
    this.colourOn.a = this.colourOff.a = byte.MaxValue;
    ((Graphic) this.wire1).raycastTarget = false;
    ((Graphic) this.wire2).raycastTarget = false;
    ((Graphic) this.wire3).raycastTarget = false;
    ((Graphic) this.wire4).raycastTarget = false;
  }

  public void SetContent(LogicCircuitNetwork network)
  {
    Color32 colourDisconnected = this.colourDisconnected;
    List<Color32> color32List = new List<Color32>();
    for (int bit = 0; bit < this.bitDepth; ++bit)
      color32List.Add(network == null ? colourDisconnected : (network.IsBitActive(bit) ? this.colourOn : this.colourOff));
    if (Color.op_Inequality(((Graphic) this.wire1).color, Color32.op_Implicit(color32List[0])))
      ((Graphic) this.wire1).color = Color32.op_Implicit(color32List[0]);
    if (Color.op_Inequality(((Graphic) this.wire2).color, Color32.op_Implicit(color32List[1])))
      ((Graphic) this.wire2).color = Color32.op_Implicit(color32List[1]);
    if (Color.op_Inequality(((Graphic) this.wire3).color, Color32.op_Implicit(color32List[2])))
      ((Graphic) this.wire3).color = Color32.op_Implicit(color32List[2]);
    if (!Color.op_Inequality(((Graphic) this.wire4).color, Color32.op_Implicit(color32List[3])))
      return;
    ((Graphic) this.wire4).color = Color32.op_Implicit(color32List[3]);
  }
}
