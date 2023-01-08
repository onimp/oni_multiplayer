// Decompiled with JetBrains decompiler
// Type: PopIn
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class PopIn : MonoBehaviour
{
  private float targetScale;
  public float speed;

  private void OnEnable() => this.StartPopIn(true);

  private void Update()
  {
    float num = Mathf.Lerp(((Component) this).transform.localScale.x, this.targetScale, Time.unscaledDeltaTime * this.speed);
    ((Component) this).transform.localScale = new Vector3(num, num, 1f);
  }

  public void StartPopIn(bool force_reset = false)
  {
    if (force_reset)
      ((Component) this).transform.localScale = new Vector3(1.5f, 1.5f, 1f);
    this.targetScale = 1f;
  }

  public void StartPopOut() => this.targetScale = 0.0f;
}
