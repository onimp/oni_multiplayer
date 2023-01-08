// Decompiled with JetBrains decompiler
// Type: KleiItemDropScreen_PermitVis
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections;
using UnityEngine;

public class KleiItemDropScreen_PermitVis : KMonoBehaviour
{
  [SerializeField]
  private RectTransform root;
  [Header("Different Permit Visualizers")]
  [SerializeField]
  private KleiItemDropScreen_PermitVis_Fallback fallbackVis;
  [SerializeField]
  private KleiItemDropScreen_PermitVis_DupeEquipment equipmentVis;

  public void ConfigureWith(PermitResource permit)
  {
    PermitPresentationInfo presentationInfo = PermitItems.GetPermitPresentationInfo(permit.Id);
    int num = permit != null ? 1 : 0;
    this.ResetState();
    ((Component) this.equipmentVis).gameObject.SetActive(false);
    ((Component) this.fallbackVis).gameObject.SetActive(false);
    if (num != 0)
    {
      if (permit.PermitCategory == PermitCategory.Equipment)
      {
        ((Component) this.equipmentVis).gameObject.SetActive(true);
        this.equipmentVis.ConfigureWith(permit, presentationInfo);
      }
      else
      {
        ((Component) this.fallbackVis).gameObject.SetActive(true);
        this.fallbackVis.ConfigureWith(permit, presentationInfo);
      }
    }
    else
    {
      ((Component) this.fallbackVis).gameObject.SetActive(true);
      this.fallbackVis.ConfigureWith(permit, presentationInfo);
    }
  }

  public Promise AnimateIn() => Updater.RunRoutine((MonoBehaviour) this, this.AnimateInRoutine());

  public Promise AnimateOut() => Updater.RunRoutine((MonoBehaviour) this, this.AnimateOutRoutine());

  private IEnumerator AnimateInRoutine()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    KleiItemDropScreen_PermitVis dropScreenPermitVis = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    ((Component) dropScreenPermitVis.root).gameObject.SetActive(true);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u003E2__current = (object) Updater.Ease(new Action<Vector3>(dropScreenPermitVis.\u003CAnimateInRoutine\u003Eb__6_0), ((Component) dropScreenPermitVis.root).transform.localScale, Vector3.one, 0.5f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  private IEnumerator AnimateOutRoutine()
  {
    // ISSUE: reference to a compiler-generated field
    int num = this.\u003C\u003E1__state;
    KleiItemDropScreen_PermitVis dropScreenPermitVis = this;
    if (num != 0)
    {
      if (num != 1)
        return false;
      // ISSUE: reference to a compiler-generated field
      this.\u003C\u003E1__state = -1;
      ((Component) dropScreenPermitVis.root).gameObject.SetActive(true);
      return false;
    }
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = -1;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated method
    this.\u003C\u003E2__current = (object) Updater.Ease(new Action<Vector3>(dropScreenPermitVis.\u003CAnimateOutRoutine\u003Eb__7_0), ((Component) dropScreenPermitVis.root).transform.localScale, Vector3.zero, 0.25f);
    // ISSUE: reference to a compiler-generated field
    this.\u003C\u003E1__state = 1;
    return true;
  }

  public void ResetState() => ((Component) this.root).transform.localScale = Vector3.zero;
}
