// Decompiled with JetBrains decompiler
// Type: SpacePOISimpleInfoPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpacePOISimpleInfoPanel : SimpleInfoPanel
{
  private Dictionary<Tag, GameObject> elementRows = new Dictionary<Tag, GameObject>();
  private Dictionary<Clustercraft, GameObject> rocketRows = new Dictionary<Clustercraft, GameObject>();
  private GameObject massHeader;
  private GameObject rocketsSpacer;
  private GameObject rocketsHeader;
  private GameObject artifactsSpacer;
  private GameObject artifactRow;

  public SpacePOISimpleInfoPanel(SimpleInfoScreen simpleInfoScreen)
    : base(simpleInfoScreen)
  {
  }

  public override void Refresh(
    CollapsibleDetailContentPanel spacePOIPanel,
    GameObject selectedTarget)
  {
    spacePOIPanel.SetTitle((string) STRINGS.UI.CLUSTERMAP.POI.TITLE);
    if (Object.op_Equality((Object) selectedTarget, (Object) null))
    {
      ((Component) spacePOIPanel).gameObject.SetActive(false);
    }
    else
    {
      HarvestablePOIClusterGridEntity cmp = Object.op_Equality((Object) selectedTarget, (Object) null) ? (HarvestablePOIClusterGridEntity) null : selectedTarget.GetComponent<HarvestablePOIClusterGridEntity>();
      Clustercraft component1 = selectedTarget.GetComponent<Clustercraft>();
      ArtifactPOIConfigurator component2 = selectedTarget.GetComponent<ArtifactPOIConfigurator>();
      if (Object.op_Equality((Object) cmp, (Object) null) && Object.op_Equality((Object) component1, (Object) null) && Object.op_Equality((Object) component2, (Object) null))
      {
        ((Component) spacePOIPanel).gameObject.SetActive(false);
      }
      else
      {
        if (Object.op_Equality((Object) cmp, (Object) null) && Object.op_Equality((Object) component2, (Object) null) && Object.op_Inequality((Object) component1, (Object) null))
        {
          RocketModuleCluster rocketModuleCluster = (RocketModuleCluster) null;
          CraftModuleInterface craftModuleInterface = (CraftModuleInterface) null;
          RocketSimpleInfoPanel.GetRocketStuffFromTarget(selectedTarget, ref rocketModuleCluster, ref component1, ref craftModuleInterface);
          if (Object.op_Inequality((Object) component1, (Object) null))
          {
            foreach (ClusterGridEntity clusterGridEntity1 in ClusterGrid.Instance.GetEntitiesOnCell(component1.GetMyWorldLocation()))
            {
              HarvestablePOIClusterGridEntity clusterGridEntity2 = clusterGridEntity1 as HarvestablePOIClusterGridEntity;
              if (Object.op_Inequality((Object) clusterGridEntity2, (Object) null))
              {
                cmp = clusterGridEntity2;
                component2 = ((Component) clusterGridEntity2).GetComponent<ArtifactPOIConfigurator>();
                break;
              }
            }
          }
        }
        bool flag = Object.op_Inequality((Object) cmp, (Object) null) || Object.op_Inequality((Object) component2, (Object) null);
        ((Component) spacePOIPanel).gameObject.SetActive(flag);
        if (!flag)
          return;
        HarvestablePOIStates.Instance harvestable = Object.op_Equality((Object) cmp, (Object) null) ? (HarvestablePOIStates.Instance) null : ((Component) cmp).GetSMI<HarvestablePOIStates.Instance>();
        this.RefreshMassHeader(harvestable, selectedTarget, spacePOIPanel);
        this.RefreshElements(harvestable, selectedTarget, spacePOIPanel);
        this.RefreshArtifacts(component2, selectedTarget, spacePOIPanel);
      }
    }
  }

  private void RefreshMassHeader(
    HarvestablePOIStates.Instance harvestable,
    GameObject selectedTarget,
    CollapsibleDetailContentPanel spacePOIPanel)
  {
    if (Object.op_Equality((Object) this.massHeader, (Object) null))
      this.massHeader = Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, ((Component) spacePOIPanel.Content).gameObject, true);
    this.massHeader.SetActive(harvestable != null);
    if (harvestable == null)
      return;
    HierarchyReferences component = this.massHeader.GetComponent<HierarchyReferences>();
    Sprite sprite = Assets.GetSprite(HashedString.op_Implicit("icon_asteroid_type"));
    if (Object.op_Inequality((Object) sprite, (Object) null))
      component.GetReference<Image>("Icon").sprite = sprite;
    ((TMP_Text) component.GetReference<LocText>("NameLabel")).text = (string) STRINGS.UI.CLUSTERMAP.POI.MASS_REMAINING;
    ((TMP_Text) component.GetReference<LocText>("ValueLabel")).text = GameUtil.GetFormattedMass(harvestable.poiCapacity);
    ((TMP_Text) component.GetReference<LocText>("ValueLabel")).alignment = (TextAlignmentOptions) 4100;
  }

  private void RefreshElements(
    HarvestablePOIStates.Instance harvestable,
    GameObject selectedTarget,
    CollapsibleDetailContentPanel spacePOIPanel)
  {
    foreach (KeyValuePair<Tag, GameObject> elementRow in this.elementRows)
    {
      if (Object.op_Inequality((Object) elementRow.Value, (Object) null))
        elementRow.Value.SetActive(false);
    }
    if (harvestable == null)
      return;
    Dictionary<SimHashes, float> elementsWithWeights = harvestable.configuration.GetElementsWithWeights();
    float num = 0.0f;
    List<KeyValuePair<SimHashes, float>> keyValuePairList = new List<KeyValuePair<SimHashes, float>>();
    foreach (KeyValuePair<SimHashes, float> keyValuePair in elementsWithWeights)
    {
      num += keyValuePair.Value;
      keyValuePairList.Add(keyValuePair);
    }
    keyValuePairList.Sort((Comparison<KeyValuePair<SimHashes, float>>) ((a, b) => b.Value.CompareTo(a.Value)));
    foreach (KeyValuePair<SimHashes, float> keyValuePair in keyValuePairList)
    {
      SimHashes key = keyValuePair.Key;
      Tag tag = key.CreateTag();
      if (!this.elementRows.ContainsKey(key.CreateTag()))
        this.elementRows.Add(tag, Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, ((Component) spacePOIPanel.Content).gameObject, true));
      this.elementRows[tag].SetActive(true);
      HierarchyReferences component = this.elementRows[tag].GetComponent<HierarchyReferences>();
      Tuple<Sprite, Color> uiSprite = Def.GetUISprite((object) tag);
      component.GetReference<Image>("Icon").sprite = uiSprite.first;
      ((Graphic) component.GetReference<Image>("Icon")).color = uiSprite.second;
      ((TMP_Text) component.GetReference<LocText>("NameLabel")).text = ElementLoader.GetElement(tag).name;
      ((TMP_Text) component.GetReference<LocText>("ValueLabel")).text = GameUtil.GetFormattedPercent((float) ((double) keyValuePair.Value / (double) num * 100.0));
      ((TMP_Text) component.GetReference<LocText>("ValueLabel")).alignment = (TextAlignmentOptions) 4100;
    }
  }

  private void RefreshRocketsAtThisLocation(
    HarvestablePOIStates.Instance harvestable,
    GameObject selectedTarget,
    CollapsibleDetailContentPanel spacePOIPanel)
  {
    if (Object.op_Equality((Object) this.rocketsHeader, (Object) null))
    {
      this.rocketsSpacer = Util.KInstantiateUI(this.simpleInfoRoot.spacerRow, ((Component) spacePOIPanel.Content).gameObject, true);
      this.rocketsHeader = Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, ((Component) spacePOIPanel.Content).gameObject, true);
      HierarchyReferences component = this.rocketsHeader.GetComponent<HierarchyReferences>();
      Sprite sprite = Assets.GetSprite(HashedString.op_Implicit("ic_rocket"));
      if (Object.op_Inequality((Object) sprite, (Object) null))
      {
        component.GetReference<Image>("Icon").sprite = sprite;
        ((Graphic) component.GetReference<Image>("Icon")).color = Color.black;
      }
      ((TMP_Text) component.GetReference<LocText>("NameLabel")).text = (string) STRINGS.UI.CLUSTERMAP.POI.ROCKETS_AT_THIS_LOCATION;
      ((TMP_Text) component.GetReference<LocText>("ValueLabel")).text = "";
    }
    ((Transform) Util.rectTransform(this.rocketsSpacer)).SetAsLastSibling();
    ((Transform) Util.rectTransform(this.rocketsHeader)).SetAsLastSibling();
    foreach (KeyValuePair<Clustercraft, GameObject> rocketRow in this.rocketRows)
      rocketRow.Value.SetActive(false);
    bool flag1 = true;
    for (int idx = 0; idx < Components.Clustercrafts.Count; ++idx)
    {
      Clustercraft clustercraft = Components.Clustercrafts[idx];
      if (!this.rocketRows.ContainsKey(clustercraft))
      {
        GameObject gameObject = Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, ((Component) spacePOIPanel.Content).gameObject, true);
        this.rocketRows.Add(clustercraft, gameObject);
      }
      bool flag2 = AxialI.op_Equality(clustercraft.Location, selectedTarget.GetComponent<KMonoBehaviour>().GetMyWorldLocation());
      flag1 = flag1 && !flag2;
      this.rocketRows[clustercraft].SetActive(flag2);
      if (flag2)
      {
        HierarchyReferences component = this.rocketRows[clustercraft].GetComponent<HierarchyReferences>();
        component.GetReference<Image>("Icon").sprite = clustercraft.GetUISprite();
        ((Graphic) component.GetReference<Image>("Icon")).color = Color.grey;
        ((TMP_Text) component.GetReference<LocText>("NameLabel")).text = clustercraft.Name;
        ((TMP_Text) component.GetReference<LocText>("ValueLabel")).text = "";
        ((TMP_Text) component.GetReference<LocText>("ValueLabel")).alignment = (TextAlignmentOptions) 4100;
        ((Transform) Util.rectTransform(this.rocketRows[clustercraft])).SetAsLastSibling();
      }
    }
    this.rocketsHeader.SetActive(!flag1);
    this.rocketsSpacer.SetActive(this.rocketsHeader.activeSelf);
  }

  private void RefreshArtifacts(
    ArtifactPOIConfigurator artifactConfigurator,
    GameObject selectedTarget,
    CollapsibleDetailContentPanel spacePOIPanel)
  {
    if (Object.op_Equality((Object) this.artifactsSpacer, (Object) null))
    {
      this.artifactsSpacer = Util.KInstantiateUI(this.simpleInfoRoot.spacerRow, ((Component) spacePOIPanel.Content).gameObject, true);
      this.artifactRow = Util.KInstantiateUI(this.simpleInfoRoot.iconLabelRow, ((Component) spacePOIPanel.Content).gameObject, true);
    }
    ((Transform) Util.rectTransform(this.artifactsSpacer)).SetAsLastSibling();
    ((Transform) Util.rectTransform(this.artifactRow)).SetAsLastSibling();
    ArtifactPOIStates.Instance smi = ((Component) artifactConfigurator).GetSMI<ArtifactPOIStates.Instance>();
    smi.configuration.GetArtifactID();
    HierarchyReferences component = this.artifactRow.GetComponent<HierarchyReferences>();
    ((TMP_Text) component.GetReference<LocText>("NameLabel")).text = (string) STRINGS.UI.CLUSTERMAP.POI.ARTIFACTS;
    ((TMP_Text) component.GetReference<LocText>("ValueLabel")).alignment = (TextAlignmentOptions) 4100;
    component.GetReference<Image>("Icon").sprite = Assets.GetSprite(HashedString.op_Implicit("ic_artifacts"));
    ((Graphic) component.GetReference<Image>("Icon")).color = Color.black;
    if (smi.CanHarvestArtifact())
      ((TMP_Text) component.GetReference<LocText>("ValueLabel")).text = (string) STRINGS.UI.CLUSTERMAP.POI.ARTIFACTS_AVAILABLE;
    else
      ((TMP_Text) component.GetReference<LocText>("ValueLabel")).text = string.Format((string) STRINGS.UI.CLUSTERMAP.POI.ARTIFACTS_DEPLETED, (object) GameUtil.GetFormattedCycles(smi.RechargeTimeRemaining(), forceCycles: true));
  }
}
