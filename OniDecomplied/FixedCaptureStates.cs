// Decompiled with JetBrains decompiler
// Type: FixedCaptureStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

public class FixedCaptureStates : 
  GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>
{
  private FixedCaptureStates.CaptureStates capture;
  private GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.capture;
    this.root.Exit("AbandonedCapturePoint", (StateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State.Callback) (smi => smi.AbandonedCapturePoint()));
    this.capture.EventTransition(GameHashes.CapturePointNoLongerAvailable, (GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State) null, (StateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.Transition.ConditionCallback) null).DefaultState((GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State) this.capture.cheer);
    this.capture.cheer.DefaultState(this.capture.cheer.pre).ToggleStatusItem((string) CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME, (string) CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.capture.cheer.pre.ScheduleGoTo(0.9f, (StateMachine.BaseState) this.capture.cheer.cheer);
    this.capture.cheer.cheer.Enter("FaceRancher", (StateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State.Callback) (smi => smi.GetComponent<Facing>().Face(TransformExtensions.GetPosition(smi.GetCapturePoint().transform)))).PlayAnim("excited_loop").OnAnimQueueComplete(this.capture.cheer.pst);
    this.capture.cheer.pst.ScheduleGoTo(0.2f, (StateMachine.BaseState) this.capture.move);
    this.capture.move.DefaultState(this.capture.move.movetoranch).ToggleStatusItem((string) CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME, (string) CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.capture.move.movetoranch.Enter("Speedup", (StateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State.Callback) (smi => smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed * 1.25f)).MoveTo(new Func<FixedCaptureStates.Instance, int>(FixedCaptureStates.GetTargetCaptureCell), this.capture.move.waitforranchertobeready).Exit("RestoreSpeed", (StateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State.Callback) (smi => smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed));
    this.capture.move.waitforranchertobeready.Enter("SetCreatureAtRanchingStation", (StateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State.Callback) (smi => smi.GetCapturePoint().Trigger(-1992722293))).EventTransition(GameHashes.RancherReadyAtCapturePoint, this.capture.ranching);
    this.capture.ranching.ToggleStatusItem((string) CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME, (string) CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToGetCaptured);
  }

  private static FixedCapturePoint.Instance GetCapturePoint(FixedCaptureStates.Instance smi) => smi.GetSMI<FixedCapturableMonitor.Instance>().targetCapturePoint;

  private static int GetTargetCaptureCell(FixedCaptureStates.Instance smi)
  {
    FixedCapturePoint.Instance capturePoint = FixedCaptureStates.GetCapturePoint(smi);
    return capturePoint.def.getTargetCapturePoint(capturePoint);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.GameInstance
  {
    public float originalSpeed;

    public Instance(Chore<FixedCaptureStates.Instance> chore, FixedCaptureStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      this.originalSpeed = this.GetComponent<Navigator>().defaultSpeed;
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsToGetCaptured);
    }

    public FixedCapturePoint.Instance GetCapturePoint() => this.GetSMI<FixedCapturableMonitor.Instance>()?.targetCapturePoint;

    public void AbandonedCapturePoint()
    {
      if (this.GetCapturePoint() == null)
        return;
      this.GetCapturePoint().Trigger(-1000356449);
    }
  }

  public class CaptureStates : 
    GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State
  {
    public FixedCaptureStates.CaptureStates.CheerStates cheer;
    public FixedCaptureStates.CaptureStates.MoveStates move;
    public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State ranching;

    public class CheerStates : 
      GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State
    {
      public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State pre;
      public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State cheer;
      public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State pst;
    }

    public class MoveStates : 
      GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State
    {
      public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State movetoranch;
      public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State waitforranchertobeready;
    }
  }
}
