// Decompiled with JetBrains decompiler
// Type: MySmi
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MySmi : MyAttributeManager<StateMachine.Instance>
{
  public static void Init() => MyAttributes.Register((IAttributeManager) new MySmi(new Dictionary<System.Type, MethodInfo>()
  {
    {
      typeof (MySmiGet),
      typeof (MySmi).GetMethod("FindSmi")
    },
    {
      typeof (MySmiReq),
      typeof (MySmi).GetMethod("RequireSmi")
    }
  }));

  public MySmi(Dictionary<System.Type, MethodInfo> attributeMap)
    : base(attributeMap, (Action<StateMachine.Instance>) null)
  {
  }

  public static StateMachine.Instance FindSmi<T>(KMonoBehaviour c, bool isStart) where T : StateMachine.Instance
  {
    StateMachineController component = ((Component) c).GetComponent<StateMachineController>();
    return Object.op_Inequality((Object) component, (Object) null) ? (StateMachine.Instance) component.GetSMI<T>() : (StateMachine.Instance) null;
  }

  public static StateMachine.Instance RequireSmi<T>(KMonoBehaviour c, bool isStart) where T : StateMachine.Instance
  {
    if (!isStart)
      return MySmi.FindSmi<T>(c, isStart);
    StateMachine.Instance smi = MySmi.FindSmi<T>(c, isStart);
    Debug.Assert(smi != null, (object) string.Format("{0} '{1}' requires a StateMachineInstance of type {2}!", (object) ((object) c).GetType().ToString(), (object) ((Object) c).name, (object) typeof (T)));
    return smi;
  }
}
