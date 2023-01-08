// Decompiled with JetBrains decompiler
// Type: DispenserSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DispenserSideScreen : SideScreenContent
{
  [SerializeField]
  private KButton dispenseButton;
  [SerializeField]
  private RectTransform itemRowContainer;
  [SerializeField]
  private GameObject itemRowPrefab;
  private IDispenser targetDispenser;
  private Dictionary<Tag, GameObject> rows = new Dictionary<Tag, GameObject>();

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<IDispenser>() != null;

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.targetDispenser = target.GetComponent<IDispenser>();
    this.Refresh();
  }

  private void Refresh()
  {
    this.dispenseButton.ClearOnClick();
    foreach (KeyValuePair<Tag, GameObject> row in this.rows)
      Object.Destroy((Object) row.Value);
    this.rows.Clear();
    foreach (Tag dispensedItem in this.targetDispenser.DispensedItems())
    {
      GameObject gameObject = Util.KInstantiateUI(this.itemRowPrefab, ((Component) this.itemRowContainer).gameObject, true);
      this.rows.Add(dispensedItem, gameObject);
      HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
      component.GetReference<Image>("Icon").sprite = Def.GetUISprite((object) dispensedItem).first;
      ((TMP_Text) component.GetReference<LocText>("Label")).text = Assets.GetPrefab(dispensedItem).GetProperName();
      gameObject.GetComponent<MultiToggle>().ChangeState(Tag.op_Equality(dispensedItem, this.targetDispenser.SelectedItem()) ? 0 : 1);
    }
    if (this.targetDispenser.HasOpenChore())
    {
      this.dispenseButton.onClick += (System.Action) (() =>
      {
        this.targetDispenser.OnCancelDispense();
        this.Refresh();
      });
      ((TMP_Text) ((Component) this.dispenseButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.UISIDESCREENS.DISPENSERSIDESCREEN.BUTTON_CANCEL;
    }
    else
    {
      this.dispenseButton.onClick += (System.Action) (() =>
      {
        this.targetDispenser.OnOrderDispense();
        this.Refresh();
      });
      ((TMP_Text) ((Component) this.dispenseButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.UISIDESCREENS.DISPENSERSIDESCREEN.BUTTON_DISPENSE;
    }
    this.targetDispenser.OnStopWorkEvent -= new System.Action(this.Refresh);
    this.targetDispenser.OnStopWorkEvent += new System.Action(this.Refresh);
  }

  private void SelectTag(Tag tag)
  {
    this.targetDispenser.SelectItem(tag);
    this.Refresh();
  }
}
