// Decompiled with JetBrains decompiler
// Type: InfoDialogScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDialogScreen : KModalScreen
{
  [SerializeField]
  private InfoScreenPlainText subHeaderTemplate;
  [SerializeField]
  private InfoScreenPlainText plainTextTemplate;
  [SerializeField]
  private InfoScreenLineItem lineItemTemplate;
  [SerializeField]
  private InfoScreenSpriteItem spriteItemTemplate;
  [Space(10f)]
  [SerializeField]
  private LocText header;
  [SerializeField]
  private GameObject contentContainer;
  [SerializeField]
  private GameObject leftButtonPrefab;
  [SerializeField]
  private GameObject rightButtonPrefab;
  [SerializeField]
  private GameObject leftButtonPanel;
  [SerializeField]
  private GameObject rightButtonPanel;
  private bool escapeCloses;
  public System.Action onDeactivateFn;

  public InfoScreenPlainText GetSubHeaderPrefab() => this.subHeaderTemplate;

  public InfoScreenPlainText GetPlainTextPrefab() => this.plainTextTemplate;

  public InfoScreenLineItem GetLineItemPrefab() => this.lineItemTemplate;

  public GameObject GetPrimaryButtonPrefab() => this.leftButtonPrefab;

  public GameObject GetSecondaryButtonPrefab() => this.rightButtonPrefab;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this).gameObject.SetActive(false);
  }

  public override bool IsModal() => true;

  public override void OnKeyDown(KButtonEvent e)
  {
    if (!this.escapeCloses)
      e.TryConsume((Action) 1);
    else if (e.TryConsume((Action) 1))
      this.Deactivate();
    else if (Object.op_Inequality((Object) PlayerController.Instance, (Object) null) && PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
      this.Deactivate();
    else
      base.OnKeyDown(e);
  }

  protected override void OnShow(bool show)
  {
    base.OnShow(show);
    if (show || this.onDeactivateFn == null)
      return;
    this.onDeactivateFn();
  }

  public InfoDialogScreen AddDefaultOK(bool escapeCloses = false)
  {
    this.AddOption((string) STRINGS.UI.CONFIRMDIALOG.OK, (Action<InfoDialogScreen>) (d => d.Deactivate()), true);
    this.escapeCloses = escapeCloses;
    return this;
  }

  public InfoDialogScreen AddDefaultCancel()
  {
    this.AddOption((string) STRINGS.UI.CONFIRMDIALOG.CANCEL, (Action<InfoDialogScreen>) (d => d.Deactivate()));
    this.escapeCloses = true;
    return this;
  }

  public InfoDialogScreen AddOption(string text, Action<InfoDialogScreen> action, bool rightSide = false)
  {
    GameObject gameObject = Util.KInstantiateUI(rightSide ? this.rightButtonPrefab : this.leftButtonPrefab, rightSide ? this.rightButtonPanel : this.leftButtonPanel, true);
    ((TMP_Text) gameObject.gameObject.GetComponentInChildren<LocText>()).text = text;
    gameObject.gameObject.GetComponent<KButton>().onClick += (System.Action) (() => action(this));
    return this;
  }

  public InfoDialogScreen AddOption(bool rightSide, out KButton button, out LocText buttonText)
  {
    GameObject gameObject = Util.KInstantiateUI(rightSide ? this.rightButtonPrefab : this.leftButtonPrefab, rightSide ? this.rightButtonPanel : this.leftButtonPanel, true);
    button = gameObject.GetComponent<KButton>();
    buttonText = gameObject.GetComponentInChildren<LocText>();
    return this;
  }

  public InfoDialogScreen SetHeader(string header)
  {
    ((TMP_Text) this.header).text = header;
    return this;
  }

  public InfoDialogScreen AddSprite(Sprite sprite)
  {
    Util.KInstantiateUI<InfoScreenSpriteItem>(((Component) this.spriteItemTemplate).gameObject, this.contentContainer, false).SetSprite(sprite);
    return this;
  }

  public InfoDialogScreen AddPlainText(string text)
  {
    Util.KInstantiateUI<InfoScreenPlainText>(((Component) this.plainTextTemplate).gameObject, this.contentContainer, false).SetText(text);
    return this;
  }

  public InfoDialogScreen AddLineItem(string text, string tooltip)
  {
    InfoScreenLineItem infoScreenLineItem = Util.KInstantiateUI<InfoScreenLineItem>(((Component) this.lineItemTemplate).gameObject, this.contentContainer, false);
    infoScreenLineItem.SetText(text);
    infoScreenLineItem.SetTooltip(tooltip);
    return this;
  }

  public InfoDialogScreen AddSubHeader(string text)
  {
    Util.KInstantiateUI<InfoScreenPlainText>(((Component) this.subHeaderTemplate).gameObject, this.contentContainer, false).SetText(text);
    return this;
  }

  public InfoDialogScreen AddSpacer(float height)
  {
    GameObject gameObject = new GameObject("spacer");
    gameObject.SetActive(false);
    gameObject.transform.SetParent(this.contentContainer.transform, false);
    LayoutElement layoutElement = gameObject.AddComponent<LayoutElement>();
    layoutElement.minHeight = height;
    layoutElement.preferredHeight = height;
    layoutElement.flexibleHeight = 0.0f;
    gameObject.SetActive(true);
    return this;
  }

  public InfoDialogScreen AddUI<T>(T prefab, out T spawn) where T : MonoBehaviour
  {
    spawn = Util.KInstantiateUI<T>(((Component) (object) prefab).gameObject, this.contentContainer, true);
    return this;
  }

  public InfoDialogScreen AddDescriptors(List<Descriptor> descriptors)
  {
    for (int index = 0; index < descriptors.Count; ++index)
    {
      Descriptor descriptor = descriptors[index];
      this.AddLineItem(((Descriptor) ref descriptor).IndentedText(), descriptors[index].tooltipText);
    }
    return this;
  }
}
