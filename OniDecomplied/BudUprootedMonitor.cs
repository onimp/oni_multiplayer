// Decompiled with JetBrains decompiler
// Type: BudUprootedMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/BudUprootedMonitor")]
public class BudUprootedMonitor : KMonoBehaviour
{
  [Serialize]
  public bool canBeUprooted = true;
  [Serialize]
  private bool uprooted;
  public Ref<KPrefabID> parentObject = new Ref<KPrefabID>();
  private HandleVector<int>.Handle partitionerEntry;
  private static readonly EventSystem.IntraObjectHandler<BudUprootedMonitor> OnUprootedDelegate = new EventSystem.IntraObjectHandler<BudUprootedMonitor>((Action<BudUprootedMonitor, object>) ((component, data) =>
  {
    if (component.uprooted)
      return;
    ((Component) component).GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
    component.uprooted = true;
    component.Trigger(-216549700, (object) null);
  }));

  public bool IsUprooted => this.uprooted || ((Component) this).GetComponent<KPrefabID>().HasTag(GameTags.Uprooted);

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<BudUprootedMonitor>(-216549700, BudUprootedMonitor.OnUprootedDelegate);
  }

  public void SetParentObject(KPrefabID id)
  {
    this.parentObject = new Ref<KPrefabID>(id);
    this.Subscribe(((Component) id).gameObject, 1969584890, new Action<object>(this.OnLoseParent));
  }

  private void OnLoseParent(object obj)
  {
    if (this.uprooted || this.isNull)
      return;
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
    this.uprooted = true;
    this.Trigger(-216549700, (object) null);
  }

  protected virtual void OnCleanUp() => base.OnCleanUp();

  public static bool IsObjectUprooted(GameObject plant)
  {
    BudUprootedMonitor component = plant.GetComponent<BudUprootedMonitor>();
    return !Object.op_Equality((Object) component, (Object) null) && component.IsUprooted;
  }
}
