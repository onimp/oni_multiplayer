// Decompiled with JetBrains decompiler
// Type: TargetScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public abstract class TargetScreen : KScreen
{
  protected GameObject selectedTarget;

  public abstract bool IsValidForTarget(GameObject target);

  public void SetTarget(GameObject target)
  {
    if (!Object.op_Inequality((Object) this.selectedTarget, (Object) target))
      return;
    if (Object.op_Inequality((Object) this.selectedTarget, (Object) null))
      this.OnDeselectTarget(this.selectedTarget);
    this.selectedTarget = target;
    if (!Object.op_Inequality((Object) this.selectedTarget, (Object) null))
      return;
    this.OnSelectTarget(this.selectedTarget);
  }

  protected virtual void OnDeactivate()
  {
    base.OnDeactivate();
    this.SetTarget((GameObject) null);
  }

  public virtual void OnSelectTarget(GameObject target) => KMonoBehaviourExtensions.Subscribe(target, 1502190696, new Action<object>(this.OnTargetDestroyed));

  public virtual void OnDeselectTarget(GameObject target) => KMonoBehaviourExtensions.Unsubscribe(target, 1502190696, new Action<object>(this.OnTargetDestroyed));

  private void OnTargetDestroyed(object data)
  {
    ((KScreen) DetailsScreen.Instance).Show(false);
    this.SetTarget((GameObject) null);
  }
}
