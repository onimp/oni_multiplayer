// Decompiled with JetBrains decompiler
// Type: ClustercraftInteriorDoor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class ClustercraftInteriorDoor : KMonoBehaviour, ISidescreenButtonControl
{
  public string SidescreenButtonText => (string) UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.LABEL;

  public string SidescreenButtonTooltip => (string) (this.SidescreenButtonInteractable() ? UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.LABEL : UI.UISIDESCREENS.ROCKETMODULESIDESCREEN.BUTTONVIEWEXTERIOR.INVALID);

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Components.ClusterCraftInteriorDoors.Add(this);
  }

  protected virtual void OnCleanUp()
  {
    Components.ClusterCraftInteriorDoors.Remove(this);
    base.OnCleanUp();
  }

  public bool SidescreenEnabled() => true;

  public bool SidescreenButtonInteractable()
  {
    WorldContainer myWorld = ((Component) this).gameObject.GetMyWorld();
    return myWorld.ParentWorldId != (int) ClusterManager.INVALID_WORLD_IDX && myWorld.ParentWorldId != myWorld.id;
  }

  public void OnSidescreenButtonPressed() => ClusterManager.Instance.SetActiveWorld(((Component) this).gameObject.GetMyWorld().ParentWorldId);

  public int ButtonSideScreenSortOrder() => 20;

  public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();
}
