// Decompiled with JetBrains decompiler
// Type: AttackToolHoverTextCard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class AttackToolHoverTextCard : HoverTextConfiguration
{
  public override void UpdateHoverElements(List<KSelectable> hover_objects)
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
      this.DrawTitle(instance, drawer);
      this.DrawInstructions(HoverTextScreen.Instance, drawer);
      drawer.EndShadowBar();
      if (hover_objects != null)
      {
        foreach (KSelectable hoverObject in hover_objects)
        {
          if (Object.op_Inequality((Object) ((Component) hoverObject).GetComponent<AttackableBase>(), (Object) null))
          {
            drawer.BeginShadowBar();
            drawer.DrawText(hoverObject.GetProperName().ToUpper(), this.Styles_Title.Standard);
            drawer.EndShadowBar();
            break;
          }
        }
      }
      drawer.EndDrawing();
    }
  }
}
