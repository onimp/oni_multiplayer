// Decompiled with JetBrains decompiler
// Type: LureableMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LureableMonitor : 
  GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>
{
  public StateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.TargetParameter targetLure;
  public GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.State nolure;
  public GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.State haslure;
  public GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.State cooldown;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.cooldown;
    this.cooldown.ScheduleGoTo((Func<LureableMonitor.Instance, float>) (smi => smi.def.cooldown), (StateMachine.BaseState) this.nolure);
    this.nolure.Update("FindLure", (System.Action<LureableMonitor.Instance, float>) ((smi, dt) => smi.FindLure()), (UpdateRate) 6).ParamTransition<GameObject>((StateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.Parameter<GameObject>) this.targetLure, this.haslure, (StateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Inequality((Object) p, (Object) null)));
    this.haslure.ParamTransition<GameObject>((StateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.Parameter<GameObject>) this.targetLure, this.nolure, (StateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Equality((Object) p, (Object) null))).Update("FindLure", (System.Action<LureableMonitor.Instance, float>) ((smi, dt) => smi.FindLure()), (UpdateRate) 6).ToggleBehaviour(GameTags.Creatures.MoveToLure, (StateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.Transition.ConditionCallback) (smi => smi.HasLure()), (System.Action<LureableMonitor.Instance>) (smi => smi.GoTo((StateMachine.BaseState) this.cooldown)));
  }

  public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
  {
    public float cooldown = 20f;
    public Tag[] lures;

    public List<Descriptor> GetDescriptors(GameObject go)
    {
      List<Descriptor> descriptors = new List<Descriptor>();
      descriptors.Add(new Descriptor((string) UI.BUILDINGEFFECTS.CAPTURE_METHOD_LURE, (string) UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_LURE, (Descriptor.DescriptorType) 1, false));
      return descriptors;
    }
  }

  public new class Instance : 
    GameStateMachine<LureableMonitor, LureableMonitor.Instance, IStateMachineTarget, LureableMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, LureableMonitor.Def def)
      : base(master, def)
    {
    }

    public void FindLure()
    {
      LureableMonitor.Instance.LureIterator iterator = new LureableMonitor.Instance.LureIterator(this.GetComponent<Navigator>(), this.def.lures);
      GameScenePartitioner.Instance.Iterate<LureableMonitor.Instance.LureIterator>(Grid.PosToCell(TransformExtensions.GetPosition(this.smi.transform)), 1, GameScenePartitioner.Instance.lure, ref iterator);
      iterator.Cleanup();
      this.sm.targetLure.Set(iterator.result, this);
    }

    public bool HasLure() => Object.op_Inequality((Object) this.sm.targetLure.Get(this), (Object) null);

    public GameObject GetTargetLure() => this.sm.targetLure.Get(this);

    private struct LureIterator : GameScenePartitioner.Iterator
    {
      private Navigator navigator;
      private Tag[] lures;

      public int cost { get; private set; }

      public GameObject result { get; private set; }

      public LureIterator(Navigator navigator, Tag[] lures)
      {
        this.navigator = navigator;
        this.lures = lures;
        this.cost = -1;
        this.result = (GameObject) null;
      }

      public void Iterate(object target_obj)
      {
        if (!(target_obj is Lure.Instance instance) || !instance.IsActive() || !instance.HasAnyLure(this.lures))
          return;
        int navigationCost = this.navigator.GetNavigationCost(Grid.PosToCell(TransformExtensions.GetPosition(instance.transform)), instance.def.lurePoints);
        if (navigationCost == -1 || this.cost != -1 && navigationCost >= this.cost)
          return;
        this.cost = navigationCost;
        this.result = instance.gameObject;
      }

      public void Cleanup()
      {
      }
    }
  }
}
