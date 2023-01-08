// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.IEnumerableDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;

namespace ImGuiObjectDrawer
{
  public sealed class IEnumerableDrawer : CollectionDrawer
  {
    public virtual bool CanDraw(in MemberDrawContext context, in MemberDetails member) => ((MemberDetails) ref member).CanAssignToType<IEnumerable>();

    public override bool IsEmpty(in MemberDrawContext context, in MemberDetails member) => !((IEnumerable) member.value).GetEnumerator().MoveNext();

    protected override void VisitElements(
      CollectionDrawer.ElementVisitor visit,
      in MemberDrawContext context,
      in MemberDetails member)
    {
      IEnumerable enumerable = (IEnumerable) member.value;
      int index = 0;
      foreach (object obj in enumerable)
      {
        object el = obj;
        visit(in context, new CollectionDrawer.Element(index, (Action) (() => DrawerUtil.Tooltip(el.GetType())), (Func<object>) (() => (object) new
        {
          value = el
        })));
        ++index;
      }
    }
  }
}
