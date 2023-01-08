// Decompiled with JetBrains decompiler
// Type: ResourceCategoryScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCategoryScreen : KScreen
{
  public static ResourceCategoryScreen Instance;
  public GameObject Prefab_CategoryBar;
  public Transform CategoryContainer;
  public MultiToggle HiderButton;
  public KLayoutElement HideTarget;
  private float HideSpeedFactor = 12f;
  private float maxHeightPadding = 480f;
  private float targetContentHideHeight;
  public Dictionary<Tag, ResourceCategoryHeader> DisplayedCategories = new Dictionary<Tag, ResourceCategoryHeader>();
  private Tag[] DisplayedCategoryKeys;
  private int categoryUpdatePacer;

  public static void DestroyInstance() => ResourceCategoryScreen.Instance = (ResourceCategoryScreen) null;

  protected virtual void OnActivate()
  {
    base.OnActivate();
    ResourceCategoryScreen.Instance = this;
    this.ConsumeMouseScroll = true;
    this.HiderButton.onClick += new System.Action(this.OnHiderClick);
    this.OnHiderClick();
    this.CreateTagSetHeaders((IEnumerable<Tag>) GameTags.MaterialCategories, GameUtil.MeasureUnit.mass);
    this.CreateTagSetHeaders((IEnumerable<Tag>) GameTags.CalorieCategories, GameUtil.MeasureUnit.kcal);
    this.CreateTagSetHeaders((IEnumerable<Tag>) GameTags.UnitCategories, GameUtil.MeasureUnit.quantity);
    if (!this.DisplayedCategories.ContainsKey(GameTags.Miscellaneous))
    {
      ResourceCategoryHeader resourceCategoryHeader = this.NewCategoryHeader(GameTags.Miscellaneous, GameUtil.MeasureUnit.mass);
      this.DisplayedCategories.Add(GameTags.Miscellaneous, resourceCategoryHeader);
    }
    this.DisplayedCategoryKeys = ((IEnumerable<Tag>) this.DisplayedCategories.Keys).ToArray<Tag>();
  }

  private void CreateTagSetHeaders(IEnumerable<Tag> set, GameUtil.MeasureUnit measure)
  {
    foreach (Tag tag in set)
    {
      ResourceCategoryHeader resourceCategoryHeader = this.NewCategoryHeader(tag, measure);
      this.DisplayedCategories.Add(tag, resourceCategoryHeader);
    }
  }

  private void OnHiderClick()
  {
    this.HiderButton.NextState();
    if (this.HiderButton.CurrentState == 0)
    {
      this.targetContentHideHeight = 0.0f;
    }
    else
    {
      double num = ((double) Screen.height - (double) this.maxHeightPadding) / (double) GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetCanvasScale();
      Rect rect = Util.rectTransform((Component) this.CategoryContainer).rect;
      double height = (double) ((Rect) ref rect).height;
      this.targetContentHideHeight = Mathf.Min((float) num, (float) height);
    }
  }

  private void Update()
  {
    if (Object.op_Equality((Object) ClusterManager.Instance.activeWorld.worldInventory, (Object) null))
      return;
    if ((double) ((LayoutElement) this.HideTarget).minHeight != (double) this.targetContentHideHeight)
    {
      float minHeight = ((LayoutElement) this.HideTarget).minHeight;
      float num1 = this.targetContentHideHeight - minHeight;
      float num2 = Mathf.Clamp(num1 * this.HideSpeedFactor * Time.unscaledDeltaTime, (double) num1 > 0.0 ? -num1 : num1, (double) num1 > 0.0 ? num1 : -num1);
      ((LayoutElement) this.HideTarget).minHeight = minHeight + num2;
    }
    for (int index = 0; index < 1; ++index)
    {
      Tag displayedCategoryKey = this.DisplayedCategoryKeys[this.categoryUpdatePacer];
      ResourceCategoryHeader displayedCategory = this.DisplayedCategories[displayedCategoryKey];
      if (DiscoveredResources.Instance.IsDiscovered(displayedCategoryKey) && !((Component) displayedCategory).gameObject.activeInHierarchy)
        ((Component) displayedCategory).gameObject.SetActive(true);
      displayedCategory.UpdateContents();
      this.categoryUpdatePacer = (this.categoryUpdatePacer + 1) % this.DisplayedCategoryKeys.Length;
    }
    if (this.HiderButton.CurrentState != 0)
    {
      double num = ((double) Screen.height - (double) this.maxHeightPadding) / (double) GameScreenManager.Instance.ssOverlayCanvas.GetComponent<KCanvasScaler>().GetCanvasScale();
      Rect rect = Util.rectTransform((Component) this.CategoryContainer).rect;
      double height = (double) ((Rect) ref rect).height;
      this.targetContentHideHeight = Mathf.Min((float) num, (float) height);
    }
    if (!Object.op_Inequality((Object) MeterScreen.Instance, (Object) null) || MeterScreen.Instance.StartValuesSet)
      return;
    MeterScreen.Instance.InitializeValues();
  }

  private ResourceCategoryHeader NewCategoryHeader(Tag categoryTag, GameUtil.MeasureUnit measure)
  {
    GameObject gameObject = Util.KInstantiateUI(this.Prefab_CategoryBar, ((Component) this.CategoryContainer).gameObject, false);
    ((Object) gameObject).name = "CategoryHeader_" + ((Tag) ref categoryTag).Name;
    ResourceCategoryHeader component = gameObject.GetComponent<ResourceCategoryHeader>();
    component.SetTag(categoryTag, measure);
    return component;
  }

  public static string QuantityTextForMeasure(float quantity, GameUtil.MeasureUnit measure)
  {
    switch (measure)
    {
      case GameUtil.MeasureUnit.mass:
        return GameUtil.GetFormattedMass(quantity);
      case GameUtil.MeasureUnit.kcal:
        return GameUtil.GetFormattedCalories(quantity);
      default:
        return quantity.ToString();
    }
  }
}
