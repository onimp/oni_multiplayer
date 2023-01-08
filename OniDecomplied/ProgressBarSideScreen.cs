// Decompiled with JetBrains decompiler
// Type: ProgressBarSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

public class ProgressBarSideScreen : SideScreenContent, IRender1000ms
{
  public LocText label;
  public GenericUIProgressBar progressBar;
  public IProgressBarSideScreen targetObject;

  protected virtual void OnSpawn() => base.OnSpawn();

  public override int GetSideScreenSortOrder() => -10;

  public override bool IsValidForTarget(GameObject target) => target.GetComponent<IProgressBarSideScreen>() != null;

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.targetObject = target.GetComponent<IProgressBarSideScreen>();
    this.RefreshBar();
  }

  private void RefreshBar()
  {
    this.progressBar.SetMaxValue(this.targetObject.GetProgressBarMaxValue());
    this.progressBar.SetFillPercentage(this.targetObject.GetProgressBarFillPercentage());
    ((TMP_Text) this.progressBar.label).SetText(this.targetObject.GetProgressBarLabel());
    ((TMP_Text) this.label).SetText(this.targetObject.GetProgressBarTitleLabel());
    ((Component) this.progressBar).GetComponentInChildren<ToolTip>().SetSimpleTooltip(this.targetObject.GetProgressBarTooltip());
  }

  public void Render1000ms(float dt) => this.RefreshBar();
}
