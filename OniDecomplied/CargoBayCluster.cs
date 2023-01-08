// Decompiled with JetBrains decompiler
// Type: CargoBayCluster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CargoBayCluster : KMonoBehaviour, IUserControlledCapacity
{
  private MeterController meter;
  [SerializeField]
  public Storage storage;
  [SerializeField]
  public CargoBay.CargoType storageType;
  [Serialize]
  private float userMaxCapacity;
  private static readonly EventSystem.IntraObjectHandler<CargoBayCluster> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<CargoBayCluster>((Action<CargoBayCluster, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<CargoBayCluster> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<CargoBayCluster>((Action<CargoBayCluster, object>) ((component, data) => component.OnStorageChange(data)));

  public float UserMaxCapacity
  {
    get => this.userMaxCapacity;
    set
    {
      this.userMaxCapacity = value;
      this.Trigger(-945020481, (object) this);
    }
  }

  public float MinCapacity => 0.0f;

  public float MaxCapacity => this.storage.capacityKg;

  public float AmountStored => this.storage.MassStored();

  public bool WholeValues => false;

  public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

  public float RemainingCapacity => this.userMaxCapacity - this.storage.MassStored();

  protected virtual void OnPrefabInit() => this.userMaxCapacity = this.storage.capacityKg;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("grounded"), (KAnim.PlayMode) 0);
    this.Subscribe<CargoBayCluster>(493375141, CargoBayCluster.OnRefreshUserMenuDelegate);
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[4]
    {
      "meter_target",
      "meter_fill",
      "meter_frame",
      "meter_OL"
    });
    this.meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
    this.OnStorageChange((object) null);
    this.Subscribe<CargoBayCluster>(-1697596308, CargoBayCluster.OnStorageChangeDelegate);
  }

  private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_empty_contents", (string) UI.USERMENUACTIONS.EMPTYSTORAGE.NAME, (System.Action) (() => this.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null)), tooltipText: ((string) UI.USERMENUACTIONS.EMPTYSTORAGE.TOOLTIP)));

  private void OnStorageChange(object data)
  {
    this.meter.SetPositionPercent(this.storage.MassStored() / this.storage.Capacity());
    this.UpdateCargoStatusItem();
  }

  private void UpdateCargoStatusItem()
  {
    RocketModuleCluster component1 = ((Component) this).GetComponent<RocketModuleCluster>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return;
    CraftModuleInterface craftInterface = component1.CraftInterface;
    if (Object.op_Equality((Object) craftInterface, (Object) null))
      return;
    Clustercraft component2 = ((Component) craftInterface).GetComponent<Clustercraft>();
    if (Object.op_Equality((Object) component2, (Object) null))
      return;
    component2.UpdateStatusItem();
  }
}
