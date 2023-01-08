// Decompiled with JetBrains decompiler
// Type: Decomposer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Decomposer")]
public class Decomposer : KMonoBehaviour
{
  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    StateMachineController component = ((Component) this).GetComponent<StateMachineController>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    DecompositionMonitor.Instance state_machine = new DecompositionMonitor.Instance((IStateMachineTarget) this, (Klei.AI.Disease) null, 1f, false);
    component.AddStateMachineInstance((StateMachine.Instance) state_machine);
    state_machine.StartSM();
    state_machine.dirtyWaterMaxRange = 3;
  }
}
