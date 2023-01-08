// Decompiled with JetBrains decompiler
// Type: StateMachineControllerExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public static class StateMachineControllerExtensions
{
  public static StateMachineInstanceType GetSMI<StateMachineInstanceType>(
    this StateMachine.Instance smi)
    where StateMachineInstanceType : StateMachine.Instance
  {
    return smi.gameObject.GetSMI<StateMachineInstanceType>();
  }

  public static DefType GetDef<DefType>(this Component cmp) where DefType : StateMachine.BaseDef => cmp.gameObject.GetDef<DefType>();

  public static DefType GetDef<DefType>(this GameObject go) where DefType : StateMachine.BaseDef
  {
    StateMachineController component = go.GetComponent<StateMachineController>();
    return Object.op_Equality((Object) component, (Object) null) ? default (DefType) : component.GetDef<DefType>();
  }

  public static StateMachineInstanceType GetSMI<StateMachineInstanceType>(this Component cmp) where StateMachineInstanceType : class => cmp.gameObject.GetSMI<StateMachineInstanceType>();

  public static StateMachineInstanceType GetSMI<StateMachineInstanceType>(this GameObject go) where StateMachineInstanceType : class
  {
    StateMachineController component = go.GetComponent<StateMachineController>();
    return Object.op_Inequality((Object) component, (Object) null) ? component.GetSMI<StateMachineInstanceType>() : default (StateMachineInstanceType);
  }

  public static List<StateMachineInstanceType> GetAllSMI<StateMachineInstanceType>(
    this Component cmp)
    where StateMachineInstanceType : class
  {
    return cmp.gameObject.GetAllSMI<StateMachineInstanceType>();
  }

  public static List<StateMachineInstanceType> GetAllSMI<StateMachineInstanceType>(
    this GameObject go)
    where StateMachineInstanceType : class
  {
    StateMachineController component = go.GetComponent<StateMachineController>();
    return Object.op_Inequality((Object) component, (Object) null) ? component.GetAllSMI<StateMachineInstanceType>() : new List<StateMachineInstanceType>();
  }
}
