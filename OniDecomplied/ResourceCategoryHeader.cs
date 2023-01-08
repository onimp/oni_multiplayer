// Decompiled with JetBrains decompiler
// Type: ResourceCategoryHeader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ResourceCategoryHeader")]
public class ResourceCategoryHeader : 
  KMonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler,
  ISim4000ms
{
  public GameObject Prefab_ResourceEntry;
  public Transform EntryContainer;
  public Tag ResourceCategoryTag;
  public GameUtil.MeasureUnit Measure;
  public bool IsOpen;
  public ImageToggleState expandArrow;
  private Button mButton;
  public Dictionary<Tag, ResourceEntry> ResourcesDiscovered = new Dictionary<Tag, ResourceEntry>();
  public ResourceCategoryHeader.ElementReferences elements;
  public Color TextColor_Interactable;
  public Color TextColor_NonInteractable;
  private string quantityString;
  private float currentQuantity;
  private bool anyDiscovered;
  public const float chartHistoryLength = 3000f;
  [MyCmpGet]
  private ToolTip tooltip;
  [SerializeField]
  private int minimizedFontSize;
  [SerializeField]
  private int maximizedFontSize;
  [SerializeField]
  private Color highlightColour;
  [SerializeField]
  private Color BackgroundHoverColor;
  [SerializeField]
  private Image Background;
  public GameObject sparkChart;
  private float cachedAvailable = float.MinValue;
  private float cachedTotal = float.MinValue;
  private float cachedReserved = float.MinValue;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.EntryContainer.SetParent(this.transform.parent);
    this.EntryContainer.SetSiblingIndex(this.transform.GetSiblingIndex() + 1);
    this.EntryContainer.localScale = Vector3.one;
    this.mButton = ((Component) this).GetComponent<Button>();
    // ISSUE: method pointer
    ((UnityEvent) this.mButton.onClick).AddListener(new UnityAction((object) this, __methodptr(\u003COnPrefabInit\u003Eb__23_0)));
    this.SetInteractable(this.anyDiscovered);
    this.SetActiveColor(false);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.tooltip.OnToolTip = new Func<string>(this.OnTooltip);
    this.UpdateContents();
    this.RefreshChart();
  }

  private void SetInteractable(bool state)
  {
    if (state)
    {
      if (!this.IsOpen)
        this.expandArrow.SetInactive();
      else
        this.expandArrow.SetActive();
    }
    else
    {
      this.SetOpen(false);
      this.expandArrow.SetDisabled();
    }
  }

  private void SetActiveColor(bool state)
  {
    if (state)
    {
      ((Graphic) this.elements.QuantityText).color = this.TextColor_Interactable;
      ((Graphic) this.elements.LabelText).color = this.TextColor_Interactable;
      this.expandArrow.ActiveColour = this.TextColor_Interactable;
      this.expandArrow.InactiveColour = this.TextColor_Interactable;
      ((Graphic) this.expandArrow.TargetImage).color = this.TextColor_Interactable;
    }
    else
    {
      ((Graphic) this.elements.LabelText).color = this.TextColor_NonInteractable;
      ((Graphic) this.elements.QuantityText).color = this.TextColor_NonInteractable;
      this.expandArrow.ActiveColour = this.TextColor_NonInteractable;
      this.expandArrow.InactiveColour = this.TextColor_NonInteractable;
      ((Graphic) this.expandArrow.TargetImage).color = this.TextColor_NonInteractable;
    }
  }

  public void SetTag(Tag t, GameUtil.MeasureUnit measure)
  {
    this.ResourceCategoryTag = t;
    this.Measure = measure;
    ((TMP_Text) this.elements.LabelText).text = t.ProperName();
    if (!SaveGame.Instance.expandedResourceTags.Contains(this.ResourceCategoryTag))
      return;
    this.anyDiscovered = true;
    this.ToggleOpen(false);
  }

  private void ToggleOpen(bool play_sound)
  {
    if (!this.anyDiscovered)
    {
      if (!play_sound)
        return;
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
    }
    else if (!this.IsOpen)
    {
      if (play_sound)
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open"));
      this.SetOpen(true);
      ((TMP_Text) this.elements.LabelText).fontSize = (float) this.maximizedFontSize;
      ((TMP_Text) this.elements.QuantityText).fontSize = (float) this.maximizedFontSize;
    }
    else
    {
      if (play_sound)
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
      this.SetOpen(false);
      ((TMP_Text) this.elements.LabelText).fontSize = (float) this.minimizedFontSize;
      ((TMP_Text) this.elements.QuantityText).fontSize = (float) this.minimizedFontSize;
    }
  }

  private void Hover(bool is_hovering)
  {
    ((Graphic) this.Background).color = is_hovering ? this.BackgroundHoverColor : new Color(0.0f, 0.0f, 0.0f, 0.0f);
    ICollection<Pickupable> pickupables = (ICollection<Pickupable>) null;
    if (Object.op_Inequality((Object) ClusterManager.Instance.activeWorld.worldInventory, (Object) null))
      pickupables = ClusterManager.Instance.activeWorld.worldInventory.GetPickupables(this.ResourceCategoryTag);
    if (pickupables == null)
      return;
    foreach (Pickupable pickupable in (IEnumerable<Pickupable>) pickupables)
    {
      if (!Object.op_Equality((Object) pickupable, (Object) null))
      {
        KAnimControllerBase component = ((Component) pickupable).GetComponent<KAnimControllerBase>();
        if (!Object.op_Equality((Object) component, (Object) null))
          component.HighlightColour = Color32.op_Implicit(is_hovering ? this.highlightColour : Color.black);
      }
    }
  }

  public void OnPointerEnter(PointerEventData eventData) => this.Hover(true);

  public void OnPointerExit(PointerEventData eventData) => this.Hover(false);

  public void SetOpen(bool open)
  {
    this.IsOpen = open;
    if (open)
    {
      this.expandArrow.SetActive();
      if (!SaveGame.Instance.expandedResourceTags.Contains(this.ResourceCategoryTag))
        SaveGame.Instance.expandedResourceTags.Add(this.ResourceCategoryTag);
    }
    else
    {
      this.expandArrow.SetInactive();
      SaveGame.Instance.expandedResourceTags.Remove(this.ResourceCategoryTag);
    }
    ((Component) this.EntryContainer).gameObject.SetActive(this.IsOpen);
  }

  private void GetAmounts(bool doExtras, out float available, out float total, out float reserved)
  {
    available = 0.0f;
    total = 0.0f;
    reserved = 0.0f;
    HashSet<Tag> resources = (HashSet<Tag>) null;
    if (!DiscoveredResources.Instance.TryGetDiscoveredResourcesFromTag(this.ResourceCategoryTag, out resources))
      return;
    ListPool<Tag, ResourceCategoryHeader>.PooledList pooledList = ListPool<Tag, ResourceCategoryHeader>.Allocate();
    foreach (Tag tag in resources)
    {
      EdiblesManager.FoodInfo food_info = (EdiblesManager.FoodInfo) null;
      if (this.Measure == GameUtil.MeasureUnit.kcal)
      {
        food_info = EdiblesManager.GetFoodInfo(((Tag) ref tag).Name);
        if (food_info == null)
        {
          ((List<Tag>) pooledList).Add(tag);
          continue;
        }
      }
      this.anyDiscovered = true;
      ResourceEntry resourceEntry = (ResourceEntry) null;
      if (!this.ResourcesDiscovered.TryGetValue(tag, out resourceEntry))
      {
        resourceEntry = this.NewResourceEntry(tag, this.Measure);
        this.ResourcesDiscovered.Add(tag, resourceEntry);
      }
      float available1;
      float total1;
      float reserved1;
      resourceEntry.GetAmounts(food_info, doExtras, out available1, out total1, out reserved1);
      available += available1;
      total += total1;
      reserved += reserved1;
    }
    foreach (Tag tag in (List<Tag>) pooledList)
      resources.Remove(tag);
    pooledList.Recycle();
  }

  public void UpdateContents()
  {
    float available;
    float total;
    float reserved;
    this.GetAmounts(false, out available, out total, out reserved);
    if ((double) available != (double) this.cachedAvailable || (double) total != (double) this.cachedTotal || (double) reserved != (double) this.cachedReserved)
    {
      if (this.quantityString == null || (double) this.currentQuantity != (double) available)
      {
        switch (this.Measure)
        {
          case GameUtil.MeasureUnit.mass:
            this.quantityString = GameUtil.GetFormattedMass(available);
            break;
          case GameUtil.MeasureUnit.kcal:
            this.quantityString = GameUtil.GetFormattedCalories(available);
            break;
          case GameUtil.MeasureUnit.quantity:
            this.quantityString = available.ToString();
            break;
        }
        ((TMP_Text) this.elements.QuantityText).text = this.quantityString;
        this.currentQuantity = available;
      }
      this.cachedAvailable = available;
      this.cachedTotal = total;
      this.cachedReserved = reserved;
    }
    foreach (KeyValuePair<Tag, ResourceEntry> keyValuePair in this.ResourcesDiscovered)
      keyValuePair.Value.UpdateValue();
    this.SetActiveColor((double) available > 0.0);
    this.SetInteractable(this.anyDiscovered);
  }

  private string OnTooltip()
  {
    float available;
    float total;
    float reserved;
    this.GetAmounts(true, out available, out total, out reserved);
    string str = ((TMP_Text) this.elements.LabelText).text + "\n" + string.Format((string) STRINGS.UI.RESOURCESCREEN.AVAILABLE_TOOLTIP, (object) ResourceCategoryScreen.QuantityTextForMeasure(available, this.Measure), (object) ResourceCategoryScreen.QuantityTextForMeasure(reserved, this.Measure), (object) ResourceCategoryScreen.QuantityTextForMeasure(total, this.Measure));
    float delta = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, this.ResourceCategoryTag).GetDelta(150f);
    return (double) delta == 0.0 ? str + "\n\n" + (string) STRINGS.UI.RESOURCESCREEN.TREND_TOOLTIP_NO_CHANGE : str + "\n\n" + string.Format((string) STRINGS.UI.RESOURCESCREEN.TREND_TOOLTIP, (double) delta > 0.0 ? (object) STRINGS.UI.RESOURCESCREEN.INCREASING_STR : (object) STRINGS.UI.RESOURCESCREEN.DECREASING_STR, (object) GameUtil.GetFormattedMass(Mathf.Abs(delta)));
  }

  private ResourceEntry NewResourceEntry(Tag resourceTag, GameUtil.MeasureUnit measure)
  {
    ResourceEntry component = Util.KInstantiateUI(this.Prefab_ResourceEntry, ((Component) this.EntryContainer).gameObject, true).GetComponent<ResourceEntry>();
    component.SetTag(resourceTag, measure);
    return component;
  }

  public void Sim4000ms(float dt) => this.RefreshChart();

  private void RefreshChart()
  {
    if (!Object.op_Inequality((Object) this.sparkChart, (Object) null))
      return;
    ResourceTracker resourceStatistic = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, this.ResourceCategoryTag);
    this.sparkChart.GetComponentInChildren<LineLayer>().RefreshLine(resourceStatistic.ChartableData(3000f), "resourceAmount");
    this.sparkChart.GetComponentInChildren<SparkLayer>().SetColor(Constants.NEUTRAL_COLOR);
  }

  [Serializable]
  public struct ElementReferences
  {
    public LocText LabelText;
    public LocText QuantityText;
  }
}
