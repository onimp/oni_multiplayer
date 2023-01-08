// Decompiled with JetBrains decompiler
// Type: DevToolChoreDebugger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class DevToolChoreDebugger : DevTool
{
  private string filter = "";
  private bool showLastSuccessfulPreconditionSnapshot;
  private bool lockSelection;
  private ChoreConsumer Consumer;
  private GameObject selectedGameObject;
  private OrderedDictionary columns = new OrderedDictionary()
  {
    {
      (object) "BP",
      (object) ""
    },
    {
      (object) "Id",
      (object) ""
    },
    {
      (object) "Class",
      (object) ""
    },
    {
      (object) "Type",
      (object) ""
    },
    {
      (object) "PriorityClass",
      (object) ""
    },
    {
      (object) "PersonalPriority",
      (object) ""
    },
    {
      (object) "PriorityValue",
      (object) ""
    },
    {
      (object) "Priority",
      (object) ""
    },
    {
      (object) "PriorityMod",
      (object) ""
    },
    {
      (object) "ConsumerPriority",
      (object) ""
    },
    {
      (object) "Cost",
      (object) ""
    },
    {
      (object) "Interrupt",
      (object) ""
    },
    {
      (object) "Precondition",
      (object) ""
    },
    {
      (object) "Override",
      (object) ""
    },
    {
      (object) "Assigned To",
      (object) ""
    },
    {
      (object) "Owner",
      (object) ""
    },
    {
      (object) "Details",
      (object) ""
    }
  };
  private int rowIndex;

  protected override void RenderTo(DevPanel panel) => this.Update();

  public void Update()
  {
    if (!Application.isPlaying || Object.op_Equality((Object) SelectTool.Instance, (Object) null) || Object.op_Equality((Object) SelectTool.Instance.selected, (Object) null) || Object.op_Equality((Object) ((Component) SelectTool.Instance.selected).gameObject, (Object) null))
      return;
    GameObject gameObject = ((Component) SelectTool.Instance.selected).gameObject;
    if (Object.op_Equality((Object) this.Consumer, (Object) null) || !this.lockSelection && Object.op_Inequality((Object) this.selectedGameObject, (Object) gameObject))
    {
      this.Consumer = gameObject.GetComponent<ChoreConsumer>();
      this.selectedGameObject = gameObject;
    }
    if (!Object.op_Inequality((Object) this.Consumer, (Object) null))
      return;
    ImGui.InputText("Filter:", ref this.filter, 256U);
    this.DisplayAvailableChores();
    ImGui.Text("");
  }

  private void DisplayAvailableChores()
  {
    ImGui.Checkbox("Lock selection", ref this.lockSelection);
    ImGui.Checkbox("Show Last Successful Chore Selection", ref this.showLastSuccessfulPreconditionSnapshot);
    ImGui.Text("Available Chores:");
    ChoreConsumer.PreconditionSnapshot preconditionSnapshot = this.Consumer.GetLastPreconditionSnapshot();
    if (this.showLastSuccessfulPreconditionSnapshot)
      preconditionSnapshot = this.Consumer.GetLastSuccessfulPreconditionSnapshot();
    this.ShowChores(preconditionSnapshot);
  }

  private void ShowChores(ChoreConsumer.PreconditionSnapshot target_snapshot)
  {
    ImGuiTableFlags imGuiTableFlags = (ImGuiTableFlags) 50341824;
    this.rowIndex = 0;
    if (!ImGui.BeginTable("Available Chores", this.columns.Count, imGuiTableFlags))
      return;
    foreach (object key in (IEnumerable) this.columns.Keys)
      ImGui.TableSetupColumn(key.ToString(), (ImGuiTableColumnFlags) 8);
    ImGui.TableHeadersRow();
    for (int index = target_snapshot.succeededContexts.Count - 1; index >= 0; --index)
      this.ShowContext(target_snapshot.succeededContexts[index]);
    if (target_snapshot.doFailedContextsNeedSorting)
    {
      target_snapshot.failedContexts.Sort();
      target_snapshot.doFailedContextsNeedSorting = false;
    }
    for (int index = target_snapshot.failedContexts.Count - 1; index >= 0; --index)
      this.ShowContext(target_snapshot.failedContexts[index]);
    ImGui.EndTable();
  }

  private void ShowContext(Chore.Precondition.Context context)
  {
    string text1 = "";
    Chore chore = context.chore;
    if (!context.IsSuccess())
      text1 = context.chore.GetPreconditions()[context.failedPreconditionId].id;
    string text2 = "";
    if (Object.op_Inequality((Object) chore.driver, (Object) null))
      text2 = ((Object) chore.driver).name;
    string text3 = "";
    if (Object.op_Inequality((Object) chore.overrideTarget, (Object) null))
      text3 = ((Object) chore.overrideTarget).name;
    string text4 = "";
    if (!chore.isNull)
      text4 = ((Object) chore.gameObject).name;
    if (Chore.Precondition.Context.ShouldFilter(this.filter, chore.GetType().ToString()) && Chore.Precondition.Context.ShouldFilter(this.filter, chore.choreType.Id) && Chore.Precondition.Context.ShouldFilter(this.filter, text1) && Chore.Precondition.Context.ShouldFilter(this.filter, text2) && Chore.Precondition.Context.ShouldFilter(this.filter, text3) && Chore.Precondition.Context.ShouldFilter(this.filter, text4))
      return;
    this.columns[(object) "BP"] = (object) chore.debug;
    this.columns[(object) "Id"] = (object) chore.id.ToString();
    this.columns[(object) "Class"] = (object) chore.GetType().ToString().Replace("`1", "");
    this.columns[(object) "Type"] = (object) chore.choreType.Id;
    this.columns[(object) "PriorityClass"] = (object) context.masterPriority.priority_class.ToString();
    this.columns[(object) "PersonalPriority"] = (object) context.personalPriority.ToString();
    this.columns[(object) "PriorityValue"] = (object) context.masterPriority.priority_value.ToString();
    this.columns[(object) "Priority"] = (object) context.priority.ToString();
    this.columns[(object) "PriorityMod"] = (object) context.priorityMod.ToString();
    this.columns[(object) "ConsumerPriority"] = (object) context.consumerPriority.ToString();
    this.columns[(object) "Cost"] = (object) context.cost.ToString();
    this.columns[(object) "Interrupt"] = (object) context.interruptPriority.ToString();
    this.columns[(object) "Precondition"] = (object) text1;
    this.columns[(object) "Override"] = (object) text3;
    this.columns[(object) "Assigned To"] = (object) text2;
    this.columns[(object) "Owner"] = (object) text4;
    this.columns[(object) "Details"] = (object) "";
    ImGui.TableNextRow();
    ImGui.PushID(string.Format("ID_row_{0}", (object) this.rowIndex++));
    int num1 = 0;
    ImGui.PushID("debug");
    int num2 = num1;
    int index = num2 + 1;
    ImGui.TableSetColumnIndex(num2);
    ImGui.Checkbox("", ref chore.debug);
    ImGui.PopID();
    for (; index < this.columns.Count; ++index)
    {
      ImGui.TableSetColumnIndex(index);
      ImGui.Text(this.columns[index].ToString());
    }
    ImGui.PopID();
  }

  public void ConsumerDebugDisplayLog()
  {
  }

  public class EditorPreconditionSnapshot
  {
    public List<DevToolChoreDebugger.EditorPreconditionSnapshot.EditorContext> SucceededContexts { get; set; }

    public List<DevToolChoreDebugger.EditorPreconditionSnapshot.EditorContext> FailedContexts { get; set; }

    public struct EditorContext
    {
      public string Chore { get; set; }

      public string ChoreType { get; set; }

      public string FailedPrecondition { get; set; }

      public int WorldId { get; set; }
    }
  }
}
