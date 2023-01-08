// Decompiled with JetBrains decompiler
// Type: TreeClimbStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class TreeClimbStates : 
  GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>
{
  public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.ApproachSubState<Uprootable> moving;
  public TreeClimbStates.ClimbState climbing;
  public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State behaviourcomplete;
  public StateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.TargetParameter target;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.moving;
    this.root.Enter(new StateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State.Callback(TreeClimbStates.SetTarget)).Enter((StateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State.Callback) (smi =>
    {
      if (TreeClimbStates.ReserveClimbable(smi))
        return;
      smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
    })).Exit(new StateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State.Callback(TreeClimbStates.UnreserveClimbable)).ToggleStatusItem((string) CREATURES.STATUSITEMS.RUMMAGINGSEED.NAME, (string) CREATURES.STATUSITEMS.RUMMAGINGSEED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.moving.MoveTo(new Func<TreeClimbStates.Instance, int>(TreeClimbStates.GetClimbableCell), (GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State) this.climbing, this.behaviourcomplete);
    this.climbing.DefaultState(this.climbing.pre);
    this.climbing.pre.PlayAnim("rummage_pre").OnAnimQueueComplete(this.climbing.loop);
    this.climbing.loop.QueueAnim("rummage_loop", true).ScheduleGoTo(3.5f, (StateMachine.BaseState) this.climbing.pst).Update(new System.Action<TreeClimbStates.Instance, float>(TreeClimbStates.Rummage), (UpdateRate) 6);
    this.climbing.pst.QueueAnim("rummage_pst").OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToClimbTree);
  }

  private static void SetTarget(TreeClimbStates.Instance smi) => smi.sm.target.Set(smi.GetSMI<ClimbableTreeMonitor.Instance>().climbTarget, smi);

  private static bool ReserveClimbable(TreeClimbStates.Instance smi)
  {
    GameObject go = smi.sm.target.Get(smi);
    if (!Object.op_Inequality((Object) go, (Object) null) || go.HasTag(GameTags.Creatures.ReservedByCreature))
      return false;
    go.AddTag(GameTags.Creatures.ReservedByCreature);
    return true;
  }

  private static void UnreserveClimbable(TreeClimbStates.Instance smi)
  {
    GameObject go = smi.sm.target.Get(smi);
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    go.RemoveTag(GameTags.Creatures.ReservedByCreature);
  }

  private static void Rummage(TreeClimbStates.Instance smi, float dt)
  {
    GameObject gameObject1 = smi.sm.target.Get(smi);
    if (!Object.op_Inequality((Object) gameObject1, (Object) null))
      return;
    BuddingTrunk component1 = gameObject1.GetComponent<BuddingTrunk>();
    if (Object.op_Implicit((Object) component1))
    {
      component1.ExtractExtraSeed();
    }
    else
    {
      Storage component2 = gameObject1.GetComponent<Storage>();
      if (!Object.op_Implicit((Object) component2) || component2.items.Count <= 0)
        return;
      int index = Random.Range(0, component2.items.Count - 1);
      GameObject gameObject2 = component2.items[index];
      Pickupable pu = Object.op_Implicit((Object) gameObject2) ? gameObject2.GetComponent<Pickupable>() : (Pickupable) null;
      if (!Object.op_Implicit((Object) pu) || (double) pu.UnreservedAmount <= 0.0099999997764825821)
        return;
      smi.Toss(pu);
    }
  }

  private static int GetClimbableCell(TreeClimbStates.Instance smi) => Grid.PosToCell(smi.sm.target.Get(smi));

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.GameInstance
  {
    private Storage storage;
    private static readonly Vector2 VEL_MIN = new Vector2(-1f, 2f);
    private static readonly Vector2 VEL_MAX = new Vector2(1f, 4f);

    public Instance(Chore<TreeClimbStates.Instance> chore, TreeClimbStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsToClimbTree);
      this.storage = this.GetComponent<Storage>();
    }

    public void Toss(Pickupable pu)
    {
      Pickupable pickupable = pu.Take(Mathf.Min(1f, pu.UnreservedAmount));
      if (!Object.op_Inequality((Object) pickupable, (Object) null))
        return;
      this.storage.Store(((Component) pickupable).gameObject, true);
      this.storage.Drop(((Component) pickupable).gameObject, true);
      this.Throw(((Component) pickupable).gameObject);
    }

    private void Throw(GameObject ore_go)
    {
      Vector3 position = TransformExtensions.GetPosition(this.transform);
      position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
      int cell = Grid.PosToCell(position);
      int num = Grid.CellAbove(cell);
      Vector2 zero;
      if (Grid.IsValidCell(cell) && Grid.Solid[cell] || Grid.IsValidCell(num) && Grid.Solid[num])
      {
        zero = Vector2.zero;
      }
      else
      {
        position.y += 0.5f;
        // ISSUE: explicit constructor call
        ((Vector2) ref zero).\u002Ector(Random.Range(TreeClimbStates.Instance.VEL_MIN.x, TreeClimbStates.Instance.VEL_MAX.x), Random.Range(TreeClimbStates.Instance.VEL_MIN.y, TreeClimbStates.Instance.VEL_MAX.y));
      }
      TransformExtensions.SetPosition(ore_go.transform, position);
      if (((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) ore_go))
        ((KGameObjectComponentManager<FallerComponent>) GameComps.Fallers).Remove(ore_go);
      GameComps.Fallers.Add(ore_go, zero);
    }
  }

  public class ClimbState : 
    GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State
  {
    public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State pre;
    public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State loop;
    public GameStateMachine<TreeClimbStates, TreeClimbStates.Instance, IStateMachineTarget, TreeClimbStates.Def>.State pst;
  }
}
