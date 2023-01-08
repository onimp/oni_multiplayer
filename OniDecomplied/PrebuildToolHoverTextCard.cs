// Decompiled with JetBrains decompiler
// Type: PrebuildToolHoverTextCard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PrebuildToolHoverTextCard : HoverTextConfiguration
{
  public string errorMessage;
  public BuildingDef currentDef;

  public override void UpdateHoverElements(List<KSelectable> selected)
  {
    HoverTextScreen instance = HoverTextScreen.Instance;
    HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
    int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
    if (!Grid.IsValidCell(cell) || (int) Grid.WorldIdx[cell] != ClusterManager.Instance.activeWorldId)
    {
      hoverTextDrawer.EndDrawing();
    }
    else
    {
      hoverTextDrawer.BeginShadowBar();
      if (!Util.IsNullOrWhiteSpace(this.errorMessage))
      {
        bool flag = true;
        string errorMessage = this.errorMessage;
        char[] chArray = new char[1]{ '\n' };
        foreach (string str in errorMessage.Split(chArray))
        {
          if (!flag)
            hoverTextDrawer.NewLine();
          hoverTextDrawer.DrawText(str.ToUpper(), this.HoverTextStyleSettings[flag ? 0 : 1]);
          flag = false;
        }
      }
      hoverTextDrawer.NewLine();
      if (KInputManager.currentControllerIsGamepad)
        hoverTextDrawer.DrawIcon(KInputManager.steamInputInterpreter.GetActionSprite((Action) 5, false), 20);
      else
        hoverTextDrawer.DrawIcon(instance.GetSprite("icon_mouse_right"), 20);
      hoverTextDrawer.DrawText(this.backStr, this.Styles_Instruction.Standard);
      hoverTextDrawer.EndShadowBar();
      hoverTextDrawer.EndDrawing();
    }
  }
}
