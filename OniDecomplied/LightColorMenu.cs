// Decompiled with JetBrains decompiler
// Type: LightColorMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LightColorMenu")]
public class LightColorMenu : KMonoBehaviour
{
  public LightColorMenu.LightColor[] lightColors;
  private int currentColor;
  private static readonly EventSystem.IntraObjectHandler<LightColorMenu> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<LightColorMenu>((Action<LightColorMenu, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  protected virtual void OnPrefabInit()
  {
    this.Subscribe<LightColorMenu>(493375141, LightColorMenu.OnRefreshUserMenuDelegate);
    this.SetColor(0);
  }

  private void OnRefreshUserMenu(object data)
  {
    if (this.lightColors.Length == 0)
      return;
    int length = this.lightColors.Length;
    for (int index = 0; index < length; ++index)
    {
      if (index != this.currentColor)
      {
        int new_color = index;
        Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo(this.lightColors[index].name, this.lightColors[index].name, (System.Action) (() => this.SetColor(new_color))));
      }
    }
  }

  private void SetColor(int color_index)
  {
    if (this.lightColors.Length != 0 && color_index < this.lightColors.Length)
    {
      foreach (Light2D componentsInChild in ((Component) this).GetComponentsInChildren<Light2D>(true))
        componentsInChild.Color = this.lightColors[color_index].color;
      foreach (Renderer componentsInChild in ((Component) this).GetComponentsInChildren<MeshRenderer>(true))
      {
        foreach (Material material in componentsInChild.materials)
        {
          if (((Object) material).name.StartsWith("matScriptedGlow01"))
            material.color = this.lightColors[color_index].color;
        }
      }
    }
    this.currentColor = color_index;
  }

  [Serializable]
  public struct LightColor
  {
    public string name;
    public Color color;

    public LightColor(string name, Color color)
    {
      this.name = name;
      this.color = color;
    }
  }
}
