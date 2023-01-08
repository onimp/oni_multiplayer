// Decompiled with JetBrains decompiler
// Type: BreakdownListRow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/BreakdownListRow")]
public class BreakdownListRow : KMonoBehaviour
{
  private static Color[] statusColour = new Color[4]
  {
    new Color(0.34117648f, 0.368627459f, 0.458823532f, 1f),
    new Color(0.721568644f, 0.384313732f, 0.0f, 1f),
    new Color(0.384313732f, 0.721568644f, 0.0f, 1f),
    new Color(0.721568644f, 0.721568644f, 0.0f, 1f)
  };
  public Image dotOutlineImage;
  public Image dotInsideImage;
  public Image iconImage;
  public Image checkmarkImage;
  public LocText nameLabel;
  public LocText valueLabel;
  private bool isHighlighted;
  private bool isDisabled;
  private bool isImportant;
  private ToolTip tooltip;
  [SerializeField]
  private Sprite statusSuccessIcon;
  [SerializeField]
  private Sprite statusWarningIcon;
  [SerializeField]
  private Sprite statusFailureIcon;

  public void ShowData(string name, string value)
  {
    ((Component) this).gameObject.transform.localScale = Vector3.one;
    ((TMP_Text) this.nameLabel).text = name;
    ((TMP_Text) this.valueLabel).text = value;
    ((Component) this.dotOutlineImage).gameObject.SetActive(true);
    Vector2 vector2 = Vector2.op_Multiply(Vector2.one, 0.6f);
    Vector3 localScale = ((Transform) ((Graphic) this.dotOutlineImage).rectTransform).localScale;
    ((Vector3) ref localScale).Set(vector2.x, vector2.y, 1f);
    ((Component) this.dotInsideImage).gameObject.SetActive(true);
    ((Graphic) this.dotInsideImage).color = BreakdownListRow.statusColour[0];
    ((Component) this.iconImage).gameObject.SetActive(false);
    ((Component) this.checkmarkImage).gameObject.SetActive(false);
    this.SetHighlighted(false);
    this.SetImportant(false);
  }

  public void ShowStatusData(string name, string value, BreakdownListRow.Status dotColor)
  {
    this.ShowData(name, value);
    ((Component) this.dotOutlineImage).gameObject.SetActive(true);
    ((Component) this.dotInsideImage).gameObject.SetActive(true);
    ((Component) this.iconImage).gameObject.SetActive(false);
    ((Component) this.checkmarkImage).gameObject.SetActive(false);
    this.SetStatusColor(dotColor);
  }

  public void SetStatusColor(BreakdownListRow.Status dotColor)
  {
    ((Component) this.checkmarkImage).gameObject.SetActive(dotColor != 0);
    ((Graphic) this.checkmarkImage).color = BreakdownListRow.statusColour[(int) dotColor];
    switch (dotColor)
    {
      case BreakdownListRow.Status.Red:
        this.checkmarkImage.sprite = this.statusFailureIcon;
        break;
      case BreakdownListRow.Status.Green:
        this.checkmarkImage.sprite = this.statusSuccessIcon;
        break;
      case BreakdownListRow.Status.Yellow:
        this.checkmarkImage.sprite = this.statusWarningIcon;
        break;
    }
  }

  public void ShowCheckmarkData(string name, string value, BreakdownListRow.Status status)
  {
    this.ShowData(name, value);
    ((Component) this.dotOutlineImage).gameObject.SetActive(true);
    ((Transform) ((Graphic) this.dotOutlineImage).rectTransform).localScale = Vector3.one;
    ((Component) this.dotInsideImage).gameObject.SetActive(true);
    ((Component) this.iconImage).gameObject.SetActive(false);
    this.SetStatusColor(status);
  }

  public void ShowIconData(string name, string value, Sprite sprite)
  {
    this.ShowData(name, value);
    ((Component) this.dotOutlineImage).gameObject.SetActive(false);
    ((Component) this.dotInsideImage).gameObject.SetActive(false);
    ((Component) this.iconImage).gameObject.SetActive(true);
    ((Component) this.checkmarkImage).gameObject.SetActive(false);
    this.iconImage.sprite = sprite;
    ((Graphic) this.iconImage).color = Color.white;
  }

  public void ShowIconData(string name, string value, Sprite sprite, Color spriteColor)
  {
    this.ShowIconData(name, value, sprite);
    ((Graphic) this.iconImage).color = spriteColor;
  }

  public void SetHighlighted(bool highlighted)
  {
    this.isHighlighted = highlighted;
    Vector2 vector2 = Vector2.op_Multiply(Vector2.one, 0.8f);
    Vector3 localScale = ((Transform) ((Graphic) this.dotOutlineImage).rectTransform).localScale;
    ((Vector3) ref localScale).Set(vector2.x, vector2.y, 1f);
    ((TMP_Text) this.nameLabel).alpha = this.isHighlighted ? 0.9f : 0.5f;
    ((TMP_Text) this.valueLabel).alpha = this.isHighlighted ? 0.9f : 0.5f;
  }

  public void SetDisabled(bool disabled)
  {
    this.isDisabled = disabled;
    ((TMP_Text) this.nameLabel).alpha = this.isDisabled ? 0.4f : 0.5f;
    ((TMP_Text) this.valueLabel).alpha = this.isDisabled ? 0.4f : 0.5f;
  }

  public void SetImportant(bool important)
  {
    this.isImportant = important;
    ((Transform) ((Graphic) this.dotOutlineImage).rectTransform).localScale = Vector3.one;
    ((TMP_Text) this.nameLabel).alpha = this.isImportant ? 1f : 0.5f;
    ((TMP_Text) this.valueLabel).alpha = this.isImportant ? 1f : 0.5f;
    ((TMP_Text) this.nameLabel).fontStyle = this.isImportant ? (FontStyles) 1 : (FontStyles) 0;
    ((TMP_Text) this.valueLabel).fontStyle = this.isImportant ? (FontStyles) 1 : (FontStyles) 0;
  }

  public void HideIcon()
  {
    ((Component) this.dotOutlineImage).gameObject.SetActive(false);
    ((Component) this.dotInsideImage).gameObject.SetActive(false);
    ((Component) this.iconImage).gameObject.SetActive(false);
    ((Component) this.checkmarkImage).gameObject.SetActive(false);
  }

  public void AddTooltip(string tooltipText)
  {
    if (Object.op_Equality((Object) this.tooltip, (Object) null))
      this.tooltip = ((Component) this).gameObject.AddComponent<ToolTip>();
    this.tooltip.SetSimpleTooltip(tooltipText);
  }

  public void ClearTooltip()
  {
    if (!Object.op_Inequality((Object) this.tooltip, (Object) null))
      return;
    this.tooltip.ClearMultiStringTooltip();
  }

  public void SetValue(string value) => ((TMP_Text) this.valueLabel).text = value;

  public enum Status
  {
    Default,
    Red,
    Green,
    Yellow,
  }
}
