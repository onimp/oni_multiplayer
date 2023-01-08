// Decompiled with JetBrains decompiler
// Type: KBatchedAnimTracker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class KBatchedAnimTracker : MonoBehaviour
{
  public KBatchedAnimController controller;
  public Vector3 offset = Vector3.zero;
  public HashedString symbol;
  public Vector3 targetPoint = Vector3.zero;
  public Vector3 previousTargetPoint;
  public bool useTargetPoint;
  public bool fadeOut = true;
  public bool forceAlwaysVisible;
  public bool matchParentOffset;
  private bool alive = true;
  private bool forceUpdate;
  private Matrix2x3 previousMatrix;
  private Vector3 previousPosition;
  [SerializeField]
  private KBatchedAnimController myAnim;

  private void Start()
  {
    if (Object.op_Equality((Object) this.controller, (Object) null))
    {
      for (Transform parent = ((Component) this).transform.parent; Object.op_Inequality((Object) parent, (Object) null); parent = parent.parent)
      {
        this.controller = ((Component) parent).GetComponent<KBatchedAnimController>();
        if (Object.op_Inequality((Object) this.controller, (Object) null))
          break;
      }
    }
    if (Object.op_Equality((Object) this.controller, (Object) null))
    {
      Debug.Log((object) ("Controller Null for tracker on " + ((Object) ((Component) this).gameObject).name), (Object) ((Component) this).gameObject);
      ((Behaviour) this).enabled = false;
    }
    else
    {
      this.controller.onAnimEnter += new KAnimControllerBase.KAnimEvent(this.OnAnimStart);
      this.controller.onAnimComplete += new KAnimControllerBase.KAnimEvent(this.OnAnimStop);
      this.controller.onLayerChanged += new Action<int>(this.OnLayerChanged);
      this.forceUpdate = true;
      if (Object.op_Inequality((Object) this.myAnim, (Object) null))
        return;
      this.myAnim = ((Component) this).GetComponent<KBatchedAnimController>();
    }
  }

  private void OnDestroy()
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null))
    {
      this.controller.onAnimEnter -= new KAnimControllerBase.KAnimEvent(this.OnAnimStart);
      this.controller.onAnimComplete -= new KAnimControllerBase.KAnimEvent(this.OnAnimStop);
      this.controller.onLayerChanged -= new Action<int>(this.OnLayerChanged);
      this.controller = (KBatchedAnimController) null;
    }
    this.myAnim = (KBatchedAnimController) null;
  }

  private void LateUpdate()
  {
    if (Object.op_Inequality((Object) this.controller, (Object) null) && (this.controller.IsVisible() || this.forceAlwaysVisible || this.forceUpdate))
      this.UpdateFrame();
    if (this.alive)
      return;
    ((Behaviour) this).enabled = false;
  }

  public void SetAnimControllers(
    KBatchedAnimController controller,
    KBatchedAnimController parentController)
  {
    this.myAnim = controller;
    this.controller = parentController;
  }

  private void UpdateFrame()
  {
    this.forceUpdate = false;
    bool symbolVisible = false;
    if (this.controller.CurrentAnim != null)
    {
      Matrix2x3 symbolLocalTransform = this.controller.GetSymbolLocalTransform(this.symbol, out symbolVisible);
      Vector3 position1 = TransformExtensions.GetPosition(((Component) this.controller).transform);
      if (symbolVisible && (Matrix2x3.op_Inequality(this.previousMatrix, symbolLocalTransform) || Vector3.op_Inequality(position1, this.previousPosition) || this.useTargetPoint && Vector3.op_Inequality(this.targetPoint, this.previousTargetPoint) || this.matchParentOffset && Vector3.op_Inequality(this.myAnim.Offset, this.controller.Offset)))
      {
        this.previousMatrix = symbolLocalTransform;
        this.previousPosition = position1;
        Matrix2x3 matrix2x3 = Matrix2x3.op_Multiply(this.controller.GetTransformMatrix(), symbolLocalTransform);
        float z = TransformExtensions.GetPosition(((Component) this).transform).z;
        TransformExtensions.SetPosition(((Component) this).transform, ((Matrix2x3) ref matrix2x3).MultiplyPoint(this.offset));
        if (this.useTargetPoint)
        {
          this.previousTargetPoint = this.targetPoint;
          Vector3 position2 = TransformExtensions.GetPosition(((Component) this).transform);
          position2.z = 0.0f;
          Vector3 vector3 = Vector3.op_Subtraction(this.targetPoint, position2);
          float num = Vector3.Angle(vector3, Vector3.right);
          if ((double) vector3.y < 0.0)
            num = 360f - num;
          ((Component) this).transform.localRotation = Quaternion.identity;
          ((Component) this).transform.RotateAround(position2, new Vector3(0.0f, 0.0f, 1f), num);
          float sqrMagnitude = ((Vector3) ref vector3).sqrMagnitude;
          this.myAnim.GetBatchInstanceData().SetClipRadius(TransformExtensions.GetPosition(((Component) this).transform).x, TransformExtensions.GetPosition(((Component) this).transform).y, sqrMagnitude, true);
        }
        else
        {
          Vector3 vector3_1 = this.controller.FlipX ? Vector3.left : Vector3.right;
          Vector3 vector3_2 = this.controller.FlipY ? Vector3.down : Vector3.up;
          ((Component) this).transform.up = ((Matrix2x3) ref matrix2x3).MultiplyVector(vector3_2);
          ((Component) this).transform.right = ((Matrix2x3) ref matrix2x3).MultiplyVector(vector3_1);
          if (Object.op_Inequality((Object) this.myAnim, (Object) null))
            this.myAnim.GetBatchInstanceData()?.SetOverrideTransformMatrix(matrix2x3);
        }
        TransformExtensions.SetPosition(((Component) this).transform, new Vector3(TransformExtensions.GetPosition(((Component) this).transform).x, TransformExtensions.GetPosition(((Component) this).transform).y, z));
        if (this.matchParentOffset)
          this.myAnim.Offset = this.controller.Offset;
        this.myAnim.SetDirty();
      }
    }
    if (!Object.op_Inequality((Object) this.myAnim, (Object) null) || symbolVisible == this.myAnim.enabled)
      return;
    this.myAnim.enabled = symbolVisible;
  }

  [ContextMenu("ForceAlive")]
  private void OnAnimStart(HashedString name)
  {
    this.alive = true;
    ((Behaviour) this).enabled = true;
    this.forceUpdate = true;
  }

  private void OnAnimStop(HashedString name) => this.alive = false;

  private void OnLayerChanged(int layer) => this.myAnim.SetLayer(layer);

  public void SetTarget(Vector3 target)
  {
    this.targetPoint = target;
    this.targetPoint.z = 0.0f;
  }
}
