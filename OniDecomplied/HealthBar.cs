// Decompiled with JetBrains decompiler
// Type: HealthBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class HealthBar : ProgressBar
{
  private float showTimer;
  private float maxShowTime = 10f;
  private float alwaysShowThreshold = 0.8f;

  private bool ShouldShow => (double) this.showTimer > 0.0 || (double) this.PercentFull < (double) this.alwaysShowThreshold;

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.barColor = ProgressBarsConfig.Instance.GetBarColor(nameof (HealthBar));
    ((Component) this).gameObject.SetActive(this.ShouldShow);
  }

  public void OnChange()
  {
    ((Behaviour) this).enabled = true;
    this.showTimer = this.maxShowTime;
  }

  public override void Update()
  {
    base.Update();
    if ((double) Time.timeScale > 0.0)
      this.showTimer = Mathf.Max(0.0f, this.showTimer - Time.unscaledDeltaTime);
    if (this.ShouldShow)
      return;
    ((Component) this).gameObject.SetActive(false);
  }

  private void OnBecameInvisible() => ((Behaviour) this).enabled = false;

  private void OnBecameVisible() => ((Behaviour) this).enabled = true;

  public override void OnOverlayChanged(object data = null)
  {
    if (!this.autoHide)
      return;
    if (HashedString.op_Equality((HashedString) data, OverlayModes.None.ID))
    {
      if (((Component) this).gameObject.activeSelf || !this.ShouldShow)
        return;
      ((Behaviour) this).enabled = true;
      ((Component) this).gameObject.SetActive(true);
    }
    else
    {
      if (!((Component) this).gameObject.activeSelf)
        return;
      ((Behaviour) this).enabled = false;
      ((Component) this).gameObject.SetActive(false);
    }
  }
}
