// Decompiled with JetBrains decompiler
// Type: ClusterLocationFilterSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClusterLocationFilterSideScreen : SideScreenContent
{
  private LogicClusterLocationSensor sensor;
  [SerializeField]
  private GameObject rowPrefab;
  [SerializeField]
  private GameObject listContainer;
  [SerializeField]
  private LocText headerLabel;
  private Dictionary<AxialI, GameObject> worldRows = new Dictionary<AxialI, GameObject>();
  private GameObject emptySpaceRow;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<LogicClusterLocationSensor>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.sensor = target.GetComponent<LogicClusterLocationSensor>();
    this.Build();
  }

  private void ClearRows()
  {
    if (Object.op_Inequality((Object) this.emptySpaceRow, (Object) null))
      Util.KDestroyGameObject(this.emptySpaceRow);
    foreach (KeyValuePair<AxialI, GameObject> worldRow in this.worldRows)
      Util.KDestroyGameObject(worldRow.Value);
    this.worldRows.Clear();
  }

  private void Build()
  {
    ((TMP_Text) this.headerLabel).SetText((string) STRINGS.UI.UISIDESCREENS.CLUSTERLOCATIONFILTERSIDESCREEN.HEADER);
    this.ClearRows();
    this.emptySpaceRow = Util.KInstantiateUI(this.rowPrefab, this.listContainer, false);
    this.emptySpaceRow.SetActive(true);
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
    {
      if (!worldContainer.IsModuleInterior)
      {
        GameObject gameObject = Util.KInstantiateUI(this.rowPrefab, this.listContainer, false);
        ((Object) gameObject.gameObject).name = ((Component) worldContainer).GetProperName();
        AxialI myWorldLocation = worldContainer.GetMyWorldLocation();
        Debug.Assert(!this.worldRows.ContainsKey(myWorldLocation), (object) ("Adding two worlds/POI with the same cluster location to ClusterLocationFilterSideScreen UI: " + ((Component) worldContainer).GetProperName()));
        this.worldRows.Add(myWorldLocation, gameObject);
      }
    }
    this.Refresh();
  }

  private void Refresh()
  {
    ((TMP_Text) this.emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")).SetText((string) STRINGS.UI.UISIDESCREENS.CLUSTERLOCATIONFILTERSIDESCREEN.EMPTY_SPACE_ROW);
    this.emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite((object) "hex_soft").first;
    ((Graphic) this.emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<Image>("Icon")).color = Color.black;
    this.emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = (System.Action) (() =>
    {
      this.sensor.SetSpaceEnabled(!this.sensor.ActiveInSpace);
      this.Refresh();
    });
    this.emptySpaceRow.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(this.sensor.ActiveInSpace ? 1 : 0);
    foreach (KeyValuePair<AxialI, GameObject> worldRow in this.worldRows)
    {
      KeyValuePair<AxialI, GameObject> kvp = worldRow;
      ClusterGridEntity cmp = ClusterGrid.Instance.cellContents[kvp.Key][0];
      ((TMP_Text) kvp.Value.GetComponent<HierarchyReferences>().GetReference<LocText>("Label")).SetText(((Component) cmp).GetProperName());
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = Def.GetUISprite((object) cmp).first;
      ((Graphic) kvp.Value.GetComponent<HierarchyReferences>().GetReference<Image>("Icon")).color = Def.GetUISprite((object) cmp).second;
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").onClick = (System.Action) (() =>
      {
        this.sensor.SetLocationEnabled(kvp.Key, !this.sensor.CheckLocationSelected(kvp.Key));
        this.Refresh();
      });
      kvp.Value.GetComponent<HierarchyReferences>().GetReference<MultiToggle>("Toggle").ChangeState(this.sensor.CheckLocationSelected(kvp.Key) ? 1 : 0);
      kvp.Value.SetActive(ClusterGrid.Instance.GetCellRevealLevel(kvp.Key) == ClusterRevealLevel.Visible);
    }
  }
}
