// Decompiled with JetBrains decompiler
// Type: BreathableCellQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class BreathableCellQuery : PathFinderQuery
{
  private OxygenBreather breather;

  public BreathableCellQuery Reset(Brain brain)
  {
    this.breather = ((Component) brain).GetComponent<OxygenBreather>();
    return this;
  }

  public override bool IsMatch(int cell, int parent_cell, int cost) => this.breather.IsBreathableElementAtCell(cell);
}
