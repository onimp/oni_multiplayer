// Decompiled with JetBrains decompiler
// Type: EntombedDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EntombedDiagnostic : ColonyDiagnostic
{
  private int m_entombedCount;

  public EntombedDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.ALL_NAME)
  {
    this.icon = "icon_action_dig";
    this.AddCriterion("CheckEntombed", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.CRITERIA.CHECKENTOMBED, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckEntombed)));
  }

  private ColonyDiagnostic.DiagnosticResult CheckEntombed()
  {
    List<BuildingComplete> worldItems = Components.EntombedBuildings.GetWorldItems(this.worldID);
    this.m_entombedCount = 0;
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.NORMAL;
    foreach (BuildingComplete cmp in worldItems)
    {
      if (!Util.IsNullOrDestroyed((object) cmp) && ((Component) cmp).HasTag(GameTags.Entombed))
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Bad;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.ENTOMBEDDIAGNOSTIC.BUILDING_ENTOMBED;
        diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(((Component) cmp).gameObject.transform.position, ((Component) cmp).gameObject);
        ++this.m_entombedCount;
      }
    }
    return diagnosticResult;
  }

  public override string GetAverageValueString() => this.m_entombedCount.ToString();
}
