// Decompiled with JetBrains decompiler
// Type: LockerNavigator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockerNavigator : KModalScreen
{
  public static LockerNavigator Instance;
  [SerializeField]
  private RectTransform slot;
  [SerializeField]
  private KButton backButton;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  public GameObject kleiInventoryScreen;
  [SerializeField]
  public GameObject duplicantCatalogueScreen;
  [SerializeField]
  public GameObject outfitDesignerScreen;
  [SerializeField]
  public GameObject outfitBrowserScreen;
  private List<GameObject> navigationHistory = new List<GameObject>();
  private Dictionary<string, GameObject> screens = new Dictionary<string, GameObject>();
  public List<Func<bool>> preventScreenPop = new List<Func<bool>>();

  public GameObject ContentSlot => ((Component) this.slot).gameObject;

  protected override void OnActivate()
  {
    LockerNavigator.Instance = this;
    base.Show(false);
    this.backButton.onClick += new System.Action(this.OnClickBack);
  }

  public override float GetSortKey() => 41f;

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      this.PopScreen();
    base.OnKeyDown(e);
  }

  public virtual void Show(bool show = true)
  {
    base.Show(show);
    if (!show && this.navigationHistory.Count > 0)
    {
      this.navigationHistory[this.navigationHistory.Count - 1].SetActive(false);
      this.navigationHistory.Clear();
    }
    StreamedTextures.SetBundlesLoaded(show);
  }

  private void OnClickBack() => this.PopScreen();

  public void PushScreen(GameObject screen)
  {
    if (Object.op_Equality((Object) screen, (Object) null))
      return;
    if (this.navigationHistory.Count == 0)
    {
      base.Show(true);
      if (KPrivacyPrefs.instance.disableDataCollection)
        LockerNavigator.MakeDataCollectionWarningPopup(((Component) ((Component) this).gameObject.transform.parent).gameObject);
    }
    if (this.navigationHistory.Count > 0 && Object.op_Equality((Object) screen, (Object) this.navigationHistory[this.navigationHistory.Count - 1]))
      return;
    if (this.navigationHistory.Count > 0)
      this.navigationHistory[this.navigationHistory.Count - 1].SetActive(false);
    this.navigationHistory.Add(screen);
    this.navigationHistory[this.navigationHistory.Count - 1].SetActive(true);
    if (!((Component) this).gameObject.activeSelf)
      ((Component) this).gameObject.SetActive(true);
    this.RefreshButtons();
  }

  public bool PopScreen()
  {
    while (this.preventScreenPop.Count > 0)
    {
      int index = this.preventScreenPop.Count - 1;
      Func<bool> func = this.preventScreenPop[index];
      this.preventScreenPop.RemoveAt(index);
      if (func())
        return true;
    }
    this.navigationHistory[this.navigationHistory.Count - 1].SetActive(false);
    this.navigationHistory.RemoveAt(this.navigationHistory.Count - 1);
    if (this.navigationHistory.Count > 0)
    {
      this.navigationHistory[this.navigationHistory.Count - 1].SetActive(true);
      this.RefreshButtons();
      return true;
    }
    base.Show(false);
    return false;
  }

  public void PopAllScreens()
  {
    int num = 0;
    while (this.PopScreen())
    {
      if (num > 100)
      {
        DebugUtil.DevAssert(false, string.Format("Can't close all LockerNavigator screens, hit limit of trying to close {0} screens", (object) 100), (Object) null);
        break;
      }
      ++num;
    }
  }

  private void RefreshButtons() => this.backButton.isInteractable = true;

  public void ShowDialogPopup(Action<InfoDialogScreen> configureDialogFn)
  {
    InfoDialogScreen dialog = Util.KInstantiateUI<InfoDialogScreen>(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, this.ContentSlot, false);
    configureDialogFn(dialog);
    dialog.Activate();
    ((Component) dialog).gameObject.AddOrGet<LayoutElement>().ignoreLayout = true;
    ((Component) dialog).gameObject.AddOrGet<RectTransform>().Fill();
    Func<bool> preventScreenPopFn = (Func<bool>) (() =>
    {
      dialog.Deactivate();
      return true;
    });
    this.preventScreenPop.Add(preventScreenPopFn);
    dialog.onDeactivateFn += (System.Action) (() => this.preventScreenPop.Remove(preventScreenPopFn));
  }

  public static void MakeDataCollectionWarningPopup(GameObject fullscreenParent) => LockerNavigator.Instance.ShowDialogPopup((Action<InfoDialogScreen>) (dialog => dialog.SetHeader((string) STRINGS.UI.LOCKER_NAVIGATOR.DATA_COLLECTION_WARNING_POPUP.HEADER).AddPlainText((string) STRINGS.UI.LOCKER_NAVIGATOR.DATA_COLLECTION_WARNING_POPUP.BODY).AddOption((string) STRINGS.UI.LOCKER_NAVIGATOR.DATA_COLLECTION_WARNING_POPUP.BUTTON_OK, (Action<InfoDialogScreen>) (d => d.Deactivate()), true).AddOption((string) STRINGS.UI.LOCKER_NAVIGATOR.DATA_COLLECTION_WARNING_POPUP.BUTTON_OPEN_SETTINGS, (Action<InfoDialogScreen>) (d =>
  {
    d.Deactivate();
    LockerNavigator.Instance.PopAllScreens();
    ((KScreen) LockerMenuScreen.Instance).Show(false);
    Util.KInstantiateUI<OptionsMenuScreen>(((Component) ScreenPrefabs.Instance.OptionsScreen).gameObject, fullscreenParent, true).ShowMetricsScreen();
  }))));
}
