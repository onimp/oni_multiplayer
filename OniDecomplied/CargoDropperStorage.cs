// Decompiled with JetBrains decompiler
// Type: CargoDropperStorage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CargoDropperStorage : 
  GameStateMachine<CargoDropperStorage, CargoDropperStorage.StatesInstance, IStateMachineTarget, CargoDropperStorage.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.EventHandler(GameHashes.JettisonCargo, (GameStateMachine<CargoDropperStorage, CargoDropperStorage.StatesInstance, IStateMachineTarget, CargoDropperStorage.Def>.GameEvent.Callback) ((smi, data) => smi.JettisonCargo(data)));
  }

  public class Def : StateMachine.BaseDef
  {
    public Vector3 dropOffset;
  }

  public class StatesInstance : 
    GameStateMachine<CargoDropperStorage, CargoDropperStorage.StatesInstance, IStateMachineTarget, CargoDropperStorage.Def>.GameInstance
  {
    public StatesInstance(IStateMachineTarget master, CargoDropperStorage.Def def)
      : base(master, def)
    {
    }

    public void JettisonCargo(object data)
    {
      Vector3 position1 = Vector3.op_Addition(TransformExtensions.GetPosition(this.master.transform), this.def.dropOffset);
      Storage component1 = this.GetComponent<Storage>();
      if (!Object.op_Inequality((Object) component1, (Object) null))
        return;
      GameObject first = component1.FindFirst(Tag.op_Implicit("ScoutRover"));
      if (Object.op_Inequality((Object) first, (Object) null))
      {
        component1.Drop(first, true);
        Vector3 position2 = TransformExtensions.GetPosition(this.master.transform);
        position2.z = Grid.GetLayerZ(Grid.SceneLayer.Creatures);
        TransformExtensions.SetPosition(first.transform, position2);
        ChoreProvider component2 = first.GetComponent<ChoreProvider>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          KBatchedAnimController component3 = first.GetComponent<KBatchedAnimController>();
          if (Object.op_Inequality((Object) component3, (Object) null))
            component3.Play(HashedString.op_Implicit("enter"));
          EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) component2, Db.Get().ChoreTypes.EmoteHighPriority, HashedString.op_Implicit((string) null), new HashedString[1]
          {
            HashedString.op_Implicit("enter")
          }, (KAnim.PlayMode) 1);
        }
        first.GetMyWorld().SetRoverLanded();
      }
      component1.DropAll(position1, offset: new Vector3());
    }
  }
}
