// Decompiled with JetBrains decompiler
// Type: Wire
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Wire")]
public class Wire : 
  KMonoBehaviour,
  IDisconnectable,
  IFirstFrameCallback,
  IWattageRating,
  IHaveUtilityNetworkMgr,
  IBridgedNetworkItem
{
  [SerializeField]
  public Wire.WattageRating MaxWattageRating;
  [SerializeField]
  private bool disconnected = true;
  public static readonly KAnimHashedString OutlineSymbol = new KAnimHashedString("outline");
  public float circuitOverloadTime;
  private static readonly EventSystem.IntraObjectHandler<Wire> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<Wire>((Action<Wire, object>) ((component, data) => component.OnBuildingBroken(data)));
  private static readonly EventSystem.IntraObjectHandler<Wire> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<Wire>((Action<Wire, object>) ((component, data) => component.OnBuildingFullyRepaired(data)));
  private static StatusItem WireCircuitStatus = (StatusItem) null;
  private static StatusItem WireMaxWattageStatus = (StatusItem) null;
  private System.Action firstFrameCallback;

  public static float GetMaxWattageAsFloat(Wire.WattageRating rating)
  {
    switch (rating)
    {
      case Wire.WattageRating.Max500:
        return 500f;
      case Wire.WattageRating.Max1000:
        return 1000f;
      case Wire.WattageRating.Max2000:
        return 2000f;
      case Wire.WattageRating.Max20000:
        return 20000f;
      case Wire.WattageRating.Max50000:
        return 50000f;
      default:
        return 0.0f;
    }
  }

  public bool IsConnected => Game.Instance.electricalConduitSystem.GetNetworkForCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform))) is ElectricalUtilityNetwork;

  public ushort NetworkID => !(Game.Instance.electricalConduitSystem.GetNetworkForCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform))) is ElectricalUtilityNetwork networkForCell) ? ushort.MaxValue : (ushort) networkForCell.id;

  protected virtual void OnSpawn()
  {
    Game.Instance.electricalConduitSystem.AddToNetworks(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), (object) this, false);
    this.InitializeSwitchState();
    this.Subscribe<Wire>(774203113, Wire.OnBuildingBrokenDelegate);
    this.Subscribe<Wire>(-1735440190, Wire.OnBuildingFullyRepairedDelegate);
    ((Component) this).GetComponent<KSelectable>().AddStatusItem(Wire.WireCircuitStatus, (object) this);
    ((Component) this).GetComponent<KSelectable>().AddStatusItem(Wire.WireMaxWattageStatus, (object) this);
    ((Component) this).GetComponent<KBatchedAnimController>().SetSymbolVisiblity(Wire.OutlineSymbol, false);
  }

  protected virtual void OnCleanUp()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    BuildingComplete component = ((Component) this).GetComponent<BuildingComplete>();
    if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Object.op_Equality((Object) Grid.Objects[cell, (int) component.Def.ReplacementLayer], (Object) null))
      Game.Instance.electricalConduitSystem.RemoveFromNetworks(cell, (object) this, false);
    this.Unsubscribe<Wire>(774203113, Wire.OnBuildingBrokenDelegate, false);
    this.Unsubscribe<Wire>(-1735440190, Wire.OnBuildingFullyRepairedDelegate, false);
    base.OnCleanUp();
  }

  private void InitializeSwitchState()
  {
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    bool flag = false;
    GameObject gameObject = Grid.Objects[cell, 1];
    if (Object.op_Inequality((Object) gameObject, (Object) null))
    {
      CircuitSwitch component = gameObject.GetComponent<CircuitSwitch>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        flag = true;
        component.AttachWire(this);
      }
    }
    if (flag)
      return;
    this.Connect();
  }

  public UtilityConnections GetWireConnections() => Game.Instance.electricalConduitSystem.GetConnections(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), true);

  public string GetWireConnectionsString() => Game.Instance.electricalConduitSystem.GetVisualizerString(this.GetWireConnections());

  private void OnBuildingBroken(object data) => this.Disconnect();

  private void OnBuildingFullyRepaired(object data) => this.InitializeSwitchState();

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Wires, false);
    if (Wire.WireCircuitStatus == null)
      Wire.WireCircuitStatus = new StatusItem("WireCircuitStatus", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID).SetResolveStringCallback((Func<string, object, string>) ((str, data) =>
      {
        Wire wire = (Wire) data;
        int cell = Grid.PosToCell(TransformExtensions.GetPosition(wire.transform));
        CircuitManager circuitManager = Game.Instance.circuitManager;
        ushort circuitId = circuitManager.GetCircuitID(cell);
        float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(circuitId);
        GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Watts;
        if (wire.MaxWattageRating >= Wire.WattageRating.Max20000)
          unit = GameUtil.WattageFormatterUnit.Kilowatts;
        float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(wire.MaxWattageRating);
        float neededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitId);
        string wireLoadColor = GameUtil.GetWireLoadColor(wattsUsedByCircuit, maxWattageAsFloat, neededWhenActive);
        string str1 = str;
        string newValue;
        if (!(wireLoadColor == Util.ToHexString(Color.white)))
          newValue = "<color=#" + wireLoadColor + ">" + GameUtil.GetFormattedWattage(wattsUsedByCircuit, unit) + "</color>";
        else
          newValue = GameUtil.GetFormattedWattage(wattsUsedByCircuit, unit);
        str = str1.Replace("{CurrentLoadAndColor}", newValue);
        str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(maxWattageAsFloat, unit));
        str = str.Replace("{WireType}", ((Component) this).GetProperName());
        return str;
      }));
    if (Wire.WireMaxWattageStatus != null)
      return;
    Wire.WireMaxWattageStatus = new StatusItem("WireMaxWattageStatus", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID).SetResolveStringCallback((Func<string, object, string>) ((str, data) =>
    {
      Wire wire = (Wire) data;
      GameUtil.WattageFormatterUnit unit = GameUtil.WattageFormatterUnit.Watts;
      if (wire.MaxWattageRating >= Wire.WattageRating.Max20000)
        unit = GameUtil.WattageFormatterUnit.Kilowatts;
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(wire.transform));
      CircuitManager circuitManager = Game.Instance.circuitManager;
      float neededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitManager.GetCircuitID(cell));
      float maxWattageAsFloat = Wire.GetMaxWattageAsFloat(wire.MaxWattageRating);
      string str2 = str;
      string newValue;
      if ((double) neededWhenActive <= (double) maxWattageAsFloat)
        newValue = GameUtil.GetFormattedWattage(neededWhenActive, unit);
      else
        newValue = "<color=#" + Util.ToHexString(new Color(0.9843137f, 0.6901961f, 0.23137255f)) + ">" + GameUtil.GetFormattedWattage(neededWhenActive, unit) + "</color>";
      str = str2.Replace("{TotalPotentialLoadAndColor}", newValue);
      str = str.Replace("{MaxLoad}", GameUtil.GetFormattedWattage(maxWattageAsFloat, unit));
      return str;
    }));
  }

  public Wire.WattageRating GetMaxWattageRating() => this.MaxWattageRating;

  public bool IsDisconnected() => this.disconnected;

  public bool Connect()
  {
    BuildingHP component = ((Component) this).GetComponent<BuildingHP>();
    if (Object.op_Equality((Object) component, (Object) null) || component.HitPoints > 0)
    {
      this.disconnected = false;
      Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
    }
    return !this.disconnected;
  }

  public void Disconnect()
  {
    this.disconnected = true;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireDisconnected);
    Game.Instance.electricalConduitSystem.ForceRebuildNetworks();
  }

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

  public IUtilityNetworkMgr GetNetworkManager() => (IUtilityNetworkMgr) Game.Instance.electricalConduitSystem;

  public void AddNetworks(ICollection<UtilityNetwork> networks)
  {
    UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
    if (networkForCell == null)
      return;
    networks.Add(networkForCell);
  }

  public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
  {
    UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
    return networks.Contains(networkForCell);
  }

  public int GetNetworkCell() => Grid.PosToCell((KMonoBehaviour) this);

  public enum WattageRating
  {
    Max500,
    Max1000,
    Max2000,
    Max20000,
    Max50000,
    NumRatings,
  }
}
