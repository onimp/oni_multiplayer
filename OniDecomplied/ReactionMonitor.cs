// Decompiled with JetBrains decompiler
// Type: ReactionMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

public class ReactionMonitor : 
  GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>
{
  public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State idle;
  public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State reacting;
  public GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State dead;
  public StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.ObjectParameter<Reactable> reactable;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.serializable = StateMachine.SerializeType.Never;
    this.root.EventHandler(GameHashes.DestinationReached, (StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State.Callback) (smi => smi.ClearLastReaction())).EventHandler(GameHashes.NavigationFailed, (StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State.Callback) (smi => smi.ClearLastReaction()));
    this.idle.Enter("ClearReactable", (StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State.Callback) (smi => this.reactable.Set((Reactable) null, smi))).TagTransition(GameTags.Dead, this.dead);
    this.reacting.Enter("Reactable.Begin", (StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State.Callback) (smi => this.reactable.Get(smi).Begin(smi.gameObject))).Enter((StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State.Callback) (smi => smi.master.Trigger(-909573545, (object) null))).Enter("Reactable.AddChorePreventionTag", (StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State.Callback) (smi =>
    {
      if (!this.reactable.Get(smi).preventChoreInterruption)
        return;
      smi.GetComponent<KPrefabID>().AddTag(GameTags.PreventChoreInterruption, false);
    })).Update("Reactable.Update", (System.Action<ReactionMonitor.Instance, float>) ((smi, dt) => this.reactable.Get(smi).Update(dt))).Exit((StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State.Callback) (smi => smi.master.Trigger(824899998, (object) null))).Exit("Reactable.End", (StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State.Callback) (smi => this.reactable.Get(smi).End())).Exit("Reactable.RemoveChorePreventionTag", (StateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.State.Callback) (smi =>
    {
      if (!this.reactable.Get(smi).preventChoreInterruption)
        return;
      smi.GetComponent<KPrefabID>().RemoveTag(GameTags.PreventChoreInterruption);
    })).EventTransition(GameHashes.NavigationFailed, this.idle).TagTransition(GameTags.Dying, this.dead).TagTransition(GameTags.Dead, this.dead);
    this.dead.DoNothing();
  }

  private static bool ShouldReact(ReactionMonitor.Instance smi) => smi.ImmediateReactable != null;

  public class Def : StateMachine.BaseDef
  {
    public ObjectLayer ReactionLayer;
  }

  public new class Instance : 
    GameStateMachine<ReactionMonitor, ReactionMonitor.Instance, IStateMachineTarget, ReactionMonitor.Def>.GameInstance
  {
    private KBatchedAnimController animController;
    private float lastReaction = float.NaN;
    private Dictionary<HashedString, float> lastReactTimes;
    private List<Reactable> oneshotReactables;

    public Reactable ImmediateReactable { get; private set; }

    public Instance(IStateMachineTarget master, ReactionMonitor.Def def)
      : base(master, def)
    {
      this.animController = this.GetComponent<KBatchedAnimController>();
      this.lastReactTimes = new Dictionary<HashedString, float>();
      this.oneshotReactables = new List<Reactable>();
    }

    public bool CanReact(Emote e) => Object.op_Inequality((Object) this.animController, (Object) null) && e.IsValidForController(this.animController);

    public bool TryReact(
      Reactable reactable,
      float clockTime,
      Navigator.ActiveTransition transition = null)
    {
      float num;
      if (reactable == null || this.lastReactTimes.TryGetValue(reactable.id, out num) && (double) num == (double) this.lastReaction || (double) clockTime - (double) num < (double) reactable.localCooldown || !reactable.CanBegin(this.gameObject, transition))
        return false;
      this.lastReactTimes[reactable.id] = clockTime;
      this.sm.reactable.Set(reactable, this.smi);
      this.smi.GoTo((StateMachine.BaseState) this.sm.reacting);
      return true;
    }

    public void PollForReactables(Navigator.ActiveTransition transition)
    {
      if (this.IsReacting())
        return;
      for (int index = this.oneshotReactables.Count - 1; index >= 0; --index)
      {
        Reactable oneshotReactable = this.oneshotReactables[index];
        if (oneshotReactable.IsExpired())
        {
          oneshotReactable.Cleanup();
          this.oneshotReactables.RemoveAt(index);
        }
      }
      Vector2I xy = Grid.CellToXY(Grid.PosToCell(this.smi.gameObject));
      ScenePartitionerLayer objectLayer = GameScenePartitioner.Instance.objectLayers[(int) this.def.ReactionLayer];
      ListPool<ScenePartitionerEntry, ReactionMonitor>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ReactionMonitor>.Allocate();
      GameScenePartitioner.Instance.GatherEntries(xy.x, xy.y, 1, 1, objectLayer, (List<ScenePartitionerEntry>) gathered_entries);
      float num = float.NaN;
      float time = GameClock.Instance.GetTime();
      for (int index = 0; index < ((List<ScenePartitionerEntry>) gathered_entries).Count; ++index)
      {
        if (this.TryReact(((List<ScenePartitionerEntry>) gathered_entries)[index].obj as Reactable, time, transition))
        {
          num = time;
          break;
        }
      }
      this.lastReaction = num;
      gathered_entries.Recycle();
    }

    public void ClearLastReaction() => this.lastReaction = float.NaN;

    public void StopReaction()
    {
      for (int index = this.oneshotReactables.Count - 1; index >= 0; --index)
      {
        if (this.sm.reactable.Get(this.smi) == this.oneshotReactables[index])
        {
          this.oneshotReactables[index].Cleanup();
          this.oneshotReactables.RemoveAt(index);
          break;
        }
      }
      this.smi.GoTo((StateMachine.BaseState) this.sm.idle);
    }

    public bool IsReacting() => this.smi.IsInsideState((StateMachine.BaseState) this.sm.reacting);

    public SelfEmoteReactable AddSelfEmoteReactable(
      GameObject target,
      HashedString reactionId,
      Emote emote,
      bool isOneShot,
      ChoreType choreType,
      float globalCooldown = 0.0f,
      float localCooldown = 20f,
      float lifeSpan = float.NegativeInfinity,
      float maxInitialDelay = 0.0f,
      List<Reactable.ReactablePrecondition> emotePreconditions = null)
    {
      if (!this.CanReact(emote))
        return (SelfEmoteReactable) null;
      SelfEmoteReactable reactable = new SelfEmoteReactable(target, reactionId, choreType, globalCooldown, localCooldown, lifeSpan, maxInitialDelay);
      reactable.SetEmote(emote);
      for (int index = 0; emotePreconditions != null && index < emotePreconditions.Count; ++index)
        reactable.AddPrecondition(emotePreconditions[index]);
      if (isOneShot)
        this.AddOneshotReactable(reactable);
      return reactable;
    }

    public SelfEmoteReactable AddSelfEmoteReactable(
      GameObject target,
      string reactionId,
      string emoteAnim,
      bool isOneShot,
      ChoreType choreType,
      float globalCooldown = 0.0f,
      float localCooldown = 20f,
      float maxTriggerTime = float.NegativeInfinity,
      float maxInitialDelay = 0.0f,
      List<Reactable.ReactablePrecondition> emotePreconditions = null)
    {
      Emote emote = new Emote((ResourceSet) null, reactionId, new EmoteStep[1]
      {
        new EmoteStep() { anim = HashedString.op_Implicit("react") }
      }, emoteAnim);
      return this.AddSelfEmoteReactable(target, HashedString.op_Implicit(reactionId), emote, isOneShot, choreType, globalCooldown, localCooldown, maxTriggerTime, maxInitialDelay, emotePreconditions);
    }

    public void AddOneshotReactable(SelfEmoteReactable reactable)
    {
      if (reactable == null)
        return;
      this.oneshotReactables.Add((Reactable) reactable);
    }

    public void CancelOneShotReactable(SelfEmoteReactable cancel_target)
    {
      for (int index = this.oneshotReactables.Count - 1; index >= 0; --index)
      {
        Reactable oneshotReactable = this.oneshotReactables[index];
        if (cancel_target == oneshotReactable)
        {
          oneshotReactable.Cleanup();
          this.oneshotReactables.RemoveAt(index);
          break;
        }
      }
    }

    public void CancelOneShotReactables(Emote reactionEmote)
    {
      for (int index = this.oneshotReactables.Count - 1; index >= 0; --index)
      {
        if (this.oneshotReactables[index] is EmoteReactable oneshotReactable && oneshotReactable.emote == reactionEmote)
        {
          oneshotReactable.Cleanup();
          this.oneshotReactables.RemoveAt(index);
        }
      }
    }
  }
}
