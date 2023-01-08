// Decompiled with JetBrains decompiler
// Type: AchievementWidget
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using FMOD.Studio;
using Klei.AI;
using STRINGS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/AchievementWidget")]
public class AchievementWidget : KMonoBehaviour
{
  private Color color_dark_red = new Color(0.282352954f, 0.160784319f, 0.149019614f);
  private Color color_gold = new Color(1f, 0.635294139f, 0.286274523f);
  private Color color_dark_grey = new Color(0.215686277f, 0.215686277f, 0.215686277f);
  private Color color_grey = new Color(0.6901961f, 0.6901961f, 0.6901961f);
  [SerializeField]
  private RectTransform sheenTransform;
  public AnimationCurve flourish_iconScaleCurve;
  public AnimationCurve flourish_sheenPositionCurve;
  public KBatchedAnimController[] sparks;
  [SerializeField]
  private RectTransform progressParent;
  [SerializeField]
  private GameObject requirementPrefab;
  [SerializeField]
  private Sprite statusSuccessIcon;
  [SerializeField]
  private Sprite statusFailureIcon;
  private int numRequirementsDisplayed;
  public bool dlcAchievement;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this).GetComponent<MultiToggle>().onClick += (System.Action) (() => this.ExpandAchievement());
  }

  private void Update()
  {
  }

  private void ExpandAchievement()
  {
    if (!Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
      return;
    ((Component) this.progressParent).gameObject.SetActive(!((Component) this.progressParent).gameObject.activeSelf);
  }

  public void ActivateNewlyAchievedFlourish(float delay = 1f) => ((MonoBehaviour) this).StartCoroutine(this.Flourish(delay));

  private IEnumerator Flourish(float startDelay)
  {
    AchievementWidget achievementWidget = this;
    achievementWidget.SetNeverAchieved();
    Canvas canvas = ((Component) achievementWidget).GetComponent<Canvas>();
    if (Object.op_Equality((Object) canvas, (Object) null))
      canvas = ((Component) achievementWidget).gameObject.AddComponent<Canvas>();
    yield return (object) SequenceUtil.WaitForSecondsRealtime(startDelay);
    KScrollRect component1 = ((Component) achievementWidget.transform.parent.parent).GetComponent<KScrollRect>();
    float num1 = 1.1f;
    double num2 = (double) achievementWidget.transform.localPosition.y * (double) num1;
    Rect rect = ((ScrollRect) component1).content.rect;
    double height = (double) ((Rect) ref rect).height;
    float num3 = (float) (1.0 + num2 / height);
    component1.SetSmoothAutoScrollTarget(num3);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(0.5f);
    canvas.overrideSorting = true;
    canvas.sortingOrder = 30;
    GameObject icon = ((Component) ((Component) ((Component) achievementWidget).GetComponent<HierarchyReferences>().GetReference<Image>("icon")).transform.parent).gameObject;
    foreach (KBatchedAnimController spark in achievementWidget.sparks)
    {
      if (Object.op_Inequality((Object) ((Component) spark).transform.parent, (Object) icon.transform.parent))
      {
        ((Component) spark).GetComponent<KBatchedAnimController>().TintColour = Color32.op_Implicit(new Color(1f, 0.86f, 0.56f, 1f));
        ((Component) spark).transform.SetParent(icon.transform.parent);
        ((Component) spark).transform.SetSiblingIndex(icon.transform.GetSiblingIndex());
        ((Component) spark).GetComponent<KBatchedAnimCanvasRenderer>().compare = (CompareFunction) 8;
      }
    }
    HierarchyReferences component2 = ((Component) achievementWidget).GetComponent<HierarchyReferences>();
    ((Graphic) component2.GetReference<Image>("iconBG")).color = achievementWidget.color_dark_red;
    ((Graphic) component2.GetReference<Image>("iconBorder")).color = achievementWidget.color_gold;
    ((Graphic) component2.GetReference<Image>("icon")).color = achievementWidget.color_gold;
    bool colorChanged = false;
    EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("AchievementUnlocked"), Vector3.zero, 1f);
    int num4 = Mathf.RoundToInt(MathUtil.Clamp(1f, 7f, startDelay - (float) ((double) startDelay % 1.0 / 1.0))) - 1;
    ((EventInstance) ref eventInstance).setParameterByName("num_achievements", (float) num4, false);
    KFMOD.EndOneShot(eventInstance);
    float i;
    for (i = 0.0f; (double) i < 1.2000000476837158; i += Time.unscaledDeltaTime)
    {
      icon.transform.localScale = Vector3.op_Multiply(Vector3.one, achievementWidget.flourish_iconScaleCurve.Evaluate(i));
      achievementWidget.sheenTransform.anchoredPosition = new Vector2(achievementWidget.flourish_sheenPositionCurve.Evaluate(i), achievementWidget.sheenTransform.anchoredPosition.y);
      if ((double) i > 1.0 && !colorChanged)
      {
        colorChanged = true;
        foreach (KAnimControllerBase spark in achievementWidget.sparks)
          spark.Play(HashedString.op_Implicit("spark"));
        achievementWidget.SetAchievedNow();
      }
      yield return (object) SequenceUtil.WaitForNextFrame;
    }
    icon.transform.localScale = Vector3.one;
    canvas.overrideSorting = false;
    for (i = 0.0f; (double) i < 0.60000002384185791; i += Time.unscaledDeltaTime)
      yield return (object) SequenceUtil.WaitForNextFrame;
    achievementWidget.transform.localScale = Vector3.one;
  }

  public void SetAchievedNow()
  {
    ((Component) this).GetComponent<MultiToggle>().ChangeState(1);
    HierarchyReferences component = ((Component) this).GetComponent<HierarchyReferences>();
    ((Graphic) component.GetReference<Image>("iconBG")).color = this.color_dark_red;
    ((Graphic) component.GetReference<Image>("iconBorder")).color = this.color_gold;
    ((Graphic) component.GetReference<Image>("icon")).color = this.color_gold;
    foreach (Graphic componentsInChild in ((Component) this).GetComponentsInChildren<LocText>())
      componentsInChild.color = Color.white;
    this.ConfigureToolTip(((Component) this).GetComponent<ToolTip>(), (string) COLONY_ACHIEVEMENTS.ACHIEVED_THIS_COLONY_TOOLTIP);
  }

  public void SetAchievedBefore()
  {
    ((Component) this).GetComponent<MultiToggle>().ChangeState(1);
    HierarchyReferences component = ((Component) this).GetComponent<HierarchyReferences>();
    ((Graphic) component.GetReference<Image>("iconBG")).color = this.color_dark_red;
    ((Graphic) component.GetReference<Image>("iconBorder")).color = this.color_gold;
    ((Graphic) component.GetReference<Image>("icon")).color = this.color_gold;
    foreach (Graphic componentsInChild in ((Component) this).GetComponentsInChildren<LocText>())
      componentsInChild.color = Color.white;
    this.ConfigureToolTip(((Component) this).GetComponent<ToolTip>(), (string) COLONY_ACHIEVEMENTS.ACHIEVED_OTHER_COLONY_TOOLTIP);
  }

  public void SetNeverAchieved()
  {
    ((Component) this).GetComponent<MultiToggle>().ChangeState(2);
    HierarchyReferences component = ((Component) this).GetComponent<HierarchyReferences>();
    ((Graphic) component.GetReference<Image>("iconBG")).color = this.color_dark_grey;
    ((Graphic) component.GetReference<Image>("iconBorder")).color = this.color_grey;
    ((Graphic) component.GetReference<Image>("icon")).color = this.color_grey;
    foreach (LocText componentsInChild in ((Component) this).GetComponentsInChildren<LocText>())
      ((Graphic) componentsInChild).color = new Color(((Graphic) componentsInChild).color.r, ((Graphic) componentsInChild).color.g, ((Graphic) componentsInChild).color.b, 0.6f);
    this.ConfigureToolTip(((Component) this).GetComponent<ToolTip>(), (string) COLONY_ACHIEVEMENTS.NOT_ACHIEVED_EVER);
  }

  public void SetNotAchieved()
  {
    ((Component) this).GetComponent<MultiToggle>().ChangeState(2);
    HierarchyReferences component = ((Component) this).GetComponent<HierarchyReferences>();
    ((Graphic) component.GetReference<Image>("iconBG")).color = this.color_dark_grey;
    ((Graphic) component.GetReference<Image>("iconBorder")).color = this.color_grey;
    ((Graphic) component.GetReference<Image>("icon")).color = this.color_grey;
    foreach (LocText componentsInChild in ((Component) this).GetComponentsInChildren<LocText>())
      ((Graphic) componentsInChild).color = new Color(((Graphic) componentsInChild).color.r, ((Graphic) componentsInChild).color.g, ((Graphic) componentsInChild).color.b, 0.6f);
    this.ConfigureToolTip(((Component) this).GetComponent<ToolTip>(), (string) COLONY_ACHIEVEMENTS.NOT_ACHIEVED_THIS_COLONY);
  }

  public void SetFailed()
  {
    ((Component) this).GetComponent<MultiToggle>().ChangeState(2);
    HierarchyReferences component = ((Component) this).GetComponent<HierarchyReferences>();
    ((Graphic) component.GetReference<Image>("iconBG")).color = this.color_dark_grey;
    KMonoBehaviourExtensions.SetAlpha(component.GetReference<Image>("iconBG"), 0.5f);
    ((Graphic) component.GetReference<Image>("iconBorder")).color = this.color_grey;
    KMonoBehaviourExtensions.SetAlpha(component.GetReference<Image>("iconBorder"), 0.5f);
    ((Graphic) component.GetReference<Image>("icon")).color = this.color_grey;
    KMonoBehaviourExtensions.SetAlpha(component.GetReference<Image>("icon"), 0.5f);
    foreach (LocText componentsInChild in ((Component) this).GetComponentsInChildren<LocText>())
      ((Graphic) componentsInChild).color = new Color(((Graphic) componentsInChild).color.r, ((Graphic) componentsInChild).color.g, ((Graphic) componentsInChild).color.b, 0.25f);
    this.ConfigureToolTip(((Component) this).GetComponent<ToolTip>(), (string) COLONY_ACHIEVEMENTS.FAILED_THIS_COLONY);
  }

  private void ConfigureToolTip(ToolTip tooltip, string status)
  {
    tooltip.ClearMultiStringTooltip();
    tooltip.AddMultiStringTooltip(status, (TextStyleSetting) null);
    if (Object.op_Inequality((Object) SaveGame.Instance, (Object) null) && !((Component) this.progressParent).gameObject.activeSelf)
      tooltip.AddMultiStringTooltip((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXPAND_TOOLTIP, (TextStyleSetting) null);
    if (!this.dlcAchievement)
      return;
    tooltip.AddMultiStringTooltip((string) COLONY_ACHIEVEMENTS.DLC.EXPANSION1, (TextStyleSetting) null);
  }

  public void ShowProgress(ColonyAchievementStatus achievement)
  {
    if (Object.op_Equality((Object) this.progressParent, (Object) null))
      return;
    this.numRequirementsDisplayed = 0;
    for (int index = 0; index < achievement.Requirements.Count; ++index)
    {
      ColonyAchievementRequirement requirement = achievement.Requirements[index];
      switch (requirement)
      {
        case CritterTypesWithTraits _:
          this.ShowCritterChecklist(requirement);
          break;
        case DupesCompleteChoreInExoSuitForCycles _:
          this.ShowDupesInExoSuitsRequirement(achievement.success, requirement);
          break;
        case DupesVsSolidTransferArmFetch _:
          this.ShowArmsOutPeformingDupesRequirement(achievement.success, requirement);
          break;
        case ProduceXEngeryWithoutUsingYList _:
          this.ShowEngeryWithoutUsing(achievement.success, requirement);
          break;
        case MinimumMorale _:
          this.ShowMinimumMoraleRequirement(achievement.success, requirement);
          break;
        case SurviveARocketWithMinimumMorale _:
          this.ShowRocketMoraleRequirement(achievement.success, requirement);
          break;
        default:
          this.ShowRequirement(achievement.success, requirement);
          break;
      }
    }
  }

  private HierarchyReferences GetNextRequirementWidget()
  {
    GameObject gameObject;
    if (((Transform) this.progressParent).childCount <= this.numRequirementsDisplayed)
    {
      gameObject = Util.KInstantiateUI(this.requirementPrefab, ((Component) this.progressParent).gameObject, true);
    }
    else
    {
      gameObject = ((Component) ((Transform) this.progressParent).GetChild(this.numRequirementsDisplayed)).gameObject;
      gameObject.SetActive(true);
    }
    ++this.numRequirementsDisplayed;
    return gameObject.GetComponent<HierarchyReferences>();
  }

  private void SetDescription(string str, HierarchyReferences refs) => ((TMP_Text) refs.GetReference<LocText>("Desc")).SetText(str);

  private void SetIcon(Sprite sprite, Color color, HierarchyReferences refs)
  {
    Image reference = refs.GetReference<Image>("Icon");
    reference.sprite = sprite;
    ((Graphic) reference).color = color;
    ((Component) reference).gameObject.SetActive(true);
  }

  private void ShowIcon(bool show, HierarchyReferences refs) => ((Component) refs.GetReference<Image>("Icon")).gameObject.SetActive(show);

  private void ShowRequirement(bool succeed, ColonyAchievementRequirement req)
  {
    HierarchyReferences requirementWidget = this.GetNextRequirementWidget();
    bool complete = req.Success() | succeed;
    bool flag = req.Fail();
    if (complete && !flag)
      this.SetIcon(this.statusSuccessIcon, Color.green, requirementWidget);
    else if (flag)
      this.SetIcon(this.statusFailureIcon, Color.red, requirementWidget);
    else
      this.ShowIcon(false, requirementWidget);
    this.SetDescription(req.GetProgress(complete), requirementWidget);
  }

  private void ShowCritterChecklist(ColonyAchievementRequirement req)
  {
    CritterTypesWithTraits critterTypesWithTraits = req as CritterTypesWithTraits;
    if (req == null)
      return;
    foreach (KeyValuePair<Tag, bool> keyValuePair in critterTypesWithTraits.critterTypesToCheck)
    {
      HierarchyReferences requirementWidget = this.GetNextRequirementWidget();
      if (keyValuePair.Value)
        this.SetIcon(this.statusSuccessIcon, Color.green, requirementWidget);
      else
        this.ShowIcon(false, requirementWidget);
      string tameACritter = (string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.TAME_A_CRITTER;
      Tag key = keyValuePair.Key;
      string str = Tag.op_Implicit(((Tag) ref key).Name).ProperName();
      this.SetDescription(string.Format(tameACritter, (object) str), requirementWidget);
    }
  }

  private void ShowArmsOutPeformingDupesRequirement(bool succeed, ColonyAchievementRequirement req)
  {
    if (!(req is DupesVsSolidTransferArmFetch transferArmFetch))
      return;
    HierarchyReferences requirementWidget1 = this.GetNextRequirementWidget();
    if (succeed)
      this.SetIcon(this.statusSuccessIcon, Color.green, requirementWidget1);
    this.SetDescription(string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ARM_PERFORMANCE, (object) (succeed ? transferArmFetch.numCycles : transferArmFetch.currentCycleCount), (object) transferArmFetch.numCycles), requirementWidget1);
    if (succeed)
      return;
    Dictionary<int, int> dupeChoreDeliveries = ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().fetchDupeChoreDeliveries;
    Dictionary<int, int> automatedChoreDeliveries = ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().fetchAutomatedChoreDeliveries;
    int num1 = 0;
    int cycle = GameClock.Instance.GetCycle();
    ref int local = ref num1;
    dupeChoreDeliveries.TryGetValue(cycle, out local);
    int num2 = 0;
    automatedChoreDeliveries.TryGetValue(GameClock.Instance.GetCycle(), out num2);
    HierarchyReferences requirementWidget2 = this.GetNextRequirementWidget();
    if ((double) num1 < (double) num2 * (double) transferArmFetch.percentage)
      this.SetIcon(this.statusSuccessIcon, Color.green, requirementWidget2);
    else
      this.ShowIcon(false, requirementWidget2);
    this.SetDescription(string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.ARM_VS_DUPE_FETCHES, (object) "SolidTransferArm", (object) num2, (object) num1), requirementWidget2);
  }

  private void ShowDupesInExoSuitsRequirement(bool succeed, ColonyAchievementRequirement req)
  {
    if (!(req is DupesCompleteChoreInExoSuitForCycles exoSuitForCycles))
      return;
    HierarchyReferences requirementWidget1 = this.GetNextRequirementWidget();
    if (succeed)
      this.SetIcon(this.statusSuccessIcon, Color.green, requirementWidget1);
    else
      this.ShowIcon(false, requirementWidget1);
    this.SetDescription(string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXOSUIT_CYCLES, (object) (succeed ? exoSuitForCycles.numCycles : exoSuitForCycles.currentCycleStreak), (object) exoSuitForCycles.numCycles), requirementWidget1);
    if (succeed)
      return;
    HierarchyReferences requirementWidget2 = this.GetNextRequirementWidget();
    int num = exoSuitForCycles.GetNumberOfDupesForCycle(GameClock.Instance.GetCycle());
    if (num >= Components.LiveMinionIdentities.Count)
    {
      num = Components.LiveMinionIdentities.Count;
      this.SetIcon(this.statusSuccessIcon, Color.green, requirementWidget2);
    }
    this.SetDescription(string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.EXOSUIT_THIS_CYCLE, (object) num, (object) Components.LiveMinionIdentities.Count), requirementWidget2);
  }

  private void ShowEngeryWithoutUsing(bool succeed, ColonyAchievementRequirement req)
  {
    ProduceXEngeryWithoutUsingYList withoutUsingYlist = req as ProduceXEngeryWithoutUsingYList;
    if (req == null)
      return;
    HierarchyReferences requirementWidget1 = this.GetNextRequirementWidget();
    float productionAmount = withoutUsingYlist.GetProductionAmount(succeed);
    this.SetDescription(string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.GENERATE_POWER, (object) GameUtil.GetFormattedRoundedJoules(productionAmount), (object) GameUtil.GetFormattedRoundedJoules(withoutUsingYlist.amountToProduce * 1000f)), requirementWidget1);
    if (succeed)
      this.SetIcon(this.statusSuccessIcon, Color.green, requirementWidget1);
    else
      this.ShowIcon(false, requirementWidget1);
    foreach (Tag disallowedBuilding in withoutUsingYlist.disallowedBuildings)
    {
      HierarchyReferences requirementWidget2 = this.GetNextRequirementWidget();
      if (Game.Instance.savedInfo.powerCreatedbyGeneratorType.ContainsKey(disallowedBuilding))
        this.SetIcon(this.statusFailureIcon, Color.red, requirementWidget2);
      else
        this.SetIcon(this.statusSuccessIcon, Color.green, requirementWidget2);
      BuildingDef buildingDef = Assets.GetBuildingDef(((Tag) ref disallowedBuilding).Name);
      this.SetDescription(string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.NO_BUILDING, (object) buildingDef.Name), requirementWidget2);
    }
  }

  private void ShowMinimumMoraleRequirement(bool success, ColonyAchievementRequirement req)
  {
    if (!(req is MinimumMorale minimumMorale))
      return;
    if (success)
    {
      this.ShowRequirement(success, req);
    }
    else
    {
      foreach (MinionAssignablesProxy assignablesProxy in Components.MinionAssignablesProxy)
      {
        GameObject targetGameObject = assignablesProxy.GetTargetGameObject();
        if (Object.op_Inequality((Object) targetGameObject, (Object) null) && !targetGameObject.HasTag(GameTags.Dead))
        {
          AttributeInstance attributeInstance = Db.Get().Attributes.QualityOfLife.Lookup((Component) targetGameObject.GetComponent<MinionModifiers>());
          if (attributeInstance != null)
          {
            HierarchyReferences requirementWidget = this.GetNextRequirementWidget();
            if ((double) attributeInstance.GetTotalValue() >= (double) minimumMorale.minimumMorale)
              this.SetIcon(this.statusSuccessIcon, Color.green, requirementWidget);
            else
              this.ShowIcon(false, requirementWidget);
            this.SetDescription(string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.MORALE, (object) targetGameObject.GetProperName(), (object) attributeInstance.GetTotalDisplayValue()), requirementWidget);
          }
        }
      }
    }
  }

  private void ShowRocketMoraleRequirement(bool success, ColonyAchievementRequirement req)
  {
    if (!(req is SurviveARocketWithMinimumMorale withMinimumMorale))
      return;
    if (success)
    {
      this.ShowRequirement(success, req);
    }
    else
    {
      foreach (KeyValuePair<int, int> keyValuePair in ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().cyclesRocketDupeMoraleAboveRequirement)
      {
        WorldContainer world = ClusterManager.Instance.GetWorld(keyValuePair.Key);
        if (Object.op_Inequality((Object) world, (Object) null))
        {
          HierarchyReferences requirementWidget = this.GetNextRequirementWidget();
          this.SetDescription(string.Format((string) COLONY_ACHIEVEMENTS.MISC_REQUIREMENTS.STATUS.SURVIVE_SPACE, (object) withMinimumMorale.minimumMorale, (object) keyValuePair.Value, (object) withMinimumMorale.numberOfCycles, (object) ((Component) world).GetProperName()), requirementWidget);
        }
      }
    }
  }
}
