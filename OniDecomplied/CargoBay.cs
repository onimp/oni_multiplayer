// Decompiled with JetBrains decompiler
// Type: CargoBay
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CargoBay")]
public class CargoBay : KMonoBehaviour
{
  public Storage storage;
  private MeterController meter;
  [Serialize]
  public float reservedResources;
  public CargoBay.CargoType storageType;
  public static Dictionary<Element.State, CargoBay.CargoType> ElementStateToCargoTypes = new Dictionary<Element.State, CargoBay.CargoType>()
  {
    {
      Element.State.Gas,
      CargoBay.CargoType.Gasses
    },
    {
      Element.State.Liquid,
      CargoBay.CargoType.Liquids
    },
    {
      Element.State.Solid,
      CargoBay.CargoType.Solids
    }
  };
  private static readonly EventSystem.IntraObjectHandler<CargoBay> OnLaunchDelegate = new EventSystem.IntraObjectHandler<CargoBay>((Action<CargoBay, object>) ((component, data) => component.OnLaunch(data)));
  private static readonly EventSystem.IntraObjectHandler<CargoBay> OnLandDelegate = new EventSystem.IntraObjectHandler<CargoBay>((Action<CargoBay, object>) ((component, data) => component.OnLand(data)));
  private static readonly EventSystem.IntraObjectHandler<CargoBay> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<CargoBay>((Action<CargoBay, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<CargoBay> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<CargoBay>((Action<CargoBay, object>) ((component, data) => component.OnStorageChange(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("grounded"), (KAnim.PlayMode) 0);
    this.Subscribe<CargoBay>(-1277991738, CargoBay.OnLaunchDelegate);
    this.Subscribe<CargoBay>(-887025858, CargoBay.OnLandDelegate);
    this.Subscribe<CargoBay>(493375141, CargoBay.OnRefreshUserMenuDelegate);
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[4]
    {
      "meter_target",
      "meter_fill",
      "meter_frame",
      "meter_OL"
    });
    this.meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
    this.OnStorageChange((object) null);
    this.Subscribe<CargoBay>(-1697596308, CargoBay.OnStorageChangeDelegate);
  }

  private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_empty_contents", (string) UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, (System.Action) (() => this.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null)), tooltipText: ((string) UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP)));

  private void OnStorageChange(object data) => this.meter.SetPositionPercent(this.storage.MassStored() / this.storage.Capacity());

  public void SpawnResources(object data)
  {
    if (DlcManager.FeatureClusterSpaceEnabled())
      return;
    ILaunchableRocket component1 = ((Component) ((Component) this).GetComponent<RocketModule>().conditionManager).GetComponent<ILaunchableRocket>();
    if (component1.registerType == LaunchableRocketRegisterType.Clustercraft)
      return;
    SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(component1));
    int cell = Grid.PosToCell(((Component) this).gameObject);
    foreach (KeyValuePair<SimHashes, float> keyValuePair in spacecraftDestination.GetMissionResourceResult(this.storage.RemainingCapacity(), this.reservedResources, this.storageType == CargoBay.CargoType.Solids, this.storageType == CargoBay.CargoType.Liquids, this.storageType == CargoBay.CargoType.Gasses))
    {
      Element elementByHash = ElementLoader.FindElementByHash(keyValuePair.Key);
      if (this.storageType == CargoBay.CargoType.Solids && elementByHash.IsSolid)
      {
        GameObject go = Scenario.SpawnPrefab(cell, 0, 0, ((Tag) ref elementByHash.tag).Name);
        go.GetComponent<PrimaryElement>().Mass = keyValuePair.Value;
        go.GetComponent<PrimaryElement>().Temperature = ElementLoader.FindElementByHash(keyValuePair.Key).defaultValues.temperature;
        go.SetActive(true);
        this.storage.Store(go);
      }
      else if (this.storageType == CargoBay.CargoType.Liquids && elementByHash.IsLiquid)
        this.storage.AddLiquid(keyValuePair.Key, keyValuePair.Value, ElementLoader.FindElementByHash(keyValuePair.Key).defaultValues.temperature, byte.MaxValue, 0);
      else if (this.storageType == CargoBay.CargoType.Gasses && elementByHash.IsGas)
        this.storage.AddGasChunk(keyValuePair.Key, keyValuePair.Value, ElementLoader.FindElementByHash(keyValuePair.Key).defaultValues.temperature, byte.MaxValue, 0, false);
    }
    if (this.storageType != CargoBay.CargoType.Entities)
      return;
    foreach (KeyValuePair<Tag, int> keyValuePair in spacecraftDestination.GetMissionEntityResult())
    {
      GameObject prefab = Assets.GetPrefab(keyValuePair.Key);
      if (Object.op_Equality((Object) prefab, (Object) null))
      {
        Tag key = keyValuePair.Key;
        KCrashReporter.Assert(false, "Missing prefab: " + ((Tag) ref key).Name);
      }
      else
      {
        for (int index = 0; index < keyValuePair.Value; ++index)
        {
          GameObject go = Util.KInstantiate(prefab, this.transform.position);
          go.SetActive(true);
          this.storage.Store(go);
          Baggable component2 = go.GetComponent<Baggable>();
          if (Object.op_Inequality((Object) component2, (Object) null))
            component2.SetWrangled();
        }
      }
    }
  }

  public void OnLaunch(object data)
  {
    this.ReserveResources();
    ConduitDispenser component = ((Component) this).GetComponent<ConduitDispenser>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.conduitType = ConduitType.None;
  }

  private void ReserveResources()
  {
    if (DlcManager.FeatureClusterSpaceEnabled())
      return;
    ILaunchableRocket component = ((Component) ((Component) this).GetComponent<RocketModule>().conditionManager).GetComponent<ILaunchableRocket>();
    if (component.registerType == LaunchableRocketRegisterType.Clustercraft)
      return;
    int spacecraftId = SpacecraftManager.instance.GetSpacecraftID(component);
    this.reservedResources = SpacecraftManager.instance.GetSpacecraftDestination(spacecraftId).ReserveResources(this);
  }

  public void OnLand(object data)
  {
    this.SpawnResources(data);
    ConduitDispenser component = ((Component) this).GetComponent<ConduitDispenser>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
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

  public enum CargoType
  {
    Solids,
    Liquids,
    Gasses,
    Entities,
  }
}
