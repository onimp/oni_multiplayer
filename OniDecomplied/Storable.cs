// Decompiled with JetBrains decompiler
// Type: Storable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class Storable : KMonoBehaviour
{
  private static readonly EventSystem.IntraObjectHandler<Storable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Storable>((Action<Storable, object>) ((component, data) => component.OnStore(data)));
  private static readonly EventSystem.IntraObjectHandler<Storable> RefreshStorageTagsDelegate = new EventSystem.IntraObjectHandler<Storable>((Action<Storable, object>) ((component, data) => component.RefreshStorageTags(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Storable>(856640610, Storable.OnStoreDelegate);
    this.Subscribe<Storable>(-778359855, Storable.RefreshStorageTagsDelegate);
  }

  public void OnStore(object data) => this.RefreshStorageTags(data);

  private void RefreshStorageTags(object data = null)
  {
    bool flag = data is Storage || data != null && (bool) data;
    Storage storage = (Storage) data;
    if (Object.op_Inequality((Object) storage, (Object) null) && Object.op_Equality((Object) ((Component) storage).gameObject, (Object) ((Component) this).gameObject))
      return;
    KPrefabID component1 = ((Component) this).GetComponent<KPrefabID>();
    SaveLoadRoot component2 = ((Component) this).GetComponent<SaveLoadRoot>();
    KSelectable component3 = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Implicit((Object) component3))
      component3.IsSelectable = !flag;
    if (flag)
    {
      component1.AddTag(GameTags.Stored, false);
      if ((Object.op_Equality((Object) storage, (Object) null) ? 1 : (!storage.allowItemRemoval ? 1 : 0)) != 0)
        component1.AddTag(GameTags.StoredPrivate, false);
      else
        component1.RemoveTag(GameTags.StoredPrivate);
      if (!Object.op_Inequality((Object) component2, (Object) null))
        return;
      component2.SetRegistered(false);
    }
    else
    {
      component1.RemoveTag(GameTags.Stored);
      component1.RemoveTag(GameTags.StoredPrivate);
      if (!Object.op_Inequality((Object) component2, (Object) null))
        return;
      component2.SetRegistered(true);
    }
  }
}
