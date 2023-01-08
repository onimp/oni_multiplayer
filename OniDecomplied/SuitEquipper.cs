// Decompiled with JetBrains decompiler
// Type: SuitEquipper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SuitEquipper")]
public class SuitEquipper : KMonoBehaviour
{
  private static readonly EventSystem.IntraObjectHandler<SuitEquipper> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<SuitEquipper>((Action<SuitEquipper, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<SuitEquipper>(493375141, SuitEquipper.OnRefreshUserMenuDelegate);
  }

  private void OnRefreshUserMenu(object data)
  {
    foreach (EquipmentSlotInstance slot in ((Component) this).GetComponent<MinionIdentity>().GetEquipment().Slots)
    {
      Equippable equippable = slot.assignable as Equippable;
      if (Object.op_Implicit((Object) equippable) && equippable.unequippable)
        Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("iconDown", string.Format((string) UI.USERMENUACTIONS.UNEQUIP.NAME, (object) equippable.def.GenericName), (System.Action) (() => equippable.Unassign())), 2f);
    }
  }

  public Equippable IsWearingAirtightSuit()
  {
    Equippable equippable = (Equippable) null;
    foreach (AssignableSlotInstance slot in ((Component) this).GetComponent<MinionIdentity>().GetEquipment().Slots)
    {
      Equippable assignable = slot.assignable as Equippable;
      if (Object.op_Implicit((Object) assignable) && ((Component) assignable).GetComponent<KPrefabID>().HasTag(GameTags.AirtightSuit) && assignable.isEquipped)
      {
        equippable = assignable;
        break;
      }
    }
    return equippable;
  }
}
