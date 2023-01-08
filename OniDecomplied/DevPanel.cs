// Decompiled with JetBrains decompiler
// Type: DevPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DevPanel
{
  public readonly string uniquePanelId;
  public readonly DevPanelList manager;
  public readonly System.Type initialDevToolType;
  public readonly uint idPostfixNumber;
  private List<DevTool> devTools;
  private int currentDevToolIndex;

  public bool isRequestingToClose { get; private set; }

  public Option<(Vector2, ImGuiCond)> nextImGuiWindowPosition { get; private set; }

  public Option<(Vector2, ImGuiCond)> nextImGuiWindowSize { get; private set; }

  public DevPanel(DevTool devTool, DevPanelList manager)
  {
    this.manager = manager;
    this.devTools = new List<DevTool>();
    this.devTools.Add(devTool);
    this.currentDevToolIndex = 0;
    this.initialDevToolType = devTool.GetType();
    manager.Internal_InitPanelId(this.initialDevToolType, out this.uniquePanelId, out this.idPostfixNumber);
  }

  public void PushValue<T>(T value) where T : class => this.PushDevTool((DevTool) new DevToolObjectViewer<T>((Func<T>) (() => value)));

  public void PushValue<T>(Func<T> value) => this.PushDevTool((DevTool) new DevToolObjectViewer<T>(value));

  public void PushDevTool<T>() where T : DevTool, new() => this.PushDevTool((DevTool) new T());

  public void PushDevTool(DevTool devTool)
  {
    if (Input.GetKey((KeyCode) 304))
    {
      this.manager.AddPanelFor(devTool);
    }
    else
    {
      for (int index = this.devTools.Count - 1; index > this.currentDevToolIndex; --index)
      {
        this.devTools[index].Internal_Uninit();
        this.devTools.RemoveAt(index);
      }
      this.devTools.Add(devTool);
      this.currentDevToolIndex = this.devTools.Count - 1;
    }
  }

  public DevTool GetCurrentDevTool() => this.devTools[this.currentDevToolIndex];

  public Option<int> TryGetDevToolIndexByOffset(int offsetFromCurrentIndex)
  {
    int num = this.currentDevToolIndex + offsetFromCurrentIndex;
    if (num < 0)
      return (Option<int>) Option.None;
    return num >= this.devTools.Count ? (Option<int>) Option.None : (Option<int>) num;
  }

  public void RenderPanel()
  {
    DevTool currentDevTool = this.GetCurrentDevTool();
    if (currentDevTool.isRequestingToClosePanel)
    {
      this.isRequestingToClose = true;
    }
    else
    {
      ImGuiWindowFlags drawFlags;
      this.ConfigureImGuiWindowFor(currentDevTool, out drawFlags);
      bool flag = true;
      if (ImGui.Begin(currentDevTool.Name + "###ID_" + this.uniquePanelId, ref flag, drawFlags))
      {
        if (!flag)
        {
          this.isRequestingToClose = true;
          ImGui.End();
          return;
        }
        if (ImGui.BeginMenuBar())
        {
          this.DrawNavigation();
          ImGui.SameLine(0.0f, 20f);
          this.DrawMenuBarContents();
          ImGui.EndMenuBar();
        }
        currentDevTool.DoImGui(this);
        if (this.GetCurrentDevTool() != currentDevTool)
          ImGui.SetScrollY(0.0f);
        ImGui.End();
      }
      if (!currentDevTool.isRequestingToClosePanel)
        return;
      this.isRequestingToClose = true;
    }
  }

  private void DrawNavigation()
  {
    Option<int> toolIndexByOffset1 = this.TryGetDevToolIndexByOffset(-1);
    if (ImGuiEx.Button(" < ", toolIndexByOffset1.HasValue))
      this.currentDevToolIndex = (int) toolIndexByOffset1;
    if (toolIndexByOffset1.HasValue)
      ImGuiEx.TooltipForPrevious("Go back to " + this.devTools[(int) toolIndexByOffset1].Name);
    else
      ImGuiEx.TooltipForPrevious("Go back");
    ImGui.SameLine(0.0f, 5f);
    Option<int> toolIndexByOffset2 = this.TryGetDevToolIndexByOffset(1);
    if (ImGuiEx.Button(" > ", toolIndexByOffset2.HasValue))
      this.currentDevToolIndex = (int) toolIndexByOffset2;
    if (toolIndexByOffset2.HasValue)
      ImGuiEx.TooltipForPrevious("Go forward to " + this.devTools[(int) toolIndexByOffset2].Name);
    else
      ImGuiEx.TooltipForPrevious("Go forward");
  }

  private void DrawMenuBarContents()
  {
  }

  private void ConfigureImGuiWindowFor(DevTool currentDevTool, out ImGuiWindowFlags drawFlags)
  {
    // ISSUE: cast to a reference type
    // ISSUE: explicit reference operation
    ^(int&) ref drawFlags = 1024 | currentDevTool.drawFlags;
    if (this.nextImGuiWindowPosition.HasValue)
    {
      (Vector2, ImGuiCond) tuple = this.nextImGuiWindowPosition.Value;
      ImGui.SetNextWindowPos(tuple.Item1, tuple.Item2);
      this.nextImGuiWindowPosition = new Option<(Vector2, ImGuiCond)>();
    }
    if (!this.nextImGuiWindowSize.HasValue)
      return;
    ImGui.SetNextWindowSize(this.nextImGuiWindowSize.Value.Item1);
    this.nextImGuiWindowSize = new Option<(Vector2, ImGuiCond)>();
  }

  public void SetPosition(Vector2 position, ImGuiCond condition = 0) => this.nextImGuiWindowPosition = (Option<(Vector2, ImGuiCond)>) (position, condition);

  public void SetSize(Vector2 size, ImGuiCond condition = 0) => this.nextImGuiWindowSize = (Option<(Vector2, ImGuiCond)>) (size, condition);

  public void Close() => this.isRequestingToClose = true;

  public void Internal_Uninit()
  {
    foreach (DevTool devTool in this.devTools)
      devTool.Internal_Uninit();
  }
}
