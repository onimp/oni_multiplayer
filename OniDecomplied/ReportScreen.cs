// Decompiled with JetBrains decompiler
// Type: ReportScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ReportScreen : KScreen
{
  [SerializeField]
  private LocText title;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KButton prevButton;
  [SerializeField]
  private KButton nextButton;
  [SerializeField]
  private KButton summaryButton;
  [SerializeField]
  private GameObject lineItem;
  [SerializeField]
  private GameObject lineItemSpacer;
  [SerializeField]
  private GameObject lineItemHeader;
  [SerializeField]
  private GameObject contentFolder;
  private Dictionary<string, GameObject> lineItems = new Dictionary<string, GameObject>();
  private ReportManager.DailyReport currentReport;

  public static ReportScreen Instance { get; private set; }

  public static void DestroyInstance() => ReportScreen.Instance = (ReportScreen) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ReportScreen.Instance = this;
    this.closeButton.onClick += (System.Action) (() => ManagementMenu.Instance.CloseAll());
    this.prevButton.onClick += (System.Action) (() => this.ShowReport(this.currentReport.day - 1));
    this.nextButton.onClick += (System.Action) (() => this.ShowReport(this.currentReport.day + 1));
    this.summaryButton.onClick += (System.Action) (() => MainMenu.ActivateRetiredColoniesScreenFromData(((Component) ((KMonoBehaviour) PauseScreen.Instance).transform.parent).gameObject, RetireColonyUtility.GetCurrentColonyRetiredColonyData()));
    this.ConsumeMouseScroll = true;
  }

  protected virtual void OnSpawn() => base.OnSpawn();

  protected virtual void OnShow(bool bShow)
  {
    base.OnShow(bShow);
    if (!Object.op_Inequality((Object) ReportManager.Instance, (Object) null))
      return;
    this.currentReport = ReportManager.Instance.TodaysReport;
  }

  public void SetTitle(string title) => ((TMP_Text) this.title).text = title;

  public virtual void ScreenUpdate(bool b)
  {
    base.ScreenUpdate(b);
    this.Refresh();
  }

  private void Refresh()
  {
    Debug.Assert(this.currentReport != null);
    if (this.currentReport.day == ReportManager.Instance.TodaysReport.day)
      this.SetTitle(string.Format((string) UI.ENDOFDAYREPORT.DAY_TITLE_TODAY, (object) this.currentReport.day));
    else if (this.currentReport.day == ReportManager.Instance.TodaysReport.day - 1)
      this.SetTitle(string.Format((string) UI.ENDOFDAYREPORT.DAY_TITLE_YESTERDAY, (object) this.currentReport.day));
    else
      this.SetTitle(string.Format((string) UI.ENDOFDAYREPORT.DAY_TITLE, (object) this.currentReport.day));
    bool flag1 = this.currentReport.day < ReportManager.Instance.TodaysReport.day;
    this.nextButton.isInteractable = flag1;
    if (flag1)
    {
      ((Component) this.nextButton).GetComponent<ToolTip>().toolTip = string.Format((string) UI.ENDOFDAYREPORT.DAY_TITLE, (object) (this.currentReport.day + 1));
      ((Behaviour) ((Component) this.nextButton).GetComponent<ToolTip>()).enabled = true;
    }
    else
      ((Behaviour) ((Component) this.nextButton).GetComponent<ToolTip>()).enabled = false;
    bool flag2 = this.currentReport.day > 1;
    this.prevButton.isInteractable = flag2;
    if (flag2)
    {
      ((Component) this.prevButton).GetComponent<ToolTip>().toolTip = string.Format((string) UI.ENDOFDAYREPORT.DAY_TITLE, (object) (this.currentReport.day - 1));
      ((Behaviour) ((Component) this.prevButton).GetComponent<ToolTip>()).enabled = true;
    }
    else
      ((Behaviour) ((Component) this.prevButton).GetComponent<ToolTip>()).enabled = false;
    this.AddSpacer(0);
    int group = 1;
    foreach (KeyValuePair<ReportManager.ReportType, ReportManager.ReportGroup> reportGroup in ReportManager.Instance.ReportGroups)
    {
      ReportManager.ReportEntry entry = this.currentReport.GetEntry(reportGroup.Key);
      if (group != reportGroup.Value.group)
      {
        group = reportGroup.Value.group;
        this.AddSpacer(group);
      }
      bool is_line_active = (double) entry.accumulate != 0.0 || reportGroup.Value.reportIfZero;
      if (reportGroup.Value.isHeader)
        this.CreateHeader(reportGroup.Value);
      else if (is_line_active)
        this.CreateOrUpdateLine(entry, reportGroup.Value, is_line_active);
    }
  }

  public void ShowReport(int day)
  {
    this.currentReport = ReportManager.Instance.FindReport(day);
    Debug.Assert(this.currentReport != null, (object) ("Can't find report for day: " + day.ToString()));
    this.Refresh();
  }

  private GameObject AddSpacer(int group)
  {
    GameObject gameObject;
    if (this.lineItems.ContainsKey(group.ToString()))
    {
      gameObject = this.lineItems[group.ToString()];
    }
    else
    {
      gameObject = Util.KInstantiateUI(this.lineItemSpacer, this.contentFolder, false);
      ((Object) gameObject).name = "Spacer" + group.ToString();
      this.lineItems[group.ToString()] = gameObject;
    }
    gameObject.SetActive(true);
    return gameObject;
  }

  private GameObject CreateHeader(ReportManager.ReportGroup reportGroup)
  {
    GameObject header = (GameObject) null;
    this.lineItems.TryGetValue(reportGroup.stringKey, out header);
    if (Object.op_Equality((Object) header, (Object) null))
    {
      header = Util.KInstantiateUI(this.lineItemHeader, this.contentFolder, true);
      ((Object) header).name = "LineItemHeader" + this.lineItems.Count.ToString();
      this.lineItems[reportGroup.stringKey] = header;
    }
    header.SetActive(true);
    header.GetComponent<ReportScreenHeader>().SetMainEntry(reportGroup);
    return header;
  }

  private GameObject CreateOrUpdateLine(
    ReportManager.ReportEntry entry,
    ReportManager.ReportGroup reportGroup,
    bool is_line_active)
  {
    GameObject orUpdateLine = (GameObject) null;
    this.lineItems.TryGetValue(reportGroup.stringKey, out orUpdateLine);
    if (!is_line_active)
    {
      if (Object.op_Inequality((Object) orUpdateLine, (Object) null) && orUpdateLine.activeSelf)
        orUpdateLine.SetActive(false);
    }
    else
    {
      if (Object.op_Equality((Object) orUpdateLine, (Object) null))
      {
        orUpdateLine = Util.KInstantiateUI(this.lineItem, this.contentFolder, true);
        ((Object) orUpdateLine).name = "LineItem" + this.lineItems.Count.ToString();
        this.lineItems[reportGroup.stringKey] = orUpdateLine;
      }
      orUpdateLine.SetActive(true);
      orUpdateLine.GetComponent<ReportScreenEntry>().SetMainEntry(entry, reportGroup);
    }
    return orUpdateLine;
  }

  private void OnClickClose()
  {
    ((KMonoBehaviour) this).PlaySound3D(GlobalAssets.GetSound("HUD_Click_Close"));
    this.Show(false);
  }
}
