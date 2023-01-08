// Decompiled with JetBrains decompiler
// Type: Trappable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Trappable")]
public class Trappable : KMonoBehaviour, IGameObjectEffectDescriptor
{
  private bool registered;
  private static readonly EventSystem.IntraObjectHandler<Trappable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Trappable>((Action<Trappable, object>) ((component, data) => component.OnStore(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Register();
    this.OnCellChange();
  }

  protected virtual void OnCleanUp()
  {
    this.Unregister();
    base.OnCleanUp();
  }

  private void OnCellChange() => GameScenePartitioner.Instance.TriggerEvent(Grid.PosToCell((KMonoBehaviour) this), GameScenePartitioner.Instance.trapsLayer, (object) this);

  protected virtual void OnCmpEnable()
  {
    base.OnCmpEnable();
    this.Register();
  }

  protected virtual void OnCmpDisable()
  {
    this.Unregister();
    base.OnCmpDisable();
  }

  private void Register()
  {
    if (this.registered)
      return;
    this.Subscribe<Trappable>(856640610, Trappable.OnStoreDelegate);
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "Trappable.Register");
    this.registered = true;
  }

  private void Unregister()
  {
    if (!this.registered)
      return;
    this.Unsubscribe<Trappable>(856640610, Trappable.OnStoreDelegate, false);
    Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange));
    this.registered = false;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor((string) UI.BUILDINGEFFECTS.CAPTURE_METHOD_TRAP, (string) UI.BUILDINGEFFECTS.TOOLTIPS.CAPTURE_METHOD_TRAP, (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  public void OnStore(object data)
  {
    Storage storage = data as Storage;
    if (Object.op_Implicit(Object.op_Implicit((Object) storage) ? (Object) ((Component) storage).GetComponent<Trap>() : (Object) null))
      ((Component) this).gameObject.AddTag(GameTags.Trapped);
    else
      ((Component) this).gameObject.RemoveTag(GameTags.Trapped);
  }
}
