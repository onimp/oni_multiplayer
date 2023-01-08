// Decompiled with JetBrains decompiler
// Type: KPopupMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KPopupMenu : KScreen
{
  [SerializeField]
  private KButtonMenu buttonMenu;
  private KButtonMenu.ButtonInfo[] Buttons;
  public Action<string, int> OnSelect;

  public void SetOptions(IList<string> options)
  {
    List<KButtonMenu.ButtonInfo> buttonInfoList = new List<KButtonMenu.ButtonInfo>();
    for (int index = 0; index < options.Count; ++index)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      KPopupMenu.\u003C\u003Ec__DisplayClass3_0 cDisplayClass30 = new KPopupMenu.\u003C\u003Ec__DisplayClass3_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.index = index;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass30.option = options[index];
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      buttonInfoList.Add(new KButtonMenu.ButtonInfo(cDisplayClass30.option, (Action) 275, new UnityAction((object) cDisplayClass30, __methodptr(\u003CSetOptions\u003Eb__0))));
    }
    this.Buttons = buttonInfoList.ToArray();
  }

  public void OnClick()
  {
    if (this.Buttons == null)
      return;
    if (((Component) this).gameObject.activeSelf)
    {
      ((Component) this).gameObject.SetActive(false);
    }
    else
    {
      this.buttonMenu.SetButtons((IList<KButtonMenu.ButtonInfo>) this.Buttons);
      this.buttonMenu.RefreshButtons();
      ((Component) this).gameObject.SetActive(true);
    }
  }

  public void SelectOption(string option, int index)
  {
    if (this.OnSelect != null)
      this.OnSelect(option, index);
    ((Component) this).gameObject.SetActive(false);
  }

  public IList<KButtonMenu.ButtonInfo> GetButtons() => (IList<KButtonMenu.ButtonInfo>) this.Buttons;
}
