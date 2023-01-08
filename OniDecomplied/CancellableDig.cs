// Decompiled with JetBrains decompiler
// Type: CancellableDig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
public class CancellableDig : Cancellable
{
  protected override void OnCancel(object data)
  {
    if ((data != null ? ((bool) data ? 1 : 0) : 0) != 0)
    {
      this.OnAnimationDone("ScaleDown");
    }
    else
    {
      EasingAnimations componentInChildren = ((Component) this).GetComponentInChildren<EasingAnimations>();
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      if (componentInChildren.IsPlaying && Grid.Element[cell].hardness == byte.MaxValue)
      {
        componentInChildren.OnAnimationDone += new Action<string>(this.DoCancelAnim);
      }
      else
      {
        componentInChildren.OnAnimationDone += new Action<string>(this.OnAnimationDone);
        componentInChildren.PlayAnimation("ScaleDown", 0.1f);
      }
    }
  }

  private void DoCancelAnim(string animName)
  {
    EasingAnimations componentInChildren = ((Component) this).GetComponentInChildren<EasingAnimations>();
    componentInChildren.OnAnimationDone -= new Action<string>(this.DoCancelAnim);
    componentInChildren.OnAnimationDone += new Action<string>(this.OnAnimationDone);
    componentInChildren.PlayAnimation("ScaleDown", 0.1f);
  }

  private void OnAnimationDone(string animationName)
  {
    if (animationName != "ScaleDown")
      return;
    ((Component) this).GetComponentInChildren<EasingAnimations>().OnAnimationDone -= new Action<string>(this.OnAnimationDone);
    TracesExtesions.DeleteObject((Component) this);
  }
}
