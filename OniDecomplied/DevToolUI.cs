// Decompiled with JetBrains decompiler
// Type: DevToolUI
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ImGuiNET;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DevToolUI : DevTool
{
  private List<RaycastResult> m_raycast_hits = new List<RaycastResult>();
  private RaycastResult? m_last_pinged_hit;

  protected override void RenderTo(DevPanel panel)
  {
    this.RepopulateRaycastHits();
    this.DrawPingObject();
    this.DrawRaycastHits();
  }

  private void DrawPingObject()
  {
    if (this.m_last_pinged_hit.HasValue)
    {
      RaycastResult raycastResult = this.m_last_pinged_hit.Value;
      GameObject gameObject = ((RaycastResult) ref raycastResult).gameObject;
      if ((!Object.op_Inequality((Object) gameObject, (Object) null) ? 0 : (Object.op_Implicit((Object) gameObject) ? 1 : 0)) != 0)
      {
        ImGui.Text("Last Pinged: \"" + DevToolUI.GetQualifiedName(gameObject) + "\"");
        ImGui.SameLine();
        if (ImGui.Button("Inspect"))
          DevToolSceneInspector.Inspect((object) gameObject);
        ImGui.Spacing();
        ImGui.Spacing();
      }
      else
        this.m_last_pinged_hit = new RaycastResult?();
    }
    ImGui.Text("Press \",\" to ping the top hovered ui object");
    ImGui.Spacing();
    ImGui.Spacing();
  }

  private void Internal_Ping(RaycastResult raycastResult)
  {
    GameObject gameObject = ((RaycastResult) ref raycastResult).gameObject;
    this.m_last_pinged_hit = new RaycastResult?(raycastResult);
  }

  public static void PingHoveredObject()
  {
    using (ListPool<RaycastResult, DevToolUI>.PooledList pooledList1 = PoolsFor<DevToolUI>.AllocateList<RaycastResult>())
    {
      EventSystem current = EventSystem.current;
      if (Object.op_Equality((Object) current, (Object) null) || !Object.op_Implicit((Object) current))
        return;
      EventSystem eventSystem = current;
      PointerEventData pointerEventData = new PointerEventData(current);
      pointerEventData.position = Vector2.op_Implicit(Input.mousePosition);
      ListPool<RaycastResult, DevToolUI>.PooledList pooledList2 = pooledList1;
      eventSystem.RaycastAll(pointerEventData, (List<RaycastResult>) pooledList2);
      DevToolManager.Instance.panels.AddOrGetDevTool<DevToolUI>().Internal_Ping(((List<RaycastResult>) pooledList1)[0]);
    }
  }

  private void DrawRaycastHits()
  {
    if (this.m_raycast_hits.Count <= 0)
    {
      ImGui.Text("Didn't hit any ui");
    }
    else
    {
      ImGui.Text("Raycast Hits:");
      ImGui.Indent();
      for (int index = 0; index < this.m_raycast_hits.Count; ++index)
      {
        RaycastResult raycastHit = this.m_raycast_hits[index];
        ImGui.BulletText(string.Format("[{0}] {1}", (object) index, (object) DevToolUI.GetQualifiedName(((RaycastResult) ref raycastHit).gameObject)));
      }
      ImGui.Unindent();
    }
  }

  private void RepopulateRaycastHits()
  {
    this.m_raycast_hits.Clear();
    EventSystem current = EventSystem.current;
    if (Object.op_Equality((Object) current, (Object) null) || !Object.op_Implicit((Object) current))
      return;
    EventSystem eventSystem = current;
    PointerEventData pointerEventData = new PointerEventData(current);
    pointerEventData.position = Vector2.op_Implicit(Input.mousePosition);
    List<RaycastResult> raycastHits = this.m_raycast_hits;
    eventSystem.RaycastAll(pointerEventData, raycastHits);
  }

  private static string GetQualifiedName(GameObject game_object)
  {
    KScreen componentInParent = game_object.GetComponentInParent<KScreen>();
    return Object.op_Inequality((Object) componentInParent, (Object) null) ? ((Object) ((Component) componentInParent).gameObject).name + " :: " + ((Object) game_object).name : ((Object) game_object).name ?? "";
  }
}
