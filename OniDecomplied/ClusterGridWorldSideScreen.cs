// Decompiled with JetBrains decompiler
// Type: ClusterGridWorldSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ClusterGridWorldSideScreen : SideScreenContent
{
  public Image icon;
  public KButton viewButton;
  private AsteroidGridEntity targetEntity;

  protected virtual void OnSpawn() => this.viewButton.onClick += new System.Action(this.OnClickView);

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<AsteroidGridEntity>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.targetEntity = target.GetComponent<AsteroidGridEntity>();
    this.icon.sprite = Def.GetUISprite((object) this.targetEntity).first;
    WorldContainer component = ((Component) this.targetEntity).GetComponent<WorldContainer>();
    bool flag = Object.op_Inequality((Object) component, (Object) null) && component.IsDiscovered;
    this.viewButton.isInteractable = flag;
    if (!flag)
      ((Component) this.viewButton).GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.UISIDESCREENS.CLUSTERWORLDSIDESCREEN.VIEW_WORLD_DISABLE_TOOLTIP);
    else
      ((Component) this.viewButton).GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.UISIDESCREENS.CLUSTERWORLDSIDESCREEN.VIEW_WORLD_TOOLTIP);
  }

  private void OnClickView()
  {
    WorldContainer component = ((Component) this.targetEntity).GetComponent<WorldContainer>();
    if (!component.IsDupeVisited)
      component.LookAtSurface();
    ClusterManager.Instance.SetActiveWorld(component.id);
    ManagementMenu.Instance.CloseAll();
  }
}
