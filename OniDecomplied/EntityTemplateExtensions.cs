// Decompiled with JetBrains decompiler
// Type: EntityTemplateExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public static class EntityTemplateExtensions
{
  public static DefType AddOrGetDef<DefType>(this GameObject go) where DefType : StateMachine.BaseDef
  {
    StateMachineController machineController = go.AddOrGet<StateMachineController>();
    DefType def = machineController.GetDef<DefType>();
    if ((object) def == null)
    {
      def = Activator.CreateInstance<DefType>();
      machineController.AddDef((StateMachine.BaseDef) def);
      def.Configure(go);
    }
    return def;
  }

  public static ComponentType AddOrGet<ComponentType>(this GameObject go) where ComponentType : Component
  {
    ComponentType componentType = go.GetComponent<ComponentType>();
    if (Object.op_Equality((Object) (object) componentType, (Object) null))
      componentType = go.AddComponent<ComponentType>();
    return componentType;
  }
}
