// Decompiled with JetBrains decompiler
// Type: ProductInfoScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductInfoScreen : KScreen
{
  public TitleBar titleBar;
  public GameObject ProductDescriptionPane;
  public LocText productDescriptionText;
  public DescriptorPanel ProductRequirementsPane;
  public DescriptorPanel ProductEffectsPane;
  public GameObject ProductFlavourPane;
  public LocText productFlavourText;
  public RectTransform BGPanel;
  public MaterialSelectionPanel materialSelectionPanelPrefab;
  public FacadeSelectionPanel facadeSelectionPanelPrefab;
  private Dictionary<string, GameObject> descLabels = new Dictionary<string, GameObject>();
  public MultiToggle sandboxInstantBuildToggle;
  [NonSerialized]
  public MaterialSelectionPanel materialSelectionPanel;
  [SerializeField]
  private FacadeSelectionPanel facadeSelectionPanel;
  [NonSerialized]
  public BuildingDef currentDef;
  public System.Action onElementsFullySelected;
  private bool expandedInfo = true;
  private bool configuring;

  public FacadeSelectionPanel FacadeSelectionPanel => this.facadeSelectionPanel;

  private void RefreshScreen()
  {
    if (Object.op_Inequality((Object) this.currentDef, (Object) null))
      this.SetTitle(this.currentDef);
    else
      this.ClearProduct();
  }

  public void ClearProduct(bool deactivateTool = true)
  {
    if (Object.op_Equality((Object) this.materialSelectionPanel, (Object) null))
      return;
    this.currentDef = (BuildingDef) null;
    this.materialSelectionPanel.ClearMaterialToggles();
    if (Object.op_Equality((Object) PlayerController.Instance.ActiveTool, (Object) BuildTool.Instance) & deactivateTool)
      BuildTool.Instance.Deactivate();
    if (Object.op_Equality((Object) PlayerController.Instance.ActiveTool, (Object) UtilityBuildTool.Instance) || Object.op_Equality((Object) PlayerController.Instance.ActiveTool, (Object) WireBuildTool.Instance))
      ToolMenu.Instance.ClearSelection();
    this.ClearLabels();
    this.Show(false);
  }

  public void Awake()
  {
    ((KMonoBehaviour) this).Awake();
    this.facadeSelectionPanel = Util.KInstantiateUI<FacadeSelectionPanel>(((Component) this.facadeSelectionPanelPrefab).gameObject, ((Component) this).gameObject, false);
    this.facadeSelectionPanel.OnFacadeSelectionChanged += new System.Action(this.OnFacadeSelectionChanged);
    this.materialSelectionPanel = Util.KInstantiateUI<MaterialSelectionPanel>(((Component) this.materialSelectionPanelPrefab).gameObject, ((Component) this).gameObject, false);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Inequality((Object) BuildingGroupScreen.Instance, (Object) null))
    {
      BuildingGroupScreen instance1 = BuildingGroupScreen.Instance;
      // ISSUE: method pointer
      instance1.pointerEnterActions = (KScreen.PointerEnterActions) Delegate.Combine((Delegate) instance1.pointerEnterActions, (Delegate) new KScreen.PointerEnterActions((object) this, __methodptr(CheckMouseOver)));
      BuildingGroupScreen instance2 = BuildingGroupScreen.Instance;
      // ISSUE: method pointer
      instance2.pointerExitActions = (KScreen.PointerExitActions) Delegate.Combine((Delegate) instance2.pointerExitActions, (Delegate) new KScreen.PointerExitActions((object) this, __methodptr(CheckMouseOver)));
    }
    if (Object.op_Inequality((Object) PlanScreen.Instance, (Object) null))
    {
      PlanScreen instance3 = PlanScreen.Instance;
      // ISSUE: method pointer
      instance3.pointerEnterActions = (KScreen.PointerEnterActions) Delegate.Combine((Delegate) instance3.pointerEnterActions, (Delegate) new KScreen.PointerEnterActions((object) this, __methodptr(CheckMouseOver)));
      PlanScreen instance4 = PlanScreen.Instance;
      // ISSUE: method pointer
      instance4.pointerExitActions = (KScreen.PointerExitActions) Delegate.Combine((Delegate) instance4.pointerExitActions, (Delegate) new KScreen.PointerExitActions((object) this, __methodptr(CheckMouseOver)));
    }
    if (Object.op_Inequality((Object) BuildMenu.Instance, (Object) null))
    {
      BuildMenu instance5 = BuildMenu.Instance;
      // ISSUE: method pointer
      instance5.pointerEnterActions = (KScreen.PointerEnterActions) Delegate.Combine((Delegate) instance5.pointerEnterActions, (Delegate) new KScreen.PointerEnterActions((object) this, __methodptr(CheckMouseOver)));
      BuildMenu instance6 = BuildMenu.Instance;
      // ISSUE: method pointer
      instance6.pointerExitActions = (KScreen.PointerExitActions) Delegate.Combine((Delegate) instance6.pointerExitActions, (Delegate) new KScreen.PointerExitActions((object) this, __methodptr(CheckMouseOver)));
    }
    // ISSUE: method pointer
    this.pointerEnterActions = (KScreen.PointerEnterActions) Delegate.Combine((Delegate) this.pointerEnterActions, (Delegate) new KScreen.PointerEnterActions((object) this, __methodptr(CheckMouseOver)));
    // ISSUE: method pointer
    this.pointerExitActions = (KScreen.PointerExitActions) Delegate.Combine((Delegate) this.pointerExitActions, (Delegate) new KScreen.PointerExitActions((object) this, __methodptr(CheckMouseOver)));
    this.ConsumeMouseScroll = true;
    this.sandboxInstantBuildToggle.ChangeState(SandboxToolParameterMenu.instance.settings.InstantBuild ? 1 : 0);
    this.sandboxInstantBuildToggle.onClick += (System.Action) (() =>
    {
      SandboxToolParameterMenu.instance.settings.InstantBuild = !SandboxToolParameterMenu.instance.settings.InstantBuild;
      this.sandboxInstantBuildToggle.ChangeState(SandboxToolParameterMenu.instance.settings.InstantBuild ? 1 : 0);
    });
    ((Component) this.sandboxInstantBuildToggle).gameObject.SetActive(Game.Instance.SandboxModeActive);
    Game.Instance.Subscribe(-1948169901, (Action<object>) (data => ((Component) this.sandboxInstantBuildToggle).gameObject.SetActive(Game.Instance.SandboxModeActive)));
  }

  public void ConfigureScreen(BuildingDef def) => this.ConfigureScreen(def, this.FacadeSelectionPanel.SelectedFacade);

  public void ConfigureScreen(BuildingDef def, string facadeID)
  {
    this.configuring = true;
    this.currentDef = def;
    this.SetTitle(def);
    this.SetDescription(def);
    this.SetEffects(def);
    this.facadeSelectionPanel.SetBuildingDef(def.PrefabID);
    this.facadeSelectionPanel.SelectedFacade = facadeID == null || !(facadeID != "DEFAULT_FACADE") || !(Db.GetBuildingFacades().Get(facadeID).PrefabID == def.PrefabID) ? "DEFAULT_FACADE" : facadeID;
    this.SetMaterials(def);
    this.configuring = false;
  }

  private void ExpandInfo(PointerEventData data) => this.ToggleExpandedInfo(true);

  private void CollapseInfo(PointerEventData data) => this.ToggleExpandedInfo(false);

  public void ToggleExpandedInfo(bool state)
  {
    this.expandedInfo = state;
    if (Object.op_Inequality((Object) this.ProductDescriptionPane, (Object) null))
      this.ProductDescriptionPane.SetActive(this.expandedInfo);
    if (Object.op_Inequality((Object) this.ProductRequirementsPane, (Object) null))
      ((Component) this.ProductRequirementsPane).gameObject.SetActive(this.expandedInfo && this.ProductRequirementsPane.HasDescriptors());
    if (Object.op_Inequality((Object) this.ProductEffectsPane, (Object) null))
      ((Component) this.ProductEffectsPane).gameObject.SetActive(this.expandedInfo && this.ProductEffectsPane.HasDescriptors());
    if (Object.op_Inequality((Object) this.ProductFlavourPane, (Object) null))
      this.ProductFlavourPane.SetActive(this.expandedInfo);
    if (!Object.op_Inequality((Object) this.materialSelectionPanel, (Object) null) || !Tag.op_Inequality(this.materialSelectionPanel.CurrentSelectedElement, Tag.op_Implicit((string) null)))
      return;
    this.materialSelectionPanel.ToggleShowDescriptorPanels(this.expandedInfo);
  }

  private void CheckMouseOver(PointerEventData data) => this.ToggleExpandedInfo(this.GetMouseOver || Object.op_Inequality((Object) PlanScreen.Instance, (Object) null) && (((Behaviour) PlanScreen.Instance).isActiveAndEnabled && PlanScreen.Instance.GetMouseOver || BuildingGroupScreen.Instance.GetMouseOver) || Object.op_Inequality((Object) BuildMenu.Instance, (Object) null) && ((Behaviour) BuildMenu.Instance).isActiveAndEnabled && BuildMenu.Instance.GetMouseOver);

  private void Update()
  {
    if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || !Object.op_Inequality((Object) this.currentDef, (Object) null) || !Tag.op_Inequality(this.materialSelectionPanel.CurrentSelectedElement, Tag.op_Implicit((string) null)) || MaterialSelector.AllowInsufficientMaterialBuild() || (double) this.currentDef.Mass[0] <= (double) ClusterManager.Instance.activeWorld.worldInventory.GetAmount(this.materialSelectionPanel.CurrentSelectedElement, true))
      return;
    this.materialSelectionPanel.AutoSelectAvailableMaterial();
  }

  private void SetTitle(BuildingDef def)
  {
    this.titleBar.SetTitle(def.Name);
    bool flag = Object.op_Inequality((Object) PlanScreen.Instance, (Object) null) && ((Behaviour) PlanScreen.Instance).isActiveAndEnabled && PlanScreen.Instance.IsDefBuildable(def) || Object.op_Inequality((Object) BuildMenu.Instance, (Object) null) && ((Behaviour) BuildMenu.Instance).isActiveAndEnabled && BuildMenu.Instance.BuildableState(def) == PlanScreen.RequirementsState.Complete;
    ((Component) this.titleBar).GetComponentInChildren<KImage>().ColorState = flag ? (KImage.ColorSelector) 0 : (KImage.ColorSelector) 2;
  }

  private void SetDescription(BuildingDef def)
  {
    if (Object.op_Equality((Object) def, (Object) null) || Object.op_Equality((Object) this.productFlavourText, (Object) null))
      return;
    string str1 = "";
    KPrefabID component1 = def.BuildingComplete.GetComponent<KPrefabID>();
    string str2 = "";
    foreach (Tag tag in component1.Tags)
    {
      string str3;
      if (CodexEntryGenerator.room_constraint_to_building_label_dict.TryGetValue(tag, out str3))
        str2 = str2 + "\n    • " + str3;
    }
    if (!string.IsNullOrWhiteSpace(str2))
      str1 += string.Format("<b>{0}</b>: {1}\n\n", (object) CODEX.HEADERS.BUILDINGTYPE, (object) str2);
    string str4 = str1 + def.Desc;
    Dictionary<Klei.AI.Attribute, float> dictionary1 = new Dictionary<Klei.AI.Attribute, float>();
    Dictionary<Klei.AI.Attribute, float> dictionary2 = new Dictionary<Klei.AI.Attribute, float>();
    foreach (Klei.AI.Attribute attribute in def.attributes)
    {
      if (!dictionary1.ContainsKey(attribute))
        dictionary1[attribute] = 0.0f;
    }
    foreach (AttributeModifier attributeModifier in def.attributeModifiers)
    {
      float num = 0.0f;
      Klei.AI.Attribute key = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
      dictionary1.TryGetValue(key, out num);
      num += attributeModifier.Value;
      dictionary1[key] = num;
    }
    if (Tag.op_Inequality(this.materialSelectionPanel.CurrentSelectedElement, Tag.op_Implicit((string) null)))
    {
      Element element = ElementLoader.GetElement(this.materialSelectionPanel.CurrentSelectedElement);
      if (element != null)
      {
        foreach (AttributeModifier attributeModifier in element.attributeModifiers)
        {
          float num = 0.0f;
          Klei.AI.Attribute key = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
          dictionary2.TryGetValue(key, out num);
          num += attributeModifier.Value;
          dictionary2[key] = num;
        }
      }
      else
      {
        PrefabAttributeModifiers component2 = Assets.TryGetPrefab(this.materialSelectionPanel.CurrentSelectedElement).GetComponent<PrefabAttributeModifiers>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          foreach (AttributeModifier descriptor in component2.descriptors)
          {
            float num = 0.0f;
            Klei.AI.Attribute key = Db.Get().BuildingAttributes.Get(descriptor.AttributeId);
            dictionary2.TryGetValue(key, out num);
            num += descriptor.Value;
            dictionary2[key] = num;
          }
        }
      }
    }
    if (dictionary1.Count > 0)
    {
      str4 += "\n\n";
      foreach (KeyValuePair<Klei.AI.Attribute, float> keyValuePair in dictionary1)
      {
        float num1 = 0.0f;
        dictionary1.TryGetValue(keyValuePair.Key, out num1);
        float num2 = 0.0f;
        string str5 = "";
        if (dictionary2.TryGetValue(keyValuePair.Key, out num2))
        {
          num2 = Mathf.Abs(num1 * num2);
          str5 = "(+" + num2.ToString() + ")";
        }
        str4 = str4 + "\n" + keyValuePair.Key.Name + ": " + (num1 + num2).ToString() + str5;
      }
    }
    ((TMP_Text) this.productFlavourText).text = str4;
  }

  private void SetEffects(BuildingDef def)
  {
    if (((TMP_Text) this.productDescriptionText).text != null)
      ((TMP_Text) this.productDescriptionText).text = string.Format("{0}", (object) def.Effect);
    List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(def.BuildingComplete);
    List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
    if (requirementDescriptors.Count > 0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.OPERATIONREQUIREMENTS, (string) UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONREQUIREMENTS, (Descriptor.DescriptorType) 1);
      requirementDescriptors.Insert(0, descriptor);
      ((Component) this.ProductRequirementsPane).gameObject.SetActive(true);
    }
    else
      ((Component) this.ProductRequirementsPane).gameObject.SetActive(false);
    this.ProductRequirementsPane.SetDescriptors((IList<Descriptor>) requirementDescriptors);
    List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(allDescriptors);
    if (effectDescriptors.Count > 0)
    {
      Descriptor descriptor = new Descriptor();
      ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.OPERATIONEFFECTS, (string) UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONEFFECTS, (Descriptor.DescriptorType) 1);
      effectDescriptors.Insert(0, descriptor);
      ((Component) this.ProductEffectsPane).gameObject.SetActive(true);
    }
    else
      ((Component) this.ProductEffectsPane).gameObject.SetActive(false);
    this.ProductEffectsPane.SetDescriptors((IList<Descriptor>) effectDescriptors);
  }

  public void ClearLabels()
  {
    List<string> stringList = new List<string>((IEnumerable<string>) this.descLabels.Keys);
    if (stringList.Count <= 0)
      return;
    foreach (string key in stringList)
    {
      GameObject descLabel = this.descLabels[key];
      if (Object.op_Inequality((Object) descLabel, (Object) null))
        Object.Destroy((Object) descLabel);
      this.descLabels.Remove(key);
    }
  }

  public void SetMaterials(BuildingDef def)
  {
    ((Component) this.materialSelectionPanel).gameObject.SetActive(true);
    Recipe craftRecipe = def.CraftRecipe;
    this.materialSelectionPanel.ClearSelectActions();
    this.materialSelectionPanel.ConfigureScreen(craftRecipe, new MaterialSelectionPanel.GetBuildableStateDelegate(PlanScreen.Instance.IsDefBuildable), new MaterialSelectionPanel.GetBuildableTooltipDelegate(PlanScreen.Instance.GetTooltipForBuildable));
    this.materialSelectionPanel.ToggleShowDescriptorPanels(false);
    this.materialSelectionPanel.AddSelectAction(new MaterialSelector.SelectMaterialActions(this.RefreshScreen));
    this.materialSelectionPanel.AddSelectAction(new MaterialSelector.SelectMaterialActions(this.onMenuMaterialChanged));
    this.materialSelectionPanel.AutoSelectAvailableMaterial();
    this.ActivateAppropriateTool(def);
  }

  private void OnFacadeSelectionChanged()
  {
    if (Object.op_Equality((Object) this.currentDef, (Object) null))
      return;
    this.ActivateAppropriateTool(this.currentDef);
  }

  private void onMenuMaterialChanged()
  {
    if (Object.op_Equality((Object) this.currentDef, (Object) null))
      return;
    this.ActivateAppropriateTool(this.currentDef);
    this.SetDescription(this.currentDef);
  }

  private void ActivateAppropriateTool(BuildingDef def)
  {
    Debug.Assert(Object.op_Inequality((Object) def, (Object) null), (object) "def was null");
    if ((Object.op_Inequality((Object) PlanScreen.Instance, (Object) null) ? (PlanScreen.Instance.IsDefBuildable(def) ? 1 : 0) : (Object.op_Inequality((Object) BuildMenu.Instance, (Object) null) ? (BuildMenu.Instance.BuildableState(def) == PlanScreen.RequirementsState.Complete ? 1 : 0) : 0)) != 0 && this.materialSelectionPanel.AllSelectorsSelected() && this.facadeSelectionPanel.SelectedFacade != null)
    {
      Util.Signal(this.onElementsFullySelected);
    }
    else
    {
      if (MaterialSelector.AllowInsufficientMaterialBuild() || DebugHandler.InstantBuildMode)
        return;
      if (Object.op_Equality((Object) PlayerController.Instance.ActiveTool, (Object) BuildTool.Instance))
        BuildTool.Instance.Deactivate();
      PrebuildTool.Instance.Activate(def, PlanScreen.Instance.GetTooltipForBuildable(def));
    }
  }

  public static bool MaterialsMet(Recipe recipe)
  {
    if (recipe == null)
    {
      Debug.LogError((object) "Trying to verify the materials on a null recipe!");
      return false;
    }
    if (recipe.Ingredients == null || recipe.Ingredients.Count == 0)
    {
      Debug.LogError((object) "Trying to verify the materials on a recipe with no MaterialCategoryTags!");
      return false;
    }
    bool flag = true;
    for (int index = 0; index < recipe.Ingredients.Count; ++index)
    {
      if ((double) MaterialSelectionPanel.Filter(recipe.Ingredients[index].tag).kgAvailable < (double) recipe.Ingredients[index].amount)
      {
        flag = false;
        break;
      }
    }
    return flag;
  }

  public void Close()
  {
    if (this.configuring)
      return;
    this.ClearProduct();
    this.Show(false);
  }
}
