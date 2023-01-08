// Decompiled with JetBrains decompiler
// Type: CreatureDeliveryPoint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CreatureDeliveryPoint : 
  StateMachineComponent<CreatureDeliveryPoint.SMInstance>,
  IUserControlledCapacity
{
  [MyCmpAdd]
  private Prioritizable prioritizable;
  [Serialize]
  private int creatureLimit = 20;
  private int storedCreatureCount;
  public CellOffset[] deliveryOffsets = new CellOffset[1];
  public CellOffset spawnOffset = new CellOffset(0, 0);
  private List<FetchOrder2> fetches;
  private static StatusItem capacityStatusItem;
  public bool playAnimsOnFetch;
  private static readonly EventSystem.IntraObjectHandler<CreatureDeliveryPoint> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<CreatureDeliveryPoint>((Action<CreatureDeliveryPoint, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly EventSystem.IntraObjectHandler<CreatureDeliveryPoint> RefreshCreatureCountDelegate = new EventSystem.IntraObjectHandler<CreatureDeliveryPoint>((Action<CreatureDeliveryPoint, object>) ((component, data) => component.RefreshCreatureCount(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.fetches = new List<FetchOrder2>();
    ((Component) this).GetComponent<TreeFilterable>().OnFilterChanged += new Action<HashSet<Tag>>(this.OnFilterChanged);
    ((Component) this).GetComponent<Storage>().SetOffsets(this.deliveryOffsets);
    Prioritizable.AddRef(((Component) this).gameObject);
    if (CreatureDeliveryPoint.capacityStatusItem == null)
    {
      CreatureDeliveryPoint.capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      CreatureDeliveryPoint.capacityStatusItem.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        IUserControlledCapacity controlledCapacity = (IUserControlledCapacity) data;
        string newValue1 = Util.FormatWholeNumber(Mathf.Floor(controlledCapacity.AmountStored));
        string newValue2 = Util.FormatWholeNumber(controlledCapacity.UserMaxCapacity);
        str = str.Replace("{Stored}", newValue1).Replace("{Capacity}", newValue2).Replace("{Units}", (string) controlledCapacity.CapacityUnits);
        return str;
      });
    }
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, CreatureDeliveryPoint.capacityStatusItem, (object) this);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    this.Subscribe<CreatureDeliveryPoint>(-905833192, CreatureDeliveryPoint.OnCopySettingsDelegate);
    this.Subscribe<CreatureDeliveryPoint>(643180843, CreatureDeliveryPoint.RefreshCreatureCountDelegate);
    this.RefreshCreatureCount();
  }

  private void OnCopySettings(object data)
  {
    GameObject gameObject = (GameObject) data;
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    CreatureDeliveryPoint component = gameObject.GetComponent<CreatureDeliveryPoint>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    this.creatureLimit = component.creatureLimit;
    this.RebalanceFetches();
  }

  private void OnFilterChanged(HashSet<Tag> tags)
  {
    this.ClearFetches();
    this.RebalanceFetches();
  }

  private void RefreshCreatureCount(object data = null)
  {
    CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.spawnOffset));
    int storedCreatureCount = this.storedCreatureCount;
    this.storedCreatureCount = 0;
    if (cavityForCell != null)
    {
      foreach (KPrefabID creature in cavityForCell.creatures)
      {
        if (!creature.HasTag(GameTags.Creatures.Bagged) && !creature.HasTag(GameTags.Trapped))
          ++this.storedCreatureCount;
      }
    }
    if (this.storedCreatureCount == storedCreatureCount)
      return;
    this.RebalanceFetches();
  }

  private void ClearFetches()
  {
    for (int index = this.fetches.Count - 1; index >= 0; --index)
      this.fetches[index].Cancel("clearing all fetches");
    this.fetches.Clear();
  }

  private void RebalanceFetches()
  {
    HashSet<Tag> tags = ((Component) this).GetComponent<TreeFilterable>().GetTags();
    ChoreType creatureFetch = Db.Get().ChoreTypes.CreatureFetch;
    Storage component = ((Component) this).GetComponent<Storage>();
    int num1 = this.creatureLimit - this.storedCreatureCount;
    int count = this.fetches.Count;
    int num2 = 0;
    int num3 = 0;
    int num4 = 0;
    int num5 = 0;
    for (int index = this.fetches.Count - 1; index >= 0; --index)
    {
      if (this.fetches[index].IsComplete())
      {
        this.fetches.RemoveAt(index);
        ++num2;
      }
    }
    int num6 = 0;
    for (int index = 0; index < this.fetches.Count; ++index)
    {
      if (!this.fetches[index].InProgress)
        ++num6;
    }
    if (num6 == 0 && this.fetches.Count < num1)
    {
      FetchOrder2 fetchOrder2 = new FetchOrder2(creatureFetch, tags, FetchChore.MatchCriteria.MatchID, GameTags.Creatures.Deliverable, (Tag[]) null, component, 1f, Operational.State.Operational);
      fetchOrder2.Submit(new Action<FetchOrder2, Pickupable>(this.OnFetchComplete), false, new Action<FetchOrder2, Pickupable>(this.OnFetchBegun));
      this.fetches.Add(fetchOrder2);
      int num7 = num3 + 1;
    }
    int num8 = this.fetches.Count - num1;
    for (int index = this.fetches.Count - 1; index >= 0 && num8 > 0; --index)
    {
      if (!this.fetches[index].InProgress)
      {
        this.fetches[index].Cancel("fewer creatures in room");
        this.fetches.RemoveAt(index);
        --num8;
        ++num4;
      }
    }
    while (num8 > 0 && this.fetches.Count > 0)
    {
      this.fetches[this.fetches.Count - 1].Cancel("fewer creatures in room");
      this.fetches.RemoveAt(this.fetches.Count - 1);
      --num8;
      ++num5;
    }
  }

  private void OnFetchComplete(FetchOrder2 fetchOrder, Pickupable fetchedItem) => this.RebalanceFetches();

  private void OnFetchBegun(FetchOrder2 fetchOrder, Pickupable fetchedItem) => this.RebalanceFetches();

  protected override void OnCleanUp()
  {
    this.smi.StopSM(nameof (OnCleanUp));
    ((Component) this).GetComponent<TreeFilterable>().OnFilterChanged -= new Action<HashSet<Tag>>(this.OnFilterChanged);
    base.OnCleanUp();
  }

  float IUserControlledCapacity.UserMaxCapacity
  {
    get => (float) this.creatureLimit;
    set
    {
      this.creatureLimit = Mathf.RoundToInt(value);
      this.RebalanceFetches();
    }
  }

  float IUserControlledCapacity.AmountStored => (float) this.storedCreatureCount;

  float IUserControlledCapacity.MinCapacity => 0.0f;

  float IUserControlledCapacity.MaxCapacity => 20f;

  bool IUserControlledCapacity.WholeValues => true;

  LocString IUserControlledCapacity.CapacityUnits => UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.UNITS_SUFFIX;

  public class SMInstance : 
    GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.GameInstance
  {
    public SMInstance(CreatureDeliveryPoint master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint>
  {
    public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State waiting;
    public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State interact_waiting;
    public GameStateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State interact_delivery;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.waiting;
      this.root.Update("RefreshCreatureCount", (Action<CreatureDeliveryPoint.SMInstance, float>) ((smi, dt) => smi.master.RefreshCreatureCount()), (UpdateRate) 6).EventHandler(GameHashes.OnStorageChange, new StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.State.Callback(CreatureDeliveryPoint.States.DropAllCreatures));
      this.waiting.EnterTransition(this.interact_waiting, (StateMachine<CreatureDeliveryPoint.States, CreatureDeliveryPoint.SMInstance, CreatureDeliveryPoint, object>.Transition.ConditionCallback) (smi => smi.master.playAnimsOnFetch));
      this.interact_waiting.WorkableStartTransition((Func<CreatureDeliveryPoint.SMInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<Storage>()), this.interact_delivery);
      this.interact_delivery.PlayAnim("working_pre").QueueAnim("working_pst").OnAnimQueueComplete(this.interact_waiting);
    }

    public static void DropAllCreatures(CreatureDeliveryPoint.SMInstance smi)
    {
      Storage component = ((Component) smi.master).GetComponent<Storage>();
      if (component.IsEmpty())
        return;
      List<GameObject> items = component.items;
      int count = items.Count;
      Vector3 posCbc = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(smi.transform)), smi.master.spawnOffset), Grid.SceneLayer.Creatures);
      for (int index = count - 1; index >= 0; --index)
      {
        GameObject go = items[index];
        component.Drop(go, true);
        TransformExtensions.SetPosition(go.transform, posCbc);
        go.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
      }
      smi.master.RefreshCreatureCount();
    }
  }
}
