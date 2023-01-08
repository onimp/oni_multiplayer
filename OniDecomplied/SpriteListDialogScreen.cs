// Decompiled with JetBrains decompiler
// Type: SpriteListDialogScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpriteListDialogScreen : KModalScreen
{
  public System.Action onDeactivateCB;
  [SerializeField]
  private GameObject buttonPrefab;
  [SerializeField]
  private GameObject buttonPanel;
  [SerializeField]
  private LocText titleText;
  [SerializeField]
  private LocText popupMessage;
  [SerializeField]
  private GameObject listPanel;
  [SerializeField]
  private GameObject listPrefab;
  private List<SpriteListDialogScreen.Button> buttons;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this).gameObject.SetActive(false);
    this.buttons = new List<SpriteListDialogScreen.Button>();
  }

  public override bool IsModal() => true;

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1))
      this.Deactivate();
    else
      base.OnKeyDown(e);
  }

  public void AddOption(string text, System.Action action)
  {
    GameObject gameObject = Util.KInstantiateUI(this.buttonPrefab, this.buttonPanel, true);
    this.buttons.Add(new SpriteListDialogScreen.Button()
    {
      label = text,
      action = action,
      gameObject = gameObject
    });
  }

  public void AddSprite(Sprite sprite, string text, float width = -1f, float height = -1f)
  {
    GameObject gameObject = Util.KInstantiateUI(this.listPrefab, this.listPanel, true);
    ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).text = text;
    Image componentInChildren = gameObject.GetComponentInChildren<Image>();
    componentInChildren.sprite = sprite;
    if ((double) width >= 0.0 || (double) height >= 0.0)
    {
      ((Behaviour) ((Component) componentInChildren).GetComponent<AspectRatioFitter>()).enabled = false;
      LayoutElement component = ((Component) componentInChildren).GetComponent<LayoutElement>();
      component.minWidth = width;
      component.preferredWidth = width;
      component.minHeight = height;
      component.preferredHeight = height;
    }
    else
    {
      AspectRatioFitter component = ((Component) componentInChildren).GetComponent<AspectRatioFitter>();
      Rect rect = sprite.rect;
      double width1 = (double) ((Rect) ref rect).width;
      rect = sprite.rect;
      double height1 = (double) ((Rect) ref rect).height;
      double num = width1 / height1;
      component.aspectRatio = (float) num;
    }
  }

  public void PopupConfirmDialog(string text, string title_text = null)
  {
    foreach (SpriteListDialogScreen.Button button in this.buttons)
    {
      ((TMP_Text) button.gameObject.GetComponentInChildren<LocText>()).text = button.label;
      button.gameObject.GetComponent<KButton>().onClick += button.action;
    }
    if (title_text != null)
      ((TMP_Text) this.titleText).text = title_text;
    ((TMP_Text) this.popupMessage).text = text;
  }

  protected override void OnDeactivate()
  {
    if (this.onDeactivateCB != null)
      this.onDeactivateCB();
    base.OnDeactivate();
  }

  private struct Button
  {
    public System.Action action;
    public GameObject gameObject;
    public string label;
  }
}
