// Decompiled with JetBrains decompiler
// Type: DevToolSceneInspector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class DevToolSceneInspector : DevTool
{
  private List<DevToolSceneInspector.StackItem> Stack = new List<DevToolSceneInspector.StackItem>();
  private int StackIndex = -1;
  private Dictionary<System.Type, DevToolSceneInspector.ViewInfo> CustomTypeViews;

  public DevToolSceneInspector()
  {
    this.drawFlags = (ImGuiWindowFlags) 1024;
    this.CustomTypeViews = new Dictionary<System.Type, DevToolSceneInspector.ViewInfo>()
    {
      {
        typeof (GameObject),
        new DevToolSceneInspector.ViewInfo("Components", (Action<object, string>) ((o, f) => this.CustomGameObjectDisplay(o, f)))
      },
      {
        typeof (KPrefabID),
        new DevToolSceneInspector.ViewInfo("Prefab tags", (Action<object, string>) ((o, f) => this.CustomPrefabTagView(o, f)))
      }
    };
  }

  public static void Inspect(object obj) => DevToolManager.Instance.panels.AddOrGetDevTool<DevToolSceneInspector>().PushObject(obj);

  public void PushObject(object obj)
  {
    if (obj == null || this.StackIndex >= 0 && this.StackIndex < this.Stack.Count && obj == this.Stack[this.StackIndex].Obj)
      return;
    if (this.Stack.Count > this.StackIndex + 1)
      this.Stack.RemoveRange(this.StackIndex + 1, this.Stack.Count - (this.StackIndex + 1));
    this.Stack.Add(new DevToolSceneInspector.StackItem()
    {
      Obj = obj,
      Filter = ""
    });
    ++this.StackIndex;
  }

  protected override void RenderTo(DevPanel panel)
  {
    for (int index = this.Stack.Count - 1; index >= 0; --index)
    {
      if (Util.IsNullOrDestroyed(this.Stack[index].Obj))
      {
        this.Stack.RemoveAt(index);
        if (this.StackIndex >= index)
          --this.StackIndex;
      }
    }
    if (ImGui.BeginMenuBar())
    {
      if (ImGui.BeginMenu("Utils"))
      {
        if (ImGui.MenuItem("Goto current selection") && Object.op_Inequality((Object) ((Component) SelectTool.Instance?.selected)?.gameObject, (Object) null))
          this.PushObject((object) ((Component) SelectTool.Instance?.selected)?.gameObject);
        ImGui.EndMenu();
      }
      ImGui.EndMenuBar();
    }
    if (ImGui.Button(" < ") && this.StackIndex > 0)
      --this.StackIndex;
    ImGui.SameLine();
    if (ImGui.Button(" > ") && this.StackIndex + 1 < this.Stack.Count)
      ++this.StackIndex;
    if (this.Stack.Count == 0)
    {
      ImGui.Text("No Selection.");
    }
    else
    {
      DevToolSceneInspector.StackItem stackItem = this.Stack[this.StackIndex];
      object obj1 = stackItem.Obj;
      System.Type type = obj1.GetType();
      ImGui.LabelText("Type", type.Name);
      if (ImGui.Button("Clear"))
        stackItem.Filter = "";
      ImGui.SameLine();
      ImGui.InputText("Filter", ref stackItem.Filter, 64U);
      ImGui.PushID(this.StackIndex);
      if (ImGui.BeginTabBar("##tabs", (ImGuiTabBarFlags) 0))
      {
        DevToolSceneInspector.ViewInfo viewInfo;
        if (this.CustomTypeViews.TryGetValue(type, out viewInfo) && ImGui.BeginTabItem(viewInfo.Name))
        {
          viewInfo.Callback(obj1, stackItem.Filter);
          ImGui.EndTabItem();
        }
        if (ImGui.BeginTabItem("Raw view"))
        {
          ImGui.BeginChild("ScrollRegion", new Vector2(0.0f, 0.0f), true, (ImGuiWindowFlags) 0);
          if (obj1 is IEnumerable)
          {
            IEnumerator enumerator = (obj1 as IEnumerable).GetEnumerator();
            int num = 0;
            while (enumerator.MoveNext())
            {
              object current = enumerator.Current;
              this.DisplayField("[" + num.ToString() + "]", current.GetType(), ref current);
              ++num;
            }
          }
          else
          {
            foreach (FieldInfo field in type.GetFields())
            {
              object obj2 = field.GetValue(obj1);
              System.Type fieldType = field.FieldType;
              if (field.GetCustomAttributes(typeof (ObsoleteAttribute), false).Length == 0 && (!(stackItem.Filter != "") || field.Name.IndexOf(stackItem.Filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1 || fieldType.Name.IndexOf(stackItem.Filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1) && this.DisplayField(field.Name, fieldType, ref obj2) && !field.IsLiteral && !field.IsInitOnly)
                field.SetValue(obj1, obj2);
            }
            BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (PropertyInfo property in type.GetProperties(bindingAttr))
            {
              if (!property.CanRead)
                ImGui.LabelText(property.Name, "Unreadable");
              else if (property.GetIndexParameters().Length == 0 && property.GetCustomAttributes(typeof (ObsoleteAttribute), false).Length == 0)
              {
                System.Type propertyType = property.PropertyType;
                object obj3 = property.GetValue(obj1);
                if ((!(stackItem.Filter != "") || property.Name.IndexOf(stackItem.Filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1 || propertyType.Name.IndexOf(stackItem.Filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1) && this.DisplayField(property.Name, propertyType, ref obj3) && property.CanWrite)
                  property.SetValue(obj1, obj3);
              }
            }
          }
          ImGui.EndChild();
          ImGui.EndTabItem();
        }
        ImGui.EndTabBar();
      }
      ImGui.PopID();
    }
  }

  private bool DisplayField(string name, System.Type ft, ref object obj)
  {
    bool flag1 = false;
    if (obj == null)
      ImGui.LabelText(name, "null");
    else if (ft == typeof (int))
    {
      int num = (int) obj;
      if (ImGui.InputInt(name, ref num))
      {
        obj = (object) num;
        flag1 = true;
      }
    }
    else if (ft == typeof (uint))
    {
      int val1 = (int) (uint) obj;
      if (ImGui.InputInt(name, ref val1))
      {
        obj = (object) (uint) Math.Max(val1, 0);
        flag1 = true;
      }
    }
    else if (ft == typeof (bool))
    {
      bool flag2 = (bool) obj;
      if (ImGui.Checkbox(name, ref flag2))
      {
        obj = (object) flag2;
        flag1 = true;
      }
    }
    else if (ft == typeof (float))
    {
      float num = (float) obj;
      if (ImGui.InputFloat(name, ref num))
      {
        obj = (object) num;
        flag1 = true;
      }
    }
    else if (ft == typeof (Vector2))
    {
      Vector2 vector2 = (Vector2) obj;
      if (ImGui.InputFloat2(name, ref vector2))
      {
        obj = (object) vector2;
        flag1 = true;
      }
    }
    else if (ft == typeof (Vector3))
    {
      Vector3 vector3 = (Vector3) obj;
      if (ImGui.InputFloat3(name, ref vector3))
      {
        obj = (object) vector3;
        flag1 = true;
      }
    }
    else if (ft == typeof (string))
    {
      string str = (string) obj;
      if (ImGui.InputText(name, ref str, 256U))
      {
        obj = (object) str;
        flag1 = true;
      }
    }
    else if (ImGui.Selectable(name + " (" + ft.Name + ")", false, (ImGuiSelectableFlags) 4) && ImGui.IsMouseDoubleClicked((ImGuiMouseButton) 0))
      this.PushObject(obj);
    return flag1;
  }

  private void CustomGameObjectDisplay(object obj, string filter)
  {
    GameObject gameObject = (GameObject) obj;
    ImGui.BeginChild("ScrollRegion", new Vector2(0.0f, 0.0f), true, (ImGuiWindowFlags) 0);
    int num = 0;
    foreach (Behaviour component in gameObject.GetComponents<Behaviour>())
    {
      System.Type type = ((object) component).GetType();
      if (!(filter != "") || type.Name.IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
      {
        ImGui.PushID(num++);
        bool enabled = component.enabled;
        if (ImGui.Checkbox("", ref enabled))
          component.enabled = enabled;
        ImGui.PopID();
        ImGui.SameLine();
        if (ImGui.Selectable(type.Name, false, (ImGuiSelectableFlags) 4) && ImGui.IsMouseDoubleClicked((ImGuiMouseButton) 0))
          this.PushObject((object) component);
      }
    }
    ImGui.EndChild();
  }

  private void CustomPrefabTagView(object obj, string filter)
  {
    KPrefabID kprefabId = (KPrefabID) obj;
    ImGui.BeginChild("ScrollRegion", new Vector2(0.0f, 0.0f), true, (ImGuiWindowFlags) 0);
    string name1 = ((Tag) ref kprefabId.PrefabTag).Name;
    ImGui.InputText("PrefabID: ", ref name1, 128U);
    int num = 0;
    foreach (Tag tag in kprefabId.Tags)
    {
      string name2 = ((Tag) ref tag).Name;
      if (!(filter != "") || name2.IndexOf(filter, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
      {
        ImGui.InputText("[" + num.ToString() + "]", ref name2, 128U);
        ++num;
      }
    }
    ImGui.EndChild();
  }

  private class StackItem
  {
    public object Obj;
    public string Filter;
  }

  private class ViewInfo
  {
    public string Name;
    public Action<object, string> Callback;

    public ViewInfo(string s, Action<object, string> a)
    {
      this.Name = s;
      this.Callback = a;
    }
  }
}
