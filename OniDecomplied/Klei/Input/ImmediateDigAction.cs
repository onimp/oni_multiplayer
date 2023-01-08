// Decompiled with JetBrains decompiler
// Type: Klei.Input.ImmediateDigAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.Actions;
using UnityEngine;

namespace Klei.Input
{
  [Action("Immediate")]
  public class ImmediateDigAction : DigAction
  {
    public override void Dig(int cell, int distFromOrigin)
    {
      if (!Grid.Solid[cell] || Grid.Foundation[cell])
        return;
      SimMessages.Dig(cell);
    }

    protected override void Uproot(Uprootable uprootable)
    {
      if (Object.op_Equality((Object) uprootable, (Object) null))
        return;
      uprootable.Uproot();
    }
  }
}
