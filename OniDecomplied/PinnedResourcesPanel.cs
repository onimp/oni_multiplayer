// Decompiled with JetBrains decompiler
// Type: PinnedResourcesPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PinnedResourcesPanel : KScreen, IRender1000ms
{
  public GameObject linePrefab;
  public GameObject rowContainer;
  public MultiToggle headerButton;
  public MultiToggle clearNewButton;
  public KButton clearAllButton;
  public MultiToggle seeAllButton;
  private LocText seeAllLabel;
  private QuickLayout rowContainerLayout;
  private Dictionary<Tag, PinnedResourcesPanel.PinnedResourceRow> rows = new Dictionary<Tag, PinnedResourcesPanel.PinnedResourceRow>();
  public static PinnedResourcesPanel Instance;
  private int clickIdx;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.rowContainerLayout = this.rowContainer.GetComponent<QuickLayout>();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    PinnedResourcesPanel.Instance = this;
    this.Populate();
    Game.Instance.Subscribe(1983128072, new Action<object>(this.Populate));
    ((Component) this.headerButton).GetComponent<MultiToggle>().onClick += (System.Action) (() => this.Refresh());
    ((Component) this.seeAllButton).GetComponent<MultiToggle>().onClick += (System.Action) (() => AllResourcesScreen.Instance.Show(!((Component) AllResourcesScreen.Instance).gameObject.activeSelf));
    this.seeAllLabel = ((Component) this.seeAllButton).GetComponentInChildren<LocText>();
    ((Component) this.clearNewButton).GetComponent<MultiToggle>().onClick += (System.Action) (() => this.ClearAllNew());
    this.clearAllButton.onClick += (System.Action) (() =>
    {
      this.ClearAllNew();
      this.UnPinAll();
      this.Refresh();
    });
    AllResourcesScreen.Instance.Init();
    this.Refresh();
  }

  protected virtual void OnForcedCleanUp()
  {
    PinnedResourcesPanel.Instance = (PinnedResourcesPanel) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  public void ClearExcessiveNewItems()
  {
    if (!DiscoveredResources.Instance.CheckAllDiscoveredAreNew())
      return;
    DiscoveredResources.Instance.newDiscoveries.Clear();
  }

  private void ClearAllNew()
  {
    foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> row in this.rows)
    {
      if (row.Value.gameObject.activeSelf && DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key))
        DiscoveredResources.Instance.newDiscoveries.Remove(row.Key);
    }
  }

  private void UnPinAll()
  {
    WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
    foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> row in this.rows)
      worldInventory.pinnedResources.Remove(row.Key);
  }

  private PinnedResourcesPanel.PinnedResourceRow CreateRow(Tag tag)
  {
    PinnedResourcesPanel.PinnedResourceRow row = new PinnedResourcesPanel.PinnedResourceRow(tag);
    GameObject gameObject = Util.KInstantiateUI(this.linePrefab, this.rowContainer, false);
    row.gameObject = gameObject;
    HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
    row.icon = component.GetReference<Image>("Icon");
    row.nameLabel = component.GetReference<LocText>("NameLabel");
    row.valueLabel = component.GetReference<LocText>("ValueLabel");
    row.pinToggle = component.GetReference<MultiToggle>("PinToggle");
    row.notifyToggle = component.GetReference<MultiToggle>("NotifyToggle");
    row.newLabel = component.GetReference<MultiToggle>("NewLabel");
    Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) tag);
    row.icon.sprite = uiSprite.first;
    ((Graphic) row.icon).color = uiSprite.second;
    ((TMP_Text) row.nameLabel).SetText(tag.ProperNameStripLink());
    row.gameObject.GetComponent<MultiToggle>().onClick += (System.Action) (() =>
    {
      List<Pickupable> pickupablesList = ClusterManager.Instance.activeWorld.worldInventory.CreatePickupablesList(tag);
      if (pickupablesList != null && pickupablesList.Count > 0)
      {
        SelectTool.Instance.SelectAndFocus(pickupablesList[this.clickIdx % pickupablesList.Count].transform.position, ((Component) pickupablesList[this.clickIdx % pickupablesList.Count]).GetComponent<KSelectable>());
        ++this.clickIdx;
      }
      else
        this.clickIdx = 0;
    });
    return row;
  }

  public void Populate(object data = null)
  {
    WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
    foreach (KeyValuePair<Tag, float> newDiscovery in DiscoveredResources.Instance.newDiscoveries)
    {
      if (!this.rows.ContainsKey(newDiscovery.Key) && this.IsDisplayedTag(newDiscovery.Key))
        this.rows.Add(newDiscovery.Key, this.CreateRow(newDiscovery.Key));
    }
    foreach (Tag pinnedResource in worldInventory.pinnedResources)
    {
      if (!this.rows.ContainsKey(pinnedResource))
        this.rows.Add(pinnedResource, this.CreateRow(pinnedResource));
    }
    foreach (Tag notifyResource in worldInventory.notifyResources)
    {
      if (!this.rows.ContainsKey(notifyResource))
        this.rows.Add(notifyResource, this.CreateRow(notifyResource));
    }
    foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> row in this.rows)
    {
      if ((((false ? 1 : (worldInventory.pinnedResources.Contains(row.Key) ? 1 : 0)) != 0 ? 1 : (worldInventory.notifyResources.Contains(row.Key) ? 1 : 0)) != 0 ? 1 : (!DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key) ? 0 : ((double) worldInventory.GetAmount(row.Key, false) > 0.0 ? 1 : 0))) != 0)
      {
        if (!row.Value.gameObject.activeSelf)
          row.Value.gameObject.SetActive(true);
      }
      else if (row.Value.gameObject.activeSelf)
        row.Value.gameObject.SetActive(false);
    }
    foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> row in this.rows)
      ((Component) row.Value.pinToggle).gameObject.SetActive(worldInventory.pinnedResources.Contains(row.Key));
    this.SortRows();
    this.rowContainerLayout.ForceUpdate();
  }

  private void SortRows()
  {
    List<PinnedResourcesPanel.PinnedResourceRow> pinnedResourceRowList = new List<PinnedResourcesPanel.PinnedResourceRow>();
    foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> row in this.rows)
      pinnedResourceRowList.Add(row.Value);
    pinnedResourceRowList.Sort((Comparison<PinnedResourcesPanel.PinnedResourceRow>) ((a, b) => a.SortableNameWithoutLink.CompareTo(b.SortableNameWithoutLink)));
    foreach (PinnedResourcesPanel.PinnedResourceRow pinnedResourceRow in pinnedResourceRowList)
      this.rows[pinnedResourceRow.Tag].gameObject.transform.SetAsLastSibling();
    this.clearNewButton.transform.SetAsLastSibling();
    this.seeAllButton.transform.SetAsLastSibling();
  }

  private bool IsDisplayedTag(Tag tag)
  {
    foreach (TagSet allowDisplayCategory in AllResourcesScreen.Instance.allowDisplayCategories)
    {
      foreach (KeyValuePair<Tag, HashSet<Tag>> resourcesFromTag in DiscoveredResources.Instance.GetDiscoveredResourcesFromTagSet(allowDisplayCategory))
      {
        if (resourcesFromTag.Value.Contains(tag))
          return true;
      }
    }
    return false;
  }

  private void SyncRows()
  {
    WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
    bool flag = false;
    foreach (Tag pinnedResource in worldInventory.pinnedResources)
    {
      if (!this.rows.ContainsKey(pinnedResource))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
    {
      foreach (KeyValuePair<Tag, float> newDiscovery in DiscoveredResources.Instance.newDiscoveries)
      {
        if (!this.rows.ContainsKey(newDiscovery.Key) && this.IsDisplayedTag(newDiscovery.Key))
        {
          flag = true;
          break;
        }
      }
    }
    if (!flag)
    {
      foreach (Tag notifyResource in worldInventory.notifyResources)
      {
        if (!this.rows.ContainsKey(notifyResource))
        {
          flag = true;
          break;
        }
      }
    }
    if (!flag)
    {
      foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> row in this.rows)
      {
        if (((worldInventory.pinnedResources.Contains(row.Key) ? 1 : (worldInventory.notifyResources.Contains(row.Key) ? 1 : 0)) != 0 ? 1 : (!DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key) ? 0 : ((double) worldInventory.GetAmount(row.Key, false) > 0.0 ? 1 : 0))) != (row.Value.gameObject.activeSelf ? 1 : 0))
        {
          flag = true;
          break;
        }
      }
    }
    if (!flag)
      return;
    this.Populate();
  }

  public void Refresh()
  {
    this.SyncRows();
    WorldInventory worldInventory = ClusterManager.Instance.GetWorld(ClusterManager.Instance.activeWorldId).worldInventory;
    bool flag = false;
    foreach (KeyValuePair<Tag, PinnedResourcesPanel.PinnedResourceRow> row in this.rows)
    {
      if (row.Value.gameObject.activeSelf)
      {
        this.RefreshLine(row.Key, worldInventory);
        flag = flag || DiscoveredResources.Instance.newDiscoveries.ContainsKey(row.Key);
      }
    }
    ((Component) this.clearNewButton).gameObject.SetActive(flag);
    ((TMP_Text) this.seeAllLabel).SetText(string.Format((string) STRINGS.UI.RESOURCESCREEN.SEE_ALL, (object) AllResourcesScreen.Instance.UniqueResourceRowCount()));
  }

  private void RefreshLine(Tag tag, WorldInventory inventory, bool initialConfig = false)
  {
    Tag tag1 = tag;
    if (!AllResourcesScreen.Instance.units.ContainsKey(tag))
      AllResourcesScreen.Instance.units.Add(tag, GameUtil.MeasureUnit.quantity);
    if (!inventory.HasValidCount)
    {
      ((TMP_Text) this.rows[tag].valueLabel).SetText((string) STRINGS.UI.ALLRESOURCESSCREEN.FIRST_FRAME_NO_DATA);
    }
    else
    {
      switch (AllResourcesScreen.Instance.units[tag])
      {
        case GameUtil.MeasureUnit.mass:
          float amount1 = inventory.GetAmount(tag1, false);
          if (this.rows[tag].CheckAmountChanged(amount1, true))
          {
            ((TMP_Text) this.rows[tag].valueLabel).SetText(GameUtil.GetFormattedMass(amount1));
            break;
          }
          break;
        case GameUtil.MeasureUnit.kcal:
          float num = RationTracker.Get().CountRationsByFoodType(((Tag) ref tag).Name, ClusterManager.Instance.activeWorld.worldInventory);
          if (this.rows[tag].CheckAmountChanged(num, true))
          {
            ((TMP_Text) this.rows[tag].valueLabel).SetText(GameUtil.GetFormattedCalories(num));
            break;
          }
          break;
        case GameUtil.MeasureUnit.quantity:
          float amount2 = inventory.GetAmount(tag1, false);
          if (this.rows[tag].CheckAmountChanged(amount2, true))
          {
            ((TMP_Text) this.rows[tag].valueLabel).SetText(GameUtil.GetFormattedUnits(amount2));
            break;
          }
          break;
      }
    }
    this.rows[tag].pinToggle.onClick = (System.Action) (() =>
    {
      inventory.pinnedResources.Remove(tag);
      this.SyncRows();
    });
    this.rows[tag].notifyToggle.onClick = (System.Action) (() =>
    {
      inventory.notifyResources.Remove(tag);
      this.SyncRows();
    });
    ((Component) this.rows[tag].newLabel).gameObject.SetActive(DiscoveredResources.Instance.newDiscoveries.ContainsKey(tag));
    this.rows[tag].newLabel.onClick = (System.Action) (() => AllResourcesScreen.Instance.Show(!((Component) AllResourcesScreen.Instance).gameObject.activeSelf));
  }

  public void Render1000ms(float dt)
  {
    if (Object.op_Inequality((Object) this.headerButton, (Object) null) && this.headerButton.CurrentState == 0)
      return;
    this.Refresh();
  }

  public class PinnedResourceRow
  {
    public GameObject gameObject;
    public Image icon;
    public LocText nameLabel;
    public LocText valueLabel;
    public MultiToggle pinToggle;
    public MultiToggle notifyToggle;
    public MultiToggle newLabel;
    private float oldResourceAmount = -1f;

    public PinnedResourceRow(Tag tag)
    {
      this.Tag = tag;
      this.SortableNameWithoutLink = tag.ProperNameStripLink();
    }

    public Tag Tag { get; private set; }

    public string SortableNameWithoutLink { get; private set; }

    public bool CheckAmountChanged(float newResourceAmount, bool updateIfTrue)
    {
      int num = (double) newResourceAmount != (double) this.oldResourceAmount ? 1 : 0;
      if ((num & (updateIfTrue ? 1 : 0)) == 0)
        return num != 0;
      this.oldResourceAmount = newResourceAmount;
      return num != 0;
    }
  }
}
