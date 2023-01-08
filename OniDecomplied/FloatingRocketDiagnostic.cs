// Decompiled with JetBrains decompiler
// Type: FloatingRocketDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class FloatingRocketDiagnostic : ColonyDiagnostic
{
  public FloatingRocketDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.ALL_NAME)
  {
    this.icon = "icon_errand_rocketry";
  }

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override ColonyDiagnostic.DiagnosticResult Evaluate()
  {
    WorldContainer world = ClusterManager.Instance.GetWorld(this.worldID);
    Clustercraft component = ((Component) world).gameObject.GetComponent<Clustercraft>();
    ColonyDiagnostic.DiagnosticResult result = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.NO_MINIONS);
    if (ColonyDiagnosticUtility.IgnoreRocketsWithNoCrewRequested(this.worldID, out result))
      return result;
    result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    if (world.ParentWorldId == (int) ClusterManager.INVALID_WORLD_IDX || world.ParentWorldId == world.id)
    {
      result.Message = (string) UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_FLIGHT;
      if (AxialI.op_Equality(component.Destination, component.Location))
      {
        bool flag = false;
        foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) component.ModuleInterface.ClusterModules)
        {
          ResourceHarvestModule.StatesInstance smi = ((Component) clusterModule.Get()).GetSMI<ResourceHarvestModule.StatesInstance>();
          if (smi != null && smi.IsInsideState((StateMachine.BaseState) smi.sm.not_grounded.harvesting))
          {
            flag = true;
            break;
          }
        }
        if (flag)
        {
          result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
          result.Message = (string) UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_UTILITY;
        }
        else
        {
          result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Suggestion;
          result.Message = (string) UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.WARNING_NO_DESTINATION;
        }
      }
      else if ((double) component.Speed == 0.0)
      {
        result.opinion = ColonyDiagnostic.DiagnosticResult.Opinion.Concern;
        result.Message = (string) UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.WARNING_NO_SPEED;
      }
    }
    else
      result.Message = (string) UI.COLONY_DIAGNOSTICS.FLOATINGROCKETDIAGNOSTIC.NORMAL_LANDED;
    return result;
  }
}
