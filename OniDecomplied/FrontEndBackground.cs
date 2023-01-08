// Decompiled with JetBrains decompiler
// Type: FrontEndBackground
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

public class FrontEndBackground : UIDupeRandomizer
{
  private KBatchedAnimController dreckoController;
  private float nextDreckoTime;
  private FrontEndBackground.Tuning tuning;

  protected override void Start()
  {
    this.tuning = TuningData<FrontEndBackground.Tuning>.Get();
    base.Start();
    for (int minion_idx = 0; minion_idx < this.anims.Length; ++minion_idx)
    {
      int minionIndex = minion_idx;
      KBatchedAnimController minion = this.anims[minion_idx].minions[0];
      if (((Component) minion).gameObject.activeInHierarchy)
      {
        minion.onAnimComplete += (KAnimControllerBase.KAnimEvent) (name => this.WaitForABit(minionIndex, name));
        this.WaitForABit(minion_idx, HashedString.Invalid);
      }
    }
    this.dreckoController = ((Component) ((Component) this).transform.GetChild(0).Find("startmenu_drecko")).GetComponent<KBatchedAnimController>();
    if (!((Component) this.dreckoController).gameObject.activeInHierarchy)
      return;
    this.dreckoController.enabled = false;
    this.nextDreckoTime = Random.Range(this.tuning.minFirstDreckoInterval, this.tuning.maxFirstDreckoInterval) + Time.unscaledTime;
  }

  protected override void Update()
  {
    base.Update();
    this.UpdateDrecko();
  }

  private void UpdateDrecko()
  {
    if (!((Component) this.dreckoController).gameObject.activeInHierarchy || (double) Time.unscaledTime <= (double) this.nextDreckoTime)
      return;
    this.dreckoController.enabled = true;
    this.dreckoController.Play(HashedString.op_Implicit("idle"));
    this.nextDreckoTime = Random.Range(this.tuning.minDreckoInterval, this.tuning.maxDreckoInterval) + Time.unscaledTime;
  }

  private void WaitForABit(int minion_idx, HashedString name) => this.StartCoroutine(this.WaitForTime(minion_idx));

  private IEnumerator WaitForTime(int minion_idx)
  {
    FrontEndBackground frontEndBackground = this;
    frontEndBackground.anims[minion_idx].lastWaitTime = Random.Range(frontEndBackground.anims[minion_idx].minSecondsBetweenAction, frontEndBackground.anims[minion_idx].maxSecondsBetweenAction);
    yield return (object) new WaitForSecondsRealtime(frontEndBackground.anims[minion_idx].lastWaitTime);
    frontEndBackground.GetNewBody(minion_idx);
    foreach (KBatchedAnimController minion in frontEndBackground.anims[minion_idx].minions)
    {
      minion.ClearQueue();
      minion.Play(HashedString.op_Implicit(frontEndBackground.anims[minion_idx].anim_name));
    }
  }

  public class Tuning : TuningData<FrontEndBackground.Tuning>
  {
    public float minDreckoInterval;
    public float maxDreckoInterval;
    public float minFirstDreckoInterval;
    public float maxFirstDreckoInterval;
  }
}
