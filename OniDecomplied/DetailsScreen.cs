// Decompiled with JetBrains decompiler
// Type: DetailsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DetailsScreen : KTabMenu
{
  public static DetailsScreen Instance;
  [SerializeField]
  private KButton CodexEntryButton;
  [SerializeField]
  private KButton ChangeOutfitButton;
  [Header("Panels")]
  public Transform UserMenuPanel;
  [Header("Name Editing (disabled)")]
  [SerializeField]
  private KButton CloseButton;
  [Header("Tabs")]
  [SerializeField]
  private EditableTitleBar TabTitle;
  [SerializeField]
  private DetailsScreen.Screens[] screens;
  [SerializeField]
  private GameObject tabHeaderContainer;
  [Header("Side Screens")]
  [SerializeField]
  private GameObject sideScreenContentBody;
  [SerializeField]
  private GameObject sideScreen;
  [SerializeField]
  private LocText sideScreenTitle;
  [SerializeField]
  private List<DetailsScreen.SideScreenRef> sideScreens;
  [Header("Secondary Side Screens")]
  [SerializeField]
  private GameObject sideScreen2ContentBody;
  [SerializeField]
  private GameObject sideScreen2;
  [SerializeField]
  private LocText sideScreen2Title;
  private KScreen activeSideScreen2;
  private bool HasActivated;
  private SideScreenContent currentSideScreen;
  private Dictionary<KScreen, KScreen> instantiatedSecondarySideScreens = new Dictionary<KScreen, KScreen>();
  private static readonly EventSystem.IntraObjectHandler<DetailsScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<DetailsScreen>((Action<DetailsScreen, object>) ((component, data) => component.OnRefreshData(data)));
  private List<KeyValuePair<GameObject, int>> sortedSideScreens = new List<KeyValuePair<GameObject, int>>();
  private int setRocketTitleHandle = -1;

  public static void DestroyInstance() => DetailsScreen.Instance = (DetailsScreen) null;

  public GameObject target { get; private set; }

  protected virtual void OnPrefabInit()
  {
    ((KScreen) this).OnPrefabInit();
    this.SortScreenOrder();
    ((KScreen) this).ConsumeMouseScroll = true;
    Debug.Assert(Object.op_Equality((Object) DetailsScreen.Instance, (Object) null));
    DetailsScreen.Instance = this;
    this.DeactivateSideContent();
    ((KScreen) this).Show(false);
    ((KMonoBehaviour) this).Subscribe(((Component) Game.Instance).gameObject, -1503271301, new Action<object>(this.OnSelectObject));
  }

  private void OnSelectObject(object data)
  {
    if (data != null)
      return;
    this.previouslyActiveTab = -1;
  }

  protected virtual void OnSpawn()
  {
    ((KScreen) this).OnSpawn();
    this.CodexEntryButton.onClick += new System.Action(this.OpenCodexEntry);
    this.ChangeOutfitButton.onClick += new System.Action(this.OnClickChangeOutfit);
    this.CloseButton.onClick += new System.Action(this.DeselectAndClose);
    this.TabTitle.OnNameChanged += new Action<string>(this.OnNameChanged);
    this.TabTitle.OnStartedEditing += new System.Action(this.OnStartedEditing);
    this.sideScreen2.SetActive(false);
    ((KMonoBehaviour) this).Subscribe<DetailsScreen>(-1514841199, DetailsScreen.OnRefreshDataDelegate);
  }

  private void OnStartedEditing()
  {
    ((KScreen) this).isEditing = true;
    KScreenManager.Instance.RefreshStack();
  }

  private void OnNameChanged(string newName)
  {
    ((KScreen) this).isEditing = false;
    if (string.IsNullOrEmpty(newName))
      return;
    MinionIdentity component1 = this.target.GetComponent<MinionIdentity>();
    UserNameable component2 = this.target.GetComponent<UserNameable>();
    ClustercraftExteriorDoor component3 = this.target.GetComponent<ClustercraftExteriorDoor>();
    CommandModule component4 = this.target.GetComponent<CommandModule>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      component1.SetName(newName);
    else if (Object.op_Inequality((Object) component4, (Object) null))
      SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) component4).GetComponent<LaunchConditionManager>()).SetRocketName(newName);
    else if (Object.op_Inequality((Object) component3, (Object) null))
      ((Component) component3.GetTargetWorld()).GetComponent<UserNameable>().SetName(newName);
    else if (Object.op_Inequality((Object) component2, (Object) null))
      component2.SetName(newName);
    this.TabTitle.UpdateRenameTooltip(this.target);
  }

  protected virtual void OnDeactivate()
  {
    if (Object.op_Inequality((Object) this.target, (Object) null) && this.setRocketTitleHandle != -1)
      KMonoBehaviourExtensions.Unsubscribe(this.target, this.setRocketTitleHandle);
    this.setRocketTitleHandle = -1;
    this.DeactivateSideContent();
    base.OnDeactivate();
  }

  protected virtual void OnShow(bool show)
  {
    if (!show)
    {
      this.DeactivateSideContent();
    }
    else
    {
      this.MaskSideContent(false);
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().MenuOpenHalfEffect);
    }
    ((KScreen) this).OnShow(show);
  }

  protected virtual void OnCmpDisable()
  {
    this.DeactivateSideContent();
    ((KMonoBehaviour) this).OnCmpDisable();
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (((KScreen) this).isEditing || !Object.op_Inequality((Object) this.target, (Object) null) || !PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
      return;
    this.DeselectAndClose();
  }

  private static Component GetComponent(GameObject go, string name)
  {
    System.Type type = System.Type.GetType(name);
    return !(type != (System.Type) null) ? go.GetComponent(name) : go.GetComponent(type);
  }

  private static bool IsExcludedPrefabTag(GameObject go, Tag[] excluded_tags)
  {
    if (excluded_tags == null || excluded_tags.Length == 0)
      return false;
    bool flag = false;
    KPrefabID component = go.GetComponent<KPrefabID>();
    for (int index = 0; index < excluded_tags.Length; ++index)
    {
      Tag excludedTag = excluded_tags[index];
      if (Tag.op_Equality(component.PrefabTag, excludedTag))
      {
        flag = true;
        break;
      }
    }
    return flag;
  }

  private void UpdateCodexButton()
  {
    this.CodexEntryButton.isInteractable = this.GetSelectedObjectCodexID() != "";
    ((Component) this.CodexEntryButton).GetComponent<ToolTip>().SetSimpleTooltip((string) (this.CodexEntryButton.isInteractable ? UI.TOOLTIPS.OPEN_CODEX_ENTRY : UI.TOOLTIPS.NO_CODEX_ENTRY));
  }

  private void UpdateOutfitButton() => ((Component) this.ChangeOutfitButton).gameObject.SetActive(Object.op_Implicit((Object) this.target.GetComponent<MinionIdentity>()));

  public void OnRefreshData(object obj)
  {
    this.SetTitle(this.PreviousActiveTab);
    for (int index = 0; index < this.tabs.Count; ++index)
    {
      if (((Component) this.tabs[index]).gameObject.activeInHierarchy)
        ((KMonoBehaviour) this.tabs[index]).Trigger(-1514841199, obj);
    }
  }

  public void Refresh(GameObject go)
  {
    if (this.screens == null)
      return;
    if (Object.op_Inequality((Object) this.target, (Object) go) && this.setRocketTitleHandle != -1)
    {
      KMonoBehaviourExtensions.Unsubscribe(this.target, this.setRocketTitleHandle);
      this.setRocketTitleHandle = -1;
    }
    this.target = go;
    this.sortedSideScreens.Clear();
    CellSelectionObject component = this.target.GetComponent<CellSelectionObject>();
    if (Object.op_Implicit((Object) component))
      component.OnObjectSelected((object) null);
    if (!this.HasActivated)
    {
      if (this.screens != null)
      {
        for (int index = 0; index < this.screens.Length; ++index)
        {
          GameObject gameObject = ((Component) KScreenManager.Instance.InstantiateScreen(((Component) this.screens[index].screen).gameObject, ((Component) this.body).gameObject)).gameObject;
          this.screens[index].screen = gameObject.GetComponent<TargetScreen>();
          this.screens[index].tabIdx = this.AddTab(this.screens[index].icon, StringEntry.op_Implicit(Strings.Get(this.screens[index].displayName)), (KScreen) this.screens[index].screen, StringEntry.op_Implicit(Strings.Get(this.screens[index].tooltip)));
        }
      }
      // ISSUE: method pointer
      this.onTabActivated += new KTabMenu.TabActivated((object) this, __methodptr(OnTabActivated));
      this.HasActivated = true;
    }
    int num1 = -1;
    int num2 = 0;
    for (int index = 0; index < this.screens.Length; ++index)
    {
      int num3 = this.screens[index].screen.IsValidForTarget(go) ? 1 : 0;
      bool flag1 = this.screens[index].hideWhenDead && ((Component) this).gameObject.HasTag(GameTags.Dead);
      bool flag2 = num3 != 0 && !flag1;
      this.SetTabEnabled(this.screens[index].tabIdx, flag2);
      if (flag2)
      {
        ++num2;
        if (num1 == -1)
        {
          if (HashedString.op_Inequality(SimDebugView.Instance.GetMode(), OverlayModes.None.ID))
          {
            if (HashedString.op_Equality(SimDebugView.Instance.GetMode(), this.screens[index].focusInViewMode))
              num1 = index;
          }
          else if (flag2 && this.previouslyActiveTab >= 0 && this.previouslyActiveTab < this.screens.Length && this.screens[index].name == this.screens[this.previouslyActiveTab].name)
            num1 = this.screens[index].tabIdx;
        }
      }
    }
    if (num1 != -1)
      this.ActivateTab(num1);
    else
      this.ActivateTab(0);
    this.tabHeaderContainer.gameObject.SetActive(this.CountTabs() > 1);
    if (this.sideScreens != null && this.sideScreens.Count > 0)
    {
      bool areAnyValid = false;
      this.sideScreens.ForEach((Action<DetailsScreen.SideScreenRef>) (scn =>
      {
        if (!scn.screenPrefab.IsValidForTarget(this.target))
        {
          if (!Object.op_Inequality((Object) scn.screenInstance, (Object) null) || !((Component) scn.screenInstance).gameObject.activeSelf)
            return;
          ((Component) scn.screenInstance).gameObject.SetActive(false);
        }
        else
        {
          areAnyValid = true;
          if (Object.op_Equality((Object) scn.screenInstance, (Object) null))
            scn.screenInstance = Util.KInstantiateUI<SideScreenContent>(((Component) scn.screenPrefab).gameObject, this.sideScreenContentBody, false);
          if (!this.sideScreen.activeSelf)
            this.sideScreen.SetActive(true);
          scn.screenInstance.SetTarget(this.target);
          scn.screenInstance.Show(true);
          int sideScreenSortOrder = scn.screenInstance.GetSideScreenSortOrder();
          this.sortedSideScreens.Add(new KeyValuePair<GameObject, int>(((Component) scn.screenInstance).gameObject, sideScreenSortOrder));
          if (Object.op_Equality((Object) this.currentSideScreen, (Object) null) || !((Component) this.currentSideScreen).gameObject.activeSelf || sideScreenSortOrder > this.sortedSideScreens.Find((Predicate<KeyValuePair<GameObject, int>>) (match => Object.op_Equality((Object) match.Key, (Object) ((Component) this.currentSideScreen).gameObject))).Value)
            this.currentSideScreen = scn.screenInstance;
          this.RefreshTitle();
        }
      }));
      if (!areAnyValid)
        this.sideScreen.SetActive(false);
    }
    this.sortedSideScreens.Sort((Comparison<KeyValuePair<GameObject, int>>) ((x, y) => x.Value <= y.Value ? 1 : -1));
    for (int index = 0; index < this.sortedSideScreens.Count; ++index)
      this.sortedSideScreens[index].Key.transform.SetSiblingIndex(index);
  }

  public void RefreshTitle()
  {
    if (!Object.op_Implicit((Object) this.currentSideScreen))
      return;
    ((TMP_Text) this.sideScreenTitle).SetText(this.currentSideScreen.GetTitle());
  }

  private void OnTabActivated(int newTab, int oldTab)
  {
    this.SetTitle(newTab);
    if (oldTab != -1)
      this.screens[oldTab].screen.SetTarget((GameObject) null);
    if (newTab == -1)
      return;
    this.screens[newTab].screen.SetTarget(this.target);
  }

  public KScreen SetSecondarySideScreen(KScreen secondaryPrefab, string title)
  {
    this.ClearSecondarySideScreen();
    if (this.instantiatedSecondarySideScreens.ContainsKey(secondaryPrefab))
    {
      this.activeSideScreen2 = this.instantiatedSecondarySideScreens[secondaryPrefab];
      ((Component) this.activeSideScreen2).gameObject.SetActive(true);
    }
    else
    {
      this.activeSideScreen2 = KScreenManager.Instance.InstantiateScreen(((Component) secondaryPrefab).gameObject, this.sideScreen2ContentBody);
      this.activeSideScreen2.Activate();
      this.instantiatedSecondarySideScreens.Add(secondaryPrefab, this.activeSideScreen2);
    }
    ((TMP_Text) this.sideScreen2Title).text = title;
    this.sideScreen2.SetActive(true);
    return this.activeSideScreen2;
  }

  public void ClearSecondarySideScreen()
  {
    if (Object.op_Inequality((Object) this.activeSideScreen2, (Object) null))
    {
      ((Component) this.activeSideScreen2).gameObject.SetActive(false);
      this.activeSideScreen2 = (KScreen) null;
    }
    this.sideScreen2.SetActive(false);
  }

  public void DeactivateSideContent()
  {
    if (Object.op_Inequality((Object) SideDetailsScreen.Instance, (Object) null) && ((Component) SideDetailsScreen.Instance).gameObject.activeInHierarchy)
      SideDetailsScreen.Instance.Show(false);
    if (this.sideScreens != null && this.sideScreens.Count > 0)
      this.sideScreens.ForEach((Action<DetailsScreen.SideScreenRef>) (scn =>
      {
        if (!Object.op_Inequality((Object) scn.screenInstance, (Object) null))
          return;
        scn.screenInstance.ClearTarget();
        scn.screenInstance.Show(false);
      }));
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MenuOpenHalfEffect);
    this.sideScreen.SetActive(false);
  }

  public void MaskSideContent(bool hide)
  {
    if (hide)
      this.sideScreen.transform.localScale = Vector3.zero;
    else
      this.sideScreen.transform.localScale = Vector3.one;
  }

  private string GetSelectedObjectCodexID()
  {
    string str = "";
    Debug.Assert(Object.op_Inequality((Object) this.target, (Object) null), (object) "Details Screen has no target");
    KSelectable component1 = this.target.GetComponent<KSelectable>();
    DebugUtil.AssertArgs((Object.op_Inequality((Object) component1, (Object) null) ? 1 : 0) != 0, new object[2]
    {
      (object) "Details Screen target is not a KSelectable",
      (object) this.target
    });
    CellSelectionObject component2 = ((Component) component1).GetComponent<CellSelectionObject>();
    BuildingUnderConstruction component3 = ((Component) component1).GetComponent<BuildingUnderConstruction>();
    CreatureBrain component4 = ((Component) component1).GetComponent<CreatureBrain>();
    PlantableSeed component5 = ((Component) component1).GetComponent<PlantableSeed>();
    BudUprootedMonitor component6 = ((Component) component1).GetComponent<BudUprootedMonitor>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      str = CodexCache.FormatLinkID(component2.element.id.ToString());
    else if (Object.op_Inequality((Object) component3, (Object) null))
      str = CodexCache.FormatLinkID(component3.Def.PrefabID);
    else if (Object.op_Inequality((Object) component4, (Object) null))
      str = CodexCache.FormatLinkID(((Component) component1).PrefabID().ToString()).Replace("BABY", "");
    else if (Object.op_Inequality((Object) component5, (Object) null))
      str = CodexCache.FormatLinkID(((Component) component1).PrefabID().ToString()).Replace("SEED", "");
    else if (Object.op_Inequality((Object) component6, (Object) null))
    {
      if (Object.op_Inequality((Object) component6.parentObject.Get(), (Object) null))
        str = CodexCache.FormatLinkID(component6.parentObject.Get().PrefabID().ToString());
      else if (Object.op_Inequality((Object) ((Component) component6).GetComponent<TreeBud>(), (Object) null))
        str = CodexCache.FormatLinkID(((Component) ((Component) component6).GetComponent<TreeBud>().buddingTrunk.Get()).PrefabID().ToString());
    }
    else
      str = CodexCache.FormatLinkID(((Component) component1).PrefabID().ToString());
    return CodexCache.entries.ContainsKey(str) || CodexCache.FindSubEntry(str) != null ? str : "";
  }

  public void OpenCodexEntry()
  {
    string selectedObjectCodexId = this.GetSelectedObjectCodexID();
    if (!(selectedObjectCodexId != ""))
      return;
    ManagementMenu.Instance.OpenCodexToEntry(selectedObjectCodexId);
  }

  public void OnClickChangeOutfit()
  {
    LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.outfitBrowserScreen);
    LockerNavigator.Instance.outfitBrowserScreen.GetComponent<OutfitBrowserScreen>().Configure(OutfitBrowserScreenConfig.Minion(this.target));
  }

  public void DeselectAndClose()
  {
    if (((Component) this).gameObject.activeInHierarchy)
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Back"));
    if (Object.op_Inequality((Object) this.GetActiveTab(), (Object) null))
      this.GetActiveTab().SetTarget((GameObject) null);
    SelectTool.Instance.Select((KSelectable) null);
    ClusterMapSelectTool.Instance.Select((KSelectable) null);
    if (Object.op_Equality((Object) this.target, (Object) null))
      return;
    this.target = (GameObject) null;
    this.DeactivateSideContent();
    ((KScreen) this).Show(false);
  }

  private void SortScreenOrder() => Array.Sort<DetailsScreen.Screens>(this.screens, (Comparison<DetailsScreen.Screens>) ((x, y) => x.displayOrderPriority.CompareTo(y.displayOrderPriority)));

  public void UpdatePortrait(GameObject target)
  {
    KSelectable component1 = target.GetComponent<KSelectable>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return;
    this.TabTitle.portrait.ClearPortrait();
    Building component2 = ((Component) component1).GetComponent<Building>();
    if (Object.op_Implicit((Object) component2))
    {
      Sprite uiSprite = component2.Def.GetUISprite();
      if (Object.op_Inequality((Object) uiSprite, (Object) null))
      {
        this.TabTitle.portrait.SetPortrait(uiSprite);
        return;
      }
    }
    if (Object.op_Implicit((Object) target.GetComponent<MinionIdentity>()))
    {
      this.TabTitle.SetPortrait(((Component) component1).gameObject);
    }
    else
    {
      Edible component3 = target.GetComponent<Edible>();
      if (Object.op_Inequality((Object) component3, (Object) null))
      {
        this.TabTitle.portrait.SetPortrait(Def.GetUISpriteFromMultiObjectAnim(((Component) component3).GetComponent<KBatchedAnimController>().AnimFiles[0]));
      }
      else
      {
        PrimaryElement component4 = target.GetComponent<PrimaryElement>();
        if (Object.op_Inequality((Object) component4, (Object) null))
        {
          this.TabTitle.portrait.SetPortrait(Def.GetUISpriteFromMultiObjectAnim(ElementLoader.FindElementByHash(component4.ElementID).substance.anim));
        }
        else
        {
          CellSelectionObject component5 = target.GetComponent<CellSelectionObject>();
          if (!Object.op_Inequality((Object) component5, (Object) null))
            return;
          string animName = component5.element.IsSolid ? "ui" : component5.element.substance.name;
          this.TabTitle.portrait.SetPortrait(Def.GetUISpriteFromMultiObjectAnim(component5.element.substance.anim, animName));
        }
      }
    }
  }

  public bool CompareTargetWith(GameObject compare) => Object.op_Equality((Object) this.target, (Object) compare);

  public void SetTitle(int selectedTabIndex)
  {
    this.UpdateCodexButton();
    this.UpdateOutfitButton();
    if (!Object.op_Inequality((Object) this.TabTitle, (Object) null))
      return;
    this.TabTitle.SetTitle(this.target.GetProperName());
    MinionIdentity minionIdentity = (MinionIdentity) null;
    UserNameable userNameable = (UserNameable) null;
    ClustercraftExteriorDoor clusterCraftDoor = (ClustercraftExteriorDoor) null;
    CommandModule commandModule = (CommandModule) null;
    if (Object.op_Inequality((Object) this.target, (Object) null))
    {
      minionIdentity = this.target.gameObject.GetComponent<MinionIdentity>();
      userNameable = this.target.gameObject.GetComponent<UserNameable>();
      clusterCraftDoor = this.target.gameObject.GetComponent<ClustercraftExteriorDoor>();
      commandModule = this.target.gameObject.GetComponent<CommandModule>();
    }
    if (Object.op_Inequality((Object) minionIdentity, (Object) null))
    {
      this.TabTitle.SetSubText(((Component) minionIdentity).GetComponent<MinionResume>().GetSkillsSubtitle());
      this.TabTitle.SetUserEditable(true);
    }
    else if (Object.op_Inequality((Object) userNameable, (Object) null))
    {
      this.TabTitle.SetSubText("");
      this.TabTitle.SetUserEditable(true);
    }
    else if (Object.op_Inequality((Object) commandModule, (Object) null))
      this.TrySetRocketTitle(commandModule);
    else if (Object.op_Inequality((Object) clusterCraftDoor, (Object) null))
    {
      this.TrySetRocketTitle(clusterCraftDoor);
    }
    else
    {
      this.TabTitle.SetSubText("");
      this.TabTitle.SetUserEditable(false);
    }
    this.TabTitle.UpdateRenameTooltip(this.target);
  }

  private void TrySetRocketTitle(ClustercraftExteriorDoor clusterCraftDoor1)
  {
    if (clusterCraftDoor1.HasTargetWorld())
    {
      this.TabTitle.SetTitle(((Component) clusterCraftDoor1.GetTargetWorld()).GetComponent<ClusterGridEntity>().Name);
      this.TabTitle.SetUserEditable(true);
      this.TabTitle.SetSubText(this.target.GetProperName());
      this.setRocketTitleHandle = -1;
    }
    else
    {
      if (this.setRocketTitleHandle != -1)
        return;
      this.setRocketTitleHandle = KMonoBehaviourExtensions.Subscribe(this.target, -71801987, (Action<object>) (clusterCraftDoor2 =>
      {
        this.OnRefreshData((object) null);
        KMonoBehaviourExtensions.Unsubscribe(this.target, this.setRocketTitleHandle);
        this.setRocketTitleHandle = -1;
      }));
    }
  }

  private void TrySetRocketTitle(CommandModule commandModule)
  {
    if (Object.op_Inequality((Object) commandModule, (Object) null))
    {
      this.TabTitle.SetTitle(SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(((Component) commandModule).GetComponent<LaunchConditionManager>()).GetRocketName());
      this.TabTitle.SetUserEditable(true);
    }
    this.TabTitle.SetSubText(this.target.GetProperName());
  }

  public void SetTitle(string title) => this.TabTitle.SetTitle(title);

  public TargetScreen GetActiveTab() => this.previouslyActiveTab >= 0 && this.previouslyActiveTab < this.screens.Length ? this.screens[this.previouslyActiveTab].screen : (TargetScreen) null;

  [Serializable]
  private struct Screens
  {
    public string name;
    public string displayName;
    public string tooltip;
    public Sprite icon;
    public TargetScreen screen;
    public int displayOrderPriority;
    public bool hideWhenDead;
    public HashedString focusInViewMode;
    [HideInInspector]
    public int tabIdx;
  }

  [Serializable]
  public class SideScreenRef
  {
    public string name;
    public SideScreenContent screenPrefab;
    public Vector2 offset;
    [HideInInspector]
    public SideScreenContent screenInstance;
  }
}
