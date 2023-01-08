// Decompiled with JetBrains decompiler
// Type: BedDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BedDiagnostic : ColonyDiagnostic
{
  public BedDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.ALL_NAME)
  {
    this.icon = "icon_action_region_bedroom";
    this.AddCriterion("CheckEnoughBeds", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.CRITERIA.CHECKENOUGHBEDS, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckEnoughBeds)));
  }

  private ColonyDiagnostic.DiagnosticResult CheckEnoughBeds()
  {
    List<MinionIdentity> worldItems1 = Components.LiveMinionIdentities.GetWorldItems(this.worldID);
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    if (worldItems1.Count == 0)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS;
    }
    else
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.NORMAL;
      int num = 0;
      List<Sleepable> worldItems2 = Components.Sleepables.GetWorldItems(this.worldID);
      for (int index = 0; index < worldItems2.Count; ++index)
      {
        if (Object.op_Inequality((Object) ((Component) worldItems2[index]).GetComponent<Assignable>(), (Object) null) && Object.op_Equality((Object) ((Component) worldItems2[index]).GetComponent<Clinic>(), (Object) null))
          ++num;
      }
      if (num < worldItems1.Count)
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.BEDDIAGNOSTIC.NOT_ENOUGH_BEDS;
      }
    }
    return diagnosticResult;
  }

  public override ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    return ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(this.worldID, out result) ? result : base.Evaluate();
  }

  public override string GetAverageValueString() => Components.Sleepables.GetWorldItems(this.worldID).FindAll((Predicate<Sleepable>) (match => Object.op_Inequality((Object) ((Component) match).GetComponent<Assignable>(), (Object) null))).Count.ToString() + "/" + Components.LiveMinionIdentities.GetWorldItems(this.worldID).Count.ToString();
}
