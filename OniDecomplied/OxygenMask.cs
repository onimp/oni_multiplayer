// Decompiled with JetBrains decompiler
// Type: OxygenMask
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class OxygenMask : KMonoBehaviour, ISim200ms
{
  private static readonly EventSystem.IntraObjectHandler<OxygenMask> OnSuitTankDeltaDelegate = new EventSystem.IntraObjectHandler<OxygenMask>((Action<OxygenMask, object>) ((component, data) => component.CheckOxygenLevels(data)));
  [MyCmpGet]
  private SuitTank suitTank;
  [MyCmpGet]
  private Storage storage;
  private float leakRate = 0.1f;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<OxygenMask>(608245985, OxygenMask.OnSuitTankDeltaDelegate);
  }

  private void CheckOxygenLevels(object data)
  {
    if (!this.suitTank.IsEmpty())
      return;
    Equippable component = ((Component) this).GetComponent<Equippable>();
    if (component.assignee == null)
      return;
    Ownables soleOwner = component.assignee.GetSoleOwner();
    if (!Object.op_Inequality((Object) soleOwner, (Object) null))
      return;
    ((Component) soleOwner).GetComponent<Equipment>().Unequip(component);
  }

  public void Sim200ms(float dt)
  {
    if (((Component) this).GetComponent<Equippable>().assignee == null)
      this.storage.DropSome(this.suitTank.elementTag, Mathf.Min(this.leakRate * dt, this.storage.GetMassAvailable(this.suitTank.elementTag)), true, true, new Vector3());
    if (!this.suitTank.IsEmpty())
      return;
    Util.KDestroyGameObject(((Component) this).gameObject);
  }
}
