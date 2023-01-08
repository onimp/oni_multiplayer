// Decompiled with JetBrains decompiler
// Type: KleiItemDropScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class KleiItemDropScreen : KModalScreen
{
  [SerializeField]
  private RectTransform shieldMaskRect;
  [SerializeField]
  private KButton closeButton;
  [Header("Animated Item")]
  [SerializeField]
  private KleiItemDropScreen_PermitVis permitVisualizer;
  [SerializeField]
  private KBatchedAnimController animatedPod;
  [Header("Item Info")]
  [SerializeField]
  private LocText itemNameLabel;
  [SerializeField]
  private LocText itemDescriptionLabel;
  [Header("Accept Button")]
  [SerializeField]
  private RectTransform acceptButtonRect;
  [SerializeField]
  private KButton acceptButton;
  [SerializeField]
  private KButton acknowledgeButton;
  private Coroutine activePresentationRoutine;
  private bool giftRevealed;
  private bool giftAcknowledged;
  public static KleiItemDropScreen Instance;
  private bool shouldDoCloseRoutine;
  private const float TEXT_AND_BUTTON_ANIMATE_OFFSET_Y = -30f;
  private PrefabDefinedUIPosition acceptButtonPosition = new PrefabDefinedUIPosition();
  private PrefabDefinedUIPosition itemNameLabelPosition = new PrefabDefinedUIPosition();
  private PrefabDefinedUIPosition itemDescriptionLabelPosition = new PrefabDefinedUIPosition();

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    KleiItemDropScreen.Instance = this;
    this.closeButton.onClick += (System.Action) (() => base.Show(false));
    if (!string.IsNullOrEmpty(KleiAccount.KleiToken))
      return;
    base.Show(false);
  }

  protected override void OnActivate()
  {
    KleiItemDropScreen.Instance = this;
    base.Show(false);
  }

  public virtual void Show(bool show = true)
  {
    if (!show)
    {
      if (this.activePresentationRoutine != null)
        ((MonoBehaviour) this).StopCoroutine(this.activePresentationRoutine);
      if (this.shouldDoCloseRoutine)
      {
        ((Component) this.closeButton).gameObject.SetActive(false);
        Updater.RunRoutine((MonoBehaviour) this, this.AnimateScreenOutRoutine()).Then((System.Action) (() => base.Show(false)));
        this.shouldDoCloseRoutine = false;
      }
      else
        base.Show(false);
    }
    else
      base.Show(true);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      base.Show(false);
    base.OnKeyDown(e);
  }

  protected override void OnShow(bool show)
  {
    base.OnShow(show);
    if (!show)
      return;
    if (KleiItems.InventoryData.AllItems != null)
    {
      this.PresentNextUnopenedItem();
      this.shouldDoCloseRoutine = true;
    }
    else
      base.Show(false);
  }

  public void PresentNextUnopenedItem(bool firstItemPresentation = true)
  {
    foreach (KleiItems.Item allItem in KleiItems.InventoryData.AllItems)
    {
      if (!allItem.IsOpened)
      {
        this.PresentItem(allItem, firstItemPresentation);
        return;
      }
    }
    base.Show(false);
  }

  public void PresentItem(KleiItems.Item item, bool firstItemPresentation)
  {
    this.giftRevealed = false;
    this.giftAcknowledged = false;
    this.activePresentationRoutine = ((MonoBehaviour) this).StartCoroutine(this.PresentItemRoutine(item, firstItemPresentation));
    this.acceptButton.ClearOnClick();
    this.acknowledgeButton.ClearOnClick();
    this.acceptButton.onClick += (System.Action) (() => this.giftRevealed = true);
    this.acknowledgeButton.onClick += (System.Action) (() =>
    {
      if (!this.giftRevealed)
        return;
      this.giftAcknowledged = true;
    });
  }

  private IEnumerator AnimateScreenInRoutine()
  {
    KleiItemDropScreen kleiItemDropScreen = this;
    KFMOD.PlayUISound(GlobalAssets.GetSound("GiftItemDrop_Screen_Open"));
    // ISSUE: reference to a compiler-generated method
    yield return (object) Updater.Ease(new Action<Vector2>(kleiItemDropScreen.\u003CAnimateScreenInRoutine\u003Eb__21_0), kleiItemDropScreen.shieldMaskRect.sizeDelta, new Vector2(kleiItemDropScreen.shieldMaskRect.sizeDelta.x, 720f), 0.5f, Easing.CircInOut);
    // ISSUE: reference to a compiler-generated method
    yield return (object) Updater.Ease(new Action<Vector2>(kleiItemDropScreen.\u003CAnimateScreenInRoutine\u003Eb__21_1), kleiItemDropScreen.shieldMaskRect.sizeDelta, new Vector2(1152f, kleiItemDropScreen.shieldMaskRect.sizeDelta.y), 0.25f, Easing.CircInOut);
  }

  private IEnumerator AnimateScreenOutRoutine()
  {
    KleiItemDropScreen kleiItemDropScreen = this;
    KFMOD.PlayUISound(GlobalAssets.GetSound("GiftItemDrop_Screen_Close"));
    // ISSUE: reference to a compiler-generated method
    yield return (object) Updater.Ease(new Action<Vector2>(kleiItemDropScreen.\u003CAnimateScreenOutRoutine\u003Eb__22_0), kleiItemDropScreen.shieldMaskRect.sizeDelta, new Vector2(8f, kleiItemDropScreen.shieldMaskRect.sizeDelta.y), 0.25f, Easing.CircInOut);
    // ISSUE: reference to a compiler-generated method
    yield return (object) Updater.Ease(new Action<Vector2>(kleiItemDropScreen.\u003CAnimateScreenOutRoutine\u003Eb__22_1), kleiItemDropScreen.shieldMaskRect.sizeDelta, new Vector2(kleiItemDropScreen.shieldMaskRect.sizeDelta.x, 0.0f), 0.25f, Easing.CircInOut);
  }

  private IEnumerator PresentItemRoutine(KleiItems.Item item, bool firstItem)
  {
    KleiItemDropScreen kleiItemDropScreen = this;
    yield return (object) null;
    if (item.ItemId == 0UL)
    {
      Debug.LogError((object) "Could not find dropped item inventory.");
    }
    else
    {
      ((TMP_Text) kleiItemDropScreen.itemNameLabel).SetText("");
      ((TMP_Text) kleiItemDropScreen.itemDescriptionLabel).SetText("");
      kleiItemDropScreen.permitVisualizer.ResetState();
      if (firstItem)
      {
        kleiItemDropScreen.animatedPod.Play(HashedString.op_Implicit("idle"), (KAnim.PlayMode) 0);
        ((Component) kleiItemDropScreen.acceptButtonRect).gameObject.SetActive(false);
        kleiItemDropScreen.shieldMaskRect.sizeDelta = new Vector2(8f, 0.0f);
        ((Component) kleiItemDropScreen.shieldMaskRect).gameObject.SetActive(true);
      }
      if (firstItem)
      {
        ((Component) kleiItemDropScreen.closeButton).gameObject.SetActive(false);
        yield return (object) Updater.WaitForSeconds(0.5f);
        yield return (object) kleiItemDropScreen.AnimateScreenInRoutine();
        yield return (object) Updater.WaitForSeconds(0.125f);
        ((Component) kleiItemDropScreen.closeButton).gameObject.SetActive(true);
      }
      else
        yield return (object) Updater.WaitForSeconds(0.25f);
      Vector2 animate_offset = new Vector2(0.0f, -30f);
      Util.FindOrAddComponent<CanvasGroup>((Component) kleiItemDropScreen.acceptButtonRect).alpha = 0.0f;
      ((Component) kleiItemDropScreen.acceptButtonRect).gameObject.SetActive(true);
      kleiItemDropScreen.acceptButtonPosition.SetOn((Component) kleiItemDropScreen.acceptButtonRect);
      kleiItemDropScreen.animatedPod.Play(HashedString.op_Implicit("powerup"));
      kleiItemDropScreen.animatedPod.Queue(HashedString.op_Implicit("working_loop"), (KAnim.PlayMode) 0);
      yield return (object) Updater.WaitForSeconds(1.25f);
      yield return (object) PresUtil.OffsetToAndFade(Util.rectTransform((Component) kleiItemDropScreen.acceptButton), animate_offset, 1f, 0.125f, Easing.ExpoOut);
      // ISSUE: reference to a compiler-generated method
      yield return (object) Updater.Until(new Func<bool>(kleiItemDropScreen.\u003CPresentItemRoutine\u003Eb__27_0));
      yield return (object) PresUtil.OffsetFromAndFade(Util.rectTransform((Component) kleiItemDropScreen.acceptButton), animate_offset, 0.0f, 0.125f, Easing.SmoothStep);
      kleiItemDropScreen.animatedPod.Play(HashedString.op_Implicit("additional_pre"));
      kleiItemDropScreen.animatedPod.Queue(HashedString.op_Implicit("working_loop"), (KAnim.PlayMode) 0);
      yield return (object) Updater.WaitForSeconds(1f);
      PermitResource permit = Db.Get().Permits.Get(PermitItems.GetPermitIDByKleiItemType(item.ItemType));
      PermitPresentationInfo permitPresInfo = PermitItems.GetPermitPresentationInfo(permit.Id);
      kleiItemDropScreen.permitVisualizer.ConfigureWith(permit);
      yield return (object) kleiItemDropScreen.permitVisualizer.AnimateIn();
      ((TMP_Text) kleiItemDropScreen.itemNameLabel).SetText(permitPresInfo.name);
      ((TMP_Text) kleiItemDropScreen.itemDescriptionLabel).SetText(permitPresInfo.description);
      kleiItemDropScreen.itemNameLabelPosition.SetOn((Component) kleiItemDropScreen.itemNameLabel);
      kleiItemDropScreen.itemDescriptionLabelPosition.SetOn((Component) kleiItemDropScreen.itemDescriptionLabel);
      yield return (object) Updater.Parallel((Updater) PresUtil.OffsetToAndFade(Util.rectTransform((Component) kleiItemDropScreen.itemNameLabel), animate_offset, 1f, 0.125f, Easing.CircInOut), (Updater) PresUtil.OffsetToAndFade(Util.rectTransform((Component) kleiItemDropScreen.itemDescriptionLabel), animate_offset, 1f, 0.125f, Easing.CircInOut));
      // ISSUE: reference to a compiler-generated method
      yield return (object) Updater.Until(new Func<bool>(kleiItemDropScreen.\u003CPresentItemRoutine\u003Eb__27_1));
      kleiItemDropScreen.animatedPod.Play(HashedString.op_Implicit("working_pst"));
      kleiItemDropScreen.animatedPod.Queue(HashedString.op_Implicit("idle"), (KAnim.PlayMode) 0);
      yield return (object) Updater.Parallel((Updater) PresUtil.OffsetFromAndFade(Util.rectTransform((Component) kleiItemDropScreen.itemNameLabel), animate_offset, 0.0f, 0.125f, Easing.CircInOut), (Updater) PresUtil.OffsetFromAndFade(Util.rectTransform((Component) kleiItemDropScreen.itemDescriptionLabel), animate_offset, 0.0f, 0.125f, Easing.CircInOut));
      ((TMP_Text) kleiItemDropScreen.itemNameLabel).SetText("");
      ((TMP_Text) kleiItemDropScreen.itemDescriptionLabel).SetText("");
      yield return (object) kleiItemDropScreen.permitVisualizer.AnimateOut();
      permitPresInfo = new PermitPresentationInfo();
      kleiItemDropScreen.PresentNextUnopenedItem(false);
    }
  }
}
