// Decompiled with JetBrains decompiler
// Type: DevToolAnimEventManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using ImGuiObjectDrawer;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DevToolAnimEventManager : DevTool
{
  protected override void RenderTo(DevPanel panel)
  {
    (Option<AnimEventManager.DevTools_DebugInfo> value, string error) managerDebugInfo = this.GetAnimEventManagerDebugInfo();
    (bool hasValue, AnimEventManager.DevTools_DebugInfo devToolsDebugInfo1) = managerDebugInfo.value;
    int num = hasValue ? 1 : 0;
    AnimEventManager.DevTools_DebugInfo devToolsDebugInfo2 = devToolsDebugInfo1;
    string error = managerDebugInfo.error;
    if (num == 0)
    {
      ImGui.Text(error);
    }
    else
    {
      if (ImGui.CollapsingHeader("World space animations", (ImGuiTreeNodeFlags) 32))
        this.DrawFor("ID_world_space_anims", devToolsDebugInfo2.eventData.GetDataList(), devToolsDebugInfo2.animData.GetDataList());
      if (ImGui.CollapsingHeader("UI space animations", (ImGuiTreeNodeFlags) 32))
        this.DrawFor("ID_ui_space_anims", devToolsDebugInfo2.uiEventData.GetDataList(), devToolsDebugInfo2.uiAnimData.GetDataList());
      if (!ImGui.CollapsingHeader("Raw AnimEventManger", (ImGuiTreeNodeFlags) 32))
        return;
      ImGuiEx.DrawObject("Anim Event Manager", (object) devToolsDebugInfo2.eventManager, new MemberDrawContext?());
    }
  }

  public void DrawFor(
    string uniqueTableId,
    List<AnimEventManager.EventPlayerData> eventDataList,
    List<AnimEventManager.AnimData> animDataList)
  {
    if (eventDataList == null)
      ImGui.Text("Can't draw table: eventData is null");
    else if (animDataList == null)
      ImGui.Text("Can't draw table: animData is null");
    else if (eventDataList.Count != animDataList.Count)
    {
      ImGui.Text(string.Format("Can't draw table: eventData.Count ({0}) != animData.Count ({1})", (object) eventDataList.Count, (object) animDataList.Count));
    }
    else
    {
      int count = eventDataList.Count;
      ImGui.PushID(uniqueTableId);
      ImGuiStoragePtr stateStorage = ImGui.GetStateStorage();
      uint id = ImGui.GetID("ID_should_expand_full_height");
      bool flag = ((ImGuiStoragePtr) ref stateStorage).GetBool(id);
      if (ImGui.Button(flag ? "Unexpand Height" : "Expand Height"))
      {
        flag = !flag;
        ((ImGuiStoragePtr) ref stateStorage).SetBool(id, flag);
      }
      if (ImGui.BeginTable("ID_table_contents", 4, (ImGuiTableFlags) 33564609, new Vector2(-1f, flag ? -1f : 400f)))
      {
        ImGui.TableSetupScrollFreeze(4, 1);
        ImGui.TableSetupColumn("Game Object Name");
        ImGui.TableSetupColumn("Event Frame");
        ImGui.TableSetupColumn("Animation Frame");
        ImGui.TableSetupColumn("Event - Animation Frame Diff");
        ImGui.TableHeadersRow();
        int num1;
        for (int index = 0; index < count; index = num1 + 1)
        {
          AnimEventManager.EventPlayerData eventData = eventDataList[index];
          AnimEventManager.AnimData animData = animDataList[index];
          ImGui.TableNextRow();
          int num2 = index;
          num1 = num2 + 1;
          ImGui.PushID(string.Format("ID_row_{0}", (object) num2));
          ImGui.TableNextColumn();
          if (ImGuiEx.Button("Focus", DevToolUtil.CanRevealAndFocus(((Component) eventData.controller).gameObject)))
            DevToolUtil.RevealAndFocus(((Component) eventData.controller).gameObject);
          ImGuiEx.TooltipForPrevious("Will move the in-game camera to this gameobject");
          ImGui.SameLine();
          ImGui.Text(UI.StripLinkFormatting(((Object) ((Component) eventData.controller).gameObject).name));
          ImGui.TableNextColumn();
          int num3 = eventData.currentFrame;
          ImGui.Text(num3.ToString());
          ImGui.TableNextColumn();
          num3 = eventData.controller.currentFrame;
          ImGui.Text(num3.ToString());
          ImGui.TableNextColumn();
          num3 = eventData.currentFrame - eventData.controller.currentFrame;
          ImGui.Text(num3.ToString());
          ImGui.PopID();
        }
        ImGui.EndTable();
      }
      ImGui.PopID();
    }
  }

  public (Option<AnimEventManager.DevTools_DebugInfo> value, string error) GetAnimEventManagerDebugInfo() => Singleton<AnimEventManager>.Instance == null ? ((Option<AnimEventManager.DevTools_DebugInfo>) Option.None, "AnimEventManager is null") : (Option.Some<AnimEventManager.DevTools_DebugInfo>(Singleton<AnimEventManager>.Instance.DevTools_GetDebugInfo()), (string) null);
}
