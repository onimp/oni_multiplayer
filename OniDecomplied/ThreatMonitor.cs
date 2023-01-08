// Decompiled with JetBrains decompiler
// Type: ThreatMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ThreatMonitor : 
  GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>
{
  private FactionAlignment alignment;
  private Navigator navigator;
  public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State safe;
  public ThreatMonitor.ThreatenedStates threatened;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.safe;
    this.root.EventHandler(GameHashes.SafeFromThreats, (GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.GameEvent.Callback) ((smi, d) => smi.OnSafe(d))).EventHandler(GameHashes.Attacked, (GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.GameEvent.Callback) ((smi, d) => smi.OnAttacked(d))).EventHandler(GameHashes.ObjectDestroyed, (GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.GameEvent.Callback) ((smi, d) => smi.Cleanup(d)));
    this.safe.Enter((StateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State.Callback) (smi =>
    {
      smi.revengeThreat.Clear();
      smi.RefreshThreat((object) null);
    })).Update("safe", (System.Action<ThreatMonitor.Instance, float>) ((smi, dt) => smi.RefreshThreat((object) null)), (UpdateRate) 6, true);
    this.threatened.duplicant.Transition(this.safe, (StateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.Transition.ConditionCallback) (smi => !smi.CheckForThreats()));
    this.threatened.duplicant.ShouldFight.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateAttackChore), this.safe).Update("DupeUpdateTarget", new System.Action<ThreatMonitor.Instance, float>(ThreatMonitor.DupeUpdateTarget));
    this.threatened.duplicant.ShoudFlee.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateFleeChore), this.safe);
    this.threatened.creature.ToggleBehaviour(GameTags.Creatures.Flee, (StateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.Transition.ConditionCallback) (smi => !smi.WillFight()), (System.Action<ThreatMonitor.Instance>) (smi => smi.GoTo((StateMachine.BaseState) this.safe))).ToggleBehaviour(GameTags.Creatures.Attack, (StateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.Transition.ConditionCallback) (smi => smi.WillFight()), (System.Action<ThreatMonitor.Instance>) (smi => smi.GoTo((StateMachine.BaseState) this.safe))).Update("Threatened", new System.Action<ThreatMonitor.Instance, float>(ThreatMonitor.CritterUpdateThreats));
  }

  private static void DupeUpdateTarget(ThreatMonitor.Instance smi, float dt)
  {
    if (!Object.op_Equality((Object) smi.MainThreat, (Object) null) && smi.MainThreat.GetComponent<FactionAlignment>().IsPlayerTargeted())
      return;
    smi.Trigger(2144432245);
  }

  private static void CritterUpdateThreats(ThreatMonitor.Instance smi, float dt)
  {
    if (smi.isMasterNull)
      return;
    if (Object.op_Inequality((Object) smi.revengeThreat.target, (Object) null) && smi.revengeThreat.Calm(dt, smi.alignment))
    {
      smi.Trigger(-21431934);
    }
    else
    {
      if (smi.CheckForThreats())
        return;
      smi.GoTo((StateMachine.BaseState) smi.sm.safe);
    }
  }

  private Chore CreateAttackChore(ThreatMonitor.Instance smi) => (Chore) new AttackChore(smi.master, smi.MainThreat);

  private Chore CreateFleeChore(ThreatMonitor.Instance smi) => (Chore) new FleeChore(smi.master, smi.MainThreat);

  public class Def : StateMachine.BaseDef
  {
    public Health.HealthState fleethresholdState = Health.HealthState.Injured;
    public Tag[] friendlyCreatureTags;
  }

  public class ThreatenedStates : 
    GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State
  {
    public ThreatMonitor.ThreatenedDuplicantStates duplicant;
    public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State creature;
  }

  public class ThreatenedDuplicantStates : 
    GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State
  {
    public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State ShoudFlee;
    public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State ShouldFight;
  }

  public struct Grudge
  {
    public FactionAlignment target;
    public float grudgeTime;

    public void Reset(FactionAlignment revengeTarget)
    {
      this.target = revengeTarget;
      this.grudgeTime = 10f;
    }

    public bool Calm(float dt, FactionAlignment self)
    {
      if ((double) this.grudgeTime <= 0.0)
        return true;
      this.grudgeTime = Mathf.Max(0.0f, this.grudgeTime - dt);
      if ((double) this.grudgeTime != 0.0)
        return false;
      if (FactionManager.Instance.GetDisposition(self.Alignment, this.target.Alignment) != FactionManager.Disposition.Attack)
        PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, (string) UI.GAMEOBJECTEFFECTS.FORGAVEATTACKER, self.transform, 2f, true);
      this.Clear();
      return true;
    }

    public void Clear()
    {
      this.grudgeTime = 0.0f;
      this.target = (FactionAlignment) null;
    }

    public bool IsValidRevengeTarget(bool isDuplicant)
    {
      if (!Object.op_Inequality((Object) this.target, (Object) null) || !this.target.IsAlignmentActive() || !Object.op_Equality((Object) this.target.health, (Object) null) && this.target.health.IsDefeated())
        return false;
      return !isDuplicant || !this.target.IsPlayerTargeted();
    }
  }

  public new class Instance : 
    GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.GameInstance
  {
    public FactionAlignment alignment;
    private Navigator navigator;
    public ChoreDriver choreDriver;
    private Health health;
    private ChoreConsumer choreConsumer;
    public ThreatMonitor.Grudge revengeThreat;
    private GameObject mainThreat;
    private List<FactionAlignment> threats = new List<FactionAlignment>();
    private System.Action<object> refreshThreatDelegate;

    public GameObject MainThreat => this.mainThreat;

    public bool IAmADuplicant => this.alignment.Alignment == FactionManager.FactionID.Duplicant;

    public Instance(IStateMachineTarget master, ThreatMonitor.Def def)
      : base(master, def)
    {
      this.alignment = master.GetComponent<FactionAlignment>();
      this.navigator = master.GetComponent<Navigator>();
      this.choreDriver = master.GetComponent<ChoreDriver>();
      this.health = master.GetComponent<Health>();
      this.choreConsumer = master.GetComponent<ChoreConsumer>();
      this.refreshThreatDelegate = new System.Action<object>(this.RefreshThreat);
    }

    public void ClearMainThreat() => this.SetMainThreat((GameObject) null);

    public void SetMainThreat(GameObject threat)
    {
      if (Object.op_Equality((Object) threat, (Object) this.mainThreat))
        return;
      if (Object.op_Inequality((Object) this.mainThreat, (Object) null))
      {
        KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1623392196, this.refreshThreatDelegate);
        KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1969584890, this.refreshThreatDelegate);
        if (Object.op_Equality((Object) threat, (Object) null))
          this.Trigger(2144432245);
      }
      if (Object.op_Inequality((Object) this.mainThreat, (Object) null))
      {
        KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1623392196, this.refreshThreatDelegate);
        KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1969584890, this.refreshThreatDelegate);
      }
      this.mainThreat = threat;
      if (!Object.op_Inequality((Object) this.mainThreat, (Object) null))
        return;
      KMonoBehaviourExtensions.Subscribe(this.mainThreat, 1623392196, this.refreshThreatDelegate);
      KMonoBehaviourExtensions.Subscribe(this.mainThreat, 1969584890, this.refreshThreatDelegate);
    }

    public void OnSafe(object data)
    {
      if (!Object.op_Inequality((Object) this.revengeThreat.target, (Object) null))
        return;
      if (!((Component) this.revengeThreat.target).GetComponent<FactionAlignment>().IsAlignmentActive())
        this.revengeThreat.Clear();
      this.ClearMainThreat();
    }

    public void OnAttacked(object data)
    {
      FactionAlignment revengeTarget = (FactionAlignment) data;
      this.revengeThreat.Reset(revengeTarget);
      if (Object.op_Equality((Object) this.mainThreat, (Object) null))
      {
        this.SetMainThreat(((Component) revengeTarget).gameObject);
        this.GoToThreatened();
      }
      else if (!this.WillFight())
        this.GoToThreatened();
      if (!Object.op_Implicit((Object) ((Component) revengeTarget).GetComponent<Bee>()))
        return;
      Chore chore = Object.op_Inequality((Object) this.choreDriver, (Object) null) ? this.choreDriver.GetCurrentChore() : (Chore) null;
      if (chore == null || !Object.op_Inequality((Object) chore.gameObject.GetComponent<HiveWorkableEmpty>(), (Object) null))
        return;
      chore.gameObject.GetComponent<HiveWorkableEmpty>().wasStung = true;
    }

    public bool WillFight() => (!Object.op_Inequality((Object) this.choreConsumer, (Object) null) || this.choreConsumer.IsPermittedByUser(Db.Get().ChoreGroups.Combat) && this.choreConsumer.IsPermittedByTraits(Db.Get().ChoreGroups.Combat)) && this.health.State < this.smi.def.fleethresholdState;

    private void GotoThreatResponse()
    {
      Chore currentChore = this.smi.master.GetComponent<ChoreDriver>().GetCurrentChore();
      if (this.WillFight() && this.mainThreat.GetComponent<FactionAlignment>().IsPlayerTargeted())
      {
        this.smi.GoTo((StateMachine.BaseState) this.smi.sm.threatened.duplicant.ShouldFight);
      }
      else
      {
        if (currentChore != null && currentChore.target != null && currentChore.target != this.master && Object.op_Inequality((Object) currentChore.target.GetComponent<Pickupable>(), (Object) null))
          return;
        this.smi.GoTo((StateMachine.BaseState) this.smi.sm.threatened.duplicant.ShoudFlee);
      }
    }

    public void GoToThreatened()
    {
      if (this.IAmADuplicant)
        this.GotoThreatResponse();
      else
        this.smi.GoTo((StateMachine.BaseState) this.sm.threatened.creature);
    }

    public void Cleanup(object data)
    {
      if (!Object.op_Implicit((Object) this.mainThreat))
        return;
      KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1623392196, this.refreshThreatDelegate);
      KMonoBehaviourExtensions.Unsubscribe(this.mainThreat, 1969584890, this.refreshThreatDelegate);
    }

    public void RefreshThreat(object data)
    {
      if (!this.IsRunning())
        return;
      if (this.smi.CheckForThreats())
      {
        this.GoToThreatened();
      }
      else
      {
        if (this.smi.GetCurrentState() == this.sm.safe)
          return;
        this.Trigger(-21431934);
        this.smi.GoTo((StateMachine.BaseState) this.sm.safe);
      }
    }

    public bool CheckForThreats()
    {
      GameObject threat = !this.revengeThreat.IsValidRevengeTarget(this.IAmADuplicant) ? this.FindThreat() : ((Component) this.revengeThreat.target).gameObject;
      this.SetMainThreat(threat);
      return Object.op_Inequality((Object) threat, (Object) null);
    }

    public GameObject FindThreat()
    {
      this.threats.Clear();
      if (this.isMasterNull)
        return (GameObject) null;
      bool flag1 = this.WillFight();
      ListPool<ScenePartitionerEntry, ThreatMonitor>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, ThreatMonitor>.Allocate();
      int radius = 20;
      GameScenePartitioner.Instance.GatherEntries(new Extents(Grid.PosToCell(this.gameObject), radius), GameScenePartitioner.Instance.attackableEntitiesLayer, (List<ScenePartitionerEntry>) gathered_entries);
      for (int index = 0; index < ((List<ScenePartitionerEntry>) gathered_entries).Count; ++index)
      {
        FactionAlignment cmp = ((List<ScenePartitionerEntry>) gathered_entries)[index].obj as FactionAlignment;
        if (!Object.op_Equality((Object) cmp.transform, (Object) null) && !Object.op_Equality((Object) cmp, (Object) this.alignment) && cmp.IsAlignmentActive() && FactionManager.Instance.GetDisposition(this.alignment.Alignment, cmp.Alignment) == FactionManager.Disposition.Attack)
        {
          if (this.def.friendlyCreatureTags != null)
          {
            bool flag2 = false;
            foreach (Tag friendlyCreatureTag in this.def.friendlyCreatureTags)
            {
              if (((Component) cmp).HasTag(friendlyCreatureTag))
                flag2 = true;
            }
            if (flag2)
              continue;
          }
          if (this.navigator.CanReach((IApproachable) cmp.attackable))
            this.threats.Add(cmp);
        }
      }
      gathered_entries.Recycle();
      if (this.IAmADuplicant && flag1)
      {
        for (int faction = 0; faction < 6; ++faction)
        {
          if (faction != 0)
          {
            foreach (FactionAlignment member in FactionManager.Instance.GetFaction((FactionManager.FactionID) faction).Members)
            {
              if (member.IsPlayerTargeted() && !member.health.IsDefeated() && !this.threats.Contains(member) && this.navigator.CanReach((IApproachable) member.attackable))
                this.threats.Add(member);
            }
          }
        }
      }
      return this.threats.Count == 0 ? (GameObject) null : this.PickBestTarget(this.threats);
    }

    public GameObject PickBestTarget(List<FactionAlignment> threats)
    {
      float num1 = 1f;
      Vector2 vector2 = Vector2.op_Implicit(TransformExtensions.GetPosition(this.gameObject.transform));
      GameObject gameObject = (GameObject) null;
      float num2 = float.PositiveInfinity;
      for (int index = threats.Count - 1; index >= 0; --index)
      {
        FactionAlignment threat = threats[index];
        float num3 = Vector2.Distance(vector2, Vector2.op_Implicit(TransformExtensions.GetPosition(threat.transform))) / num1;
        if ((double) num3 < (double) num2)
        {
          num2 = num3;
          gameObject = ((Component) threat).gameObject;
        }
      }
      return gameObject;
    }
  }
}
