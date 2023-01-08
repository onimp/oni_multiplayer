// Decompiled with JetBrains decompiler
// Type: PowerUseTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PowerUseTracker : WorldTracker
{
  public PowerUseTracker(int worldID)
    : base(worldID)
  {
  }

  public override void UpdateData()
  {
    float num = 0.0f;
    foreach (ElectricalUtilityNetwork network in (IEnumerable<UtilityNetwork>) Game.Instance.electricalConduitSystem.GetNetworks())
    {
      if (network.allWires != null && network.allWires.Count != 0)
      {
        int cell = Grid.PosToCell((KMonoBehaviour) network.allWires[0]);
        if ((int) Grid.WorldIdx[cell] == this.WorldID)
          num += Game.Instance.circuitManager.GetWattsUsedByCircuit(Game.Instance.circuitManager.GetCircuitID(cell));
      }
    }
    this.AddPoint(Mathf.Round(num));
  }

  public override string FormatValueString(float value) => GameUtil.GetFormattedWattage(value);
}
