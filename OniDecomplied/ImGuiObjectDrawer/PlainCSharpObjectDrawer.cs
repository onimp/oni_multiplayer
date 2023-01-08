// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.PlainCSharpObjectDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;

namespace ImGuiObjectDrawer
{
  public class PlainCSharpObjectDrawer : MemberDrawer
  {
    public virtual bool CanDraw(in MemberDrawContext context, in MemberDetails member) => true;

    public virtual MemberDrawType GetDrawType(in MemberDrawContext context, in MemberDetails member) => (MemberDrawType) 1;

    protected virtual void DrawInline(in MemberDrawContext context, in MemberDetails member) => throw new InvalidOperationException();

    protected virtual void DrawCustom(
      in MemberDrawContext context,
      in MemberDetails member,
      int depth)
    {
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

    protected virtual void DrawContents(
      in MemberDrawContext context,
      in MemberDetails member,
      int depth)
    {
      DrawerUtil.DrawObjectContents(member.value, ref context, depth + 1);
    }
  }
}
