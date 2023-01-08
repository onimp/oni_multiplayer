// Decompiled with JetBrains decompiler
// Type: SpaceDestination
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class SpaceDestination
{
  private const int MASS_TO_RECOVER_AMOUNT = 1000;
  private static List<Tuple<float, int>> RARE_ELEMENT_CHANCES;
  private static readonly List<Tuple<SimHashes, MathUtil.MinMax>> RARE_ELEMENTS;
  private const float RARE_ITEM_CHANCE = 0.33f;
  private static readonly List<Tuple<string, MathUtil.MinMax>> RARE_ITEMS;
  [Serialize]
  public int id;
  [Serialize]
  public string type;
  public bool startAnalyzed;
  [Serialize]
  public int distance;
  [Serialize]
  public float activePeriod = 20f;
  [Serialize]
  public float inactivePeriod = 10f;
  [Serialize]
  public float startingOrbitPercentage;
  [Serialize]
  public Dictionary<SimHashes, float> recoverableElements = new Dictionary<SimHashes, float>();
  [Serialize]
  public List<SpaceDestination.ResearchOpportunity> researchOpportunities = new List<SpaceDestination.ResearchOpportunity>();
  [Serialize]
  private float availableMass;

  private static Tuple<SimHashes, MathUtil.MinMax> GetRareElement(SimHashes id)
  {
    foreach (Tuple<SimHashes, MathUtil.MinMax> rareElement in SpaceDestination.RARE_ELEMENTS)
    {
      if (rareElement.first == id)
        return rareElement;
    }
    return (Tuple<SimHashes, MathUtil.MinMax>) null;
  }

  public int OneBasedDistance => this.distance + 1;

  public float CurrentMass => (float) this.GetDestinationType().minimumMass + this.availableMass;

  public float AvailableMass => this.availableMass;

  public SpaceDestination(int id, string type, int distance)
  {
    this.id = id;
    this.type = type;
    this.distance = distance;
    SpaceDestinationType destinationType = this.GetDestinationType();
    this.availableMass = (float) (destinationType.maxiumMass - destinationType.minimumMass);
    this.GenerateSurfaceElements();
    this.GenerateResearchOpportunities();
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 9))
      return;
    SpaceDestinationType destinationType = this.GetDestinationType();
    this.availableMass = (float) (destinationType.maxiumMass - destinationType.minimumMass);
  }

  public SpaceDestinationType GetDestinationType() => Db.Get().SpaceDestinationTypes.Get(this.type);

  public SpaceDestination.ResearchOpportunity TryCompleteResearchOpportunity()
  {
    foreach (SpaceDestination.ResearchOpportunity researchOpportunity in this.researchOpportunities)
    {
      if (researchOpportunity.TryComplete(this))
        return researchOpportunity;
    }
    return (SpaceDestination.ResearchOpportunity) null;
  }

  public void GenerateSurfaceElements()
  {
    foreach (KeyValuePair<SimHashes, MathUtil.MinMax> keyValuePair in this.GetDestinationType().elementTable)
      this.recoverableElements.Add(keyValuePair.Key, Random.value);
  }

  public SpacecraftManager.DestinationAnalysisState AnalysisState() => SpacecraftManager.instance.GetDestinationAnalysisState(this);

  public void GenerateResearchOpportunities()
  {
    this.researchOpportunities.Add(new SpaceDestination.ResearchOpportunity((string) UI.STARMAP.DESTINATIONSTUDY.UPPERATMO, TUNING.ROCKETRY.DESTINATION_RESEARCH.BASIC));
    this.researchOpportunities.Add(new SpaceDestination.ResearchOpportunity((string) UI.STARMAP.DESTINATIONSTUDY.LOWERATMO, TUNING.ROCKETRY.DESTINATION_RESEARCH.BASIC));
    this.researchOpportunities.Add(new SpaceDestination.ResearchOpportunity((string) UI.STARMAP.DESTINATIONSTUDY.MAGNETICFIELD, TUNING.ROCKETRY.DESTINATION_RESEARCH.BASIC));
    this.researchOpportunities.Add(new SpaceDestination.ResearchOpportunity((string) UI.STARMAP.DESTINATIONSTUDY.SURFACE, TUNING.ROCKETRY.DESTINATION_RESEARCH.BASIC));
    this.researchOpportunities.Add(new SpaceDestination.ResearchOpportunity((string) UI.STARMAP.DESTINATIONSTUDY.SUBSURFACE, TUNING.ROCKETRY.DESTINATION_RESEARCH.BASIC));
    float num1 = 0.0f;
    foreach (Tuple<float, int> tuple in SpaceDestination.RARE_ELEMENT_CHANCES)
      num1 += tuple.first;
    float num2 = Random.value * num1;
    int num3 = 0;
    foreach (Tuple<float, int> tuple in SpaceDestination.RARE_ELEMENT_CHANCES)
    {
      num2 -= tuple.first;
      if ((double) num2 <= 0.0)
        num3 = tuple.second;
    }
    for (int index = 0; index < num3; ++index)
      this.researchOpportunities[Random.Range(0, this.researchOpportunities.Count)].discoveredRareResource = SpaceDestination.RARE_ELEMENTS[Random.Range(0, SpaceDestination.RARE_ELEMENTS.Count)].first;
    if ((double) Random.value >= 0.33000001311302185)
      return;
    this.researchOpportunities[Random.Range(0, this.researchOpportunities.Count)].discoveredRareItem = SpaceDestination.RARE_ITEMS[Random.Range(0, SpaceDestination.RARE_ITEMS.Count)].first;
  }

  public float GetResourceValue(SimHashes resource, float roll)
  {
    if (this.GetDestinationType().elementTable.ContainsKey(resource))
    {
      MathUtil.MinMax minMax = this.GetDestinationType().elementTable[resource];
      return ((MathUtil.MinMax) ref minMax).Lerp(roll);
    }
    if (!SpaceDestinationTypes.extendedElementTable.ContainsKey(resource))
      return 0.0f;
    MathUtil.MinMax minMax1 = SpaceDestinationTypes.extendedElementTable[resource];
    return ((MathUtil.MinMax) ref minMax1).Lerp(roll);
  }

  public Dictionary<SimHashes, float> GetMissionResourceResult(
    float totalCargoSpace,
    float reservedMass,
    bool solids = true,
    bool liquids = true,
    bool gasses = true)
  {
    Dictionary<SimHashes, float> missionResourceResult = new Dictionary<SimHashes, float>();
    float num1 = 0.0f;
    foreach (KeyValuePair<SimHashes, float> recoverableElement in this.recoverableElements)
    {
      if (ElementLoader.FindElementByHash(recoverableElement.Key).IsSolid & solids || ElementLoader.FindElementByHash(recoverableElement.Key).IsLiquid & liquids || ElementLoader.FindElementByHash(recoverableElement.Key).IsGas & gasses)
        num1 += this.GetResourceValue(recoverableElement.Key, recoverableElement.Value);
    }
    float num2 = Mathf.Min(this.CurrentMass + reservedMass - (float) this.GetDestinationType().minimumMass, totalCargoSpace);
    foreach (KeyValuePair<SimHashes, float> recoverableElement in this.recoverableElements)
    {
      if (ElementLoader.FindElementByHash(recoverableElement.Key).IsSolid & solids || ElementLoader.FindElementByHash(recoverableElement.Key).IsLiquid & liquids || ElementLoader.FindElementByHash(recoverableElement.Key).IsGas & gasses)
      {
        float num3 = num2 * (this.GetResourceValue(recoverableElement.Key, recoverableElement.Value) / num1);
        missionResourceResult.Add(recoverableElement.Key, num3);
      }
    }
    return missionResourceResult;
  }

  public Dictionary<Tag, int> GetRecoverableEntities()
  {
    Dictionary<Tag, int> recoverableEntities1 = new Dictionary<Tag, int>();
    Dictionary<string, int> recoverableEntities2 = this.GetDestinationType().recoverableEntities;
    if (recoverableEntities2 != null)
    {
      foreach (KeyValuePair<string, int> keyValuePair in recoverableEntities2)
        recoverableEntities1.Add(Tag.op_Implicit(keyValuePair.Key), keyValuePair.Value);
    }
    return recoverableEntities1;
  }

  public Dictionary<Tag, int> GetMissionEntityResult() => this.GetRecoverableEntities();

  public float ReserveResources(CargoBay bay)
  {
    float num = 0.0f;
    if (Object.op_Inequality((Object) bay, (Object) null))
    {
      Storage component = ((Component) bay).GetComponent<Storage>();
      foreach (KeyValuePair<SimHashes, float> recoverableElement in this.recoverableElements)
      {
        if (this.HasElementType(bay.storageType))
        {
          num += component.capacityKg;
          this.availableMass = Mathf.Max(0.0f, this.availableMass - component.capacityKg);
          break;
        }
      }
    }
    return num;
  }

  public bool HasElementType(CargoBay.CargoType type)
  {
    foreach (KeyValuePair<SimHashes, float> recoverableElement in this.recoverableElements)
    {
      if (ElementLoader.FindElementByHash(recoverableElement.Key).IsSolid && type == CargoBay.CargoType.Solids || ElementLoader.FindElementByHash(recoverableElement.Key).IsLiquid && type == CargoBay.CargoType.Liquids || ElementLoader.FindElementByHash(recoverableElement.Key).IsGas && type == CargoBay.CargoType.Gasses)
        return true;
    }
    return false;
  }

  public void Replenish(float dt)
  {
    SpaceDestinationType destinationType = this.GetDestinationType();
    if ((double) this.CurrentMass >= (double) destinationType.maxiumMass)
      return;
    this.availableMass += destinationType.replishmentPerSim1000ms;
  }

  public float GetAvailableResourcesPercentage(CargoBay.CargoType cargoType)
  {
    float resourcesPercentage = 0.0f;
    float totalMass = this.GetTotalMass();
    foreach (KeyValuePair<SimHashes, float> recoverableElement in this.recoverableElements)
    {
      if (ElementLoader.FindElementByHash(recoverableElement.Key).IsSolid && cargoType == CargoBay.CargoType.Solids || ElementLoader.FindElementByHash(recoverableElement.Key).IsLiquid && cargoType == CargoBay.CargoType.Liquids || ElementLoader.FindElementByHash(recoverableElement.Key).IsGas && cargoType == CargoBay.CargoType.Gasses)
        resourcesPercentage += this.GetResourceValue(recoverableElement.Key, recoverableElement.Value) / totalMass;
    }
    return resourcesPercentage;
  }

  public float GetTotalMass()
  {
    float totalMass = 0.0f;
    foreach (KeyValuePair<SimHashes, float> recoverableElement in this.recoverableElements)
      totalMass += this.GetResourceValue(recoverableElement.Key, recoverableElement.Value);
    return totalMass;
  }

  static SpaceDestination()
  {
    List<Tuple<float, int>> tupleList1 = new List<Tuple<float, int>>();
    tupleList1.Add(new Tuple<float, int>(1f, 0));
    tupleList1.Add(new Tuple<float, int>(0.33f, 1));
    tupleList1.Add(new Tuple<float, int>(0.03f, 2));
    SpaceDestination.RARE_ELEMENT_CHANCES = tupleList1;
    List<Tuple<SimHashes, MathUtil.MinMax>> tupleList2 = new List<Tuple<SimHashes, MathUtil.MinMax>>();
    tupleList2.Add(new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Katairite, new MathUtil.MinMax(1f, 10f)));
    tupleList2.Add(new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Niobium, new MathUtil.MinMax(1f, 10f)));
    tupleList2.Add(new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Fullerene, new MathUtil.MinMax(1f, 10f)));
    tupleList2.Add(new Tuple<SimHashes, MathUtil.MinMax>(SimHashes.Isoresin, new MathUtil.MinMax(1f, 10f)));
    SpaceDestination.RARE_ELEMENTS = tupleList2;
    List<Tuple<string, MathUtil.MinMax>> tupleList3 = new List<Tuple<string, MathUtil.MinMax>>();
    tupleList3.Add(new Tuple<string, MathUtil.MinMax>("GeneShufflerRecharge", new MathUtil.MinMax(1f, 2f)));
    SpaceDestination.RARE_ITEMS = tupleList3;
  }

  [SerializationConfig]
  public class ResearchOpportunity
  {
    [Serialize]
    public string description;
    [Serialize]
    public int dataValue;
    [Serialize]
    public bool completed;
    [Serialize]
    public SimHashes discoveredRareResource = SimHashes.Void;
    [Serialize]
    public string discoveredRareItem;

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized()
    {
      if (this.discoveredRareResource == (SimHashes) 0)
        this.discoveredRareResource = SimHashes.Void;
      if (this.dataValue <= 50)
        return;
      this.dataValue = 50;
    }

    public ResearchOpportunity(string description, int pointValue)
    {
      this.description = description;
      this.dataValue = pointValue;
    }

    public bool TryComplete(SpaceDestination destination)
    {
      if (this.completed)
        return false;
      this.completed = true;
      if (this.discoveredRareResource != SimHashes.Void && !destination.recoverableElements.ContainsKey(this.discoveredRareResource))
        destination.recoverableElements.Add(this.discoveredRareResource, Random.value);
      return true;
    }
  }
}
