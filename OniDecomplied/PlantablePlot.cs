// Decompiled with JetBrains decompiler
// Type: PlantablePlot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class PlantablePlot : SingleEntityReceptacle, ISaveLoadable, IGameObjectEffectDescriptor
{
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  [Serialize]
  private Ref<KPrefabID> plantRef;
  public Vector3 occupyingObjectVisualOffset = Vector3.zero;
  public Grid.SceneLayer plantLayer = Grid.SceneLayer.BuildingBack;
  private EntityPreview plantPreview;
  [SerializeField]
  private bool accepts_fertilizer;
  [SerializeField]
  private bool accepts_irrigation = true;
  [SerializeField]
  public bool has_liquid_pipe_input;
  private static readonly EventSystem.IntraObjectHandler<PlantablePlot> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<PlantablePlot>((Action<PlantablePlot, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly EventSystem.IntraObjectHandler<PlantablePlot> OnUpdateRoomDelegate = new EventSystem.IntraObjectHandler<PlantablePlot>((Action<PlantablePlot, object>) ((component, data) =>
  {
    if (!Object.op_Inequality((Object) component.plantRef.Get(), (Object) null))
      return;
    ((KMonoBehaviour) component.plantRef.Get()).Trigger(144050788, data);
  }));

  public KPrefabID plant
  {
    get => this.plantRef.Get();
    set => this.plantRef.Set(value);
  }

  public bool ValidPlant => Object.op_Equality((Object) this.plantPreview, (Object) null) || this.plantPreview.Valid;

  public bool AcceptsFertilizer => this.accepts_fertilizer;

  public bool AcceptsIrrigation => this.accepts_irrigation;

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (!DlcManager.FeaturePlantMutationsEnabled())
    {
      this.requestedEntityAdditionalFilterTag = Tag.Invalid;
    }
    else
    {
      if (!((Tag) ref this.requestedEntityTag).IsValid || !((Tag) ref this.requestedEntityAdditionalFilterTag).IsValid || PlantSubSpeciesCatalog.Instance.IsValidPlantableSeed(this.requestedEntityTag, this.requestedEntityAdditionalFilterTag))
        return;
      this.requestedEntityAdditionalFilterTag = Tag.Invalid;
    }
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.choreType = Db.Get().ChoreTypes.FarmFetch;
    this.statusItemNeed = Db.Get().BuildingStatusItems.NeedSeed;
    this.statusItemNoneAvailable = Db.Get().BuildingStatusItems.NoAvailableSeed;
    this.statusItemAwaitingDelivery = Db.Get().BuildingStatusItems.AwaitingSeedDelivery;
    this.plantRef = new Ref<KPrefabID>();
    this.Subscribe<PlantablePlot>(-905833192, PlantablePlot.OnCopySettingsDelegate);
    this.Subscribe<PlantablePlot>(144050788, PlantablePlot.OnUpdateRoomDelegate);
    if (!((Component) this).HasTag(GameTags.FarmTiles))
      return;
    this.storage.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
    DropAllWorkable component1 = ((Component) this).GetComponent<DropAllWorkable>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      component1.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
    Toggleable component2 = ((Component) this).GetComponent<Toggleable>();
    if (!Object.op_Inequality((Object) component2, (Object) null))
      return;
    component2.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
  }

  private void OnCopySettings(object data)
  {
    PlantablePlot component1 = ((GameObject) data).GetComponent<PlantablePlot>();
    if (!Object.op_Inequality((Object) component1, (Object) null))
      return;
    if (Object.op_Equality((Object) this.occupyingObject, (Object) null) && (Tag.op_Inequality(this.requestedEntityTag, component1.requestedEntityTag) || Tag.op_Inequality(this.requestedEntityAdditionalFilterTag, component1.requestedEntityAdditionalFilterTag) || Object.op_Inequality((Object) component1.occupyingObject, (Object) null)))
    {
      Tag requestedEntityTag = component1.requestedEntityTag;
      Tag additionalFilterTag = component1.requestedEntityAdditionalFilterTag;
      if (Object.op_Inequality((Object) component1.occupyingObject, (Object) null))
      {
        SeedProducer component2 = component1.occupyingObject.GetComponent<SeedProducer>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          requestedEntityTag = TagManager.Create(component2.seedInfo.seedId);
          MutantPlant component3 = component1.occupyingObject.GetComponent<MutantPlant>();
          additionalFilterTag = Object.op_Implicit((Object) component3) ? component3.SubSpeciesID : Tag.Invalid;
        }
      }
      this.CancelActiveRequest();
      this.CreateOrder(requestedEntityTag, additionalFilterTag);
    }
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    Prioritizable component4 = ((Component) this).GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component4, (Object) null))
      return;
    Prioritizable component5 = this.occupyingObject.GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component5, (Object) null))
      return;
    component5.SetMasterPriority(component4.GetMasterPriority());
  }

  public override void CreateOrder(Tag entityTag, Tag additionalFilterTag)
  {
    this.SetPreview(entityTag, false);
    if (this.ValidPlant)
      base.CreateOrder(entityTag, additionalFilterTag);
    else
      this.SetPreview(Tag.Invalid, false);
  }

  private void SyncPriority(PrioritySetting priority)
  {
    Prioritizable component1 = ((Component) this).GetComponent<Prioritizable>();
    if (!object.Equals((object) component1.GetMasterPriority(), (object) priority))
      component1.SetMasterPriority(priority);
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    Prioritizable component2 = this.occupyingObject.GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component2, (Object) null) || object.Equals((object) component2.GetMasterPriority(), (object) priority))
      return;
    component2.SetMasterPriority(component1.GetMasterPriority());
  }

  protected override void OnSpawn()
  {
    if (Object.op_Inequality((Object) this.plant, (Object) null))
      this.RegisterWithPlant(((Component) this.plant).gameObject);
    base.OnSpawn();
    this.autoReplaceEntity = false;
    Components.PlantablePlots.Add(((Component) this).gameObject.GetMyWorldId(), this);
    ((Component) this).GetComponent<Prioritizable>().onPriorityChanged += new Action<PrioritySetting>(this.SyncPriority);
  }

  public void SetFertilizationFlags(bool fertilizer, bool liquid_piping)
  {
    this.accepts_fertilizer = fertilizer;
    this.has_liquid_pipe_input = liquid_piping;
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    if (Object.op_Inequality((Object) this.plantPreview, (Object) null))
      Util.KDestroyGameObject(((Component) this.plantPreview).gameObject);
    if (Object.op_Implicit((Object) this.occupyingObject))
      EventExtensions.Trigger(this.occupyingObject, -216549700, (object) null);
    Components.PlantablePlots.Remove(((Component) this).gameObject.GetMyWorldId(), this);
  }

  protected override GameObject SpawnOccupyingObject(GameObject depositedEntity)
  {
    PlantableSeed component1 = depositedEntity.GetComponent<PlantableSeed>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      Vector3 posCbc = Grid.CellToPosCBC(Grid.PosToCell((KMonoBehaviour) this), this.plantLayer);
      GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(component1.PlantID), posCbc, this.plantLayer);
      MutantPlant component2 = gameObject.GetComponent<MutantPlant>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        ((Component) component1).GetComponent<MutantPlant>().CopyMutationsTo(component2);
      gameObject.SetActive(true);
      this.destroyEntityOnDeposit = true;
      return gameObject;
    }
    this.destroyEntityOnDeposit = false;
    return depositedEntity;
  }

  protected override void ConfigureOccupyingObject(GameObject newPlant)
  {
    this.plantRef.Set(newPlant.GetComponent<KPrefabID>());
    this.RegisterWithPlant(newPlant);
    UprootedMonitor component1 = newPlant.GetComponent<UprootedMonitor>();
    if (Object.op_Implicit((Object) component1))
      component1.canBeUprooted = false;
    this.autoReplaceEntity = false;
    Prioritizable component2 = ((Component) this).GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component2, (Object) null))
      return;
    Prioritizable component3 = newPlant.GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component3, (Object) null))
      return;
    component3.SetMasterPriority(component2.GetMasterPriority());
    component3.onPriorityChanged += new Action<PrioritySetting>(this.SyncPriority);
  }

  public void ReplacePlant(GameObject plant, bool keepStorage)
  {
    if (keepStorage)
    {
      this.UnsubscribeFromOccupant();
      this.occupyingObject = (GameObject) null;
    }
    this.ForceDeposit(plant);
  }

  protected override void PositionOccupyingObject()
  {
    base.PositionOccupyingObject();
    KBatchedAnimController component = this.occupyingObject.GetComponent<KBatchedAnimController>();
    component.SetSceneLayer(this.plantLayer);
    this.OffsetAnim(component, this.occupyingObjectVisualOffset);
  }

  private void RegisterWithPlant(GameObject plant)
  {
    this.occupyingObject = plant;
    ReceptacleMonitor component = plant.GetComponent<ReceptacleMonitor>();
    if (Object.op_Implicit((Object) component))
      component.SetReceptacle(this);
    EventExtensions.Trigger(plant, 1309017699, (object) this.storage);
  }

  protected override void SubscribeToOccupant()
  {
    base.SubscribeToOccupant();
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    this.Subscribe(this.occupyingObject, -216549700, new Action<object>(this.OnOccupantUprooted));
  }

  protected override void UnsubscribeFromOccupant()
  {
    base.UnsubscribeFromOccupant();
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    this.Unsubscribe(this.occupyingObject, -216549700, new Action<object>(this.OnOccupantUprooted));
  }

  private void OnOccupantUprooted(object data)
  {
    this.autoReplaceEntity = false;
    this.requestedEntityTag = Tag.Invalid;
    this.requestedEntityAdditionalFilterTag = Tag.Invalid;
  }

  public override void OrderRemoveOccupant()
  {
    if (Object.op_Equality((Object) this.Occupant, (Object) null))
      return;
    Uprootable component = this.Occupant.GetComponent<Uprootable>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    component.MarkForUproot();
  }

  public override void SetPreview(Tag entityTag, bool solid = false)
  {
    PlantableSeed plantableSeed = (PlantableSeed) null;
    if (((Tag) ref entityTag).IsValid)
    {
      GameObject prefab = Assets.GetPrefab(entityTag);
      if (Object.op_Equality((Object) prefab, (Object) null))
      {
        DebugUtil.LogWarningArgs((Object) ((Component) this).gameObject, new object[2]
        {
          (object) "Planter tried previewing a tag with no asset! If this was the 'Empty' tag, ignore it, that will go away in new save games. Otherwise... Eh? Tag was: ",
          (object) entityTag
        });
        return;
      }
      plantableSeed = prefab.GetComponent<PlantableSeed>();
    }
    if (Object.op_Inequality((Object) this.plantPreview, (Object) null))
    {
      KPrefabID component = ((Component) this.plantPreview).GetComponent<KPrefabID>();
      if (Object.op_Inequality((Object) plantableSeed, (Object) null) && Object.op_Inequality((Object) component, (Object) null) && Tag.op_Equality(component.PrefabTag, plantableSeed.PreviewID))
        return;
      KMonoBehaviourExtensions.Unsubscribe(((Component) this.plantPreview).gameObject, -1820564715, new Action<object>(this.OnValidChanged));
      Util.KDestroyGameObject(((Component) this.plantPreview).gameObject);
    }
    if (!Object.op_Inequality((Object) plantableSeed, (Object) null))
      return;
    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(plantableSeed.PreviewID), Grid.SceneLayer.Front);
    this.plantPreview = gameObject.GetComponent<EntityPreview>();
    TransformExtensions.SetPosition(gameObject.transform, Vector3.zero);
    gameObject.transform.SetParent(((Component) this).gameObject.transform, false);
    TransformExtensions.SetLocalPosition(gameObject.transform, Vector3.zero);
    if (Object.op_Inequality((Object) this.rotatable, (Object) null))
    {
      if (plantableSeed.direction == SingleEntityReceptacle.ReceptacleDirection.Top)
        TransformExtensions.SetLocalPosition(gameObject.transform, this.occupyingObjectRelativePosition);
      else if (plantableSeed.direction == SingleEntityReceptacle.ReceptacleDirection.Side)
        TransformExtensions.SetLocalPosition(gameObject.transform, Rotatable.GetRotatedOffset(this.occupyingObjectRelativePosition, Orientation.R90));
      else
        TransformExtensions.SetLocalPosition(gameObject.transform, Rotatable.GetRotatedOffset(this.occupyingObjectRelativePosition, Orientation.R180));
    }
    else
      TransformExtensions.SetLocalPosition(gameObject.transform, this.occupyingObjectRelativePosition);
    this.OffsetAnim(gameObject.GetComponent<KBatchedAnimController>(), this.occupyingObjectVisualOffset);
    gameObject.SetActive(true);
    KMonoBehaviourExtensions.Subscribe(gameObject, -1820564715, new Action<object>(this.OnValidChanged));
    if (solid)
      this.plantPreview.SetSolid();
    this.plantPreview.UpdateValidity();
  }

  private void OffsetAnim(KBatchedAnimController kanim, Vector3 offset)
  {
    if (Object.op_Inequality((Object) this.rotatable, (Object) null))
      offset = this.rotatable.GetRotatedOffset(offset);
    kanim.Offset = offset;
  }

  private void OnValidChanged(object obj)
  {
    this.Trigger(-1820564715, obj);
    if (this.plantPreview.Valid || this.GetActiveRequest == null)
      return;
    this.CancelActiveRequest();
  }

  public override List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.ENABLESDOMESTICGROWTH, (string) UI.BUILDINGEFFECTS.TOOLTIPS.ENABLESDOMESTICGROWTH, (Descriptor.DescriptorType) 1);
    descriptors.Add(descriptor);
    return descriptors;
  }
}
