// Decompiled with JetBrains decompiler
// Type: UICurvePath
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/UICurvePath")]
public class UICurvePath : KMonoBehaviour
{
  public Transform startPoint;
  public Transform endPoint;
  public Transform controlPointStart;
  public Transform controlPointEnd;
  public Image sprite;
  public bool loop = true;
  public bool animateScale;
  public Vector3 initialScale;
  private float startDelay;
  public float initialAlpha = 0.5f;
  public float moveSpeed = 0.1f;
  private float tick;
  private Vector3 A;
  private Vector3 B;
  private Vector3 C;
  private Vector3 D;

  protected virtual void OnSpawn()
  {
    this.Init();
    ScreenResize.Instance.OnResize += new System.Action(this.OnResize);
    this.OnResize();
    this.startDelay = (float) Random.Range(0, 8);
  }

  private void OnResize()
  {
    this.A = this.startPoint.position;
    this.B = this.controlPointStart.position;
    this.C = this.controlPointEnd.position;
    this.D = this.endPoint.position;
  }

  protected virtual void OnCleanUp()
  {
    ScreenResize.Instance.OnResize -= new System.Action(this.OnResize);
    base.OnCleanUp();
  }

  private void Update()
  {
    this.startDelay -= Time.unscaledDeltaTime;
    ((Component) this.sprite).gameObject.SetActive((double) this.startDelay < 0.0);
    if ((double) this.startDelay > 0.0)
      return;
    this.tick += Time.unscaledDeltaTime * this.moveSpeed;
    ((Component) this.sprite).transform.position = this.DeCasteljausAlgorithm(this.tick);
    KMonoBehaviourExtensions.SetAlpha(this.sprite, Mathf.Min(((Graphic) this.sprite).color.a + this.tick / 2f, 1f));
    if (this.animateScale)
    {
      float num = Mathf.Min(((Component) this.sprite).transform.localScale.x + Time.unscaledDeltaTime * this.moveSpeed, 1f);
      ((Component) this.sprite).transform.localScale = new Vector3(num, num, 1f);
    }
    if (!this.loop || (double) this.tick <= 1.0)
      return;
    this.Init();
  }

  private void Init()
  {
    ((Component) this.sprite).transform.position = this.startPoint.position;
    this.tick = 0.0f;
    if (this.animateScale)
      ((Component) this.sprite).transform.localScale = this.initialScale;
    KMonoBehaviourExtensions.SetAlpha(this.sprite, this.initialAlpha);
  }

  private void OnDrawGizmos()
  {
    if (!Application.isPlaying)
    {
      this.A = this.startPoint.position;
      this.B = this.controlPointStart.position;
      this.C = this.controlPointEnd.position;
      this.D = this.endPoint.position;
    }
    Gizmos.color = Color.white;
    Vector3 a = this.A;
    float num1 = 0.02f;
    int num2 = Mathf.FloorToInt(1f / num1);
    for (int index = 1; index <= num2; ++index)
      this.DeCasteljausAlgorithm((float) index * num1);
    Gizmos.color = Color.green;
  }

  private Vector3 DeCasteljausAlgorithm(float t)
  {
    double num = 1.0 - (double) t;
    Vector3 vector3_1 = Vector3.op_Addition(Vector3.op_Multiply((float) num, this.A), Vector3.op_Multiply(t, this.B));
    Vector3 vector3_2 = Vector3.op_Addition(Vector3.op_Multiply((float) num, this.B), Vector3.op_Multiply(t, this.C));
    Vector3 vector3_3 = Vector3.op_Addition(Vector3.op_Multiply((float) num, this.C), Vector3.op_Multiply(t, this.D));
    Vector3 vector3_4 = Vector3.op_Addition(Vector3.op_Multiply((float) num, vector3_1), Vector3.op_Multiply(t, vector3_2));
    Vector3 vector3_5 = Vector3.op_Addition(Vector3.op_Multiply((float) num, vector3_2), Vector3.op_Multiply(t, vector3_3));
    return Vector3.op_Addition(Vector3.op_Multiply((float) num, vector3_4), Vector3.op_Multiply(t, vector3_5));
  }
}
