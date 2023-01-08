// Decompiled with JetBrains decompiler
// Type: FilterSideScreenRow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/FilterSideScreenRow")]
public class FilterSideScreenRow : KMonoBehaviour
{
  [SerializeField]
  private LocText labelText;
  [SerializeField]
  private Image BG;
  [SerializeField]
  private Image outline;
  [SerializeField]
  private Color outlineHighLightColor = Color32.op_Implicit(new Color32((byte) 168, (byte) 74, (byte) 121, byte.MaxValue));
  [SerializeField]
  private Color BGHighLightColor = Color32.op_Implicit(new Color32((byte) 168, (byte) 74, (byte) 121, (byte) 80));
  [SerializeField]
  private Color outlineDefaultColor = Color32.op_Implicit(new Color32((byte) 204, (byte) 204, (byte) 204, byte.MaxValue));
  private Color regularColor = Color.white;
  [SerializeField]
  public KButton button;

  public Tag tag { get; private set; }

  public bool isSelected { get; private set; }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.regularColor = ((Graphic) this.outline).color;
    if (!Object.op_Inequality((Object) this.button, (Object) null))
      return;
    this.button.onPointerEnter += (System.Action) (() =>
    {
      if (this.isSelected)
        return;
      ((Graphic) this.outline).color = this.outlineHighLightColor;
    });
    this.button.onPointerExit += (System.Action) (() =>
    {
      if (this.isSelected)
        return;
      ((Graphic) this.outline).color = this.regularColor;
    });
  }

  public void SetTag(Tag tag)
  {
    this.tag = tag;
    this.SetText(Tag.op_Equality(tag, GameTags.Void) ? STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION.text : tag.ProperName());
  }

  private void SetText(string assignmentStr) => ((TMP_Text) this.labelText).text = !string.IsNullOrEmpty(assignmentStr) ? assignmentStr : "-";

  public void SetSelected(bool selected)
  {
    this.isSelected = selected;
    ((Graphic) this.outline).color = selected ? this.outlineHighLightColor : this.outlineDefaultColor;
    ((Graphic) this.BG).color = selected ? this.BGHighLightColor : Color.white;
  }
}
