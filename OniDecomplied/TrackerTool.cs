// Decompiled with JetBrains decompiler
// Type: TrackerTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackerTool : KMonoBehaviour
{
  public static TrackerTool Instance;
  private List<WorldTracker> worldTrackers = new List<WorldTracker>();
  private Dictionary<MinionIdentity, List<MinionTracker>> minionTrackers = new Dictionary<MinionIdentity, List<MinionTracker>>();
  private int updatingWorldTracker;
  private int updatingMinionTracker;
  public bool trackerActive = true;
  private int numUpdatesPerFrame = 50;

  protected virtual void OnSpawn()
  {
    TrackerTool.Instance = this;
    base.OnSpawn();
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
      this.AddNewWorldTrackers(worldContainer.id);
    foreach (MinionIdentity liveMinionIdentity in Components.LiveMinionIdentities)
      this.AddMinionTrackers(liveMinionIdentity);
    Components.LiveMinionIdentities.OnAdd += new Action<MinionIdentity>(this.AddMinionTrackers);
    ClusterManager.Instance.Subscribe(-1280433810, new Action<object>(this.Refresh));
    ClusterManager.Instance.Subscribe(-1078710002, new Action<object>(this.RemoveWorld));
  }

  protected virtual void OnForcedCleanUp()
  {
    TrackerTool.Instance = (TrackerTool) null;
    base.OnForcedCleanUp();
  }

  private void AddMinionTrackers(MinionIdentity identity)
  {
    this.minionTrackers.Add(identity, new List<MinionTracker>());
    identity.Subscribe(1969584890, (Action<object>) (data => this.minionTrackers.Remove(identity)));
  }

  private void Refresh(object data) => this.AddNewWorldTrackers((int) data);

  private void RemoveWorld(object data)
  {
    int world_id = (int) data;
    this.worldTrackers.RemoveAll((Predicate<WorldTracker>) (match => match.WorldID == world_id));
  }

  public bool IsRocketInterior(int worldID) => ClusterManager.Instance.GetWorld(worldID).IsModuleInterior;

  private void AddNewWorldTrackers(int worldID)
  {
    this.worldTrackers.Add((WorldTracker) new StressTracker(worldID));
    this.worldTrackers.Add((WorldTracker) new KCalTracker(worldID));
    this.worldTrackers.Add((WorldTracker) new IdleTracker(worldID));
    this.worldTrackers.Add((WorldTracker) new BreathabilityTracker(worldID));
    this.worldTrackers.Add((WorldTracker) new PowerUseTracker(worldID));
    this.worldTrackers.Add((WorldTracker) new BatteryTracker(worldID));
    this.worldTrackers.Add((WorldTracker) new CropTracker(worldID));
    this.worldTrackers.Add((WorldTracker) new WorkingToiletTracker(worldID));
    this.worldTrackers.Add((WorldTracker) new RadiationTracker(worldID));
    if (ClusterManager.Instance.GetWorld(worldID).IsModuleInterior)
    {
      this.worldTrackers.Add((WorldTracker) new RocketFuelTracker(worldID));
      this.worldTrackers.Add((WorldTracker) new RocketOxidizerTracker(worldID));
    }
    for (int index = 0; index < ((ResourceSet) Db.Get().ChoreGroups).Count; ++index)
    {
      this.worldTrackers.Add((WorldTracker) new WorkTimeTracker(worldID, Db.Get().ChoreGroups[index]));
      this.worldTrackers.Add((WorldTracker) new ChoreCountTracker(worldID, Db.Get().ChoreGroups[index]));
    }
    this.worldTrackers.Add((WorldTracker) new AllChoresCountTracker(worldID));
    this.worldTrackers.Add((WorldTracker) new AllWorkTimeTracker(worldID));
    foreach (Tag calorieCategory in GameTags.CalorieCategories)
    {
      this.worldTrackers.Add((WorldTracker) new ResourceTracker(worldID, calorieCategory));
      foreach (GameObject gameObject in Assets.GetPrefabsWithTag(calorieCategory))
        this.AddResourceTracker(worldID, gameObject.GetComponent<KPrefabID>().PrefabTag);
    }
    foreach (Tag unitCategory in GameTags.UnitCategories)
    {
      this.worldTrackers.Add((WorldTracker) new ResourceTracker(worldID, unitCategory));
      foreach (GameObject gameObject in Assets.GetPrefabsWithTag(unitCategory))
        this.AddResourceTracker(worldID, gameObject.GetComponent<KPrefabID>().PrefabTag);
    }
    foreach (Tag materialCategory in GameTags.MaterialCategories)
    {
      this.worldTrackers.Add((WorldTracker) new ResourceTracker(worldID, materialCategory));
      foreach (GameObject gameObject in Assets.GetPrefabsWithTag(materialCategory))
        this.AddResourceTracker(worldID, gameObject.GetComponent<KPrefabID>().PrefabTag);
    }
    foreach (Tag otherEntityTag in GameTags.OtherEntityTags)
    {
      this.worldTrackers.Add((WorldTracker) new ResourceTracker(worldID, otherEntityTag));
      foreach (GameObject gameObject in Assets.GetPrefabsWithTag(otherEntityTag))
        this.AddResourceTracker(worldID, gameObject.GetComponent<KPrefabID>().PrefabTag);
    }
    foreach (GameObject gameObject in Assets.GetPrefabsWithTag(GameTags.CookingIngredient))
      this.AddResourceTracker(worldID, gameObject.GetComponent<KPrefabID>().PrefabTag);
    foreach (EdiblesManager.FoodInfo allFoodType in EdiblesManager.GetAllFoodTypes())
      this.AddResourceTracker(worldID, Tag.op_Implicit(allFoodType.Id));
    foreach (Element element in ElementLoader.elements)
      this.AddResourceTracker(worldID, element.tag);
  }

  private void AddResourceTracker(int worldID, Tag tag)
  {
    if (this.worldTrackers.Find((Predicate<WorldTracker>) (match => match is ResourceTracker && match.WorldID == worldID && Tag.op_Equality(((ResourceTracker) match).tag, tag))) != null)
      return;
    this.worldTrackers.Add((WorldTracker) new ResourceTracker(worldID, tag));
  }

  public ResourceTracker GetResourceStatistic(int worldID, Tag tag) => (ResourceTracker) this.worldTrackers.Find((Predicate<WorldTracker>) (match => match is ResourceTracker && match.WorldID == worldID && Tag.op_Equality(((ResourceTracker) match).tag, tag)));

  public WorldTracker GetWorldTracker<T>(int worldID) where T : WorldTracker => this.worldTrackers.Find((Predicate<WorldTracker>) (match => match is T obj && obj.WorldID == worldID));

  public ChoreCountTracker GetChoreGroupTracker(int worldID, ChoreGroup choreGroup) => (ChoreCountTracker) this.worldTrackers.Find((Predicate<WorldTracker>) (match => match is ChoreCountTracker && match.WorldID == worldID && ((ChoreCountTracker) match).choreGroup == choreGroup));

  public WorkTimeTracker GetWorkTimeTracker(int worldID, ChoreGroup choreGroup) => (WorkTimeTracker) this.worldTrackers.Find((Predicate<WorldTracker>) (match => match is WorkTimeTracker && match.WorldID == worldID && ((WorkTimeTracker) match).choreGroup == choreGroup));

  public MinionTracker GetMinionTracker<T>(MinionIdentity identity) where T : MinionTracker => this.minionTrackers[identity].Find((Predicate<MinionTracker>) (match => match is T));

  public void Update()
  {
    if (SpeedControlScreen.Instance.IsPaused || !this.trackerActive)
      return;
    if (this.minionTrackers.Count > 0)
    {
      ++this.updatingMinionTracker;
      if (this.updatingMinionTracker >= this.minionTrackers.Count)
        this.updatingMinionTracker = 0;
      KeyValuePair<MinionIdentity, List<MinionTracker>> keyValuePair = this.minionTrackers.ElementAt<KeyValuePair<MinionIdentity, List<MinionTracker>>>(this.updatingMinionTracker);
      for (int index = 0; index < keyValuePair.Value.Count; ++index)
        keyValuePair.Value[index].UpdateData();
    }
    if (this.worldTrackers.Count <= 0)
      return;
    for (int index = 0; index < this.numUpdatesPerFrame; ++index)
    {
      ++this.updatingWorldTracker;
      if (this.updatingWorldTracker >= this.worldTrackers.Count)
        this.updatingWorldTracker = 0;
      this.worldTrackers[this.updatingWorldTracker].UpdateData();
    }
  }
}
