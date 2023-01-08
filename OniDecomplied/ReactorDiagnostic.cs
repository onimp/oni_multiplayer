// Decompiled with JetBrains decompiler
// Type: ReactorDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ReactorDiagnostic : ColonyDiagnostic
{
  public ReactorDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.ALL_NAME)
  {
    this.icon = "overlay_radiation";
    this.AddCriterion("CheckTemperature", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA.CHECKTEMPERATURE, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckTemperature)));
    this.AddCriterion("CheckCoolant", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA.CHECKCOOLANT, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckCoolant)));
  }

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  private ColonyDiagnostic.DiagnosticResult CheckTemperature()
  {
    List<Reactor> worldItems = Components.NuclearReactors.GetWorldItems(this.worldID);
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.NORMAL;
    foreach (Reactor reactor in worldItems)
    {
      if ((double) reactor.FuelTemperature > 1254.862548828125)
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Warning;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA_TEMPERATURE_WARNING;
        diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(((Component) reactor).gameObject.transform.position, ((Component) reactor).gameObject);
      }
    }
    return diagnosticResult;
  }

  private ColonyDiagnostic.DiagnosticResult CheckCoolant()
  {
    List<Reactor> worldItems = Components.NuclearReactors.GetWorldItems(this.worldID);
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.NORMAL;
    foreach (Reactor reactor in worldItems)
    {
      if (reactor.On && (double) reactor.ReserveCoolantMass <= 45.0)
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.REACTORDIAGNOSTIC.CRITERIA_COOLANT_WARNING;
        diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(((Component) reactor).gameObject.transform.position, ((Component) reactor).gameObject);
      }
    }
    return diagnosticResult;
  }
}
