// Decompiled with JetBrains decompiler
// Type: Trap
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Trap : StateMachineComponent<Trap.StatesInstance>
{
  [Serialize]
  private Ref<KPrefabID> contents;
  public TagSet captureTags = new TagSet();
  private static StatusItem statusReady;
  private static StatusItem statusSprung;

  private static void CreateStatusItems()
  {
    if (Trap.statusSprung != null)
      return;
    Trap.statusReady = new StatusItem("Ready", (string) BUILDING.STATUSITEMS.CREATURE_TRAP.READY.NAME, (string) BUILDING.STATUSITEMS.CREATURE_TRAP.READY.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
    Trap.statusSprung = new StatusItem("Sprung", (string) BUILDING.STATUSITEMS.CREATURE_TRAP.SPRUNG.NAME, (string) BUILDING.STATUSITEMS.CREATURE_TRAP.SPRUNG.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
    Trap.statusSprung.resolveTooltipCallback = (Func<string, object, string>) ((str, obj) =>
    {
      Trap.StatesInstance statesInstance = (Trap.StatesInstance) obj;
      return string.Format(str, (object) ((Component) statesInstance.master.contents.Get()).GetProperName());
    });
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.contents = new Ref<KPrefabID>();
    Trap.CreateStatusItems();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Storage component1 = ((Component) this).GetComponent<Storage>();
    this.smi.StartSM();
    if (component1.IsEmpty())
      return;
    KPrefabID component2 = component1.items[0].GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component2, (Object) null))
    {
      this.contents.Set(component2);
      this.smi.GoTo((StateMachine.BaseState) this.smi.sm.occupied);
    }
    else
      component1.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
  }

  public class StatesInstance : 
    GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.GameInstance
  {
    public StatesInstance(Trap master)
      : base(master)
    {
    }

    public void OnTrapTriggered(object data)
    {
      this.master.contents.Set(((GameObject) data).GetComponent<KPrefabID>());
      this.smi.sm.trapTriggered.Trigger(this.smi);
    }
  }

  public class States : GameStateMachine<Trap.States, Trap.StatesInstance, Trap>
  {
    public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State ready;
    public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State trapping;
    public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State finishedUsing;
    public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State destroySelf;
    public StateMachine<Trap.States, Trap.StatesInstance, Trap, object>.Signal trapTriggered;
    public Trap.States.OccupiedStates occupied;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.ready;
      this.serializable = StateMachine.SerializeType.Never;
      Trap.CreateStatusItems();
      this.ready.EventHandler(GameHashes.TrapTriggered, (GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.GameEvent.Callback) ((smi, data) => smi.OnTrapTriggered(data))).OnSignal(this.trapTriggered, this.trapping).ToggleStatusItem(Trap.statusReady);
      this.trapping.PlayAnim("working_pre").OnAnimQueueComplete((GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State) this.occupied);
      this.occupied.ToggleTag(GameTags.Trapped).ToggleStatusItem(Trap.statusSprung, (Func<Trap.StatesInstance, object>) (smi => (object) smi)).DefaultState(this.occupied.idle).EventTransition(GameHashes.OnStorageChange, this.finishedUsing, (StateMachine<Trap.States, Trap.StatesInstance, Trap, object>.Transition.ConditionCallback) (smi => ((Component) smi.master).GetComponent<Storage>().IsEmpty()));
      this.occupied.idle.PlayAnim("working_loop", (KAnim.PlayMode) 0);
      this.finishedUsing.PlayAnim("working_pst").OnAnimQueueComplete(this.destroySelf);
      this.destroySelf.Enter((StateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State.Callback) (smi => Util.KDestroyGameObject(((Component) smi.master).gameObject)));
    }

    public class OccupiedStates : 
      GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State
    {
      public GameStateMachine<Trap.States, Trap.StatesInstance, Trap, object>.State idle;
    }
  }
}
