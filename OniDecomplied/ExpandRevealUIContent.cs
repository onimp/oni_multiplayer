// Decompiled with JetBrains decompiler
// Type: ExpandRevealUIContent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

public class ExpandRevealUIContent : MonoBehaviour
{
  private Coroutine activeRoutine;
  private Action<object> activeRoutineCompleteCallback;
  public AnimationCurve expandAnimation;
  public AnimationCurve collapseAnimation;
  public KRectStretcher MaskRectStretcher;
  public KRectStretcher BGRectStretcher;
  public KChildFitter MaskChildFitter;
  public KChildFitter BGChildFitter;
  public float speedScale = 1f;
  public bool Collapsing;
  public bool Expanding;

  private void OnDisable()
  {
    if (Object.op_Implicit((Object) this.BGChildFitter))
      this.BGChildFitter.WidthScale = this.BGChildFitter.HeightScale = 0.0f;
    if (Object.op_Implicit((Object) this.MaskChildFitter))
    {
      if (this.MaskChildFitter.fitWidth)
        this.MaskChildFitter.WidthScale = 0.0f;
      if (this.MaskChildFitter.fitHeight)
        this.MaskChildFitter.HeightScale = 0.0f;
    }
    if (Object.op_Implicit((Object) this.BGRectStretcher))
    {
      this.BGRectStretcher.XStretchFactor = this.BGRectStretcher.YStretchFactor = 0.0f;
      this.BGRectStretcher.UpdateStretching();
    }
    if (!Object.op_Implicit((Object) this.MaskRectStretcher))
      return;
    this.MaskRectStretcher.XStretchFactor = this.MaskRectStretcher.YStretchFactor = 0.0f;
    this.MaskRectStretcher.UpdateStretching();
  }

  public void Expand(Action<object> completeCallback)
  {
    if (Object.op_Implicit((Object) this.MaskChildFitter) && Object.op_Implicit((Object) this.MaskRectStretcher))
      Debug.LogWarning((object) "ExpandRevealUIContent has references to both a MaskChildFitter and a MaskRectStretcher. It should have only one or the other. ChildFitter to match child size, RectStretcher to match parent size.");
    if (Object.op_Implicit((Object) this.BGChildFitter) && Object.op_Implicit((Object) this.BGRectStretcher))
      Debug.LogWarning((object) "ExpandRevealUIContent has references to both a BGChildFitter and a BGRectStretcher . It should have only one or the other.  ChildFitter to match child size, RectStretcher to match parent size.");
    if (this.activeRoutine != null)
      this.StopCoroutine(this.activeRoutine);
    this.CollapsedImmediate();
    this.activeRoutineCompleteCallback = completeCallback;
    this.activeRoutine = this.StartCoroutine(this.expand((Action<object>) null));
  }

  public void Collapse(Action<object> completeCallback)
  {
    if (this.activeRoutine != null)
    {
      if (this.activeRoutineCompleteCallback != null)
        this.activeRoutineCompleteCallback((object) null);
      this.StopCoroutine(this.activeRoutine);
    }
    this.activeRoutineCompleteCallback = completeCallback;
    if (((Component) this).gameObject.activeInHierarchy)
    {
      this.activeRoutine = this.StartCoroutine(this.collapse(completeCallback));
    }
    else
    {
      this.activeRoutine = (Coroutine) null;
      if (completeCallback == null)
        return;
      completeCallback((object) null);
    }
  }

  private IEnumerator expand(Action<object> completeCallback)
  {
    this.Collapsing = false;
    this.Expanding = true;
    float num = 0.0f;
    foreach (Keyframe key in this.expandAnimation.keys)
    {
      if ((double) ((Keyframe) ref key).time > (double) num)
        num = ((Keyframe) ref key).time;
    }
    float duration = num / this.speedScale;
    for (float remaining = duration; (double) remaining >= 0.0; remaining -= Time.unscaledDeltaTime * this.speedScale)
    {
      this.SetStretch(this.expandAnimation.Evaluate(duration - remaining));
      yield return (object) null;
    }
    this.SetStretch(this.expandAnimation.Evaluate(duration));
    if (completeCallback != null)
      completeCallback((object) null);
    this.activeRoutine = (Coroutine) null;
    this.Expanding = false;
  }

  private void SetStretch(float value)
  {
    if (Object.op_Implicit((Object) this.BGRectStretcher))
    {
      if (this.BGRectStretcher.StretchX)
        this.BGRectStretcher.XStretchFactor = value;
      if (this.BGRectStretcher.StretchY)
        this.BGRectStretcher.YStretchFactor = value;
    }
    if (Object.op_Implicit((Object) this.MaskRectStretcher))
    {
      if (this.MaskRectStretcher.StretchX)
        this.MaskRectStretcher.XStretchFactor = value;
      if (this.MaskRectStretcher.StretchY)
        this.MaskRectStretcher.YStretchFactor = value;
    }
    if (Object.op_Implicit((Object) this.BGChildFitter))
    {
      if (this.BGChildFitter.fitWidth)
        this.BGChildFitter.WidthScale = value;
      if (this.BGChildFitter.fitHeight)
        this.BGChildFitter.HeightScale = value;
    }
    if (!Object.op_Implicit((Object) this.MaskChildFitter))
      return;
    if (this.MaskChildFitter.fitWidth)
      this.MaskChildFitter.WidthScale = value;
    if (!this.MaskChildFitter.fitHeight)
      return;
    this.MaskChildFitter.HeightScale = value;
  }

  private IEnumerator collapse(Action<object> completeCallback)
  {
    ExpandRevealUIContent expandRevealUiContent = this;
    expandRevealUiContent.Expanding = false;
    expandRevealUiContent.Collapsing = true;
    float num = 0.0f;
    foreach (Keyframe key in expandRevealUiContent.collapseAnimation.keys)
    {
      if ((double) ((Keyframe) ref key).time > (double) num)
        num = ((Keyframe) ref key).time;
    }
    float duration = num;
    for (float remaining = duration; (double) remaining >= 0.0; remaining -= Time.unscaledDeltaTime)
    {
      expandRevealUiContent.SetStretch(expandRevealUiContent.collapseAnimation.Evaluate(duration - remaining));
      yield return (object) null;
    }
    expandRevealUiContent.SetStretch(expandRevealUiContent.collapseAnimation.Evaluate(duration));
    if (completeCallback != null)
      completeCallback((object) null);
    expandRevealUiContent.activeRoutine = (Coroutine) null;
    expandRevealUiContent.Collapsing = false;
    ((Component) expandRevealUiContent).gameObject.SetActive(false);
  }

  public void CollapsedImmediate() => this.SetStretch(this.collapseAnimation.Evaluate((float) this.collapseAnimation.length));
}
