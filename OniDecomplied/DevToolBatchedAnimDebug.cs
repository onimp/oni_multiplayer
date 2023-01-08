// Decompiled with JetBrains decompiler
// Type: DevToolBatchedAnimDebug
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DevToolBatchedAnimDebug : DevTool
{
  private GameObject Selection;
  private bool LockSelection;
  private string Filter = "";
  private int FrameIndex;
  private int FrameElementIndex;

  public DevToolBatchedAnimDebug() => this.drawFlags = (ImGuiWindowFlags) 1024;

  protected override void RenderTo(DevPanel panel)
  {
    if (ImGui.BeginMenuBar())
    {
      ImGui.Checkbox("Lock selection", ref this.LockSelection);
      ImGui.EndMenuBar();
    }
    if (!this.LockSelection)
      this.Selection = ((Component) SelectTool.Instance?.selected)?.gameObject;
    if (Object.op_Equality((Object) this.Selection, (Object) null))
    {
      ImGui.Text("No selection.");
    }
    else
    {
      KBatchedAnimController component1 = this.Selection.GetComponent<KBatchedAnimController>();
      if (Object.op_Equality((Object) component1, (Object) null))
      {
        ImGui.Text("No anim controller.");
      }
      else
      {
        KBatchGroupData batchGroupData = KAnimBatchManager.Instance().GetBatchGroupData(component1.batchGroupID);
        SymbolOverrideController component2 = this.Selection.GetComponent<SymbolOverrideController>();
        ImGui.Text("Group: " + component1.GetBatch().group.batchID.ToString() + ", Build: " + component1.curBuild.name);
        if (!ImGui.BeginTabBar("##tabs", (ImGuiTabBarFlags) 0))
          return;
        if (ImGui.BeginTabItem("BatchGroup"))
        {
          KAnimBatchGroup group = component1.GetBatch().group;
          ImGui.BeginChild("ScrollRegion", new Vector2(0.0f, 0.0f), true, (ImGuiWindowFlags) 0);
          ImGui.Text(string.Format("Group mesh.vertices.Count: {0}", (object) ((IEnumerable<Vector3>) group.mesh.vertices).Count<Vector3>()));
          ImGui.Text(string.Format("Group data.maxVisibleSymbols: {0}", (object) group.data.maxVisibleSymbols));
          ImGui.Text(string.Format("Group maxGroupSize: {0}", (object) group.maxGroupSize));
          ImGui.EndChild();
          ImGui.EndTabItem();
        }
        if (Object.op_Inequality((Object) component2, (Object) null) && ImGui.BeginTabItem("SymbolOverrides"))
        {
          ImGui.InputText("Symbol Filter", ref this.Filter, 128U);
          int num = Hash.SDBMLower(this.Filter);
          ImGui.LabelText("Filter Hash", "0x" + num.ToString("X"));
          SymbolOverrideController.SymbolEntry[] getSymbolOverrides = component2.GetSymbolOverrides;
          ImGui.BeginChild("ScrollRegion", new Vector2(0.0f, 0.0f), true, (ImGuiWindowFlags) 0);
          for (int index = 0; index < getSymbolOverrides.Length; ++index)
          {
            SymbolOverrideController.SymbolEntry symbolEntry = getSymbolOverrides[index];
            KAnim.Build.Symbol symbol = batchGroupData.GetSymbol(KAnimHashedString.op_Implicit(symbolEntry.targetSymbol));
            if (((HashedString) ref symbolEntry.targetSymbol).HashValue == num || ((KAnimHashedString) ref symbolEntry.sourceSymbol.hash).HashValue == num || this.StringContains(symbolEntry.sourceSymbol.hash.ToString(), this.Filter) || this.StringContains(symbol.hash.ToString(), this.Filter))
            {
              ImGui.Text(string.Format("[{0}] source: {1}, {2}, ({3}), priority: {4}", (object) index, (object) symbolEntry.sourceSymbol.hash, (object) symbolEntry.sourceSymbol.build.name, (object) ((Object) symbolEntry.sourceSymbol.build.GetTexture(0)).name, (object) symbolEntry.priority));
              ImGui.Text(string.Format("       firstFrameIdx = {0}, numFrames = {1}", (object) symbolEntry.sourceSymbol.firstFrameIdx, (object) symbolEntry.sourceSymbol.numFrames));
              ImGui.Text(string.Format("   target: {0}", (object) symbol.hash));
              ImGui.Text(string.Format("       firstFrameIdx = {0}, numFrames = {1}", (object) symbol.firstFrameIdx, (object) symbol.numFrames));
            }
          }
          ImGui.EndChild();
          ImGui.EndTabItem();
        }
        if (ImGui.BeginTabItem("Build Symbols"))
        {
          ImGui.InputText("Symbol Filter", ref this.Filter, 128U);
          int num = Hash.SDBMLower(this.Filter);
          ImGui.LabelText("Filter Hash", "0x" + num.ToString("X"));
          ImGui.BeginChild("ScrollRegion", new Vector2(0.0f, 0.0f), true, (ImGuiWindowFlags) 0);
          KBatchGroupData data = component1.GetBatch().group.data;
          for (int index = 0; index < data.GetSymbolCount(); ++index)
          {
            KAnim.Build.Symbol symbol = data.GetSymbol(index);
            if (((KAnimHashedString) ref symbol.hash).HashValue == num || this.StringContains(symbol.hash.ToString(), this.Filter))
              ImGui.Text(string.Format("[{0}]: {1}", (object) symbol.symbolIndexInSourceBuild, (object) symbol.hash));
          }
          ImGui.EndChild();
          ImGui.EndTabItem();
        }
        if (ImGui.BeginTabItem("Anim Frame Data"))
        {
          ImGui.Text("Current frame: " + component1.GetCurrentFrameIndex().ToString());
          ImGuiEx.InputIntRange("Frame Index", ref this.FrameIndex, 0, batchGroupData.GetAnimFrames().Count);
          KAnim.Anim.Frame frame = batchGroupData.GetFrame(this.FrameIndex);
          ImGui.Text(string.Format("Frame [{0}]: firstElementIdx= {1} numElements= {2}", (object) this.FrameIndex, (object) frame.firstElementIdx, (object) frame.numElements));
          ImGuiEx.InputIntRange("Frame Element Index", ref this.FrameElementIndex, 0, frame.numElements);
          KAnim.Anim.FrameElement frameElement = batchGroupData.GetFrameElement(frame.firstElementIdx + this.FrameElementIndex);
          ImGui.Text(string.Format("FrameElement [{0}]: symbolIdx= {1} symbol= {2}", (object) (frame.firstElementIdx + this.FrameElementIndex), (object) frameElement.symbolIdx, (object) frameElement.symbol));
          ImGui.EndTabItem();
        }
        if (ImGui.BeginTabItem("Texture atlases"))
        {
          ImGui.BeginChild("ScrollRegion", new Vector2(0.0f, 0.0f), true, (ImGuiWindowFlags) 0);
          List<Texture2D> source = new List<Texture2D>((IEnumerable<Texture2D>) component1.GetBatch().atlases.GetTextures());
          int num = ((IEnumerable<Texture2D>) source).Count<Texture2D>();
          if (Object.op_Inequality((Object) component2, (Object) null))
            source.AddRange((IEnumerable<Texture2D>) component2.GetAtlasList().GetTextures());
          for (int index = 0; index < source.Count; ++index)
          {
            Texture2D texture2D = source[index];
            string str = index >= num ? "symbol override" : "base";
            ImGui.Text(string.Format("[{0}]: {1}, [{2},{3}] ({4})", (object) index, (object) ((Object) texture2D).name, (object) ((Texture) texture2D).width, (object) ((Texture) texture2D).height, (object) str));
            if (ImGui.IsItemHovered())
            {
              ImGui.BeginTooltip();
              ImGuiEx.Image(texture2D, new Vector2((float) ((Texture) texture2D).width, (float) ((Texture) texture2D).height));
              ImGui.EndTooltip();
            }
          }
          ImGui.EndChild();
          ImGui.EndTabItem();
        }
        ImGui.EndTabBar();
      }
    }
  }

  private bool StringContains(string target, string query) => this.Filter == "" || target.IndexOf(query, 0, StringComparison.CurrentCultureIgnoreCase) != -1;
}
