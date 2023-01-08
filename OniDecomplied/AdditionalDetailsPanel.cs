// Decompiled with JetBrains decompiler
// Type: AdditionalDetailsPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdditionalDetailsPanel : TargetScreen
{
  public GameObject attributesLabelTemplate;
  private GameObject detailsPanel;
  private DetailsPanelDrawer drawer;

  public override bool IsValidForTarget(GameObject target) => true;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.detailsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.drawer = new DetailsPanelDrawer(this.attributesLabelTemplate, ((Component) this.detailsPanel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject);
  }

  private void Update() => this.Refresh();

  public override void OnSelectTarget(GameObject target)
  {
    base.OnSelectTarget(target);
    this.Refresh();
  }

  public override void OnDeselectTarget(GameObject target) => base.OnDeselectTarget(target);

  private void Refresh()
  {
    this.drawer.BeginDrawing();
    this.RefreshDetails();
    this.drawer.EndDrawing();
  }

  private GameObject AddOrGetLabel(
    Dictionary<string, GameObject> labels,
    GameObject panel,
    string id)
  {
    GameObject label;
    if (labels.ContainsKey(id))
    {
      label = labels[id];
    }
    else
    {
      label = Util.KInstantiate(this.attributesLabelTemplate, ((Component) panel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject, (string) null);
      label.transform.localScale = new Vector3(1f, 1f, 1f);
      labels[id] = label;
    }
    label.SetActive(true);
    return label;
  }

  private void RefreshDetails()
  {
    this.detailsPanel.SetActive(true);
    ((TMP_Text) this.detailsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.DETAILS.GROUPNAME_DETAILS;
    PrimaryElement component1 = this.selectedTarget.GetComponent<PrimaryElement>();
    CellSelectionObject component2 = this.selectedTarget.GetComponent<CellSelectionObject>();
    float mass;
    float temperature;
    Element element;
    byte diseaseIdx;
    int diseaseCount;
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      mass = component1.Mass;
      temperature = component1.Temperature;
      element = component1.Element;
      diseaseIdx = component1.DiseaseIdx;
      diseaseCount = component1.DiseaseCount;
    }
    else
    {
      if (!Object.op_Inequality((Object) component2, (Object) null))
        return;
      mass = component2.Mass;
      temperature = component2.temperature;
      element = component2.element;
      diseaseIdx = component2.diseaseIdx;
      diseaseCount = component2.diseaseCount;
    }
    bool flag1 = element.id == SimHashes.Vacuum || element.id == SimHashes.Void;
    float specificHeatCapacity = element.specificHeatCapacity;
    float highTemp = element.highTemp;
    float lowTemp = element.lowTemp;
    BuildingComplete component3 = this.selectedTarget.GetComponent<BuildingComplete>();
    float num1 = !Object.op_Inequality((Object) component3, (Object) null) ? -1f : component3.creationTime;
    LogicPorts component4 = this.selectedTarget.GetComponent<LogicPorts>();
    EnergyConsumer component5 = this.selectedTarget.GetComponent<EnergyConsumer>();
    Operational component6 = this.selectedTarget.GetComponent<Operational>();
    Battery component7 = this.selectedTarget.GetComponent<Battery>();
    this.drawer.NewLabel(this.drawer.Format((string) UI.ELEMENTAL.PRIMARYELEMENT.NAME, element.name)).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.PRIMARYELEMENT.TOOLTIP, element.name)).NewLabel(this.drawer.Format((string) UI.ELEMENTAL.MASS.NAME, GameUtil.GetFormattedMass(mass))).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.MASS.TOOLTIP, GameUtil.GetFormattedMass(mass)));
    if ((double) num1 > 0.0)
      this.drawer.NewLabel(this.drawer.Format((string) UI.ELEMENTAL.AGE.NAME, Util.FormatTwoDecimalPlace((float) (((double) GameClock.Instance.GetTime() - (double) num1) / 600.0)))).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.AGE.TOOLTIP, Util.FormatTwoDecimalPlace((float) (((double) GameClock.Instance.GetTime() - (double) num1) / 600.0))));
    int num_cycles = 5;
    float num2;
    float num3;
    float num4;
    if (Object.op_Inequality((Object) component6, (Object) null) && (Object.op_Inequality((Object) component4, (Object) null) || Object.op_Inequality((Object) component5, (Object) null) || Object.op_Inequality((Object) component7, (Object) null)))
    {
      num2 = component6.GetCurrentCycleUptime();
      num3 = component6.GetLastCycleUptime();
      num4 = component6.GetUptimeOverCycles(num_cycles);
    }
    else
    {
      num2 = -1f;
      num3 = -1f;
      num4 = -1f;
    }
    if ((double) num2 >= 0.0)
      this.drawer.NewLabel(((string) UI.ELEMENTAL.UPTIME.NAME).Replace("{0}", "    • ").Replace("{1}", (string) UI.ELEMENTAL.UPTIME.THIS_CYCLE).Replace("{2}", GameUtil.GetFormattedPercent(num2 * 100f)).Replace("{3}", (string) UI.ELEMENTAL.UPTIME.LAST_CYCLE).Replace("{4}", GameUtil.GetFormattedPercent(num3 * 100f)).Replace("{5}", UI.ELEMENTAL.UPTIME.LAST_X_CYCLES.Replace("{0}", num_cycles.ToString())).Replace("{6}", GameUtil.GetFormattedPercent(num4 * 100f)));
    if (!flag1)
    {
      bool flag2 = false;
      float thermalConductivity = element.thermalConductivity;
      Building component8 = this.selectedTarget.GetComponent<Building>();
      if (Object.op_Inequality((Object) component8, (Object) null))
      {
        thermalConductivity *= component8.Def.ThermalConductivity;
        flag2 = (double) component8.Def.ThermalConductivity < 1.0;
      }
      string temperatureUnitSuffix = GameUtil.GetTemperatureUnitSuffix();
      float shc = specificHeatCapacity * 1f;
      string text1 = string.Format((string) UI.ELEMENTAL.SHC.NAME, (object) GameUtil.GetDisplaySHC(shc).ToString("0.000"));
      string tooltip_text1 = ((string) UI.ELEMENTAL.SHC.TOOLTIP).Replace("{SPECIFIC_HEAT_CAPACITY}", text1 + GameUtil.GetSHCSuffix()).Replace("{TEMPERATURE_UNIT}", temperatureUnitSuffix);
      string text2 = string.Format((string) UI.ELEMENTAL.THERMALCONDUCTIVITY.NAME, (object) GameUtil.GetDisplayThermalConductivity(thermalConductivity).ToString("0.000"));
      string tooltip_text2 = ((string) UI.ELEMENTAL.THERMALCONDUCTIVITY.TOOLTIP).Replace("{THERMAL_CONDUCTIVITY}", text2 + GameUtil.GetThermalConductivitySuffix()).Replace("{TEMPERATURE_UNIT}", temperatureUnitSuffix);
      this.drawer.NewLabel(this.drawer.Format((string) UI.ELEMENTAL.TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(temperature))).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.TEMPERATURE.TOOLTIP, GameUtil.GetFormattedTemperature(temperature))).NewLabel(this.drawer.Format((string) UI.ELEMENTAL.DISEASE.NAME, GameUtil.GetFormattedDisease(diseaseIdx, diseaseCount))).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.DISEASE.TOOLTIP, GameUtil.GetFormattedDisease(diseaseIdx, diseaseCount, true))).NewLabel(text1).Tooltip(tooltip_text1).NewLabel(text2).Tooltip(tooltip_text2);
      if (flag2)
        this.drawer.NewLabel((string) UI.GAMEOBJECTEFFECTS.INSULATED.NAME).Tooltip((string) UI.GAMEOBJECTEFFECTS.INSULATED.TOOLTIP);
    }
    if (element.IsSolid)
    {
      this.drawer.NewLabel(this.drawer.Format((string) UI.ELEMENTAL.MELTINGPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp))).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.MELTINGPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp)));
      if (Object.op_Inequality((Object) this.selectedTarget.GetComponent<ElementChunk>(), (Object) null))
      {
        AttributeModifier attributeModifier = component1.Element.attributeModifiers.Find((Predicate<AttributeModifier>) (m => m.AttributeId == Db.Get().BuildingAttributes.OverheatTemperature.Id));
        if (attributeModifier != null)
          this.drawer.NewLabel(this.drawer.Format((string) UI.ELEMENTAL.OVERHEATPOINT.NAME, attributeModifier.GetFormattedString())).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.OVERHEATPOINT.TOOLTIP, attributeModifier.GetFormattedString()));
      }
    }
    else if (element.IsLiquid)
      this.drawer.NewLabel(this.drawer.Format((string) UI.ELEMENTAL.FREEZEPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp))).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.FREEZEPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp))).NewLabel(this.drawer.Format((string) UI.ELEMENTAL.VAPOURIZATIONPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp))).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.VAPOURIZATIONPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp)));
    else if (!flag1)
      this.drawer.NewLabel(this.drawer.Format((string) UI.ELEMENTAL.DEWPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp))).Tooltip(this.drawer.Format((string) UI.ELEMENTAL.DEWPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp)));
    if (DlcManager.FeatureRadiationEnabled())
    {
      string formattedPercent = GameUtil.GetFormattedPercent(GameUtil.GetRadiationAbsorptionPercentage(Grid.PosToCell(this.selectedTarget)) * 100f);
      this.drawer.NewLabel(this.drawer.Format((string) UI.DETAILTABS.DETAILS.RADIATIONABSORPTIONFACTOR.NAME, formattedPercent)).Tooltip(this.drawer.Format((string) UI.DETAILTABS.DETAILS.RADIATIONABSORPTIONFACTOR.TOOLTIP, formattedPercent));
    }
    Attributes attributes = this.selectedTarget.GetAttributes();
    if (attributes != null)
    {
      for (int index = 0; index < attributes.Count; ++index)
      {
        AttributeInstance attributeInstance = attributes.AttributeTable[index];
        if (DlcManager.IsDlcListValidForCurrentContent(attributeInstance.Attribute.DLCIds) && (attributeInstance.Attribute.ShowInUI == Klei.AI.Attribute.Display.Details || attributeInstance.Attribute.ShowInUI == Klei.AI.Attribute.Display.Expectation))
          this.drawer.NewLabel(attributeInstance.modifier.Name + ": " + attributeInstance.GetFormattedValue()).Tooltip(attributeInstance.GetAttributeValueTooltip());
      }
    }
    List<Descriptor> detailDescriptors = GameUtil.GetDetailDescriptors(GameUtil.GetAllDescriptors(this.selectedTarget));
    for (int index = 0; index < detailDescriptors.Count; ++index)
    {
      Descriptor descriptor = detailDescriptors[index];
      this.drawer.NewLabel(descriptor.text).Tooltip(descriptor.tooltipText);
    }
  }
}
