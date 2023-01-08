// Decompiled with JetBrains decompiler
// Type: SimpleInfoScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using ProcGen;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInfoScreen : TargetScreen, ISim4000ms, ISim1000ms
{
  public GameObject attributesLabelTemplate;
  public GameObject attributesLabelButtonTemplate;
  public GameObject DescriptionContainerTemplate;
  private DescriptionContainer descriptionContainer;
  public GameObject StampContainerTemplate;
  public GameObject StampPrefab;
  public GameObject VitalsPanelTemplate;
  public Sprite DefaultPortraitIcon;
  public Text StatusPanelCurrentActionLabel;
  public GameObject StatusItemPrefab;
  public Sprite statusWarningIcon;
  private RocketSimpleInfoPanel rocketSimpleInfoPanel;
  private SpacePOISimpleInfoPanel spaceSimpleInfoPOIPanel;
  [SerializeField]
  private HierarchyReferences processConditionHeader;
  [SerializeField]
  private GameObject processConditionRow;
  private CollapsibleDetailContentPanel statusItemPanel;
  private CollapsibleDetailContentPanel vitalsPanel;
  private CollapsibleDetailContentPanel fertilityPanel;
  private CollapsibleDetailContentPanel rocketStatusContainer;
  private CollapsibleDetailContentPanel worldLifePanel;
  private CollapsibleDetailContentPanel worldElementsPanel;
  private CollapsibleDetailContentPanel worldBiomesPanel;
  private CollapsibleDetailContentPanel worldGeysersPanel;
  private CollapsibleDetailContentPanel spacePOIPanel;
  private CollapsibleDetailContentPanel worldTraitsPanel;
  [SerializeField]
  public GameObject iconLabelRow;
  [SerializeField]
  public GameObject bigIconLabelRow;
  private Dictionary<Tag, GameObject> lifeformRows = new Dictionary<Tag, GameObject>();
  private Dictionary<Tag, GameObject> biomeRows = new Dictionary<Tag, GameObject>();
  private Dictionary<Tag, GameObject> geyserRows = new Dictionary<Tag, GameObject>();
  private List<GameObject> worldTraitRows = new List<GameObject>();
  private List<GameObject> surfaceConditionRows = new List<GameObject>();
  [SerializeField]
  public GameObject spacerRow;
  private GameObject infoPanel;
  private GameObject stampContainer;
  private MinionVitalsPanel vitalsContainer;
  private GameObject InfoFolder;
  private GameObject statusItemsFolder;
  public GameObject TextContainerPrefab;
  private GameObject processConditionContainer;
  private GameObject stressPanel;
  private DetailsPanelDrawer stressDrawer;
  private Dictionary<string, GameObject> storageLabels = new Dictionary<string, GameObject>();
  public TextStyleSetting ToolTipStyle_Property;
  public TextStyleSetting StatusItemStyle_Main;
  public TextStyleSetting StatusItemStyle_Other;
  public Color statusItemTextColor_regular = Color.black;
  public Color statusItemTextColor_old = new Color(0.8235294f, 0.8235294f, 0.8235294f);
  private GameObject lastTarget;
  private bool TargetIsMinion;
  private List<SimpleInfoScreen.StatusItemEntry> statusItems = new List<SimpleInfoScreen.StatusItemEntry>();
  private List<SimpleInfoScreen.StatusItemEntry> oldStatusItems = new List<SimpleInfoScreen.StatusItemEntry>();
  private List<LocText> attributeLabels = new List<LocText>();
  private List<GameObject> processConditionRows = new List<GameObject>();
  private Action<object> onStorageChangeDelegate;
  private static readonly EventSystem.IntraObjectHandler<SimpleInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<SimpleInfoScreen>((Action<SimpleInfoScreen, object>) ((component, data) => component.OnRefreshData(data)));

  public GameObject StoragePanel { get; private set; }

  public override bool IsValidForTarget(GameObject target) => true;

  protected virtual void OnPrefabInit()
  {
    this.onStorageChangeDelegate = new Action<object>(this.OnStorageChange);
    base.OnPrefabInit();
    this.processConditionContainer = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((TMP_Text) this.processConditionContainer.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS.NAME;
    this.statusItemPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((LayoutGroup) ((Component) this.statusItemPanel.Content).GetComponent<VerticalLayoutGroup>()).padding.bottom = 10;
    ((TMP_Text) this.statusItemPanel.HeaderLabel).text = (string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_STATUS;
    this.statusItemPanel.scalerMask.hoverLock = true;
    this.statusItemsFolder = ((Component) this.statusItemPanel.Content).gameObject;
    this.spaceSimpleInfoPOIPanel = new SpacePOISimpleInfoPanel(this);
    this.spacePOIPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.rocketSimpleInfoPanel = new RocketSimpleInfoPanel(this);
    this.rocketStatusContainer = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.rocketStatusContainer.SetTitle((string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_ROCKET);
    this.vitalsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.vitalsPanel.SetTitle((string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION);
    this.vitalsContainer = Util.KInstantiateUI(this.VitalsPanelTemplate, ((Component) this.vitalsPanel.Content).gameObject, false).GetComponent<MinionVitalsPanel>();
    this.fertilityPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.fertilityPanel.SetTitle((string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_FERTILITY);
    this.infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((TMP_Text) this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_DESCRIPTION;
    GameObject gameObject = ((Component) this.infoPanel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject;
    this.descriptionContainer = Util.KInstantiateUI<DescriptionContainer>(this.DescriptionContainerTemplate, gameObject, false);
    this.worldLifePanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.worldLifePanel.SetTitle((string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_LIFE);
    this.worldTraitsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    ((TMP_Text) ((Component) this.worldTraitsPanel).GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_WORLDTRAITS;
    this.worldElementsPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.worldElementsPanel.SetTitle((string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_ELEMENTS);
    this.worldGeysersPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.worldGeysersPanel.SetTitle((string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_GEYSERS);
    this.worldBiomesPanel = Util.KInstantiateUI<CollapsibleDetailContentPanel>(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.worldBiomesPanel.SetTitle((string) STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_BIOMES);
    this.StoragePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.stressPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, ((Component) this).gameObject, false);
    this.stressDrawer = new DetailsPanelDrawer(this.attributesLabelTemplate, ((Component) this.stressPanel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject);
    this.stampContainer = Util.KInstantiateUI(this.StampContainerTemplate, gameObject, false);
    ((KMonoBehaviour) this).Subscribe<SimpleInfoScreen>(-1514841199, SimpleInfoScreen.OnRefreshDataDelegate);
  }

  public override void OnSelectTarget(GameObject target)
  {
    base.OnSelectTarget(target);
    ((KMonoBehaviour) this).Subscribe(target, -1697596308, this.onStorageChangeDelegate);
    ((KMonoBehaviour) this).Subscribe(target, -1197125120, this.onStorageChangeDelegate);
    this.RefreshStorage();
    ((KMonoBehaviour) this).Subscribe(target, 1059811075, new Action<object>(this.OnBreedingChanceChanged));
    this.RefreshBreedingChance();
    this.vitalsPanel.SetTitle((string) (Object.op_Equality((Object) target.GetComponent<WiltCondition>(), (Object) null) ? STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_CONDITION : STRINGS.UI.DETAILTABS.SIMPLEINFO.GROUPNAME_REQUIREMENTS));
    KSelectable component = target.GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      StatusItemGroup statusItemGroup = component.GetStatusItemGroup();
      if (statusItemGroup != null)
      {
        statusItemGroup.OnAddStatusItem += new Action<StatusItemGroup.Entry, StatusItemCategory>(this.OnAddStatusItem);
        statusItemGroup.OnRemoveStatusItem += new Action<StatusItemGroup.Entry, bool>(this.OnRemoveStatusItem);
        foreach (StatusItemGroup.Entry status_item in statusItemGroup)
        {
          if (status_item.category != null && status_item.category.Id == "Main")
            this.DoAddStatusItem(status_item, status_item.category);
        }
        foreach (StatusItemGroup.Entry status_item in statusItemGroup)
        {
          if (status_item.category == null || status_item.category.Id != "Main")
            this.DoAddStatusItem(status_item, status_item.category);
        }
      }
    }
    ((Component) this.statusItemPanel).gameObject.SetActive(true);
    this.statusItemPanel.scalerMask.UpdateSize();
    this.Refresh(true);
    this.RefreshWorld();
    this.spaceSimpleInfoPOIPanel.Refresh(this.spacePOIPanel, this.selectedTarget);
  }

  public override void OnDeselectTarget(GameObject target)
  {
    base.OnDeselectTarget(target);
    if (Object.op_Inequality((Object) target, (Object) null))
    {
      ((KMonoBehaviour) this).Unsubscribe(target, -1697596308, this.onStorageChangeDelegate);
      ((KMonoBehaviour) this).Unsubscribe(target, -1197125120, this.onStorageChangeDelegate);
      ((KMonoBehaviour) this).Unsubscribe(target, 1059811075, new Action<object>(this.OnBreedingChanceChanged));
    }
    KSelectable component = target.GetComponent<KSelectable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    StatusItemGroup statusItemGroup = component.GetStatusItemGroup();
    if (statusItemGroup == null)
      return;
    statusItemGroup.OnAddStatusItem -= new Action<StatusItemGroup.Entry, StatusItemCategory>(this.OnAddStatusItem);
    statusItemGroup.OnRemoveStatusItem -= new Action<StatusItemGroup.Entry, bool>(this.OnRemoveStatusItem);
    foreach (SimpleInfoScreen.StatusItemEntry statusItem in this.statusItems)
      statusItem.Destroy(true);
    this.statusItems.Clear();
    foreach (SimpleInfoScreen.StatusItemEntry oldStatusItem in this.oldStatusItems)
    {
      oldStatusItem.onDestroy = (Action<SimpleInfoScreen.StatusItemEntry>) null;
      oldStatusItem.Destroy(true);
    }
    this.oldStatusItems.Clear();
  }

  private void OnStorageChange(object data) => this.RefreshStorage();

  private void OnBreedingChanceChanged(object data) => this.RefreshBreedingChance();

  private void OnAddStatusItem(StatusItemGroup.Entry status_item, StatusItemCategory category) => this.DoAddStatusItem(status_item, category);

  private void DoAddStatusItem(
    StatusItemGroup.Entry status_item,
    StatusItemCategory category,
    bool show_immediate = false)
  {
    GameObject statusItemsFolder = this.statusItemsFolder;
    Color color = status_item.item.notificationType == NotificationType.BadMinor || status_item.item.notificationType == NotificationType.Bad || status_item.item.notificationType == NotificationType.DuplicantThreatening ? Color32.op_Implicit(GlobalAssets.Instance.colorSet.statusItemBad) : (status_item.item.notificationType != NotificationType.Event ? this.statusItemTextColor_regular : Color32.op_Implicit(GlobalAssets.Instance.colorSet.statusItemEvent));
    TextStyleSetting style = category == Db.Get().StatusItemCategories.Main ? this.StatusItemStyle_Main : this.StatusItemStyle_Other;
    SimpleInfoScreen.StatusItemEntry statusItemEntry1 = new SimpleInfoScreen.StatusItemEntry(status_item, category, this.StatusItemPrefab, statusItemsFolder.transform, this.ToolTipStyle_Property, color, style, show_immediate, new Action<SimpleInfoScreen.StatusItemEntry>(this.OnStatusItemDestroy));
    statusItemEntry1.SetSprite(status_item.item.sprite);
    if (category != null)
    {
      int index = -1;
      foreach (SimpleInfoScreen.StatusItemEntry statusItemEntry2 in this.oldStatusItems.FindAll((Predicate<SimpleInfoScreen.StatusItemEntry>) (e => e.category == category)))
      {
        index = statusItemEntry2.GetIndex();
        statusItemEntry2.Destroy(true);
        this.oldStatusItems.Remove(statusItemEntry2);
      }
      if (category == Db.Get().StatusItemCategories.Main)
        index = 0;
      if (index != -1)
        statusItemEntry1.SetIndex(index);
    }
    this.statusItems.Add(statusItemEntry1);
  }

  private void OnRemoveStatusItem(StatusItemGroup.Entry status_item, bool immediate = false) => this.DoRemoveStatusItem(status_item, immediate);

  private void DoRemoveStatusItem(StatusItemGroup.Entry status_item, bool destroy_immediate = false)
  {
    for (int index = 0; index < this.statusItems.Count; ++index)
    {
      if (this.statusItems[index].item.item == status_item.item)
      {
        SimpleInfoScreen.StatusItemEntry statusItem = this.statusItems[index];
        this.statusItems.RemoveAt(index);
        this.oldStatusItems.Add(statusItem);
        statusItem.Destroy(destroy_immediate);
        break;
      }
    }
  }

  private void OnStatusItemDestroy(SimpleInfoScreen.StatusItemEntry item) => this.oldStatusItems.Remove(item);

  private void Update() => this.Refresh();

  private void OnRefreshData(object obj) => this.Refresh();

  public void Refresh(bool force = false)
  {
    if (Object.op_Inequality((Object) this.selectedTarget, (Object) this.lastTarget) || force)
    {
      this.lastTarget = this.selectedTarget;
      if (Object.op_Inequality((Object) this.selectedTarget, (Object) null))
      {
        this.SetPanels(this.selectedTarget);
        this.SetStamps(this.selectedTarget);
      }
    }
    int count = this.statusItems.Count;
    ((Component) this.statusItemPanel).gameObject.SetActive(count > 0);
    for (int index = 0; index < count; ++index)
      this.statusItems[index].Refresh();
    if (((Behaviour) this.vitalsContainer).isActiveAndEnabled)
      this.vitalsContainer.Refresh();
    this.RefreshStress();
    this.RefreshStorage();
    this.rocketSimpleInfoPanel.Refresh(this.rocketStatusContainer, this.selectedTarget);
  }

  private void SetPanels(GameObject target)
  {
    MinionIdentity component1 = target.GetComponent<MinionIdentity>();
    Amounts amounts = target.GetAmounts();
    PrimaryElement component2 = target.GetComponent<PrimaryElement>();
    BuildingComplete component3 = target.GetComponent<BuildingComplete>();
    BuildingUnderConstruction component4 = target.GetComponent<BuildingUnderConstruction>();
    CellSelectionObject component5 = target.GetComponent<CellSelectionObject>();
    InfoDescription component6 = target.GetComponent<InfoDescription>();
    Edible component7 = target.GetComponent<Edible>();
    IProcessConditionSet component8 = target.GetComponent<IProcessConditionSet>();
    this.attributeLabels.ForEach((Action<LocText>) (x => Object.Destroy((Object) ((Component) x).gameObject)));
    this.attributeLabels.Clear();
    ((Component) this.vitalsPanel).gameObject.SetActive(amounts != null);
    string str1 = "";
    string str2 = "";
    if (amounts != null)
    {
      this.vitalsContainer.selectedEntity = this.selectedTarget;
      Uprootable component9 = this.selectedTarget.gameObject.GetComponent<Uprootable>();
      if (Object.op_Inequality((Object) component9, (Object) null))
        ((Component) this.vitalsPanel).gameObject.SetActive(Object.op_Inequality((Object) component9.GetPlanterStorage, (Object) null));
      if (Object.op_Inequality((Object) this.selectedTarget.gameObject.GetComponent<WiltCondition>(), (Object) null))
        ((Component) this.vitalsPanel).gameObject.SetActive(true);
    }
    if (component8 != null)
    {
      this.processConditionContainer.SetActive(true);
      this.RefreshProcessConditions();
    }
    else
      this.processConditionContainer.SetActive(false);
    if (Object.op_Implicit((Object) component1))
      str1 = "";
    else if (Object.op_Implicit((Object) component6))
      str1 = component6.description;
    else if (Object.op_Inequality((Object) component3, (Object) null))
    {
      str1 = component3.Def.Effect;
      str2 = component3.Desc;
    }
    else if (Object.op_Inequality((Object) component4, (Object) null))
    {
      str1 = component4.Def.Effect;
      str2 = component4.Desc;
    }
    else if (Object.op_Inequality((Object) component7, (Object) null))
    {
      EdiblesManager.FoodInfo foodInfo = component7.FoodInfo;
      str1 += string.Format((string) STRINGS.UI.GAMEOBJECTEFFECTS.CALORIES, (object) GameUtil.GetFormattedCalories(foodInfo.CaloriesPerUnit));
    }
    else if (Object.op_Inequality((Object) component5, (Object) null))
      str1 = component5.element.FullDescription(false);
    else if (Object.op_Inequality((Object) component2, (Object) null))
    {
      Element elementByHash = ElementLoader.FindElementByHash(component2.ElementID);
      str1 = elementByHash != null ? elementByHash.FullDescription(false) : "";
    }
    List<Descriptor> gameObjectEffects = GameUtil.GetGameObjectEffects(target, true);
    bool flag1 = gameObjectEffects.Count > 0;
    ((Component) this.descriptionContainer).gameObject.SetActive(flag1);
    ((Component) this.descriptionContainer.descriptors).gameObject.SetActive(flag1);
    if (flag1)
      this.descriptionContainer.descriptors.SetDescriptors((IList<Descriptor>) gameObjectEffects);
    ((TMP_Text) this.descriptionContainer.description).text = str1;
    ((TMP_Text) this.descriptionContainer.flavour).text = str2;
    bool flag2 = Util.IsNullOrWhiteSpace(str1) && Util.IsNullOrWhiteSpace(str2) && !flag1;
    this.infoPanel.gameObject.SetActive(Object.op_Equality((Object) component1, (Object) null) && !flag2);
    ((Component) this.descriptionContainer).gameObject.SetActive(this.infoPanel.activeSelf);
    ((Component) this.descriptionContainer.flavour).gameObject.SetActive(str2 != "" && str2 != "\n");
    if (!((Component) this.vitalsPanel).gameObject.activeSelf || amounts.Count != 0)
      return;
    ((Component) this.vitalsPanel).gameObject.SetActive(false);
  }

  private void RefreshBreedingChance()
  {
    if (Object.op_Equality((Object) this.selectedTarget, (Object) null))
    {
      ((Component) this.fertilityPanel).gameObject.SetActive(false);
    }
    else
    {
      FertilityMonitor.Instance smi = this.selectedTarget.GetSMI<FertilityMonitor.Instance>();
      if (smi == null)
      {
        ((Component) this.fertilityPanel).gameObject.SetActive(false);
      }
      else
      {
        int num1 = 0;
        foreach (FertilityMonitor.BreedingChance breedingChance in smi.breedingChances)
        {
          List<FertilityModifier> forTag = Db.Get().FertilityModifiers.GetForTag(breedingChance.egg);
          int num2;
          if (forTag.Count > 0)
          {
            string str = "";
            foreach (FertilityModifier fertilityModifier in forTag)
              str += string.Format((string) STRINGS.UI.DETAILTABS.EGG_CHANCES.CHANCE_MOD_FORMAT, (object) fertilityModifier.GetTooltip());
            CollapsibleDetailContentPanel fertilityPanel = this.fertilityPanel;
            num2 = num1++;
            string id = "breeding_" + num2.ToString();
            string text = string.Format((string) STRINGS.UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, (object) breedingChance.egg.ProperName(), (object) GameUtil.GetFormattedPercent(breedingChance.weight * 100f));
            string tooltip = string.Format((string) STRINGS.UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP, (object) breedingChance.egg.ProperName(), (object) GameUtil.GetFormattedPercent(breedingChance.weight * 100f), (object) str);
            fertilityPanel.SetLabel(id, text, tooltip);
          }
          else
          {
            CollapsibleDetailContentPanel fertilityPanel = this.fertilityPanel;
            num2 = num1++;
            string id = "breeding_" + num2.ToString();
            string text = string.Format((string) STRINGS.UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT, (object) breedingChance.egg.ProperName(), (object) GameUtil.GetFormattedPercent(breedingChance.weight * 100f));
            string tooltip = string.Format((string) STRINGS.UI.DETAILTABS.EGG_CHANCES.CHANCE_FORMAT_TOOLTIP_NOMOD, (object) breedingChance.egg.ProperName(), (object) GameUtil.GetFormattedPercent(breedingChance.weight * 100f));
            fertilityPanel.SetLabel(id, text, tooltip);
          }
        }
        this.fertilityPanel.Commit();
      }
    }
  }

  private void RefreshStorage()
  {
    if (Object.op_Equality((Object) this.selectedTarget, (Object) null))
    {
      this.StoragePanel.gameObject.SetActive(false);
    }
    else
    {
      IStorage[] componentsInChildren = this.selectedTarget.GetComponentsInChildren<IStorage>();
      if (componentsInChildren == null)
      {
        this.StoragePanel.gameObject.SetActive(false);
      }
      else
      {
        IStorage[] all = Array.FindAll<IStorage>(componentsInChildren, (Predicate<IStorage>) (n => n.ShouldShowInUI()));
        if (all.Length == 0)
        {
          this.StoragePanel.gameObject.SetActive(false);
        }
        else
        {
          this.StoragePanel.gameObject.SetActive(true);
          string str1 = (string) (Object.op_Inequality((Object) this.selectedTarget.GetComponent<MinionIdentity>(), (Object) null) ? STRINGS.UI.DETAILTABS.DETAILS.GROUPNAME_MINION_CONTENTS : STRINGS.UI.DETAILTABS.DETAILS.GROUPNAME_CONTENTS);
          ((TMP_Text) this.StoragePanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = str1;
          foreach (KeyValuePair<string, GameObject> storageLabel in this.storageLabels)
            storageLabel.Value.SetActive(false);
          int num = 0;
          foreach (IStorage storage in all)
          {
            ListPool<Tuple<string, TextStyleSetting>, SimpleInfoScreen>.PooledList pooledList = ListPool<Tuple<string, TextStyleSetting>, SimpleInfoScreen>.Allocate();
            foreach (GameObject go in storage.GetItems())
            {
              if (!Object.op_Equality((Object) go, (Object) null))
              {
                PrimaryElement component1 = go.GetComponent<PrimaryElement>();
                if (!Object.op_Inequality((Object) component1, (Object) null) || (double) component1.Mass != 0.0)
                {
                  Rottable.Instance smi = go.GetSMI<Rottable.Instance>();
                  HighEnergyParticleStorage component2 = go.GetComponent<HighEnergyParticleStorage>();
                  string str2 = "";
                  ((List<Tuple<string, TextStyleSetting>>) pooledList).Clear();
                  if (Object.op_Inequality((Object) component1, (Object) null) && Object.op_Equality((Object) component2, (Object) null))
                  {
                    string unitFormattedName = GameUtil.GetUnitFormattedName(go);
                    string str3 = string.Format((string) STRINGS.UI.DETAILTABS.DETAILS.CONTENTS_MASS, (object) unitFormattedName, (object) GameUtil.GetFormattedMass(component1.Mass));
                    str2 = string.Format((string) STRINGS.UI.DETAILTABS.DETAILS.CONTENTS_TEMPERATURE, (object) str3, (object) GameUtil.GetFormattedTemperature(component1.Temperature));
                  }
                  if (Object.op_Inequality((Object) component2, (Object) null))
                  {
                    string name = (string) ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME;
                    str2 = string.Format((string) STRINGS.UI.DETAILTABS.DETAILS.CONTENTS_MASS, (object) name, (object) GameUtil.GetFormattedHighEnergyParticles(component2.Particles));
                  }
                  if (smi != null)
                  {
                    string str4 = smi.StateString();
                    if (!string.IsNullOrEmpty(str4))
                      str2 += string.Format((string) STRINGS.UI.DETAILTABS.DETAILS.CONTENTS_ROTTABLE, (object) str4);
                    ((List<Tuple<string, TextStyleSetting>>) pooledList).Add(new Tuple<string, TextStyleSetting>(smi.GetToolTip(), PluginAssets.Instance.defaultTextStyleSetting));
                  }
                  if (component1.DiseaseIdx != byte.MaxValue)
                  {
                    str2 += string.Format((string) STRINGS.UI.DETAILTABS.DETAILS.CONTENTS_DISEASED, (object) GameUtil.GetFormattedDisease(component1.DiseaseIdx, component1.DiseaseCount));
                    string formattedDisease = GameUtil.GetFormattedDisease(component1.DiseaseIdx, component1.DiseaseCount, true);
                    ((List<Tuple<string, TextStyleSetting>>) pooledList).Add(new Tuple<string, TextStyleSetting>(formattedDisease, PluginAssets.Instance.defaultTextStyleSetting));
                  }
                  GameObject storageLabel = this.AddOrGetStorageLabel(this.storageLabels, this.StoragePanel, "storage_" + num.ToString());
                  ++num;
                  ((TMP_Text) storageLabel.GetComponentInChildren<LocText>()).text = str2;
                  storageLabel.GetComponentInChildren<ToolTip>().ClearMultiStringTooltip();
                  foreach (Tuple<string, TextStyleSetting> tuple in (List<Tuple<string, TextStyleSetting>>) pooledList)
                    storageLabel.GetComponentInChildren<ToolTip>().AddMultiStringTooltip(tuple.first, tuple.second);
                  KButton component3 = storageLabel.GetComponent<KButton>();
                  GameObject select_target = go;
                  System.Action action = (System.Action) (() => SelectTool.Instance.Select(select_target.GetComponent<KSelectable>()));
                  component3.onClick += action;
                  if (storage.allowUIItemRemoval)
                  {
                    Transform transform = storageLabel.transform.Find("removeAttributeButton");
                    if (Object.op_Inequality((Object) transform, (Object) null))
                    {
                      KButton component4 = ((Component) transform).GetComponent<KButton>();
                      ((Behaviour) component4).enabled = true;
                      ((Component) component4).gameObject.SetActive(true);
                      GameObject select_item = go;
                      IStorage selected_storage = storage;
                      component4.onClick += (System.Action) (() => selected_storage.Drop(select_item));
                    }
                  }
                }
              }
            }
            pooledList.Recycle();
          }
          if (num != 0)
            return;
          ((TMP_Text) this.AddOrGetStorageLabel(this.storageLabels, this.StoragePanel, "empty").GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.DETAILTABS.DETAILS.STORAGE_EMPTY;
        }
      }
    }
  }

  private void CreateWorldTraitRow()
  {
    GameObject gameObject = Util.KInstantiateUI(this.iconLabelRow, ((Component) this.worldTraitsPanel.Content).gameObject, true);
    this.worldTraitRows.Add(gameObject);
    HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
    ((Component) component.GetReference<Image>("Icon")).gameObject.SetActive(false);
    ((Component) component.GetReference<LocText>("ValueLabel")).gameObject.SetActive(false);
  }

  private void RefreshWorld()
  {
    WorldContainer worldContainer = Object.op_Equality((Object) this.selectedTarget, (Object) null) ? (WorldContainer) null : this.selectedTarget.GetComponent<WorldContainer>();
    AsteroidGridEntity asteroidGridEntity = Object.op_Equality((Object) this.selectedTarget, (Object) null) ? (AsteroidGridEntity) null : this.selectedTarget.GetComponent<AsteroidGridEntity>();
    bool flag = ManagementMenu.Instance.IsScreenOpen((KScreen) ClusterMapScreen.Instance) && Object.op_Inequality((Object) worldContainer, (Object) null) && Object.op_Inequality((Object) asteroidGridEntity, (Object) null);
    ((Component) this.worldBiomesPanel).gameObject.SetActive(flag);
    ((Component) this.worldGeysersPanel).gameObject.SetActive(flag);
    ((Component) this.worldTraitsPanel).gameObject.SetActive(flag);
    if (!flag)
      return;
    foreach (KeyValuePair<Tag, GameObject> biomeRow in this.biomeRows)
      biomeRow.Value.SetActive(false);
    if (worldContainer.Biomes != null)
    {
      foreach (string biome in worldContainer.Biomes)
      {
        Sprite biomeSprite = GameUtil.GetBiomeSprite(biome);
        if (!this.biomeRows.ContainsKey(Tag.op_Implicit(biome)))
        {
          this.biomeRows.Add(Tag.op_Implicit(biome), Util.KInstantiateUI(this.bigIconLabelRow, ((Component) this.worldBiomesPanel.Content).gameObject, true));
          HierarchyReferences component = this.biomeRows[Tag.op_Implicit(biome)].GetComponent<HierarchyReferences>();
          component.GetReference<Image>("Icon").sprite = biomeSprite;
          ((TMP_Text) component.GetReference<LocText>("NameLabel")).SetText(STRINGS.UI.FormatAsLink(StringEntry.op_Implicit(Strings.Get("STRINGS.SUBWORLDS." + biome.ToUpper() + ".NAME")), "BIOME" + biome.ToUpper()));
          ((TMP_Text) component.GetReference<LocText>("DescriptionLabel")).SetText(StringEntry.op_Implicit(Strings.Get("STRINGS.SUBWORLDS." + biome.ToUpper() + ".DESC")));
        }
        this.biomeRows[Tag.op_Implicit(biome)].SetActive(true);
      }
    }
    else
      ((Component) this.worldBiomesPanel).gameObject.SetActive(false);
    List<Tag> tagList = new List<Tag>();
    foreach (Geyser geyser in Object.FindObjectsOfType<Geyser>())
    {
      if (geyser.GetMyWorldId() == worldContainer.id)
        tagList.Add(((Component) geyser).PrefabID());
    }
    tagList.AddRange((IEnumerable<Tag>) SaveGame.Instance.worldGenSpawner.GetUnspawnedWithType<Geyser>(worldContainer.id));
    tagList.AddRange((IEnumerable<Tag>) SaveGame.Instance.worldGenSpawner.GetSpawnersWithTag(Tag.op_Implicit("OilWell"), worldContainer.id, true));
    foreach (KeyValuePair<Tag, GameObject> geyserRow in this.geyserRows)
      geyserRow.Value.SetActive(false);
    foreach (Tag tag in tagList)
    {
      Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) tag);
      if (!this.geyserRows.ContainsKey(tag))
      {
        this.geyserRows.Add(tag, Util.KInstantiateUI(this.iconLabelRow, ((Component) this.worldGeysersPanel.Content).gameObject, true));
        HierarchyReferences component = this.geyserRows[tag].GetComponent<HierarchyReferences>();
        component.GetReference<Image>("Icon").sprite = uiSprite.first;
        ((Graphic) component.GetReference<Image>("Icon")).color = uiSprite.second;
        ((TMP_Text) component.GetReference<LocText>("NameLabel")).SetText(Assets.GetPrefab(tag).GetProperName());
        ((Component) component.GetReference<LocText>("ValueLabel")).gameObject.SetActive(false);
      }
      this.geyserRows[tag].SetActive(true);
    }
    int count = SaveGame.Instance.worldGenSpawner.GetSpawnersWithTag(Tag.op_Implicit("GeyserGeneric"), worldContainer.id).Count;
    if (count > 0)
    {
      Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) "GeyserGeneric");
      Tag key = Tag.op_Implicit("GeyserGeneric");
      if (!this.geyserRows.ContainsKey(key))
      {
        this.geyserRows.Add(key, Util.KInstantiateUI(this.iconLabelRow, ((Component) this.worldGeysersPanel.Content).gameObject, true));
        HierarchyReferences component = this.geyserRows[key].GetComponent<HierarchyReferences>();
        component.GetReference<Image>("Icon").sprite = uiSprite.first;
        ((Graphic) component.GetReference<Image>("Icon")).color = uiSprite.second;
        ((TMP_Text) component.GetReference<LocText>("NameLabel")).SetText(STRINGS.UI.DETAILTABS.SIMPLEINFO.UNKNOWN_GEYSERS.Replace("{num}", count.ToString()));
        ((Component) component.GetReference<LocText>("ValueLabel")).gameObject.SetActive(false);
      }
      this.geyserRows[key].SetActive(true);
    }
    Tag key1 = Tag.op_Implicit("NoGeysers");
    if (!this.geyserRows.ContainsKey(key1))
    {
      this.geyserRows.Add(key1, Util.KInstantiateUI(this.iconLabelRow, ((Component) this.worldGeysersPanel.Content).gameObject, true));
      HierarchyReferences component = this.geyserRows[key1].GetComponent<HierarchyReferences>();
      component.GetReference<Image>("Icon").sprite = Assets.GetSprite(HashedString.op_Implicit("icon_action_cancel"));
      ((TMP_Text) component.GetReference<LocText>("NameLabel")).SetText((string) STRINGS.UI.DETAILTABS.SIMPLEINFO.NO_GEYSERS);
      ((Component) component.GetReference<LocText>("ValueLabel")).gameObject.SetActive(false);
    }
    this.geyserRows[key1].gameObject.SetActive(tagList.Count == 0);
    List<string> worldTraitIds = worldContainer.WorldTraitIds;
    if (worldTraitIds != null)
    {
      for (int index = 0; index < worldTraitIds.Count; ++index)
      {
        if (index > this.worldTraitRows.Count - 1)
          this.CreateWorldTraitRow();
        WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(worldTraitIds[index], false);
        Image reference = this.worldTraitRows[index].GetComponent<HierarchyReferences>().GetReference<Image>("Icon");
        if (cachedWorldTrait != null)
        {
          Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(cachedWorldTrait.filePath.Substring(cachedWorldTrait.filePath.LastIndexOf("/") + 1)));
          ((Component) reference).gameObject.SetActive(true);
          reference.sprite = Object.op_Equality((Object) sprite, (Object) null) ? Assets.GetSprite(HashedString.op_Implicit("unknown")) : sprite;
          ((Graphic) reference).color = Util.ColorFromHex(cachedWorldTrait.colorHex);
          ((TMP_Text) this.worldTraitRows[index].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel")).SetText(StringEntry.op_Implicit(Strings.Get(cachedWorldTrait.name)));
          this.worldTraitRows[index].AddOrGet<ToolTip>().SetSimpleTooltip(StringEntry.op_Implicit(Strings.Get(cachedWorldTrait.description)));
        }
        else
        {
          Sprite sprite = Assets.GetSprite(HashedString.op_Implicit("NoTraits"));
          ((Component) reference).gameObject.SetActive(true);
          reference.sprite = sprite;
          ((Graphic) reference).color = Color.white;
          ((TMP_Text) this.worldTraitRows[index].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel")).SetText((string) WORLD_TRAITS.MISSING_TRAIT);
          this.worldTraitRows[index].AddOrGet<ToolTip>().SetSimpleTooltip("");
        }
      }
      for (int index = 0; index < this.worldTraitRows.Count; ++index)
        this.worldTraitRows[index].SetActive(index < worldTraitIds.Count);
      if (worldTraitIds.Count == 0)
      {
        if (this.worldTraitRows.Count < 1)
          this.CreateWorldTraitRow();
        Image reference = this.worldTraitRows[0].GetComponent<HierarchyReferences>().GetReference<Image>("Icon");
        Sprite sprite = Assets.GetSprite(HashedString.op_Implicit("NoTraits"));
        ((Component) reference).gameObject.SetActive(true);
        reference.sprite = sprite;
        ((Graphic) reference).color = Color.black;
        ((TMP_Text) this.worldTraitRows[0].GetComponent<HierarchyReferences>().GetReference<LocText>("NameLabel")).SetText((string) WORLD_TRAITS.NO_TRAITS.NAME_SHORTHAND);
        this.worldTraitRows[0].AddOrGet<ToolTip>().SetSimpleTooltip((string) WORLD_TRAITS.NO_TRAITS.DESCRIPTION);
        this.worldTraitRows[0].SetActive(true);
      }
    }
    for (int index = this.surfaceConditionRows.Count - 1; index >= 0; --index)
      Util.KDestroyGameObject(this.surfaceConditionRows[index]);
    this.surfaceConditionRows.Clear();
    GameObject gameObject1 = Util.KInstantiateUI(this.iconLabelRow, ((Component) this.worldTraitsPanel.Content).gameObject, true);
    HierarchyReferences component1 = gameObject1.GetComponent<HierarchyReferences>();
    component1.GetReference<Image>("Icon").sprite = Assets.GetSprite(HashedString.op_Implicit("overlay_lights"));
    ((TMP_Text) component1.GetReference<LocText>("NameLabel")).SetText((string) STRINGS.UI.CLUSTERMAP.ASTEROIDS.SURFACE_CONDITIONS.LIGHT);
    ((TMP_Text) component1.GetReference<LocText>("ValueLabel")).SetText(GameUtil.GetFormattedLux(worldContainer.SunlightFixedTraits[worldContainer.sunlightFixedTrait]));
    ((TMP_Text) component1.GetReference<LocText>("ValueLabel")).alignment = (TextAlignmentOptions) 4100;
    this.surfaceConditionRows.Add(gameObject1);
    GameObject gameObject2 = Util.KInstantiateUI(this.iconLabelRow, ((Component) this.worldTraitsPanel.Content).gameObject, true);
    HierarchyReferences component2 = gameObject2.GetComponent<HierarchyReferences>();
    component2.GetReference<Image>("Icon").sprite = Assets.GetSprite(HashedString.op_Implicit("overlay_radiation"));
    ((TMP_Text) component2.GetReference<LocText>("NameLabel")).SetText((string) STRINGS.UI.CLUSTERMAP.ASTEROIDS.SURFACE_CONDITIONS.RADIATION);
    ((TMP_Text) component2.GetReference<LocText>("ValueLabel")).SetText(GameUtil.GetFormattedRads((float) worldContainer.CosmicRadiationFixedTraits[worldContainer.cosmicRadiationFixedTrait]));
    ((TMP_Text) component2.GetReference<LocText>("ValueLabel")).alignment = (TextAlignmentOptions) 4100;
    this.surfaceConditionRows.Add(gameObject2);
  }

  private void RefreshProcessConditions()
  {
    foreach (GameObject processConditionRow in this.processConditionRows)
      Util.KDestroyGameObject(processConditionRow);
    this.processConditionRows.Clear();
    if (!DlcManager.FeatureClusterSpaceEnabled())
    {
      if (Object.op_Inequality((Object) this.selectedTarget.GetComponent<LaunchableRocket>(), (Object) null))
      {
        this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketPrep);
        this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketStorage);
        this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketBoard);
      }
      else
        this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.All);
    }
    else if (Object.op_Inequality((Object) this.selectedTarget.GetComponent<LaunchPad>(), (Object) null) || Object.op_Inequality((Object) this.selectedTarget.GetComponent<RocketProcessConditionDisplayTarget>(), (Object) null))
    {
      this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketFlight);
      this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketPrep);
      this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketStorage);
      this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.RocketBoard);
    }
    else
      this.RefreshProcessConditionsForType(ProcessCondition.ProcessConditionType.All);
  }

  private void RefreshProcessConditionsForType(
    ProcessCondition.ProcessConditionType conditionType)
  {
    List<ProcessCondition> conditionSet = this.selectedTarget.GetComponent<IProcessConditionSet>().GetConditionSet(conditionType);
    if (conditionSet.Count == 0)
      return;
    HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(((Component) this.processConditionHeader).gameObject, ((Component) this.processConditionContainer.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject, true);
    ((TMP_Text) hierarchyReferences.GetReference<LocText>("Label")).text = StringEntry.op_Implicit(Strings.Get("STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS." + conditionType.ToString().ToUpper()));
    ((Component) hierarchyReferences).GetComponent<ToolTip>().toolTip = StringEntry.op_Implicit(Strings.Get("STRINGS.UI.DETAILTABS.PROCESS_CONDITIONS." + conditionType.ToString().ToUpper() + "_TOOLTIP"));
    this.processConditionRows.Add(((Component) hierarchyReferences).gameObject);
    List<ProcessCondition> processConditionList = new List<ProcessCondition>();
    foreach (ProcessCondition processCondition in conditionSet)
    {
      ProcessCondition condition = processCondition;
      if (condition.ShowInUI() && (condition.GetType() == typeof (RequireAttachedComponent) || processConditionList.Find((Predicate<ProcessCondition>) (match => match.GetType() == condition.GetType())) == null))
      {
        processConditionList.Add(condition);
        GameObject row = Util.KInstantiateUI(this.processConditionRow, ((Component) this.processConditionContainer.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject, true);
        this.processConditionRows.Add(row);
        ConditionListSideScreen.SetRowState(row, condition);
      }
    }
  }

  public GameObject AddOrGetStorageLabel(
    Dictionary<string, GameObject> labels,
    GameObject panel,
    string id)
  {
    GameObject storageLabel;
    if (labels.ContainsKey(id))
    {
      storageLabel = labels[id];
      storageLabel.GetComponent<KButton>().ClearOnClick();
      Transform transform = storageLabel.transform.Find("removeAttributeButton");
      if (Object.op_Inequality((Object) transform, (Object) null))
      {
        KButton component = Util.FindComponent<KButton>((Component) transform);
        ((Behaviour) component).enabled = false;
        ((Component) component).gameObject.SetActive(false);
        component.ClearOnClick();
      }
    }
    else
    {
      storageLabel = Util.KInstantiate(this.attributesLabelButtonTemplate, ((Component) panel.GetComponent<CollapsibleDetailContentPanel>().Content).gameObject, (string) null);
      storageLabel.transform.localScale = new Vector3(1f, 1f, 1f);
      labels[id] = storageLabel;
    }
    storageLabel.SetActive(true);
    return storageLabel;
  }

  private void RefreshStress()
  {
    MinionIdentity identity = Object.op_Inequality((Object) this.selectedTarget, (Object) null) ? this.selectedTarget.GetComponent<MinionIdentity>() : (MinionIdentity) null;
    if (Object.op_Equality((Object) identity, (Object) null))
    {
      this.stressPanel.SetActive(false);
    }
    else
    {
      List<ReportManager.ReportEntry.Note> stressNotes = new List<ReportManager.ReportEntry.Note>();
      this.stressPanel.SetActive(true);
      ((TMP_Text) this.stressPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel).text = (string) STRINGS.UI.DETAILTABS.STATS.GROUPNAME_STRESS;
      ReportManager.ReportEntry reportEntry1 = ReportManager.Instance.TodaysReport.reportEntries.Find((Predicate<ReportManager.ReportEntry>) (entry => entry.reportType == ReportManager.ReportType.StressDelta));
      this.stressDrawer.BeginDrawing();
      float num = 0.0f;
      stressNotes.Clear();
      int index1 = reportEntry1.contextEntries.FindIndex((Predicate<ReportManager.ReportEntry>) (entry => entry.context == identity.GetProperName()));
      ReportManager.ReportEntry reportEntry2 = index1 != -1 ? reportEntry1.contextEntries[index1] : (ReportManager.ReportEntry) null;
      if (reportEntry2 != null)
      {
        reportEntry2.IterateNotes((Action<ReportManager.ReportEntry.Note>) (note => stressNotes.Add(note)));
        stressNotes.Sort((Comparison<ReportManager.ReportEntry.Note>) ((a, b) => a.value.CompareTo(b.value)));
        for (int index2 = 0; index2 < stressNotes.Count; ++index2)
        {
          this.stressDrawer.NewLabel(((double) stressNotes[index2].value > 0.0 ? UIConstants.ColorPrefixRed : "") + stressNotes[index2].note + ": " + Util.FormatTwoDecimalPlace(stressNotes[index2].value) + "%" + ((double) stressNotes[index2].value > 0.0 ? UIConstants.ColorSuffix : ""));
          num += stressNotes[index2].value;
        }
      }
      this.stressDrawer.NewLabel(((double) num > 0.0 ? UIConstants.ColorPrefixRed : "") + string.Format((string) STRINGS.UI.DETAILTABS.DETAILS.NET_STRESS, (object) Util.FormatTwoDecimalPlace(num)) + ((double) num > 0.0 ? UIConstants.ColorSuffix : ""));
      this.stressDrawer.EndDrawing();
    }
  }

  private void ShowAttributes(GameObject target)
  {
    Attributes attributes = target.GetAttributes();
    if (attributes == null)
      return;
    List<AttributeInstance> all = attributes.AttributeTable.FindAll((Predicate<AttributeInstance>) (a => a.Attribute.ShowInUI == Klei.AI.Attribute.Display.General));
    if (all.Count <= 0)
      return;
    ((Component) this.descriptionContainer.descriptors).gameObject.SetActive(true);
    List<Descriptor> descriptors = new List<Descriptor>();
    foreach (AttributeInstance attributeInstance in all)
    {
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(string.Format("{0}: {1}", (object) attributeInstance.Name, (object) attributeInstance.GetFormattedValue()), attributeInstance.GetAttributeValueTooltip(), (Descriptor.DescriptorType) 1, false);
      ((Descriptor) ref descriptor).IncreaseIndent();
      descriptors.Add(descriptor);
    }
    this.descriptionContainer.descriptors.SetDescriptors((IList<Descriptor>) descriptors);
  }

  private void SetStamps(GameObject target)
  {
    for (int index = 0; index < this.stampContainer.transform.childCount; ++index)
      Object.Destroy((Object) ((Component) this.stampContainer.transform.GetChild(index)).gameObject);
    Object.op_Inequality((Object) target.GetComponent<BuildingComplete>(), (Object) null);
  }

  public void Sim1000ms(float dt)
  {
    if (!Object.op_Inequality((Object) this.selectedTarget, (Object) null) || this.selectedTarget.GetComponent<IProcessConditionSet>() == null)
      return;
    this.RefreshProcessConditions();
  }

  public void Sim4000ms(float dt)
  {
    this.RefreshWorld();
    this.spaceSimpleInfoPOIPanel.Refresh(this.spacePOIPanel, this.selectedTarget);
  }

  [DebuggerDisplay("{item.item.Name}")]
  public class StatusItemEntry : IRenderEveryTick
  {
    public StatusItemGroup.Entry item;
    public StatusItemCategory category;
    private LayoutElement spacerLayout;
    private GameObject widget;
    private ToolTip toolTip;
    private TextStyleSetting tooltipStyle;
    public Action<SimpleInfoScreen.StatusItemEntry> onDestroy;
    private Image image;
    private LocText text;
    private KButton button;
    public Color color;
    public TextStyleSetting style;
    private SimpleInfoScreen.StatusItemEntry.FadeStage fadeStage;
    private float fade;
    private float fadeInTime;
    private float fadeOutTime = 1.8f;

    public Image GetImage => this.image;

    public StatusItemEntry(
      StatusItemGroup.Entry item,
      StatusItemCategory category,
      GameObject status_item_prefab,
      Transform parent,
      TextStyleSetting tooltip_style,
      Color color,
      TextStyleSetting style,
      bool skip_fade,
      Action<SimpleInfoScreen.StatusItemEntry> onDestroy)
    {
      this.item = item;
      this.category = category;
      this.tooltipStyle = tooltip_style;
      this.onDestroy = onDestroy;
      this.color = color;
      this.style = style;
      this.widget = Util.KInstantiateUI(status_item_prefab, ((Component) parent).gameObject, false);
      this.text = this.widget.GetComponentInChildren<LocText>(true);
      SetTextStyleSetting.ApplyStyle((TextMeshProUGUI) this.text, style);
      this.toolTip = this.widget.GetComponentInChildren<ToolTip>(true);
      this.image = this.widget.GetComponentInChildren<Image>(true);
      item.SetIcon(this.image);
      this.widget.SetActive(true);
      this.toolTip.OnToolTip = new Func<string>(this.OnToolTip);
      this.button = this.widget.GetComponentInChildren<KButton>();
      if (item.item.statusItemClickCallback != null)
        this.button.onClick += new System.Action(this.OnClick);
      else
        ((Behaviour) this.button).enabled = false;
      this.fadeStage = skip_fade ? SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT : SimpleInfoScreen.StatusItemEntry.FadeStage.IN;
      SimAndRenderScheduler.instance.Add((object) this, false);
      this.Refresh();
      this.SetColor();
    }

    internal void SetSprite(TintedSprite sprite)
    {
      if (sprite == null)
        return;
      this.image.sprite = sprite.sprite;
    }

    public int GetIndex() => this.widget.transform.GetSiblingIndex();

    public void SetIndex(int index) => this.widget.transform.SetSiblingIndex(index);

    public void RenderEveryTick(float dt)
    {
      switch (this.fadeStage)
      {
        case SimpleInfoScreen.StatusItemEntry.FadeStage.IN:
          this.fade = Mathf.Min(this.fade + Time.deltaTime / this.fadeInTime, 1f);
          this.SetColor(this.fade);
          if ((double) this.fade < 1.0)
            break;
          this.fadeStage = SimpleInfoScreen.StatusItemEntry.FadeStage.WAIT;
          break;
        case SimpleInfoScreen.StatusItemEntry.FadeStage.OUT:
          this.SetColor(this.fade);
          this.fade = Mathf.Max(this.fade - Time.deltaTime / this.fadeOutTime, 0.0f);
          if ((double) this.fade > 0.0)
            break;
          this.Destroy(true);
          break;
      }
    }

    private string OnToolTip()
    {
      this.item.ShowToolTip(this.toolTip, this.tooltipStyle);
      return "";
    }

    private void OnClick() => this.item.OnClick();

    public void Refresh()
    {
      string name = this.item.GetName();
      if (!(name != ((TMP_Text) this.text).text))
        return;
      ((TMP_Text) this.text).text = name;
      this.SetColor();
    }

    private void SetColor(float alpha = 1f)
    {
      Color color;
      // ISSUE: explicit constructor call
      ((Color) ref color).\u002Ector(this.color.r, this.color.g, this.color.b, alpha);
      ((Graphic) this.image).color = color;
      ((Graphic) this.text).color = color;
    }

    public void Destroy(bool immediate)
    {
      if (Object.op_Inequality((Object) this.toolTip, (Object) null))
        this.toolTip.OnToolTip = (Func<string>) null;
      if (Object.op_Inequality((Object) this.button, (Object) null) && ((Behaviour) this.button).enabled)
        this.button.onClick -= new System.Action(this.OnClick);
      if (immediate)
      {
        if (this.onDestroy != null)
          this.onDestroy(this);
        SimAndRenderScheduler.instance.Remove((object) this);
        Object.Destroy((Object) this.widget);
      }
      else
      {
        this.fade = 0.5f;
        this.fadeStage = SimpleInfoScreen.StatusItemEntry.FadeStage.OUT;
      }
    }

    private enum FadeStage
    {
      IN,
      WAIT,
      OUT,
    }
  }
}
