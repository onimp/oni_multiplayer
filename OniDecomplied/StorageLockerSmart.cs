// Decompiled with JetBrains decompiler
// Type: StorageLockerSmart
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class StorageLockerSmart : StorageLocker
{
  [MyCmpGet]
  private LogicPorts ports;
  [MyCmpGet]
  private Operational operational;
  private static readonly EventSystem.IntraObjectHandler<StorageLockerSmart> UpdateLogicCircuitCBDelegate = new EventSystem.IntraObjectHandler<StorageLockerSmart>((Action<StorageLockerSmart, object>) ((component, data) => component.UpdateLogicCircuitCB(data)));

  protected override void OnPrefabInit() => this.Initialize(true);

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.ports = ((Component) this).gameObject.GetComponent<LogicPorts>();
    this.Subscribe<StorageLockerSmart>(-1697596308, StorageLockerSmart.UpdateLogicCircuitCBDelegate);
    this.Subscribe<StorageLockerSmart>(-592767678, StorageLockerSmart.UpdateLogicCircuitCBDelegate);
    this.UpdateLogicAndActiveState();
  }

  private void UpdateLogicCircuitCB(object data) => this.UpdateLogicAndActiveState();

  private void UpdateLogicAndActiveState()
  {
    int num1 = this.filteredStorage.IsFull() ? 1 : 0;
    bool isOperational = this.operational.IsOperational;
    int num2 = isOperational ? 1 : 0;
    bool on = (num1 & num2) != 0;
    this.ports.SendSignal(FilteredStorage.FULL_PORT_ID, on ? 1 : 0);
    this.filteredStorage.SetLogicMeter(on);
    this.operational.SetActive(isOperational);
  }

  public override float UserMaxCapacity
  {
    get => base.UserMaxCapacity;
    set
    {
      base.UserMaxCapacity = value;
      this.UpdateLogicAndActiveState();
    }
  }
}
