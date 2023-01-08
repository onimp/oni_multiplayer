// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.KAnimHashedStringDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace ImGuiObjectDrawer
{
  public sealed class KAnimHashedStringDrawer : InlineDrawer
  {
    public virtual bool CanDraw(in MemberDrawContext context, in MemberDetails member) => member.value is KAnimHashedString;

    protected virtual void DrawInline(in MemberDrawContext context, in MemberDetails member) => ImGuiEx.SimpleField(member.name, string.Format("{0}({1})", member.value, (object) ((KAnimHashedString) member.value).ToString()));
  }
}
