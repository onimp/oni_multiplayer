// Decompiled with JetBrains decompiler
// Type: OperationalControlledSwitch
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;

[SerializationConfig]
public class OperationalControlledSwitch : CircuitSwitch
{
  private static readonly EventSystem.IntraObjectHandler<OperationalControlledSwitch> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<OperationalControlledSwitch>((Action<OperationalControlledSwitch, object>) ((component, data) => component.OnOperationalChanged(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.manuallyControlled = false;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<OperationalControlledSwitch>(-592767678, OperationalControlledSwitch.OnOperationalChangedDelegate);
  }

  private void OnOperationalChanged(object data) => this.SetState((bool) data);
}
