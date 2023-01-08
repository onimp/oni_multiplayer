// Decompiled with JetBrains decompiler
// Type: OptionSelector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionSelector : MonoBehaviour
{
  private object id;
  public Action<object, int> OnChangePriority;
  [SerializeField]
  private KImage selectedItem;
  [SerializeField]
  private KImage itemTemplate;

  private void Start() => ((Component) this.selectedItem).GetComponent<KButton>().onBtnClick += new Action<KKeyCode>(this.OnClick);

  public void Initialize(object id) => this.id = id;

  private void OnClick(KKeyCode button)
  {
    if (button != 323)
    {
      if (button != 324)
        return;
      this.OnChangePriority(this.id, -1);
    }
    else
      this.OnChangePriority(this.id, 1);
  }

  public void ConfigureItem(bool disabled, OptionSelector.DisplayOptionInfo display_info)
  {
    HierarchyReferences component = ((Component) this.selectedItem).GetComponent<HierarchyReferences>();
    KImage reference1 = component.GetReference("BG") as KImage;
    if (display_info.bgOptions == null)
      ((Component) reference1).gameObject.SetActive(false);
    else
      ((Image) reference1).sprite = display_info.bgOptions[display_info.bgIndex];
    KImage reference2 = component.GetReference("FG") as KImage;
    if (display_info.fgOptions == null)
      ((Component) reference2).gameObject.SetActive(false);
    else
      ((Image) reference2).sprite = display_info.fgOptions[display_info.fgIndex];
    KImage reference3 = component.GetReference("Fill") as KImage;
    if (Object.op_Inequality((Object) reference3, (Object) null))
    {
      ((Behaviour) reference3).enabled = !disabled;
      ((Graphic) reference3).color = Color32.op_Implicit(display_info.fillColour);
    }
    KImage reference4 = component.GetReference("Outline") as KImage;
    if (!Object.op_Inequality((Object) reference4, (Object) null))
      return;
    ((Behaviour) reference4).enabled = !disabled;
  }

  public class DisplayOptionInfo
  {
    public IList<Sprite> bgOptions;
    public IList<Sprite> fgOptions;
    public int bgIndex;
    public int fgIndex;
    public Color32 fillColour;
  }
}
