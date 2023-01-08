// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.ArrayDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace ImGuiObjectDrawer
{
  public sealed class ArrayDrawer : CollectionDrawer
  {
    public virtual bool CanDraw(in MemberDrawContext context, in MemberDetails member) => member.type.IsArray;

    public override bool IsEmpty(in MemberDrawContext context, in MemberDetails member) => ((Array) member.value).Length == 0;

    protected override void VisitElements(
      CollectionDrawer.ElementVisitor visit,
      in MemberDrawContext context,
      in MemberDetails member)
    {
      Array array = (Array) member.value;
      for (int i = 0; i < array.Length; ++i)
        visit(in context, new CollectionDrawer.Element(i, closure_0 ?? (closure_0 = (Action) (() => DrawerUtil.Tooltip(array.GetType().GetElementType()))), (Func<object>) (() => (object) new
        {
          value = array.GetValue(i)
        })));
    }
  }
}
