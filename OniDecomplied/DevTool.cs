// Decompiled with JetBrains decompiler
// Type: DevTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using UnityEngine;

public abstract class DevTool
{
  public string Name;
  public bool RequiresGameRunning;
  public bool isRequestingToClosePanel;
  public ImGuiWindowFlags drawFlags;

  public event System.Action OnUninit;

  public DevTool() => this.Name = DevToolUtil.GenerateDevToolName(this);

  public void DoImGui(DevPanel panel)
  {
    if (this.RequiresGameRunning && Object.op_Equality((Object) Game.Instance, (Object) null))
      ImGui.Text("Game not loaded");
    else
      this.RenderTo(panel);
  }

  public void ClosePanel() => this.isRequestingToClosePanel = true;

  protected abstract void RenderTo(DevPanel panel);

  public void Internal_Uninit()
  {
    if (this.OnUninit == null)
      return;
    this.OnUninit();
  }
}
