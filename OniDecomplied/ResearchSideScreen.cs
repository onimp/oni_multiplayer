// Decompiled with JetBrains decompiler
// Type: ResearchSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchSideScreen : SideScreenContent
{
  public KButton selectResearchButton;
  public Image researchButtonIcon;
  public GameObject content;
  private GameObject target;
  private Action<object> refreshDisplayStateDelegate;
  public LocText DescriptionText;

  public ResearchSideScreen() => this.refreshDisplayStateDelegate = new Action<object>(this.RefreshDisplayState);

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.selectResearchButton.onClick += (System.Action) (() => ManagementMenu.Instance.ToggleResearch());
    Research.Instance.Subscribe(-1914338957, this.refreshDisplayStateDelegate);
    Research.Instance.Subscribe(-125623018, this.refreshDisplayStateDelegate);
    this.RefreshDisplayState();
  }

  protected virtual void OnCmpEnable()
  {
    ((KMonoBehaviour) this).OnCmpEnable();
    this.RefreshDisplayState();
    this.target = ((Component) ((Component) SelectTool.Instance.selected).GetComponent<KMonoBehaviour>()).gameObject;
    KMonoBehaviourExtensions.Subscribe(this.target.gameObject, -1852328367, this.refreshDisplayStateDelegate);
    KMonoBehaviourExtensions.Subscribe(this.target.gameObject, -592767678, this.refreshDisplayStateDelegate);
  }

  protected virtual void OnCmpDisable()
  {
    ((KMonoBehaviour) this).OnCmpDisable();
    if (!Object.op_Implicit((Object) this.target))
      return;
    KMonoBehaviourExtensions.Unsubscribe(this.target.gameObject, -1852328367, this.refreshDisplayStateDelegate);
    KMonoBehaviourExtensions.Unsubscribe(this.target.gameObject, 187661686, this.refreshDisplayStateDelegate);
    this.target = (GameObject) null;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Research.Instance.Unsubscribe(-1914338957, this.refreshDisplayStateDelegate);
    Research.Instance.Unsubscribe(-125623018, this.refreshDisplayStateDelegate);
    if (!Object.op_Implicit((Object) this.target))
      return;
    KMonoBehaviourExtensions.Unsubscribe(this.target.gameObject, -1852328367, this.refreshDisplayStateDelegate);
    KMonoBehaviourExtensions.Unsubscribe(this.target.gameObject, 187661686, this.refreshDisplayStateDelegate);
    this.target = (GameObject) null;
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<ResearchCenter>(), (Object) null) || Object.op_Inequality((Object) target.GetComponent<NuclearResearchCenter>(), (Object) null);

  private void RefreshDisplayState(object data = null)
  {
    if (Object.op_Equality((Object) SelectTool.Instance.selected, (Object) null))
      return;
    string str1 = "";
    ResearchCenter component1 = ((Component) SelectTool.Instance.selected).GetComponent<ResearchCenter>();
    NuclearResearchCenter component2 = ((Component) SelectTool.Instance.selected).GetComponent<NuclearResearchCenter>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      str1 = component1.research_point_type_id;
    if (Object.op_Inequality((Object) component2, (Object) null))
      str1 = component2.researchTypeID;
    if (Object.op_Equality((Object) component1, (Object) null) && Object.op_Equality((Object) component2, (Object) null))
      return;
    this.researchButtonIcon.sprite = Research.Instance.researchTypes.GetResearchType(str1).sprite;
    TechInstance activeResearch = Research.Instance.GetActiveResearch();
    if (activeResearch == null)
    {
      ((TMP_Text) this.DescriptionText).text = "<b>" + (string) STRINGS.UI.UISIDESCREENS.RESEARCHSIDESCREEN.NOSELECTEDRESEARCH + "</b>";
    }
    else
    {
      string str2 = "";
      if (!activeResearch.tech.costsByResearchTypeID.ContainsKey(str1) || (double) activeResearch.tech.costsByResearchTypeID[str1] <= 0.0)
        str2 += "<color=#7f7f7f>";
      string str3 = str2 + "<b>" + activeResearch.tech.Name + "</b>";
      if (!activeResearch.tech.costsByResearchTypeID.ContainsKey(str1) || (double) activeResearch.tech.costsByResearchTypeID[str1] <= 0.0)
        str3 += "</color>";
      foreach (KeyValuePair<string, float> keyValuePair in activeResearch.tech.costsByResearchTypeID)
      {
        if ((double) keyValuePair.Value != 0.0)
        {
          bool flag = keyValuePair.Key == str1;
          str3 += "\n   ";
          str3 += "<b>";
          if (!flag)
            str3 += "<color=#7f7f7f>";
          str3 = str3 + "- " + Research.Instance.researchTypes.GetResearchType(keyValuePair.Key).name + ": " + activeResearch.progressInventory.PointsByTypeID[keyValuePair.Key].ToString() + "/" + activeResearch.tech.costsByResearchTypeID[keyValuePair.Key].ToString();
          if (!flag)
            str3 += "</color>";
          str3 += "</b>";
        }
      }
      ((TMP_Text) this.DescriptionText).text = str3;
    }
  }
}
