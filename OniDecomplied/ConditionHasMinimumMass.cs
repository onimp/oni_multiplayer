// Decompiled with JetBrains decompiler
// Type: ConditionHasMinimumMass
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class ConditionHasMinimumMass : ProcessCondition
{
  private CommandModule commandModule;

  public ConditionHasMinimumMass(CommandModule command) => this.commandModule = command;

  public override ProcessCondition.Status EvaluateCondition()
  {
    int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this.commandModule).GetComponent<LaunchConditionManager>()).id;
    SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
    return spacecraftDestination != null && SpacecraftManager.instance.GetDestinationAnalysisState(spacecraftDestination) == SpacecraftManager.DestinationAnalysisState.Complete && (double) spacecraftDestination.AvailableMass >= (double) ConditionHasMinimumMass.CargoCapacity(spacecraftDestination, this.commandModule) ? ProcessCondition.Status.Ready : ProcessCondition.Status.Warning;
  }

  public override string GetStatusMessage(ProcessCondition.Status status)
  {
    int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this.commandModule).GetComponent<LaunchConditionManager>()).id;
    SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
    if (spacecraftDestination == null)
      return (string) UI.STARMAP.LAUNCHCHECKLIST.NO_DESTINATION;
    return SpacecraftManager.instance.GetDestinationAnalysisState(spacecraftDestination) == SpacecraftManager.DestinationAnalysisState.Complete ? string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.MINIMUM_MASS, (object) GameUtil.GetFormattedMass(spacecraftDestination.AvailableMass, massFormat: GameUtil.MetricMassFormat.Kilogram)) : string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.MINIMUM_MASS, (object) UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT);
  }

  public override string GetStatusTooltip(ProcessCondition.Status status)
  {
    int id = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) this.commandModule).GetComponent<LaunchConditionManager>()).id;
    SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(id);
    bool flag = spacecraftDestination != null && SpacecraftManager.instance.GetDestinationAnalysisState(spacecraftDestination) == SpacecraftManager.DestinationAnalysisState.Complete;
    string statusTooltip = "";
    if (flag)
    {
      if ((double) spacecraftDestination.AvailableMass <= (double) ConditionHasMinimumMass.CargoCapacity(spacecraftDestination, this.commandModule))
        statusTooltip = statusTooltip + (string) UI.STARMAP.LAUNCHCHECKLIST.INSUFFICENT_MASS_TOOLTIP + "\n\n";
      statusTooltip = statusTooltip + string.Format((string) UI.STARMAP.LAUNCHCHECKLIST.RESOURCE_MASS_TOOLTIP, (object) spacecraftDestination.GetDestinationType().Name, (object) GameUtil.GetFormattedMass(spacecraftDestination.AvailableMass, massFormat: GameUtil.MetricMassFormat.Kilogram), (object) GameUtil.GetFormattedMass(ConditionHasMinimumMass.CargoCapacity(spacecraftDestination, this.commandModule), massFormat: GameUtil.MetricMassFormat.Kilogram)) + "\n\n";
    }
    float num1 = spacecraftDestination != null ? spacecraftDestination.AvailableMass : 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.commandModule).GetComponent<AttachableBuilding>()))
    {
      CargoBay component = gameObject.GetComponent<CargoBay>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        if (flag)
        {
          float resourcesPercentage = spacecraftDestination.GetAvailableResourcesPercentage(component.storageType);
          float num2 = Mathf.Min(component.storage.Capacity(), resourcesPercentage * num1);
          num1 -= num2;
          statusTooltip = statusTooltip + ((Component) component).gameObject.GetProperName() + " " + string.Format((string) UI.STARMAP.STORAGESTATS.STORAGECAPACITY, (object) GameUtil.GetFormattedMass(Mathf.Min(num2, component.storage.Capacity()), massFormat: GameUtil.MetricMassFormat.Kilogram), (object) GameUtil.GetFormattedMass(component.storage.Capacity(), massFormat: GameUtil.MetricMassFormat.Kilogram)) + "\n";
        }
        else
          statusTooltip = statusTooltip + ((Component) component).gameObject.GetProperName() + " " + string.Format((string) UI.STARMAP.STORAGESTATS.STORAGECAPACITY, (object) GameUtil.GetFormattedMass(0.0f, massFormat: GameUtil.MetricMassFormat.Kilogram), (object) GameUtil.GetFormattedMass(component.storage.Capacity(), massFormat: GameUtil.MetricMassFormat.Kilogram)) + "\n";
      }
    }
    return statusTooltip;
  }

  public static float CargoCapacity(SpaceDestination destination, CommandModule module)
  {
    if (Object.op_Equality((Object) module, (Object) null))
      return 0.0f;
    float num = 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) module).GetComponent<AttachableBuilding>()))
    {
      CargoBay component1 = gameObject.GetComponent<CargoBay>();
      if (Object.op_Inequality((Object) component1, (Object) null) && destination.HasElementType(component1.storageType))
      {
        Storage component2 = ((Component) component1).GetComponent<Storage>();
        num += component2.capacityKg;
      }
    }
    return num;
  }

  public override bool ShowInUI() => true;
}
