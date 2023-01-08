// Decompiled with JetBrains decompiler
// Type: CometDetectorSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CometDetectorSideScreen : SideScreenContent
{
  private CometDetector.Instance detector;
  private ClusterCometDetector.Instance clusterDetector;
  public GameObject rowPrefab;
  public RectTransform rowContainer;
  public Dictionary<object, GameObject> rows = new Dictionary<object, GameObject>();

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (!show)
      return;
    this.RefreshOptions();
  }

  private void RefreshOptions()
  {
    if (this.clusterDetector != null)
    {
      int idx1 = 0;
      int num1 = idx1 + 1;
      this.SetClusterRow(idx1, (string) STRINGS.UI.UISIDESCREENS.COMETDETECTORSIDESCREEN.COMETS, Assets.GetSprite(HashedString.op_Implicit("meteors")), ClusterCometDetector.Instance.ClusterCometDetectorState.MeteorShower);
      int idx2 = num1;
      int num2 = idx2 + 1;
      this.SetClusterRow(idx2, (string) STRINGS.UI.UISIDESCREENS.COMETDETECTORSIDESCREEN.DUPEMADE, Assets.GetSprite(HashedString.op_Implicit("dupe_made_ballistics")), ClusterCometDetector.Instance.ClusterCometDetectorState.BallisticObject);
      foreach (Clustercraft clustercraft in Components.Clustercrafts)
        this.SetClusterRow(num2++, clustercraft.Name, Assets.GetSprite(HashedString.op_Implicit("rocket_landing")), ClusterCometDetector.Instance.ClusterCometDetectorState.Rocket, clustercraft);
      for (int index = num2; index < ((Transform) this.rowContainer).childCount; ++index)
        ((Component) ((Transform) this.rowContainer).GetChild(index)).gameObject.SetActive(false);
    }
    else
    {
      int idx = 0;
      int num = idx + 1;
      this.SetRow(idx, (string) STRINGS.UI.UISIDESCREENS.COMETDETECTORSIDESCREEN.COMETS, Assets.GetSprite(HashedString.op_Implicit("meteors")), (LaunchConditionManager) null);
      foreach (Spacecraft spacecraft in SpacecraftManager.instance.GetSpacecraft())
        this.SetRow(num++, spacecraft.GetRocketName(), Assets.GetSprite(HashedString.op_Implicit("rocket_landing")), spacecraft.launchConditions);
      for (int index = num; index < ((Transform) this.rowContainer).childCount; ++index)
        ((Component) ((Transform) this.rowContainer).GetChild(index)).gameObject.SetActive(false);
    }
  }

  private void ClearRows()
  {
    for (int index = ((Transform) this.rowContainer).childCount - 1; index >= 0; --index)
      Util.KDestroyGameObject((Component) ((Transform) this.rowContainer).GetChild(index));
    this.rows.Clear();
  }

  public override void SetTarget(GameObject target)
  {
    if (DlcManager.IsExpansion1Active())
      this.clusterDetector = target.GetSMI<ClusterCometDetector.Instance>();
    else
      this.detector = target.GetSMI<CometDetector.Instance>();
    this.RefreshOptions();
  }

  private void SetClusterRow(
    int idx,
    string name,
    Sprite icon,
    ClusterCometDetector.Instance.ClusterCometDetectorState state,
    Clustercraft rocketTarget = null)
  {
    GameObject gameObject = idx >= ((Transform) this.rowContainer).childCount ? Util.KInstantiateUI(this.rowPrefab, ((Component) this.rowContainer).gameObject, true) : ((Component) ((Transform) this.rowContainer).GetChild(idx)).gameObject;
    HierarchyReferences component1 = gameObject.GetComponent<HierarchyReferences>();
    ((TMP_Text) component1.GetReference<LocText>("label")).text = name;
    component1.GetReference<Image>(nameof (icon)).sprite = icon;
    MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
    component2.ChangeState(this.clusterDetector.GetDetectorState() != state || !Object.op_Equality((Object) this.clusterDetector.GetClustercraftTarget(), (Object) rocketTarget) ? 0 : 1);
    ClusterCometDetector.Instance.ClusterCometDetectorState _state = state;
    Clustercraft _rocketTarget = rocketTarget;
    component2.onClick = (System.Action) (() =>
    {
      this.clusterDetector.SetDetectorState(_state);
      this.clusterDetector.SetClustercraftTarget(_rocketTarget);
      this.RefreshOptions();
    });
  }

  private void SetRow(int idx, string name, Sprite icon, LaunchConditionManager target)
  {
    GameObject gameObject = idx >= ((Transform) this.rowContainer).childCount ? Util.KInstantiateUI(this.rowPrefab, ((Component) this.rowContainer).gameObject, true) : ((Component) ((Transform) this.rowContainer).GetChild(idx)).gameObject;
    HierarchyReferences component1 = gameObject.GetComponent<HierarchyReferences>();
    ((TMP_Text) component1.GetReference<LocText>("label")).text = name;
    component1.GetReference<Image>(nameof (icon)).sprite = icon;
    MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
    component2.ChangeState(Object.op_Equality((Object) this.detector.GetTargetCraft(), (Object) target) ? 1 : 0);
    LaunchConditionManager _target = target;
    component2.onClick = (System.Action) (() =>
    {
      this.detector.SetTargetCraft(_target);
      this.RefreshOptions();
    });
  }

  public override bool IsValidForTarget(GameObject target) => DlcManager.IsExpansion1Active() ? target.GetSMI<ClusterCometDetector.Instance>() != null : target.GetSMI<CometDetector.Instance>() != null;
}
