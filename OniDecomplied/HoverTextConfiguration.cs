// Decompiled with JetBrains decompiler
// Type: HoverTextConfiguration
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HoverTextConfiguration")]
public class HoverTextConfiguration : KMonoBehaviour
{
  public TextStyleSetting[] HoverTextStyleSettings;
  public string ToolNameStringKey = "";
  public string ActionStringKey = "";
  [HideInInspector]
  public string ActionName = "";
  [HideInInspector]
  public string ToolName;
  protected string backStr;
  public TextStyleSetting ToolTitleTextStyle;
  public HoverTextConfiguration.TextStylePair Styles_Title;
  public HoverTextConfiguration.TextStylePair Styles_BodyText;
  public HoverTextConfiguration.TextStylePair Styles_Instruction;
  public HoverTextConfiguration.TextStylePair Styles_Warning;
  public HoverTextConfiguration.ValuePropertyTextStyles Styles_Values;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.ConfigureHoverScreen();
  }

  protected virtual void ConfigureTitle(HoverTextScreen screen)
  {
    if (!string.IsNullOrEmpty(this.ToolName))
      return;
    this.ToolName = Strings.Get(this.ToolNameStringKey).String.ToUpper();
  }

  protected void DrawTitle(HoverTextScreen screen, HoverTextDrawer drawer) => drawer.DrawText(this.ToolName, this.ToolTitleTextStyle);

  protected void DrawInstructions(HoverTextScreen screen, HoverTextDrawer drawer)
  {
    TextStyleSetting standard = this.Styles_Instruction.Standard;
    drawer.NewLine();
    if (KInputManager.currentControllerIsGamepad)
      drawer.DrawIcon(KInputManager.steamInputInterpreter.GetActionSprite((Action) 3, false), 20);
    else
      drawer.DrawIcon(screen.GetSprite("icon_mouse_left"), 20);
    drawer.DrawText(this.ActionName, standard);
    drawer.AddIndent(8);
    if (KInputManager.currentControllerIsGamepad)
      drawer.DrawIcon(KInputManager.steamInputInterpreter.GetActionSprite((Action) 5, false), 20);
    else
      drawer.DrawIcon(screen.GetSprite("icon_mouse_right"), 20);
    drawer.DrawText(this.backStr, standard);
  }

  public virtual void ConfigureHoverScreen()
  {
    if (!string.IsNullOrEmpty(this.ActionStringKey))
      this.ActionName = StringEntry.op_Implicit(Strings.Get(this.ActionStringKey));
    this.ConfigureTitle(HoverTextScreen.Instance);
    this.backStr = UI.TOOLS.GENERIC.BACK.ToString().ToUpper();
  }

  public virtual void UpdateHoverElements(List<KSelectable> hover_objects)
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
      drawer.EndDrawing();
    }
  }

  [Serializable]
  public struct TextStylePair
  {
    public TextStyleSetting Standard;
    public TextStyleSetting Selected;
  }

  [Serializable]
  public struct ValuePropertyTextStyles
  {
    public HoverTextConfiguration.TextStylePair Property;
    public HoverTextConfiguration.TextStylePair Property_Decimal;
    public HoverTextConfiguration.TextStylePair Property_Unit;
  }
}
