// Decompiled with JetBrains decompiler
// Type: AtmoSuit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AtmoSuit")]
public class AtmoSuit : KMonoBehaviour
{
  private static readonly EventSystem.IntraObjectHandler<AtmoSuit> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<AtmoSuit>((Action<AtmoSuit, object>) ((component, data) => component.RefreshStatusEffects(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<AtmoSuit>(-1697596308, AtmoSuit.OnStorageChangedDelegate);
  }

  private void RefreshStatusEffects(object data)
  {
    if (Object.op_Equality((Object) this, (Object) null))
      return;
    Equippable component1 = ((Component) this).GetComponent<Equippable>();
    bool flag = ((Component) this).GetComponent<Storage>().Has(GameTags.AnyWater);
    if (!(component1.assignee != null & flag))
      return;
    Ownables soleOwner = component1.assignee.GetSoleOwner();
    if (!Object.op_Inequality((Object) soleOwner, (Object) null))
      return;
    GameObject targetGameObject = ((Component) soleOwner).GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
    if (!Object.op_Implicit((Object) targetGameObject))
      return;
    AssignableSlotInstance slot = ((Component) component1.assignee).GetComponent<Equipment>().GetSlot(component1.slot);
    Effects component2 = targetGameObject.GetComponent<Effects>();
    if (!Object.op_Inequality((Object) component2, (Object) null) || component2.HasEffect("SoiledSuit") || slot.IsUnassigning())
      return;
    component2.Add("SoiledSuit", true);
  }
}
