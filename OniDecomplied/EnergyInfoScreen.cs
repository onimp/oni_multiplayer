// Decompiled with JetBrains decompiler
// Type: EnergyInfoScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

public class EnergyInfoScreen : TargetScreen
{
  public GameObject labelTemplate;
  private GameObject overviewPanel;
  private GameObject generatorsPanel;
  private GameObject consumersPanel;
  private GameObject batteriesPanel;
  private Dictionary<string, GameObject> overviewLabels = new Dictionary<string, GameObject>();
  private Dictionary<string, GameObject> generatorsLabels = new Dictionary<string, GameObject>();
  private Dictionary<string, GameObject> consumersLabels = new Dictionary<string, GameObject>();
  private Dictionary<string, GameObject> batteriesLabels = new Dictionary<string, GameObject>();

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<ICircuitConnected>() != null || Object.op_Inequality((Object) target.GetComponent<Wire>(), (Object) null);

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.overviewPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((TMP_Text) this.overviewPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.ENERGYGENERATOR.CIRCUITOVERVIEW;
    this.generatorsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((TMP_Text) this.generatorsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.ENERGYGENERATOR.GENERATORS;
    this.consumersPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((TMP_Text) this.consumersPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.ENERGYGENERATOR.CONSUMERS;
    this.batteriesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((TMP_Text) this.batteriesPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.ENERGYGENERATOR.BATTERIES;
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
      label = Util.KInstantiate(this.labelTemplate, ((Component) panel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject, (string) null);
      label.transform.localScale = new Vector3(1f, 1f, 1f);
      labels[id] = label;
      label.SetActive(true);
    }
    return label;
  }

  private void LateUpdate() => this.Refresh();

  private void Refresh()
  {
    if (Object.op_Equality((Object) this.selectedTarget, (Object) null))
      return;
    foreach (KeyValuePair<string, GameObject> overviewLabel in this.overviewLabels)
      overviewLabel.Value.SetActive(false);
    foreach (KeyValuePair<string, GameObject> generatorsLabel in this.generatorsLabels)
      generatorsLabel.Value.SetActive(false);
    foreach (KeyValuePair<string, GameObject> consumersLabel in this.consumersLabels)
      consumersLabel.Value.SetActive(false);
    foreach (KeyValuePair<string, GameObject> batteriesLabel in this.batteriesLabels)
      batteriesLabel.Value.SetActive(false);
    CircuitManager circuitManager = Game.Instance.circuitManager;
    ushort circuitID = ushort.MaxValue;
    ICircuitConnected component = this.selectedTarget.GetComponent<ICircuitConnected>();
    if (component != null)
      circuitID = circuitManager.GetCircuitID(component);
    else if (Object.op_Inequality((Object) this.selectedTarget.GetComponent<Wire>(), (Object) null))
      circuitID = Game.Instance.circuitManager.GetCircuitID(Grid.PosToCell(TransformExtensions.GetPosition(this.selectedTarget.transform)));
    if (circuitID != ushort.MaxValue)
    {
      this.overviewPanel.SetActive(true);
      this.generatorsPanel.SetActive(true);
      this.consumersPanel.SetActive(true);
      this.batteriesPanel.SetActive(true);
      float availableOnCircuit = circuitManager.GetJoulesAvailableOnCircuit(circuitID);
      GameObject label1 = this.AddOrGetLabel(this.overviewLabels, this.overviewPanel, "joulesAvailable");
      ((TMP_Text) label1.GetComponent<LocText>()).text = string.Format((string) UI.DETAILTABS.ENERGYGENERATOR.AVAILABLE_JOULES, (object) GameUtil.GetFormattedJoules(availableOnCircuit));
      label1.GetComponent<ToolTip>().toolTip = (string) UI.DETAILTABS.ENERGYGENERATOR.AVAILABLE_JOULES_TOOLTIP;
      label1.SetActive(true);
      float generatedByCircuit1 = circuitManager.GetWattsGeneratedByCircuit(circuitID);
      float generatedByCircuit2 = circuitManager.GetPotentialWattsGeneratedByCircuit(circuitID);
      GameObject label2 = this.AddOrGetLabel(this.overviewLabels, this.overviewPanel, "wattageGenerated");
      string str = (double) generatedByCircuit1 != (double) generatedByCircuit2 ? string.Format("{0} / {1}", (object) GameUtil.GetFormattedWattage(generatedByCircuit1), (object) GameUtil.GetFormattedWattage(generatedByCircuit2)) : GameUtil.GetFormattedWattage(generatedByCircuit1);
      ((TMP_Text) label2.GetComponent<LocText>()).text = string.Format((string) UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_GENERATED, (object) str);
      label2.GetComponent<ToolTip>().toolTip = (string) UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_GENERATED_TOOLTIP;
      label2.SetActive(true);
      GameObject label3 = this.AddOrGetLabel(this.overviewLabels, this.overviewPanel, "wattageConsumed");
      ((TMP_Text) label3.GetComponent<LocText>()).text = string.Format((string) UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_CONSUMED, (object) GameUtil.GetFormattedWattage(circuitManager.GetWattsUsedByCircuit(circuitID)));
      label3.GetComponent<ToolTip>().toolTip = (string) UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_CONSUMED_TOOLTIP;
      label3.SetActive(true);
      GameObject label4 = this.AddOrGetLabel(this.overviewLabels, this.overviewPanel, "potentialWattageConsumed");
      ((TMP_Text) label4.GetComponent<LocText>()).text = string.Format((string) UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, (object) GameUtil.GetFormattedWattage(circuitManager.GetWattsNeededWhenActive(circuitID)));
      label4.GetComponent<ToolTip>().toolTip = (string) UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED_TOOLTIP;
      label4.SetActive(true);
      GameObject label5 = this.AddOrGetLabel(this.overviewLabels, this.overviewPanel, "maxSafeWattage");
      ((TMP_Text) label5.GetComponent<LocText>()).text = string.Format((string) UI.DETAILTABS.ENERGYGENERATOR.MAX_SAFE_WATTAGE, (object) GameUtil.GetFormattedWattage(circuitManager.GetMaxSafeWattageForCircuit(circuitID)));
      label5.GetComponent<ToolTip>().toolTip = (string) UI.DETAILTABS.ENERGYGENERATOR.MAX_SAFE_WATTAGE_TOOLTIP;
      label5.SetActive(true);
      ReadOnlyCollection<Generator> generatorsOnCircuit = circuitManager.GetGeneratorsOnCircuit(circuitID);
      ReadOnlyCollection<IEnergyConsumer> consumersOnCircuit = circuitManager.GetConsumersOnCircuit(circuitID);
      List<Battery> batteriesOnCircuit = circuitManager.GetBatteriesOnCircuit(circuitID);
      ReadOnlyCollection<Battery> transformersOnCircuit = circuitManager.GetTransformersOnCircuit(circuitID);
      if (generatorsOnCircuit.Count > 0)
      {
        foreach (Generator generator in generatorsOnCircuit)
        {
          if (Object.op_Inequality((Object) generator, (Object) null) && Object.op_Equality((Object) ((Component) generator).GetComponent<Battery>(), (Object) null))
          {
            label5 = this.AddOrGetLabel(this.generatorsLabels, this.generatorsPanel, ((Object) ((Component) generator).gameObject).GetInstanceID().ToString());
            if (generator.IsProducingPower())
              ((TMP_Text) label5.GetComponent<LocText>()).text = string.Format("{0}: {1}", (object) ((Component) generator).GetComponent<KSelectable>().entityName, (object) GameUtil.GetFormattedWattage(generator.WattageRating));
            else
              ((TMP_Text) label5.GetComponent<LocText>()).text = string.Format("{0}: {1} / {2}", (object) ((Component) generator).GetComponent<KSelectable>().entityName, (object) GameUtil.GetFormattedWattage(0.0f), (object) GameUtil.GetFormattedWattage(generator.WattageRating));
            label5.SetActive(true);
            ((TMP_Text) label5.GetComponent<LocText>()).fontStyle = Object.op_Equality((Object) ((Component) generator).gameObject, (Object) this.selectedTarget) ? (FontStyles) 1 : (FontStyles) 0;
          }
        }
      }
      else
      {
        label5 = this.AddOrGetLabel(this.generatorsLabels, this.generatorsPanel, "nogenerators");
        ((TMP_Text) label5.GetComponent<LocText>()).text = (string) UI.DETAILTABS.ENERGYGENERATOR.NOGENERATORS;
        label5.SetActive(true);
      }
      if (consumersOnCircuit.Count > 0 || transformersOnCircuit.Count > 0)
      {
        foreach (IEnergyConsumer consumer in consumersOnCircuit)
          this.AddConsumerInfo(consumer, label5);
        foreach (IEnergyConsumer consumer in transformersOnCircuit)
          this.AddConsumerInfo(consumer, label5);
      }
      else
      {
        GameObject label6 = this.AddOrGetLabel(this.consumersLabels, this.consumersPanel, "noconsumers");
        ((TMP_Text) label6.GetComponent<LocText>()).text = (string) UI.DETAILTABS.ENERGYGENERATOR.NOCONSUMERS;
        label6.SetActive(true);
      }
      if (batteriesOnCircuit.Count > 0)
      {
        foreach (Battery battery in batteriesOnCircuit)
        {
          if (Object.op_Inequality((Object) battery, (Object) null))
          {
            GameObject label7 = this.AddOrGetLabel(this.batteriesLabels, this.batteriesPanel, ((Object) ((Component) battery).gameObject).GetInstanceID().ToString());
            ((TMP_Text) label7.GetComponent<LocText>()).text = string.Format("{0}: {1}", (object) ((Component) battery).GetComponent<KSelectable>().entityName, (object) GameUtil.GetFormattedJoules(battery.JoulesAvailable));
            label7.SetActive(true);
            ((TMP_Text) label7.GetComponent<LocText>()).fontStyle = Object.op_Equality((Object) ((Component) battery).gameObject, (Object) this.selectedTarget) ? (FontStyles) 1 : (FontStyles) 0;
          }
        }
      }
      else
      {
        GameObject label8 = this.AddOrGetLabel(this.batteriesLabels, this.batteriesPanel, "nobatteries");
        ((TMP_Text) label8.GetComponent<LocText>()).text = (string) UI.DETAILTABS.ENERGYGENERATOR.NOBATTERIES;
        label8.SetActive(true);
      }
    }
    else
    {
      this.overviewPanel.SetActive(true);
      this.generatorsPanel.SetActive(false);
      this.consumersPanel.SetActive(false);
      this.batteriesPanel.SetActive(false);
      GameObject label = this.AddOrGetLabel(this.overviewLabels, this.overviewPanel, "nocircuit");
      ((TMP_Text) label.GetComponent<LocText>()).text = (string) UI.DETAILTABS.ENERGYGENERATOR.DISCONNECTED;
      label.SetActive(true);
    }
  }

  private void AddConsumerInfo(IEnergyConsumer consumer, GameObject label)
  {
    KMonoBehaviour kmonoBehaviour = consumer as KMonoBehaviour;
    if (!Object.op_Inequality((Object) kmonoBehaviour, (Object) null))
      return;
    label = this.AddOrGetLabel(this.consumersLabels, this.consumersPanel, ((Object) ((Component) kmonoBehaviour).gameObject).GetInstanceID().ToString());
    float wattsUsed = consumer.WattsUsed;
    float neededWhenActive = consumer.WattsNeededWhenActive;
    string str = (double) wattsUsed != (double) neededWhenActive ? string.Format("{0} / {1}", (object) GameUtil.GetFormattedWattage(wattsUsed), (object) GameUtil.GetFormattedWattage(neededWhenActive)) : GameUtil.GetFormattedWattage(wattsUsed);
    ((TMP_Text) label.GetComponent<LocText>()).text = string.Format("{0}: {1}", (object) consumer.Name, (object) str);
    label.SetActive(true);
    ((TMP_Text) label.GetComponent<LocText>()).fontStyle = Object.op_Equality((Object) ((Component) kmonoBehaviour).gameObject, (Object) this.selectedTarget) ? (FontStyles) 1 : (FontStyles) 0;
  }
}
