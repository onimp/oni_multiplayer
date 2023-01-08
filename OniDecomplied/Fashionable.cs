// Decompiled with JetBrains decompiler
// Type: Fashionable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[SkipSaveFileSerialization]
public class Fashionable : StateMachineComponent<Fashionable.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  protected bool IsUncomfortable()
  {
    ClothingWearer component = ((Component) this).GetComponent<ClothingWearer>();
    return Object.op_Inequality((Object) component, (Object) null) && component.currentClothing.decorMod <= 0;
  }

  public class StatesInstance : 
    GameStateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable, object>.GameInstance
  {
    public StatesInstance(Fashionable master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable>
  {
    public GameStateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable, object>.State satisfied;
    public GameStateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable, object>.State suffering;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.satisfied;
      this.root.EventHandler(GameHashes.EquippedItemEquipper, (StateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable, object>.State.Callback) (smi =>
      {
        if (smi.master.IsUncomfortable())
          smi.GoTo((StateMachine.BaseState) this.suffering);
        else
          smi.GoTo((StateMachine.BaseState) this.satisfied);
      })).EventHandler(GameHashes.UnequippedItemEquipper, (StateMachine<Fashionable.States, Fashionable.StatesInstance, Fashionable, object>.State.Callback) (smi =>
      {
        if (smi.master.IsUncomfortable())
          smi.GoTo((StateMachine.BaseState) this.suffering);
        else
          smi.GoTo((StateMachine.BaseState) this.satisfied);
      }));
      this.suffering.AddEffect("UnfashionableClothing").ToggleExpression(Db.Get().Expressions.Uncomfortable);
      this.satisfied.DoNothing();
    }
  }
}
