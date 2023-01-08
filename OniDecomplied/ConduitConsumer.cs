// Decompiled with JetBrains decompiler
// Type: ConduitConsumer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/ConduitConsumer")]
public class ConduitConsumer : KMonoBehaviour, IConduitConsumer
{
  [SerializeField]
  public ConduitType conduitType;
  [SerializeField]
  public bool ignoreMinMassCheck;
  [SerializeField]
  public Tag capacityTag = GameTags.Any;
  [SerializeField]
  public float capacityKG = float.PositiveInfinity;
  [SerializeField]
  public bool forceAlwaysSatisfied;
  [SerializeField]
  public bool alwaysConsume;
  [SerializeField]
  public bool keepZeroMassObject = true;
  [SerializeField]
  public bool useSecondaryInput;
  [SerializeField]
  public bool isOn = true;
  [NonSerialized]
  public bool isConsuming = true;
  [NonSerialized]
  public bool consumedLastTick = true;
  [MyCmpReq]
  public Operational operational;
  [MyCmpReq]
  private Building building;
  public Operational.State OperatingRequirement;
  public ISecondaryInput targetSecondaryInput;
  [MyCmpGet]
  public Storage storage;
  [MyCmpGet]
  private BuildingComplete m_buildingComplete;
  private int utilityCell = -1;
  public float consumptionRate = float.PositiveInfinity;
  public SimHashes lastConsumedElement = SimHashes.Vacuum;
  private HandleVector<int>.Handle partitionerEntry;
  private bool satisfied;
  public ConduitConsumer.WrongElementResult wrongElementResult;

  public Storage Storage => this.storage;

  public ConduitType ConduitType => this.conduitType;

  public bool IsConnected => Object.op_Inequality((Object) Grid.Objects[this.utilityCell, this.conduitType == ConduitType.Gas ? 12 : 16], (Object) null) && Object.op_Inequality((Object) this.m_buildingComplete, (Object) null);

  public bool CanConsume
  {
    get
    {
      bool canConsume = false;
      if (this.IsConnected)
        canConsume = (double) this.GetConduitManager().GetContents(this.utilityCell).mass > 0.0;
      return canConsume;
    }
  }

  public float stored_mass
  {
    get
    {
      if (Object.op_Equality((Object) this.storage, (Object) null))
        return 0.0f;
      return !Tag.op_Inequality(this.capacityTag, GameTags.Any) ? this.storage.MassStored() : this.storage.GetMassAvailable(this.capacityTag);
    }
  }

  public float space_remaining_kg
  {
    get
    {
      float num = this.capacityKG - this.stored_mass;
      return !Object.op_Equality((Object) this.storage, (Object) null) ? Mathf.Min(this.storage.RemainingCapacity(), num) : num;
    }
  }

  public void SetConduitData(ConduitType type) => this.conduitType = type;

  public ConduitType TypeOfConduit => this.conduitType;

  public bool IsAlmostEmpty => !this.ignoreMinMassCheck && (double) this.MassAvailable < (double) this.ConsumptionRate * 30.0;

  public bool IsEmpty
  {
    get
    {
      if (this.ignoreMinMassCheck)
        return false;
      return (double) this.MassAvailable == 0.0 || (double) this.MassAvailable < (double) this.ConsumptionRate;
    }
  }

  public float ConsumptionRate => this.consumptionRate;

  public bool IsSatisfied
  {
    get => this.satisfied || !this.isConsuming;
    set => this.satisfied = value || this.forceAlwaysSatisfied;
  }

  private ConduitFlow GetConduitManager()
  {
    switch (this.conduitType)
    {
      case ConduitType.Gas:
        return Game.Instance.gasConduitFlow;
      case ConduitType.Liquid:
        return Game.Instance.liquidConduitFlow;
      default:
        return (ConduitFlow) null;
    }
  }

  public float MassAvailable
  {
    get
    {
      ConduitFlow conduitManager = this.GetConduitManager();
      int inputCell = this.GetInputCell(conduitManager.conduitType);
      return conduitManager.GetContents(inputCell).mass;
    }
  }

  private int GetInputCell(ConduitType inputConduitType)
  {
    if (!this.useSecondaryInput)
      return this.building.GetUtilityInputCell();
    ISecondaryInput[] components = ((Component) this).GetComponents<ISecondaryInput>();
    foreach (ISecondaryInput secondaryInput in components)
    {
      if (secondaryInput.HasSecondaryConduitType(inputConduitType))
        return Grid.OffsetCell(this.building.NaturalBuildingCell(), secondaryInput.GetSecondaryConduitOffset(inputConduitType));
    }
    Debug.LogWarning((object) "No secondaryInput of type was found");
    return Grid.OffsetCell(this.building.NaturalBuildingCell(), components[0].GetSecondaryConduitOffset(inputConduitType));
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    GameScheduler.Instance.Schedule("PlumbingTutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing)), (object) null, (SchedulerGroup) null);
    this.utilityCell = this.GetInputCell(this.GetConduitManager().conduitType);
    this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", (object) ((Component) this).gameObject, this.utilityCell, GameScenePartitioner.Instance.objectLayers[this.conduitType == ConduitType.Gas ? 12 : 16], new Action<object>(this.OnConduitConnectionChanged));
    this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Default);
    this.OnConduitConnectionChanged((object) null);
  }

  protected virtual void OnCleanUp()
  {
    this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  private void OnConduitConnectionChanged(object data) => this.Trigger(-2094018600, (object) this.IsConnected);

  public void SetOnState(bool onState) => this.isOn = onState;

  private void ConduitUpdate(float dt)
  {
    if (!this.isConsuming || !this.isOn)
      return;
    ConduitFlow conduitManager = this.GetConduitManager();
    this.Consume(dt, conduitManager);
  }

  private void Consume(float dt, ConduitFlow conduit_mgr)
  {
    this.IsSatisfied = false;
    this.consumedLastTick = true;
    if (this.building.Def.CanMove)
      this.utilityCell = this.GetInputCell(conduit_mgr.conduitType);
    if (!this.IsConnected)
      return;
    ConduitFlow.ConduitContents contents = conduit_mgr.GetContents(this.utilityCell);
    if ((double) contents.mass <= 0.0)
      return;
    this.IsSatisfied = true;
    if (!this.alwaysConsume && !this.operational.MeetsRequirements(this.OperatingRequirement))
      return;
    float delta = Mathf.Min(this.ConsumptionRate * dt, this.space_remaining_kg);
    Element elementByHash1 = ElementLoader.FindElementByHash(contents.element);
    if (contents.element != this.lastConsumedElement)
      DiscoveredResources.Instance.Discover(elementByHash1.tag, elementByHash1.materialCategory);
    float mass = 0.0f;
    if ((double) delta > 0.0)
    {
      ConduitFlow.ConduitContents conduitContents = conduit_mgr.RemoveElement(this.utilityCell, delta);
      mass = conduitContents.mass;
      this.lastConsumedElement = conduitContents.element;
    }
    bool flag = elementByHash1.HasTag(this.capacityTag);
    if ((double) mass > 0.0 && Tag.op_Inequality(this.capacityTag, GameTags.Any) && !flag)
      this.Trigger(-794517298, (object) new BuildingHP.DamageSourceInfo()
      {
        damage = 1,
        source = (string) BUILDINGS.DAMAGESOURCES.BAD_INPUT_ELEMENT,
        popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.WRONG_ELEMENT
      });
    if (flag || this.wrongElementResult == ConduitConsumer.WrongElementResult.Store || contents.element == SimHashes.Vacuum || Tag.op_Equality(this.capacityTag, GameTags.Any))
    {
      if ((double) mass <= 0.0)
        return;
      this.consumedLastTick = false;
      int disease_count = (int) ((double) contents.diseaseCount * ((double) mass / (double) contents.mass));
      Element elementByHash2 = ElementLoader.FindElementByHash(contents.element);
      switch (this.conduitType)
      {
        case ConduitType.Gas:
          if (elementByHash2.IsGas)
          {
            this.storage.AddGasChunk(contents.element, mass, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
            break;
          }
          Debug.LogWarning((object) ("Gas conduit consumer consuming non gas: " + elementByHash2.id.ToString()));
          break;
        case ConduitType.Liquid:
          if (elementByHash2.IsLiquid)
          {
            this.storage.AddLiquid(contents.element, mass, contents.temperature, contents.diseaseIdx, disease_count, this.keepZeroMassObject, false);
            break;
          }
          Debug.LogWarning((object) ("Liquid conduit consumer consuming non liquid: " + elementByHash2.id.ToString()));
          break;
      }
    }
    else
    {
      if ((double) mass <= 0.0)
        return;
      this.consumedLastTick = false;
      if (this.wrongElementResult != ConduitConsumer.WrongElementResult.Dump)
        return;
      int disease_count = (int) ((double) contents.diseaseCount * ((double) mass / (double) contents.mass));
      SimMessages.AddRemoveSubstance(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), contents.element, CellEventLogger.Instance.ConduitConsumerWrongElement, mass, contents.temperature, contents.diseaseIdx, disease_count);
    }
  }

  public enum WrongElementResult
  {
    Destroy,
    Dump,
    Store,
  }
}
