// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.Vector4Drawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace ImGuiObjectDrawer
{
  public sealed class Vector4Drawer : InlineDrawer
  {
    public virtual bool CanDraw(in MemberDrawContext context, in MemberDetails member) => member.value is Vector4;

    protected virtual void DrawInline(in MemberDrawContext context, in MemberDetails member)
    {
      Vector4 vector4 = (Vector4) member.value;
      ImGuiEx.SimpleField(member.name, string.Format("( {0}, {1}, {2}, {3} )", (object) vector4.x, (object) vector4.y, (object) vector4.z, (object) vector4.w));
    }
  }
}
