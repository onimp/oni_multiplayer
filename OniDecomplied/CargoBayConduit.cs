// Decompiled with JetBrains decompiler
// Type: CargoBayConduit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CargoBay")]
public class CargoBayConduit : KMonoBehaviour
{
  public static Dictionary<ConduitType, CargoBay.CargoType> ElementToCargoMap = new Dictionary<ConduitType, CargoBay.CargoType>()
  {
    {
      ConduitType.Solid,
      CargoBay.CargoType.Solids
    },
    {
      ConduitType.Liquid,
      CargoBay.CargoType.Liquids
    },
    {
      ConduitType.Gas,
      CargoBay.CargoType.Gasses
    }
  };
  private static readonly EventSystem.IntraObjectHandler<CargoBayConduit> OnLaunchDelegate = new EventSystem.IntraObjectHandler<CargoBayConduit>((Action<CargoBayConduit, object>) ((component, data) => component.OnLaunch(data)));
  private static readonly EventSystem.IntraObjectHandler<CargoBayConduit> OnLandDelegate = new EventSystem.IntraObjectHandler<CargoBayConduit>((Action<CargoBayConduit, object>) ((component, data) => component.OnLand(data)));
  private static StatusItem connectedPortStatus;
  private static StatusItem connectedWrongPortStatus;
  private static StatusItem connectedNoPortStatus;
  private CargoBay.CargoType storageType;
  private Guid connectedConduitPortStatusItem;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (CargoBayConduit.connectedPortStatus == null)
    {
      CargoBayConduit.connectedPortStatus = new StatusItem("CONNECTED_ROCKET_PORT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, true, OverlayModes.None.ID);
      CargoBayConduit.connectedWrongPortStatus = new StatusItem("CONNECTED_ROCKET_WRONG_PORT", "BUILDING", "", StatusItem.IconType.Info, NotificationType.BadMinor, true, OverlayModes.None.ID);
      CargoBayConduit.connectedNoPortStatus = new StatusItem("CONNECTED_ROCKET_NO_PORT", "BUILDING", "status_item_no_liquid_to_pump", StatusItem.IconType.Custom, NotificationType.Bad, true, OverlayModes.None.ID);
    }
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad, (Object) null))
    {
      this.OnLaunchpadChainChanged((object) null);
      ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.Subscribe(-1009905786, new Action<object>(this.OnLaunchpadChainChanged));
    }
    this.Subscribe<CargoBayConduit>(-1277991738, CargoBayConduit.OnLaunchDelegate);
    this.Subscribe<CargoBayConduit>(-887025858, CargoBayConduit.OnLandDelegate);
    this.storageType = ((Component) this).GetComponent<CargoBay>().storageType;
    this.UpdateStatusItems();
  }

  protected virtual void OnCleanUp()
  {
    LaunchPad currentPad = ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
    if (Object.op_Inequality((Object) currentPad, (Object) null))
      currentPad.Unsubscribe(-1009905786, new Action<object>(this.OnLaunchpadChainChanged));
    base.OnCleanUp();
  }

  public void OnLaunch(object data)
  {
    ConduitDispenser component = ((Component) this).GetComponent<ConduitDispenser>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.conduitType = ConduitType.None;
    ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.Unsubscribe(-1009905786, new Action<object>(this.OnLaunchpadChainChanged));
  }

  public void OnLand(object data)
  {
    ConduitDispenser component = ((Component) this).GetComponent<ConduitDispenser>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      switch (this.storageType)
      {
        case CargoBay.CargoType.Liquids:
          component.conduitType = ConduitType.Liquid;
          break;
        case CargoBay.CargoType.Gasses:
          component.conduitType = ConduitType.Gas;
          break;
        default:
          component.conduitType = ConduitType.None;
          break;
      }
    }
    ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad.Subscribe(-1009905786, new Action<object>(this.OnLaunchpadChainChanged));
    this.UpdateStatusItems();
  }

  private void OnLaunchpadChainChanged(object data) => this.UpdateStatusItems();

  private void UpdateStatusItems()
  {
    bool hasMatch;
    bool hasAny;
    this.HasMatchingConduitPort(out hasMatch, out hasAny);
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (hasMatch)
      this.connectedConduitPortStatusItem = component.ReplaceStatusItem(this.connectedConduitPortStatusItem, CargoBayConduit.connectedPortStatus, (object) this);
    else if (hasAny)
      this.connectedConduitPortStatusItem = component.ReplaceStatusItem(this.connectedConduitPortStatusItem, CargoBayConduit.connectedWrongPortStatus, (object) this);
    else
      this.connectedConduitPortStatusItem = component.ReplaceStatusItem(this.connectedConduitPortStatusItem, CargoBayConduit.connectedNoPortStatus, (object) this);
  }

  private void HasMatchingConduitPort(out bool hasMatch, out bool hasAny)
  {
    hasMatch = false;
    hasAny = false;
    LaunchPad currentPad = ((Component) this).GetComponent<RocketModuleCluster>().CraftInterface.CurrentPad;
    if (Object.op_Equality((Object) currentPad, (Object) null))
      return;
    ChainedBuilding.StatesInstance smi = ((Component) currentPad).GetSMI<ChainedBuilding.StatesInstance>();
    if (smi == null)
      return;
    HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain = HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.Allocate();
    smi.GetLinkedBuildings(ref chain);
    foreach (StateMachine.Instance instance in (HashSet<ChainedBuilding.StatesInstance>) chain)
    {
      IConduitDispenser component = instance.GetComponent<IConduitDispenser>();
      if (component != null)
      {
        hasAny = true;
        if (CargoBayConduit.ElementToCargoMap[component.ConduitType] == this.storageType)
        {
          hasMatch = true;
          break;
        }
      }
    }
    chain.Recycle();
  }
}
