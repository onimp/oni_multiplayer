// Decompiled with JetBrains decompiler
// Type: Crop
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Crop")]
public class Crop : KMonoBehaviour, IGameObjectEffectDescriptor
{
  [MyCmpReq]
  private KSelectable selectable;
  public Crop.CropVal cropVal;
  private AttributeInstance yield;
  public string domesticatedDesc = "";
  private Storage planterStorage;
  private static readonly EventSystem.IntraObjectHandler<Crop> OnHarvestDelegate = new EventSystem.IntraObjectHandler<Crop>((Action<Crop, object>) ((component, data) => component.OnHarvest(data)));
  private static readonly EventSystem.IntraObjectHandler<Crop> OnSeedDroppedDelegate = new EventSystem.IntraObjectHandler<Crop>((Action<Crop, object>) ((component, data) => component.OnSeedDropped(data)));

  public string cropId => this.cropVal.cropId;

  public Storage PlanterStorage
  {
    get => this.planterStorage;
    set => this.planterStorage = value;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Components.Crops.Add(this);
    this.yield = this.GetAttributes().Add(Db.Get().PlantAttributes.YieldAmount);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<Crop>(1272413801, Crop.OnHarvestDelegate);
    this.Subscribe<Crop>(-1736624145, Crop.OnSeedDroppedDelegate);
  }

  public void Configure(Crop.CropVal cropval) => this.cropVal = cropval;

  public bool CanGrow() => this.cropVal.renewable;

  public void SpawnConfiguredFruit(object callbackParam)
  {
    if (Object.op_Equality((Object) this, (Object) null))
      return;
    Crop.CropVal cropVal = this.cropVal;
    if (string.IsNullOrEmpty(cropVal.cropId))
      return;
    this.SpawnSomeFruit(Tag.op_Implicit(cropVal.cropId), this.yield.GetTotalValue());
    this.Trigger(-1072826864, (object) this);
  }

  public void SpawnSomeFruit(Tag cropID, float amount)
  {
    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(cropID), Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), new Vector3(0.0f, 0.75f, 0.0f)), Grid.SceneLayer.Ore);
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      MutantPlant component1 = ((Component) this).GetComponent<MutantPlant>();
      MutantPlant component2 = gameObject.GetComponent<MutantPlant>();
      if (Object.op_Inequality((Object) component1, (Object) null) && component1.IsOriginal && Object.op_Inequality((Object) component2, (Object) null) && ((Component) this).GetComponent<SeedProducer>().RollForMutation())
        component2.Mutate();
      gameObject.SetActive(true);
      PrimaryElement component3 = gameObject.GetComponent<PrimaryElement>();
      component3.Units = amount;
      component3.Temperature = ((Component) this).gameObject.GetComponent<PrimaryElement>().Temperature;
      this.Trigger(35625290, (object) gameObject);
      Edible component4 = gameObject.GetComponent<Edible>();
      if (!Object.op_Implicit((Object) component4))
        return;
      ReportManager.Instance.ReportValue(ReportManager.ReportType.CaloriesCreated, component4.Calories, StringFormatter.Replace((string) UI.ENDOFDAYREPORT.NOTES.HARVESTED, "{0}", ((Component) component4).GetProperName()), (string) UI.ENDOFDAYREPORT.NOTES.HARVESTED_CONTEXT);
    }
    else
      DebugUtil.LogErrorArgs((Object) ((Component) this).gameObject, new object[2]
      {
        (object) "tried to spawn an invalid crop prefab:",
        (object) cropID
      });
  }

  protected virtual void OnCleanUp()
  {
    Components.Crops.Remove(this);
    base.OnCleanUp();
  }

  private void OnHarvest(object obj)
  {
  }

  public void OnSeedDropped(object data)
  {
  }

  public List<Descriptor> RequirementDescriptors(GameObject go) => new List<Descriptor>();

  public List<Descriptor> InformationDescriptors(GameObject go)
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    Tag tag;
    // ISSUE: explicit constructor call
    ((Tag) ref tag).\u002Ector(this.cropVal.cropId);
    GameObject prefab = Assets.GetPrefab(tag);
    Edible component = prefab.GetComponent<Edible>();
    Klei.AI.Attribute yieldAmount = Db.Get().PlantAttributes.YieldAmount;
    float modifiedAttributeValue = go.GetComponent<Modifiers>().GetPreModifiedAttributeValue(yieldAmount);
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      DebugUtil.Assert(GameTags.DisplayAsCalories.Contains(tag), "Trying to display crop info for an edible fruit which isn't displayed as calories!", tag.ToString());
      float caloriesPerUnit = component.FoodInfo.CaloriesPerUnit;
      float calories = caloriesPerUnit * modifiedAttributeValue;
      string formattedCalories = GameUtil.GetFormattedCalories(calories);
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD, (object) prefab.GetProperName(), (object) formattedCalories), string.Format((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD, (object) "", (object) GameUtil.GetFormattedCalories(caloriesPerUnit), (object) GameUtil.GetFormattedCalories(calories)), (Descriptor.DescriptorType) 1, false);
      descriptorList.Add(descriptor);
    }
    else
    {
      string str = !GameTags.DisplayAsUnits.Contains(tag) ? GameUtil.GetFormattedMass((float) this.cropVal.numProduced) : GameUtil.GetFormattedUnits((float) this.cropVal.numProduced, displaySuffix: false);
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.YIELD_NONFOOD, (object) prefab.GetProperName(), (object) str), string.Format((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.YIELD_NONFOOD, (object) str), (Descriptor.DescriptorType) 1, false);
      descriptorList.Add(descriptor);
    }
    return descriptorList;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    foreach (Descriptor requirementDescriptor in this.RequirementDescriptors(go))
      descriptors.Add(requirementDescriptor);
    foreach (Descriptor informationDescriptor in this.InformationDescriptors(go))
      descriptors.Add(informationDescriptor);
    return descriptors;
  }

  [Serializable]
  public struct CropVal
  {
    public string cropId;
    public float cropDuration;
    public int numProduced;
    public bool renewable;

    public CropVal(string crop_id, float crop_duration, int num_produced = 1, bool renewable = true)
    {
      this.cropId = crop_id;
      this.cropDuration = crop_duration;
      this.numProduced = num_produced;
      this.renewable = renewable;
    }
  }
}
