// Decompiled with JetBrains decompiler
// Type: GameplayEventPreconditions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using System;
using System.Linq;
using UnityEngine;

public class GameplayEventPreconditions
{
  private static GameplayEventPreconditions _instance;

  public static GameplayEventPreconditions Instance
  {
    get
    {
      if (GameplayEventPreconditions._instance == null)
        GameplayEventPreconditions._instance = new GameplayEventPreconditions();
      return GameplayEventPreconditions._instance;
    }
  }

  public GameplayEventPrecondition LiveMinions(int count = 1) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => Components.LiveMinionIdentities.Count >= count),
    description = string.Format("At least {0} dupes alive", (object) count)
  };

  public GameplayEventPrecondition BuildingExists(string buildingId, int count = 1) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => BuildingInventory.Instance.BuildingCount(new Tag(buildingId)) >= count),
    description = string.Format("{0} {1} has been built", (object) count, (object) buildingId)
  };

  public GameplayEventPrecondition ResearchCompleted(string techName) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => Research.Instance.Get(Db.Get().Techs.Get(techName)).IsComplete()),
    description = "Has researched " + techName + "."
  };

  public GameplayEventPrecondition AchievementUnlocked(ColonyAchievement achievement) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().IsAchievementUnlocked(achievement)),
    description = "Unlocked the " + achievement.Id + " achievement"
  };

  public GameplayEventPrecondition RoomBuilt(RoomType roomType) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => Game.Instance.roomProber.rooms.Exists((Predicate<Room>) (match => match.roomType == roomType))),
    description = "Built a " + roomType.Id + " room"
  };

  public GameplayEventPrecondition CycleRestriction(float min = 0.0f, float max = float.PositiveInfinity) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => (double) GameUtil.GetCurrentTimeInCycles() >= (double) min && (double) GameUtil.GetCurrentTimeInCycles() <= (double) max),
    description = string.Format("After cycle {0} and before cycle {1}", (object) min, (object) max)
  };

  public GameplayEventPrecondition MinionsWithEffect(string effectId, int count = 1) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => Components.LiveMinionIdentities.Items.Count<MinionIdentity>((Func<MinionIdentity, bool>) (minion => ((Component) minion).GetComponent<Effects>().Get(effectId) != null)) >= count),
    description = string.Format("At least {0} dupes have the {1} effect applied", (object) count, (object) effectId)
  };

  public GameplayEventPrecondition MinionsWithStatusItem(StatusItem statusItem, int count = 1) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => Components.LiveMinionIdentities.Items.Count<MinionIdentity>((Func<MinionIdentity, bool>) (minion => ((Component) minion).GetComponent<KSelectable>().HasStatusItem(statusItem))) >= count),
    description = string.Format("At least {0} dupes have the {1} status item", (object) count, (object) statusItem)
  };

  public GameplayEventPrecondition MinionsWithChoreGroupPriorityOrGreater(
    ChoreGroup choreGroup,
    int count,
    int priority)
  {
    return new GameplayEventPrecondition()
    {
      condition = (GameplayEventPrecondition.PreconditionFn) (() => Components.LiveMinionIdentities.Items.Count<MinionIdentity>((Func<MinionIdentity, bool>) (minion =>
      {
        ChoreConsumer component = ((Component) minion).GetComponent<ChoreConsumer>();
        return !component.IsChoreGroupDisabled(choreGroup) && component.GetPersonalPriority(choreGroup) >= priority;
      })) >= count),
      description = string.Format("At least {0} dupes have their {1} set to {2} or higher.", (object) count, (object) choreGroup.Name, (object) priority)
    };
  }

  public GameplayEventPrecondition PastEventCount(string evtId, int count = 1) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => GameplayEventManager.Instance.NumberOfPastEvents(HashedString.op_Implicit(evtId)) >= count),
    description = string.Format("The {0} event has triggered {1} times.", (object) evtId, (object) count)
  };

  public GameplayEventPrecondition PastEventCountAndNotActive(GameplayEvent evt, int count = 1) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => GameplayEventManager.Instance.NumberOfPastEvents(evt.IdHash) >= count && !GameplayEventManager.Instance.IsGameplayEventActive(evt)),
    description = string.Format("The {0} event has triggered {1} times and is not active.", (object) evt.Id, (object) count)
  };

  public GameplayEventPrecondition Not(GameplayEventPrecondition precondition) => new GameplayEventPrecondition()
  {
    condition = (GameplayEventPrecondition.PreconditionFn) (() => !precondition.condition()),
    description = "Not[" + precondition.description + "]"
  };

  public GameplayEventPrecondition Or(
    GameplayEventPrecondition precondition1,
    GameplayEventPrecondition precondition2)
  {
    return new GameplayEventPrecondition()
    {
      condition = (GameplayEventPrecondition.PreconditionFn) (() => precondition1.condition() || precondition2.condition()),
      description = "[" + precondition1.description + "]-OR-[" + precondition2.description + "]"
    };
  }
}
