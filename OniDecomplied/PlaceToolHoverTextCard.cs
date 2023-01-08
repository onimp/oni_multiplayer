// Decompiled with JetBrains decompiler
// Type: PlaceToolHoverTextCard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class PlaceToolHoverTextCard : HoverTextConfiguration
{
  public Placeable currentPlaceable;

  public override void UpdateHoverElements(List<KSelectable> hoverObjects)
  {
    HoverTextScreen instance = HoverTextScreen.Instance;
    HoverTextDrawer drawer = instance.BeginDrawing();
    int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
    if (!Grid.IsValidCell(cell) || (int) Grid.WorldIdx[cell] != ClusterManager.Instance.activeWorldId)
    {
      drawer.EndDrawing();
    }
    else
    {
      drawer.BeginShadowBar();
      this.ActionName = (string) UI.TOOLS.PLACE.TOOLACTION;
      if (Object.op_Inequality((Object) this.currentPlaceable, (Object) null) && ((Component) this.currentPlaceable).GetProperName() != null)
        this.ToolName = string.Format((string) UI.TOOLS.PLACE.NAME, (object) ((Component) this.currentPlaceable).GetProperName());
      this.DrawTitle(instance, drawer);
      this.DrawInstructions(instance, drawer);
      int min_height = 26;
      int width = 8;
      string reason;
      if (Object.op_Inequality((Object) this.currentPlaceable, (Object) null) && !this.currentPlaceable.IsValidPlaceLocation(cell, out reason))
      {
        drawer.NewLine(min_height);
        drawer.AddIndent(width);
        drawer.DrawText(reason, this.HoverTextStyleSettings[1]);
      }
      drawer.EndShadowBar();
      drawer.EndDrawing();
    }
  }
}
