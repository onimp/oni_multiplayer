// Decompiled with JetBrains decompiler
// Type: CollapsibleDetailContentPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CollapsibleDetailContentPanel")]
public class CollapsibleDetailContentPanel : KMonoBehaviour
{
  public ImageToggleState ArrowIcon;
  public LocText HeaderLabel;
  public MultiToggle collapseButton;
  public Transform Content;
  public ScalerMask scalerMask;
  [Space(10f)]
  public DetailLabel labelTemplate;
  public DetailLabelWithButton labelWithActionButtonTemplate;
  private Dictionary<string, CollapsibleDetailContentPanel.Label<DetailLabel>> labels;
  private Dictionary<string, CollapsibleDetailContentPanel.Label<DetailLabelWithButton>> buttonLabels;
  private LoggerFSS log;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.collapseButton.onClick += new System.Action(this.ToggleOpen);
    this.ArrowIcon.SetActive();
    this.log = new LoggerFSS("detailpanel", 35);
    this.labels = new Dictionary<string, CollapsibleDetailContentPanel.Label<DetailLabel>>();
    this.buttonLabels = new Dictionary<string, CollapsibleDetailContentPanel.Label<DetailLabelWithButton>>();
    this.Commit();
  }

  public void SetTitle(string title) => ((TMP_Text) this.HeaderLabel).text = title;

  public void Commit()
  {
    int num = 0;
    foreach (CollapsibleDetailContentPanel.Label<DetailLabel> label in this.labels.Values)
    {
      if (label.used)
      {
        ++num;
        if (!((Component) label.obj).gameObject.activeSelf)
          ((Component) label.obj).gameObject.SetActive(true);
      }
      else if (!label.used && ((Component) label.obj).gameObject.activeSelf)
        ((Component) label.obj).gameObject.SetActive(false);
      label.used = false;
    }
    foreach (CollapsibleDetailContentPanel.Label<DetailLabelWithButton> label in this.buttonLabels.Values)
    {
      if (label.used)
      {
        ++num;
        if (!((Component) label.obj).gameObject.activeSelf)
          ((Component) label.obj).gameObject.SetActive(true);
      }
      else if (!label.used && ((Component) label.obj).gameObject.activeSelf)
        ((Component) label.obj).gameObject.SetActive(false);
      label.used = false;
    }
    if (((Component) this).gameObject.activeSelf && num == 0)
    {
      ((Component) this).gameObject.SetActive(false);
    }
    else
    {
      if (((Component) this).gameObject.activeSelf || num <= 0)
        return;
      ((Component) this).gameObject.SetActive(true);
    }
  }

  public void SetLabel(string id, string text, string tooltip)
  {
    CollapsibleDetailContentPanel.Label<DetailLabel> label;
    if (!this.labels.TryGetValue(id, out label))
    {
      label = new CollapsibleDetailContentPanel.Label<DetailLabel>()
      {
        used = true,
        obj = Util.KInstantiateUI(((Component) this.labelTemplate).gameObject, ((Component) this.Content).gameObject, false).GetComponent<DetailLabel>()
      };
      ((Object) ((Component) label.obj).gameObject).name = id;
      this.labels[id] = label;
    }
    label.obj.label.AllowLinks = true;
    ((TMP_Text) label.obj.label).text = text;
    label.obj.toolTip.toolTip = tooltip;
    label.used = true;
  }

  public void SetLabelWithButton(
    string id,
    string text,
    string tooltip,
    string buttonText,
    string buttonTooltip,
    System.Action buttonCb)
  {
    CollapsibleDetailContentPanel.Label<DetailLabelWithButton> label;
    if (!this.buttonLabels.TryGetValue(id, out label))
    {
      label = new CollapsibleDetailContentPanel.Label<DetailLabelWithButton>()
      {
        used = true,
        obj = Util.KInstantiateUI(((Component) this.labelWithActionButtonTemplate).gameObject, ((Component) this.Content).gameObject, false).GetComponent<DetailLabelWithButton>()
      };
      ((Object) ((Component) label.obj).gameObject).name = id;
      this.buttonLabels[id] = label;
    }
    label.obj.label.AllowLinks = true;
    ((TMP_Text) label.obj.label).text = text;
    label.obj.toolTip.toolTip = tooltip;
    ((TMP_Text) label.obj.buttonLabel).text = buttonText;
    label.obj.buttonToolTip.toolTip = buttonTooltip;
    label.obj.button.ClearOnClick();
    label.obj.button.onClick += buttonCb;
    label.used = true;
  }

  private void ToggleOpen()
  {
    bool flag = !((Component) this.scalerMask).gameObject.activeSelf;
    ((Component) this.scalerMask).gameObject.SetActive(flag);
    if (flag)
    {
      this.ArrowIcon.SetActive();
      this.ForceLocTextsMeshRebuild();
    }
    else
      this.ArrowIcon.SetInactive();
  }

  public void ForceLocTextsMeshRebuild()
  {
    foreach (TMP_Text componentsInChild in ((Component) this).GetComponentsInChildren<LocText>())
      componentsInChild.ForceMeshUpdate();
  }

  private class Label<T>
  {
    public T obj;
    public bool used;
  }
}
