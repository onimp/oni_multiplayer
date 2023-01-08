// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.Vector3Drawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace ImGuiObjectDrawer
{
  public sealed class Vector3Drawer : InlineDrawer
  {
    public virtual bool CanDraw(in MemberDrawContext context, in MemberDetails member) => member.value is Vector3;

    protected virtual void DrawInline(in MemberDrawContext context, in MemberDetails member)
    {
      Vector3 vector3 = (Vector3) member.value;
      ImGuiEx.SimpleField(member.name, string.Format("( {0}, {1}, {2} )", (object) vector3.x, (object) vector3.y, (object) vector3.z));
    }
  }
}
