// Decompiled with JetBrains decompiler
// Type: Ownable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class Ownable : Assignable, ISaveLoadable, IGameObjectEffectDescriptor
{
  public bool tintWhenUnassigned = true;
  private Color unownedTint = Color.gray;
  private Color ownedTint = Color.white;

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
      AssignableSlotInstance slot = ((Component) new_assignee.GetSoleOwner()).GetComponent<Ownables>().GetSlot(this.slot);
      if (slot != null)
      {
        Assignable assignable = slot.assignable;
        if (Object.op_Inequality((Object) assignable, (Object) null))
          assignable.Unassign();
      }
    }
    base.Assign(new_assignee);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.UpdateTint();
    this.UpdateStatusString();
    this.OnAssign += new Action<IAssignableIdentity>(this.OnNewAssignment);
    if (this.assignee != null)
      return;
    MinionStorage component1 = ((Component) this).GetComponent<MinionStorage>();
    if (!Object.op_Implicit((Object) component1))
      return;
    List<MinionStorage.Info> storedMinionInfo = component1.GetStoredMinionInfo();
    if (storedMinionInfo.Count <= 0)
      return;
    Ref<KPrefabID> serializedMinion = storedMinionInfo[0].serializedMinion;
    if (serializedMinion == null || serializedMinion.GetId() == -1)
      return;
    StoredMinionIdentity component2 = ((Component) serializedMinion.Get()).GetComponent<StoredMinionIdentity>();
    component2.ValidateProxy();
    this.Assign((IAssignableIdentity) component2);
  }

  private void OnNewAssignment(IAssignableIdentity assignables)
  {
    this.UpdateTint();
    this.UpdateStatusString();
  }

  private void UpdateTint()
  {
    if (!this.tintWhenUnassigned)
      return;
    KAnimControllerBase component1 = ((Component) this).GetComponent<KAnimControllerBase>();
    if (Object.op_Inequality((Object) component1, (Object) null) && component1.HasBatchInstanceData)
    {
      component1.TintColour = Color32.op_Implicit(this.assignee == null ? this.unownedTint : this.ownedTint);
    }
    else
    {
      KBatchedAnimController component2 = ((Component) this).GetComponent<KBatchedAnimController>();
      if (!Object.op_Inequality((Object) component2, (Object) null) || !component2.HasBatchInstanceData)
        return;
      component2.TintColour = Color32.op_Implicit(this.assignee == null ? this.unownedTint : this.ownedTint);
    }
  }

  private void UpdateStatusString()
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    StatusItem status_item = this.assignee == null ? Db.Get().BuildingStatusItems.Unassigned : (!(this.assignee is MinionIdentity) ? (!(this.assignee is Room) ? Db.Get().BuildingStatusItems.AssignedTo : Db.Get().BuildingStatusItems.AssignedTo) : Db.Get().BuildingStatusItems.AssignedTo);
    component.SetStatusItem(Db.Get().StatusItemCategories.Ownable, status_item, (object) this);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.ASSIGNEDDUPLICANT, (string) UI.BUILDINGEFFECTS.TOOLTIPS.ASSIGNEDDUPLICANT, (Descriptor.DescriptorType) 0);
    descriptors.Add(descriptor);
    return descriptors;
  }
}
