// Decompiled with JetBrains decompiler
// Type: DevToolObjectViewer`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiObjectDrawer;
using System;

public class DevToolObjectViewer<T> : DevTool
{
  private Func<T> getValue;

  public DevToolObjectViewer(Func<T> getValue)
  {
    this.getValue = getValue;
    this.Name = typeof (T).Name;
  }

  protected override void RenderTo(DevPanel panel)
  {
    T obj = this.getValue();
    this.Name = obj.GetType().Name;
    ImGuiEx.DrawObject((object) obj, new MemberDrawContext?());
  }
}
