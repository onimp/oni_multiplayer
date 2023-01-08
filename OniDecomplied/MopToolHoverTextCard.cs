// Decompiled with JetBrains decompiler
// Type: MopToolHoverTextCard
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MopToolHoverTextCard : HoverTextConfiguration
{
  private MopToolHoverTextCard.HoverScreenFields hoverScreenElements;

  public override void UpdateHoverElements(List<KSelectable> selected)
  {
    int cell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
    HoverTextScreen instance = HoverTextScreen.Instance;
    HoverTextDrawer drawer = instance.BeginDrawing();
    if (!Grid.IsValidCell(cell) || (int) Grid.WorldIdx[cell] != ClusterManager.Instance.activeWorldId)
    {
      drawer.EndDrawing();
    }
    else
    {
      drawer.BeginShadowBar();
      if (Grid.IsVisible(cell))
      {
        this.DrawTitle(instance, drawer);
        this.DrawInstructions(HoverTextScreen.Instance, drawer);
        Element element = Grid.Element[cell];
        if (element.IsLiquid)
        {
          drawer.NewLine();
          drawer.DrawText(element.nameUpperCase, this.Styles_Title.Standard);
          drawer.NewLine();
          drawer.DrawIcon(instance.GetSprite("dash"));
          drawer.DrawText(element.GetMaterialCategoryTag().ProperName(), this.Styles_BodyText.Standard);
          drawer.NewLine();
          drawer.DrawIcon(instance.GetSprite("dash"));
          string[] strArray = HoverTextHelper.MassStringsReadOnly(cell);
          drawer.DrawText(strArray[0], this.Styles_Values.Property.Standard);
          drawer.DrawText(strArray[1], this.Styles_Values.Property_Decimal.Standard);
          drawer.DrawText(strArray[2], this.Styles_Values.Property.Standard);
          drawer.DrawText(strArray[3], this.Styles_Values.Property.Standard);
        }
      }
      else
      {
        drawer.DrawIcon(instance.GetSprite("iconWarning"));
        drawer.DrawText(STRINGS.UI.TOOLS.GENERIC.UNKNOWN.ToString().ToUpper(), this.Styles_BodyText.Standard);
      }
      drawer.EndShadowBar();
      drawer.EndDrawing();
    }
  }

  private struct HoverScreenFields
  {
    public GameObject UnknownAreaLine;
    public Image ElementStateIcon;
    public LocText ElementCategory;
    public LocText ElementName;
    public LocText[] ElementMass;
    public LocText ElementHardness;
    public LocText ElementHardnessDescription;
  }
}
