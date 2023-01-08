// Decompiled with JetBrains decompiler
// Type: ChorePreconditions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class ChorePreconditions
{
  private static ChorePreconditions _instance;
  public Chore.Precondition IsPreemptable;
  public Chore.Precondition HasUrge;
  public Chore.Precondition IsValid;
  public Chore.Precondition IsPermitted;
  public Chore.Precondition IsAssignedtoMe;
  public Chore.Precondition IsInMyWorld;
  public Chore.Precondition IsInMyParentWorld;
  public Chore.Precondition IsCellNotInMyWorld;
  public Chore.Precondition IsInMyRoom;
  public Chore.Precondition IsPreferredAssignable;
  public Chore.Precondition IsPreferredAssignableOrUrgentBladder;
  public Chore.Precondition IsNotTransferArm;
  public Chore.Precondition HasSkillPerk;
  public Chore.Precondition IsMinion;
  public Chore.Precondition IsMoreSatisfyingEarly;
  public Chore.Precondition IsMoreSatisfyingLate;
  public Chore.Precondition IsChattable;
  public Chore.Precondition IsNotRedAlert;
  public Chore.Precondition IsScheduledTime;
  public Chore.Precondition CanMoveTo;
  public Chore.Precondition CanMoveToCell;
  public Chore.Precondition CanPickup;
  public Chore.Precondition IsAwake;
  public Chore.Precondition IsStanding;
  public Chore.Precondition IsMoving;
  public Chore.Precondition IsOffLadder;
  public Chore.Precondition NotInTube;
  public Chore.Precondition ConsumerHasTrait;
  public Chore.Precondition IsOperational;
  public Chore.Precondition IsNotMarkedForDeconstruction;
  public Chore.Precondition IsNotMarkedForDisable;
  public Chore.Precondition IsFunctional;
  public Chore.Precondition IsOverrideTargetNullOrMe;
  public Chore.Precondition NotChoreCreator;
  public Chore.Precondition IsGettingMoreStressed;
  public Chore.Precondition IsAllowedByAutomation;
  public Chore.Precondition HasTag;
  public Chore.Precondition CheckBehaviourPrecondition;
  public Chore.Precondition CanDoWorkerPrioritizable;
  public Chore.Precondition IsExclusivelyAvailableWithOtherChores;
  public Chore.Precondition IsBladderFull;
  public Chore.Precondition IsBladderNotFull;
  public Chore.Precondition NoDeadBodies;
  public Chore.Precondition IsNotARobot;
  public Chore.Precondition NotCurrentlyPeeing;
  public Chore.Precondition IsRocketTravelling;

  public static ChorePreconditions instance
  {
    get
    {
      if (ChorePreconditions._instance == null)
        ChorePreconditions._instance = new ChorePreconditions();
      return ChorePreconditions._instance;
    }
  }

  public static void DestroyInstance() => ChorePreconditions._instance = (ChorePreconditions) null;

  public ChorePreconditions()
  {
    Chore.Precondition precondition = new Chore.Precondition();
    precondition.id = nameof (IsPreemptable);
    precondition.sortOrder = 1;
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_PREEMPTABLE;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => context.isAttemptingOverride || context.chore.CanPreempt(context) || Object.op_Equality((Object) context.chore.driver, (Object) null));
    this.IsPreemptable = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (HasUrge);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.HAS_URGE;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (context.chore.choreType.urge == null)
        return true;
      foreach (Urge urge in context.consumerState.consumer.GetUrges())
      {
        if (context.chore.SatisfiesUrge(urge))
          return true;
      }
      return false;
    });
    this.HasUrge = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsValid);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_VALID;
    precondition.sortOrder = -4;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => !context.chore.isNull && context.chore.IsValid());
    this.IsValid = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsPermitted);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_PERMITTED;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => context.consumerState.consumer.IsPermittedOrEnabled(context.choreTypeForPermission, context.chore));
    this.IsPermitted = precondition;
    precondition = new Chore.Precondition();
    precondition.id = "IsAssignedToMe";
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_ASSIGNED_TO_ME;
    precondition.sortOrder = 10;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Assignable assignable = (Assignable) data;
      IAssignableIdentity component = context.consumerState.gameObject.GetComponent<IAssignableIdentity>();
      return component != null && assignable.IsAssignedTo(component);
    });
    this.IsAssignedtoMe = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsInMyWorld);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_IN_MY_WORLD;
    precondition.sortOrder = -1;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => !context.chore.isNull && context.chore.gameObject.IsMyWorld(context.consumerState.gameObject));
    this.IsInMyWorld = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsInMyParentWorld);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_IN_MY_WORLD;
    precondition.sortOrder = -1;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => !context.chore.isNull && context.chore.gameObject.IsMyParentWorld(context.consumerState.gameObject));
    this.IsInMyParentWorld = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsCellNotInMyWorld);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_CELL_NOT_IN_MY_WORLD;
    precondition.sortOrder = -1;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (context.chore.isNull)
        return false;
      int cell = (int) data;
      return !Grid.IsValidCell(cell) || (int) Grid.WorldIdx[cell] != context.consumerState.gameObject.GetMyWorldId();
    });
    this.IsCellNotInMyWorld = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsInMyRoom);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_IN_MY_ROOM;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      CavityInfo cavityForCell1 = Game.Instance.roomProber.GetCavityForCell((int) data);
      Room room1 = (Room) null;
      if (cavityForCell1 != null)
        room1 = cavityForCell1.room;
      if (room1 != null)
      {
        if (Object.op_Inequality((Object) context.consumerState.ownable, (Object) null))
        {
          foreach (Component owner in room1.GetOwners())
          {
            if (Object.op_Equality((Object) owner.gameObject, (Object) context.consumerState.gameObject))
              return true;
          }
        }
        else
        {
          Room room2 = (Room) null;
          if (context.chore is FetchChore chore2 && Object.op_Inequality((Object) chore2.destination, (Object) null))
          {
            CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell((KMonoBehaviour) chore2.destination));
            if (cavityForCell2 != null)
              room2 = cavityForCell2.room;
            return room2 != null && room2 == room1;
          }
          if (!(context.chore is WorkChore<Tinkerable>))
            return false;
          CavityInfo cavityForCell3 = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell((context.chore as WorkChore<Tinkerable>).gameObject));
          if (cavityForCell3 != null)
            room2 = cavityForCell3.room;
          return room2 != null && room2 == room1;
        }
      }
      return false;
    });
    this.IsInMyRoom = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsPreferredAssignable);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_PREFERRED_ASSIGNABLE;
    precondition.sortOrder = 10;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Assignable assignable = (Assignable) data;
      return Game.Instance.assignmentManager.GetPreferredAssignables(context.consumerState.assignables, assignable.slot).Contains(assignable);
    });
    this.IsPreferredAssignable = precondition;
    precondition = new Chore.Precondition();
    precondition.id = "IsPreferredAssignableOrUrgent";
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_PREFERRED_ASSIGNABLE_OR_URGENT_BLADDER;
    precondition.sortOrder = 10;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Assignable candidate = (Assignable) data;
      if (Game.Instance.assignmentManager.IsPreferredAssignable(context.consumerState.assignables, candidate))
        return true;
      PeeChoreMonitor.Instance smi = context.consumerState.gameObject.GetSMI<PeeChoreMonitor.Instance>();
      return smi != null && smi.IsInsideState((StateMachine.BaseState) smi.sm.critical);
    });
    this.IsPreferredAssignableOrUrgentBladder = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsNotTransferArm);
    precondition.description = "";
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => !context.consumerState.hasSolidTransferArm);
    this.IsNotTransferArm = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (HasSkillPerk);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.HAS_SKILL_PERK;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      MinionResume resume = context.consumerState.resume;
      if (!Object.op_Implicit((Object) resume))
        return false;
      switch (data)
      {
        case SkillPerk _:
          SkillPerk perk = data as SkillPerk;
          return resume.HasPerk(perk);
        case HashedString _:
          HashedString perkId1 = (HashedString) data;
          return resume.HasPerk(perkId1);
        case string _:
          HashedString perkId2 = HashedString.op_Implicit((string) data);
          return resume.HasPerk(perkId2);
        default:
          return false;
      }
    });
    this.HasSkillPerk = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsMinion);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_MINION;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => Object.op_Inequality((Object) context.consumerState.resume, (Object) null));
    this.IsMinion = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsMoreSatisfyingEarly);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_MORE_SATISFYING;
    precondition.sortOrder = -2;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (context.isAttemptingOverride || context.skipMoreSatisfyingEarlyPrecondition || context.consumerState.selectable.IsSelected)
        return true;
      Chore currentChore = context.consumerState.choreDriver.GetCurrentChore();
      if (currentChore == null)
        return true;
      if (context.masterPriority.priority_class != currentChore.masterPriority.priority_class)
        return context.masterPriority.priority_class > currentChore.masterPriority.priority_class;
      if (Object.op_Inequality((Object) context.consumerState.consumer, (Object) null) && context.personalPriority != context.consumerState.consumer.GetPersonalPriority(currentChore.choreType))
        return context.personalPriority > context.consumerState.consumer.GetPersonalPriority(currentChore.choreType);
      return context.masterPriority.priority_value != currentChore.masterPriority.priority_value ? context.masterPriority.priority_value > currentChore.masterPriority.priority_value : context.priority > currentChore.choreType.priority;
    });
    this.IsMoreSatisfyingEarly = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsMoreSatisfyingLate);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_MORE_SATISFYING;
    precondition.sortOrder = 10000;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (context.isAttemptingOverride || !context.consumerState.selectable.IsSelected)
        return true;
      Chore currentChore = context.consumerState.choreDriver.GetCurrentChore();
      if (currentChore == null)
        return true;
      if (context.masterPriority.priority_class != currentChore.masterPriority.priority_class)
        return context.masterPriority.priority_class > currentChore.masterPriority.priority_class;
      if (Object.op_Inequality((Object) context.consumerState.consumer, (Object) null) && context.personalPriority != context.consumerState.consumer.GetPersonalPriority(currentChore.choreType))
        return context.personalPriority > context.consumerState.consumer.GetPersonalPriority(currentChore.choreType);
      return context.masterPriority.priority_value != currentChore.masterPriority.priority_value ? context.masterPriority.priority_value > currentChore.masterPriority.priority_value : context.priority > currentChore.choreType.priority;
    });
    this.IsMoreSatisfyingLate = precondition;
    precondition = new Chore.Precondition();
    precondition.id = "CanChat";
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CAN_CHAT;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      KMonoBehaviour kmonoBehaviour = (KMonoBehaviour) data;
      return !Object.op_Equality((Object) context.consumerState.consumer, (Object) null) && !Object.op_Equality((Object) context.consumerState.navigator, (Object) null) && !Object.op_Equality((Object) kmonoBehaviour, (Object) null) && context.consumerState.navigator.CanReach((IApproachable) ((Component) kmonoBehaviour).GetComponent<Chattable>());
    });
    this.IsChattable = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsNotRedAlert);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_RED_ALERT;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => context.chore.masterPriority.priority_class == PriorityScreen.PriorityClass.topPriority || !context.chore.gameObject.GetMyWorld().IsRedAlert());
    this.IsNotRedAlert = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsScheduledTime);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_SCHEDULED_TIME;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (context.chore.gameObject.GetMyWorld().IsRedAlert())
        return true;
      ScheduleBlockType type = (ScheduleBlockType) data;
      ScheduleBlock scheduleBlock = context.consumerState.scheduleBlock;
      return scheduleBlock == null || scheduleBlock.IsAllowed(type);
    });
    this.IsScheduledTime = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (CanMoveTo);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (Object.op_Equality((Object) context.consumerState.consumer, (Object) null))
        return false;
      KMonoBehaviour kmonoBehaviour = (KMonoBehaviour) data;
      if (Object.op_Equality((Object) kmonoBehaviour, (Object) null))
        return false;
      IApproachable approachable = (IApproachable) kmonoBehaviour;
      int cost;
      if (!context.consumerState.consumer.GetNavigationCost(approachable, out cost))
        return false;
      context.cost += cost;
      return true;
    });
    this.CanMoveTo = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (CanMoveToCell);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CAN_MOVE_TO;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (Object.op_Equality((Object) context.consumerState.consumer, (Object) null))
        return false;
      int cell = (int) data;
      int cost;
      if (!Grid.IsValidCell(cell) || !context.consumerState.consumer.GetNavigationCost(cell, out cost))
        return false;
      context.cost += cost;
      return true;
    });
    this.CanMoveToCell = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (CanPickup);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CAN_PICKUP;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Pickupable cmp = (Pickupable) data;
      return !Object.op_Equality((Object) cmp, (Object) null) && !Object.op_Equality((Object) context.consumerState.consumer, (Object) null) && !((Component) cmp).HasTag(GameTags.StoredPrivate) && cmp.CouldBePickedUpByMinion(context.consumerState.gameObject) && context.consumerState.consumer.CanReach((IApproachable) cmp);
    });
    this.CanPickup = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsAwake);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_AWAKE;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (Object.op_Equality((Object) context.consumerState.consumer, (Object) null))
        return false;
      StaminaMonitor.Instance smi = ((Component) context.consumerState.consumer).GetSMI<StaminaMonitor.Instance>();
      return !smi.IsInsideState((StateMachine.BaseState) smi.sm.sleepy.sleeping);
    });
    this.IsAwake = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsStanding);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_STANDING;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => !Object.op_Equality((Object) context.consumerState.consumer, (Object) null) && !Object.op_Equality((Object) context.consumerState.navigator, (Object) null) && context.consumerState.navigator.CurrentNavType == NavType.Floor);
    this.IsStanding = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsMoving);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_MOVING;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => !Object.op_Equality((Object) context.consumerState.consumer, (Object) null) && !Object.op_Equality((Object) context.consumerState.navigator, (Object) null) && context.consumerState.navigator.IsMoving());
    this.IsMoving = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsOffLadder);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_OFF_LADDER;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => !Object.op_Equality((Object) context.consumerState.consumer, (Object) null) && !Object.op_Equality((Object) context.consumerState.navigator, (Object) null) && context.consumerState.navigator.CurrentNavType != NavType.Ladder && context.consumerState.navigator.CurrentNavType != NavType.Pole);
    this.IsOffLadder = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (NotInTube);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.NOT_IN_TUBE;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => !Object.op_Equality((Object) context.consumerState.consumer, (Object) null) && !Object.op_Equality((Object) context.consumerState.navigator, (Object) null) && context.consumerState.navigator.CurrentNavType != NavType.Tube);
    this.NotInTube = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (ConsumerHasTrait);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.HAS_TRAIT;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      string trait_id = (string) data;
      Traits traits = context.consumerState.traits;
      return !Object.op_Equality((Object) traits, (Object) null) && traits.HasTrait(trait_id);
    });
    this.ConsumerHasTrait = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsOperational);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_OPERATIONAL;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => (data as Operational).IsOperational);
    this.IsOperational = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsNotMarkedForDeconstruction);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DECONSTRUCTION;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Deconstructable deconstructable = data as Deconstructable;
      return Object.op_Equality((Object) deconstructable, (Object) null) || !deconstructable.IsMarkedForDeconstruction();
    });
    this.IsNotMarkedForDeconstruction = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsNotMarkedForDisable);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DISABLE;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      BuildingEnabledButton buildingEnabledButton = data as BuildingEnabledButton;
      if (Object.op_Equality((Object) buildingEnabledButton, (Object) null))
        return true;
      return buildingEnabledButton.IsEnabled && !buildingEnabledButton.WaitingForDisable;
    });
    this.IsNotMarkedForDisable = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsFunctional);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_FUNCTIONAL;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => (data as Operational).IsFunctional);
    this.IsFunctional = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsOverrideTargetNullOrMe);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_OVERRIDE_TARGET_NULL_OR_ME;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => context.isAttemptingOverride || Object.op_Equality((Object) context.chore.overrideTarget, (Object) null) || Object.op_Equality((Object) context.chore.overrideTarget, (Object) context.consumerState.consumer));
    this.IsOverrideTargetNullOrMe = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (NotChoreCreator);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.NOT_CHORE_CREATOR;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      GameObject gameObject = (GameObject) data;
      return !Object.op_Equality((Object) context.consumerState.consumer, (Object) null) && !Object.op_Equality((Object) context.consumerState.gameObject, (Object) gameObject);
    });
    this.NotChoreCreator = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsGettingMoreStressed);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_GETTING_MORE_STRESSED;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => (double) Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject).GetDelta() > 0.0);
    this.IsGettingMoreStressed = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsAllowedByAutomation);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_ALLOWED_BY_AUTOMATION;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => ((Automatable) data).AllowedByAutomation(context.consumerState.hasSolidTransferArm));
    this.IsAllowedByAutomation = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (HasTag);
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Tag tag = (Tag) data;
      return context.consumerState.prefabid.HasTag(tag);
    });
    this.HasTag = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (CheckBehaviourPrecondition);
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Tag tag = (Tag) data;
      return context.consumerState.consumer.RunBehaviourPrecondition(tag);
    });
    this.CheckBehaviourPrecondition = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (CanDoWorkerPrioritizable);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CAN_DO_RECREATION;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      if (Object.op_Equality((Object) context.consumerState.consumer, (Object) null) || !(data is IWorkerPrioritizable workerPrioritizable2))
        return false;
      int priority = 0;
      if (!workerPrioritizable2.GetWorkerPriority(context.consumerState.worker, out priority))
        return false;
      context.consumerPriority += priority;
      return true;
    });
    this.CanDoWorkerPrioritizable = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsExclusivelyAvailableWithOtherChores);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.EXCLUSIVELY_AVAILABLE;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      foreach (Chore chore3 in (List<Chore>) data)
      {
        if (chore3 != context.chore && Object.op_Inequality((Object) chore3.driver, (Object) null))
          return false;
      }
      return true;
    });
    this.IsExclusivelyAvailableWithOtherChores = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsBladderFull);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.BLADDER_FULL;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      BladderMonitor.Instance smi = context.consumerState.gameObject.GetSMI<BladderMonitor.Instance>();
      return smi != null && smi.NeedsToPee();
    });
    this.IsBladderFull = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsBladderNotFull);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.BLADDER_NOT_FULL;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      BladderMonitor.Instance smi = context.consumerState.gameObject.GetSMI<BladderMonitor.Instance>();
      return smi == null || !smi.NeedsToPee();
    });
    this.IsBladderNotFull = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (NoDeadBodies);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.NO_DEAD_BODIES;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => Components.LiveMinionIdentities.Count == Components.MinionIdentities.Count);
    this.NoDeadBodies = precondition;
    precondition = new Chore.Precondition();
    precondition.id = "NoRobots";
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.NOT_A_ROBOT;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => Object.op_Inequality((Object) context.consumerState.gameObject.GetComponent<MinionResume>(), (Object) null));
    this.IsNotARobot = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (NotCurrentlyPeeing);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.CURRENTLY_PEEING;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      bool flag = true;
      Chore currentChore = context.consumerState.choreDriver.GetCurrentChore();
      if (currentChore != null)
      {
        string id = currentChore.choreType.Id;
        flag = id != Db.Get().ChoreTypes.BreakPee.Id && id != Db.Get().ChoreTypes.Pee.Id;
      }
      return flag;
    });
    this.NotCurrentlyPeeing = precondition;
    precondition = new Chore.Precondition();
    precondition.id = nameof (IsRocketTravelling);
    precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_ROCKET_TRAVELLING;
    precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Clustercraft component = ((Component) ClusterManager.Instance.GetWorld(context.chore.gameObject.GetMyWorldId())).GetComponent<Clustercraft>();
      return !Object.op_Equality((Object) component, (Object) null) && component.IsTravellingAndFueled();
    });
    this.IsRocketTravelling = precondition;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }
}
