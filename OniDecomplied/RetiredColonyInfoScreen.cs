// Decompiled with JetBrains decompiler
// Type: RetiredColonyInfoScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RetiredColonyInfoScreen : KModalScreen
{
  public static RetiredColonyInfoScreen Instance;
  private bool wasPixelPerfect;
  [Header("Screen")]
  [SerializeField]
  private KButton closeButton;
  [Header("Header References")]
  [SerializeField]
  private GameObject explorerHeaderContainer;
  [SerializeField]
  private GameObject colonyHeaderContainer;
  [SerializeField]
  private LocText colonyName;
  [SerializeField]
  private LocText cycleCount;
  [Header("Timelapse References")]
  [SerializeField]
  private Slideshow slideshow;
  [SerializeField]
  private GameObject worldPrefab;
  private string focusedWorld;
  private string[] currentSlideshowFiles;
  [Header("Main Layout")]
  [SerializeField]
  private GameObject coloniesSection;
  [SerializeField]
  private GameObject achievementsSection;
  [Header("Achievement References")]
  [SerializeField]
  private GameObject achievementsContainer;
  [SerializeField]
  private GameObject achievementsPrefab;
  [SerializeField]
  private GameObject victoryAchievementsPrefab;
  [SerializeField]
  private KInputTextField achievementSearch;
  [SerializeField]
  private KButton clearAchievementSearchButton;
  [SerializeField]
  private GameObject[] achievementVeils;
  [Header("Duplicant References")]
  [SerializeField]
  private GameObject duplicantPrefab;
  [Header("Building References")]
  [SerializeField]
  private GameObject buildingPrefab;
  [Header("Colony Stat References")]
  [SerializeField]
  private GameObject statsContainer;
  [SerializeField]
  private GameObject specialMediaBlock;
  [SerializeField]
  private GameObject tallFeatureBlock;
  [SerializeField]
  private GameObject standardStatBlock;
  [SerializeField]
  private GameObject lineGraphPrefab;
  public RetiredColonyData[] retiredColonyData;
  [Header("Explorer References")]
  [SerializeField]
  private GameObject colonyScroll;
  [SerializeField]
  private GameObject explorerRoot;
  [SerializeField]
  private GameObject explorerGrid;
  [SerializeField]
  private GameObject colonyDataRoot;
  [SerializeField]
  private GameObject colonyButtonPrefab;
  [SerializeField]
  private KInputTextField explorerSearch;
  [SerializeField]
  private KButton clearExplorerSearchButton;
  [Header("Navigation Buttons")]
  [SerializeField]
  private KButton closeScreenButton;
  [SerializeField]
  private KButton viewOtherColoniesButton;
  [SerializeField]
  private KButton quitToMainMenuButton;
  [SerializeField]
  private GameObject disabledPlatformUnlocks;
  private bool explorerGridConfigured;
  private Dictionary<string, GameObject> achievementEntries;
  private List<GameObject> activeColonyWidgetContainers;
  private Dictionary<string, GameObject> activeColonyWidgets;
  private const float maxAchievementWidth = 830f;
  private Canvas canvasRef;
  private Dictionary<string, Color> statColors;
  private Dictionary<string, GameObject> explorerColonyWidgets;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    RetiredColonyInfoScreen.Instance = this;
    this.ConfigButtons();
    this.LoadExplorer();
    this.PopulateAchievements();
    this.ConsumeMouseScroll = true;
    ((TMP_InputField) this.explorerSearch).text = "";
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.explorerSearch).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(\u003COnPrefabInit\u003Eb__45_0)));
    this.clearExplorerSearchButton.onClick += (System.Action) (() => ((TMP_InputField) this.explorerSearch).text = "");
    ((TMP_InputField) this.achievementSearch).text = "";
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.achievementSearch).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(\u003COnPrefabInit\u003Eb__45_2)));
    this.clearAchievementSearchButton.onClick += (System.Action) (() => ((TMP_InputField) this.achievementSearch).text = "");
    this.RefreshUIScale();
    ((KMonoBehaviour) this).Subscribe(-810220474, new Action<object>(this.RefreshUIScale));
  }

  private void RefreshUIScale(object data = null) => ((MonoBehaviour) this).StartCoroutine(this.DelayedRefreshScale());

  private IEnumerator DelayedRefreshScale()
  {
    for (int i = 0; i < 3; ++i)
      yield return (object) 0;
    float num1 = 36f;
    if (Object.op_Inequality((Object) GameObject.Find("ScreenSpaceOverlayCanvas"), (Object) null))
    {
      Transform parent = this.explorerRoot.transform.parent;
      Vector3 one = Vector3.one;
      Rect rect = Util.rectTransform(this.colonyScroll).rect;
      double num2 = (double) ((Rect) ref rect).width - (double) num1;
      rect = Util.rectTransform((Component) this.explorerRoot.transform.parent).rect;
      double width = (double) ((Rect) ref rect).width;
      double num3 = num2 / width;
      Vector3 vector3 = Vector3.op_Multiply(one, (float) num3);
      parent.localScale = vector3;
    }
    else
    {
      Transform parent = this.explorerRoot.transform.parent;
      Vector3 one = Vector3.one;
      Rect rect = Util.rectTransform(this.colonyScroll).rect;
      double num4 = (double) ((Rect) ref rect).width - (double) num1;
      rect = Util.rectTransform((Component) this.explorerRoot.transform.parent).rect;
      double width = (double) ((Rect) ref rect).width;
      double num5 = num4 / width;
      Vector3 vector3 = Vector3.op_Multiply(one, (float) num5);
      parent.localScale = vector3;
    }
  }

  private void ConfigButtons()
  {
    this.closeButton.ClearOnClick();
    this.closeButton.onClick += (System.Action) (() => this.Show(false));
    this.viewOtherColoniesButton.ClearOnClick();
    this.viewOtherColoniesButton.onClick += (System.Action) (() => this.ToggleExplorer(true));
    this.quitToMainMenuButton.ClearOnClick();
    this.quitToMainMenuButton.onClick += (System.Action) (() => this.ConfirmDecision((string) STRINGS.UI.FRONTEND.MAINMENU.QUITCONFIRM, new System.Action(this.OnQuitConfirm)));
    this.closeScreenButton.ClearOnClick();
    this.closeScreenButton.onClick += (System.Action) (() => this.Show(false));
    ((Component) this.viewOtherColoniesButton).gameObject.SetActive(false);
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
    {
      ((Component) this.closeScreenButton).gameObject.SetActive(true);
      ((TMP_Text) ((Component) this.closeScreenButton).GetComponentInChildren<LocText>()).SetText((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.BUTTONS.RETURN_TO_GAME);
      ((Component) this.quitToMainMenuButton).gameObject.SetActive(true);
    }
    else
    {
      ((Component) this.closeScreenButton).gameObject.SetActive(true);
      ((TMP_Text) ((Component) this.closeScreenButton).GetComponentInChildren<LocText>()).SetText((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.BUTTONS.CLOSE);
      ((Component) this.quitToMainMenuButton).gameObject.SetActive(false);
    }
  }

  private void ConfirmDecision(string text, System.Action onConfirm)
  {
    ((Component) this).gameObject.SetActive(false);
    ((ConfirmDialogScreen) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) ((KMonoBehaviour) this).transform.parent).gameObject, (GameScreenManager.UIRenderTarget) 2)).PopupConfirmDialog(text, onConfirm, new System.Action(this.OnCancelPopup));
  }

  private void OnCancelPopup() => ((Component) this).gameObject.SetActive(true);

  private void OnQuitConfirm() => LoadingOverlay.Load((System.Action) (() =>
  {
    this.Deactivate();
    PauseScreen.TriggerQuitGame();
  }));

  protected override void OnCmpEnable()
  {
    base.OnCmpEnable();
    this.GetCanvasRef();
    this.wasPixelPerfect = this.canvasRef.pixelPerfect;
    this.canvasRef.pixelPerfect = false;
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return;
    if (e.TryConsume((Action) 5))
      this.Show(false);
    base.OnKeyDown(e);
  }

  private void GetCanvasRef()
  {
    if (Object.op_Inequality((Object) ((Component) ((KMonoBehaviour) this).transform.parent).GetComponent<Canvas>(), (Object) null))
      this.canvasRef = ((Component) ((KMonoBehaviour) this).transform.parent).GetComponent<Canvas>();
    else
      this.canvasRef = ((Component) ((KMonoBehaviour) this).transform.parent.parent).GetComponent<Canvas>();
  }

  protected override void OnCmpDisable()
  {
    this.canvasRef.pixelPerfect = this.wasPixelPerfect;
    base.OnCmpDisable();
  }

  public RetiredColonyData GetColonyDataByBaseName(string name)
  {
    name = RetireColonyUtility.StripInvalidCharacters(name);
    for (int index = 0; index < this.retiredColonyData.Length; ++index)
    {
      if (RetireColonyUtility.StripInvalidCharacters(this.retiredColonyData[index].colonyName) == name)
        return this.retiredColonyData[index];
    }
    return (RetiredColonyData) null;
  }

  protected override void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
      this.RefreshUIScale();
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
    {
      if (!show)
      {
        if (MusicManager.instance.SongIsPlaying("Music_Victory_03_StoryAndSummary"))
          MusicManager.instance.StopSong("Music_Victory_03_StoryAndSummary");
      }
      else
      {
        this.retiredColonyData = RetireColonyUtility.LoadRetiredColonies(true);
        if (MusicManager.instance.SongIsPlaying("Music_Victory_03_StoryAndSummary"))
          MusicManager.instance.SetSongParameter("Music_Victory_03_StoryAndSummary", "songSection", 2f);
      }
    }
    else if (Object.op_Equality((Object) Game.Instance, (Object) null))
      this.ToggleExplorer(true);
    this.disabledPlatformUnlocks.SetActive(Object.op_Inequality((Object) SaveGame.Instance, (Object) null));
    if (!Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
      return;
    this.disabledPlatformUnlocks.GetComponent<HierarchyReferences>().GetReference("enabled").gameObject.SetActive(!DebugHandler.InstantBuildMode && !SaveGame.Instance.sandboxEnabled && !Game.Instance.debugWasUsed);
    this.disabledPlatformUnlocks.GetComponent<HierarchyReferences>().GetReference("disabled").gameObject.SetActive(DebugHandler.InstantBuildMode || SaveGame.Instance.sandboxEnabled || Game.Instance.debugWasUsed);
  }

  public void LoadColony(RetiredColonyData data)
  {
    ((TMP_Text) this.colonyName).text = data.colonyName.ToUpper();
    ((TMP_Text) this.cycleCount).text = string.Format((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.CYCLE_COUNT, (object) data.cycleCount.ToString());
    this.focusedWorld = data.startWorld;
    this.ToggleExplorer(false);
    this.RefreshUIScale();
    if (Object.op_Equality((Object) Game.Instance, (Object) null))
      ((Component) this.viewOtherColoniesButton).gameObject.SetActive(true);
    this.ClearColony();
    if (Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
    {
      ColonyAchievementTracker component = ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>();
      this.UpdateAchievementData(data, component.achievementsToDisplay.ToArray());
      component.ClearDisplayAchievements();
      this.PopulateAchievementProgress(component);
    }
    else
      this.UpdateAchievementData(data);
    this.DisplayStatistics(data);
    TransformExtensions.SetPosition((Transform) Util.rectTransform((Component) this.colonyDataRoot.transform.parent), new Vector3(((Transform) Util.rectTransform((Component) this.colonyDataRoot.transform.parent)).position.x, 0.0f, 0.0f));
  }

  private void PopulateAchievementProgress(ColonyAchievementTracker tracker)
  {
    if (!Object.op_Inequality((Object) tracker, (Object) null))
      return;
    foreach (KeyValuePair<string, GameObject> achievementEntry in this.achievementEntries)
    {
      ColonyAchievementStatus achievement;
      tracker.achievements.TryGetValue(achievementEntry.Key, out achievement);
      if (achievement != null)
      {
        AchievementWidget component = achievementEntry.Value.GetComponent<AchievementWidget>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          component.ShowProgress(achievement);
          if (achievement.failed)
            component.SetFailed();
        }
      }
    }
  }

  private bool LoadSlideshow(RetiredColonyData data)
  {
    this.clearCurrentSlideshow();
    this.currentSlideshowFiles = RetireColonyUtility.LoadColonySlideshowFiles(data.colonyName, this.focusedWorld);
    this.slideshow.SetFiles(this.currentSlideshowFiles);
    return this.currentSlideshowFiles != null && this.currentSlideshowFiles.Length != 0;
  }

  private void clearCurrentSlideshow() => this.currentSlideshowFiles = new string[0];

  private bool LoadScreenshot(RetiredColonyData data, string world)
  {
    this.clearCurrentSlideshow();
    Sprite sprite = RetireColonyUtility.LoadRetiredColonyPreview(data.colonyName, world);
    if (Object.op_Inequality((Object) sprite, (Object) null))
    {
      this.slideshow.setSlide(sprite);
      this.CorrectTimelapseImageSize(sprite);
    }
    return Object.op_Inequality((Object) sprite, (Object) null);
  }

  private void ClearColony()
  {
    foreach (Object colonyWidgetContainer in this.activeColonyWidgetContainers)
      Object.Destroy(colonyWidgetContainer);
    this.activeColonyWidgetContainers.Clear();
    this.activeColonyWidgets.Clear();
    this.UpdateAchievementData((RetiredColonyData) null);
  }

  private void PopulateAchievements()
  {
    foreach (ColonyAchievement resource in Db.Get().ColonyAchievements.resources)
    {
      GameObject gameObject = Util.KInstantiateUI(resource.isVictoryCondition ? this.victoryAchievementsPrefab : this.achievementsPrefab, this.achievementsContainer, true);
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      ((TMP_Text) component.GetReference<LocText>("nameLabel")).SetText(resource.Name);
      ((TMP_Text) component.GetReference<LocText>("descriptionLabel")).SetText(resource.description);
      if (string.IsNullOrEmpty(resource.icon) || Object.op_Equality((Object) Assets.GetSprite(HashedString.op_Implicit(resource.icon)), (Object) null))
      {
        if (Object.op_Inequality((Object) Assets.GetSprite(HashedString.op_Implicit(resource.Name)), (Object) null))
          component.GetReference<Image>("icon").sprite = Assets.GetSprite(HashedString.op_Implicit(resource.Name));
        else
          component.GetReference<Image>("icon").sprite = Assets.GetSprite(HashedString.op_Implicit("check"));
      }
      else
        component.GetReference<Image>("icon").sprite = Assets.GetSprite(HashedString.op_Implicit(resource.icon));
      if (resource.isVictoryCondition)
        gameObject.transform.SetAsFirstSibling();
      bool flag = !DlcManager.IsValidForVanilla(resource.dlcIds);
      ((Component) component.GetReference<KImage>("dlc_overlay")).gameObject.SetActive(flag);
      gameObject.GetComponent<MultiToggle>().ChangeState(2);
      gameObject.GetComponent<AchievementWidget>().dlcAchievement = flag;
      this.achievementEntries.Add(resource.Id, gameObject);
    }
    this.UpdateAchievementData((RetiredColonyData) null);
  }

  private IEnumerator ClearAchievementVeil(float delay = 0.0f)
  {
    yield return (object) new WaitForSecondsRealtime(delay);
    for (float i = 0.7f; (double) i >= 0.0; i -= Time.unscaledDeltaTime)
    {
      foreach (GameObject achievementVeil in this.achievementVeils)
        ((Graphic) achievementVeil.GetComponent<Image>()).color = new Color(0.0f, 0.0f, 0.0f, i);
      yield return (object) 0;
    }
    foreach (GameObject achievementVeil in this.achievementVeils)
      achievementVeil.SetActive(false);
  }

  private IEnumerator ShowAchievementVeil()
  {
    float targetAlpha = 0.7f;
    foreach (GameObject achievementVeil in this.achievementVeils)
      achievementVeil.SetActive(true);
    for (float i = 0.0f; (double) i <= (double) targetAlpha; i += Time.unscaledDeltaTime)
    {
      foreach (GameObject achievementVeil in this.achievementVeils)
        ((Graphic) achievementVeil.GetComponent<Image>()).color = new Color(0.0f, 0.0f, 0.0f, i);
      yield return (object) 0;
    }
    for (float num = 0.0f; (double) num <= (double) targetAlpha; num += Time.unscaledDeltaTime)
    {
      foreach (GameObject achievementVeil in this.achievementVeils)
        ((Graphic) achievementVeil.GetComponent<Image>()).color = new Color(0.0f, 0.0f, 0.0f, targetAlpha);
    }
  }

  private void UpdateAchievementData(RetiredColonyData data, string[] newlyAchieved = null)
  {
    int num1 = 0;
    float num2 = 2f;
    float num3 = 1f;
    if (newlyAchieved != null && newlyAchieved.Length != 0)
      this.retiredColonyData = RetireColonyUtility.LoadRetiredColonies(true);
    foreach (KeyValuePair<string, GameObject> achievementEntry in this.achievementEntries)
    {
      bool flag1 = false;
      bool flag2 = false;
      if (data != null)
      {
        foreach (string achievement in data.achievements)
        {
          if (achievement == achievementEntry.Key)
          {
            flag1 = true;
            break;
          }
        }
      }
      if (!flag1 && data == null && this.retiredColonyData != null)
      {
        foreach (RetiredColonyData retiredColonyData in this.retiredColonyData)
        {
          foreach (string achievement in retiredColonyData.achievements)
          {
            if (achievement == achievementEntry.Key)
              flag2 = true;
          }
        }
      }
      bool flag3 = false;
      if (newlyAchieved != null)
      {
        for (int index = 0; index < newlyAchieved.Length; ++index)
        {
          if (newlyAchieved[index] == achievementEntry.Key)
            flag3 = true;
        }
      }
      if (flag1 | flag3)
      {
        if (flag3)
        {
          achievementEntry.Value.GetComponent<AchievementWidget>().ActivateNewlyAchievedFlourish(num3 + (float) num1 * num2);
          ++num1;
        }
        else
          achievementEntry.Value.GetComponent<AchievementWidget>().SetAchievedNow();
      }
      else if (flag2)
        achievementEntry.Value.GetComponent<AchievementWidget>().SetAchievedBefore();
      else if (data == null)
        achievementEntry.Value.GetComponent<AchievementWidget>().SetNeverAchieved();
      else
        achievementEntry.Value.GetComponent<AchievementWidget>().SetNotAchieved();
    }
    if (newlyAchieved == null || newlyAchieved.Length == 0)
      return;
    ((MonoBehaviour) this).StartCoroutine(this.ShowAchievementVeil());
    ((MonoBehaviour) this).StartCoroutine(this.ClearAchievementVeil(num3 + (float) num1 * num2));
  }

  private void DisplayInfoBlock(RetiredColonyData data, GameObject container)
  {
    ((TMP_Text) container.GetComponent<HierarchyReferences>().GetReference<LocText>("ColonyNameLabel")).SetText(data.colonyName);
    ((TMP_Text) container.GetComponent<HierarchyReferences>().GetReference<LocText>("CycleCountLabel")).SetText(string.Format((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.CYCLE_COUNT, (object) data.cycleCount.ToString()));
  }

  private void CorrectTimelapseImageSize(Sprite sprite)
  {
    Vector2 sizeDelta = ((Component) this.slideshow.transform.parent).GetComponent<RectTransform>().sizeDelta;
    Vector2 fittedSize = this.slideshow.GetFittedSize(sprite, sizeDelta.x, sizeDelta.y);
    LayoutElement component = ((Component) this.slideshow).GetComponent<LayoutElement>();
    if ((double) fittedSize.y > (double) component.preferredHeight)
    {
      component.minHeight = component.preferredHeight / (fittedSize.y / fittedSize.x);
      component.minHeight = component.preferredHeight;
    }
    else
    {
      component.minWidth = component.preferredWidth = fittedSize.x;
      component.minHeight = component.preferredHeight = fittedSize.y;
    }
  }

  private void DisplayTimelapse(RetiredColonyData data, GameObject container)
  {
    ((TMP_Text) container.GetComponent<HierarchyReferences>().GetReference<LocText>("Title")).SetText((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.TITLES.TIMELAPSE);
    RectTransform reference1 = container.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Worlds");
    this.DisplayWorlds(data, ((Component) reference1).gameObject);
    RectTransform reference2 = container.GetComponent<HierarchyReferences>().GetReference<RectTransform>("PlayIcon");
    this.slideshow = container.GetComponent<HierarchyReferences>().GetReference<Slideshow>("Slideshow");
    this.slideshow.updateType = SlideshowUpdateType.loadOnDemand;
    this.slideshow.SetPaused(true);
    this.slideshow.onBeforePlay = (Slideshow.onBeforeAndEndPlayDelegate) (() => this.LoadSlideshow(data));
    this.slideshow.onEndingPlay = (Slideshow.onBeforeAndEndPlayDelegate) (() => this.LoadScreenshot(data, this.focusedWorld));
    if (!this.LoadScreenshot(data, this.focusedWorld))
    {
      ((Component) this.slideshow).gameObject.SetActive(false);
      ((Component) reference2).gameObject.SetActive(false);
    }
    else
    {
      ((Component) this.slideshow).gameObject.SetActive(true);
      ((Component) reference2).gameObject.SetActive(true);
    }
  }

  private void DisplayDuplicants(
    RetiredColonyData data,
    GameObject container,
    int range_min = -1,
    int range_max = -1)
  {
    for (int index = container.transform.childCount - 1; index >= 0; --index)
      Object.DestroyImmediate((Object) ((Component) container.transform.GetChild(index)).gameObject);
    for (int index = 0; index < data.Duplicants.Length; ++index)
    {
      if (index < range_min || index > range_max && range_max != -1)
      {
        new GameObject().transform.SetParent(container.transform);
      }
      else
      {
        RetiredColonyData.RetiredDuplicantData duplicant = data.Duplicants[index];
        GameObject gameObject = Util.KInstantiateUI(this.duplicantPrefab, container, true);
        HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
        ((TMP_Text) component.GetReference<LocText>("NameLabel")).SetText(duplicant.name);
        ((TMP_Text) component.GetReference<LocText>("AgeLabel")).SetText(string.Format((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.DUPLICANT_AGE, (object) duplicant.age.ToString()));
        ((TMP_Text) component.GetReference<LocText>("SkillLabel")).SetText(string.Format((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.SKILL_LEVEL, (object) duplicant.skillPointsGained.ToString()));
        SymbolOverrideController reference = component.GetReference<SymbolOverrideController>("SymbolOverrideController");
        reference.RemoveAllSymbolOverrides();
        KBatchedAnimController componentInChildren = gameObject.GetComponentInChildren<KBatchedAnimController>();
        componentInChildren.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_neck"), false);
        componentInChildren.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_goggles"), false);
        componentInChildren.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_hat"), false);
        componentInChildren.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_headfx"), false);
        componentInChildren.SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapTo_hat_hair"), false);
        foreach (KeyValuePair<string, string> accessory in duplicant.accessories)
        {
          if (Db.Get().Accessories.Exists(accessory.Value))
          {
            KAnim.Build.Symbol symbol = Db.Get().Accessories.Get(accessory.Value).symbol;
            AccessorySlot accessorySlot = Db.Get().AccessorySlots.Get(accessory.Key);
            reference.AddSymbolOverride(HashedString.op_Implicit(accessorySlot.targetSymbolId), symbol);
            gameObject.GetComponentInChildren<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit(accessory.Key), true);
          }
        }
        reference.ApplyOverrides();
      }
    }
    ((MonoBehaviour) this).StartCoroutine(this.ActivatePortraitsWhenReady(container));
  }

  private IEnumerator ActivatePortraitsWhenReady(GameObject container)
  {
    yield return (object) 0;
    if (Object.op_Equality((Object) container, (Object) null))
    {
      Debug.LogError((object) "RetiredColonyInfoScreen minion container is null");
    }
    else
    {
      for (int index = 0; index < container.transform.childCount; ++index)
      {
        KBatchedAnimController componentInChildren = ((Component) container.transform.GetChild(index)).GetComponentInChildren<KBatchedAnimController>();
        if (Object.op_Inequality((Object) componentInChildren, (Object) null))
          ((Component) componentInChildren).transform.localScale = Vector3.one;
      }
    }
  }

  private void DisplayBuildings(RetiredColonyData data, GameObject container)
  {
    for (int index = container.transform.childCount - 1; index >= 0; --index)
      Object.Destroy((Object) ((Component) container.transform.GetChild(index)).gameObject);
    data.buildings.Sort((Comparison<Tuple<string, int>>) ((a, b) =>
    {
      if (a.second > b.second)
        return 1;
      return a.second == b.second ? 0 : -1;
    }));
    data.buildings.Reverse();
    foreach (Tuple<string, int> building in data.buildings)
    {
      GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(building.first));
      if (!Object.op_Equality((Object) prefab, (Object) null))
      {
        HierarchyReferences component = Util.KInstantiateUI(this.buildingPrefab, container, true).GetComponent<HierarchyReferences>();
        ((TMP_Text) component.GetReference<LocText>("NameLabel")).SetText(GameUtil.ApplyBoldString(prefab.GetProperName()));
        ((TMP_Text) component.GetReference<LocText>("CountLabel")).SetText(string.Format((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.BUILDING_COUNT, (object) building.second.ToString()));
        Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) prefab);
        component.GetReference<Image>("Portrait").sprite = uiSprite.first;
      }
    }
  }

  private void DisplayWorlds(RetiredColonyData data, GameObject container)
  {
    container.SetActive(data.worldIdentities.Count > 0);
    for (int index = container.transform.childCount - 1; index >= 0; --index)
      Object.Destroy((Object) ((Component) container.transform.GetChild(index)).gameObject);
    if (data.worldIdentities.Count <= 0)
      return;
    foreach (KeyValuePair<string, string> worldIdentity in data.worldIdentities)
    {
      KeyValuePair<string, string> worldPair = worldIdentity;
      GameObject gameObject = Util.KInstantiateUI(this.worldPrefab, container, true);
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      World worldData = SettingsCache.worlds.GetWorldData(worldPair.Value);
      Sprite sprite = worldData != null ? ColonyDestinationAsteroidBeltData.GetUISprite(worldData.asteroidIcon) : (Sprite) null;
      if (Object.op_Inequality((Object) sprite, (Object) null))
        component.GetReference<Image>("Portrait").sprite = sprite;
      gameObject.GetComponent<KButton>().onClick += (System.Action) (() =>
      {
        this.focusedWorld = worldPair.Key;
        this.LoadScreenshot(data, this.focusedWorld);
      });
    }
  }

  private IEnumerator ComputeSizeStatGrid()
  {
    RetiredColonyInfoScreen colonyInfoScreen = this;
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    GridLayoutGroup component = colonyInfoScreen.statsContainer.GetComponent<GridLayoutGroup>();
    component.constraint = (GridLayoutGroup.Constraint) 1;
    component.constraintCount = Screen.width < 1920 ? 2 : 3;
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    Rect rect1 = Util.rectTransform(((Component) colonyInfoScreen).gameObject).rect;
    double width1 = (double) ((Rect) ref rect1).width;
    Rect rect2 = Util.rectTransform((Component) colonyInfoScreen.explorerRoot.transform.parent).rect;
    double width2 = (double) ((Rect) ref rect2).width;
    float num = Mathf.Min(830f, (float) (width1 - width2 - 50.0));
    colonyInfoScreen.achievementsSection.GetComponent<LayoutElement>().preferredWidth = num;
  }

  private IEnumerator ComputeSizeExplorerGrid()
  {
    RetiredColonyInfoScreen colonyInfoScreen = this;
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    GridLayoutGroup component = colonyInfoScreen.explorerGrid.GetComponent<GridLayoutGroup>();
    component.constraint = (GridLayoutGroup.Constraint) 1;
    component.constraintCount = Screen.width < 1920 ? 2 : 3;
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    Rect rect1 = Util.rectTransform(((Component) colonyInfoScreen).gameObject).rect;
    double width1 = (double) ((Rect) ref rect1).width;
    Rect rect2 = Util.rectTransform((Component) colonyInfoScreen.explorerRoot.transform.parent).rect;
    double width2 = (double) ((Rect) ref rect2).width;
    float num = Mathf.Min(830f, (float) (width1 - width2 - 50.0));
    colonyInfoScreen.achievementsSection.GetComponent<LayoutElement>().preferredWidth = num;
  }

  private void DisplayStatistics(RetiredColonyData data)
  {
    GameObject container = Util.KInstantiateUI(this.specialMediaBlock, this.statsContainer, true);
    this.activeColonyWidgetContainers.Add(container);
    this.activeColonyWidgets.Add("timelapse", container);
    this.DisplayTimelapse(data, container);
    GameObject duplicantBlock = Util.KInstantiateUI(this.tallFeatureBlock, this.statsContainer, true);
    this.activeColonyWidgetContainers.Add(duplicantBlock);
    this.activeColonyWidgets.Add("duplicants", duplicantBlock);
    ((TMP_Text) duplicantBlock.GetComponent<HierarchyReferences>().GetReference<LocText>("Title")).SetText((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.TITLES.DUPLICANTS);
    PageView pageView = duplicantBlock.GetComponentInChildren<PageView>();
    pageView.OnChangePage = (Action<int>) (page => this.DisplayDuplicants(data, duplicantBlock.GetComponent<HierarchyReferences>().GetReference("Content").gameObject, page * pageView.ChildrenPerPage, (page + 1) * pageView.ChildrenPerPage));
    this.DisplayDuplicants(data, duplicantBlock.GetComponent<HierarchyReferences>().GetReference("Content").gameObject);
    GameObject gameObject = Util.KInstantiateUI(this.tallFeatureBlock, this.statsContainer, true);
    this.activeColonyWidgetContainers.Add(gameObject);
    this.activeColonyWidgets.Add("buildings", gameObject);
    ((TMP_Text) gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Title")).SetText((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.TITLES.BUILDINGS);
    this.DisplayBuildings(data, gameObject.GetComponent<HierarchyReferences>().GetReference("Content").gameObject);
    int num = 2;
    for (int index1 = 0; index1 < data.Stats.Length; index1 += num)
    {
      GameObject layoutBlockGameObject = Util.KInstantiateUI(this.standardStatBlock, this.statsContainer, true);
      this.activeColonyWidgetContainers.Add(layoutBlockGameObject);
      for (int index2 = 0; index2 < num; ++index2)
      {
        if (index1 + index2 <= data.Stats.Length - 1)
          this.ConfigureGraph(this.GetStatistic(data.Stats[index1 + index2].id, data), layoutBlockGameObject);
      }
    }
    ((MonoBehaviour) this).StartCoroutine(this.ComputeSizeStatGrid());
  }

  private void ConfigureGraph(
    RetiredColonyData.RetiredColonyStatistic statistic,
    GameObject layoutBlockGameObject)
  {
    GameObject gameObject = Util.KInstantiateUI(this.lineGraphPrefab, layoutBlockGameObject, true);
    this.activeColonyWidgets.Add(statistic.name, gameObject);
    GraphBase componentInChildren1 = gameObject.GetComponentInChildren<GraphBase>();
    componentInChildren1.graphName = statistic.name;
    ((TMP_Text) componentInChildren1.label_title).SetText(componentInChildren1.graphName);
    componentInChildren1.axis_x.name = statistic.nameX;
    componentInChildren1.axis_y.name = statistic.nameY;
    ((TMP_Text) componentInChildren1.label_x).SetText(componentInChildren1.axis_x.name);
    ((TMP_Text) componentInChildren1.label_y).SetText(componentInChildren1.axis_y.name);
    LineLayer componentInChildren2 = gameObject.GetComponentInChildren<LineLayer>();
    componentInChildren1.axis_y.min_value = 0.0f;
    componentInChildren1.axis_y.max_value = statistic.GetByMaxValue().second * 1.2f;
    if (float.IsNaN(componentInChildren1.axis_y.max_value))
      componentInChildren1.axis_y.max_value = 1f;
    componentInChildren1.axis_x.min_value = 0.0f;
    componentInChildren1.axis_x.max_value = statistic.GetByMaxKey().first;
    componentInChildren1.axis_x.guide_frequency = (float) (((double) componentInChildren1.axis_x.max_value - (double) componentInChildren1.axis_x.min_value) / 10.0);
    componentInChildren1.axis_y.guide_frequency = (float) (((double) componentInChildren1.axis_y.max_value - (double) componentInChildren1.axis_y.min_value) / 10.0);
    componentInChildren1.RefreshGuides();
    Tuple<float, float>[] points = statistic.value;
    GraphedLine graphedLine = componentInChildren2.NewLine(points, statistic.id);
    if (this.statColors.ContainsKey(statistic.id))
      componentInChildren2.line_formatting[componentInChildren2.line_formatting.Length - 1].color = this.statColors[statistic.id];
    ((Graphic) graphedLine.line_renderer).color = componentInChildren2.line_formatting[componentInChildren2.line_formatting.Length - 1].color;
  }

  private RetiredColonyData.RetiredColonyStatistic GetStatistic(string id, RetiredColonyData data)
  {
    foreach (RetiredColonyData.RetiredColonyStatistic stat in data.Stats)
    {
      if (stat.id == id)
        return stat;
    }
    return (RetiredColonyData.RetiredColonyStatistic) null;
  }

  private void ToggleExplorer(bool active)
  {
    if (active && Object.op_Equality((Object) Game.Instance, (Object) null))
      WorldGen.LoadSettings();
    this.ConfigButtons();
    this.explorerRoot.SetActive(active);
    this.colonyDataRoot.SetActive(!active);
    if (!this.explorerGridConfigured)
    {
      this.explorerGridConfigured = true;
      ((MonoBehaviour) this).StartCoroutine(this.ComputeSizeExplorerGrid());
    }
    this.explorerHeaderContainer.SetActive(active);
    this.colonyHeaderContainer.SetActive(!active);
    if (active)
      TransformExtensions.SetPosition((Transform) Util.rectTransform((Component) this.colonyDataRoot.transform.parent), new Vector3(((Transform) Util.rectTransform((Component) this.colonyDataRoot.transform.parent)).position.x, 0.0f, 0.0f));
    this.UpdateAchievementData((RetiredColonyData) null);
    ((TMP_InputField) this.explorerSearch).text = "";
  }

  private void LoadExplorer()
  {
    if (Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
      return;
    this.ToggleExplorer(true);
    this.retiredColonyData = RetireColonyUtility.LoadRetiredColonies();
    foreach (RetiredColonyData retiredColonyData in this.retiredColonyData)
    {
      RetiredColonyData data = retiredColonyData;
      GameObject gameObject = Util.KInstantiateUI(this.colonyButtonPrefab, this.explorerGrid, true);
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      Sprite sprite = RetireColonyUtility.LoadRetiredColonyPreview(RetireColonyUtility.StripInvalidCharacters(data.colonyName), data.startWorld);
      Image reference1 = component.GetReference<Image>("ColonyImage");
      RectTransform reference2 = component.GetReference<RectTransform>("PreviewUnavailableText");
      if (Object.op_Inequality((Object) sprite, (Object) null))
      {
        ((Behaviour) reference1).enabled = true;
        reference1.sprite = sprite;
        ((Component) reference2).gameObject.SetActive(false);
      }
      else
      {
        ((Behaviour) reference1).enabled = false;
        ((Component) reference2).gameObject.SetActive(true);
      }
      ((TMP_Text) component.GetReference<LocText>("ColonyNameLabel")).SetText(retiredColonyData.colonyName);
      ((TMP_Text) component.GetReference<LocText>("CycleCountLabel")).SetText(string.Format((string) STRINGS.UI.RETIRED_COLONY_INFO_SCREEN.CYCLE_COUNT, (object) retiredColonyData.cycleCount.ToString()));
      ((TMP_Text) component.GetReference<LocText>("DateLabel")).SetText(retiredColonyData.date);
      gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.LoadColony(data));
      string key = retiredColonyData.colonyName;
      int num = 0;
      for (; this.explorerColonyWidgets.ContainsKey(key); key = retiredColonyData.colonyName + "_" + num.ToString())
        ++num;
      this.explorerColonyWidgets.Add(key, gameObject);
    }
  }

  private void FilterExplorer(string search)
  {
    foreach (KeyValuePair<string, GameObject> explorerColonyWidget in this.explorerColonyWidgets)
    {
      if (string.IsNullOrEmpty(search) || explorerColonyWidget.Key.ToUpper().Contains(search.ToUpper()))
        explorerColonyWidget.Value.SetActive(true);
      else
        explorerColonyWidget.Value.SetActive(false);
    }
  }

  private void FilterColonyData(string search)
  {
    foreach (KeyValuePair<string, GameObject> activeColonyWidget in this.activeColonyWidgets)
    {
      if (string.IsNullOrEmpty(search) || activeColonyWidget.Key.ToUpper().Contains(search.ToUpper()))
        activeColonyWidget.Value.SetActive(true);
      else
        activeColonyWidget.Value.SetActive(false);
    }
  }

  private void FilterAchievements(string search)
  {
    foreach (KeyValuePair<string, GameObject> achievementEntry in this.achievementEntries)
    {
      if (string.IsNullOrEmpty(search) || Db.Get().ColonyAchievements.Get(achievementEntry.Key).Name.ToUpper().Contains(search.ToUpper()))
        achievementEntry.Value.SetActive(true);
      else
        achievementEntry.Value.SetActive(false);
    }
  }

  public RetiredColonyInfoScreen()
  {
    Dictionary<string, Color> dictionary = new Dictionary<string, Color>();
    dictionary.Add(RetiredColonyData.DataIDs.OxygenProduced, new Color(0.17f, 0.91f, 0.91f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.OxygenConsumed, new Color(0.17f, 0.91f, 0.91f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.CaloriesProduced, new Color(0.24f, 0.49f, 0.32f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.CaloriesRemoved, new Color(0.24f, 0.49f, 0.32f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.PowerProduced, new Color(0.98f, 0.69f, 0.23f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.PowerWasted, new Color(0.82f, 0.3f, 0.35f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.WorkTime, new Color(0.99f, 0.51f, 0.28f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.TravelTime, new Color(0.55f, 0.55f, 0.75f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.AverageWorkTime, new Color(0.99f, 0.51f, 0.28f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.AverageTravelTime, new Color(0.55f, 0.55f, 0.75f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.LiveDuplicants, new Color(0.98f, 0.69f, 0.23f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.RocketsInFlight, new Color(0.9f, 0.9f, 0.16f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.AverageStressCreated, new Color(0.8f, 0.32f, 0.33f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.AverageStressRemoved, new Color(0.8f, 0.32f, 0.33f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.AverageGerms, new Color(0.68f, 0.79f, 0.18f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.DomesticatedCritters, new Color(0.62f, 0.31f, 0.47f, 1f));
    dictionary.Add(RetiredColonyData.DataIDs.WildCritters, new Color(0.62f, 0.31f, 0.47f, 1f));
    this.statColors = dictionary;
    this.explorerColonyWidgets = new Dictionary<string, GameObject>();
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }
}
