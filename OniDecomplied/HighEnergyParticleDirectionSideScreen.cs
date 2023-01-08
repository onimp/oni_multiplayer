// Decompiled with JetBrains decompiler
// Type: HighEnergyParticleDirectionSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighEnergyParticleDirectionSideScreen : SideScreenContent
{
  private IHighEnergyParticleDirection target;
  public List<KButton> Buttons;
  private KButton activeButton;
  public LocText directionLabel;
  private string[] directionStrings = new string[8]
  {
    (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_N,
    (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_NW,
    (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_W,
    (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_SW,
    (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_S,
    (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_SE,
    (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_E,
    (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.DIRECTION_NE
  };

  public override string GetTitle() => (string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.TITLE;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    for (int index = 0; index < this.Buttons.Count; ++index)
    {
      KButton button = this.Buttons[index];
      button.onClick += (System.Action) (() =>
      {
        int num = this.Buttons.IndexOf(button);
        if (Object.op_Inequality((Object) this.activeButton, (Object) null))
          this.activeButton.isInteractable = true;
        button.isInteractable = false;
        this.activeButton = button;
        if (this.target == null)
          return;
        this.target.Direction = EightDirectionUtil.AngleToDirection(num * 45);
        Game.Instance.ForceOverlayUpdate(true);
        this.Refresh();
      });
    }
  }

  public override int GetSideScreenSortOrder() => 10;

  public override bool IsValidForTarget(GameObject target)
  {
    HighEnergyParticleRedirector component = target.GetComponent<HighEnergyParticleRedirector>();
    bool flag1 = Object.op_Inequality((Object) component, (Object) null);
    if (flag1)
      flag1 = flag1 && component.directionControllable;
    bool flag2 = Object.op_Inequality((Object) target.GetComponent<HighEnergyParticleSpawner>(), (Object) null) || Object.op_Inequality((Object) target.GetComponent<ManualHighEnergyParticleSpawner>(), (Object) null);
    return flag1 | flag2 && target.GetComponent<IHighEnergyParticleDirection>() != null;
  }

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.target = new_target.GetComponent<IHighEnergyParticleDirection>();
      if (this.target == null)
        Debug.LogError((object) "The gameObject received does not contain IHighEnergyParticleDirection component");
      else
        this.Refresh();
    }
  }

  private void Refresh()
  {
    int directionIndex = EightDirectionUtil.GetDirectionIndex(this.target.Direction);
    if (directionIndex >= 0 && directionIndex < this.Buttons.Count)
    {
      this.Buttons[directionIndex].SignalClick((KKeyCode) 323);
    }
    else
    {
      if (Object.op_Implicit((Object) this.activeButton))
        this.activeButton.isInteractable = true;
      this.activeButton = (KButton) null;
    }
    ((TMP_Text) this.directionLabel).SetText(string.Format((string) UI.UISIDESCREENS.HIGHENERGYPARTICLEDIRECTIONSIDESCREEN.SELECTED_DIRECTION, (object) this.directionStrings[directionIndex]));
  }
}
