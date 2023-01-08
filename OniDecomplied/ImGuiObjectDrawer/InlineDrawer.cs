// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.InlineDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace ImGuiObjectDrawer
{
  public abstract class InlineDrawer : MemberDrawer
  {
    public virtual MemberDrawType GetDrawType(in MemberDrawContext context, in MemberDetails member) => (MemberDrawType) 0;

    protected virtual void DrawCustom(
      in MemberDrawContext context,
      in MemberDetails member,
      int depth)
    {
      this.DrawInline(ref context, ref member);
    }
  }
}
