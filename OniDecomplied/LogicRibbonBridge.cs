// Decompiled with JetBrains decompiler
// Type: LogicRibbonBridge
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class LogicRibbonBridge : KMonoBehaviour
{
  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    switch (((Component) this).GetComponent<Rotatable>().GetOrientation())
    {
      case Orientation.Neutral:
        component.Play(HashedString.op_Implicit("0"));
        break;
      case Orientation.R90:
        component.Play(HashedString.op_Implicit("90"));
        break;
      case Orientation.R180:
        component.Play(HashedString.op_Implicit("180"));
        break;
      case Orientation.R270:
        component.Play(HashedString.op_Implicit("270"));
        break;
    }
  }
}
