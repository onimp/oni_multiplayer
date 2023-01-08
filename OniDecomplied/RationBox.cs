// Decompiled with JetBrains decompiler
// Type: RationBox
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/RationBox")]
public class RationBox : KMonoBehaviour, IUserControlledCapacity, IRender1000ms, IRottable
{
  [MyCmpReq]
  private Storage storage;
  [Serialize]
  private float userMaxCapacity = float.PositiveInfinity;
  private FilteredStorage filteredStorage;
  private static readonly EventSystem.IntraObjectHandler<RationBox> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<RationBox>((Action<RationBox, object>) ((component, data) => component.OnOperationalChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<RationBox> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<RationBox>((Action<RationBox, object>) ((component, data) => component.OnCopySettings(data)));

  protected virtual void OnPrefabInit()
  {
    this.filteredStorage = new FilteredStorage((KMonoBehaviour) this, new Tag[1]
    {
      GameTags.Compostable
    }, (IUserControlledCapacity) this, false, Db.Get().ChoreTypes.FoodFetch);
    this.Subscribe<RationBox>(-592767678, RationBox.OnOperationalChangedDelegate);
    this.Subscribe<RationBox>(-905833192, RationBox.OnCopySettingsDelegate);
    DiscoveredResources.Instance.Discover(TagExtensions.ToTag("FieldRation"), GameTags.Edible);
  }

  protected virtual void OnSpawn()
  {
    Operational component = ((Component) this).GetComponent<Operational>();
    component.SetActive(component.IsOperational);
    this.filteredStorage.FilterChanged();
  }

  protected virtual void OnCleanUp() => this.filteredStorage.CleanUp();

  private void OnOperationalChanged(object data)
  {
    Operational component = ((Component) this).GetComponent<Operational>();
    component.SetActive(component.IsOperational);
  }

  private void OnCopySettings(object data)
  {
    GameObject gameObject = (GameObject) data;
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    RationBox component = gameObject.GetComponent<RationBox>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    this.UserMaxCapacity = component.UserMaxCapacity;
  }

  public void Render1000ms(float dt) => Rottable.SetStatusItems((IRottable) this);

  public float UserMaxCapacity
  {
    get => Mathf.Min(this.userMaxCapacity, this.storage.capacityKg);
    set
    {
      this.userMaxCapacity = value;
      this.filteredStorage.FilterChanged();
    }
  }

  public float AmountStored => this.storage.MassStored();

  public float MinCapacity => 0.0f;

  public float MaxCapacity => this.storage.capacityKg;

  public bool WholeValues => false;

  public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();

  public float RotTemperature => 277.15f;

  public float PreserveTemperature => 255.15f;

  [SpecialName]
  GameObject IRottable.get_gameObject() => ((Component) this).gameObject;
}
