// Decompiled with JetBrains decompiler
// Type: ConduitDispenser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/ConduitDispenser")]
public class ConduitDispenser : KMonoBehaviour, ISaveLoadable, IConduitDispenser
{
  [SerializeField]
  public ConduitType conduitType;
  [SerializeField]
  public SimHashes[] elementFilter;
  [SerializeField]
  public bool invertElementFilter;
  [SerializeField]
  public bool alwaysDispense;
  [SerializeField]
  public bool isOn = true;
  [SerializeField]
  public bool blocked;
  [SerializeField]
  public bool empty = true;
  [SerializeField]
  public bool useSecondaryOutput;
  private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);
  [MyCmpReq]
  private Operational operational;
  [MyCmpReq]
  public Storage storage;
  [MyCmpReq]
  private Building building;
  private HandleVector<int>.Handle partitionerEntry;
  private int utilityCell = -1;
  private int elementOutputOffset;

  public Storage Storage => this.storage;

  public ConduitType ConduitType => this.conduitType;

  public ConduitFlow.ConduitContents ConduitContents => this.GetConduitManager().GetContents(this.utilityCell);

  public void SetConduitData(ConduitType type) => this.conduitType = type;

  public ConduitFlow GetConduitManager()
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

  private void OnConduitConnectionChanged(object data) => this.Trigger(-2094018600, (object) this.IsConnected);

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    GameScheduler.Instance.Schedule("PlumbingTutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Plumbing)), (object) null, (SchedulerGroup) null);
    this.utilityCell = this.GetOutputCell(this.GetConduitManager().conduitType);
    this.partitionerEntry = GameScenePartitioner.Instance.Add("ConduitConsumer.OnSpawn", (object) ((Component) this).gameObject, this.utilityCell, GameScenePartitioner.Instance.objectLayers[this.conduitType == ConduitType.Gas ? 12 : 16], new Action<object>(this.OnConduitConnectionChanged));
    this.GetConduitManager().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Dispense);
    this.OnConduitConnectionChanged((object) null);
  }

  protected virtual void OnCleanUp()
  {
    this.GetConduitManager().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  public void SetOnState(bool onState) => this.isOn = onState;

  private void ConduitUpdate(float dt)
  {
    this.operational.SetFlag(ConduitDispenser.outputConduitFlag, this.IsConnected);
    this.blocked = false;
    if (!this.isOn)
      return;
    this.Dispense(dt);
  }

  private void Dispense(float dt)
  {
    if (!this.operational.IsOperational && !this.alwaysDispense)
      return;
    if (this.building.Def.CanMove)
      this.utilityCell = this.GetOutputCell(this.GetConduitManager().conduitType);
    PrimaryElement suitableElement = this.FindSuitableElement();
    if (Object.op_Inequality((Object) suitableElement, (Object) null))
    {
      this.empty = false;
      float num1 = this.GetConduitManager().AddElement(this.utilityCell, suitableElement.ElementID, suitableElement.Mass, suitableElement.Temperature, suitableElement.DiseaseIdx, suitableElement.DiseaseCount);
      if ((double) num1 > 0.0)
      {
        int num2 = (int) ((double) num1 / (double) suitableElement.Mass * (double) suitableElement.DiseaseCount);
        suitableElement.ModifyDiseaseCount(-num2, "ConduitDispenser.ConduitUpdate");
        suitableElement.Mass -= num1;
        this.storage.Trigger(-1697596308, (object) ((Component) suitableElement).gameObject);
      }
      else
        this.blocked = true;
    }
    else
      this.empty = true;
  }

  private PrimaryElement FindSuitableElement()
  {
    List<GameObject> items = this.storage.items;
    int count = items.Count;
    for (int index1 = 0; index1 < count; ++index1)
    {
      int index2 = (index1 + this.elementOutputOffset) % count;
      PrimaryElement component = items[index2].GetComponent<PrimaryElement>();
      if (Object.op_Inequality((Object) component, (Object) null) && (double) component.Mass > 0.0 && (this.conduitType == ConduitType.Liquid ? (component.Element.IsLiquid ? 1 : 0) : (component.Element.IsGas ? 1 : 0)) != 0 && (this.elementFilter == null || this.elementFilter.Length == 0 || !this.invertElementFilter && this.IsFilteredElement(component.ElementID) || this.invertElementFilter && !this.IsFilteredElement(component.ElementID)))
      {
        this.elementOutputOffset = (this.elementOutputOffset + 1) % count;
        return component;
      }
    }
    return (PrimaryElement) null;
  }

  private bool IsFilteredElement(SimHashes element)
  {
    for (int index = 0; index != this.elementFilter.Length; ++index)
    {
      if (this.elementFilter[index] == element)
        return true;
    }
    return false;
  }

  public bool IsConnected
  {
    get
    {
      GameObject gameObject = Grid.Objects[this.utilityCell, this.conduitType == ConduitType.Gas ? 12 : 16];
      return Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject.GetComponent<BuildingComplete>(), (Object) null);
    }
  }

  private int GetOutputCell(ConduitType outputConduitType)
  {
    Building component = ((Component) this).GetComponent<Building>();
    if (!this.useSecondaryOutput)
      return component.GetUtilityOutputCell();
    ISecondaryOutput[] components = ((Component) this).GetComponents<ISecondaryOutput>();
    foreach (ISecondaryOutput secondaryOutput in components)
    {
      if (secondaryOutput.HasSecondaryConduitType(outputConduitType))
        return Grid.OffsetCell(component.NaturalBuildingCell(), secondaryOutput.GetSecondaryConduitOffset(outputConduitType));
    }
    return Grid.OffsetCell(component.NaturalBuildingCell(), components[0].GetSecondaryConduitOffset(outputConduitType));
  }
}
