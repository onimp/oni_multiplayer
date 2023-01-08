// Decompiled with JetBrains decompiler
// Type: Constructable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/Constructable")]
public class Constructable : Workable, ISaveLoadable
{
  [MyCmpAdd]
  private Storage storage;
  [MyCmpAdd]
  private Notifier notifier;
  [MyCmpAdd]
  private Prioritizable prioritizable;
  [MyCmpReq]
  private Building building;
  [MyCmpGet]
  private Rotatable rotatable;
  private Notification invalidLocation;
  private float initialTemperature = -1f;
  [Serialize]
  private bool isPrioritized;
  private FetchList2 fetchList;
  private Chore buildChore;
  private bool materialNeedsCleared;
  private bool hasUnreachableDigs;
  private bool finished;
  private bool unmarked;
  public bool isDiggingRequired = true;
  private bool waitForFetchesBeforeDigging;
  private bool hasLadderNearby;
  private Extents ladderDetectionExtents;
  [Serialize]
  public bool IsReplacementTile;
  private HandleVector<int>.Handle solidPartitionerEntry;
  private HandleVector<int>.Handle digPartitionerEntry;
  private HandleVector<int>.Handle ladderParititonerEntry;
  private LoggerFSS log = new LoggerFSS(nameof (Constructable), 35);
  [Serialize]
  private Tag[] selectedElementsTags;
  private Element[] selectedElements;
  [Serialize]
  private int[] ids;
  private static readonly EventSystem.IntraObjectHandler<Constructable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Constructable>((Action<Constructable, object>) ((component, data) => component.OnReachableChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<Constructable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Constructable>((Action<Constructable, object>) ((component, data) => component.OnCancel(data)));
  private static readonly EventSystem.IntraObjectHandler<Constructable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Constructable>((Action<Constructable, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  public Recipe Recipe => this.building.Def.CraftRecipe;

  public IList<Tag> SelectedElementsTags
  {
    get => (IList<Tag>) this.selectedElementsTags;
    set
    {
      if (this.selectedElementsTags == null || this.selectedElementsTags.Length != ((ICollection<Tag>) value).Count)
        this.selectedElementsTags = new Tag[((ICollection<Tag>) value).Count];
      ((ICollection<Tag>) value).CopyTo(this.selectedElementsTags, 0);
    }
  }

  public override string GetConversationTopic() => this.building.Def.PrefabID;

  protected override void OnCompleteWork(Worker worker)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    bool flag1 = true;
    foreach (GameObject gameObject in this.storage.items)
    {
      if (!Object.op_Equality((Object) gameObject, (Object) null))
      {
        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
        if (!Object.op_Equality((Object) component, (Object) null))
        {
          num1 += component.Mass;
          num2 += component.Temperature * component.Mass;
          flag1 = flag1 && ((Component) component).HasTag(GameTags.Liquifiable);
        }
      }
    }
    if ((double) num1 <= 0.0)
    {
      DebugUtil.LogWarningArgs((Object) ((Component) this).gameObject, new object[3]
      {
        (object) "uhhh this constructable is about to generate a nan",
        (object) "Item Count: ",
        (object) this.storage.items.Count
      });
    }
    else
    {
      this.initialTemperature = !flag1 ? Mathf.Clamp(num2 / num1, 288.15f, 318.15f) : Mathf.Min(num2 / num1, 318.15f);
      KAnimGraphTileVisualizer component1 = ((Component) this).GetComponent<KAnimGraphTileVisualizer>();
      UtilityConnections connections = Object.op_Equality((Object) component1, (Object) null) ? (UtilityConnections) 0 : component1.Connections;
      bool flag2 = true;
      if (this.IsReplacementTile)
      {
        int cell = Grid.PosToCell(TransformExtensions.GetLocalPosition(this.transform));
        GameObject replacementCandidate = this.building.Def.GetReplacementCandidate(cell);
        if (Object.op_Inequality((Object) replacementCandidate, (Object) null))
        {
          flag2 = false;
          SimCellOccupier component2 = replacementCandidate.GetComponent<SimCellOccupier>();
          if (Object.op_Inequality((Object) component2, (Object) null))
          {
            component2.DestroySelf((System.Action) (() =>
            {
              if (!Object.op_Inequality((Object) this, (Object) null) || !Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null))
                return;
              this.FinishConstruction(connections, worker);
            }));
          }
          else
          {
            Conduit component3 = replacementCandidate.GetComponent<Conduit>();
            if (Object.op_Inequality((Object) component3, (Object) null))
              component3.GetFlowManager().MarkForReplacement(cell);
            BuildingComplete component4 = replacementCandidate.GetComponent<BuildingComplete>();
            if (Object.op_Inequality((Object) component4, (Object) null))
            {
              component4.Subscribe(-21016276, (Action<object>) (data => this.FinishConstruction(connections, worker)));
            }
            else
            {
              Debug.LogWarning((object) ("Why am I trying to replace a: " + ((Object) replacementCandidate).name));
              this.FinishConstruction(connections, worker);
            }
          }
          KAnimGraphTileVisualizer component5 = replacementCandidate.GetComponent<KAnimGraphTileVisualizer>();
          if (Object.op_Inequality((Object) component5, (Object) null))
            component5.skipCleanup = true;
          Deconstructable component6 = replacementCandidate.GetComponent<Deconstructable>();
          if (Object.op_Inequality((Object) component6, (Object) null))
            component6.SpawnItemsFromConstruction();
          EventExtensions.Trigger(replacementCandidate, 1606648047, (object) this.building.Def.TileLayer);
          TracesExtesions.DeleteObject(replacementCandidate);
        }
      }
      if (flag2)
        this.FinishConstruction(connections, worker);
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, ((Component) this).GetComponent<KSelectable>().GetName(), this.transform);
    }
  }

  private void FinishConstruction(UtilityConnections connections, Worker workerForGameplayEvent)
  {
    Rotatable component1 = ((Component) this).GetComponent<Rotatable>();
    Orientation orientation = Object.op_Inequality((Object) component1, (Object) null) ? component1.GetOrientation() : Orientation.Neutral;
    int cell = Grid.PosToCell(TransformExtensions.GetLocalPosition(this.transform));
    this.UnmarkArea();
    GameObject gameObject = this.building.Def.Build(cell, orientation, this.storage, (IList<Tag>) this.selectedElementsTags, this.initialTemperature, ((Component) this).GetComponent<BuildingFacade>().CurrentFacade, timeBuilt: GameClock.Instance.GetTime());
    GameplayEventManager.Instance.Trigger(-1661515756, (object) new BonusEvent.GameplayEventData()
    {
      building = gameObject.GetComponent<BuildingComplete>(),
      workable = (Workable) this,
      worker = workerForGameplayEvent,
      eventTrigger = GameHashes.NewBuilding
    });
    gameObject.transform.rotation = this.transform.rotation;
    Rotatable component2 = gameObject.GetComponent<Rotatable>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      component2.SetOrientation(orientation);
    KAnimGraphTileVisualizer component3 = ((Component) this).GetComponent<KAnimGraphTileVisualizer>();
    if (Object.op_Inequality((Object) component3, (Object) null))
    {
      gameObject.GetComponent<KAnimGraphTileVisualizer>().Connections = connections;
      component3.skipCleanup = true;
    }
    KSelectable component4 = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) component4, (Object) null) && component4.IsSelected && Object.op_Inequality((Object) gameObject.GetComponent<KSelectable>(), (Object) null))
    {
      component4.Unselect();
      if (((Object) PlayerController.Instance.ActiveTool).name == "SelectTool")
        ((SelectTool) PlayerController.Instance.ActiveTool).SelectNextFrame(gameObject.GetComponent<KSelectable>());
    }
    EventExtensions.Trigger(gameObject, 2121280625, (object) this);
    this.storage.ConsumeAllIgnoringDisease();
    this.finished = true;
    TracesExtesions.DeleteObject((Component) this);
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.invalidLocation = new Notification((string) MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.NAME, NotificationType.BadMinor, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) MISC.NOTIFICATIONS.INVALIDCONSTRUCTIONLOCATION.TOOLTIP + notificationList.ReduceMessages(false)));
    this.faceTargetWhenWorking = true;
    this.Subscribe<Constructable>(-1432940121, Constructable.OnReachableChangedDelegate);
    if (Object.op_Equality((Object) this.rotatable, (Object) null))
      this.MarkArea();
    if (Db.Get().TechItems.GetTechTierForItem(this.building.Def.PrefabID) > 2)
      this.requireMinionToWork = true;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Building;
    this.workingStatusItem = (StatusItem) null;
    this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
    this.minimumAttributeMultiplier = 0.75f;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
    this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
    Prioritizable.AddRef(((Component) this).gameObject);
    this.synchronizeAnims = false;
    this.multitoolContext = HashedString.op_Implicit("build");
    this.multitoolHitEffectTag = Tag.op_Implicit(EffectConfigs.BuildSplashId);
    this.workingPstComplete = (HashedString[]) null;
    this.workingPstFailed = (HashedString[]) null;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    CellOffset[][] table = OffsetGroups.InvertedStandardTable;
    if (this.building.Def.IsTilePiece)
      table = OffsetGroups.InvertedStandardTableWithCorners;
    CellOffset[] area_offsets = this.building.Def.PlacementOffsets;
    if (Object.op_Inequality((Object) this.rotatable, (Object) null))
    {
      area_offsets = new CellOffset[this.building.Def.PlacementOffsets.Length];
      for (int index = 0; index < area_offsets.Length; ++index)
        area_offsets[index] = this.rotatable.GetRotatedCellOffset(this.building.Def.PlacementOffsets[index]);
    }
    CellOffset[][] offset_table = OffsetGroups.BuildReachabilityTable(area_offsets, table, this.building.Def.ConstructionOffsetFilter);
    this.SetOffsetTable(offset_table);
    this.storage.SetOffsetTable(offset_table);
    this.Subscribe<Constructable>(2127324410, Constructable.OnCancelDelegate);
    if (Object.op_Inequality((Object) this.rotatable, (Object) null))
      this.MarkArea();
    this.fetchList = new FetchList2(this.storage, Db.Get().ChoreTypes.BuildFetch);
    PrimaryElement component1 = ((Component) this).GetComponent<PrimaryElement>();
    Element element = ElementLoader.GetElement(this.SelectedElementsTags[0]);
    Debug.Assert(element != null, (object) "Missing primary element for Constructable");
    component1.ElementID = element.id;
    double num1;
    float num2 = (float) (num1 = 293.14999389648438);
    component1.Temperature = (float) num1;
    component1.Temperature = num2;
    foreach (Recipe.Ingredient allIngredient in this.Recipe.GetAllIngredients((IList<Tag>) this.selectedElementsTags))
    {
      this.fetchList.Add(allIngredient.tag, amount: allIngredient.amount);
      MaterialNeeds.UpdateNeed(allIngredient.tag, allIngredient.amount, ((Component) this).gameObject.GetMyWorldId());
    }
    if (!this.building.Def.IsTilePiece)
      ((Component) this).gameObject.layer = LayerMask.NameToLayer("Construction");
    this.building.RunOnArea((Action<int>) (offset_cell =>
    {
      if (!Object.op_Equality((Object) ((Component) this).gameObject.GetComponent<ConduitBridge>(), (Object) null))
        return;
      GameObject gameObject = Grid.Objects[offset_cell, 7];
      if (!Object.op_Inequality((Object) gameObject, (Object) null))
        return;
      TracesExtesions.DeleteObject(gameObject);
    }));
    if (this.IsReplacementTile)
    {
      if (this.building.Def.ReplacementLayer != ObjectLayer.NumLayers)
      {
        int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
        GameObject gameObject1 = Grid.Objects[cell, (int) this.building.Def.ReplacementLayer];
        if (Object.op_Equality((Object) gameObject1, (Object) null) || Object.op_Equality((Object) gameObject1, (Object) ((Component) this).gameObject))
        {
          Grid.Objects[cell, (int) this.building.Def.ReplacementLayer] = ((Component) this).gameObject;
          if (Object.op_Inequality((Object) ((Component) this).gameObject.GetComponent<SimCellOccupier>(), (Object) null))
            World.Instance.blockTileRenderer.AddBlock(LayerMask.NameToLayer("Overlay"), this.building.Def, this.IsReplacementTile, SimHashes.Void, cell);
          TileVisualizer.RefreshCell(cell, this.building.Def.TileLayer, this.building.Def.ReplacementLayer);
        }
        else
        {
          Debug.LogError((object) "multiple replacement tiles on the same cell!");
          Util.KDestroyGameObject(((Component) this).gameObject);
        }
        GameObject gameObject2 = Grid.Objects[cell, (int) this.building.Def.ObjectLayer];
        if (Object.op_Inequality((Object) gameObject2, (Object) null))
        {
          Deconstructable component2 = gameObject2.GetComponent<Deconstructable>();
          if (Object.op_Inequality((Object) component2, (Object) null))
            component2.CancelDeconstruction();
        }
      }
    }
    bool flag = Object.op_Implicit((Object) this.building.Def.BuildingComplete.GetComponent<Ladder>());
    this.waitForFetchesBeforeDigging = flag || Object.op_Implicit((Object) this.building.Def.BuildingComplete.GetComponent<SimCellOccupier>()) || Object.op_Implicit((Object) this.building.Def.BuildingComplete.GetComponent<Door>()) || Object.op_Implicit((Object) this.building.Def.BuildingComplete.GetComponent<LiquidPumpingStation>());
    if (flag)
    {
      int x = 0;
      int y1 = 0;
      Grid.CellToXY(Grid.PosToCell((KMonoBehaviour) this), out x, out y1);
      int y2 = y1 - 3;
      this.ladderDetectionExtents = new Extents(x, y2, 1, 5);
      this.ladderParititonerEntry = GameScenePartitioner.Instance.Add("Constructable.OnNearbyBuildingLayerChanged", (object) ((Component) this).gameObject, this.ladderDetectionExtents, GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnNearbyBuildingLayerChanged));
      this.OnNearbyBuildingLayerChanged((object) null);
    }
    this.fetchList.Submit(new System.Action(this.OnFetchListComplete), true);
    this.PlaceDiggables();
    new ReachabilityMonitor.Instance((Workable) this).StartSM();
    this.Subscribe<Constructable>(493375141, Constructable.OnRefreshUserMenuDelegate);
    Prioritizable component3 = ((Component) this).GetComponent<Prioritizable>();
    component3.onPriorityChanged += new Action<PrioritySetting>(this.OnPriorityChanged);
    this.OnPriorityChanged(component3.GetMasterPriority());
  }

  private void OnPriorityChanged(PrioritySetting priority) => this.building.RunOnArea((Action<int>) (cell =>
  {
    Diggable diggable = Diggable.GetDiggable(cell);
    if (!Object.op_Inequality((Object) diggable, (Object) null))
      return;
    ((Component) diggable).GetComponent<Prioritizable>().SetMasterPriority(priority);
  }));

  private void MarkArea()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    BuildingDef def = this.building.Def;
    Orientation orientation = this.building.Orientation;
    ObjectLayer layer = this.IsReplacementTile ? def.ReplacementLayer : def.ObjectLayer;
    def.MarkArea(cell, orientation, layer, ((Component) this).gameObject);
    if (!def.IsTilePiece)
      return;
    if (Object.op_Equality((Object) Grid.Objects[cell, (int) def.TileLayer], (Object) null))
    {
      def.MarkArea(cell, orientation, def.TileLayer, ((Component) this).gameObject);
      def.RunOnArea(cell, orientation, (Action<int>) (c => TileVisualizer.RefreshCell(c, def.TileLayer, def.ReplacementLayer)));
    }
    Grid.IsTileUnderConstruction[cell] = true;
  }

  private void UnmarkArea()
  {
    if (this.unmarked)
      return;
    this.unmarked = true;
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    BuildingDef def = this.building.Def;
    ObjectLayer layer = this.IsReplacementTile ? this.building.Def.ReplacementLayer : this.building.Def.ObjectLayer;
    def.UnmarkArea(cell, this.building.Orientation, layer, ((Component) this).gameObject);
    if (!def.IsTilePiece)
      return;
    Grid.IsTileUnderConstruction[cell] = false;
  }

  private void OnNearbyBuildingLayerChanged(object data)
  {
    this.hasLadderNearby = false;
    for (int y = this.ladderDetectionExtents.y; y < this.ladderDetectionExtents.y + this.ladderDetectionExtents.height; ++y)
    {
      int num = Grid.OffsetCell(0, this.ladderDetectionExtents.x, y);
      if (Grid.IsValidCell(num))
      {
        GameObject gameObject = (GameObject) null;
        Grid.ObjectLayers[1].TryGetValue(num, out gameObject);
        if (Object.op_Inequality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject.GetComponent<Ladder>(), (Object) null))
        {
          this.hasLadderNearby = true;
          break;
        }
      }
    }
  }

  private bool IsWire() => ((Object) this.building.Def).name.Contains("Wire");

  public bool IconConnectionAnimation(
    float delay,
    int connectionCount,
    string defName,
    string soundName)
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    if (this.building.Def.Name.Contains(defName))
    {
      Building building = (Building) null;
      GameObject gameObject = Grid.Objects[cell, 1];
      if (Object.op_Inequality((Object) gameObject, (Object) null))
        building = gameObject.GetComponent<Building>();
      if (Object.op_Inequality((Object) building, (Object) null))
      {
        bool flag = this.IsWire();
        int num1 = flag ? building.GetPowerInputCell() : building.GetUtilityInputCell();
        int num2 = flag ? num1 : building.GetUtilityOutputCell();
        if (cell == num1 || cell == num2)
        {
          BuildingCellVisualizer component = ((Component) building).gameObject.GetComponent<BuildingCellVisualizer>();
          if (Object.op_Inequality((Object) component, (Object) null) && (flag ? (component.RequiresPower ? 1 : 0) : (component.RequiresUtilityConnection ? 1 : 0)) != 0)
          {
            component.ConnectedEventWithDelay(delay, connectionCount, cell, soundName);
            return true;
          }
        }
      }
    }
    return false;
  }

  protected override void OnCleanUp()
  {
    if (this.IsReplacementTile && this.building.Def.isKAnimTile)
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      GameObject gameObject = Grid.Objects[cell, (int) this.building.Def.ReplacementLayer];
      if (Object.op_Equality((Object) gameObject, (Object) ((Component) this).gameObject) && Object.op_Inequality((Object) gameObject.GetComponent<SimCellOccupier>(), (Object) null))
        World.Instance.blockTileRenderer.RemoveBlock(this.building.Def, this.IsReplacementTile, SimHashes.Void, cell);
    }
    GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
    GameScenePartitioner.Instance.Free(ref this.digPartitionerEntry);
    GameScenePartitioner.Instance.Free(ref this.ladderParititonerEntry);
    SaveLoadRoot component = ((Component) this).GetComponent<SaveLoadRoot>();
    if (Object.op_Inequality((Object) component, (Object) null))
      SaveLoader.Instance.saveManager.Unregister(component);
    if (this.fetchList != null)
      this.fetchList.Cancel("Constructable destroyed");
    this.UnmarkArea();
    foreach (int placementCell in this.building.PlacementCells)
    {
      Diggable diggable = Diggable.GetDiggable(placementCell);
      if (Object.op_Inequality((Object) diggable, (Object) null))
        TracesExtesions.DeleteObject(((Component) diggable).gameObject);
    }
    base.OnCleanUp();
  }

  private void OnDiggableReachabilityChanged(object data)
  {
    if (this.IsReplacementTile)
      return;
    int diggable_count = 0;
    int unreachable_count = 0;
    this.building.RunOnArea((Action<int>) (offset_cell =>
    {
      Diggable diggable = Diggable.GetDiggable(offset_cell);
      if (!Object.op_Inequality((Object) diggable, (Object) null))
        return;
      ++diggable_count;
      if (((Component) diggable).GetComponent<KPrefabID>().HasTag(GameTags.Reachable))
        return;
      ++unreachable_count;
    }));
    bool flag = unreachable_count > 0 && unreachable_count == diggable_count;
    if (flag == this.hasUnreachableDigs)
      return;
    if (flag)
      ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ConstructableDigUnreachable);
    else
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ConstructableDigUnreachable);
    this.hasUnreachableDigs = flag;
  }

  private void PlaceDiggables()
  {
    if (this.waitForFetchesBeforeDigging && this.fetchList != null && !this.hasLadderNearby)
    {
      this.OnDiggableReachabilityChanged((object) null);
    }
    else
    {
      bool digs_complete = true;
      if (!this.solidPartitionerEntry.IsValid())
      {
        Extents placementExtents = this.building.GetValidPlacementExtents();
        this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnFetchListComplete", (object) ((Component) this).gameObject, placementExtents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChangedOrDigDestroyed));
        this.digPartitionerEntry = GameScenePartitioner.Instance.Add("Constructable.OnFetchListComplete", (object) ((Component) this).gameObject, placementExtents, GameScenePartitioner.Instance.digDestroyedLayer, new Action<object>(this.OnSolidChangedOrDigDestroyed));
      }
      if (!this.IsReplacementTile)
      {
        this.building.RunOnArea((Action<int>) (offset_cell =>
        {
          PrioritySetting masterPriority = ((Component) this).GetComponent<Prioritizable>().GetMasterPriority();
          if (!Diggable.IsDiggable(offset_cell))
            return;
          digs_complete = false;
          Diggable diggable = Diggable.GetDiggable(offset_cell);
          if (Object.op_Equality((Object) diggable, (Object) null))
          {
            diggable = GameUtil.KInstantiate(Assets.GetPrefab(new Tag("DigPlacer")), Grid.SceneLayer.Move).GetComponent<Diggable>();
            ((Component) diggable).gameObject.SetActive(true);
            TransformExtensions.SetPosition(diggable.transform, Grid.CellToPosCBC(offset_cell, Grid.SceneLayer.Move));
            diggable.Subscribe(-1432940121, new Action<object>(this.OnDiggableReachabilityChanged));
            Grid.Objects[offset_cell, 7] = ((Component) diggable).gameObject;
          }
          else
          {
            diggable.Unsubscribe(-1432940121, new Action<object>(this.OnDiggableReachabilityChanged));
            diggable.Subscribe(-1432940121, new Action<object>(this.OnDiggableReachabilityChanged));
          }
          diggable.choreTypeIdHash = Db.Get().ChoreTypes.BuildDig.IdHash;
          ((Component) diggable).GetComponent<Prioritizable>().SetMasterPriority(masterPriority);
          RenderUtil.EnableRenderer(diggable.transform, false);
          SaveLoadRoot component = ((Component) diggable).GetComponent<SaveLoadRoot>();
          if (!Object.op_Inequality((Object) component, (Object) null))
            return;
          Object.Destroy((Object) component);
        }));
        this.OnDiggableReachabilityChanged((object) null);
      }
      bool flag1 = this.building.Def.IsValidBuildLocation(((Component) this).gameObject, TransformExtensions.GetPosition(this.transform), this.building.Orientation, this.IsReplacementTile);
      if (flag1)
        this.notifier.Remove(this.invalidLocation);
      else
        this.notifier.Add(this.invalidLocation);
      ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidBuildingLocation, !flag1, (object) this);
      bool flag2 = digs_complete & flag1 && this.fetchList == null;
      if (flag2 && this.buildChore == null)
      {
        this.buildChore = (Chore) new WorkChore<Constructable>(Db.Get().ChoreTypes.Build, (IStateMachineTarget) this, on_complete: new Action<Chore>(this.UpdateBuildState), on_begin: new Action<Chore>(this.UpdateBuildState), on_end: new Action<Chore>(this.UpdateBuildState), is_preemptable: true);
        this.UpdateBuildState(this.buildChore);
      }
      else
      {
        if (flag2 || this.buildChore == null)
          return;
        this.buildChore.Cancel("Need to dig");
        this.buildChore = (Chore) null;
      }
    }
  }

  private void OnFetchListComplete()
  {
    this.fetchList = (FetchList2) null;
    this.PlaceDiggables();
    this.ClearMaterialNeeds();
  }

  private void ClearMaterialNeeds()
  {
    if (this.materialNeedsCleared)
      return;
    foreach (Recipe.Ingredient allIngredient in this.Recipe.GetAllIngredients(this.SelectedElementsTags))
      MaterialNeeds.UpdateNeed(allIngredient.tag, -allIngredient.amount, ((Component) this).gameObject.GetMyWorldId());
    this.materialNeedsCleared = true;
  }

  private void OnSolidChangedOrDigDestroyed(object data)
  {
    if (Object.op_Equality((Object) this, (Object) null) || this.finished)
      return;
    this.PlaceDiggables();
  }

  private void UpdateBuildState(Chore chore)
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (chore.InProgress())
      component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstruction);
    else
      component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.UnderConstructionNoWorker);
  }

  [System.Runtime.Serialization.OnDeserialized]
  internal void OnDeserialized()
  {
    if (this.ids == null)
      return;
    this.selectedElements = new Element[this.ids.Length];
    for (int index = 0; index < this.ids.Length; ++index)
      this.selectedElements[index] = ElementLoader.FindElementByHash((SimHashes) this.ids[index]);
    if (this.selectedElementsTags == null)
    {
      this.selectedElementsTags = new Tag[this.ids.Length];
      for (int index = 0; index < this.ids.Length; ++index)
        this.selectedElementsTags[index] = ElementLoader.FindElementByHash((SimHashes) this.ids[index]).tag;
    }
    Debug.Assert(this.selectedElements.Length == this.selectedElementsTags.Length);
    for (int index = 0; index < this.selectedElements.Length; ++index)
      Debug.Assert(Tag.op_Equality(this.selectedElements[index].tag, this.SelectedElementsTags[index]));
  }

  private void OnReachableChanged(object data)
  {
    KAnimControllerBase component = ((Component) this).GetComponent<KAnimControllerBase>();
    if ((bool) data)
    {
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.ConstructionUnreachable);
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.TintColour = Color32.op_Implicit(Game.Instance.uiColours.Build.validLocation);
    }
    else
    {
      ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.ConstructionUnreachable, (object) this);
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.TintColour = Color32.op_Implicit(Game.Instance.uiColours.Build.unreachable);
    }
  }

  private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", (string) UI.USERMENUACTIONS.CANCELCONSTRUCTION.NAME, new System.Action(this.OnPressCancel), tooltipText: ((string) UI.USERMENUACTIONS.CANCELCONSTRUCTION.TOOLTIP)));

  private void OnPressCancel() => EventExtensions.Trigger(((Component) this).gameObject, 2127324410, (object) null);

  private void OnCancel(object data = null)
  {
    ((KScreen) DetailsScreen.Instance).Show(false);
    this.ClearMaterialNeeds();
  }
}
