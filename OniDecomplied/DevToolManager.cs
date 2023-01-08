// Decompiled with JetBrains decompiler
// Type: DevToolManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DevToolManager
{
  public const string SHOW_DEVTOOLS = "ShowDevtools";
  public static DevToolManager Instance;
  private bool toggleKeyWasDown;
  private bool showImGui;
  private bool prevShowImGui;
  private bool doesImGuiWantInput;
  private bool prevDoesImGuiWantInput;
  private bool showImguiState;
  private bool showImguiDemo;
  public bool UserAcceptedWarning;
  private DevToolWarning warning = new DevToolWarning();
  private DevToolMenuFontSize menuFontSize = new DevToolMenuFontSize();
  public DevPanelList panels = new DevPanelList();
  public DevToolMenuNodeList menuNodes = new DevToolMenuNodeList();
  public Dictionary<System.Type, string> devToolNameDict = new Dictionary<System.Type, string>();
  private HashSet<System.Type> dontAutomaticallyRegisterTypes = new HashSet<System.Type>();

  public bool Show => this.showImGui;

  public DevToolManager()
  {
    DevToolManager.Instance = this;
    this.RegisterDevTool<DevToolSimDebug>("Debuggers/Sim Debug");
    this.RegisterDevTool<DevToolStateMachineDebug>("Debuggers/State Machine");
    this.RegisterDevTool<DevToolSaveGameInfo>("Debuggers/Save Game Info");
    this.RegisterDevTool<DevToolPrintingPodDebug>("Debuggers/Printing Pod Debug");
    this.RegisterDevTool<DevToolBigBaseMutations>("Debuggers/Big Base Mutation Utilities");
    this.RegisterDevTool<DevToolNavGrid>("Debuggers/Nav Grid");
    this.RegisterDevTool<DevToolResearchDebugger>("Debuggers/Research");
    this.RegisterDevTool<DevToolStatusItems>("Debuggers/StatusItems");
    this.RegisterDevTool<DevToolUI>("Debuggers/UI");
    this.RegisterDevTool<DevToolUnlockedIds>("Debuggers/UnlockedIds List");
    this.RegisterDevTool<DevToolStringsTable>("Debuggers/StringsTable");
    this.RegisterDevTool<DevToolChoreDebugger>("Debuggers/Chore");
    this.RegisterDevTool<DevToolBatchedAnimDebug>("Debuggers/Batched Anim");
    this.RegisterDevTool<DevTool_StoryTraits_Reveal>("Debuggers/Story Traits Reveal");
    this.RegisterDevTool<DevTool_StoryTrait_CritterManipulator>("Debuggers/Story Trait - Critter Manipulator");
    this.RegisterDevTool<DevToolAnimEventManager>("Debuggers/Anim Event Manager");
    this.RegisterDevTool<DevToolSceneBrowser>("Scene/Browser");
    this.RegisterDevTool<DevToolSceneInspector>("Scene/Inspector");
    this.menuNodes.AddAction("Help/" + UI.FRONTEND.DEVTOOLS.TITLE.text, (System.Action) (() => this.warning.ShouldDrawWindow = true));
    this.RegisterDevTool<DevToolCommandPalette>("Help/Command Palette");
    this.RegisterAdditionalDevToolsByReflection();
  }

  public void Init() => this.UserAcceptedWarning = KPlayerPrefs.GetInt("ShowDevtools", 0) == 1;

  private void RegisterDevTool<T>(string location) where T : DevTool, new()
  {
    this.menuNodes.AddAction(location, (System.Action) (() => this.panels.AddPanelFor<T>()));
    this.dontAutomaticallyRegisterTypes.Add(typeof (T));
    this.devToolNameDict[typeof (T)] = System.IO.Path.GetFileName(location);
  }

  private void RegisterAdditionalDevToolsByReflection()
  {
    foreach (System.Type type1 in ReflectionUtil.CollectTypesThatInheritOrImplement<DevTool>(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
    {
      System.Type type = type1;
      if (!type.IsAbstract && !this.dontAutomaticallyRegisterTypes.Contains(type) && ReflectionUtil.HasDefaultConstructor(type))
        this.menuNodes.AddAction("Debuggers/" + DevToolUtil.GenerateDevToolName(type), (System.Action) (() => this.panels.AddPanelFor((DevTool) Activator.CreateInstance(type))));
    }
  }

  public void UpdateShouldShowTools()
  {
    if (!DebugHandler.enabled)
    {
      this.showImGui = false;
    }
    else
    {
      bool flag = Input.GetKeyDown((KeyCode) 96) && (Input.GetKey((KeyCode) 306) || Input.GetKeyDown((KeyCode) 305));
      if (!this.toggleKeyWasDown & flag)
        this.showImGui = !this.showImGui;
      this.toggleKeyWasDown = flag;
    }
  }

  public void UpdateTools()
  {
    if (!DebugHandler.enabled)
      return;
    if (this.showImGui)
    {
      if (this.warning.ShouldDrawWindow)
        this.warning.DrawWindow(out this.warning.ShouldDrawWindow);
      if (!this.UserAcceptedWarning)
      {
        this.warning.DrawMenuBar();
      }
      else
      {
        this.DrawMenu();
        this.panels.Render();
        if (this.showImguiState)
        {
          if (ImGui.Begin("ImGui state", ref this.showImguiState))
          {
            ImGuiIOPtr io1 = ImGui.GetIO();
            ImGui.Checkbox("ImGui.GetIO().WantCaptureMouse", ref ((ImGuiIOPtr) ref io1).WantCaptureMouse);
            ImGuiIOPtr io2 = ImGui.GetIO();
            ImGui.Checkbox("ImGui.GetIO().WantCaptureKeyboard", ref ((ImGuiIOPtr) ref io2).WantCaptureKeyboard);
          }
          ImGui.End();
        }
        if (this.showImguiDemo)
          ImGui.ShowDemoWindow(ref this.showImguiDemo);
      }
    }
    this.UpdateConsumingGameInputs();
    this.UpdateShortcuts();
  }

  private void UpdateShortcuts()
  {
    if (!this.showImGui || !this.UserAcceptedWarning)
      return;
    DoUpdate();

    void DoUpdate()
    {
      if ((Input.GetKey((KeyCode) 306) || Input.GetKey((KeyCode) 305)) && Input.GetKeyDown((KeyCode) 32))
      {
        DevToolCommandPalette.Init();
        this.showImGui = true;
      }
      if (!Input.GetKeyDown((KeyCode) 44))
        return;
      DevToolUI.PingHoveredObject();
      this.showImGui = true;
    }
  }

  private void DrawMenu()
  {
    this.menuFontSize.InitializeIfNeeded();
    if (!ImGui.BeginMainMenuBar())
      return;
    this.menuNodes.Draw();
    this.menuFontSize.DrawMenu();
    if (ImGui.BeginMenu("IMGUI"))
    {
      ImGui.Checkbox("ImGui state", ref this.showImguiState);
      ImGui.Checkbox("ImGui Demo", ref this.showImguiDemo);
      ImGui.EndMenu();
    }
    ImGui.EndMainMenuBar();
  }

  private void UpdateConsumingGameInputs()
  {
    this.doesImGuiWantInput = false;
    if (this.showImGui)
    {
      ImGuiIOPtr io1 = ImGui.GetIO();
      int num;
      if (!((ImGuiIOPtr) ref io1).WantCaptureMouse)
      {
        ImGuiIOPtr io2 = ImGui.GetIO();
        num = ((ImGuiIOPtr) ref io2).WantCaptureKeyboard ? 1 : 0;
      }
      else
        num = 1;
      this.doesImGuiWantInput = num != 0;
      if (!this.prevDoesImGuiWantInput && this.doesImGuiWantInput)
        OnInputEnterImGui();
      if (this.prevDoesImGuiWantInput && !this.doesImGuiWantInput)
        OnInputExitImGui();
    }
    if (this.prevShowImGui && this.prevDoesImGuiWantInput && !this.showImGui)
      OnInputExitImGui();
    this.prevShowImGui = this.showImGui;
    this.prevDoesImGuiWantInput = this.doesImGuiWantInput;
    KInputManager.devToolFocus = this.showImGui && this.doesImGuiWantInput;

    static void OnInputEnterImGui()
    {
      UnityMouseCatcherUI.SetEnabled(true);
      GameInputManager inputManager = Global.GetInputManager();
      for (int index = 0; index < ((KInputManager) inputManager).GetControllerCount(); ++index)
        ((KInputManager) inputManager).GetController(index).HandleCancelInput();
    }

    static void OnInputExitImGui() => UnityMouseCatcherUI.SetEnabled(false);
  }
}
