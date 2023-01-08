// Decompiled with JetBrains decompiler
// Type: LockerMenuScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LockerMenuScreen : KModalScreen
{
  public static LockerMenuScreen Instance;
  [SerializeField]
  private MultiToggle buttonInventory;
  [SerializeField]
  private MultiToggle buttonDuplicants;
  [SerializeField]
  private MultiToggle buttonOutfitBroswer;
  [SerializeField]
  private MultiToggle buttonClaimItems;
  [SerializeField]
  private LocText descriptionArea;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private GameObject dropsAvailableNotification;
  private const string REDEEM_MYSTERY_BOX_URL = "https://accounts.klei.com/account/rewards?game=ONI";
  private static bool shouldPreventDisplayingDropsAvailableNotification;

  protected override void OnActivate()
  {
    LockerMenuScreen.Instance = this;
    base.Show(false);
  }

  public override float GetSortKey() => 40f;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.buttonInventory.onClick += (System.Action) (() => LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.kleiInventoryScreen));
    this.buttonDuplicants.onClick += (System.Action) (() => LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.duplicantCatalogueScreen));
    this.buttonOutfitBroswer.onClick += (System.Action) (() => OutfitBrowserScreenConfig.Mannequin().ApplyAndOpenScreen());
    this.buttonClaimItems.onClick += (System.Action) (() =>
    {
      KleiItemsStatusRefresher.Active = true;
      Application.OpenURL("https://accounts.klei.com/account/rewards?game=ONI");
      LockerMenuScreen.shouldPreventDisplayingDropsAvailableNotification = true;
      this.dropsAvailableNotification.SetActive(false);
    });
    this.closeButton.onClick += (System.Action) (() =>
    {
      base.Show(false);
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot);
    });
    Color defaultColor = new Color(0.309803933f, 0.34117648f, 0.384313732f, 1f);
    Color hoverColor = new Color(0.7019608f, 0.3647059f, 0.533333361f, 1f);
    ConfigureHoverFor(this.buttonInventory, (string) STRINGS.UI.LOCKER_MENU.BUTTON_INVENTORY_DESCRIPTION);
    ConfigureHoverFor(this.buttonDuplicants, (string) STRINGS.UI.LOCKER_MENU.BUTTON_DUPLICANTS_DESCRIPTION);
    ConfigureHoverFor(this.buttonOutfitBroswer, (string) STRINGS.UI.LOCKER_MENU.BUTTON_OUTFITS_DESCRIPTION);
    ConfigureHoverFor(this.buttonClaimItems, (string) STRINGS.UI.LOCKER_MENU.BUTTON_CLAIM_DESCRIPTION);
    ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.LOCKER_MENU.DEFAULT_DESCRIPTION;

    void ConfigureHoverFor(MultiToggle toggle, string desc)
    {
      toggle.onEnter += OnHoverEnterFn(toggle, desc);
      toggle.onExit += OnHoverExitFn(toggle);
    }

    System.Action OnHoverEnterFn(MultiToggle toggle, string desc)
    {
      Image headerBackground = ((Component) ((Component) toggle).GetComponent<HierarchyReferences>().GetReference<RectTransform>("HeaderBackground")).GetComponent<Image>();
      return (System.Action) (() =>
      {
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
        ((Graphic) headerBackground).color = hoverColor;
        ((TMP_Text) this.descriptionArea).text = desc;
      });
    }

    System.Action OnHoverExitFn(MultiToggle toggle)
    {
      Image headerBackground = ((Component) ((Component) toggle).GetComponent<HierarchyReferences>().GetReference<RectTransform>("HeaderBackground")).GetComponent<Image>();
      return (System.Action) (() =>
      {
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
        ((Graphic) headerBackground).color = defaultColor;
        ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.LOCKER_MENU.DEFAULT_DESCRIPTION;
      });
    }
  }

  public virtual void Show(bool show = true)
  {
    base.Show(show);
    if (show)
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot);
    if (LockerMenuScreen.shouldPreventDisplayingDropsAvailableNotification)
      this.dropsAvailableNotification.SetActive(false);
    else
      this.dropsAvailableNotification.SetActive(this.AreAllOwnablePermitsLocked());
  }

  private bool AreAllOwnablePermitsLocked()
  {
    foreach (PermitResource resource in Db.Get().Permits.resources)
    {
      if (resource.IsOwnable() && resource.IsUnlocked())
        return false;
    }
    return true;
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
    {
      base.Show(false);
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot);
    }
    base.OnKeyDown(e);
  }
}
