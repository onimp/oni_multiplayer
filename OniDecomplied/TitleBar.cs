// Decompiled with JetBrains decompiler
// Type: TitleBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/TitleBar")]
public class TitleBar : KMonoBehaviour
{
  public LocText titleText;
  public LocText subtextText;
  public GameObject WarningNotification;
  public Text NotificationText;
  public Image NotificationIcon;
  public Sprite techIcon;
  public Sprite materialIcon;
  public TitleBarPortrait portrait;
  public bool userEditable;
  public bool setCameraControllerState = true;

  public void SetTitle(string Name) => ((TMP_Text) this.titleText).text = Name;

  public void SetSubText(string subtext, string tooltip = "")
  {
    ((TMP_Text) this.subtextText).text = subtext;
    ((Component) this.subtextText).GetComponent<ToolTip>().toolTip = tooltip;
  }

  public void SetWarningActve(bool state) => this.WarningNotification.SetActive(state);

  public void SetWarning(Sprite icon, string label)
  {
    this.SetWarningActve(true);
    this.NotificationIcon.sprite = icon;
    this.NotificationText.text = label;
  }

  public void SetPortrait(GameObject target) => this.portrait.SetPortrait(target);
}
