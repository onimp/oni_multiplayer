// Decompiled with JetBrains decompiler
// Type: Equipment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using KSerialization;
using System;
using TUNING;
using UnityEngine;

[SerializationConfig]
public class Equipment : Assignables
{
  private SchedulerHandle refreshHandle;
  private static readonly EventSystem.IntraObjectHandler<Equipment> SetDestroyedTrueDelegate = new EventSystem.IntraObjectHandler<Equipment>((Action<Equipment, object>) ((component, data) => component.destroyed = true));

  public bool destroyed { get; private set; }

  public GameObject GetTargetGameObject()
  {
    MinionAssignablesProxy assignableIdentity = (MinionAssignablesProxy) this.GetAssignableIdentity();
    return Object.op_Implicit((Object) assignableIdentity) ? assignableIdentity.GetTargetGameObject() : (GameObject) null;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Components.Equipment.Add(this);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<Equipment>(1502190696, Equipment.SetDestroyedTrueDelegate);
    this.Subscribe<Equipment>(1969584890, Equipment.SetDestroyedTrueDelegate);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    this.refreshHandle.ClearScheduler();
    Components.Equipment.Remove(this);
  }

  public void Equip(Equippable equippable)
  {
    GameObject targetGameObject = this.GetTargetGameObject();
    KBatchedAnimController component1 = targetGameObject.GetComponent<KBatchedAnimController>();
    bool flag = Object.op_Equality((Object) component1, (Object) null);
    if (!flag)
    {
      PrimaryElement component2 = ((Component) equippable).GetComponent<PrimaryElement>();
      SimUtil.DiseaseInfo invalid1 = SimUtil.DiseaseInfo.Invalid with
      {
        idx = component2.DiseaseIdx,
        count = (int) ((double) component2.DiseaseCount * 0.33000001311302185)
      };
      PrimaryElement component3 = targetGameObject.GetComponent<PrimaryElement>();
      SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid with
      {
        idx = component3.DiseaseIdx,
        count = (int) ((double) component3.DiseaseCount * 0.33000001311302185)
      };
      component3.ModifyDiseaseCount(-invalid2.count, "Equipment.Equip");
      component2.ModifyDiseaseCount(-invalid1.count, "Equipment.Equip");
      if (invalid2.count > 0)
        component2.AddDisease(invalid2.idx, invalid2.count, "Equipment.Equip");
      if (invalid1.count > 0)
        component3.AddDisease(invalid1.idx, invalid1.count, "Equipment.Equip");
    }
    AssignableSlotInstance slot = this.GetSlot(equippable.slot);
    slot.Assign((Assignable) equippable);
    Debug.Assert(Object.op_Implicit((Object) targetGameObject), (object) "GetTargetGameObject returned null in Equip");
    EventExtensions.Trigger(targetGameObject, -448952673, (object) ((Component) equippable).GetComponent<KPrefabID>());
    equippable.Trigger(-1617557748, (object) this);
    Attributes attributes = targetGameObject.GetAttributes();
    if (attributes != null)
    {
      foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
        attributes.Add(attributeModifier);
    }
    SnapOn component4 = targetGameObject.GetComponent<SnapOn>();
    if (Object.op_Inequality((Object) component4, (Object) null))
    {
      component4.AttachSnapOnByName(equippable.def.SnapOn);
      if (equippable.def.SnapOn1 != null)
        component4.AttachSnapOnByName(equippable.def.SnapOn1);
    }
    if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Inequality((Object) equippable.GetBuildOverride(), (Object) null))
      ((Component) component1).GetComponent<SymbolOverrideController>().AddBuildOverride(equippable.GetBuildOverride().GetData(), equippable.def.BuildOverridePriority);
    if (Object.op_Implicit((Object) equippable.transform.parent))
    {
      Storage component5 = ((Component) equippable.transform.parent).GetComponent<Storage>();
      if (Object.op_Implicit((Object) component5))
        component5.Drop(((Component) equippable).gameObject, true);
    }
    equippable.transform.parent = slot.gameObject.transform;
    TransformExtensions.SetLocalPosition(equippable.transform, Vector3.zero);
    this.SetEquippableStoredModifiers(equippable, true);
    equippable.OnEquip(slot);
    if ((double) this.refreshHandle.TimeRemaining > 0.0)
    {
      Debug.LogWarning((object) (targetGameObject.GetProperName() + " is already in the process of changing equipment (equip)"));
      this.refreshHandle.ClearScheduler();
    }
    CreatureSimTemperatureTransfer transferer = targetGameObject.GetComponent<CreatureSimTemperatureTransfer>();
    if (!flag)
      this.refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 2f, (Action<object>) (obj =>
      {
        if (!Object.op_Inequality((Object) transferer, (Object) null))
          return;
        transferer.RefreshRegistration();
      }), (object) null, (SchedulerGroup) null);
    Game.Instance.Trigger(-2146166042, (object) null);
  }

  public void Unequip(Equippable equippable)
  {
    AssignableSlotInstance slot = this.GetSlot(equippable.slot);
    slot.Unassign();
    GameObject targetGameObject = this.GetTargetGameObject();
    MinionResume minionResume = Object.op_Inequality((Object) targetGameObject, (Object) null) ? targetGameObject.GetComponent<MinionResume>() : (MinionResume) null;
    Durability component1 = ((Component) equippable).GetComponent<Durability>();
    if (Object.op_Implicit((Object) component1) && Object.op_Implicit((Object) minionResume) && !slot.IsUnassigning() && minionResume.HasPerk(HashedString.op_Implicit(Db.Get().SkillPerks.ExosuitDurability.Id)))
    {
      float num = (GameClock.Instance.GetTimeInCycles() - component1.TimeEquipped) * EQUIPMENT.SUITS.SUIT_DURABILITY_SKILL_BONUS;
      component1.TimeEquipped += num;
    }
    equippable.Trigger(-170173755, (object) this);
    if (!Object.op_Implicit((Object) targetGameObject))
      return;
    EventExtensions.Trigger(targetGameObject, -1285462312, (object) ((Component) equippable).GetComponent<KPrefabID>());
    KBatchedAnimController component2 = targetGameObject.GetComponent<KBatchedAnimController>();
    if (!this.destroyed)
    {
      if (Object.op_Inequality((Object) equippable.GetBuildOverride(), (Object) null) && Object.op_Inequality((Object) component2, (Object) null))
        ((Component) component2).GetComponent<SymbolOverrideController>().TryRemoveBuildOverride(equippable.GetBuildOverride().GetData(), equippable.def.BuildOverridePriority);
      Attributes attributes = targetGameObject.GetAttributes();
      if (attributes != null)
      {
        foreach (AttributeModifier attributeModifier in equippable.def.AttributeModifiers)
          attributes.Remove(attributeModifier);
      }
      if (!equippable.def.IsBody)
      {
        SnapOn component3 = targetGameObject.GetComponent<SnapOn>();
        if (equippable.def.SnapOn != null)
          component3.DetachSnapOnByName(equippable.def.SnapOn);
        if (equippable.def.SnapOn1 != null)
          component3.DetachSnapOnByName(equippable.def.SnapOn1);
      }
      if (Object.op_Implicit((Object) equippable.transform.parent))
      {
        Storage component4 = ((Component) equippable.transform.parent).GetComponent<Storage>();
        if (Object.op_Implicit((Object) component4))
          component4.Drop(((Component) equippable).gameObject, true);
      }
      this.SetEquippableStoredModifiers(equippable, false);
      equippable.transform.parent = (Transform) null;
      TransformExtensions.SetPosition(equippable.transform, Vector3.op_Addition(TransformExtensions.GetPosition(targetGameObject.transform), Vector3.op_Division(Vector3.up, 2f)));
      KBatchedAnimController component5 = ((Component) equippable).GetComponent<KBatchedAnimController>();
      if (Object.op_Implicit((Object) component5))
        component5.SetSceneLayer(Grid.SceneLayer.Ore);
      if (!Object.op_Equality((Object) component2, (Object) null))
      {
        if ((double) this.refreshHandle.TimeRemaining > 0.0)
          this.refreshHandle.ClearScheduler();
        Equipment instance = this;
        this.refreshHandle = GameScheduler.Instance.Schedule("ChangeEquipment", 1f, (Action<object>) (obj =>
        {
          GameObject gameObject = Object.op_Inequality((Object) instance, (Object) null) ? instance.GetTargetGameObject() : (GameObject) null;
          if (!Object.op_Implicit((Object) gameObject))
            return;
          CreatureSimTemperatureTransfer component6 = gameObject.GetComponent<CreatureSimTemperatureTransfer>();
          if (!Object.op_Inequality((Object) component6, (Object) null))
            return;
          component6.RefreshRegistration();
        }), (object) null, (SchedulerGroup) null);
      }
      if (!slot.IsUnassigning())
      {
        PrimaryElement component7 = ((Component) equippable).GetComponent<PrimaryElement>();
        PrimaryElement component8 = targetGameObject.GetComponent<PrimaryElement>();
        if (Object.op_Inequality((Object) component7, (Object) null) && Object.op_Inequality((Object) component8, (Object) null))
        {
          SimUtil.DiseaseInfo invalid1 = SimUtil.DiseaseInfo.Invalid with
          {
            idx = component7.DiseaseIdx,
            count = (int) ((double) component7.DiseaseCount * 0.33000001311302185)
          };
          SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid with
          {
            idx = component8.DiseaseIdx,
            count = (int) ((double) component8.DiseaseCount * 0.33000001311302185)
          };
          component8.ModifyDiseaseCount(-invalid2.count, "Equipment.Unequip");
          component7.ModifyDiseaseCount(-invalid1.count, "Equipment.Unequip");
          if (invalid2.count > 0)
            component7.AddDisease(invalid2.idx, invalid2.count, "Equipment.Unequip");
          if (invalid1.count > 0)
            component8.AddDisease(invalid1.idx, invalid1.count, "Equipment.Unequip");
          if (Object.op_Inequality((Object) component1, (Object) null) && component1.IsWornOut())
            component1.ConvertToWornObject();
        }
      }
    }
    Game.Instance.Trigger(-2146166042, (object) null);
  }

  public bool IsEquipped(Equippable equippable) => equippable.assignee is Equipment && Object.op_Equality((Object) equippable.assignee, (Object) this) && equippable.isEquipped;

  public bool IsSlotOccupied(AssignableSlot slot)
  {
    EquipmentSlotInstance slot1 = this.GetSlot(slot) as EquipmentSlotInstance;
    return slot1.IsAssigned() && (slot1.assignable as Equippable).isEquipped;
  }

  public void UnequipAll()
  {
    foreach (AssignableSlotInstance slot in this.slots)
    {
      if (Object.op_Inequality((Object) slot.assignable, (Object) null))
        slot.assignable.Unassign();
    }
  }

  private void SetEquippableStoredModifiers(Equippable equippable, bool isStoring)
  {
    GameObject gameObject = ((Component) equippable).gameObject;
    Storage.MakeItemTemperatureInsulated(gameObject, isStoring, false);
    Storage.MakeItemInvisible(gameObject, isStoring, false);
  }
}
