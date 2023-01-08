// Decompiled with JetBrains decompiler
// Type: EntombedChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class EntombedChore : Chore<EntombedChore.StatesInstance>
{
  public EntombedChore(IStateMachineTarget target, string entombedAnimOverride)
    : base(Db.Get().ChoreTypes.Entombed, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new EntombedChore.StatesInstance(this, target.gameObject, entombedAnimOverride);
  }

  public class StatesInstance : 
    GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.GameInstance
  {
    public string entombedAnimOverride;

    public StatesInstance(EntombedChore master, GameObject entombable, string entombedAnimOverride)
      : base(master)
    {
      this.sm.entombable.Set(entombable, this.smi, false);
      this.entombedAnimOverride = entombedAnimOverride;
    }

    public void UpdateFaceEntombed()
    {
      int num = Grid.CellAbove(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
      this.sm.isFaceEntombed.Set(Grid.IsValidCell(num) && Grid.Solid[num], this.smi);
    }
  }

  public class States : 
    GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore>
  {
    public StateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.BoolParameter isFaceEntombed;
    public StateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.TargetParameter entombable;
    public GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.State entombedface;
    public GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.State entombedbody;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.entombedbody;
      this.Target(this.entombable);
      this.root.ToggleAnims((Func<EntombedChore.StatesInstance, HashedString>) (smi => HashedString.op_Implicit(smi.entombedAnimOverride))).Update("IsFaceEntombed", (Action<EntombedChore.StatesInstance, float>) ((smi, dt) => smi.UpdateFaceEntombed())).ToggleStatusItem(Db.Get().DuplicantStatusItems.EntombedChore);
      this.entombedface.PlayAnim("entombed_ceiling", (KAnim.PlayMode) 0).ParamTransition<bool>((StateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.Parameter<bool>) this.isFaceEntombed, this.entombedbody, GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.IsFalse);
      this.entombedbody.PlayAnim("entombed_floor", (KAnim.PlayMode) 0).StopMoving().ParamTransition<bool>((StateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.Parameter<bool>) this.isFaceEntombed, this.entombedface, GameStateMachine<EntombedChore.States, EntombedChore.StatesInstance, EntombedChore, object>.IsTrue);
    }
  }
}
