// Decompiled with JetBrains decompiler
// Type: NearbyCreatureMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class NearbyCreatureMonitor : 
  GameStateMachine<NearbyCreatureMonitor, NearbyCreatureMonitor.Instance, IStateMachineTarget>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.Update("UpdateNearbyCreatures", (System.Action<NearbyCreatureMonitor.Instance, float>) ((smi, dt) => smi.UpdateNearbyCreatures(dt)), (UpdateRate) 6);
  }

  public new class Instance : 
    GameStateMachine<NearbyCreatureMonitor, NearbyCreatureMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public event System.Action<float, List<KPrefabID>> OnUpdateNearbyCreatures;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public void UpdateNearbyCreatures(float dt)
    {
      CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(this.gameObject));
      if (cavityForCell == null)
        return;
      this.OnUpdateNearbyCreatures(dt, cavityForCell.creatures);
    }
  }
}
