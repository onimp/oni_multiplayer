// Decompiled with JetBrains decompiler
// Type: LaunchPadConditions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class LaunchPadConditions : KMonoBehaviour, IProcessConditionSet
{
  private List<ProcessCondition> conditions;

  public List<ProcessCondition> GetConditionSet(
    ProcessCondition.ProcessConditionType conditionType)
  {
    return conditionType != ProcessCondition.ProcessConditionType.RocketStorage ? (List<ProcessCondition>) null : this.conditions;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.conditions = new List<ProcessCondition>();
    this.conditions.Add((ProcessCondition) new TransferCargoCompleteCondition(((Component) this).gameObject));
  }
}
