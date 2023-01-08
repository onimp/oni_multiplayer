// Decompiled with JetBrains decompiler
// Type: MessageDialogFrame
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;

public class MessageDialogFrame : KScreen
{
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KToggle nextMessageButton;
  [SerializeField]
  private GameObject dontShowAgainElement;
  [SerializeField]
  private MultiToggle dontShowAgainButton;
  [SerializeField]
  private LocText title;
  [SerializeField]
  private RectTransform body;
  private System.Action dontShowAgainDelegate;

  public virtual float GetSortKey() => 15f;

  protected virtual void OnActivate()
  {
    this.closeButton.onClick += new System.Action(this.OnClickClose);
    this.nextMessageButton.onClick += new System.Action(this.OnClickNextMessage);
    this.dontShowAgainButton.onClick += new System.Action(this.OnClickDontShowAgain);
    this.dontShowAgainButton.ChangeState(KPlayerPrefs.GetInt("HideTutorial_CheckState", 0) == 1 ? 0 : 1);
    ((KMonoBehaviour) this).Subscribe(((Component) Messenger.Instance).gameObject, -599791736, new Action<object>(this.OnMessagesChanged));
    this.OnMessagesChanged((object) null);
  }

  protected virtual void OnDeactivate() => ((KMonoBehaviour) this).Unsubscribe(((Component) Messenger.Instance).gameObject, -599791736, new Action<object>(this.OnMessagesChanged));

  private void OnClickClose()
  {
    this.TryDontShowAgain();
    Object.Destroy((Object) ((Component) this).gameObject);
  }

  private void OnClickNextMessage()
  {
    this.TryDontShowAgain();
    Object.Destroy((Object) ((Component) this).gameObject);
    NotificationScreen.Instance.OnClickNextMessage();
  }

  private void OnClickDontShowAgain()
  {
    this.dontShowAgainButton.NextState();
    KPlayerPrefs.SetInt("HideTutorial_CheckState", this.dontShowAgainButton.CurrentState == 0 ? 1 : 0);
  }

  private void OnMessagesChanged(object data) => ((Component) this.nextMessageButton).gameObject.SetActive(Messenger.Instance.Count != 0);

  public void SetMessage(MessageDialog dialog, Message message)
  {
    ((TMP_Text) this.title).text = message.GetTitle().ToUpper();
    ((Transform) ((Component) dialog).GetComponent<RectTransform>()).SetParent((Transform) ((Component) this.body).GetComponent<RectTransform>());
    RectTransform component = ((Component) dialog).GetComponent<RectTransform>();
    component.offsetMin = Vector2.zero;
    component.offsetMax = Vector2.zero;
    TransformExtensions.SetLocalPosition(dialog.transform, Vector3.zero);
    dialog.SetMessage(message);
    dialog.OnClickAction();
    if (dialog.CanDontShowAgain)
    {
      this.dontShowAgainElement.SetActive(true);
      this.dontShowAgainDelegate = new System.Action(dialog.OnDontShowAgain);
    }
    else
    {
      this.dontShowAgainElement.SetActive(false);
      this.dontShowAgainDelegate = (System.Action) null;
    }
  }

  private void TryDontShowAgain()
  {
    if (this.dontShowAgainDelegate == null || this.dontShowAgainButton.CurrentState != 0)
      return;
    this.dontShowAgainDelegate();
  }
}
