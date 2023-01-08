// Decompiled with JetBrains decompiler
// Type: DefendStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class DefendStates : 
  GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>
{
  public StateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.TargetParameter target;
  public DefendStates.ProtectStates protectEntity;
  public GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.protectEntity.moveToThreat;
    this.root.Enter("SetTarget", (StateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.State.Callback) (smi => this.target.Set(smi.GetSMI<EggProtectionMonitor.Instance>().MainThreat, smi))).ToggleStatusItem((string) CREATURES.STATUSITEMS.ATTACKINGENTITY.NAME, (string) CREATURES.STATUSITEMS.ATTACKINGENTITY.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.protectEntity.moveToThreat.InitializeStates(this.masterTarget, this.target, this.protectEntity.attackThreat, override_offsets: new CellOffset[5]
    {
      new CellOffset(0, 0),
      new CellOffset(1, 0),
      new CellOffset(-1, 0),
      new CellOffset(1, 1),
      new CellOffset(-1, 1)
    });
    this.protectEntity.attackThreat.Enter((StateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.State.Callback) (smi =>
    {
      smi.Play("slap_pre");
      smi.Queue("slap");
      smi.Queue("slap_pst");
      smi.Schedule(0.5f, (System.Action<object>) (_param1 => smi.GetComponent<Weapon>().AttackTarget(this.target.Get(smi))), (object) null);
    })).OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Defend);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.GameInstance
  {
    public Instance(Chore<DefendStates.Instance> chore, DefendStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Defend);
    }
  }

  public class ProtectStates : 
    GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.State
  {
    public GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.ApproachSubState<AttackableBase> moveToThreat;
    public GameStateMachine<DefendStates, DefendStates.Instance, IStateMachineTarget, DefendStates.Def>.State attackThreat;
  }
}
