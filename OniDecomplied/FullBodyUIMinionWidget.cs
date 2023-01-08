// Decompiled with JetBrains decompiler
// Type: FullBodyUIMinionWidget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FullBodyUIMinionWidget : KMonoBehaviour
{
  [SerializeField]
  private GameObject duplicantAnimAnchor;
  public const float UI_MINION_PORTRAIT_ANIM_SCALE = 0.38f;
  private Tuple<KAnimFileData, int> buildOverrideData;

  public KBatchedAnimController animController { get; private set; }

  protected virtual void OnSpawn() => this.TrySpawnDisplayMinion();

  private void TrySpawnDisplayMinion()
  {
    if (!Object.op_Equality((Object) this.animController, (Object) null))
      return;
    this.animController = Util.KInstantiateUI(Assets.GetPrefab(new Tag("FullMinionUIPortrait")), this.duplicantAnimAnchor.gameObject, false).GetComponent<KBatchedAnimController>();
    ((Component) this.animController).gameObject.SetActive(true);
    this.animController.animScale = 0.38f;
  }

  private void InitializeAnimator()
  {
    this.TrySpawnDisplayMinion();
    this.animController.Queue(HashedString.op_Implicit("idle_default"), (KAnim.PlayMode) 0);
    Accessorizer component = ((Component) this.animController).GetComponent<Accessorizer>();
    for (int index = component.GetAccessories().Count - 1; index >= 0; --index)
      component.RemoveAccessory(component.GetAccessories()[index].Get());
  }

  public void SetDefaultPortraitAnimator()
  {
    MinionIdentity identity = Components.MinionIdentities.Count > 0 ? Components.MinionIdentities[0] : (MinionIdentity) null;
    if (Object.op_Equality((Object) identity, (Object) null))
      return;
    this.InitializeAnimator();
    ((Component) this.animController).GetComponent<Accessorizer>().ApplyMinionPersonality(Db.Get().Personalities.Get(identity.personalityResourceId));
    Accessorizer component = ((Component) identity).GetComponent<Accessorizer>();
    KAnim.Build.Symbol hair_symbol = (KAnim.Build.Symbol) null;
    KAnim.Build.Symbol hat_hair_symbol = (KAnim.Build.Symbol) null;
    if (Object.op_Implicit((Object) component))
    {
      hair_symbol = component.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
      hat_hair_symbol = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
    }
    this.UpdateHatOverride((string) null, hair_symbol, hat_hair_symbol);
    this.UpdateClothingOverride(((Component) this.animController).GetComponent<SymbolOverrideController>(), identity, (StoredMinionIdentity) null);
  }

  public void SetPortraitAnimator(IAssignableIdentity assignableIdentity)
  {
    if (assignableIdentity == null || assignableIdentity.IsNull())
    {
      this.SetDefaultPortraitAnimator();
    }
    else
    {
      this.InitializeAnimator();
      string current_hat = "";
      MinionIdentity minionIdentity;
      StoredMinionIdentity storedMinionIdentity;
      this.GetMinionIdentity(assignableIdentity, out minionIdentity, out storedMinionIdentity);
      Accessorizer component1 = ((Component) this.animController).GetComponent<Accessorizer>();
      KAnim.Build.Symbol hair_symbol = (KAnim.Build.Symbol) null;
      KAnim.Build.Symbol hat_hair_symbol = (KAnim.Build.Symbol) null;
      if (Object.op_Inequality((Object) minionIdentity, (Object) null))
      {
        Accessorizer component2 = ((Component) minionIdentity).GetComponent<Accessorizer>();
        foreach (ResourceRef<Accessory> accessory in component2.GetAccessories())
          component1.AddAccessory(accessory.Get());
        current_hat = ((Component) minionIdentity).GetComponent<MinionResume>().CurrentHat;
        hair_symbol = component2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
        hat_hair_symbol = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(component2.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
      }
      else if (Object.op_Inequality((Object) storedMinionIdentity, (Object) null))
      {
        foreach (ResourceRef<Accessory> accessory in storedMinionIdentity.accessories)
          component1.AddAccessory(accessory.Get());
        current_hat = storedMinionIdentity.currentHat;
        hair_symbol = storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol;
        hat_hair_symbol = Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(storedMinionIdentity.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol;
      }
      this.UpdateHatOverride(current_hat, hair_symbol, hat_hair_symbol);
      this.UpdateClothingOverride(((Component) this.animController).GetComponent<SymbolOverrideController>(), minionIdentity, storedMinionIdentity);
    }
  }

  private void UpdateHatOverride(
    string current_hat,
    KAnim.Build.Symbol hair_symbol,
    KAnim.Build.Symbol hat_hair_symbol)
  {
    this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hat.targetSymbolId, !string.IsNullOrEmpty(current_hat));
    this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.Hair.targetSymbolId, string.IsNullOrEmpty(current_hat));
    this.animController.SetSymbolVisiblity(Db.Get().AccessorySlots.HatHair.targetSymbolId, !string.IsNullOrEmpty(current_hat));
    SymbolOverrideController component = ((Component) this.animController).GetComponent<SymbolOverrideController>();
    component.AddSymbolOverride(HashedString.op_Implicit("snapto_hair_always"), hair_symbol, 1);
    component.AddSymbolOverride(HashedString.op_Implicit(Db.Get().AccessorySlots.HatHair.targetSymbolId), hat_hair_symbol, 1);
  }

  private void UpdateClothingOverride(
    SymbolOverrideController symbolOverrideController,
    MinionIdentity identity,
    StoredMinionIdentity storedMinionIdentity)
  {
    Equipment equipment = (Equipment) null;
    if (Object.op_Inequality((Object) identity, (Object) null))
      equipment = ((Component) identity.assignableProxy.Get()).GetComponent<Equipment>();
    else if (Object.op_Inequality((Object) storedMinionIdentity, (Object) null))
      equipment = ((Component) storedMinionIdentity.assignableProxy.Get()).GetComponent<Equipment>();
    if (this.buildOverrideData != null)
    {
      symbolOverrideController.RemoveBuildOverride(this.buildOverrideData.first, this.buildOverrideData.second);
      this.buildOverrideData = (Tuple<KAnimFileData, int>) null;
    }
    AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Outfit);
    if (Object.op_Inequality((Object) slot.assignable, (Object) null))
    {
      Equippable component = ((Component) slot.assignable).GetComponent<Equippable>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      this.UpdateClothingOverride(component.GetBuildOverride().GetData(), component.def.BuildOverridePriority);
    }
    else
    {
      if (!Object.op_Inequality((Object) equipment.GetSlot(Db.Get().AssignableSlots.Suit).assignable, (Object) null))
        return;
      this.animController.SetSymbolVisiblity(KAnimHashedString.op_Implicit("belt"), true);
    }
  }

  public void UpdateClothingOverride(KAnimFileData clothingData, int priority = 4)
  {
    SymbolOverrideController component = ((Component) this.animController).GetComponent<SymbolOverrideController>();
    if (this.buildOverrideData != null)
    {
      component.RemoveBuildOverride(this.buildOverrideData.first, this.buildOverrideData.second);
      this.buildOverrideData = (Tuple<KAnimFileData, int>) null;
    }
    this.buildOverrideData = new Tuple<KAnimFileData, int>(clothingData, priority);
    component.AddBuildOverride(this.buildOverrideData.first, this.buildOverrideData.second);
    bool is_visible = this.buildOverrideData.first.build.GetSymbol(KAnimHashedString.op_Implicit("belt")) != null;
    this.animController.SetSymbolVisiblity(KAnimHashedString.op_Implicit("belt"), is_visible);
  }

  private void GetMinionIdentity(
    IAssignableIdentity assignableIdentity,
    out MinionIdentity minionIdentity,
    out StoredMinionIdentity storedMinionIdentity)
  {
    if (assignableIdentity is MinionAssignablesProxy)
    {
      minionIdentity = ((MinionAssignablesProxy) assignableIdentity).GetTargetGameObject().GetComponent<MinionIdentity>();
      storedMinionIdentity = ((MinionAssignablesProxy) assignableIdentity).GetTargetGameObject().GetComponent<StoredMinionIdentity>();
    }
    else
    {
      minionIdentity = assignableIdentity as MinionIdentity;
      storedMinionIdentity = assignableIdentity as StoredMinionIdentity;
    }
  }
}
