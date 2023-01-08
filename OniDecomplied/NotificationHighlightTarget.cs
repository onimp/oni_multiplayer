// Decompiled with JetBrains decompiler
// Type: NotificationHighlightTarget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class NotificationHighlightTarget : KMonoBehaviour
{
  public string targetKey;
  private NotificationHighlightController controller;

  protected void OnEnable()
  {
    this.controller = ((Component) this).GetComponentInParent<NotificationHighlightController>();
    if (!Object.op_Inequality((Object) this.controller, (Object) null))
      return;
    this.controller.AddTarget(this);
  }

  protected void OnDisable()
  {
    if (!Object.op_Inequality((Object) this.controller, (Object) null))
      return;
    this.controller.RemoveTarget(this);
  }

  public void View() => ((Component) this).GetComponentInParent<NotificationHighlightController>().TargetViewed(this);
}
