// Decompiled with JetBrains decompiler
// Type: ClusterMapSelectToolHoverTextCard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class ClusterMapSelectToolHoverTextCard : HoverTextConfiguration
{
  private Sprite m_iconWarning;
  private Sprite m_iconDash;
  private Sprite m_iconHighlighted;

  public override void ConfigureHoverScreen()
  {
    base.ConfigureHoverScreen();
    HoverTextScreen instance = HoverTextScreen.Instance;
    this.m_iconWarning = instance.GetSprite("iconWarning");
    this.m_iconDash = instance.GetSprite("dash");
    this.m_iconHighlighted = instance.GetSprite("dash_arrow");
  }

  public override void UpdateHoverElements(List<KSelectable> hoverObjects)
  {
    if (Object.op_Equality((Object) this.m_iconWarning, (Object) null))
      this.ConfigureHoverScreen();
    HoverTextDrawer hoverTextDrawer = HoverTextScreen.Instance.BeginDrawing();
    foreach (KSelectable hoverObject in hoverObjects)
    {
      hoverTextDrawer.BeginShadowBar(Object.op_Equality((Object) ClusterMapSelectTool.Instance.GetSelected(), (Object) hoverObject));
      string unitFormattedName = GameUtil.GetUnitFormattedName(((Component) hoverObject).gameObject, true);
      hoverTextDrawer.DrawText(unitFormattedName, this.Styles_Title.Standard);
      foreach (StatusItemGroup.Entry entry in hoverObject.GetStatusItemGroup())
      {
        if (entry.category != null && entry.category.Id == "Main")
        {
          TextStyleSetting style = this.IsStatusItemWarning(entry) ? this.Styles_Warning.Standard : this.Styles_BodyText.Standard;
          Sprite icon = entry.item.sprite != null ? entry.item.sprite.sprite : this.m_iconWarning;
          Color color = this.IsStatusItemWarning(entry) ? this.Styles_Warning.Standard.textColor : this.Styles_BodyText.Standard.textColor;
          hoverTextDrawer.NewLine();
          hoverTextDrawer.DrawIcon(icon, color);
          hoverTextDrawer.DrawText(entry.GetName(), style);
        }
      }
      foreach (StatusItemGroup.Entry entry in hoverObject.GetStatusItemGroup())
      {
        if (entry.category == null || entry.category.Id != "Main")
        {
          TextStyleSetting style = this.IsStatusItemWarning(entry) ? this.Styles_Warning.Standard : this.Styles_BodyText.Standard;
          Sprite icon = entry.item.sprite != null ? entry.item.sprite.sprite : this.m_iconWarning;
          Color color = this.IsStatusItemWarning(entry) ? this.Styles_Warning.Standard.textColor : this.Styles_BodyText.Standard.textColor;
          hoverTextDrawer.NewLine();
          hoverTextDrawer.DrawIcon(icon, color);
          hoverTextDrawer.DrawText(entry.GetName(), style);
        }
      }
      hoverTextDrawer.EndShadowBar();
    }
    hoverTextDrawer.EndDrawing();
  }

  private bool IsStatusItemWarning(StatusItemGroup.Entry item) => item.item.notificationType == NotificationType.Bad || item.item.notificationType == NotificationType.BadMinor || item.item.notificationType == NotificationType.DuplicantThreatening;
}
