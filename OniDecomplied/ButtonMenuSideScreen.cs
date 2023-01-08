// Decompiled with JetBrains decompiler
// Type: ButtonMenuSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ButtonMenuSideScreen : SideScreenContent
{
  public const int DefaultButtonMenuSideScreenSortOrder = 20;
  public GameObject buttonPrefab;
  public RectTransform buttonContainer;
  private List<GameObject> liveButtons = new List<GameObject>();
  private List<ISidescreenButtonControl> targets;

  public override bool IsValidForTarget(GameObject target)
  {
    ISidescreenButtonControl sidescreenButtonControl = target.GetComponent<ISidescreenButtonControl>() ?? target.GetSMI<ISidescreenButtonControl>();
    return sidescreenButtonControl != null && sidescreenButtonControl.SidescreenEnabled();
  }

  public override int GetSideScreenSortOrder() => this.targets == null ? 20 : this.targets[0].ButtonSideScreenSortOrder();

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.targets = new_target.GetAllSMI<ISidescreenButtonControl>();
      this.targets.AddRange((IEnumerable<ISidescreenButtonControl>) new_target.GetComponents<ISidescreenButtonControl>());
      this.Refresh();
    }
  }

  private void Refresh()
  {
    while (this.liveButtons.Count < this.targets.Count)
      this.liveButtons.Add(Util.KInstantiateUI(this.buttonPrefab, ((Component) this.buttonContainer).gameObject, true));
    for (int index = 0; index < this.liveButtons.Count; ++index)
    {
      if (index >= this.targets.Count)
      {
        this.liveButtons[index].SetActive(false);
      }
      else
      {
        if (!this.liveButtons[index].activeSelf)
          this.liveButtons[index].SetActive(true);
        KButton componentInChildren1 = this.liveButtons[index].GetComponentInChildren<KButton>();
        ToolTip componentInChildren2 = this.liveButtons[index].GetComponentInChildren<ToolTip>();
        LocText componentInChildren3 = this.liveButtons[index].GetComponentInChildren<LocText>();
        componentInChildren1.isInteractable = this.targets[index].SidescreenButtonInteractable();
        componentInChildren1.ClearOnClick();
        componentInChildren1.onClick += new System.Action(this.targets[index].OnSidescreenButtonPressed);
        componentInChildren1.onClick += new System.Action(this.Refresh);
        ((TMP_Text) componentInChildren3).SetText(this.targets[index].SidescreenButtonText);
        componentInChildren2.SetSimpleTooltip(this.targets[index].SidescreenButtonTooltip);
      }
    }
  }
}
