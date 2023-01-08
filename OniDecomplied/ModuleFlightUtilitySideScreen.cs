// Decompiled with JetBrains decompiler
// Type: ModuleFlightUtilitySideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModuleFlightUtilitySideScreen : SideScreenContent
{
  private Clustercraft targetCraft;
  public GameObject moduleContentContainer;
  public GameObject modulePanelPrefab;
  public ColorStyleSetting repeatOff;
  public ColorStyleSetting repeatOn;
  private Dictionary<IEmptyableCargo, HierarchyReferences> modulePanels = new Dictionary<IEmptyableCargo, HierarchyReferences>();
  private List<int> refreshHandle = new List<int>();

  private CraftModuleInterface craftModuleInterface => ((Component) this.targetCraft).GetComponent<CraftModuleInterface>();

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    this.ConsumeMouseScroll = true;
  }

  public virtual float GetSortKey() => 21f;

  public override bool IsValidForTarget(GameObject target)
  {
    if (Object.op_Inequality((Object) target.GetComponent<Clustercraft>(), (Object) null) && this.HasFlightUtilityModule(target.GetComponent<CraftModuleInterface>()))
      return true;
    RocketControlStation component = target.GetComponent<RocketControlStation>();
    return Object.op_Inequality((Object) component, (Object) null) && this.HasFlightUtilityModule(((Component) component.GetMyWorld()).GetComponent<Clustercraft>().ModuleInterface);
  }

  private bool HasFlightUtilityModule(CraftModuleInterface craftModuleInterface)
  {
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) craftModuleInterface.ClusterModules)
    {
      if (((Component) clusterModule.Get()).GetSMI<IEmptyableCargo>() != null)
        return true;
    }
    return false;
  }

  public override void SetTarget(GameObject target)
  {
    if (Object.op_Inequality((Object) target, (Object) null))
    {
      foreach (int num in this.refreshHandle)
        KMonoBehaviourExtensions.Unsubscribe(target, num);
      this.refreshHandle.Clear();
    }
    base.SetTarget(target);
    this.targetCraft = target.GetComponent<Clustercraft>();
    if (Object.op_Equality((Object) this.targetCraft, (Object) null) && Object.op_Inequality((Object) target.GetComponent<RocketControlStation>(), (Object) null))
      this.targetCraft = ((Component) target.GetMyWorld()).GetComponent<Clustercraft>();
    this.refreshHandle.Add(KMonoBehaviourExtensions.Subscribe(((Component) this.targetCraft).gameObject, -1298331547, new Action<object>(this.RefreshAll)));
    this.refreshHandle.Add(KMonoBehaviourExtensions.Subscribe(((Component) this.targetCraft).gameObject, 1792516731, new Action<object>(this.RefreshAll)));
    this.BuildModules();
  }

  private void ClearModules()
  {
    foreach (KeyValuePair<IEmptyableCargo, HierarchyReferences> modulePanel in this.modulePanels)
      Util.KDestroyGameObject(((Component) modulePanel.Value).gameObject);
    this.modulePanels.Clear();
  }

  private void BuildModules()
  {
    this.ClearModules();
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.craftModuleInterface.ClusterModules)
    {
      IEmptyableCargo smi = ((Component) clusterModule.Get()).GetSMI<IEmptyableCargo>();
      if (smi != null)
      {
        HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.modulePanelPrefab, this.moduleContentContainer, true);
        this.modulePanels.Add(smi, hierarchyReferences);
        this.RefreshModulePanel(smi);
      }
    }
  }

  private void RefreshAll(object data = null) => this.BuildModules();

  private void RefreshModulePanel(IEmptyableCargo module)
  {
    HierarchyReferences modulePanel = this.modulePanels[module];
    modulePanel.GetReference<Image>("icon").sprite = Def.GetUISprite((object) module.master.gameObject).first;
    KButton reference1 = modulePanel.GetReference<KButton>("button");
    reference1.isInteractable = module.CanEmptyCargo();
    reference1.ClearOnClick();
    reference1.onClick += new System.Action(module.EmptyCargo);
    KButton reference2 = modulePanel.GetReference<KButton>("repeatButton");
    if (module.CanAutoDeploy)
    {
      this.StyleRepeatButton(module);
      reference2.ClearOnClick();
      reference2.onClick += (System.Action) (() => this.OnRepeatClicked(module));
      ((Component) reference2).gameObject.SetActive(true);
    }
    else
      ((Component) reference2).gameObject.SetActive(false);
    DropDown reference3 = modulePanel.GetReference<DropDown>("dropDown");
    reference3.targetDropDownContainer = GameScreenManager.Instance.ssOverlayCanvas;
    reference3.Close();
    CrewPortrait reference4 = modulePanel.GetReference<CrewPortrait>("selectedPortrait");
    WorldContainer component = ((Component) (module as StateMachine.Instance).GetMaster().GetComponent<RocketModuleCluster>().CraftInterface).GetComponent<WorldContainer>();
    if (Object.op_Inequality((Object) component, (Object) null) && module.ChooseDuplicant)
    {
      int id = component.id;
      ((Component) reference3).gameObject.SetActive(true);
      reference3.Initialize((IEnumerable<IListableOption>) Components.LiveMinionIdentities.GetWorldItems(id), new Action<IListableOption, object>(this.OnDuplicantEntryClick), refreshAction: new Action<DropDownEntry, object>(this.DropDownEntryRefreshAction), targetData: ((object) module));
      ((TMP_Text) reference3.selectedLabel).text = Object.op_Inequality((Object) module.ChosenDuplicant, (Object) null) ? this.GetDuplicantRowName(module.ChosenDuplicant) : STRINGS.UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.SELECT_DUPLICANT.ToString();
      ((Component) reference4).gameObject.SetActive(true);
      reference4.SetIdentityObject((IAssignableIdentity) module.ChosenDuplicant, false);
      reference3.openButton.isInteractable = !module.ModuleDeployed;
    }
    else
    {
      ((Component) reference3).gameObject.SetActive(false);
      ((Component) reference4).gameObject.SetActive(false);
    }
    ((TMP_Text) modulePanel.GetReference<LocText>("label")).SetText(module.master.gameObject.GetProperName());
  }

  private string GetDuplicantRowName(MinionIdentity minion)
  {
    MinionResume component = ((Component) minion).GetComponent<MinionResume>();
    return Object.op_Inequality((Object) component, (Object) null) && component.HasPerk(Db.Get().SkillPerks.CanUseRocketControlStation) ? string.Format((string) STRINGS.UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.PILOT_FMT, (object) minion.GetProperName()) : minion.GetProperName();
  }

  private void OnRepeatClicked(IEmptyableCargo module)
  {
    module.AutoDeploy = !module.AutoDeploy;
    this.StyleRepeatButton(module);
  }

  private void OnDuplicantEntryClick(IListableOption option, object data)
  {
    MinionIdentity minionIdentity = (MinionIdentity) option;
    IEmptyableCargo key = (IEmptyableCargo) data;
    key.ChosenDuplicant = minionIdentity;
    HierarchyReferences modulePanel = this.modulePanels[key];
    ((TMP_Text) modulePanel.GetReference<DropDown>("dropDown").selectedLabel).text = Object.op_Inequality((Object) key.ChosenDuplicant, (Object) null) ? this.GetDuplicantRowName(key.ChosenDuplicant) : STRINGS.UI.UISIDESCREENS.MODULEFLIGHTUTILITYSIDESCREEN.SELECT_DUPLICANT.ToString();
    modulePanel.GetReference<CrewPortrait>("selectedPortrait").SetIdentityObject((IAssignableIdentity) key.ChosenDuplicant, false);
    this.RefreshAll();
  }

  private void DropDownEntryRefreshAction(DropDownEntry entry, object targetData)
  {
    MinionIdentity entryData = (MinionIdentity) entry.entryData;
    ((TMP_Text) entry.label).text = this.GetDuplicantRowName(entryData);
    entry.portrait.SetIdentityObject((IAssignableIdentity) entryData, false);
    bool flag = false;
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) this.targetCraft.ModuleInterface.ClusterModules)
    {
      RocketModuleCluster cmp = clusterModule.Get();
      if (!Object.op_Equality((Object) cmp, (Object) null))
      {
        IEmptyableCargo smi = ((Component) cmp).GetSMI<IEmptyableCargo>();
        if (smi != null && !Object.op_Equality((Object) ((IEmptyableCargo) targetData).ChosenDuplicant, (Object) entryData))
          flag = flag || Object.op_Equality((Object) smi.ChosenDuplicant, (Object) entryData);
      }
    }
    entry.button.isInteractable = !flag;
  }

  private void StyleRepeatButton(IEmptyableCargo module)
  {
    KButton reference = this.modulePanels[module].GetReference<KButton>("repeatButton");
    reference.bgImage.colorStyleSetting = module.AutoDeploy ? this.repeatOn : this.repeatOff;
    reference.bgImage.ApplyColorStyleSetting();
  }
}
