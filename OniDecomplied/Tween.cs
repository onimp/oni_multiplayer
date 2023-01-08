// Decompiled with JetBrains decompiler
// Type: Tween
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tween : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
  private static float Scale = 1.025f;
  private static float ScaleSpeed = 0.5f;
  private Selectable Selectable;
  private float Direction = -1f;

  private void Awake() => this.Selectable = ((Component) this).GetComponent<Selectable>();

  public void OnPointerEnter(PointerEventData data) => this.Direction = 1f;

  public void OnPointerExit(PointerEventData data) => this.Direction = -1f;

  private void Update()
  {
    if (!this.Selectable.interactable)
      return;
    float x = ((Component) this).transform.localScale.x;
    float num = Mathf.Max(Mathf.Min(x + this.Direction * Time.unscaledDeltaTime * Tween.ScaleSpeed, Tween.Scale), 1f);
    if ((double) num == (double) x)
      return;
    ((Component) this).transform.localScale = new Vector3(num, num, 1f);
  }
}
