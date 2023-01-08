// Decompiled with JetBrains decompiler
// Type: TimeOfDayPositioner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/TimeOfDayPositioner")]
public class TimeOfDayPositioner : KMonoBehaviour
{
  [SerializeField]
  private RectTransform targetRect;

  private void Update()
  {
    double cycleAsPercentage = (double) GameClock.Instance.GetCurrentCycleAsPercentage();
    Rect rect = this.targetRect.rect;
    double width = (double) ((Rect) ref rect).width;
    (this.transform as RectTransform).anchoredPosition = Vector2.op_Addition(this.targetRect.anchoredPosition, new Vector2(Mathf.Round((float) (cycleAsPercentage * width)), 0.0f));
  }
}
