// Decompiled with JetBrains decompiler
// Type: InvalidPortReporter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class InvalidPortReporter : KMonoBehaviour
{
  public static readonly Operational.Flag portsNotOverlapping = new Operational.Flag("ports_not_overlapping", Operational.Flag.Type.Functional);
  private static readonly EventSystem.IntraObjectHandler<InvalidPortReporter> OnTagsChangedDelegate = new EventSystem.IntraObjectHandler<InvalidPortReporter>((Action<InvalidPortReporter, object>) ((component, data) => component.OnTagsChanged(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.OnTagsChanged((object) null);
    this.Subscribe<InvalidPortReporter>(-1582839653, InvalidPortReporter.OnTagsChangedDelegate);
  }

  protected virtual void OnCleanUp() => base.OnCleanUp();

  private void OnTagsChanged(object data)
  {
    bool on = ((Component) this).gameObject.HasTag(GameTags.HasInvalidPorts);
    Operational component1 = ((Component) this).GetComponent<Operational>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      component1.SetFlag(InvalidPortReporter.portsNotOverlapping, !on);
    KSelectable component2 = ((Component) this).GetComponent<KSelectable>();
    if (!Object.op_Inequality((Object) component2, (Object) null))
      return;
    component2.ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidPortOverlap, on, (object) ((Component) this).gameObject);
  }
}
