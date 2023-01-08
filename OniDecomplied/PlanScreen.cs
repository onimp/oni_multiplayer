// Decompiled with JetBrains decompiler
// Type: PlanScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlanScreen : KIconToggleMenu
{
  [SerializeField]
  private GameObject planButtonPrefab;
  [SerializeField]
  private GameObject recipeInfoScreenParent;
  [SerializeField]
  private GameObject productInfoScreenPrefab;
  [SerializeField]
  private GameObject copyBuildingButton;
  private bool USE_SUB_CATEGORY_LAYOUT;
  private int refreshScaleHandle = -1;
  [SerializeField]
  private GameObject adjacentPinnedButtons;
  private static Dictionary<HashedString, string> iconNameMap;
  private Dictionary<KIconToggleMenu.ToggleInfo, bool> CategoryInteractive = new Dictionary<KIconToggleMenu.ToggleInfo, bool>();
  [SerializeField]
  public PlanScreen.BuildingToolTipSettings buildingToolTipSettings;
  public PlanScreen.BuildingNameTextSetting buildingNameTextSettings;
  private KIconToggleMenu.ToggleInfo activeCategoryInfo;
  public Dictionary<BuildingDef, PlanBuildingToggle> ActiveCategoryBuildingToggles = new Dictionary<BuildingDef, PlanBuildingToggle>();
  private float timeSinceNotificationPing;
  private float notificationPingExpire = 0.5f;
  private float specialNotificationEmbellishDelay = 8f;
  private int notificationPingCount;
  private Dictionary<string, GameObject> allSubCategoryObjects = new Dictionary<string, GameObject>();
  private Dictionary<string, GameObject> currentSubCategoryObjects = new Dictionary<string, GameObject>();
  private Dictionary<string, GameObject> allBuildingToggles = new Dictionary<string, GameObject>();
  private static Vector2 bigBuildingButtonSize;
  private static Vector2 standarduildingButtonSize;
  public static int fontSizeBigMode;
  public static int fontSizeStandardMode;
  [SerializeField]
  private GameObject subgroupPrefab;
  public Transform GroupsTransform;
  public Sprite Overlay_NeedTech;
  public RectTransform buildingGroupsRoot;
  public RectTransform BuildButtonBGPanel;
  public RectTransform BuildingGroupContentsRect;
  public Sprite defaultBuildingIconSprite;
  private KScrollRect planScreenScrollRect;
  public Material defaultUIMaterial;
  public Material desaturatedUIMaterial;
  public LocText PlanCategoryLabel;
  private List<PlanScreen.ToggleEntry> toggleEntries = new List<PlanScreen.ToggleEntry>();
  private int ignoreToolChangeMessages;
  private Dictionary<string, PlanScreen.RequirementsState> _buildableStatesByID = new Dictionary<string, PlanScreen.RequirementsState>();
  private Dictionary<Def, bool> _researchedDefs = new Dictionary<Def, bool>();
  [SerializeField]
  private TextStyleSetting[] CategoryLabelTextStyles;
  private float initTime;
  private Dictionary<Tag, HashedString> tagCategoryMap;
  private Dictionary<Tag, int> tagOrderMap;
  private BuildingDef lastSelectedBuildingDef;
  private Building lastSelectedBuilding;
  private string lastSelectedBuildingFacade = "DEFAULT_FACADE";
  private int buildable_state_update_idx;
  private int nextCategoryToUpdateIDX = -1;
  private bool forceUpdateAllCategoryToggles;
  private int building_button_refresh_idx;
  private int maxToggleRefreshPerFrame = 1;
  private bool categoryPanelSizeNeedsRefresh;
  private float buildGrid_bg_width = 320f;
  private float buildGrid_bg_borderHeight = 48f;
  private float buildGrid_bg_rowHeight;

  public static PlanScreen Instance { get; private set; }

  public static void DestroyInstance() => PlanScreen.Instance = (PlanScreen) null;

  public static Dictionary<HashedString, string> IconNameMap => PlanScreen.iconNameMap;

  private static HashedString CacheHashedString(string str) => HashCache.Get().Add(str);

  public ProductInfoScreen ProductInfoScreen { get; private set; }

  public GameObject SelectedBuildingGameObject { get; private set; }

  public virtual float GetSortKey() => 2f;

  public PlanScreen.RequirementsState GetBuildableState(BuildingDef def) => Object.op_Equality((Object) def, (Object) null) ? PlanScreen.RequirementsState.Materials : this._buildableStatesByID[def.PrefabID];

  private bool IsDefResearched(BuildingDef def)
  {
    bool flag = false;
    if (!this._researchedDefs.TryGetValue((Def) def, out flag))
      flag = this.UpdateDefResearched(def);
    return flag;
  }

  private bool UpdateDefResearched(BuildingDef def) => this._researchedDefs[(Def) def] = Db.Get().TechItems.IsTechItemComplete(def.PrefabID);

  protected virtual void OnPrefabInit()
  {
    if (BuildMenu.UseHotkeyBuildMenu())
    {
      ((Component) this).gameObject.SetActive(false);
    }
    else
    {
      base.OnPrefabInit();
      PlanScreen.Instance = this;
      this.ProductInfoScreen = Util.KInstantiateUI<ProductInfoScreen>(this.productInfoScreenPrefab, this.recipeInfoScreenParent, false);
      Util.rectTransform((Component) this.ProductInfoScreen).pivot = new Vector2(0.0f, 0.0f);
      TransformExtensions.SetLocalPosition((Transform) Util.rectTransform((Component) this.ProductInfoScreen), new Vector3(326f, 0.0f, 0.0f));
      this.ProductInfoScreen.onElementsFullySelected = new System.Action(this.OnRecipeElementsFullySelected);
      // ISSUE: method pointer
      KInputManager.InputChange.AddListener(new UnityAction((object) this, __methodptr(RefreshToolTip)));
      this.planScreenScrollRect = ((Component) ((KMonoBehaviour) this).transform.parent).GetComponentInParent<KScrollRect>();
      Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
      Game.Instance.Subscribe(1174281782, new Action<object>(this.OnActiveToolChanged));
      Game.Instance.Subscribe(1557339983, new Action<object>(this.ForceUpdateAllCategoryToggles));
    }
    ((Component) this.buildingGroupsRoot).gameObject.SetActive(false);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.ConsumeMouseScroll = true;
    this.initTime = KTime.Instance.UnscaledGameTime;
    foreach (Def buildingDef in Assets.BuildingDefs)
      this._buildableStatesByID.Add(buildingDef.PrefabID, PlanScreen.RequirementsState.Materials);
    if (BuildMenu.UseHotkeyBuildMenu())
    {
      ((Component) this).gameObject.SetActive(false);
    }
    else
    {
      this.onSelect += new KIconToggleMenu.OnSelect(this.OnClickCategory);
      this.Refresh();
      foreach (Toggle toggle in this.toggles)
        toggle.group = ((Component) this).GetComponent<ToggleGroup>();
      this.RefreshBuildableStates(true);
      Game.Instance.Subscribe(288942073, new Action<object>(this.OnUIClear));
    }
    this.copyBuildingButton.GetComponent<MultiToggle>().onClick = (System.Action) (() => this.OnClickCopyBuilding());
    this.RefreshCopyBuildingButton();
    Game.Instance.Subscribe(-1503271301, new Action<object>(this.RefreshCopyBuildingButton));
    Game.Instance.Subscribe(1983128072, (Action<object>) (data => this.CloseRecipe()));
    // ISSUE: method pointer
    this.pointerEnterActions = (KScreen.PointerEnterActions) Delegate.Combine((Delegate) this.pointerEnterActions, (Delegate) new KScreen.PointerEnterActions((object) this, __methodptr(PointerEnter)));
    // ISSUE: method pointer
    this.pointerExitActions = (KScreen.PointerExitActions) Delegate.Combine((Delegate) this.pointerExitActions, (Delegate) new KScreen.PointerExitActions((object) this, __methodptr(PointerExit)));
    this.copyBuildingButton.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.COPY_BUILDING_TOOLTIP, (Action) 50));
    this.RefreshScale();
    this.refreshScaleHandle = Game.Instance.Subscribe(-442024484, new Action<object>(this.RefreshScale));
  }

  private void RefreshScale(object data = null)
  {
    ((Component) this).GetComponent<GridLayoutGroup>().cellSize = ScreenResolutionMonitor.UsingGamepadUIMode() ? new Vector2(54f, 50f) : new Vector2(45f, 45f);
    this.toggles.ForEach((Action<KToggle>) (to => ((TMP_Text) ((Component) to).GetComponentInChildren<LocText>()).fontSize = ScreenResolutionMonitor.UsingGamepadUIMode() ? (float) PlanScreen.fontSizeBigMode : (float) PlanScreen.fontSizeStandardMode));
    LayoutElement component = this.copyBuildingButton.GetComponent<LayoutElement>();
    component.minWidth = ScreenResolutionMonitor.UsingGamepadUIMode() ? 58f : 54f;
    component.minHeight = ScreenResolutionMonitor.UsingGamepadUIMode() ? 58f : 54f;
    Util.rectTransform(((Component) this).gameObject).anchoredPosition = new Vector2(0.0f, ScreenResolutionMonitor.UsingGamepadUIMode() ? -68f : -74f);
    ((LayoutGroup) this.adjacentPinnedButtons.GetComponent<HorizontalLayoutGroup>()).padding.bottom = ScreenResolutionMonitor.UsingGamepadUIMode() ? 14 : 6;
    foreach (KeyValuePair<string, GameObject> subCategoryObject in this.currentSubCategoryObjects)
      subCategoryObject.Value.GetComponentInChildren<GridLayoutGroup>().cellSize = ScreenResolutionMonitor.UsingGamepadUIMode() ? PlanScreen.bigBuildingButtonSize : PlanScreen.standarduildingButtonSize;
    Vector2 sizeDelta = Util.rectTransform((Component) this.buildingGroupsRoot).sizeDelta;
    Vector2 vector2 = ScreenResolutionMonitor.UsingGamepadUIMode() ? new Vector2(320f, sizeDelta.y) : new Vector2(264f, sizeDelta.y);
    Util.rectTransform((Component) this.buildingGroupsRoot).sizeDelta = vector2;
    Util.rectTransform((Component) this.ProductInfoScreen).anchoredPosition = new Vector2(vector2.x + 8f, Util.rectTransform((Component) this.ProductInfoScreen).anchoredPosition.y);
  }

  protected virtual void OnForcedCleanUp()
  {
    // ISSUE: method pointer
    KInputManager.InputChange.RemoveListener(new UnityAction((object) this, __methodptr(RefreshToolTip)));
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  protected virtual void OnCleanUp()
  {
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
      Game.Instance.Unsubscribe(this.refreshScaleHandle);
    base.OnCleanUp();
  }

  private void OnClickCopyBuilding()
  {
    if (!Util.IsNullOrDestroyed((object) this.LastSelectedBuilding) && ((Component) this.LastSelectedBuilding).gameObject.activeInHierarchy)
    {
      PlanScreen.Instance.CopyBuildingOrder(this.LastSelectedBuilding);
    }
    else
    {
      if (!Object.op_Inequality((Object) this.lastSelectedBuildingDef, (Object) null))
        return;
      PlanScreen.Instance.CopyBuildingOrder(this.lastSelectedBuildingDef, this.LastSelectedBuildingFacade);
    }
  }

  private Building LastSelectedBuilding
  {
    get => this.lastSelectedBuilding;
    set
    {
      this.lastSelectedBuilding = value;
      if (!Object.op_Inequality((Object) this.lastSelectedBuilding, (Object) null))
        return;
      this.lastSelectedBuildingDef = this.lastSelectedBuilding.Def;
      if (!((Component) this.lastSelectedBuilding).gameObject.activeInHierarchy)
        return;
      this.LastSelectedBuildingFacade = ((Component) this.lastSelectedBuilding).GetComponent<BuildingFacade>().CurrentFacade;
    }
  }

  public string LastSelectedBuildingFacade
  {
    get => this.lastSelectedBuildingFacade;
    set => this.lastSelectedBuildingFacade = value;
  }

  public void RefreshCopyBuildingButton(object data = null)
  {
    RectTransform rectTransform = Util.rectTransform(this.adjacentPinnedButtons);
    double x = (double) Util.rectTransform(((Component) this).gameObject).sizeDelta.x;
    Rect rect = Util.rectTransform((Component) ((KMonoBehaviour) this).transform.parent).rect;
    double width = (double) ((Rect) ref rect).width;
    Vector2 vector2 = new Vector2(Mathf.Min((float) x, (float) width), 0.0f);
    rectTransform.anchoredPosition = vector2;
    MultiToggle component1 = this.copyBuildingButton.GetComponent<MultiToggle>();
    if (Object.op_Inequality((Object) SelectTool.Instance, (Object) null) && Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) null))
    {
      Building component2 = ((Component) SelectTool.Instance.selected).GetComponent<Building>();
      if (Object.op_Inequality((Object) component2, (Object) null) && component2.Def.ShouldShowInBuildMenu() && component2.Def.IsAvailable())
        this.LastSelectedBuilding = component2;
    }
    if (Object.op_Inequality((Object) this.lastSelectedBuildingDef, (Object) null))
    {
      ((Component) component1).gameObject.SetActive(((Component) PlanScreen.Instance).gameObject.activeInHierarchy);
      Sprite sprite = this.lastSelectedBuildingDef.GetUISprite();
      if (this.LastSelectedBuildingFacade != null && this.LastSelectedBuildingFacade != "DEFAULT_FACADE")
        sprite = Def.GetFacadeUISprite(this.LastSelectedBuildingFacade);
      ((Component) component1.transform.Find("FG")).GetComponent<Image>().sprite = sprite;
      ((Graphic) ((Component) component1.transform.Find("FG")).GetComponent<Image>()).color = Color.white;
      component1.ChangeState(1);
    }
    else
    {
      ((Component) component1).gameObject.SetActive(false);
      component1.ChangeState(0);
    }
  }

  public void RefreshToolTip()
  {
    for (int index = 0; index < TUNING.BUILDINGS.PLANORDER.Count; ++index)
    {
      PlanScreen.PlanInfo planInfo = TUNING.BUILDINGS.PLANORDER[index];
      if (DlcManager.IsContentActive(planInfo.RequiredDlcId))
      {
        Action action = index < 14 ? (Action) (36 + index) : (Action) 275;
        string upper = HashCache.Get().Get(planInfo.category).ToUpper();
        this.toggleInfo[index].tooltip = GameUtil.ReplaceHotkeyString(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + upper + ".TOOLTIP")), action);
      }
    }
    this.copyBuildingButton.GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.COPY_BUILDING_TOOLTIP, (Action) 50));
  }

  public void Refresh()
  {
    List<KIconToggleMenu.ToggleInfo> toggleInfo = new List<KIconToggleMenu.ToggleInfo>();
    if (this.tagCategoryMap != null)
      return;
    int building_index = 0;
    this.tagCategoryMap = new Dictionary<Tag, HashedString>();
    this.tagOrderMap = new Dictionary<Tag, int>();
    if (TUNING.BUILDINGS.PLANORDER.Count > 14)
      DebugUtil.LogWarningArgs(new object[2]
      {
        (object) "Insufficient keys to cover root plan menu",
        (object) ("Max of 14 keys supported but TUNING.BUILDINGS.PLANORDER has " + TUNING.BUILDINGS.PLANORDER.Count.ToString())
      });
    this.toggleEntries.Clear();
    for (int index = 0; index < TUNING.BUILDINGS.PLANORDER.Count; ++index)
    {
      PlanScreen.PlanInfo planInfo = TUNING.BUILDINGS.PLANORDER[index];
      if (DlcManager.IsContentActive(planInfo.RequiredDlcId))
      {
        Action action = index < 14 ? (Action) (36 + index) : (Action) 275;
        string iconName = PlanScreen.iconNameMap[planInfo.category];
        string upper = HashCache.Get().Get(planInfo.category).ToUpper();
        KIconToggleMenu.ToggleInfo toggle_info = new KIconToggleMenu.ToggleInfo(STRINGS.UI.StripLinkFormatting(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + upper + ".NAME"))), iconName, (object) planInfo.category, action, GameUtil.ReplaceHotkeyString(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.BUILDCATEGORIES." + upper + ".TOOLTIP")), action));
        toggleInfo.Add(toggle_info);
        PlanScreen.PopulateOrderInfo(planInfo.category, (object) planInfo.buildingAndSubcategoryData, this.tagCategoryMap, this.tagOrderMap, ref building_index);
        List<BuildingDef> building_defs = new List<BuildingDef>();
        foreach (BuildingDef buildingDef in Assets.BuildingDefs)
        {
          HashedString hashedString;
          if (buildingDef.IsAvailable() && this.tagCategoryMap.TryGetValue(buildingDef.Tag, out hashedString) && !HashedString.op_Inequality(hashedString, planInfo.category))
            building_defs.Add(buildingDef);
        }
        this.toggleEntries.Add(new PlanScreen.ToggleEntry(toggle_info, planInfo.category, building_defs, planInfo.hideIfNotResearched));
      }
    }
    this.Setup((IList<KIconToggleMenu.ToggleInfo>) toggleInfo);
    this.toggles.ForEach((Action<KToggle>) (to =>
    {
      foreach (ImageToggleState component in ((Component) to).GetComponents<ImageToggleState>())
      {
        if (Object.op_Inequality((Object) component.TargetImage.sprite, (Object) null) && ((Object) component.TargetImage).name == "FG" && !component.useSprites)
          component.SetSprites(Assets.GetSprite(HashedString.op_Implicit(((Object) component.TargetImage.sprite).name + "_disabled")), component.TargetImage.sprite, component.TargetImage.sprite, Assets.GetSprite(HashedString.op_Implicit(((Object) component.TargetImage.sprite).name + "_disabled")));
      }
      ((WidgetSoundPlayer) ((Component) to).GetComponent<KToggle>().soundPlayer).Enabled = false;
      ((TMP_Text) ((Component) to).GetComponentInChildren<LocText>()).fontSize = ScreenResolutionMonitor.UsingGamepadUIMode() ? (float) PlanScreen.fontSizeBigMode : (float) PlanScreen.fontSizeStandardMode;
    }));
    for (int index = 0; index < this.toggleEntries.Count; ++index)
    {
      PlanScreen.ToggleEntry toggleEntry = this.toggleEntries[index];
      toggleEntry.CollectToggleImages();
      this.toggleEntries[index] = toggleEntry;
    }
    this.ForceUpdateAllCategoryToggles();
  }

  private void ForceUpdateAllCategoryToggles(object data = null) => this.forceUpdateAllCategoryToggles = true;

  public void CopyBuildingOrder(BuildingDef buildingDef, string facadeID)
  {
    foreach (PlanScreen.PlanInfo planInfo in TUNING.BUILDINGS.PLANORDER)
    {
      foreach (KeyValuePair<string, string> keyValuePair in planInfo.buildingAndSubcategoryData)
      {
        if (buildingDef.PrefabID == keyValuePair.Key)
        {
          this.OpenCategoryByName(HashCache.Get().Get(planInfo.category));
          this.OnSelectBuilding(((Component) this.ActiveCategoryBuildingToggles[buildingDef]).gameObject, buildingDef, facadeID);
          break;
        }
      }
    }
  }

  public void CopyBuildingOrder(Building building)
  {
    this.CopyBuildingOrder(building.Def, ((Component) building).GetComponent<BuildingFacade>().CurrentFacade);
    if (Object.op_Equality((Object) this.ProductInfoScreen.materialSelectionPanel, (Object) null))
    {
      DebugUtil.DevLogError(((Object) building.Def).name + " def likely needs to be marked def.ShowInBuildMenu = false");
    }
    else
    {
      this.ProductInfoScreen.materialSelectionPanel.SelectSourcesMaterials(building);
      Rotatable component = ((Component) building).GetComponent<Rotatable>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      BuildTool.Instance.SetToolOrientation(component.GetOrientation());
    }
  }

  private static void PopulateOrderInfo(
    HashedString category,
    object data,
    Dictionary<Tag, HashedString> category_map,
    Dictionary<Tag, int> order_map,
    ref int building_index)
  {
    if (data.GetType() == typeof (PlanScreen.PlanInfo))
    {
      PlanScreen.PlanInfo planInfo = (PlanScreen.PlanInfo) data;
      PlanScreen.PopulateOrderInfo(planInfo.category, (object) planInfo.buildingAndSubcategoryData, category_map, order_map, ref building_index);
    }
    else
    {
      foreach (KeyValuePair<string, string> keyValuePair in (List<KeyValuePair<string, string>>) data)
      {
        Tag key;
        // ISSUE: explicit constructor call
        ((Tag) ref key).\u002Ector(keyValuePair.Key);
        category_map[key] = category;
        order_map[key] = building_index;
        ++building_index;
      }
    }
  }

  protected virtual void OnCmpEnable()
  {
    this.Refresh();
    this.RefreshCopyBuildingButton();
  }

  protected virtual void OnCmpDisable() => this.ClearButtons();

  private void ClearButtons()
  {
    foreach (KeyValuePair<string, GameObject> subCategoryObject in this.currentSubCategoryObjects)
      subCategoryObject.Value.gameObject.SetActive(false);
    this.currentSubCategoryObjects.Clear();
    foreach (KeyValuePair<BuildingDef, PlanBuildingToggle> categoryBuildingToggle in this.ActiveCategoryBuildingToggles)
      ((Component) categoryBuildingToggle.Value).gameObject.SetActive(false);
    this.ActiveCategoryBuildingToggles.Clear();
    this.copyBuildingButton.gameObject.SetActive(false);
    this.copyBuildingButton.GetComponent<MultiToggle>().ChangeState(0);
  }

  public void OnSelectBuilding(GameObject button_go, BuildingDef def, string facadeID = null)
  {
    if (Object.op_Equality((Object) button_go, (Object) null))
      Debug.Log((object) "Button gameObject is null", (Object) ((Component) this).gameObject);
    else if (Object.op_Equality((Object) button_go, (Object) this.SelectedBuildingGameObject))
    {
      this.CloseRecipe(true);
    }
    else
    {
      ++this.ignoreToolChangeMessages;
      this.SelectedBuildingGameObject = button_go;
      this.currentlySelectedToggle = button_go.GetComponent<KToggle>();
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
      PlanScreen.ToggleEntry toggleEntry;
      if (this.GetToggleEntryForCategory(this.tagCategoryMap[def.Tag], out toggleEntry) && toggleEntry.pendingResearchAttentions.Contains(def.Tag))
      {
        toggleEntry.pendingResearchAttentions.Remove(def.Tag);
        button_go.GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
        if (toggleEntry.pendingResearchAttentions.Count == 0)
          ((Component) toggleEntry.toggleInfo.toggle).GetComponent<PlanCategoryNotifications>().ToggleAttention(false);
      }
      this.ProductInfoScreen.ClearProduct(false);
      ToolMenu.Instance.ClearSelection();
      PrebuildTool.Instance.Activate(def, this.GetTooltipForBuildable(def));
      this.LastSelectedBuilding = def.BuildingComplete.GetComponent<Building>();
      this.RefreshCopyBuildingButton();
      this.ProductInfoScreen.Show(true);
      this.ProductInfoScreen.ConfigureScreen(def, facadeID);
      --this.ignoreToolChangeMessages;
    }
  }

  private void RefreshBuildableStates(bool force_update)
  {
    if (Assets.BuildingDefs == null || Assets.BuildingDefs.Count == 0)
      return;
    if ((double) this.timeSinceNotificationPing < (double) this.specialNotificationEmbellishDelay)
      this.timeSinceNotificationPing += Time.unscaledDeltaTime;
    if ((double) this.timeSinceNotificationPing >= (double) this.notificationPingExpire)
      this.notificationPingCount = 0;
    int num = 10;
    if (force_update)
    {
      num = Assets.BuildingDefs.Count;
      this.buildable_state_update_idx = 0;
    }
    ListPool<HashedString, PlanScreen>.PooledList pooledList = ListPool<HashedString, PlanScreen>.Allocate();
    for (int index = 0; index < num; ++index)
    {
      this.buildable_state_update_idx = (this.buildable_state_update_idx + 1) % Assets.BuildingDefs.Count;
      BuildingDef buildingDef = Assets.BuildingDefs[this.buildable_state_update_idx];
      PlanScreen.RequirementsState buildableStateForDef = this.GetBuildableStateForDef(buildingDef);
      HashedString hashedString;
      if (this.tagCategoryMap.TryGetValue(buildingDef.Tag, out hashedString) && this._buildableStatesByID[buildingDef.PrefabID] != buildableStateForDef)
      {
        this._buildableStatesByID[buildingDef.PrefabID] = buildableStateForDef;
        if (Object.op_Equality((Object) this.ProductInfoScreen.currentDef, (Object) buildingDef))
        {
          ++this.ignoreToolChangeMessages;
          this.ProductInfoScreen.ClearProduct(false);
          this.ProductInfoScreen.Show(true);
          this.ProductInfoScreen.ConfigureScreen(buildingDef);
          --this.ignoreToolChangeMessages;
        }
        if (buildableStateForDef == PlanScreen.RequirementsState.Complete)
        {
          foreach (KIconToggleMenu.ToggleInfo toggleInfo in (IEnumerable<KIconToggleMenu.ToggleInfo>) this.toggleInfo)
          {
            if (HashedString.op_Equality((HashedString) toggleInfo.userData, hashedString))
            {
              Bouncer component = ((Component) toggleInfo.toggle).GetComponent<Bouncer>();
              if (Object.op_Inequality((Object) component, (Object) null) && !component.IsBouncing() && !((List<HashedString>) pooledList).Contains(hashedString))
              {
                ((List<HashedString>) pooledList).Add(hashedString);
                component.Bounce();
                if ((double) KTime.Instance.UnscaledGameTime - (double) this.initTime > 1.5)
                {
                  if ((double) this.timeSinceNotificationPing >= (double) this.specialNotificationEmbellishDelay)
                  {
                    string sound = GlobalAssets.GetSound("NewBuildable_Embellishment");
                    if (sound != null)
                      SoundEvent.EndOneShot(SoundEvent.BeginOneShot(sound, TransformExtensions.GetPosition(((Component) SoundListenerController.Instance).transform)));
                  }
                  string sound1 = GlobalAssets.GetSound("NewBuildable");
                  if (sound1 != null)
                  {
                    EventInstance instance = SoundEvent.BeginOneShot(sound1, TransformExtensions.GetPosition(((Component) SoundListenerController.Instance).transform));
                    ((EventInstance) ref instance).setParameterByName("playCount", (float) this.notificationPingCount, false);
                    SoundEvent.EndOneShot(instance);
                  }
                }
                this.timeSinceNotificationPing = 0.0f;
                ++this.notificationPingCount;
              }
            }
          }
        }
      }
    }
    pooledList.Recycle();
  }

  private PlanScreen.RequirementsState GetBuildableStateForDef(BuildingDef def)
  {
    if (!def.IsAvailable())
      return PlanScreen.RequirementsState.Invalid;
    PlanScreen.RequirementsState buildableStateForDef = PlanScreen.RequirementsState.Complete;
    if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !this.IsDefResearched(def))
      buildableStateForDef = PlanScreen.RequirementsState.Tech;
    else if (def.BuildingComplete.HasTag(GameTags.Telepad) && ClusterUtil.ActiveWorldHasPrinter())
      buildableStateForDef = PlanScreen.RequirementsState.TelepadBuilt;
    else if (def.BuildingComplete.HasTag(GameTags.RocketInteriorBuilding) && !ClusterUtil.ActiveWorldIsRocketInterior())
      buildableStateForDef = PlanScreen.RequirementsState.RocketInteriorOnly;
    else if (def.BuildingComplete.HasTag(GameTags.NotRocketInteriorBuilding) && ClusterUtil.ActiveWorldIsRocketInterior())
      buildableStateForDef = PlanScreen.RequirementsState.RocketInteriorForbidden;
    else if (def.BuildingComplete.HasTag(GameTags.UniquePerWorld) && BuildingInventory.Instance.BuildingCountForWorld_BAD_PERF(def.Tag, ClusterManager.Instance.activeWorldId) > 0)
      buildableStateForDef = PlanScreen.RequirementsState.UniquePerWorld;
    else if (!DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && !ProductInfoScreen.MaterialsMet(def.CraftRecipe))
      buildableStateForDef = PlanScreen.RequirementsState.Materials;
    return buildableStateForDef;
  }

  private void SetCategoryButtonState()
  {
    this.nextCategoryToUpdateIDX = (this.nextCategoryToUpdateIDX + 1) % this.toggleEntries.Count;
    for (int index = 0; index < this.toggleEntries.Count; ++index)
    {
      if (this.forceUpdateAllCategoryToggles || index == this.nextCategoryToUpdateIDX)
      {
        PlanScreen.ToggleEntry toggleEntry = this.toggleEntries[index];
        KIconToggleMenu.ToggleInfo toggleInfo = toggleEntry.toggleInfo;
        toggleInfo.toggle.ActivateFlourish(this.activeCategoryInfo != null && toggleInfo.userData == this.activeCategoryInfo.userData);
        bool flag1 = false;
        bool flag2 = true;
        if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
        {
          flag1 = true;
          flag2 = false;
        }
        else
        {
          foreach (BuildingDef buildingDef in toggleEntry.buildingDefs)
          {
            if (this.GetBuildableState(buildingDef) == PlanScreen.RequirementsState.Complete)
            {
              flag1 = true;
              flag2 = false;
              break;
            }
          }
          if (flag2 && toggleEntry.AreAnyRequiredTechItemsAvailable())
            flag2 = false;
        }
        this.CategoryInteractive[toggleInfo] = !flag2;
        GameObject gameObject = ((Component) ((Component) toggleInfo.toggle.fgImage).transform.Find("ResearchIcon")).gameObject;
        if (!flag1)
        {
          if (flag2 && toggleEntry.hideIfNotResearched)
            ((Component) toggleInfo.toggle).gameObject.SetActive(false);
          else if (flag2)
          {
            ((Component) toggleInfo.toggle).gameObject.SetActive(true);
            KMonoBehaviourExtensions.SetAlpha(toggleInfo.toggle.fgImage, 0.2509804f);
            gameObject.gameObject.SetActive(true);
          }
          else
          {
            ((Component) toggleInfo.toggle).gameObject.SetActive(true);
            KMonoBehaviourExtensions.SetAlpha(toggleInfo.toggle.fgImage, 1f);
            gameObject.gameObject.SetActive(false);
          }
          ImageToggleState.State state = this.activeCategoryInfo == null || toggleInfo.userData != this.activeCategoryInfo.userData ? (ImageToggleState.State) 0 : (ImageToggleState.State) 3;
          foreach (ImageToggleState toggleImage in toggleEntry.toggleImages)
            toggleImage.SetState(state);
        }
        else
        {
          ((Component) toggleInfo.toggle).gameObject.SetActive(true);
          KMonoBehaviourExtensions.SetAlpha(toggleInfo.toggle.fgImage, 1f);
          gameObject.gameObject.SetActive(false);
          ImageToggleState.State state = this.activeCategoryInfo == null || toggleInfo.userData != this.activeCategoryInfo.userData ? (ImageToggleState.State) 1 : (ImageToggleState.State) 2;
          foreach (ImageToggleState toggleImage in toggleEntry.toggleImages)
            toggleImage.SetState(state);
        }
      }
    }
    this.RefreshCopyBuildingButton();
    this.forceUpdateAllCategoryToggles = false;
  }

  private void DeactivateBuildTools()
  {
    InterfaceTool activeTool = PlayerController.Instance.ActiveTool;
    if (!Object.op_Inequality((Object) activeTool, (Object) null))
      return;
    System.Type type = ((object) activeTool).GetType();
    if (!(type == typeof (BuildTool)) && !typeof (BaseUtilityBuildTool).IsAssignableFrom(type) && !(type == typeof (PrebuildTool)))
      return;
    activeTool.DeactivateTool();
    PlayerController.Instance.ActivateTool((InterfaceTool) SelectTool.Instance);
  }

  public void CloseRecipe(bool playSound = false)
  {
    if (playSound)
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect"));
    if (PlayerController.Instance.ActiveTool is PrebuildTool || PlayerController.Instance.ActiveTool is BuildTool)
      ToolMenu.Instance.ClearSelection();
    this.DeactivateBuildTools();
    if (Object.op_Inequality((Object) this.ProductInfoScreen, (Object) null))
      this.ProductInfoScreen.ClearProduct();
    if (this.activeCategoryInfo != null)
      this.UpdateBuildingButtonList(this.activeCategoryInfo);
    this.SelectedBuildingGameObject = (GameObject) null;
  }

  private void CloseCategoryPanel(bool playSound = true)
  {
    this.activeCategoryInfo = (KIconToggleMenu.ToggleInfo) null;
    if (playSound)
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
    ((Component) this.buildingGroupsRoot).GetComponent<ExpandRevealUIContent>().Collapse((Action<object>) (s =>
    {
      this.ClearButtons();
      ((Component) this.buildingGroupsRoot).gameObject.SetActive(false);
      this.ForceUpdateAllCategoryToggles();
    }));
    ((TMP_Text) this.PlanCategoryLabel).text = "";
    this.ForceUpdateAllCategoryToggles();
  }

  private void OnClickCategory(KIconToggleMenu.ToggleInfo toggle_info)
  {
    this.CloseRecipe();
    if (!this.CategoryInteractive.ContainsKey(toggle_info) || !this.CategoryInteractive[toggle_info])
    {
      this.CloseCategoryPanel(false);
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
    }
    else
    {
      if (this.activeCategoryInfo == toggle_info)
        this.CloseCategoryPanel();
      else
        this.OpenCategoryPanel(toggle_info);
      this.ConfigurePanelSize();
      this.SetScrollPoint(0.0f);
    }
  }

  private void OpenCategoryPanel(KIconToggleMenu.ToggleInfo toggle_info, bool play_sound = true)
  {
    HashedString userData = (HashedString) toggle_info.userData;
    this.ClearButtons();
    ((Component) this.buildingGroupsRoot).gameObject.SetActive(true);
    this.activeCategoryInfo = toggle_info;
    if (play_sound)
      UISounds.PlaySound(UISounds.Sound.ClickObject);
    this.BuildButtonList(userData, ((Component) this.GroupsTransform).gameObject);
    this.ForceUpdateAllCategoryToggles();
    ((TMP_Text) this.PlanCategoryLabel).text = this.activeCategoryInfo.text.ToUpper();
    ((Component) this.buildingGroupsRoot).GetComponent<ExpandRevealUIContent>().Expand((Action<object>) null);
  }

  public void OpenCategoryByName(string category)
  {
    PlanScreen.ToggleEntry toggleEntry;
    if (!this.GetToggleEntryForCategory(HashedString.op_Implicit(category), out toggleEntry))
      return;
    this.OpenCategoryPanel(toggleEntry.toggleInfo, false);
    this.ConfigurePanelSize();
  }

  private void UpdateBuildingButtonList(KIconToggleMenu.ToggleInfo toggle_info)
  {
    KToggle toggle = toggle_info.toggle;
    if (Object.op_Equality((Object) toggle, (Object) null))
    {
      foreach (KIconToggleMenu.ToggleInfo toggleInfo in (IEnumerable<KIconToggleMenu.ToggleInfo>) this.toggleInfo)
      {
        if (toggleInfo.userData == toggle_info.userData)
        {
          toggle = toggleInfo.toggle;
          break;
        }
      }
    }
    if (Object.op_Inequality((Object) toggle, (Object) null) && this.ActiveCategoryBuildingToggles.Count != 0)
    {
      for (int index = 0; index < this.maxToggleRefreshPerFrame; ++index)
      {
        if (this.building_button_refresh_idx >= this.ActiveCategoryBuildingToggles.Count)
          this.building_button_refresh_idx = 0;
        this.categoryPanelSizeNeedsRefresh = this.ActiveCategoryBuildingToggles.ElementAt<KeyValuePair<BuildingDef, PlanBuildingToggle>>(this.building_button_refresh_idx).Value.Refresh() || this.categoryPanelSizeNeedsRefresh;
        ++this.building_button_refresh_idx;
      }
    }
    if (this.categoryPanelSizeNeedsRefresh && this.building_button_refresh_idx >= this.ActiveCategoryBuildingToggles.Count)
    {
      this.categoryPanelSizeNeedsRefresh = false;
      this.ConfigurePanelSize();
    }
    if (!((Component) this.ProductInfoScreen).gameObject.activeSelf)
      return;
    this.ProductInfoScreen.materialSelectionPanel.UpdateResourceToggleValues();
  }

  public virtual void ScreenUpdate(bool topLevel)
  {
    base.ScreenUpdate(topLevel);
    this.RefreshBuildableStates(false);
    this.SetCategoryButtonState();
    if (this.activeCategoryInfo == null)
      return;
    this.UpdateBuildingButtonList(this.activeCategoryInfo);
  }

  private void BuildButtonList(HashedString plan_category, GameObject parent)
  {
    this.ActiveCategoryBuildingToggles.Clear();
    int btnIndex = 0;
    string plan_category1 = plan_category.ToString();
    Dictionary<string, List<BuildingDef>> dictionary = new Dictionary<string, List<BuildingDef>>();
    foreach (KeyValuePair<string, string> keyValuePair in TUNING.BUILDINGS.PLANORDER.Find((Predicate<PlanScreen.PlanInfo>) (match => HashedString.op_Equality(match.category, plan_category))).buildingAndSubcategoryData)
    {
      BuildingDef buildingDef = Assets.GetBuildingDef(keyValuePair.Key);
      if (buildingDef.IsAvailable() && buildingDef.ShouldShowInBuildMenu())
      {
        if (this.USE_SUB_CATEGORY_LAYOUT)
        {
          if (!dictionary.ContainsKey(keyValuePair.Value))
            dictionary.Add(keyValuePair.Value, new List<BuildingDef>());
          dictionary[keyValuePair.Value].Add(buildingDef);
        }
        else
        {
          if (!dictionary.ContainsKey("default"))
            dictionary.Add("default", new List<BuildingDef>());
          dictionary["default"].Add(buildingDef);
        }
      }
    }
    PlanScreen.ToggleEntry toggleEntry = (PlanScreen.ToggleEntry) null;
    this.GetToggleEntryForCategory(plan_category, out toggleEntry);
    this.currentSubCategoryObjects.Clear();
    foreach (KeyValuePair<string, List<BuildingDef>> keyValuePair in dictionary)
    {
      if (!this.allSubCategoryObjects.ContainsKey(keyValuePair.Key))
        this.allSubCategoryObjects.Add(keyValuePair.Key, Util.KInstantiateUI(this.subgroupPrefab, parent, true));
      this.allSubCategoryObjects[keyValuePair.Key].SetActive(true);
      this.currentSubCategoryObjects.Add(keyValuePair.Key, this.allSubCategoryObjects[keyValuePair.Key]);
      GameObject gameObject = ((Component) this.currentSubCategoryObjects[keyValuePair.Key].GetComponent<HierarchyReferences>().GetReference<GridLayoutGroup>("Grid")).gameObject;
      ((Component) this.currentSubCategoryObjects[keyValuePair.Key].GetComponent<HierarchyReferences>().GetReference<RectTransform>("Header")).gameObject.SetActive(this.USE_SUB_CATEGORY_LAYOUT);
      foreach (BuildingDef def in keyValuePair.Value)
      {
        GameObject button = this.CreateButton(def, gameObject, plan_category1, btnIndex);
        if (toggleEntry != null && toggleEntry.pendingResearchAttentions.Contains(Tag.op_Implicit(def.PrefabID)))
          button.GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
        ++btnIndex;
      }
    }
    this.RefreshScale();
  }

  private void ConfigurePanelSize(object data = null)
  {
    this.buildGrid_bg_rowHeight = ScreenResolutionMonitor.UsingGamepadUIMode() ? PlanScreen.bigBuildingButtonSize.y : PlanScreen.standarduildingButtonSize.y;
    this.buildGrid_bg_rowHeight += this.subgroupPrefab.GetComponent<HierarchyReferences>().GetReference<GridLayoutGroup>("Grid").spacing.y;
    int num1 = 0;
    for (int index1 = 0; index1 < this.GroupsTransform.childCount; ++index1)
    {
      int num2 = 0;
      HierarchyReferences component = ((Component) this.GroupsTransform.GetChild(index1)).GetComponent<HierarchyReferences>();
      if (!Object.op_Equality((Object) component, (Object) null))
      {
        GridLayoutGroup reference = component.GetReference<GridLayoutGroup>("Grid");
        if (!Object.op_Equality((Object) reference, (Object) null))
        {
          for (int index2 = 0; index2 < ((Component) reference).transform.childCount; ++index2)
          {
            if (((Component) ((Component) reference).transform.GetChild(index2)).gameObject.activeSelf)
              ++num2;
          }
          num1 += num2 / reference.constraintCount;
          if (num2 % reference.constraintCount != 0)
            ++num1;
        }
      }
    }
    int num3 = num1;
    int num4 = Math.Min(Math.Max(1, Screen.height / (int) this.buildGrid_bg_rowHeight - 3), 6);
    ((Component) ((Component) this.BuildingGroupContentsRect).GetComponent<ScrollRect>().verticalScrollbar).gameObject.SetActive(num3 >= num4 - 1);
    float num5 = this.buildGrid_bg_borderHeight + (float) Mathf.Clamp(num3, 0, num4) * this.buildGrid_bg_rowHeight;
    if (this.USE_SUB_CATEGORY_LAYOUT)
    {
      float minHeight = ((Component) this.subgroupPrefab.GetComponent<HierarchyReferences>().GetReference("HeaderLabel").transform.parent).GetComponent<LayoutElement>().minHeight;
      num5 += minHeight;
    }
    this.buildingGroupsRoot.sizeDelta = new Vector2(this.buildGrid_bg_width, num5);
    this.RefreshScale();
  }

  private void SetScrollPoint(float targetY) => this.BuildingGroupContentsRect.anchoredPosition = new Vector2(this.BuildingGroupContentsRect.anchoredPosition.x, targetY);

  private GameObject CreateButton(
    BuildingDef def,
    GameObject parent,
    string plan_category,
    int btnIndex)
  {
    GameObject button;
    PlanBuildingToggle componentInChildren;
    if (this.allBuildingToggles.ContainsKey(def.PrefabID))
    {
      button = this.allBuildingToggles[def.PrefabID];
      componentInChildren = button.GetComponentInChildren<PlanBuildingToggle>();
      componentInChildren.Refresh();
    }
    else
    {
      button = Util.KInstantiateUI(this.planButtonPrefab, parent, true);
      ((Object) button).name = STRINGS.UI.StripLinkFormatting(((Object) def).name) + " Group:" + plan_category;
      componentInChildren = button.GetComponentInChildren<PlanBuildingToggle>();
      componentInChildren.Config(def);
      ((WidgetSoundPlayer) componentInChildren.soundPlayer).Enabled = false;
      this.allBuildingToggles.Add(def.PrefabID, button);
    }
    this.ActiveCategoryBuildingToggles.Add(def, componentInChildren);
    return button;
  }

  public static bool TechRequirementsMet(TechItem techItem) => DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || techItem == null || techItem.IsComplete();

  private static bool TechRequirementsUpcoming(TechItem techItem) => PlanScreen.TechRequirementsMet(techItem);

  private bool GetToggleEntryForCategory(
    HashedString category,
    out PlanScreen.ToggleEntry toggleEntry)
  {
    toggleEntry = (PlanScreen.ToggleEntry) null;
    foreach (PlanScreen.ToggleEntry toggleEntry1 in this.toggleEntries)
    {
      if (HashedString.op_Equality(toggleEntry1.planCategory, category))
      {
        toggleEntry = toggleEntry1;
        return true;
      }
    }
    return false;
  }

  public bool IsDefBuildable(BuildingDef def) => this.GetBuildableState(def) == PlanScreen.RequirementsState.Complete;

  public string GetTooltipForBuildable(BuildingDef def)
  {
    PlanScreen.RequirementsState buildableState = this.GetBuildableState(def);
    return PlanScreen.GetTooltipForRequirementsState(def, buildableState);
  }

  public static string GetTooltipForRequirementsState(
    BuildingDef def,
    PlanScreen.RequirementsState state)
  {
    TechItem techItem = Db.Get().TechItems.TryGet(def.PrefabID);
    string requirementsState = (string) null;
    if (Game.Instance.SandboxModeActive)
      requirementsState = UIConstants.ColorPrefixYellow + (string) STRINGS.UI.SANDBOXTOOLS.SETTINGS.INSTANT_BUILD.NAME + UIConstants.ColorSuffix;
    else if (DebugHandler.InstantBuildMode)
    {
      requirementsState = UIConstants.ColorPrefixYellow + (string) STRINGS.UI.DEBUG_TOOLS.DEBUG_ACTIVE + UIConstants.ColorSuffix;
    }
    else
    {
      switch (state)
      {
        case PlanScreen.RequirementsState.Tech:
          requirementsState = string.Format((string) STRINGS.UI.PRODUCTINFO_REQUIRESRESEARCHDESC, (object) techItem.ParentTech.Name);
          break;
        case PlanScreen.RequirementsState.Materials:
          requirementsState = (string) STRINGS.UI.PRODUCTINFO_MISSINGRESOURCES_HOVER;
          using (List<Recipe.Ingredient>.Enumerator enumerator = def.CraftRecipe.Ingredients.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Recipe.Ingredient current = enumerator.Current;
              string str = string.Format("{0}{1}: {2}", (object) "• ", (object) current.tag.ProperName(), (object) GameUtil.GetFormattedMass(current.amount));
              requirementsState = requirementsState + "\n" + str;
            }
            break;
          }
        case PlanScreen.RequirementsState.TelepadBuilt:
          requirementsState = (string) STRINGS.UI.PRODUCTINFO_UNIQUE_PER_WORLD;
          break;
        case PlanScreen.RequirementsState.UniquePerWorld:
          requirementsState = (string) STRINGS.UI.PRODUCTINFO_UNIQUE_PER_WORLD;
          break;
        case PlanScreen.RequirementsState.RocketInteriorOnly:
          requirementsState = (string) STRINGS.UI.PRODUCTINFO_ROCKET_INTERIOR;
          break;
        case PlanScreen.RequirementsState.RocketInteriorForbidden:
          requirementsState = (string) STRINGS.UI.PRODUCTINFO_ROCKET_NOT_INTERIOR;
          break;
      }
    }
    return requirementsState;
  }

  private void PointerEnter(PointerEventData data) => this.planScreenScrollRect.mouseIsOver = true;

  private void PointerExit(PointerEventData data) => this.planScreenScrollRect.mouseIsOver = false;

  public override void OnKeyDown(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return;
    if (this.mouseOver && this.ConsumeMouseScroll)
    {
      if (KInputManager.currentControllerIsGamepad)
      {
        if (e.IsAction((Action) 7) || e.IsAction((Action) 8))
          this.planScreenScrollRect.OnKeyDown(e);
      }
      else if (!e.TryConsume((Action) 7))
        e.TryConsume((Action) 8);
    }
    if (e.IsAction((Action) 50) && e.TryConsume((Action) 50))
      this.OnClickCopyBuilding();
    if (this.toggles == null)
      return;
    if (!((KInputEvent) e).Consumed && this.activeCategoryInfo != null && e.TryConsume((Action) 1))
    {
      this.OnClickCategory(this.activeCategoryInfo);
      SelectTool.Instance.Activate();
      this.ClearSelection();
    }
    else
    {
      if (((KInputEvent) e).Consumed)
        return;
      base.OnKeyDown(e);
    }
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    if (this.mouseOver && this.ConsumeMouseScroll)
    {
      if (KInputManager.currentControllerIsGamepad)
      {
        if (e.IsAction((Action) 7) || e.IsAction((Action) 8))
          this.planScreenScrollRect.OnKeyUp(e);
      }
      else if (!e.TryConsume((Action) 7))
        e.TryConsume((Action) 8);
    }
    if (((KInputEvent) e).Consumed)
      return;
    if (Object.op_Inequality((Object) this.SelectedBuildingGameObject, (Object) null) && PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
    {
      this.CloseRecipe();
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close"));
    }
    else if (this.activeCategoryInfo != null && PlayerController.Instance.ConsumeIfNotDragging(e, (Action) 5))
      this.OnUIClear((object) null);
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyUp(e);
  }

  private void OnRecipeElementsFullySelected()
  {
    BuildingDef def = (BuildingDef) null;
    foreach (KeyValuePair<BuildingDef, PlanBuildingToggle> categoryBuildingToggle in this.ActiveCategoryBuildingToggles)
    {
      if (Object.op_Equality((Object) categoryBuildingToggle.Value, (Object) this.currentlySelectedToggle))
      {
        def = categoryBuildingToggle.Key;
        break;
      }
    }
    DebugUtil.DevAssert(Object.op_Implicit((Object) def), "def is null", (Object) null);
    if (!Object.op_Implicit((Object) def))
      return;
    if (def.isKAnimTile && def.isUtility)
    {
      IList<Tag> selectedElementAsList = this.ProductInfoScreen.materialSelectionPanel.GetSelectedElementAsList;
      (Object.op_Inequality((Object) def.BuildingComplete.GetComponent<Wire>(), (Object) null) ? (BaseUtilityBuildTool) WireBuildTool.Instance : (BaseUtilityBuildTool) UtilityBuildTool.Instance).Activate(def, selectedElementAsList);
    }
    else
      BuildTool.Instance.Activate(def, this.ProductInfoScreen.materialSelectionPanel.GetSelectedElementAsList, this.ProductInfoScreen.FacadeSelectionPanel.SelectedFacade);
  }

  public void OnResearchComplete(object tech)
  {
    foreach (Resource unlockedItem in ((Tech) tech).unlockedItems)
    {
      BuildingDef buildingDef = Assets.GetBuildingDef(unlockedItem.Id);
      if (Object.op_Inequality((Object) buildingDef, (Object) null))
      {
        this.UpdateDefResearched(buildingDef);
        PlanScreen.ToggleEntry toggleEntry;
        if (this.tagCategoryMap.ContainsKey(buildingDef.Tag) && this.GetToggleEntryForCategory(this.tagCategoryMap[buildingDef.Tag], out toggleEntry))
        {
          toggleEntry.pendingResearchAttentions.Add(buildingDef.Tag);
          ((Component) toggleEntry.toggleInfo.toggle).GetComponent<PlanCategoryNotifications>().ToggleAttention(true);
          toggleEntry.Refresh();
        }
      }
    }
  }

  private void OnUIClear(object data)
  {
    if (this.activeCategoryInfo == null)
      return;
    this.selected = -1;
    this.OnClickCategory(this.activeCategoryInfo);
    SelectTool.Instance.Activate();
    PlayerController.Instance.ActivateTool((InterfaceTool) SelectTool.Instance);
    SelectTool.Instance.Select((KSelectable) null, true);
  }

  private void OnActiveToolChanged(object data)
  {
    if (data == null || this.ignoreToolChangeMessages > 0)
      return;
    System.Type type = data.GetType();
    if (typeof (BuildTool).IsAssignableFrom(type) || typeof (PrebuildTool).IsAssignableFrom(type) || typeof (BaseUtilityBuildTool).IsAssignableFrom(type))
      return;
    this.CloseRecipe();
    this.CloseCategoryPanel(false);
  }

  public PrioritySetting GetBuildingPriority() => this.ProductInfoScreen.materialSelectionPanel.PriorityScreen.GetLastSelectedPriority();

  static PlanScreen()
  {
    Dictionary<HashedString, string> dictionary = new Dictionary<HashedString, string>();
    dictionary.Add(PlanScreen.CacheHashedString("Base"), "icon_category_base");
    dictionary.Add(PlanScreen.CacheHashedString("Oxygen"), "icon_category_oxygen");
    dictionary.Add(PlanScreen.CacheHashedString("Power"), "icon_category_electrical");
    dictionary.Add(PlanScreen.CacheHashedString("Food"), "icon_category_food");
    dictionary.Add(PlanScreen.CacheHashedString("Plumbing"), "icon_category_plumbing");
    dictionary.Add(PlanScreen.CacheHashedString("HVAC"), "icon_category_ventilation");
    dictionary.Add(PlanScreen.CacheHashedString("Refining"), "icon_category_refinery");
    dictionary.Add(PlanScreen.CacheHashedString("Medical"), "icon_category_medical");
    dictionary.Add(PlanScreen.CacheHashedString("Furniture"), "icon_category_furniture");
    dictionary.Add(PlanScreen.CacheHashedString("Equipment"), "icon_category_misc");
    dictionary.Add(PlanScreen.CacheHashedString("Utilities"), "icon_category_utilities");
    dictionary.Add(PlanScreen.CacheHashedString("Automation"), "icon_category_automation");
    dictionary.Add(PlanScreen.CacheHashedString("Conveyance"), "icon_category_shipping");
    dictionary.Add(PlanScreen.CacheHashedString("Rocketry"), "icon_category_rocketry");
    dictionary.Add(PlanScreen.CacheHashedString("HEP"), "icon_category_radiation");
    PlanScreen.iconNameMap = dictionary;
    PlanScreen.bigBuildingButtonSize = new Vector2(98f, 123f);
    PlanScreen.standarduildingButtonSize = Vector2.op_Multiply(PlanScreen.bigBuildingButtonSize, 0.8f);
    PlanScreen.fontSizeBigMode = 16;
    PlanScreen.fontSizeStandardMode = 14;
  }

  public struct PlanInfo
  {
    public HashedString category;
    public bool hideIfNotResearched;
    [Obsolete("Modders: Use ModUtil.AddBuildingToPlanScreen")]
    public List<string> data;
    public List<KeyValuePair<string, string>> buildingAndSubcategoryData;
    public string RequiredDlcId;

    public PlanInfo(
      HashedString category,
      bool hideIfNotResearched,
      List<string> listData,
      string RequiredDlcId = "")
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      foreach (string key in listData)
        keyValuePairList.Add(new KeyValuePair<string, string>(key, TUNING.BUILDINGS.PLANSUBCATEGORYSORTING.ContainsKey(key) ? TUNING.BUILDINGS.PLANSUBCATEGORYSORTING[key] : "uncategorized"));
      this.category = category;
      this.hideIfNotResearched = hideIfNotResearched;
      this.data = listData;
      this.buildingAndSubcategoryData = keyValuePairList;
      this.RequiredDlcId = RequiredDlcId;
    }
  }

  [Serializable]
  public struct BuildingToolTipSettings
  {
    public TextStyleSetting BuildButtonName;
    public TextStyleSetting BuildButtonDescription;
    public TextStyleSetting MaterialRequirement;
    public TextStyleSetting ResearchRequirement;
  }

  [Serializable]
  public struct BuildingNameTextSetting
  {
    public TextStyleSetting ActiveSelected;
    public TextStyleSetting ActiveDeselected;
    public TextStyleSetting InactiveSelected;
    public TextStyleSetting InactiveDeselected;
  }

  private class ToggleEntry
  {
    public KIconToggleMenu.ToggleInfo toggleInfo;
    public HashedString planCategory;
    public List<BuildingDef> buildingDefs;
    public List<Tag> pendingResearchAttentions;
    private List<TechItem> requiredTechItems;
    public ImageToggleState[] toggleImages;
    public bool hideIfNotResearched;
    private bool _areAnyRequiredTechItemsAvailable;

    public ToggleEntry(
      KIconToggleMenu.ToggleInfo toggle_info,
      HashedString plan_category,
      List<BuildingDef> building_defs,
      bool hideIfNotResearched)
    {
      this.toggleInfo = toggle_info;
      this.planCategory = plan_category;
      this.buildingDefs = building_defs;
      this.hideIfNotResearched = hideIfNotResearched;
      this.pendingResearchAttentions = new List<Tag>();
      this.requiredTechItems = new List<TechItem>();
      this.toggleImages = (ImageToggleState[]) null;
      foreach (BuildingDef buildingDef in building_defs)
      {
        TechItem techItem = Db.Get().TechItems.TryGet(buildingDef.PrefabID);
        if (techItem == null)
        {
          this.requiredTechItems.Clear();
          break;
        }
        if (!this.requiredTechItems.Contains(techItem))
          this.requiredTechItems.Add(techItem);
      }
      this._areAnyRequiredTechItemsAvailable = false;
      this.Refresh();
    }

    public bool AreAnyRequiredTechItemsAvailable() => this._areAnyRequiredTechItemsAvailable;

    public void Refresh()
    {
      if (this._areAnyRequiredTechItemsAvailable)
        return;
      if (this.requiredTechItems.Count == 0)
      {
        this._areAnyRequiredTechItemsAvailable = true;
      }
      else
      {
        foreach (TechItem requiredTechItem in this.requiredTechItems)
        {
          if (PlanScreen.TechRequirementsUpcoming(requiredTechItem))
          {
            this._areAnyRequiredTechItemsAvailable = true;
            break;
          }
        }
      }
    }

    public void CollectToggleImages() => this.toggleImages = ((Component) this.toggleInfo.toggle).gameObject.GetComponents<ImageToggleState>();
  }

  public enum RequirementsState
  {
    Invalid,
    Tech,
    Materials,
    Complete,
    TelepadBuilt,
    UniquePerWorld,
    RocketInteriorOnly,
    RocketInteriorForbidden,
  }
}
