// Decompiled with JetBrains decompiler
// Type: FabricatorListScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class FabricatorListScreen : KToggleMenu
{
  private void Refresh()
  {
    List<KToggleMenu.ToggleInfo> toggleInfo = new List<KToggleMenu.ToggleInfo>();
    foreach (Fabricator user_data in Components.Fabricators.Items)
    {
      KSelectable component = ((Component) user_data).GetComponent<KSelectable>();
      toggleInfo.Add(new KToggleMenu.ToggleInfo(component.GetName(), (object) user_data));
    }
    this.Setup((IList<KToggleMenu.ToggleInfo>) toggleInfo);
  }

  protected virtual void OnSpawn() => this.onSelect += new KToggleMenu.OnSelect(this.OnClickFabricator);

  protected virtual void OnActivate()
  {
    base.OnActivate();
    this.Refresh();
  }

  private void OnClickFabricator(KToggleMenu.ToggleInfo toggle_info)
  {
    Fabricator userData = (Fabricator) toggle_info.userData;
    SelectTool.Instance.Select(((Component) userData).GetComponent<KSelectable>());
  }
}
