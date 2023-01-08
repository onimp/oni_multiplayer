// Decompiled with JetBrains decompiler
// Type: CO2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/CO2")]
public class CO2 : KMonoBehaviour
{
  [Serialize]
  [NonSerialized]
  public Vector3 velocity = Vector3.zero;
  [Serialize]
  [NonSerialized]
  public float mass;
  [Serialize]
  [NonSerialized]
  public float temperature;
  [Serialize]
  [NonSerialized]
  public float lifetimeRemaining;

  public void StartLoop()
  {
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    component.Play(HashedString.op_Implicit("exhale_pre"));
    component.Play(HashedString.op_Implicit("exhale_loop"), (KAnim.PlayMode) 0);
  }

  public void TriggerDestroy() => ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("exhale_pst"));
}
