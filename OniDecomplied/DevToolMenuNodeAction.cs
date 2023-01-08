// Decompiled with JetBrains decompiler
// Type: DevToolMenuNodeAction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class DevToolMenuNodeAction : IMenuNode
{
  public string name;
  public System.Action onClickFn;
  public Func<bool> isEnabledFn;

  public DevToolMenuNodeAction(string name, System.Action onClickFn)
  {
    this.name = name;
    this.onClickFn = onClickFn;
  }

  public string GetName() => this.name;

  public void Draw()
  {
    if (!ImGuiEx.MenuItem(this.name, this.isEnabledFn == null || this.isEnabledFn()))
      return;
    this.onClickFn();
  }
}
