// Decompiled with JetBrains decompiler
// Type: DevToolCommandPalette
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevToolCommandPalette : DevTool
{
  private int m_selected_index;
  private StringSearchableList<DevToolCommandPalette.Command> commands = new StringSearchableList<DevToolCommandPalette.Command>((StringSearchableList<DevToolCommandPalette.Command>.ShouldFilterOutFn) ((DevToolCommandPalette.Command command, in string filter) => !StringSearchableListUtil.DoAnyTagsMatchFilter(command.tags, in filter)));
  private bool m_should_focus_search = true;
  private bool shouldScrollToSelectedCommandFlag;

  public DevToolCommandPalette()
    : this((List<DevToolCommandPalette.Command>) null)
  {
  }

  public DevToolCommandPalette(List<DevToolCommandPalette.Command> commands = null)
  {
    this.drawFlags = (ImGuiWindowFlags) (this.drawFlags | 2);
    this.drawFlags = (ImGuiWindowFlags) (this.drawFlags | 8);
    this.drawFlags = (ImGuiWindowFlags) (this.drawFlags | 16);
    if (commands == null)
      this.commands.allValues = DevToolCommandPaletteUtil.GenerateDefaultCommandPalette();
    else
      this.commands.allValues = commands;
  }

  public static void Init() => DevToolCommandPalette.InitWithCommands(DevToolCommandPaletteUtil.GenerateDefaultCommandPalette());

  public static void InitWithCommands(List<DevToolCommandPalette.Command> commands) => DevToolManager.Instance.panels.AddPanelFor((DevTool) new DevToolCommandPalette(commands));

  protected override void RenderTo(DevPanel panel)
  {
    DevToolCommandPalette.Resize(panel);
    if (this.commands.allValues == null)
      ImGui.Text("No commands list given");
    else if (this.commands.allValues.Count == 0)
      ImGui.Text("Given command list is empty, no results to show.");
    else if (Input.GetKeyDown((KeyCode) 27))
      panel.Close();
    else if (!ImGui.IsWindowFocused((ImGuiFocusedFlags) 1))
    {
      panel.Close();
    }
    else
    {
      if (Input.GetKeyDown((KeyCode) 273))
      {
        --this.m_selected_index;
        this.shouldScrollToSelectedCommandFlag = true;
      }
      if (Input.GetKeyDown((KeyCode) 274))
      {
        ++this.m_selected_index;
        this.shouldScrollToSelectedCommandFlag = true;
      }
      if (this.commands.filteredValues.Count > 0)
      {
        while (this.m_selected_index < 0)
          this.m_selected_index += this.commands.filteredValues.Count;
        this.m_selected_index %= this.commands.filteredValues.Count;
      }
      else
        this.m_selected_index = 0;
      if ((Input.GetKeyUp((KeyCode) 13) || Input.GetKeyUp((KeyCode) 271)) && this.commands.filteredValues.Count > 0)
      {
        this.SelectCommand(this.commands.filteredValues[this.m_selected_index], panel);
      }
      else
      {
        if (this.m_should_focus_search)
          ImGui.SetKeyboardFocusHere();
        if (ImGui.InputText("Filter", ref this.commands.filter, 30U) || this.m_should_focus_search)
          this.commands.Refilter();
        this.m_should_focus_search = false;
        ImGui.Separator();
        string str = "Up arrow & down arrow to navigate. Enter to select. ";
        if (this.commands.filteredValues.Count > 0 && this.commands.didUseFilter)
          str += string.Format("Found {0} Results", (object) this.commands.filteredValues.Count);
        ImGui.Text(str);
        ImGui.Separator();
        if (!ImGui.BeginChild("ID_scroll_region"))
          return;
        if (this.commands.filteredValues.Count <= 0)
        {
          ImGui.Text("Couldn't find anything that matches \"" + this.commands.filter + "\", maybe it hasn't been added yet?");
        }
        else
        {
          for (int index = 0; index < this.commands.filteredValues.Count; ++index)
          {
            DevToolCommandPalette.Command filteredValue = this.commands.filteredValues[index];
            bool flag1 = index == this.m_selected_index;
            ImGui.PushID(index);
            bool flag2 = !flag1 ? ImGui.Selectable("  " + filteredValue.display_name) : ImGui.Selectable("> " + filteredValue.display_name);
            ImGui.PopID();
            if (this.shouldScrollToSelectedCommandFlag & flag1)
              ImGui.SetScrollHereY(0.5f);
            if (flag2)
            {
              this.SelectCommand(filteredValue, panel);
              return;
            }
          }
        }
        ImGui.EndChild();
      }
    }
  }

  private void SelectCommand(DevToolCommandPalette.Command command, DevPanel panel)
  {
    command.Internal_Select();
    panel.Close();
  }

  private static void Resize(DevPanel devToolPanel)
  {
    float num1 = 800f;
    float num2 = 400f;
    Rect rect1;
    // ISSUE: explicit constructor call
    ((Rect) ref rect1).\u002Ector(0.0f, 0.0f, (float) Screen.width, (float) Screen.height);
    Rect rect2 = new Rect();
    ((Rect) ref rect2).x = (float) ((double) ((Rect) ref rect1).x + (double) ((Rect) ref rect1).width / 2.0 - (double) num1 / 2.0);
    ((Rect) ref rect2).y = (float) ((double) ((Rect) ref rect1).y + (double) ((Rect) ref rect1).height / 2.0 - (double) num2 / 2.0);
    ((Rect) ref rect2).width = num1;
    ((Rect) ref rect2).height = num2;
    Rect rect3 = rect2;
    devToolPanel.SetPosition(((Rect) ref rect3).position);
    devToolPanel.SetSize(((Rect) ref rect3).size);
  }

  public class Command
  {
    public string display_name;
    public string[] tags;
    private System.Action m_on_select;

    public Command(string primary_tag, System.Action on_select)
      : this(new string[1]{ primary_tag }, on_select)
    {
    }

    public Command(string primary_tag, string tag_a, System.Action on_select)
      : this(new string[2]{ primary_tag, tag_a }, on_select)
    {
    }

    public Command(string primary_tag, string tag_a, string tag_b, System.Action on_select)
      : this(new string[3]{ primary_tag, tag_a, tag_b }, on_select)
    {
    }

    public Command(
      string primary_tag,
      string tag_a,
      string tag_b,
      string tag_c,
      System.Action on_select)
      : this(new string[4]
      {
        primary_tag,
        tag_a,
        tag_b,
        tag_c
      }, on_select)
    {
    }

    public Command(
      string primary_tag,
      string tag_a,
      string tag_b,
      string tag_c,
      string tag_d,
      System.Action on_select)
      : this(new string[5]
      {
        primary_tag,
        tag_a,
        tag_b,
        tag_c,
        tag_d
      }, on_select)
    {
    }

    public Command(
      string primary_tag,
      string tag_a,
      string tag_b,
      string tag_c,
      string tag_d,
      string tag_e,
      System.Action on_select)
      : this(new string[6]
      {
        primary_tag,
        tag_a,
        tag_b,
        tag_c,
        tag_d,
        tag_e
      }, on_select)
    {
    }

    public Command(
      string primary_tag,
      string tag_a,
      string tag_b,
      string tag_c,
      string tag_d,
      string tag_e,
      string tag_f,
      System.Action on_select)
      : this(new string[7]
      {
        primary_tag,
        tag_a,
        tag_b,
        tag_c,
        tag_d,
        tag_e,
        tag_f
      }, on_select)
    {
    }

    public Command(string primary_tag, string[] additional_tags, System.Action on_select)
      : this(((IEnumerable<string>) new string[1]
      {
        primary_tag
      }).Concat<string>((IEnumerable<string>) additional_tags).ToArray<string>(), on_select)
    {
    }

    public Command(string[] tags, System.Action on_select)
    {
      this.display_name = tags[0];
      this.tags = ((IEnumerable<string>) tags).Select<string, string>((Func<string, string>) (t => t.ToLowerInvariant())).ToArray<string>();
      this.m_on_select = on_select;
    }

    public void Internal_Select()
    {
      try
      {
        this.m_on_select();
      }
      catch (Exception ex)
      {
        Debug.LogException(ex);
      }
    }
  }
}
