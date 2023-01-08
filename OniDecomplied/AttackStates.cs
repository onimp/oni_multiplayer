// Decompiled with JetBrains decompiler
// Type: AttackStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class AttackStates : 
  GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>
{
  public StateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.TargetParameter target;
  public GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.ApproachSubState<AttackableBase> approach;
  public CellOffset[] cellOffsets;
  public GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State waitBeforeAttack;
  public AttackStates.AttackingStates attack;
  public GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.waitBeforeAttack;
    this.root.Enter("SetTarget", (StateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State.Callback) (smi =>
    {
      this.target.Set(smi.GetSMI<ThreatMonitor.Instance>().MainThreat, smi);
      this.cellOffsets = smi.def.cellOffsets;
    }));
    this.waitBeforeAttack.ScheduleGoTo((Func<AttackStates.Instance, float>) (smi => Random.Range(0.0f, 4f)), (StateMachine.BaseState) this.approach);
    this.approach.InitializeStates(this.masterTarget, this.target, (GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State) this.attack, override_offsets: this.cellOffsets).ToggleStatusItem((string) CREATURES.STATUSITEMS.ATTACK_APPROACH.NAME, (string) CREATURES.STATUSITEMS.ATTACK_APPROACH.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.attack.DefaultState(this.attack.pre).ToggleStatusItem((string) CREATURES.STATUSITEMS.ATTACK.NAME, (string) CREATURES.STATUSITEMS.ATTACK.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.attack.pre.PlayAnim((Func<AttackStates.Instance, string>) (smi => smi.def.preAnim)).Exit((StateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State.Callback) (smi => smi.GetComponent<Weapon>().AttackTarget(this.target.Get(smi)))).OnAnimQueueComplete(this.attack.pst);
    this.attack.pst.PlayAnim((Func<AttackStates.Instance, string>) (smi => smi.def.pstAnim)).OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Attack);
  }

  public class Def : StateMachine.BaseDef
  {
    public string preAnim;
    public string pstAnim;
    public CellOffset[] cellOffsets = new CellOffset[5]
    {
      new CellOffset(0, 0),
      new CellOffset(1, 0),
      new CellOffset(-1, 0),
      new CellOffset(1, 1),
      new CellOffset(-1, 1)
    };

    public Def(string pre_anim = "eat_pre", string pst_anim = "eat_pst", CellOffset[] cell_offsets = null)
    {
      this.preAnim = pre_anim;
      this.pstAnim = pst_anim;
      if (cell_offsets == null)
        return;
      this.cellOffsets = cell_offsets;
    }
  }

  public class AttackingStates : 
    GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State
  {
    public GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State pre;
    public GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.State pst;
  }

  public new class Instance : 
    GameStateMachine<AttackStates, AttackStates.Instance, IStateMachineTarget, AttackStates.Def>.GameInstance
  {
    public Instance(Chore<AttackStates.Instance> chore, AttackStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Attack);
    }
  }
}
