// Decompiled with JetBrains decompiler
// Type: LaunchPad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class LaunchPad : KMonoBehaviour, ISim1000ms, IListableOption, IProcessConditionSet
{
  public HashedString triggerPort;
  public HashedString statusPort;
  public HashedString landedRocketPort;
  private CellOffset baseModulePosition = new CellOffset(0, 2);
  private SchedulerHandle RebuildLaunchTowerHeightHandler;
  private AttachableBuilding lastBaseAttachable;
  private LaunchPad.LaunchPadTower tower;
  [Serialize]
  public int maxTowerHeight;
  private bool dirtyTowerHeight;
  private HandleVector<int>.Handle partitionerEntry;
  private Guid landedRocketPassengerModuleStatusItem = Guid.Empty;

  public RocketModuleCluster LandedRocket
  {
    get
    {
      GameObject gameObject = (GameObject) null;
      Grid.ObjectLayers[1].TryGetValue(this.RocketBottomPosition, out gameObject);
      if (Object.op_Inequality((Object) gameObject, (Object) null))
      {
        RocketModuleCluster component = gameObject.GetComponent<RocketModuleCluster>();
        Clustercraft clustercraft = !Object.op_Inequality((Object) component, (Object) null) || !Object.op_Inequality((Object) component.CraftInterface, (Object) null) ? (Clustercraft) null : ((Component) component.CraftInterface).GetComponent<Clustercraft>();
        if (Object.op_Inequality((Object) clustercraft, (Object) null) && (clustercraft.Status == Clustercraft.CraftStatus.Grounded || clustercraft.Status == Clustercraft.CraftStatus.Landing))
          return component;
      }
      return (RocketModuleCluster) null;
    }
  }

  public int RocketBottomPosition => Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.baseModulePosition);

  [OnDeserialized]
  private void OnDeserialzed()
  {
    if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 24))
      return;
    Building component = ((Component) this).GetComponent<Building>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.RunOnArea((Action<int>) (cell =>
    {
      if (!Grid.IsValidCell(cell) || !Grid.Solid[cell])
        return;
      SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.LaunchpadDesolidify, 0.0f);
    }));
  }

  protected virtual void OnPrefabInit()
  {
    UserNameable component = ((Component) this).GetComponent<UserNameable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetName(GameUtil.GenerateRandomLaunchPadName());
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.tower = new LaunchPad.LaunchPadTower(this, this.maxTowerHeight);
    this.OnRocketBuildingChanged((object) this.GetRocketBaseModule());
    this.partitionerEntry = GameScenePartitioner.Instance.Add("LaunchPad.OnSpawn", (object) ((Component) this).gameObject, Extents.OneCell(this.RocketBottomPosition), GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnRocketBuildingChanged));
    Components.LaunchPads.Add(this);
    this.CheckLandedRocketPassengerModuleStatus();
    int ceilingEdge = ConditionFlightPathIsClear.PadTopEdgeDistanceToCeilingEdge(((Component) this).gameObject);
    if (ceilingEdge >= 35)
      return;
    ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.RocketPlatformCloseToCeiling, (object) ceilingEdge);
  }

  protected virtual void OnCleanUp()
  {
    Components.LaunchPads.Remove(this);
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    if (Object.op_Inequality((Object) this.lastBaseAttachable, (Object) null))
    {
      this.lastBaseAttachable.onAttachmentNetworkChanged -= new Action<object>(this.OnRocketLayoutChanged);
      this.lastBaseAttachable = (AttachableBuilding) null;
    }
    this.RebuildLaunchTowerHeightHandler.ClearScheduler();
    base.OnCleanUp();
  }

  private void CheckLandedRocketPassengerModuleStatus()
  {
    if (Object.op_Equality((Object) this.LandedRocket, (Object) null))
    {
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(this.landedRocketPassengerModuleStatusItem);
      this.landedRocketPassengerModuleStatusItem = Guid.Empty;
    }
    else if (Object.op_Equality((Object) this.LandedRocket.CraftInterface.GetPassengerModule(), (Object) null))
    {
      if (!(this.landedRocketPassengerModuleStatusItem == Guid.Empty))
        return;
      this.landedRocketPassengerModuleStatusItem = ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.LandedRocketLacksPassengerModule);
    }
    else
    {
      if (!(this.landedRocketPassengerModuleStatusItem != Guid.Empty))
        return;
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(this.landedRocketPassengerModuleStatusItem);
      this.landedRocketPassengerModuleStatusItem = Guid.Empty;
    }
  }

  public bool IsLogicInputConnected() => Game.Instance.logicCircuitManager.GetNetworkForCell(((Component) this).GetComponent<LogicPorts>().GetPortCell(this.triggerPort)) != null;

  public void Sim1000ms(float dt)
  {
    LogicPorts component = ((Component) this).gameObject.GetComponent<LogicPorts>();
    RocketModuleCluster landedRocket = this.LandedRocket;
    if (Object.op_Inequality((Object) landedRocket, (Object) null) && this.IsLogicInputConnected())
    {
      if (component.GetInputValue(this.triggerPort) == 1)
      {
        if (landedRocket.CraftInterface.CheckReadyForAutomatedLaunchCommand())
          landedRocket.CraftInterface.TriggerLaunch(true);
        else
          landedRocket.CraftInterface.CancelLaunch();
      }
      else
        landedRocket.CraftInterface.CancelLaunch();
    }
    this.CheckLandedRocketPassengerModuleStatus();
    component.SendSignal(this.landedRocketPort, Object.op_Inequality((Object) landedRocket, (Object) null) ? 1 : 0);
    if (Object.op_Inequality((Object) landedRocket, (Object) null))
      component.SendSignal(this.statusPort, landedRocket.CraftInterface.CheckReadyForAutomatedLaunch() || ((Component) landedRocket.CraftInterface).HasTag(GameTags.RocketNotOnGround) ? 1 : 0);
    else
      component.SendSignal(this.statusPort, 0);
  }

  public GameObject AddBaseModule(BuildingDef moduleDefID, IList<Tag> elements)
  {
    int cell = Grid.OffsetCell(Grid.PosToCell(((Component) this).gameObject), this.baseModulePosition);
    GameObject data = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive ? moduleDefID.Build(cell, Orientation.Neutral, (Storage) null, elements, 293.15f, timeBuilt: GameClock.Instance.GetTime()) : moduleDefID.TryPlace((GameObject) null, Grid.CellToPosCBC(cell, moduleDefID.SceneLayer), Orientation.Neutral, elements);
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("Clustercraft")), (GameObject) null, (string) null);
    gameObject.SetActive(true);
    Clustercraft component = gameObject.GetComponent<Clustercraft>();
    ((Component) component).GetComponent<CraftModuleInterface>().AddModule(data.GetComponent<RocketModuleCluster>());
    component.Init(this.GetMyWorldLocation(), this);
    if (Object.op_Inequality((Object) data.GetComponent<BuildingUnderConstruction>(), (Object) null))
      this.OnRocketBuildingChanged((object) data);
    this.Trigger(374403796, (object) null);
    return data;
  }

  private void OnRocketBuildingChanged(object data)
  {
    GameObject gameObject = (GameObject) data;
    RocketModuleCluster landedRocket = this.LandedRocket;
    Debug.Assert(Object.op_Equality((Object) gameObject, (Object) null) || Object.op_Equality((Object) landedRocket, (Object) null) || Object.op_Equality((Object) ((Component) landedRocket).gameObject, (Object) gameObject), (object) "Launch Pad had a rocket land or take off on it twice??");
    Clustercraft clustercraft = !Object.op_Inequality((Object) landedRocket, (Object) null) || !Object.op_Inequality((Object) landedRocket.CraftInterface, (Object) null) ? (Clustercraft) null : ((Component) landedRocket.CraftInterface).GetComponent<Clustercraft>();
    if (Object.op_Inequality((Object) clustercraft, (Object) null))
    {
      if (clustercraft.Status == Clustercraft.CraftStatus.Landing)
        this.Trigger(-887025858, (object) landedRocket);
      else if (clustercraft.Status == Clustercraft.CraftStatus.Launching)
      {
        this.Trigger(-1277991738, (object) landedRocket);
        ((Component) landedRocket.CraftInterface.ClusterModules[0].Get()).GetComponent<AttachableBuilding>().onAttachmentNetworkChanged -= new Action<object>(this.OnRocketLayoutChanged);
      }
    }
    this.OnRocketLayoutChanged((object) null);
  }

  private void OnRocketLayoutChanged(object data)
  {
    if (Object.op_Inequality((Object) this.lastBaseAttachable, (Object) null))
    {
      this.lastBaseAttachable.onAttachmentNetworkChanged -= new Action<object>(this.OnRocketLayoutChanged);
      this.lastBaseAttachable = (AttachableBuilding) null;
    }
    GameObject rocketBaseModule = this.GetRocketBaseModule();
    if (Object.op_Inequality((Object) rocketBaseModule, (Object) null))
    {
      this.lastBaseAttachable = rocketBaseModule.GetComponent<AttachableBuilding>();
      this.lastBaseAttachable.onAttachmentNetworkChanged += new Action<object>(this.OnRocketLayoutChanged);
    }
    this.DirtyTowerHeight();
  }

  public bool HasRocket() => Object.op_Inequality((Object) this.LandedRocket, (Object) null);

  public bool HasRocketWithCommandModule() => this.HasRocket() && Object.op_Inequality((Object) this.LandedRocket.CraftInterface.FindLaunchableRocket(), (Object) null);

  private GameObject GetRocketBaseModule()
  {
    GameObject gameObject = Grid.Objects[Grid.OffsetCell(Grid.PosToCell(((Component) this).gameObject), this.baseModulePosition), 1];
    return !Object.op_Inequality((Object) gameObject, (Object) null) || !Object.op_Inequality((Object) gameObject.GetComponent<RocketModule>(), (Object) null) ? (GameObject) null : gameObject;
  }

  public void DirtyTowerHeight()
  {
    if (this.dirtyTowerHeight)
      return;
    this.dirtyTowerHeight = true;
    if (this.RebuildLaunchTowerHeightHandler.IsValid)
      return;
    this.RebuildLaunchTowerHeightHandler = GameScheduler.Instance.ScheduleNextFrame("RebuildLaunchTowerHeight", new Action<object>(this.RebuildLaunchTowerHeight));
  }

  private void RebuildLaunchTowerHeight(object obj)
  {
    RocketModuleCluster landedRocket = this.LandedRocket;
    if (Object.op_Inequality((Object) landedRocket, (Object) null))
      this.tower.SetTowerHeight(landedRocket.CraftInterface.MaxHeight);
    this.dirtyTowerHeight = false;
    this.RebuildLaunchTowerHeightHandler.ClearScheduler();
  }

  public string GetProperName() => ((Component) this).gameObject.GetProperName();

  public List<ProcessCondition> GetConditionSet(
    ProcessCondition.ProcessConditionType conditionType)
  {
    RocketProcessConditionDisplayTarget conditionDisplayTarget = (RocketProcessConditionDisplayTarget) null;
    RocketModuleCluster landedRocket = this.LandedRocket;
    if (Object.op_Inequality((Object) landedRocket, (Object) null))
    {
      for (int index = 0; index < ((ICollection<Ref<RocketModuleCluster>>) landedRocket.CraftInterface.ClusterModules).Count; ++index)
      {
        RocketProcessConditionDisplayTarget component = ((Component) landedRocket.CraftInterface.ClusterModules[index].Get()).GetComponent<RocketProcessConditionDisplayTarget>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          conditionDisplayTarget = component;
          break;
        }
      }
    }
    return Object.op_Inequality((Object) conditionDisplayTarget, (Object) null) ? conditionDisplayTarget.GetConditionSet(conditionType) : new List<ProcessCondition>();
  }

  public static List<LaunchPad> GetLaunchPadsForDestination(AxialI destination)
  {
    List<LaunchPad> padsForDestination = new List<LaunchPad>();
    foreach (LaunchPad launchPad in Components.LaunchPads)
    {
      if (AxialI.op_Equality(launchPad.GetMyWorldLocation(), destination))
        padsForDestination.Add(launchPad);
    }
    return padsForDestination;
  }

  public class LaunchPadTower
  {
    private LaunchPad pad;
    private KAnimLink animLink;
    private Coroutine activeAnimationRoutine;
    private string[] towerBGAnimNames = new string[10]
    {
      "A1",
      "A2",
      "A3",
      "B",
      "C",
      "D",
      "E1",
      "E2",
      "F1",
      "F2"
    };
    private string towerBGAnimSuffix_on = "_on";
    private string towerBGAnimSuffix_on_pre = "_on_pre";
    private string towerBGAnimSuffix_off_pre = "_off_pre";
    private string towerBGAnimSuffix_off = "_off";
    private List<KBatchedAnimController> towerAnimControllers = new List<KBatchedAnimController>();
    private int targetHeight;
    private int currentHeight;

    public LaunchPadTower(LaunchPad pad, int startHeight)
    {
      this.pad = pad;
      this.SetTowerHeight(startHeight);
    }

    public void AddTowerRow()
    {
      GameObject gameObject = new GameObject("LaunchPadTowerRow");
      gameObject.SetActive(false);
      gameObject.transform.SetParent(this.pad.transform);
      TransformExtensions.SetLocalPosition(gameObject.transform, Vector3.op_Multiply(Vector3.op_Multiply(Grid.CellSizeInMeters, Vector3.up), (float) (this.towerAnimControllers.Count + this.pad.baseModulePosition.y)));
      TransformExtensions.SetPosition(gameObject.transform, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, Grid.GetLayerZ(Grid.SceneLayer.Backwall)));
      KBatchedAnimController slave = gameObject.AddComponent<KBatchedAnimController>();
      slave.AnimFiles = new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit("rocket_launchpad_tower_kanim"))
      };
      gameObject.SetActive(true);
      this.towerAnimControllers.Add(slave);
      slave.initialAnim = this.towerBGAnimNames[this.towerAnimControllers.Count % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_off;
      this.animLink = new KAnimLink(((Component) this.pad).GetComponent<KAnimControllerBase>(), (KAnimControllerBase) slave);
    }

    public void RemoveTowerRow()
    {
    }

    public void SetTowerHeight(int height)
    {
      if (height < 8)
        height = 0;
      this.targetHeight = height;
      this.pad.maxTowerHeight = height;
      while (this.targetHeight > this.towerAnimControllers.Count)
        this.AddTowerRow();
      if (this.activeAnimationRoutine != null)
        ((MonoBehaviour) this.pad).StopCoroutine(this.activeAnimationRoutine);
      this.activeAnimationRoutine = ((MonoBehaviour) this.pad).StartCoroutine(this.TowerRoutine());
    }

    private IEnumerator TowerRoutine()
    {
      float delay;
      while (this.currentHeight < this.targetHeight)
      {
        bool animComplete = false;
        this.towerAnimControllers[this.currentHeight].Queue(HashedString.op_Implicit(this.towerBGAnimNames[this.currentHeight % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_on_pre));
        this.towerAnimControllers[this.currentHeight].Queue(HashedString.op_Implicit(this.towerBGAnimNames[this.currentHeight % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_on));
        this.towerAnimControllers[this.currentHeight].onAnimComplete += (KAnimControllerBase.KAnimEvent) (arg => animComplete = true);
        delay = 0.25f;
        while (!animComplete && (double) delay > 0.0)
        {
          delay -= Time.deltaTime;
          yield return (object) 0;
        }
        ++this.currentHeight;
      }
      while (this.currentHeight > this.targetHeight)
      {
        --this.currentHeight;
        bool animComplete = false;
        this.towerAnimControllers[this.currentHeight].Queue(HashedString.op_Implicit(this.towerBGAnimNames[this.currentHeight % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_off_pre));
        this.towerAnimControllers[this.currentHeight].Queue(HashedString.op_Implicit(this.towerBGAnimNames[this.currentHeight % this.towerBGAnimNames.Length] + this.towerBGAnimSuffix_off));
        this.towerAnimControllers[this.currentHeight].onAnimComplete += (KAnimControllerBase.KAnimEvent) (arg => animComplete = true);
        delay = 0.25f;
        while (!animComplete && (double) delay > 0.0)
        {
          delay -= Time.deltaTime;
          yield return (object) 0;
        }
      }
      yield return (object) 0;
    }
  }
}
