// Decompiled with JetBrains decompiler
// Type: FoodDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FoodDiagnostic : ColonyDiagnostic
{
  public FoodDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.ALL_NAME)
  {
    this.tracker = (Tracker) TrackerTool.Instance.GetWorldTracker<KCalTracker>(worldID);
    this.icon = "icon_category_food";
    this.trackerSampleCountSeconds = 150f;
    this.presentationSetting = ColonyDiagnostic.PresentationSetting.CurrentValue;
    this.AddCriterion("CheckEnoughFood", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA.CHECKENOUGHFOOD, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckEnoughFood)));
    this.AddCriterion("CheckStarvation", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA.CHECKSTARVATION, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckStarvation)));
  }

  private ColonyDiagnostic.DiagnosticResult CheckAnyFood()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA_HAS_FOOD.PASS);
    if (Components.LiveMinionIdentities.GetWorldItems(this.worldID).Count != 0)
    {
      if ((double) this.tracker.GetDataTimeLength() < 10.0)
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.NO_DATA;
      }
      else if ((double) this.tracker.GetAverageValue(this.trackerSampleCountSeconds) == 0.0)
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Bad;
        diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.CRITERIA_HAS_FOOD.FAIL;
      }
    }
    return diagnosticResult;
  }

  private ColonyDiagnostic.DiagnosticResult CheckEnoughFood()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    List<MinionIdentity> worldItems = Components.LiveMinionIdentities.GetWorldItems(this.worldID);
    if ((double) this.tracker.GetDataTimeLength() < 10.0)
    {
      diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
      diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.NO_DATA;
    }
    else
    {
      int num1 = 3000;
      if ((double) worldItems.Count * (1000.0 * (double) num1) > (double) this.tracker.GetAverageValue(this.trackerSampleCountSeconds))
      {
        diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
        float currentValue = this.tracker.GetCurrentValue();
        double num2 = (double) Components.LiveMinionIdentities.GetWorldItems(this.worldID).Count * -1000000.0;
        string formattedCalories1 = GameUtil.GetFormattedCalories(currentValue);
        string formattedCalories2 = GameUtil.GetFormattedCalories(Mathf.Abs((float) num2));
        string str = ((string) MISC.NOTIFICATIONS.FOODLOW.TOOLTIP).Replace("{0}", formattedCalories1).Replace("{1}", formattedCalories2);
        diagnosticResult.Message = str;
      }
    }
    return diagnosticResult;
  }

  private ColonyDiagnostic.DiagnosticResult CheckStarvation()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    foreach (MinionIdentity worldItem in Components.LiveMinionIdentities.GetWorldItems(this.worldID))
    {
      if (!worldItem.IsNull())
      {
        CalorieMonitor.Instance smi = ((Component) worldItem).GetSMI<CalorieMonitor.Instance>();
        if (!smi.IsNullOrStopped() && smi.IsInsideState((StateMachine.BaseState) smi.sm.hungry.starving))
        {
          diagnosticResult.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Bad;
          diagnosticResult.Message = (string) UI.COLONY_DIAGNOSTICS.FOODDIAGNOSTIC.HUNGRY;
          diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(smi.gameObject.transform.position, smi.gameObject);
        }
      }
    }
    return diagnosticResult;
  }

  public override string GetCurrentValueString() => GameUtil.GetFormattedCalories(this.tracker.GetCurrentValue());

  public override ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    return ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(this.worldID, out result) ? result : base.Evaluate();
  }
}
