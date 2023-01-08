// Decompiled with JetBrains decompiler
// Type: LogicControlInputUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/LogicRibbonDisplayUI")]
public class LogicControlInputUI : KMonoBehaviour
{
  [SerializeField]
  private Image icon;
  [SerializeField]
  private Image border;
  [SerializeField]
  private LogicModeUI uiAsset;
  private Color32 colourOn;
  private Color32 colourOff;
  private Color32 colourDisconnected;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.colourOn = GlobalAssets.Instance.colorSet.logicOn;
    this.colourOff = GlobalAssets.Instance.colorSet.logicOff;
    this.colourOn.a = this.colourOff.a = byte.MaxValue;
    this.colourDisconnected = GlobalAssets.Instance.colorSet.logicDisconnected;
    ((Graphic) this.icon).raycastTarget = false;
    ((Graphic) this.border).raycastTarget = false;
  }

  public void SetContent(LogicCircuitNetwork network) => ((Graphic) this.icon).color = Color32.op_Implicit(network == null ? GlobalAssets.Instance.colorSet.logicDisconnected : (network.IsBitActive(0) ? this.colourOn : this.colourOff));
}
