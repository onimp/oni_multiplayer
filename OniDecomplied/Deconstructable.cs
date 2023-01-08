// Decompiled with JetBrains decompiler
// Type: Deconstructable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Deconstructable")]
public class Deconstructable : Workable
{
  public Chore chore;
  public bool allowDeconstruction = true;
  public string audioSize;
  [Serialize]
  private bool isMarkedForDeconstruction;
  [Serialize]
  public Tag[] constructionElements;
  public bool looseEntityDeconstructable;
  private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Deconstructable>((Action<Deconstructable, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnCancelDelegate = new EventSystem.IntraObjectHandler<Deconstructable>((Action<Deconstructable, object>) ((component, data) => component.OnCancel(data)));
  private static readonly EventSystem.IntraObjectHandler<Deconstructable> OnDeconstructDelegate = new EventSystem.IntraObjectHandler<Deconstructable>((Action<Deconstructable, object>) ((component, data) => component.OnDeconstruct(data)));
  private static readonly Vector2 INITIAL_VELOCITY_RANGE = new Vector2(0.5f, 4f);
  private bool destroyed;

  private CellOffset[] placementOffsets
  {
    get
    {
      Building component1 = ((Component) this).GetComponent<Building>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        CellOffset[] placementOffsets = component1.Def.PlacementOffsets;
        Rotatable component2 = ((Component) component1).GetComponent<Rotatable>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          placementOffsets = new CellOffset[component1.Def.PlacementOffsets.Length];
          for (int index = 0; index < placementOffsets.Length; ++index)
            placementOffsets[index] = component2.GetRotatedCellOffset(component1.Def.PlacementOffsets[index]);
        }
        return placementOffsets;
      }
      OccupyArea component3 = ((Component) this).GetComponent<OccupyArea>();
      if (Object.op_Inequality((Object) component3, (Object) null))
        return component3.OccupiedCellsOffsets;
      if (this.looseEntityDeconstructable)
        return new CellOffset[1]{ new CellOffset(0, 0) };
      Debug.Assert(false, (object) "Ack! We put a Deconstructable on something that's neither a Building nor OccupyArea!", (Object) this);
      return (CellOffset[]) null;
    }
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.faceTargetWhenWorking = true;
    this.synchronizeAnims = false;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
    this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
    this.minimumAttributeMultiplier = 0.75f;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
    this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
    this.multitoolContext = HashedString.op_Implicit("build");
    this.multitoolHitEffectTag = Tag.op_Implicit(EffectConfigs.BuildSplashId);
    this.workingPstComplete = (HashedString[]) null;
    this.workingPstFailed = (HashedString[]) null;
    Building component = ((Component) this).GetComponent<Building>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.Def.IsTilePiece)
      this.SetWorkTime(component.Def.ConstructionTime * 0.5f);
    else
      this.SetWorkTime(30f);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    CellOffset[] filter = (CellOffset[]) null;
    CellOffset[][] table = OffsetGroups.InvertedStandardTable;
    Building component = ((Component) this).GetComponent<Building>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.Def.IsTilePiece)
    {
      table = OffsetGroups.InvertedStandardTableWithCorners;
      filter = component.Def.ConstructionOffsetFilter;
    }
    this.SetOffsetTable(OffsetGroups.BuildReachabilityTable(this.placementOffsets, table, filter));
    this.Subscribe<Deconstructable>(493375141, Deconstructable.OnRefreshUserMenuDelegate);
    this.Subscribe<Deconstructable>(-111137758, Deconstructable.OnRefreshUserMenuDelegate);
    this.Subscribe<Deconstructable>(2127324410, Deconstructable.OnCancelDelegate);
    this.Subscribe<Deconstructable>(-790448070, Deconstructable.OnDeconstructDelegate);
    if (this.constructionElements == null || this.constructionElements.Length == 0)
    {
      this.constructionElements = new Tag[1];
      this.constructionElements[0] = ((Component) this).GetComponent<PrimaryElement>().Element.tag;
    }
    if (!this.isMarkedForDeconstruction)
      return;
    this.QueueDeconstruction();
  }

  protected override void OnStartWork(Worker worker)
  {
    this.progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("DeconstructBar");
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction);
    this.Trigger(1830962028, (object) this);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    Building component1 = ((Component) this).GetComponent<Building>();
    SimCellOccupier component2 = ((Component) this).GetComponent<SimCellOccupier>();
    if (Object.op_Inequality((Object) DetailsScreen.Instance, (Object) null) && DetailsScreen.Instance.CompareTargetWith(((Component) this).gameObject))
      ((KScreen) DetailsScreen.Instance).Show(false);
    PrimaryElement component3 = ((Component) this).GetComponent<PrimaryElement>();
    float temperature = component3.Temperature;
    byte disease_idx = component3.DiseaseIdx;
    int disease_count = component3.DiseaseCount;
    if (Object.op_Inequality((Object) component2, (Object) null))
    {
      if (component1.Def.TileLayer != ObjectLayer.NumLayers)
      {
        int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
        if (Object.op_Equality((Object) Grid.Objects[cell, (int) component1.Def.TileLayer], (Object) ((Component) this).gameObject))
        {
          Grid.Objects[cell, (int) component1.Def.ObjectLayer] = (GameObject) null;
          Grid.Objects[cell, (int) component1.Def.TileLayer] = (GameObject) null;
          Grid.Foundation[cell] = false;
          TileVisualizer.RefreshCell(cell, component1.Def.TileLayer, component1.Def.ReplacementLayer);
        }
      }
      component2.DestroySelf((System.Action) (() => this.TriggerDestroy(temperature, disease_idx, disease_count)));
    }
    else
      this.TriggerDestroy(temperature, disease_idx, disease_count);
    if (Object.op_Equality((Object) component1, (Object) null) || component1.Def.PlayConstructionSounds)
    {
      string sound = GlobalAssets.GetSound("Finish_Deconstruction_" + (!Util.IsNullOrWhiteSpace(this.audioSize) ? this.audioSize : component1.Def.AudioSize));
      if (sound != null)
        KMonoBehaviour.PlaySound3DAtLocation(sound, TransformExtensions.GetPosition(((Component) this).gameObject.transform));
    }
    this.Trigger(-702296337, (object) this);
  }

  public bool HasBeenDestroyed => this.destroyed;

  public List<GameObject> ForceDestroyAndGetMaterials()
  {
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    return this.TriggerDestroy(component.Temperature, component.DiseaseIdx, component.DiseaseCount);
  }

  private List<GameObject> TriggerDestroy(float temperature, byte disease_idx, int disease_count)
  {
    if (Object.op_Equality((Object) this, (Object) null) || this.destroyed)
      return (List<GameObject>) null;
    List<GameObject> gameObjectList = this.SpawnItemsFromConstruction(temperature, disease_idx, disease_count);
    this.destroyed = true;
    TracesExtesions.DeleteObject(((Component) this).gameObject);
    return gameObjectList;
  }

  private void QueueDeconstruction()
  {
    if (DebugHandler.InstantBuildMode)
    {
      this.OnCompleteWork((Worker) null);
    }
    else
    {
      if (this.chore != null)
        return;
      BuildingComplete component = ((Component) this).GetComponent<BuildingComplete>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.Def.ReplacementLayer != ObjectLayer.NumLayers)
      {
        int cell = Grid.PosToCell((KMonoBehaviour) component);
        if (Object.op_Inequality((Object) Grid.Objects[cell, (int) component.Def.ReplacementLayer], (Object) null))
          return;
      }
      Prioritizable.AddRef(((Component) this).gameObject);
      this.chore = (Chore) new WorkChore<Deconstructable>(Db.Get().ChoreTypes.Deconstruct, (IStateMachineTarget) this, only_when_operational: false, is_preemptable: true, ignore_building_assignment: true);
      ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction, (object) this);
      this.isMarkedForDeconstruction = true;
      this.Trigger(2108245096, (object) "Deconstruct");
    }
  }

  private void OnDeconstruct()
  {
    if (this.chore == null)
      this.QueueDeconstruction();
    else
      this.CancelDeconstruction();
  }

  public bool IsMarkedForDeconstruction() => this.chore != null;

  public void SetAllowDeconstruction(bool allow)
  {
    this.allowDeconstruction = allow;
    if (this.allowDeconstruction)
      return;
    this.CancelDeconstruction();
  }

  public void SpawnItemsFromConstruction()
  {
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    this.SpawnItemsFromConstruction(component.Temperature, component.DiseaseIdx, component.DiseaseCount);
  }

  private List<GameObject> SpawnItemsFromConstruction(
    float temperature,
    byte disease_idx,
    int disease_count)
  {
    List<GameObject> gameObjectList = new List<GameObject>();
    if (!this.allowDeconstruction)
      return gameObjectList;
    Building component = ((Component) this).GetComponent<Building>();
    float[] numArray;
    if (Object.op_Inequality((Object) component, (Object) null))
      numArray = component.Def.Mass;
    else
      numArray = new float[1]
      {
        ((Component) this).GetComponent<PrimaryElement>().Mass
      };
    for (int index = 0; index < this.constructionElements.Length && numArray.Length > index; ++index)
    {
      GameObject go = this.SpawnItem(TransformExtensions.GetPosition(this.transform), this.constructionElements[index], numArray[index], temperature, disease_idx, disease_count);
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(go.transform));
      int num = Grid.CellAbove(cell);
      Vector2 zero;
      if (Grid.IsValidCell(cell) && Grid.Solid[cell] || Grid.IsValidCell(num) && Grid.Solid[num])
      {
        zero = Vector2.zero;
      }
      else
      {
        // ISSUE: explicit constructor call
        ((Vector2) ref zero).\u002Ector(Random.Range(-1f, 1f) * Deconstructable.INITIAL_VELOCITY_RANGE.x, Deconstructable.INITIAL_VELOCITY_RANGE.y);
      }
      if (((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) go))
        ((KGameObjectComponentManager<FallerComponent>) GameComps.Fallers).Remove(go);
      GameComps.Fallers.Add(go, zero);
      gameObjectList.Add(go);
    }
    return gameObjectList;
  }

  public GameObject SpawnItem(
    Vector3 position,
    Tag src_element,
    float src_mass,
    float src_temperature,
    byte disease_idx,
    int disease_count)
  {
    GameObject gameObject = (GameObject) null;
    int cell1 = Grid.PosToCell(position);
    CellOffset[] placementOffsets = this.placementOffsets;
    Element element = ElementLoader.GetElement(src_element);
    if (element != null)
    {
      float num = src_mass;
      for (int index1 = 0; (double) index1 < (double) src_mass / 400.0; ++index1)
      {
        int index2 = index1 % placementOffsets.Length;
        int cell2 = Grid.OffsetCell(cell1, placementOffsets[index2]);
        float mass = num;
        if ((double) num > 400.0)
        {
          mass = 400f;
          num -= 400f;
        }
        gameObject = element.substance.SpawnResource(Grid.CellToPosCBC(cell2, Grid.SceneLayer.Ore), mass, src_temperature, disease_idx, disease_count);
      }
    }
    else
    {
      for (int index3 = 0; (double) index3 < (double) src_mass; ++index3)
      {
        int index4 = index3 % placementOffsets.Length;
        int cell3 = Grid.OffsetCell(cell1, placementOffsets[index4]);
        gameObject = GameUtil.KInstantiate(Assets.GetPrefab(src_element), Grid.CellToPosCBC(cell3, Grid.SceneLayer.Ore), Grid.SceneLayer.Ore);
        gameObject.SetActive(true);
      }
    }
    return gameObject;
  }

  private void OnRefreshUserMenu(object data)
  {
    if (!this.allowDeconstruction)
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, this.chore == null ? new KIconButtonMenu.ButtonInfo("action_deconstruct", (string) UI.USERMENUACTIONS.DECONSTRUCT.NAME, new System.Action(this.OnDeconstruct), tooltipText: ((string) UI.USERMENUACTIONS.DECONSTRUCT.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_deconstruct", (string) UI.USERMENUACTIONS.DECONSTRUCT.NAME_OFF, new System.Action(this.OnDeconstruct), tooltipText: ((string) UI.USERMENUACTIONS.DECONSTRUCT.TOOLTIP_OFF)), 0.0f);
  }

  public void CancelDeconstruction()
  {
    if (this.chore == null)
      return;
    this.chore.Cancel("Cancelled deconstruction");
    this.chore = (Chore) null;
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.PendingDeconstruction);
    this.ShowProgressBar(false);
    this.isMarkedForDeconstruction = false;
    Prioritizable.RemoveRef(((Component) this).gameObject);
  }

  private void OnCancel(object data) => this.CancelDeconstruction();

  private void OnDeconstruct(object data)
  {
    if (!this.allowDeconstruction && !DebugHandler.InstantBuildMode)
      return;
    this.QueueDeconstruction();
  }
}
