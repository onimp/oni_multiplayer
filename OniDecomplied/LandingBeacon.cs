// Decompiled with JetBrains decompiler
// Type: LandingBeacon
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class LandingBeacon : GameStateMachine<LandingBeacon, LandingBeacon.Instance>
{
  public GameStateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.State off;
  public LandingBeacon.WorkingStates working;
  public static readonly Operational.Flag noSurfaceSight = new Operational.Flag(nameof (noSurfaceSight), Operational.Flag.Type.Requirement);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.off;
    this.root.Update(new System.Action<LandingBeacon.Instance, float>(LandingBeacon.UpdateLineOfSight));
    this.off.PlayAnim("off").EventTransition(GameHashes.OperationalChanged, (GameStateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.State) this.working, (StateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.operational.IsOperational));
    this.working.DefaultState(this.working.pre).EventTransition(GameHashes.OperationalChanged, this.off, (StateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.operational.IsOperational));
    this.working.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.working.loop);
    this.working.loop.PlayAnim("working_loop", (KAnim.PlayMode) 0).Enter("SetActive", (StateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.operational.SetActive(true))).Exit("SetActive", (StateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.operational.SetActive(false)));
  }

  public static void UpdateLineOfSight(LandingBeacon.Instance smi, float dt)
  {
    WorldContainer myWorld = smi.GetMyWorld();
    bool flag = true;
    int num = Grid.PosToCell((StateMachine.Instance) smi);
    for (int y = (int) myWorld.maximumBounds.y; Grid.CellRow(num) <= y; num = Grid.CellAbove(num))
    {
      if (!Grid.IsValidCell(num) || Grid.Solid[num])
      {
        flag = false;
        break;
      }
    }
    if (smi.skyLastVisible == flag)
      return;
    smi.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoSurfaceSight, !flag);
    smi.operational.SetFlag(LandingBeacon.noSurfaceSight, flag);
    smi.skyLastVisible = flag;
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class WorkingStates : 
    GameStateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.State pre;
    public GameStateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.State loop;
    public GameStateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.State pst;
  }

  public new class Instance : 
    GameStateMachine<LandingBeacon, LandingBeacon.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Operational operational;
    public KSelectable selectable;
    public bool skyLastVisible = true;

    public Instance(IStateMachineTarget master, LandingBeacon.Def def)
      : base(master, (object) def)
    {
      Components.LandingBeacons.Add(this);
      this.operational = this.GetComponent<Operational>();
      this.selectable = this.GetComponent<KSelectable>();
    }

    public override void StartSM()
    {
      base.StartSM();
      LandingBeacon.UpdateLineOfSight(this, 0.0f);
    }

    protected override void OnCleanUp()
    {
      base.OnCleanUp();
      Components.LandingBeacons.Remove(this);
    }

    public bool CanBeTargeted() => this.IsInsideState((StateMachine.BaseState) this.sm.working);
  }
}
