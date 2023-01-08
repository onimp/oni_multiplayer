// Decompiled with JetBrains decompiler
// Type: ResourceEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ResourceEntry")]
public class ResourceEntry : 
  KMonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler,
  ISim4000ms
{
  public Tag Resource;
  public GameUtil.MeasureUnit Measure;
  public LocText NameLabel;
  public LocText QuantityLabel;
  public Image image;
  [SerializeField]
  private Color AvailableColor;
  [SerializeField]
  private Color UnavailableColor;
  [SerializeField]
  private Color OverdrawnColor;
  [SerializeField]
  private Color HighlightColor;
  [SerializeField]
  private Color BackgroundHoverColor;
  [SerializeField]
  private Image Background;
  [MyCmpGet]
  private ToolTip tooltip;
  [MyCmpReq]
  private Button button;
  public GameObject sparkChart;
  private const float CLICK_RESET_TIME_THRESHOLD = 10f;
  private int selectionIdx;
  private float lastClickTime;
  private List<Pickupable> cachedPickupables;
  private float currentQuantity = float.MinValue;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Graphic) this.QuantityLabel).color = this.AvailableColor;
    ((Graphic) this.NameLabel).color = this.AvailableColor;
    // ISSUE: method pointer
    ((UnityEvent) this.button.onClick).AddListener(new UnityAction((object) this, __methodptr(OnClick)));
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.tooltip.OnToolTip = new Func<string>(this.OnToolTip);
    this.RefreshChart();
  }

  private void OnClick()
  {
    this.lastClickTime = Time.unscaledTime;
    if (this.cachedPickupables == null)
    {
      this.cachedPickupables = ClusterManager.Instance.activeWorld.worldInventory.CreatePickupablesList(this.Resource);
      ((MonoBehaviour) this).StartCoroutine(this.ClearCachedPickupablesAfterThreshold());
    }
    if (this.cachedPickupables == null)
      return;
    Pickupable cmp = (Pickupable) null;
    for (int index = 0; index < this.cachedPickupables.Count; ++index)
    {
      ++this.selectionIdx;
      cmp = this.cachedPickupables[this.selectionIdx % this.cachedPickupables.Count];
      if (Object.op_Inequality((Object) cmp, (Object) null) && !((Component) cmp).HasTag(GameTags.StoredPrivate))
        break;
    }
    if (!Object.op_Inequality((Object) cmp, (Object) null))
      return;
    Transform transform = cmp.transform;
    if (Object.op_Inequality((Object) cmp.storage, (Object) null))
      transform = cmp.storage.transform;
    SelectTool.Instance.SelectAndFocus(TransformExtensions.GetPosition(((Component) transform).transform), ((Component) transform).GetComponent<KSelectable>(), Vector3.zero);
    for (int index = 0; index < this.cachedPickupables.Count; ++index)
    {
      Pickupable cachedPickupable = this.cachedPickupables[index];
      if (Object.op_Inequality((Object) cachedPickupable, (Object) null))
      {
        KAnimControllerBase component = ((Component) cachedPickupable).GetComponent<KAnimControllerBase>();
        if (Object.op_Inequality((Object) component, (Object) null))
          component.HighlightColour = Color32.op_Implicit(this.HighlightColor);
      }
    }
  }

  private IEnumerator ClearCachedPickupablesAfterThreshold()
  {
    while (this.cachedPickupables != null && (double) this.lastClickTime != 0.0 && (double) Time.unscaledTime - (double) this.lastClickTime < 10.0)
      yield return (object) SequenceUtil.WaitForSeconds(1f);
    this.cachedPickupables = (List<Pickupable>) null;
  }

  public void GetAmounts(
    EdiblesManager.FoodInfo food_info,
    bool doExtras,
    out float available,
    out float total,
    out float reserved)
  {
    available = ClusterManager.Instance.activeWorld.worldInventory.GetAmount(this.Resource, false);
    total = doExtras ? ClusterManager.Instance.activeWorld.worldInventory.GetTotalAmount(this.Resource, false) : 0.0f;
    reserved = doExtras ? MaterialNeeds.GetAmount(this.Resource, ClusterManager.Instance.activeWorldId, false) : 0.0f;
    if (food_info == null)
      return;
    available *= food_info.CaloriesPerUnit;
    total *= food_info.CaloriesPerUnit;
    reserved *= food_info.CaloriesPerUnit;
  }

  private void GetAmounts(bool doExtras, out float available, out float total, out float reserved) => this.GetAmounts(this.Measure == GameUtil.MeasureUnit.kcal ? EdiblesManager.GetFoodInfo(((Tag) ref this.Resource).Name) : (EdiblesManager.FoodInfo) null, doExtras, out available, out total, out reserved);

  public void UpdateValue()
  {
    this.SetName(this.Resource.ProperName());
    float available;
    float total;
    float reserved;
    this.GetAmounts(GenericGameSettings.instance.allowInsufficientMaterialBuild, out available, out total, out reserved);
    if ((double) this.currentQuantity != (double) available)
    {
      this.currentQuantity = available;
      ((TMP_Text) this.QuantityLabel).text = ResourceCategoryScreen.QuantityTextForMeasure(available, this.Measure);
    }
    Color color = this.AvailableColor;
    if ((double) reserved > (double) total)
      color = this.OverdrawnColor;
    else if ((double) available == 0.0)
      color = this.UnavailableColor;
    if (Color.op_Inequality(((Graphic) this.QuantityLabel).color, color))
      ((Graphic) this.QuantityLabel).color = color;
    if (!Color.op_Inequality(((Graphic) this.NameLabel).color, color))
      return;
    ((Graphic) this.NameLabel).color = color;
  }

  private string OnToolTip()
  {
    float available;
    float total;
    float reserved;
    this.GetAmounts(true, out available, out total, out reserved);
    string str = ((TMP_Text) this.NameLabel).text + "\n" + string.Format((string) STRINGS.UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, (object) ResourceCategoryScreen.QuantityTextForMeasure(available, this.Measure), (object) ResourceCategoryScreen.QuantityTextForMeasure(reserved, this.Measure), (object) ResourceCategoryScreen.QuantityTextForMeasure(total, this.Measure));
    float delta = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, this.Resource).GetDelta(150f);
    return (double) delta == 0.0 ? str + "\n\n" + (string) STRINGS.UI.RESOURCESCREEN.TREND_TOOLTIP_NO_CHANGE : str + "\n\n" + string.Format((string) STRINGS.UI.RESOURCESCREEN.TREND_TOOLTIP, (double) delta > 0.0 ? (object) STRINGS.UI.RESOURCESCREEN.INCREASING_STR : (object) STRINGS.UI.RESOURCESCREEN.DECREASING_STR, (object) GameUtil.GetFormattedMass(Mathf.Abs(delta)));
  }

  public void SetName(string name) => ((TMP_Text) this.NameLabel).text = name;

  public void SetTag(Tag t, GameUtil.MeasureUnit measure)
  {
    this.Resource = t;
    this.Measure = measure;
    this.cachedPickupables = (List<Pickupable>) null;
  }

  private void Hover(bool is_hovering)
  {
    if (Object.op_Equality((Object) ClusterManager.Instance.activeWorld.worldInventory, (Object) null))
      return;
    if (is_hovering)
      ((Graphic) this.Background).color = this.BackgroundHoverColor;
    else
      ((Graphic) this.Background).color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    ICollection<Pickupable> pickupables = ClusterManager.Instance.activeWorld.worldInventory.GetPickupables(this.Resource);
    if (pickupables == null)
      return;
    foreach (Pickupable pickupable in (IEnumerable<Pickupable>) pickupables)
    {
      if (!Object.op_Equality((Object) pickupable, (Object) null))
      {
        KAnimControllerBase component = ((Component) pickupable).GetComponent<KAnimControllerBase>();
        if (!Object.op_Equality((Object) component, (Object) null))
          component.HighlightColour = !is_hovering ? Color32.op_Implicit(Color.black) : Color32.op_Implicit(this.HighlightColor);
      }
    }
  }

  public void OnPointerEnter(PointerEventData eventData) => this.Hover(true);

  public void OnPointerExit(PointerEventData eventData) => this.Hover(false);

  public void SetSprite(Tag t)
  {
    Element elementByName = ElementLoader.FindElementByName(((Tag) ref this.Resource).Name);
    if (elementByName == null)
      return;
    Sprite fromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(elementByName.substance.anim);
    if (!Object.op_Inequality((Object) fromMultiObjectAnim, (Object) null))
      return;
    this.image.sprite = fromMultiObjectAnim;
  }

  public void SetSprite(Sprite sprite) => this.image.sprite = sprite;

  public void Sim4000ms(float dt) => this.RefreshChart();

  private void RefreshChart()
  {
    if (!Object.op_Inequality((Object) this.sparkChart, (Object) null))
      return;
    ResourceTracker resourceStatistic = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, this.Resource);
    this.sparkChart.GetComponentInChildren<LineLayer>().RefreshLine(resourceStatistic.ChartableData(3000f), "resourceAmount");
    this.sparkChart.GetComponentInChildren<SparkLayer>().SetColor(Constants.NEUTRAL_COLOR);
  }
}
