// Decompiled with JetBrains decompiler
// Type: DevToolUnlockedIds
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevToolUnlockedIds : DevTool
{
  private string filterForUnlockIds = "";
  private string unlockIdToAdd = "";

  public DevToolUnlockedIds() => this.RequiresGameRunning = true;

  protected override void RenderTo(DevPanel panel)
  {
    (bool hasValue, DevToolUnlockedIds.UnlocksWrapper unlocksWrapper1) = this.GetUnlocks();
    int num1 = hasValue ? 1 : 0;
    DevToolUnlockedIds.UnlocksWrapper unlocksWrapper2 = unlocksWrapper1;
    if (num1 == 0)
    {
      ImGui.Text("Couldn't access global unlocks");
    }
    else
    {
      if (ImGui.TreeNode("Help"))
      {
        ImGui.TextWrapped("This is a list of global unlocks that are persistant across saves. Changes made here will be saved to disk immediately.");
        ImGui.Spacing();
        ImGui.TextWrapped("NOTE: It may be necessary to relaunch the game after modifying unlocks in order for systems to respond.");
        ImGui.TreePop();
      }
      ImGui.Spacing();
      ImGuiEx.InputFilter("Filter", ref this.filterForUnlockIds, 50U);
      if (!ImGui.BeginTable("ID_unlockIds", 2, (ImGuiTableFlags) 33556416))
        return;
      ImGui.TableSetupScrollFreeze(2, 2);
      ImGui.TableSetupColumn("Unlock ID");
      ImGui.TableSetupColumn("Actions", (ImGuiTableColumnFlags) 8);
      ImGui.TableHeadersRow();
      ImGui.PushID("ID_row_add_new");
      ImGui.TableNextRow();
      ImGui.TableSetColumnIndex(0);
      ImGui.InputText("", ref this.unlockIdToAdd, 50U);
      ImGui.TableSetColumnIndex(1);
      if (ImGui.Button("Add"))
      {
        unlocksWrapper2.AddId(this.unlockIdToAdd);
        Debug.Log((object) ("[Added unlock id] " + this.unlockIdToAdd));
        this.unlockIdToAdd = "";
      }
      ImGui.PopID();
      int num2 = 0;
      foreach (string allId in unlocksWrapper2.GetAllIds())
      {
        string str = allId == null ? "<<null>>" : "\"" + allId + "\"";
        if (str.ToLower().Contains(this.filterForUnlockIds.ToLower()))
        {
          ImGui.TableNextRow();
          ImGui.PushID(string.Format("ID_row_{0}", (object) num2++));
          ImGui.TableSetColumnIndex(0);
          ImGui.Text(str);
          ImGui.TableSetColumnIndex(1);
          if (ImGui.Button("Copy"))
          {
            GUIUtility.systemCopyBuffer = allId;
            Debug.Log((object) ("[Copied to clipboard] " + allId));
          }
          ImGui.SameLine();
          if (ImGui.Button("Remove"))
          {
            unlocksWrapper2.RemoveId(allId);
            Debug.Log((object) ("[Removed unlock id] " + allId));
          }
          ImGui.PopID();
        }
      }
      ImGui.EndTable();
    }
  }

  private Option<DevToolUnlockedIds.UnlocksWrapper> GetUnlocks()
  {
    if (App.IsExiting)
      return (Option<DevToolUnlockedIds.UnlocksWrapper>) Option.None;
    if (Object.op_Equality((Object) Game.Instance, (Object) null) || !Object.op_Implicit((Object) Game.Instance))
      return (Option<DevToolUnlockedIds.UnlocksWrapper>) Option.None;
    return Object.op_Equality((Object) Game.Instance.unlocks, (Object) null) ? (Option<DevToolUnlockedIds.UnlocksWrapper>) Option.None : Option.Some<DevToolUnlockedIds.UnlocksWrapper>(new DevToolUnlockedIds.UnlocksWrapper(Game.Instance.unlocks));
  }

  public readonly struct UnlocksWrapper
  {
    public readonly Unlocks unlocks;

    public UnlocksWrapper(Unlocks unlocks) => this.unlocks = unlocks;

    public void AddId(string unlockId) => this.unlocks.Unlock(unlockId);

    public void RemoveId(string unlockId) => this.unlocks.Lock(unlockId);

    public IEnumerable<string> GetAllIds() => (IEnumerable<string>) this.unlocks.GetAllUnlockedIds().OrderBy<string, string>((Func<string, string>) (s => s));

    public int Count => this.unlocks.GetAllUnlockedIds().Count;
  }
}
