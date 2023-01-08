// Decompiled with JetBrains decompiler
// Type: CodexConversionPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexConversionPanel : CodexWidget<CodexConversionPanel>
{
  private LocText label;
  private GameObject materialPrefab;
  private GameObject fabricatorPrefab;
  private GameObject ingredientsContainer;
  private GameObject resultsContainer;
  private GameObject fabricatorContainer;
  private GameObject arrow1;
  private GameObject arrow2;
  private string title;
  private ElementUsage[] ins;
  private ElementUsage[] outs;
  private GameObject Converter;

  public CodexConversionPanel(
    string title,
    Tag ctag,
    float inputAmount,
    bool inputContinuous,
    Tag ptag,
    float outputAmount,
    bool outputContinuous,
    GameObject converter)
  {
    this.title = title;
    this.ins = new ElementUsage[1]
    {
      new ElementUsage(ctag, inputAmount, inputContinuous)
    };
    this.outs = new ElementUsage[1]
    {
      new ElementUsage(ptag, outputAmount, outputContinuous)
    };
    this.Converter = converter;
  }

  public CodexConversionPanel(
    string title,
    ElementUsage[] ins,
    ElementUsage[] outs,
    GameObject converter)
  {
    this.title = title;
    this.ins = ins != null ? ins : new ElementUsage[0];
    this.outs = outs != null ? outs : new ElementUsage[0];
    this.Converter = converter;
  }

  public override void Configure(
    GameObject contentGameObject,
    Transform displayPane,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
  {
    HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
    this.label = component.GetReference<LocText>("Title");
    this.materialPrefab = ((Component) component.GetReference<RectTransform>("MaterialPrefab")).gameObject;
    this.fabricatorPrefab = ((Component) component.GetReference<RectTransform>("FabricatorPrefab")).gameObject;
    this.ingredientsContainer = ((Component) component.GetReference<RectTransform>("IngredientsContainer")).gameObject;
    this.resultsContainer = ((Component) component.GetReference<RectTransform>("ResultsContainer")).gameObject;
    this.fabricatorContainer = ((Component) component.GetReference<RectTransform>("FabricatorContainer")).gameObject;
    this.arrow1 = ((Component) component.GetReference<RectTransform>("Arrow1")).gameObject;
    this.arrow2 = ((Component) component.GetReference<RectTransform>("Arrow2")).gameObject;
    this.ClearPanel();
    this.ConfigureConversion();
  }

  private Tuple<Sprite, Color> GetUISprite(Tag tag)
  {
    if (ElementLoader.GetElement(tag) != null)
      return Def.GetUISprite((object) ElementLoader.GetElement(tag));
    if (Object.op_Inequality((Object) Assets.GetPrefab(tag), (Object) null))
      return Def.GetUISprite((object) Assets.GetPrefab(tag));
    return Object.op_Inequality((Object) Assets.GetSprite(HashedString.op_Implicit(((Tag) ref tag).Name)), (Object) null) ? new Tuple<Sprite, Color>(Assets.GetSprite(HashedString.op_Implicit(((Tag) ref tag).Name)), Color.white) : (Tuple<Sprite, Color>) null;
  }

  private void ConfigureConversion()
  {
    ((TMP_Text) this.label).text = this.title;
    bool flag1 = false;
    foreach (ElementUsage elementUsage in this.ins)
    {
      Tag tag = elementUsage.tag;
      if (!Tag.op_Equality(tag, Tag.Invalid))
      {
        float amount = elementUsage.amount;
        flag1 = true;
        HierarchyReferences component = Util.KInstantiateUI(this.materialPrefab, this.ingredientsContainer, true).GetComponent<HierarchyReferences>();
        Tuple<Sprite, Color> uiSprite = this.GetUISprite(tag);
        if (uiSprite != null)
        {
          component.GetReference<Image>("Icon").sprite = uiSprite.first;
          ((Graphic) component.GetReference<Image>("Icon")).color = uiSprite.second;
        }
        GameUtil.TimeSlice timeSlice = elementUsage.continuous ? GameUtil.TimeSlice.PerCycle : GameUtil.TimeSlice.None;
        ((TMP_Text) component.GetReference<LocText>("Amount")).text = GameUtil.GetFormattedByTag(tag, amount, timeSlice);
        ((Graphic) component.GetReference<LocText>("Amount")).color = Color.black;
        string str = tag.ProperName();
        GameObject prefab = Assets.GetPrefab(tag);
        if (Object.op_Implicit((Object) prefab) && Object.op_Inequality((Object) prefab.GetComponent<Edible>(), (Object) null))
          str = str + "\n    • " + string.Format((string) STRINGS.UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, (object) GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
        component.GetReference<ToolTip>("Tooltip").toolTip = str;
        component.GetReference<KButton>("Button").onClick += (System.Action) (() => ManagementMenu.Instance.codexScreen.ChangeArticle(STRINGS.UI.ExtractLinkID(tag.ProperName()), targetPosition: new Vector3()));
      }
    }
    this.arrow1.SetActive(flag1);
    Tag tag1 = this.Converter.PrefabID();
    string name = ((Tag) ref tag1).Name;
    HierarchyReferences component1 = Util.KInstantiateUI(this.fabricatorPrefab, this.fabricatorContainer, true).GetComponent<HierarchyReferences>();
    Tuple<Sprite, Color> uiSprite1 = Def.GetUISprite((object) name);
    component1.GetReference<Image>("Icon").sprite = uiSprite1.first;
    ((Graphic) component1.GetReference<Image>("Icon")).color = uiSprite1.second;
    component1.GetReference<ToolTip>("Tooltip").toolTip = this.Converter.GetProperName();
    component1.GetReference<KButton>("Button").onClick += (System.Action) (() => ManagementMenu.Instance.codexScreen.ChangeArticle(STRINGS.UI.ExtractLinkID(this.Converter.GetProperName()), targetPosition: new Vector3()));
    bool flag2 = false;
    foreach (ElementUsage elementUsage in this.outs)
    {
      Tag tag = elementUsage.tag;
      if (!Tag.op_Equality(tag, Tag.Invalid))
      {
        float amount = elementUsage.amount;
        flag2 = true;
        HierarchyReferences component2 = Util.KInstantiateUI(this.materialPrefab, this.resultsContainer, true).GetComponent<HierarchyReferences>();
        Tuple<Sprite, Color> uiSprite2 = this.GetUISprite(tag);
        if (uiSprite2 != null)
        {
          component2.GetReference<Image>("Icon").sprite = uiSprite2.first;
          ((Graphic) component2.GetReference<Image>("Icon")).color = uiSprite2.second;
        }
        GameUtil.TimeSlice timeSlice = elementUsage.continuous ? GameUtil.TimeSlice.PerCycle : GameUtil.TimeSlice.None;
        ((TMP_Text) component2.GetReference<LocText>("Amount")).text = GameUtil.GetFormattedByTag(tag, amount, timeSlice);
        ((Graphic) component2.GetReference<LocText>("Amount")).color = Color.black;
        string str = tag.ProperName();
        GameObject prefab = Assets.GetPrefab(tag);
        if (Object.op_Implicit((Object) prefab) && Object.op_Inequality((Object) prefab.GetComponent<Edible>(), (Object) null))
          str = str + "\n    • " + string.Format((string) STRINGS.UI.GAMEOBJECTEFFECTS.FOOD_QUALITY, (object) GameUtil.GetFormattedFoodQuality(prefab.GetComponent<Edible>().GetQuality()));
        component2.GetReference<ToolTip>("Tooltip").toolTip = str;
        component2.GetReference<KButton>("Button").onClick += (System.Action) (() => ManagementMenu.Instance.codexScreen.ChangeArticle(STRINGS.UI.ExtractLinkID(tag.ProperName()), targetPosition: new Vector3()));
      }
    }
    this.arrow2.SetActive(flag2);
  }

  private void ClearPanel()
  {
    foreach (Component component in this.ingredientsContainer.transform)
      Object.Destroy((Object) component.gameObject);
    foreach (Component component in this.resultsContainer.transform)
      Object.Destroy((Object) component.gameObject);
    foreach (Component component in this.fabricatorContainer.transform)
      Object.Destroy((Object) component.gameObject);
  }
}
