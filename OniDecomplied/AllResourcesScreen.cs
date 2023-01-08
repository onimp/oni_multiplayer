// Decompiled with JetBrains decompiler
// Type: AllResourcesScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AllResourcesScreen : KScreen, ISim4000ms, ISim1000ms
{
  private Dictionary<Tag, AllResourcesScreen.ResourceRow> resourceRows = new Dictionary<Tag, AllResourcesScreen.ResourceRow>();
  private Dictionary<Tag, AllResourcesScreen.CategoryRow> categoryRows = new Dictionary<Tag, AllResourcesScreen.CategoryRow>();
  public Dictionary<Tag, GameUtil.MeasureUnit> units = new Dictionary<Tag, GameUtil.MeasureUnit>();
  public GameObject rootListContainer;
  public GameObject resourceLinePrefab;
  public GameObject categoryLinePrefab;
  public KButton closeButton;
  public bool allowRefresh = true;
  private bool currentlyShown;
  [SerializeField]
  private KInputTextField searchInputField;
  [SerializeField]
  private KButton clearSearchButton;
  public static AllResourcesScreen Instance;
  public Dictionary<Tag, bool> currentlyDisplayedRows = new Dictionary<Tag, bool>();
  public List<TagSet> allowDisplayCategories = new List<TagSet>();
  private bool initialized;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    AllResourcesScreen.Instance = this;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.ConsumeMouseScroll = true;
    this.Init();
  }

  protected virtual void OnForcedCleanUp()
  {
    AllResourcesScreen.Instance = (AllResourcesScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  public void Init()
  {
    if (this.initialized)
      return;
    this.initialized = true;
    this.Populate();
    Game.Instance.Subscribe(1983128072, new Action<object>(this.Populate));
    DiscoveredResources.Instance.OnDiscover += (Action<Tag, Tag>) ((a, b) => this.Populate());
    this.closeButton.onClick += (System.Action) (() => this.Show(false));
    this.clearSearchButton.onClick += (System.Action) (() => ((TMP_InputField) this.searchInputField).text = "");
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.searchInputField).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(\u003CInit\u003Eb__21_3)));
    KInputTextField searchInputField = this.searchInputField;
    ((TMP_InputField) searchInputField).onFocus = ((TMP_InputField) searchInputField).onFocus + (System.Action) (() => this.isEditing = true);
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.searchInputField).onEndEdit).AddListener(new UnityAction<string>((object) this, __methodptr(\u003CInit\u003Eb__21_4)));
    this.Show(false);
  }

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    this.currentlyShown = show;
    if (!show)
      return;
    ManagementMenu.Instance.CloseAll();
    AllDiagnosticsScreen.Instance.Show(false);
    this.RefreshRows();
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1))
    {
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
      this.Show(false);
      ((KInputEvent) e).Consumed = true;
    }
    if (this.isEditing)
      ((KInputEvent) e).Consumed = true;
    else
      base.OnKeyDown(e);
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
    {
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
      this.Show(false);
      ((KInputEvent) e).Consumed = true;
    }
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyUp(e);
  }

  public virtual float GetSortKey() => 50f;

  public void Populate(object data = null) => this.SpawnRows();

  private void SpawnRows()
  {
    WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
    this.allowDisplayCategories.Add(GameTags.MaterialCategories);
    this.allowDisplayCategories.Add(GameTags.CalorieCategories);
    this.allowDisplayCategories.Add(GameTags.UnitCategories);
    foreach (Tag materialCategory in GameTags.MaterialCategories)
      this.SpawnCategoryRow(materialCategory, GameUtil.MeasureUnit.mass);
    foreach (Tag calorieCategory in GameTags.CalorieCategories)
      this.SpawnCategoryRow(calorieCategory, GameUtil.MeasureUnit.kcal);
    foreach (Tag unitCategory in GameTags.UnitCategories)
      this.SpawnCategoryRow(unitCategory, GameUtil.MeasureUnit.quantity);
    List<Tag> tagList = new List<Tag>();
    foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> categoryRow in this.categoryRows)
      tagList.Add(categoryRow.Key);
    tagList.Sort((Comparison<Tag>) ((a, b) => a.ProperNameStripLink().CompareTo(b.ProperNameStripLink())));
    foreach (Tag key in tagList)
      this.categoryRows[key].GameObject.transform.SetAsLastSibling();
  }

  private void SpawnCategoryRow(Tag categoryTag, GameUtil.MeasureUnit unit)
  {
    if (!this.categoryRows.ContainsKey(categoryTag))
    {
      GameObject gameObject = Util.KInstantiateUI(this.categoryLinePrefab, this.rootListContainer, true);
      AllResourcesScreen.CategoryRow categoryRow = new AllResourcesScreen.CategoryRow(categoryTag, gameObject);
      ((TMP_Text) gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel")).SetText(categoryTag.ProperNameStripLink());
      this.categoryRows.Add(categoryTag, categoryRow);
      this.currentlyDisplayedRows.Add(categoryTag, true);
      this.units.Add(categoryTag, unit);
      GraphBase component = ((Component) categoryRow.sparkLayer).GetComponent<GraphBase>();
      component.axis_x.min_value = 0.0f;
      component.axis_x.max_value = 600f;
      component.axis_x.guide_frequency = 120f;
      component.RefreshGuides();
    }
    GameObject container = this.categoryRows[categoryTag].FoldOutPanel.container;
    foreach (Tag tag in DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(categoryTag))
    {
      if (!this.resourceRows.ContainsKey(tag))
      {
        GameObject gameObject = Util.KInstantiateUI(this.resourceLinePrefab, container, true);
        HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
        Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) tag);
        component.GetReference<Image>("Icon").sprite = uiSprite.first;
        ((Graphic) component.GetReference<Image>("Icon")).color = uiSprite.second;
        ((TMP_Text) component.GetReference<LocText>("NameLabel")).SetText(tag.ProperNameStripLink());
        Tag targetTag = tag;
        MultiToggle pinToggle = component.GetReference<MultiToggle>("PinToggle");
        pinToggle.onClick += (System.Action) (() =>
        {
          if (ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(targetTag))
          {
            ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Remove(targetTag);
          }
          else
          {
            ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Add(targetTag);
            if (DiscoveredResources.Instance.newDiscoveries.ContainsKey(targetTag))
              DiscoveredResources.Instance.newDiscoveries.Remove(targetTag);
          }
          this.RefreshPinnedState(targetTag);
          pinToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(targetTag) ? 1 : 0);
        });
        gameObject.GetComponent<MultiToggle>().onClick = pinToggle.onClick;
        MultiToggle notifyToggle = component.GetReference<MultiToggle>("NotificationToggle");
        notifyToggle.onClick += (System.Action) (() =>
        {
          if (ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(targetTag))
            ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Remove(targetTag);
          else
            ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Add(targetTag);
          this.RefreshPinnedState(targetTag);
          notifyToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(targetTag) ? 1 : 0);
        });
        ((Component) component.GetReference<SparkLayer>("Chart")).GetComponent<GraphBase>().axis_x.min_value = 0.0f;
        ((Component) component.GetReference<SparkLayer>("Chart")).GetComponent<GraphBase>().axis_x.max_value = 600f;
        ((Component) component.GetReference<SparkLayer>("Chart")).GetComponent<GraphBase>().axis_x.guide_frequency = 120f;
        ((Component) component.GetReference<SparkLayer>("Chart")).GetComponent<GraphBase>().RefreshGuides();
        AllResourcesScreen.ResourceRow resourceRow = new AllResourcesScreen.ResourceRow(tag, gameObject);
        this.resourceRows.Add(tag, resourceRow);
        this.currentlyDisplayedRows.Add(tag, true);
        if (this.units.ContainsKey(tag))
          Debug.LogError((object) ("Trying to add " + tag.ToString() + ":UnitType " + this.units[tag].ToString() + " but units dictionary already has key " + tag.ToString() + " with unit type:" + unit.ToString()));
        else
          this.units.Add(tag, unit);
      }
    }
  }

  private void FilterRowBySearch(Tag tag, string filter) => this.currentlyDisplayedRows[tag] = this.PassesSearchFilter(tag, filter);

  private void SearchFilter(string search)
  {
    foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> resourceRow in this.resourceRows)
      this.FilterRowBySearch(resourceRow.Key, search);
    foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> categoryRow in this.categoryRows)
    {
      if (this.PassesSearchFilter(categoryRow.Key, search))
      {
        this.currentlyDisplayedRows[categoryRow.Key] = true;
        foreach (Tag key in DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(categoryRow.Key))
        {
          if (this.currentlyDisplayedRows.ContainsKey(key))
            this.currentlyDisplayedRows[key] = true;
        }
      }
      else
        this.currentlyDisplayedRows[categoryRow.Key] = false;
    }
    this.EnableCategoriesByActiveChildren();
    this.SetRowsActive();
  }

  private bool PassesSearchFilter(Tag tag, string filter)
  {
    filter = filter.ToUpper();
    string upper = tag.ProperName().ToUpper();
    return !(filter != "") || upper.Contains(filter) || ((Tag) ref tag).Name.ToUpper().Contains(filter);
  }

  private void EnableCategoriesByActiveChildren()
  {
    foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> categoryRow in this.categoryRows)
    {
      if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(categoryRow.Key).Count == 0)
      {
        this.currentlyDisplayedRows[categoryRow.Key] = false;
      }
      else
      {
        GameObject container = categoryRow.Value.GameObject.GetComponent<FoldOutPanel>().container;
        foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> resourceRow in this.resourceRows)
        {
          if (!Object.op_Inequality((Object) ((Component) resourceRow.Value.GameObject.transform.parent).gameObject, (Object) container))
            this.currentlyDisplayedRows[categoryRow.Key] = this.currentlyDisplayedRows[categoryRow.Key] || this.currentlyDisplayedRows[resourceRow.Key];
        }
      }
    }
  }

  private void RefreshPinnedState(Tag tag)
  {
    this.resourceRows[tag].notificiationToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.notifyResources.Contains(tag) ? 1 : 0);
    this.resourceRows[tag].pinToggle.ChangeState(ClusterManager.Instance.activeWorld.worldInventory.pinnedResources.Contains(tag) ? 1 : 0);
  }

  public void RefreshRows()
  {
    WorldInventory worldInventory1 = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
    this.EnableCategoriesByActiveChildren();
    this.SetRowsActive();
    if (!this.allowRefresh)
      return;
    foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> categoryRow in this.categoryRows)
    {
      if (categoryRow.Value.GameObject.activeInHierarchy)
      {
        float amount = worldInventory1.GetAmount(categoryRow.Key, false);
        float totalAmount = worldInventory1.GetTotalAmount(categoryRow.Key, false);
        if (!worldInventory1.HasValidCount)
        {
          ((TMP_Text) categoryRow.Value.availableLabel).SetText((string) STRINGS.UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
          ((TMP_Text) categoryRow.Value.totalLabel).SetText((string) STRINGS.UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
          ((TMP_Text) categoryRow.Value.reservedLabel).SetText((string) STRINGS.UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
        }
        else
        {
          switch (this.units[categoryRow.Key])
          {
            case GameUtil.MeasureUnit.mass:
              if (categoryRow.Value.CheckAvailableAmountChanged(amount, true))
                ((TMP_Text) categoryRow.Value.availableLabel).SetText(GameUtil.GetFormattedMass(amount));
              if (categoryRow.Value.CheckTotalResourceAmountChanged(totalAmount, true))
                ((TMP_Text) categoryRow.Value.totalLabel).SetText(GameUtil.GetFormattedMass(totalAmount));
              if (categoryRow.Value.CheckReservedResourceAmountChanged(totalAmount - amount, true))
              {
                ((TMP_Text) categoryRow.Value.reservedLabel).SetText(GameUtil.GetFormattedMass(totalAmount - amount));
                continue;
              }
              continue;
            case GameUtil.MeasureUnit.kcal:
              float calories = RationTracker.Get().CountRations((Dictionary<string, float>) null, ClusterManager.Instance.activeWorld.worldInventory);
              if (categoryRow.Value.CheckAvailableAmountChanged(amount, true))
                ((TMP_Text) categoryRow.Value.availableLabel).SetText(GameUtil.GetFormattedCalories(calories));
              if (categoryRow.Value.CheckTotalResourceAmountChanged(totalAmount, true))
                ((TMP_Text) categoryRow.Value.totalLabel).SetText(GameUtil.GetFormattedCalories(totalAmount));
              if (categoryRow.Value.CheckReservedResourceAmountChanged(totalAmount - amount, true))
              {
                ((TMP_Text) categoryRow.Value.reservedLabel).SetText(GameUtil.GetFormattedCalories(totalAmount - amount));
                continue;
              }
              continue;
            case GameUtil.MeasureUnit.quantity:
              if (categoryRow.Value.CheckAvailableAmountChanged(amount, true))
                ((TMP_Text) categoryRow.Value.availableLabel).SetText(GameUtil.GetFormattedUnits(amount));
              if (categoryRow.Value.CheckTotalResourceAmountChanged(totalAmount, true))
                ((TMP_Text) categoryRow.Value.totalLabel).SetText(GameUtil.GetFormattedUnits(totalAmount));
              if (categoryRow.Value.CheckReservedResourceAmountChanged(totalAmount - amount, true))
              {
                ((TMP_Text) categoryRow.Value.reservedLabel).SetText(GameUtil.GetFormattedUnits(totalAmount - amount));
                continue;
              }
              continue;
            default:
              continue;
          }
        }
      }
    }
    foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> resourceRow in this.resourceRows)
    {
      if (resourceRow.Value.GameObject.activeInHierarchy)
      {
        float amount = worldInventory1.GetAmount(resourceRow.Key, false);
        float totalAmount = worldInventory1.GetTotalAmount(resourceRow.Key, false);
        if (!worldInventory1.HasValidCount)
        {
          ((TMP_Text) resourceRow.Value.availableLabel).SetText((string) STRINGS.UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
          ((TMP_Text) resourceRow.Value.totalLabel).SetText((string) STRINGS.UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
          ((TMP_Text) resourceRow.Value.reservedLabel).SetText((string) STRINGS.UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
        }
        else
        {
          switch (this.units[resourceRow.Key])
          {
            case GameUtil.MeasureUnit.mass:
              if (resourceRow.Value.CheckAvailableAmountChanged(amount, true))
                ((TMP_Text) resourceRow.Value.availableLabel).SetText(GameUtil.GetFormattedMass(amount));
              if (resourceRow.Value.CheckTotalResourceAmountChanged(totalAmount, true))
                ((TMP_Text) resourceRow.Value.totalLabel).SetText(GameUtil.GetFormattedMass(totalAmount));
              if (resourceRow.Value.CheckReservedResourceAmountChanged(totalAmount - amount, true))
              {
                ((TMP_Text) resourceRow.Value.reservedLabel).SetText(GameUtil.GetFormattedMass(totalAmount - amount));
                break;
              }
              break;
            case GameUtil.MeasureUnit.kcal:
              RationTracker rationTracker = RationTracker.Get();
              Tag key = resourceRow.Key;
              string name = ((Tag) ref key).Name;
              WorldInventory worldInventory2 = ClusterManager.Instance.activeWorld.worldInventory;
              float num = rationTracker.CountRationsByFoodType(name, worldInventory2);
              if (resourceRow.Value.CheckAvailableAmountChanged(num, true))
                ((TMP_Text) resourceRow.Value.availableLabel).SetText(GameUtil.GetFormattedCalories(num));
              if (resourceRow.Value.CheckTotalResourceAmountChanged(totalAmount, true))
                ((TMP_Text) resourceRow.Value.totalLabel).SetText(GameUtil.GetFormattedCalories(totalAmount));
              if (resourceRow.Value.CheckReservedResourceAmountChanged(totalAmount - amount, true))
              {
                ((TMP_Text) resourceRow.Value.reservedLabel).SetText(GameUtil.GetFormattedCalories(totalAmount - amount));
                break;
              }
              break;
            case GameUtil.MeasureUnit.quantity:
              if (resourceRow.Value.CheckAvailableAmountChanged(amount, true))
                ((TMP_Text) resourceRow.Value.availableLabel).SetText(GameUtil.GetFormattedUnits(amount));
              if (resourceRow.Value.CheckTotalResourceAmountChanged(totalAmount, true))
                ((TMP_Text) resourceRow.Value.totalLabel).SetText(GameUtil.GetFormattedUnits(totalAmount));
              if (resourceRow.Value.CheckReservedResourceAmountChanged(totalAmount - amount, true))
              {
                ((TMP_Text) resourceRow.Value.reservedLabel).SetText(GameUtil.GetFormattedUnits(totalAmount - amount));
                break;
              }
              break;
          }
        }
        this.RefreshPinnedState(resourceRow.Key);
      }
    }
  }

  public int UniqueResourceRowCount() => this.resourceRows.Count;

  private void RefreshCharts()
  {
    float time = GameClock.Instance.GetTime();
    float periodLength = 3000f;
    foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> categoryRow in this.categoryRows)
    {
      ResourceTracker resourceStatistic = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, categoryRow.Key);
      if (resourceStatistic != null)
      {
        SparkLayer sparkLayer = categoryRow.Value.sparkLayer;
        Tuple<float, float>[] data = resourceStatistic.ChartableData(periodLength);
        if (data.Length != 0)
          sparkLayer.graph.axis_x.max_value = data[data.Length - 1].first;
        else
          sparkLayer.graph.axis_x.max_value = 0.0f;
        sparkLayer.graph.axis_x.min_value = time - periodLength;
        sparkLayer.RefreshLine(data, "resourceAmount");
      }
      else
        DebugUtil.DevLogError("DevError: No tracker found for resource category " + categoryRow.Key.ToString());
    }
    foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> resourceRow in this.resourceRows)
    {
      if (resourceRow.Value.GameObject.activeInHierarchy)
      {
        ResourceTracker resourceStatistic = TrackerTool.Instance.GetResourceStatistic(ClusterManager.Instance.activeWorldId, resourceRow.Key);
        if (resourceStatistic != null)
        {
          SparkLayer sparkLayer = resourceRow.Value.sparkLayer;
          Tuple<float, float>[] data = resourceStatistic.ChartableData(periodLength);
          if (data.Length != 0)
            sparkLayer.graph.axis_x.max_value = data[data.Length - 1].first;
          else
            sparkLayer.graph.axis_x.max_value = 0.0f;
          sparkLayer.graph.axis_x.min_value = time - periodLength;
          sparkLayer.RefreshLine(data, "resourceAmount");
        }
        else
          DebugUtil.DevLogError("DevError: No tracker found for resource " + resourceRow.Key.ToString());
      }
    }
  }

  private void SetRowsActive()
  {
    foreach (KeyValuePair<Tag, AllResourcesScreen.CategoryRow> categoryRow in this.categoryRows)
    {
      if (categoryRow.Value.GameObject.activeSelf != this.currentlyDisplayedRows[categoryRow.Key])
        categoryRow.Value.GameObject.SetActive(this.currentlyDisplayedRows[categoryRow.Key]);
    }
    foreach (KeyValuePair<Tag, AllResourcesScreen.ResourceRow> resourceRow in this.resourceRows)
    {
      if (resourceRow.Value.GameObject.activeSelf != this.currentlyDisplayedRows[resourceRow.Key])
        resourceRow.Value.GameObject.SetActive(this.currentlyDisplayedRows[resourceRow.Key]);
    }
  }

  public void Sim4000ms(float dt)
  {
    if (!this.currentlyShown)
      return;
    this.RefreshCharts();
  }

  public void Sim1000ms(float dt)
  {
    if (!this.currentlyShown)
      return;
    this.RefreshRows();
  }

  private class ScreenRowBase
  {
    public LocText availableLabel;
    public LocText totalLabel;
    public LocText reservedLabel;
    public SparkLayer sparkLayer;
    private float oldAvailableResourceAmount = -1f;
    private float oldTotalResourceAmount = -1f;
    private float oldReserverResourceAmount = -1f;

    public ScreenRowBase(Tag tag, GameObject gameObject)
    {
      this.Tag = tag;
      this.GameObject = gameObject;
      HierarchyReferences component = this.GameObject.GetComponent<HierarchyReferences>();
      this.availableLabel = component.GetReference<LocText>("AvailableLabel");
      this.totalLabel = component.GetReference<LocText>("TotalLabel");
      this.reservedLabel = component.GetReference<LocText>("ReservedLabel");
      this.sparkLayer = component.GetReference<SparkLayer>("Chart");
    }

    public Tag Tag { get; private set; }

    public GameObject GameObject { get; private set; }

    public bool CheckAvailableAmountChanged(float newAvailableResourceAmount, bool updateIfTrue)
    {
      int num = (double) newAvailableResourceAmount != (double) this.oldAvailableResourceAmount ? 1 : 0;
      if ((num & (updateIfTrue ? 1 : 0)) == 0)
        return num != 0;
      this.oldAvailableResourceAmount = newAvailableResourceAmount;
      return num != 0;
    }

    public bool CheckTotalResourceAmountChanged(float newTotalResourceAmount, bool updateIfTrue)
    {
      int num = (double) newTotalResourceAmount != (double) this.oldTotalResourceAmount ? 1 : 0;
      if ((num & (updateIfTrue ? 1 : 0)) == 0)
        return num != 0;
      this.oldTotalResourceAmount = newTotalResourceAmount;
      return num != 0;
    }

    public bool CheckReservedResourceAmountChanged(
      float newReservedResourceAmount,
      bool updateIfTrue)
    {
      int num = (double) newReservedResourceAmount != (double) this.oldReserverResourceAmount ? 1 : 0;
      if ((num & (updateIfTrue ? 1 : 0)) == 0)
        return num != 0;
      this.oldReserverResourceAmount = newReservedResourceAmount;
      return num != 0;
    }
  }

  private class CategoryRow : AllResourcesScreen.ScreenRowBase
  {
    public CategoryRow(Tag tag, GameObject gameObject)
      : base(tag, gameObject)
    {
      this.FoldOutPanel = this.GameObject.GetComponent<FoldOutPanel>();
    }

    public FoldOutPanel FoldOutPanel { get; private set; }
  }

  private class ResourceRow : AllResourcesScreen.ScreenRowBase
  {
    public MultiToggle notificiationToggle;
    public MultiToggle pinToggle;

    public ResourceRow(Tag tag, GameObject gameObject)
      : base(tag, gameObject)
    {
      HierarchyReferences component = this.GameObject.GetComponent<HierarchyReferences>();
      this.notificiationToggle = component.GetReference<MultiToggle>("NotificationToggle");
      this.pinToggle = component.GetReference<MultiToggle>("PinToggle");
    }
  }
}
