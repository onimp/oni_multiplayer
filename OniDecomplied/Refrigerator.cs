// Decompiled with JetBrains decompiler
// Type: Refrigerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Refrigerator")]
public class Refrigerator : KMonoBehaviour, IUserControlledCapacity
{
  [MyCmpGet]
  private Storage storage;
  [MyCmpGet]
  private Operational operational;
  [MyCmpGet]
  private LogicPorts ports;
  [Serialize]
  private float userMaxCapacity = float.PositiveInfinity;
  private FilteredStorage filteredStorage;
  private static readonly EventSystem.IntraObjectHandler<Refrigerator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Refrigerator>((Action<Refrigerator, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly EventSystem.IntraObjectHandler<Refrigerator> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<Refrigerator>((Action<Refrigerator, object>) ((component, data) => component.UpdateLogicCircuitCB(data)));

  protected virtual void OnPrefabInit() => this.filteredStorage = new FilteredStorage((KMonoBehaviour) this, new Tag[1]
  {
    GameTags.Compostable
  }, (IUserControlledCapacity) this, true, Db.Get().ChoreTypes.FoodFetch);

  protected virtual void OnSpawn()
  {
    ((Component) this).GetComponent<KAnimControllerBase>().Play(HashedString.op_Implicit("off"));
    ((Component) this).GetComponent<FoodStorage>().FilteredStorage = this.filteredStorage;
    this.filteredStorage.FilterChanged();
    this.UpdateLogicCircuit();
    this.Subscribe<Refrigerator>(-905833192, Refrigerator.OnCopySettingsDelegate);
    this.Subscribe<Refrigerator>(-1697596308, Refrigerator.UpdateLogicCircuitCBDelegate);
    this.Subscribe<Refrigerator>(-592767678, Refrigerator.UpdateLogicCircuitCBDelegate);
  }

  protected virtual void OnCleanUp() => this.filteredStorage.CleanUp();

  public bool IsActive() => this.operational.IsActive;

  private void OnCopySettings(object data)
  {
    GameObject gameObject = (GameObject) data;
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    Refrigerator component = gameObject.GetComponent<Refrigerator>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    this.UserMaxCapacity = component.UserMaxCapacity;
  }

  public float UserMaxCapacity
  {
    get => Mathf.Min(this.userMaxCapacity, this.storage.capacityKg);
    set
    {
      this.userMaxCapacity = value;
      this.filteredStorage.FilterChanged();
      this.UpdateLogicCircuit();
    }
  }

  public float AmountStored => this.storage.MassStored();

  public float MinCapacity => 0.0f;

  public float MaxCapacity => this.storage.capacityKg;

  public bool WholeValues => false;

  public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

  private void UpdateLogicCircuitCB(object data) => this.UpdateLogicCircuit();

  private void UpdateLogicCircuit()
  {
    bool on = this.filteredStorage.IsFull() & this.operational.IsOperational;
    this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, on ? 1 : 0);
    this.filteredStorage.SetLogicMeter(on);
  }
}
