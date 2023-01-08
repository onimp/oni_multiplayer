// Decompiled with JetBrains decompiler
// Type: ConditionDestinationReachable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class ConditionDestinationReachable : ProcessCondition
{
  private LaunchableRocketRegisterType craftRegisterType;
  private RocketModule module;

  public ConditionDestinationReachable(RocketModule module)
  {
    this.module = module;
    this.craftRegisterType = ((Component) module).GetComponent<ILaunchableRocket>().registerType;
  }

  public override ProcessCondition.Status EvaluateCondition()
  {
    ProcessCondition.Status condition = ProcessCondition.Status.Failure;
    switch (this.craftRegisterType)
    {
      case LaunchableRocketRegisterType.Spacecraft:
        int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this.module).GetComponent<LaunchConditionManager>()).id;
        SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
        if (spacecraftDestination != null && this.CanReachSpacecraftDestination(spacecraftDestination) && spacecraftDestination.GetDestinationType().visitable)
        {
          condition = ProcessCondition.Status.Ready;
          break;
        }
        break;
      case LaunchableRocketRegisterType.Clustercraft:
        if (!((Component) ((Component) this.module).GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<RocketClusterDestinationSelector>().IsAtDestination())
        {
          condition = ProcessCondition.Status.Ready;
          break;
        }
        break;
    }
    return condition;
  }

  public bool CanReachSpacecraftDestination(SpaceDestination destination)
  {
    Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
    float rocketMaxDistance = ((Component) this.module).GetComponent<CommandModule>().rocketStats.GetRocketMaxDistance();
    return (double) destination.OneBasedDistance * 10000.0 <= (double) rocketMaxDistance;
  }

  public SpaceDestination GetSpacecraftDestination()
  {
    Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
    int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this.module).GetComponent<LaunchConditionManager>()).id;
    return SpacecraftManager.instance.GetSpacecraftDestination(id);
  }

  public override string GetStatusMessage(ProcessCondition.Status status)
  {
    string statusMessage = "";
    switch (this.craftRegisterType)
    {
      case LaunchableRocketRegisterType.Spacecraft:
        statusMessage = status != ProcessCondition.Status.Ready || this.GetSpacecraftDestination() == null ? (this.GetSpacecraftDestination() == null ? (string) UI.STARMAP.DESTINATIONSELECTION.NOTSELECTED : (string) UI.STARMAP.DESTINATIONSELECTION.UNREACHABLE) : (string) UI.STARMAP.DESTINATIONSELECTION.REACHABLE;
        break;
      case LaunchableRocketRegisterType.Clustercraft:
        statusMessage = (string) UI.STARMAP.DESTINATIONSELECTION.REACHABLE;
        break;
    }
    return statusMessage;
  }

  public override string GetStatusTooltip(ProcessCondition.Status status)
  {
    string statusTooltip = "";
    switch (this.craftRegisterType)
    {
      case LaunchableRocketRegisterType.Spacecraft:
        statusTooltip = status != ProcessCondition.Status.Ready || this.GetSpacecraftDestination() == null ? (this.GetSpacecraftDestination() == null ? (string) UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED : (string) UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.UNREACHABLE) : (string) UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.REACHABLE;
        break;
      case LaunchableRocketRegisterType.Clustercraft:
        statusTooltip = status != ProcessCondition.Status.Ready ? (string) UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED : (string) UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.REACHABLE;
        break;
    }
    return statusTooltip;
  }

  public override bool ShowInUI() => true;
}
