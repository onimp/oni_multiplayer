// Decompiled with JetBrains decompiler
// Type: Lure
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class Lure : GameStateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>
{
  public GameStateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.State off;
  public GameStateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.State on;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.off.DoNothing();
    this.on.Enter(new StateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.State.Callback(this.AddToScenePartitioner)).Exit(new StateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.State.Callback(this.RemoveFromScenePartitioner));
  }

  private void AddToScenePartitioner(Lure.Instance smi)
  {
    Extents extents = new Extents(Grid.PosToCell(TransformExtensions.GetPosition(smi.transform)), smi.def.radius);
    smi.partitionerEntry = GameScenePartitioner.Instance.Add(this.name, (object) smi, extents, GameScenePartitioner.Instance.lure, (System.Action<object>) null);
  }

  private void RemoveFromScenePartitioner(Lure.Instance smi) => GameScenePartitioner.Instance.Free(ref smi.partitionerEntry);

  public class Def : StateMachine.BaseDef
  {
    public CellOffset[] lurePoints = new CellOffset[1];
    public int radius = 50;
    public Tag[] initialLures;
  }

  public new class Instance : 
    GameStateMachine<Lure, Lure.Instance, IStateMachineTarget, Lure.Def>.GameInstance
  {
    private Tag[] lures;
    public HandleVector<int>.Handle partitionerEntry;

    public Instance(IStateMachineTarget master, Lure.Def def)
      : base(master, def)
    {
    }

    public override void StartSM()
    {
      base.StartSM();
      if (this.def.initialLures == null)
        return;
      this.SetActiveLures(this.def.initialLures);
    }

    public void SetActiveLures(Tag[] lures)
    {
      this.lures = lures;
      if (lures == null || lures.Length == 0)
        this.GoTo((StateMachine.BaseState) this.sm.off);
      else
        this.GoTo((StateMachine.BaseState) this.sm.on);
    }

    public bool IsActive() => this.GetCurrentState() == this.sm.on;

    public bool HasAnyLure(Tag[] creature_lures)
    {
      if (this.lures == null || creature_lures == null)
        return false;
      foreach (Tag creatureLure in creature_lures)
      {
        foreach (Tag lure in this.lures)
        {
          if (Tag.op_Equality(creatureLure, lure))
            return true;
        }
      }
      return false;
    }
  }
}
