// Decompiled with JetBrains decompiler
// Type: Research
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Research")]
public class Research : KMonoBehaviour, ISaveLoadable
{
  public static Research Instance;
  [MyCmpAdd]
  private Notifier notifier;
  private List<TechInstance> techs = new List<TechInstance>();
  private List<TechInstance> queuedTech = new List<TechInstance>();
  private TechInstance activeResearch;
  private Notification NoResearcherRole;
  private Notification MissingResearchStation = new Notification((string) RESEARCH.MESSAGING.MISSING_RESEARCH_STATION, NotificationType.Bad, (Func<List<Notification>, object, string>) ((list, data) => RESEARCH.MESSAGING.MISSING_RESEARCH_STATION_TOOLTIP.ToString().Replace("{0}", Research.Instance.GetMissingResearchBuildingName())), expires: false, delay: 11f);
  private List<IResearchCenter> researchCenterPrefabs = new List<IResearchCenter>();
  public ResearchTypes researchTypes;
  public bool UseGlobalPointInventory;
  [Serialize]
  public ResearchPointInventory globalPointInventory;
  [Serialize]
  private Research.SaveData saveData;
  private static readonly EventSystem.IntraObjectHandler<Research> OnRolesUpdatedDelegate = new EventSystem.IntraObjectHandler<Research>((Action<Research, object>) ((component, data) => component.OnRolesUpdated(data)));

  public static void DestroyInstance() => Research.Instance = (Research) null;

  public TechInstance GetTechInstance(string techID) => this.techs.Find((Predicate<TechInstance>) (match => match.tech.Id == techID));

  public bool IsBeingResearched(Tech tech) => this.activeResearch != null && tech != null && this.activeResearch.tech == tech;

  protected virtual void OnPrefabInit()
  {
    Research.Instance = this;
    this.researchTypes = new ResearchTypes();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.globalPointInventory == null)
      this.globalPointInventory = new ResearchPointInventory();
    this.Subscribe<Research>(-1523247426, Research.OnRolesUpdatedDelegate);
    Components.ResearchCenters.OnAdd += new Action<IResearchCenter>(this.CheckResearchBuildings);
    Components.ResearchCenters.OnRemove += new Action<IResearchCenter>(this.CheckResearchBuildings);
    foreach (Component prefab in Assets.Prefabs)
    {
      IResearchCenter component = prefab.GetComponent<IResearchCenter>();
      if (component != null)
        this.researchCenterPrefabs.Add(component);
    }
  }

  public ResearchType GetResearchType(string id) => this.researchTypes.GetResearchType(id);

  public TechInstance GetActiveResearch() => this.activeResearch;

  public TechInstance GetTargetResearch() => this.queuedTech != null && this.queuedTech.Count > 0 ? this.queuedTech[this.queuedTech.Count - 1] : (TechInstance) null;

  public TechInstance Get(Tech tech)
  {
    foreach (TechInstance tech1 in this.techs)
    {
      if (tech1.tech == tech)
        return tech1;
    }
    return (TechInstance) null;
  }

  public TechInstance GetOrAdd(Tech tech)
  {
    TechInstance orAdd1 = this.techs.Find((Predicate<TechInstance>) (tc => tc.tech == tech));
    if (orAdd1 != null)
      return orAdd1;
    TechInstance orAdd2 = new TechInstance(tech);
    this.techs.Add(orAdd2);
    return orAdd2;
  }

  public void GetNextTech()
  {
    if (this.queuedTech.Count > 0)
      this.queuedTech.RemoveAt(0);
    if (this.queuedTech.Count > 0)
      this.SetActiveResearch(this.queuedTech[this.queuedTech.Count - 1].tech);
    else
      this.SetActiveResearch((Tech) null);
  }

  private void AddTechToQueue(Tech tech)
  {
    TechInstance orAdd = this.GetOrAdd(tech);
    if (!orAdd.IsComplete())
      this.queuedTech.Add(orAdd);
    orAdd.tech.requiredTech.ForEach((Action<Tech>) (_tech => this.AddTechToQueue(_tech)));
  }

  public void CancelResearch(Tech tech, bool clickedEntry = true)
  {
    TechInstance ti = this.queuedTech.Find((Predicate<TechInstance>) (qt => qt.tech == tech));
    if (ti == null)
      return;
    if (ti == this.queuedTech[this.queuedTech.Count - 1] & clickedEntry)
      this.SetActiveResearch((Tech) null);
    for (int i = ti.tech.unlockedTech.Count - 1; i >= 0; i--)
    {
      if (this.queuedTech.Find((Predicate<TechInstance>) (qt => qt.tech == ti.tech.unlockedTech[i])) != null)
        this.CancelResearch(ti.tech.unlockedTech[i], false);
    }
    this.queuedTech.Remove(ti);
    if (!clickedEntry)
      return;
    this.NotifyResearchCenters(GameHashes.ActiveResearchChanged, (object) this.queuedTech);
  }

  private void NotifyResearchCenters(GameHashes hash, object data)
  {
    foreach (KMonoBehaviour researchCenter in Components.ResearchCenters)
      researchCenter.Trigger(-1914338957, data);
    this.Trigger((int) hash, data);
  }

  public void SetActiveResearch(Tech tech, bool clearQueue = false)
  {
    if (clearQueue)
      this.queuedTech.Clear();
    this.activeResearch = (TechInstance) null;
    if (tech != null)
    {
      if (this.queuedTech.Count == 0)
        this.AddTechToQueue(tech);
      if (this.queuedTech.Count > 0)
      {
        this.queuedTech.Sort((Comparison<TechInstance>) ((x, y) => x.tech.tier.CompareTo(y.tech.tier)));
        this.activeResearch = this.queuedTech[0];
      }
    }
    else
      this.queuedTech.Clear();
    this.NotifyResearchCenters(GameHashes.ActiveResearchChanged, (object) this.queuedTech);
    this.CheckBuyResearch();
    this.CheckResearchBuildings((object) null);
    if (this.NoResearcherRole != null)
    {
      this.notifier.Remove(this.NoResearcherRole);
      this.NoResearcherRole = (Notification) null;
    }
    if (this.activeResearch == null)
      return;
    Skill tooltip_data = (Skill) null;
    if (this.activeResearch.tech.costsByResearchTypeID.ContainsKey("advanced") && (double) this.activeResearch.tech.costsByResearchTypeID["advanced"] > 0.0 && !MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowAdvancedResearch.Id))
      tooltip_data = Db.Get().Skills.GetSkillsWithPerk(Db.Get().SkillPerks.AllowAdvancedResearch)[0];
    else if (this.activeResearch.tech.costsByResearchTypeID.ContainsKey("space") && (double) this.activeResearch.tech.costsByResearchTypeID["space"] > 0.0 && !MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowInterstellarResearch.Id))
      tooltip_data = Db.Get().Skills.GetSkillsWithPerk(Db.Get().SkillPerks.AllowInterstellarResearch)[0];
    else if (this.activeResearch.tech.costsByResearchTypeID.ContainsKey("nuclear") && (double) this.activeResearch.tech.costsByResearchTypeID["nuclear"] > 0.0 && !MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowNuclearResearch.Id))
      tooltip_data = Db.Get().Skills.GetSkillsWithPerk(Db.Get().SkillPerks.AllowNuclearResearch)[0];
    else if (this.activeResearch.tech.costsByResearchTypeID.ContainsKey("orbital") && (double) this.activeResearch.tech.costsByResearchTypeID["orbital"] > 0.0 && !MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowOrbitalResearch.Id))
      tooltip_data = Db.Get().Skills.GetSkillsWithPerk(Db.Get().SkillPerks.AllowOrbitalResearch)[0];
    if (tooltip_data == null)
      return;
    this.NoResearcherRole = new Notification((string) RESEARCH.MESSAGING.NO_RESEARCHER_SKILL, NotificationType.Bad, new Func<List<Notification>, object, string>(this.NoResearcherRoleTooltip), (object) tooltip_data, false, 12f);
    this.notifier.Add(this.NoResearcherRole);
  }

  private string NoResearcherRoleTooltip(List<Notification> list, object data)
  {
    Skill skill = (Skill) data;
    return RESEARCH.MESSAGING.NO_RESEARCHER_SKILL_TOOLTIP.Replace("{ResearchType}", skill.Name);
  }

  public void AddResearchPoints(string researchTypeID, float points)
  {
    if (!this.UseGlobalPointInventory && this.activeResearch == null)
    {
      Debug.LogWarning((object) "No active research to add research points to. Global research inventory is disabled.");
    }
    else
    {
      (this.UseGlobalPointInventory ? this.globalPointInventory : this.activeResearch.progressInventory).AddResearchPoints(researchTypeID, points);
      this.CheckBuyResearch();
      this.NotifyResearchCenters(GameHashes.ResearchPointsChanged, (object) null);
    }
  }

  private void CheckBuyResearch()
  {
    if (this.activeResearch == null)
      return;
    ResearchPointInventory pointInventory = this.UseGlobalPointInventory ? this.globalPointInventory : this.activeResearch.progressInventory;
    if (!this.activeResearch.tech.CanAfford(pointInventory))
      return;
    foreach (KeyValuePair<string, float> keyValuePair in this.activeResearch.tech.costsByResearchTypeID)
      pointInventory.RemoveResearchPoints(keyValuePair.Key, keyValuePair.Value);
    this.activeResearch.Purchased();
    Game.Instance.Trigger(-107300940, (object) this.activeResearch.tech);
    this.GetNextTech();
  }

  protected virtual void OnCleanUp()
  {
    Components.ResearchCenters.OnAdd -= new Action<IResearchCenter>(this.CheckResearchBuildings);
    Components.ResearchCenters.OnRemove -= new Action<IResearchCenter>(this.CheckResearchBuildings);
    base.OnCleanUp();
  }

  public void CompleteQueue()
  {
    while (this.queuedTech.Count > 0)
    {
      foreach (KeyValuePair<string, float> keyValuePair in this.activeResearch.tech.costsByResearchTypeID)
        this.AddResearchPoints(keyValuePair.Key, keyValuePair.Value);
    }
  }

  public List<TechInstance> GetResearchQueue() => new List<TechInstance>((IEnumerable<TechInstance>) this.queuedTech);

  [System.Runtime.Serialization.OnSerializing]
  internal void OnSerializing()
  {
    this.saveData = new Research.SaveData();
    this.saveData.activeResearchId = this.activeResearch == null ? "" : this.activeResearch.tech.Id;
    this.saveData.targetResearchId = this.queuedTech == null || this.queuedTech.Count <= 0 ? "" : this.queuedTech[this.queuedTech.Count - 1].tech.Id;
    this.saveData.techs = new TechInstance.SaveData[this.techs.Count];
    for (int index = 0; index < this.techs.Count; ++index)
      this.saveData.techs[index] = this.techs[index].Save();
  }

  [System.Runtime.Serialization.OnDeserialized]
  internal void OnDeserialized()
  {
    if (this.saveData.techs != null)
    {
      foreach (TechInstance.SaveData tech1 in this.saveData.techs)
      {
        Tech tech2 = Db.Get().Techs.TryGet(tech1.techId);
        if (tech2 != null)
          this.GetOrAdd(tech2).Load(tech1);
      }
    }
    foreach (TechInstance tech in this.techs)
    {
      if (this.saveData.targetResearchId == tech.tech.Id)
      {
        this.SetActiveResearch(tech.tech);
        break;
      }
    }
  }

  private void OnRolesUpdated(object data)
  {
    if (this.activeResearch != null && this.activeResearch.tech.costsByResearchTypeID.Count > 1)
    {
      if (!MinionResume.AnyMinionHasPerk(Db.Get().SkillPerks.AllowAdvancedResearch.Id))
        this.notifier.Add(this.NoResearcherRole);
      else
        this.notifier.Remove(this.NoResearcherRole);
    }
    else
      this.notifier.Remove(this.NoResearcherRole);
  }

  public string GetMissingResearchBuildingName()
  {
    foreach (KeyValuePair<string, float> keyValuePair in this.activeResearch.tech.costsByResearchTypeID)
    {
      bool flag = true;
      if ((double) keyValuePair.Value > 0.0)
      {
        flag = false;
        foreach (IResearchCenter researchCenter in Components.ResearchCenters.Items)
        {
          if (researchCenter.GetResearchType() == keyValuePair.Key)
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
      {
        foreach (IResearchCenter researchCenterPrefab in this.researchCenterPrefabs)
        {
          if (researchCenterPrefab.GetResearchType() == keyValuePair.Key)
            return ((Component) researchCenterPrefab).GetProperName();
        }
        return (string) null;
      }
    }
    return (string) null;
  }

  private void CheckResearchBuildings(object data)
  {
    if (this.activeResearch == null)
      this.notifier.Remove(this.MissingResearchStation);
    else if (string.IsNullOrEmpty(this.GetMissingResearchBuildingName()))
      this.notifier.Remove(this.MissingResearchStation);
    else
      this.notifier.Add(this.MissingResearchStation);
  }

  private struct SaveData
  {
    public string activeResearchId;
    public string targetResearchId;
    public TechInstance.SaveData[] techs;
  }
}
