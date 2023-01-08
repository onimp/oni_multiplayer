// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.Vector2Drawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace ImGuiObjectDrawer
{
  public sealed class Vector2Drawer : InlineDrawer
  {
    public virtual bool CanDraw(in MemberDrawContext context, in MemberDetails member) => member.value is Vector2;

    protected virtual void DrawInline(in MemberDrawContext context, in MemberDetails member)
    {
      Vector2 vector2 = (Vector2) member.value;
      ImGuiEx.SimpleField(member.name, string.Format("( {0}, {1} )", (object) vector2.x, (object) vector2.y));
    }
  }
}
