// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.IDictionaryDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;

namespace ImGuiObjectDrawer
{
  public sealed class IDictionaryDrawer : CollectionDrawer
  {
    public virtual bool CanDraw(in MemberDrawContext context, in MemberDetails member) => ((MemberDetails) ref member).CanAssignToType<IDictionary>();

    public override bool IsEmpty(in MemberDrawContext context, in MemberDetails member) => ((ICollection) member.value).Count == 0;

    protected override void VisitElements(
      CollectionDrawer.ElementVisitor visit,
      in MemberDrawContext context,
      in MemberDetails member)
    {
      IDictionary dictionary = (IDictionary) member.value;
      int index = 0;
      foreach (DictionaryEntry dictionaryEntry in dictionary)
      {
        DictionaryEntry kvp = dictionaryEntry;
        visit(in context, new CollectionDrawer.Element(index, (Action) (() => DrawerUtil.Tooltip(string.Format("{0} -> {1}", (object) kvp.Key.GetType(), (object) kvp.Value.GetType()))), (Func<object>) (() => (object) new
        {
          key = kvp.Key,
          value = kvp.Value
        })));
        ++index;
      }
    }
  }
}
