// Decompiled with JetBrains decompiler
// Type: TinkerStation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/TinkerStation")]
public class TinkerStation : Workable, IGameObjectEffectDescriptor, ISim1000ms
{
  public HashedString choreType;
  public HashedString fetchChoreType;
  private Chore chore;
  [MyCmpAdd]
  private Operational operational;
  [MyCmpAdd]
  private Storage storage;
  public bool useFilteredStorage;
  protected FilteredStorage filteredStorage;
  public bool alwaysTinker;
  public float massPerTinker;
  public Tag inputMaterial;
  public Tag outputPrefab;
  public float outputTemperature;
  private static readonly EventSystem.IntraObjectHandler<TinkerStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<TinkerStation>((Action<TinkerStation, object>) ((component, data) => component.OnOperationalChanged(data)));

  public AttributeConverter AttributeConverter
  {
    set => this.attributeConverter = value;
  }

  public float AttributeExperienceMultiplier
  {
    set => this.attributeExperienceMultiplier = value;
  }

  public string SkillExperienceSkillGroup
  {
    set => this.skillExperienceSkillGroup = value;
  }

  public float SkillExperienceMultiplier
  {
    set => this.skillExperienceMultiplier = value;
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
    this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
    if (this.useFilteredStorage)
      this.filteredStorage = new FilteredStorage((KMonoBehaviour) this, (Tag[]) null, (IUserControlledCapacity) null, false, Db.Get().ChoreTypes.GetByHash(this.fetchChoreType));
    this.SetWorkTime(15f);
    this.Subscribe<TinkerStation>(-592767678, TinkerStation.OnOperationalChangedDelegate);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if (!this.useFilteredStorage || this.filteredStorage == null)
      return;
    this.filteredStorage.FilterChanged();
  }

  protected override void OnCleanUp()
  {
    if (this.filteredStorage != null)
      this.filteredStorage.CleanUp();
    base.OnCleanUp();
  }

  private bool CorrectRolePrecondition(MinionIdentity worker)
  {
    MinionResume component = ((Component) worker).GetComponent<MinionResume>();
    return Object.op_Inequality((Object) component, (Object) null) && component.HasPerk(HashedString.op_Implicit(this.requiredSkillPerk));
  }

  private void OnOperationalChanged(object data)
  {
    RoomTracker component = ((Component) this).GetComponent<RoomTracker>();
    if (!Object.op_Inequality((Object) component, (Object) null) || component.room == null)
      return;
    component.room.RetriggerBuildings();
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    if (!this.operational.IsOperational)
      return;
    this.operational.SetActive(true);
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    this.ShowProgressBar(false);
    this.operational.SetActive(false);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    this.storage.ConsumeAndGetDisease(this.inputMaterial, this.massPerTinker, out float _, out SimUtil.DiseaseInfo _, out float _);
    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(this.outputPrefab), TransformExtensions.GetPosition(this.transform), Grid.SceneLayer.Ore);
    gameObject.GetComponent<PrimaryElement>().Temperature = this.outputTemperature;
    gameObject.SetActive(true);
    this.chore = (Chore) null;
  }

  public void Sim1000ms(float dt) => this.UpdateChore();

  private void UpdateChore()
  {
    if (this.operational.IsOperational && (this.ToolsRequested() || this.alwaysTinker) && this.HasMaterial())
    {
      if (this.chore != null)
        return;
      this.chore = (Chore) new WorkChore<TinkerStation>(Db.Get().ChoreTypes.GetByHash(this.choreType), (IStateMachineTarget) this);
      this.chore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) this.requiredSkillPerk);
      this.SetWorkTime(this.workTime);
    }
    else
    {
      if (this.chore == null)
        return;
      this.chore.Cancel("Can't tinker");
      this.chore = (Chore) null;
    }
  }

  private bool HasMaterial() => (double) this.storage.MassStored() > 0.0;

  private bool ToolsRequested() => (double) MaterialNeeds.GetAmount(this.outputPrefab, ((Component) this).gameObject.GetMyWorldId(), false) > 0.0 && (double) this.GetMyWorld().worldInventory.GetAmount(this.outputPrefab, true) <= 0.0;

  public override List<Descriptor> GetDescriptors(GameObject go)
  {
    string str = this.inputMaterial.ProperName();
    List<Descriptor> descriptors = base.GetDescriptors(go);
    descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object) str, (object) GameUtil.GetFormattedMass(this.massPerTinker)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object) str, (object) GameUtil.GetFormattedMass(this.massPerTinker)), (Descriptor.DescriptorType) 0, false));
    descriptors.AddRange((IEnumerable<Descriptor>) GameUtil.GetAllDescriptors(Assets.GetPrefab(this.outputPrefab)));
    List<Tinkerable> tinkerableList = new List<Tinkerable>();
    foreach (GameObject gameObject in Assets.GetPrefabsWithComponent<Tinkerable>())
    {
      Tinkerable component = gameObject.GetComponent<Tinkerable>();
      if (Tag.op_Equality(component.tinkerMaterialTag, this.outputPrefab))
        tinkerableList.Add(component);
    }
    if (tinkerableList.Count > 0)
    {
      Effect effect = Db.Get().effects.Get(tinkerableList[0].addedEffect);
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ADDED_EFFECT, (object) effect.Name), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ADDED_EFFECT, (object) effect.Name, (object) Effect.CreateTooltip(effect, true)), (Descriptor.DescriptorType) 1, false));
      descriptors.Add(new Descriptor((string) UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS, (string) UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS, (Descriptor.DescriptorType) 1, false));
      foreach (Tinkerable cmp in tinkerableList)
      {
        Descriptor descriptor;
        // ISSUE: explicit constructor call
        ((Descriptor) ref descriptor).\u002Ector(string.Format((string) UI.BUILDINGEFFECTS.IMPROVED_BUILDINGS_ITEM, (object) ((Component) cmp).GetProperName()), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.IMPROVED_BUILDINGS_ITEM, (object) ((Component) cmp).GetProperName()), (Descriptor.DescriptorType) 1, false);
        ((Descriptor) ref descriptor).IncreaseIndent();
        descriptors.Add(descriptor);
      }
    }
    return descriptors;
  }

  public static TinkerStation AddTinkerStation(GameObject go, string required_room_type)
  {
    TinkerStation tinkerStation = go.AddOrGet<TinkerStation>();
    go.AddOrGet<RoomTracker>().requiredRoomType = required_room_type;
    return tinkerStation;
  }
}
