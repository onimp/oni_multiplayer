// Decompiled with JetBrains decompiler
// Type: DevToolSceneBrowser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevToolSceneBrowser : DevTool
{
  private List<DevToolSceneBrowser.StackItem> Stack = new List<DevToolSceneBrowser.StackItem>();
  private int StackIndex;
  private static int SelectedIndex = -1;
  private static string SearchFilter = "";
  private static List<GameObject> SearchResults = new List<GameObject>();
  private static int SearchSelectedIndex = -1;

  public DevToolSceneBrowser()
  {
    this.drawFlags = (ImGuiWindowFlags) 1024;
    this.Stack.Add(new DevToolSceneBrowser.StackItem()
    {
      SceneRoot = true,
      Filter = ""
    });
  }

  private void PushGameObject(GameObject go)
  {
    if (this.StackIndex < this.Stack.Count && Object.op_Equality((Object) go, (Object) this.Stack[this.StackIndex].Root))
      return;
    if (this.Stack.Count > this.StackIndex + 1)
      this.Stack.RemoveRange(this.StackIndex + 1, this.Stack.Count - (this.StackIndex + 1));
    this.Stack.Add(new DevToolSceneBrowser.StackItem()
    {
      SceneRoot = Object.op_Equality((Object) go, (Object) null),
      Root = go,
      Filter = ""
    });
    ++this.StackIndex;
  }

  protected override void RenderTo(DevPanel panel)
  {
    for (int index = this.Stack.Count - 1; index > 0; --index)
    {
      DevToolSceneBrowser.StackItem stackItem = this.Stack[index];
      if (!stackItem.SceneRoot && Util.IsNullOrDestroyed((object) stackItem.Root))
      {
        this.Stack.RemoveAt(index);
        this.StackIndex = Math.Min(index - 1, this.StackIndex);
      }
    }
    bool flag1 = false;
    if (ImGui.BeginMenuBar())
    {
      if (ImGui.BeginMenu("Utils"))
      {
        if (ImGui.MenuItem("Goto current selection") && Object.op_Inequality((Object) ((Component) SelectTool.Instance?.selected)?.gameObject, (Object) null))
          this.PushGameObject(((Component) SelectTool.Instance?.selected)?.gameObject);
        if (ImGui.MenuItem("Search All"))
          flag1 = true;
        ImGui.EndMenu();
      }
      ImGui.EndMenuBar();
    }
    if (ImGui.Button(" < ") && this.StackIndex > 0)
      --this.StackIndex;
    ImGui.SameLine();
    if (ImGui.Button(" ^ ") && this.StackIndex > 0 && !this.Stack[this.StackIndex].SceneRoot)
      this.PushGameObject(((Component) this.Stack[this.StackIndex].Root.transform.parent)?.gameObject);
    ImGui.SameLine();
    if (ImGui.Button(" > ") && this.StackIndex + 1 < this.Stack.Count)
      ++this.StackIndex;
    DevToolSceneBrowser.StackItem stackItem1 = this.Stack[this.StackIndex];
    if (!stackItem1.SceneRoot)
    {
      ImGui.SameLine();
      if (ImGui.Button("Inspect"))
        DevToolSceneInspector.Inspect((object) stackItem1.Root);
    }
    List<GameObject> gameObjectList;
    if (stackItem1.SceneRoot)
    {
      ImGui.Text("Scene root");
      Scene activeScene = SceneManager.GetActiveScene();
      gameObjectList = new List<GameObject>(((Scene) ref activeScene).rootCount);
      ((Scene) ref activeScene).GetRootGameObjects(gameObjectList);
    }
    else
    {
      ImGui.LabelText("Selected object", ((Object) stackItem1.Root).name);
      gameObjectList = new List<GameObject>();
      foreach (Transform transform in stackItem1.Root.transform)
      {
        if (Object.op_Inequality((Object) ((Component) transform).gameObject, (Object) stackItem1.Root))
          gameObjectList.Add(((Component) transform).gameObject);
      }
    }
    if (ImGui.Button("Clear"))
      stackItem1.Filter = "";
    ImGui.SameLine();
    ImGui.InputText("Filter", ref stackItem1.Filter, 64U);
    ImGui.BeginChild("ScrollRegion", new Vector2(0.0f, 0.0f), true, (ImGuiWindowFlags) 0);
    for (int index = 0; index < gameObjectList.Count; ++index)
    {
      GameObject go = gameObjectList[index];
      if (!(stackItem1.Filter != "") || ((Object) go).name.IndexOf(stackItem1.Filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
      {
        if (ImGui.Selectable(((Object) go).name, false, (ImGuiSelectableFlags) 4) && ImGui.IsMouseDoubleClicked((ImGuiMouseButton) 0))
          this.PushGameObject(go);
        if (ImGui.IsItemClicked((ImGuiMouseButton) 1))
        {
          DevToolSceneBrowser.SelectedIndex = index;
          ImGui.OpenPopup("RightClickMenu");
        }
      }
    }
    if (ImGui.BeginPopup("RightClickMenu"))
    {
      if (ImGui.MenuItem("Inspect"))
      {
        DevToolSceneInspector.Inspect((object) gameObjectList[DevToolSceneBrowser.SelectedIndex]);
        DevToolSceneBrowser.SelectedIndex = -1;
      }
      ImGui.EndPopup();
    }
    ImGui.EndChild();
    if (flag1)
      ImGui.OpenPopup("SearchAll");
    if (!ImGui.BeginPopupModal("SearchAll"))
      return;
    ImGui.Text("Search all objects in the scene:");
    ImGui.Separator();
    if (ImGui.Button("Clear"))
      DevToolSceneBrowser.SearchFilter = "";
    ImGui.SameLine();
    if (ImGui.InputText("Filter", ref DevToolSceneBrowser.SearchFilter, 64U))
      DevToolSceneBrowser.SearchResults = ((IEnumerable<GameObject>) ((IEnumerable<GameObject>) Object.FindObjectsOfType<GameObject>()).Where<GameObject>((Func<GameObject, bool>) (go => ((Object) go).name.IndexOf(DevToolSceneBrowser.SearchFilter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)).OrderBy<GameObject, string>((Func<GameObject, string>) (go => ((Object) go).name))).ToList<GameObject>();
    ImGui.BeginChild("ScrollRegion", new Vector2(0.0f, 200f), true, (ImGuiWindowFlags) 0);
    int num = 0;
    foreach (Object searchResult in DevToolSceneBrowser.SearchResults)
    {
      if (ImGui.Selectable(searchResult.name, DevToolSceneBrowser.SearchSelectedIndex == num))
        DevToolSceneBrowser.SearchSelectedIndex = num;
      ++num;
    }
    ImGui.EndChild();
    bool flag2 = false;
    if (ImGui.Button("Browse") && DevToolSceneBrowser.SearchSelectedIndex >= 0)
    {
      this.PushGameObject(DevToolSceneBrowser.SearchResults[DevToolSceneBrowser.SearchSelectedIndex]);
      flag2 = true;
    }
    ImGui.SameLine();
    if (ImGui.Button("Inspect") && DevToolSceneBrowser.SearchSelectedIndex >= 0)
    {
      DevToolSceneInspector.Inspect((object) DevToolSceneBrowser.SearchResults[DevToolSceneBrowser.SearchSelectedIndex]);
      flag2 = true;
    }
    ImGui.SameLine();
    if (ImGui.Button("Cancel"))
      flag2 = true;
    if (flag2)
    {
      DevToolSceneBrowser.SearchFilter = "";
      DevToolSceneBrowser.SearchResults.Clear();
      DevToolSceneBrowser.SearchSelectedIndex = -1;
      ImGui.CloseCurrentPopup();
    }
    ImGui.EndPopup();
  }

  private class StackItem
  {
    public bool SceneRoot;
    public GameObject Root;
    public string Filter;
  }
}
