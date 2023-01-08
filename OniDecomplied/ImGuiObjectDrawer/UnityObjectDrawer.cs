// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.UnityObjectDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using UnityEngine;

namespace ImGuiObjectDrawer
{
  public class UnityObjectDrawer : PlainCSharpObjectDrawer
  {
    public override bool CanDraw(in MemberDrawContext context, in MemberDetails member) => member.value is Object;

    protected override void DrawCustom(
      in MemberDrawContext context,
      in MemberDetails member,
      int depth)
    {
      Object @object = (Object) member.value;
      ImGuiTreeNodeFlags guiTreeNodeFlags = (ImGuiTreeNodeFlags) 0;
      if (context.default_open && depth <= 0)
        guiTreeNodeFlags = (ImGuiTreeNodeFlags) (guiTreeNodeFlags | 32);
      int num = ImGui.TreeNodeEx(member.name, guiTreeNodeFlags) ? 1 : 0;
      DrawerUtil.Tooltip(member.type);
      if (num == 0)
        return;
      this.DrawContents(in context, in member, depth);
      ImGui.TreePop();
    }
  }
}
