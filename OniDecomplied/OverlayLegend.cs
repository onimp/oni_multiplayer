// Decompiled with JetBrains decompiler
// Type: OverlayLegend
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverlayLegend : KScreen
{
  public static OverlayLegend Instance;
  [SerializeField]
  private LocText title;
  [SerializeField]
  private Sprite emptySprite;
  [SerializeField]
  private List<OverlayLegend.OverlayInfo> overlayInfoList;
  [SerializeField]
  private GameObject unitPrefab;
  [SerializeField]
  private GameObject activeUnitsParent;
  [SerializeField]
  private GameObject diagramsParent;
  [SerializeField]
  private GameObject inactiveUnitsParent;
  [SerializeField]
  private GameObject toolParameterMenuPrefab;
  private ToolParameterMenu filterMenu;
  private OverlayModes.Mode currentMode;
  private List<GameObject> inactiveUnitObjs;
  private List<GameObject> activeUnitObjs;
  private List<GameObject> activeDiagrams = new List<GameObject>();

  [ContextMenu("Set all fonts color")]
  public void SetAllFontsColor()
  {
    foreach (OverlayLegend.OverlayInfo overlayInfo in this.overlayInfoList)
    {
      for (int index = 0; index < overlayInfo.infoUnits.Count; ++index)
      {
        if (Color.op_Equality(overlayInfo.infoUnits[index].fontColor, Color.clear))
          overlayInfo.infoUnits[index].fontColor = Color.white;
      }
    }
  }

  [ContextMenu("Set all tooltips")]
  public void SetAllTooltips()
  {
    foreach (OverlayLegend.OverlayInfo overlayInfo in this.overlayInfoList)
    {
      string oldValue = overlayInfo.name.Replace("NAME", "");
      for (int index = 0; index < overlayInfo.infoUnits.Count; ++index)
      {
        string str1 = overlayInfo.infoUnits[index].description.Replace(oldValue, "");
        string str2 = oldValue + "TOOLTIPS." + str1;
        overlayInfo.infoUnits[index].tooltip = str2;
      }
    }
  }

  [ContextMenu("Set Sliced for empty icons")]
  public void SetSlicedForEmptyIcons()
  {
    foreach (OverlayLegend.OverlayInfo overlayInfo in this.overlayInfoList)
    {
      for (int index = 0; index < overlayInfo.infoUnits.Count; ++index)
      {
        if (Object.op_Equality((Object) overlayInfo.infoUnits[index].icon, (Object) this.emptySprite))
          overlayInfo.infoUnits[index].sliceIcon = true;
      }
    }
  }

  protected virtual void OnSpawn()
  {
    this.ConsumeMouseScroll = true;
    base.OnSpawn();
    if (Object.op_Equality((Object) OverlayLegend.Instance, (Object) null))
    {
      OverlayLegend.Instance = this;
      this.activeUnitObjs = new List<GameObject>();
      this.inactiveUnitObjs = new List<GameObject>();
      foreach (OverlayLegend.OverlayInfo overlayInfo in this.overlayInfoList)
      {
        overlayInfo.name = StringEntry.op_Implicit(Strings.Get(overlayInfo.name));
        for (int index = 0; index < overlayInfo.infoUnits.Count; ++index)
        {
          overlayInfo.infoUnits[index].description = StringEntry.op_Implicit(Strings.Get(overlayInfo.infoUnits[index].description));
          if (!string.IsNullOrEmpty(overlayInfo.infoUnits[index].tooltip))
            overlayInfo.infoUnits[index].tooltip = StringEntry.op_Implicit(Strings.Get(overlayInfo.infoUnits[index].tooltip));
        }
      }
      ((Component) this).GetComponent<LayoutElement>().minWidth = DlcManager.FeatureClusterSpaceEnabled() ? 322f : 288f;
      this.ClearLegend();
    }
    else
      Object.Destroy((Object) ((Component) this).gameObject);
  }

  protected virtual void OnLoadLevel()
  {
    OverlayLegend.Instance = (OverlayLegend) null;
    this.activeDiagrams.Clear();
    Object.Destroy((Object) ((Component) this).gameObject);
    ((KMonoBehaviour) this).OnLoadLevel();
  }

  private void SetLegend(OverlayLegend.OverlayInfo overlayInfo)
  {
    if (overlayInfo == null)
      this.ClearLegend();
    else if (!overlayInfo.isProgrammaticallyPopulated && (overlayInfo.infoUnits == null || overlayInfo.infoUnits.Count == 0))
    {
      this.ClearLegend();
    }
    else
    {
      this.Show(true);
      ((TMP_Text) this.title).text = overlayInfo.name;
      if (overlayInfo.isProgrammaticallyPopulated)
        this.PopulateGeneratedLegend(overlayInfo);
      else
        this.PopulateOverlayInfoUnits(overlayInfo);
    }
  }

  public void SetLegend(OverlayModes.Mode mode, bool refreshing = false)
  {
    if (this.currentMode != null && HashedString.op_Equality(this.currentMode.ViewMode(), mode.ViewMode()) && !refreshing)
      return;
    this.ClearLegend();
    OverlayLegend.OverlayInfo overlayInfo = this.overlayInfoList.Find((Predicate<OverlayLegend.OverlayInfo>) (ol => HashedString.op_Equality(ol.mode, mode.ViewMode())));
    this.currentMode = mode;
    this.SetLegend(overlayInfo);
  }

  public GameObject GetFreeUnitObject()
  {
    if (this.inactiveUnitObjs.Count == 0)
      this.inactiveUnitObjs.Add(Util.KInstantiateUI(this.unitPrefab, this.inactiveUnitsParent, false));
    GameObject inactiveUnitObj = this.inactiveUnitObjs[0];
    this.inactiveUnitObjs.RemoveAt(0);
    this.activeUnitObjs.Add(inactiveUnitObj);
    return inactiveUnitObj;
  }

  private void RemoveActiveObjects()
  {
    while (this.activeUnitObjs.Count > 0)
    {
      ((Behaviour) ((Component) this.activeUnitObjs[0].transform.Find("Icon")).GetComponent<Image>()).enabled = false;
      ((Behaviour) this.activeUnitObjs[0].GetComponentInChildren<LocText>()).enabled = false;
      this.activeUnitObjs[0].transform.SetParent(this.inactiveUnitsParent.transform);
      this.activeUnitObjs[0].SetActive(false);
      this.inactiveUnitObjs.Add(this.activeUnitObjs[0]);
      this.activeUnitObjs.RemoveAt(0);
    }
  }

  public void ClearLegend()
  {
    this.RemoveActiveObjects();
    for (int index = 0; index < this.activeDiagrams.Count; ++index)
    {
      if (Object.op_Inequality((Object) this.activeDiagrams[index], (Object) null))
        Object.Destroy((Object) this.activeDiagrams[index]);
    }
    this.activeDiagrams.Clear();
    Vector2 sizeDelta = this.diagramsParent.GetComponent<RectTransform>().sizeDelta;
    sizeDelta.y = 0.0f;
    this.diagramsParent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
    this.Show(false);
  }

  public OverlayLegend.OverlayInfo GetOverlayInfo(OverlayModes.Mode mode)
  {
    for (int index = 0; index < this.overlayInfoList.Count; ++index)
    {
      if (HashedString.op_Equality(this.overlayInfoList[index].mode, mode.ViewMode()))
        return this.overlayInfoList[index];
    }
    return (OverlayLegend.OverlayInfo) null;
  }

  private void PopulateOverlayInfoUnits(OverlayLegend.OverlayInfo overlayInfo, bool isRefresh = false)
  {
    if (overlayInfo.infoUnits != null && overlayInfo.infoUnits.Count > 0)
    {
      this.activeUnitsParent.SetActive(true);
      foreach (OverlayLegend.OverlayInfoUnit infoUnit in overlayInfo.infoUnits)
      {
        GameObject freeUnitObject = this.GetFreeUnitObject();
        if (Object.op_Inequality((Object) infoUnit.icon, (Object) null))
        {
          Image component = ((Component) freeUnitObject.transform.Find("Icon")).GetComponent<Image>();
          ((Component) component).gameObject.SetActive(true);
          component.sprite = infoUnit.icon;
          ((Graphic) component).color = infoUnit.color;
          ((Behaviour) component).enabled = true;
          component.type = infoUnit.sliceIcon ? (Image.Type) 1 : (Image.Type) 0;
        }
        else
          ((Component) freeUnitObject.transform.Find("Icon")).gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(infoUnit.description))
        {
          LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
          ((TMP_Text) componentInChildren).text = string.Format(infoUnit.description, infoUnit.formatData);
          ((Graphic) componentInChildren).color = infoUnit.fontColor;
          ((Behaviour) componentInChildren).enabled = true;
        }
        ToolTip component1 = freeUnitObject.GetComponent<ToolTip>();
        if (!string.IsNullOrEmpty(infoUnit.tooltip))
        {
          component1.toolTip = string.Format(infoUnit.tooltip, infoUnit.tooltipFormatData);
          ((Behaviour) component1).enabled = true;
        }
        else
          ((Behaviour) component1).enabled = false;
        freeUnitObject.SetActive(true);
        freeUnitObject.transform.SetParent(this.activeUnitsParent.transform);
      }
    }
    else
      this.activeUnitsParent.SetActive(false);
    if (isRefresh)
      return;
    if (overlayInfo.diagrams != null && overlayInfo.diagrams.Count > 0)
    {
      this.diagramsParent.SetActive(true);
      foreach (GameObject diagram in overlayInfo.diagrams)
        this.activeDiagrams.Add(Util.KInstantiateUI(diagram, this.diagramsParent, false));
    }
    else
      this.diagramsParent.SetActive(false);
  }

  private void PopulateGeneratedLegend(OverlayLegend.OverlayInfo info, bool isRefresh = false)
  {
    if (isRefresh)
      this.RemoveActiveObjects();
    if (info.infoUnits != null && info.infoUnits.Count > 0)
      this.PopulateOverlayInfoUnits(info, isRefresh);
    List<LegendEntry> customLegendData = this.currentMode.GetCustomLegendData();
    if (customLegendData != null)
    {
      this.activeUnitsParent.SetActive(true);
      foreach (LegendEntry legendEntry in customLegendData)
      {
        GameObject freeUnitObject = this.GetFreeUnitObject();
        Image component1 = ((Component) freeUnitObject.transform.Find("Icon")).GetComponent<Image>();
        ((Component) component1).gameObject.SetActive(legendEntry.displaySprite);
        component1.sprite = legendEntry.sprite;
        ((Graphic) component1).color = legendEntry.colour;
        ((Behaviour) component1).enabled = true;
        component1.type = (Image.Type) 0;
        LocText componentInChildren = freeUnitObject.GetComponentInChildren<LocText>();
        ((TMP_Text) componentInChildren).text = legendEntry.name;
        ((Graphic) componentInChildren).color = Color.white;
        ((Behaviour) componentInChildren).enabled = true;
        ToolTip component2 = freeUnitObject.GetComponent<ToolTip>();
        ((Behaviour) component2).enabled = legendEntry.desc != null || legendEntry.desc_arg != null;
        component2.toolTip = legendEntry.desc_arg == null ? legendEntry.desc : string.Format(legendEntry.desc, (object) legendEntry.desc_arg);
        freeUnitObject.SetActive(true);
        freeUnitObject.transform.SetParent(this.activeUnitsParent.transform);
      }
    }
    else
      this.activeUnitsParent.SetActive(false);
    if (isRefresh || this.currentMode.legendFilters == null)
      return;
    GameObject gameObject = Util.KInstantiateUI(this.toolParameterMenuPrefab, this.diagramsParent, false);
    this.activeDiagrams.Add(gameObject);
    this.diagramsParent.SetActive(true);
    this.filterMenu = gameObject.GetComponent<ToolParameterMenu>();
    this.filterMenu.PopulateMenu(this.currentMode.legendFilters);
    this.filterMenu.onParametersChanged += new System.Action(this.OnFiltersChanged);
    this.OnFiltersChanged();
  }

  private void OnFiltersChanged()
  {
    this.currentMode.OnFiltersChanged();
    this.PopulateGeneratedLegend(this.GetOverlayInfo(this.currentMode), true);
    Game.Instance.ForceOverlayUpdate();
  }

  private void DisableOverlay()
  {
    this.filterMenu.onParametersChanged -= new System.Action(this.OnFiltersChanged);
    this.filterMenu.ClearMenu();
    ((Component) this.filterMenu).gameObject.SetActive(false);
    this.filterMenu = (ToolParameterMenu) null;
  }

  [Serializable]
  public class OverlayInfoUnit
  {
    public Sprite icon;
    public string description;
    public string tooltip;
    public Color color;
    public Color fontColor;
    public object formatData;
    public object tooltipFormatData;
    public bool sliceIcon;

    public OverlayInfoUnit(
      Sprite icon,
      string description,
      Color color,
      Color fontColor,
      object formatData = null,
      bool sliceIcon = false)
    {
      this.icon = icon;
      this.description = description;
      this.color = color;
      this.fontColor = fontColor;
      this.formatData = formatData;
      this.sliceIcon = sliceIcon;
    }
  }

  [Serializable]
  public class OverlayInfo
  {
    public string name;
    public HashedString mode;
    public List<OverlayLegend.OverlayInfoUnit> infoUnits;
    public List<GameObject> diagrams;
    public bool isProgrammaticallyPopulated;
  }
}
