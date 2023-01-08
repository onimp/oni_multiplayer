// Decompiled with JetBrains decompiler
// Type: DevToolMenuNodeParent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System.Collections.Generic;

public class DevToolMenuNodeParent : IMenuNode
{
  public string name;
  public List<IMenuNode> children;

  public DevToolMenuNodeParent(string name)
  {
    this.name = name;
    this.children = new List<IMenuNode>();
  }

  public void AddChild(IMenuNode menuNode) => this.children.Add(menuNode);

  public string GetName() => this.name;

  public void Draw()
  {
    if (!ImGui.BeginMenu(this.name))
      return;
    foreach (IMenuNode child in this.children)
      child.Draw();
    ImGui.EndMenu();
  }
}
