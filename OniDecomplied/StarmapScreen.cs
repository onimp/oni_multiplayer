// Decompiled with JetBrains decompiler
// Type: StarmapScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StarmapScreen : KModalScreen
{
  public GameObject listPanel;
  public GameObject rocketPanel;
  public LocText listHeaderLabel;
  public LocText listHeaderStatusLabel;
  public HierarchyReferences listRocketTemplate;
  public LocText listNoRocketText;
  public RectTransform rocketListContainer;
  private Dictionary<Spacecraft, HierarchyReferences> listRocketRows = new Dictionary<Spacecraft, HierarchyReferences>();
  [Header("Shared References")]
  public BreakdownList breakdownListPrefab;
  public GameObject progressBarPrefab;
  [Header("Selected Rocket References")]
  public LocText rocketHeaderLabel;
  public LocText rocketHeaderStatusLabel;
  private BreakdownList rocketDetailsStatus;
  public Sprite rocketDetailsStatusIcon;
  private BreakdownList rocketDetailsChecklist;
  public Sprite rocketDetailsChecklistIcon;
  private BreakdownList rocketDetailsMass;
  public Sprite rocketDetailsMassIcon;
  private BreakdownList rocketDetailsRange;
  public Sprite rocketDetailsRangeIcon;
  public RocketThrustWidget rocketThrustWidget;
  private BreakdownList rocketDetailsStorage;
  public Sprite rocketDetailsStorageIcon;
  private BreakdownList rocketDetailsDupes;
  public Sprite rocketDetailsDupesIcon;
  private BreakdownList rocketDetailsFuel;
  public Sprite rocketDetailsFuelIcon;
  private BreakdownList rocketDetailsOxidizer;
  public Sprite rocketDetailsOxidizerIcon;
  public RectTransform rocketDetailsContainer;
  [Header("Selected Destination References")]
  public LocText destinationHeaderLabel;
  public LocText destinationStatusLabel;
  public LocText destinationNameLabel;
  public LocText destinationTypeNameLabel;
  public LocText destinationTypeValueLabel;
  public LocText destinationDistanceNameLabel;
  public LocText destinationDistanceValueLabel;
  public LocText destinationDescriptionLabel;
  private BreakdownList destinationDetailsAnalysis;
  private GenericUIProgressBar destinationAnalysisProgressBar;
  public Sprite destinationDetailsAnalysisIcon;
  private BreakdownList destinationDetailsResearch;
  public Sprite destinationDetailsResearchIcon;
  private BreakdownList destinationDetailsMass;
  public Sprite destinationDetailsMassIcon;
  private BreakdownList destinationDetailsComposition;
  public Sprite destinationDetailsCompositionIcon;
  private BreakdownList destinationDetailsResources;
  public Sprite destinationDetailsResourcesIcon;
  private BreakdownList destinationDetailsArtifacts;
  public Sprite destinationDetailsArtifactsIcon;
  public RectTransform destinationDetailsContainer;
  public MultiToggle showRocketsButton;
  public MultiToggle launchButton;
  public MultiToggle analyzeButton;
  private int rocketConditionEventHandler = -1;
  [Header("Map References")]
  public RectTransform Map;
  public RectTransform rowsContiner;
  public GameObject rowPrefab;
  public StarmapPlanet planetPrefab;
  public GameObject rocketIconPrefab;
  private List<GameObject> planetRows = new List<GameObject>();
  private Dictionary<SpaceDestination, StarmapPlanet> planetWidgets = new Dictionary<SpaceDestination, StarmapPlanet>();
  private float planetsMaxDistance = 1f;
  public Image distanceOverlay;
  private int distanceOverlayVerticalOffset = 500;
  private int distanceOverlayYOffset = 24;
  public Image visualizeRocketImage;
  public Image visualizeRocketTrajectory;
  public LocText visualizeRocketLabel;
  public LocText visualizeRocketProgress;
  public Color[] distanceColors;
  public LocText titleBarLabel;
  public KButton button;
  private const int DESTINATION_ICON_SCALE = 2;
  public static StarmapScreen Instance;
  private int selectionUpdateHandle = -1;
  private SpaceDestination selectedDestination;
  private KSelectable currentSelectable;
  private CommandModule currentCommandModule;
  private LaunchConditionManager currentLaunchConditionManager;
  private bool currentRocketHasGasContainer;
  private bool currentRocketHasLiquidContainer;
  private bool currentRocketHasSolidContainer;
  private bool currentRocketHasEntitiesContainer;
  private bool forceScrollDown = true;
  private Coroutine animateAnalysisRoutine;
  private Coroutine animateSelectedPlanetRoutine;
  private BreakdownListRow rangeRowTotal;

  public override float GetSortKey() => this.isEditing ? 50f : 20f;

  public static void DestroyInstance() => StarmapScreen.Instance = (StarmapScreen) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.ConsumeMouseScroll = true;
    this.rocketDetailsStatus = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.rocketDetailsContainer);
    this.rocketDetailsStatus.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.MISSIONSTATUS);
    this.rocketDetailsStatus.SetIcon(this.rocketDetailsStatusIcon);
    ((Object) ((Component) this.rocketDetailsStatus).gameObject).name = "rocketDetailsStatus";
    this.rocketDetailsChecklist = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.rocketDetailsContainer);
    this.rocketDetailsChecklist.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.LAUNCHCHECKLIST);
    this.rocketDetailsChecklist.SetIcon(this.rocketDetailsChecklistIcon);
    ((Object) ((Component) this.rocketDetailsChecklist).gameObject).name = "rocketDetailsChecklist";
    this.rocketDetailsRange = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.rocketDetailsContainer);
    this.rocketDetailsRange.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.MAXRANGE);
    this.rocketDetailsRange.SetIcon(this.rocketDetailsRangeIcon);
    ((Object) ((Component) this.rocketDetailsRange).gameObject).name = "rocketDetailsRange";
    this.rocketDetailsMass = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.rocketDetailsContainer);
    this.rocketDetailsMass.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.MASS);
    this.rocketDetailsMass.SetIcon(this.rocketDetailsMassIcon);
    ((Object) ((Component) this.rocketDetailsMass).gameObject).name = "rocketDetailsMass";
    this.rocketThrustWidget = Object.Instantiate<RocketThrustWidget>(this.rocketThrustWidget, (Transform) this.rocketDetailsContainer);
    this.rocketDetailsStorage = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.rocketDetailsContainer);
    this.rocketDetailsStorage.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.STORAGE);
    this.rocketDetailsStorage.SetIcon(this.rocketDetailsStorageIcon);
    ((Object) ((Component) this.rocketDetailsStorage).gameObject).name = "rocketDetailsStorage";
    this.rocketDetailsFuel = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.rocketDetailsContainer);
    this.rocketDetailsFuel.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.FUEL);
    this.rocketDetailsFuel.SetIcon(this.rocketDetailsFuelIcon);
    ((Object) ((Component) this.rocketDetailsFuel).gameObject).name = "rocketDetailsFuel";
    this.rocketDetailsOxidizer = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.rocketDetailsContainer);
    this.rocketDetailsOxidizer.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.OXIDIZER);
    this.rocketDetailsOxidizer.SetIcon(this.rocketDetailsOxidizerIcon);
    ((Object) ((Component) this.rocketDetailsOxidizer).gameObject).name = "rocketDetailsOxidizer";
    this.rocketDetailsDupes = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.rocketDetailsContainer);
    this.rocketDetailsDupes.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.PASSENGERS);
    this.rocketDetailsDupes.SetIcon(this.rocketDetailsDupesIcon);
    ((Object) ((Component) this.rocketDetailsDupes).gameObject).name = "rocketDetailsDupes";
    this.destinationDetailsAnalysis = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.destinationDetailsContainer);
    this.destinationDetailsAnalysis.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.ANALYSIS);
    this.destinationDetailsAnalysis.SetIcon(this.destinationDetailsAnalysisIcon);
    ((Object) ((Component) this.destinationDetailsAnalysis).gameObject).name = "destinationDetailsAnalysis";
    this.destinationDetailsAnalysis.SetDescription(string.Format((string) STRINGS.UI.STARMAP.ANALYSIS_DESCRIPTION, (object) 0));
    this.destinationAnalysisProgressBar = Object.Instantiate<GameObject>(this.progressBarPrefab.gameObject, (Transform) this.destinationDetailsContainer).GetComponent<GenericUIProgressBar>();
    this.destinationAnalysisProgressBar.SetMaxValue((float) TUNING.ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
    this.destinationDetailsResearch = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.destinationDetailsContainer);
    this.destinationDetailsResearch.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.RESEARCH);
    this.destinationDetailsResearch.SetIcon(this.destinationDetailsResearchIcon);
    ((Object) ((Component) this.destinationDetailsResearch).gameObject).name = "destinationDetailsResearch";
    this.destinationDetailsResearch.SetDescription(string.Format((string) STRINGS.UI.STARMAP.RESEARCH_DESCRIPTION, (object) 0));
    this.destinationDetailsMass = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.destinationDetailsContainer);
    this.destinationDetailsMass.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.DESTINATION_MASS);
    this.destinationDetailsMass.SetIcon(this.destinationDetailsMassIcon);
    ((Object) ((Component) this.destinationDetailsMass).gameObject).name = "destinationDetailsMass";
    this.destinationDetailsComposition = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.destinationDetailsContainer);
    this.destinationDetailsComposition.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.WORLDCOMPOSITION);
    this.destinationDetailsComposition.SetIcon(this.destinationDetailsCompositionIcon);
    ((Object) ((Component) this.destinationDetailsComposition).gameObject).name = "destinationDetailsComposition";
    this.destinationDetailsResources = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.destinationDetailsContainer);
    this.destinationDetailsResources.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.RESOURCES);
    this.destinationDetailsResources.SetIcon(this.destinationDetailsResourcesIcon);
    ((Object) ((Component) this.destinationDetailsResources).gameObject).name = "destinationDetailsResources";
    this.destinationDetailsArtifacts = Object.Instantiate<BreakdownList>(this.breakdownListPrefab, (Transform) this.destinationDetailsContainer);
    this.destinationDetailsArtifacts.SetTitle((string) STRINGS.UI.STARMAP.LISTTITLES.ARTIFACTS);
    this.destinationDetailsArtifacts.SetIcon(this.destinationDetailsArtifactsIcon);
    ((Object) ((Component) this.destinationDetailsArtifacts).gameObject).name = "destinationDetailsArtifacts";
    this.LoadPlanets();
    this.selectionUpdateHandle = Game.Instance.Subscribe(-1503271301, new Action<object>(this.OnSelectableChanged));
    ((TMP_Text) this.titleBarLabel).text = (string) STRINGS.UI.STARMAP.TITLE;
    this.button.onClick += (System.Action) (() => ManagementMenu.Instance.ToggleStarmap());
    this.launchButton.play_sound_on_click = false;
    this.launchButton.onClick += (System.Action) (() =>
    {
      if (Object.op_Inequality((Object) this.currentLaunchConditionManager, (Object) null) && this.selectedDestination != null)
      {
        KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
        this.LaunchRocket(this.currentLaunchConditionManager);
      }
      else
        KFMOD.PlayUISound(GlobalAssets.GetSound("Negative"));
    });
    this.launchButton.ChangeState(1);
    this.showRocketsButton.onClick += (System.Action) (() => this.OnSelectableChanged((object) null));
    this.SelectDestination((SpaceDestination) null);
    SpacecraftManager.instance.Subscribe(532901469, (Action<object>) (data =>
    {
      this.RefreshAnalyzeButton();
      this.UpdateDestinationStates();
    }));
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (this.selectionUpdateHandle != -1)
      Game.Instance.Unsubscribe(this.selectionUpdateHandle);
    ((MonoBehaviour) this).StopAllCoroutines();
  }

  protected override void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
    {
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUStarmapSnapshot);
      MusicManager.instance.PlaySong("Music_Starmap");
      this.UpdateDestinationStates();
      this.Refresh();
    }
    else
    {
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUStarmapSnapshot);
      MusicManager.instance.StopSong("Music_Starmap");
    }
    this.OnSelectableChanged(Object.op_Equality((Object) SelectTool.Instance.selected, (Object) null) ? (object) (GameObject) null : (object) ((Component) SelectTool.Instance.selected).gameObject);
    this.forceScrollDown = true;
  }

  private void UpdateDestinationStates()
  {
    int num1 = 0;
    int num2 = 0;
    int num3 = 1;
    foreach (SpaceDestination destination in SpacecraftManager.instance.destinations)
    {
      num1 = Mathf.Max(num1, destination.OneBasedDistance);
      if (destination.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
        num2 = Mathf.Max(num2, destination.OneBasedDistance);
    }
    for (int index = num2; index < num1; ++index)
    {
      bool flag = false;
      foreach (SpaceDestination destination in SpacecraftManager.instance.destinations)
      {
        if (destination.distance == index)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        ++num3;
      else
        break;
    }
    foreach (KeyValuePair<SpaceDestination, StarmapPlanet> planetWidget in this.planetWidgets)
    {
      KeyValuePair<SpaceDestination, StarmapPlanet> KVP = planetWidget;
      SpaceDestination key = KVP.Key;
      StarmapPlanet planet = KVP.Value;
      Color color1;
      // ISSUE: explicit constructor call
      ((Color) ref color1).\u002Ector(0.25f, 0.25f, 0.25f, 0.5f);
      Color color2;
      // ISSUE: explicit constructor call
      ((Color) ref color2).\u002Ector(0.75f, 0.75f, 0.75f, 0.75f);
      if (KVP.Key.distance >= num2 + num3)
      {
        planet.SetUnknownBGActive(false, Color.white);
        planet.SetSprite(Assets.GetSprite(HashedString.op_Implicit("unknown_far")), color1);
      }
      else
      {
        planet.SetAnalysisActive(SpacecraftManager.instance.GetStarmapAnalysisDestinationID() == KVP.Key.id);
        bool flag = SpacecraftManager.instance.GetDestinationAnalysisState(key) == SpacecraftManager.DestinationAnalysisState.Complete;
        SpaceDestinationType destinationType = key.GetDestinationType();
        planet.SetLabel(flag ? destinationType.Name + "\n<color=#979798> " + GameUtil.GetFormattedDistance((float) ((double) KVP.Key.OneBasedDistance * 10000.0 * 1000.0)) + "</color>" : (string) STRINGS.UI.STARMAP.UNKNOWN_DESTINATION + "\n" + string.Format(STRINGS.UI.STARMAP.ANALYSIS_AMOUNT.text, (object) GameUtil.GetFormattedPercent((float) (100.0 * ((double) SpacecraftManager.instance.GetDestinationAnalysisScore(KVP.Key) / (double) TUNING.ROCKETRY.DESTINATION_ANALYSIS.COMPLETE)))));
        planet.SetSprite(flag ? Assets.GetSprite(HashedString.op_Implicit(destinationType.spriteName)) : Assets.GetSprite(HashedString.op_Implicit("unknown")), flag ? Color.white : color2);
        planet.SetUnknownBGActive(SpacecraftManager.instance.GetDestinationAnalysisState(KVP.Key) != SpacecraftManager.DestinationAnalysisState.Complete, color2);
        planet.SetFillAmount(SpacecraftManager.instance.GetDestinationAnalysisScore(KVP.Key) / (float) TUNING.ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
        List<int> spacecraftsForDestination = SpacecraftManager.instance.GetSpacecraftsForDestination(key);
        planet.SetRocketIcons(spacecraftsForDestination.Count, this.rocketIconPrefab);
        bool show = Object.op_Inequality((Object) this.currentLaunchConditionManager, (Object) null) && key == SpacecraftManager.instance.GetSpacecraftDestination(this.currentLaunchConditionManager);
        planet.ShowAsCurrentRocketDestination(show);
        planet.SetOnClick((System.Action) (() =>
        {
          if (Object.op_Equality((Object) this.currentLaunchConditionManager, (Object) null))
          {
            this.SelectDestination(KVP.Key);
          }
          else
          {
            if (SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.currentLaunchConditionManager).state != Spacecraft.MissionState.Grounded)
              return;
            this.SelectDestination(KVP.Key);
          }
        }));
        planet.SetOnEnter((System.Action) (() => planet.ShowLabel(true)));
        planet.SetOnExit((System.Action) (() => planet.ShowLabel(false)));
      }
    }
  }

  protected override void OnActivate()
  {
    base.OnActivate();
    StarmapScreen.Instance = this;
  }

  private string DisplayDistance(float distance) => Util.FormatWholeNumber(distance) + " " + (string) STRINGS.UI.UNITSUFFIXES.DISTANCE.KILOMETER;

  private string DisplayDestinationMass(SpaceDestination selectedDestination) => GameUtil.GetFormattedMass(selectedDestination.AvailableMass, massFormat: GameUtil.MetricMassFormat.Tonne);

  private string DisplayTotalStorageCapacity(CommandModule command)
  {
    float mass = 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) command).GetComponent<AttachableBuilding>()))
    {
      CargoBay component = gameObject.GetComponent<CargoBay>();
      if (Object.op_Inequality((Object) component, (Object) null))
        mass += component.storage.Capacity();
    }
    return GameUtil.GetFormattedMass(mass, massFormat: GameUtil.MetricMassFormat.Tonne);
  }

  private string StorageCapacityTooltip(CommandModule command, SpaceDestination dest)
  {
    string str = "";
    bool flag = dest != null && SpacecraftManager.instance.GetDestinationAnalysisState(dest) == SpacecraftManager.DestinationAnalysisState.Complete;
    if (dest != null && flag)
    {
      if ((double) dest.AvailableMass <= (double) ConditionHasMinimumMass.CargoCapacity(dest, command))
        str = str + (string) STRINGS.UI.STARMAP.LAUNCHCHECKLIST.INSUFFICENT_MASS_TOOLTIP + "\n\n";
      str = str + string.Format((string) STRINGS.UI.STARMAP.LAUNCHCHECKLIST.RESOURCE_MASS_TOOLTIP, (object) dest.GetDestinationType().Name, (object) GameUtil.GetFormattedMass(dest.AvailableMass, massFormat: GameUtil.MetricMassFormat.Kilogram), (object) GameUtil.GetFormattedMass(ConditionHasMinimumMass.CargoCapacity(dest, command), massFormat: GameUtil.MetricMassFormat.Kilogram)) + "\n\n";
    }
    float num1 = dest != null ? dest.AvailableMass : 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) command).GetComponent<AttachableBuilding>()))
    {
      CargoBay component = gameObject.GetComponent<CargoBay>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        if (flag)
        {
          float resourcesPercentage = dest.GetAvailableResourcesPercentage(component.storageType);
          float num2 = Mathf.Min(component.storage.Capacity(), resourcesPercentage * num1);
          num1 -= num2;
          str = str + ((Component) component).gameObject.GetProperName() + " " + string.Format((string) STRINGS.UI.STARMAP.STORAGESTATS.STORAGECAPACITY, (object) GameUtil.GetFormattedMass(Mathf.Min(num2, component.storage.Capacity()), massFormat: GameUtil.MetricMassFormat.Kilogram), (object) GameUtil.GetFormattedMass(component.storage.Capacity(), massFormat: GameUtil.MetricMassFormat.Kilogram)) + "\n";
        }
        else
          str = str + ((Component) component).gameObject.GetProperName() + " " + string.Format((string) STRINGS.UI.STARMAP.STORAGESTATS.STORAGECAPACITY, (object) GameUtil.GetFormattedMass(0.0f, massFormat: GameUtil.MetricMassFormat.Kilogram), (object) GameUtil.GetFormattedMass(component.storage.Capacity(), massFormat: GameUtil.MetricMassFormat.Kilogram)) + "\n";
      }
    }
    return str;
  }

  private void LoadPlanets()
  {
    foreach (SpaceDestination destination in Game.Instance.spacecraftManager.destinations)
    {
      if ((double) destination.OneBasedDistance * 10000.0 > (double) this.planetsMaxDistance)
        this.planetsMaxDistance = (float) destination.OneBasedDistance * 10000f;
      while (this.planetRows.Count < destination.distance + 1)
      {
        GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, ((Component) this.rowsContiner).gameObject, true);
        ((Transform) Util.rectTransform(gameObject)).SetAsFirstSibling();
        this.planetRows.Add(gameObject);
        ((Graphic) gameObject.GetComponentInChildren<Image>()).color = this.distanceColors[this.planetRows.Count % this.distanceColors.Length];
        ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).text = this.DisplayDistance((float) (this.planetRows.Count + 1) * 10000f);
      }
      GameObject gameObject1 = Util.KInstantiateUI(((Component) this.planetPrefab).gameObject, this.planetRows[destination.distance], true);
      this.planetWidgets.Add(destination, gameObject1.GetComponent<StarmapPlanet>());
    }
    this.UpdateDestinationStates();
  }

  private void UnselectAllPlanets()
  {
    if (this.animateSelectedPlanetRoutine != null)
      ((MonoBehaviour) this).StopCoroutine(this.animateSelectedPlanetRoutine);
    foreach (KeyValuePair<SpaceDestination, StarmapPlanet> planetWidget in this.planetWidgets)
    {
      planetWidget.Value.SetSelectionActive(false);
      planetWidget.Value.ShowAsCurrentRocketDestination(false);
    }
  }

  private void SelectPlanet(StarmapPlanet planet)
  {
    planet.SetSelectionActive(true);
    if (this.animateSelectedPlanetRoutine != null)
      ((MonoBehaviour) this).StopCoroutine(this.animateSelectedPlanetRoutine);
    this.animateSelectedPlanetRoutine = ((MonoBehaviour) this).StartCoroutine(this.AnimatePlanetSelection(planet));
  }

  private IEnumerator AnimatePlanetSelection(StarmapPlanet planet)
  {
    while (true)
    {
      planet.AnimateSelector(Time.unscaledTime);
      yield return (object) SequenceUtil.WaitForEndOfFrame;
    }
  }

  private void Update()
  {
    this.PositionPlanetWidgets();
    if (!this.forceScrollDown)
      return;
    this.ScrollToBottom();
    this.forceScrollDown = false;
  }

  private void ScrollToBottom()
  {
    RectTransform rectTransform1 = Util.rectTransform((Component) ((Component) this.Map).GetComponentInChildren<VerticalLayoutGroup>());
    RectTransform rectTransform2 = rectTransform1;
    double x = (double) ((Transform) rectTransform1).localPosition.x;
    Rect rect = rectTransform1.rect;
    double height1 = (double) ((Rect) ref rect).height;
    rect = this.Map.rect;
    double height2 = (double) ((Rect) ref rect).height;
    double num = height1 - height2;
    double z = (double) ((Transform) rectTransform1).localPosition.z;
    Vector3 vector3 = new Vector3((float) x, (float) num, (float) z);
    TransformExtensions.SetLocalPosition((Transform) rectTransform2, vector3);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (this.CheckBlockedInput())
    {
      if (((KInputEvent) e).Consumed)
        return;
      ((KInputEvent) e).Consumed = true;
    }
    else
      base.OnKeyDown(e);
  }

  private bool CheckBlockedInput()
  {
    if (Object.op_Inequality((Object) EventSystem.current, (Object) null))
    {
      GameObject selectedGameObject = EventSystem.current.currentSelectedGameObject;
      if (Object.op_Inequality((Object) selectedGameObject, (Object) null))
      {
        foreach (KeyValuePair<Spacecraft, HierarchyReferences> listRocketRow in this.listRocketRows)
        {
          EditableTitleBar component = ((Component) listRocketRow.Value.GetReference<RectTransform>("EditableTitle")).GetComponent<EditableTitleBar>();
          if (Object.op_Equality((Object) selectedGameObject, (Object) ((Component) component.inputField).gameObject))
            return true;
        }
      }
    }
    return false;
  }

  private void PositionPlanetWidgets()
  {
    Rect rect = this.rowPrefab.GetComponent<RectTransform>().rect;
    float num = ((Rect) ref rect).height / 2f;
    foreach (KeyValuePair<SpaceDestination, StarmapPlanet> planetWidget in this.planetWidgets)
      Util.rectTransform((Component) planetWidget.Value).anchoredPosition = new Vector2(Util.rectTransform((Component) planetWidget.Value.transform.parent).sizeDelta.x * planetWidget.Key.startingOrbitPercentage, -num);
  }

  private void OnSelectableChanged(object data)
  {
    if (!((Component) this).gameObject.activeSelf)
      return;
    if (this.rocketConditionEventHandler != -1)
      ((KMonoBehaviour) this).Unsubscribe(this.rocketConditionEventHandler);
    if (data != null)
    {
      this.currentSelectable = ((GameObject) data).GetComponent<KSelectable>();
      this.currentCommandModule = ((Component) this.currentSelectable).GetComponent<CommandModule>();
      this.currentLaunchConditionManager = ((Component) this.currentSelectable).GetComponent<LaunchConditionManager>();
      if (Object.op_Inequality((Object) this.currentCommandModule, (Object) null) && Object.op_Inequality((Object) this.currentLaunchConditionManager, (Object) null))
      {
        this.SelectDestination(SpacecraftManager.instance.GetSpacecraftDestination(this.currentLaunchConditionManager));
        this.rocketConditionEventHandler = this.currentLaunchConditionManager.Subscribe(1655598572, new Action<object>(this.Refresh));
        this.ShowRocketDetailsPanel();
      }
      else
      {
        this.currentSelectable = (KSelectable) null;
        this.currentCommandModule = (CommandModule) null;
        this.currentLaunchConditionManager = (LaunchConditionManager) null;
        this.ShowRocketListPanel();
      }
    }
    else
    {
      this.currentSelectable = (KSelectable) null;
      this.currentCommandModule = (CommandModule) null;
      this.currentLaunchConditionManager = (LaunchConditionManager) null;
      this.ShowRocketListPanel();
    }
    this.Refresh();
  }

  private void ShowRocketListPanel()
  {
    this.listPanel.SetActive(true);
    this.rocketPanel.SetActive(false);
    this.launchButton.ChangeState(1);
    this.UpdateDistanceOverlay();
    this.UpdateMissionOverlay();
  }

  private void ShowRocketDetailsPanel()
  {
    this.listPanel.SetActive(false);
    this.rocketPanel.SetActive(true);
    this.ValidateTravelAbility();
    this.UpdateDistanceOverlay();
    this.UpdateMissionOverlay();
  }

  private void LaunchRocket(LaunchConditionManager lcm)
  {
    SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(lcm);
    if (spacecraftDestination == null)
      return;
    lcm.Launch(spacecraftDestination);
    this.ClearRocketListPanel();
    this.FillRocketListPanel();
    this.ShowRocketListPanel();
    this.Refresh();
  }

  private void OnStartedTitlebarEditing()
  {
    this.isEditing = true;
    KScreenManager.Instance.RefreshStack();
  }

  private void OnEndEditing(string data) => this.isEditing = false;

  private void FillRocketListPanel()
  {
    this.ClearRocketListPanel();
    List<Spacecraft> spacecraft1 = SpacecraftManager.instance.GetSpacecraft();
    if (spacecraft1.Count == 0)
    {
      ((TMP_Text) this.listHeaderStatusLabel).text = (string) STRINGS.UI.STARMAP.NO_ROCKETS_TITLE;
      ((Component) this.listNoRocketText).gameObject.SetActive(true);
    }
    else
    {
      ((TMP_Text) this.listHeaderStatusLabel).text = string.Format((string) STRINGS.UI.STARMAP.ROCKET_COUNT, (object) spacecraft1.Count);
      ((Component) this.listNoRocketText).gameObject.SetActive(false);
    }
    foreach (Spacecraft spacecraft2 in spacecraft1)
    {
      Spacecraft rocket = spacecraft2;
      HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(((Component) this.listRocketTemplate).gameObject, ((Component) this.rocketListContainer).gameObject, true);
      BreakdownList component1 = ((Component) hierarchyReferences).GetComponent<BreakdownList>();
      MultiToggle component2 = ((Component) hierarchyReferences).GetComponent<MultiToggle>();
      EditableTitleBar component3 = ((Component) hierarchyReferences.GetReference<RectTransform>("EditableTitle")).GetComponent<EditableTitleBar>();
      component3.OnStartedEditing += new System.Action(this.OnStartedTitlebarEditing);
      // ISSUE: method pointer
      ((UnityEvent<string>) ((TMP_InputField) component3.inputField).onEndEdit).AddListener(new UnityAction<string>((object) this, __methodptr(OnEndEditing)));
      MultiToggle component4 = ((Component) hierarchyReferences.GetReference<RectTransform>("LaunchRocketButton")).GetComponent<MultiToggle>();
      MultiToggle component5 = ((Component) hierarchyReferences.GetReference<RectTransform>("LandRocketButton")).GetComponent<MultiToggle>();
      HierarchyReferences component6 = ((Component) hierarchyReferences.GetReference<RectTransform>("ProgressBar")).GetComponent<HierarchyReferences>();
      LaunchConditionManager launchConditionManager = rocket.launchConditions;
      CommandModule component7 = ((Component) launchConditionManager).GetComponent<CommandModule>();
      MinionStorage component8 = ((Component) launchConditionManager).GetComponent<MinionStorage>();
      component3.SetTitle(rocket.rocketName);
      component3.OnNameChanged += (Action<string>) (newName => rocket.SetRocketName(newName));
      component2.onEnter += (System.Action) (() =>
      {
        LaunchConditionManager launchConditions = rocket.launchConditions;
        this.UpdateDistanceOverlay(launchConditions);
        this.UpdateMissionOverlay(launchConditions);
      });
      component2.onExit += (System.Action) (() =>
      {
        this.UpdateDistanceOverlay();
        this.UpdateMissionOverlay();
      });
      component2.onClick += (System.Action) (() => this.OnSelectableChanged((object) ((Component) rocket.launchConditions).gameObject));
      component4.play_sound_on_click = false;
      component4.onClick += (System.Action) (() =>
      {
        if (Object.op_Inequality((Object) launchConditionManager, (Object) null))
        {
          KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
          this.LaunchRocket(launchConditionManager);
        }
        else
          KFMOD.PlayUISound(GlobalAssets.GetSound("Negative"));
      });
      if ((DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive) && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).state != Spacecraft.MissionState.Grounded)
      {
        ((Component) component5).gameObject.SetActive(true);
        component5.transform.SetAsLastSibling();
        component5.play_sound_on_click = false;
        component5.onClick += (System.Action) (() =>
        {
          if (Object.op_Inequality((Object) launchConditionManager, (Object) null))
          {
            KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
            SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(launchConditionManager).ForceComplete();
            this.ClearRocketListPanel();
            this.FillRocketListPanel();
            this.ShowRocketListPanel();
            this.Refresh();
          }
          else
            KFMOD.PlayUISound(GlobalAssets.GetSound("Negative"));
        });
      }
      else
        ((Component) component5).gameObject.SetActive(false);
      BreakdownListRow breakdownListRow1 = component1.AddRow();
      string grounded = (string) STRINGS.UI.STARMAP.MISSION_STATUS.GROUNDED;
      Tuple<string, BreakdownListRow.Status> textForState = StarmapScreen.GetTextForState(rocket.state, rocket);
      string first = textForState.first;
      BreakdownListRow.Status second = textForState.second;
      breakdownListRow1.ShowStatusData((string) STRINGS.UI.STARMAP.ROCKETSTATUS.STATUS, first, second);
      breakdownListRow1.SetHighlighted(true);
      if (Object.op_Inequality((Object) component8, (Object) null))
      {
        List<MinionStorage.Info> storedMinionInfo = component8.GetStoredMinionInfo();
        BreakdownListRow breakdownListRow2 = component1.AddRow();
        int count = storedMinionInfo.Count;
        string passengers = (string) STRINGS.UI.STARMAP.LISTTITLES.PASSENGERS;
        string str = count.ToString();
        int dotColor = count == 0 ? 1 : 2;
        breakdownListRow2.ShowStatusData(passengers, str, (BreakdownListRow.Status) dotColor);
      }
      if (rocket.state == Spacecraft.MissionState.Grounded)
      {
        string tooltipText = "";
        List<GameObject> attachedNetwork = AttachableBuilding.GetAttachedNetwork(((Component) launchConditionManager).GetComponent<AttachableBuilding>());
        foreach (GameObject go in attachedNetwork)
          tooltipText = tooltipText + go.GetProperName() + "\n";
        BreakdownListRow breakdownListRow3 = component1.AddRow();
        breakdownListRow3.ShowData((string) STRINGS.UI.STARMAP.LISTTITLES.MODULES, attachedNetwork.Count.ToString());
        breakdownListRow3.AddTooltip(tooltipText);
        component1.AddRow().ShowData((string) STRINGS.UI.STARMAP.LISTTITLES.MAXRANGE, this.DisplayDistance(component7.rocketStats.GetRocketMaxDistance()));
        BreakdownListRow breakdownListRow4 = component1.AddRow();
        breakdownListRow4.ShowData((string) STRINGS.UI.STARMAP.LISTTITLES.STORAGE, this.DisplayTotalStorageCapacity(component7));
        breakdownListRow4.AddTooltip(this.StorageCapacityTooltip(component7, this.selectedDestination));
        BreakdownListRow breakdownListRow5 = component1.AddRow();
        if (this.selectedDestination != null)
        {
          if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
          {
            bool flag = (double) this.selectedDestination.AvailableMass >= (double) ConditionHasMinimumMass.CargoCapacity(this.selectedDestination, component7);
            breakdownListRow5.ShowStatusData((string) STRINGS.UI.STARMAP.LISTTITLES.DESTINATION_MASS, this.DisplayDestinationMass(this.selectedDestination), flag ? BreakdownListRow.Status.Default : BreakdownListRow.Status.Yellow);
            breakdownListRow5.AddTooltip(this.StorageCapacityTooltip(component7, this.selectedDestination));
          }
          else
            breakdownListRow5.ShowStatusData((string) STRINGS.UI.STARMAP.LISTTITLES.DESTINATION_MASS, (string) STRINGS.UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT, BreakdownListRow.Status.Default);
        }
        else
        {
          breakdownListRow5.ShowStatusData((string) STRINGS.UI.STARMAP.DESTINATIONSELECTION.NOTSELECTED, "", BreakdownListRow.Status.Red);
          breakdownListRow5.AddTooltip((string) STRINGS.UI.STARMAP.DESTINATIONSELECTION_TOOLTIP.NOTSELECTED);
        }
        ((Transform) ((Component) component4).GetComponent<RectTransform>()).SetAsLastSibling();
        ((Component) component4).gameObject.SetActive(true);
        ((Component) component6).gameObject.SetActive(false);
      }
      else
      {
        float duration = rocket.GetDuration();
        float timeLeft = rocket.GetTimeLeft();
        float num = (double) duration == 0.0 ? 0.0f : (float) (1.0 - (double) timeLeft / (double) duration);
        component1.AddRow().ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATUS.TIMEREMAINING, ((double) rocket.controlStationBuffTimeRemaining <= 0.0 ? "" : STRINGS.UI.STARMAP.ROCKETSTATUS.BOOSTED_TIME_MODIFIER.text) + Util.FormatOneDecimalPlace(timeLeft / 600f) + " / " + GameUtil.GetFormattedCycles(duration));
        ((Component) component6).gameObject.SetActive(true);
        RectTransform reference = component6.GetReference<RectTransform>("ProgressImage");
        LocText component9 = ((Component) component6.GetReference<RectTransform>("ProgressText")).GetComponent<LocText>();
        ((Component) reference).transform.localScale = new Vector3(num, 1f, 1f);
        string formattedPercent = GameUtil.GetFormattedPercent(num * 100f);
        ((TMP_Text) component9).text = formattedPercent;
        ((Transform) ((Component) component6).GetComponent<RectTransform>()).SetAsLastSibling();
        ((Component) component4).gameObject.SetActive(false);
      }
      this.listRocketRows.Add(rocket, hierarchyReferences);
    }
    this.UpdateRocketRowsTravelAbility();
  }

  public static Tuple<string, BreakdownListRow.Status> GetTextForState(
    Spacecraft.MissionState state,
    Spacecraft spacecraft)
  {
    switch (state)
    {
      case Spacecraft.MissionState.Grounded:
        return new Tuple<string, BreakdownListRow.Status>((string) STRINGS.UI.STARMAP.MISSION_STATUS.GROUNDED, BreakdownListRow.Status.Green);
      case Spacecraft.MissionState.Launching:
        return new Tuple<string, BreakdownListRow.Status>((string) STRINGS.UI.STARMAP.MISSION_STATUS.LAUNCHING, BreakdownListRow.Status.Yellow);
      case Spacecraft.MissionState.Underway:
        return new Tuple<string, BreakdownListRow.Status>((double) spacecraft.controlStationBuffTimeRemaining <= 0.0 ? STRINGS.UI.STARMAP.MISSION_STATUS.UNDERWAY.text : STRINGS.UI.STARMAP.MISSION_STATUS.UNDERWAY_BOOSTED.text, BreakdownListRow.Status.Red);
      case Spacecraft.MissionState.WaitingToLand:
        return new Tuple<string, BreakdownListRow.Status>((string) STRINGS.UI.STARMAP.MISSION_STATUS.WAITING_TO_LAND, BreakdownListRow.Status.Yellow);
      case Spacecraft.MissionState.Landing:
        return new Tuple<string, BreakdownListRow.Status>((string) STRINGS.UI.STARMAP.MISSION_STATUS.LANDING, BreakdownListRow.Status.Yellow);
      default:
        return new Tuple<string, BreakdownListRow.Status>((string) STRINGS.UI.STARMAP.MISSION_STATUS.DESTROYED, BreakdownListRow.Status.Red);
    }
  }

  private void ClearRocketListPanel()
  {
    ((TMP_Text) this.listHeaderStatusLabel).text = (string) STRINGS.UI.STARMAP.NO_ROCKETS_TITLE;
    foreach (KeyValuePair<Spacecraft, HierarchyReferences> listRocketRow in this.listRocketRows)
      Object.Destroy((Object) ((Component) listRocketRow.Value).gameObject);
    this.listRocketRows.Clear();
  }

  private void FillChecklist(LaunchConditionManager launchConditionManager)
  {
    foreach (ProcessCondition launchCondition in launchConditionManager.GetLaunchConditionList())
    {
      BreakdownListRow breakdownListRow = this.rocketDetailsChecklist.AddRow();
      string statusMessage = launchCondition.GetStatusMessage(ProcessCondition.Status.Ready);
      ProcessCondition.Status condition = launchCondition.EvaluateCondition();
      BreakdownListRow.Status status = BreakdownListRow.Status.Green;
      switch (condition)
      {
        case ProcessCondition.Status.Failure:
          status = BreakdownListRow.Status.Red;
          break;
        case ProcessCondition.Status.Warning:
          status = BreakdownListRow.Status.Yellow;
          break;
      }
      breakdownListRow.ShowCheckmarkData(statusMessage, "", status);
      if (condition != ProcessCondition.Status.Ready)
        breakdownListRow.SetHighlighted(true);
      breakdownListRow.AddTooltip(launchCondition.GetStatusTooltip(condition));
    }
  }

  private void SelectDestination(SpaceDestination destination)
  {
    this.selectedDestination = destination;
    this.UnselectAllPlanets();
    if (this.selectedDestination != null)
    {
      this.SelectPlanet(this.planetWidgets[this.selectedDestination]);
      if (Object.op_Inequality((Object) this.currentLaunchConditionManager, (Object) null))
        SpacecraftManager.instance.SetSpacecraftDestination(this.currentLaunchConditionManager, this.selectedDestination);
      this.ShowDestinationPanel();
      this.UpdateRocketRowsTravelAbility();
    }
    else
      this.ClearDestinationPanel();
    if (Object.op_Inequality((Object) this.rangeRowTotal, (Object) null) && this.selectedDestination != null && Object.op_Inequality((Object) this.currentCommandModule, (Object) null))
      this.rangeRowTotal.SetStatusColor(this.currentCommandModule.conditions.reachable.CanReachSpacecraftDestination(this.selectedDestination) ? BreakdownListRow.Status.Green : BreakdownListRow.Status.Red);
    this.UpdateDestinationStates();
    this.Refresh();
  }

  private void UpdateRocketRowsTravelAbility()
  {
    foreach (KeyValuePair<Spacecraft, HierarchyReferences> listRocketRow in this.listRocketRows)
    {
      Spacecraft key = listRocketRow.Key;
      LaunchConditionManager launchConditions = key.launchConditions;
      CommandModule component1 = ((Component) launchConditions).GetComponent<CommandModule>();
      MultiToggle component2 = ((Component) listRocketRow.Value.GetReference<RectTransform>("LaunchRocketButton")).GetComponent<MultiToggle>();
      bool flag1 = key.state == Spacecraft.MissionState.Grounded;
      SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(launchConditions);
      bool flag2 = spacecraftDestination != null && component1.conditions.reachable.CanReachSpacecraftDestination(spacecraftDestination);
      bool launch = launchConditions.CheckReadyToLaunch();
      component2.ChangeState(flag1 & flag2 & launch ? 0 : 1);
    }
  }

  private void RefreshAnalyzeButton()
  {
    if (this.selectedDestination == null)
    {
      this.analyzeButton.ChangeState(1);
      this.analyzeButton.onClick = (System.Action) null;
      ((TMP_Text) ((Component) this.analyzeButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.STARMAP.NO_ANALYZABLE_DESTINATION_SELECTED;
    }
    else if (this.selectedDestination.AnalysisState() == SpacecraftManager.DestinationAnalysisState.Complete)
    {
      if (DebugHandler.InstantBuildMode)
      {
        this.analyzeButton.ChangeState(0);
        this.analyzeButton.onClick = (System.Action) (() =>
        {
          this.selectedDestination.TryCompleteResearchOpportunity();
          this.ShowDestinationPanel();
        });
        ((TMP_Text) ((Component) this.analyzeButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.STARMAP.ANALYSIS_COMPLETE + " (debug research)";
      }
      else
      {
        this.analyzeButton.ChangeState(1);
        this.analyzeButton.onClick = (System.Action) null;
        ((TMP_Text) ((Component) this.analyzeButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.STARMAP.ANALYSIS_COMPLETE;
      }
    }
    else
    {
      this.analyzeButton.ChangeState(0);
      if (this.selectedDestination.id == SpacecraftManager.instance.GetStarmapAnalysisDestinationID())
      {
        ((TMP_Text) ((Component) this.analyzeButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.STARMAP.SUSPEND_DESTINATION_ANALYSIS;
        this.analyzeButton.onClick = (System.Action) (() => SpacecraftManager.instance.SetStarmapAnalysisDestinationID(-1));
      }
      else
      {
        ((TMP_Text) ((Component) this.analyzeButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.STARMAP.ANALYZE_DESTINATION;
        this.analyzeButton.onClick = (System.Action) (() =>
        {
          if (DebugHandler.InstantBuildMode)
          {
            SpacecraftManager.instance.SetStarmapAnalysisDestinationID(this.selectedDestination.id);
            SpacecraftManager.instance.EarnDestinationAnalysisPoints(this.selectedDestination.id, 99999f);
            this.ShowDestinationPanel();
          }
          else
            SpacecraftManager.instance.SetStarmapAnalysisDestinationID(this.selectedDestination.id);
        });
      }
    }
  }

  private void Refresh(object data = null)
  {
    this.FillRocketListPanel();
    this.RefreshAnalyzeButton();
    this.ShowDestinationPanel();
    if (Object.op_Inequality((Object) this.currentCommandModule, (Object) null) && Object.op_Inequality((Object) this.currentLaunchConditionManager, (Object) null))
    {
      this.FillRocketPanel();
      if (this.selectedDestination == null)
        return;
      this.ValidateTravelAbility();
    }
    else
      this.ClearRocketPanel();
  }

  private void ClearRocketPanel()
  {
    ((TMP_Text) this.rocketHeaderStatusLabel).text = (string) STRINGS.UI.STARMAP.ROCKETSTATUS.NONE;
    this.rocketDetailsChecklist.ClearRows();
    this.rocketDetailsMass.ClearRows();
    this.rocketDetailsRange.ClearRows();
    ((Component) this.rocketThrustWidget).gameObject.SetActive(false);
    this.rocketDetailsStorage.ClearRows();
    this.rocketDetailsFuel.ClearRows();
    this.rocketDetailsOxidizer.ClearRows();
    this.rocketDetailsDupes.ClearRows();
    this.rocketDetailsStatus.ClearRows();
    this.currentRocketHasLiquidContainer = false;
    this.currentRocketHasGasContainer = false;
    this.currentRocketHasSolidContainer = false;
    this.currentRocketHasEntitiesContainer = false;
    LayoutRebuilder.ForceRebuildLayoutImmediate(this.rocketDetailsContainer);
  }

  private void FillRocketPanel()
  {
    this.ClearRocketPanel();
    ((TMP_Text) this.rocketHeaderStatusLabel).text = (string) STRINGS.UI.STARMAP.STATUS;
    this.UpdateDistanceOverlay();
    this.UpdateMissionOverlay();
    this.FillChecklist(this.currentLaunchConditionManager);
    this.UpdateRangeDisplay();
    this.UpdateMassDisplay();
    this.UpdateOxidizerDisplay();
    this.UpdateStorageDisplay();
    this.UpdateFuelDisplay();
    LayoutRebuilder.ForceRebuildLayoutImmediate(this.rocketDetailsContainer);
  }

  private void UpdateRangeDisplay()
  {
    this.rocketDetailsRange.AddRow().ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZABLE_FUEL, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalOxidizableFuel()));
    this.rocketDetailsRange.AddRow().ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATS.ENGINE_EFFICIENCY, GameUtil.GetFormattedEngineEfficiency(this.currentCommandModule.rocketStats.GetEngineEfficiency()));
    this.rocketDetailsRange.AddRow().ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATS.OXIDIZER_EFFICIENCY, GameUtil.GetFormattedPercent(this.currentCommandModule.rocketStats.GetAverageOxidizerEfficiency()));
    float meters = this.currentCommandModule.rocketStats.GetBoosterThrust() * 1000f;
    if ((double) meters != 0.0)
      this.rocketDetailsRange.AddRow().ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATS.SOLID_BOOSTER, GameUtil.GetFormattedDistance(meters));
    BreakdownListRow breakdownListRow1 = this.rocketDetailsRange.AddRow();
    breakdownListRow1.ShowStatusData((string) STRINGS.UI.STARMAP.ROCKETSTATS.TOTAL_THRUST, GameUtil.GetFormattedDistance(this.currentCommandModule.rocketStats.GetTotalThrust() * 1000f), BreakdownListRow.Status.Green);
    breakdownListRow1.SetImportant(true);
    float distance = (float) -((double) this.currentCommandModule.rocketStats.GetTotalThrust() - (double) this.currentCommandModule.rocketStats.GetRocketMaxDistance());
    ((Component) this.rocketThrustWidget).gameObject.SetActive(true);
    BreakdownListRow breakdownListRow2 = this.rocketDetailsRange.AddRow();
    breakdownListRow2.ShowStatusData((string) STRINGS.UI.STARMAP.ROCKETSTATUS.WEIGHTPENALTY, this.DisplayDistance(distance), BreakdownListRow.Status.Red);
    breakdownListRow2.SetHighlighted(true);
    this.rocketDetailsRange.AddCustomRow(((Component) this.rocketThrustWidget).gameObject);
    this.rocketThrustWidget.Draw(this.currentCommandModule);
    BreakdownListRow breakdownListRow3 = this.rocketDetailsRange.AddRow();
    breakdownListRow3.ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATS.TOTAL_RANGE, GameUtil.GetFormattedDistance(this.currentCommandModule.rocketStats.GetRocketMaxDistance() * 1000f));
    breakdownListRow3.SetImportant(true);
  }

  private void UpdateMassDisplay()
  {
    this.rocketDetailsMass.AddRow().ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATS.DRY_MASS, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetDryMass(), massFormat: GameUtil.MetricMassFormat.Tonne));
    this.rocketDetailsMass.AddRow().ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATS.WET_MASS, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetWetMass(), massFormat: GameUtil.MetricMassFormat.Tonne));
    BreakdownListRow breakdownListRow = this.rocketDetailsMass.AddRow();
    breakdownListRow.ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATUS.TOTAL, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalMass(), massFormat: GameUtil.MetricMassFormat.Tonne));
    breakdownListRow.SetImportant(true);
  }

  private void UpdateFuelDisplay()
  {
    Tag engineFuelTag = this.currentCommandModule.rocketStats.GetEngineFuelTag();
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.currentCommandModule).GetComponent<AttachableBuilding>()))
    {
      IFuelTank component1 = gameObject.GetComponent<IFuelTank>();
      if (!Util.IsNullOrDestroyed((object) component1))
      {
        BreakdownListRow breakdownListRow = this.rocketDetailsFuel.AddRow();
        if (((Tag) ref engineFuelTag).IsValid)
        {
          Element element = ElementLoader.GetElement(engineFuelTag);
          Debug.Assert(element != null, (object) "fuel_element");
          breakdownListRow.ShowData(gameObject.gameObject.GetProperName() + " (" + element.name + ")", GameUtil.GetFormattedMass(component1.Storage.GetAmountAvailable(engineFuelTag), massFormat: GameUtil.MetricMassFormat.Tonne));
        }
        else
        {
          breakdownListRow.ShowData(gameObject.gameObject.GetProperName(), (string) STRINGS.UI.STARMAP.ROCKETSTATS.NO_ENGINE);
          breakdownListRow.SetStatusColor(BreakdownListRow.Status.Red);
        }
      }
      SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
      if (Object.op_Inequality((Object) component2, (Object) null))
      {
        BreakdownListRow breakdownListRow = this.rocketDetailsFuel.AddRow();
        Element element = ElementLoader.GetElement(component2.fuelTag);
        Debug.Assert(element != null, (object) "fuel_element");
        string name = gameObject.gameObject.GetProperName() + " (" + element.name + ")";
        string formattedMass = GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(component2.fuelTag), massFormat: GameUtil.MetricMassFormat.Tonne);
        breakdownListRow.ShowData(name, formattedMass);
      }
    }
    BreakdownListRow breakdownListRow1 = this.rocketDetailsFuel.AddRow();
    breakdownListRow1.ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATS.TOTAL_FUEL, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalFuel(true), massFormat: GameUtil.MetricMassFormat.Tonne));
    breakdownListRow1.SetImportant(true);
  }

  private void UpdateOxidizerDisplay()
  {
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.currentCommandModule).GetComponent<AttachableBuilding>()))
    {
      OxidizerTank component1 = gameObject.GetComponent<OxidizerTank>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        foreach (KeyValuePair<Tag, float> keyValuePair in component1.GetOxidizersAvailable())
        {
          if ((double) keyValuePair.Value != 0.0)
            this.rocketDetailsOxidizer.AddRow().ShowData(gameObject.gameObject.GetProperName() + " (" + keyValuePair.Key.ProperName() + ")", GameUtil.GetFormattedMass(keyValuePair.Value, massFormat: GameUtil.MetricMassFormat.Tonne));
        }
      }
      SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        this.rocketDetailsOxidizer.AddRow().ShowData(gameObject.gameObject.GetProperName() + " (" + ElementLoader.FindElementByHash(SimHashes.OxyRock).name + ")", GameUtil.GetFormattedMass(component2.fuelStorage.GetMassAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag), massFormat: GameUtil.MetricMassFormat.Tonne));
    }
    BreakdownListRow breakdownListRow = this.rocketDetailsOxidizer.AddRow();
    breakdownListRow.ShowData((string) STRINGS.UI.STARMAP.ROCKETSTATS.TOTAL_OXIDIZER, GameUtil.GetFormattedMass(this.currentCommandModule.rocketStats.GetTotalOxidizer(true), massFormat: GameUtil.MetricMassFormat.Tonne));
    breakdownListRow.SetImportant(true);
  }

  private void UpdateStorageDisplay()
  {
    float num1 = this.selectedDestination != null ? this.selectedDestination.AvailableMass : 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.currentCommandModule).GetComponent<AttachableBuilding>()))
    {
      CargoBay component = gameObject.GetComponent<CargoBay>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        BreakdownListRow breakdownListRow = this.rocketDetailsStorage.AddRow();
        if (this.selectedDestination != null)
        {
          float resourcesPercentage = this.selectedDestination.GetAvailableResourcesPercentage(component.storageType);
          float num2 = Mathf.Min(component.storage.Capacity(), resourcesPercentage * num1);
          num1 -= num2;
          breakdownListRow.ShowData(gameObject.gameObject.GetProperName(), string.Format((string) STRINGS.UI.STARMAP.STORAGESTATS.STORAGECAPACITY, (object) GameUtil.GetFormattedMass(Mathf.Min(num2, component.storage.Capacity()), massFormat: GameUtil.MetricMassFormat.Kilogram), (object) GameUtil.GetFormattedMass(component.storage.Capacity(), massFormat: GameUtil.MetricMassFormat.Kilogram)));
        }
        else
          breakdownListRow.ShowData(gameObject.gameObject.GetProperName(), string.Format((string) STRINGS.UI.STARMAP.STORAGESTATS.STORAGECAPACITY, (object) GameUtil.GetFormattedMass(0.0f, massFormat: GameUtil.MetricMassFormat.Kilogram), (object) GameUtil.GetFormattedMass(component.storage.Capacity(), massFormat: GameUtil.MetricMassFormat.Kilogram)));
      }
    }
  }

  private void ClearDestinationPanel()
  {
    ((Component) this.destinationDetailsContainer).gameObject.SetActive(false);
    ((TMP_Text) this.destinationStatusLabel).text = (string) STRINGS.UI.STARMAP.ROCKETSTATUS.NONE;
  }

  private void ShowDestinationPanel()
  {
    if (this.selectedDestination == null)
      return;
    ((TMP_Text) this.destinationStatusLabel).text = (string) STRINGS.UI.STARMAP.ROCKETSTATUS.SELECTED;
    if (Object.op_Inequality((Object) this.currentLaunchConditionManager, (Object) null) && SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this.currentLaunchConditionManager).state != Spacecraft.MissionState.Grounded)
      ((TMP_Text) this.destinationStatusLabel).text = (string) STRINGS.UI.STARMAP.ROCKETSTATUS.LOCKEDIN;
    SpaceDestinationType destinationType = this.selectedDestination.GetDestinationType();
    ((TMP_Text) this.destinationNameLabel).text = SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete ? destinationType.Name : STRINGS.UI.STARMAP.UNKNOWN_DESTINATION.text;
    ((TMP_Text) this.destinationTypeValueLabel).text = SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete ? destinationType.typeName : STRINGS.UI.STARMAP.UNKNOWN_TYPE.text;
    ((TMP_Text) this.destinationDistanceValueLabel).text = this.DisplayDistance((float) this.selectedDestination.OneBasedDistance * 10000f);
    ((TMP_Text) this.destinationDescriptionLabel).text = destinationType.description;
    this.destinationDetailsComposition.ClearRows();
    this.destinationDetailsResearch.ClearRows();
    this.destinationDetailsMass.ClearRows();
    this.destinationDetailsResources.ClearRows();
    this.destinationDetailsArtifacts.ClearRows();
    if (destinationType.visitable)
    {
      float num = 0.0f;
      if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
        num = this.selectedDestination.GetTotalMass();
      if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
      {
        foreach (SpaceDestination.ResearchOpportunity researchOpportunity in this.selectedDestination.researchOpportunities)
          this.destinationDetailsResearch.AddRow().ShowCheckmarkData(researchOpportunity.discoveredRareResource != SimHashes.Void ? string.Format("(!!) {0}", (object) researchOpportunity.description) : researchOpportunity.description, researchOpportunity.dataValue.ToString(), researchOpportunity.completed ? BreakdownListRow.Status.Green : BreakdownListRow.Status.Default);
      }
      this.destinationAnalysisProgressBar.SetFillPercentage(SpacecraftManager.instance.GetDestinationAnalysisScore(this.selectedDestination.id) / (float) TUNING.ROCKETRY.DESTINATION_ANALYSIS.COMPLETE);
      float mass = ConditionHasMinimumMass.CargoCapacity(this.selectedDestination, this.currentCommandModule);
      if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
      {
        string formattedMass1 = GameUtil.GetFormattedMass(this.selectedDestination.CurrentMass, massFormat: GameUtil.MetricMassFormat.Tonne);
        string formattedMass2 = GameUtil.GetFormattedMass((float) destinationType.minimumMass, massFormat: GameUtil.MetricMassFormat.Tonne);
        BreakdownListRow breakdownListRow1 = this.destinationDetailsMass.AddRow();
        breakdownListRow1.ShowData((string) STRINGS.UI.STARMAP.CURRENT_MASS, formattedMass1);
        if ((double) this.selectedDestination.AvailableMass < (double) mass)
        {
          breakdownListRow1.SetStatusColor(BreakdownListRow.Status.Yellow);
          breakdownListRow1.AddTooltip(string.Format((string) STRINGS.UI.STARMAP.CURRENT_MASS_TOOLTIP, (object) GameUtil.GetFormattedMass(this.selectedDestination.AvailableMass, massFormat: GameUtil.MetricMassFormat.Kilogram), (object) GameUtil.GetFormattedMass(mass, massFormat: GameUtil.MetricMassFormat.Kilogram)));
        }
        this.destinationDetailsMass.AddRow().ShowData((string) STRINGS.UI.STARMAP.MAXIMUM_MASS, GameUtil.GetFormattedMass((float) destinationType.maxiumMass, massFormat: GameUtil.MetricMassFormat.Tonne));
        BreakdownListRow breakdownListRow2 = this.destinationDetailsMass.AddRow();
        breakdownListRow2.ShowData((string) STRINGS.UI.STARMAP.MINIMUM_MASS, formattedMass2);
        breakdownListRow2.AddTooltip((string) STRINGS.UI.STARMAP.MINIMUM_MASS_TOOLTIP);
        BreakdownListRow breakdownListRow3 = this.destinationDetailsMass.AddRow();
        breakdownListRow3.ShowData((string) STRINGS.UI.STARMAP.REPLENISH_RATE, GameUtil.GetFormattedMass(destinationType.replishmentPerCycle, massFormat: GameUtil.MetricMassFormat.Kilogram));
        breakdownListRow3.AddTooltip((string) STRINGS.UI.STARMAP.REPLENISH_RATE_TOOLTIP);
      }
      if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
      {
        foreach (KeyValuePair<SimHashes, float> recoverableElement in this.selectedDestination.recoverableElements)
        {
          BreakdownListRow breakdownListRow = this.destinationDetailsComposition.AddRow();
          float percent = (float) ((double) this.selectedDestination.GetResourceValue(recoverableElement.Key, recoverableElement.Value) / (double) num * 100.0);
          Element elementByHash = ElementLoader.FindElementByHash(recoverableElement.Key);
          Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) elementByHash);
          if ((double) percent <= 1.0)
            breakdownListRow.ShowIconData(elementByHash.name, (string) STRINGS.UI.STARMAP.COMPOSITION_SMALL_AMOUNT, uiSprite.first, uiSprite.second);
          else
            breakdownListRow.ShowIconData(elementByHash.name, GameUtil.GetFormattedPercent(percent), uiSprite.first, uiSprite.second);
          if (elementByHash.IsGas)
          {
            string properName = Assets.GetPrefab(TagExtensions.ToTag("GasCargoBay")).GetProperName();
            if (this.currentRocketHasGasContainer)
            {
              breakdownListRow.SetHighlighted(true);
              breakdownListRow.AddTooltip(string.Format((string) STRINGS.UI.STARMAP.CAN_CARRY_ELEMENT, (object) elementByHash.name, (object) properName));
            }
            else
            {
              breakdownListRow.SetDisabled(true);
              breakdownListRow.AddTooltip(string.Format((string) STRINGS.UI.STARMAP.CONTAINER_REQUIRED, (object) properName));
            }
          }
          if (elementByHash.IsLiquid)
          {
            string properName = Assets.GetPrefab(TagExtensions.ToTag("LiquidCargoBay")).GetProperName();
            if (this.currentRocketHasLiquidContainer)
            {
              breakdownListRow.SetHighlighted(true);
              breakdownListRow.AddTooltip(string.Format((string) STRINGS.UI.STARMAP.CAN_CARRY_ELEMENT, (object) elementByHash.name, (object) properName));
            }
            else
            {
              breakdownListRow.SetDisabled(true);
              breakdownListRow.AddTooltip(string.Format((string) STRINGS.UI.STARMAP.CONTAINER_REQUIRED, (object) properName));
            }
          }
          if (elementByHash.IsSolid)
          {
            string properName = Assets.GetPrefab(TagExtensions.ToTag("CargoBay")).GetProperName();
            if (this.currentRocketHasSolidContainer)
            {
              breakdownListRow.SetHighlighted(true);
              breakdownListRow.AddTooltip(string.Format((string) STRINGS.UI.STARMAP.CAN_CARRY_ELEMENT, (object) elementByHash.name, (object) properName));
            }
            else
            {
              breakdownListRow.SetDisabled(true);
              breakdownListRow.AddTooltip(string.Format((string) STRINGS.UI.STARMAP.CONTAINER_REQUIRED, (object) properName));
            }
          }
        }
        foreach (SpaceDestination.ResearchOpportunity researchOpportunity in this.selectedDestination.researchOpportunities)
        {
          if (!researchOpportunity.completed && researchOpportunity.discoveredRareResource != SimHashes.Void)
          {
            BreakdownListRow breakdownListRow = this.destinationDetailsComposition.AddRow();
            breakdownListRow.ShowData((string) STRINGS.UI.STARMAP.COMPOSITION_UNDISCOVERED, (string) STRINGS.UI.STARMAP.COMPOSITION_UNDISCOVERED_AMOUNT);
            breakdownListRow.SetDisabled(true);
            breakdownListRow.AddTooltip((string) STRINGS.UI.STARMAP.COMPOSITION_UNDISCOVERED_TOOLTIP);
          }
        }
      }
      if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
      {
        foreach (KeyValuePair<Tag, int> recoverableEntity in this.selectedDestination.GetRecoverableEntities())
        {
          BreakdownListRow breakdownListRow = this.destinationDetailsResources.AddRow();
          GameObject prefab = Assets.GetPrefab(recoverableEntity.Key);
          Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) prefab);
          breakdownListRow.ShowIconData(prefab.GetProperName(), "", uiSprite.first, uiSprite.second);
          string properName = Assets.GetPrefab(TagExtensions.ToTag("SpecialCargoBay")).GetProperName();
          if (this.currentRocketHasEntitiesContainer)
          {
            breakdownListRow.SetHighlighted(true);
            breakdownListRow.AddTooltip(string.Format((string) STRINGS.UI.STARMAP.CAN_CARRY_ELEMENT, (object) prefab.GetProperName(), (object) properName));
          }
          else
          {
            breakdownListRow.SetDisabled(true);
            breakdownListRow.AddTooltip(string.Format((string) STRINGS.UI.STARMAP.CANT_CARRY_ELEMENT, (object) properName, (object) prefab.GetProperName()));
          }
        }
      }
      if (SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) == SpacecraftManager.DestinationAnalysisState.Complete)
      {
        ArtifactDropRate artifactDropTable = this.selectedDestination.GetDestinationType().artifactDropTable;
        foreach (Tuple<ArtifactTier, float> rate in artifactDropTable.rates)
          this.destinationDetailsArtifacts.AddRow().ShowData(StringEntry.op_Implicit(Strings.Get(rate.first.name_key)), GameUtil.GetFormattedPercent((float) ((double) rate.second / (double) artifactDropTable.totalWeight * 100.0)));
      }
      ((Component) this.destinationDetailsContainer).gameObject.SetActive(true);
    }
    LayoutRebuilder.ForceRebuildLayoutImmediate(this.destinationDetailsContainer);
  }

  private void ValidateTravelAbility()
  {
    if (this.selectedDestination == null || SpacecraftManager.instance.GetDestinationAnalysisState(this.selectedDestination) != SpacecraftManager.DestinationAnalysisState.Complete || !Object.op_Inequality((Object) this.currentCommandModule, (Object) null) || !Object.op_Inequality((Object) this.currentLaunchConditionManager, (Object) null))
      return;
    this.launchButton.ChangeState(this.currentLaunchConditionManager.CheckReadyToLaunch() ? 0 : 1);
  }

  private void UpdateDistanceOverlay(LaunchConditionManager lcmToVisualize = null)
  {
    if (Object.op_Equality((Object) lcmToVisualize, (Object) null))
      lcmToVisualize = this.currentLaunchConditionManager;
    Spacecraft spacecraft = (Spacecraft) null;
    if (Object.op_Inequality((Object) lcmToVisualize, (Object) null))
      spacecraft = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(lcmToVisualize);
    if (Object.op_Inequality((Object) lcmToVisualize, (Object) null) && spacecraft != null && spacecraft.state == Spacecraft.MissionState.Grounded)
    {
      ((Component) this.distanceOverlay).gameObject.SetActive(true);
      float num1 = (float) (int) ((double) ((Component) lcmToVisualize).GetComponent<CommandModule>().rocketStats.GetRocketMaxDistance() / 10000.0) * 10000f;
      Vector2 sizeDelta = ((Graphic) this.distanceOverlay).rectTransform.sizeDelta;
      ref Vector2 local1 = ref sizeDelta;
      Rect rect1 = this.rowsContiner.rect;
      double width = (double) ((Rect) ref rect1).width;
      local1.x = (float) width;
      ref Vector2 local2 = ref sizeDelta;
      double num2 = 1.0 - (double) num1 / (double) this.planetsMaxDistance;
      Rect rect2 = this.rowsContiner.rect;
      double height = (double) ((Rect) ref rect2).height;
      double num3 = num2 * height + (double) this.distanceOverlayYOffset + (double) this.distanceOverlayVerticalOffset;
      local2.y = (float) num3;
      ((Graphic) this.distanceOverlay).rectTransform.sizeDelta = sizeDelta;
      ((Graphic) this.distanceOverlay).rectTransform.anchoredPosition = Vector2.op_Implicit(new Vector3(0.0f, (float) this.distanceOverlayVerticalOffset, 0.0f));
    }
    else
      ((Component) this.distanceOverlay).gameObject.SetActive(false);
  }

  private void UpdateMissionOverlay(LaunchConditionManager lcmToVisualize = null)
  {
    if (Object.op_Equality((Object) lcmToVisualize, (Object) null))
      lcmToVisualize = this.currentLaunchConditionManager;
    Spacecraft spacecraft = (Spacecraft) null;
    if (Object.op_Inequality((Object) lcmToVisualize, (Object) null))
      spacecraft = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(lcmToVisualize);
    if (Object.op_Inequality((Object) lcmToVisualize, (Object) null) && spacecraft != null)
    {
      SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(lcmToVisualize);
      if (spacecraftDestination == null)
      {
        Debug.Log((object) "destination is null");
      }
      else
      {
        StarmapPlanet planetWidget = this.planetWidgets[spacecraftDestination];
        if (spacecraft == null)
          Debug.Log((object) "craft is null");
        else if (Object.op_Equality((Object) planetWidget, (Object) null))
        {
          Debug.Log((object) "planet is null");
        }
        else
        {
          this.UnselectAllPlanets();
          this.SelectPlanet(planetWidget);
          planetWidget.ShowAsCurrentRocketDestination(spacecraftDestination.GetDestinationType().visitable);
          if (spacecraft.state == Spacecraft.MissionState.Grounded || !spacecraftDestination.GetDestinationType().visitable)
            return;
          ((Component) this.visualizeRocketImage).gameObject.SetActive(true);
          ((Component) this.visualizeRocketTrajectory).gameObject.SetActive(true);
          ((Component) this.visualizeRocketLabel).gameObject.SetActive(true);
          ((Component) this.visualizeRocketProgress).gameObject.SetActive(true);
          float duration = spacecraft.GetDuration();
          float timeLeft = spacecraft.GetTimeLeft();
          float num1 = (double) duration == 0.0 ? 0.0f : (float) (1.0 - (double) timeLeft / (double) duration);
          int num2 = (double) num1 > 0.5 ? 1 : 0;
          Vector2 vector2_1;
          ref Vector2 local = ref vector2_1;
          Rect rect = this.rowsContiner.rect;
          double num3 = -(double) ((Rect) ref rect).size.y;
          // ISSUE: explicit constructor call
          ((Vector2) ref local).\u002Ector(0.0f, (float) num3);
          Vector3 vector3_1 = Vector3.op_Addition(((Transform) Util.rectTransform((Component) planetWidget)).localPosition, new Vector3(Util.rectTransform((Component) planetWidget).sizeDelta.x * 0.5f, 0.0f, 0.0f));
          Vector3 vector3_2 = Vector3.op_Addition(((Transform) Util.rectTransform((Component) planetWidget.transform.parent)).localPosition, vector3_1);
          Vector2 vector2_2;
          // ISSUE: explicit constructor call
          ((Vector2) ref vector2_2).\u002Ector(vector3_2.x, vector3_2.y);
          float num4 = Vector2.Distance(vector2_1, vector2_2);
          Vector2 vector2_3 = Vector2.op_Subtraction(vector2_2, vector2_1);
          float num5 = Mathf.Atan2(vector2_3.y, vector2_3.x) * 57.29578f;
          Vector2 vector2_4;
          if (num2 != 0)
          {
            // ISSUE: explicit constructor call
            ((Vector2) ref vector2_4).\u002Ector(Mathf.Lerp(vector2_1.x, vector2_2.x, (float) (1.0 - (double) num1 * 2.0 + 1.0)), Mathf.Lerp(vector2_1.y, vector2_2.y, (float) (1.0 - (double) num1 * 2.0 + 1.0)));
          }
          else
          {
            // ISSUE: explicit constructor call
            ((Vector2) ref vector2_4).\u002Ector(Mathf.Lerp(vector2_1.x, vector2_2.x, num1 * 2f), Mathf.Lerp(vector2_1.y, vector2_2.y, num1 * 2f));
          }
          ((TMP_Text) this.visualizeRocketLabel).text = StarmapScreen.GetTextForState(spacecraft.state, spacecraft).first;
          ((TMP_Text) this.visualizeRocketProgress).text = GameUtil.GetFormattedPercent(num1 * 100f);
          TransformExtensions.SetLocalPosition(((Component) this.visualizeRocketTrajectory).transform, Vector2.op_Implicit(vector2_1));
          ((Graphic) this.visualizeRocketTrajectory).rectTransform.sizeDelta = new Vector2(num4, ((Graphic) this.visualizeRocketTrajectory).rectTransform.sizeDelta.y);
          ((Transform) ((Graphic) this.visualizeRocketTrajectory).rectTransform).localRotation = Quaternion.Euler(0.0f, 0.0f, num5);
          TransformExtensions.SetLocalPosition(((Component) this.visualizeRocketImage).transform, Vector2.op_Implicit(vector2_4));
        }
      }
    }
    else
    {
      if (this.selectedDestination != null && this.planetWidgets.ContainsKey(this.selectedDestination))
      {
        this.UnselectAllPlanets();
        this.SelectPlanet(this.planetWidgets[this.selectedDestination]);
      }
      else
        this.UnselectAllPlanets();
      ((Component) this.visualizeRocketImage).gameObject.SetActive(false);
      ((Component) this.visualizeRocketTrajectory).gameObject.SetActive(false);
      ((Component) this.visualizeRocketLabel).gameObject.SetActive(false);
      ((Component) this.visualizeRocketProgress).gameObject.SetActive(false);
    }
  }
}
