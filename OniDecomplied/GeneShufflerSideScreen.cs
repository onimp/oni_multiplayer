// Decompiled with JetBrains decompiler
// Type: GeneShufflerSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TMPro;
using UnityEngine;

public class GeneShufflerSideScreen : SideScreenContent
{
  [SerializeField]
  private LocText label;
  [SerializeField]
  private KButton button;
  [SerializeField]
  private LocText buttonLabel;
  [SerializeField]
  private GeneShuffler target;
  [SerializeField]
  private GameObject contents;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.button.onClick += new System.Action(this.OnButtonClick);
    this.Refresh();
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<GeneShuffler>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    GeneShuffler component = target.GetComponent<GeneShuffler>();
    if (Object.op_Equality((Object) component, (Object) null))
    {
      Debug.LogError((object) "Target doesn't have a GeneShuffler associated with it.");
    }
    else
    {
      this.target = component;
      this.Refresh();
    }
  }

  private void OnButtonClick()
  {
    if (this.target.WorkComplete)
    {
      this.target.SetWorkTime(0.0f);
    }
    else
    {
      if (!this.target.IsConsumed)
        return;
      this.target.RequestRecharge(!this.target.RechargeRequested);
      this.Refresh();
    }
  }

  private void Refresh()
  {
    if (Object.op_Inequality((Object) this.target, (Object) null))
    {
      if (this.target.WorkComplete)
      {
        this.contents.SetActive(true);
        ((TMP_Text) this.label).text = (string) UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.COMPLETE;
        ((Component) this.button).gameObject.SetActive(true);
        ((TMP_Text) this.buttonLabel).text = (string) UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.BUTTON;
      }
      else if (this.target.IsConsumed)
      {
        this.contents.SetActive(true);
        ((Component) this.button).gameObject.SetActive(true);
        if (this.target.RechargeRequested)
        {
          ((TMP_Text) this.label).text = (string) UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.CONSUMED_WAITING;
          ((TMP_Text) this.buttonLabel).text = (string) UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.BUTTON_RECHARGE_CANCEL;
        }
        else
        {
          ((TMP_Text) this.label).text = (string) UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.CONSUMED;
          ((TMP_Text) this.buttonLabel).text = (string) UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.BUTTON_RECHARGE;
        }
      }
      else if (this.target.IsWorking)
      {
        this.contents.SetActive(true);
        ((TMP_Text) this.label).text = (string) UI.UISIDESCREENS.GENESHUFFLERSIDESREEN.UNDERWAY;
        ((Component) this.button).gameObject.SetActive(false);
      }
      else
        this.contents.SetActive(false);
    }
    else
      this.contents.SetActive(false);
  }
}
