// Decompiled with JetBrains decompiler
// Type: NameDisplayScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameDisplayScreen : KScreen
{
  [SerializeField]
  private float HideDistance;
  public static NameDisplayScreen Instance;
  public GameObject nameAndBarsPrefab;
  public GameObject barsPrefab;
  public TextStyleSetting ToolTipStyle_Property;
  [SerializeField]
  private Color selectedColor;
  [SerializeField]
  private Color defaultColor;
  public int fontsize_min = 14;
  public int fontsize_max = 32;
  public float cameraDistance_fontsize_min = 6f;
  public float cameraDistance_fontsize_max = 4f;
  public List<NameDisplayScreen.Entry> entries = new List<NameDisplayScreen.Entry>();
  public List<NameDisplayScreen.TextEntry> textEntries = new List<NameDisplayScreen.TextEntry>();
  public bool worldSpace = true;
  private int updateSectionIndex;
  private List<System.Action> lateUpdateSections = new List<System.Action>();
  private bool isOverlayChangeBound;
  private HashedString lastKnownOverlayID = OverlayModes.None.ID;
  private List<KCollider2D> workingList = new List<KCollider2D>();

  public static void DestroyInstance() => NameDisplayScreen.Instance = (NameDisplayScreen) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    NameDisplayScreen.Instance = this;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.Health.Register(new Action<Health>(this.OnHealthAdded), (Action<Health>) null);
    Components.Equipment.Register(new Action<Equipment>(this.OnEquipmentAdded), (Action<Equipment>) null);
    this.updateSectionIndex = 0;
    this.lateUpdateSections = new List<System.Action>()
    {
      new System.Action(this.LateUpdatePart0),
      new System.Action(this.LateUpdatePart1),
      new System.Action(this.LateUpdatePart2)
    };
    this.bindOnOverlayChange();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (!this.isOverlayChangeBound || !Object.op_Inequality((Object) OverlayScreen.Instance, (Object) null))
      return;
    OverlayScreen.Instance.OnOverlayChanged -= new Action<HashedString>(this.OnOverlayChanged);
    this.isOverlayChangeBound = false;
  }

  private void bindOnOverlayChange()
  {
    if (this.isOverlayChangeBound || !Object.op_Inequality((Object) OverlayScreen.Instance, (Object) null))
      return;
    OverlayScreen.Instance.OnOverlayChanged += new Action<HashedString>(this.OnOverlayChanged);
    this.isOverlayChangeBound = true;
  }

  private void OnOverlayChanged(HashedString new_mode)
  {
    HashedString lastKnownOverlayId = this.lastKnownOverlayID;
    this.lastKnownOverlayID = new_mode;
    HashedString id = OverlayModes.None.ID;
    if (HashedString.op_Inequality(lastKnownOverlayId, id) && HashedString.op_Inequality(new_mode, OverlayModes.None.ID))
      return;
    bool flag = HashedString.op_Equality(new_mode, OverlayModes.None.ID);
    int count = this.entries.Count;
    for (int index = 0; index < count; ++index)
    {
      NameDisplayScreen.Entry entry = this.entries[index];
      if (!Object.op_Equality((Object) entry.world_go, (Object) null))
        entry.display_go.SetActive(flag);
    }
  }

  private void OnHealthAdded(Health health) => this.RegisterComponent(((Component) health).gameObject, (object) health);

  private void OnEquipmentAdded(Equipment equipment)
  {
    MinionAssignablesProxy component = ((Component) equipment).GetComponent<MinionAssignablesProxy>();
    GameObject targetGameObject = component.GetTargetGameObject();
    if (Object.op_Implicit((Object) targetGameObject))
      this.RegisterComponent(targetGameObject, (object) equipment);
    else
      Debug.LogWarningFormat("OnEquipmentAdded proxy target {0} was null.", new object[1]
      {
        (object) component.TargetInstanceID
      });
  }

  private bool ShouldShowName(GameObject representedObject)
  {
    CharacterOverlay component = representedObject.GetComponent<CharacterOverlay>();
    return Object.op_Inequality((Object) component, (Object) null) && component.shouldShowName;
  }

  public Guid AddWorldText(string initialText, GameObject prefab)
  {
    NameDisplayScreen.TextEntry textEntry = new NameDisplayScreen.TextEntry();
    textEntry.guid = Guid.NewGuid();
    textEntry.display_go = Util.KInstantiateUI(prefab, ((Component) this).gameObject, true);
    ((TMP_Text) textEntry.display_go.GetComponentInChildren<LocText>()).text = initialText;
    this.textEntries.Add(textEntry);
    return textEntry.guid;
  }

  public GameObject GetWorldText(Guid guid)
  {
    GameObject worldText = (GameObject) null;
    foreach (NameDisplayScreen.TextEntry textEntry in this.textEntries)
    {
      if (textEntry.guid == guid)
      {
        worldText = textEntry.display_go;
        break;
      }
    }
    return worldText;
  }

  public void RemoveWorldText(Guid guid)
  {
    int index1 = -1;
    for (int index2 = 0; index2 < this.textEntries.Count; ++index2)
    {
      if (this.textEntries[index2].guid == guid)
      {
        index1 = index2;
        break;
      }
    }
    if (index1 < 0)
      return;
    Object.Destroy((Object) this.textEntries[index1].display_go);
    this.textEntries.RemoveAt(index1);
  }

  public void AddNewEntry(GameObject representedObject)
  {
    NameDisplayScreen.Entry entry = new NameDisplayScreen.Entry();
    entry.world_go = representedObject;
    entry.world_go_anim_controller = representedObject.GetComponent<KAnimControllerBase>();
    GameObject gameObject = Util.KInstantiateUI(this.ShouldShowName(representedObject) ? this.nameAndBarsPrefab : this.barsPrefab, ((Component) this).gameObject, true);
    entry.display_go = gameObject;
    entry.display_go_rect = gameObject.GetComponent<RectTransform>();
    if (this.worldSpace)
      entry.display_go.transform.localScale = Vector3.op_Multiply(Vector3.one, 0.01f);
    ((Object) gameObject).name = ((Object) representedObject).name + " character overlay";
    entry.Name = ((Object) representedObject).name;
    entry.refs = gameObject.GetComponent<HierarchyReferences>();
    this.entries.Add(entry);
    KSelectable component1 = representedObject.GetComponent<KSelectable>();
    FactionAlignment component2 = representedObject.GetComponent<FactionAlignment>();
    if (!Object.op_Inequality((Object) component1, (Object) null))
      return;
    if (Object.op_Inequality((Object) component2, (Object) null))
    {
      if (component2.Alignment != FactionManager.FactionID.Friendly && component2.Alignment != FactionManager.FactionID.Duplicant)
        return;
      this.UpdateName(representedObject);
    }
    else
      this.UpdateName(representedObject);
  }

  public void RegisterComponent(
    GameObject representedObject,
    object component,
    bool force_new_entry = false)
  {
    NameDisplayScreen.Entry entry = force_new_entry ? (NameDisplayScreen.Entry) null : this.GetEntry(representedObject);
    if (entry == null)
    {
      CharacterOverlay component1 = representedObject.GetComponent<CharacterOverlay>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        component1.Register();
        entry = this.GetEntry(representedObject);
      }
    }
    if (entry == null)
      return;
    Transform reference = entry.refs.GetReference<Transform>("Bars");
    entry.bars_go = ((Component) reference).gameObject;
    switch (component)
    {
      case Health _:
        if (!Object.op_Implicit((Object) entry.healthBar))
        {
          Health health = (Health) component;
          GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.healthBarPrefab, ((Component) reference).gameObject, false);
          ((Object) gameObject).name = "Health Bar";
          health.healthBar = gameObject.GetComponent<HealthBar>();
          ((Component) health.healthBar).GetComponent<KSelectable>().entityName = (string) STRINGS.UI.METERS.HEALTH.TOOLTIP;
          ((Component) health.healthBar).GetComponent<KSelectableHealthBar>().IsSelectable = Object.op_Inequality((Object) representedObject.GetComponent<MinionBrain>(), (Object) null);
          entry.healthBar = health.healthBar;
          entry.healthBar.autoHide = false;
          ((Graphic) ((Component) gameObject.transform.Find("Bar")).GetComponent<Image>()).color = ProgressBarsConfig.Instance.GetBarColor("HealthBar");
          break;
        }
        Debug.LogWarningFormat("Health added twice {0}", new object[1]
        {
          component
        });
        break;
      case OxygenBreather _:
        if (!Object.op_Implicit((Object) entry.breathBar))
        {
          GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, ((Component) reference).gameObject, false);
          entry.breathBar = gameObject.GetComponent<ProgressBar>();
          entry.breathBar.autoHide = false;
          gameObject.gameObject.GetComponent<ToolTip>().AddMultiStringTooltip("Breath", this.ToolTipStyle_Property);
          ((Object) gameObject).name = "Breath Bar";
          ((Graphic) ((Component) gameObject.transform.Find("Bar")).GetComponent<Image>()).color = ProgressBarsConfig.Instance.GetBarColor("BreathBar");
          gameObject.GetComponent<KSelectable>().entityName = (string) STRINGS.UI.METERS.BREATH.TOOLTIP;
          break;
        }
        Debug.LogWarningFormat("OxygenBreather added twice {0}", new object[1]
        {
          component
        });
        break;
      case Equipment _:
        if (!Object.op_Implicit((Object) entry.suitBar))
        {
          GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, ((Component) reference).gameObject, false);
          entry.suitBar = gameObject.GetComponent<ProgressBar>();
          entry.suitBar.autoHide = false;
          ((Object) gameObject).name = "Suit Tank Bar";
          ((Graphic) ((Component) gameObject.transform.Find("Bar")).GetComponent<Image>()).color = ProgressBarsConfig.Instance.GetBarColor("OxygenTankBar");
          gameObject.GetComponent<KSelectable>().entityName = (string) STRINGS.UI.METERS.BREATH.TOOLTIP;
        }
        else
          Debug.LogWarningFormat("SuitBar added twice {0}", new object[1]
          {
            component
          });
        if (!Object.op_Implicit((Object) entry.suitFuelBar))
        {
          GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, ((Component) reference).gameObject, false);
          entry.suitFuelBar = gameObject.GetComponent<ProgressBar>();
          entry.suitFuelBar.autoHide = false;
          ((Object) gameObject).name = "Suit Fuel Bar";
          ((Graphic) ((Component) gameObject.transform.Find("Bar")).GetComponent<Image>()).color = ProgressBarsConfig.Instance.GetBarColor("FuelTankBar");
          gameObject.GetComponent<KSelectable>().entityName = (string) STRINGS.UI.METERS.FUEL.TOOLTIP;
        }
        else
          Debug.LogWarningFormat("FuelBar added twice {0}", new object[1]
          {
            component
          });
        if (!Object.op_Implicit((Object) entry.suitBatteryBar))
        {
          GameObject gameObject = Util.KInstantiateUI(ProgressBarsConfig.Instance.progressBarUIPrefab, ((Component) reference).gameObject, false);
          entry.suitBatteryBar = gameObject.GetComponent<ProgressBar>();
          entry.suitBatteryBar.autoHide = false;
          ((Object) gameObject).name = "Suit Battery Bar";
          ((Graphic) ((Component) gameObject.transform.Find("Bar")).GetComponent<Image>()).color = ProgressBarsConfig.Instance.GetBarColor("BatteryBar");
          gameObject.GetComponent<KSelectable>().entityName = (string) STRINGS.UI.METERS.BATTERY.TOOLTIP;
          break;
        }
        Debug.LogWarningFormat("CoolantBar added twice {0}", new object[1]
        {
          component
        });
        break;
      case ThoughtGraph.Instance _:
        if (!Object.op_Implicit((Object) entry.thoughtBubble))
        {
          GameObject gameObject1 = Util.KInstantiateUI(EffectPrefabs.Instance.ThoughtBubble, entry.display_go, false);
          entry.thoughtBubble = gameObject1.GetComponent<HierarchyReferences>();
          ((Object) gameObject1).name = "Thought Bubble";
          GameObject gameObject2 = Util.KInstantiateUI(EffectPrefabs.Instance.ThoughtBubbleConvo, entry.display_go, false);
          entry.thoughtBubbleConvo = gameObject2.GetComponent<HierarchyReferences>();
          ((Object) gameObject2).name = "Thought Bubble Convo";
          break;
        }
        Debug.LogWarningFormat("ThoughtGraph added twice {0}", new object[1]
        {
          component
        });
        break;
      case GameplayEventMonitor.Instance _:
        if (!Object.op_Implicit((Object) entry.gameplayEventDisplay))
        {
          GameObject gameObject = Util.KInstantiateUI(EffectPrefabs.Instance.GameplayEventDisplay, entry.display_go, false);
          entry.gameplayEventDisplay = gameObject.GetComponent<HierarchyReferences>();
          ((Object) gameObject).name = "Gameplay Event Display";
          break;
        }
        Debug.LogWarningFormat("GameplayEventDisplay added twice {0}", new object[1]
        {
          component
        });
        break;
      case Dreamer.Instance _:
        if (Object.op_Implicit((Object) entry.dreamBubble))
          break;
        GameObject gameObject3 = Util.KInstantiateUI(EffectPrefabs.Instance.DreamBubble, entry.display_go, false);
        ((Object) gameObject3).name = "Dream Bubble";
        entry.dreamBubble = gameObject3.GetComponent<DreamBubble>();
        break;
    }
  }

  private void LateUpdate()
  {
    if (App.isLoading || App.IsExiting)
      return;
    this.bindOnOverlayChange();
    Camera mainCamera = Game.MainCamera;
    if (Object.op_Equality((Object) mainCamera, (Object) null) || HashedString.op_Inequality(this.lastKnownOverlayID, OverlayModes.None.ID))
      return;
    int count = this.entries.Count;
    this.LateUpdatePos((double) mainCamera.orthographicSize < (double) this.HideDistance);
    this.lateUpdateSections[this.updateSectionIndex]();
    this.updateSectionIndex = (this.updateSectionIndex + 1) % this.lateUpdateSections.Count;
  }

  private void LateUpdatePos(bool visibleToZoom)
  {
    CameraController instance = CameraController.Instance;
    Transform followTarget = instance.followTarget;
    int count = this.entries.Count;
    for (int index = 0; index < count; ++index)
    {
      NameDisplayScreen.Entry entry = this.entries[index];
      GameObject worldGo = entry.world_go;
      if (!Object.op_Equality((Object) worldGo, (Object) null))
      {
        Vector3 pos = TransformExtensions.GetPosition(worldGo.transform);
        if (visibleToZoom && CameraController.Instance.IsVisiblePos(pos))
        {
          if (Object.op_Inequality((Object) instance, (Object) null) && Object.op_Equality((Object) followTarget, (Object) worldGo.transform))
            pos = instance.followTargetPos;
          else if (Object.op_Inequality((Object) entry.world_go_anim_controller, (Object) null))
            pos = entry.world_go_anim_controller.GetWorldPivot();
          entry.display_go_rect.anchoredPosition = Vector2.op_Implicit(this.worldSpace ? pos : this.WorldToScreen(pos));
          entry.display_go.SetActive(true);
        }
        else if (entry.display_go.activeSelf)
          entry.display_go.SetActive(false);
      }
    }
  }

  private void LateUpdatePart0()
  {
    int count = this.entries.Count;
    int index = 0;
    while (index < count)
    {
      if (Object.op_Equality((Object) this.entries[index].world_go, (Object) null))
      {
        Object.Destroy((Object) this.entries[index].display_go);
        --count;
        this.entries[index] = this.entries[count];
      }
      else
        ++index;
    }
    this.entries.RemoveRange(count, this.entries.Count - count);
  }

  private void LateUpdatePart1()
  {
    int count = this.entries.Count;
    for (int index = 0; index < count; ++index)
    {
      if (!Object.op_Equality((Object) this.entries[index].world_go, (Object) null) && this.entries[index].world_go.HasTag(GameTags.Dead))
        this.entries[index].bars_go.SetActive(false);
    }
  }

  private void LateUpdatePart2()
  {
    int count = this.entries.Count;
    for (int index = 0; index < count; ++index)
    {
      if (Object.op_Inequality((Object) this.entries[index].bars_go, (Object) null))
      {
        this.entries[index].bars_go.GetComponentsInChildren<KCollider2D>(false, this.workingList);
        foreach (KCollider2D working in this.workingList)
          working.MarkDirty();
      }
    }
  }

  public void UpdateName(GameObject representedObject)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(representedObject);
    if (entry == null)
      return;
    KSelectable component = representedObject.GetComponent<KSelectable>();
    ((Object) entry.display_go).name = component.GetProperName() + " character overlay";
    LocText componentInChildren = entry.display_go.GetComponentInChildren<LocText>();
    if (!Object.op_Inequality((Object) componentInChildren, (Object) null))
      return;
    ((TMP_Text) componentInChildren).text = component.GetProperName();
    if (!Object.op_Inequality((Object) representedObject.GetComponent<RocketModule>(), (Object) null))
      return;
    ((TMP_Text) componentInChildren).text = representedObject.GetComponent<RocketModule>().GetParentRocketName();
  }

  public void SetDream(GameObject minion_go, Dream dream)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.dreamBubble, (Object) null))
      return;
    entry.dreamBubble.SetDream(dream);
    ((Component) entry.dreamBubble).GetComponent<KSelectable>().entityName = "Dreaming";
    ((Component) entry.dreamBubble).gameObject.SetActive(true);
    entry.dreamBubble.SetVisibility(true);
  }

  public void StopDreaming(GameObject minion_go)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.dreamBubble, (Object) null))
      return;
    entry.dreamBubble.StopDreaming();
    ((Component) entry.dreamBubble).gameObject.SetActive(false);
  }

  public void DreamTick(GameObject minion_go, float dt)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.dreamBubble, (Object) null))
      return;
    entry.dreamBubble.Tick(dt);
  }

  public void SetThoughtBubbleDisplay(
    GameObject minion_go,
    bool bVisible,
    string hover_text,
    Sprite bubble_sprite,
    Sprite topic_sprite)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.thoughtBubble, (Object) null))
      return;
    this.ApplyThoughtSprite(entry.thoughtBubble, bubble_sprite, nameof (bubble_sprite));
    this.ApplyThoughtSprite(entry.thoughtBubble, topic_sprite, "icon_sprite");
    ((Component) entry.thoughtBubble).GetComponent<KSelectable>().entityName = hover_text;
    ((Component) entry.thoughtBubble).gameObject.SetActive(bVisible);
  }

  public void SetThoughtBubbleConvoDisplay(
    GameObject minion_go,
    bool bVisible,
    string hover_text,
    Sprite bubble_sprite,
    Sprite topic_sprite,
    Sprite mode_sprite)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.thoughtBubble, (Object) null))
      return;
    this.ApplyThoughtSprite(entry.thoughtBubbleConvo, bubble_sprite, nameof (bubble_sprite));
    this.ApplyThoughtSprite(entry.thoughtBubbleConvo, topic_sprite, "icon_sprite");
    this.ApplyThoughtSprite(entry.thoughtBubbleConvo, mode_sprite, "icon_sprite_mode");
    ((Component) entry.thoughtBubbleConvo).GetComponent<KSelectable>().entityName = hover_text;
    ((Component) entry.thoughtBubbleConvo).gameObject.SetActive(bVisible);
  }

  private void ApplyThoughtSprite(HierarchyReferences active_bubble, Sprite sprite, string target) => active_bubble.GetReference<Image>(target).sprite = sprite;

  public void SetGameplayEventDisplay(
    GameObject minion_go,
    bool bVisible,
    string hover_text,
    Sprite sprite)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.gameplayEventDisplay, (Object) null))
      return;
    entry.gameplayEventDisplay.GetReference<Image>("icon_sprite").sprite = sprite;
    ((Component) entry.gameplayEventDisplay).GetComponent<KSelectable>().entityName = hover_text;
    ((Component) entry.gameplayEventDisplay).gameObject.SetActive(bVisible);
  }

  public void SetBreathDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.breathBar, (Object) null))
      return;
    entry.breathBar.SetUpdateFunc(updatePercentFull);
    ((Component) entry.breathBar).gameObject.SetActive(bVisible);
  }

  public void SetHealthDisplay(GameObject minion_go, Func<float> updatePercentFull, bool bVisible)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.healthBar, (Object) null))
      return;
    entry.healthBar.OnChange();
    entry.healthBar.SetUpdateFunc(updatePercentFull);
    if (((Component) entry.healthBar).gameObject.activeSelf == bVisible)
      return;
    ((Component) entry.healthBar).gameObject.SetActive(bVisible);
  }

  public void SetSuitTankDisplay(
    GameObject minion_go,
    Func<float> updatePercentFull,
    bool bVisible)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.suitBar, (Object) null))
      return;
    entry.suitBar.SetUpdateFunc(updatePercentFull);
    ((Component) entry.suitBar).gameObject.SetActive(bVisible);
  }

  public void SetSuitFuelDisplay(
    GameObject minion_go,
    Func<float> updatePercentFull,
    bool bVisible)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.suitFuelBar, (Object) null))
      return;
    entry.suitFuelBar.SetUpdateFunc(updatePercentFull);
    ((Component) entry.suitFuelBar).gameObject.SetActive(bVisible);
  }

  public void SetSuitBatteryDisplay(
    GameObject minion_go,
    Func<float> updatePercentFull,
    bool bVisible)
  {
    NameDisplayScreen.Entry entry = this.GetEntry(minion_go);
    if (entry == null || Object.op_Equality((Object) entry.suitBatteryBar, (Object) null))
      return;
    entry.suitBatteryBar.SetUpdateFunc(updatePercentFull);
    ((Component) entry.suitBatteryBar).gameObject.SetActive(bVisible);
  }

  private NameDisplayScreen.Entry GetEntry(GameObject worldObject) => this.entries.Find((Predicate<NameDisplayScreen.Entry>) (entry => Object.op_Equality((Object) entry.world_go, (Object) worldObject)));

  [Serializable]
  public class Entry
  {
    public string Name;
    public GameObject world_go;
    public GameObject display_go;
    public GameObject bars_go;
    public KAnimControllerBase world_go_anim_controller;
    public RectTransform display_go_rect;
    public HealthBar healthBar;
    public ProgressBar breathBar;
    public ProgressBar suitBar;
    public ProgressBar suitFuelBar;
    public ProgressBar suitBatteryBar;
    public DreamBubble dreamBubble;
    public HierarchyReferences thoughtBubble;
    public HierarchyReferences thoughtBubbleConvo;
    public HierarchyReferences gameplayEventDisplay;
    public HierarchyReferences refs;
  }

  public class TextEntry
  {
    public Guid guid;
    public GameObject display_go;
  }
}
