// Decompiled with JetBrains decompiler
// Type: PortraitTableColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class PortraitTableColumn : TableColumn
{
  public GameObject prefab_portrait = Assets.UIPrefabs.TableScreenWidgets.MinionPortrait;
  private bool double_click_to_target;

  public PortraitTableColumn(
    Action<IAssignableIdentity, GameObject> on_load_action,
    Comparison<IAssignableIdentity> sort_comparison,
    bool double_click_to_target = true)
    : base(on_load_action, sort_comparison)
  {
    this.double_click_to_target = double_click_to_target;
  }

  public override GameObject GetDefaultWidget(GameObject parent)
  {
    GameObject defaultWidget = Util.KInstantiateUI(this.prefab_portrait, parent, true);
    ((Behaviour) defaultWidget.GetComponent<CrewPortrait>().targetImage).enabled = true;
    return defaultWidget;
  }

  public override GameObject GetHeaderWidget(GameObject parent) => Util.KInstantiateUI(this.prefab_portrait, parent, true);

  public override GameObject GetMinionWidget(GameObject parent)
  {
    GameObject minionWidget = Util.KInstantiateUI(this.prefab_portrait, parent, true);
    if (this.double_click_to_target)
    {
      minionWidget.GetComponent<KButton>().onClick += (System.Action) (() => parent.GetComponent<TableRow>().SelectMinion());
      minionWidget.GetComponent<KButton>().onDoubleClick += (System.Action) (() => parent.GetComponent<TableRow>().SelectAndFocusMinion());
    }
    return minionWidget;
  }
}
