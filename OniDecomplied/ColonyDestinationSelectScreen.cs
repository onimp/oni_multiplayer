// Decompiled with JetBrains decompiler
// Type: ColonyDestinationSelectScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using ProcGen;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColonyDestinationSelectScreen : NewGameFlowScreen
{
  [SerializeField]
  private GameObject destinationMap;
  [SerializeField]
  private GameObject customSettings;
  [SerializeField]
  private MultiToggle[] menuTabs;
  private int selectedMenuTabIdx;
  [SerializeField]
  private KButton backButton;
  [SerializeField]
  private KButton customizeButton;
  [SerializeField]
  private KButton launchButton;
  [SerializeField]
  private KButton shuffleButton;
  [SerializeField]
  private KButton storyTraitShuffleButton;
  [SerializeField]
  private HierarchyReferences locationIcons;
  [SerializeField]
  private RectTransform worldsScrollPanel;
  [SerializeField]
  private RectTransform storyScrollPanel;
  [SerializeField]
  private RectTransform destinationDetailsParent_Asteroid;
  [SerializeField]
  private RectTransform destinationDetailsParent_Story;
  [SerializeField]
  private LocText storyTraitsDestinationDetailsLabel;
  private const int DESTINATION_HEADER_BUTTON_HEIGHT_CLUSTER = 164;
  private const int DESTINATION_HEADER_BUTTON_HEIGHT_BASE = 76;
  private const int WORLDS_SCROLL_PANEL_HEIGHT_CLUSTER = 436;
  private const int WORLDS_SCROLL_PANEL_HEIGHT_BASE = 524;
  [SerializeField]
  private AsteroidDescriptorPanel destinationProperties;
  [SerializeField]
  private AsteroidDescriptorPanel selectedLocationProperties;
  [SerializeField]
  private KInputTextField coordinate;
  [SerializeField]
  private RectTransform destinationDetailsHeader;
  [SerializeField]
  private RectTransform destinationInfoPanel;
  [SerializeField]
  private RectTransform storyInfoPanel;
  [MyCmpReq]
  public NewGameSettingsPanel newGameSettings;
  [MyCmpReq]
  private DestinationSelectPanel destinationMapPanel;
  [SerializeField]
  private StoryContentPanel storyContentPanel;
  private KRandom random;
  private bool isEditingCoordinate;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.backButton.onClick += new System.Action(this.BackClicked);
    this.customizeButton.onClick += new System.Action(this.CustomizeClicked);
    this.launchButton.onClick += new System.Action(this.LaunchClicked);
    this.shuffleButton.onClick += new System.Action(this.ShuffleClicked);
    this.storyTraitShuffleButton.onClick += new System.Action(this.StoryTraitShuffleClicked);
    ((Component) this.storyTraitShuffleButton).gameObject.SetActive(((ResourceSet) Db.Get().Stories).Count > 3);
    this.destinationMapPanel.OnAsteroidClicked += new Action<ColonyDestinationAsteroidBeltData>(this.OnAsteroidClicked);
    KInputTextField coordinate = this.coordinate;
    ((TMP_InputField) coordinate).onFocus = ((TMP_InputField) coordinate).onFocus + new System.Action(this.CoordinateEditStarted);
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.coordinate).onEndEdit).AddListener(new UnityAction<string>((object) this, __methodptr(CoordinateEditFinished)));
    if (Object.op_Inequality((Object) this.locationIcons, (Object) null))
      ((Component) this.locationIcons).gameObject.SetActive(SaveLoader.GetCloudSavesAvailable());
    this.random = new KRandom();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.RefreshCloudSavePref();
    this.RefreshCloudLocalIcon();
    this.newGameSettings.Init();
    this.newGameSettings.SetCloseAction(new System.Action(this.CustomizeClose));
    this.destinationMapPanel.Init();
    CustomGameSettings.Instance.OnQualitySettingChanged += new Action<SettingConfig, SettingLevel>(this.QualitySettingChanged);
    CustomGameSettings.Instance.OnStorySettingChanged += new Action<SettingConfig, SettingLevel>(this.QualitySettingChanged);
    this.ShuffleClicked();
    this.RefreshMenuTabs();
    for (int index = 0; index < this.menuTabs.Length; ++index)
    {
      int target = index;
      this.menuTabs[index].onClick = (System.Action) (() =>
      {
        this.selectedMenuTabIdx = target;
        this.RefreshMenuTabs();
      });
    }
    this.ResizeLayout();
    this.storyContentPanel.Init();
    this.storyContentPanel.SelectRandomStories(useBias: true);
    this.storyContentPanel.SelectDefault();
    this.RefreshStoryLabel();
    this.RefreshRowsAndDescriptions();
  }

  private void ResizeLayout()
  {
    Vector2 sizeDelta1 = Util.rectTransform((Component) this.destinationProperties.clusterDetailsButton).sizeDelta;
    Util.rectTransform((Component) this.destinationProperties.clusterDetailsButton).sizeDelta = new Vector2(sizeDelta1.x, DlcManager.FeatureClusterSpaceEnabled() ? 164f : 76f);
    Vector2 sizeDelta2 = Util.rectTransform((Component) this.worldsScrollPanel).sizeDelta;
    Vector2 anchoredPosition = Util.rectTransform((Component) this.worldsScrollPanel).anchoredPosition;
    if (!DlcManager.FeatureClusterSpaceEnabled())
      Util.rectTransform((Component) this.worldsScrollPanel).anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y + 88f);
    float num1 = DlcManager.FeatureClusterSpaceEnabled() ? 436f : 524f;
    LayoutRebuilder.ForceRebuildLayoutImmediate(Util.rectTransform(((Component) this).gameObject));
    float num2 = Mathf.Min(num1, (float) ((double) this.destinationInfoPanel.sizeDelta.y - (DlcManager.FeatureClusterSpaceEnabled() ? 164.0 : 76.0) - 22.0));
    Util.rectTransform((Component) this.worldsScrollPanel).sizeDelta = new Vector2(sizeDelta2.x, num2);
    Util.rectTransform((Component) this.storyScrollPanel).sizeDelta = new Vector2(sizeDelta2.x, num2);
  }

  protected virtual void OnCleanUp()
  {
    CustomGameSettings.Instance.OnQualitySettingChanged -= new Action<SettingConfig, SettingLevel>(this.QualitySettingChanged);
    CustomGameSettings.Instance.OnStorySettingChanged -= new Action<SettingConfig, SettingLevel>(this.QualitySettingChanged);
    this.storyContentPanel.Cleanup();
    base.OnCleanUp();
  }

  private void RefreshCloudLocalIcon()
  {
    if (Object.op_Equality((Object) this.locationIcons, (Object) null) || !SaveLoader.GetCloudSavesAvailable())
      return;
    HierarchyReferences component1 = ((Component) this.locationIcons).GetComponent<HierarchyReferences>();
    LocText component2 = ((Component) component1.GetReference<RectTransform>("LocationText")).GetComponent<LocText>();
    KButton component3 = ((Component) component1.GetReference<RectTransform>("CloudButton")).GetComponent<KButton>();
    KButton component4 = ((Component) component1.GetReference<RectTransform>("LocalButton")).GetComponent<KButton>();
    ToolTip component5 = ((Component) component3).GetComponent<ToolTip>();
    ToolTip component6 = ((Component) component4).GetComponent<ToolTip>();
    string str = string.Format("{0}\n{1}", (object) STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP, (object) STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP_EXTRA);
    component5.toolTip = str;
    component6.toolTip = string.Format("{0}\n{1}", (object) STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP_LOCAL, (object) STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP_EXTRA);
    bool flag = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SaveToCloud).id == "Enabled";
    ((TMP_Text) component2).text = (string) (flag ? STRINGS.UI.FRONTEND.LOADSCREEN.CLOUD_SAVE : STRINGS.UI.FRONTEND.LOADSCREEN.LOCAL_SAVE);
    ((Component) component3).gameObject.SetActive(flag);
    component3.ClearOnClick();
    if (flag)
      component3.onClick += (System.Action) (() =>
      {
        CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, "Disabled");
        this.RefreshCloudLocalIcon();
      });
    ((Component) component4).gameObject.SetActive(!flag);
    component4.ClearOnClick();
    if (flag)
      return;
    component4.onClick += (System.Action) (() =>
    {
      CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, "Enabled");
      this.RefreshCloudLocalIcon();
    });
  }

  private void RefreshCloudSavePref()
  {
    if (!SaveLoader.GetCloudSavesAvailable())
      return;
    string savesDefaultPref = SaveLoader.GetCloudSavesDefaultPref();
    CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, savesDefaultPref);
  }

  private void BackClicked()
  {
    this.newGameSettings.Cancel();
    this.NavigateBackward();
  }

  private void CustomizeClicked()
  {
    this.newGameSettings.Refresh();
    this.customSettings.SetActive(true);
  }

  private void CustomizeClose() => this.customSettings.SetActive(false);

  private void LaunchClicked() => this.NavigateForward();

  private void RefreshMenuTabs()
  {
    for (int index = 0; index < this.menuTabs.Length; ++index)
    {
      this.menuTabs[index].ChangeState(index == this.selectedMenuTabIdx ? 1 : 0);
      ((Graphic) ((Component) this.menuTabs[index]).GetComponentInChildren<LocText>()).color = index == this.selectedMenuTabIdx ? Color.white : Color.grey;
    }
    ((Component) this.destinationInfoPanel).gameObject.SetActive(this.selectedMenuTabIdx == 0);
    ((Component) this.storyInfoPanel).gameObject.SetActive(this.selectedMenuTabIdx == 1);
    ((Transform) this.destinationDetailsHeader).SetParent(this.selectedMenuTabIdx == 0 ? (Transform) this.destinationDetailsParent_Asteroid : (Transform) this.destinationDetailsParent_Story);
    ((Transform) this.destinationDetailsHeader).SetAsFirstSibling();
  }

  private void ShuffleClicked()
  {
    int num = this.random.Next();
    this.newGameSettings.SetSetting((SettingConfig) CustomGameSettingConfigs.WorldgenSeed, num.ToString());
  }

  private void StoryTraitShuffleClicked() => this.storyContentPanel.SelectRandomStories();

  private void CoordinateChanged(string text)
  {
    string[] settingCoordinate = CustomGameSettings.ParseSettingCoordinate(text);
    if (settingCoordinate.Length != 4 && settingCoordinate.Length != 5 || !int.TryParse(settingCoordinate[2], out int _))
      return;
    ClusterLayout clusterLayout = (ClusterLayout) null;
    foreach (string clusterName in SettingsCache.GetClusterNames())
    {
      ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(clusterName);
      if (clusterData.coordinatePrefix == settingCoordinate[1])
        clusterLayout = clusterData;
    }
    if (clusterLayout != null)
      this.newGameSettings.SetSetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout, clusterLayout.filePath);
    this.newGameSettings.SetSetting((SettingConfig) CustomGameSettingConfigs.WorldgenSeed, settingCoordinate[2]);
    this.newGameSettings.ConsumeSettingsCode(settingCoordinate[3]);
    this.newGameSettings.ConsumeStoryTraitsCode(settingCoordinate.Length >= 5 ? settingCoordinate[4] : "0");
  }

  private void CoordinateEditStarted() => this.isEditingCoordinate = true;

  private void CoordinateEditFinished(string text)
  {
    this.CoordinateChanged(text);
    this.isEditingCoordinate = false;
    ((TMP_InputField) this.coordinate).text = CustomGameSettings.Instance.GetSettingsCoordinate();
  }

  private void QualitySettingChanged(SettingConfig config, SettingLevel level)
  {
    if (config == CustomGameSettingConfigs.SaveToCloud)
      this.RefreshCloudLocalIcon();
    if (!this.isEditingCoordinate)
      ((TMP_InputField) this.coordinate).text = CustomGameSettings.Instance.GetSettingsCoordinate();
    this.RefreshRowsAndDescriptions();
  }

  public void RefreshRowsAndDescriptions()
  {
    string setting1 = this.newGameSettings.GetSetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout);
    string setting2 = this.newGameSettings.GetSetting((SettingConfig) CustomGameSettingConfigs.WorldgenSeed);
    this.destinationMapPanel.UpdateDisplayedClusters();
    int seed;
    ref int local = ref seed;
    int.TryParse(setting2, out local);
    ColonyDestinationAsteroidBeltData cluster;
    try
    {
      cluster = this.destinationMapPanel.SelectCluster(setting1, seed);
    }
    catch
    {
      string defaultAsteroid = this.destinationMapPanel.GetDefaultAsteroid();
      this.newGameSettings.SetSetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout, defaultAsteroid);
      cluster = this.destinationMapPanel.SelectCluster(defaultAsteroid, seed);
    }
    if (DlcManager.IsContentActive("EXPANSION1_ID"))
    {
      this.destinationProperties.EnableClusterLocationLabels(true);
      this.destinationProperties.RefreshAsteroidLines(cluster, this.selectedLocationProperties, this.storyContentPanel.GetActiveStories());
      this.destinationProperties.EnableClusterDetails(true);
      this.destinationProperties.SetClusterDetailLabels(cluster);
      ((TMP_Text) this.selectedLocationProperties.headerLabel).SetText((string) STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.SELECTED_CLUSTER_TRAITS_HEADER);
      this.destinationProperties.clusterDetailsButton.onClick = (System.Action) (() => this.destinationProperties.SelectWholeClusterDetails(cluster, this.selectedLocationProperties, this.storyContentPanel.GetActiveStories()));
    }
    else
    {
      this.destinationProperties.EnableClusterDetails(false);
      this.destinationProperties.EnableClusterLocationLabels(false);
      this.destinationProperties.SetParameterDescriptors((IList<AsteroidDescriptor>) cluster.GetParamDescriptors());
      this.selectedLocationProperties.SetTraitDescriptors((IList<AsteroidDescriptor>) cluster.GetTraitDescriptors(), this.storyContentPanel.GetActiveStories());
    }
    this.RefreshStoryLabel();
  }

  public void RefreshStoryLabel()
  {
    ((TMP_Text) this.storyTraitsDestinationDetailsLabel).SetText(this.storyContentPanel.GetTraitsString());
    ((Component) this.storyTraitsDestinationDetailsLabel).GetComponent<ToolTip>().SetSimpleTooltip(this.storyContentPanel.GetTraitsString(true));
  }

  private void OnAsteroidClicked(ColonyDestinationAsteroidBeltData cluster)
  {
    this.newGameSettings.SetSetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout, cluster.beltPath);
    this.ShuffleClicked();
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (this.isEditingCoordinate)
      return;
    if (!((KInputEvent) e).Consumed && e.TryConsume((Action) 136))
      this.destinationMapPanel.ScrollLeft();
    else if (!((KInputEvent) e).Consumed && e.TryConsume((Action) 137))
      this.destinationMapPanel.ScrollRight();
    else if (this.customSettings.activeSelf && !((KInputEvent) e).Consumed && (e.TryConsume((Action) 1) || e.TryConsume((Action) 5)))
      this.CustomizeClose();
    base.OnKeyDown(e);
  }
}
