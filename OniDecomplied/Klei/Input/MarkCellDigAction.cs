// Decompiled with JetBrains decompiler
// Type: Klei.Input.MarkCellDigAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.Actions;
using UnityEngine;

namespace Klei.Input
{
  [Action("Mark Cell")]
  public class MarkCellDigAction : DigAction
  {
    public override void Dig(int cell, int distFromOrigin)
    {
      GameObject gameObject = DigTool.PlaceDig(cell, distFromOrigin);
      if (!Object.op_Inequality((Object) gameObject, (Object) null))
        return;
      Prioritizable component = gameObject.GetComponent<Prioritizable>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
    }

    protected override void Uproot(Uprootable uprootable)
    {
      if (Object.op_Equality((Object) uprootable, (Object) null))
        return;
      uprootable.MarkForUproot();
    }
  }
}
