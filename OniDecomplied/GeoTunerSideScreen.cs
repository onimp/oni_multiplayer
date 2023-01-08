// Decompiled with JetBrains decompiler
// Type: GeoTunerSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GeoTunerSideScreen : SideScreenContent
{
  private GeoTuner.Instance targetGeotuner;
  public GameObject rowPrefab;
  public RectTransform rowContainer;
  [SerializeField]
  private TextStyleSetting AnalyzedTextStyle;
  [SerializeField]
  private TextStyleSetting UnanalyzedTextStyle;
  public Dictionary<object, GameObject> rows = new Dictionary<object, GameObject>();
  private int uiRefreshSubHandle = -1;

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    this.rowPrefab.SetActive(false);
    if (!show)
      return;
    this.RefreshOptions();
  }

  public override bool IsValidForTarget(GameObject target) => target.GetSMI<GeoTuner.Instance>() != null;

  public override void SetTarget(GameObject target)
  {
    this.targetGeotuner = target.GetSMI<GeoTuner.Instance>();
    this.RefreshOptions();
    this.uiRefreshSubHandle = KMonoBehaviourExtensions.Subscribe(target, 1980521255, new Action<object>(this.RefreshOptions));
  }

  public override void ClearTarget()
  {
    if (this.uiRefreshSubHandle == -1 || this.targetGeotuner == null)
      return;
    KMonoBehaviourExtensions.Unsubscribe(this.targetGeotuner.gameObject, this.uiRefreshSubHandle);
    this.uiRefreshSubHandle = -1;
  }

  private void RefreshOptions(object data = null)
  {
    int idx = 0;
    int num = idx + 1;
    this.SetRow(idx, (string) STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.NOTHING, Assets.GetSprite(HashedString.op_Implicit("action_building_disabled")), (Geyser) null, true);
    List<Geyser> items = Components.Geysers.GetItems(this.targetGeotuner.GetMyWorldId());
    foreach (Geyser geyser in items)
    {
      if (((Component) geyser).GetComponent<Studyable>().Studied)
        this.SetRow(num++, STRINGS.UI.StripLinkFormatting(((Component) geyser).GetProperName()), Def.GetUISprite((object) ((Component) geyser).gameObject).first, geyser, true);
    }
    foreach (Geyser geyser in items)
    {
      if (!((Component) geyser).GetComponent<Studyable>().Studied && ((Component) geyser).GetComponent<Uncoverable>().IsUncovered)
        this.SetRow(num++, STRINGS.UI.StripLinkFormatting(((Component) geyser).GetProperName()), Def.GetUISprite((object) ((Component) geyser).gameObject).first, geyser, false);
    }
    for (int index = num; index < ((Transform) this.rowContainer).childCount; ++index)
      ((Component) ((Transform) this.rowContainer).GetChild(index)).gameObject.SetActive(false);
  }

  private void ClearRows()
  {
    for (int index = ((Transform) this.rowContainer).childCount - 1; index >= 0; --index)
      Util.KDestroyGameObject((Component) ((Transform) this.rowContainer).GetChild(index));
    this.rows.Clear();
  }

  private void SetRow(int idx, string name, Sprite icon, Geyser geyser, bool studied)
  {
    bool flag = Object.op_Equality((Object) geyser, (Object) null);
    GameObject gameObject = idx >= ((Transform) this.rowContainer).childCount ? Util.KInstantiateUI(this.rowPrefab, ((Component) this.rowContainer).gameObject, true) : ((Component) ((Transform) this.rowContainer).GetChild(idx)).gameObject;
    HierarchyReferences component1 = gameObject.GetComponent<HierarchyReferences>();
    LocText reference1 = component1.GetReference<LocText>("label");
    ((TMP_Text) reference1).text = name;
    reference1.textStyleSetting = studied | flag ? this.AnalyzedTextStyle : this.UnanalyzedTextStyle;
    reference1.ApplySettings();
    Image reference2 = component1.GetReference<Image>(nameof (icon));
    reference2.sprite = icon;
    ((Graphic) reference2).color = studied ? Color.white : new Color(0.0f, 0.0f, 0.0f, 0.5f);
    if (flag)
      ((Graphic) reference2).color = Color.black;
    int count = Components.GeoTuners.GetItems(this.targetGeotuner.GetMyWorldId()).Count<GeoTuner.Instance>((Func<GeoTuner.Instance, bool>) (x => Object.op_Equality((Object) x.GetFutureGeyser(), (Object) geyser)));
    int geotunedCount = Components.GeoTuners.GetItems(this.targetGeotuner.GetMyWorldId()).Count<GeoTuner.Instance>((Func<GeoTuner.Instance, bool>) (x => Object.op_Equality((Object) x.GetFutureGeyser(), (Object) geyser) || Object.op_Equality((Object) x.GetAssignedGeyser(), (Object) geyser)));
    ToolTip[] componentsInChildren = gameObject.GetComponentsInChildren<ToolTip>();
    ToolTip toolTip1 = ((IEnumerable<ToolTip>) componentsInChildren).First<ToolTip>();
    bool usingStudiedTooltip = Object.op_Inequality((Object) geyser, (Object) null) && flag | studied;
    toolTip1.SetSimpleTooltip(usingStudiedTooltip ? STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP.ToString() : STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.UNSTUDIED_TOOLTIP.ToString());
    ((Behaviour) toolTip1).enabled = Object.op_Inequality((Object) geyser, (Object) null);
    toolTip1.OnToolTip = (Func<string>) (() =>
    {
      if (!usingStudiedTooltip)
        return STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.UNSTUDIED_TOOLTIP.ToString();
      if (Object.op_Inequality((Object) geyser, (Object) this.targetGeotuner.GetFutureGeyser()) && geotunedCount >= 5)
        return STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.GEOTUNER_LIMIT_TOOLTIP.ToString();
      Func<float, float, float, float> func = (Func<float, float, float, float>) ((iterationLength, massPerCycle, eruptionDuration) =>
      {
        float num = 600f / iterationLength;
        return massPerCycle / num / eruptionDuration;
      });
      GeoTunerConfig.GeotunedGeyserSettings settingsForGeyser = this.targetGeotuner.def.GetSettingsForGeyser(geyser);
      float temp = Geyser.temperatureModificationMethod == Geyser.ModificationMethod.Percentages ? settingsForGeyser.template.temperatureModifier * geyser.configuration.geyserType.temperature : settingsForGeyser.template.temperatureModifier;
      float mass = ((Func<float, float>) (emissionPerCycleModifier =>
      {
        float num = 600f / geyser.configuration.GetIterationLength();
        return emissionPerCycleModifier / num / geyser.configuration.GetOnDuration();
      }))(Geyser.massModificationMethod == Geyser.ModificationMethod.Percentages ? settingsForGeyser.template.massPerCycleModifier * geyser.configuration.scaledRate : settingsForGeyser.template.massPerCycleModifier);
      float temperature = geyser.configuration.geyserType.temperature;
      double num1 = (double) func(geyser.configuration.scaledIterationLength, geyser.configuration.scaledRate, geyser.configuration.scaledIterationLength * geyser.configuration.scaledIterationPercent);
      string str1 = ((double) temp > 0.0 ? "+" : "") + GameUtil.GetFormattedTemperature(temp, interpretation: GameUtil.TemperatureInterpretation.Relative);
      string str2 = ((double) mass > 0.0 ? "+" : "") + GameUtil.GetFormattedMass(mass, GameUtil.TimeSlice.PerSecond, floatFormat: "{0:0.##}");
      string newValue = settingsForGeyser.material.ProperName();
      return ((string) STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP + "\n" + "\n" + (string) STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP_MATERIAL).Replace("{MATERIAL}", newValue) + "\n" + str1 + "\n" + str2 + "\n" + "\n" + (string) STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP_VISIT_GEYSER;
    });
    if (usingStudiedTooltip && count > 0)
    {
      ToolTip toolTip2 = ((IEnumerable<ToolTip>) componentsInChildren).Last<ToolTip>();
      toolTip2.SetSimpleTooltip("");
      toolTip2.OnToolTip = (Func<string>) (() => STRINGS.UI.UISIDESCREENS.GEOTUNERSIDESCREEN.STUDIED_TOOLTIP_NUMBER_HOVERED.ToString().Replace("{0}", count.ToString()));
    }
    LocText reference3 = component1.GetReference<LocText>("amount");
    ((TMP_Text) reference3).SetText(count.ToString());
    ((Component) ((TMP_Text) reference3).transform.parent).gameObject.SetActive(!flag && count > 0);
    MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
    component2.ChangeState(Object.op_Equality((Object) this.targetGeotuner.GetFutureGeyser(), (Object) geyser) ? 1 : 0);
    component2.onClick = (System.Action) (() =>
    {
      if (!Object.op_Equality((Object) geyser, (Object) null) && !((Component) geyser).GetComponent<Studyable>().Studied || Object.op_Equality((Object) geyser, (Object) this.targetGeotuner.GetFutureGeyser()) || Components.GeoTuners.GetItems(this.targetGeotuner.GetMyWorldId()).Count<GeoTuner.Instance>((Func<GeoTuner.Instance, bool>) (x => Object.op_Equality((Object) x.GetFutureGeyser(), (Object) geyser) || Object.op_Equality((Object) x.GetAssignedGeyser(), (Object) geyser))) + 1 > 5)
        return;
      this.targetGeotuner.AssignFutureGeyser(geyser);
      this.RefreshOptions();
    });
    component2.onDoubleClick = (Func<bool>) (() =>
    {
      if (!Object.op_Inequality((Object) geyser, (Object) null))
        return false;
      CameraController.Instance.CameraGoTo(TransformExtensions.GetPosition(geyser.transform));
      return true;
    });
  }
}
