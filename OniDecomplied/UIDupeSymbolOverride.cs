// Decompiled with JetBrains decompiler
// Type: UIDupeSymbolOverride
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using UnityEngine;

[RequireComponent(typeof (SymbolOverrideController))]
public class UIDupeSymbolOverride : MonoBehaviour
{
  private KBatchedAnimController animController;
  private AccessorySlots slots;
  private SymbolOverrideController symbolOverrideController;

  public void Apply(MinionIdentity minionIdentity)
  {
    if (this.slots == null)
      this.slots = new AccessorySlots((ResourceSet) null);
    if (Object.op_Equality((Object) this.symbolOverrideController, (Object) null))
      this.symbolOverrideController = ((Component) this).GetComponent<SymbolOverrideController>();
    if (Object.op_Equality((Object) this.animController, (Object) null))
      this.animController = ((Component) this).GetComponent<KBatchedAnimController>();
    Personality fromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(minionIdentity.nameStringKey);
    DebugUtil.DevAssert(fromNameStringKey != null, "Personality is not found", (Object) null);
    KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(fromNameStringKey);
    this.symbolOverrideController.RemoveAllSymbolOverrides();
    this.SetAccessory(this.animController, this.slots.Hair.Lookup(bodyData.hair));
    this.SetAccessory(this.animController, this.slots.HatHair.Lookup("hat_" + HashCache.Get().Get(bodyData.hair)));
    this.SetAccessory(this.animController, this.slots.Eyes.Lookup(bodyData.eyes));
    this.SetAccessory(this.animController, this.slots.HeadShape.Lookup(bodyData.headShape));
    this.SetAccessory(this.animController, this.slots.Mouth.Lookup(bodyData.mouth));
    this.SetAccessory(this.animController, this.slots.Neck.Lookup(bodyData.neck));
    this.SetAccessory(this.animController, this.slots.Body.Lookup(bodyData.body));
    this.SetAccessory(this.animController, this.slots.Leg.Lookup(bodyData.legs));
    this.SetAccessory(this.animController, this.slots.Arm.Lookup(bodyData.arms));
    this.SetAccessory(this.animController, this.slots.ArmLower.Lookup(bodyData.armslower));
    this.SetAccessory(this.animController, this.slots.Pelvis.Lookup(bodyData.pelvis));
    this.SetAccessory(this.animController, this.slots.Belt.Lookup(bodyData.belt));
    this.SetAccessory(this.animController, this.slots.Foot.Lookup(bodyData.foot));
    this.SetAccessory(this.animController, this.slots.Cuff.Lookup(bodyData.cuff));
    this.SetAccessory(this.animController, this.slots.Hand.Lookup(bodyData.hand));
  }

  private KAnimHashedString SetAccessory(KBatchedAnimController minion, Accessory accessory)
  {
    if (accessory == null)
      return KAnimHashedString.op_Implicit(HashedString.Invalid);
    this.symbolOverrideController.TryRemoveSymbolOverride(HashedString.op_Implicit(accessory.slot.targetSymbolId));
    this.symbolOverrideController.AddSymbolOverride(HashedString.op_Implicit(accessory.slot.targetSymbolId), accessory.symbol);
    minion.SetSymbolVisiblity(accessory.slot.targetSymbolId, true);
    return accessory.slot.targetSymbolId;
  }
}
