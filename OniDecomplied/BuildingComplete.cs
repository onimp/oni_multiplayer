// Decompiled with JetBrains decompiler
// Type: BuildingComplete
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingComplete : Building
{
  [MyCmpReq]
  private Modifiers modifiers;
  [MyCmpGet]
  public KPrefabID prefabid;
  public bool isManuallyOperated;
  public bool isArtable;
  public PrimaryElement primaryElement;
  [Serialize]
  public float creationTime = -1f;
  private bool hasSpawnedKComponents;
  private ObjectLayer replacingTileLayer = ObjectLayer.NumLayers;
  public List<AttributeModifier> regionModifiers = new List<AttributeModifier>();
  private static readonly EventSystem.IntraObjectHandler<BuildingComplete> OnEntombedChange = new EventSystem.IntraObjectHandler<BuildingComplete>((Action<BuildingComplete, object>) ((component, data) => component.OnEntombedChanged()));
  private static readonly EventSystem.IntraObjectHandler<BuildingComplete> OnObjectReplacedDelegate = new EventSystem.IntraObjectHandler<BuildingComplete>((Action<BuildingComplete, object>) ((component, data) => component.OnObjectReplaced(data)));
  private HandleVector<int>.Handle scenePartitionerEntry;
  public static float MinKelvinSeen = float.MaxValue;

  private bool WasReplaced() => this.replacingTileLayer != ObjectLayer.NumLayers;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    position.z = Grid.GetLayerZ(this.Def.SceneLayer);
    TransformExtensions.SetPosition(this.transform, position);
    Util.SetLayerRecursively(((Component) this).gameObject, LayerMask.NameToLayer("Default"));
    KBatchedAnimController component1 = ((Component) this).GetComponent<KBatchedAnimController>();
    Rotatable component2 = ((Component) this).GetComponent<Rotatable>();
    if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Equality((Object) component2, (Object) null))
      component1.Offset = this.Def.GetVisualizerOffset();
    KBoxCollider2D component3 = ((Component) this).GetComponent<KBoxCollider2D>();
    if (Object.op_Inequality((Object) component3, (Object) null))
    {
      Vector3 visualizerOffset = this.Def.GetVisualizerOffset();
      component3.offset = Vector2.op_Addition(component3.offset, new Vector2(visualizerOffset.x, visualizerOffset.y));
    }
    Attributes attributes = this.GetAttributes();
    foreach (Klei.AI.Attribute attribute in this.Def.attributes)
      attributes.Add(attribute);
    foreach (AttributeModifier attributeModifier in this.Def.attributeModifiers)
    {
      Klei.AI.Attribute attribute = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
      if (attributes.Get(attribute) == null)
        attributes.Add(attribute);
      attributes.Add(attributeModifier);
    }
    foreach (AttributeInstance attributeInstance in attributes)
      this.regionModifiers.Add(new AttributeModifier(attributeInstance.Id, attributeInstance.GetTotalValue()));
    if (this.Def.UseStructureTemperature)
      GameComps.StructureTemperatures.Add(((Component) this).gameObject);
    this.Subscribe<BuildingComplete>(1606648047, BuildingComplete.OnObjectReplacedDelegate);
    if (!this.Def.Entombable)
      return;
    this.Subscribe<BuildingComplete>(-1089732772, BuildingComplete.OnEntombedChange);
  }

  private void OnEntombedChanged()
  {
    if (((Component) this).gameObject.HasTag(GameTags.Entombed))
      Components.EntombedBuildings.Add(this);
    else
      Components.EntombedBuildings.Remove(this);
  }

  public override void UpdatePosition()
  {
    base.UpdatePosition();
    GameScenePartitioner.Instance.UpdatePosition(this.scenePartitionerEntry, this.GetExtents());
  }

  private void OnObjectReplaced(object data) => this.replacingTileLayer = (ObjectLayer) data;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.primaryElement = ((Component) this).GetComponent<PrimaryElement>();
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    if (this.Def.IsFoundation)
    {
      foreach (int placementCell in this.PlacementCells)
      {
        Grid.Foundation[placementCell] = true;
        Game.Instance.roomProber.SolidChangedEvent(placementCell, false);
      }
    }
    if (Grid.IsValidCell(cell))
      TransformExtensions.SetPosition(this.transform, Grid.CellToPosCBC(cell, this.Def.SceneLayer));
    if (Object.op_Inequality((Object) this.primaryElement, (Object) null))
    {
      if ((double) this.primaryElement.Mass == 0.0)
        this.primaryElement.Mass = this.Def.Mass[0];
      float temperature = this.primaryElement.Temperature;
      if ((double) temperature > 0.0 && !float.IsNaN(temperature) && !float.IsInfinity(temperature))
        BuildingComplete.MinKelvinSeen = Mathf.Min(BuildingComplete.MinKelvinSeen, temperature);
      this.primaryElement.setTemperatureCallback += new PrimaryElement.SetTemperatureCallback(this.OnSetTemperature);
    }
    if (!((Component) this).gameObject.HasTag(GameTags.RocketInSpace))
    {
      this.Def.MarkArea(cell, this.Orientation, this.Def.ObjectLayer, ((Component) this).gameObject);
      if (this.Def.IsTilePiece)
      {
        this.Def.MarkArea(cell, this.Orientation, this.Def.TileLayer, ((Component) this).gameObject);
        this.Def.RunOnArea(cell, this.Orientation, (Action<int>) (c => TileVisualizer.RefreshCell(c, this.Def.TileLayer, this.Def.ReplacementLayer)));
      }
    }
    this.RegisterBlockTileRenderer();
    if (this.Def.PreventIdleTraversalPastBuilding)
    {
      for (int index = 0; index < this.PlacementCells.Length; ++index)
        Grid.PreventIdleTraversal[this.PlacementCells[index]] = true;
    }
    Components.BuildingCompletes.Add(this);
    BuildingConfigManager.Instance.AddBuildingCompleteKComponents(((Component) this).gameObject, this.Def.Tag);
    this.hasSpawnedKComponents = true;
    this.scenePartitionerEntry = GameScenePartitioner.Instance.Add(((Object) this).name, (object) this, this.GetExtents(), GameScenePartitioner.Instance.completeBuildings, (Action<object>) null);
    if (this.prefabid.HasTag(GameTags.TemplateBuilding))
      Components.TemplateBuildings.Add(this);
    Attributes attributes = this.GetAttributes();
    if (attributes != null)
    {
      Deconstructable component1 = ((Component) this).GetComponent<Deconstructable>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        for (int index = 1; index < component1.constructionElements.Length; ++index)
        {
          Tag constructionElement = component1.constructionElements[index];
          Element element = ElementLoader.GetElement(constructionElement);
          if (element != null)
          {
            foreach (AttributeModifier attributeModifier in element.attributeModifiers)
              attributes.Add(attributeModifier);
          }
          else
          {
            GameObject prefab = Assets.TryGetPrefab(constructionElement);
            if (Object.op_Inequality((Object) prefab, (Object) null))
            {
              PrefabAttributeModifiers component2 = prefab.GetComponent<PrefabAttributeModifiers>();
              if (Object.op_Inequality((Object) component2, (Object) null))
              {
                foreach (AttributeModifier descriptor in component2.descriptors)
                  attributes.Add(descriptor);
              }
            }
          }
        }
      }
    }
    BuildingInventory.Instance.RegisterBuilding(this);
  }

  private void OnSetTemperature(PrimaryElement primary_element, float temperature) => BuildingComplete.MinKelvinSeen = Mathf.Min(BuildingComplete.MinKelvinSeen, temperature);

  public void SetCreationTime(float time) => this.creationTime = time;

  private string GetInspectSound() => GlobalAssets.GetSound("AI_Inspect_" + ((Tag) ref ((Component) this).GetComponent<KPrefabID>().PrefabTag).Name);

  protected override void OnCleanUp()
  {
    if (Game.quitting)
      return;
    GameScenePartitioner.Instance.Free(ref this.scenePartitionerEntry);
    if (this.hasSpawnedKComponents)
      BuildingConfigManager.Instance.DestroyBuildingCompleteKComponents(((Component) this).gameObject, this.Def.Tag);
    if (this.Def.UseStructureTemperature)
      GameComps.StructureTemperatures.Remove(((Component) this).gameObject);
    base.OnCleanUp();
    if (!this.WasReplaced() && ((Component) this).gameObject.GetMyWorldId() != (int) ClusterManager.INVALID_WORLD_IDX)
    {
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      this.Def.UnmarkArea(cell, this.Orientation, this.Def.ObjectLayer, ((Component) this).gameObject);
      if (this.Def.IsTilePiece)
      {
        this.Def.UnmarkArea(cell, this.Orientation, this.Def.TileLayer, ((Component) this).gameObject);
        this.Def.RunOnArea(cell, this.Orientation, (Action<int>) (c => TileVisualizer.RefreshCell(c, this.Def.TileLayer, this.Def.ReplacementLayer)));
      }
      if (this.Def.IsFoundation)
      {
        foreach (int placementCell in this.PlacementCells)
        {
          Grid.Foundation[placementCell] = false;
          Game.Instance.roomProber.SolidChangedEvent(placementCell, false);
        }
      }
      if (this.Def.PreventIdleTraversalPastBuilding)
      {
        for (int index = 0; index < this.PlacementCells.Length; ++index)
          Grid.PreventIdleTraversal[this.PlacementCells[index]] = false;
      }
    }
    if (this.WasReplaced() && this.Def.IsTilePiece && this.replacingTileLayer != this.Def.TileLayer)
    {
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      this.Def.UnmarkArea(cell, this.Orientation, this.Def.TileLayer, ((Component) this).gameObject);
      this.Def.RunOnArea(cell, this.Orientation, (Action<int>) (c => TileVisualizer.RefreshCell(c, this.Def.TileLayer, this.Def.ReplacementLayer)));
    }
    Components.BuildingCompletes.Remove(this);
    Components.EntombedBuildings.Remove(this);
    Components.TemplateBuildings.Remove(this);
    this.UnregisterBlockTileRenderer();
    BuildingInventory.Instance.UnregisterBuilding(this);
    this.Trigger(-21016276, (object) this);
  }
}
