// Decompiled with JetBrains decompiler
// Type: KToggleMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KToggleMenu : KScreen
{
  [SerializeField]
  private Transform toggleParent;
  [SerializeField]
  private KToggle prefab;
  [SerializeField]
  private ToggleGroup group;
  protected IList<KToggleMenu.ToggleInfo> toggleInfo;
  protected List<KToggle> toggles = new List<KToggle>();
  private static int selected = -1;

  public event KToggleMenu.OnSelect onSelect;

  public void Setup(IList<KToggleMenu.ToggleInfo> toggleInfo)
  {
    this.toggleInfo = toggleInfo;
    this.RefreshButtons();
  }

  protected void Setup() => this.RefreshButtons();

  private void RefreshButtons()
  {
    foreach (KToggle toggle in this.toggles)
    {
      if (Object.op_Inequality((Object) toggle, (Object) null))
        Object.Destroy((Object) ((Component) toggle).gameObject);
    }
    this.toggles.Clear();
    if (this.toggleInfo == null)
      return;
    Transform transform = Object.op_Inequality((Object) this.toggleParent, (Object) null) ? this.toggleParent : ((KMonoBehaviour) this).transform;
    for (int index = 0; index < this.toggleInfo.Count; ++index)
    {
      int idx = index;
      KToggleMenu.ToggleInfo toggleInfo = this.toggleInfo[index];
      if (toggleInfo == null)
      {
        this.toggles.Add((KToggle) null);
      }
      else
      {
        KToggle ktoggle = Object.Instantiate<KToggle>(this.prefab, Vector3.zero, Quaternion.identity);
        ((Object) ((Component) ktoggle).gameObject).name = "Toggle:" + toggleInfo.text;
        ((Component) ktoggle).transform.SetParent(transform, false);
        ((Toggle) ktoggle).group = this.group;
        ktoggle.onClick += (System.Action) (() => this.OnClick(idx));
        ((Component) ktoggle).GetComponentsInChildren<Text>(true)[0].text = toggleInfo.text;
        toggleInfo.toggle = ktoggle;
        this.toggles.Add(ktoggle);
      }
    }
  }

  public int GetSelected() => KToggleMenu.selected;

  private void OnClick(int i)
  {
    UISounds.PlaySound(UISounds.Sound.ClickObject);
    if (this.onSelect == null)
      return;
    this.onSelect(this.toggleInfo[i]);
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (this.toggles == null)
      return;
    for (int index = 0; index < this.toggleInfo.Count; ++index)
    {
      Action hotKey = this.toggleInfo[index].hotKey;
      if (hotKey != 275 && e.TryConsume(hotKey))
      {
        this.toggles[index].Click();
        break;
      }
    }
  }

  public delegate void OnSelect(KToggleMenu.ToggleInfo toggleInfo);

  public class ToggleInfo
  {
    public string text;
    public object userData;
    public KToggle toggle;
    public Action hotKey;

    public ToggleInfo(string text, object user_data = null, Action hotKey = 275)
    {
      this.text = text;
      this.userData = user_data;
      this.hotKey = hotKey;
    }
  }
}
