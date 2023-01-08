// Decompiled with JetBrains decompiler
// Type: CodexConfigurableConsumerRecipePanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodexConfigurableConsumerRecipePanel : CodexWidget<CodexConfigurableConsumerRecipePanel>
{
  private LocText title;
  private LocText result_description;
  private Image resultIcon;
  private GameObject ingredient_original;
  private IConfigurableConsumerOption data;
  private GameObject[] _ingredientRows;

  public CodexConfigurableConsumerRecipePanel(IConfigurableConsumerOption data) => this.data = data;

  public override void Configure(
    GameObject contentGameObject,
    Transform displayPane,
    Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
  {
    HierarchyReferences component = contentGameObject.GetComponent<HierarchyReferences>();
    this.title = component.GetReference<LocText>("Title");
    this.result_description = component.GetReference<LocText>("ResultDescription");
    this.resultIcon = component.GetReference<Image>("ResultIcon");
    this.ingredient_original = ((Component) component.GetReference<RectTransform>("IngredientPrefab")).gameObject;
    this.ingredient_original.SetActive(false);
    new CodexText().ConfigureLabel(this.ingredient_original.GetComponent<HierarchyReferences>().GetReference<LocText>("Name"), textStyles);
    this.Clear();
    if (this.data == null)
      return;
    ((TMP_Text) this.title).text = this.data.GetName();
    ((TMP_Text) this.result_description).text = this.data.GetDescription();
    ((Graphic) this.result_description).color = Color.black;
    this.resultIcon.sprite = this.data.GetIcon();
    IConfigurableConsumerIngredient[] ingredients = this.data.GetIngredients();
    this._ingredientRows = new GameObject[ingredients.Length];
    for (int index = 0; index < this._ingredientRows.Length; ++index)
      this._ingredientRows[index] = this.CreateIngredientRow(ingredients[index]);
  }

  public GameObject CreateIngredientRow(IConfigurableConsumerIngredient ingredient)
  {
    Tag[] idSets = ingredient.GetIDSets();
    if (!Object.op_Inequality((Object) this.ingredient_original, (Object) null) || idSets.Length == 0)
      return (GameObject) null;
    GameObject ingredientRow = Util.KInstantiateUI(this.ingredient_original, ((Component) this.ingredient_original.transform.parent).gameObject, true);
    HierarchyReferences component = ingredientRow.GetComponent<HierarchyReferences>();
    Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) idSets[0]);
    component.GetReference<Image>("Icon").sprite = uiSprite.first;
    ((Graphic) component.GetReference<Image>("Icon")).color = uiSprite.second;
    ((TMP_Text) component.GetReference<LocText>("Name")).text = idSets[0].ProperName();
    ((TMP_Text) component.GetReference<LocText>("Amount")).text = GameUtil.GetFormattedByTag(idSets[0], ingredient.GetAmount());
    ((Graphic) component.GetReference<LocText>("Amount")).color = Color.black;
    return ingredientRow;
  }

  public void Clear()
  {
    if (this._ingredientRows == null)
      return;
    for (int index = 0; index < this._ingredientRows.Length; ++index)
      Object.Destroy((Object) this._ingredientRows[index]);
    this._ingredientRows = (GameObject[]) null;
  }
}
