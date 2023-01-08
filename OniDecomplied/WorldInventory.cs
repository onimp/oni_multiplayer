// Decompiled with JetBrains decompiler
// Type: WorldInventory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/WorldInventory")]
public class WorldInventory : KMonoBehaviour, ISaveLoadable
{
  private WorldContainer m_worldContainer;
  [Serialize]
  public List<Tag> pinnedResources = new List<Tag>();
  [Serialize]
  public List<Tag> notifyResources = new List<Tag>();
  private Dictionary<Tag, HashSet<Pickupable>> Inventory = new Dictionary<Tag, HashSet<Pickupable>>();
  private MinionGroupProber Prober;
  private Dictionary<Tag, float> accessibleAmounts = new Dictionary<Tag, float>();
  private bool hasValidCount;
  private static readonly EventSystem.IntraObjectHandler<WorldInventory> OnNewDayDelegate = new EventSystem.IntraObjectHandler<WorldInventory>((Action<WorldInventory, object>) ((component, data) => component.GenerateInventoryReport(data)));
  private int accessibleUpdateIndex;
  private bool firstUpdate = true;

  public WorldContainer WorldContainer
  {
    get
    {
      if (Object.op_Equality((Object) this.m_worldContainer, (Object) null))
        this.m_worldContainer = ((Component) this).GetComponent<WorldContainer>();
      return this.m_worldContainer;
    }
  }

  public bool HasValidCount => this.hasValidCount;

  private int worldId
  {
    get
    {
      WorldContainer worldContainer = this.WorldContainer;
      return !Object.op_Inequality((Object) worldContainer, (Object) null) ? -1 : worldContainer.id;
    }
  }

  protected virtual void OnPrefabInit()
  {
    this.Subscribe(((Component) Game.Instance).gameObject, -1588644844, new Action<object>(this.OnAddedFetchable));
    this.Subscribe(((Component) Game.Instance).gameObject, -1491270284, new Action<object>(this.OnRemovedFetchable));
    this.Subscribe<WorldInventory>(631075836, WorldInventory.OnNewDayDelegate);
    this.m_worldContainer = ((Component) this).GetComponent<WorldContainer>();
  }

  protected virtual void OnCleanUp()
  {
    this.Unsubscribe(((Component) Game.Instance).gameObject, -1588644844, new Action<object>(this.OnAddedFetchable));
    this.Unsubscribe(((Component) Game.Instance).gameObject, -1491270284, new Action<object>(this.OnRemovedFetchable));
    base.OnCleanUp();
  }

  private void GenerateInventoryReport(object data)
  {
    int num1 = 0;
    int num2 = 0;
    foreach (Brain worldItem in Components.Brains.GetWorldItems(this.worldId))
    {
      CreatureBrain cmp = worldItem as CreatureBrain;
      if (Object.op_Inequality((Object) cmp, (Object) null))
      {
        if (((Component) cmp).HasTag(GameTags.Creatures.Wild))
        {
          ++num1;
          ReportManager.Instance.ReportValue(ReportManager.ReportType.WildCritters, 1f, ((Component) cmp).GetProperName(), ((Component) cmp).GetProperName());
        }
        else
        {
          ++num2;
          ReportManager.Instance.ReportValue(ReportManager.ReportType.DomesticatedCritters, 1f, ((Component) cmp).GetProperName(), ((Component) cmp).GetProperName());
        }
      }
    }
    if (DlcManager.IsExpansion1Active())
    {
      WorldContainer component1 = ((Component) this).GetComponent<WorldContainer>();
      if (!Object.op_Inequality((Object) component1, (Object) null) || !component1.IsModuleInterior)
        return;
      Clustercraft component2 = ((Component) component1).GetComponent<ClusterGridEntity>() as Clustercraft;
      if (!Object.op_Inequality((Object) component2, (Object) null) || component2.Status == Clustercraft.CraftStatus.Grounded)
        return;
      ReportManager.Instance.ReportValue(ReportManager.ReportType.RocketsInFlight, 1f, component2.Name);
    }
    else
    {
      foreach (Spacecraft spacecraft in SpacecraftManager.instance.GetSpacecraft())
      {
        if (spacecraft.state != Spacecraft.MissionState.Grounded && spacecraft.state != Spacecraft.MissionState.Destroyed)
          ReportManager.Instance.ReportValue(ReportManager.ReportType.RocketsInFlight, 1f, spacecraft.rocketName);
      }
    }
  }

  protected virtual void OnSpawn()
  {
    this.Prober = MinionGroupProber.Get();
    ((MonoBehaviour) this).StartCoroutine(this.InitialRefresh());
  }

  private IEnumerator InitialRefresh()
  {
    for (int i = 0; i < 1; ++i)
      yield return (object) null;
    for (int idx = 0; idx < Components.Pickupables.Count; ++idx)
    {
      Pickupable pickupable = Components.Pickupables[idx];
      if (Object.op_Inequality((Object) pickupable, (Object) null))
        ((Component) pickupable).GetSMI<ReachabilityMonitor.Instance>()?.UpdateReachability();
    }
  }

  public bool IsReachable(Pickupable pickupable) => this.Prober.IsReachable((Workable) pickupable);

  public float GetTotalAmount(Tag tag, bool includeRelatedWorlds)
  {
    float totalAmount = 0.0f;
    this.accessibleAmounts.TryGetValue(tag, out totalAmount);
    return totalAmount;
  }

  public ICollection<Pickupable> GetPickupables(Tag tag, bool includeRelatedWorlds = false)
  {
    if (includeRelatedWorlds)
      return (ICollection<Pickupable>) ClusterUtil.GetPickupablesFromRelatedWorlds(this, tag);
    HashSet<Pickupable> pickupables = (HashSet<Pickupable>) null;
    this.Inventory.TryGetValue(tag, out pickupables);
    return (ICollection<Pickupable>) pickupables;
  }

  public List<Pickupable> CreatePickupablesList(Tag tag)
  {
    HashSet<Pickupable> source = (HashSet<Pickupable>) null;
    this.Inventory.TryGetValue(tag, out source);
    return source == null ? (List<Pickupable>) null : source.ToList<Pickupable>();
  }

  public float GetAmount(Tag tag, bool includeRelatedWorlds) => Mathf.Max(includeRelatedWorlds ? ClusterUtil.GetAmountFromRelatedWorlds(this, tag) : this.GetTotalAmount(tag, includeRelatedWorlds) - MaterialNeeds.GetAmount(tag, this.worldId, includeRelatedWorlds), 0.0f);

  public int GetCountWithAdditionalTag(Tag tag, Tag additionalTag, bool includeRelatedWorlds = false)
  {
    ICollection<Pickupable> pickupables = includeRelatedWorlds ? (ICollection<Pickupable>) ClusterUtil.GetPickupablesFromRelatedWorlds(this, tag) : this.GetPickupables(tag);
    int withAdditionalTag = 0;
    if (pickupables != null)
    {
      if (((Tag) ref additionalTag).IsValid)
      {
        foreach (Component cmp in (IEnumerable<Pickupable>) pickupables)
        {
          if (cmp.HasTag(additionalTag))
            ++withAdditionalTag;
        }
      }
      else
        withAdditionalTag = pickupables.Count;
    }
    return withAdditionalTag;
  }

  public float GetAmountWithoutTag(Tag tag, bool includeRelatedWorlds = false, Tag[] forbiddenTags = null)
  {
    if (forbiddenTags == null)
      return this.GetAmount(tag, includeRelatedWorlds);
    float amountWithoutTag = 0.0f;
    ICollection<Pickupable> pickupables = includeRelatedWorlds ? (ICollection<Pickupable>) ClusterUtil.GetPickupablesFromRelatedWorlds(this, tag) : this.GetPickupables(tag);
    if (pickupables != null)
    {
      foreach (Pickupable cmp in (IEnumerable<Pickupable>) pickupables)
      {
        if (Object.op_Inequality((Object) cmp, (Object) null) && !((Component) cmp).HasTag(GameTags.StoredPrivate) && !((Component) cmp).HasAnyTags(forbiddenTags))
          amountWithoutTag += cmp.TotalAmount;
      }
    }
    return amountWithoutTag;
  }

  private void Update()
  {
    int num1 = 0;
    Dictionary<Tag, HashSet<Pickupable>>.Enumerator enumerator = this.Inventory.GetEnumerator();
    int worldId = this.worldId;
    while (enumerator.MoveNext())
    {
      KeyValuePair<Tag, HashSet<Pickupable>> current = enumerator.Current;
      if (num1 == this.accessibleUpdateIndex || this.firstUpdate)
      {
        Tag key = current.Key;
        HashSet<Pickupable> pickupableSet = current.Value;
        float num2 = 0.0f;
        foreach (Pickupable pickupable in (IEnumerable<Pickupable>) pickupableSet)
        {
          if (Object.op_Inequality((Object) pickupable, (Object) null) && pickupable.GetMyWorldId() == worldId && !((Component) pickupable).HasTag(GameTags.StoredPrivate))
            num2 += pickupable.TotalAmount;
        }
        if (!this.hasValidCount && this.accessibleUpdateIndex + 1 >= this.Inventory.Count)
        {
          this.hasValidCount = true;
          if (this.worldId == ClusterManager.Instance.activeWorldId)
          {
            this.hasValidCount = true;
            PinnedResourcesPanel.Instance.ClearExcessiveNewItems();
            PinnedResourcesPanel.Instance.Refresh();
          }
        }
        this.accessibleAmounts[key] = num2;
        this.accessibleUpdateIndex = (this.accessibleUpdateIndex + 1) % this.Inventory.Count;
        break;
      }
      ++num1;
    }
    this.firstUpdate = false;
  }

  protected virtual void OnLoadLevel() => base.OnLoadLevel();

  private void OnAddedFetchable(object data)
  {
    GameObject gameObject = (GameObject) data;
    if (Object.op_Inequality((Object) gameObject.GetComponent<Navigator>(), (Object) null))
      return;
    Pickupable component1 = gameObject.GetComponent<Pickupable>();
    if (component1.GetMyWorldId() != this.worldId)
      return;
    KPrefabID component2 = ((Component) component1).GetComponent<KPrefabID>();
    Tag tag1 = component2.PrefabID();
    if (!this.Inventory.ContainsKey(tag1))
    {
      Tag categoryForEntity = DiscoveredResources.GetCategoryForEntity(component2);
      DebugUtil.DevAssertArgs((((Tag) ref categoryForEntity).IsValid ? 1 : 0) != 0, new object[2]
      {
        (object) ((Object) component1).name,
        (object) "was found by worldinventory but doesn't have a category! Add it to the element definition."
      });
      DiscoveredResources.Instance.Discover(tag1, categoryForEntity);
    }
    HashSet<Pickupable> pickupableSet;
    if (!this.Inventory.TryGetValue(tag1, out pickupableSet))
    {
      pickupableSet = new HashSet<Pickupable>();
      this.Inventory[tag1] = pickupableSet;
    }
    pickupableSet.Add(component1);
    foreach (Tag tag2 in component2.Tags)
    {
      if (!this.Inventory.TryGetValue(tag2, out pickupableSet))
      {
        pickupableSet = new HashSet<Pickupable>();
        this.Inventory[tag2] = pickupableSet;
      }
      pickupableSet.Add(component1);
    }
  }

  private void OnRemovedFetchable(object data)
  {
    Pickupable component = ((GameObject) data).GetComponent<Pickupable>();
    KPrefabID kprefabId = component.KPrefabID;
    HashSet<Pickupable> pickupableSet;
    if (this.Inventory.TryGetValue(kprefabId.PrefabTag, out pickupableSet))
      pickupableSet.Remove(component);
    foreach (Tag tag in kprefabId.Tags)
    {
      if (this.Inventory.TryGetValue(tag, out pickupableSet))
        pickupableSet.Remove(component);
    }
  }

  public Dictionary<Tag, float> GetAccessibleAmounts() => this.accessibleAmounts;
}
