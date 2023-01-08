// Decompiled with JetBrains decompiler
// Type: WorkableReactable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class WorkableReactable : Reactable
{
  protected Workable workable;
  private Worker worker;
  public WorkableReactable.AllowedDirection allowedDirection;

  public WorkableReactable(
    Workable workable,
    HashedString id,
    ChoreType chore_type,
    WorkableReactable.AllowedDirection allowed_direction = WorkableReactable.AllowedDirection.Any)
    : base(((Component) workable).gameObject, id, chore_type, 1, 1)
  {
    this.workable = workable;
    this.allowedDirection = allowed_direction;
  }

  public override bool InternalCanBegin(
    GameObject new_reactor,
    Navigator.ActiveTransition transition)
  {
    if (Object.op_Equality((Object) this.workable, (Object) null) || Object.op_Inequality((Object) this.reactor, (Object) null))
      return false;
    Brain component1 = new_reactor.GetComponent<Brain>();
    if (Object.op_Equality((Object) component1, (Object) null) || !component1.IsRunning())
      return false;
    Navigator component2 = new_reactor.GetComponent<Navigator>();
    if (Object.op_Equality((Object) component2, (Object) null) || !component2.IsMoving())
      return false;
    if (this.allowedDirection == WorkableReactable.AllowedDirection.Any)
      return true;
    Facing component3 = new_reactor.GetComponent<Facing>();
    if (Object.op_Equality((Object) component3, (Object) null))
      return false;
    bool facing = component3.GetFacing();
    return (!facing || this.allowedDirection != WorkableReactable.AllowedDirection.Right) && (facing || this.allowedDirection != WorkableReactable.AllowedDirection.Left);
  }

  protected override void InternalBegin()
  {
    this.worker = this.reactor.GetComponent<Worker>();
    this.worker.StartWork(new Worker.StartWorkInfo(this.workable));
  }

  public override void Update(float dt)
  {
    if (Object.op_Equality((Object) this.worker.workable, (Object) null))
    {
      this.End();
    }
    else
    {
      if (this.worker.Work(dt) == Worker.WorkResult.InProgress)
        return;
      this.End();
    }
  }

  protected override void InternalEnd()
  {
    if (!Object.op_Inequality((Object) this.worker, (Object) null))
      return;
    this.worker.StopWork();
  }

  protected override void InternalCleanup()
  {
  }

  public enum AllowedDirection
  {
    Any,
    Left,
    Right,
  }
}
