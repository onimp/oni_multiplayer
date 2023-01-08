// Decompiled with JetBrains decompiler
// Type: ImGuiObjectDrawer.FallbackDrawer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace ImGuiObjectDrawer
{
  public sealed class FallbackDrawer : SimpleDrawer
  {
    public override bool CanDraw(in MemberDrawContext context, in MemberDetails member) => true;

    public override bool CanDrawAtDepth(int depth) => true;
  }
}
