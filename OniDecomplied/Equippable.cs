// Decompiled with JetBrains decompiler
// Type: Equippable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class Equippable : Assignable, ISaveLoadable, IGameObjectEffectDescriptor, IQuality
{
  private QualityLevel quality;
  [MyCmpAdd]
  private EquippableWorkable equippableWorkable;
  [MyCmpAdd]
  private EquippableFacade facade;
  [MyCmpReq]
  private KSelectable selectable;
  public DefHandle defHandle;
  [Serialize]
  public bool isEquipped;
  private bool destroyed;
  [Serialize]
  public bool unequippable = true;
  [Serialize]
  public bool hideInCodex;
  private static readonly EventSystem.IntraObjectHandler<Equippable> SetDestroyedTrueDelegate = new EventSystem.IntraObjectHandler<Equippable>((Action<Equippable, object>) ((component, data) => component.destroyed = true));

  public QualityLevel GetQuality() => this.quality;

  public void SetQuality(QualityLevel level) => this.quality = level;

  public EquipmentDef def
  {
    get => ((DefHandle) ref this.defHandle).Get<EquipmentDef>();
    set => ((DefHandle) ref this.defHandle).Set<EquipmentDef>(value);
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (this.def.AdditionalTags == null)
      return;
    foreach (Tag additionalTag in this.def.AdditionalTags)
      ((Component) this).GetComponent<KPrefabID>().AddTag(additionalTag, false);
  }

  protected override void OnSpawn()
  {
    if (this.isEquipped)
    {
      if (this.assignee != null && this.assignee is MinionIdentity)
      {
        this.assignee = (IAssignableIdentity) (this.assignee as MinionIdentity).assignableProxy.Get();
        this.assignee_identityRef.Set(this.assignee as KMonoBehaviour);
      }
      if (this.assignee == null && Object.op_Inequality((Object) this.assignee_identityRef.Get(), (Object) null))
        this.assignee = ((Component) this.assignee_identityRef.Get()).GetComponent<IAssignableIdentity>();
      if (this.assignee != null)
      {
        ((Component) this.assignee.GetSoleOwner()).GetComponent<Equipment>().Equip(this);
      }
      else
      {
        Debug.LogWarning((object) "Equippable trying to be equipped to missing prefab");
        this.isEquipped = false;
      }
    }
    this.Subscribe<Equippable>(1969584890, Equippable.SetDestroyedTrueDelegate);
  }

  public KAnimFile GetBuildOverride()
  {
    EquippableFacade component = ((Component) this).GetComponent<EquippableFacade>();
    return Object.op_Equality((Object) component, (Object) null) || component.BuildOverride == null ? this.def.BuildOverride : Assets.GetAnim(HashedString.op_Implicit(component.BuildOverride));
  }

  public override void Assign(IAssignableIdentity new_assignee)
  {
    if (new_assignee == this.assignee)
      return;
    if (this.slot != null && new_assignee is MinionIdentity)
      new_assignee = (IAssignableIdentity) (new_assignee as MinionIdentity).assignableProxy.Get();
    if (this.slot != null && new_assignee is StoredMinionIdentity)
      new_assignee = (IAssignableIdentity) (new_assignee as StoredMinionIdentity).assignableProxy.Get();
    if (new_assignee is MinionAssignablesProxy)
    {
      AssignableSlotInstance slot = ((Component) new_assignee.GetSoleOwner()).GetComponent<Equipment>().GetSlot(this.slot);
      if (slot != null)
      {
        Assignable assignable = slot.assignable;
        if (Object.op_Inequality((Object) assignable, (Object) null))
          assignable.Unassign();
      }
    }
    base.Assign(new_assignee);
  }

  public override void Unassign()
  {
    if (this.isEquipped)
    {
      (this.assignee is MinionIdentity ? ((Component) ((MinionIdentity) this.assignee).assignableProxy.Get()).GetComponent<Equipment>() : ((Component) this.assignee).GetComponent<Equipment>()).Unequip(this);
      this.OnUnequip();
    }
    base.Unassign();
  }

  public void OnEquip(AssignableSlotInstance slot)
  {
    this.isEquipped = true;
    if (Object.op_Equality((Object) SelectTool.Instance.selected, (Object) this.selectable))
      SelectTool.Instance.Select((KSelectable) null);
    ((Component) this).GetComponent<KBatchedAnimController>().enabled = false;
    ((Component) this).GetComponent<KSelectable>().IsSelectable = false;
    string name = ((Tag) ref ((Component) this).GetComponent<KPrefabID>().PrefabTag).Name;
    Effects component = slot.gameObject.GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<Effects>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      foreach (Effect effectImmunite in this.def.EffectImmunites)
        component.AddImmunity(effectImmunite, name);
    }
    if (this.def.OnEquipCallBack != null)
      this.def.OnEquipCallBack(this);
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Equipped, false);
  }

  public void OnUnequip()
  {
    this.isEquipped = false;
    if (this.destroyed)
      return;
    ((Component) this).GetComponent<KPrefabID>().RemoveTag(GameTags.Equipped);
    ((Component) this).GetComponent<KBatchedAnimController>().enabled = true;
    ((Component) this).GetComponent<KSelectable>().IsSelectable = true;
    string name = ((Tag) ref ((Component) this).GetComponent<KPrefabID>().PrefabTag).Name;
    if (this.assignee != null)
    {
      Ownables soleOwner = this.assignee.GetSoleOwner();
      if (Object.op_Implicit((Object) soleOwner))
      {
        GameObject targetGameObject = ((Component) soleOwner).GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
        if (Object.op_Implicit((Object) targetGameObject))
        {
          Effects component = targetGameObject.GetComponent<Effects>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            foreach (Effect effectImmunite in this.def.EffectImmunites)
              component.RemoveImmunity(effectImmunite, name);
          }
        }
      }
    }
    if (this.def.OnUnequipCallBack == null)
      return;
    this.def.OnUnequipCallBack(this);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    if (!Object.op_Inequality((Object) this.def, (Object) null))
      return new List<Descriptor>();
    List<Descriptor> equipmentEffects = GameUtil.GetEquipmentEffects(this.def);
    if (this.def.additionalDescriptors != null)
    {
      foreach (Descriptor additionalDescriptor in this.def.additionalDescriptors)
        equipmentEffects.Add(additionalDescriptor);
    }
    return equipmentEffects;
  }
}
