// Decompiled with JetBrains decompiler
// Type: TrappedDuplicantDiagnostic
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TrappedDuplicantDiagnostic : ColonyDiagnostic
{
  public TrappedDuplicantDiagnostic(int worldID)
    : base(worldID, (string) UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.ALL_NAME)
  {
    this.icon = "overlay_power";
    this.AddCriterion("CheckTrapped", new DiagnosticCriterion((string) UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.CRITERIA.CHECKTRAPPED, new Func<ColonyDiagnostic.DiagnosticResult>(this.CheckTrapped)));
  }

  public ColonyDiagnostic.DiagnosticResult CheckTrapped()
  {
    ColonyDiagnostic.DiagnosticResult diagnosticResult = new ColonyDiagnostic.DiagnosticResult(ColonyDiagnostic.DiagnosticResult.Opinion.Normal, (string) UI.COLONY_DIAGNOSTICS.GENERIC_CRITERIA_PASS);
    bool flag1 = false;
    foreach (MinionIdentity worldItem1 in Components.LiveMinionIdentities.GetWorldItems(this.worldID))
    {
      if (!flag1)
      {
        if (!ClusterManager.Instance.GetWorld(this.worldID).IsModuleInterior && this.CheckMinionBasicallyIdle(worldItem1))
        {
          Navigator component1 = ((Component) worldItem1).GetComponent<Navigator>();
          bool flag2 = true;
          foreach (MinionIdentity worldItem2 in Components.LiveMinionIdentities.GetWorldItems(this.worldID))
          {
            if (!Object.op_Equality((Object) worldItem1, (Object) worldItem2) && !this.CheckMinionBasicallyIdle(worldItem2) && component1.CanReach(((Component) worldItem2).GetComponent<IApproachable>()))
            {
              flag2 = false;
              break;
            }
          }
          List<Telepad> worldItems1 = Components.Telepads.GetWorldItems(component1.GetMyWorld().id);
          if (worldItems1 != null && worldItems1.Count > 0)
            flag2 = flag2 && !component1.CanReach(((Component) worldItems1[0]).GetComponent<IApproachable>());
          List<WarpReceiver> worldItems2 = Components.WarpReceivers.GetWorldItems(component1.GetMyWorld().id);
          if (worldItems2 != null && worldItems2.Count > 0)
          {
            foreach (WarpReceiver warpReceiver in worldItems2)
              flag2 = flag2 && !component1.CanReach(((Component) worldItems2[0]).GetComponent<IApproachable>());
          }
          List<Sleepable> worldItems3 = Components.Sleepables.GetWorldItems(component1.GetMyWorld().id);
          for (int index = 0; index < worldItems3.Count; ++index)
          {
            Assignable component2 = ((Component) worldItems3[index]).GetComponent<Assignable>();
            if (Object.op_Inequality((Object) component2, (Object) null) && component2.IsAssignedTo((IAssignableIdentity) worldItem1))
              flag2 = flag2 && !component1.CanReach(((Component) worldItems3[index]).GetComponent<IApproachable>());
          }
          if (flag2)
            diagnosticResult.clickThroughTarget = new Tuple<Vector3, GameObject>(worldItem1.transform.position, ((Component) worldItem1).gameObject);
          flag1 |= flag2;
        }
      }
      else
        break;
    }
    diagnosticResult.opinion = flag1 ? ColonyDiagnostic.DiagnosticResult.Opinion.Bad : ColonyDiagnostic.DiagnosticResult.Opinion.Normal;
    diagnosticResult.Message = (string) (flag1 ? UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.STUCK : UI.COLONY_DIAGNOSTICS.TRAPPEDDUPLICANTDIAGNOSTIC.NORMAL);
    return diagnosticResult;
  }

  private bool CheckMinionBasicallyIdle(MinionIdentity minion) => ((Component) minion).HasTag(GameTags.Idle) || ((Component) minion).HasTag(GameTags.RecoveringBreath) || ((Component) minion).HasTag(GameTags.MakingMess);
}
