// Decompiled with JetBrains decompiler
// Type: HugEggStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using UnityEngine;

public class HugEggStates : 
  GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>
{
  public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.ApproachSubState<EggIncubator> moving;
  public HugEggStates.HugState hug;
  public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State behaviourcomplete;
  public StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.TargetParameter target;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.moving;
    this.root.Enter(new StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State.Callback(HugEggStates.SetTarget)).Enter((StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State.Callback) (smi =>
    {
      if (HugEggStates.Reserve(smi))
        return;
      smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
    })).Exit(new StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State.Callback(HugEggStates.Unreserve)).ToggleStatusItem((string) CREATURES.STATUSITEMS.HUGEGG.NAME, (string) CREATURES.STATUSITEMS.HUGEGG.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).OnTargetLost(this.target, this.behaviourcomplete);
    this.moving.MoveTo(new Func<HugEggStates.Instance, int>(HugEggStates.GetClimbableCell), (GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State) this.hug, this.behaviourcomplete);
    this.hug.DefaultState(this.hug.pre).Enter((StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State.Callback) (smi => smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Front))).Exit((StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State.Callback) (smi => smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures)));
    this.hug.pre.Face(this.target, 0.5f).PlayAnim((Func<HugEggStates.Instance, string>) (smi => HugEggStates.GetAnims(smi).pre)).OnAnimQueueComplete(this.hug.loop);
    this.hug.loop.QueueAnim((Func<HugEggStates.Instance, string>) (smi => HugEggStates.GetAnims(smi).loop), true).ScheduleGoTo((Func<HugEggStates.Instance, float>) (smi => smi.def.hugTime), (StateMachine.BaseState) this.hug.pst);
    this.hug.pst.QueueAnim((Func<HugEggStates.Instance, string>) (smi => HugEggStates.GetAnims(smi).pst)).Enter(new StateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State.Callback(HugEggStates.ApplyEffect)).OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete((Func<HugEggStates.Instance, Tag>) (smi => smi.def.behaviourTag));
  }

  private static void SetTarget(HugEggStates.Instance smi) => smi.sm.target.Set(smi.GetSMI<HugMonitor.Instance>().hugTarget, smi);

  private static HugEggStates.AnimSet GetAnims(HugEggStates.Instance smi) => !Object.op_Inequality((Object) smi.sm.target.Get(smi).GetComponent<EggIncubator>(), (Object) null) ? smi.def.hugAnims : smi.def.incubatorHugAnims;

  private static bool Reserve(HugEggStates.Instance smi)
  {
    GameObject go = smi.sm.target.Get(smi);
    if (!Object.op_Inequality((Object) go, (Object) null) || go.HasTag(GameTags.Creatures.ReservedByCreature))
      return false;
    go.AddTag(GameTags.Creatures.ReservedByCreature);
    return true;
  }

  private static void Unreserve(HugEggStates.Instance smi)
  {
    GameObject go = smi.sm.target.Get(smi);
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    go.RemoveTag(GameTags.Creatures.ReservedByCreature);
  }

  private static int GetClimbableCell(HugEggStates.Instance smi) => Grid.PosToCell(smi.sm.target.Get(smi));

  private static void ApplyEffect(HugEggStates.Instance smi)
  {
    GameObject go = smi.sm.target.Get(smi);
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    EggIncubator component = go.GetComponent<EggIncubator>();
    if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Inequality((Object) component.Occupant, (Object) null))
    {
      component.Occupant.GetComponent<Effects>().Add("EggHug", true);
    }
    else
    {
      if (!go.HasTag(GameTags.Egg))
        return;
      go.GetComponent<Effects>().Add("EggHug", true);
    }
  }

  public class AnimSet
  {
    public string pre;
    public string loop;
    public string pst;
  }

  public class Def : StateMachine.BaseDef
  {
    public float hugTime = 15f;
    public Tag behaviourTag;
    public HugEggStates.AnimSet hugAnims = new HugEggStates.AnimSet()
    {
      pre = "hug_egg_pre",
      loop = "hug_egg_loop",
      pst = "hug_egg_pst"
    };
    public HugEggStates.AnimSet incubatorHugAnims = new HugEggStates.AnimSet()
    {
      pre = "hug_incubator_pre",
      loop = "hug_incubator_loop",
      pst = "hug_incubator_pst"
    };

    public Def(Tag behaviourTag) => this.behaviourTag = behaviourTag;
  }

  public new class Instance : 
    GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.GameInstance
  {
    public Instance(Chore<HugEggStates.Instance> chore, HugEggStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) def.behaviourTag);
    }
  }

  public class HugState : 
    GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State
  {
    public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State pre;
    public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State loop;
    public GameStateMachine<HugEggStates, HugEggStates.Instance, IStateMachineTarget, HugEggStates.Def>.State pst;
  }
}
