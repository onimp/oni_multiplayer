// Decompiled with JetBrains decompiler
// Type: ResearchEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

[AddComponentMenu("KMonoBehaviour/scripts/ResearchEntry")]
public class ResearchEntry : KMonoBehaviour
{
  [Header("Labels")]
  [SerializeField]
  private LocText researchName;
  [Header("Transforms")]
  [SerializeField]
  private Transform progressBarContainer;
  [SerializeField]
  private Transform lineContainer;
  [Header("Prefabs")]
  [SerializeField]
  private GameObject iconPanel;
  [SerializeField]
  private GameObject iconPrefab;
  [SerializeField]
  private GameObject linePrefab;
  [SerializeField]
  private GameObject progressBarPrefab;
  [Header("Graphics")]
  [SerializeField]
  private Image BG;
  [SerializeField]
  private Image titleBG;
  [SerializeField]
  private Image borderHighlight;
  [SerializeField]
  private Image filterHighlight;
  [SerializeField]
  private Image filterLowlight;
  [SerializeField]
  private Sprite hoverBG;
  [SerializeField]
  private Sprite completedBG;
  [Header("Colors")]
  [SerializeField]
  private Color defaultColor = Color.blue;
  [SerializeField]
  private Color completedColor = Color.yellow;
  [SerializeField]
  private Color pendingColor = Color.magenta;
  [SerializeField]
  private Color completedHeaderColor = Color.grey;
  [SerializeField]
  private Color incompleteHeaderColor = Color.grey;
  [SerializeField]
  private Color pendingHeaderColor = Color.grey;
  private Sprite defaultBG;
  [MyCmpGet]
  private KToggle toggle;
  private ResearchScreen researchScreen;
  private Dictionary<Tech, UILineRenderer> techLineMap;
  private Tech targetTech;
  private bool isOn = true;
  private Coroutine fadeRoutine;
  public Color activeLineColor;
  public Color inactiveLineColor;
  public int lineThickness_active = 6;
  public int lineThickness_inactive = 2;
  public Material StandardUIMaterial;
  private Dictionary<string, GameObject> progressBarsByResearchTypeID = new Dictionary<string, GameObject>();
  public static readonly string UnlockedTechKey = "UnlockedTech";
  private Dictionary<string, object> unlockedTechMetric = new Dictionary<string, object>()
  {
    {
      ResearchEntry.UnlockedTechKey,
      (object) null
    }
  };

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.techLineMap = new Dictionary<Tech, UILineRenderer>();
    ((Graphic) this.BG).color = this.defaultColor;
    foreach (Tech key in this.targetTech.requiredTech)
    {
      float num1 = (float) ((double) this.targetTech.width / 2.0 + 18.0);
      Vector2 zero1 = Vector2.zero;
      Vector2 zero2 = Vector2.zero;
      if ((double) key.center.y > (double) this.targetTech.center.y + 2.0)
      {
        // ISSUE: explicit constructor call
        ((Vector2) ref zero1).\u002Ector(0.0f, 20f);
        // ISSUE: explicit constructor call
        ((Vector2) ref zero2).\u002Ector(0.0f, -20f);
      }
      else if ((double) key.center.y < (double) this.targetTech.center.y - 2.0)
      {
        // ISSUE: explicit constructor call
        ((Vector2) ref zero1).\u002Ector(0.0f, -20f);
        // ISSUE: explicit constructor call
        ((Vector2) ref zero2).\u002Ector(0.0f, 20f);
      }
      UILineRenderer component = Util.KInstantiateUI(this.linePrefab, ((Component) this.lineContainer).gameObject, true).GetComponent<UILineRenderer>();
      float num2 = 32f;
      component.Points = new Vector2[4]
      {
        Vector2.op_Addition(new Vector2(0.0f, 0.0f), zero1),
        Vector2.op_Addition(new Vector2(-num2, 0.0f), zero1),
        Vector2.op_Addition(new Vector2(-num2, key.center.y - this.targetTech.center.y), zero2),
        Vector2.op_Addition(new Vector2((float) (-((double) this.targetTech.center.x - (double) num1 - ((double) key.center.x + (double) num1)) + 2.0), key.center.y - this.targetTech.center.y), zero2)
      };
      component.LineThickness = (float) this.lineThickness_inactive;
      ((Graphic) component).color = this.inactiveLineColor;
      this.techLineMap.Add(key, component);
    }
    this.QueueStateChanged(false);
    if (this.targetTech == null)
      return;
    foreach (TechInstance research in Research.Instance.GetResearchQueue())
    {
      if (research.tech == this.targetTech)
        this.QueueStateChanged(true);
    }
  }

  public void SetTech(Tech newTech)
  {
    if (newTech == null)
    {
      Debug.LogError((object) "The research provided is null!");
    }
    else
    {
      if (this.targetTech == newTech)
        return;
      foreach (ResearchType type in Research.Instance.researchTypes.Types)
      {
        if (newTech.costsByResearchTypeID.ContainsKey(type.id) && (double) newTech.costsByResearchTypeID[type.id] > 0.0)
        {
          GameObject gameObject = Util.KInstantiateUI(this.progressBarPrefab, ((Component) this.progressBarContainer).gameObject, true);
          Image componentsInChild = gameObject.GetComponentsInChildren<Image>()[2];
          Image component = ((Component) gameObject.transform.Find("Icon")).GetComponent<Image>();
          ((Graphic) componentsInChild).color = type.color;
          Sprite sprite = type.sprite;
          component.sprite = sprite;
          this.progressBarsByResearchTypeID[type.id] = gameObject;
        }
      }
      if (Object.op_Equality((Object) this.researchScreen, (Object) null))
        this.researchScreen = ((Component) this.transform.parent).GetComponentInParent<ResearchScreen>();
      if (newTech.IsComplete())
        this.ResearchCompleted(false);
      this.targetTech = newTech;
      ((TMP_Text) this.researchName).text = this.targetTech.Name;
      string str1 = "";
      foreach (TechItem unlockedItem in this.targetTech.unlockedItems)
      {
        HierarchyReferences component = this.GetFreeIcon().GetComponent<HierarchyReferences>();
        if (str1 != "")
          str1 += ", ";
        str1 += unlockedItem.Name;
        ((Image) component.GetReference<KImage>("Icon")).sprite = unlockedItem.UISprite();
        component.GetReference<KImage>("Background");
        ((Component) component.GetReference<KImage>("DLCOverlay")).gameObject.SetActive(!DlcManager.IsValidForVanilla(unlockedItem.dlcIds));
        string str2 = string.Format("{0}\n{1}", (object) unlockedItem.Name, (object) unlockedItem.description);
        if (!DlcManager.IsValidForVanilla(unlockedItem.dlcIds))
          str2 += (string) RESEARCH.MESSAGING.DLC.EXPANSION1;
        ((Component) component).GetComponent<ToolTip>().toolTip = str2;
      }
      string str3 = string.Format((string) STRINGS.UI.RESEARCHSCREEN_UNLOCKSTOOLTIP, (object) str1);
      ((Component) this.researchName).GetComponent<ToolTip>().toolTip = string.Format("{0}\n{1}\n\n{2}", (object) this.targetTech.Name, (object) this.targetTech.desc, (object) str3);
      this.toggle.ClearOnClick();
      this.toggle.onClick += new System.Action(this.OnResearchClicked);
      // ISSUE: method pointer
      this.toggle.onPointerEnter += new KToggle.PointerEvent((object) this, __methodptr(\u003CSetTech\u003Eb__34_0));
      this.toggle.soundPlayer.AcceptClickCondition = (Func<bool>) (() => !this.targetTech.IsComplete());
      // ISSUE: method pointer
      this.toggle.onPointerExit += new KToggle.PointerEvent((object) this, __methodptr(\u003CSetTech\u003Eb__34_2));
    }
  }

  public void SetEverythingOff()
  {
    if (!this.isOn)
      return;
    ((Component) this.borderHighlight).gameObject.SetActive(false);
    foreach (KeyValuePair<Tech, UILineRenderer> techLine in this.techLineMap)
    {
      techLine.Value.LineThickness = (float) this.lineThickness_inactive;
      ((Graphic) techLine.Value).color = this.inactiveLineColor;
    }
    this.isOn = false;
  }

  public void SetEverythingOn()
  {
    if (this.isOn)
      return;
    this.UpdateProgressBars();
    ((Component) this.borderHighlight).gameObject.SetActive(true);
    foreach (KeyValuePair<Tech, UILineRenderer> techLine in this.techLineMap)
    {
      techLine.Value.LineThickness = (float) this.lineThickness_active;
      ((Graphic) techLine.Value).color = this.activeLineColor;
    }
    this.transform.SetAsLastSibling();
    this.isOn = true;
  }

  public void OnHover(bool entered, Tech hoverSource)
  {
    this.SetEverythingOn();
    foreach (Tech tech in this.targetTech.requiredTech)
    {
      ResearchEntry entry = this.researchScreen.GetEntry(tech);
      if (Object.op_Inequality((Object) entry, (Object) null))
        entry.OnHover(entered, this.targetTech);
    }
  }

  private void OnResearchClicked()
  {
    TechInstance activeResearch = Research.Instance.GetActiveResearch();
    if (activeResearch != null && activeResearch.tech != this.targetTech)
      this.researchScreen.CancelResearch();
    Research.Instance.SetActiveResearch(this.targetTech, true);
    if (DebugHandler.InstantBuildMode)
      Research.Instance.CompleteQueue();
    this.UpdateProgressBars();
  }

  private void OnResearchCanceled()
  {
    if (this.targetTech.IsComplete())
      return;
    this.toggle.ClearOnClick();
    this.toggle.onClick += new System.Action(this.OnResearchClicked);
    this.researchScreen.CancelResearch();
    Research.Instance.CancelResearch(this.targetTech);
  }

  public void QueueStateChanged(bool isSelected)
  {
    if (isSelected)
    {
      if (!this.targetTech.IsComplete())
      {
        this.toggle.isOn = true;
        ((Graphic) this.BG).color = this.pendingColor;
        ((Graphic) this.titleBG).color = this.pendingHeaderColor;
        this.toggle.ClearOnClick();
        this.toggle.onClick += new System.Action(this.OnResearchCanceled);
      }
      else
        this.toggle.isOn = false;
      foreach (KeyValuePair<string, GameObject> keyValuePair in this.progressBarsByResearchTypeID)
        ((Graphic) ((Component) keyValuePair.Value.transform.GetChild(0)).GetComponentsInChildren<Image>()[1]).color = Color.white;
      foreach (Graphic componentsInChild in this.iconPanel.GetComponentsInChildren<Image>())
        componentsInChild.material = this.StandardUIMaterial;
    }
    else if (this.targetTech.IsComplete())
    {
      this.toggle.isOn = false;
      ((Graphic) this.BG).color = this.completedColor;
      ((Graphic) this.titleBG).color = this.completedHeaderColor;
      this.defaultColor = this.completedColor;
      this.toggle.ClearOnClick();
      foreach (KeyValuePair<string, GameObject> keyValuePair in this.progressBarsByResearchTypeID)
        ((Graphic) ((Component) keyValuePair.Value.transform.GetChild(0)).GetComponentsInChildren<Image>()[1]).color = Color.white;
      foreach (Graphic componentsInChild in this.iconPanel.GetComponentsInChildren<Image>())
        componentsInChild.material = this.StandardUIMaterial;
    }
    else
    {
      this.toggle.isOn = false;
      ((Graphic) this.BG).color = this.defaultColor;
      ((Graphic) this.titleBG).color = this.incompleteHeaderColor;
      this.toggle.ClearOnClick();
      this.toggle.onClick += new System.Action(this.OnResearchClicked);
      foreach (KeyValuePair<string, GameObject> keyValuePair in this.progressBarsByResearchTypeID)
        ((Graphic) ((Component) keyValuePair.Value.transform.GetChild(0)).GetComponentsInChildren<Image>()[1]).color = new Color(0.521568656f, 0.521568656f, 0.521568656f);
    }
  }

  public void UpdateFilterState(bool state) => ((Component) this.filterLowlight).gameObject.SetActive(!state);

  public void SetPercentage(float percent)
  {
  }

  public void UpdateProgressBars()
  {
    foreach (KeyValuePair<string, GameObject> keyValuePair in this.progressBarsByResearchTypeID)
    {
      Transform child = keyValuePair.Value.transform.GetChild(0);
      float num1;
      float num2;
      if (this.targetTech.IsComplete())
      {
        num1 = 1f;
        LocText componentInChildren = ((Component) child).GetComponentInChildren<LocText>();
        num2 = this.targetTech.costsByResearchTypeID[keyValuePair.Key];
        string str1 = num2.ToString();
        num2 = this.targetTech.costsByResearchTypeID[keyValuePair.Key];
        string str2 = num2.ToString();
        string str3 = str1 + "/" + str2;
        ((TMP_Text) componentInChildren).text = str3;
      }
      else
      {
        TechInstance orAdd = Research.Instance.GetOrAdd(this.targetTech);
        if (orAdd != null)
        {
          LocText componentInChildren = ((Component) child).GetComponentInChildren<LocText>();
          num2 = orAdd.progressInventory.PointsByTypeID[keyValuePair.Key];
          string str4 = num2.ToString();
          num2 = this.targetTech.costsByResearchTypeID[keyValuePair.Key];
          string str5 = num2.ToString();
          string str6 = str4 + "/" + str5;
          ((TMP_Text) componentInChildren).text = str6;
          num1 = orAdd.progressInventory.PointsByTypeID[keyValuePair.Key] / this.targetTech.costsByResearchTypeID[keyValuePair.Key];
        }
        else
          continue;
      }
      ((Component) child).GetComponentsInChildren<Image>()[2].fillAmount = num1;
      ((Component) child).GetComponent<ToolTip>().SetSimpleTooltip(Research.Instance.researchTypes.GetResearchType(keyValuePair.Key).description);
    }
  }

  private GameObject GetFreeIcon()
  {
    GameObject freeIcon = Util.KInstantiateUI(this.iconPrefab, this.iconPanel, false);
    freeIcon.SetActive(true);
    return freeIcon;
  }

  private Image GetFreeLine() => Util.KInstantiateUI<Image>(this.linePrefab.gameObject, ((Component) this).gameObject, false);

  public void ResearchCompleted(bool notify = true)
  {
    ((Graphic) this.BG).color = this.completedColor;
    ((Graphic) this.titleBG).color = this.completedHeaderColor;
    this.defaultColor = this.completedColor;
    if (notify)
    {
      this.unlockedTechMetric[ResearchEntry.UnlockedTechKey] = (object) this.targetTech.Id;
      ThreadedHttps<KleiMetrics>.Instance.SendEvent(this.unlockedTechMetric, nameof (ResearchCompleted));
    }
    this.toggle.ClearOnClick();
    if (!notify)
      return;
    ResearchCompleteMessage researchCompleteMessage = new ResearchCompleteMessage(this.targetTech);
    MusicManager.instance.PlaySong("Stinger_ResearchComplete");
    Messenger.Instance.QueueMessage((Message) researchCompleteMessage);
  }
}
