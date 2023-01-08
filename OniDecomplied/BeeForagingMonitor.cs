// Decompiled with JetBrains decompiler
// Type: BeeForagingMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class BeeForagingMonitor : 
  GameStateMachine<BeeForagingMonitor, BeeForagingMonitor.Instance, IStateMachineTarget, BeeForagingMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.Creatures.WantsToForage, new StateMachine<BeeForagingMonitor, BeeForagingMonitor.Instance, IStateMachineTarget, BeeForagingMonitor.Def>.Transition.ConditionCallback(BeeForagingMonitor.ShouldForage), (System.Action<BeeForagingMonitor.Instance>) (smi => smi.RefreshSearchTime()));
  }

  public static bool ShouldForage(BeeForagingMonitor.Instance smi)
  {
    bool flag = (double) GameClock.Instance.GetTimeInCycles() >= (double) smi.nextSearchTime;
    KPrefabID hiveInRoom = smi.master.GetComponent<Bee>().FindHiveInRoom();
    if (Object.op_Inequality((Object) hiveInRoom, (Object) null))
    {
      BeehiveCalorieMonitor.Instance smi1 = ((Component) hiveInRoom).GetSMI<BeehiveCalorieMonitor.Instance>();
      if (smi1 == null || !smi1.IsHungry())
        flag = false;
    }
    return flag && Object.op_Inequality((Object) hiveInRoom, (Object) null);
  }

  public class Def : StateMachine.BaseDef
  {
    public float searchMinInterval = 0.25f;
    public float searchMaxInterval = 0.3f;
  }

  public new class Instance : 
    GameStateMachine<BeeForagingMonitor, BeeForagingMonitor.Instance, IStateMachineTarget, BeeForagingMonitor.Def>.GameInstance
  {
    public float nextSearchTime;

    public Instance(IStateMachineTarget master, BeeForagingMonitor.Def def)
      : base(master, def)
    {
      this.RefreshSearchTime();
    }

    public void RefreshSearchTime() => this.nextSearchTime = GameClock.Instance.GetTimeInCycles() + Mathf.Lerp(this.def.searchMinInterval, this.def.searchMaxInterval, Random.value);
  }
}
