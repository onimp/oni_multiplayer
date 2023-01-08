// Decompiled with JetBrains decompiler
// Type: TelepadSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TelepadSideScreen : SideScreenContent
{
  [SerializeField]
  private LocText timeLabel;
  [SerializeField]
  private KButton viewImmigrantsBtn;
  [SerializeField]
  private Telepad targetTelepad;
  [SerializeField]
  private KButton viewColonySummaryBtn;
  [SerializeField]
  private Image newAchievementsEarned;
  [SerializeField]
  private KButton openRolesScreenButton;
  [SerializeField]
  private Image skillPointsAvailable;
  [SerializeField]
  private GameObject victoryConditionsContainer;
  [SerializeField]
  private GameObject conditionContainerTemplate;
  [SerializeField]
  private GameObject checkboxLinePrefab;
  private Dictionary<string, Dictionary<ColonyAchievementRequirement, GameObject>> entries = new Dictionary<string, Dictionary<ColonyAchievementRequirement, GameObject>>();
  private Dictionary<ColonyAchievement, Dictionary<ColonyAchievementRequirement, GameObject>> victoryAchievementWidgets = new Dictionary<ColonyAchievement, Dictionary<ColonyAchievementRequirement, GameObject>>();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.viewImmigrantsBtn.onClick += (System.Action) (() =>
    {
      ImmigrantScreen.InitializeImmigrantScreen(this.targetTelepad);
      Game.Instance.Trigger(288942073, (object) null);
    });
    this.viewColonySummaryBtn.onClick += (System.Action) (() =>
    {
      ((Component) this.newAchievementsEarned).gameObject.SetActive(false);
      MainMenu.ActivateRetiredColoniesScreenFromData(((Component) ((KMonoBehaviour) PauseScreen.Instance).transform.parent).gameObject, RetireColonyUtility.GetCurrentColonyRetiredColonyData());
    });
    this.openRolesScreenButton.onClick += (System.Action) (() => ManagementMenu.Instance.ToggleSkills());
    this.BuildVictoryConditions();
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<Telepad>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    Telepad component = target.GetComponent<Telepad>();
    if (Object.op_Equality((Object) component, (Object) null))
    {
      Debug.LogError((object) "Target doesn't have a telepad associated with it.");
    }
    else
    {
      this.targetTelepad = component;
      if (Object.op_Inequality((Object) this.targetTelepad, (Object) null))
        ((Component) this).gameObject.SetActive(false);
      else
        ((Component) this).gameObject.SetActive(true);
    }
  }

  private void Update()
  {
    if (!Object.op_Inequality((Object) this.targetTelepad, (Object) null))
      return;
    if (Object.op_Inequality((Object) GameFlowManager.Instance, (Object) null) && GameFlowManager.Instance.IsGameOver())
    {
      ((Component) this).gameObject.SetActive(false);
      ((TMP_Text) this.timeLabel).text = (string) STRINGS.UI.UISIDESCREENS.TELEPADSIDESCREEN.GAMEOVER;
      this.SetContentState(true);
    }
    else
    {
      if (((Component) this.targetTelepad).GetComponent<Operational>().IsOperational)
        ((TMP_Text) this.timeLabel).text = string.Format((string) STRINGS.UI.UISIDESCREENS.TELEPADSIDESCREEN.NEXTPRODUCTION, (object) GameUtil.GetFormattedCycles(this.targetTelepad.GetTimeRemaining()));
      else
        ((Component) this).gameObject.SetActive(false);
      this.SetContentState(!Immigration.Instance.ImmigrantsAvailable);
    }
    this.UpdateVictoryConditions();
    this.UpdateAchievementsUnlocked();
    this.UpdateSkills();
  }

  private void SetContentState(bool isLabel)
  {
    if (((Component) this.timeLabel).gameObject.activeInHierarchy != isLabel)
      ((Component) this.timeLabel).gameObject.SetActive(isLabel);
    if (((Component) this.viewImmigrantsBtn).gameObject.activeInHierarchy != isLabel)
      return;
    ((Component) this.viewImmigrantsBtn).gameObject.SetActive(!isLabel);
  }

  private void BuildVictoryConditions()
  {
    foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
    {
      if (resource.isVictoryCondition && !resource.Disabled)
      {
        Dictionary<ColonyAchievementRequirement, GameObject> dictionary = new Dictionary<ColonyAchievementRequirement, GameObject>();
        this.victoryAchievementWidgets.Add(resource, dictionary);
        GameObject gameObject1 = Util.KInstantiateUI(this.conditionContainerTemplate, this.victoryConditionsContainer, true);
        ((TMP_Text) gameObject1.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")).SetText(resource.Name);
        foreach (ColonyAchievementRequirement key in resource.requirementChecklist)
        {
          if (key is VictoryColonyAchievementRequirement achievementRequirement)
          {
            GameObject gameObject2 = Util.KInstantiateUI(this.checkboxLinePrefab, gameObject1, true);
            ((TMP_Text) gameObject2.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")).SetText(achievementRequirement.Name());
            gameObject2.GetComponent<ToolTip>().SetSimpleTooltip(achievementRequirement.Description());
            dictionary.Add(key, gameObject2);
          }
          else
            Debug.LogWarning((object) string.Format("Colony achievement {0} is not a victory requirement but it is attached to a victory achievement {1}.", (object) key.GetType().ToString(), (object) resource.Name));
        }
        this.entries.Add(resource.Id, dictionary);
      }
    }
  }

  private void UpdateVictoryConditions()
  {
    foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
    {
      if (resource.isVictoryCondition && !resource.Disabled)
      {
        foreach (ColonyAchievementRequirement key in resource.requirementChecklist)
          ((Behaviour) this.entries[resource.Id][key].GetComponent<HierarchyReferences>().GetReference<Image>("Check")).enabled = key.Success();
      }
    }
    foreach (KeyValuePair<ColonyAchievement, Dictionary<ColonyAchievementRequirement, GameObject>> achievementWidget in this.victoryAchievementWidgets)
    {
      foreach (KeyValuePair<ColonyAchievementRequirement, GameObject> keyValuePair in achievementWidget.Value)
        keyValuePair.Value.GetComponent<ToolTip>().SetSimpleTooltip(keyValuePair.Key.GetProgress(keyValuePair.Key.Success()));
    }
  }

  private void UpdateAchievementsUnlocked()
  {
    if (((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().achievementsToDisplay.Count <= 0)
      return;
    ((Component) this.newAchievementsEarned).gameObject.SetActive(true);
  }

  private void UpdateSkills()
  {
    bool flag = false;
    foreach (MinionResume minionResume in Components.MinionResumes)
    {
      if (!((Component) minionResume).HasTag(GameTags.Dead) && minionResume.TotalSkillPointsGained - minionResume.SkillsMastered > 0)
      {
        flag = true;
        break;
      }
    }
    ((Component) this.skillPointsAvailable).gameObject.SetActive(flag);
  }
}
