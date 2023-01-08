// Decompiled with JetBrains decompiler
// Type: ResearchScreenSideBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ResearchScreenSideBar : KScreen
{
  [Header("Containers")]
  [SerializeField]
  private GameObject queueContainer;
  [SerializeField]
  private GameObject projectsContainer;
  [SerializeField]
  private GameObject searchFiltersContainer;
  [Header("Prefabs")]
  [SerializeField]
  private GameObject headerTechTypePrefab;
  [SerializeField]
  private GameObject filterButtonPrefab;
  [SerializeField]
  private GameObject techWidgetRootPrefab;
  [SerializeField]
  private GameObject techWidgetRootAltPrefab;
  [SerializeField]
  private GameObject techItemPrefab;
  [SerializeField]
  private GameObject techWidgetUnlockedItemPrefab;
  [SerializeField]
  private GameObject techWidgetRowPrefab;
  [SerializeField]
  private GameObject techCategoryPrefab;
  [SerializeField]
  private GameObject techCategoryPrefabAlt;
  [Header("Other references")]
  [SerializeField]
  private KInputTextField searchBox;
  [SerializeField]
  private MultiToggle allFilter;
  [SerializeField]
  private MultiToggle availableFilter;
  [SerializeField]
  private MultiToggle completedFilter;
  [SerializeField]
  private ResearchScreen researchScreen;
  [SerializeField]
  private KButton clearSearchButton;
  [SerializeField]
  private Color evenRowColor;
  [SerializeField]
  private Color oddRowColor;
  private ResearchScreenSideBar.CompletionState completionFilter;
  private Dictionary<string, bool> filterStates;
  private Dictionary<string, bool> categoryExpanded;
  private string currentSearchString;
  private Dictionary<string, GameObject> queueTechs;
  private Dictionary<string, GameObject> projectTechs;
  private Dictionary<string, GameObject> projectCategories;
  private Dictionary<string, GameObject> filterButtons;
  private Dictionary<string, Dictionary<string, GameObject>> projectTechItems;
  private Dictionary<string, List<Tag>> filterPresets;
  private List<GameObject> QueuedActivations;
  private List<GameObject> QueuedDeactivations;
  [SerializeField]
  private int activationPerFrame;
  private bool evenRow;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.PopualteProjects();
    this.PopulateFilterButtons();
    this.RefreshCategoriesContentExpanded();
    this.RefreshWidgets();
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.searchBox).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(UpdateCurrentSearch)));
    KInputTextField searchBox = this.searchBox;
    ((TMP_InputField) searchBox).onFocus = ((TMP_InputField) searchBox).onFocus + (System.Action) (() => this.isEditing = true);
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.searchBox).onEndEdit).AddListener(new UnityAction<string>((object) this, __methodptr(\u003COnSpawn\u003Eb__33_0)));
    this.clearSearchButton.onClick += (System.Action) (() =>
    {
      ((TMP_InputField) this.searchBox).text = "";
      foreach (KeyValuePair<string, GameObject> filterButton in this.filterButtons)
      {
        this.filterStates[filterButton.Key] = false;
        this.filterButtons[filterButton.Key].GetComponent<MultiToggle>().ChangeState(this.filterStates[filterButton.Key] ? 1 : 0);
      }
    });
    this.ConfigCompletionFilters();
    this.ConsumeMouseScroll = true;
    Game.Instance.Subscribe(-107300940, new Action<object>(this.UpdateProjectFilter));
  }

  private void Update()
  {
    for (int index = 0; index < Math.Min(this.QueuedActivations.Count, this.activationPerFrame); ++index)
      this.QueuedActivations[index].SetActive(true);
    this.QueuedActivations.RemoveRange(0, Math.Min(this.QueuedActivations.Count, this.activationPerFrame));
    for (int index = 0; index < Math.Min(this.QueuedDeactivations.Count, this.activationPerFrame); ++index)
      this.QueuedDeactivations[index].SetActive(false);
    this.QueuedDeactivations.RemoveRange(0, Math.Min(this.QueuedDeactivations.Count, this.activationPerFrame));
  }

  private void ConfigCompletionFilters()
  {
    this.allFilter.onClick += (System.Action) (() => this.SetCompletionFilter(ResearchScreenSideBar.CompletionState.All));
    this.completedFilter.onClick += (System.Action) (() => this.SetCompletionFilter(ResearchScreenSideBar.CompletionState.Completed));
    this.availableFilter.onClick += (System.Action) (() => this.SetCompletionFilter(ResearchScreenSideBar.CompletionState.Available));
    this.SetCompletionFilter(ResearchScreenSideBar.CompletionState.All);
  }

  private void SetCompletionFilter(ResearchScreenSideBar.CompletionState state)
  {
    this.completionFilter = state;
    ((Component) this.allFilter).GetComponent<MultiToggle>().ChangeState(this.completionFilter == ResearchScreenSideBar.CompletionState.All ? 1 : 0);
    ((Component) this.completedFilter).GetComponent<MultiToggle>().ChangeState(this.completionFilter == ResearchScreenSideBar.CompletionState.Completed ? 1 : 0);
    ((Component) this.availableFilter).GetComponent<MultiToggle>().ChangeState(this.completionFilter == ResearchScreenSideBar.CompletionState.Available ? 1 : 0);
    this.UpdateProjectFilter();
  }

  public virtual float GetSortKey() => this.isEditing ? 50f : 21f;

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (Object.op_Inequality((Object) this.researchScreen, (Object) null) && Object.op_Implicit((Object) this.researchScreen.canvas) && !((Behaviour) this.researchScreen.canvas).enabled)
      return;
    if (this.isEditing)
    {
      ((KInputEvent) e).Consumed = true;
    }
    else
    {
      if (((KInputEvent) e).Consumed)
        return;
      Vector2 vector2 = Vector2.op_Implicit(((Transform) Util.rectTransform((Component) ((KMonoBehaviour) this).transform)).InverseTransformPoint(KInputManager.GetMousePos()));
      if ((double) vector2.x < 0.0)
        return;
      double x = (double) vector2.x;
      Rect rect = Util.rectTransform((Component) ((KMonoBehaviour) this).transform).rect;
      double width = (double) ((Rect) ref rect).width;
      if (x > width || e.TryConsume((Action) 5) || e.TryConsume((Action) 3) || KInputManager.currentControllerIsGamepad || e.TryConsume((Action) 7))
        return;
      e.TryConsume((Action) 8);
    }
  }

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    this.RefreshWidgets();
  }

  private void UpdateCurrentSearch(string newValue)
  {
    if (this.isEditing)
    {
      foreach (KeyValuePair<string, GameObject> filterButton in this.filterButtons)
      {
        this.filterStates[filterButton.Key] = false;
        filterButton.Value.GetComponent<MultiToggle>().ChangeState(0);
      }
    }
    this.currentSearchString = newValue;
    this.UpdateProjectFilter();
  }

  private void UpdateProjectFilter(object data = null)
  {
    Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
    foreach (KeyValuePair<string, GameObject> projectCategory in this.projectCategories)
      dictionary.Add(projectCategory.Key, false);
    this.RefreshProjectsActive();
    foreach (KeyValuePair<string, GameObject> projectTech in this.projectTechs)
    {
      if ((projectTech.Value.activeSelf || this.QueuedActivations.Contains(projectTech.Value)) && !this.QueuedDeactivations.Contains(projectTech.Value))
      {
        dictionary[Db.Get().Techs.Get(projectTech.Key).category] = true;
        this.categoryExpanded[Db.Get().Techs.Get(projectTech.Key).category] = true;
      }
    }
    foreach (KeyValuePair<string, bool> keyValuePair in dictionary)
      this.ChangeGameObjectActive(this.projectCategories[keyValuePair.Key], keyValuePair.Value);
    this.RefreshCategoriesContentExpanded();
  }

  private void RefreshProjectsActive()
  {
    foreach (KeyValuePair<string, GameObject> projectTech in this.projectTechs)
    {
      bool flag1 = this.CheckTechPassesFilters(projectTech.Key);
      this.ChangeGameObjectActive(projectTech.Value, flag1);
      this.researchScreen.GetEntry(Db.Get().Techs.Get(projectTech.Key)).UpdateFilterState(flag1);
      foreach (KeyValuePair<string, GameObject> keyValuePair in this.projectTechItems[projectTech.Key])
      {
        bool flag2 = this.CheckTechItemPassesFilters(keyValuePair.Key);
        HierarchyReferences component = keyValuePair.Value.GetComponent<HierarchyReferences>();
        ((Graphic) component.GetReference<LocText>("Label")).color = flag2 ? Color.white : Color.grey;
        ((Graphic) component.GetReference<Image>("Icon")).color = flag2 ? Color.white : new Color(1f, 1f, 1f, 0.5f);
      }
    }
  }

  private void RefreshCategoriesContentExpanded()
  {
    foreach (KeyValuePair<string, GameObject> projectCategory in this.projectCategories)
    {
      ((Component) projectCategory.Value.GetComponent<HierarchyReferences>().GetReference<RectTransform>("Content")).gameObject.SetActive(this.categoryExpanded[projectCategory.Key]);
      projectCategory.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(this.categoryExpanded[projectCategory.Key] ? 1 : 0);
    }
  }

  private void PopualteProjects()
  {
    List<Tuple<Tuple<string, GameObject>, int>> tupleList = new List<Tuple<Tuple<string, GameObject>, int>>();
    for (int index = 0; index < ((ResourceSet) Db.Get().Techs).Count; ++index)
    {
      Tech tech = (Tech) ((ResourceSet) Db.Get().Techs).GetResource(index);
      if (!this.projectCategories.ContainsKey(tech.category))
      {
        string categoryID = tech.category;
        GameObject gameObject = Util.KInstantiateUI(this.techCategoryPrefabAlt, this.projectsContainer, true);
        ((Object) gameObject).name = categoryID;
        ((TMP_Text) gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")).SetText(StringEntry.op_Implicit(Strings.Get("STRINGS.RESEARCH.TREES.TITLE" + categoryID.ToUpper())));
        this.categoryExpanded.Add(categoryID, false);
        this.projectCategories.Add(categoryID, gameObject);
        gameObject.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = (System.Action) (() =>
        {
          this.categoryExpanded[categoryID] = !this.categoryExpanded[categoryID];
          this.RefreshCategoriesContentExpanded();
        });
      }
      GameObject gameObject1 = this.SpawnTechWidget(tech.Id, this.projectCategories[tech.category]);
      tupleList.Add(new Tuple<Tuple<string, GameObject>, int>(new Tuple<string, GameObject>(tech.Id, gameObject1), tech.tier));
      this.projectTechs.Add(tech.Id, gameObject1);
      gameObject1.GetComponent<ToolTip>().SetSimpleTooltip(tech.desc);
      gameObject1.GetComponent<MultiToggle>().onEnter += (System.Action) (() =>
      {
        this.researchScreen.TurnEverythingOff();
        this.researchScreen.GetEntry(tech).OnHover(true, tech);
      });
      gameObject1.GetComponent<MultiToggle>().onExit += (System.Action) (() => this.researchScreen.TurnEverythingOff());
    }
    foreach (KeyValuePair<string, GameObject> projectTech in this.projectTechs)
    {
      Transform reference = this.projectCategories[Db.Get().Techs.Get(projectTech.Key).category].GetComponent<HierarchyReferences>().GetReference<Transform>("Content");
      this.projectTechs[projectTech.Key].transform.SetParent(reference);
    }
    tupleList.Sort((Comparison<Tuple<Tuple<string, GameObject>, int>>) ((a, b) => a.second.CompareTo(b.second)));
    foreach (Tuple<Tuple<string, GameObject>, int> tuple in tupleList)
      tuple.first.second.transform.SetAsLastSibling();
  }

  private void PopulateFilterButtons()
  {
    foreach (KeyValuePair<string, List<Tag>> filterPreset in this.filterPresets)
    {
      KeyValuePair<string, List<Tag>> kvp = filterPreset;
      GameObject gameObject = Util.KInstantiateUI(this.filterButtonPrefab, this.searchFiltersContainer, true);
      this.filterButtons.Add(kvp.Key, gameObject);
      this.filterStates.Add(kvp.Key, false);
      MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
      ((TMP_Text) gameObject.GetComponentInChildren<LocText>()).SetText(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.RESEARCHSCREEN.FILTER_BUTTONS." + kvp.Key.ToUpper())));
      toggle.onClick += (System.Action) (() =>
      {
        foreach (KeyValuePair<string, GameObject> filterButton in this.filterButtons)
        {
          if (filterButton.Key != kvp.Key)
          {
            this.filterStates[filterButton.Key] = false;
            this.filterButtons[filterButton.Key].GetComponent<MultiToggle>().ChangeState(this.filterStates[filterButton.Key] ? 1 : 0);
          }
        }
        this.filterStates[kvp.Key] = !this.filterStates[kvp.Key];
        toggle.ChangeState(this.filterStates[kvp.Key] ? 1 : 0);
        if (this.filterStates[kvp.Key])
          ((TMP_InputField) this.searchBox).text = Strings.Get("STRINGS.UI.RESEARCHSCREEN.FILTER_BUTTONS." + kvp.Key.ToUpper()).String;
        else
          ((TMP_InputField) this.searchBox).text = "";
      });
    }
  }

  public void RefreshQueue()
  {
  }

  private void RefreshWidgets()
  {
    List<TechInstance> researchQueue = Research.Instance.GetResearchQueue();
    foreach (KeyValuePair<string, GameObject> projectTech in this.projectTechs)
    {
      KeyValuePair<string, GameObject> kvp = projectTech;
      if (Db.Get().Techs.Get(kvp.Key).IsComplete())
        kvp.Value.GetComponent<MultiToggle>().ChangeState(2);
      else if (researchQueue.Find((Predicate<TechInstance>) (match => match.tech.Id == kvp.Key)) != null)
        kvp.Value.GetComponent<MultiToggle>().ChangeState(1);
      else
        kvp.Value.GetComponent<MultiToggle>().ChangeState(0);
    }
  }

  private void RefreshWidgetProgressBars(string techID, GameObject widget)
  {
    HierarchyReferences component1 = widget.GetComponent<HierarchyReferences>();
    ResearchPointInventory progressInventory = Research.Instance.GetTechInstance(techID).progressInventory;
    int num1 = 0;
    for (int index = 0; index < Research.Instance.researchTypes.Types.Count; ++index)
    {
      if (Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID.ContainsKey(Research.Instance.researchTypes.Types[index].id) && (double) Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID[Research.Instance.researchTypes.Types[index].id] > 0.0)
      {
        HierarchyReferences component2 = ((Component) ((Transform) component1.GetReference<RectTransform>("BarRows")).GetChild(1 + num1)).GetComponent<HierarchyReferences>();
        float num2 = progressInventory.PointsByTypeID[Research.Instance.researchTypes.Types[index].id] / Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID[Research.Instance.researchTypes.Types[index].id];
        RectTransform rectTransform1 = ((Graphic) component2.GetReference<Image>("Bar")).rectTransform;
        RectTransform rectTransform2 = rectTransform1;
        Rect rect = Util.rectTransform((Component) ((Transform) rectTransform1).parent).rect;
        Vector2 vector2 = new Vector2(((Rect) ref rect).width * num2, rectTransform1.sizeDelta.y);
        rectTransform2.sizeDelta = vector2;
        ((TMP_Text) component2.GetReference<LocText>("Label")).SetText(progressInventory.PointsByTypeID[Research.Instance.researchTypes.Types[index].id].ToString() + "/" + Research.Instance.GetTechInstance(techID).tech.costsByResearchTypeID[Research.Instance.researchTypes.Types[index].id].ToString());
        ++num1;
      }
    }
  }

  private GameObject SpawnTechWidget(string techID, GameObject parentContainer)
  {
    GameObject gameObject1 = Util.KInstantiateUI(this.techWidgetRootAltPrefab, parentContainer, true);
    HierarchyReferences component = gameObject1.GetComponent<HierarchyReferences>();
    ((Object) gameObject1).name = Db.Get().Techs.Get(techID).Name;
    ((TMP_Text) component.GetReference<LocText>("Label")).SetText(Db.Get().Techs.Get(techID).Name);
    if (!this.projectTechItems.ContainsKey(techID))
      this.projectTechItems.Add(techID, new Dictionary<string, GameObject>());
    RectTransform reference = component.GetReference<RectTransform>("UnlockContainer");
    foreach (TechItem unlockedItem in Db.Get().Techs.Get(techID).unlockedItems)
    {
      GameObject gameObject2 = Util.KInstantiateUI(this.techItemPrefab, ((Component) reference).gameObject, true);
      gameObject2.GetComponentsInChildren<Image>()[1].sprite = unlockedItem.UISprite();
      ((TMP_Text) gameObject2.GetComponentsInChildren<LocText>()[0]).SetText(unlockedItem.Name);
      gameObject2.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.researchScreen.ZoomToTech(techID));
      ((Graphic) gameObject2.GetComponentsInChildren<Image>()[0]).color = this.evenRow ? this.evenRowColor : this.oddRowColor;
      this.evenRow = !this.evenRow;
      if (!this.projectTechItems[techID].ContainsKey(unlockedItem.Id))
        this.projectTechItems[techID].Add(unlockedItem.Id, gameObject2);
    }
    gameObject1.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.researchScreen.ZoomToTech(techID));
    return gameObject1;
  }

  private void ChangeGameObjectActive(GameObject target, bool targetActiveState)
  {
    if (target.activeSelf == targetActiveState)
      return;
    if (targetActiveState)
    {
      this.QueuedActivations.Add(target);
      if (!this.QueuedDeactivations.Contains(target))
        return;
      this.QueuedDeactivations.Remove(target);
    }
    else
    {
      this.QueuedDeactivations.Add(target);
      if (!this.QueuedActivations.Contains(target))
        return;
      this.QueuedActivations.Remove(target);
    }
  }

  private bool CheckTechItemPassesFilters(string techItemID)
  {
    TechItem techItem = Db.Get().TechItems.Get(techItemID);
    bool flag1 = true;
    switch (this.completionFilter)
    {
      case ResearchScreenSideBar.CompletionState.Available:
        flag1 = flag1 && !techItem.IsComplete() && techItem.ParentTech.ArePrerequisitesComplete();
        break;
      case ResearchScreenSideBar.CompletionState.Completed:
        flag1 = flag1 && techItem.IsComplete();
        break;
    }
    if (!flag1)
      return flag1;
    bool flag2 = flag1 && ResearchScreen.TechItemPassesSearchFilter(techItemID, this.currentSearchString);
    foreach (KeyValuePair<string, bool> filterState in this.filterStates)
      ;
    return flag2;
  }

  private bool CheckTechPassesFilters(string techID)
  {
    Tech tech = Db.Get().Techs.Get(techID);
    bool flag1 = true;
    switch (this.completionFilter)
    {
      case ResearchScreenSideBar.CompletionState.Available:
        flag1 = flag1 && !tech.IsComplete() && tech.ArePrerequisitesComplete();
        break;
      case ResearchScreenSideBar.CompletionState.Completed:
        flag1 = flag1 && tech.IsComplete();
        break;
    }
    if (!flag1)
      return flag1;
    bool flag2 = flag1 && ResearchScreen.TechPassesSearchFilter(techID, this.currentSearchString);
    foreach (KeyValuePair<string, bool> filterState in this.filterStates)
      ;
    return flag2;
  }

  public ResearchScreenSideBar()
  {
    Dictionary<string, List<Tag>> dictionary = new Dictionary<string, List<Tag>>();
    dictionary.Add("Oxygen", new List<Tag>());
    dictionary.Add("Food", new List<Tag>());
    dictionary.Add("Water", new List<Tag>());
    dictionary.Add("Power", new List<Tag>());
    dictionary.Add("Morale", new List<Tag>());
    dictionary.Add("Ranching", new List<Tag>());
    dictionary.Add("Filter", new List<Tag>());
    dictionary.Add("Tile", new List<Tag>());
    dictionary.Add("Transport", new List<Tag>());
    dictionary.Add("Automation", new List<Tag>());
    dictionary.Add("Medicine", new List<Tag>());
    dictionary.Add("Rocket", new List<Tag>());
    this.filterPresets = dictionary;
    this.QueuedActivations = new List<GameObject>();
    this.QueuedDeactivations = new List<GameObject>();
    this.activationPerFrame = 5;
    // ISSUE: explicit constructor call
    base.\u002Ector();
  }

  private enum CompletionState
  {
    All,
    Available,
    Completed,
  }
}
