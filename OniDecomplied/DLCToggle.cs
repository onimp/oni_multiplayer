// Decompiled with JetBrains decompiler
// Type: DLCToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DLCToggle : KMonoBehaviour
{
  [SerializeField]
  private KButton button;
  [SerializeField]
  private LocText label;
  [SerializeField]
  private Image logo;
  private bool expansion1Active;

  protected virtual void OnPrefabInit()
  {
    this.expansion1Active = DlcManager.IsExpansion1Active();
    this.button.onClick += new System.Action(this.ToggleExpansion1Cicked);
    ((TMP_Text) this.label).text = (string) (this.expansion1Active ? STRINGS.UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1 : STRINGS.UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1);
    this.logo.sprite = this.expansion1Active ? GlobalResources.Instance().baseGameLogoSmall : GlobalResources.Instance().expansion1LogoSmall;
    ((Component) this.logo).gameObject.SetActive(!this.expansion1Active);
  }

  private void ToggleExpansion1Cicked() => Util.KInstantiateUI<InfoDialogScreen>(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, ((Component) ((Component) this).GetComponentInParent<Canvas>()).gameObject, true).AddDefaultCancel().SetHeader((string) (this.expansion1Active ? STRINGS.UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1 : STRINGS.UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1)).AddSprite(this.expansion1Active ? GlobalResources.Instance().baseGameLogoSmall : GlobalResources.Instance().expansion1LogoSmall).AddPlainText((string) (this.expansion1Active ? STRINGS.UI.FRONTEND.MAINMENU.DLC.DEACTIVATE_EXPANSION1_DESC : STRINGS.UI.FRONTEND.MAINMENU.DLC.ACTIVATE_EXPANSION1_DESC)).AddOption((string) STRINGS.UI.CONFIRMDIALOG.OK, (Action<InfoDialogScreen>) (screen => DlcManager.ToggleDLC("EXPANSION1_ID")), true);
}
