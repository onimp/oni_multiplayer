// Decompiled with JetBrains decompiler
// Type: Conduit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Conduit")]
public class Conduit : 
  KMonoBehaviour,
  IFirstFrameCallback,
  IHaveUtilityNetworkMgr,
  IBridgedNetworkItem,
  IDisconnectable,
  FlowUtilityNetwork.IItem
{
  [MyCmpReq]
  private KAnimGraphTileVisualizer graphTileDependency;
  [SerializeField]
  private bool disconnected = true;
  public ConduitType type;
  private System.Action firstFrameCallback;
  protected static readonly EventSystem.IntraObjectHandler<Conduit> OnHighlightedDelegate = new EventSystem.IntraObjectHandler<Conduit>((Action<Conduit, object>) ((component, data) => component.OnHighlighted(data)));
  protected static readonly EventSystem.IntraObjectHandler<Conduit> OnConduitFrozenDelegate = new EventSystem.IntraObjectHandler<Conduit>((Action<Conduit, object>) ((component, data) => component.OnConduitFrozen(data)));
  protected static readonly EventSystem.IntraObjectHandler<Conduit> OnConduitBoilingDelegate = new EventSystem.IntraObjectHandler<Conduit>((Action<Conduit, object>) ((component, data) => component.OnConduitBoiling(data)));
  protected static readonly EventSystem.IntraObjectHandler<Conduit> OnStructureTemperatureRegisteredDelegate = new EventSystem.IntraObjectHandler<Conduit>((Action<Conduit, object>) ((component, data) => component.OnStructureTemperatureRegistered(data)));
  protected static readonly EventSystem.IntraObjectHandler<Conduit> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<Conduit>((Action<Conduit, object>) ((component, data) => component.OnBuildingBroken(data)));
  protected static readonly EventSystem.IntraObjectHandler<Conduit> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<Conduit>((Action<Conduit, object>) ((component, data) => component.OnBuildingFullyRepaired(data)));

  public void SetFirstFrameCallback(System.Action ffCb)
  {
    this.firstFrameCallback = ffCb;
    ((MonoBehaviour) this).StartCoroutine(this.RunCallback());
  }

  private IEnumerator RunCallback()
  {
    yield return (object) null;
    if (this.firstFrameCallback != null)
    {
      this.firstFrameCallback();
      this.firstFrameCallback = (System.Action) null;
    }
    yield return (object) null;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Conduit>(-1201923725, Conduit.OnHighlightedDelegate);
    this.Subscribe<Conduit>(-700727624, Conduit.OnConduitFrozenDelegate);
    this.Subscribe<Conduit>(-1152799878, Conduit.OnConduitBoilingDelegate);
    this.Subscribe<Conduit>(-1555603773, Conduit.OnStructureTemperatureRegisteredDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<Conduit>(774203113, Conduit.OnBuildingBrokenDelegate);
    this.Subscribe<Conduit>(-1735440190, Conduit.OnBuildingFullyRepairedDelegate);
  }

  protected virtual void OnStructureTemperatureRegistered(object data)
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    this.GetNetworkManager().AddToNetworks(cell, (object) this, false);
    this.Connect();
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Pipe, (object) this);
    BuildingDef def = ((Component) this).GetComponent<Building>().Def;
    if (!Object.op_Inequality((Object) def, (Object) null) || (double) def.ThermalConductivity == 1.0)
      return;
    this.GetFlowVisualizer().AddThermalConductivity(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), def.ThermalConductivity);
  }

  protected virtual void OnCleanUp()
  {
    this.Unsubscribe<Conduit>(774203113, Conduit.OnBuildingBrokenDelegate, false);
    this.Unsubscribe<Conduit>(-1735440190, Conduit.OnBuildingFullyRepairedDelegate, false);
    BuildingDef def = ((Component) this).GetComponent<Building>().Def;
    if (Object.op_Inequality((Object) def, (Object) null) && (double) def.ThermalConductivity != 1.0)
      this.GetFlowVisualizer().RemoveThermalConductivity(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), def.ThermalConductivity);
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    this.GetNetworkManager().RemoveFromNetworks(cell, (object) this, false);
    BuildingComplete component = ((Component) this).GetComponent<BuildingComplete>();
    if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Object.op_Equality((Object) Grid.Objects[cell, (int) component.Def.ReplacementLayer], (Object) null))
    {
      this.GetNetworkManager().RemoveFromNetworks(cell, (object) this, false);
      this.GetFlowManager().EmptyConduit(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
    }
    base.OnCleanUp();
  }

  protected ConduitFlowVisualizer GetFlowVisualizer() => this.type != ConduitType.Gas ? Game.Instance.liquidFlowVisualizer : Game.Instance.gasFlowVisualizer;

  public IUtilityNetworkMgr GetNetworkManager() => this.type != ConduitType.Gas ? (IUtilityNetworkMgr) Game.Instance.liquidConduitSystem : (IUtilityNetworkMgr) Game.Instance.gasConduitSystem;

  public ConduitFlow GetFlowManager() => this.type != ConduitType.Gas ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;

  public static ConduitFlow GetFlowManager(ConduitType type) => type != ConduitType.Gas ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow;

  public static IUtilityNetworkMgr GetNetworkManager(ConduitType type) => type != ConduitType.Gas ? (IUtilityNetworkMgr) Game.Instance.liquidConduitSystem : (IUtilityNetworkMgr) Game.Instance.gasConduitSystem;

  public virtual void AddNetworks(ICollection<UtilityNetwork> networks)
  {
    UtilityNetwork networkForCell = this.GetNetworkManager().GetNetworkForCell(Grid.PosToCell((KMonoBehaviour) this));
    if (networkForCell == null)
      return;
    networks.Add(networkForCell);
  }

  public virtual bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
  {
    UtilityNetwork networkForCell = this.GetNetworkManager().GetNetworkForCell(Grid.PosToCell((KMonoBehaviour) this));
    return networks.Contains(networkForCell);
  }

  public virtual int GetNetworkCell() => Grid.PosToCell((KMonoBehaviour) this);

  private void OnHighlighted(object data)
  {
    int cell = (bool) data ? Grid.PosToCell(TransformExtensions.GetPosition(this.transform)) : -1;
    this.GetFlowVisualizer().SetHighlightedCell(cell);
  }

  private void OnConduitFrozen(object data)
  {
    this.Trigger(-794517298, (object) new BuildingHP.DamageSourceInfo()
    {
      damage = 1,
      source = (string) BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_FROZE,
      popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_FROZE,
      takeDamageEffect = (this.ConduitType == ConduitType.Gas ? SpawnFXHashes.BuildingLeakLiquid : SpawnFXHashes.BuildingFreeze),
      fullDamageEffectName = (this.ConduitType == ConduitType.Gas ? "water_damage_kanim" : "ice_damage_kanim")
    });
    this.GetFlowManager().EmptyConduit(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
  }

  private void OnConduitBoiling(object data)
  {
    this.Trigger(-794517298, (object) new BuildingHP.DamageSourceInfo()
    {
      damage = 1,
      source = (string) BUILDINGS.DAMAGESOURCES.CONDUIT_CONTENTS_BOILED,
      popString = (string) UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.CONDUIT_CONTENTS_BOILED,
      takeDamageEffect = SpawnFXHashes.BuildingLeakGas,
      fullDamageEffectName = "gas_damage_kanim"
    });
    this.GetFlowManager().EmptyConduit(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
  }

  private void OnBuildingBroken(object data) => this.Disconnect();

  private void OnBuildingFullyRepaired(object data) => this.Connect();

  public bool IsDisconnected() => this.disconnected;

  public bool Connect()
  {
    BuildingHP component = ((Component) this).GetComponent<BuildingHP>();
    if (Object.op_Equality((Object) component, (Object) null) || component.HitPoints > 0)
    {
      this.disconnected = false;
      this.GetNetworkManager().ForceRebuildNetworks();
    }
    return !this.disconnected;
  }

  public void Disconnect()
  {
    this.disconnected = true;
    this.GetNetworkManager().ForceRebuildNetworks();
  }

  public FlowUtilityNetwork Network
  {
    set
    {
    }
  }

  public int Cell => Grid.PosToCell((KMonoBehaviour) this);

  public Endpoint EndpointType => Endpoint.Conduit;

  public ConduitType ConduitType => this.type;

  public GameObject GameObject => ((Component) this).gameObject;
}
