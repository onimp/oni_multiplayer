// Decompiled with JetBrains decompiler
// Type: VitalsTableScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VitalsTableScreen : TableScreen
{
  protected override void OnActivate()
  {
    this.has_default_duplicant_row = false;
    this.title = (string) UI.VITALS;
    base.OnActivate();
    this.AddPortraitColumn("Portrait", new Action<IAssignableIdentity, GameObject>(((TableScreen) this).on_load_portrait), (Comparison<IAssignableIdentity>) null);
    this.AddButtonLabelColumn("Names", new Action<IAssignableIdentity, GameObject>(((TableScreen) this).on_load_name_label), new Func<IAssignableIdentity, GameObject, string>(((TableScreen) this).get_value_name_label), (Action<GameObject>) (widget_go => this.GetWidgetRow(widget_go).SelectMinion()), (Action<GameObject>) (widget_go => this.GetWidgetRow(widget_go).SelectAndFocusMinion()), new Comparison<IAssignableIdentity>(((TableScreen) this).compare_rows_alphabetical), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_name), new Action<IAssignableIdentity, GameObject, ToolTip>(((TableScreen) this).on_tooltip_sort_alphabetically));
    this.AddLabelColumn("Stress", new Action<IAssignableIdentity, GameObject>(this.on_load_stress), new Func<IAssignableIdentity, GameObject, string>(this.get_value_stress_label), new Comparison<IAssignableIdentity>(this.compare_rows_stress), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_stress), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_sort_stress), 64, true);
    this.AddLabelColumn("QOLExpectations", new Action<IAssignableIdentity, GameObject>(this.on_load_qualityoflife_expectations), new Func<IAssignableIdentity, GameObject, string>(this.get_value_qualityoflife_expectations_label), new Comparison<IAssignableIdentity>(this.compare_rows_qualityoflife_expectations), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_qualityoflife_expectations), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_sort_qualityoflife_expectations), should_refresh_columns: true);
    this.AddLabelColumn("Fullness", new Action<IAssignableIdentity, GameObject>(this.on_load_fullness), new Func<IAssignableIdentity, GameObject, string>(this.get_value_fullness_label), new Comparison<IAssignableIdentity>(this.compare_rows_fullness), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_fullness), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_sort_fullness), 96, true);
    this.AddLabelColumn("EatenToday", new Action<IAssignableIdentity, GameObject>(this.on_load_eaten_today), new Func<IAssignableIdentity, GameObject, string>(this.get_value_eaten_today_label), new Comparison<IAssignableIdentity>(this.compare_rows_eaten_today), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_eaten_today), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_sort_eaten_today), 96, true);
    this.AddLabelColumn("Health", new Action<IAssignableIdentity, GameObject>(this.on_load_health), new Func<IAssignableIdentity, GameObject, string>(this.get_value_health_label), new Comparison<IAssignableIdentity>(this.compare_rows_health), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_health), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_sort_health), 64, true);
    this.AddLabelColumn("Immunity", new Action<IAssignableIdentity, GameObject>(this.on_load_sickness), new Func<IAssignableIdentity, GameObject, string>(this.get_value_sickness_label), new Comparison<IAssignableIdentity>(this.compare_rows_sicknesses), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_sicknesses), new Action<IAssignableIdentity, GameObject, ToolTip>(this.on_tooltip_sort_sicknesses), 192, true);
  }

  private void on_load_stress(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
    if (minion != null)
      ((TMP_Text) componentInChildren).text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
    else
      ((TMP_Text) componentInChildren).text = widgetRow.isDefault ? "" : UI.VITALSSCREEN.STRESS.ToString();
  }

  private string get_value_stress_label(IAssignableIdentity identity, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    if (widgetRow.rowType == TableRow.RowType.Minion)
    {
      MinionIdentity cmp = identity as MinionIdentity;
      if (Object.op_Inequality((Object) cmp, (Object) null))
        return Db.Get().Amounts.Stress.Lookup((Component) cmp).GetValueString();
    }
    else if (widgetRow.rowType == TableRow.RowType.StoredMinon)
      return (string) UI.TABLESCREENS.NA;
    return "";
  }

  private int compare_rows_stress(IAssignableIdentity a, IAssignableIdentity b)
  {
    MinionIdentity cmp1 = a as MinionIdentity;
    MinionIdentity cmp2 = b as MinionIdentity;
    if (Object.op_Equality((Object) cmp1, (Object) null) && Object.op_Equality((Object) cmp2, (Object) null))
      return 0;
    if (Object.op_Equality((Object) cmp1, (Object) null))
      return -1;
    if (Object.op_Equality((Object) cmp2, (Object) null))
      return 1;
    float num = Db.Get().Amounts.Stress.Lookup((Component) cmp1).value;
    return Db.Get().Amounts.Stress.Lookup((Component) cmp2).value.CompareTo(num);
  }

  protected void on_tooltip_stress(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Minion:
        MinionIdentity cmp = minion as MinionIdentity;
        if (!Object.op_Inequality((Object) cmp, (Object) null))
          break;
        tooltip.AddMultiStringTooltip(Db.Get().Amounts.Stress.Lookup((Component) cmp).GetTooltip(), (TextStyleSetting) null);
        break;
      case TableRow.RowType.StoredMinon:
        this.StoredMinionTooltip(minion, tooltip);
        break;
    }
  }

  protected void on_tooltip_sort_stress(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Header:
        tooltip.AddMultiStringTooltip((string) UI.TABLESCREENS.COLUMN_SORT_BY_STRESS, (TextStyleSetting) null);
        break;
    }
  }

  private void on_load_qualityoflife_expectations(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
    if (minion != null)
      ((TMP_Text) componentInChildren).text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
    else
      ((TMP_Text) componentInChildren).text = widgetRow.isDefault ? "" : UI.VITALSSCREEN.QUALITYOFLIFE_EXPECTATIONS.ToString();
  }

  private string get_value_qualityoflife_expectations_label(
    IAssignableIdentity identity,
    GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    if (widgetRow.rowType == TableRow.RowType.Minion)
    {
      MinionIdentity cmp = identity as MinionIdentity;
      if (Object.op_Inequality((Object) cmp, (Object) null))
        return Db.Get().Attributes.QualityOfLife.Lookup((Component) cmp).GetFormattedValue();
    }
    else if (widgetRow.rowType == TableRow.RowType.StoredMinon)
      return (string) UI.TABLESCREENS.NA;
    return "";
  }

  private int compare_rows_qualityoflife_expectations(IAssignableIdentity a, IAssignableIdentity b)
  {
    MinionIdentity cmp1 = a as MinionIdentity;
    MinionIdentity cmp2 = b as MinionIdentity;
    if (Object.op_Equality((Object) cmp1, (Object) null) && Object.op_Equality((Object) cmp2, (Object) null))
      return 0;
    if (Object.op_Equality((Object) cmp1, (Object) null))
      return -1;
    return Object.op_Equality((Object) cmp2, (Object) null) ? 1 : Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) cmp1).GetTotalValue().CompareTo(Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component) cmp2).GetTotalValue());
  }

  protected void on_tooltip_qualityoflife_expectations(
    IAssignableIdentity identity,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Minion:
        MinionIdentity cmp = identity as MinionIdentity;
        if (!Object.op_Inequality((Object) cmp, (Object) null))
          break;
        tooltip.AddMultiStringTooltip(Db.Get().Attributes.QualityOfLife.Lookup((Component) cmp).GetAttributeValueTooltip(), (TextStyleSetting) null);
        break;
      case TableRow.RowType.StoredMinon:
        this.StoredMinionTooltip(identity, tooltip);
        break;
    }
  }

  protected void on_tooltip_sort_qualityoflife_expectations(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Header:
        tooltip.AddMultiStringTooltip((string) UI.TABLESCREENS.COLUMN_SORT_BY_EXPECTATIONS, (TextStyleSetting) null);
        break;
    }
  }

  private void on_load_health(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
    if (minion != null)
      ((TMP_Text) componentInChildren).text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
    else
      ((TMP_Text) componentInChildren).text = widgetRow.isDefault ? "" : (((TMP_Text) componentInChildren).text = UI.VITALSSCREEN_HEALTH.ToString());
  }

  private string get_value_health_label(IAssignableIdentity minion, GameObject widget_go)
  {
    if (minion != null)
    {
      TableRow widgetRow = this.GetWidgetRow(widget_go);
      if (widgetRow.rowType == TableRow.RowType.Minion && Object.op_Inequality((Object) (minion as MinionIdentity), (Object) null))
        return Db.Get().Amounts.HitPoints.Lookup((Component) (minion as MinionIdentity)).GetValueString();
      if (widgetRow.rowType == TableRow.RowType.StoredMinon)
        return (string) UI.TABLESCREENS.NA;
    }
    return "";
  }

  private int compare_rows_health(IAssignableIdentity a, IAssignableIdentity b)
  {
    MinionIdentity cmp1 = a as MinionIdentity;
    MinionIdentity cmp2 = b as MinionIdentity;
    if (Object.op_Equality((Object) cmp1, (Object) null) && Object.op_Equality((Object) cmp2, (Object) null))
      return 0;
    if (Object.op_Equality((Object) cmp1, (Object) null))
      return -1;
    if (Object.op_Equality((Object) cmp2, (Object) null))
      return 1;
    float num = Db.Get().Amounts.HitPoints.Lookup((Component) cmp1).value;
    return Db.Get().Amounts.HitPoints.Lookup((Component) cmp2).value.CompareTo(num);
  }

  protected void on_tooltip_health(
    IAssignableIdentity identity,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Minion:
        MinionIdentity cmp = identity as MinionIdentity;
        if (!Object.op_Inequality((Object) cmp, (Object) null))
          break;
        tooltip.AddMultiStringTooltip(Db.Get().Amounts.HitPoints.Lookup((Component) cmp).GetTooltip(), (TextStyleSetting) null);
        break;
      case TableRow.RowType.StoredMinon:
        this.StoredMinionTooltip(identity, tooltip);
        break;
    }
  }

  protected void on_tooltip_sort_health(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Header:
        tooltip.AddMultiStringTooltip((string) UI.TABLESCREENS.COLUMN_SORT_BY_HITPOINTS, (TextStyleSetting) null);
        break;
    }
  }

  private void on_load_sickness(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
    if (minion != null)
      ((TMP_Text) componentInChildren).text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
    else
      ((TMP_Text) componentInChildren).text = widgetRow.isDefault ? "" : UI.VITALSSCREEN_SICKNESS.ToString();
  }

  private string get_value_sickness_label(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    if (widgetRow.rowType == TableRow.RowType.Minion)
    {
      MinionIdentity cmp = minion as MinionIdentity;
      if (Object.op_Inequality((Object) cmp, (Object) null))
      {
        List<KeyValuePair<string, float>> keyValuePairList = new List<KeyValuePair<string, float>>();
        foreach (SicknessInstance sickness in (Modifications<Sickness, SicknessInstance>) ((Component) cmp).GetComponent<MinionModifiers>().sicknesses)
          keyValuePairList.Add(new KeyValuePair<string, float>(sickness.modifier.Name, sickness.GetInfectedTimeRemaining()));
        if (DlcManager.FeatureRadiationEnabled())
        {
          RadiationMonitor.Instance smi = ((Component) cmp).GetSMI<RadiationMonitor.Instance>();
          if (smi != null && smi.sm.isSick.Get(smi))
          {
            Effects component = ((Component) cmp).GetComponent<Effects>();
            string key = !component.HasEffect(RadiationMonitor.minorSicknessEffect) ? (!component.HasEffect(RadiationMonitor.majorSicknessEffect) ? (!component.HasEffect(RadiationMonitor.extremeSicknessEffect) ? (string) DUPLICANTS.MODIFIERS.RADIATIONEXPOSUREDEADLY.NAME : Db.Get().effects.Get(RadiationMonitor.extremeSicknessEffect).Name) : Db.Get().effects.Get(RadiationMonitor.majorSicknessEffect).Name) : Db.Get().effects.Get(RadiationMonitor.minorSicknessEffect).Name;
            keyValuePairList.Add(new KeyValuePair<string, float>(key, smi.SicknessSecondsRemaining()));
          }
        }
        if (keyValuePairList.Count <= 0)
          return (string) UI.VITALSSCREEN.NO_SICKNESSES;
        string valueSicknessLabel = "";
        if (keyValuePairList.Count > 1)
        {
          float seconds = 0.0f;
          foreach (KeyValuePair<string, float> keyValuePair in keyValuePairList)
            seconds = Mathf.Min(new float[1]
            {
              keyValuePair.Value
            });
          valueSicknessLabel += string.Format((string) UI.VITALSSCREEN.MULTIPLE_SICKNESSES, (object) GameUtil.GetFormattedCycles(seconds));
        }
        else
        {
          foreach (KeyValuePair<string, float> keyValuePair in keyValuePairList)
          {
            if (!string.IsNullOrEmpty(valueSicknessLabel))
              valueSicknessLabel += "\n";
            valueSicknessLabel += string.Format((string) UI.VITALSSCREEN.SICKNESS_REMAINING, (object) keyValuePair.Key, (object) GameUtil.GetFormattedCycles(keyValuePair.Value));
          }
        }
        return valueSicknessLabel;
      }
    }
    else if (widgetRow.rowType == TableRow.RowType.StoredMinon)
      return (string) UI.TABLESCREENS.NA;
    return "";
  }

  private int compare_rows_sicknesses(IAssignableIdentity a, IAssignableIdentity b) => 0.0f.CompareTo(0.0f);

  protected void on_tooltip_sicknesses(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Minion:
        MinionIdentity cmp = minion as MinionIdentity;
        if (!Object.op_Inequality((Object) cmp, (Object) null))
          break;
        bool flag = false;
        List<KeyValuePair<string, float>> keyValuePairList = new List<KeyValuePair<string, float>>();
        if (DlcManager.FeatureRadiationEnabled())
        {
          RadiationMonitor.Instance smi = ((Component) cmp).GetSMI<RadiationMonitor.Instance>();
          if (smi != null && smi.sm.isSick.Get(smi))
          {
            tooltip.AddMultiStringTooltip(smi.GetEffectStatusTooltip(), (TextStyleSetting) null);
            flag = true;
          }
        }
        Sicknesses sicknesses = ((Component) cmp).GetComponent<MinionModifiers>().sicknesses;
        if (sicknesses.IsInfected())
        {
          flag = true;
          foreach (SicknessInstance sicknessInstance in (Modifications<Sickness, SicknessInstance>) sicknesses)
          {
            tooltip.AddMultiStringTooltip(UI.HORIZONTAL_RULE, (TextStyleSetting) null);
            tooltip.AddMultiStringTooltip(sicknessInstance.modifier.Name, (TextStyleSetting) null);
            StatusItem statusItem = sicknessInstance.GetStatusItem();
            tooltip.AddMultiStringTooltip(statusItem.GetTooltip((object) sicknessInstance.ExposureInfo), (TextStyleSetting) null);
          }
        }
        if (flag)
          break;
        tooltip.AddMultiStringTooltip((string) UI.VITALSSCREEN.NO_SICKNESSES, (TextStyleSetting) null);
        break;
      case TableRow.RowType.StoredMinon:
        this.StoredMinionTooltip(minion, tooltip);
        break;
    }
  }

  protected void on_tooltip_sort_sicknesses(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Header:
        tooltip.AddMultiStringTooltip((string) UI.TABLESCREENS.COLUMN_SORT_BY_SICKNESSES, (TextStyleSetting) null);
        break;
    }
  }

  private void on_load_fullness(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
    if (minion != null)
      ((TMP_Text) componentInChildren).text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
    else
      ((TMP_Text) componentInChildren).text = widgetRow.isDefault ? "" : UI.VITALSSCREEN_CALORIES.ToString();
  }

  private string get_value_fullness_label(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    if (widgetRow.rowType == TableRow.RowType.Minion && Object.op_Inequality((Object) (minion as MinionIdentity), (Object) null))
      return Db.Get().Amounts.Calories.Lookup((Component) (minion as MinionIdentity)).GetValueString();
    return widgetRow.rowType == TableRow.RowType.StoredMinon ? (string) UI.TABLESCREENS.NA : "";
  }

  private int compare_rows_fullness(IAssignableIdentity a, IAssignableIdentity b)
  {
    MinionIdentity cmp1 = a as MinionIdentity;
    MinionIdentity cmp2 = b as MinionIdentity;
    if (Object.op_Equality((Object) cmp1, (Object) null) && Object.op_Equality((Object) cmp2, (Object) null))
      return 0;
    if (Object.op_Equality((Object) cmp1, (Object) null))
      return -1;
    if (Object.op_Equality((Object) cmp2, (Object) null))
      return 1;
    float num = Db.Get().Amounts.Calories.Lookup((Component) cmp1).value;
    return Db.Get().Amounts.Calories.Lookup((Component) cmp2).value.CompareTo(num);
  }

  protected void on_tooltip_fullness(
    IAssignableIdentity identity,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Minion:
        MinionIdentity cmp = identity as MinionIdentity;
        if (!Object.op_Inequality((Object) cmp, (Object) null))
          break;
        tooltip.AddMultiStringTooltip(Db.Get().Amounts.Calories.Lookup((Component) cmp).GetTooltip(), (TextStyleSetting) null);
        break;
      case TableRow.RowType.StoredMinon:
        this.StoredMinionTooltip(identity, tooltip);
        break;
    }
  }

  protected void on_tooltip_sort_fullness(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Header:
        tooltip.AddMultiStringTooltip((string) UI.TABLESCREENS.COLUMN_SORT_BY_FULLNESS, (TextStyleSetting) null);
        break;
    }
  }

  protected void on_tooltip_name(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Minion:
        if (minion == null)
          break;
        tooltip.AddMultiStringTooltip(string.Format((string) UI.TABLESCREENS.GOTO_DUPLICANT_BUTTON, (object) minion.GetProperName()), (TextStyleSetting) null);
        break;
    }
  }

  private void on_load_eaten_today(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
    if (minion != null)
      ((TMP_Text) componentInChildren).text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
    else
      ((TMP_Text) componentInChildren).text = widgetRow.isDefault ? "" : UI.VITALSSCREEN_EATENTODAY.ToString();
  }

  private static float RationsEatenToday(MinionIdentity minion)
  {
    float num = 0.0f;
    if (Object.op_Inequality((Object) minion, (Object) null))
    {
      RationMonitor.Instance smi = ((Component) minion).GetSMI<RationMonitor.Instance>();
      if (smi != null)
        num = smi.GetRationsAteToday();
    }
    return num;
  }

  private string get_value_eaten_today_label(IAssignableIdentity minion, GameObject widget_go)
  {
    TableRow widgetRow = this.GetWidgetRow(widget_go);
    if (widgetRow.rowType == TableRow.RowType.Minion)
      return GameUtil.GetFormattedCalories(VitalsTableScreen.RationsEatenToday(minion as MinionIdentity));
    return widgetRow.rowType == TableRow.RowType.StoredMinon ? (string) UI.TABLESCREENS.NA : "";
  }

  private int compare_rows_eaten_today(IAssignableIdentity a, IAssignableIdentity b)
  {
    MinionIdentity minion1 = a as MinionIdentity;
    MinionIdentity minion2 = b as MinionIdentity;
    if (Object.op_Equality((Object) minion1, (Object) null) && Object.op_Equality((Object) minion2, (Object) null))
      return 0;
    if (Object.op_Equality((Object) minion1, (Object) null))
      return -1;
    if (Object.op_Equality((Object) minion2, (Object) null))
      return 1;
    float num = VitalsTableScreen.RationsEatenToday(minion1);
    return VitalsTableScreen.RationsEatenToday(minion2).CompareTo(num);
  }

  protected void on_tooltip_eaten_today(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Minion:
        if (minion == null)
          break;
        float calories = VitalsTableScreen.RationsEatenToday(minion as MinionIdentity);
        tooltip.AddMultiStringTooltip(string.Format((string) UI.VITALSSCREEN.EATEN_TODAY_TOOLTIP, (object) GameUtil.GetFormattedCalories(calories)), (TextStyleSetting) null);
        break;
      case TableRow.RowType.StoredMinon:
        this.StoredMinionTooltip(minion, tooltip);
        break;
    }
  }

  protected void on_tooltip_sort_eaten_today(
    IAssignableIdentity minion,
    GameObject widget_go,
    ToolTip tooltip)
  {
    tooltip.ClearMultiStringTooltip();
    switch (this.GetWidgetRow(widget_go).rowType)
    {
      case TableRow.RowType.Header:
        tooltip.AddMultiStringTooltip((string) UI.TABLESCREENS.COLUMN_SORT_BY_EATEN_TODAY, (TextStyleSetting) null);
        break;
    }
  }

  private void StoredMinionTooltip(IAssignableIdentity minion, ToolTip tooltip)
  {
    if (minion == null || !Object.op_Inequality((Object) (minion as StoredMinionIdentity), (Object) null))
      return;
    tooltip.AddMultiStringTooltip(string.Format((string) UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, (object) (minion as StoredMinionIdentity).GetStorageReason(), (object) minion.GetProperName()), (TextStyleSetting) null);
  }
}
