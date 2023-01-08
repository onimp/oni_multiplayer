// Decompiled with JetBrains decompiler
// Type: NToggleSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NToggleSideScreen : SideScreenContent
{
  [SerializeField]
  private KToggle buttonPrefab;
  [SerializeField]
  private LocText description;
  private INToggleSideScreenControl target;
  private List<KToggle> buttonList = new List<KToggle>();

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<INToggleSideScreenControl>() != null;

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.target = target.GetComponent<INToggleSideScreenControl>();
    if (this.target == null)
      return;
    this.titleKey = this.target.SidescreenTitleKey;
    ((Component) this).gameObject.SetActive(true);
    this.Refresh();
  }

  private void Refresh()
  {
    for (int index = 0; index < Mathf.Max(this.target.Options.Count, this.buttonList.Count); ++index)
    {
      if (index >= this.target.Options.Count)
      {
        ((Component) this.buttonList[index]).gameObject.SetActive(false);
      }
      else
      {
        if (index >= this.buttonList.Count)
        {
          KToggle ktoggle = Util.KInstantiateUI<KToggle>(((Component) this.buttonPrefab).gameObject, this.ContentContainer, false);
          int idx = index;
          ktoggle.onClick += (System.Action) (() =>
          {
            this.target.QueueSelectedOption(idx);
            this.Refresh();
          });
          this.buttonList.Add(ktoggle);
        }
        ((TMP_Text) ((Component) this.buttonList[index]).GetComponentInChildren<LocText>()).text = (string) this.target.Options[index];
        ((Component) this.buttonList[index]).GetComponentInChildren<ToolTip>().toolTip = (string) this.target.Tooltips[index];
        if (this.target.SelectedOption == index && this.target.QueuedOption == index)
        {
          this.buttonList[index].isOn = true;
          foreach (ImageToggleState componentsInChild in ((Component) this.buttonList[index]).GetComponentsInChildren<ImageToggleState>())
            componentsInChild.SetActive();
          ((Behaviour) ((Component) this.buttonList[index]).GetComponent<ImageToggleStateThrobber>()).enabled = false;
        }
        else if (this.target.QueuedOption == index)
        {
          this.buttonList[index].isOn = true;
          foreach (ImageToggleState componentsInChild in ((Component) this.buttonList[index]).GetComponentsInChildren<ImageToggleState>())
            componentsInChild.SetActive();
          ((Behaviour) ((Component) this.buttonList[index]).GetComponent<ImageToggleStateThrobber>()).enabled = true;
        }
        else
        {
          this.buttonList[index].isOn = false;
          foreach (ImageToggleState componentsInChild in ((Component) this.buttonList[index]).GetComponentsInChildren<ImageToggleState>())
          {
            componentsInChild.SetInactive();
            componentsInChild.SetInactive();
          }
          ((Behaviour) ((Component) this.buttonList[index]).GetComponent<ImageToggleStateThrobber>()).enabled = false;
        }
        ((Component) this.buttonList[index]).gameObject.SetActive(true);
      }
    }
    ((TMP_Text) this.description).text = this.target.Description;
    ((Component) this.description).gameObject.SetActive(!string.IsNullOrEmpty(this.target.Description));
  }
}
