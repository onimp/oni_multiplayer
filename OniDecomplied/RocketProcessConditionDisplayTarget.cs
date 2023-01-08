// Decompiled with JetBrains decompiler
// Type: RocketProcessConditionDisplayTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class RocketProcessConditionDisplayTarget : KMonoBehaviour, IProcessConditionSet, ISim1000ms
{
  private CraftModuleInterface craftModuleInterface;
  private Guid statusHandle = Guid.Empty;

  public List<ProcessCondition> GetConditionSet(
    ProcessCondition.ProcessConditionType conditionType)
  {
    if (Object.op_Equality((Object) this.craftModuleInterface, (Object) null))
      this.craftModuleInterface = ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface;
    return this.craftModuleInterface.GetConditionSet(conditionType);
  }

  public void Sim1000ms(float dt)
  {
    bool flag = false;
    foreach (ProcessCondition condition in this.GetConditionSet(ProcessCondition.ProcessConditionType.All))
    {
      if (condition.EvaluateCondition() == ProcessCondition.Status.Failure)
      {
        flag = true;
        if (this.statusHandle == Guid.Empty)
        {
          this.statusHandle = ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.RocketChecklistIncomplete);
          break;
        }
        break;
      }
    }
    if (flag || !(this.statusHandle != Guid.Empty))
      return;
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(this.statusHandle);
  }
}
