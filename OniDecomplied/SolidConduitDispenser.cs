// Decompiled with JetBrains decompiler
// Type: SolidConduitDispenser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/SolidConduitDispenser")]
public class SolidConduitDispenser : KMonoBehaviour, ISaveLoadable, IConduitDispenser
{
  [SerializeField]
  public SimHashes[] elementFilter;
  [SerializeField]
  public bool invertElementFilter;
  [SerializeField]
  public bool alwaysDispense;
  [SerializeField]
  public bool useSecondaryOutput;
  [SerializeField]
  public bool solidOnly;
  private static readonly Operational.Flag outputConduitFlag = new Operational.Flag("output_conduit", Operational.Flag.Type.Functional);
  [MyCmpReq]
  private Operational operational;
  [MyCmpReq]
  public Storage storage;
  private HandleVector<int>.Handle partitionerEntry;
  private int utilityCell = -1;
  private bool dispensing;
  private int round_robin_index;

  public Storage Storage => this.storage;

  public ConduitType ConduitType => ConduitType.Solid;

  public SolidConduitFlow.ConduitContents ConduitContents => this.GetConduitFlow().GetContents(this.utilityCell);

  public bool IsDispensing => this.dispensing;

  public SolidConduitFlow GetConduitFlow() => Game.Instance.solidConduitFlow;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.utilityCell = this.GetOutputCell();
    this.partitionerEntry = GameScenePartitioner.Instance.Add("SolidConduitConsumer.OnSpawn", (object) ((Component) this).gameObject, this.utilityCell, GameScenePartitioner.Instance.objectLayers[20], new Action<object>(this.OnConduitConnectionChanged));
    this.GetConduitFlow().AddConduitUpdater(new Action<float>(this.ConduitUpdate), ConduitFlowPriority.Dispense);
    this.OnConduitConnectionChanged((object) null);
  }

  protected virtual void OnCleanUp()
  {
    this.GetConduitFlow().RemoveConduitUpdater(new Action<float>(this.ConduitUpdate));
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    base.OnCleanUp();
  }

  private void OnConduitConnectionChanged(object data)
  {
    this.dispensing = this.dispensing && this.IsConnected;
    this.Trigger(-2094018600, (object) this.IsConnected);
  }

  private void ConduitUpdate(float dt)
  {
    bool flag = false;
    this.operational.SetFlag(SolidConduitDispenser.outputConduitFlag, this.IsConnected);
    if (this.operational.IsOperational || this.alwaysDispense)
    {
      SolidConduitFlow conduitFlow = this.GetConduitFlow();
      if (conduitFlow.HasConduit(this.utilityCell) && conduitFlow.IsConduitEmpty(this.utilityCell))
      {
        Pickupable suitableItem = this.FindSuitableItem();
        if (Object.op_Implicit((Object) suitableItem))
        {
          if ((double) suitableItem.PrimaryElement.Mass > 20.0)
            suitableItem = suitableItem.Take(20f);
          conduitFlow.AddPickupable(this.utilityCell, suitableItem);
          flag = true;
        }
      }
    }
    this.storage.storageNetworkID = this.GetConnectedNetworkID();
    this.dispensing = flag;
  }

  private bool isSolid(GameObject o)
  {
    PrimaryElement component = o.GetComponent<PrimaryElement>();
    return Object.op_Equality((Object) component, (Object) null) || component.Element.IsLiquid || component.Element.IsGas;
  }

  private Pickupable FindSuitableItem()
  {
    List<GameObject> collection = this.storage.items;
    if (this.solidOnly)
    {
      List<GameObject> gameObjectList = new List<GameObject>((IEnumerable<GameObject>) collection);
      gameObjectList.RemoveAll(new Predicate<GameObject>(this.isSolid));
      collection = gameObjectList;
    }
    if (collection.Count < 1)
      return (Pickupable) null;
    this.round_robin_index %= collection.Count;
    GameObject gameObject = collection[this.round_robin_index];
    ++this.round_robin_index;
    return !Object.op_Implicit((Object) gameObject) ? (Pickupable) null : gameObject.GetComponent<Pickupable>();
  }

  public bool IsConnected
  {
    get
    {
      GameObject gameObject = Grid.Objects[this.utilityCell, 20];
      return Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject.GetComponent<BuildingComplete>(), (Object) null);
    }
  }

  private int GetConnectedNetworkID()
  {
    GameObject gameObject = Grid.Objects[this.utilityCell, 20];
    SolidConduit solidConduit = Object.op_Inequality((Object) gameObject, (Object) null) ? gameObject.GetComponent<SolidConduit>() : (SolidConduit) null;
    UtilityNetwork utilityNetwork = Object.op_Inequality((Object) solidConduit, (Object) null) ? solidConduit.GetNetwork() : (UtilityNetwork) null;
    return utilityNetwork == null ? -1 : utilityNetwork.id;
  }

  private int GetOutputCell()
  {
    Building component1 = ((Component) this).GetComponent<Building>();
    if (!this.useSecondaryOutput)
      return component1.GetUtilityOutputCell();
    foreach (ISecondaryOutput component2 in ((Component) this).GetComponents<ISecondaryOutput>())
    {
      if (component2.HasSecondaryConduitType(ConduitType.Solid))
        return Grid.OffsetCell(component1.NaturalBuildingCell(), component2.GetSecondaryConduitOffset(ConduitType.Solid));
    }
    return Grid.OffsetCell(component1.NaturalBuildingCell(), CellOffset.none);
  }
}
