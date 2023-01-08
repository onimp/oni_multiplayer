// Decompiled with JetBrains decompiler
// Type: PlanterSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlanterSideScreen : ReceptacleSideScreen
{
  public DescriptorPanel RequirementsDescriptorPanel;
  public DescriptorPanel HarvestDescriptorPanel;
  public DescriptorPanel EffectsDescriptorPanel;
  public GameObject mutationPanel;
  public GameObject mutationViewport;
  public GameObject mutationContainer;
  public GameObject mutationOption;
  public GameObject blankMutationOption;
  public GameObject selectSpeciesPrompt;
  private bool mutationPanelCollapsed = true;
  public Dictionary<GameObject, Tag> subspeciesToggles = new Dictionary<GameObject, Tag>();
  private List<GameObject> blankMutationObjects = new List<GameObject>();
  private Dictionary<PlantablePlot, Tag> entityPreviousSubSelectionMap = new Dictionary<PlantablePlot, Tag>();
  private Coroutine activeAnimationRoutine;
  private const float EXPAND_DURATION = 0.33f;
  private const float EXPAND_MIN = 24f;
  private const float EXPAND_MAX = 118f;

  private Tag selectedSubspecies
  {
    get
    {
      if (!this.entityPreviousSubSelectionMap.ContainsKey((PlantablePlot) this.targetReceptacle))
        this.entityPreviousSubSelectionMap.Add((PlantablePlot) this.targetReceptacle, Tag.Invalid);
      return this.entityPreviousSubSelectionMap[(PlantablePlot) this.targetReceptacle];
    }
    set
    {
      if (!this.entityPreviousSubSelectionMap.ContainsKey((PlantablePlot) this.targetReceptacle))
        this.entityPreviousSubSelectionMap.Add((PlantablePlot) this.targetReceptacle, Tag.Invalid);
      this.entityPreviousSubSelectionMap[(PlantablePlot) this.targetReceptacle] = value;
      this.selectedDepositObjectAdditionalTag = value;
    }
  }

  private void LoadTargetSubSpeciesRequest()
  {
    PlantablePlot targetReceptacle = (PlantablePlot) this.targetReceptacle;
    Tag tag = Tag.Invalid;
    if (Tag.op_Inequality(targetReceptacle.requestedEntityTag, Tag.Invalid) && Tag.op_Inequality(targetReceptacle.requestedEntityTag, GameTags.Empty))
      tag = targetReceptacle.requestedEntityTag;
    else if (Object.op_Inequality((Object) this.selectedEntityToggle, (Object) null))
      tag = this.depositObjectMap[this.selectedEntityToggle].tag;
    if (!DlcManager.FeaturePlantMutationsEnabled() || !((Tag) ref tag).IsValid)
      return;
    MutantPlant component = Assets.GetPrefab(tag).GetComponent<MutantPlant>();
    if (Object.op_Equality((Object) component, (Object) null))
      this.selectedSubspecies = Tag.Invalid;
    else if (Tag.op_Inequality(targetReceptacle.requestedEntityAdditionalFilterTag, Tag.Invalid) && Tag.op_Inequality(targetReceptacle.requestedEntityAdditionalFilterTag, GameTags.Empty))
    {
      this.selectedSubspecies = targetReceptacle.requestedEntityAdditionalFilterTag;
    }
    else
    {
      if (!Tag.op_Equality(this.selectedSubspecies, Tag.Invalid))
        return;
      PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo;
      if (PlantSubSpeciesCatalog.Instance.GetOriginalSubSpecies(component.SpeciesID, out subSpeciesInfo))
        this.selectedSubspecies = subSpeciesInfo.ID;
      targetReceptacle.requestedEntityAdditionalFilterTag = this.selectedSubspecies;
    }
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<PlantablePlot>(), (Object) null);

  protected override void ToggleClicked(ReceptacleToggle toggle)
  {
    base.ToggleClicked(toggle);
    this.LoadTargetSubSpeciesRequest();
    this.UpdateState((object) null);
  }

  protected void MutationToggleClicked(GameObject toggle)
  {
    this.selectedSubspecies = this.subspeciesToggles[toggle];
    this.UpdateState((object) null);
  }

  protected override void UpdateState(object data)
  {
    base.UpdateState(data);
    this.requestSelectedEntityBtn.onClick += new System.Action(this.RefreshSubspeciesToggles);
    this.RefreshSubspeciesToggles();
  }

  private IEnumerator ExpandMutations()
  {
    LayoutElement le = this.mutationViewport.GetComponent<LayoutElement>();
    float travelPerSecond = 94f / 0.33f;
    while ((double) le.minHeight < 118.0)
    {
      float num = Mathf.Min(le.minHeight + Time.unscaledDeltaTime * travelPerSecond, 118f);
      le.minHeight = num;
      le.preferredHeight = num;
      yield return (object) new WaitForEndOfFrame();
    }
    this.mutationPanelCollapsed = false;
    this.activeAnimationRoutine = (Coroutine) null;
    yield return (object) 0;
  }

  private IEnumerator CollapseMutations()
  {
    LayoutElement le = this.mutationViewport.GetComponent<LayoutElement>();
    float travelPerSecond = -94f / 0.33f;
    while ((double) le.minHeight > 24.0)
    {
      float num = Mathf.Max(le.minHeight + Time.unscaledDeltaTime * travelPerSecond, 24f);
      le.minHeight = num;
      le.preferredHeight = num;
      yield return (object) SequenceUtil.WaitForEndOfFrame;
    }
    this.mutationPanelCollapsed = true;
    this.activeAnimationRoutine = (Coroutine) null;
    yield return (object) SequenceUtil.WaitForNextFrame;
  }

  private void RefreshSubspeciesToggles()
  {
    foreach (KeyValuePair<GameObject, Tag> subspeciesToggle in this.subspeciesToggles)
      Object.Destroy((Object) subspeciesToggle.Key);
    this.subspeciesToggles.Clear();
    if (!PlantSubSpeciesCatalog.Instance.AnyNonOriginalDiscovered)
    {
      this.mutationPanel.SetActive(false);
    }
    else
    {
      this.mutationPanel.SetActive(true);
      foreach (Object blankMutationObject in this.blankMutationObjects)
        Object.Destroy(blankMutationObject);
      this.blankMutationObjects.Clear();
      this.selectSpeciesPrompt.SetActive(false);
      if (((Tag) ref this.selectedDepositObjectTag).IsValid)
      {
        Tag plantId = Assets.GetPrefab(this.selectedDepositObjectTag).GetComponent<PlantableSeed>().PlantID;
        List<PlantSubSpeciesCatalog.SubSpeciesInfo> speciesForSpecies = PlantSubSpeciesCatalog.Instance.GetAllSubSpeciesForSpecies(plantId);
        if (speciesForSpecies != null)
        {
          foreach (PlantSubSpeciesCatalog.SubSpeciesInfo subSpeciesInfo in speciesForSpecies)
          {
            GameObject option = Util.KInstantiateUI(this.mutationOption, this.mutationContainer, true);
            ((TMP_Text) option.GetComponentInChildren<LocText>()).text = subSpeciesInfo.GetNameWithMutations(plantId.ProperName(), PlantSubSpeciesCatalog.Instance.IsSubSpeciesIdentified(subSpeciesInfo.ID), false);
            option.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.MutationToggleClicked(option));
            option.GetComponent<ToolTip>().SetSimpleTooltip(subSpeciesInfo.GetMutationsTooltip());
            this.subspeciesToggles.Add(option, subSpeciesInfo.ID);
          }
          for (int count = speciesForSpecies.Count; count < 5; ++count)
            this.blankMutationObjects.Add(Util.KInstantiateUI(this.blankMutationOption, this.mutationContainer, true));
          Tag selectedSubspecies = this.selectedSubspecies;
          if (!((Tag) ref selectedSubspecies).IsValid || !this.subspeciesToggles.ContainsValue(this.selectedSubspecies))
            this.selectedSubspecies = speciesForSpecies[0].ID;
        }
      }
      else
        this.selectSpeciesPrompt.SetActive(true);
      ICollection<Pickupable> pickupables1 = (ICollection<Pickupable>) new List<Pickupable>();
      bool flag1 = this.CheckReceptacleOccupied();
      bool flag2 = this.targetReceptacle.GetActiveRequest != null;
      WorldContainer myWorld = this.targetReceptacle.GetMyWorld();
      ICollection<Pickupable> pickupables2 = myWorld.worldInventory.GetPickupables(this.selectedDepositObjectTag, myWorld.IsModuleInterior);
      foreach (KeyValuePair<GameObject, Tag> subspeciesToggle in this.subspeciesToggles)
      {
        float num = 0.0f;
        bool flag3 = PlantSubSpeciesCatalog.Instance.IsSubSpeciesIdentified(subspeciesToggle.Value);
        if (pickupables2 != null)
        {
          foreach (Pickupable cmp in (IEnumerable<Pickupable>) pickupables2)
          {
            if (((Component) cmp).HasTag(subspeciesToggle.Value))
              num += ((Component) cmp).GetComponent<PrimaryElement>().Units;
          }
        }
        if ((double) num > 0.0 & flag3)
          subspeciesToggle.Key.GetComponent<MultiToggle>().ChangeState(Tag.op_Equality(subspeciesToggle.Value, this.selectedSubspecies) ? 1 : 0);
        else
          subspeciesToggle.Key.GetComponent<MultiToggle>().ChangeState(Tag.op_Equality(subspeciesToggle.Value, this.selectedSubspecies) ? 3 : 2);
        LocText componentInChildren = subspeciesToggle.Key.GetComponentInChildren<LocText>();
        ((TMP_Text) componentInChildren).text = ((TMP_Text) componentInChildren).text + string.Format(" ({0})", (object) num);
        if (flag1 | flag2)
        {
          if (Tag.op_Equality(subspeciesToggle.Value, this.selectedSubspecies))
          {
            subspeciesToggle.Key.SetActive(true);
            subspeciesToggle.Key.GetComponent<MultiToggle>().ChangeState(1);
          }
          else
            subspeciesToggle.Key.SetActive(false);
        }
        else
          subspeciesToggle.Key.SetActive(Object.op_Inequality((Object) this.selectedEntityToggle, (Object) null));
      }
      bool flag4 = !flag1 && !flag2 && Object.op_Inequality((Object) this.selectedEntityToggle, (Object) null) && this.subspeciesToggles.Count >= 1;
      if (flag4 && this.mutationPanelCollapsed)
      {
        if (this.activeAnimationRoutine != null)
          ((MonoBehaviour) this).StopCoroutine(this.activeAnimationRoutine);
        this.activeAnimationRoutine = ((MonoBehaviour) this).StartCoroutine(this.ExpandMutations());
      }
      else
      {
        if (flag4 || this.mutationPanelCollapsed)
          return;
        if (this.activeAnimationRoutine != null)
          ((MonoBehaviour) this).StopCoroutine(this.activeAnimationRoutine);
        this.activeAnimationRoutine = ((MonoBehaviour) this).StartCoroutine(this.CollapseMutations());
      }
    }
  }

  protected override Sprite GetEntityIcon(Tag prefabTag)
  {
    PlantableSeed component = Assets.GetPrefab(prefabTag).GetComponent<PlantableSeed>();
    return Object.op_Inequality((Object) component, (Object) null) ? base.GetEntityIcon(new Tag(component.PlantID)) : base.GetEntityIcon(prefabTag);
  }

  protected override void SetResultDescriptions(GameObject seed_or_plant)
  {
    string str = "";
    GameObject go = seed_or_plant;
    PlantableSeed component1 = seed_or_plant.GetComponent<PlantableSeed>();
    List<Descriptor> descriptorList = new List<Descriptor>();
    bool flag = true;
    if (Object.op_Inequality((Object) seed_or_plant.GetComponent<MutantPlant>(), (Object) null) && Tag.op_Inequality(this.selectedDepositObjectAdditionalTag, Tag.Invalid))
      flag = PlantSubSpeciesCatalog.Instance.IsSubSpeciesIdentified(this.selectedDepositObjectAdditionalTag);
    if (!flag)
      str = (string) CREATURES.PLANT_MUTATIONS.UNIDENTIFIED + "\n\n" + (string) CREATURES.PLANT_MUTATIONS.UNIDENTIFIED_DESC;
    else if (Object.op_Inequality((Object) component1, (Object) null))
    {
      descriptorList = component1.GetDescriptors(((Component) component1).gameObject);
      if (Object.op_Inequality((Object) this.targetReceptacle.rotatable, (Object) null) && this.targetReceptacle.Direction != component1.direction)
      {
        if (component1.direction == SingleEntityReceptacle.ReceptacleDirection.Top)
          str += (string) STRINGS.UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_FLOOR;
        else if (component1.direction == SingleEntityReceptacle.ReceptacleDirection.Side)
          str += (string) STRINGS.UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_WALL;
        else if (component1.direction == SingleEntityReceptacle.ReceptacleDirection.Bottom)
          str += (string) STRINGS.UI.UISIDESCREENS.PLANTERSIDESCREEN.ROTATION_NEED_CEILING;
        str += "\n\n";
      }
      go = Assets.GetPrefab(component1.PlantID);
      MutantPlant component2 = go.GetComponent<MutantPlant>();
      if (Object.op_Inequality((Object) component2, (Object) null) && ((Tag) ref this.selectedDepositObjectAdditionalTag).IsValid)
      {
        PlantSubSpeciesCatalog.SubSpeciesInfo subSpecies = PlantSubSpeciesCatalog.Instance.GetSubSpecies(component1.PlantID, this.selectedDepositObjectAdditionalTag);
        component2.DummySetSubspecies(subSpecies.mutationIDs);
      }
      if (!string.IsNullOrEmpty(component1.domesticatedDescription))
        str += component1.domesticatedDescription;
    }
    else
    {
      InfoDescription component3 = go.GetComponent<InfoDescription>();
      if (Object.op_Implicit((Object) component3))
        str += component3.description;
    }
    ((TMP_Text) this.descriptionLabel).SetText(str);
    List<Descriptor> cycleDescriptors = GameUtil.GetPlantLifeCycleDescriptors(go);
    if (cycleDescriptors.Count > 0 & flag)
    {
      this.HarvestDescriptorPanel.SetDescriptors((IList<Descriptor>) cycleDescriptors);
      ((Component) this.HarvestDescriptorPanel).gameObject.SetActive(true);
    }
    else
      ((Component) this.HarvestDescriptorPanel).gameObject.SetActive(false);
    List<Descriptor> requirementDescriptors = GameUtil.GetPlantRequirementDescriptors(go);
    if (descriptorList.Count > 0)
    {
      GameUtil.IndentListOfDescriptors(descriptorList);
      requirementDescriptors.InsertRange(requirementDescriptors.Count, (IEnumerable<Descriptor>) descriptorList);
    }
    if (requirementDescriptors.Count > 0 && flag)
    {
      this.RequirementsDescriptorPanel.SetDescriptors((IList<Descriptor>) requirementDescriptors);
      ((Component) this.RequirementsDescriptorPanel).gameObject.SetActive(true);
    }
    else
      ((Component) this.RequirementsDescriptorPanel).gameObject.SetActive(false);
    List<Descriptor> effectDescriptors = GameUtil.GetPlantEffectDescriptors(go);
    if (effectDescriptors.Count > 0 && flag)
    {
      this.EffectsDescriptorPanel.SetDescriptors((IList<Descriptor>) effectDescriptors);
      ((Component) this.EffectsDescriptorPanel).gameObject.SetActive(true);
    }
    else
      ((Component) this.EffectsDescriptorPanel).gameObject.SetActive(false);
  }

  protected override bool AdditionalCanDepositTest()
  {
    bool flag = false;
    if (((Tag) ref this.selectedDepositObjectTag).IsValid)
      flag = !DlcManager.FeaturePlantMutationsEnabled() ? ((Tag) ref this.selectedDepositObjectTag).IsValid : PlantSubSpeciesCatalog.Instance.IsValidPlantableSeed(this.selectedDepositObjectTag, this.selectedDepositObjectAdditionalTag);
    PlantablePlot targetReceptacle = this.targetReceptacle as PlantablePlot;
    WorldContainer myWorld = this.targetReceptacle.GetMyWorld();
    return flag && targetReceptacle.ValidPlant && myWorld.worldInventory.GetCountWithAdditionalTag(this.selectedDepositObjectTag, this.selectedDepositObjectAdditionalTag, myWorld.IsModuleInterior) > 0;
  }

  public override void SetTarget(GameObject target)
  {
    this.selectedDepositObjectTag = Tag.Invalid;
    this.selectedDepositObjectAdditionalTag = Tag.Invalid;
    base.SetTarget(target);
    this.LoadTargetSubSpeciesRequest();
    this.RefreshSubspeciesToggles();
  }

  protected override void RestoreSelectionFromOccupant()
  {
    base.RestoreSelectionFromOccupant();
    PlantablePlot targetReceptacle = (PlantablePlot) this.targetReceptacle;
    Tag tag1 = Tag.Invalid;
    Tag tag2 = Tag.Invalid;
    bool flag = false;
    if (Object.op_Inequality((Object) targetReceptacle.Occupant, (Object) null))
    {
      tag1 = Tag.op_Implicit(targetReceptacle.Occupant.GetComponent<SeedProducer>().seedInfo.seedId);
      MutantPlant component = targetReceptacle.Occupant.GetComponent<MutantPlant>();
      if (Object.op_Inequality((Object) component, (Object) null))
        tag2 = component.SubSpeciesID;
    }
    else if (targetReceptacle.GetActiveRequest != null)
    {
      tag1 = targetReceptacle.requestedEntityTag;
      tag2 = targetReceptacle.requestedEntityAdditionalFilterTag;
      this.selectedDepositObjectTag = tag1;
      this.selectedDepositObjectAdditionalTag = tag2;
      flag = true;
    }
    if (!Tag.op_Inequality(tag1, Tag.Invalid))
      return;
    if (!this.entityPreviousSelectionMap.ContainsKey((SingleEntityReceptacle) targetReceptacle) | flag)
    {
      int num = 0;
      foreach (KeyValuePair<ReceptacleToggle, ReceptacleSideScreen.SelectableEntity> depositObject in this.depositObjectMap)
      {
        if (Tag.op_Equality(depositObject.Value.tag, tag1))
          num = this.entityToggles.IndexOf(depositObject.Key);
      }
      if (!this.entityPreviousSelectionMap.ContainsKey((SingleEntityReceptacle) targetReceptacle))
        this.entityPreviousSelectionMap.Add((SingleEntityReceptacle) targetReceptacle, -1);
      this.entityPreviousSelectionMap[(SingleEntityReceptacle) targetReceptacle] = num;
    }
    if (!this.entityPreviousSubSelectionMap.ContainsKey(targetReceptacle))
      this.entityPreviousSubSelectionMap.Add(targetReceptacle, Tag.Invalid);
    if (!(Tag.op_Equality(this.entityPreviousSubSelectionMap[targetReceptacle], Tag.Invalid) | flag))
      return;
    this.entityPreviousSubSelectionMap[targetReceptacle] = tag2;
  }
}
