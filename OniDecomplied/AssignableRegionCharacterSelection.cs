// Decompiled with JetBrains decompiler
// Type: AssignableRegionCharacterSelection
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AssignableRegionCharacterSelection")]
public class AssignableRegionCharacterSelection : KMonoBehaviour
{
  [SerializeField]
  private KButton buttonPrefab;
  [SerializeField]
  private GameObject buttonParent;
  private UIPool<KButton> buttonPool;
  private Dictionary<KButton, MinionIdentity> buttonIdentityMap = new Dictionary<KButton, MinionIdentity>();
  private List<CrewPortrait> portraitList = new List<CrewPortrait>();

  public event Action<MinionIdentity> OnDuplicantSelected;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.buttonPool = new UIPool<KButton>(this.buttonPrefab);
    ((Component) this).gameObject.SetActive(false);
  }

  public void Open()
  {
    ((Component) this).gameObject.SetActive(true);
    this.buttonPool.ClearAll();
    foreach (MinionIdentity identity in Components.MinionIdentities.Items)
    {
      KButton btn = this.buttonPool.GetFreeElement(this.buttonParent, true);
      CrewPortrait componentInChildren = ((Component) btn).GetComponentInChildren<CrewPortrait>();
      componentInChildren.SetIdentityObject((IAssignableIdentity) identity);
      this.portraitList.Add(componentInChildren);
      btn.ClearOnClick();
      btn.onClick += (System.Action) (() => this.SelectDuplicant(btn));
      this.buttonIdentityMap.Add(btn, identity);
    }
  }

  public void Close()
  {
    this.buttonPool.DestroyAllActive();
    this.buttonIdentityMap.Clear();
    this.portraitList.Clear();
    ((Component) this).gameObject.SetActive(false);
  }

  private void SelectDuplicant(KButton btn)
  {
    if (this.OnDuplicantSelected != null)
      this.OnDuplicantSelected(this.buttonIdentityMap[btn]);
    this.Close();
  }
}
