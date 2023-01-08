// Decompiled with JetBrains decompiler
// Type: AnimEventHandler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AnimEventHandler")]
public class AnimEventHandler : KMonoBehaviour
{
  private const int UPDATE_FRAME_RATE = 3;
  [MyCmpGet]
  private KBatchedAnimController controller;
  [MyCmpGet]
  private KBoxCollider2D animCollider;
  [MyCmpGet]
  private Navigator navigator;
  private Vector3 targetPos;
  public Vector2 baseOffset;
  public int isDirty;
  private HashedString context;
  private int instanceIndex;
  private static int InstanceSequence;

  private event AnimEventHandler.SetPos onWorkTargetSet;

  public void SetDirty() => this.isDirty = 2;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    foreach (KBatchedAnimTracker componentsInChild in ((Component) this).GetComponentsInChildren<KBatchedAnimTracker>(true))
    {
      if (componentsInChild.useTargetPoint)
        this.onWorkTargetSet += new AnimEventHandler.SetPos(componentsInChild.SetTarget);
    }
    this.baseOffset = this.animCollider.offset;
    this.instanceIndex = AnimEventHandler.InstanceSequence++;
    this.SetDirty();
  }

  protected virtual void OnForcedCleanUp()
  {
    this.navigator = (Navigator) null;
    base.OnForcedCleanUp();
  }

  public HashedString GetContext() => this.context;

  public void UpdateWorkTarget(Vector3 pos)
  {
    if (this.onWorkTargetSet == null)
      return;
    this.onWorkTargetSet(pos);
  }

  public void SetContext(HashedString context) => this.context = context;

  public void SetTargetPos(Vector3 target_pos) => this.targetPos = target_pos;

  public Vector3 GetTargetPos() => this.targetPos;

  public void ClearContext() => this.context = new HashedString();

  public void LateUpdate()
  {
    if (Time.frameCount % 3 != this.instanceIndex % 3 && this.isDirty <= 0)
      return;
    this.UpdateOffset();
  }

  public void UpdateOffset()
  {
    Vector3 pivotSymbolPosition = this.controller.GetPivotSymbolPosition();
    Vector3 vector3 = Vector2.op_Implicit(this.navigator.NavGrid.GetNavTypeData(this.navigator.CurrentNavType).animControllerOffset);
    this.animCollider.offset = new Vector2(this.baseOffset.x + pivotSymbolPosition.x - TransformExtensions.GetPosition(this.transform).x - vector3.x, this.baseOffset.y + pivotSymbolPosition.y - TransformExtensions.GetPosition(this.transform).y + vector3.y);
    this.isDirty = Mathf.Max(0, this.isDirty - 1);
  }

  private delegate void SetPos(Vector3 pos);
}
