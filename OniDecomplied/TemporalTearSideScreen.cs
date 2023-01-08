// Decompiled with JetBrains decompiler
// Type: TemporalTearSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TMPro;
using UnityEngine;

public class TemporalTearSideScreen : SideScreenContent
{
  private Clustercraft targetCraft;

  private CraftModuleInterface craftModuleInterface => ((Component) this.targetCraft).GetComponent<CraftModuleInterface>();

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    this.ConsumeMouseScroll = true;
  }

  public virtual float GetSortKey() => 21f;

  public override bool IsValidForTarget(GameObject target)
  {
    Clustercraft component = target.GetComponent<Clustercraft>();
    TemporalTear temporalTear = ((Component) ClusterManager.Instance).GetComponent<ClusterPOIManager>().GetTemporalTear();
    return Object.op_Inequality((Object) component, (Object) null) && Object.op_Inequality((Object) temporalTear, (Object) null) && AxialI.op_Equality(temporalTear.Location, component.Location);
  }

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.targetCraft = target.GetComponent<Clustercraft>();
    KButton reference = ((Component) this).GetComponent<HierarchyReferences>().GetReference<KButton>("button");
    reference.ClearOnClick();
    reference.onClick += (System.Action) (() =>
    {
      target.GetComponent<Clustercraft>();
      ((Component) ClusterManager.Instance).GetComponent<ClusterPOIManager>().GetTemporalTear().ConsumeCraft(this.targetCraft);
    });
    this.RefreshPanel();
  }

  private void RefreshPanel(object data = null)
  {
    TemporalTear temporalTear = ((Component) ClusterManager.Instance).GetComponent<ClusterPOIManager>().GetTemporalTear();
    HierarchyReferences component = ((Component) this).GetComponent<HierarchyReferences>();
    bool flag = temporalTear.IsOpen();
    ((TMP_Text) component.GetReference<LocText>("label")).SetText((string) (flag ? UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.BUTTON_OPEN : UI.UISIDESCREENS.TEMPORALTEARSIDESCREEN.BUTTON_CLOSED));
    component.GetReference<KButton>("button").isInteractable = flag;
  }
}
