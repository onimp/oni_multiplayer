// Decompiled with JetBrains decompiler
// Type: SizePulse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SizePulse : MonoBehaviour
{
  public System.Action onComplete;
  public Vector2 from = Vector2.one;
  public Vector2 to = Vector2.one;
  public float multiplier = 1.25f;
  public float speed = 1f;
  public bool updateWhenPaused;
  private Vector2 cur;
  private SizePulse.State state;

  private void Start()
  {
    if (((Component) this).GetComponents<SizePulse>().Length > 1)
      Object.Destroy((Object) this);
    this.from = Vector2.op_Implicit(((Component) this).transform.localScale);
    this.cur = this.from;
    this.to = Vector2.op_Multiply(this.from, this.multiplier);
  }

  private void Update()
  {
    float num = (this.updateWhenPaused ? Time.unscaledDeltaTime : Time.deltaTime) * this.speed;
    switch (this.state)
    {
      case SizePulse.State.Up:
        this.cur = Vector2.Lerp(this.cur, this.to, num);
        Vector2 vector2_1 = Vector2.op_Subtraction(this.to, this.cur);
        if ((double) ((Vector2) ref vector2_1).sqrMagnitude < 9.9999997473787516E-05)
        {
          this.cur = this.to;
          this.state = SizePulse.State.Down;
          break;
        }
        break;
      case SizePulse.State.Down:
        this.cur = Vector2.Lerp(this.cur, this.from, num);
        Vector2 vector2_2 = Vector2.op_Subtraction(this.from, this.cur);
        if ((double) ((Vector2) ref vector2_2).sqrMagnitude < 9.9999997473787516E-05)
        {
          this.cur = this.from;
          this.state = SizePulse.State.Finished;
          if (this.onComplete != null)
          {
            this.onComplete();
            break;
          }
          break;
        }
        break;
    }
    ((Component) this).transform.localScale = new Vector3(this.cur.x, this.cur.y, 1f);
  }

  private enum State
  {
    Up,
    Down,
    Finished,
  }
}
