// Decompiled with JetBrains decompiler
// Type: ClusterMapSelectTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClusterMapSelectTool : InterfaceTool
{
  private List<KSelectable> m_hoveredSelectables = new List<KSelectable>();
  private KSelectable m_selected;
  public static ClusterMapSelectTool Instance;
  private KSelectable delayedNextSelection;
  private bool delayedSkipSound;

  public static void DestroyInstance() => ClusterMapSelectTool.Instance = (ClusterMapSelectTool) null;

  protected virtual void OnPrefabInit() => ClusterMapSelectTool.Instance = this;

  public void Activate()
  {
    PlayerController.Instance.ActivateTool((InterfaceTool) this);
    ToolMenu.Instance.PriorityScreen.ResetPriority();
    this.Select((KSelectable) null);
  }

  public KSelectable GetSelected() => this.m_selected;

  public override bool ShowHoverUI() => ClusterMapScreen.Instance.HasCurrentHover();

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    this.ClearHover();
    this.Select((KSelectable) null);
  }

  private void UpdateHoveredSelectables()
  {
    this.m_hoveredSelectables.Clear();
    if (!ClusterMapScreen.Instance.HasCurrentHover())
      return;
    AxialI currentHoverLocation = ClusterMapScreen.Instance.GetCurrentHoverLocation();
    this.m_hoveredSelectables.AddRange((IEnumerable<KSelectable>) ClusterGrid.Instance.GetVisibleEntitiesAtCell(currentHoverLocation).Select<ClusterGridEntity, KSelectable>((Func<ClusterGridEntity, KSelectable>) (entity => ((Component) entity).GetComponent<KSelectable>())).Where<KSelectable>((Func<KSelectable, bool>) (selectable => Object.op_Inequality((Object) selectable, (Object) null) && selectable.IsSelectable)).ToList<KSelectable>());
  }

  public override void LateUpdate()
  {
    this.UpdateHoveredSelectables();
    KSelectable kselectable = this.m_hoveredSelectables.Count > 0 ? this.m_hoveredSelectables[0] : (KSelectable) null;
    this.UpdateHoverElements(this.m_hoveredSelectables);
    if (!this.hasFocus)
      this.ClearHover();
    else if (Object.op_Inequality((Object) kselectable, (Object) this.hover))
    {
      this.ClearHover();
      this.hover = kselectable;
      if (Object.op_Inequality((Object) kselectable, (Object) null))
      {
        Game.Instance.Trigger(2095258329, (object) ((Component) kselectable).gameObject);
        kselectable.Hover(!this.playedSoundThisFrame);
        this.playedSoundThisFrame = true;
      }
    }
    this.playedSoundThisFrame = false;
  }

  public void SelectNextFrame(KSelectable new_selected, bool skipSound = false)
  {
    this.delayedNextSelection = new_selected;
    this.delayedSkipSound = skipSound;
    UIScheduler.Instance.ScheduleNextFrame("DelayedSelect", new Action<object>(this.DoSelectNextFrame));
  }

  private void DoSelectNextFrame(object data)
  {
    this.Select(this.delayedNextSelection, this.delayedSkipSound);
    this.delayedNextSelection = (KSelectable) null;
  }

  public void Select(KSelectable new_selected, bool skipSound = false)
  {
    if (Object.op_Equality((Object) new_selected, (Object) this.m_selected))
      return;
    if (Object.op_Inequality((Object) this.m_selected, (Object) null))
      this.m_selected.Unselect();
    GameObject gameObject = (GameObject) null;
    if (Object.op_Inequality((Object) new_selected, (Object) null) && new_selected.GetMyWorldId() == -1)
    {
      if (Object.op_Equality((Object) new_selected, (Object) this.hover))
        this.ClearHover();
      new_selected.Select();
      gameObject = ((Component) new_selected).gameObject;
    }
    this.m_selected = Object.op_Equality((Object) gameObject, (Object) null) ? (KSelectable) null : new_selected;
    Game.Instance.Trigger(-1503271301, (object) gameObject);
  }
}
