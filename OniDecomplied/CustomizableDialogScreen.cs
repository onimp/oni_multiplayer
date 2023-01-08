// Decompiled with JetBrains decompiler
// Type: CustomizableDialogScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizableDialogScreen : KModalScreen
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
  private Image image;
  private List<CustomizableDialogScreen.Button> buttons;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this).gameObject.SetActive(false);
    this.buttons = new List<CustomizableDialogScreen.Button>();
  }

  public override bool IsModal() => true;

  public void AddOption(string text, System.Action action)
  {
    GameObject gameObject = Util.KInstantiateUI(this.buttonPrefab, this.buttonPanel, true);
    this.buttons.Add(new CustomizableDialogScreen.Button()
    {
      label = text,
      action = action,
      gameObject = gameObject
    });
  }

  public void PopupConfirmDialog(string text, string title_text = null, Sprite image_sprite = null)
  {
    foreach (CustomizableDialogScreen.Button button in this.buttons)
    {
      ((TMP_Text) button.gameObject.GetComponentInChildren<LocText>()).text = button.label;
      button.gameObject.GetComponent<KButton>().onClick += button.action;
    }
    if (Object.op_Inequality((Object) image_sprite, (Object) null))
    {
      this.image.sprite = image_sprite;
      ((Component) this.image).gameObject.SetActive(true);
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
