// Decompiled with JetBrains decompiler
// Type: HabitatModuleSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HabitatModuleSideScreen : SideScreenContent
{
  private Clustercraft targetCraft;
  public GameObject moduleContentContainer;
  public GameObject modulePanelPrefab;

  private CraftModuleInterface craftModuleInterface => ((Component) this.targetCraft).GetComponent<CraftModuleInterface>();

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    this.ConsumeMouseScroll = true;
  }

  public virtual float GetSortKey() => 21f;

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<Clustercraft>(), (Object) null) && Object.op_Inequality((Object) this.GetPassengerModule(target.GetComponent<Clustercraft>()), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.targetCraft = target.GetComponent<Clustercraft>();
    this.RefreshModulePanel(this.GetPassengerModule(this.targetCraft));
  }

  private PassengerRocketModule GetPassengerModule(Clustercraft craft)
  {
    foreach (Ref<RocketModuleCluster> clusterModule in (IEnumerable<Ref<RocketModuleCluster>>) ((Component) craft).GetComponent<CraftModuleInterface>().ClusterModules)
    {
      PassengerRocketModule component = ((Component) clusterModule.Get()).GetComponent<PassengerRocketModule>();
      if (Object.op_Inequality((Object) component, (Object) null))
        return component;
    }
    return (PassengerRocketModule) null;
  }

  private void RefreshModulePanel(PassengerRocketModule module)
  {
    HierarchyReferences component = ((Component) this).GetComponent<HierarchyReferences>();
    component.GetReference<Image>("icon").sprite = Def.GetUISprite((object) ((Component) module).gameObject).first;
    KButton reference = component.GetReference<KButton>("button");
    reference.ClearOnClick();
    reference.onClick += (System.Action) (() =>
    {
      AudioMixer.instance.Start(module.interiorReverbSnapshot);
      ClusterManager.Instance.SetActiveWorld(((Component) module).GetComponent<ClustercraftExteriorDoor>().GetTargetWorld().id);
      ManagementMenu.Instance.CloseAll();
    });
    ((TMP_Text) component.GetReference<LocText>("label")).SetText(((Component) module).gameObject.GetProperName());
  }
}
