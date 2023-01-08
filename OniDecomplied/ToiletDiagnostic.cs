// Decompiled with JetBrains decompiler
// Type: ToiletDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ToiletDiagnostic : ColonyDiagnostic
{
  public ToiletDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.ALL_NAME)
  {
    this.icon = "icon_action_region_toilet";
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<WorkingToiletTracker>(worldID);
    this.AddCriterion("CheckHasAnyToilets", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.CRITERIA.CHECKHASANYTOILETS, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckHasAnyToilets)));
    this.AddCriterion("CheckEnoughToilets", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.CRITERIA.CHECKENOUGHTOILETS, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckEnoughToilets)));
    this.AddCriterion("CheckBladders", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.CRITERIA.CHECKBLADDERS, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckBladders)));
  }

  private ColonyDiagnostic.DiagnosticResult CheckHasAnyToilets()
  {
    List<IUsable> worldItems1 = Components.Toilets.GetWorldItems(this.worldID);
    List<MinionIdentity> worldItems2 = Components.LiveMinionIdentities.GetWorldItems(this.worldID);
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    if (worldItems2.Count == 0)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS;
    }
    else if (worldItems1.Count == 0)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NO_TOILETS;
    }
    return diagnosticResult;
  }

  private ColonyDiagnostic.DiagnosticResult CheckEnoughToilets()
  {
    Components.Toilets.GetWorldItems(this.worldID);
    List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(this.worldID);
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    if (worldItems.Count == 0)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS;
    }
    else
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NORMAL;
      if ((double) this.tracker.GetDataTimeLength() > 10.0 && (double) this.tracker.GetAverageValue(this.trackerSampleCountSeconds) <= 0.0)
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NO_WORKING_TOILETS;
      }
    }
    return diagnosticResult;
  }

  private ColonyDiagnostic.DiagnosticResult CheckBladders()
  {
    List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(this.worldID);
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    if (worldItems.Count == 0)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS;
    }
    else
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.NORMAL;
      foreach (Component cmp in worldItems)
      {
        PeeChoreMonitor.Instance smi = cmp.GetSMI<PeeChoreMonitor.Instance>();
        if (smi != null && smi.IsCritical())
        {
          diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Warning;
          diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.TOILETDIAGNOSTIC.TOILET_URGENT;
          break;
        }
      }
    }
    return diagnosticResult;
  }

  public override ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    return ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(this.worldID, out result) ? result : base.Evaluate();
  }

  public override string GetAverageValueString()
  {
    List<IUsable> worldItems = Components.Toilets.GetWorldItems(this.worldID);
    int count = worldItems.Count;
    for (int index = 0; index < worldItems.Count; ++index)
    {
      if (!worldItems[index].IsUsable())
        --count;
    }
    return count.ToString() + ":" + Components.LiveMinionIdentities.GetWorldItems(this.worldID).Count.ToString();
  }
}
