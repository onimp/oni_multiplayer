// Decompiled with JetBrains decompiler
// Type: PowerUseDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PowerUseDiagnostic : ColonyDiagnostic
{
  public PowerUseDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.ALL_NAME)
  {
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<PowerUseTracker>(worldID);
    this.trackerSampleCountSeconds = 30f;
    this.icon = "overlay_power";
    this.AddCriterion("CheckOverWattage", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.CRITERIA.CHECKOVERWATTAGE, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckOverWattage)));
    this.AddCriterion("CheckPowerUseChange", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.CRITERIA.CHECKPOWERUSECHANGE, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckPowerChange)));
  }

  private ColonyDiagnostic.DiagnosticResult CheckOverWattage()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.NORMAL;
    foreach (ElectricalUtilityNetwork network in (IEnumerable<UtilityNetwork>) Game.Instance.electricalConduitSystem.GetNetworks())
    {
      if (network.allWires != null && network.allWires.Count != 0)
      {
        int cell = Grid.PosToCell((KMonoBehaviour) network.allWires[0]);
        if ((int) Grid.WorldIdx[cell] == this.worldID)
        {
          ushort circuitId = Game.Instance.circuitManager.GetCircuitID(cell);
          float wattageForCircuit = Game.Instance.circuitManager.GetMaxSafeWattageForCircuit(circuitId);
          float wattsUsedByCircuit = Game.Instance.circuitManager.GetWattsUsedByCircuit(circuitId);
          if ((double) wattsUsedByCircuit > (double) wattageForCircuit)
          {
            GameObject gameObject = ((Component) network.allWires[0]).gameObject;
            diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(gameObject.transform.position, gameObject);
            diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
            diagnosticResult.Message = string.Format((string) UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.CIRCUIT_OVER_CAPACITY, (object) GameUtil.GetFormattedWattage(wattsUsedByCircuit), (object) GameUtil.GetFormattedWattage(wattageForCircuit));
            break;
          }
        }
      }
    }
    return diagnosticResult;
  }

  private ColonyDiagnostic.DiagnosticResult CheckPowerChange()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.NORMAL;
    float num1 = 60f;
    if ((double) this.tracker.GetDataTimeLength() < (double) num1)
      return diagnosticResult;
    float averageValue1 = this.tracker.GetAverageValue(1f);
    float averageValue2 = this.tracker.GetAverageValue(Mathf.Min(60f, this.trackerSampleCountSeconds));
    float num2 = 240f;
    if ((double) averageValue1 < (double) num2 && (double) averageValue2 < (double) num2)
      return diagnosticResult;
    float num3 = 0.5f;
    if ((double) Mathf.Abs(averageValue1 - averageValue2) / (double) averageValue2 > (double) num3)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
      diagnosticResult.Message = string.Format((string) UI.COLONY_DIAGNOSTICS.POWERUSEDIAGNOSTIC.SIGNIFICANT_POWER_CHANGE_DETECTED, (object) GameUtil.GetFormattedWattage(averageValue2), (object) GameUtil.GetFormattedWattage(averageValue1));
    }
    return diagnosticResult;
  }

  public override ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    return ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(this.worldID, out result) ? result : base.Evaluate();
  }
}
