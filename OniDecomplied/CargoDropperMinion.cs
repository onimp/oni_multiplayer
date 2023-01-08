// Decompiled with JetBrains decompiler
// Type: CargoDropperMinion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CargoDropperMinion : 
  GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>
{
  private GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State notLanded;
  private GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State landed;
  private GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State exiting;
  private GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State complete;
  public StateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.BoolParameter hasLanded = new StateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.BoolParameter(false);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    default_state = (StateMachine.BaseState) this.notLanded;
    this.root.ParamTransition<bool>((StateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.Parameter<bool>) this.hasLanded, this.complete, GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.IsTrue);
    this.notLanded.EventHandlerTransition(GameHashes.JettisonCargo, this.landed, (Func<CargoDropperMinion.StatesInstance, object, bool>) ((smi, obj) => true));
    this.landed.Enter((StateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State.Callback) (smi =>
    {
      smi.JettisonCargo();
      smi.GoTo((StateMachine.BaseState) this.exiting);
    }));
    this.exiting.Update((System.Action<CargoDropperMinion.StatesInstance, float>) ((smi, dt) =>
    {
      if (smi.SyncMinionExitAnimation())
        return;
      smi.GoTo((StateMachine.BaseState) this.complete);
    }));
    this.complete.Enter((StateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.State.Callback) (smi => this.hasLanded.Set(true, smi)));
  }

  public class Def : StateMachine.BaseDef
  {
    public Vector3 dropOffset;
    public string kAnimName;
    public string animName;
    public Grid.SceneLayer animLayer = Grid.SceneLayer.Move;
    public bool notifyOnJettison;
  }

  public class StatesInstance : 
    GameStateMachine<CargoDropperMinion, CargoDropperMinion.StatesInstance, IStateMachineTarget, CargoDropperMinion.Def>.GameInstance
  {
    public MinionIdentity escapingMinion;
    public Chore exitAnimChore;

    public StatesInstance(IStateMachineTarget master, CargoDropperMinion.Def def)
      : base(master, def)
    {
    }

    public void JettisonCargo(object data = null)
    {
      Vector3 pos = Vector3.op_Addition(TransformExtensions.GetPosition(this.master.transform), this.def.dropOffset);
      MinionStorage component1 = this.GetComponent<MinionStorage>();
      if (!Object.op_Inequality((Object) component1, (Object) null))
        return;
      List<MinionStorage.Info> storedMinionInfo = component1.GetStoredMinionInfo();
      for (int index = storedMinionInfo.Count - 1; index >= 0; --index)
      {
        MinionStorage.Info info = storedMinionInfo[index];
        GameObject gameObject = component1.DeserializeMinion(info.id, pos);
        this.escapingMinion = gameObject.GetComponent<MinionIdentity>();
        gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
        ChoreProvider component2 = gameObject.GetComponent<ChoreProvider>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          this.exitAnimChore = (Chore) new EmoteChore((IStateMachineTarget) component2, Db.Get().ChoreTypes.EmoteHighPriority, HashedString.op_Implicit(this.def.kAnimName), new HashedString[1]
          {
            HashedString.op_Implicit(this.def.animName)
          }, (KAnim.PlayMode) 1);
          Vector3 position = TransformExtensions.GetPosition(gameObject.transform);
          position.z = Grid.GetLayerZ(this.def.animLayer);
          TransformExtensions.SetPosition(gameObject.transform, position);
          gameObject.GetMyWorld().SetDupeVisited();
        }
        if (this.def.notifyOnJettison)
          gameObject.GetComponent<Notifier>().Add(this.CreateCrashLandedNotification());
      }
    }

    public bool SyncMinionExitAnimation()
    {
      if (Object.op_Inequality((Object) this.escapingMinion, (Object) null) && this.exitAnimChore != null && !this.exitAnimChore.isComplete)
      {
        KBatchedAnimController component1 = ((Component) this.escapingMinion).GetComponent<KBatchedAnimController>();
        KBatchedAnimController component2 = this.master.GetComponent<KBatchedAnimController>();
        if (component2.CurrentAnim.name == this.def.animName)
        {
          component1.SetElapsedTime(component2.GetElapsedTime());
          return true;
        }
      }
      return false;
    }

    public Notification CreateCrashLandedNotification() => new Notification((string) MISC.NOTIFICATIONS.DUPLICANT_CRASH_LANDED.NAME, NotificationType.Bad, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) MISC.NOTIFICATIONS.DUPLICANT_CRASH_LANDED.TOOLTIP + notificationList.ReduceMessages(false)));
  }
}
