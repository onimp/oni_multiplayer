// Decompiled with JetBrains decompiler
// Type: AccessControl
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/AccessControl")]
public class AccessControl : KMonoBehaviour, ISaveLoadable, IGameObjectEffectDescriptor
{
  [MyCmpGet]
  private Operational operational;
  [MyCmpReq]
  private KSelectable selectable;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private bool isTeleporter;
  private int[] registeredBuildingCells;
  [Serialize]
  private List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>> savedPermissions = new List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>>();
  [Serialize]
  private AccessControl.Permission _defaultPermission;
  [Serialize]
  public bool registered = true;
  [Serialize]
  public bool controlEnabled;
  public Door.ControlState overrideAccess;
  private static StatusItem accessControlActive;
  private static readonly EventSystem.IntraObjectHandler<AccessControl> OnControlStateChangedDelegate = new EventSystem.IntraObjectHandler<AccessControl>((Action<AccessControl, object>) ((component, data) => component.OnControlStateChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<AccessControl> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<AccessControl>((Action<AccessControl, object>) ((component, data) => component.OnCopySettings(data)));

  public AccessControl.Permission DefaultPermission
  {
    get => this._defaultPermission;
    set
    {
      this._defaultPermission = value;
      this.SetStatusItem();
      this.SetGridRestrictions((KPrefabID) null, this._defaultPermission);
    }
  }

  public bool Online => true;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (AccessControl.accessControlActive == null)
      AccessControl.accessControlActive = new StatusItem("accessControlActive", (string) BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.NAME, (string) BUILDING.STATUSITEMS.ACCESS_CONTROL.ACTIVE.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
    this.Subscribe<AccessControl>(279163026, AccessControl.OnControlStateChangedDelegate);
    this.Subscribe<AccessControl>(-905833192, AccessControl.OnCopySettingsDelegate);
  }

  private void CheckForBadData()
  {
    List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>> keyValuePairList = new List<KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>>();
    foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> savedPermission in this.savedPermissions)
    {
      if (Object.op_Equality((Object) savedPermission.Key.Get(), (Object) null))
        keyValuePairList.Add(savedPermission);
    }
    foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> keyValuePair in keyValuePairList)
      this.savedPermissions.Remove(keyValuePair);
  }

  protected virtual void OnSpawn()
  {
    this.isTeleporter = Object.op_Inequality((Object) ((Component) this).GetComponent<NavTeleporter>(), (Object) null);
    base.OnSpawn();
    if (this.savedPermissions.Count > 0)
      this.CheckForBadData();
    if (this.registered)
    {
      this.RegisterInGrid(true);
      this.RestorePermissions();
    }
    ListPool<Tuple<MinionAssignablesProxy, AccessControl.Permission>, AccessControl>.PooledList pooledList1 = ListPool<Tuple<MinionAssignablesProxy, AccessControl.Permission>, AccessControl>.Allocate();
    for (int index = this.savedPermissions.Count - 1; index >= 0; --index)
    {
      KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> savedPermission = this.savedPermissions[index];
      KPrefabID kpid = savedPermission.Key.Get();
      if (Object.op_Inequality((Object) kpid, (Object) null))
      {
        MinionIdentity component = ((Component) kpid).GetComponent<MinionIdentity>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          ListPool<Tuple<MinionAssignablesProxy, AccessControl.Permission>, AccessControl>.PooledList pooledList2 = pooledList1;
          MinionAssignablesProxy assignablesProxy = component.assignableProxy.Get();
          savedPermission = this.savedPermissions[index];
          int num = (int) savedPermission.Value;
          Tuple<MinionAssignablesProxy, AccessControl.Permission> tuple = new Tuple<MinionAssignablesProxy, AccessControl.Permission>(assignablesProxy, (AccessControl.Permission) num);
          ((List<Tuple<MinionAssignablesProxy, AccessControl.Permission>>) pooledList2).Add(tuple);
          this.savedPermissions.RemoveAt(index);
          this.ClearGridRestrictions(kpid);
        }
      }
    }
    foreach (Tuple<MinionAssignablesProxy, AccessControl.Permission> tuple in (List<Tuple<MinionAssignablesProxy, AccessControl.Permission>>) pooledList1)
      this.SetPermission(tuple.first, tuple.second);
    pooledList1.Recycle();
    this.SetStatusItem();
  }

  protected virtual void OnCleanUp()
  {
    this.RegisterInGrid(false);
    base.OnCleanUp();
  }

  private void OnControlStateChanged(object data) => this.overrideAccess = (Door.ControlState) data;

  private void OnCopySettings(object data)
  {
    AccessControl component = ((GameObject) data).GetComponent<AccessControl>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.savedPermissions.Clear();
    foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> savedPermission in component.savedPermissions)
    {
      if (Object.op_Inequality((Object) savedPermission.Key.Get(), (Object) null))
        this.SetPermission(((Component) savedPermission.Key.Get()).GetComponent<MinionAssignablesProxy>(), savedPermission.Value);
    }
    this._defaultPermission = component._defaultPermission;
    this.SetGridRestrictions((KPrefabID) null, this.DefaultPermission);
  }

  public void SetRegistered(bool newRegistered)
  {
    if (newRegistered && !this.registered)
    {
      this.RegisterInGrid(true);
      this.RestorePermissions();
    }
    else
    {
      if (newRegistered || !this.registered)
        return;
      this.RegisterInGrid(false);
    }
  }

  public void SetPermission(MinionAssignablesProxy key, AccessControl.Permission permission)
  {
    KPrefabID component = ((Component) key).GetComponent<KPrefabID>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    bool flag = false;
    for (int index = 0; index < this.savedPermissions.Count; ++index)
    {
      if (this.savedPermissions[index].Key.GetId() == component.InstanceID)
      {
        flag = true;
        KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> savedPermission = this.savedPermissions[index];
        this.savedPermissions[index] = new KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>(savedPermission.Key, permission);
        break;
      }
    }
    if (!flag)
      this.savedPermissions.Add(new KeyValuePair<Ref<KPrefabID>, AccessControl.Permission>(new Ref<KPrefabID>(component), permission));
    this.SetStatusItem();
    this.SetGridRestrictions(component, permission);
  }

  private void RestorePermissions()
  {
    this.SetGridRestrictions((KPrefabID) null, this.DefaultPermission);
    foreach (KeyValuePair<Ref<KPrefabID>, AccessControl.Permission> savedPermission in this.savedPermissions)
    {
      KPrefabID kprefabId = savedPermission.Key.Get();
      if (Object.op_Equality((Object) kprefabId, (Object) null))
        DebugUtil.Assert(Object.op_Equality((Object) kprefabId, (Object) null), "Tried to set a duplicant-specific access restriction with a null key! This will result in an invisible default permission!");
      this.SetGridRestrictions(savedPermission.Key.Get(), savedPermission.Value);
    }
  }

  private void RegisterInGrid(bool register)
  {
    Building component1 = ((Component) this).GetComponent<Building>();
    OccupyArea component2 = ((Component) this).GetComponent<OccupyArea>();
    if (Object.op_Equality((Object) component2, (Object) null) && Object.op_Equality((Object) component1, (Object) null))
      return;
    if (register)
    {
      Rotatable component3 = ((Component) this).GetComponent<Rotatable>();
      Grid.Restriction.Orientation orientation = this.isTeleporter ? Grid.Restriction.Orientation.SingleCell : (Object.op_Equality((Object) component3, (Object) null) || component3.GetOrientation() == Orientation.Neutral ? Grid.Restriction.Orientation.Vertical : Grid.Restriction.Orientation.Horizontal);
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        this.registeredBuildingCells = component1.PlacementCells;
        foreach (int registeredBuildingCell in this.registeredBuildingCells)
          Grid.RegisterRestriction(registeredBuildingCell, orientation);
      }
      else
      {
        foreach (CellOffset occupiedCellsOffset in component2.OccupiedCellsOffsets)
          Grid.RegisterRestriction(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) component2), occupiedCellsOffset), orientation);
      }
      if (this.isTeleporter)
        Grid.RegisterRestriction(((Component) this).GetComponent<NavTeleporter>().GetCell(), orientation);
    }
    else
    {
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        if (component1.GetMyWorldId() != (int) ClusterManager.INVALID_WORLD_IDX && this.registeredBuildingCells != null)
        {
          foreach (int registeredBuildingCell in this.registeredBuildingCells)
            Grid.UnregisterRestriction(registeredBuildingCell);
          this.registeredBuildingCells = (int[]) null;
        }
      }
      else
      {
        foreach (CellOffset occupiedCellsOffset in component2.OccupiedCellsOffsets)
          Grid.UnregisterRestriction(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) component2), occupiedCellsOffset));
      }
      if (this.isTeleporter)
      {
        int cell = ((Component) this).GetComponent<NavTeleporter>().GetCell();
        if (cell != Grid.InvalidCell)
          Grid.UnregisterRestriction(cell);
      }
    }
    this.registered = register;
  }

  private void SetGridRestrictions(KPrefabID kpid, AccessControl.Permission permission)
  {
    if (!this.registered || !this.isSpawned)
      return;
    Building component1 = ((Component) this).GetComponent<Building>();
    OccupyArea component2 = ((Component) this).GetComponent<OccupyArea>();
    if (Object.op_Equality((Object) component2, (Object) null) && Object.op_Equality((Object) component1, (Object) null))
      return;
    int minionInstanceID = Object.op_Inequality((Object) kpid, (Object) null) ? kpid.InstanceID : -1;
    Grid.Restriction.Directions directions = (Grid.Restriction.Directions) 0;
    switch (permission)
    {
      case AccessControl.Permission.Both:
        directions = (Grid.Restriction.Directions) 0;
        break;
      case AccessControl.Permission.GoLeft:
        directions = Grid.Restriction.Directions.Right;
        break;
      case AccessControl.Permission.GoRight:
        directions = Grid.Restriction.Directions.Left;
        break;
      case AccessControl.Permission.Neither:
        directions = Grid.Restriction.Directions.Left | Grid.Restriction.Directions.Right;
        break;
    }
    if (this.isTeleporter)
      directions = directions == (Grid.Restriction.Directions) 0 ? (Grid.Restriction.Directions) 0 : Grid.Restriction.Directions.Teleport;
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      foreach (int registeredBuildingCell in this.registeredBuildingCells)
        Grid.SetRestriction(registeredBuildingCell, minionInstanceID, directions);
    }
    else
    {
      foreach (CellOffset occupiedCellsOffset in component2.OccupiedCellsOffsets)
        Grid.SetRestriction(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) component2), occupiedCellsOffset), minionInstanceID, directions);
    }
    if (!this.isTeleporter)
      return;
    Grid.SetRestriction(((Component) this).GetComponent<NavTeleporter>().GetCell(), minionInstanceID, directions);
  }

  private void ClearGridRestrictions(KPrefabID kpid)
  {
    Building component1 = ((Component) this).GetComponent<Building>();
    OccupyArea component2 = ((Component) this).GetComponent<OccupyArea>();
    if (Object.op_Equality((Object) component2, (Object) null) && Object.op_Equality((Object) component1, (Object) null))
      return;
    int minionInstanceID = Object.op_Inequality((Object) kpid, (Object) null) ? kpid.InstanceID : -1;
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      foreach (int registeredBuildingCell in this.registeredBuildingCells)
        Grid.ClearRestriction(registeredBuildingCell, minionInstanceID);
    }
    else
    {
      foreach (CellOffset occupiedCellsOffset in component2.OccupiedCellsOffsets)
        Grid.ClearRestriction(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) component2), occupiedCellsOffset), minionInstanceID);
    }
  }

  public AccessControl.Permission GetPermission(Navigator minion)
  {
    switch (this.overrideAccess)
    {
      case Door.ControlState.Opened:
        return AccessControl.Permission.Both;
      case Door.ControlState.Locked:
        return AccessControl.Permission.Neither;
      default:
        return this.GetSetPermission(this.GetKeyForNavigator(minion));
    }
  }

  private MinionAssignablesProxy GetKeyForNavigator(Navigator minion) => ((Component) minion).GetComponent<MinionIdentity>().assignableProxy.Get();

  public AccessControl.Permission GetSetPermission(MinionAssignablesProxy key) => this.GetSetPermission(((Component) key).GetComponent<KPrefabID>());

  private AccessControl.Permission GetSetPermission(KPrefabID kpid)
  {
    AccessControl.Permission defaultPermission = this.DefaultPermission;
    if (Object.op_Inequality((Object) kpid, (Object) null))
    {
      for (int index = 0; index < this.savedPermissions.Count; ++index)
      {
        if (this.savedPermissions[index].Key.GetId() == kpid.InstanceID)
        {
          defaultPermission = this.savedPermissions[index].Value;
          break;
        }
      }
    }
    return defaultPermission;
  }

  public void ClearPermission(MinionAssignablesProxy key)
  {
    KPrefabID component = ((Component) key).GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      for (int index = 0; index < this.savedPermissions.Count; ++index)
      {
        if (this.savedPermissions[index].Key.GetId() == component.InstanceID)
        {
          this.savedPermissions.RemoveAt(index);
          break;
        }
      }
    }
    this.SetStatusItem();
    this.ClearGridRestrictions(component);
  }

  public bool IsDefaultPermission(MinionAssignablesProxy key)
  {
    bool flag = false;
    KPrefabID component = ((Component) key).GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      for (int index = 0; index < this.savedPermissions.Count; ++index)
      {
        if (this.savedPermissions[index].Key.GetId() == component.InstanceID)
        {
          flag = true;
          break;
        }
      }
    }
    return !flag;
  }

  private void SetStatusItem()
  {
    if (this._defaultPermission != AccessControl.Permission.Both || this.savedPermissions.Count > 0)
      this.selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, AccessControl.accessControlActive);
    else
      this.selectable.SetStatusItem(Db.Get().StatusItemCategories.AccessControl, (StatusItem) null);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.ACCESS_CONTROL, (string) UI.BUILDINGEFFECTS.TOOLTIPS.ACCESS_CONTROL, (Descriptor.DescriptorType) 1);
    descriptors.Add(descriptor);
    return descriptors;
  }

  public enum Permission
  {
    Both,
    GoLeft,
    GoRight,
    Neither,
  }
}
