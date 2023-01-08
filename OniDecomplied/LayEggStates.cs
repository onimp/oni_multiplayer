// Decompiled with JetBrains decompiler
// Type: LayEggStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using STRINGS;
using System;
using UnityEngine;

public class LayEggStates : 
  GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>
{
  public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State layeggpre;
  public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State layeggpst;
  public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State moveaside;
  public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State lookategg;
  public GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.layeggpre;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.LAYINGANEGG.NAME, (string) CREATURES.STATUSITEMS.LAYINGANEGG.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.layeggpre.Enter(new StateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State.Callback(LayEggStates.LayEgg)).Exit(new StateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State.Callback(LayEggStates.ShowEgg)).PlayAnim("lay_egg_pre").OnAnimQueueComplete(this.layeggpst);
    this.layeggpst.PlayAnim("lay_egg_pst").OnAnimQueueComplete(this.moveaside);
    this.moveaside.MoveTo(new Func<LayEggStates.Instance, int>(LayEggStates.GetMoveAsideCell), this.lookategg, this.behaviourcomplete);
    this.lookategg.Enter(new StateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.State.Callback(LayEggStates.FaceEgg)).GoTo(this.behaviourcomplete);
    this.behaviourcomplete.QueueAnim("idle_loop", true).BehaviourComplete(GameTags.Creatures.Fertile);
  }

  private static void LayEgg(LayEggStates.Instance smi)
  {
    smi.eggPos = TransformExtensions.GetPosition(smi.transform);
    smi.GetSMI<FertilityMonitor.Instance>().LayEgg();
  }

  private static void ShowEgg(LayEggStates.Instance smi) => smi.GetSMI<FertilityMonitor.Instance>()?.ShowEgg();

  private static void FaceEgg(LayEggStates.Instance smi) => smi.Get<Facing>().Face(smi.eggPos);

  private static int GetMoveAsideCell(LayEggStates.Instance smi)
  {
    int x = 1;
    if (GenericGameSettings.instance.acceleratedLifecycle)
      x = 8;
    int cell1 = Grid.PosToCell((StateMachine.Instance) smi);
    if (Grid.IsValidCell(cell1))
    {
      int moveAsideCell = Grid.OffsetCell(cell1, x, 0);
      if (Grid.IsValidCell(moveAsideCell) && !Grid.Solid[moveAsideCell])
        return moveAsideCell;
      int cell2 = Grid.OffsetCell(cell1, -x, 0);
      if (Grid.IsValidCell(cell2))
        return cell2;
    }
    return Grid.InvalidCell;
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<LayEggStates, LayEggStates.Instance, IStateMachineTarget, LayEggStates.Def>.GameInstance
  {
    public Vector3 eggPos;

    public Instance(Chore<LayEggStates.Instance> chore, LayEggStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Fertile);
    }
  }
}
