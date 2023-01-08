// Decompiled with JetBrains decompiler
// Type: Klei.Actions.ActionAttribute
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace Klei.Actions
{
  [AttributeUsage(AttributeTargets.Class)]
  public class ActionAttribute : Attribute
  {
    public readonly string ActionName;

    public ActionAttribute(string actionName) => this.ActionName = actionName;
  }
}
