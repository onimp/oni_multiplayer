// Decompiled with JetBrains decompiler
// Type: User
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/User")]
public class User : KMonoBehaviour
{
  public void OnStateMachineStop(string reason, StateMachine.Status status)
  {
    if (status == StateMachine.Status.Success)
      this.Trigger(58624316, (object) null);
    else
      this.Trigger(1572098533, (object) null);
  }
}
