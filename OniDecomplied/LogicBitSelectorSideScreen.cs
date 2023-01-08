// Decompiled with JetBrains decompiler
// Type: LogicBitSelectorSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogicBitSelectorSideScreen : SideScreenContent, IRenderEveryTick
{
  private ILogicRibbonBitSelector target;
  public GameObject rowPrefab;
  public KImage inputDisplayIcon;
  public KImage outputDisplayIcon;
  public GameObject readerDescriptionContainer;
  public GameObject writerDescriptionContainer;
  [NonSerialized]
  public Dictionary<int, MultiToggle> toggles_by_int = new Dictionary<int, MultiToggle>();
  private Color activeColor;
  private Color inactiveColor;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.activeColor = Color32.op_Implicit(GlobalAssets.Instance.colorSet.logicOnText);
    this.inactiveColor = Color32.op_Implicit(GlobalAssets.Instance.colorSet.logicOffText);
  }

  public void SelectToggle(int bit)
  {
    this.target.SetBitSelection(bit);
    this.target.UpdateVisuals();
    this.RefreshToggles();
  }

  private void RefreshToggles()
  {
    for (int index = 0; index < this.target.GetBitDepth(); ++index)
    {
      int n = index;
      if (!this.toggles_by_int.ContainsKey(index))
      {
        GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, ((Component) this.rowPrefab.transform.parent).gameObject, true);
        ((TMP_Text) gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("bitName")).SetText(string.Format((string) STRINGS.UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.BIT, (object) (index + 1)));
        ((Graphic) gameObject.GetComponent<HierarchyReferences>().GetReference<KImage>("stateIcon")).color = this.target.IsBitActive(index) ? this.activeColor : this.inactiveColor;
        ((TMP_Text) gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("stateText")).SetText((string) (this.target.IsBitActive(index) ? STRINGS.UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_ACTIVE : STRINGS.UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_INACTIVE));
        MultiToggle component = gameObject.GetComponent<MultiToggle>();
        this.toggles_by_int.Add(index, component);
      }
      this.toggles_by_int[index].onClick = (System.Action) (() => this.SelectToggle(n));
    }
    foreach (KeyValuePair<int, MultiToggle> keyValuePair in this.toggles_by_int)
    {
      if (this.target.GetBitSelection() == keyValuePair.Key)
        keyValuePair.Value.ChangeState(0);
      else
        keyValuePair.Value.ChangeState(1);
    }
  }

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<ILogicRibbonBitSelector>() != null;

  public override void SetTarget(GameObject new_target)
  {
    if (Object.op_Equality((Object) new_target, (Object) null))
    {
      Debug.LogError((object) "Invalid gameObject received");
    }
    else
    {
      this.target = new_target.GetComponent<ILogicRibbonBitSelector>();
      if (this.target == null)
      {
        Debug.LogError((object) "The gameObject received is not an ILogicRibbonBitSelector");
      }
      else
      {
        this.titleKey = this.target.SideScreenTitle;
        this.readerDescriptionContainer.SetActive(this.target.SideScreenDisplayReaderDescription());
        this.writerDescriptionContainer.SetActive(this.target.SideScreenDisplayWriterDescription());
        this.RefreshToggles();
        this.UpdateInputOutputDisplay();
        foreach (KeyValuePair<int, MultiToggle> keyValuePair in this.toggles_by_int)
          this.UpdateStateVisuals(keyValuePair.Key);
      }
    }
  }

  public void RenderEveryTick(float dt)
  {
    if (this.target.Equals((object) null))
      return;
    foreach (KeyValuePair<int, MultiToggle> keyValuePair in this.toggles_by_int)
      this.UpdateStateVisuals(keyValuePair.Key);
    this.UpdateInputOutputDisplay();
  }

  private void UpdateInputOutputDisplay()
  {
    if (this.target.SideScreenDisplayReaderDescription())
      ((Graphic) this.outputDisplayIcon).color = this.target.GetOutputValue() > 0 ? this.activeColor : this.inactiveColor;
    if (!this.target.SideScreenDisplayWriterDescription())
      return;
    ((Graphic) this.inputDisplayIcon).color = this.target.GetInputValue() > 0 ? this.activeColor : this.inactiveColor;
  }

  private void UpdateStateVisuals(int bit)
  {
    MultiToggle multiToggle = this.toggles_by_int[bit];
    ((Graphic) ((Component) multiToggle).gameObject.GetComponent<HierarchyReferences>().GetReference<KImage>("stateIcon")).color = this.target.IsBitActive(bit) ? this.activeColor : this.inactiveColor;
    ((TMP_Text) ((Component) multiToggle).gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("stateText")).SetText((string) (this.target.IsBitActive(bit) ? STRINGS.UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_ACTIVE : STRINGS.UI.UISIDESCREENS.LOGICBITSELECTORSIDESCREEN.STATE_INACTIVE));
  }
}
