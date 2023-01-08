// Decompiled with JetBrains decompiler
// Type: StorageLocker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/StorageLocker")]
public class StorageLocker : KMonoBehaviour, IUserControlledCapacity
{
  private LoggerFS log;
  [Serialize]
  private float userMaxCapacity = float.PositiveInfinity;
  [Serialize]
  public string lockerName = "";
  protected FilteredStorage filteredStorage;
  [MyCmpGet]
  private UserNameable nameable;
  public string choreTypeID = Db.Get().ChoreTypes.StorageFetch.Id;
  private static readonly EventSystem.IntraObjectHandler<StorageLocker> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<StorageLocker>((Action<StorageLocker, object>) ((component, data) => component.OnCopySettings(data)));

  protected virtual void OnPrefabInit() => this.Initialize(false);

  protected void Initialize(bool use_logic_meter)
  {
    base.OnPrefabInit();
    this.log = new LoggerFS(nameof (StorageLocker), 35);
    ChoreType fetch_chore_type = Db.Get().ChoreTypes.Get(this.choreTypeID);
    this.filteredStorage = new FilteredStorage((KMonoBehaviour) this, (Tag[]) null, (IUserControlledCapacity) this, use_logic_meter, fetch_chore_type);
    this.Subscribe<StorageLocker>(-905833192, StorageLocker.OnCopySettingsDelegate);
  }

  protected virtual void OnSpawn()
  {
    this.filteredStorage.FilterChanged();
    if (!Object.op_Inequality((Object) this.nameable, (Object) null) || Util.IsNullOrWhiteSpace(this.lockerName))
      return;
    this.nameable.SetName(this.lockerName);
  }

  protected virtual void OnCleanUp() => this.filteredStorage.CleanUp();

  private void OnCopySettings(object data)
  {
    GameObject gameObject = (GameObject) data;
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return;
    StorageLocker component = gameObject.GetComponent<StorageLocker>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    this.UserMaxCapacity = component.UserMaxCapacity;
  }

  public void UpdateForbiddenTag(Tag game_tag, bool forbidden)
  {
    if (forbidden)
      this.filteredStorage.RemoveForbiddenTag(game_tag);
    else
      this.filteredStorage.AddForbiddenTag(game_tag);
  }

  public virtual float UserMaxCapacity
  {
    get => Mathf.Min(this.userMaxCapacity, ((Component) this).GetComponent<Storage>().capacityKg);
    set
    {
      this.userMaxCapacity = value;
      this.filteredStorage.FilterChanged();
    }
  }

  public float AmountStored => ((Component) this).GetComponent<Storage>().MassStored();

  public float MinCapacity => 0.0f;

  public float MaxCapacity => ((Component) this).GetComponent<Storage>().capacityKg;

  public bool WholeValues => false;

  public LocString CapacityUnits => GameUtil.GetCurrentMassUnit();
}
