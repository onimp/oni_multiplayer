// Decompiled with JetBrains decompiler
// Type: DevToolAnimFile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiObjectDrawer;
using UnityEngine;

public class DevToolAnimFile : DevTool
{
  private KAnimFile animFile;

  public DevToolAnimFile(KAnimFile animFile)
  {
    this.animFile = animFile;
    this.Name = "Anim File: \"" + ((Object) animFile).name + "\"";
  }

  protected override void RenderTo(DevPanel panel)
  {
    ImGuiEx.DrawObject((object) this.animFile, new MemberDrawContext?());
    ImGuiEx.DrawObject((object) this.animFile.GetData(), new MemberDrawContext?());
  }
}
