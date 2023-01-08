// Decompiled with JetBrains decompiler
// Type: MinionPersonalityPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinionPersonalityPanel : TargetScreen
{
  public GameObject attributesLabelTemplate;
  private GameObject bioPanel;
  private GameObject traitsPanel;
  private DetailsPanelDrawer bioDrawer;
  private DetailsPanelDrawer traitsDrawer;
  public MinionEquipmentPanel panel;
  private SchedulerHandle updateHandle;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<MinionIdentity>(), (Object) null);

  public virtual void ScreenUpdate(bool topLevel) => base.ScreenUpdate(topLevel);

  public override void OnSelectTarget(GameObject target)
  {
    this.panel.SetSelectedMinion(target);
    this.panel.Refresh();
    base.OnSelectTarget(target);
    this.Refresh();
  }

  public override void OnDeselectTarget(GameObject target)
  {
  }

  protected virtual void OnActivate()
  {
    base.OnActivate();
    if (!Object.op_Equality((Object) this.panel, (Object) null))
      return;
    this.panel = ((Component) this).GetComponent<MinionEquipmentPanel>();
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.bioPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.traitsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.bioDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, ((Component) this.bioPanel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject);
    this.traitsDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, ((Component) this.traitsPanel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject);
  }

  protected virtual void OnCleanUp()
  {
    this.updateHandle.ClearScheduler();
    base.OnCleanUp();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Equality((Object) this.panel, (Object) null))
      this.panel = ((Component) this).GetComponent<MinionEquipmentPanel>();
    this.Refresh();
    this.ScheduleUpdate();
  }

  private void ScheduleUpdate() => this.updateHandle = UIScheduler.Instance.Schedule("RefreshMinionPersonalityPanel", 1f, (Action<object>) (o =>
  {
    this.Refresh();
    this.ScheduleUpdate();
  }), (object) null, (SchedulerGroup) null);

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

  private void Refresh()
  {
    if (!((Component) this).gameObject.activeSelf || Object.op_Equality((Object) this.selectedTarget, (Object) null) || Object.op_Equality((Object) this.selectedTarget.GetComponent<MinionIdentity>(), (Object) null))
      return;
    this.RefreshBio();
    this.RefreshTraits();
  }

  private void RefreshBio()
  {
    MinionIdentity component1 = this.selectedTarget.GetComponent<MinionIdentity>();
    if (!Object.op_Implicit((Object) component1))
    {
      this.bioPanel.SetActive(false);
    }
    else
    {
      this.bioPanel.SetActive(true);
      ((TMP_Text) this.bioPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.PERSONALITY.GROUPNAME_BIO;
      this.bioDrawer.BeginDrawing().NewLabel((string) DUPLICANTS.NAMETITLE + ((Object) component1).name).NewLabel((string) DUPLICANTS.ARRIVALTIME + GameUtil.GetFormattedCycles((float) (((double) GameClock.Instance.GetCycle() - (double) component1.arrivalTime) * 600.0), "F0", true)).Tooltip(string.Format((string) DUPLICANTS.ARRIVALTIME_TOOLTIP, (object) (float) ((double) component1.arrivalTime + 1.0), (object) ((Object) component1).name)).NewLabel((string) DUPLICANTS.GENDERTITLE + string.Format(StringEntry.op_Implicit(Strings.Get(string.Format("STRINGS.DUPLICANTS.GENDER.{0}.NAME", (object) component1.genderStringKey.ToUpper()))), (object) component1.gender)).NewLabel(string.Format(StringEntry.op_Implicit(Strings.Get(string.Format("STRINGS.DUPLICANTS.PERSONALITIES.{0}.DESC", (object) component1.nameStringKey.ToUpper()))), (object) ((Object) component1).name)).Tooltip(string.Format(StringEntry.op_Implicit(Strings.Get(string.Format("STRINGS.DUPLICANTS.DESC_TOOLTIP", (object) component1.nameStringKey.ToUpper()))), (object) ((Object) component1).name));
      MinionResume component2 = this.selectedTarget.GetComponent<MinionResume>();
      if (Object.op_Inequality((Object) component2, (Object) null) && component2.AptitudeBySkillGroup.Count > 0)
      {
        this.bioDrawer.NewLabel((string) UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.NAME + "\n").Tooltip(string.Format((string) UI.DETAILTABS.PERSONALITY.RESUME.APTITUDES.TOOLTIP, (object) ((Object) this.selectedTarget).name));
        foreach (KeyValuePair<HashedString, float> keyValuePair in component2.AptitudeBySkillGroup)
        {
          if ((double) keyValuePair.Value != 0.0)
          {
            SkillGroup skillGroup = Db.Get().SkillGroups.TryGet(keyValuePair.Key);
            if (skillGroup != null)
              this.bioDrawer.NewLabel("  • " + skillGroup.Name).Tooltip(string.Format((string) DUPLICANTS.ROLES.GROUPS.APTITUDE_DESCRIPTION, (object) skillGroup.Name, (object) keyValuePair.Value));
          }
        }
      }
      this.bioDrawer.EndDrawing();
    }
  }

  private void RefreshTraits()
  {
    if (!Object.op_Implicit((Object) this.selectedTarget.GetComponent<MinionIdentity>()))
    {
      this.traitsPanel.SetActive(false);
    }
    else
    {
      this.traitsPanel.SetActive(true);
      ((TMP_Text) this.traitsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.STATS.GROUPNAME_TRAITS;
      this.traitsDrawer.BeginDrawing();
      foreach (Trait trait in this.selectedTarget.GetComponent<Traits>().TraitList)
      {
        if (!string.IsNullOrEmpty(trait.Name))
          this.traitsDrawer.NewLabel(trait.Name).Tooltip(trait.GetTooltip());
      }
      this.traitsDrawer.EndDrawing();
    }
  }
}
