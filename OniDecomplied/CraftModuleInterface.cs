// Decompiled with JetBrains decompiler
// Type: CraftModuleInterface
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class CraftModuleInterface : KMonoBehaviour, ISim4000ms
{
  [Serialize]
  private List<Ref<RocketModule>> modules = new List<Ref<RocketModule>>();
  [Serialize]
  private List<Ref<RocketModuleCluster>> clusterModules = new List<Ref<RocketModuleCluster>>();
  private Ref<RocketModuleCluster> bottomModule;
  [Serialize]
  private Dictionary<int, Ref<LaunchPad>> preferredLaunchPad = new Dictionary<int, Ref<LaunchPad>>();
  [MyCmpReq]
  private Clustercraft m_clustercraft;
  private List<ProcessCondition.ProcessConditionType> conditionsToCheck = new List<ProcessCondition.ProcessConditionType>()
  {
    ProcessCondition.ProcessConditionType.RocketPrep,
    ProcessCondition.ProcessConditionType.RocketStorage,
    ProcessCondition.ProcessConditionType.RocketBoard,
    ProcessCondition.ProcessConditionType.RocketFlight
  };
  private int lastConditionTypeSucceeded = -1;
  private List<ProcessCondition> returnConditions = new List<ProcessCondition>();

  public IList<Ref<RocketModuleCluster>> ClusterModules => (IList<Ref<RocketModuleCluster>>) this.clusterModules.AsReadOnly();

  public LaunchPad GetPreferredLaunchPadForWorld(int world_id) => this.preferredLaunchPad.ContainsKey(world_id) ? this.preferredLaunchPad[world_id].Get() : (LaunchPad) null;

  private void SetPreferredLaunchPadForWorld(LaunchPad pad)
  {
    if (!this.preferredLaunchPad.ContainsKey(pad.GetMyWorldId()))
      this.preferredLaunchPad.Add(this.CurrentPad.GetMyWorldId(), new Ref<LaunchPad>());
    this.preferredLaunchPad[this.CurrentPad.GetMyWorldId()].Set(this.CurrentPad);
  }

  public LaunchPad CurrentPad
  {
    get
    {
      if (Object.op_Inequality((Object) this.m_clustercraft, (Object) null) && this.m_clustercraft.Status != Clustercraft.CraftStatus.InFlight && this.clusterModules.Count > 0)
      {
        if (this.bottomModule == null)
          this.SetBottomModule();
        Debug.Assert(this.bottomModule != null && Object.op_Inequality((Object) this.bottomModule.Get(), (Object) null), (object) "More than one cluster module but no bottom module found.");
        int num = Grid.CellBelow(Grid.PosToCell(this.bottomModule.Get().transform.position));
        if (Grid.IsValidCell(num))
        {
          GameObject gameObject = (GameObject) null;
          Grid.ObjectLayers[1].TryGetValue(num, out gameObject);
          if (Object.op_Inequality((Object) gameObject, (Object) null))
            return gameObject.GetComponent<LaunchPad>();
        }
      }
      return (LaunchPad) null;
    }
  }

  public float Speed => this.m_clustercraft.Speed;

  public float Range
  {
    get
    {
      foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      {
        RocketEngineCluster component = ((Component) clusterModule.Get()).GetComponent<RocketEngineCluster>();
        if (Object.op_Inequality((Object) component, (Object) null))
          return this.BurnableMassRemaining / ((Component) component).GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance;
      }
      return 0.0f;
    }
  }

  public float FuelPerHex
  {
    get
    {
      foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      {
        RocketEngineCluster component = ((Component) clusterModule.Get()).GetComponent<RocketEngineCluster>();
        if (Object.op_Inequality((Object) component, (Object) null))
          return ((Component) component).GetComponent<RocketModuleCluster>().performanceStats.FuelKilogramPerDistance * 600f;
      }
      return float.PositiveInfinity;
    }
  }

  public float BurnableMassRemaining
  {
    get
    {
      RocketEngineCluster rocketEngineCluster = (RocketEngineCluster) null;
      foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      {
        rocketEngineCluster = ((Component) clusterModule.Get()).GetComponent<RocketEngineCluster>();
        if (Object.op_Inequality((Object) rocketEngineCluster, (Object) null))
          break;
      }
      if (Object.op_Equality((Object) rocketEngineCluster, (Object) null))
        return 0.0f;
      return !rocketEngineCluster.requireOxidizer ? this.FuelRemaining : Mathf.Min(this.FuelRemaining, this.OxidizerPowerRemaining);
    }
  }

  public float FuelRemaining
  {
    get
    {
      RocketEngineCluster engine = this.GetEngine();
      if (Object.op_Equality((Object) engine, (Object) null))
        return 0.0f;
      float num = 0.0f;
      foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      {
        IFuelTank component = ((Component) clusterModule.Get()).GetComponent<IFuelTank>();
        if (!Util.IsNullOrDestroyed((object) component))
          num += component.Storage.GetAmountAvailable(engine.fuelTag);
      }
      return (float) Mathf.CeilToInt(num);
    }
  }

  public float OxidizerPowerRemaining
  {
    get
    {
      float num = 0.0f;
      foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      {
        OxidizerTank component = ((Component) clusterModule.Get()).GetComponent<OxidizerTank>();
        if (Object.op_Inequality((Object) component, (Object) null))
          num += component.TotalOxidizerPower;
      }
      return (float) Mathf.CeilToInt(num);
    }
  }

  public int MaxHeight
  {
    get
    {
      RocketEngineCluster engine = this.GetEngine();
      return Object.op_Inequality((Object) engine, (Object) null) ? engine.maxHeight : -1;
    }
  }

  public float TotalBurden => this.m_clustercraft.TotalBurden;

  public float EnginePower => this.m_clustercraft.EnginePower;

  public int RocketHeight
  {
    get
    {
      int rocketHeight = 0;
      foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.ClusterModules)
        rocketHeight += ((Component) clusterModule.Get()).GetComponent<Building>().Def.HeightInCells;
      return rocketHeight;
    }
  }

  public bool HasCargoModule
  {
    get
    {
      foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.ClusterModules)
      {
        if (((Component) clusterModule.Get()).GetComponent<CargoBayCluster>() != null)
          return true;
      }
      return false;
    }
  }

  protected virtual void OnPrefabInit() => Game.Instance.OnLoad += new Action<Game.GameSaveData>(this.OnLoad);

  protected virtual void OnSpawn()
  {
    Game.Instance.OnLoad -= new Action<Game.GameSaveData>(this.OnLoad);
    if (this.m_clustercraft.Status != Clustercraft.CraftStatus.Grounded)
      this.ForceAttachmentNetwork();
    this.SetBottomModule();
    this.Subscribe(-1311384361, new Action<object>(this.CompleteSelfDestruct));
  }

  private void OnLoad(Game.GameSaveData data)
  {
    foreach (Ref<RocketModule> module in this.modules)
      this.clusterModules.Add(new Ref<RocketModuleCluster>(((Component) module.Get()).GetComponent<RocketModuleCluster>()));
    this.modules.Clear();
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
    {
      if (!Object.op_Equality((Object) clusterModule.Get(), (Object) null))
        clusterModule.Get().CraftInterface = this;
    }
    bool flag = false;
    for (int index = this.clusterModules.Count - 1; index >= 0; --index)
    {
      if (this.clusterModules[index] == null || Object.op_Equality((Object) this.clusterModules[index].Get(), (Object) null))
      {
        Debug.LogWarning((object) string.Format("Rocket {0} had a null module at index {1} on load! Why????", (object) ((Object) this).name, (object) index), (Object) this);
        this.clusterModules.RemoveAt(index);
        flag = true;
      }
    }
    this.SetBottomModule();
    if (!flag || this.m_clustercraft.Status != Clustercraft.CraftStatus.Grounded)
      return;
    Debug.LogWarning((object) ("The module stack was broken. Collapsing " + ((Object) this).name + "..."), (Object) this);
    this.SortModuleListByPosition();
    LaunchPad currentPad = this.CurrentPad;
    if (Object.op_Inequality((Object) currentPad, (Object) null))
    {
      int cell = currentPad.RocketBottomPosition;
      for (int index = 0; index < this.clusterModules.Count; ++index)
      {
        RocketModuleCluster rocketModuleCluster = this.clusterModules[index].Get();
        if (cell != Grid.PosToCell(TransformExtensions.GetPosition(rocketModuleCluster.transform)))
        {
          Debug.LogWarning((object) string.Format("Collapsing space under module {0}:{1}", (object) index, (object) ((Object) rocketModuleCluster).name));
          TransformExtensions.SetPosition(rocketModuleCluster.transform, Grid.CellToPos(cell, (CellAlignment) 1, Grid.SceneLayer.Building));
        }
        cell = Grid.OffsetCell(cell, 0, ((Component) this.clusterModules[index].Get()).GetComponent<Building>().Def.HeightInCells);
      }
    }
    for (int index = 0; index < this.clusterModules.Count - 1; ++index)
    {
      BuildingAttachPoint component1 = ((Component) this.clusterModules[index].Get()).GetComponent<BuildingAttachPoint>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        AttachableBuilding component2 = ((Component) this.clusterModules[index + 1].Get()).GetComponent<AttachableBuilding>();
        if (Object.op_Inequality((Object) component1.points[0].attachedBuilding, (Object) component2))
        {
          Debug.LogWarning((object) ("Reattaching " + ((Object) component1).name + " & " + ((Object) component2).name));
          component1.points[0].attachedBuilding = component2;
        }
      }
    }
  }

  public void AddModule(RocketModuleCluster newModule)
  {
    for (int index = 0; index < this.clusterModules.Count; ++index)
    {
      if (Object.op_Equality((Object) this.clusterModules[index].Get(), (Object) newModule))
        Debug.LogError((object) ("Adding module " + ((object) newModule)?.ToString() + " to the same rocket (" + this.m_clustercraft.Name + ") twice"));
    }
    this.clusterModules.Add(new Ref<RocketModuleCluster>(newModule));
    newModule.CraftInterface = this;
    this.Trigger(1512695988, (object) newModule);
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
    {
      RocketModuleCluster rocketModuleCluster = clusterModule.Get();
      if (Object.op_Inequality((Object) rocketModuleCluster, (Object) null) && Object.op_Inequality((Object) rocketModuleCluster, (Object) newModule))
        rocketModuleCluster.Trigger(1512695988, (object) newModule);
    }
    newModule.Trigger(1512695988, (object) newModule);
    this.SetBottomModule();
  }

  public void RemoveModule(RocketModuleCluster module)
  {
    for (int index = this.clusterModules.Count - 1; index >= 0; --index)
    {
      if (Object.op_Equality((Object) this.clusterModules[index].Get(), (Object) module))
      {
        this.clusterModules.RemoveAt(index);
        break;
      }
    }
    this.Trigger(1512695988, (object) null);
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      clusterModule.Get().Trigger(1512695988, (object) null);
    this.SetBottomModule();
    if (this.clusterModules.Count != 0)
      return;
    TracesExtesions.DeleteObject(((Component) this).gameObject);
  }

  private void SortModuleListByPosition() => this.clusterModules.Sort((Comparison<Ref<RocketModuleCluster>>) ((a, b) => (double) Grid.CellToPos(Grid.PosToCell((KMonoBehaviour) a.Get())).y >= (double) Grid.CellToPos(Grid.PosToCell((KMonoBehaviour) b.Get())).y ? 1 : -1));

  private void SetBottomModule()
  {
    if (this.clusterModules.Count > 0)
    {
      this.bottomModule = this.clusterModules[0];
      Vector3 vector3 = this.bottomModule.Get().transform.position;
      foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      {
        Vector3 position = clusterModule.Get().transform.position;
        if ((double) position.y < (double) vector3.y)
        {
          this.bottomModule = clusterModule;
          vector3 = position;
        }
      }
    }
    else
      this.bottomModule = (Ref<RocketModuleCluster>) null;
  }

  public int GetHeightOfModuleTop(GameObject module)
  {
    int heightOfModuleTop = 0;
    for (int index = 0; index < ((ICollection<Ref<RocketModuleCluster>>) this.ClusterModules).Count; ++index)
    {
      heightOfModuleTop += ((Component) this.clusterModules[index].Get()).GetComponent<Building>().Def.HeightInCells;
      if (Object.op_Equality((Object) ((Component) this.clusterModules[index].Get()).gameObject, (Object) module))
        return heightOfModuleTop;
    }
    Debug.LogError((object) ("Could not find module " + module.GetProperName() + " in CraftModuleInterface craft " + this.m_clustercraft.Name));
    return 0;
  }

  public int GetModuleRelativeVerticalPosition(GameObject module)
  {
    int verticalPosition = 0;
    for (int index = 0; index < ((ICollection<Ref<RocketModuleCluster>>) this.ClusterModules).Count; ++index)
    {
      if (Object.op_Equality((Object) ((Component) this.clusterModules[index].Get()).gameObject, (Object) module))
        return verticalPosition;
      verticalPosition += ((Component) this.clusterModules[index].Get()).GetComponent<Building>().Def.HeightInCells;
    }
    Debug.LogError((object) ("Could not find module " + module.GetProperName() + " in CraftModuleInterface craft " + this.m_clustercraft.Name));
    return 0;
  }

  public void Sim4000ms(float dt)
  {
    int num = 0;
    foreach (ProcessCondition.ProcessConditionType conditionType in this.conditionsToCheck)
    {
      if (this.EvaluateConditionSet(conditionType) != ProcessCondition.Status.Failure)
        ++num;
    }
    if (num == this.lastConditionTypeSucceeded)
      return;
    this.lastConditionTypeSucceeded = num;
    this.TriggerEventOnCraftAndRocket(GameHashes.LaunchConditionChanged, (object) null);
  }

  public bool IsLaunchRequested() => this.m_clustercraft.LaunchRequested;

  public bool CheckPreppedForLaunch() => this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) != ProcessCondition.Status.Failure && this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) != ProcessCondition.Status.Failure && this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketFlight) != 0;

  public bool CheckReadyToLaunch() => this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) != ProcessCondition.Status.Failure && this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) != ProcessCondition.Status.Failure && this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketFlight) != ProcessCondition.Status.Failure && this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketBoard) != 0;

  public bool HasLaunchWarnings() => this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) == ProcessCondition.Status.Warning || this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) == ProcessCondition.Status.Warning || this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketBoard) == ProcessCondition.Status.Warning;

  public bool CheckReadyForAutomatedLaunchCommand() => this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) == ProcessCondition.Status.Ready && this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) == ProcessCondition.Status.Ready;

  public bool CheckReadyForAutomatedLaunch() => this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketPrep) == ProcessCondition.Status.Ready && this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketStorage) == ProcessCondition.Status.Ready && this.EvaluateConditionSet(ProcessCondition.ProcessConditionType.RocketBoard) == ProcessCondition.Status.Ready;

  public void TriggerEventOnCraftAndRocket(GameHashes evt, object data)
  {
    this.Trigger((int) evt, data);
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      clusterModule.Get().Trigger((int) evt, data);
  }

  public void CancelLaunch() => this.m_clustercraft.CancelLaunch();

  public void TriggerLaunch(bool automated = false) => this.m_clustercraft.RequestLaunch(automated);

  public void DoLaunch()
  {
    this.SortModuleListByPosition();
    this.CurrentPad.Trigger(705820818, (object) this);
    this.SetPreferredLaunchPadForWorld(this.CurrentPad);
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      clusterModule.Get().Trigger(705820818, (object) this);
  }

  public void DoLand(LaunchPad pad)
  {
    int num = pad.RocketBottomPosition;
    for (int index = 0; index < this.clusterModules.Count; ++index)
    {
      this.clusterModules[index].Get().MoveToPad(num);
      num = Grid.OffsetCell(num, 0, ((Component) this.clusterModules[index].Get()).GetComponent<Building>().Def.HeightInCells);
    }
    this.SetBottomModule();
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      clusterModule.Get().Trigger(-1165815793, (object) pad);
    pad.Trigger(-1165815793, (object) this);
  }

  public LaunchConditionManager FindLaunchConditionManager()
  {
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
    {
      LaunchConditionManager component = ((Component) clusterModule.Get()).GetComponent<LaunchConditionManager>();
      if (Object.op_Inequality((Object) component, (Object) null))
        return component;
    }
    return (LaunchConditionManager) null;
  }

  public LaunchableRocketCluster FindLaunchableRocket()
  {
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
    {
      RocketModuleCluster rocketModuleCluster = clusterModule.Get();
      LaunchableRocketCluster component = ((Component) rocketModuleCluster).GetComponent<LaunchableRocketCluster>();
      if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Inequality((Object) rocketModuleCluster.CraftInterface, (Object) null) && ((Component) rocketModuleCluster.CraftInterface).GetComponent<Clustercraft>().Status == Clustercraft.CraftStatus.Grounded)
        return component;
    }
    return (LaunchableRocketCluster) null;
  }

  public List<GameObject> GetParts()
  {
    List<GameObject> parts = new List<GameObject>();
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      parts.Add(((Component) clusterModule.Get()).gameObject);
    return parts;
  }

  public RocketEngineCluster GetEngine()
  {
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
    {
      RocketEngineCluster component = ((Component) clusterModule.Get()).GetComponent<RocketEngineCluster>();
      if (Object.op_Inequality((Object) component, (Object) null))
        return component;
    }
    return (RocketEngineCluster) null;
  }

  public PassengerRocketModule GetPassengerModule()
  {
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
    {
      PassengerRocketModule component = ((Component) clusterModule.Get()).GetComponent<PassengerRocketModule>();
      if (Object.op_Inequality((Object) component, (Object) null))
        return component;
    }
    return (PassengerRocketModule) null;
  }

  public WorldContainer GetInteriorWorld()
  {
    PassengerRocketModule passengerModule = this.GetPassengerModule();
    if (Object.op_Equality((Object) passengerModule, (Object) null))
      return (WorldContainer) null;
    ClustercraftInteriorDoor interiorDoor = ((Component) passengerModule).GetComponent<ClustercraftExteriorDoor>().GetInteriorDoor();
    return Object.op_Equality((Object) interiorDoor, (Object) null) ? (WorldContainer) null : interiorDoor.GetMyWorld();
  }

  public RocketClusterDestinationSelector GetClusterDestinationSelector() => ((Component) this).GetComponent<RocketClusterDestinationSelector>();

  public bool HasClusterDestinationSelector() => Object.op_Inequality((Object) ((Component) this).GetComponent<RocketClusterDestinationSelector>(), (Object) null);

  public List<ProcessCondition> GetConditionSet(
    ProcessCondition.ProcessConditionType conditionType)
  {
    this.returnConditions.Clear();
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
    {
      List<ProcessCondition> conditionSet = clusterModule.Get().GetConditionSet(conditionType);
      if (conditionSet != null)
        this.returnConditions.AddRange((IEnumerable<ProcessCondition>) conditionSet);
    }
    if (Object.op_Inequality((Object) this.CurrentPad, (Object) null))
    {
      List<ProcessCondition> conditionSet = ((Component) this.CurrentPad).GetComponent<LaunchPadConditions>().GetConditionSet(conditionType);
      if (conditionSet != null)
        this.returnConditions.AddRange((IEnumerable<ProcessCondition>) conditionSet);
    }
    return this.returnConditions;
  }

  private ProcessCondition.Status EvaluateConditionSet(
    ProcessCondition.ProcessConditionType conditionType)
  {
    ProcessCondition.Status conditionSet = ProcessCondition.Status.Ready;
    foreach (ProcessCondition condition1 in this.GetConditionSet(conditionType))
    {
      ProcessCondition.Status condition2 = condition1.EvaluateCondition();
      if (condition2 < conditionSet)
        conditionSet = condition2;
      if (conditionSet == ProcessCondition.Status.Failure)
        break;
    }
    return conditionSet;
  }

  private void ForceAttachmentNetwork()
  {
    RocketModuleCluster rocketModuleCluster1 = (RocketModuleCluster) null;
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
    {
      RocketModuleCluster rocketModuleCluster2 = clusterModule.Get();
      if (Object.op_Inequality((Object) rocketModuleCluster1, (Object) null))
        ((Component) rocketModuleCluster1).GetComponent<BuildingAttachPoint>().points[0].attachedBuilding = ((Component) rocketModuleCluster2).GetComponent<AttachableBuilding>();
      rocketModuleCluster1 = rocketModuleCluster2;
    }
  }

  public static Storage SpawnRocketDebris(string nameSuffix, SimHashes element)
  {
    Vector3 vector3;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3).\u002Ector(-1f, -1f, 0.0f);
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("DebrisPayload")), vector3);
    gameObject.GetComponent<PrimaryElement>().SetElement(element);
    ((Object) gameObject).name = ((Object) gameObject).name + nameSuffix;
    gameObject.SetActive(true);
    return gameObject.GetComponent<Storage>();
  }

  public void CompleteSelfDestruct(object data = null)
  {
    Debug.Assert(((Component) this).HasTag(GameTags.RocketInSpace), (object) "Self Destruct is only valid for in-space rockets!");
    SimHashes elementId = ((Component) this.GetPassengerModule()).GetComponent<PrimaryElement>().ElementID;
    List<RocketModule> rocketModuleList = new List<RocketModule>();
    foreach (Ref<RocketModuleCluster> clusterModule in this.clusterModules)
      rocketModuleList.Add((RocketModule) clusterModule.Get());
    List<GameObject> collect_dropped_items = new List<GameObject>();
    foreach (RocketModule rocketModule in rocketModuleList)
    {
      foreach (Storage component in ((Component) rocketModule).GetComponents<Storage>())
        component.DropAll(false, false, new Vector3(), true, collect_dropped_items);
      Deconstructable component1 = ((Component) rocketModule).GetComponent<Deconstructable>();
      collect_dropped_items.AddRange((IEnumerable<GameObject>) component1.ForceDestroyAndGetMaterials());
    }
    List<Storage> storageList = new List<Storage>();
    foreach (GameObject gameObject in collect_dropped_items)
    {
      Pickupable component = gameObject.GetComponent<Pickupable>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        component.PrimaryElement.Units = (float) Mathf.Max(1, Mathf.RoundToInt(component.PrimaryElement.Units * 0.5f));
        if ((storageList.Count == 0 || (double) storageList[storageList.Count - 1].RemainingCapacity() == 0.0) && (double) component.PrimaryElement.Mass > 0.0)
          storageList.Add(CraftModuleInterface.SpawnRocketDebris(" from CMI", elementId));
        Storage storage = storageList[storageList.Count - 1];
        while ((double) component.PrimaryElement.Mass > (double) storage.RemainingCapacity())
        {
          Pickupable pickupable = component.Take(storage.RemainingCapacity());
          storage.Store(((Component) pickupable).gameObject);
          storage = CraftModuleInterface.SpawnRocketDebris(" from CMI", elementId);
          storageList.Add(storage);
        }
        if ((double) component.PrimaryElement.Mass > 0.0)
          storage.Store(((Component) component).gameObject);
      }
    }
    foreach (Component cmp in storageList)
    {
      RailGunPayload.StatesInstance smi = cmp.GetSMI<RailGunPayload.StatesInstance>();
      smi.StartSM();
      smi.Travel(this.m_clustercraft.Location, ClusterUtil.ClosestVisibleAsteroidToLocation(this.m_clustercraft.Location).Location);
    }
    this.m_clustercraft.SetExploding();
  }
}
