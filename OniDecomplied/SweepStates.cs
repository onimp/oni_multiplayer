// Decompiled with JetBrains decompiler
// Type: SweepStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

public class SweepStates : 
  GameStateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>
{
  public const float TIME_UNTIL_BORED = 30f;
  public const string MOVE_LOOP_SOUND = "SweepBot_mvmt_lp";
  public StateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.BoolParameter headingRight;
  private StateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.FloatParameter timeUntilBored;
  public StateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.BoolParameter bored;
  private GameStateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State beginPatrol;
  private GameStateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State moving;
  private GameStateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State pause;
  private GameStateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State mopping;
  private GameStateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State redirected;
  private GameStateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State emoteRedirected;
  private GameStateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State sweep;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.beginPatrol;
    this.beginPatrol.Enter((StateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State.Callback) (smi =>
    {
      double num = (double) smi.sm.timeUntilBored.Set(30f, smi);
      smi.GoTo((StateMachine.BaseState) this.moving);
      SweepStates.Instance instance = smi;
      instance.OnStop = instance.OnStop + (System.Action<string, StateMachine.Status>) ((data, status) => this.StopMoveSound(smi));
    }));
    this.moving.Enter((StateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State.Callback) (smi => { })).MoveTo((Func<SweepStates.Instance, int>) (smi => this.GetNextCell(smi)), this.pause, this.redirected).Update((System.Action<SweepStates.Instance, float>) ((smi, dt) =>
    {
      double num1 = (double) smi.sm.timeUntilBored.Set(smi.sm.timeUntilBored.Get(smi) - dt, smi);
      if ((double) smi.sm.timeUntilBored.Get(smi) <= 0.0)
      {
        smi.sm.bored.Set(true, smi);
        double num2 = (double) smi.sm.timeUntilBored.Set(30f, smi);
        smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("react_bored"));
      }
      StorageUnloadMonitor.Instance smi1 = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
      Storage cmp = smi1.sm.sweepLocker.Get(smi1);
      if (!Object.op_Inequality((Object) cmp, (Object) null) || smi.sm.headingRight.Get(smi) != (double) smi.master.transform.position.x > (double) cmp.transform.position.x)
        return;
      Navigator component = smi.master.gameObject.GetComponent<Navigator>();
      if (component.GetNavigationCost(Grid.PosToCell((KMonoBehaviour) cmp)) < component.maxProbingRadius - 1)
        return;
      smi.GoTo((StateMachine.BaseState) smi.sm.emoteRedirected);
    }), (UpdateRate) 6);
    this.emoteRedirected.Enter((StateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State.Callback) (smi =>
    {
      this.StopMoveSound(smi);
      int cell = Grid.PosToCell(smi.master.gameObject);
      if (Grid.IsCellOffsetValid(cell, this.headingRight.Get(smi) ? 1 : -1, -1) && !Grid.Solid[Grid.OffsetCell(cell, this.headingRight.Get(smi) ? 1 : -1, -1)])
        smi.Play("gap");
      else
        smi.Play("bump");
      this.headingRight.Set(!this.headingRight.Get(smi), smi);
    })).OnAnimQueueComplete(this.pause);
    this.redirected.StopMoving().GoTo(this.emoteRedirected);
    this.sweep.PlayAnim("pickup").ToggleEffect("BotSweeping").Enter((StateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State.Callback) (smi =>
    {
      this.StopMoveSound(smi);
      smi.sm.bored.Set(false, smi);
      double num = (double) smi.sm.timeUntilBored.Set(30f, smi);
    })).OnAnimQueueComplete(this.moving);
    this.pause.Enter((StateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State.Callback) (smi =>
    {
      if (Grid.IsLiquid(Grid.PosToCell((StateMachine.Instance) smi)))
        smi.GoTo((StateMachine.BaseState) this.mopping);
      else if (this.TrySweep(smi))
        smi.GoTo((StateMachine.BaseState) this.sweep);
      else
        smi.GoTo((StateMachine.BaseState) this.moving);
    }));
    this.mopping.PlayAnim("mop_pre", (KAnim.PlayMode) 1).QueueAnim("mop_loop", true).ToggleEffect("BotMopping").Enter((StateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.State.Callback) (smi =>
    {
      double num = (double) smi.sm.timeUntilBored.Set(30f, smi);
      smi.sm.bored.Set(false, smi);
      this.StopMoveSound(smi);
    })).Update((System.Action<SweepStates.Instance, float>) ((smi, dt) =>
    {
      if ((double) smi.timeinstate > 16.0 || !Grid.IsLiquid(Grid.PosToCell((StateMachine.Instance) smi)))
        smi.GoTo((StateMachine.BaseState) this.moving);
      else
        this.TryMop(smi, dt);
    }), (UpdateRate) 6);
  }

  public void StopMoveSound(SweepStates.Instance smi)
  {
    LoopingSounds component = smi.gameObject.GetComponent<LoopingSounds>();
    component.StopSound(GlobalAssets.GetSound("SweepBot_mvmt_lp"));
    component.StopAllSounds();
  }

  public void StartMoveSound(SweepStates.Instance smi)
  {
    LoopingSounds component = smi.gameObject.GetComponent<LoopingSounds>();
    if (component.IsSoundPlaying(GlobalAssets.GetSound("SweepBot_mvmt_lp")))
      return;
    component.StartSound(GlobalAssets.GetSound("SweepBot_mvmt_lp"));
  }

  public void TryMop(SweepStates.Instance smi, float dt)
  {
    int cell = Grid.PosToCell((StateMachine.Instance) smi);
    if (!Grid.IsLiquid(cell))
      return;
    Moppable.MopCell(cell, Mathf.Min(Grid.Mass[cell], 10f * dt), (System.Action<Sim.MassConsumedCallback, object>) ((mass_cb_info, data) =>
    {
      if (this == null || (double) mass_cb_info.mass <= 0.0)
        return;
      SubstanceChunk chunk = LiquidSourceManager.Instance.CreateChunk(ElementLoader.elements[(int) mass_cb_info.elemIdx], mass_cb_info.mass, mass_cb_info.temperature, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount, Grid.CellToPosCCC(cell, Grid.SceneLayer.Ore));
      TransformExtensions.SetPosition(chunk.transform, Vector3.op_Addition(TransformExtensions.GetPosition(chunk.transform), new Vector3((float) (((double) Random.value - 0.5) * 0.5), 0.0f, 0.0f)));
      this.TryStore(((Component) chunk).gameObject, smi);
    }));
  }

  public bool TrySweep(SweepStates.Instance smi)
  {
    int cell = Grid.PosToCell((StateMachine.Instance) smi);
    GameObject gameObject = Grid.Objects[cell, 3];
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return false;
    ObjectLayerListItem nextItem = gameObject.GetComponent<Pickupable>().objectLayerListItem.nextItem;
    return nextItem != null && this.TryStore(nextItem.gameObject, smi);
  }

  public bool TryStore(GameObject go, SweepStates.Instance smi)
  {
    Pickupable component1 = go.GetComponent<Pickupable>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return false;
    Storage component2 = smi.master.gameObject.GetComponents<Storage>()[1];
    if (component2.IsFull() || !Object.op_Inequality((Object) component1, (Object) null) || !component1.absorbable)
      return false;
    SingleEntityReceptacle component3 = smi.master.GetComponent<SingleEntityReceptacle>();
    if (Object.op_Equality((Object) ((Component) component1).gameObject, (Object) component3.Occupant))
      return false;
    bool flag;
    if ((double) component1.TotalAmount > 10.0)
    {
      ((Component) component1).GetComponent<EntitySplitter>();
      Pickupable pickupable = EntitySplitter.Split(component1, Mathf.Min(10f, component2.RemainingCapacity()));
      double num = (double) smi.gameObject.GetAmounts().GetValue(Db.Get().Amounts.InternalBattery.Id);
      component2.Store(((Component) pickupable).gameObject);
      flag = true;
    }
    else
    {
      double num = (double) smi.gameObject.GetAmounts().GetValue(Db.Get().Amounts.InternalBattery.Id);
      component2.Store(((Component) component1).gameObject);
      flag = true;
    }
    return flag;
  }

  public int GetNextCell(SweepStates.Instance smi)
  {
    int num1 = 0;
    int num2 = Grid.PosToCell((StateMachine.Instance) smi);
    int invalidCell = Grid.InvalidCell;
    if (!Grid.Solid[Grid.CellBelow(num2)] || Grid.Solid[num2])
      return Grid.InvalidCell;
    for (; num1 < 1; ++num1)
    {
      int num3 = smi.sm.headingRight.Get(smi) ? Grid.CellRight(num2) : Grid.CellLeft(num2);
      if (Grid.IsValidCell(num3) && !Grid.Solid[num3] && Grid.IsValidCell(Grid.CellBelow(num3)) && Grid.Solid[Grid.CellBelow(num3)])
        num2 = num3;
      else
        break;
    }
    return num2 == Grid.PosToCell((StateMachine.Instance) smi) ? Grid.InvalidCell : num2;
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<SweepStates, SweepStates.Instance, IStateMachineTarget, SweepStates.Def>.GameInstance
  {
    public Instance(Chore<SweepStates.Instance> chore, SweepStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
    }

    public override void StartSM()
    {
      base.StartSM();
      this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().RobotStatusItems.Working, (object) this.gameObject);
    }

    protected override void OnCleanUp()
    {
      base.OnCleanUp();
      this.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().RobotStatusItems.Working);
    }
  }
}
