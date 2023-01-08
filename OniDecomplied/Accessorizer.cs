// Decompiled with JetBrains decompiler
// Type: Accessorizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Accessorizer")]
public class Accessorizer : KMonoBehaviour
{
  [Serialize]
  private List<ResourceRef<Accessory>> accessories = new List<ResourceRef<Accessory>>();
  [MyCmpReq]
  private KAnimControllerBase animController;
  [Serialize]
  private bool ClothingItemsDisabled;
  [Serialize]
  private List<ResourceRef<ClothingItemResource>> clothingItems = new List<ResourceRef<ClothingItemResource>>();

  public List<ResourceRef<Accessory>> GetAccessories() => this.accessories;

  public void SetAccessories(List<ResourceRef<Accessory>> data) => this.accessories = data;

  public KCompBuilder.BodyData bodyData { get; set; }

  public string[] GetClothingItemIds()
  {
    string[] clothingItemIds = new string[this.clothingItems.Count];
    for (int index = 0; index < this.clothingItems.Count; ++index)
      clothingItemIds[index] = this.clothingItems[index].Get().Id;
    return clothingItemIds;
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 30))
    {
      MinionIdentity component = ((Component) this).GetComponent<MinionIdentity>();
      if (Object.op_Inequality((Object) component, (Object) null))
        this.bodyData = Accessorizer.UpdateAccessorySlots(component.nameStringKey, ref this.accessories);
      this.accessories.RemoveAll((Predicate<ResourceRef<Accessory>>) (x => x.Get() == null));
    }
    this.ApplyAccessories();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    MinionIdentity component = ((Component) this).GetComponent<MinionIdentity>();
    if (Object.op_Inequality((Object) component, (Object) null))
      this.bodyData = MinionStartingStats.CreateBodyData(Db.Get().Personalities.Get(component.personalityResourceId));
    this.Subscribe(-448952673, new Action<object>(this.EquippedItem));
    this.Subscribe(-1285462312, new Action<object>(this.UnequippedItem));
  }

  public void EquippedItem(object data)
  {
    KPrefabID kprefabId = data as KPrefabID;
    if (!Object.op_Inequality((Object) kprefabId, (Object) null) || !Object.op_Inequality((Object) ((Component) kprefabId).GetComponent<Equippable>().def.BuildOverride, (Object) null))
      return;
    this.ClothingItemsDisabled = true;
    this.ClearAllItemSlots();
    this.ValidateSlots(false);
  }

  private void UnequippedItem(object data)
  {
    if (!this.ClothingItemsDisabled)
      return;
    KPrefabID kprefabId = data as KPrefabID;
    if (!Object.op_Inequality((Object) kprefabId, (Object) null) || !Object.op_Inequality((Object) ((Component) kprefabId).GetComponent<Equippable>().def.BuildOverride, (Object) null))
      return;
    this.ClearAllItemSlots();
    foreach (ResourceRef<ClothingItemResource> clothingItem in this.clothingItems)
      this.ApplyClothingItem(clothingItem.Get());
    this.ClothingItemsDisabled = false;
    this.ValidateSlots();
  }

  public void AddAccessory(Accessory accessory)
  {
    if (accessory == null)
      return;
    if (Object.op_Equality((Object) this.animController, (Object) null))
      this.animController = ((Component) this).GetComponent<KAnimControllerBase>();
    this.animController.SetSymbolVisiblity(accessory.slot.targetSymbolId, true);
    ((Component) this.animController).GetComponent<SymbolOverrideController>().AddSymbolOverride(HashedString.op_Implicit(accessory.slot.targetSymbolId), accessory.symbol);
    if (this.HasAccessory(accessory))
      return;
    ResourceRef<Accessory> resourceRef = new ResourceRef<Accessory>(accessory);
    if (resourceRef == null)
      return;
    this.accessories.Add(resourceRef);
  }

  public void RemoveAccessory(Accessory accessory)
  {
    this.accessories.RemoveAll((Predicate<ResourceRef<Accessory>>) (x => x.Get() == accessory));
    if (!((Component) this.animController).GetComponent<SymbolOverrideController>().TryRemoveSymbolOverride(HashedString.op_Implicit(accessory.slot.targetSymbolId)))
      return;
    this.animController.SetSymbolVisiblity(accessory.slot.targetSymbolId, false);
  }

  public void ApplyAccessories()
  {
    foreach (ResourceRef<Accessory> accessory1 in this.accessories)
    {
      Accessory accessory2 = accessory1.Get();
      if (accessory2 != null)
        this.AddAccessory(accessory2);
    }
    foreach (AccessorySlot resource in Db.Get().AccessorySlots.resources)
    {
      if (this.GetAccessory(resource) == null)
        this.animController.SetSymbolVisiblity(resource.targetSymbolId, false);
    }
  }

  public static KCompBuilder.BodyData UpdateAccessorySlots(
    string nameString,
    ref List<ResourceRef<Accessory>> accessories)
  {
    accessories.RemoveAll((Predicate<ResourceRef<Accessory>>) (acc => acc.Get() == null));
    Personality fromNameStringKey = Db.Get().Personalities.GetPersonalityFromNameStringKey(nameString);
    if (fromNameStringKey == null)
      return new KCompBuilder.BodyData();
    KCompBuilder.BodyData bodyData = MinionStartingStats.CreateBodyData(fromNameStringKey);
    foreach (AccessorySlot resource1 in Db.Get().AccessorySlots.resources)
    {
      if (resource1.accessories.Count != 0)
      {
        Accessory resource2 = (Accessory) null;
        if (resource1 == Db.Get().AccessorySlots.Body)
          resource2 = resource1.Lookup(bodyData.body);
        else if (resource1 == Db.Get().AccessorySlots.Arm)
          resource2 = resource1.Lookup(bodyData.arms);
        else if (resource1 == Db.Get().AccessorySlots.ArmLower)
          resource2 = resource1.Lookup(bodyData.armslower);
        else if (resource1 == Db.Get().AccessorySlots.ArmLowerSkin)
          resource2 = resource1.Lookup(bodyData.armLowerSkin);
        else if (resource1 == Db.Get().AccessorySlots.ArmUpperSkin)
          resource2 = resource1.Lookup(bodyData.armUpperSkin);
        else if (resource1 == Db.Get().AccessorySlots.LegSkin)
          resource2 = resource1.Lookup(bodyData.legSkin);
        else if (resource1 == Db.Get().AccessorySlots.Leg)
          resource2 = resource1.Lookup(bodyData.legs);
        else if (resource1 == Db.Get().AccessorySlots.Belt)
          resource2 = resource1.Lookup(bodyData.belt);
        else if (resource1 == Db.Get().AccessorySlots.Neck)
          resource2 = resource1.Lookup("neck");
        else if (resource1 == Db.Get().AccessorySlots.Pelvis)
          resource2 = resource1.Lookup(bodyData.pelvis);
        else if (resource1 == Db.Get().AccessorySlots.Foot)
          resource2 = resource1.Lookup(bodyData.foot);
        else if (resource1 == Db.Get().AccessorySlots.Cuff)
          resource2 = resource1.Lookup(bodyData.cuff);
        else if (resource1 == Db.Get().AccessorySlots.Hand)
          resource2 = resource1.Lookup(bodyData.hand);
        if (resource2 != null)
        {
          ResourceRef<Accessory> resourceRef = new ResourceRef<Accessory>(resource2);
          accessories.Add(resourceRef);
        }
      }
    }
    return bodyData;
  }

  public bool HasAccessory(Accessory accessory) => this.accessories.Exists((Predicate<ResourceRef<Accessory>>) (x => x.Get() == accessory));

  public bool HasAccessoryInSlot(AccessorySlot slot) => this.accessories.Exists((Predicate<ResourceRef<Accessory>>) (x => x.Get().slot == slot));

  public Accessory GetAccessory(AccessorySlot slot)
  {
    for (int index = 0; index < this.accessories.Count; ++index)
    {
      if (this.accessories[index].Get() != null && this.accessories[index].Get().slot == slot)
        return this.accessories[index].Get();
    }
    return (Accessory) null;
  }

  public void ApplyClothingItem(ClothingItemResource clothingItem)
  {
    if (!this.clothingItems.Exists((Predicate<ResourceRef<ClothingItemResource>>) (x => HashedString.op_Equality(x.Get().IdHash, clothingItem.IdHash))))
    {
      this.clothingItems.RemoveAll((Predicate<ResourceRef<ClothingItemResource>>) (x => x.Get().Category == clothingItem.Category));
      this.clothingItems.Add(new ResourceRef<ClothingItemResource>(clothingItem));
    }
    KAnim.Build build = clothingItem.AnimFile.GetData().build;
    for (int index = 0; index < build.symbols.Length; ++index)
    {
      string str = HashCache.Get().Get(build.symbols[index].hash);
      AccessorySlot slot = Db.Get().AccessorySlots.Find(KAnimHashedString.op_Implicit(str));
      if (slot != null)
      {
        Accessory accessory1 = this.GetAccessory(slot);
        if (accessory1 != null)
          this.RemoveAccessory(accessory1);
        Accessory accessory2 = slot.Lookup(clothingItem.Id + str);
        if (accessory2 != null)
          this.AddAccessory(accessory2);
      }
    }
  }

  public void RemoveClothingItem(ClothingItemResource clothing_item)
  {
    this.clothingItems.RemoveAll((Predicate<ResourceRef<ClothingItemResource>>) (x => HashedString.op_Equality(x.Get().IdHash, clothing_item.IdHash)));
    KAnim.Build build = clothing_item.AnimFile.GetData().build;
    for (int index = 0; index < build.symbols.Length; ++index)
    {
      string str = HashCache.Get().Get(build.symbols[index].hash);
      AccessorySlot accessorySlot = Db.Get().AccessorySlots.Find(KAnimHashedString.op_Implicit(str));
      if (accessorySlot != null)
      {
        Accessory accessory = accessorySlot.Lookup(clothing_item.Id + str);
        if (accessory != null)
          this.RemoveAccessory(accessory);
      }
    }
    this.ValidateClothingAccessory(clothing_item.Category);
  }

  public void ApplyMinionPersonality(Personality personality)
  {
    this.bodyData = MinionStartingStats.CreateBodyData(personality);
    this.accessories.Clear();
    if (Object.op_Equality((Object) this.animController, (Object) null))
      this.animController = ((Component) this).GetComponent<KAnimControllerBase>();
    string[] strArray = new string[9]
    {
      "snapTo_hat",
      "snapTo_hat_hair",
      "snapTo_goggles",
      "snapTo_headFX",
      "snapTo_neck",
      "snapTo_chest",
      "snapTo_pivot",
      "skirt",
      "necklace"
    };
    foreach (string str in strArray)
    {
      ((Component) this.animController).GetComponent<SymbolOverrideController>().RemoveSymbolOverride(HashedString.op_Implicit(str));
      this.animController.SetSymbolVisiblity(KAnimHashedString.op_Implicit(str), false);
    }
    this.AddAccessory(Db.Get().AccessorySlots.Eyes.Lookup(this.bodyData.eyes));
    this.AddAccessory(Db.Get().AccessorySlots.Hair.Lookup(this.bodyData.hair));
    this.AddAccessory(Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(this.bodyData.hair)));
    this.AddAccessory(Db.Get().AccessorySlots.HeadShape.Lookup(this.bodyData.headShape));
    this.AddAccessory(Db.Get().AccessorySlots.Mouth.Lookup(this.bodyData.mouth));
    this.AddAccessory(Db.Get().AccessorySlots.Body.Lookup(this.bodyData.body));
    this.AddAccessory(Db.Get().AccessorySlots.Arm.Lookup(this.bodyData.arms));
    this.AddAccessory(Db.Get().AccessorySlots.ArmLower.Lookup(this.bodyData.armslower));
    this.AddAccessory(Db.Get().AccessorySlots.Neck.Lookup(this.bodyData.neck));
    this.AddAccessory(Db.Get().AccessorySlots.Pelvis.Lookup(this.bodyData.pelvis));
    this.AddAccessory(Db.Get().AccessorySlots.Leg.Lookup(this.bodyData.legs));
    this.AddAccessory(Db.Get().AccessorySlots.Foot.Lookup(this.bodyData.foot));
    this.AddAccessory(Db.Get().AccessorySlots.Hand.Lookup(this.bodyData.hand));
    this.AddAccessory(Db.Get().AccessorySlots.Cuff.Lookup(this.bodyData.cuff));
    this.AddAccessory(Db.Get().AccessorySlots.Belt.Lookup(this.bodyData.belt));
    this.AddAccessory(Db.Get().AccessorySlots.ArmLowerSkin.Lookup(this.bodyData.armLowerSkin));
    this.AddAccessory(Db.Get().AccessorySlots.ArmUpperSkin.Lookup(this.bodyData.armUpperSkin));
    this.AddAccessory(Db.Get().AccessorySlots.LegSkin.Lookup(this.bodyData.legSkin));
    this.UpdateHairBasedOnHat();
  }

  public void ApplyClothingOutfit(ClothingOutfitResource outfit, bool respectRequiredAccessorySlots = true) => this.ApplyClothingItems(((IEnumerable<string>) outfit.itemsInOutfit).Select<string, ClothingItemResource>((Func<string, ClothingItemResource>) (itemId => Db.Get().Permits.ClothingItems.Get(itemId))), respectRequiredAccessorySlots);

  public void ApplyClothingItems(
    IEnumerable<ClothingItemResource> items,
    bool respectRequiredAccessorySlots = true)
  {
    this.clothingItems.Clear();
    this.ClearAllItemSlots();
    foreach (ClothingItemResource clothingItem in items)
      this.ApplyClothingItem(clothingItem);
    if (respectRequiredAccessorySlots)
      this.ValidateSlots();
    this.UpdateHairBasedOnHat();
  }

  public void UpdateHairBasedOnHat()
  {
    if (!Util.IsNullOrDestroyed((object) this.GetAccessory(Db.Get().AccessorySlots.Hat)))
    {
      this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, false);
      this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, true);
    }
    else
    {
      this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, true);
      this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, false);
      this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hat.targetSymbolId, false);
    }
  }

  private void ValidateSlots(bool check_accessory = true)
  {
    this.ValidateClothingAccessory(PermitCategory.DupeBottoms);
    this.ValidateClothingAccessory(PermitCategory.DupeTops);
    this.ValidateClothingAccessory(PermitCategory.DupeGloves);
    this.ValidateClothingAccessory(PermitCategory.DupeShoes);
    if (check_accessory)
      this.ValidateClothingAccessory(PermitCategory.DupeAccessories);
    MinionResume component = ((Component) this).GetComponent<MinionResume>();
    if (!Object.op_Inequality((Object) component, (Object) null) || Util.IsNullOrWhiteSpace(component.CurrentHat))
      return;
    MinionResume.AddHat(component.CurrentHat, ((Component) this).GetComponent<KBatchedAnimController>());
  }

  private void ValidateClothingAccessory(PermitCategory category)
  {
    if (this.HasClothingAccessory(category))
      return;
    if (category == PermitCategory.DupeBottoms && !this.HasBottomItem())
    {
      this.AddAccessory(Db.Get().AccessorySlots.Leg.Lookup(this.bodyData.legs));
      this.AddAccessory(Db.Get().AccessorySlots.Pelvis.Lookup(this.bodyData.pelvis));
    }
    else if (category == PermitCategory.DupeTops && !this.HasTopItem())
    {
      this.AddAccessory(Db.Get().AccessorySlots.Arm.Lookup(this.bodyData.arms));
      this.AddAccessory(Db.Get().AccessorySlots.ArmLower.Lookup(this.bodyData.armslower));
      this.AddAccessory(Db.Get().AccessorySlots.Body.Lookup(this.bodyData.body));
      this.AddAccessory(Db.Get().AccessorySlots.Neck.Lookup(this.bodyData.neck));
    }
    else if (category == PermitCategory.DupeGloves && !this.HasGloveItem())
    {
      this.AddAccessory(Db.Get().AccessorySlots.Cuff.Lookup(this.bodyData.cuff));
      this.AddAccessory(Db.Get().AccessorySlots.Hand.Lookup(this.bodyData.hand));
    }
    else if (category == PermitCategory.DupeShoes && !this.HasFootItem())
    {
      this.AddAccessory(Db.Get().AccessorySlots.Foot.Lookup(this.bodyData.foot));
    }
    else
    {
      if (category != PermitCategory.DupeAccessories || this.HasAccessoryItem())
        return;
      this.AddAccessory(Db.Get().AccessorySlots.Belt.Lookup(this.bodyData.belt));
    }
  }

  public bool HasClothingAccessory(PermitCategory category) => !this.ClothingItemsDisabled && this.clothingItems.Exists((Predicate<ResourceRef<ClothingItemResource>>) (ci => ci.Get().Category == category));

  private bool HasBottomItem() => this.HasAccessoryInSlot(Db.Get().AccessorySlots.Skirt) || this.HasAccessoryInSlot(Db.Get().AccessorySlots.Pelvis);

  private bool HasTopItem() => this.HasAccessoryInSlot(Db.Get().AccessorySlots.Body);

  private bool HasAccessoryItem() => this.HasAccessoryInSlot(Db.Get().AccessorySlots.Belt) || this.HasAccessoryInSlot(Db.Get().AccessorySlots.Necklace);

  private bool HasFootItem() => this.HasAccessoryInSlot(Db.Get().AccessorySlots.Foot);

  private bool HasGloveItem() => this.HasAccessoryInSlot(Db.Get().AccessorySlots.Hand);

  private void ClearAllItemSlots()
  {
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Hat);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Neck);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Body);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Belt);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Arm);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.ArmLower);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Pelvis);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Leg);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Skirt);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Necklace);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Cuff);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Hand);
    this.RemoveAccessoryFromSlot(Db.Get().AccessorySlots.Foot);
  }

  private void RemoveAccessoryFromSlot(AccessorySlot slot)
  {
    Accessory accessory = this.GetAccessory(slot);
    if (accessory == null)
      return;
    this.RemoveAccessory(accessory);
  }

  public void GetBodySlots(ref KCompBuilder.BodyData fd)
  {
    fd.eyes = HashedString.Invalid;
    fd.hair = HashedString.Invalid;
    fd.headShape = HashedString.Invalid;
    fd.mouth = HashedString.Invalid;
    fd.neck = HashedString.Invalid;
    fd.body = HashedString.Invalid;
    fd.arms = HashedString.Invalid;
    fd.armslower = HashedString.Invalid;
    fd.hat = HashedString.Invalid;
    fd.faceFX = HashedString.Invalid;
    fd.armLowerSkin = HashedString.Invalid;
    fd.armUpperSkin = HashedString.Invalid;
    fd.legSkin = HashedString.Invalid;
    fd.belt = HashedString.Invalid;
    fd.pelvis = HashedString.Invalid;
    fd.foot = HashedString.Invalid;
    fd.skirt = HashedString.Invalid;
    fd.necklace = HashedString.Invalid;
    fd.cuff = HashedString.Invalid;
    fd.hand = HashedString.Invalid;
    for (int index = 0; index < this.accessories.Count; ++index)
    {
      Accessory accessory = this.accessories[index].Get();
      if (accessory != null)
      {
        if (accessory.slot.Id == "Eyes")
          fd.eyes = accessory.IdHash;
        else if (accessory.slot.Id == "Hair")
          fd.hair = accessory.IdHash;
        else if (accessory.slot.Id == "HeadShape")
          fd.headShape = accessory.IdHash;
        else if (accessory.slot.Id == "Mouth")
          fd.mouth = accessory.IdHash;
        else if (accessory.slot.Id == "Neck")
          fd.neck = accessory.IdHash;
        else if (accessory.slot.Id == "Torso")
          fd.body = accessory.IdHash;
        else if (accessory.slot.Id == "Arm_Sleeve")
          fd.arms = accessory.IdHash;
        else if (accessory.slot.Id == "Arm_Lower_Sleeve")
          fd.armslower = accessory.IdHash;
        else if (accessory.slot.Id == "Hat")
          fd.hat = HashedString.Invalid;
        else if (accessory.slot.Id == "FaceEffect")
          fd.faceFX = HashedString.Invalid;
        else if (accessory.slot.Id == "Arm_Lower")
          fd.armLowerSkin = HashedString.op_Implicit(accessory.Id);
        else if (accessory.slot.Id == "Arm_Upper")
          fd.armUpperSkin = HashedString.op_Implicit(accessory.Id);
        else if (accessory.slot.Id == "Leg_Skin")
          fd.legSkin = HashedString.op_Implicit(accessory.Id);
        else if (accessory.slot.Id == "Leg")
          fd.legs = HashedString.op_Implicit(accessory.Id);
        else if (accessory.slot.Id == "Belt")
          fd.belt = accessory.IdHash;
        else if (accessory.slot.Id == "Pelvis")
          fd.pelvis = accessory.IdHash;
        else if (accessory.slot.Id == "Foot")
          fd.foot = accessory.IdHash;
        else if (accessory.slot.Id == "Cuff")
          fd.cuff = accessory.IdHash;
        else if (accessory.slot.Id == "Skirt")
          fd.skirt = accessory.IdHash;
        else if (accessory.slot.Id == "Hand")
          fd.hand = accessory.IdHash;
      }
    }
  }
}
