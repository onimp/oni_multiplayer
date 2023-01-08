// Decompiled with JetBrains decompiler
// Type: FetchAreaChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FetchAreaChore : Chore<FetchAreaChore.StatesInstance>
{
  public bool IsFetching => this.smi.pickingup;

  public bool IsDelivering => this.smi.delivering;

  public GameObject GetFetchTarget => this.smi.sm.fetchTarget.Get(this.smi);

  public FetchAreaChore(Chore.Precondition.Context context)
    : base(context.chore.choreType, (IStateMachineTarget) context.consumerState.consumer, context.consumerState.choreProvider, false, master_priority_class: context.masterPriority.priority_class, master_priority_value: context.masterPriority.priority_value)
  {
    this.showAvailabilityInHoverText = false;
    this.smi = new FetchAreaChore.StatesInstance(this, context);
  }

  public override void Cleanup() => base.Cleanup();

  public override void Begin(Chore.Precondition.Context context)
  {
    this.smi.Begin(context);
    base.Begin(context);
  }

  protected override void End(string reason)
  {
    this.smi.End();
    base.End(reason);
  }

  private void OnTagsChanged(object data)
  {
    if (!Object.op_Inequality((Object) this.smi.sm.fetchTarget.Get(this.smi), (Object) null))
      return;
    this.Fail("Tags changed");
  }

  private static bool IsPickupableStillValidForChore(Pickupable pickupable, FetchChore chore)
  {
    KPrefabID component = ((Component) pickupable).GetComponent<KPrefabID>();
    if (chore.criteria == FetchChore.MatchCriteria.MatchID && !chore.tags.Contains(component.PrefabTag) || chore.criteria == FetchChore.MatchCriteria.MatchTags && !component.HasTag(chore.tagsFirst))
    {
      Debug.Log((object) string.Format("Pickupable {0} is not valid for chore because it is not or does not contain one of these tags: {1}", (object) pickupable, (object) string.Join<Tag>(",", (IEnumerable<Tag>) chore.tags)));
      return false;
    }
    if (((Tag) ref chore.requiredTag).IsValid && !component.HasTag(chore.requiredTag))
    {
      Debug.Log((object) string.Format("Pickupable {0} is not valid for chore because it does not have the required tag: {1}", (object) pickupable, (object) chore.requiredTag));
      return false;
    }
    if (!component.HasAnyTags(chore.forbiddenTags))
      return true;
    Debug.Log((object) string.Format("Pickupable {0} is not valid for chore because it has the forbidden tags: {1}", (object) pickupable, (object) string.Join<Tag>(",", (IEnumerable<Tag>) chore.forbiddenTags)));
    return false;
  }

  public static void GatherNearbyFetchChores(
    FetchChore root_chore,
    Chore.Precondition.Context context,
    int x,
    int y,
    int radius,
    List<Chore.Precondition.Context> succeeded_contexts,
    List<Chore.Precondition.Context> failed_contexts)
  {
    ListPool<ScenePartitionerEntry, FetchAreaChore>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, FetchAreaChore>.Allocate();
    GameScenePartitioner.Instance.GatherEntries(x - radius, y - radius, radius * 2 + 1, radius * 2 + 1, GameScenePartitioner.Instance.fetchChoreLayer, (List<ScenePartitionerEntry>) gathered_entries);
    for (int index = 0; index < ((List<ScenePartitionerEntry>) gathered_entries).Count; ++index)
      (((List<ScenePartitionerEntry>) gathered_entries)[index].obj as FetchChore).CollectChoresFromGlobalChoreProvider(context.consumerState, succeeded_contexts, failed_contexts, true);
    gathered_entries.Recycle();
  }

  public class StatesInstance : 
    GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.GameInstance
  {
    private List<FetchChore> chores = new List<FetchChore>();
    private List<Pickupable> fetchables = new List<Pickupable>();
    private List<FetchAreaChore.StatesInstance.Reservation> reservations = new List<FetchAreaChore.StatesInstance.Reservation>();
    private List<Pickupable> deliverables = new List<Pickupable>();
    public List<FetchAreaChore.StatesInstance.Delivery> deliveries = new List<FetchAreaChore.StatesInstance.Delivery>();
    private FetchChore rootChore;
    private Chore.Precondition.Context rootContext;
    private float fetchAmountRequested;
    public bool delivering;
    public bool pickingup;
    private static Tag[] s_transientDeliveryTags = new Tag[2]
    {
      GameTags.Garbage,
      GameTags.Creatures.Deliverable
    };

    public StatesInstance(FetchAreaChore master, Chore.Precondition.Context context)
      : base(master)
    {
      this.rootContext = context;
      this.rootChore = context.chore as FetchChore;
    }

    public void Begin(Chore.Precondition.Context context)
    {
      this.sm.fetcher.Set(context.consumerState.gameObject, this.smi, false);
      this.chores.Clear();
      this.chores.Add(this.rootChore);
      int x1;
      int y1;
      Grid.CellToXY(Grid.PosToCell(TransformExtensions.GetPosition(this.rootChore.destination.transform)), out x1, out y1);
      ListPool<Chore.Precondition.Context, FetchAreaChore>.PooledList succeeded_contexts = ListPool<Chore.Precondition.Context, FetchAreaChore>.Allocate();
      ListPool<Chore.Precondition.Context, FetchAreaChore>.PooledList failed_contexts = ListPool<Chore.Precondition.Context, FetchAreaChore>.Allocate();
      if (this.rootChore.allowMultifetch)
        FetchAreaChore.GatherNearbyFetchChores(this.rootChore, context, x1, y1, 3, (List<Chore.Precondition.Context>) succeeded_contexts, (List<Chore.Precondition.Context>) failed_contexts);
      float num1 = Mathf.Max(1f, Db.Get().Attributes.CarryAmount.Lookup((Component) context.consumerState.consumer).GetTotalValue());
      Pickupable pickupable1 = context.data as Pickupable;
      if (Object.op_Equality((Object) pickupable1, (Object) null))
      {
        Debug.Assert(((List<Chore.Precondition.Context>) succeeded_contexts).Count > 0, (object) "succeeded_contexts was empty");
        FetchChore chore = (FetchChore) ((List<Chore.Precondition.Context>) succeeded_contexts)[0].chore;
        Debug.Assert(chore != null, (object) "fetch_chore was null");
        DebugUtil.LogWarningArgs(new object[3]
        {
          (object) "Missing root_fetchable for FetchAreaChore",
          (object) chore.destination,
          (object) chore.tagsFirst
        });
        pickupable1 = chore.FindFetchTarget(context.consumerState);
      }
      Debug.Assert(Object.op_Inequality((Object) pickupable1, (Object) null), (object) "root_fetchable was null");
      List<Pickupable> pickupableList = new List<Pickupable>();
      pickupableList.Add(pickupable1);
      float unreservedAmount1 = pickupable1.UnreservedAmount;
      float minTakeAmount = pickupable1.MinTakeAmount;
      int x2 = 0;
      int y2 = 0;
      Grid.CellToXY(Grid.PosToCell(TransformExtensions.GetPosition(pickupable1.transform)), out x2, out y2);
      int num2 = 9;
      int x_bottomLeft = x2 - 3;
      int y_bottomLeft = y2 - 3;
      ListPool<ScenePartitionerEntry, FetchAreaChore>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, FetchAreaChore>.Allocate();
      GameScenePartitioner.Instance.GatherEntries(x_bottomLeft, y_bottomLeft, num2, num2, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
      Tag prefabTag = ((Component) pickupable1).GetComponent<KPrefabID>().PrefabTag;
      for (int index = 0; index < ((List<ScenePartitionerEntry>) gathered_entries).Count; ++index)
      {
        ScenePartitionerEntry partitionerEntry = ((List<ScenePartitionerEntry>) gathered_entries)[index];
        if ((double) unreservedAmount1 <= (double) num1)
        {
          Pickupable pickupable2 = partitionerEntry.obj as Pickupable;
          KPrefabID component = ((Component) pickupable2).GetComponent<KPrefabID>();
          if (!Tag.op_Inequality(component.PrefabTag, prefabTag) && (double) pickupable2.UnreservedAmount > 0.0 && (this.rootChore.criteria != FetchChore.MatchCriteria.MatchID || this.rootChore.tags.Contains(component.PrefabTag)) && (this.rootChore.criteria != FetchChore.MatchCriteria.MatchTags || component.HasTag(this.rootChore.tagsFirst)) && (!((Tag) ref this.rootChore.requiredTag).IsValid || component.HasTag(this.rootChore.requiredTag)) && !component.HasAnyTags(this.rootChore.forbiddenTags) && !pickupableList.Contains(pickupable2) && this.rootContext.consumerState.consumer.CanReach((IApproachable) pickupable2))
          {
            float unreservedAmount2 = pickupable2.UnreservedAmount;
            pickupableList.Add(pickupable2);
            unreservedAmount1 += unreservedAmount2;
            if (pickupableList.Count >= 10)
              break;
          }
        }
        else
          break;
      }
      gathered_entries.Recycle();
      float num3 = Mathf.Min(num1, unreservedAmount1);
      if ((double) minTakeAmount > 0.0)
        num3 -= num3 % minTakeAmount;
      this.deliveries.Clear();
      float amount_to_be_fetched1 = Mathf.Min(this.rootChore.originalAmount, num3);
      if ((double) minTakeAmount > 0.0)
        amount_to_be_fetched1 -= amount_to_be_fetched1 % minTakeAmount;
      this.deliveries.Add(new FetchAreaChore.StatesInstance.Delivery(this.rootContext, amount_to_be_fetched1, new Action<FetchChore>(this.OnFetchChoreCancelled)));
      float num4 = amount_to_be_fetched1;
      for (int index = 0; index < ((List<Chore.Precondition.Context>) succeeded_contexts).Count && (double) num4 < (double) num3; ++index)
      {
        Chore.Precondition.Context context1 = ((List<Chore.Precondition.Context>) succeeded_contexts)[index];
        FetchChore chore = context1.chore as FetchChore;
        if (chore != this.rootChore && Object.op_Equality((Object) chore.overrideTarget, (Object) null) && Object.op_Equality((Object) chore.driver, (Object) null) && chore.tagsHash == this.rootChore.tagsHash && Tag.op_Equality(chore.requiredTag, this.rootChore.requiredTag) && chore.forbidHash == this.rootChore.forbidHash)
        {
          float amount_to_be_fetched2 = Mathf.Min(chore.originalAmount, num3 - num4);
          if ((double) minTakeAmount > 0.0)
            amount_to_be_fetched2 -= amount_to_be_fetched2 % minTakeAmount;
          this.chores.Add(chore);
          this.deliveries.Add(new FetchAreaChore.StatesInstance.Delivery(context1, amount_to_be_fetched2, new Action<FetchChore>(this.OnFetchChoreCancelled)));
          num4 += amount_to_be_fetched2;
          if (this.deliveries.Count >= 10)
            break;
        }
      }
      float num5 = Mathf.Min(num4, num3);
      float num6 = num5;
      this.fetchables.Clear();
      for (int index = 0; index < pickupableList.Count && (double) num6 > 0.0; ++index)
      {
        Pickupable pickupable3 = pickupableList[index];
        num6 -= pickupable3.UnreservedAmount;
        this.fetchables.Add(pickupable3);
      }
      this.fetchAmountRequested = num5;
      this.reservations.Clear();
      succeeded_contexts.Recycle();
      failed_contexts.Recycle();
    }

    public void End()
    {
      foreach (FetchAreaChore.StatesInstance.Delivery delivery in this.deliveries)
        delivery.Cleanup();
      this.deliveries.Clear();
    }

    public void SetupDelivery()
    {
      if (this.deliveries.Count == 0)
      {
        this.StopSM("FetchAreaChoreComplete");
      }
      else
      {
        FetchAreaChore.StatesInstance.Delivery nextDelivery = this.deliveries[0];
        if (((IEnumerable<Tag>) FetchAreaChore.StatesInstance.s_transientDeliveryTags).Contains<Tag>(nextDelivery.chore.requiredTag))
          nextDelivery.chore.requiredTag = Tag.Invalid;
        this.deliverables.RemoveAll((Predicate<Pickupable>) (x =>
        {
          if (Object.op_Equality((Object) x, (Object) null) || (double) x.TotalAmount <= 0.0)
            return true;
          if (FetchAreaChore.IsPickupableStillValidForChore(x, nextDelivery.chore))
            return false;
          Debug.LogWarning((object) string.Format("Removing deliverable {0} for a delivery to {1} which did not request it", (object) x, (object) nextDelivery.chore.destination));
          return true;
        }));
        if (this.deliverables.Count == 0)
        {
          this.StopSM("FetchAreaChoreComplete");
        }
        else
        {
          this.sm.deliveryDestination.Set((KMonoBehaviour) nextDelivery.destination, this.smi);
          this.sm.deliveryObject.Set((KMonoBehaviour) this.deliverables[0], this.smi);
          if (Object.op_Inequality((Object) nextDelivery.destination, (Object) null))
          {
            if (this.rootContext.consumerState.hasSolidTransferArm)
            {
              if (this.rootContext.consumerState.consumer.IsWithinReach((IApproachable) this.deliveries[0].destination))
                this.GoTo((StateMachine.BaseState) this.sm.delivering.storing);
              else
                this.GoTo((StateMachine.BaseState) this.sm.delivering.deliverfail);
            }
            else
              this.GoTo((StateMachine.BaseState) this.sm.delivering.movetostorage);
          }
          else
            this.smi.GoTo((StateMachine.BaseState) this.sm.delivering.deliverfail);
        }
      }
    }

    public void SetupFetch()
    {
      if (this.reservations.Count > 0)
      {
        this.sm.fetchTarget.Set((KMonoBehaviour) this.reservations[0].pickupable, this.smi);
        this.sm.fetchResultTarget.Set((KMonoBehaviour) null, this.smi);
        double num = (double) this.sm.fetchAmount.Set(this.reservations[0].amount, this.smi);
        if (Object.op_Inequality((Object) this.reservations[0].pickupable, (Object) null))
        {
          if (this.rootContext.consumerState.hasSolidTransferArm)
          {
            if (this.rootContext.consumerState.consumer.IsWithinReach((IApproachable) this.reservations[0].pickupable))
              this.GoTo((StateMachine.BaseState) this.sm.fetching.pickup);
            else
              this.GoTo((StateMachine.BaseState) this.sm.fetching.fetchfail);
          }
          else
            this.GoTo((StateMachine.BaseState) this.sm.fetching.movetopickupable);
        }
        else
          this.GoTo((StateMachine.BaseState) this.sm.fetching.fetchfail);
      }
      else
        this.GoTo((StateMachine.BaseState) this.sm.delivering.next);
    }

    public void DeliverFail()
    {
      if (this.deliveries.Count > 0)
      {
        this.deliveries[0].Cleanup();
        this.deliveries.RemoveAt(0);
      }
      this.GoTo((StateMachine.BaseState) this.sm.delivering.next);
    }

    public void DeliverComplete()
    {
      Pickupable pickupable = this.sm.deliveryObject.Get<Pickupable>(this.smi);
      if (Object.op_Equality((Object) pickupable, (Object) null) || (double) pickupable.TotalAmount <= 0.0)
      {
        if (this.deliveries.Count > 0 && (double) this.deliveries[0].chore.amount < (double) PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
        {
          FetchAreaChore.StatesInstance.Delivery delivery = this.deliveries[0];
          Chore chore = (Chore) delivery.chore;
          delivery.Complete(this.deliverables);
          delivery.Cleanup();
          if (this.deliveries.Count > 0 && this.deliveries[0].chore == chore)
            this.deliveries.RemoveAt(0);
          this.GoTo((StateMachine.BaseState) this.sm.delivering.next);
        }
        else
          this.smi.GoTo((StateMachine.BaseState) this.sm.delivering.deliverfail);
      }
      else
      {
        if (this.deliveries.Count > 0)
        {
          FetchAreaChore.StatesInstance.Delivery delivery = this.deliveries[0];
          Chore chore = (Chore) delivery.chore;
          delivery.Complete(this.deliverables);
          delivery.Cleanup();
          if (this.deliveries.Count > 0 && this.deliveries[0].chore == chore)
            this.deliveries.RemoveAt(0);
        }
        this.GoTo((StateMachine.BaseState) this.sm.delivering.next);
      }
    }

    public void FetchFail()
    {
      this.reservations[0].Cleanup();
      this.reservations.RemoveAt(0);
      this.GoTo((StateMachine.BaseState) this.sm.fetching.next);
    }

    public void FetchComplete()
    {
      this.reservations[0].Cleanup();
      this.reservations.RemoveAt(0);
      this.GoTo((StateMachine.BaseState) this.sm.fetching.next);
    }

    public void SetupDeliverables()
    {
      foreach (GameObject gameObject in this.sm.fetcher.Get<Storage>(this.smi).items)
      {
        if (!Object.op_Equality((Object) gameObject, (Object) null))
        {
          KPrefabID component1 = gameObject.GetComponent<KPrefabID>();
          if (!Object.op_Equality((Object) component1, (Object) null))
          {
            Pickupable component2 = ((Component) component1).GetComponent<Pickupable>();
            if (Object.op_Inequality((Object) component2, (Object) null))
              this.deliverables.Add(component2);
          }
        }
      }
    }

    public void ReservePickupables()
    {
      ChoreConsumer consumer = this.sm.fetcher.Get<ChoreConsumer>(this.smi);
      float fetchAmountRequested = this.fetchAmountRequested;
      foreach (Pickupable fetchable in this.fetchables)
      {
        if ((double) fetchAmountRequested <= 0.0)
          break;
        float reservation_amount = Math.Min(fetchAmountRequested, fetchable.UnreservedAmount);
        fetchAmountRequested -= reservation_amount;
        this.reservations.Add(new FetchAreaChore.StatesInstance.Reservation(consumer, fetchable, reservation_amount));
      }
    }

    private void OnFetchChoreCancelled(FetchChore chore)
    {
      for (int index = 0; index < this.deliveries.Count; ++index)
      {
        if (this.deliveries[index].chore == chore)
        {
          if (this.deliveries.Count == 1)
          {
            this.StopSM("AllDelivericesCancelled");
            break;
          }
          if (index == 0)
          {
            this.sm.currentdeliverycancelled.Trigger(this);
            break;
          }
          this.deliveries[index].Cleanup();
          this.deliveries.RemoveAt(index);
          break;
        }
      }
    }

    public void UnreservePickupables()
    {
      foreach (FetchAreaChore.StatesInstance.Reservation reservation in this.reservations)
        reservation.Cleanup();
      this.reservations.Clear();
    }

    public bool SameDestination(FetchChore fetch)
    {
      foreach (FetchChore chore in this.chores)
      {
        if (Object.op_Equality((Object) chore.destination, (Object) fetch.destination))
          return true;
      }
      return false;
    }

    public struct Delivery
    {
      private Action<FetchChore> onCancelled;
      private Action<Chore> onFetchChoreCleanup;

      public Storage destination { get; private set; }

      public float amount { get; private set; }

      public FetchChore chore { get; private set; }

      public Delivery(
        Chore.Precondition.Context context,
        float amount_to_be_fetched,
        Action<FetchChore> on_cancelled)
        : this()
      {
        this.chore = context.chore as FetchChore;
        this.amount = this.chore.originalAmount;
        this.destination = this.chore.destination;
        this.chore.SetOverrideTarget(context.consumerState.consumer);
        this.onCancelled = on_cancelled;
        this.onFetchChoreCleanup = new Action<Chore>(this.OnFetchChoreCleanup);
        this.chore.FetchAreaBegin(context, amount_to_be_fetched);
        FetchChore chore = this.chore;
        chore.onCleanup = chore.onCleanup + this.onFetchChoreCleanup;
      }

      public void Complete(List<Pickupable> deliverables)
      {
        KProfiler.Region region;
        // ISSUE: explicit constructor call
        ((KProfiler.Region) ref region).\u002Ector("FAC.Delivery.Complete", (Object) null);
        try
        {
          if (Object.op_Equality((Object) this.destination, (Object) null) || this.destination.IsEndOfLife())
            return;
          FetchChore chore = this.chore;
          chore.onCleanup = chore.onCleanup - this.onFetchChoreCleanup;
          float amount = this.amount;
          Pickupable pickupable1 = (Pickupable) null;
          for (int index = 0; index < deliverables.Count && (double) amount > 0.0; ++index)
          {
            if (Object.op_Equality((Object) deliverables[index], (Object) null))
            {
              if ((double) amount < (double) PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT)
                this.destination.ForceStore(this.chore.tagsFirst, amount);
            }
            else if (!FetchAreaChore.IsPickupableStillValidForChore(deliverables[index], this.chore))
            {
              Debug.LogError((object) string.Format("Attempting to store {0} in a {1} which did not request it", (object) deliverables[index], (object) this.destination));
            }
            else
            {
              Pickupable pickupable2 = deliverables[index].Take(amount);
              if (Object.op_Inequality((Object) pickupable2, (Object) null) && (double) pickupable2.TotalAmount > 0.0)
              {
                amount -= pickupable2.TotalAmount;
                this.destination.Store(((Component) pickupable2).gameObject);
                pickupable1 = pickupable2;
                if (Object.op_Equality((Object) pickupable2, (Object) deliverables[index]))
                  deliverables[index] = (Pickupable) null;
              }
            }
          }
          if (Object.op_Inequality((Object) this.chore.overrideTarget, (Object) null))
            this.chore.FetchAreaEnd(((Component) this.chore.overrideTarget).GetComponent<ChoreDriver>(), pickupable1, true);
          this.chore = (FetchChore) null;
        }
        finally
        {
          region.Dispose();
        }
      }

      private void OnFetchChoreCleanup(Chore chore)
      {
        if (this.onCancelled == null)
          return;
        this.onCancelled(chore as FetchChore);
      }

      public void Cleanup()
      {
        if (this.chore == null)
          return;
        FetchChore chore = this.chore;
        chore.onCleanup = chore.onCleanup - this.onFetchChoreCleanup;
        this.chore.FetchAreaEnd((ChoreDriver) null, (Pickupable) null, false);
      }
    }

    public struct Reservation
    {
      private int handle;

      public float amount { get; private set; }

      public Pickupable pickupable { get; private set; }

      public Reservation(ChoreConsumer consumer, Pickupable pickupable, float reservation_amount)
        : this()
      {
        if ((double) reservation_amount <= 0.0)
          Debug.LogError((object) ("Invalid amount: " + reservation_amount.ToString()));
        this.amount = reservation_amount;
        this.pickupable = pickupable;
        this.handle = pickupable.Reserve(nameof (FetchAreaChore), ((Component) consumer).gameObject, reservation_amount);
      }

      public void Cleanup()
      {
        if (!Object.op_Inequality((Object) this.pickupable, (Object) null))
          return;
        this.pickupable.Unreserve(nameof (FetchAreaChore), this.handle);
      }
    }
  }

  public class States : 
    GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore>
  {
    public FetchAreaChore.States.FetchStates fetching;
    public FetchAreaChore.States.DeliverStates delivering;
    public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter fetcher;
    public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter fetchTarget;
    public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter fetchResultTarget;
    public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.FloatParameter fetchAmount;
    public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter deliveryDestination;
    public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.TargetParameter deliveryObject;
    public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.FloatParameter deliveryAmount;
    public StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.Signal currentdeliverycancelled;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.fetching;
      this.Target(this.fetcher);
      this.fetching.DefaultState(this.fetching.next).Enter("ReservePickupables", (StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.ReservePickupables())).Exit("UnreservePickupables", (StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.UnreservePickupables())).Enter("pickingup-on", (StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.pickingup = true)).Exit("pickingup-off", (StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.pickingup = false));
      this.fetching.next.Enter("SetupFetch", (StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.SetupFetch()));
      this.fetching.movetopickupable.InitializeStates(this.fetcher, this.fetchTarget, this.fetching.pickup, this.fetching.fetchfail, tactic: NavigationTactics.ReduceTravelDistance);
      this.fetching.pickup.DoPickup(this.fetchTarget, this.fetchResultTarget, this.fetchAmount, this.fetching.fetchcomplete, this.fetching.fetchfail);
      this.fetching.fetchcomplete.Enter((StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.FetchComplete()));
      this.fetching.fetchfail.Enter((StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.FetchFail()));
      this.delivering.DefaultState(this.delivering.next).OnSignal(this.currentdeliverycancelled, this.delivering.deliverfail).Enter("SetupDeliverables", (StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.SetupDeliverables())).Enter("delivering-on", (StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.delivering = true)).Exit("delivering-off", (StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.delivering = false));
      this.delivering.next.Enter("SetupDelivery", (StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.SetupDelivery()));
      this.delivering.movetostorage.InitializeStates(this.fetcher, this.deliveryDestination, this.delivering.storing, this.delivering.deliverfail, tactic: NavigationTactics.ReduceTravelDistance).Enter((StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) this.deliveryObject.Get(smi), (Object) null) || !Object.op_Inequality((Object) this.deliveryObject.Get(smi).GetComponent<MinionIdentity>(), (Object) null))
          return;
        TransformExtensions.SetLocalPosition(this.deliveryObject.Get(smi).transform, Vector3.zero);
        KBatchedAnimTracker component = this.deliveryObject.Get(smi).GetComponent<KBatchedAnimTracker>();
        component.symbol = new HashedString("snapTo_chest");
        component.offset = new Vector3(0.0f, 0.0f, 1f);
      }));
      this.delivering.storing.DoDelivery(this.fetcher, this.deliveryDestination, this.delivering.delivercomplete, this.delivering.deliverfail);
      this.delivering.deliverfail.Enter((StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.DeliverFail()));
      this.delivering.delivercomplete.Enter((StateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State.Callback) (smi => smi.DeliverComplete()));
    }

    public class FetchStates : 
      GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State
    {
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State next;
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.ApproachSubState<Pickupable> movetopickupable;
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State pickup;
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State fetchfail;
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State fetchcomplete;
    }

    public class DeliverStates : 
      GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State
    {
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State next;
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.ApproachSubState<Storage> movetostorage;
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State storing;
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State deliverfail;
      public GameStateMachine<FetchAreaChore.States, FetchAreaChore.StatesInstance, FetchAreaChore, object>.State delivercomplete;
    }
  }
}
