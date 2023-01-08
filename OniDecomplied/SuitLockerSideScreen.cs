// Decompiled with JetBrains decompiler
// Type: SuitLockerSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TMPro;
using UnityEngine;

public class SuitLockerSideScreen : SideScreenContent
{
  [SerializeField]
  private GameObject initialConfigScreen;
  [SerializeField]
  private GameObject regularConfigScreen;
  [SerializeField]
  private LocText initialConfigLabel;
  [SerializeField]
  private KButton initialConfigRequestSuitButton;
  [SerializeField]
  private KButton initialConfigNoSuitButton;
  [SerializeField]
  private LocText regularConfigLabel;
  [SerializeField]
  private KButton regularConfigRequestSuitButton;
  [SerializeField]
  private KButton regularConfigDropSuitButton;
  private SuitLocker suitLocker;

  protected virtual void OnSpawn() => base.OnSpawn();

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<SuitLocker>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    this.suitLocker = target.GetComponent<SuitLocker>();
    ((Component) this.initialConfigRequestSuitButton).GetComponentInChildren<ToolTip>().SetSimpleTooltip((string) UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_REQUEST_SUIT_TOOLTIP);
    ((Component) this.initialConfigNoSuitButton).GetComponentInChildren<ToolTip>().SetSimpleTooltip((string) UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_NO_SUIT_TOOLTIP);
    this.initialConfigRequestSuitButton.ClearOnClick();
    this.initialConfigRequestSuitButton.onClick += (System.Action) (() => this.suitLocker.ConfigRequestSuit());
    this.initialConfigNoSuitButton.ClearOnClick();
    this.initialConfigNoSuitButton.onClick += (System.Action) (() => this.suitLocker.ConfigNoSuit());
    this.regularConfigRequestSuitButton.ClearOnClick();
    this.regularConfigRequestSuitButton.onClick += (System.Action) (() =>
    {
      if (this.suitLocker.smi.sm.isWaitingForSuit.Get(this.suitLocker.smi))
        this.suitLocker.ConfigNoSuit();
      else
        this.suitLocker.ConfigRequestSuit();
    });
    this.regularConfigDropSuitButton.ClearOnClick();
    this.regularConfigDropSuitButton.onClick += (System.Action) (() => this.suitLocker.DropSuit());
  }

  private void Update()
  {
    bool flag1 = this.suitLocker.smi.sm.isConfigured.Get(this.suitLocker.smi);
    this.initialConfigScreen.gameObject.SetActive(!flag1);
    this.regularConfigScreen.gameObject.SetActive(flag1);
    bool flag2 = Object.op_Inequality((Object) this.suitLocker.GetStoredOutfit(), (Object) null);
    int num = this.suitLocker.smi.sm.isWaitingForSuit.Get(this.suitLocker.smi) ? 1 : 0;
    this.regularConfigRequestSuitButton.isInteractable = !flag2;
    if (num == 0)
    {
      ((TMP_Text) ((Component) this.regularConfigRequestSuitButton).GetComponentInChildren<LocText>()).text = (string) UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_REQUEST_SUIT;
      ((Component) this.regularConfigRequestSuitButton).GetComponentInChildren<ToolTip>().SetSimpleTooltip((string) UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_REQUEST_SUIT_TOOLTIP);
    }
    else
    {
      ((TMP_Text) ((Component) this.regularConfigRequestSuitButton).GetComponentInChildren<LocText>()).text = (string) UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_CANCEL_REQUEST;
      ((Component) this.regularConfigRequestSuitButton).GetComponentInChildren<ToolTip>().SetSimpleTooltip((string) UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_CANCEL_REQUEST_TOOLTIP);
    }
    if (flag2)
    {
      this.regularConfigDropSuitButton.isInteractable = true;
      ((Component) this.regularConfigDropSuitButton).GetComponentInChildren<ToolTip>().SetSimpleTooltip((string) UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_DROP_SUIT_TOOLTIP);
    }
    else
    {
      this.regularConfigDropSuitButton.isInteractable = false;
      ((Component) this.regularConfigDropSuitButton).GetComponentInChildren<ToolTip>().SetSimpleTooltip((string) UI.UISIDESCREENS.SUIT_SIDE_SCREEN.CONFIG_DROP_SUIT_NO_SUIT_TOOLTIP);
    }
    KSelectable component = ((Component) this.suitLocker).GetComponent<KSelectable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    StatusItemGroup.Entry statusItem = component.GetStatusItem(Db.Get().StatusItemCategories.Main);
    if (statusItem.item == null)
      return;
    ((TMP_Text) this.regularConfigLabel).text = statusItem.item.GetName(statusItem.data);
    ((Component) this.regularConfigLabel).GetComponentInChildren<ToolTip>().SetSimpleTooltip(statusItem.item.GetTooltip(statusItem.data));
  }
}
