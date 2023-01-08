// Decompiled with JetBrains decompiler
// Type: AsteroidDescriptorPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using ProcGen;
using STRINGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/AsteroidDescriptorPanel")]
public class AsteroidDescriptorPanel : KMonoBehaviour
{
  [Header("Destination Details")]
  [SerializeField]
  private GameObject customLabelPrefab;
  [SerializeField]
  private GameObject prefabTraitWidget;
  [SerializeField]
  private GameObject prefabTraitCategoryWidget;
  [SerializeField]
  private GameObject prefabParameterWidget;
  [SerializeField]
  private GameObject startingAsteroidRowContainer;
  [SerializeField]
  private GameObject nearbyAsteroidRowContainer;
  [SerializeField]
  private GameObject distantAsteroidRowContainer;
  [SerializeField]
  private LocText clusterNameLabel;
  [SerializeField]
  private LocText clusterDifficultyLabel;
  [SerializeField]
  public LocText headerLabel;
  [SerializeField]
  public MultiToggle clusterDetailsButton;
  [SerializeField]
  public GameObject storyTraitHeader;
  private List<GameObject> labels = new List<GameObject>();
  [Header("Selected Asteroid Details")]
  [SerializeField]
  private GameObject SpacedOutContentContainer;
  public Image selectedAsteroidIcon;
  public LocText selectedAsteroidLabel;
  public LocText selectedAsteroidDescription;
  [SerializeField]
  private GameObject prefabAsteroidLine;
  private Dictionary<World, GameObject> asteroidLines = new Dictionary<World, GameObject>();
  private List<GameObject> traitWidgets = new List<GameObject>();
  private List<GameObject> traitCategoryWidgets = new List<GameObject>();
  private List<GameObject> parameterWidgets = new List<GameObject>();

  public bool HasDescriptors() => this.labels.Count > 0;

  public void EnableClusterDetails(bool setActive)
  {
    ((Component) this.clusterNameLabel).gameObject.SetActive(setActive);
    ((Component) this.clusterDifficultyLabel).gameObject.SetActive(setActive);
  }

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  public void SetClusterDetailLabels(ColonyDestinationAsteroidBeltData cluster)
  {
    StringEntry stringEntry;
    Strings.TryGet(cluster.properName, ref stringEntry);
    ((TMP_Text) this.clusterNameLabel).SetText(stringEntry == null ? "" : string.Format((string) WORLDS.SURVIVAL_CHANCE.CLUSTERNAME, (object) stringEntry.String));
    int index = Mathf.Clamp(cluster.difficulty, 0, ColonyDestinationAsteroidBeltData.survivalOptions.Count - 1);
    Tuple<string, string, string> survivalOption = ColonyDestinationAsteroidBeltData.survivalOptions[index];
    ((TMP_Text) this.clusterDifficultyLabel).SetText(string.Format((string) WORLDS.SURVIVAL_CHANCE.TITLE, (object) survivalOption.first, (object) survivalOption.third).Trim('\n'));
  }

  public void SetParameterDescriptors(IList<AsteroidDescriptor> descriptors)
  {
    for (int index = 0; index < this.parameterWidgets.Count; ++index)
      Object.Destroy((Object) this.parameterWidgets[index]);
    this.parameterWidgets.Clear();
    for (int index = 0; index < descriptors.Count; ++index)
    {
      GameObject gameObject = Util.KInstantiateUI(this.prefabParameterWidget, ((Component) this).gameObject, true);
      ((TMP_Text) gameObject.GetComponent<LocText>()).SetText(descriptors[index].text);
      ToolTip component = gameObject.GetComponent<ToolTip>();
      if (!string.IsNullOrEmpty(descriptors[index].tooltip))
        component.SetSimpleTooltip(descriptors[index].tooltip);
      this.parameterWidgets.Add(gameObject);
    }
  }

  private void ClearTraitDescriptors()
  {
    for (int index = 0; index < this.traitWidgets.Count; ++index)
      Object.Destroy((Object) this.traitWidgets[index]);
    this.traitWidgets.Clear();
    for (int index = 0; index < this.traitCategoryWidgets.Count; ++index)
      Object.Destroy((Object) this.traitCategoryWidgets[index]);
    this.traitCategoryWidgets.Clear();
  }

  public void SetTraitDescriptors(
    IList<AsteroidDescriptor> descriptors,
    List<string> stories,
    bool includeDescriptions = true)
  {
    foreach (string storey in stories)
    {
      WorldTrait storyTrait = Db.Get().Stories.Get(storey).StoryTrait;
      string tooltip = StringEntry.op_Implicit(DlcManager.IsPureVanilla() ? Strings.Get(storyTrait.description + "_SHORT") : Strings.Get(storyTrait.description));
      descriptors.Add(new AsteroidDescriptor(Strings.Get(storyTrait.name).String, tooltip, Color.white, associatedIcon: storyTrait.icon));
    }
    this.SetTraitDescriptors(new List<IList<AsteroidDescriptor>>()
    {
      descriptors
    }, includeDescriptions);
    if (stories.Count != 0)
    {
      ((Transform) Util.rectTransform(this.storyTraitHeader)).SetSiblingIndex(((Transform) Util.rectTransform(this.storyTraitHeader)).parent.childCount - stories.Count - 1);
      this.storyTraitHeader.SetActive(true);
    }
    else
      this.storyTraitHeader.SetActive(false);
  }

  public void SetTraitDescriptors(IList<AsteroidDescriptor> descriptors, bool includeDescriptions = true) => this.SetTraitDescriptors(new List<IList<AsteroidDescriptor>>()
  {
    descriptors
  }, includeDescriptions);

  public void SetTraitDescriptors(
    List<IList<AsteroidDescriptor>> descriptorSets,
    bool includeDescriptions = true,
    List<Tuple<string, Sprite>> headerData = null)
  {
    this.ClearTraitDescriptors();
    for (int index1 = 0; index1 < descriptorSets.Count; ++index1)
    {
      IList<AsteroidDescriptor> descriptorSet = descriptorSets[index1];
      GameObject gameObject1 = ((Component) this).gameObject;
      if (descriptorSets.Count > 1)
      {
        Debug.Assert(headerData != null, (object) "Asteroid Header data is null - traits wont have their world as contex in the selection UI");
        GameObject gameObject2 = Util.KInstantiate(this.prefabTraitCategoryWidget, ((Component) this).gameObject, (string) null);
        HierarchyReferences component = gameObject2.GetComponent<HierarchyReferences>();
        gameObject2.transform.localScale = Vector3.one;
        StringEntry stringEntry;
        Strings.TryGet(headerData[index1].first, ref stringEntry);
        ((TMP_Text) component.GetReference<LocText>("NameLabel")).SetText(stringEntry.String);
        component.GetReference<Image>("Icon").sprite = headerData[index1].second;
        gameObject2.SetActive(true);
        gameObject1 = ((Component) component.GetReference<RectTransform>("Contents")).gameObject;
        this.traitCategoryWidgets.Add(gameObject2);
      }
      for (int index2 = 0; index2 < descriptorSet.Count; ++index2)
      {
        GameObject gameObject3 = Util.KInstantiate(this.prefabTraitWidget, gameObject1, (string) null);
        HierarchyReferences component = gameObject3.GetComponent<HierarchyReferences>();
        gameObject3.SetActive(true);
        ((TMP_Text) component.GetReference<LocText>("NameLabel")).SetText("<b>" + descriptorSet[index2].text + "</b>");
        Image reference1 = component.GetReference<Image>("Icon");
        ((Graphic) reference1).color = descriptorSet[index2].associatedColor;
        if (descriptorSet[index2].associatedIcon != null)
        {
          Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(descriptorSet[index2].associatedIcon));
          if (Object.op_Inequality((Object) sprite, (Object) null))
            reference1.sprite = sprite;
        }
        if (Object.op_Inequality((Object) gameObject3.GetComponent<ToolTip>(), (Object) null))
          gameObject3.GetComponent<ToolTip>().SetSimpleTooltip(descriptorSet[index2].tooltip);
        LocText reference2 = component.GetReference<LocText>("DescLabel");
        if (includeDescriptions && !string.IsNullOrEmpty(descriptorSet[index2].tooltip))
          ((TMP_Text) reference2).SetText(descriptorSet[index2].tooltip);
        else
          ((Component) reference2).gameObject.SetActive(false);
        gameObject3.transform.localScale = new Vector3(1f, 1f, 1f);
        gameObject3.SetActive(true);
        this.traitWidgets.Add(gameObject3);
      }
    }
  }

  public void EnableClusterLocationLabels(bool enable)
  {
    ((Component) this.startingAsteroidRowContainer.transform.parent).gameObject.SetActive(enable);
    ((Component) this.nearbyAsteroidRowContainer.transform.parent).gameObject.SetActive(enable);
    ((Component) this.distantAsteroidRowContainer.transform.parent).gameObject.SetActive(enable);
  }

  public void RefreshAsteroidLines(
    ColonyDestinationAsteroidBeltData cluster,
    AsteroidDescriptorPanel selectedAsteroidDetailsPanel,
    List<string> storyTraits)
  {
    foreach (KeyValuePair<World, GameObject> asteroidLine in this.asteroidLines)
    {
      if (!Util.IsNullOrDestroyed((object) asteroidLine.Value))
        Object.Destroy((Object) asteroidLine.Value);
    }
    this.asteroidLines.Clear();
    this.SpawnAsteroidLine(cluster.GetStartWorld, this.startingAsteroidRowContainer, cluster);
    for (int index1 = 0; index1 < cluster.worlds.Count; ++index1)
    {
      World world = cluster.worlds[index1];
      WorldPlacement worldPlacement = (WorldPlacement) null;
      for (int index2 = 0; index2 < cluster.Layout.worldPlacements.Count; ++index2)
      {
        if (cluster.Layout.worldPlacements[index2].world == world.filePath)
        {
          worldPlacement = cluster.Layout.worldPlacements[index2];
          break;
        }
      }
      this.SpawnAsteroidLine(world, worldPlacement.locationType == 2 ? this.nearbyAsteroidRowContainer : this.distantAsteroidRowContainer, cluster);
    }
    foreach (KeyValuePair<World, GameObject> asteroidLine in this.asteroidLines)
    {
      KeyValuePair<World, GameObject> line = asteroidLine;
      line.Value.GetComponent<MultiToggle>().onClick += (System.Action) (() => this.SelectAsteroidInCluster(line.Key, cluster, selectedAsteroidDetailsPanel));
    }
    this.SelectWholeClusterDetails(cluster, selectedAsteroidDetailsPanel, storyTraits);
  }

  private void SelectAsteroidInCluster(
    World asteroid,
    ColonyDestinationAsteroidBeltData cluster,
    AsteroidDescriptorPanel selectedAsteroidDetailsPanel)
  {
    selectedAsteroidDetailsPanel.SpacedOutContentContainer.SetActive(true);
    ((Component) this.clusterDetailsButton).GetComponent<MultiToggle>().ChangeState(0);
    foreach (KeyValuePair<World, GameObject> asteroidLine in this.asteroidLines)
    {
      asteroidLine.Value.GetComponent<MultiToggle>().ChangeState(asteroidLine.Key == asteroid ? 1 : 0);
      if (asteroidLine.Key == asteroid)
        this.SetSelectedAsteroid(asteroidLine.Key, selectedAsteroidDetailsPanel, cluster.GenerateTraitDescriptors(asteroidLine.Key));
    }
  }

  public void SelectWholeClusterDetails(
    ColonyDestinationAsteroidBeltData cluster,
    AsteroidDescriptorPanel selectedAsteroidDetailsPanel,
    List<string> stories)
  {
    selectedAsteroidDetailsPanel.SpacedOutContentContainer.SetActive(false);
    foreach (KeyValuePair<World, GameObject> asteroidLine in this.asteroidLines)
      asteroidLine.Value.GetComponent<MultiToggle>().ChangeState(0);
    this.SetSelectedCluster(cluster, selectedAsteroidDetailsPanel, stories);
    ((Component) this.clusterDetailsButton).GetComponent<MultiToggle>().ChangeState(1);
  }

  private void SpawnAsteroidLine(
    World asteroid,
    GameObject parentContainer,
    ColonyDestinationAsteroidBeltData cluster)
  {
    if (this.asteroidLines.ContainsKey(asteroid))
      return;
    GameObject gameObject = Util.KInstantiateUI(this.prefabAsteroidLine, parentContainer.gameObject, true);
    HierarchyReferences component1 = gameObject.GetComponent<HierarchyReferences>();
    Image reference1 = component1.GetReference<Image>("Icon");
    LocText reference2 = component1.GetReference<LocText>("Label");
    RectTransform reference3 = component1.GetReference<RectTransform>("TraitsRow");
    LocText reference4 = component1.GetReference<LocText>("TraitLabel");
    ToolTip component2 = gameObject.GetComponent<ToolTip>();
    Sprite uiSprite = ColonyDestinationAsteroidBeltData.GetUISprite(asteroid.asteroidIcon);
    reference1.sprite = uiSprite;
    StringEntry stringEntry1;
    Strings.TryGet(asteroid.name, ref stringEntry1);
    ((TMP_Text) reference2).SetText(stringEntry1.String);
    List<WorldTrait> worldTraits = cluster.GetWorldTraits(asteroid);
    ((Component) reference4).gameObject.SetActive(worldTraits.Count == 0);
    ((TMP_Text) reference4).SetText((string) STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.NO_TRAITS);
    RectTransform reference5 = component1.GetReference<RectTransform>("TraitIconPrefab");
    foreach (WorldTrait worldTrait in worldTraits)
    {
      Image component3 = Util.KInstantiateUI(((Component) reference5).gameObject, ((Component) reference3).gameObject, true).GetComponent<Image>();
      Sprite sprite = Assets.GetSprite(HashedString.op_Implicit(worldTrait.filePath.Substring(worldTrait.filePath.LastIndexOf("/") + 1)));
      if (Object.op_Inequality((Object) sprite, (Object) null))
        component3.sprite = sprite;
      ((Graphic) component3).color = Util.ColorFromHex(worldTrait.colorHex);
    }
    string str = "";
    if (worldTraits.Count > 0)
    {
      for (int index = 0; index < worldTraits.Count; ++index)
      {
        StringEntry stringEntry2;
        Strings.TryGet(worldTraits[index].name, ref stringEntry2);
        StringEntry stringEntry3;
        Strings.TryGet(worldTraits[index].description, ref stringEntry3);
        str = str + "<color=#" + worldTraits[index].colorHex + ">" + stringEntry2.String + "</color>\n" + stringEntry3.String;
        if (index != worldTraits.Count - 1)
          str += "\n\n";
      }
    }
    else
      str = (string) STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.NO_TRAITS;
    component2.SetSimpleTooltip(str);
    this.asteroidLines.Add(asteroid, gameObject);
  }

  private void SetSelectedAsteroid(
    World asteroid,
    AsteroidDescriptorPanel detailPanel,
    List<AsteroidDescriptor> traitDescriptors)
  {
    detailPanel.SetTraitDescriptors((IList<AsteroidDescriptor>) traitDescriptors);
    detailPanel.selectedAsteroidIcon.sprite = ColonyDestinationAsteroidBeltData.GetUISprite(asteroid.asteroidIcon);
    ((Component) detailPanel.selectedAsteroidIcon).gameObject.SetActive(true);
    StringEntry stringEntry1;
    Strings.TryGet(asteroid.name, ref stringEntry1);
    ((TMP_Text) detailPanel.selectedAsteroidLabel).SetText(stringEntry1.String);
    StringEntry stringEntry2;
    Strings.TryGet(asteroid.description, ref stringEntry2);
    ((TMP_Text) detailPanel.selectedAsteroidDescription).SetText(stringEntry2.String);
  }

  private void SetSelectedCluster(
    ColonyDestinationAsteroidBeltData cluster,
    AsteroidDescriptorPanel detailPanel,
    List<string> stories)
  {
    List<IList<AsteroidDescriptor>> descriptorSets = new List<IList<AsteroidDescriptor>>();
    List<Tuple<string, Sprite>> headerData = new List<Tuple<string, Sprite>>();
    List<AsteroidDescriptor> traitDescriptors1 = cluster.GenerateTraitDescriptors(cluster.GetStartWorld, false);
    if (traitDescriptors1.Count != 0)
    {
      headerData.Add(new Tuple<string, Sprite>(cluster.GetStartWorld.name, ColonyDestinationAsteroidBeltData.GetUISprite(cluster.GetStartWorld.asteroidIcon)));
      descriptorSets.Add((IList<AsteroidDescriptor>) traitDescriptors1);
    }
    foreach (World world in cluster.worlds)
    {
      List<AsteroidDescriptor> traitDescriptors2 = cluster.GenerateTraitDescriptors(world, false);
      if (traitDescriptors2.Count != 0)
      {
        headerData.Add(new Tuple<string, Sprite>(world.name, ColonyDestinationAsteroidBeltData.GetUISprite(world.asteroidIcon)));
        descriptorSets.Add((IList<AsteroidDescriptor>) traitDescriptors2);
      }
    }
    headerData.Add(new Tuple<string, Sprite>("STRINGS.UI.FRONTEND.COLONYDESTINATIONSCREEN.STORY_TRAITS_HEADER", Assets.GetSprite(HashedString.op_Implicit("codexIconStoryTraits"))));
    List<AsteroidDescriptor> asteroidDescriptorList = new List<AsteroidDescriptor>();
    foreach (string storey in stories)
    {
      Story story = Db.Get().Stories.Get(storey);
      string icon = story.StoryTrait.icon;
      AsteroidDescriptor asteroidDescriptor = new AsteroidDescriptor(Strings.Get(story.StoryTrait.name).String, Strings.Get(story.StoryTrait.description).String, Color.white, associatedIcon: icon);
      asteroidDescriptorList.Add(asteroidDescriptor);
    }
    descriptorSets.Add((IList<AsteroidDescriptor>) asteroidDescriptorList);
    detailPanel.SetTraitDescriptors(descriptorSets, false, headerData);
    ((Component) detailPanel.selectedAsteroidIcon).gameObject.SetActive(false);
    StringEntry stringEntry;
    Strings.TryGet(cluster.properName, ref stringEntry);
    ((TMP_Text) detailPanel.selectedAsteroidLabel).SetText(stringEntry.String);
    ((TMP_Text) detailPanel.selectedAsteroidDescription).SetText("");
  }
}
