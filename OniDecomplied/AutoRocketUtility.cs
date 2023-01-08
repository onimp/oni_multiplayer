// Decompiled with JetBrains decompiler
// Type: AutoRocketUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AutoRocketUtility
{
  public static void StartAutoRocket(LaunchPad selectedPad) => ((MonoBehaviour) selectedPad).StartCoroutine(AutoRocketUtility.AutoRocketRoutine(selectedPad));

  private static IEnumerator AutoRocketRoutine(LaunchPad selectedPad)
  {
    GameObject oxidizerTank = AutoRocketUtility.AddOxidizerTank(AutoRocketUtility.AddEngine(selectedPad));
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    AutoRocketUtility.AddOxidizer(oxidizerTank);
    GameObject baseModule = AutoRocketUtility.AddPassengerModule(oxidizerTank);
    AutoRocketUtility.AddDrillCone(AutoRocketUtility.AddSolidStorageModule(baseModule));
    PassengerRocketModule passengerModule = baseModule.GetComponent<PassengerRocketModule>();
    ClustercraftExteriorDoor exteriorDoor = ((Component) passengerModule).GetComponent<ClustercraftExteriorDoor>();
    int max = 100;
    while (Object.op_Equality((Object) exteriorDoor.GetInteriorDoor(), (Object) null) && max > 0)
    {
      --max;
      yield return (object) SequenceUtil.WaitForEndOfFrame;
    }
    WorldContainer interiorWorld = ((Component) passengerModule).GetComponent<RocketModuleCluster>().CraftInterface.GetInteriorWorld();
    RocketControlStation worldItem = Components.RocketControlStations.GetWorldItems(interiorWorld.id)[0];
    GameObject minion = AutoRocketUtility.AddPilot(worldItem);
    AutoRocketUtility.AddOxygen(worldItem);
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    AutoRocketUtility.AssignCrew(minion, passengerModule);
  }

  private static GameObject AddEngine(LaunchPad selectedPad)
  {
    BuildingDef buildingDef = Assets.GetBuildingDef("KeroseneEngineClusterSmall");
    List<Tag> tagList = new List<Tag>();
    tagList.Add(SimHashes.Steel.CreateTag());
    List<Tag> elements = tagList;
    GameObject gameObject = selectedPad.AddBaseModule(buildingDef, (IList<Tag>) elements);
    Element element = ElementLoader.GetElement(gameObject.GetComponent<RocketEngineCluster>().fuelTag);
    Storage component = gameObject.GetComponent<Storage>();
    if (element.IsGas)
    {
      component.AddGasChunk(element.id, component.Capacity(), element.defaultValues.temperature, byte.MaxValue, 0, false);
      return gameObject;
    }
    if (element.IsLiquid)
    {
      component.AddLiquid(element.id, component.Capacity(), element.defaultValues.temperature, byte.MaxValue, 0);
      return gameObject;
    }
    if (!element.IsSolid)
      return gameObject;
    component.AddOre(element.id, component.Capacity(), element.defaultValues.temperature, byte.MaxValue, 0);
    return gameObject;
  }

  private static GameObject AddPassengerModule(GameObject baseModule)
  {
    ReorderableBuilding component = baseModule.GetComponent<ReorderableBuilding>();
    BuildingDef buildingDef = Assets.GetBuildingDef("HabitatModuleMedium");
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(SimHashes.Cuprite.CreateTag());
    List<Tag> tagList2 = tagList1;
    BuildingDef def = buildingDef;
    List<Tag> buildMaterials = tagList2;
    return component.AddModule(def, (IList<Tag>) buildMaterials);
  }

  private static GameObject AddSolidStorageModule(GameObject baseModule)
  {
    ReorderableBuilding component = baseModule.GetComponent<ReorderableBuilding>();
    BuildingDef buildingDef = Assets.GetBuildingDef("SolidCargoBaySmall");
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(SimHashes.Steel.CreateTag());
    List<Tag> tagList2 = tagList1;
    BuildingDef def = buildingDef;
    List<Tag> buildMaterials = tagList2;
    return component.AddModule(def, (IList<Tag>) buildMaterials);
  }

  private static GameObject AddDrillCone(GameObject baseModule)
  {
    ReorderableBuilding component = baseModule.GetComponent<ReorderableBuilding>();
    BuildingDef buildingDef = Assets.GetBuildingDef("NoseconeHarvest");
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(SimHashes.Steel.CreateTag());
    tagList1.Add(SimHashes.Polypropylene.CreateTag());
    List<Tag> tagList2 = tagList1;
    BuildingDef def = buildingDef;
    List<Tag> buildMaterials = tagList2;
    GameObject gameObject = component.AddModule(def, (IList<Tag>) buildMaterials);
    gameObject.GetComponent<Storage>().AddOre(SimHashes.Diamond, 1000f, 273f, byte.MaxValue, 0);
    return gameObject;
  }

  private static GameObject AddOxidizerTank(GameObject baseModule)
  {
    ReorderableBuilding component = baseModule.GetComponent<ReorderableBuilding>();
    BuildingDef buildingDef = Assets.GetBuildingDef("SmallOxidizerTank");
    List<Tag> tagList1 = new List<Tag>();
    tagList1.Add(SimHashes.Cuprite.CreateTag());
    List<Tag> tagList2 = tagList1;
    BuildingDef def = buildingDef;
    List<Tag> buildMaterials = tagList2;
    return component.AddModule(def, (IList<Tag>) buildMaterials);
  }

  private static void AddOxidizer(GameObject oxidizerTank)
  {
    SimHashes simHashes = SimHashes.OxyRock;
    Element elementByHash = ElementLoader.FindElementByHash(simHashes);
    DiscoveredResources.Instance.Discover(elementByHash.tag, elementByHash.GetMaterialCategoryTag());
    oxidizerTank.GetComponent<OxidizerTank>().DEBUG_FillTank(simHashes);
  }

  private static GameObject AddPilot(RocketControlStation station)
  {
    Vector3 position = station.transform.position;
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID)), (GameObject) null, (string) null);
    ((Object) gameObject).name = ((Object) Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID))).name;
    Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
    Vector3 posCbc = Grid.CellToPosCBC(Grid.PosToCell(position), Grid.SceneLayer.Move);
    TransformExtensions.SetLocalPosition(gameObject.transform, posCbc);
    gameObject.SetActive(true);
    new MinionStartingStats(false, isDebugMinion: true).Apply(gameObject);
    MinionResume component = gameObject.GetComponent<MinionResume>();
    if (DebugHandler.InstantBuildMode && component.AvailableSkillpoints < 1)
      component.ForceAddSkillPoint();
    string id = Db.Get().Skills.RocketPiloting1.Id;
    MinionResume.SkillMasteryConditions[] masteryConditions = component.GetSkillMasteryConditions(id);
    bool flag = component.CanMasterSkill(masteryConditions);
    if (((!Object.op_Inequality((Object) component, (Object) null) ? 0 : (!component.HasMasteredSkill(id) ? 1 : 0)) & (flag ? 1 : 0)) != 0)
      component.MasterSkill(id);
    return gameObject;
  }

  private static void AddOxygen(RocketControlStation station) => SimMessages.ReplaceElement(Grid.PosToCell(Vector3.op_Addition(station.transform.position, Vector3.op_Multiply(Vector3.up, 2f))), SimHashes.OxyRock, CellEventLogger.Instance.DebugTool, 1000f, 273f);

  private static void AssignCrew(GameObject minion, PassengerRocketModule passengerModule)
  {
    for (int idx = 0; idx < Components.MinionAssignablesProxy.Count; ++idx)
    {
      if (Object.op_Equality((Object) Components.MinionAssignablesProxy[idx].GetTargetGameObject(), (Object) minion))
      {
        ((Component) passengerModule).GetComponent<AssignmentGroupController>().SetMember(Components.MinionAssignablesProxy[idx], true);
        break;
      }
    }
    passengerModule.RequestCrewBoard(PassengerRocketModule.RequestCrewState.Request);
  }

  private static void SetDestination(
    CraftModuleInterface craftModuleInterface,
    PassengerRocketModule passengerModule)
  {
    ((Component) craftModuleInterface).GetComponent<ClusterDestinationSelector>().SetDestination(AxialI.op_Addition(passengerModule.GetMyWorldLocation(), AxialI.NORTHEAST));
  }
}
