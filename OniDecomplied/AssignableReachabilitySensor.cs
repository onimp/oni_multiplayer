// Decompiled with JetBrains decompiler
// Type: AssignableReachabilitySensor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class AssignableReachabilitySensor : Sensor
{
  private AssignableReachabilitySensor.SlotEntry[] slots;
  private Navigator navigator;

  public AssignableReachabilitySensor(Sensors sensors)
    : base(sensors)
  {
    MinionAssignablesProxy assignablesProxy = this.gameObject.GetComponent<MinionIdentity>().assignableProxy.Get();
    assignablesProxy.ConfigureAssignableSlots();
    Assignables[] components = ((Component) assignablesProxy).GetComponents<Assignables>();
    if (components.Length == 0)
      Debug.LogError((object) (this.gameObject.GetProperName() + ": No 'Assignables' components found for AssignableReachabilitySensor"));
    int length = 0;
    for (int index = 0; index < components.Length; ++index)
    {
      Assignables assignables = components[index];
      length += assignables.Slots.Count;
    }
    this.slots = new AssignableReachabilitySensor.SlotEntry[length];
    int num = 0;
    for (int index1 = 0; index1 < components.Length; ++index1)
    {
      Assignables assignables = components[index1];
      for (int index2 = 0; index2 < assignables.Slots.Count; ++index2)
        this.slots[num++].slot = assignables.Slots[index2];
    }
    this.navigator = this.GetComponent<Navigator>();
  }

  public bool IsReachable(AssignableSlot slot)
  {
    for (int index = 0; index < this.slots.Length; ++index)
    {
      if (this.slots[index].slot.slot == slot)
        return this.slots[index].isReachable;
    }
    Debug.LogError((object) ("Could not find slot: " + ((object) slot)?.ToString()));
    return false;
  }

  public override void Update()
  {
    for (int index = 0; index < this.slots.Length; ++index)
    {
      AssignableReachabilitySensor.SlotEntry slot1 = this.slots[index];
      AssignableSlotInstance slot2 = slot1.slot;
      if (slot2.IsAssigned())
      {
        bool flag = slot2.assignable.GetNavigationCost(this.navigator) != -1;
        Operational component = ((Component) slot2.assignable).GetComponent<Operational>();
        if (Object.op_Inequality((Object) component, (Object) null))
          flag = flag && component.IsOperational;
        if (flag != slot1.isReachable)
        {
          slot1.isReachable = flag;
          this.slots[index] = slot1;
          this.Trigger(334784980, (object) slot1);
        }
      }
      else if (slot1.isReachable)
      {
        slot1.isReachable = false;
        this.slots[index] = slot1;
        this.Trigger(334784980, (object) slot1);
      }
    }
  }

  private struct SlotEntry
  {
    public AssignableSlotInstance slot;
    public bool isReachable;
  }
}
