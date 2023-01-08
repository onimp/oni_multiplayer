// Decompiled with JetBrains decompiler
// Type: RocketCommandConditions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class RocketCommandConditions : KMonoBehaviour
{
  public ConditionDestinationReachable reachable;
  public ConditionHasAstronaut hasAstronaut;
  public ConditionPilotOnBoard pilotOnBoard;
  public ConditionPassengersOnBoard passengersOnBoard;
  public ConditionNoExtraPassengers noExtraPassengers;
  public ConditionHasAtmoSuit hasSuit;
  public CargoBayIsEmpty cargoEmpty;
  public ConditionHasMinimumMass destHasResources;
  public ConditionAllModulesComplete allModulesComplete;
  public ConditionHasControlStation hasControlStation;
  public ConditionHasEngine hasEngine;
  public ConditionHasNosecone hasNosecone;
  public ConditionOnLaunchPad onLaunchPad;
  public ConditionFlightPathIsClear flightPathIsClear;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    RocketModule component = ((Component) this).GetComponent<RocketModule>();
    this.reachable = (ConditionDestinationReachable) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition) new ConditionDestinationReachable(((Component) this).GetComponent<RocketModule>()));
    this.allModulesComplete = (ConditionAllModulesComplete) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition) new ConditionAllModulesComplete(((Component) this).GetComponent<ILaunchableRocket>()));
    if (((Component) this).GetComponent<ILaunchableRocket>().registerType == LaunchableRocketRegisterType.Spacecraft)
    {
      this.destHasResources = (ConditionHasMinimumMass) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition) new ConditionHasMinimumMass(((Component) this).GetComponent<CommandModule>()));
      this.hasAstronaut = (ConditionHasAstronaut) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition) new ConditionHasAstronaut(((Component) this).GetComponent<CommandModule>()));
      this.hasSuit = (ConditionHasAtmoSuit) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition) new ConditionHasAtmoSuit(((Component) this).GetComponent<CommandModule>()));
      this.cargoEmpty = (CargoBayIsEmpty) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (ProcessCondition) new CargoBayIsEmpty(((Component) this).GetComponent<CommandModule>()));
    }
    else if (((Component) this).GetComponent<ILaunchableRocket>().registerType == LaunchableRocketRegisterType.Clustercraft)
    {
      this.hasEngine = (ConditionHasEngine) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition) new ConditionHasEngine(((Component) this).GetComponent<ILaunchableRocket>()));
      this.hasNosecone = (ConditionHasNosecone) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition) new ConditionHasNosecone(((Component) this).GetComponent<LaunchableRocketCluster>()));
      this.hasControlStation = (ConditionHasControlStation) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition) new ConditionHasControlStation(((Component) this).GetComponent<RocketModuleCluster>()));
      this.pilotOnBoard = (ConditionPilotOnBoard) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketBoard, (ProcessCondition) new ConditionPilotOnBoard(((Component) this).GetComponent<PassengerRocketModule>()));
      this.passengersOnBoard = (ConditionPassengersOnBoard) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketBoard, (ProcessCondition) new ConditionPassengersOnBoard(((Component) this).GetComponent<PassengerRocketModule>()));
      this.noExtraPassengers = (ConditionNoExtraPassengers) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketBoard, (ProcessCondition) new ConditionNoExtraPassengers(((Component) this).GetComponent<PassengerRocketModule>()));
      this.onLaunchPad = (ConditionOnLaunchPad) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition) new ConditionOnLaunchPad(((Component) this).GetComponent<RocketModuleCluster>().CraftInterface));
    }
    int bufferWidth = 1;
    if (DlcManager.FeatureClusterSpaceEnabled())
      bufferWidth = 0;
    this.flightPathIsClear = (ConditionFlightPathIsClear) component.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketFlight, (ProcessCondition) new ConditionFlightPathIsClear(((Component) this).gameObject, bufferWidth));
  }
}
