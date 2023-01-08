// Decompiled with JetBrains decompiler
// Type: DividerColumn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class DividerColumn : TableColumn
{
  public DividerColumn(Func<bool> revealed = null, string scrollerID = "")
    : base((Action<IAssignableIdentity, GameObject>) ((minion, widget_go) =>
    {
      if (revealed != null)
      {
        if (revealed())
        {
          if (widget_go.activeSelf)
            return;
          widget_go.SetActive(true);
        }
        else
        {
          if (!widget_go.activeSelf)
            return;
          widget_go.SetActive(false);
        }
      }
      else
        widget_go.SetActive(true);
    }), (Comparison<IAssignableIdentity>) null, revealed: revealed, scrollerID: scrollerID)
  {
  }

  public override GameObject GetDefaultWidget(GameObject parent) => Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Spacer, parent, true);

  public override GameObject GetMinionWidget(GameObject parent) => Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Spacer, parent, true);

  public override GameObject GetHeaderWidget(GameObject parent) => Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Spacer, parent, true);
}
