// Decompiled with JetBrains decompiler
// Type: ClusterMapHex
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ClusterMapHex : MultiToggle, ICanvasRaycastFilter
{
  private RectTransform rectTransform;
  public Color hoverColorValid;
  public Color hoverColorInvalid;
  public Image fogOfWar;
  public Image peekedTile;
  public TextStyleSetting invalidDestinationTooltipStyle;
  public TextStyleSetting informationTooltipStyle;
  [MyCmpGet]
  private ToolTip m_tooltip;
  private ClusterRevealLevel _revealLevel;

  public AxialI location { get; private set; }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.rectTransform = ((Component) this).GetComponent<RectTransform>();
    this.onClick = new System.Action(this.TrySelect);
    this.onDoubleClick = new Func<bool>(this.TryGoTo);
    this.onEnter = new System.Action(this.OnHover);
    this.onExit = new System.Action(this.OnUnhover);
  }

  public void SetLocation(AxialI location) => this.location = location;

  public void SetRevealed(ClusterRevealLevel level)
  {
    this._revealLevel = level;
    switch (level)
    {
      case ClusterRevealLevel.Hidden:
        ((Component) this.fogOfWar).gameObject.SetActive(true);
        ((Component) this.peekedTile).gameObject.SetActive(false);
        break;
      case ClusterRevealLevel.Peeked:
        ((Component) this.fogOfWar).gameObject.SetActive(false);
        ((Component) this.peekedTile).gameObject.SetActive(true);
        break;
      case ClusterRevealLevel.Visible:
        ((Component) this.fogOfWar).gameObject.SetActive(false);
        ((Component) this.peekedTile).gameObject.SetActive(false);
        break;
    }
  }

  public void SetDestinationStatus(string fail_reason)
  {
    this.m_tooltip.ClearMultiStringTooltip();
    this.UpdateHoverColors(string.IsNullOrEmpty(fail_reason));
    if (string.IsNullOrEmpty(fail_reason))
      return;
    this.m_tooltip.AddMultiStringTooltip(fail_reason, this.invalidDestinationTooltipStyle);
  }

  public void SetDestinationStatus(
    string fail_reason,
    int pathLength,
    int rocketRange,
    bool repeat)
  {
    this.m_tooltip.ClearMultiStringTooltip();
    if (pathLength > 0)
    {
      string format = (string) (repeat ? STRINGS.UI.CLUSTERMAP.TOOLTIP_PATH_LENGTH_RETURN : STRINGS.UI.CLUSTERMAP.TOOLTIP_PATH_LENGTH);
      if (repeat)
        pathLength *= 2;
      this.m_tooltip.AddMultiStringTooltip(string.Format(format, (object) pathLength, (object) GameUtil.GetFormattedRocketRange((float) rocketRange, GameUtil.TimeSlice.None)), this.informationTooltipStyle);
    }
    this.UpdateHoverColors(string.IsNullOrEmpty(fail_reason));
    if (string.IsNullOrEmpty(fail_reason))
      return;
    this.m_tooltip.AddMultiStringTooltip(fail_reason, this.invalidDestinationTooltipStyle);
  }

  public void UpdateToggleState(ClusterMapHex.ToggleState state)
  {
    int new_state_index = -1;
    switch (state)
    {
      case ClusterMapHex.ToggleState.Unselected:
        new_state_index = 0;
        break;
      case ClusterMapHex.ToggleState.Selected:
        new_state_index = 1;
        break;
      case ClusterMapHex.ToggleState.OrbitHighlight:
        new_state_index = 2;
        break;
    }
    this.ChangeState(new_state_index);
  }

  private void TrySelect()
  {
    if (DebugHandler.InstantBuildMode)
      ((Component) SaveGame.Instance).GetSMI<ClusterFogOfWarManager.Instance>().RevealLocation(this.location);
    ClusterMapScreen.Instance.SelectHex(this);
  }

  private bool TryGoTo()
  {
    List<WorldContainer> list = ClusterGrid.Instance.GetVisibleEntitiesAtCell(this.location).Select<ClusterGridEntity, WorldContainer>((Func<ClusterGridEntity, WorldContainer>) (entity => ((Component) entity).GetComponent<WorldContainer>())).Where<WorldContainer>((Func<WorldContainer, bool>) (x => Object.op_Inequality((Object) x, (Object) null))).ToList<WorldContainer>();
    if (list.Count != 1)
      return false;
    CameraController.Instance.ActiveWorldStarWipe(list[0].id);
    return true;
  }

  private void OnHover()
  {
    this.m_tooltip.ClearMultiStringTooltip();
    string str = "";
    switch (this._revealLevel)
    {
      case ClusterRevealLevel.Hidden:
        str = (string) STRINGS.UI.CLUSTERMAP.TOOLTIP_HIDDEN_HEX;
        break;
      case ClusterRevealLevel.Peeked:
        List<ClusterGridEntity> entitiesOfLayerAtCell1 = ClusterGrid.Instance.GetHiddenEntitiesOfLayerAtCell(this.location, EntityLayer.Asteroid);
        List<ClusterGridEntity> entitiesOfLayerAtCell2 = ClusterGrid.Instance.GetHiddenEntitiesOfLayerAtCell(this.location, EntityLayer.POI);
        str = (string) (entitiesOfLayerAtCell1.Count > 0 || entitiesOfLayerAtCell2.Count > 0 ? STRINGS.UI.CLUSTERMAP.TOOLTIP_PEEKED_HEX_WITH_OBJECT : STRINGS.UI.CLUSTERMAP.TOOLTIP_HIDDEN_HEX);
        break;
      case ClusterRevealLevel.Visible:
        if (ClusterGrid.Instance.GetEntitiesOnCell(this.location).Count == 0)
        {
          str = (string) STRINGS.UI.CLUSTERMAP.TOOLTIP_EMPTY_HEX;
          break;
        }
        break;
    }
    if (!Util.IsNullOrWhiteSpace(str))
      this.m_tooltip.AddMultiStringTooltip(str, this.informationTooltipStyle);
    this.UpdateHoverColors(true);
    ClusterMapScreen.Instance.OnHoverHex(this);
  }

  private void OnUnhover() => ClusterMapScreen.Instance.OnUnhoverHex(this);

  private void UpdateHoverColors(bool validDestination)
  {
    Color color = validDestination ? this.hoverColorValid : this.hoverColorInvalid;
    for (int index1 = 0; index1 < this.states.Length; ++index1)
    {
      this.states[index1].color_on_hover = color;
      for (int index2 = 0; index2 < this.states[index1].additional_display_settings.Length; ++index2)
        this.states[index1].additional_display_settings[index2].color_on_hover = color;
    }
    this.RefreshHoverColor();
  }

  public bool IsRaycastLocationValid(Vector2 inputPoint, Camera eventCamera)
  {
    Vector2 vector2_1 = Vector2.op_Implicit(((Transform) this.rectTransform).position);
    float num1 = Mathf.Abs(inputPoint.x - vector2_1.x);
    float num2 = Mathf.Abs(inputPoint.y - vector2_1.y);
    Vector2 vector2_2 = Vector2.op_Implicit(((Transform) this.rectTransform).lossyScale);
    return (double) num1 <= (double) vector2_2.x && (double) num2 <= (double) vector2_2.y && (double) vector2_2.y * (double) vector2_2.x - (double) vector2_2.y / 2.0 * (double) num1 - (double) vector2_2.x * (double) num2 >= 0.0;
  }

  public enum ToggleState
  {
    Unselected,
    Selected,
    OrbitHighlight,
  }
}
