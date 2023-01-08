// Decompiled with JetBrains decompiler
// Type: FarmDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FarmDiagnostic : ColonyDiagnostic
{
  private List<PlantablePlot> plots;

  public FarmDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.ALL_NAME)
  {
    this.icon = "icon_errand_farm";
    this.AddCriterion("CheckHasFarms", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKHASFARMS, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckHasFarms)));
    this.AddCriterion("CheckPlanted", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKPLANTED, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckPlanted)));
    this.AddCriterion("CheckWilting", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKWILTING, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckWilting)));
    this.AddCriterion("CheckOperational", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.CRITERIA.CHECKOPERATIONAL, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckOperational)));
  }

  private void RefreshPlots() => this.plots = Components.PlantablePlots.GetItems(this.worldID).FindAll((Predicate<PlantablePlot>) (match => match.HasDepositTag(GameTags.CropSeed)));

  private ColonyDiagnostic.DiagnosticResult CheckHasFarms()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    if (this.plots.Count == 0)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NONE;
    }
    return diagnosticResult;
  }

  private ColonyDiagnostic.DiagnosticResult CheckPlanted()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    bool flag = false;
    foreach (PlantablePlot plot in this.plots)
    {
      if (Object.op_Inequality((Object) plot.plant, (Object) null))
      {
        flag = true;
        break;
      }
    }
    if (!flag)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.NONE_PLANTED;
    }
    return diagnosticResult;
  }

  private ColonyDiagnostic.DiagnosticResult CheckWilting()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    foreach (PlantablePlot plot in this.plots)
    {
      if (Object.op_Inequality((Object) plot.plant, (Object) null) && plot.plant.HasTag(GameTags.Wilting))
      {
        StandardCropPlant component = ((Component) plot.plant).GetComponent<StandardCropPlant>();
        if (Object.op_Inequality((Object) component, (Object) null) && component.smi.IsInsideState((StateMachine.BaseState) component.smi.sm.alive.wilting) && (double) component.smi.timeinstate > 15.0)
        {
          diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
          diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.WILTING;
          diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(plot.transform.position, ((Component) plot).gameObject);
          break;
        }
      }
    }
    return diagnosticResult;
  }

  private ColonyDiagnostic.DiagnosticResult CheckOperational()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    foreach (PlantablePlot plot in this.plots)
    {
      if (Object.op_Inequality((Object) plot.plant, (Object) null) && !((Component) plot).HasTag(GameTags.Operational))
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.FARMDIAGNOSTIC.INOPERATIONAL;
        diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(plot.transform.position, ((Component) plot).gameObject);
        break;
      }
    }
    return diagnosticResult;
  }

  public override string GetAverageValueString()
  {
    if (this.plots == null)
      this.RefreshPlots();
    return TrackerTool.Instance.GetWorldTracker<CropTracker>(this.worldID).GetCurrentValue().ToString() + "/" + this.plots.Count.ToString();
  }

  public override ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(this.worldID, out result))
      return result;
    this.RefreshPlots();
    return base.Evaluate();
  }
}
