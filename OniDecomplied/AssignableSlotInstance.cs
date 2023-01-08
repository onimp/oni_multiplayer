// Decompiled with JetBrains decompiler
// Type: AssignableSlotInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public abstract class AssignableSlotInstance
{
  public AssignableSlot slot;
  public Assignable assignable;
  private bool unassigning;

  public Assignables assignables { get; private set; }

  public GameObject gameObject => ((Component) this.assignables).gameObject;

  public AssignableSlotInstance(Assignables assignables, AssignableSlot slot)
  {
    this.slot = slot;
    this.assignables = assignables;
  }

  public void Assign(Assignable assignable)
  {
    if (Object.op_Equality((Object) this.assignable, (Object) assignable))
      return;
    this.Unassign(false);
    this.assignable = assignable;
    this.assignables.Trigger(-1585839766, (object) this);
  }

  public virtual void Unassign(bool trigger_event = true)
  {
    if (this.unassigning || !this.IsAssigned())
      return;
    this.unassigning = true;
    this.assignable.Unassign();
    if (trigger_event)
      this.assignables.Trigger(-1585839766, (object) this);
    this.assignable = (Assignable) null;
    this.unassigning = false;
  }

  public bool IsAssigned() => Object.op_Inequality((Object) this.assignable, (Object) null);

  public bool IsUnassigning() => this.unassigning;
}
