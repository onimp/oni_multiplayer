// Decompiled with JetBrains decompiler
// Type: SimCellOccupier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SimCellOccupier")]
public class SimCellOccupier : KMonoBehaviour, IGameObjectEffectDescriptor
{
  [MyCmpReq]
  private Building building;
  [MyCmpReq]
  private PrimaryElement primaryElement;
  [SerializeField]
  public bool doReplaceElement = true;
  [SerializeField]
  public bool setGasImpermeable;
  [SerializeField]
  public bool setLiquidImpermeable;
  [SerializeField]
  public bool setTransparent;
  [SerializeField]
  public bool setOpaque;
  [SerializeField]
  public bool notifyOnMelt;
  [SerializeField]
  private bool setConstructedTile;
  [SerializeField]
  public float strengthMultiplier = 1f;
  [SerializeField]
  public float movementSpeedMultiplier = 1f;
  private bool isReady;
  private bool callDestroy = true;
  private static readonly EventSystem.IntraObjectHandler<SimCellOccupier> OnBuildingRepairedDelegate = new EventSystem.IntraObjectHandler<SimCellOccupier>((Action<SimCellOccupier, object>) ((component, data) => component.OnBuildingRepaired(data)));

  public bool IsVisuallySolid => this.doReplaceElement;

  protected virtual void OnPrefabInit()
  {
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Normal);
    if (!this.building.Def.IsFoundation)
      return;
    this.setConstructedTile = true;
  }

  protected virtual void OnSpawn()
  {
    HandleVector<Game.CallbackInfo>.Handle callbackHandle = Game.Instance.callbackManager.Add(new Game.CallbackInfo(new System.Action(this.OnModifyComplete)));
    float mass_per_cell = this.primaryElement.Mass / (float) this.building.Def.PlacementOffsets.Length;
    this.building.RunOnArea((Action<int>) (offset_cell =>
    {
      if (this.doReplaceElement)
      {
        SimMessages.ReplaceAndDisplaceElement(offset_cell, this.primaryElement.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, mass_per_cell, this.primaryElement.Temperature, this.primaryElement.DiseaseIdx, this.primaryElement.DiseaseCount, callbackHandle.index);
        callbackHandle = HandleVector<Game.CallbackInfo>.InvalidHandle;
        SimMessages.SetStrength(offset_cell, 0, this.strengthMultiplier);
        Game.Instance.RemoveSolidChangedFilter(offset_cell);
      }
      else
      {
        if (SaveGame.Instance.sandboxEnabled && Grid.Element[offset_cell].IsSolid)
          SimMessages.Dig(offset_cell);
        this.ForceSetGameCellData(offset_cell);
        Game.Instance.AddSolidChangedFilter(offset_cell);
      }
      Sim.Cell.Properties simCellProperties = this.GetSimCellProperties();
      SimMessages.SetCellProperties(offset_cell, (byte) simCellProperties);
      Grid.RenderedByWorld[offset_cell] = false;
      ((Component) Game.Instance).GetComponent<EntombedItemVisualizer>().ForceClear(offset_cell);
    }));
    this.Subscribe<SimCellOccupier>(-1699355994, SimCellOccupier.OnBuildingRepairedDelegate);
  }

  protected virtual void OnCleanUp()
  {
    if (!this.callDestroy)
      return;
    this.DestroySelf((System.Action) null);
  }

  private Sim.Cell.Properties GetSimCellProperties()
  {
    Sim.Cell.Properties simCellProperties = Sim.Cell.Properties.SolidImpermeable;
    if (this.setGasImpermeable)
      simCellProperties |= Sim.Cell.Properties.GasImpermeable;
    if (this.setLiquidImpermeable)
      simCellProperties |= Sim.Cell.Properties.LiquidImpermeable;
    if (this.setTransparent)
      simCellProperties |= Sim.Cell.Properties.Transparent;
    if (this.setOpaque)
      simCellProperties |= Sim.Cell.Properties.Opaque;
    if (this.setConstructedTile)
      simCellProperties |= Sim.Cell.Properties.ConstructedTile;
    if (this.notifyOnMelt)
      simCellProperties |= Sim.Cell.Properties.NotifyOnMelt;
    return simCellProperties;
  }

  public void DestroySelf(System.Action onComplete)
  {
    this.callDestroy = false;
    for (int index = 0; index < this.building.PlacementCells.Length; ++index)
    {
      int placementCell = this.building.PlacementCells[index];
      Game.Instance.RemoveSolidChangedFilter(placementCell);
      Sim.Cell.Properties simCellProperties = this.GetSimCellProperties();
      SimMessages.ClearCellProperties(placementCell, (byte) simCellProperties);
      if (this.doReplaceElement && Grid.Element[placementCell].id == this.primaryElement.ElementID)
      {
        HandleVector<int>.Handle handle1 = GameComps.DiseaseContainers.GetHandle(((Component) this).gameObject);
        if (handle1.IsValid())
        {
          DiseaseHeader header = ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).GetHeader(handle1) with
          {
            diseaseIdx = Grid.DiseaseIdx[placementCell],
            diseaseCount = Grid.DiseaseCount[placementCell]
          };
          ((KSplitCompactedVector<DiseaseHeader, DiseaseContainer>) GameComps.DiseaseContainers).SetHeader(handle1, header);
        }
        if (onComplete != null)
        {
          HandleVector<Game.CallbackInfo>.Handle handle2 = Game.Instance.callbackManager.Add(new Game.CallbackInfo(onComplete));
          SimMessages.ReplaceElement(placementCell, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierDestroySelf, 0.0f, callbackIdx: handle2.index);
        }
        else
          SimMessages.ReplaceElement(placementCell, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierDestroySelf, 0.0f);
        SimMessages.SetStrength(placementCell, 1, 1f);
      }
      else
      {
        Grid.SetSolid(placementCell, false, CellEventLogger.Instance.SimCellOccupierDestroy);
        Util.Signal(onComplete);
        World.Instance.OnSolidChanged(placementCell);
        GameScenePartitioner.Instance.TriggerEvent(placementCell, GameScenePartitioner.Instance.solidChangedLayer, (object) null);
      }
    }
  }

  public bool IsReady() => this.isReady;

  private void OnModifyComplete()
  {
    if (Object.op_Equality((Object) this, (Object) null) || Object.op_Equality((Object) ((Component) this).gameObject, (Object) null))
      return;
    this.isReady = true;
    ((Component) this).GetComponent<PrimaryElement>().SetUseSimDiseaseInfo(true);
    Vector2I xy = Grid.PosToXY(TransformExtensions.GetPosition(this.transform));
    GameScenePartitioner.Instance.TriggerEvent(xy.x, xy.y, 1, 1, GameScenePartitioner.Instance.solidChangedLayer, (object) null);
  }

  private void ForceSetGameCellData(int cell)
  {
    bool solid = !Grid.DupePassable[cell];
    Grid.SetSolid(cell, solid, CellEventLogger.Instance.SimCellOccupierForceSolid);
    Pathfinding.Instance.AddDirtyNavGridCell(cell);
    GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.solidChangedLayer, (object) null);
    Grid.Damage[cell] = 0.0f;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = (List<Descriptor>) null;
    if ((double) this.movementSpeedMultiplier != 1.0)
    {
      descriptors = new List<Descriptor>();
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.DUPLICANTMOVEMENTBOOST, (object) GameUtil.AddPositiveSign(GameUtil.GetFormattedPercent((float) ((double) this.movementSpeedMultiplier * 100.0 - 100.0)), (double) this.movementSpeedMultiplier - 1.0 >= 0.0)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.DUPLICANTMOVEMENTBOOST, (object) GameUtil.GetFormattedPercent((float) ((double) this.movementSpeedMultiplier * 100.0 - 100.0))), (Descriptor.DescriptorType) 1);
      descriptors.Add(descriptor);
    }
    return descriptors;
  }

  private void OnBuildingRepaired(object data)
  {
    BuildingHP buildingHp = (BuildingHP) data;
    float damage = (float) (1.0 - (double) buildingHp.HitPoints / (double) buildingHp.MaxHitPoints);
    this.building.RunOnArea((Action<int>) (offset_cell => WorldDamage.Instance.RestoreDamageToValue(offset_cell, damage)));
  }
}
