// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.CollectionDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;

namespace ImGuiObjectDrawer
{
  public abstract class CollectionDrawer : MemberDrawer
  {
    public abstract bool IsEmpty(in MemberDrawContext context, in MemberDetails member);

    public virtual MemberDrawType GetDrawType(in MemberDrawContext context, in MemberDetails member) => this.IsEmpty(in context, in member) ? (MemberDrawType) 0 : (MemberDrawType) 1;

    protected virtual void DrawInline(in MemberDrawContext context, in MemberDetails member)
    {
      Debug.Assert(this.IsEmpty(in context, in member));
      this.DrawEmpty(in context, in member);
    }

    protected virtual void DrawCustom(
      in MemberDrawContext context,
      in MemberDetails member,
      int depth)
    {
      Debug.Assert(!this.IsEmpty(in context, in member));
      this.DrawWithContents(in context, in member, depth);
    }

    private void DrawEmpty(in MemberDrawContext context, in MemberDetails member) => ImGui.Text(member.name + "(empty)");

    private void DrawWithContents(in MemberDrawContext context, in MemberDetails member, int depth)
    {
      ImGuiTreeNodeFlags guiTreeNodeFlags = (ImGuiTreeNodeFlags) 0;
      if (context.default_open && depth <= 0)
        guiTreeNodeFlags = (ImGuiTreeNodeFlags) (guiTreeNodeFlags | 32);
      int num = ImGui.TreeNodeEx(member.name, guiTreeNodeFlags) ? 1 : 0;
      DrawerUtil.Tooltip(member.type);
      if (num == 0)
        return;
      this.VisitElements(new CollectionDrawer.ElementVisitor(Visitor), in context, in member);
      ImGui.TreePop();

      void Visitor(in MemberDrawContext context, CollectionDrawer.Element element)
      {
        int num = ImGui.TreeNode(element.node_name) ? 1 : 0;
        element.draw_tooltip();
        if (num == 0)
          return;
        DrawerUtil.DrawObjectContents(element.get_object_to_inspect(), ref context, depth + 1);
        ImGui.TreePop();
      }
    }

    protected abstract void VisitElements(
      CollectionDrawer.ElementVisitor visit,
      in MemberDrawContext context,
      in MemberDetails member);

    protected delegate void ElementVisitor(
      in MemberDrawContext context,
      CollectionDrawer.Element element);

    protected struct Element
    {
      public readonly string node_name;
      public readonly System.Action draw_tooltip;
      public readonly Func<object> get_object_to_inspect;

      public Element(string node_name, System.Action draw_tooltip, Func<object> get_object_to_inspect)
      {
        this.node_name = node_name;
        this.draw_tooltip = draw_tooltip;
        this.get_object_to_inspect = get_object_to_inspect;
      }

      public Element(int index, System.Action draw_tooltip, Func<object> get_object_to_inspect)
        : this(string.Format("[{0}]", (object) index), draw_tooltip, get_object_to_inspect)
      {
      }
    }
  }
}
