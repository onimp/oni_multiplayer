// Decompiled with JetBrains decompiler
// Type: SweepBotTrappedMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SweepBotTrappedMonitor : 
  GameStateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>
{
  public static CellOffset[] defaultOffsets = new CellOffset[1]
  {
    new CellOffset(0, 0)
  };
  public GameStateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>.State notTrapped;
  public GameStateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>.State trapped;
  public GameStateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>.State death;
  public GameStateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>.State destroySelf;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.notTrapped;
    this.notTrapped.Update((System.Action<SweepBotTrappedMonitor.Instance, float>) ((smi, dt) =>
    {
      StorageUnloadMonitor.Instance smi1 = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
      Storage cmp = smi1.sm.sweepLocker.Get(smi1);
      Navigator component = smi.master.GetComponent<Navigator>();
      if (Object.op_Equality((Object) cmp, (Object) null))
      {
        smi.GoTo((StateMachine.BaseState) this.death);
      }
      else
      {
        if (!smi.master.gameObject.HasTag(GameTags.Robots.Behaviours.RechargeBehaviour) && !smi.master.gameObject.HasTag(GameTags.Robots.Behaviours.UnloadBehaviour) || component.CanReach(Grid.PosToCell((KMonoBehaviour) cmp), SweepBotTrappedMonitor.defaultOffsets))
          return;
        smi.GoTo((StateMachine.BaseState) this.trapped);
      }
    }), (UpdateRate) 6);
    this.trapped.ToggleBehaviour(GameTags.Robots.Behaviours.TrappedBehaviour, (StateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>.Transition.ConditionCallback) (data => true)).ToggleStatusItem(Db.Get().RobotStatusItems.CantReachStation, (Func<SweepBotTrappedMonitor.Instance, object>) null, Db.Get().StatusItemCategories.Main).Update((System.Action<SweepBotTrappedMonitor.Instance, float>) ((smi, dt) =>
    {
      StorageUnloadMonitor.Instance smi2 = smi.master.gameObject.GetSMI<StorageUnloadMonitor.Instance>();
      Storage cmp = smi2.sm.sweepLocker.Get(smi2);
      Navigator component = smi.master.GetComponent<Navigator>();
      if (Object.op_Equality((Object) cmp, (Object) null))
        smi.GoTo((StateMachine.BaseState) this.death);
      else if (!smi.master.gameObject.HasTag(GameTags.Robots.Behaviours.RechargeBehaviour) && !smi.master.gameObject.HasTag(GameTags.Robots.Behaviours.UnloadBehaviour) || component.CanReach(Grid.PosToCell((KMonoBehaviour) cmp), SweepBotTrappedMonitor.defaultOffsets))
        smi.GoTo((StateMachine.BaseState) this.notTrapped);
      if (Object.op_Inequality((Object) cmp, (Object) null) && component.CanReach(Grid.PosToCell((KMonoBehaviour) cmp), SweepBotTrappedMonitor.defaultOffsets))
      {
        smi.GoTo((StateMachine.BaseState) this.notTrapped);
      }
      else
      {
        if (!Object.op_Equality((Object) cmp, (Object) null))
          return;
        smi.GoTo((StateMachine.BaseState) this.death);
      }
    }), (UpdateRate) 6);
    this.death.Enter((StateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>.State.Callback) (smi =>
    {
      smi.master.gameObject.GetComponent<OrnamentReceptacle>().OrderRemoveOccupant();
      smi.master.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnim(HashedString.op_Implicit("death"));
    })).OnAnimQueueComplete(this.destroySelf);
    this.destroySelf.Enter((StateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>.State.Callback) (smi =>
    {
      Game.Instance.SpawnFX(SpawnFXHashes.MeteorImpactDust, smi.master.transform.position, 0.0f);
      foreach (Storage component in smi.master.gameObject.GetComponents<Storage>())
      {
        for (int index = component.items.Count - 1; index >= 0; --index)
        {
          GameObject go = component.Drop(component.items[index], true);
          if (Object.op_Inequality((Object) go, (Object) null))
          {
            if (((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) go))
              ((KGameObjectComponentManager<FallerComponent>) GameComps.Fallers).Remove(go);
            GameComps.Fallers.Add(go, new Vector2((float) Random.Range(-5, 5), 8f));
          }
        }
      }
      PrimaryElement component1 = smi.master.GetComponent<PrimaryElement>();
      component1.Element.substance.SpawnResource(Grid.CellToPosCCC(Grid.PosToCell(smi.master.gameObject), Grid.SceneLayer.Ore), SweepBotConfig.MASS, component1.Temperature, component1.DiseaseIdx, component1.DiseaseCount);
      Util.KDestroyGameObject(smi.master.gameObject);
    }));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<SweepBotTrappedMonitor, SweepBotTrappedMonitor.Instance, IStateMachineTarget, SweepBotTrappedMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, SweepBotTrappedMonitor.Def def)
      : base(master, def)
    {
    }
  }
}
