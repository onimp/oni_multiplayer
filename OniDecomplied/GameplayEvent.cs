// Decompiled with JetBrains decompiler
// Type: GameplayEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{base.Id}")]
public abstract class GameplayEvent : Resource, IComparable<GameplayEvent>
{
  public const int INFINITE = -1;
  public int numTimesAllowed = -1;
  public bool allowMultipleEventInstances;
  public int durration;
  public int warning;
  protected int basePriority;
  protected int calculatedPriority;
  public List<GameplayEventPrecondition> preconditions;
  public List<GameplayEventMinionFilter> minionFilters;
  public List<HashedString> successEvents;
  public List<HashedString> failureEvents;
  public string title;
  public string description;
  public HashedString animFileName;
  public List<Tag> tags;

  public int importance { get; private set; }

  public virtual bool IsAllowed()
  {
    if (this.WillNeverRunAgain() || !this.allowMultipleEventInstances && GameplayEventManager.Instance.IsGameplayEventActive(this))
      return false;
    foreach (GameplayEventPrecondition precondition in this.preconditions)
    {
      if (precondition.required && !precondition.condition())
        return false;
    }
    return true;
  }

  public virtual bool WillNeverRunAgain() => this.numTimesAllowed != -1 && GameplayEventManager.Instance.NumberOfPastEvents(HashedString.op_Implicit(this.Id)) >= this.numTimesAllowed;

  public int GetCashedPriority() => this.calculatedPriority;

  public virtual int CalculatePriority()
  {
    this.calculatedPriority = this.basePriority + this.CalculateBoost();
    return this.calculatedPriority;
  }

  public int CalculateBoost()
  {
    int boost = 0;
    foreach (GameplayEventPrecondition precondition in this.preconditions)
    {
      if (!precondition.required && precondition.condition())
        boost += precondition.priorityModifier;
    }
    return boost;
  }

  public GameplayEvent AddPrecondition(GameplayEventPrecondition precondition)
  {
    precondition.required = true;
    this.preconditions.Add(precondition);
    return this;
  }

  public GameplayEvent AddPriorityBoost(GameplayEventPrecondition precondition, int priorityBoost)
  {
    precondition.required = false;
    precondition.priorityModifier = priorityBoost;
    this.preconditions.Add(precondition);
    return this;
  }

  public GameplayEvent AddMinionFilter(GameplayEventMinionFilter filter)
  {
    this.minionFilters.Add(filter);
    return this;
  }

  public GameplayEvent TrySpawnEventOnSuccess(HashedString evt)
  {
    this.successEvents.Add(evt);
    return this;
  }

  public GameplayEvent TrySpawnEventOnFailure(HashedString evt)
  {
    this.failureEvents.Add(evt);
    return this;
  }

  public GameplayEvent SetVisuals(HashedString animFileName)
  {
    this.animFileName = animFileName;
    return this;
  }

  public virtual Sprite GetDisplaySprite() => (Sprite) null;

  public virtual string GetDisplayString() => (string) null;

  public MinionIdentity GetRandomFilteredMinion()
  {
    List<MinionIdentity> minionIdentityList = new List<MinionIdentity>((IEnumerable<MinionIdentity>) Components.LiveMinionIdentities.Items);
    foreach (GameplayEventMinionFilter minionFilter in this.minionFilters)
    {
      GameplayEventMinionFilter filter = minionFilter;
      minionIdentityList.RemoveAll((Predicate<MinionIdentity>) (x => !filter.filter(x)));
    }
    return minionIdentityList.Count != 0 ? minionIdentityList[Random.Range(0, minionIdentityList.Count)] : (MinionIdentity) null;
  }

  public MinionIdentity GetRandomMinionPrioritizeFiltered()
  {
    MinionIdentity randomFilteredMinion = this.GetRandomFilteredMinion();
    return !Object.op_Equality((Object) randomFilteredMinion, (Object) null) ? randomFilteredMinion : Components.LiveMinionIdentities.Items[Random.Range(0, Components.LiveMinionIdentities.Items.Count)];
  }

  public int CompareTo(GameplayEvent other) => -this.GetCashedPriority().CompareTo(other.GetCashedPriority());

  public GameplayEvent(string id, int priority, int importance)
    : base(id, (ResourceSet) null, (string) null)
  {
    this.tags = new List<Tag>();
    this.basePriority = priority;
    this.preconditions = new List<GameplayEventPrecondition>();
    this.minionFilters = new List<GameplayEventMinionFilter>();
    this.successEvents = new List<HashedString>();
    this.failureEvents = new List<HashedString>();
    this.importance = importance;
    this.animFileName = HashedString.op_Implicit(id);
  }

  public abstract StateMachine.Instance GetSMI(
    GameplayEventManager manager,
    GameplayEventInstance eventInstance);

  public GameplayEventInstance CreateInstance(int worldId)
  {
    GameplayEventInstance instance = new GameplayEventInstance(this, worldId);
    if (this.tags != null)
      instance.tags.AddRange((IEnumerable<Tag>) this.tags);
    return instance;
  }

  public enum Occurance
  {
    Once,
    Infinity,
  }
}
