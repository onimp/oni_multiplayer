// Decompiled with JetBrains decompiler
// Type: ElementDropper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ElementDropper")]
public class ElementDropper : KMonoBehaviour
{
  [SerializeField]
  public Tag emitTag;
  [SerializeField]
  public float emitMass;
  [SerializeField]
  public Vector3 emitOffset = Vector3.zero;
  [MyCmpGet]
  private Storage storage;
  private static readonly EventSystem.IntraObjectHandler<ElementDropper> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<ElementDropper>((Action<ElementDropper, object>) ((component, data) => component.OnStorageChanged(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<ElementDropper>(-1697596308, ElementDropper.OnStorageChangedDelegate);
  }

  private void OnStorageChanged(object data)
  {
    if ((double) this.storage.GetMassAvailable(this.emitTag) < (double) this.emitMass)
      return;
    this.storage.DropSome(this.emitTag, this.emitMass, offset: this.emitOffset, showInWorldNotification: true);
  }
}
