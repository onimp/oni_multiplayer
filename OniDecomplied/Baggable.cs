// Decompiled with JetBrains decompiler
// Type: Baggable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Baggable")]
public class Baggable : KMonoBehaviour
{
  [SerializeField]
  private KAnimFile minionAnimOverride;
  public bool mustStandOntopOfTrapForPickup;
  [Serialize]
  public bool wrangled;
  public bool useGunForPickup;
  private static readonly EventSystem.IntraObjectHandler<Baggable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Baggable>((Action<Baggable, object>) ((component, data) => component.OnStore(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.minionAnimOverride = Assets.GetAnim(HashedString.op_Implicit("anim_restrain_creature_kanim"));
    Pickupable pickupable = ((Component) this).gameObject.AddOrGet<Pickupable>();
    pickupable.workAnims = new HashedString[2]
    {
      new HashedString("capture"),
      new HashedString("pickup")
    };
    pickupable.workAnimPlayMode = (KAnim.PlayMode) 1;
    pickupable.workingPstComplete = (HashedString[]) null;
    pickupable.workingPstFailed = (HashedString[]) null;
    pickupable.overrideAnims = new KAnimFile[1]
    {
      this.minionAnimOverride
    };
    pickupable.trackOnPickup = false;
    pickupable.useGunforPickup = this.useGunForPickup;
    pickupable.synchronizeAnims = false;
    pickupable.SetWorkTime(3f);
    if (this.mustStandOntopOfTrapForPickup)
      pickupable.SetOffsets(new CellOffset[2]
      {
        null,
        new CellOffset(0, -1)
      });
    this.Subscribe<Baggable>(856640610, Baggable.OnStoreDelegate);
    if (Object.op_Inequality((Object) this.transform.parent, (Object) null))
    {
      if (Object.op_Inequality((Object) ((Component) this.transform.parent).GetComponent<Trap>(), (Object) null))
        ((Component) this).GetComponent<KBatchedAnimController>().enabled = true;
      if (Object.op_Inequality((Object) ((Component) this.transform.parent).GetComponent<EggIncubator>(), (Object) null))
        this.wrangled = true;
    }
    if (!this.wrangled)
      return;
    this.SetWrangled();
  }

  private void OnStore(object data)
  {
    Storage cmp = data as Storage;
    if ((Object.op_Inequality((Object) cmp, (Object) null) ? 1 : (data != null ? ((bool) data ? 1 : 0) : 0)) != 0)
    {
      ((Component) this).gameObject.AddTag(GameTags.Creatures.Bagged);
      if (!Object.op_Implicit((Object) cmp) || !((Component) cmp).IsPrefabID(GameTags.Minion))
        return;
      this.SetVisible(false);
    }
    else
      this.Free();
  }

  private void SetVisible(bool visible)
  {
    KAnimControllerBase component1 = ((Component) this).gameObject.GetComponent<KAnimControllerBase>();
    if (Object.op_Inequality((Object) component1, (Object) null) && component1.enabled != visible)
      component1.enabled = visible;
    KSelectable component2 = ((Component) this).gameObject.GetComponent<KSelectable>();
    if (!Object.op_Inequality((Object) component2, (Object) null) || ((Behaviour) component2).enabled == visible)
      return;
    ((Behaviour) component2).enabled = visible;
  }

  public void SetWrangled()
  {
    this.wrangled = true;
    Navigator component = ((Component) this).GetComponent<Navigator>();
    if (Object.op_Implicit((Object) component) && component.IsValidNavType(NavType.Floor))
      component.SetCurrentNavType(NavType.Floor);
    ((Component) this).gameObject.AddTag(GameTags.Creatures.Bagged);
    ((Component) this).GetComponent<KAnimControllerBase>().Play(HashedString.op_Implicit("trussed"), (KAnim.PlayMode) 0);
  }

  public void Free()
  {
    ((Component) this).gameObject.RemoveTag(GameTags.Creatures.Bagged);
    this.wrangled = false;
    this.SetVisible(true);
  }
}
