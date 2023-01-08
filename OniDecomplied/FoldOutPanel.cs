// Decompiled with JetBrains decompiler
// Type: FoldOutPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FoldOutPanel : KMonoBehaviour
{
  private bool panelOpen = true;
  public GameObject container;
  public bool startOpen = true;

  protected virtual void OnSpawn()
  {
    ((Component) this).GetComponentInChildren<MultiToggle>().onClick += new System.Action(this.OnClick);
    this.ToggleOpen(this.startOpen);
  }

  private void OnClick() => this.ToggleOpen(!this.panelOpen);

  private void ToggleOpen(bool open)
  {
    this.panelOpen = open;
    this.container.SetActive(this.panelOpen);
    ((Component) this).GetComponentInChildren<MultiToggle>().ChangeState(this.panelOpen ? 1 : 0);
  }
}
