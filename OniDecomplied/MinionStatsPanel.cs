// Decompiled with JetBrains decompiler
// Type: MinionStatsPanel
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

public class MinionStatsPanel : TargetScreen
{
  public GameObject attributesLabelTemplate;
  private GameObject resumePanel;
  private GameObject attributesPanel;
  private DetailsPanelDrawer resumeDrawer;
  private DetailsPanelDrawer attributesDrawer;
  private SchedulerHandle updateHandle;

  public override bool IsValidForTarget(GameObject target) => Object.op_Implicit((Object) target.GetComponent<MinionIdentity>());

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.resumePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.attributesPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.resumeDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, ((Component) this.resumePanel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject);
    this.attributesDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, ((Component) this.attributesPanel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject);
  }

  protected virtual void OnCleanUp()
  {
    this.updateHandle.ClearScheduler();
    base.OnCleanUp();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Refresh();
    this.ScheduleUpdate();
  }

  public override void OnSelectTarget(GameObject target)
  {
    base.OnSelectTarget(target);
    this.Refresh();
  }

  private void ScheduleUpdate() => this.updateHandle = UIScheduler.Instance.Schedule("RefreshMinionStatsPanel", 1f, (Action<object>) (o =>
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
    this.RefreshResume();
    this.RefreshAttributes();
  }

  private void RefreshAttributes()
  {
    if (!Object.op_Implicit((Object) this.selectedTarget.GetComponent<MinionIdentity>()))
    {
      this.attributesPanel.SetActive(false);
    }
    else
    {
      this.attributesPanel.SetActive(true);
      ((TMP_Text) this.attributesPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) UI.DETAILTABS.STATS.GROUPNAME_ATTRIBUTES;
      List<AttributeInstance> all = new List<AttributeInstance>((IEnumerable<AttributeInstance>) this.selectedTarget.GetAttributes().AttributeTable).FindAll((Predicate<AttributeInstance>) (a => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.Skill));
      this.attributesDrawer.BeginDrawing();
      if (all.Count > 0)
      {
        foreach (AttributeInstance attributeInstance in all)
          this.attributesDrawer.NewLabel(string.Format("{0}: {1}", (object) attributeInstance.Name, (object) attributeInstance.GetFormattedValue())).Tooltip(attributeInstance.GetAttributeValueTooltip());
      }
      this.attributesDrawer.EndDrawing();
    }
  }

  private void RefreshResume()
  {
    MinionResume component = this.selectedTarget.GetComponent<MinionResume>();
    if (!Object.op_Implicit((Object) component))
    {
      this.resumePanel.SetActive(false);
    }
    else
    {
      this.resumePanel.SetActive(true);
      ((TMP_Text) this.resumePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = string.Format((string) UI.DETAILTABS.PERSONALITY.GROUPNAME_RESUME, (object) ((Object) this.selectedTarget).name.ToUpper());
      this.resumeDrawer.BeginDrawing();
      List<Skill> skillList = new List<Skill>();
      foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
      {
        if (keyValuePair.Value)
        {
          Skill skill = Db.Get().Skills.Get(keyValuePair.Key);
          skillList.Add(skill);
        }
      }
      this.resumeDrawer.NewLabel((string) UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_SKILLS).Tooltip((string) UI.DETAILTABS.PERSONALITY.RESUME.MASTERED_SKILLS_TOOLTIP);
      if (skillList.Count == 0)
      {
        this.resumeDrawer.NewLabel("  • " + (string) UI.DETAILTABS.PERSONALITY.RESUME.NO_MASTERED_SKILLS.NAME).Tooltip(string.Format((string) UI.DETAILTABS.PERSONALITY.RESUME.NO_MASTERED_SKILLS.TOOLTIP, (object) ((Object) this.selectedTarget).name));
      }
      else
      {
        foreach (Skill skill in skillList)
        {
          string str = "";
          foreach (SkillPerk perk in skill.perks)
            str = str + "  • " + perk.Name + "\n";
          this.resumeDrawer.NewLabel("  • " + skill.Name).Tooltip(skill.description + "\n" + str);
        }
      }
      this.resumeDrawer.EndDrawing();
    }
  }
}
