// Decompiled with JetBrains decompiler
// Type: KAnimSynchronizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class KAnimSynchronizer
{
  private KAnimControllerBase masterController;
  private List<KAnimControllerBase> Targets = new List<KAnimControllerBase>();
  private List<KAnimSynchronizedController> SyncedControllers = new List<KAnimSynchronizedController>();

  public KAnimSynchronizer(KAnimControllerBase master_controller) => this.masterController = master_controller;

  private void Clear(KAnimControllerBase controller) => controller.Play(HashedString.op_Implicit(controller.defaultAnim), (KAnim.PlayMode) 0);

  public void Add(KAnimControllerBase controller) => this.Targets.Add(controller);

  public void Remove(KAnimControllerBase controller)
  {
    this.Clear(controller);
    this.Targets.Remove(controller);
  }

  private void Clear(KAnimSynchronizedController controller) => controller.Play(HashedString.op_Implicit(controller.synchronizedController.defaultAnim), (KAnim.PlayMode) 0);

  public void Add(KAnimSynchronizedController controller) => this.SyncedControllers.Add(controller);

  public void Remove(KAnimSynchronizedController controller)
  {
    this.Clear(controller);
    this.SyncedControllers.Remove(controller);
  }

  public void Clear()
  {
    foreach (KAnimControllerBase target in this.Targets)
    {
      if (!Object.op_Equality((Object) target, (Object) null) && target.AnimFiles != null)
        this.Clear(target);
    }
    this.Targets.Clear();
    foreach (KAnimSynchronizedController syncedController in this.SyncedControllers)
    {
      if (!Object.op_Equality((Object) syncedController.synchronizedController, (Object) null) && syncedController.synchronizedController.AnimFiles != null)
        this.Clear(syncedController);
    }
    this.SyncedControllers.Clear();
  }

  public void Sync(KAnimControllerBase controller)
  {
    if (Object.op_Equality((Object) this.masterController, (Object) null) || Object.op_Equality((Object) controller, (Object) null))
      return;
    KAnim.Anim currentAnim = this.masterController.GetCurrentAnim();
    if (currentAnim != null && !string.IsNullOrEmpty(controller.defaultAnim) && !controller.HasAnimation(HashedString.op_Implicit(currentAnim.name)))
    {
      controller.Play(HashedString.op_Implicit(controller.defaultAnim), (KAnim.PlayMode) 0);
    }
    else
    {
      if (currentAnim == null)
        return;
      KAnim.PlayMode mode = this.masterController.GetMode();
      float playSpeed = this.masterController.GetPlaySpeed();
      float elapsedTime = this.masterController.GetElapsedTime();
      controller.Play(HashedString.op_Implicit(currentAnim.name), mode, playSpeed, elapsedTime);
      Facing component = ((Component) controller).GetComponent<Facing>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        float target_x = TransformExtensions.GetPosition(component.transform).x + (this.masterController.FlipX ? -0.5f : 0.5f);
        component.Face(target_x);
      }
      else
      {
        controller.FlipX = this.masterController.FlipX;
        controller.FlipY = this.masterController.FlipY;
      }
    }
  }

  public void SyncController(KAnimSynchronizedController controller)
  {
    if (Object.op_Equality((Object) this.masterController, (Object) null) || controller == null)
      return;
    KAnim.Anim currentAnim = this.masterController.GetCurrentAnim();
    string str = currentAnim != null ? currentAnim.name + controller.Postfix : string.Empty;
    if (!string.IsNullOrEmpty(controller.synchronizedController.defaultAnim) && !controller.synchronizedController.HasAnimation(HashedString.op_Implicit(str)))
    {
      controller.Play(HashedString.op_Implicit(controller.synchronizedController.defaultAnim), (KAnim.PlayMode) 0);
    }
    else
    {
      if (currentAnim == null)
        return;
      KAnim.PlayMode mode = this.masterController.GetMode();
      float playSpeed = this.masterController.GetPlaySpeed();
      float elapsedTime = this.masterController.GetElapsedTime();
      controller.Play(HashedString.op_Implicit(str), mode, playSpeed, elapsedTime);
      Facing component = ((Component) controller.synchronizedController).GetComponent<Facing>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        float target_x = TransformExtensions.GetPosition(component.transform).x + (this.masterController.FlipX ? -0.5f : 0.5f);
        component.Face(target_x);
      }
      else
      {
        controller.synchronizedController.FlipX = this.masterController.FlipX;
        controller.synchronizedController.FlipY = this.masterController.FlipY;
      }
    }
  }

  public void Sync()
  {
    for (int index = 0; index < this.Targets.Count; ++index)
      this.Sync(this.Targets[index]);
    for (int index = 0; index < this.SyncedControllers.Count; ++index)
      this.SyncController(this.SyncedControllers[index]);
  }

  public void SyncTime()
  {
    float elapsedTime = this.masterController.GetElapsedTime();
    for (int index = 0; index < this.Targets.Count; ++index)
      this.Targets[index].SetElapsedTime(elapsedTime);
    for (int index = 0; index < this.SyncedControllers.Count; ++index)
      this.SyncedControllers[index].synchronizedController.SetElapsedTime(elapsedTime);
  }
}
