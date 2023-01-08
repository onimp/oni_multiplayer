// Decompiled with JetBrains decompiler
// Type: PixelPackSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PixelPackSideScreen : SideScreenContent
{
  public PixelPack targetPixelPack;
  public KButton copyActiveToStandbyButton;
  public KButton copyStandbyToActiveButton;
  public KButton swapColorsButton;
  public GameObject colorSwatchContainer;
  public GameObject swatchEntry;
  public GameObject activeColorsContainer;
  public GameObject standbyColorsContainer;
  public List<GameObject> activeColors;
  public List<GameObject> standbyColors;
  public Color paintingColor;
  public GameObject selectedSwatchEntry;
  private Dictionary<Color, GameObject> swatch_object_by_color;
  private List<GameObject> highlightedSwatchGameObjects;
  private List<Color> colorSwatch;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.swatch_object_by_color.Count == 0)
      this.InitializeColorSwatch();
    this.PopulateColorSelections();
    this.copyActiveToStandbyButton.onClick += new System.Action(this.CopyActiveToStandby);
    this.copyStandbyToActiveButton.onClick += new System.Action(this.CopyStandbyToActive);
    this.swapColorsButton.onClick += new System.Action(this.SwapColors);
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<PixelPack>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.targetPixelPack = target.GetComponent<PixelPack>();
    this.PopulateColorSelections();
    this.HighlightUsedColors();
  }

  private void HighlightUsedColors()
  {
    if (this.swatch_object_by_color.Count == 0)
      this.InitializeColorSwatch();
    for (int index = 0; index < this.highlightedSwatchGameObjects.Count; ++index)
      ((Component) this.highlightedSwatchGameObjects[index].GetComponent<HierarchyReferences>().GetReference("used").GetComponentInChildren<Image>()).gameObject.SetActive(false);
    this.highlightedSwatchGameObjects.Clear();
    for (int index = 0; index < this.targetPixelPack.colorSettings.Count; ++index)
    {
      this.swatch_object_by_color[this.targetPixelPack.colorSettings[index].activeColor].GetComponent<HierarchyReferences>().GetReference("used").gameObject.SetActive(true);
      this.swatch_object_by_color[this.targetPixelPack.colorSettings[index].standbyColor].GetComponent<HierarchyReferences>().GetReference("used").gameObject.SetActive(true);
      this.highlightedSwatchGameObjects.Add(this.swatch_object_by_color[this.targetPixelPack.colorSettings[index].activeColor]);
      this.highlightedSwatchGameObjects.Add(this.swatch_object_by_color[this.targetPixelPack.colorSettings[index].standbyColor]);
    }
  }

  private void PopulateColorSelections()
  {
    for (int index = 0; index < this.targetPixelPack.colorSettings.Count; ++index)
    {
      int current_id = index;
      ((Graphic) this.activeColors[index].GetComponent<Image>()).color = this.targetPixelPack.colorSettings[index].activeColor;
      this.activeColors[index].GetComponent<KButton>().onClick += (System.Action) (() =>
      {
        PixelPack.ColorPair colorSetting = this.targetPixelPack.colorSettings[current_id];
        ((Graphic) this.activeColors[current_id].GetComponent<Image>()).color = this.paintingColor;
        colorSetting.activeColor = this.paintingColor;
        this.targetPixelPack.colorSettings[current_id] = colorSetting;
        this.targetPixelPack.UpdateColors();
        this.HighlightUsedColors();
      });
      ((Graphic) this.standbyColors[index].GetComponent<Image>()).color = this.targetPixelPack.colorSettings[index].standbyColor;
      this.standbyColors[index].GetComponent<KButton>().onClick += (System.Action) (() =>
      {
        PixelPack.ColorPair colorSetting = this.targetPixelPack.colorSettings[current_id];
        ((Graphic) this.standbyColors[current_id].GetComponent<Image>()).color = this.paintingColor;
        colorSetting.standbyColor = this.paintingColor;
        this.targetPixelPack.colorSettings[current_id] = colorSetting;
        this.targetPixelPack.UpdateColors();
        this.HighlightUsedColors();
      });
    }
  }

  private void InitializeColorSwatch()
  {
    bool flag = false;
    for (int index = 0; index < this.colorSwatch.Count; ++index)
    {
      GameObject swatch = Util.KInstantiateUI(this.swatchEntry, this.colorSwatchContainer, true);
      Image component1 = swatch.GetComponent<Image>();
      ((Graphic) component1).color = this.colorSwatch[index];
      KButton component2 = swatch.GetComponent<KButton>();
      Color color = this.colorSwatch[index];
      if (Color.op_Equality(((Graphic) component1).color, Color.black))
        ((Graphic) swatch.GetComponent<HierarchyReferences>().GetReference("selected").GetComponentInChildren<Image>()).color = Color.white;
      if (!flag)
      {
        this.SelectColor(color, swatch);
        flag = true;
      }
      component2.onClick += (System.Action) (() => this.SelectColor(color, swatch));
      this.swatch_object_by_color[color] = swatch;
    }
  }

  private void SelectColor(Color color, GameObject swatchEntry)
  {
    this.paintingColor = color;
    swatchEntry.GetComponent<HierarchyReferences>().GetReference("selected").gameObject.SetActive(true);
    if (Object.op_Inequality((Object) this.selectedSwatchEntry, (Object) null) && Object.op_Inequality((Object) this.selectedSwatchEntry, (Object) swatchEntry))
      this.selectedSwatchEntry.GetComponent<HierarchyReferences>().GetReference("selected").gameObject.SetActive(false);
    this.selectedSwatchEntry = swatchEntry;
  }

  private void CopyActiveToStandby()
  {
    for (int index = 0; index < this.targetPixelPack.colorSettings.Count; ++index)
    {
      PixelPack.ColorPair colorSetting = this.targetPixelPack.colorSettings[index];
      colorSetting.standbyColor = colorSetting.activeColor;
      this.targetPixelPack.colorSettings[index] = colorSetting;
      ((Graphic) this.standbyColors[index].GetComponent<Image>()).color = colorSetting.activeColor;
    }
    this.HighlightUsedColors();
    this.targetPixelPack.UpdateColors();
  }

  private void CopyStandbyToActive()
  {
    for (int index = 0; index < this.targetPixelPack.colorSettings.Count; ++index)
    {
      PixelPack.ColorPair colorSetting = this.targetPixelPack.colorSettings[index];
      colorSetting.activeColor = colorSetting.standbyColor;
      this.targetPixelPack.colorSettings[index] = colorSetting;
      ((Graphic) this.activeColors[index].GetComponent<Image>()).color = colorSetting.standbyColor;
    }
    this.HighlightUsedColors();
    this.targetPixelPack.UpdateColors();
  }

  private void SwapColors()
  {
    for (int index = 0; index < this.targetPixelPack.colorSettings.Count; ++index)
    {
      PixelPack.ColorPair colorPair = new PixelPack.ColorPair();
      colorPair.activeColor = this.targetPixelPack.colorSettings[index].standbyColor;
      colorPair.standbyColor = this.targetPixelPack.colorSettings[index].activeColor;
      this.targetPixelPack.colorSettings[index] = colorPair;
      ((Graphic) this.activeColors[index].GetComponent<Image>()).color = colorPair.activeColor;
      ((Graphic) this.standbyColors[index].GetComponent<Image>()).color = colorPair.standbyColor;
    }
    this.HighlightUsedColors();
    this.targetPixelPack.UpdateColors();
  }

  public PixelPackSideScreen()
  {
    List<Color> colorList = new List<Color>();
    colorList.Add(new Color(0.4862745f, 0.4862745f, 0.4862745f));
    colorList.Add(new Color(0.0f, 0.0f, 0.9882353f));
    colorList.Add(new Color(0.0f, 0.0f, 0.7372549f));
    colorList.Add(new Color(0.266666681f, 0.156862751f, 0.7372549f));
    colorList.Add(new Color(0.5803922f, 0.0f, 0.5176471f));
    colorList.Add(new Color(0.65882355f, 0.0f, 0.1254902f));
    colorList.Add(new Color(0.65882355f, 0.0627451f, 0.0f));
    colorList.Add(new Color(0.533333361f, 0.0784313753f, 0.0f));
    colorList.Add(new Color(0.3137255f, 0.1882353f, 0.0f));
    colorList.Add(new Color(0.0f, 0.470588237f, 0.0f));
    colorList.Add(new Color(0.0f, 0.407843143f, 0.0f));
    colorList.Add(new Color(0.0f, 0.345098048f, 0.0f));
    colorList.Add(new Color(0.0f, 0.2509804f, 0.345098048f));
    colorList.Add(new Color(0.0f, 0.0f, 0.0f));
    colorList.Add(new Color(0.7372549f, 0.7372549f, 0.7372549f));
    colorList.Add(new Color(0.0f, 0.470588237f, 0.972549f));
    colorList.Add(new Color(0.0f, 0.345098048f, 0.972549f));
    colorList.Add(new Color(0.407843143f, 0.266666681f, 0.9882353f));
    colorList.Add(new Color(0.847058833f, 0.0f, 0.8f));
    colorList.Add(new Color(0.894117653f, 0.0f, 0.345098048f));
    colorList.Add(new Color(0.972549f, 0.219607845f, 0.0f));
    colorList.Add(new Color(0.894117653f, 0.360784322f, 0.0627451f));
    colorList.Add(new Color(0.6745098f, 0.4862745f, 0.0f));
    colorList.Add(new Color(0.0f, 0.721568644f, 0.0f));
    colorList.Add(new Color(0.0f, 0.65882355f, 0.0f));
    colorList.Add(new Color(0.0f, 0.65882355f, 0.266666681f));
    colorList.Add(new Color(0.0f, 0.533333361f, 0.533333361f));
    colorList.Add(new Color(0.0f, 0.0f, 0.0f));
    colorList.Add(new Color(0.972549f, 0.972549f, 0.972549f));
    colorList.Add(new Color(0.235294119f, 0.7372549f, 0.9882353f));
    colorList.Add(new Color(0.407843143f, 0.533333361f, 0.9882353f));
    colorList.Add(new Color(0.596078455f, 0.470588237f, 0.972549f));
    colorList.Add(new Color(0.972549f, 0.470588237f, 0.972549f));
    colorList.Add(new Color(0.972549f, 0.345098048f, 0.596078455f));
    colorList.Add(new Color(0.972549f, 0.470588237f, 0.345098048f));
    colorList.Add(new Color(0.9882353f, 0.627451f, 0.266666681f));
    colorList.Add(new Color(0.972549f, 0.721568644f, 0.0f));
    colorList.Add(new Color(0.721568644f, 0.972549f, 0.09411765f));
    colorList.Add(new Color(0.345098048f, 0.847058833f, 0.329411775f));
    colorList.Add(new Color(0.345098048f, 0.972549f, 0.596078455f));
    colorList.Add(new Color(0.0f, 0.9098039f, 0.847058833f));
    colorList.Add(new Color(0.470588237f, 0.470588237f, 0.470588237f));
    colorList.Add(new Color(0.9882353f, 0.9882353f, 0.9882353f));
    colorList.Add(new Color(0.6431373f, 0.894117653f, 0.9882353f));
    colorList.Add(new Color(0.721568644f, 0.721568644f, 0.972549f));
    colorList.Add(new Color(0.847058833f, 0.721568644f, 0.972549f));
    colorList.Add(new Color(0.972549f, 0.721568644f, 0.972549f));
    colorList.Add(new Color(0.972549f, 0.721568644f, 0.7529412f));
    colorList.Add(new Color(0.9411765f, 0.8156863f, 0.6901961f));
    colorList.Add(new Color(0.9882353f, 0.8784314f, 0.65882355f));
    colorList.Add(new Color(0.972549f, 0.847058833f, 0.470588237f));
    colorList.Add(new Color(0.847058833f, 0.972549f, 0.470588237f));
    colorList.Add(new Color(0.721568644f, 0.972549f, 0.721568644f));
    colorList.Add(new Color(0.721568644f, 0.972549f, 0.847058833f));
    colorList.Add(new Color(0.0f, 0.9882353f, 0.9882353f));
    colorList.Add(new Color(0.847058833f, 0.847058833f, 0.847058833f));
    this.colorSwatch = colorList;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }
}
