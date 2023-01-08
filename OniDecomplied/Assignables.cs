// Decompiled with JetBrains decompiler
// Type: Assignables
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Assignables")]
public class Assignables : KMonoBehaviour
{
  protected List<AssignableSlotInstance> slots = new List<AssignableSlotInstance>();
  private static readonly EventSystem.IntraObjectHandler<Assignables> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler<Assignables>(GameTags.Dead, (Action<Assignables, object>) ((component, data) => component.OnDeath(data)));

  public List<AssignableSlotInstance> Slots => this.slots;

  protected IAssignableIdentity GetAssignableIdentity()
  {
    MinionIdentity component = ((Component) this).GetComponent<MinionIdentity>();
    return Object.op_Inequality((Object) component, (Object) null) ? (IAssignableIdentity) component.assignableProxy.Get() : (IAssignableIdentity) ((Component) this).GetComponent<MinionAssignablesProxy>();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    GameUtil.SubscribeToTags<Assignables>(this, Assignables.OnDeadTagAddedDelegate, true);
  }

  private void OnDeath(object data)
  {
    foreach (AssignableSlotInstance slot in this.slots)
      slot.Unassign();
  }

  public void Add(AssignableSlotInstance slot_instance) => this.slots.Add(slot_instance);

  public Assignable GetAssignable(AssignableSlot slot) => this.GetSlot(slot)?.assignable;

  public AssignableSlotInstance GetSlot(AssignableSlot slot)
  {
    Debug.Assert(this.slots.Count > 0, (object) "GetSlot called with no slots configured");
    if (slot == null)
      return (AssignableSlotInstance) null;
    foreach (AssignableSlotInstance slot1 in this.slots)
    {
      if (slot1.slot == slot)
        return slot1;
    }
    return (AssignableSlotInstance) null;
  }

  public Assignable AutoAssignSlot(AssignableSlot slot)
  {
    Assignable assignable1 = this.GetAssignable(slot);
    if (Object.op_Inequality((Object) assignable1, (Object) null))
      return assignable1;
    GameObject targetGameObject = ((Component) this).GetComponent<MinionAssignablesProxy>().GetTargetGameObject();
    if (Object.op_Equality((Object) targetGameObject, (Object) null))
    {
      Debug.LogWarning((object) "AutoAssignSlot failed, proxy game object was null.");
      return (Assignable) null;
    }
    Navigator component = targetGameObject.GetComponent<Navigator>();
    IAssignableIdentity assignableIdentity = this.GetAssignableIdentity();
    int num = int.MaxValue;
    foreach (Assignable assignable2 in Game.Instance.assignmentManager)
    {
      if (!Object.op_Equality((Object) assignable2, (Object) null) && !assignable2.IsAssigned() && assignable2.slot == slot && assignable2.CanAutoAssignTo(assignableIdentity))
      {
        int navigationCost = assignable2.GetNavigationCost(component);
        if (navigationCost != -1 && navigationCost < num)
        {
          num = navigationCost;
          assignable1 = assignable2;
        }
      }
    }
    if (Object.op_Inequality((Object) assignable1, (Object) null))
      assignable1.Assign(assignableIdentity);
    return assignable1;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    foreach (AssignableSlotInstance slot in this.slots)
      slot.Unassign();
  }
}
