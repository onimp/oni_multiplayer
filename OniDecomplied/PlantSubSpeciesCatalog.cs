// Decompiled with JetBrains decompiler
// Type: PlantSubSpeciesCatalog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[SerializationConfig]
public class PlantSubSpeciesCatalog : KMonoBehaviour
{
  public static PlantSubSpeciesCatalog Instance;
  [Serialize]
  private Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> discoveredSubspeciesBySpecies = new Dictionary<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>>();
  [Serialize]
  private HashSet<Tag> identifiedSubSpecies = new HashSet<Tag>();

  public static void DestroyInstance() => PlantSubSpeciesCatalog.Instance = (PlantSubSpeciesCatalog) null;

  public bool AnyNonOriginalDiscovered
  {
    get
    {
      foreach (KeyValuePair<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> subspeciesBySpecy in this.discoveredSubspeciesBySpecies)
      {
        if (subspeciesBySpecy.Value.Find((Predicate<PlantSubSpeciesCatalog.SubSpeciesInfo>) (ss => !ss.IsOriginal)).IsValid)
          return true;
      }
      return false;
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    PlantSubSpeciesCatalog.Instance = this;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.EnsureOriginalSubSpecies();
  }

  public List<Tag> GetAllDiscoveredSpecies() => ((IEnumerable<Tag>) this.discoveredSubspeciesBySpecies.Keys).ToList<Tag>();

  public List<PlantSubSpeciesCatalog.SubSpeciesInfo> GetAllSubSpeciesForSpecies(Tag speciesID)
  {
    List<PlantSubSpeciesCatalog.SubSpeciesInfo> subSpeciesInfoList;
    return this.discoveredSubspeciesBySpecies.TryGetValue(speciesID, out subSpeciesInfoList) ? subSpeciesInfoList : (List<PlantSubSpeciesCatalog.SubSpeciesInfo>) null;
  }

  public bool GetOriginalSubSpecies(
    Tag speciesID,
    out PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo)
  {
    if (!this.discoveredSubspeciesBySpecies.ContainsKey(speciesID))
    {
      subSpeciesInfo = new PlantSubSpeciesCatalog.SubSpeciesInfo();
      return false;
    }
    subSpeciesInfo = this.discoveredSubspeciesBySpecies[speciesID].Find((Predicate<PlantSubSpeciesCatalog.SubSpeciesInfo>) (i => i.IsOriginal));
    return true;
  }

  public PlantSubSpeciesCatalog.SubSpeciesInfo GetSubSpecies(Tag speciesID, Tag subSpeciesID) => this.discoveredSubspeciesBySpecies[speciesID].Find((Predicate<PlantSubSpeciesCatalog.SubSpeciesInfo>) (i => Tag.op_Equality(i.ID, subSpeciesID)));

  public PlantSubSpeciesCatalog.SubSpeciesInfo FindSubSpecies(Tag subSpeciesID)
  {
    foreach (KeyValuePair<Tag, List<PlantSubSpeciesCatalog.SubSpeciesInfo>> subspeciesBySpecy in this.discoveredSubspeciesBySpecies)
    {
      PlantSubSpeciesCatalog.SubSpeciesInfo subSpecies = subspeciesBySpecy.Value.Find((Predicate<PlantSubSpeciesCatalog.SubSpeciesInfo>) (i => Tag.op_Equality(i.ID, subSpeciesID)));
      if (((Tag) ref subSpecies.ID).IsValid)
        return subSpecies;
    }
    return new PlantSubSpeciesCatalog.SubSpeciesInfo();
  }

  public void DiscoverSubSpecies(
    PlantSubSpeciesCatalog.SubSpeciesInfo newSubSpeciesInfo,
    MutantPlant source)
  {
    if (this.discoveredSubspeciesBySpecies[newSubSpeciesInfo.speciesID].Contains(newSubSpeciesInfo))
      return;
    this.discoveredSubspeciesBySpecies[newSubSpeciesInfo.speciesID].Add(newSubSpeciesInfo);
    Notification notification = new Notification((string) MISC.NOTIFICATIONS.NEWMUTANTSEED.NAME, NotificationType.Good, new Func<List<Notification>, object, string>(this.NewSubspeciesTooltipCB), (object) newSubSpeciesInfo, click_focus: source.transform);
    ((Component) this).gameObject.AddOrGet<Notifier>().Add(notification);
  }

  private string NewSubspeciesTooltipCB(List<Notification> notifications, object data)
  {
    PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo = (PlantSubSpeciesCatalog.SubSpeciesInfo) data;
    return MISC.NOTIFICATIONS.NEWMUTANTSEED.TOOLTIP.Replace("{Plant}", subSpeciesInfo.speciesID.ProperName());
  }

  public void IdentifySubSpecies(Tag subSpeciesID)
  {
    if (!this.identifiedSubSpecies.Add(subSpeciesID))
      return;
    this.FindSubSpecies(subSpeciesID);
    foreach (MutantPlant mutantPlant in Components.MutantPlants)
    {
      if (((Component) mutantPlant).HasTag(subSpeciesID))
        mutantPlant.UpdateNameAndTags();
    }
    GeneticAnalysisCompleteMessage analysisCompleteMessage = new GeneticAnalysisCompleteMessage(subSpeciesID);
    Messenger.Instance.QueueMessage((Message) analysisCompleteMessage);
  }

  public bool IsSubSpeciesIdentified(Tag subSpeciesID) => this.identifiedSubSpecies.Contains(subSpeciesID);

  public List<PlantSubSpeciesCatalog.SubSpeciesInfo> GetAllUnidentifiedSubSpecies(Tag speciesID) => this.discoveredSubspeciesBySpecies[speciesID].FindAll((Predicate<PlantSubSpeciesCatalog.SubSpeciesInfo>) (ss => !this.IsSubSpeciesIdentified(ss.ID)));

  public bool IsValidPlantableSeed(Tag seedID, Tag subspeciesID)
  {
    if (!((Tag) ref seedID).IsValid)
      return false;
    MutantPlant component = Assets.GetPrefab(seedID).GetComponent<MutantPlant>();
    if (Object.op_Equality((Object) component, (Object) null))
      return !((Tag) ref subspeciesID).IsValid;
    List<PlantSubSpeciesCatalog.SubSpeciesInfo> speciesForSpecies = PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(component.SpeciesID);
    return speciesForSpecies != null && speciesForSpecies.FindIndex((Predicate<PlantSubSpeciesCatalog.SubSpeciesInfo>) (s => Tag.op_Equality(s.ID, subspeciesID))) != -1 && PlantSubSpeciesCatalog.Instance.IsSubSpeciesIdentified(subspeciesID);
  }

  private void EnsureOriginalSubSpecies()
  {
    foreach (GameObject gameObject in Assets.GetPrefabsWithComponent<MutantPlant>())
    {
      MutantPlant component = gameObject.GetComponent<MutantPlant>();
      Tag speciesId = component.SpeciesID;
      if (!this.discoveredSubspeciesBySpecies.ContainsKey(speciesId))
      {
        this.discoveredSubspeciesBySpecies[speciesId] = new List<PlantSubSpeciesCatalog.SubSpeciesInfo>();
        this.discoveredSubspeciesBySpecies[speciesId].Add(component.GetSubSpeciesInfo());
      }
      this.identifiedSubSpecies.Add(component.SubSpeciesID);
    }
  }

  [Serializable]
  public struct SubSpeciesInfo : IEquatable<PlantSubSpeciesCatalog.SubSpeciesInfo>
  {
    public Tag speciesID;
    public Tag ID;
    public List<string> mutationIDs;
    private const string ORIGINAL_SUFFIX = "_Original";

    public bool IsValid => ((Tag) ref this.ID).IsValid;

    public bool IsOriginal => this.mutationIDs == null || this.mutationIDs.Count == 0;

    public SubSpeciesInfo(Tag speciesID, List<string> mutationIDs)
    {
      this.speciesID = speciesID;
      this.mutationIDs = mutationIDs != null ? new List<string>((IEnumerable<string>) mutationIDs) : new List<string>();
      this.ID = PlantSubSpeciesCatalog.SubSpeciesInfo.SubSpeciesIDFromMutations(speciesID, mutationIDs);
    }

    public static Tag SubSpeciesIDFromMutations(Tag speciesID, List<string> mutationIDs)
    {
      if (mutationIDs == null || mutationIDs.Count == 0)
        return Tag.op_Implicit(speciesID.ToString() + "_Original");
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append((object) speciesID);
      foreach (string mutationId in mutationIDs)
      {
        stringBuilder.Append("_");
        stringBuilder.Append(mutationId);
      }
      return TagExtensions.ToTag(stringBuilder.ToString());
    }

    public string GetMutationsNames() => this.mutationIDs == null || this.mutationIDs.Count == 0 ? (string) CREATURES.PLANT_MUTATIONS.NONE.NAME : string.Join(", ", (IEnumerable<string>) Db.Get().PlantMutations.GetNamesForMutations(this.mutationIDs));

    public string GetNameWithMutations(string properName, bool identified, bool cleanOriginal) => this.mutationIDs == null || this.mutationIDs.Count == 0 ? (!cleanOriginal ? CREATURES.PLANT_MUTATIONS.PLANT_NAME_FMT.Replace("{PlantName}", properName).Replace("{MutationList}", (string) CREATURES.PLANT_MUTATIONS.NONE.NAME) : properName) : (identified ? CREATURES.PLANT_MUTATIONS.PLANT_NAME_FMT.Replace("{PlantName}", properName).Replace("{MutationList}", string.Join(", ", (IEnumerable<string>) Db.Get().PlantMutations.GetNamesForMutations(this.mutationIDs))) : CREATURES.PLANT_MUTATIONS.PLANT_NAME_FMT.Replace("{PlantName}", properName).Replace("{MutationList}", (string) CREATURES.PLANT_MUTATIONS.UNIDENTIFIED));

    public static bool operator ==(
      PlantSubSpeciesCatalog.SubSpeciesInfo obj1,
      PlantSubSpeciesCatalog.SubSpeciesInfo obj2)
    {
      return obj1.Equals(obj2);
    }

    public static bool operator !=(
      PlantSubSpeciesCatalog.SubSpeciesInfo obj1,
      PlantSubSpeciesCatalog.SubSpeciesInfo obj2)
    {
      return !(obj1 == obj2);
    }

    public override bool Equals(object other) => other is PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo && this == subSpeciesInfo;

    public bool Equals(PlantSubSpeciesCatalog.SubSpeciesInfo other) => Tag.op_Equality(this.ID, other.ID);

    public override int GetHashCode() => this.ID.GetHashCode();

    public string GetMutationsTooltip()
    {
      if (this.mutationIDs == null || this.mutationIDs.Count == 0)
        return (string) CREATURES.STATUSITEMS.ORIGINALPLANTMUTATION.TOOLTIP;
      if (!PlantSubSpeciesCatalog.Instance.IsSubSpeciesIdentified(this.ID))
        return (string) CREATURES.STATUSITEMS.UNKNOWNMUTATION.TOOLTIP;
      string mutationId = this.mutationIDs[0];
      PlantMutation plantMutation = Db.Get().PlantMutations.Get(mutationId);
      return CREATURES.STATUSITEMS.SPECIFICPLANTMUTATION.TOOLTIP.Replace("{MutationName}", plantMutation.Name) + "\n" + plantMutation.GetTooltip();
    }
  }
}
