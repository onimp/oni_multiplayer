// Decompiled with JetBrains decompiler
// Type: ScheduleBlockPainter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduleBlockPainter")]
public class ScheduleBlockPainter : KMonoBehaviour
{
  [SerializeField]
  private KButtonDrag button;
  private Action<float> blockPaintHandler;
  [MyCmpGet]
  private RectTransform rectTransform;

  public void Setup(Action<float> blockPaintHandler)
  {
    this.blockPaintHandler = blockPaintHandler;
    ((KButton) this.button).onPointerDown += new System.Action(this.OnPointerDown);
    this.button.onDrag += new System.Action(this.OnDrag);
  }

  private void OnPointerDown() => this.Transmit();

  private void OnDrag() => this.Transmit();

  private void Transmit()
  {
    double x1 = (double) this.transform.InverseTransformPoint(KInputManager.GetMousePos()).x;
    Rect rect1 = this.rectTransform.rect;
    double x2 = (double) ((Rect) ref rect1).x;
    double num = x1 - x2;
    Rect rect2 = this.rectTransform.rect;
    double width = (double) ((Rect) ref rect2).width;
    this.blockPaintHandler((float) (num / width));
  }
}
