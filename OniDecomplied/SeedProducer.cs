// Decompiled with JetBrains decompiler
// Type: SeedProducer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SeedProducer")]
public class SeedProducer : KMonoBehaviour, IGameObjectEffectDescriptor
{
  public SeedProducer.SeedInfo seedInfo;
  private bool droppedSeedAlready;
  private static readonly EventSystem.IntraObjectHandler<SeedProducer> DropSeedDelegate = new EventSystem.IntraObjectHandler<SeedProducer>((Action<SeedProducer, object>) ((component, data) => component.DropSeed(data)));
  private static readonly EventSystem.IntraObjectHandler<SeedProducer> CropPickedDelegate = new EventSystem.IntraObjectHandler<SeedProducer>((Action<SeedProducer, object>) ((component, data) => component.CropPicked(data)));

  public void Configure(
    string SeedID,
    SeedProducer.ProductionType productionType,
    int newSeedsProduced = 1)
  {
    this.seedInfo.seedId = SeedID;
    this.seedInfo.productionType = productionType;
    this.seedInfo.newSeedsProduced = newSeedsProduced;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<SeedProducer>(-216549700, SeedProducer.DropSeedDelegate);
    this.Subscribe<SeedProducer>(1623392196, SeedProducer.DropSeedDelegate);
    this.Subscribe<SeedProducer>(-1072826864, SeedProducer.CropPickedDelegate);
  }

  private GameObject ProduceSeed(string seedId, int units = 1, bool canMutate = true)
  {
    if (seedId == null || units <= 0)
      return (GameObject) null;
    Vector3 position = Vector3.op_Addition(TransformExtensions.GetPosition(((Component) this).gameObject.transform), new Vector3(0.0f, 0.5f, 0.0f));
    GameObject go = GameUtil.KInstantiate(Assets.GetPrefab(new Tag(seedId)), position, Grid.SceneLayer.Ore);
    MutantPlant component1 = ((Component) this).GetComponent<MutantPlant>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      MutantPlant component2 = go.GetComponent<MutantPlant>();
      bool flag = false;
      if (canMutate && Object.op_Inequality((Object) component2, (Object) null) && component2.IsOriginal)
        flag = this.RollForMutation();
      if (flag)
        component2.Mutate();
      else
        component1.CopyMutationsTo(component2);
    }
    PrimaryElement component3 = ((Component) this).gameObject.GetComponent<PrimaryElement>();
    PrimaryElement component4 = go.GetComponent<PrimaryElement>();
    component4.Temperature = component3.Temperature;
    component4.Units = (float) units;
    this.Trigger(472291861, (object) go.GetComponent<PlantableSeed>());
    go.SetActive(true);
    string str = go.GetProperName();
    if (Object.op_Inequality((Object) component1, (Object) null))
      str = component1.GetSubSpeciesInfo().GetNameWithMutations(str, component1.IsIdentified, false);
    PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, str, go.transform);
    return go;
  }

  public void DropSeed(object data = null)
  {
    if (this.droppedSeedAlready)
      return;
    this.Trigger(-1736624145, (object) this.ProduceSeed(this.seedInfo.seedId, canMutate: false).GetComponent<PlantableSeed>());
    this.droppedSeedAlready = true;
  }

  public void CropDepleted(object data) => this.DropSeed();

  public void CropPicked(object data)
  {
    if (this.seedInfo.productionType != SeedProducer.ProductionType.Harvest)
      return;
    Worker completedBy = ((Component) this).GetComponent<Harvestable>().completed_by;
    float num = 0.1f;
    if (Object.op_Inequality((Object) completedBy, (Object) null))
      num += ((Component) completedBy).GetComponent<AttributeConverters>().Get(Db.Get().AttributeConverters.SeedHarvestChance).Evaluate();
    this.ProduceSeed(this.seedInfo.seedId, (double) Random.Range(0.0f, 1f) <= (double) num ? 1 : 0);
  }

  public bool RollForMutation()
  {
    AttributeInstance attributeInstance = Db.Get().PlantAttributes.MaxRadiationThreshold.Lookup((Component) this);
    int cell = Grid.PosToCell(((Component) this).gameObject);
    return (double) Random.value < (double) Mathf.Clamp(Grid.IsValidCell(cell) ? Grid.Radiation[cell] : 0.0f, 0.0f, attributeInstance.GetTotalValue()) / (double) attributeInstance.GetTotalValue() * 0.800000011920929;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    Object.op_Inequality((Object) Assets.GetPrefab(new Tag(this.seedInfo.seedId)), (Object) null);
    switch (this.seedInfo.productionType)
    {
      case SeedProducer.ProductionType.DigOnly:
        return (List<Descriptor>) null;
      case SeedProducer.ProductionType.Harvest:
        descriptors.Add(new Descriptor((string) UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_HARVEST, (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_HARVEST, (Descriptor.DescriptorType) 2, true));
        descriptors.Add(new Descriptor(string.Format((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.BONUS_SEEDS, (object) GameUtil.GetFormattedPercent(10f)), string.Format((string) UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.BONUS_SEEDS, (object) GameUtil.GetFormattedPercent(10f)), (Descriptor.DescriptorType) 1, false));
        break;
      case SeedProducer.ProductionType.Fruit:
        descriptors.Add(new Descriptor((string) UI.GAMEOBJECTEFFECTS.SEED_PRODUCTION_FRUIT, (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.SEED_PRODUCTION_DIG_ONLY, (Descriptor.DescriptorType) 2, true));
        break;
      case SeedProducer.ProductionType.Sterile:
        descriptors.Add(new Descriptor((string) UI.GAMEOBJECTEFFECTS.MUTANT_STERILE, (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.MUTANT_STERILE, (Descriptor.DescriptorType) 1, false));
        break;
      default:
        return (List<Descriptor>) null;
    }
    return descriptors;
  }

  [Serializable]
  public struct SeedInfo
  {
    public string seedId;
    public SeedProducer.ProductionType productionType;
    public int newSeedsProduced;
  }

  public enum ProductionType
  {
    Hidden,
    DigOnly,
    Harvest,
    Fruit,
    Sterile,
  }
}
