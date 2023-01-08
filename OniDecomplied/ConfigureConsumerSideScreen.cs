// Decompiled with JetBrains decompiler
// Type: ConfigureConsumerSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigureConsumerSideScreen : SideScreenContent
{
  [SerializeField]
  private RectTransform consumptionSettingToggleContainer;
  [SerializeField]
  private GameObject consumptionSettingTogglePrefab;
  [SerializeField]
  private RectTransform settingRequirementRowsContainer;
  [SerializeField]
  private RectTransform settingEffectRowsContainer;
  [SerializeField]
  private LocText selectedOptionNameLabel;
  [SerializeField]
  private GameObject settingDescriptorPrefab;
  private IConfigurableConsumer targetProducer;
  private IConfigurableConsumerOption[] settings;
  private LocText descriptor;
  private List<HierarchyReferences> settingToggles = new List<HierarchyReferences>();
  private List<GameObject> requirementRows = new List<GameObject>();

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<IConfigurableConsumer>() != null;

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.targetProducer = target.GetComponent<IConfigurableConsumer>();
    if (this.settings == null)
      this.settings = this.targetProducer.GetSettingOptions();
    this.PopulateOptions();
  }

  private void ClearOldOptions()
  {
    if (Object.op_Inequality((Object) this.descriptor, (Object) null))
      ((Component) this.descriptor).gameObject.SetActive(false);
    for (int index = 0; index < this.settingToggles.Count; ++index)
      ((Component) this.settingToggles[index]).gameObject.SetActive(false);
  }

  private void PopulateOptions()
  {
    this.ClearOldOptions();
    for (int count = this.settingToggles.Count; count < this.settings.Length; ++count)
    {
      IConfigurableConsumerOption setting = this.settings[count];
      HierarchyReferences component = Util.KInstantiateUI(this.consumptionSettingTogglePrefab, ((Component) this.consumptionSettingToggleContainer).gameObject, true).GetComponent<HierarchyReferences>();
      this.settingToggles.Add(component);
      ((TMP_Text) component.GetReference<LocText>("Label")).text = setting.GetName();
      component.GetReference<Image>("Image").sprite = setting.GetIcon();
      component.GetReference<MultiToggle>("Toggle").onClick += (System.Action) (() => this.SelectOption(setting));
    }
    this.RefreshToggles();
    this.RefreshDetails();
  }

  private void SelectOption(IConfigurableConsumerOption option)
  {
    this.targetProducer.SetSelectedOption(option);
    this.RefreshToggles();
    this.RefreshDetails();
  }

  private void RefreshToggles()
  {
    for (int index = 0; index < this.settingToggles.Count; ++index)
    {
      MultiToggle reference = this.settingToggles[index].GetReference<MultiToggle>("Toggle");
      reference.ChangeState(this.settings[index] == this.targetProducer.GetSelectedOption() ? 1 : 0);
      ((Component) reference).gameObject.SetActive(true);
    }
  }

  private void RefreshDetails()
  {
    if (Object.op_Equality((Object) this.descriptor, (Object) null))
      this.descriptor = Util.KInstantiateUI(this.settingDescriptorPrefab, ((Component) this.settingEffectRowsContainer).gameObject, true).GetComponent<LocText>();
    IConfigurableConsumerOption selectedOption = this.targetProducer.GetSelectedOption();
    if (selectedOption == null)
      return;
    ((TMP_Text) this.descriptor).text = selectedOption.GetDetailedDescription();
    ((TMP_Text) this.selectedOptionNameLabel).text = "<b>" + selectedOption.GetName() + "</b>";
    ((Component) this.descriptor).gameObject.SetActive(true);
  }

  public override int GetSideScreenSortOrder() => 1;
}
