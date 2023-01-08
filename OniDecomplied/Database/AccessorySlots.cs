// Decompiled with JetBrains decompiler
// Type: Database.AccessorySlots
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class AccessorySlots : ResourceSet<AccessorySlot>
  {
    public AccessorySlot Eyes;
    public AccessorySlot Hair;
    public AccessorySlot HeadShape;
    public AccessorySlot Mouth;
    public AccessorySlot Body;
    public AccessorySlot Arm;
    public AccessorySlot ArmLower;
    public AccessorySlot Hat;
    public AccessorySlot HatHair;
    public AccessorySlot HeadEffects;
    public AccessorySlot Belt;
    public AccessorySlot Neck;
    public AccessorySlot Pelvis;
    public AccessorySlot Leg;
    public AccessorySlot Foot;
    public AccessorySlot Skirt;
    public AccessorySlot Necklace;
    public AccessorySlot Cuff;
    public AccessorySlot Hand;
    public AccessorySlot ArmLowerSkin;
    public AccessorySlot ArmUpperSkin;
    public AccessorySlot LegSkin;

    public AccessorySlots(ResourceSet parent)
      : base(nameof (AccessorySlots), parent)
    {
      parent = (ResourceSet) Db.Get().Accessories;
      KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("head_swap_kanim"));
      KAnimFile anim2 = Assets.GetAnim(HashedString.op_Implicit("body_comp_default_kanim"));
      KAnimFile anim3 = Assets.GetAnim(HashedString.op_Implicit("body_swap_kanim"));
      KAnimFile anim4 = Assets.GetAnim(HashedString.op_Implicit("hair_swap_kanim"));
      KAnimFile anim5 = Assets.GetAnim(HashedString.op_Implicit("hat_swap_kanim"));
      this.Eyes = new AccessorySlot(nameof (Eyes), (ResourceSet) this, anim1, true);
      this.Hair = new AccessorySlot(nameof (Hair), (ResourceSet) this, anim4, true);
      this.HeadShape = new AccessorySlot(nameof (HeadShape), (ResourceSet) this, anim1, true);
      this.Mouth = new AccessorySlot(nameof (Mouth), (ResourceSet) this, anim1, true);
      this.Hat = new AccessorySlot(nameof (Hat), (ResourceSet) this, anim5);
      this.HatHair = new AccessorySlot("Hat_Hair", (ResourceSet) this, anim4);
      this.HeadEffects = new AccessorySlot("HeadFX", (ResourceSet) this, anim1);
      this.Body = new AccessorySlot("Torso", (ResourceSet) this, new KAnimHashedString("torso"), anim3, true);
      this.Arm = new AccessorySlot("Arm_Sleeve", (ResourceSet) this, new KAnimHashedString("arm_sleeve"), anim3);
      this.ArmLower = new AccessorySlot("Arm_Lower_Sleeve", (ResourceSet) this, new KAnimHashedString("arm_lower_sleeve"), anim3);
      this.Belt = new AccessorySlot(nameof (Belt), (ResourceSet) this, new KAnimHashedString("belt"), anim2);
      this.Neck = new AccessorySlot(nameof (Neck), (ResourceSet) this, new KAnimHashedString("neck"), anim2);
      this.Pelvis = new AccessorySlot(nameof (Pelvis), (ResourceSet) this, new KAnimHashedString("pelvis"), anim2, true);
      this.Foot = new AccessorySlot(nameof (Foot), (ResourceSet) this, new KAnimHashedString("foot"), anim2, true, Assets.GetAnim(HashedString.op_Implicit("shoes_basic_black_kanim")));
      this.Leg = new AccessorySlot(nameof (Leg), (ResourceSet) this, new KAnimHashedString("leg"), anim2);
      this.Necklace = new AccessorySlot(nameof (Necklace), (ResourceSet) this, new KAnimHashedString("necklace"), anim2);
      this.Cuff = new AccessorySlot(nameof (Cuff), (ResourceSet) this, new KAnimHashedString("cuff"), anim2, true);
      this.Skirt = new AccessorySlot(nameof (Skirt), (ResourceSet) this, new KAnimHashedString("skirt"), anim3);
      this.ArmLowerSkin = new AccessorySlot("Arm_Lower", (ResourceSet) this, new KAnimHashedString("arm_lower"), anim3, true);
      this.ArmUpperSkin = new AccessorySlot("Arm_Upper", (ResourceSet) this, new KAnimHashedString("arm_upper"), anim3, true);
      this.LegSkin = new AccessorySlot("Leg_Skin", (ResourceSet) this, new KAnimHashedString("leg_skin"), anim3, true);
      this.Hand = new AccessorySlot(nameof (Hand), (ResourceSet) this, new KAnimHashedString("hand_paint"), anim2, true);
      foreach (AccessorySlot resource in this.resources)
        resource.AddAccessories(resource.AnimFile, parent);
    }

    public AccessorySlot Find(KAnimHashedString symbol_name)
    {
      foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
      {
        if (KAnimHashedString.op_Equality(symbol_name, resource.targetSymbolId))
          return resource;
      }
      return (AccessorySlot) null;
    }
  }
}
