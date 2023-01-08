// Decompiled with JetBrains decompiler
// Type: Compostable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Compostable")]
public class Compostable : KMonoBehaviour
{
  [SerializeField]
  public bool isMarkedForCompost;
  public GameObject originalPrefab;
  public GameObject compostPrefab;
  private static readonly EventSystem.IntraObjectHandler<Compostable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Compostable>((Action<Compostable, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<Compostable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Compostable>((Action<Compostable, object>) ((component, data) => component.OnStore(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.isMarkedForCompost = ((Component) this).GetComponent<KPrefabID>().HasTag(GameTags.Compostable);
    if (this.isMarkedForCompost)
      this.MarkForCompost();
    this.Subscribe<Compostable>(493375141, Compostable.OnRefreshUserMenuDelegate);
    this.Subscribe<Compostable>(856640610, Compostable.OnStoreDelegate);
  }

  private void MarkForCompost(bool force = false)
  {
    this.RefreshStatusItem();
    Storage storage = ((Component) this).GetComponent<Pickupable>().storage;
    if (!Object.op_Inequality((Object) storage, (Object) null))
      return;
    storage.Drop(((Component) this).gameObject, true);
  }

  private void OnToggleCompost()
  {
    if (!this.isMarkedForCompost)
    {
      Pickupable component = ((Component) this).GetComponent<Pickupable>();
      if (Object.op_Inequality((Object) component.storage, (Object) null))
        component.storage.Drop(((Component) this).gameObject, true);
      Pickupable pickupable = EntitySplitter.Split(component, component.TotalAmount, this.compostPrefab);
      if (!Object.op_Inequality((Object) pickupable, (Object) null))
        return;
      SelectTool.Instance.SelectNextFrame(((Component) pickupable).GetComponent<KSelectable>(), true);
    }
    else
    {
      Pickupable component = ((Component) this).GetComponent<Pickupable>();
      Pickupable pickupable = EntitySplitter.Split(component, component.TotalAmount, this.originalPrefab);
      SelectTool.Instance.SelectNextFrame(((Component) pickupable).GetComponent<KSelectable>(), true);
    }
  }

  private void RefreshStatusItem()
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForCompost);
    component.RemoveStatusItem(Db.Get().MiscStatusItems.MarkedForCompostInStorage);
    if (!this.isMarkedForCompost)
      return;
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<Pickupable>(), (Object) null) && Object.op_Equality((Object) ((Component) this).GetComponent<Pickupable>().storage, (Object) null))
      component.AddStatusItem(Db.Get().MiscStatusItems.MarkedForCompost);
    else
      component.AddStatusItem(Db.Get().MiscStatusItems.MarkedForCompostInStorage);
  }

  private void OnStore(object data) => this.RefreshStatusItem();

  private void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(((Component) this).gameObject, this.isMarkedForCompost ? new KIconButtonMenu.ButtonInfo("action_compost", (string) UI.USERMENUACTIONS.COMPOST.NAME_OFF, new System.Action(this.OnToggleCompost), tooltipText: ((string) UI.USERMENUACTIONS.COMPOST.TOOLTIP_OFF)) : new KIconButtonMenu.ButtonInfo("action_compost", (string) UI.USERMENUACTIONS.COMPOST.NAME, new System.Action(this.OnToggleCompost), tooltipText: ((string) UI.USERMENUACTIONS.COMPOST.TOOLTIP)));
}
