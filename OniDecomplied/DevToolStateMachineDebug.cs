// Decompiled with JetBrains decompiler
// Type: DevToolStateMachineDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevToolStateMachineDebug : DevTool
{
  private int selectedStateMachine;
  private int selectedLog;
  private GameObject selectedGameObject;
  private Vector2 scrollPos;
  private bool lockSelection;
  private bool showSettings;
  private string stateMachineFilter = "";

  private void Update()
  {
    if (!Application.isPlaying)
      return;
    if (Object.op_Equality((Object) this.selectedGameObject, (Object) null))
      this.lockSelection = false;
    GameObject gameObject = ((Component) SelectTool.Instance?.selected)?.gameObject;
    if (this.lockSelection || !Object.op_Inequality((Object) this.selectedGameObject, (Object) gameObject) || !Object.op_Inequality((Object) gameObject, (Object) null) || gameObject.GetComponentsInChildren<StateMachineController>().Length == 0)
      return;
    this.selectedGameObject = gameObject;
    this.selectedStateMachine = 0;
  }

  public void ShowEditor(StateMachineDebuggerSettings.Entry entry)
  {
    ImGui.Text(entry.typeName);
    ImGui.SameLine();
    ImGui.PushID(entry.typeName);
    ImGui.PushID(1);
    ImGui.Checkbox("", ref entry.enableConsoleLogging);
    ImGui.PopID();
    ImGui.SameLine();
    ImGui.PushID(2);
    ImGui.Checkbox("", ref entry.breakOnGoTo);
    ImGui.PopID();
    ImGui.SameLine();
    ImGui.PushID(3);
    ImGui.Checkbox("", ref entry.saveHistory);
    ImGui.PopID();
    ImGui.PopID();
  }

  protected override void RenderTo(DevPanel panel)
  {
    this.Update();
    ImGui.InputText("Filter:", ref this.stateMachineFilter, 256U);
    if (this.showSettings = ImGui.CollapsingHeader("Debug Settings:"))
    {
      if (ImGui.Button("Reset"))
        StateMachineDebuggerSettings.Get().Clear();
      ImGui.Text("EnableConsoleLogging / BreakOnGoTo / SaveHistory");
      int num = 0;
      foreach (StateMachineDebuggerSettings.Entry entry in StateMachineDebuggerSettings.Get())
      {
        if (string.IsNullOrEmpty(this.stateMachineFilter) || entry.typeName.ToLower().IndexOf(this.stateMachineFilter) >= 0)
        {
          this.ShowEditor(entry);
          ++num;
        }
      }
    }
    if (!Application.isPlaying || !Object.op_Inequality((Object) this.selectedGameObject, (Object) null))
      return;
    StateMachineController[] componentsInChildren = this.selectedGameObject.GetComponentsInChildren<StateMachineController>();
    if (componentsInChildren.Length == 0)
      return;
    List<string> source1 = new List<string>();
    List<StateMachine.Instance> instanceList = new List<StateMachine.Instance>();
    List<StateMachine.BaseDef> source2 = new List<StateMachine.BaseDef>();
    foreach (StateMachineController machineController in componentsInChildren)
    {
      foreach (StateMachine.Instance instance in machineController)
      {
        string str = ((Object) machineController).name + "." + instance.ToString();
        if (instance.isCrashed)
          str = "(ERROR)" + str;
        source1.Add(str);
      }
    }
    List<string> stringList = this.stateMachineFilter == null || this.stateMachineFilter == "" ? source1.Select<string, string>((Func<string, string>) (name => name.ToLower())).ToList<string>() : source1.Where<string>((Func<string, bool>) (name => name.ToLower().Contains(this.stateMachineFilter))).Select<string, string>((Func<string, string>) (name => name.ToLower())).ToList<string>();
    foreach (StateMachineController machineController in componentsInChildren)
    {
      foreach (StateMachine.Instance instance in machineController)
      {
        string str = ((Object) machineController).name + "." + instance.ToString();
        if (instance.isCrashed)
          str = "(ERROR)" + str;
        if (stringList.Contains(str.ToLower()))
          instanceList.Add(instance);
      }
      foreach (StateMachine.BaseDef def in machineController.GetDefs<StateMachine.BaseDef>())
        source2.Add(def);
    }
    if (stringList.Count == 0)
    {
      ImGui.LabelText("Defs", source2.Count == 0 ? "(none)" : string.Join(", ", source2.Select<StateMachine.BaseDef, string>((Func<StateMachine.BaseDef, string>) (d => d.GetType().ToString()))));
      foreach (StateMachineController controller in componentsInChildren)
        this.ShowControllerLog(controller);
    }
    else
    {
      this.selectedStateMachine = Math.Min(this.selectedStateMachine, stringList.Count - 1);
      ImGui.LabelText("Defs", source2.Count == 0 ? "(none)" : string.Join(", ", source2.Select<StateMachine.BaseDef, string>((Func<StateMachine.BaseDef, string>) (d => d.GetType().ToString()))));
      ImGui.Checkbox("Lock selection", ref this.lockSelection);
      ImGui.Indent();
      ImGui.Combo("Select state machine", ref this.selectedStateMachine, stringList.ToArray(), stringList.Count);
      ImGui.Unindent();
      StateMachine.Instance instance = instanceList[this.selectedStateMachine];
      this.ShowStates(instance);
      this.ShowTags(instance);
      this.ShowDetails(instance);
      this.ShowLog(instance);
      this.ShowControllerLog(instance);
      this.ShowHistory(instance.GetMaster().GetComponent<StateMachineController>());
      this.ShowKAnimControllerLog();
    }
  }

  private void ShowStates(StateMachine.Instance state_machine_instance)
  {
    StateMachine stateMachine = state_machine_instance.GetStateMachine();
    ImGui.Text(stateMachine.ToString() + ": ");
    ImGui.Checkbox("Break On GoTo: ", ref state_machine_instance.breakOnGoTo);
    ImGui.Checkbox("Console Logging: ", ref state_machine_instance.enableConsoleLogging);
    string str = "None";
    StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
    if (currentState != null)
      str = currentState.name;
    string[] array = Util.Append<string>(stateMachine.GetStateNames(), "None");
    array[0] = array[0];
    int num = Array.IndexOf<string>(array, str);
    int index1 = num;
    for (int index2 = 0; index2 < array.Length; ++index2)
      ImGui.RadioButton(array[index2], ref index1, index2);
    if (index1 == num)
      return;
    if (array[index1] == "None")
      state_machine_instance.StopSM("StateMachineEditor.StopSM");
    else
      state_machine_instance.GoTo(array[index1]);
  }

  public void ShowTags(StateMachine.Instance state_machine_instance)
  {
    ImGui.Text("Tags:");
    ImGui.Indent();
    KPrefabID component = state_machine_instance.GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      foreach (Tag tag in component.Tags)
        ImGui.Text(((Tag) ref tag).Name);
    }
    ImGui.Unindent();
  }

  private void ShowDetails(StateMachine.Instance state_machine_instance)
  {
    state_machine_instance.GetStateMachine();
    string str = "None";
    StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
    if (currentState != null)
      str = currentState.name;
    ImGui.Text(str + ": ");
    ImGui.Indent();
    this.ShowParameters(state_machine_instance);
    this.ShowEvents(state_machine_instance);
    this.ShowTransitions(state_machine_instance);
    this.ShowEnterActions(state_machine_instance);
    this.ShowExitActions(state_machine_instance);
    ImGui.Unindent();
  }

  private void ShowParameters(StateMachine.Instance state_machine_instance)
  {
    ImGui.Text("Parameters:");
    ImGui.Indent();
    foreach (StateMachine.Parameter.Context parameterContext in state_machine_instance.GetParameterContexts())
      parameterContext.ShowDevTool(state_machine_instance);
    ImGui.Unindent();
  }

  private void ShowEvents(StateMachine.Instance state_machine_instance)
  {
    StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
    ImGui.Text("Events: ");
    if (currentState == null)
      return;
    ImGui.Indent();
    for (int idx = 0; idx < currentState.GetStateCount(); ++idx)
    {
      StateMachine.BaseState state = currentState.GetState(idx);
      if (state.events != null)
      {
        foreach (StateEvent stateEvent in state.events)
          ImGui.Text(stateEvent.GetName());
      }
    }
    ImGui.Unindent();
  }

  private void ShowTransitions(StateMachine.Instance state_machine_instance)
  {
    StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
    ImGui.Text("Transitions:");
    if (currentState == null)
      return;
    ImGui.Indent();
    for (int idx = 0; idx < currentState.GetStateCount(); ++idx)
    {
      StateMachine.BaseState state = currentState.GetState(idx);
      if (state.transitions != null)
      {
        for (int index = 0; index < state.transitions.Count; ++index)
          ImGui.Text(state.transitions[index].ToString());
      }
    }
    ImGui.Unindent();
  }

  private void ShowExitActions(StateMachine.Instance state_machine_instance)
  {
    StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
    ImGui.Text("Exit Actions: ");
    if (currentState == null)
      return;
    ImGui.Indent();
    for (int idx = 0; idx < currentState.GetStateCount(); ++idx)
    {
      StateMachine.BaseState state = currentState.GetState(idx);
      if (state.exitActions != null)
      {
        foreach (StateMachine.Action exitAction in state.exitActions)
          ImGui.Text(exitAction.name);
      }
    }
    ImGui.Unindent();
  }

  private void ShowEnterActions(StateMachine.Instance state_machine_instance)
  {
    StateMachine.BaseState currentState = state_machine_instance.GetCurrentState();
    ImGui.Text("Enter Actions: ");
    if (currentState == null)
      return;
    ImGui.Indent();
    for (int idx = 0; idx < currentState.GetStateCount(); ++idx)
    {
      StateMachine.BaseState state = currentState.GetState(idx);
      if (state.enterActions != null)
      {
        foreach (StateMachine.Action enterAction in state.enterActions)
          ImGui.Text(enterAction.name);
      }
    }
    ImGui.Unindent();
  }

  private void ShowLog(StateMachine.Instance state_machine_instance) => ImGui.Text("Machine Log:");

  private void ShowKAnimControllerLog() => Object.op_Equality((Object) this.selectedGameObject.GetComponentInChildren<KAnimControllerBase>(), (Object) null);

  private void ShowHistory(StateMachineController controller) => ImGui.Text("Logger disabled");

  private void ShowControllerLog(StateMachineController controller) => ImGui.Text("Object Log:");

  private void ShowControllerLog(StateMachine.Instance state_machine)
  {
    if (state_machine.GetMaster().isNull)
      return;
    this.ShowControllerLog(state_machine.GetMaster().GetComponent<StateMachineController>());
  }
}
