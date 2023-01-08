// Decompiled with JetBrains decompiler
// Type: ConditionProperlyFueled
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class ConditionProperlyFueled : ProcessCondition
{
  private IFuelTank fuelTank;

  public ConditionProperlyFueled(IFuelTank fuelTank) => this.fuelTank = fuelTank;

  public override ProcessCondition.Status EvaluateCondition()
  {
    RocketModuleCluster component1 = ((Component) this.fuelTank).GetComponent<RocketModuleCluster>();
    if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) component1.CraftInterface, (Object) null))
    {
      Clustercraft component2 = ((Component) component1.CraftInterface).GetComponent<Clustercraft>();
      ClusterTraveler component3 = ((Component) component1.CraftInterface).GetComponent<ClusterTraveler>();
      if (Object.op_Equality((Object) component2, (Object) null) || Object.op_Equality((Object) component3, (Object) null) || component3.CurrentPath == null)
        return ProcessCondition.Status.Failure;
      int hexes = component3.RemainingTravelNodes();
      if (hexes == 0)
        return !component2.HasResourcesToMove(combustionResource: Clustercraft.CombustionResource.Fuel) ? ProcessCondition.Status.Failure : ProcessCondition.Status.Ready;
      bool move1 = component2.HasResourcesToMove(hexes * 2, Clustercraft.CombustionResource.Fuel);
      bool move2 = component2.HasResourcesToMove(hexes, Clustercraft.CombustionResource.Fuel);
      if (move1)
        return ProcessCondition.Status.Ready;
      if (move2)
        return ProcessCondition.Status.Warning;
    }
    return ProcessCondition.Status.Failure;
  }

  public override string GetStatusMessage(ProcessCondition.Status status)
  {
    string statusMessage;
    switch (status)
    {
      case ProcessCondition.Status.Failure:
        statusMessage = (string) UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.STATUS.FAILURE;
        break;
      case ProcessCondition.Status.Ready:
        statusMessage = (string) UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.STATUS.READY;
        break;
      default:
        statusMessage = (string) UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.STATUS.WARNING;
        break;
    }
    return statusMessage;
  }

  public override string GetStatusTooltip(ProcessCondition.Status status)
  {
    Clustercraft component = ((Component) ((Component) this.fuelTank).GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<Clustercraft>();
    string statusTooltip;
    switch (status)
    {
      case ProcessCondition.Status.Failure:
        statusTooltip = (string) UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.FAILURE;
        break;
      case ProcessCondition.Status.Ready:
        statusTooltip = !AxialI.op_Equality(component.Destination, component.Location) ? (string) UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.READY : (string) UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.READY_NO_DESTINATION;
        break;
      default:
        statusTooltip = (string) UI.STARMAP.LAUNCHCHECKLIST.PROPERLY_FUELED.TOOLTIP.WARNING;
        break;
    }
    return statusTooltip;
  }

  public override bool ShowInUI() => true;
}
